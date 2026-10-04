namespace LivingSim.Core;

public sealed class WorldSimulation
{
    private const int HerbivoreVision = 4;
    private const int PredatorVision = 6;
    private static readonly SimulationSystem[] OrderedSystems =
    [
        SimulationSystem.Climate,
        SimulationSystem.ResourceRegeneration,
        SimulationSystem.Metabolism,
        SimulationSystem.SpatialIndexUpdate,
        SimulationSystem.Perception,
        SimulationSystem.Movement,
        SimulationSystem.Combat,
        SimulationSystem.Feeding,
        SimulationSystem.LifecycleAndDeath,
        SimulationSystem.CarcassProcessing,
        SimulationSystem.Metrics,
    ];
    private static readonly IReadOnlyList<SimulationSystem> DefinedSystemOrder = Array.AsReadOnly(OrderedSystems);

    public WorldSimulation(World world)
    {
        World = world;
        SpatialIndex = new SpatialIndex(world);
    }

    public World World { get; }
    public EntityStore Entities { get; } = new();
    public CarcassStore Carcasses { get; } = new();
    public SpatialIndex SpatialIndex { get; }
    public long Tick { get; private set; }
    public SimulationMetrics Metrics { get; private set; }
    public static IReadOnlyList<SimulationSystem> SystemOrder => DefinedSystemOrder;

    public int SpawnAnimal(AnimalSpecies species, int x, int y)
    {
        if ((uint)x >= (uint)World.Width || (uint)y >= (uint)World.Height)
        {
            throw new ArgumentOutOfRangeException($"({x}, {y}) is outside the world.");
        }

        return Entities.Spawn(species, x, y);
    }

    public void Advance(int ticks = 1)
    {
        if (ticks < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ticks));
        }

        for (var tick = 0; tick < ticks; tick++)
        {
            foreach (var system in OrderedSystems)
            {
                Execute(system);
            }

            Tick++;
        }
    }

    private void Execute(SimulationSystem system)
    {
        switch (system)
        {
            case SimulationSystem.Climate: return; // Slice 7 owns climate state.
            case SimulationSystem.ResourceRegeneration: RegenerateBiomass(); return;
            case SimulationSystem.Metabolism: ApplyMetabolism(); return;
            case SimulationSystem.SpatialIndexUpdate: SpatialIndex.Rebuild(Entities); return;
            case SimulationSystem.Perception: AcquireTargets(); return;
            case SimulationSystem.Movement: MoveAnimals(); return;
            case SimulationSystem.Combat: ResolveCombat(); return;
            case SimulationSystem.Feeding: ResolveFeeding(); return;
            case SimulationSystem.LifecycleAndDeath: ResolveLifecycleAndDeaths(); return;
            case SimulationSystem.CarcassProcessing: ProcessCarcasses(); return;
            case SimulationSystem.Metrics: CollectMetrics(); return;
            default: throw new InvalidOperationException($"Unknown simulation system {system}.");
        }
    }

    private void RegenerateBiomass()
    {
        var cells = World.MutableCells;
        for (var index = 0; index < cells.Length; index++)
        {
            ref var cell = ref cells[index];
            cell.PlantBiomass = Math.Min(cell.MaxPlantBiomass, cell.PlantBiomass + cell.BiomassRegenerationPerTick);
        }
    }

    private void ApplyMetabolism()
    {
        for (var id = 1; id <= Entities.Count; id++)
        {
            ref var animal = ref Entities.GetById(id);
            if (!animal.IsAlive)
            {
                continue;
            }

            animal.AgeTicks++;
            animal.Energy -= animal.Species == AnimalSpecies.Herbivore ? 1 : 2;
            if (animal.Energy <= 0)
            {
                animal.Health = 0;
            }
        }
    }

    private void AcquireTargets()
    {
        for (var id = 1; id <= Entities.Count; id++)
        {
            ref var animal = ref Entities.GetById(id);
            if (!animal.IsAlive)
            {
                continue;
            }

            if (animal.Species == AnimalSpecies.Herbivore)
            {
                FindBestPlantCell(ref animal);
            }
            else
            {
                animal.TargetEntityId = SpatialIndex.FindNearestHerbivore(Entities, animal.X, animal.Y, PredatorVision);
                if (animal.TargetEntityId != 0)
                {
                    var prey = Entities.GetById(animal.TargetEntityId);
                    animal.TargetX = prey.X;
                    animal.TargetY = prey.Y;
                }
                else
                {
                    animal.TargetX = animal.X;
                    animal.TargetY = animal.Y;
                }
            }
        }
    }

    private void FindBestPlantCell(ref AnimalState animal)
    {
        var bestBiomass = -1;
        var bestX = animal.X;
        var bestY = animal.Y;
        for (var y = Math.Max(0, animal.Y - HerbivoreVision); y <= Math.Min(World.Height - 1, animal.Y + HerbivoreVision); y++)
        {
            for (var x = Math.Max(0, animal.X - HerbivoreVision); x <= Math.Min(World.Width - 1, animal.X + HerbivoreVision); x++)
            {
                var cell = World.CellAt(x, y);
                var candidateDistance = Math.Abs(x - animal.X) + Math.Abs(y - animal.Y);
                var bestDistance = Math.Abs(bestX - animal.X) + Math.Abs(bestY - animal.Y);
                if (cell.PlantBiomass > bestBiomass || (cell.PlantBiomass == bestBiomass && candidateDistance < bestDistance))
                {
                    bestBiomass = cell.PlantBiomass;
                    bestX = x;
                    bestY = y;
                }
            }
        }

        animal.TargetEntityId = 0;
        animal.TargetX = bestX;
        animal.TargetY = bestY;
    }

    private void MoveAnimals()
    {
        for (var id = 1; id <= Entities.Count; id++)
        {
            ref var animal = ref Entities.GetById(id);
            if (!animal.IsAlive)
            {
                continue;
            }

            animal.X = Math.Clamp(animal.X + Math.Sign(animal.TargetX - animal.X), 0, World.Width - 1);
            animal.Y = Math.Clamp(animal.Y + Math.Sign(animal.TargetY - animal.Y), 0, World.Height - 1);
        }
    }

    private void ResolveCombat()
    {
        for (var id = 1; id <= Entities.Count; id++)
        {
            ref var predator = ref Entities.GetById(id);
            if (!predator.IsAlive || predator.Species != AnimalSpecies.Predator || predator.TargetEntityId == 0)
            {
                continue;
            }

            ref var prey = ref Entities.GetById(predator.TargetEntityId);
            if (prey.IsAlive && prey.Species == AnimalSpecies.Herbivore && Math.Abs(predator.X - prey.X) + Math.Abs(predator.Y - prey.Y) <= 1)
            {
                prey.Health -= 20;
            }
        }
    }

    private void ResolveFeeding()
    {
        for (var id = 1; id <= Entities.Count; id++)
        {
            ref var animal = ref Entities.GetById(id);
            if (!animal.IsAlive)
            {
                continue;
            }

            if (animal.Species == AnimalSpecies.Herbivore)
            {
                ref var cell = ref World.CellAt(animal.X, animal.Y);
                var eaten = Math.Min(20, cell.PlantBiomass);
                cell.PlantBiomass -= eaten;
                animal.Energy += eaten;
                continue;
            }

            for (var carcassIndex = 0; carcassIndex < Carcasses.Count; carcassIndex++)
            {
                ref var carcass = ref Carcasses.Get(carcassIndex);
                if (Math.Abs(carcass.X - animal.X) + Math.Abs(carcass.Y - animal.Y) > 1)
                {
                    continue;
                }

                var eaten = Math.Min(25, carcass.Nutrition);
                carcass.Nutrition -= eaten;
                animal.Energy += eaten;
                break;
            }
        }
    }

    private void ResolveLifecycleAndDeaths()
    {
        for (var id = 1; id <= Entities.Count; id++)
        {
            ref var animal = ref Entities.GetById(id);
            if (animal.IsAlive && (animal.Health <= 0 || animal.AgeTicks >= MaximumAge(animal.Species)))
            {
                animal.IsAlive = false;
            }

            if (!animal.IsAlive && !animal.CarcassCreated)
            {
                Carcasses.Add(animal.Species, animal.X, animal.Y);
                animal.CarcassCreated = true;
            }
        }
    }

    private void ProcessCarcasses()
    {
        for (var index = Carcasses.Count - 1; index >= 0; index--)
        {
            ref var carcass = ref Carcasses.Get(index);
            carcass.Nutrition--;
            if (carcass.Nutrition <= 0)
            {
                Carcasses.RemoveAt(index);
            }
        }
    }

    private void CollectMetrics()
    {
        var herbivores = 0;
        var predators = 0;
        foreach (var animal in Entities.Items)
        {
            if (!animal.IsAlive)
            {
                continue;
            }

            if (animal.Species == AnimalSpecies.Herbivore) herbivores++;
            else predators++;
        }

        Metrics = new SimulationMetrics(herbivores, predators, Carcasses.Count);
    }

    private static int MaximumAge(AnimalSpecies species) => species == AnimalSpecies.Herbivore ? 12_000 : 16_000;
}

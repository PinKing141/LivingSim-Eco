namespace LivingSim.Core;

public sealed class WorldSimulation
{
    private const int MaturityAgeTicks = 200;
    private const int ReproductionEnergyCost = 30;
    private static readonly SimulationSystem[] OrderedSystems =
    [
        SimulationSystem.Climate,
        SimulationSystem.ResourceRegeneration,
        SimulationSystem.Metabolism,
        SimulationSystem.SpatialIndexUpdate,
        SimulationSystem.Perception,
        SimulationSystem.SocialAndMigration,
        SimulationSystem.Movement,
        SimulationSystem.Combat,
        SimulationSystem.Feeding,
        SimulationSystem.Reproduction,
        SimulationSystem.LifecycleAndDeath,
        SimulationSystem.CarcassProcessing,
        SimulationSystem.Metrics,
    ];
    private static readonly IReadOnlyList<SimulationSystem> DefinedSystemOrder = Array.AsReadOnly(OrderedSystems);
    private readonly Dictionary<AnimalSpecies, int> _populationBySpecies = [];
    private readonly Dictionary<int, PopulationGroup> _groups = [];
    private readonly Dictionary<int, GroupAccumulator> _groupAccumulators = [];

    public WorldSimulation(World world, ClimateMode climateMode = ClimateMode.Dynamic)
    {
        World = world;
        SpatialIndex = new SpatialIndex(world);
        ClimateMode = climateMode;
        Climate = ClimateModel.At(0, climateMode);
    }

    public World World { get; }
    public EntityStore Entities { get; } = new();
    public CarcassStore Carcasses { get; } = new();
    public SpatialIndex SpatialIndex { get; }
    public long Tick { get; private set; }
    public ClimateMode ClimateMode { get; }
    public ClimateState Climate { get; private set; }
    public ClimateHistory ClimateHistory { get; } = new();
    public LineageHistory Lineages { get; } = new();
    public NaturalHistory NaturalHistory { get; } = new();
    public HuntingTelemetry? HuntingTelemetry { get; set; }
    public ResourceTelemetry? ResourceTelemetry { get; set; }
    public SimulationMetrics Metrics { get; private set; }
    public IReadOnlyDictionary<AnimalSpecies, int> PopulationBySpecies => _populationBySpecies;
    public IReadOnlyDictionary<int, PopulationGroup> Groups => _groups;
    public static IReadOnlyList<SimulationSystem> SystemOrder => DefinedSystemOrder;

    internal void Restore(long tick, ClimateState climate, IEnumerable<AnimalState> animals, IEnumerable<Carcass> carcasses, IEnumerable<ClimateRecord> climateRecords, IEnumerable<LineageRecord> lineageRecords, NaturalHistorySnapshot naturalHistory)
    {
        Entities.Restore(animals);
        Carcasses.Restore(carcasses);
        ClimateHistory.Restore(climateRecords);
        Lineages.Restore(lineageRecords);
        NaturalHistory.Restore(naturalHistory);
        Tick = tick;
        Climate = climate;
        SpatialIndex.Rebuild(Entities);
        UpdateGroups(updateTargets: false);
        BuildMetrics();
    }

    public int SpawnAnimal(AnimalSpecies species, int x, int y)
    {
        if ((uint)x >= (uint)World.Width || (uint)y >= (uint)World.Height)
        {
            throw new ArgumentOutOfRangeException($"({x}, {y}) is outside the world.");
        }

        var id = Entities.Spawn(species, x, y);
        ref var animal = ref Entities.GetById(id);
        var profile = SpeciesProfiles.For(species);
        animal.Sex = (DeterministicHash.At(World.Settings.Seed, id, 0, 30) & 1) == 0 ? AnimalSex.Female : AnimalSex.Male;
        animal.Traits = CreateInitialTraits(id);
        animal.Energy = profile.StartingEnergy;
        animal.Health = profile.StartingHealth;
        animal.GroupId = CreateGroupId(species, x, y);
        Lineages.RegisterBirth(animal, Tick);
        return id;
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
            case SimulationSystem.Climate: UpdateClimate(); return;
            case SimulationSystem.ResourceRegeneration: RegenerateBiomass(); return;
            case SimulationSystem.Metabolism: ApplyMetabolism(); return;
            case SimulationSystem.SpatialIndexUpdate: SpatialIndex.Rebuild(Entities); return;
            case SimulationSystem.Perception: AcquireTargets(); return;
            case SimulationSystem.SocialAndMigration: ApplySocialAndMigration(); return;
            case SimulationSystem.Movement: MoveAnimals(); return;
            case SimulationSystem.Combat: ResolveCombat(); return;
            case SimulationSystem.Feeding: ResolveFeeding(); return;
            case SimulationSystem.Reproduction: ResolveReproduction(); return;
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
            if (cell.MaxPlantBiomass == 0)
            {
                continue;
            }

            // Habitat can recover only after animals ease local grazing pressure.
            cell.HabitatPressure = Climate.IsDrought && cell.PlantBiomass < cell.MaxPlantBiomass / 4
                ? Math.Min(1_000, cell.HabitatPressure + 1)
                : Math.Max(0, cell.HabitatPressure - 1);
            var effectiveRegeneration = cell.BiomassRegenerationPerTick * (1_000 - cell.HabitatPressure) / 1_000;
            effectiveRegeneration = effectiveRegeneration * Climate.BiomassModifierMilli / 1_000;
            var previousBiomass = cell.PlantBiomass;
            cell.PlantBiomass = Math.Min(cell.MaxPlantBiomass, cell.PlantBiomass + effectiveRegeneration);
            ResourceTelemetry?.RecordRegeneration(cell.PlantBiomass - previousBiomass);
        }
    }

    private void UpdateClimate()
    {
        Climate = ClimateModel.At(Tick, ClimateMode);
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
            var metabolicCost = MetabolicCost(animal);
            animal.Energy -= metabolicCost;
            if (SpeciesProfiles.For(animal.Species).HuntsPrey) HuntingTelemetry?.RecordMetabolism(animal.Id, metabolicCost, animal.TargetEntityId != 0);
            if (animal.ReproductionCooldown > 0)
            {
                animal.ReproductionCooldown--;
            }
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

            var profile = SpeciesProfiles.For(animal.Species);
            if (profile.HuntsPrey)
            {
                var previousTargetId = animal.TargetEntityId;
                var previousTargetSurvived = previousTargetId != 0 && Entities.GetById(previousTargetId).IsAlive;
                if (animal.Energy >= SatiatedEnergy(profile))
                {
                    SetWanderTarget(ref animal);
                    HuntingTelemetry?.RecordTarget(animal.Id, previousTargetId, 0, previousTargetSurvived, Tick);
                    continue;
                }
                if (animal.Sex == AnimalSex.Female && animal.AgeTicks >= MaturityAgeTicks &&
                    animal.ReproductionCooldown == 0 && animal.Energy >= profile.StartingEnergy)
                {
                    var mateId = SpatialIndex.FindNearestMate(Entities, animal.X, animal.Y,
                        animal.Traits.Vision * 8, animal.Species, AnimalSex.Male);
                    if (mateId != 0)
                    {
                        var mate = Entities.GetById(mateId);
                        var mateDistance = Math.Abs(mate.X - animal.X) + Math.Abs(mate.Y - animal.Y);
                        if (mateDistance > animal.Traits.Vision * 3)
                        {
                            animal.TargetEntityId = 0;
                            animal.TargetX = mate.X;
                            animal.TargetY = mate.Y;
                            HuntingTelemetry?.RecordTarget(animal.Id, previousTargetId, 0, previousTargetSurvived, Tick);
                            continue;
                        }
                    }
                }
                var carcassSearchRadius = previousTargetId == 0 ? animal.Traits.Vision * 2 : 2;
                if (profile.Diet.HasFlag(Diet.Carcasses) && TryFindNearestCarcass(animal, carcassSearchRadius, out var foodX, out var foodY))
                {
                    animal.TargetEntityId = 0;
                    animal.TargetX = foodX;
                    animal.TargetY = foodY;
                    HuntingTelemetry?.RecordTarget(animal.Id, previousTargetId, 0, previousTargetSurvived, Tick);
                    continue;
                }

                var previousPrey = previousTargetSurvived ? Entities.GetById(previousTargetId) : default;
                var keepCurrentPrey = previousTargetSurvived && previousTargetId != animal.Id &&
                    SpeciesProfiles.IsPlantEater(previousPrey.Species) &&
                    previousPrey.Traits.Size <= animal.Traits.Size &&
                    Math.Abs(previousPrey.X - animal.X) + Math.Abs(previousPrey.Y - animal.Y) <= animal.Traits.Vision * 3;
                animal.TargetEntityId = keepCurrentPrey
                    ? previousTargetId
                    : SpatialIndex.FindNearestPlantEater(Entities, animal.X, animal.Y,
                        animal.Traits.Vision * (Metrics.Predators <= 4 ? 4 : 2), animal.Traits.Size, animal.Id);
                if (animal.TargetEntityId != 0)
                {
                    var prey = Entities.GetById(animal.TargetEntityId);
                    animal.TargetX = prey.X;
                    animal.TargetY = prey.Y;
                }
                else if (profile.Diet.HasFlag(Diet.Carcasses) && TryFindNearestCarcass(animal, animal.Traits.Vision * 2, out var carcassX, out var carcassY))
                {
                    animal.TargetX = carcassX;
                    animal.TargetY = carcassY;
                }
                else if (profile.Diet.HasFlag(Diet.Plants))
                {
                    FindBestPlantCell(ref animal);
                }
                else
                {
                    SetWanderTarget(ref animal);
                }
                HuntingTelemetry?.RecordTarget(animal.Id, previousTargetId, animal.TargetEntityId, previousTargetSurvived, Tick);
            }
            else if (profile.Diet.HasFlag(Diet.Carcasses) && TryFindNearestCarcass(animal, animal.Traits.Vision * 2, out var carcassX, out var carcassY))
            {
                animal.TargetEntityId = 0;
                animal.TargetX = carcassX;
                animal.TargetY = carcassY;
            }
            else if (profile.Diet.HasFlag(Diet.Plants))
            {
                FindBestPlantCell(ref animal);
            }
            else
            {
                SetWanderTarget(ref animal);
            }
        }
    }

    private void FindBestPlantCell(ref AnimalState animal)
    {
        var bestBiomass = -1;
        var bestX = animal.X;
        var bestY = animal.Y;
        for (var y = Math.Max(0, animal.Y - animal.Traits.Vision); y <= Math.Min(World.Height - 1, animal.Y + animal.Traits.Vision); y++)
        {
            for (var x = Math.Max(0, animal.X - animal.Traits.Vision); x <= Math.Min(World.Width - 1, animal.X + animal.Traits.Vision); x++)
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

    private void ApplySocialAndMigration()
        => UpdateGroups(updateTargets: true);

    private void UpdateGroups(bool updateTargets)
    {
        var accumulators = _groupAccumulators;
        accumulators.Clear();
        foreach (var animal in Entities.Items)
        {
            if (!animal.IsAlive)
            {
                continue;
            }

            accumulators.TryGetValue(animal.GroupId, out var accumulator);
            accumulator.Add(animal);
            accumulators[animal.GroupId] = accumulator;
        }

        _groups.Clear();
        foreach (var (groupId, accumulator) in accumulators)
        {
            var centerX = accumulator.SumX / accumulator.Members;
            var centerY = accumulator.SumY / accumulator.Members;
            var habitat = World.CellAt(centerX, centerY);
            var plantEater = SpeciesProfiles.For(accumulator.Species).Diet.HasFlag(Diet.Plants);
            var migrating = plantEater && accumulator.Members > 1 && (habitat.HabitatPressure >= 700 || habitat.PlantBiomass < 100);
            var target = migrating ? FindMigrationTarget(centerX, centerY) : (centerX, centerY);
            _groups[groupId] = new PopulationGroup(groupId, accumulator.Species, accumulator.Members, centerX, centerY, migrating, target.Item1, target.Item2);
        }

        if (!updateTargets)
        {
            return;
        }

        for (var id = 1; id <= Entities.Count; id++)
        {
            ref var animal = ref Entities.GetById(id);
            if (!animal.IsAlive || !_groups.TryGetValue(animal.GroupId, out var group) || SpeciesProfiles.For(animal.Species).SocialBehavior == SocialBehavior.None)
            {
                continue;
            }

            // A pack that has acquired prey must keep the hunt target.
            if (animal.TargetEntityId != 0)
            {
                continue;
            }

            if (group.IsMigrating)
            {
                animal.TargetX = group.MigrationTargetX;
                animal.TargetY = group.MigrationTargetY;
                continue;
            }

            if (Math.Abs(animal.X - group.CenterX) + Math.Abs(animal.Y - group.CenterY) > 4)
            {
                animal.TargetX = group.CenterX;
                animal.TargetY = group.CenterY;
            }
        }
    }

    private (int, int) FindMigrationTarget(int originX, int originY)
    {
        var targetX = originX;
        var targetY = originY;
        var bestScore = int.MinValue;
        const int radius = 12;
        for (var y = Math.Max(0, originY - radius); y <= Math.Min(World.Height - 1, originY + radius); y++)
        {
            for (var x = Math.Max(0, originX - radius); x <= Math.Min(World.Width - 1, originX + radius); x++)
            {
                var cell = World.CellAt(x, y);
                var score = cell.PlantBiomass - cell.HabitatPressure;
                if (score > bestScore)
                {
                    bestScore = score;
                    targetX = x;
                    targetY = y;
                }
            }
        }

        return (targetX, targetY);
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

            var movementSteps = animal.Traits.Speed + (SpeciesProfiles.For(animal.Species).HuntsPrey ? 1 : 0);
            var startingX = animal.X;
            var startingY = animal.Y;
            for (var step = 0; step < movementSteps; step++)
            {
                animal.X = Math.Clamp(animal.X + Math.Sign(animal.TargetX - animal.X), 0, World.Width - 1);
                animal.Y = Math.Clamp(animal.Y + Math.Sign(animal.TargetY - animal.Y), 0, World.Height - 1);
            }
            if (SpeciesProfiles.For(animal.Species).HuntsPrey)
                HuntingTelemetry?.RecordMovement(animal.Id, Math.Abs(animal.X - startingX) + Math.Abs(animal.Y - startingY));
        }
    }

    private void ResolveCombat()
    {
        for (var id = 1; id <= Entities.Count; id++)
        {
            ref var predator = ref Entities.GetById(id);
            if (!predator.IsAlive || !SpeciesProfiles.For(predator.Species).HuntsPrey || predator.TargetEntityId == 0 || predator.TargetEntityId == predator.Id)
            {
                continue;
            }

            ref var prey = ref Entities.GetById(predator.TargetEntityId);
            if (prey.IsAlive && SpeciesProfiles.IsPlantEater(prey.Species) && prey.Traits.Size <= predator.Traits.Size && Math.Abs(predator.X - prey.X) + Math.Abs(predator.Y - prey.Y) <= 1)
            {
                HuntingTelemetry?.RecordAttack(predator.Id);
                var wasAlive = prey.Health > 0;
                prey.Health -= 16 + predator.Traits.Size * 5 + predator.Traits.Speed;
                if (wasAlive && prey.Health <= 0)
                {
                    prey.IsAlive = false;
                    Carcasses.Add(prey.Species, prey.X, prey.Y);
                    HuntingTelemetry?.RecordCarcassCreated(SpeciesProfiles.For(prey.Species).CarcassNutrition, killedByPredator: true);
                    prey.CarcassCreated = true;
                    Lineages.RecordDeath(prey.Id, Tick, prey.X, prey.Y);
                    HuntingTelemetry?.RecordKill(predator.Id, prey.Species, Tick);
                }
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

            var profile = SpeciesProfiles.For(animal.Species);
            if (profile.Diet.HasFlag(Diet.Carcasses) && animal.Energy < SatiatedEnergy(profile) && TryEatCarcass(ref animal))
            {
                continue;
            }

            if (profile.Diet.HasFlag(Diet.Plants) && (!profile.HuntsPrey || animal.Energy < SatiatedEnergy(profile)))
            {
                ref var cell = ref World.CellAt(animal.X, animal.Y);
                var eaten = Math.Min(10 + animal.Traits.Size * 4 + animal.Traits.Metabolism * 2, cell.PlantBiomass);
                cell.PlantBiomass -= eaten;
                ResourceTelemetry?.RecordConsumption(animal.Species, eaten);
                cell.HabitatPressure = Math.Min(1_000, cell.HabitatPressure + eaten * 5);
                animal.Energy += eaten * (100 + animal.Traits.Metabolism * 5) / 100;
            }
        }
    }

    private bool TryEatCarcass(ref AnimalState animal)
    {
        for (var carcassIndex = 0; carcassIndex < Carcasses.Count; carcassIndex++)
        {
            ref var carcass = ref Carcasses.Get(carcassIndex);
            if (Math.Abs(carcass.X - animal.X) + Math.Abs(carcass.Y - animal.Y) > 1)
            {
                continue;
            }

            var eaten = Math.Min(25, carcass.Nutrition);
            carcass.Nutrition -= eaten;
            HuntingTelemetry?.RecordCarcassConsumed(eaten);
            var energyGained = eaten * (100 + animal.Traits.Metabolism * 5) / 100;
            animal.Energy += energyGained;
            if (eaten > 0)
                HuntingTelemetry?.RecordMeal(animal.Id, energyGained, Tick);
            return eaten > 0;
        }

        return false;
    }

    private void SetWanderTarget(ref AnimalState animal)
    {
        animal.TargetEntityId = 0;
        animal.TargetX = Math.Clamp(animal.X + WanderStep(animal.Id, 60), 0, World.Width - 1);
        animal.TargetY = Math.Clamp(animal.Y + WanderStep(animal.Id, 61), 0, World.Height - 1);
    }

    private void ResolveReproduction()
    {
        var initialEntityCount = Entities.Count;
        for (var id = 1; id <= initialEntityCount; id++)
        {
            var mother = Entities.GetById(id);
            if (!CanReproduce(mother) || mother.Sex != AnimalSex.Female)
            {
                continue;
            }

            var fatherId = SpatialIndex.FindNearestMate(
                Entities,
                mother.X,
                mother.Y,
                SpeciesProfiles.For(mother.Species).HuntsPrey ? mother.Traits.Vision * 3 : mother.Traits.Vision,
                mother.Species,
                AnimalSex.Male);
            if (fatherId == 0)
            {
                continue;
            }

            var father = Entities.GetById(fatherId);
            if (!CanReproduce(father))
            {
                continue;
            }

            CreateOffspring(mother, father);
            ref var motherState = ref Entities.GetById(mother.Id);
            ref var fatherState = ref Entities.GetById(father.Id);
            motherState.Energy -= ReproductionEnergyCost;
            fatherState.Energy -= ReproductionEnergyCost;
            motherState.ReproductionCooldown = ReproductionCooldown(mother.Traits.Fertility);
            fatherState.ReproductionCooldown = ReproductionCooldown(father.Traits.Fertility);
        }
    }

    private void CreateOffspring(AnimalState mother, AnimalState father)
    {
        var childId = SpawnAnimal(mother.Species, mother.X, mother.Y);
        ref var child = ref Entities.GetById(childId);
        child.Generation = Math.Max(mother.Generation, father.Generation) + 1;
        child.ParentAId = mother.Id;
        child.ParentBId = father.Id;
        child.GroupId = mother.GroupId;
        child.Traits = InheritTraits(mother.Traits, father.Traits, childId);
        child.Energy = SpeciesProfiles.For(mother.Species).StartingEnergy;
        child.Health = SpeciesProfiles.For(mother.Species).StartingHealth;
        Lineages.RegisterBirth(child, Tick);
    }

    private bool CanReproduce(AnimalState animal)
    {
        var profile = SpeciesProfiles.For(animal.Species);
        var minimumReproductionEnergy = profile.HuntsPrey ? SatiatedEnergy(profile) : ReproductionEnergyCost + 50;
        if (!animal.IsAlive ||
            animal.AgeTicks < MaturityAgeTicks ||
            animal.Health <= 0 ||
            animal.Energy < minimumReproductionEnergy ||
            animal.ReproductionCooldown != 0)
        {
            return false;
        }

        if (!profile.Diet.HasFlag(Diet.Plants))
        {
            return true;
        }

        var localHabitat = World.CellAt(animal.X, animal.Y);
        return
            localHabitat.PlantBiomass >= 100 &&
            localHabitat.HabitatPressure < 700;
    }

    private AnimalTraits CreateInitialTraits(int id)
    {
        var profile = SpeciesProfiles.For(Entities.GetById(id).Species);
        return new AnimalTraits
        {
            Speed = Between(profile.MinimumTraits.Speed, profile.MaximumTraits.Speed, id, 40),
            Metabolism = Between(profile.MinimumTraits.Metabolism, profile.MaximumTraits.Metabolism, id, 41),
            Vision = Between(profile.MinimumTraits.Vision, profile.MaximumTraits.Vision, id, 42),
            Size = Between(profile.MinimumTraits.Size, profile.MaximumTraits.Size, id, 43),
            Fertility = Between(profile.MinimumTraits.Fertility, profile.MaximumTraits.Fertility, id, 44),
        };
    }

    private AnimalTraits InheritTraits(AnimalTraits mother, AnimalTraits father, int childId)
    {
        var bounds = SpeciesProfiles.For(Entities.GetById(childId).Species);
        return new AnimalTraits
        {
            Speed = Mutate(InheritValue(mother.Speed, father.Speed, childId, 50), bounds.MinimumTraits.Speed, bounds.MaximumTraits.Speed, childId, 50),
            Metabolism = Mutate(InheritValue(mother.Metabolism, father.Metabolism, childId, 51), bounds.MinimumTraits.Metabolism, bounds.MaximumTraits.Metabolism, childId, 51),
            Vision = Mutate(InheritValue(mother.Vision, father.Vision, childId, 52), bounds.MinimumTraits.Vision, bounds.MaximumTraits.Vision, childId, 52),
            Size = Mutate(InheritValue(mother.Size, father.Size, childId, 53), bounds.MinimumTraits.Size, bounds.MaximumTraits.Size, childId, 53),
            Fertility = Mutate(InheritValue(mother.Fertility, father.Fertility, childId, 54), bounds.MinimumTraits.Fertility, bounds.MaximumTraits.Fertility, childId, 54),
        };
    }

    private int Between(int minimum, int maximum, int id, uint stream) =>
        minimum + (int)(DeterministicHash.At(World.Settings.Seed, id, 0, stream) % (uint)(maximum - minimum + 1));

    private int InheritValue(int mother, int father, int childId, uint stream)
    {
        var sum = mother + father;
        var rounding = (sum & 1) == 0 ? 0 : (int)(DeterministicHash.At(World.Settings.Seed, childId, (int)Tick, stream + 100) & 1);
        return sum / 2 + rounding;
    }

    private static int CreateGroupId(AnimalSpecies species, int x, int y) =>
        ((int)species + 1) * 1_000_000 + (x / 16) * 1_000 + y / 16;

    private int Mutate(int inherited, int minimum, int maximum, int childId, uint stream)
    {
        var roll = DeterministicHash.At(World.Settings.Seed, childId, (int)Tick, stream);
        if (roll % 5 != 0)
        {
            return inherited;
        }

        return Math.Clamp(inherited + ((roll & 0x10) == 0 ? -1 : 1), minimum, maximum);
    }

    private static int ReproductionCooldown(int fertility) => 900 - fertility * 150;

    private int WanderStep(int animalId, uint stream) => (int)(DeterministicHash.At(World.Settings.Seed, animalId, (int)Tick, stream) % 3) - 1;

    private bool TryFindNearestCarcass(AnimalState animal, int radius, out int x, out int y)
    {
        var closestDistance = int.MaxValue;
        x = animal.X;
        y = animal.Y;
        foreach (var carcass in Carcasses.Items)
        {
            var distance = Math.Abs(carcass.X - animal.X) + Math.Abs(carcass.Y - animal.Y);
            if (distance <= radius && distance < closestDistance)
            {
                closestDistance = distance;
                x = carcass.X;
                y = carcass.Y;
            }
        }

        return closestDistance != int.MaxValue;
    }

    private static int MetabolicCost(AnimalState animal)
    {
        var traitCost = animal.Traits.Speed + animal.Traits.Metabolism + animal.Traits.Vision / 3 + animal.Traits.Size + animal.Traits.Fertility / 2;
        return SpeciesProfiles.For(animal.Species).Diet.HasFlag(Diet.Carcasses) ? 1 + traitCost / 10 : 1 + traitCost;
    }

    private static int SatiatedEnergy(SpeciesProfile profile) => profile.StartingEnergy * 3;

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
                if (SpeciesProfiles.IsPlantEater(animal.Species) && animal.Energy <= 0)
                    ResourceTelemetry?.RecordHerbivoreStarvation();
                if (SpeciesProfiles.For(animal.Species).HuntsPrey && animal.Energy <= 0)
                    HuntingTelemetry?.RecordStarvation(animal.Id);
                Carcasses.Add(animal.Species, animal.X, animal.Y);
                HuntingTelemetry?.RecordCarcassCreated(SpeciesProfiles.For(animal.Species).CarcassNutrition, killedByPredator: false);
                animal.CarcassCreated = true;
                Lineages.RecordDeath(animal.Id, Tick, animal.X, animal.Y);
            }
        }
    }

    private void ProcessCarcasses()
    {
        for (var index = Carcasses.Count - 1; index >= 0; index--)
        {
            ref var carcass = ref Carcasses.Get(index);
            if (carcass.Nutrition > 0)
            {
                carcass.Nutrition--;
                HuntingTelemetry?.RecordCarcassDecayed();
            }
            if (carcass.Nutrition <= 0)
            {
                Carcasses.RemoveAt(index);
            }
        }
    }

    private void CollectMetrics()
    {
        BuildMetrics();
        ClimateHistory.Record(Tick, Climate, Metrics.HerbivoreTraits, Metrics.PredatorTraits);
        NaturalHistory.Update(this);
    }

    private void BuildMetrics()
    {
        var herbivores = 0;
        var predators = 0;
        var herbivoreTotals = new TraitTotals();
        var predatorTotals = new TraitTotals();
        _populationBySpecies.Clear();
        foreach (var animal in Entities.Items)
        {
            if (!animal.IsAlive)
            {
                continue;
            }

            _populationBySpecies.TryGetValue(animal.Species, out var speciesCount);
            _populationBySpecies[animal.Species] = speciesCount + 1;

            var profile = SpeciesProfiles.For(animal.Species);
            if (profile.Diet.HasFlag(Diet.Plants))
            {
                herbivores++;
                herbivoreTotals.Add(animal);
            }
            if (profile.HuntsPrey)
            {
                predators++;
                predatorTotals.Add(animal);
            }
        }

        Metrics = new SimulationMetrics(herbivores, predators, Carcasses.Count, herbivoreTotals.Averages(), predatorTotals.Averages());
    }

    private static int MaximumAge(AnimalSpecies species) => SpeciesProfiles.For(species).HuntsPrey ? 16_000 : 12_000;

    private struct TraitTotals
    {
        private int _population;
        private long _speed;
        private long _metabolism;
        private long _vision;
        private long _size;
        private long _fertility;
        private long _generation;

        public void Add(AnimalState animal)
        {
            _population++;
            _speed += animal.Traits.Speed;
            _metabolism += animal.Traits.Metabolism;
            _vision += animal.Traits.Vision;
            _size += animal.Traits.Size;
            _fertility += animal.Traits.Fertility;
            _generation += animal.Generation;
        }

        public TraitAverages Averages() => _population == 0
            ? default
            : new TraitAverages(_population, (double)_speed / _population, (double)_metabolism / _population, (double)_vision / _population, (double)_size / _population, (double)_fertility / _population, (double)_generation / _population);
    }

    private struct GroupAccumulator
    {
        public AnimalSpecies Species;
        public int Members;
        public int SumX;
        public int SumY;

        public void Add(AnimalState animal)
        {
            Species = animal.Species;
            Members++;
            SumX += animal.X;
            SumY += animal.Y;
        }
    }
}

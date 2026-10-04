namespace LivingSim.Core;

public sealed class WorldSimulation
{
    private static readonly SimulationSystem[] OrderedSystems =
    [SimulationSystem.Climate, SimulationSystem.ResourceRegeneration, SimulationSystem.Metrics];
    private static readonly IReadOnlyList<SimulationSystem> DefinedSystemOrder = Array.AsReadOnly(OrderedSystems);

    public WorldSimulation(World world)
    {
        World = world;
    }

    public World World { get; }
    public long Tick { get; private set; }
    public static IReadOnlyList<SimulationSystem> SystemOrder => DefinedSystemOrder;

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
            case SimulationSystem.Climate:
            case SimulationSystem.Metrics:
                return; // Reserved deterministic phases; they hold no Slice 1 state yet.
            case SimulationSystem.ResourceRegeneration:
                RegenerateBiomass();
                return;
            default:
                throw new InvalidOperationException($"Unknown simulation system {system}.");
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
}

namespace LivingSim.Core;

public static class HeadlessWorldRunner
{
    public static WorldSimulation Create(WorldSettings settings) => new(WorldGenerator.Generate(settings));

    public static WorldSimulation CreatePopulated(WorldSettings settings, int herbivores, int predators)
    {
        if (herbivores < 0 || predators < 0)
        {
            throw new ArgumentOutOfRangeException("Population counts cannot be negative.");
        }

        var simulation = Create(settings);
        SpawnPopulation(simulation, AnimalSpecies.Herbivore, herbivores, stream: 10);
        SpawnPopulation(simulation, AnimalSpecies.Predator, predators, stream: 20);
        return simulation;
    }

    public static WorldSimulation Run(WorldSettings settings, int ticks)
    {
        var simulation = Create(settings);
        simulation.Advance(ticks);
        return simulation;
    }

    private static void SpawnPopulation(WorldSimulation simulation, AnimalSpecies species, int count, uint stream)
    {
        for (var index = 0; index < count; index++)
        {
            var x = (int)(DeterministicHash.At(simulation.World.Settings.Seed, index, 0, stream) % (uint)simulation.World.Width);
            var y = (int)(DeterministicHash.At(simulation.World.Settings.Seed, index, 1, stream) % (uint)simulation.World.Height);
            simulation.SpawnAnimal(species, x, y);
        }
    }
}

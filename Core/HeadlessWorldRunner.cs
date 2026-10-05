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
        SpawnPredatorsNearHerbivores(simulation, predators, herbivores);
        return simulation;
    }

    public static WorldSimulation CreateFoodWeb(WorldSettings settings, params SpeciesPopulation[] populations)
    {
        var simulation = Create(settings);
        foreach (var population in populations)
        {
            AddPopulation(simulation, population.Species, population.Count);
        }

        return simulation;
    }

    public static void AddPopulation(WorldSimulation simulation, AnimalSpecies species, int count)
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        SpawnPopulation(simulation, species, count, stream: 100 + (uint)species);
    }

    public static WorldSimulation Run(WorldSettings settings, int ticks)
    {
        var simulation = Create(settings);
        simulation.Advance(ticks);
        return simulation;
    }

    public static LongRunReport RunWithReport(WorldSimulation simulation, int ticks, int sampleInterval = 100)
    {
        if (ticks < 0 || sampleInterval < 1)
        {
            throw new ArgumentOutOfRangeException(ticks < 0 ? nameof(ticks) : nameof(sampleInterval));
        }

        var report = new LongRunReport();
        if (ticks == 0)
        {
            return report;
        }

        simulation.Advance();
        report.Record(simulation);
        for (var completed = 1; completed < ticks;)
        {
            var step = Math.Min(sampleInterval, ticks - completed);
            simulation.Advance(step);
            completed += step;
            report.Record(simulation);
        }

        return report;
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

    private static void SpawnPredatorsNearHerbivores(WorldSimulation simulation, int predators, int herbivores)
    {
        for (var index = 0; index < predators; index++)
        {
            var preyId = herbivores == 0 ? 0 : 1;
            var prey = preyId == 0 ? default : simulation.Entities.GetById(preyId);
            var x = preyId == 0
                ? (int)(DeterministicHash.At(simulation.World.Settings.Seed, index, 0, 20) % (uint)simulation.World.Width)
                : Math.Min(simulation.World.Width - 1, prey.X + 1);
            var y = preyId == 0
                ? (int)(DeterministicHash.At(simulation.World.Settings.Seed, index, 1, 20) % (uint)simulation.World.Height)
                : prey.Y;
            simulation.SpawnAnimal(AnimalSpecies.Predator, x, y);
        }
    }
}

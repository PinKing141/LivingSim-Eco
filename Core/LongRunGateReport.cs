using System.Diagnostics;

namespace LivingSim.Core;

public readonly record struct LongRunGateCheckpoint(
    long Tick,
    int Herbivores,
    int Predators,
    int MaximumGeneration,
    long PlantBiomass);

public readonly record struct LongRunGateReport(
    int Seed,
    int Ticks,
    int InitialHerbivores,
    int FinalHerbivores,
    int InitialPredators,
    int FinalPredators,
    int LowestHerbivores,
    int HighestHerbivores,
    int MaximumGeneration,
    int PopulationReversals,
    long? PredatorExtinctionTick,
    IReadOnlyList<LongRunGateCheckpoint> Checkpoints,
    double ElapsedSeconds,
    string StateHash);

public static class LongRunGateRunner
{
    private const int SampleInterval = 480;
    private const int ValidationInterval = 120;

    public static LongRunGateReport Run(
        int seed,
        int ticks = 480_000,
        int herbivores = 96,
        int predators = 2,
        int width = 96,
        int height = 64)
    {
        if (ticks < 0) throw new ArgumentOutOfRangeException(nameof(ticks));
        if (herbivores < 0) throw new ArgumentOutOfRangeException(nameof(herbivores));
        if (predators < 0) throw new ArgumentOutOfRangeException(nameof(predators));
        if (width < 1) throw new ArgumentOutOfRangeException(nameof(width));
        if (height < 1) throw new ArgumentOutOfRangeException(nameof(height));

        var simulation = HeadlessWorldRunner.CreatePopulated(
            new WorldSettings(seed, width, height), herbivores, predators);
        var checkpoints = new List<LongRunGateCheckpoint>();
        var lowestHerbivores = herbivores;
        var highestHerbivores = herbivores;
        var maximumGeneration = 0;
        var populationReversals = 0;
        var previousPopulation = herbivores;
        var previousDirection = 0;
        long? predatorExtinctionTick = null;
        var watch = Stopwatch.StartNew();

        for (var elapsed = 0; elapsed < ticks;)
        {
            var step = Math.Min(ValidationInterval, ticks - elapsed);
            simulation.Advance(step);
            elapsed += step;
            SimulationInvariants.Validate(simulation);

            if (predatorExtinctionTick is null && simulation.Metrics.Predators == 0)
                predatorExtinctionTick = simulation.Tick;

            if (simulation.Tick % SampleInterval != 0 && simulation.Tick != ticks)
                continue;

            var checkpoint = Capture(simulation);
            checkpoints.Add(checkpoint);
            lowestHerbivores = Math.Min(lowestHerbivores, checkpoint.Herbivores);
            highestHerbivores = Math.Max(highestHerbivores, checkpoint.Herbivores);
            maximumGeneration = Math.Max(maximumGeneration, checkpoint.MaximumGeneration);

            var direction = Math.Sign(checkpoint.Herbivores - previousPopulation);
            if (direction != 0)
            {
                if (previousDirection != 0 && direction != previousDirection)
                    populationReversals++;
                previousDirection = direction;
                previousPopulation = checkpoint.Herbivores;
            }
        }

        watch.Stop();
        var final = checkpoints.Count == 0 ? Capture(simulation) : checkpoints[^1];
        return new LongRunGateReport(seed, ticks, herbivores, final.Herbivores, predators, final.Predators,
            lowestHerbivores, highestHerbivores, maximumGeneration, populationReversals,
            predatorExtinctionTick, checkpoints, watch.Elapsed.TotalSeconds, SimulationStateHasher.Hash(simulation));
    }

    private static LongRunGateCheckpoint Capture(WorldSimulation simulation)
    {
        var generation = 0;
        foreach (var animal in simulation.Entities.Items)
        {
            if (animal.Species == AnimalSpecies.Herbivore)
                generation = Math.Max(generation, animal.Generation);
        }

        long biomass = 0;
        foreach (var cell in simulation.World.Cells)
            biomass += cell.PlantBiomass;

        return new LongRunGateCheckpoint(
            simulation.Tick,
            simulation.Metrics.Herbivores,
            simulation.Metrics.Predators,
            generation,
            biomass);
    }
}

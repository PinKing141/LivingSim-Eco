using System.Diagnostics;

namespace LivingSim.Core;

public readonly record struct SeedDiversityResult(
    int Seed,
    int InitialHerbivores,
    int FinalHerbivores,
    int LowestHerbivores,
    int InitialPredators,
    int FinalPredators,
    long? PredatorExtinctionTick,
    int MaximumGeneration,
    string StateHash);

public readonly record struct DiversityGateReport(
    int Ticks,
    IReadOnlyList<SeedDiversityResult> Seeds,
    int HerbivoreSurvivors,
    int PredatorSurvivors,
    double ElapsedSeconds);

public static class DiversityGateRunner
{
    private const int ValidationInterval = 120;
    private static readonly int[] CanonicalSeeds = [3, 10, 12, 20, 30, 44, 55, 71, 99, 321, 412, 777];

    public static DiversityGateReport Run(
        int ticks = 24_000,
        IReadOnlyList<int>? seeds = null,
        int herbivores = 24,
        int predators = 3,
        int width = 64,
        int height = 48)
    {
        if (ticks < 0) throw new ArgumentOutOfRangeException(nameof(ticks));
        if (herbivores < 0) throw new ArgumentOutOfRangeException(nameof(herbivores));
        if (predators < 0) throw new ArgumentOutOfRangeException(nameof(predators));
        if (width < 1) throw new ArgumentOutOfRangeException(nameof(width));
        if (height < 1) throw new ArgumentOutOfRangeException(nameof(height));

        var selectedSeeds = seeds ?? CanonicalSeeds;
        if (selectedSeeds.Count == 0) throw new ArgumentException("At least one seed is required.", nameof(seeds));

        var results = new List<SeedDiversityResult>(selectedSeeds.Count);
        var watch = Stopwatch.StartNew();
        foreach (var seed in selectedSeeds)
            results.Add(RunSeed(seed, ticks, herbivores, predators, width, height));
        watch.Stop();

        return new DiversityGateReport(
            ticks,
            results,
            results.Count(result => result.FinalHerbivores > 0),
            results.Count(result => result.FinalPredators > 0),
            watch.Elapsed.TotalSeconds);
    }

    private static SeedDiversityResult RunSeed(int seed, int ticks, int herbivores, int predators, int width, int height)
    {
        var simulation = HeadlessWorldRunner.CreatePopulated(new WorldSettings(seed, width, height), herbivores, predators);
        var lowestHerbivores = herbivores;
        long? predatorExtinctionTick = null;
        var maximumGeneration = 0;

        for (var elapsed = 0; elapsed < ticks;)
        {
            var step = Math.Min(ValidationInterval, ticks - elapsed);
            simulation.Advance(step);
            elapsed += step;
            SimulationInvariants.Validate(simulation);
            lowestHerbivores = Math.Min(lowestHerbivores, simulation.Metrics.Herbivores);
            maximumGeneration = Math.Max(maximumGeneration, MaximumHerbivoreGeneration(simulation));
            if (predatorExtinctionTick is null && simulation.Metrics.Predators == 0)
                predatorExtinctionTick = simulation.Tick;
        }

        return new SeedDiversityResult(
            seed,
            herbivores,
            simulation.Metrics.Herbivores,
            lowestHerbivores,
            predators,
            simulation.Metrics.Predators,
            predatorExtinctionTick,
            maximumGeneration,
            SimulationStateHasher.Hash(simulation));
    }

    private static int MaximumHerbivoreGeneration(WorldSimulation simulation)
    {
        var generation = 0;
        foreach (var animal in simulation.Entities.Items)
        {
            if (animal.Species == AnimalSpecies.Herbivore)
                generation = Math.Max(generation, animal.Generation);
        }

        return generation;
    }
}

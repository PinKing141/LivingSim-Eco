using System.Diagnostics;

namespace LivingSim.Core;

public readonly record struct FoundationGateReport(
    int Seed,
    int Ticks,
    int InitialHerbivores,
    int InitialPredators,
    int FinalHerbivores,
    int FinalPredators,
    int HerbivoreLow,
    int HerbivoreHigh,
    int PredatorHigh,
    long? PredatorExtinctionTick,
    int PredatorBirths,
    int HerbivoreBirths,
    int MaxGeneration,
    double InitialMetabolism,
    double FinalMetabolism,
    int PopulationReversals,
    int ClimateEraCount,
    int HistoryEvents,
    long AllocatedBytes,
    double ElapsedSeconds,
    string StateHash);

/// <summary>Sampled, headless measurements for the foundation exit gate.</summary>
public static class FoundationGateRunner
{
    public static FoundationGateReport Run(int seed, int ticks, int herbivores = 24, int predators = 3, int width = 64, int height = 48)
    {
        var simulation = HeadlessWorldRunner.CreatePopulated(new WorldSettings(seed, width, height), herbivores, predators);
        var initialMetabolism = MeanMetabolism(simulation);
        var low = herbivores;
        var high = herbivores;
        var predatorHigh = predators;
        long? predatorExtinction = null;
        var reversals = 0;
        var previousHerbivores = herbivores;
        var priorDirection = 0;
        var climateEras = new HashSet<int>();
        var allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
        var watch = Stopwatch.StartNew();

        for (var elapsed = 0; elapsed < ticks;)
        {
            var step = Math.Min(120, ticks - elapsed);
            simulation.Advance(step);
            elapsed += step;
            SimulationInvariants.Validate(simulation);
            var currentHerbivores = simulation.Metrics.Herbivores;
            var currentPredators = simulation.Metrics.Predators;
            low = Math.Min(low, currentHerbivores);
            high = Math.Max(high, currentHerbivores);
            predatorHigh = Math.Max(predatorHigh, currentPredators);
            if (predators > 0 && predatorExtinction is null && currentPredators == 0) predatorExtinction = simulation.Tick;
            var direction = Math.Sign(currentHerbivores - previousHerbivores);
            if (direction != 0 && priorDirection != 0 && direction != priorDirection) reversals++;
            if (direction != 0) priorDirection = direction;
            previousHerbivores = currentHerbivores;
            climateEras.Add(Math.Sign(simulation.Climate.ClimateIndexMilli));
        }

        watch.Stop();
        var allocated = GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;
        var herbivoreBirths = 0;
        var predatorBirths = 0;
        var maxGeneration = 0;
        foreach (var animal in simulation.Entities.Items)
        {
            if (animal.ParentAId == 0) continue;
            if (SpeciesProfiles.For(animal.Species).HuntsPrey) predatorBirths++;
            else herbivoreBirths++;
            maxGeneration = Math.Max(maxGeneration, animal.Generation);
        }

        return new FoundationGateReport(seed, ticks, herbivores, predators, simulation.Metrics.Herbivores, simulation.Metrics.Predators,
            low, high, predatorHigh, predatorExtinction, predatorBirths, herbivoreBirths, maxGeneration,
            initialMetabolism, simulation.Metrics.HerbivoreTraits.Metabolism, reversals, climateEras.Count,
            simulation.NaturalHistory.Events.Count, allocated, watch.Elapsed.TotalSeconds, SimulationStateHasher.Hash(simulation));
    }

    private static double MeanMetabolism(WorldSimulation simulation)
    {
        var sum = 0;
        var count = 0;
        foreach (var animal in simulation.Entities.Items)
            if (SpeciesProfiles.For(animal.Species).Diet.HasFlag(Diet.Plants)) { sum += animal.Traits.Metabolism; count++; }
        return count == 0 ? 0 : (double)sum / count;
    }
}

using System.Diagnostics;

namespace LivingSim.Core;

public sealed record ReleaseBenchmark(
    string Name,
    string Purpose,
    WorldSettings Settings,
    IReadOnlyList<SpeciesPopulation> Population);

public readonly record struct ReleaseBenchmarkResult(
    string Name,
    int Ticks,
    int AnimalsCreated,
    int LivingAnimals,
    TimeSpan Elapsed,
    long AllocatedBytes,
    string StateHash,
    int NaturalHistoryEvents);

/// <summary>Small, repeatable scenarios used to evaluate release readiness.</summary>
public static class ReleaseBenchmarks
{
    public static IReadOnlyList<ReleaseBenchmark> Scenarios { get; } =
    [
        Scenario("density", "High population density and spatial-query pressure.", 71, 128, 96, (AnimalSpecies.SmallHerbivore, 700), (AnimalSpecies.Herbivore, 350), (AnimalSpecies.Predator, 80)),
        Scenario("coexistence", "Mixed herbivore/predator food-web stability.", 44, 96, 64, (AnimalSpecies.Herbivore, 30), (AnimalSpecies.LargeHerbivore, 12), (AnimalSpecies.SmallHerbivore, 18), (AnimalSpecies.Predator, 5), (AnimalSpecies.ApexPredator, 2), (AnimalSpecies.Omnivore, 5), (AnimalSpecies.Scavenger, 4)),
        Scenario("extinction", "A constrained predator-prey collapse case.", 3, 32, 24, (AnimalSpecies.Herbivore, 4), (AnimalSpecies.Predator, 8)),
        Scenario("drought", "Climate stress through a drought-era seed.", 99, 80, 56, (AnimalSpecies.Herbivore, 28), (AnimalSpecies.Predator, 4), (AnimalSpecies.Omnivore, 3)),
        Scenario("evolution", "Multi-generation trait movement.", 10, 96, 64, (AnimalSpecies.Herbivore, 24)),
        Scenario("migration", "Social groups under local resource pressure.", 12, 72, 48, (AnimalSpecies.LargeHerbivore, 18), (AnimalSpecies.SmallHerbivore, 24), (AnimalSpecies.Predator, 4)),
        Scenario("save-load", "Deterministic save, reload, and continuation.", 321, 64, 48, (AnimalSpecies.Herbivore, 18), (AnimalSpecies.Predator, 3), (AnimalSpecies.Omnivore, 3), (AnimalSpecies.Scavenger, 2)),
    ];

    public static ReleaseBenchmark Find(string name) => Scenarios.FirstOrDefault(scenario => string.Equals(scenario.Name, name, StringComparison.OrdinalIgnoreCase))
        ?? throw new ArgumentException($"Unknown benchmark '{name}'. Available: {string.Join(", ", Scenarios.Select(scenario => scenario.Name))}.", nameof(name));

    public static ReleaseBenchmarkResult Run(ReleaseBenchmark scenario, int ticks)
    {
        if (ticks < 0) throw new ArgumentOutOfRangeException(nameof(ticks));
        var simulation = HeadlessWorldRunner.CreateFoodWeb(scenario.Settings, scenario.Population.ToArray());
        var animalsCreated = simulation.Entities.Count;
        var allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
        var watch = Stopwatch.StartNew();
        simulation.Advance(ticks);
        watch.Stop();
        var livingAnimals = 0;
        foreach (var animal in simulation.Entities.Items)
            if (animal.IsAlive) livingAnimals++;
        var allocated = GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;
        return new ReleaseBenchmarkResult(scenario.Name, ticks, animalsCreated, livingAnimals, watch.Elapsed, allocated, SimulationStateHasher.Hash(simulation), simulation.NaturalHistory.Events.Count);
    }

    private static ReleaseBenchmark Scenario(string name, string purpose, int seed, int width, int height, params (AnimalSpecies Species, int Count)[] species) =>
        new(name, purpose, new WorldSettings(seed, width, height), species.Select(entry => new SpeciesPopulation(entry.Species, entry.Count)).ToArray());
}

using LivingSim.Core;
using Xunit;

namespace LivingSim.Tests;

public sealed class ReleaseBenchmarkTests
{
    [Fact]
    public void Catalog_CoversEveryReleaseBenchmarkCategory()
    {
        var names = ReleaseBenchmarks.Scenarios.Select(scenario => scenario.Name).ToArray();

        Assert.Equal(["density", "coexistence", "extinction", "drought", "evolution", "migration", "save-load"], names);
    }

    [Fact]
    public void DensityBenchmark_HandlesMoreThanOneThousandAnimalsDeterministically()
    {
        var scenario = ReleaseBenchmarks.Find("density");
        var first = ReleaseBenchmarks.Run(scenario, 25);
        var second = ReleaseBenchmarks.Run(scenario, 25);

        Assert.True(first.AnimalsCreated > 1_000);
        Assert.Equal(first.StateHash, second.StateHash);
        Assert.True(first.Elapsed >= TimeSpan.Zero);
    }

    [Fact]
    public void LongRunSoak_CompletesWithoutObserverParticipation()
    {
        var simulation = HeadlessWorldRunner.CreateFoodWeb(new WorldSettings(10, 64, 48),
            new SpeciesPopulation(AnimalSpecies.Herbivore, 16),
            new SpeciesPopulation(AnimalSpecies.Predator, 2),
            new SpeciesPopulation(AnimalSpecies.Omnivore, 2));

        simulation.Advance(10_000);

        Assert.Equal(10_000, simulation.Tick);
        Assert.NotEmpty(simulation.ClimateHistory.Records);
        Assert.NotEmpty(SimulationStateHasher.Hash(simulation));
    }

    [Fact]
    public void ScaleGate_ProducesDeterministicRepeatedSamples()
    {
        var report = ScaleGateRunner.Run(ticks: 25, warmupRuns: 1, measuredRuns: 2);

        Assert.Equal("density", report.Scenario);
        Assert.Equal(2, report.Samples.Count);
        Assert.True(report.StateHashesMatch);
        Assert.All(report.Samples, sample => Assert.True(sample.AnimalsCreated > 1_000));
        Assert.True(report.MinimumElapsed <= report.MedianElapsed);
        Assert.True(report.MedianElapsed <= report.MaximumElapsed);
    }
}

using LivingSim.Core;
using Xunit;

namespace LivingSim.Tests;

public sealed class EcologicalStabilityTests
{
    [Fact]
    public void GrazingAddsLocalPressure_AndUnusedHabitatRecovers()
    {
        var simulation = HeadlessWorldRunner.Create(new WorldSettings(Seed: 4, Width: 3, Height: 3));
        ConfigureUniformHabitat(simulation, biomass: 100, maximum: 1_000, regeneration: 20);
        var herbivoreId = simulation.SpawnAnimal(AnimalSpecies.Herbivore, 1, 1);
        ref var herbivore = ref simulation.Entities.GetById(herbivoreId);
        herbivore.Traits = new AnimalTraits { Speed = 1, Metabolism = 1, Vision = 2, Size = 1, Fertility = 1 };

        simulation.Advance();
        var pressureAfterGrazing = simulation.World.CellAt(1, 1).HabitatPressure;
        Assert.True(pressureAfterGrazing > 0);

        simulation.Entities.SetEnergy(herbivoreId, 0);
        simulation.Advance(100);
        var recoveredCell = simulation.World.CellAt(1, 1);
        Assert.True(recoveredCell.HabitatPressure < pressureAfterGrazing);
        Assert.True(recoveredCell.PlantBiomass > 0);
    }

    [Fact]
    public void LongRunReport_RecordsPopulationAndDetectsExtinction()
    {
        var simulation = HeadlessWorldRunner.CreatePopulated(new WorldSettings(Seed: 10, Width: 32, Height: 24), herbivores: 12, predators: 3);

        var report = HeadlessWorldRunner.RunWithReport(simulation, ticks: 2_000, sampleInterval: 100);

        Assert.Equal(21, report.Samples.Count);
        Assert.Equal(2_000, report.Samples[^1].Tick);
        Assert.True(report.Signals.HasFlag(EcologicalSignal.PredatorExtinction));
    }

    [Fact]
    public void LongRunGate_CapturesFixedIntervalCheckpoints()
    {
        var report = LongRunGateRunner.Run(seed: 3, ticks: 2_400, herbivores: 24, predators: 2, width: 32, height: 24);

        Assert.Equal(5, report.Checkpoints.Count);
        Assert.Equal(480, report.Checkpoints[0].Tick);
        Assert.Equal(2_400, report.Checkpoints[^1].Tick);
        Assert.Equal(report.FinalHerbivores, report.Checkpoints[^1].Herbivores);
        Assert.True(report.MaximumGeneration >= 1);
        Assert.All(report.Checkpoints, checkpoint => Assert.True(checkpoint.PlantBiomass >= 0));
    }

    [Fact]
    public void BenchmarkSeeds_ProduceDifferentShortRunHistories()
    {
        var summaries = new List<(int Herbivores, int Predators, EcologicalSignal Signals)>();
        foreach (var seed in new[] { 10, 20, 30 })
        {
            var simulation = HeadlessWorldRunner.CreatePopulated(new WorldSettings(seed, 96, 64), herbivores: 24, predators: 3);
            var report = HeadlessWorldRunner.RunWithReport(simulation, ticks: 500, sampleInterval: 100);
            summaries.Add((simulation.Metrics.Herbivores, simulation.Metrics.Predators, report.Signals));
        }

        Assert.True(summaries.Distinct().Count() > 1);
    }

    [Fact]
    public void DiversityGate_ReportsEveryRequestedSeed()
    {
        var report = DiversityGateRunner.Run(ticks: 1_200, seeds: new[] { 3, 10 }, herbivores: 12, predators: 2, width: 32, height: 24);

        Assert.Equal(2, report.Seeds.Count);
        Assert.Equal(2, report.HerbivoreSurvivors + report.Seeds.Count(result => result.FinalHerbivores == 0));
        Assert.All(report.Seeds, result => Assert.False(string.IsNullOrWhiteSpace(result.StateHash)));
    }

    [Fact]
    public void FoundationAcceptance_RejectsTotalExtinction()
    {
        var report = new DiversityGateReport(
            24_000,
            new[]
            {
                new SeedDiversityResult(3, 24, 100, 20, 3, 1, null, 10, "a"),
                new SeedDiversityResult(99, 24, 0, 0, 3, 0, 1_200, 0, "b"),
            },
            HerbivoreSurvivors: 1,
            PredatorSurvivors: 1,
            ElapsedSeconds: 0);

        Assert.False(FoundationGateAcceptance.AcceptsDiversity(report));
    }

    [Fact]
    public void FoundationAcceptance_AllowsReportedPredatorRiskWhenPreyCycles()
    {
        var report = new LongRunGateReport(
            Seed: 3,
            Ticks: 480_000,
            InitialHerbivores: 96,
            FinalHerbivores: 236,
            InitialPredators: 2,
            FinalPredators: 0,
            LowestHerbivores: 73,
            HighestHerbivores: 323,
            MaximumGeneration: 157,
            PopulationReversals: 269,
            PredatorExtinctionTick: 45_240,
            Checkpoints: new[] { new LongRunGateCheckpoint(480, 136, 2, 2, 4_549_190) },
            ElapsedSeconds: 0,
            StateHash: "long-run");

        Assert.True(FoundationGateAcceptance.AcceptsLongRun(report));
    }

    [Fact]
    public void SaveReloadGate_ProducesMatchingContinuationHash()
    {
        var report = SaveReloadGateRunner.Run(seed: 3, totalTicks: 2_400, saveTick: 1_200, herbivores: 12, predators: 2, width: 32, height: 24);

        Assert.True(report.HashesMatch);
        Assert.Equal(report.UninterruptedFinalHerbivores, report.ReloadedFinalHerbivores);
        Assert.Equal(report.UninterruptedFinalPredators, report.ReloadedFinalPredators);
    }

    private static void ConfigureUniformHabitat(WorldSimulation simulation, int biomass, int maximum, int regeneration)
    {
        for (var y = 0; y < simulation.World.Height; y++)
        {
            for (var x = 0; x < simulation.World.Width; x++)
            {
                ref var cell = ref simulation.World.CellAt(x, y);
                cell.Terrain = TerrainType.Plains;
                cell.Biome = BiomeType.TemperateGrassland;
                cell.PlantBiomass = biomass;
                cell.MaxPlantBiomass = maximum;
                cell.BiomassRegenerationPerTick = regeneration;
                cell.HabitatPressure = 0;
            }
        }
    }
}

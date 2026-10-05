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

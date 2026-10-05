using LivingSim.Core;
using Xunit;

namespace LivingSim.Tests;

public sealed class ClimateTests
{
    [Fact]
    public void ClimateModel_CyclesSeasonsAndEnvironmentalErasDeterministically()
    {
        var drought = ClimateModel.At(0);
        var summer = ClimateModel.At(ClimateModel.TicksPerSeason);
        var abundance = ClimateModel.At(ClimateModel.ClimatePeriodTicks / 2);

        Assert.Equal(Season.Spring, drought.Season);
        Assert.Equal(Season.Summer, summer.Season);
        Assert.True(drought.IsDrought);
        Assert.True(abundance.IsAbundance);
        Assert.True(abundance.BiomassModifierMilli > drought.BiomassModifierMilli);
        Assert.Equal(drought, ClimateModel.At(0));
    }

    [Fact]
    public void ClimateHistory_RecordsTraitDataAcrossSeasons()
    {
        var simulation = HeadlessWorldRunner.CreatePopulated(new WorldSettings(Seed: 10, Width: 32, Height: 24), herbivores: 12, predators: 0);

        simulation.Advance(ClimateModel.TicksPerSeason * 3 + 1);

        Assert.True(simulation.ClimateHistory.Records.Count >= 4);
        Assert.Contains(simulation.ClimateHistory.Records, record => record.Climate.Season == Season.Summer);
        Assert.All(simulation.ClimateHistory.Records, record => Assert.True(record.PlantEaterTraits.Population >= 0));
    }

    [Fact]
    public void ClimateDrivenWorld_RemainsDeterministic()
    {
        var settings = new WorldSettings(Seed: 412, Width: 32, Height: 24);
        var first = HeadlessWorldRunner.CreateFoodWeb(settings,
            new SpeciesPopulation(AnimalSpecies.Herbivore, 12),
            new SpeciesPopulation(AnimalSpecies.Predator, 2),
            new SpeciesPopulation(AnimalSpecies.Scavenger, 2));
        var second = HeadlessWorldRunner.CreateFoodWeb(settings,
            new SpeciesPopulation(AnimalSpecies.Herbivore, 12),
            new SpeciesPopulation(AnimalSpecies.Predator, 2),
            new SpeciesPopulation(AnimalSpecies.Scavenger, 2));

        first.Advance(1_000);
        second.Advance(1_000);

        Assert.Equal(SimulationStateHasher.Hash(first), SimulationStateHasher.Hash(second));
        Assert.Equal(first.ClimateHistory.Records, second.ClimateHistory.Records);
    }
}

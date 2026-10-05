using LivingSim.Core;
using Xunit;

namespace LivingSim.Tests;

public sealed class FoodWebTests
{
    [Fact]
    public void SpeciesProfiles_DefineMeaningfulEcologicalNiches()
    {
        var largeHerbivore = SpeciesProfiles.For(AnimalSpecies.LargeHerbivore);
        var smallHerbivore = SpeciesProfiles.For(AnimalSpecies.SmallHerbivore);
        var omnivore = SpeciesProfiles.For(AnimalSpecies.Omnivore);
        var scavenger = SpeciesProfiles.For(AnimalSpecies.Scavenger);

        Assert.True(largeHerbivore.MinimumTraits.Size > smallHerbivore.MinimumTraits.Size);
        Assert.True(smallHerbivore.MinimumTraits.Fertility > largeHerbivore.MinimumTraits.Fertility);
        Assert.Equal(Diet.Plants | Diet.Carcasses, omnivore.Diet);
        Assert.False(scavenger.HuntsPrey);
        Assert.True(scavenger.Diet.HasFlag(Diet.Carcasses));
    }

    [Fact]
    public void PlantEaters_CompeteForSharedBiomass()
    {
        var simulation = CreateUniformWorld();
        simulation.SpawnAnimal(AnimalSpecies.LargeHerbivore, 2, 2);
        simulation.SpawnAnimal(AnimalSpecies.SmallHerbivore, 2, 2);

        simulation.Advance();

        Assert.True(simulation.World.CellAt(2, 2).PlantBiomass < 1_000);
        Assert.Equal(2, simulation.Metrics.Herbivores);
    }

    [Fact]
    public void Omnivores_CanFeedFromPlants()
    {
        var simulation = CreateUniformWorld();
        simulation.SpawnAnimal(AnimalSpecies.Omnivore, 2, 2);

        simulation.Advance();

        Assert.True(simulation.World.CellAt(2, 2).PlantBiomass < 1_000);
    }

    [Fact]
    public void Scavengers_ConsumeCarcassesWithoutHunting()
    {
        var simulation = CreateUniformWorld();
        var herbivore = simulation.SpawnAnimal(AnimalSpecies.Herbivore, 2, 2);
        simulation.Entities.SetEnergy(herbivore, 0);
        simulation.Advance();
        var nutritionBefore = simulation.Carcasses.Items[0].Nutrition;

        simulation.SpawnAnimal(AnimalSpecies.Scavenger, 2, 2);
        simulation.Advance();

        Assert.True(simulation.Carcasses.Items[0].Nutrition < nutritionBefore);
    }

    [Fact]
    public void FoodWebSeed_IsDeterministic()
    {
        var composition = new[]
        {
            new SpeciesPopulation(AnimalSpecies.Herbivore, 8),
            new SpeciesPopulation(AnimalSpecies.LargeHerbivore, 3),
            new SpeciesPopulation(AnimalSpecies.SmallHerbivore, 4),
            new SpeciesPopulation(AnimalSpecies.Predator, 2),
            new SpeciesPopulation(AnimalSpecies.ApexPredator, 1),
            new SpeciesPopulation(AnimalSpecies.Omnivore, 2),
            new SpeciesPopulation(AnimalSpecies.Scavenger, 2),
        };
        var first = HeadlessWorldRunner.CreateFoodWeb(new WorldSettings(44, 32, 24), composition);
        var second = HeadlessWorldRunner.CreateFoodWeb(new WorldSettings(44, 32, 24), composition);

        first.Advance(100);
        second.Advance(100);

        Assert.Equal(SimulationStateHasher.Hash(first), SimulationStateHasher.Hash(second));
        Assert.True(first.PopulationBySpecies.Count > 1);
    }

    private static WorldSimulation CreateUniformWorld()
    {
        var simulation = HeadlessWorldRunner.Create(new WorldSettings(Seed: 5, Width: 5, Height: 5));
        for (var y = 0; y < simulation.World.Height; y++)
        {
            for (var x = 0; x < simulation.World.Width; x++)
            {
                ref var cell = ref simulation.World.CellAt(x, y);
                cell.Terrain = TerrainType.Plains;
                cell.Biome = BiomeType.TemperateGrassland;
                cell.PlantBiomass = 1_000;
                cell.MaxPlantBiomass = 1_000;
                cell.BiomassRegenerationPerTick = 0;
                cell.HabitatPressure = 0;
            }
        }

        return simulation;
    }
}

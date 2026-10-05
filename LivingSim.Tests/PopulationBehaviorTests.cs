using LivingSim.Core;
using Xunit;

namespace LivingSim.Tests;

public sealed class PopulationBehaviorTests
{
    [Fact]
    public void HerdMembers_CohereWhenTheyDriftApart()
    {
        var simulation = CreateRichWorld(width: 13, height: 5);
        var leftId = simulation.SpawnAnimal(AnimalSpecies.SmallHerbivore, 0, 2);
        var rightId = simulation.SpawnAnimal(AnimalSpecies.SmallHerbivore, 12, 2);
        ref var left = ref simulation.Entities.GetById(leftId);
        ref var right = ref simulation.Entities.GetById(rightId);
        right.GroupId = left.GroupId;
        left.Traits = new AnimalTraits { Speed = 1, Metabolism = 1, Vision = 3, Size = 1, Fertility = 2 };
        right.Traits = left.Traits;

        simulation.Advance();

        Assert.True(Math.Abs(simulation.Entities.GetById(leftId).X - simulation.Entities.GetById(rightId).X) < 12);
        Assert.Single(simulation.Groups);
    }

    [Fact]
    public void DepletedGroupHabitat_TriggersMigrationToRicherCells()
    {
        var simulation = CreateRichWorld(width: 9, height: 9);
        ref var depleted = ref simulation.World.CellAt(1, 1);
        depleted.PlantBiomass = 0;
        depleted.HabitatPressure = 1_000;
        var firstId = simulation.SpawnAnimal(AnimalSpecies.Herbivore, 1, 1);
        var secondId = simulation.SpawnAnimal(AnimalSpecies.Herbivore, 1, 1);
        ref var second = ref simulation.Entities.GetById(secondId);
        second.GroupId = simulation.Entities.GetById(firstId).GroupId;

        simulation.Advance();

        var group = Assert.Single(simulation.Groups).Value;
        Assert.True(group.IsMigrating);
        Assert.NotEqual((1, 1), (group.MigrationTargetX, group.MigrationTargetY));
    }

    [Fact]
    public void SpeciesProfiles_OnlyApplySocialBehaviorWhereIntended()
    {
        Assert.Equal(SocialBehavior.Herd, SpeciesProfiles.For(AnimalSpecies.LargeHerbivore).SocialBehavior);
        Assert.Equal(SocialBehavior.Pack, SpeciesProfiles.For(AnimalSpecies.Predator).SocialBehavior);
        Assert.Equal(SocialBehavior.None, SpeciesProfiles.For(AnimalSpecies.Scavenger).SocialBehavior);
    }

    private static WorldSimulation CreateRichWorld(int width, int height)
    {
        var simulation = HeadlessWorldRunner.Create(new WorldSettings(Seed: 8, Width: width, Height: height));
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
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

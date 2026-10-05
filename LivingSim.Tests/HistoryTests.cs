using LivingSim.Core;
using LivingSim.Observation;
using Xunit;

namespace LivingSim.Tests;

public sealed class HistoryTests
{
    [Fact]
    public void Lineage_IndexesParentsAndDescendantsWhenOffspringIsBorn()
    {
        var simulation = CreateUniformWorld();
        var motherId = simulation.SpawnAnimal(AnimalSpecies.Herbivore, 2, 2);
        var fatherId = simulation.SpawnAnimal(AnimalSpecies.Herbivore, 2, 2);
        ref var mother = ref simulation.Entities.GetById(motherId);
        ref var father = ref simulation.Entities.GetById(fatherId);
        mother.Sex = AnimalSex.Female;
        father.Sex = AnimalSex.Male;
        mother.AgeTicks = father.AgeTicks = 500;
        mother.Energy = father.Energy = 200;
        mother.Traits = father.Traits = new AnimalTraits { Speed = 1, Metabolism = 1, Vision = 4, Size = 1, Fertility = 3 };

        simulation.Advance();

        var child = simulation.Entities.GetById(3);
        var record = simulation.Lineages.Records[child.Id];
        Assert.Equal(motherId, record.ParentAId);
        Assert.Equal(fatherId, record.ParentBId);
        Assert.Contains(simulation.Lineages.AncestorsOf(child.Id), ancestor => ancestor.AnimalId == motherId);
        Assert.Contains(simulation.Lineages.DescendantsOf(motherId), descendant => descendant.AnimalId == child.Id);
    }

    [Fact]
    public void DeathAndExtinction_AreRecordedAsNaturalHistory()
    {
        var simulation = CreateUniformWorld();
        var animalId = simulation.SpawnAnimal(AnimalSpecies.Herbivore, 2, 2);
        simulation.Advance();
        ref var animal = ref simulation.Entities.GetById(animalId);
        animal.Health = 0;

        simulation.Advance();

        Assert.True(simulation.Lineages.Records[animalId].DeathTick.HasValue);
        Assert.Contains(simulation.NaturalHistory.Events, history => history.Type == NaturalHistoryEventType.Extinction && history.Species == AnimalSpecies.Herbivore);
    }

    [Fact]
    public void ObserverCanFocusTheLocationOfARecordedEvent()
    {
        var simulation = CreateUniformWorld();
        var controller = new ObserverController();
        var historyEvent = new NaturalHistoryEvent(0, NaturalHistoryEventType.Migration, AnimalSpecies.Herbivore, 1, 3, 4, "test");

        controller.Focus(historyEvent);

        Assert.Equal(3, controller.State.CameraX);
        Assert.Equal(4, controller.State.CameraY);
        Assert.Contains("History:", WorldObserver.Timeline(simulation));
    }

    private static WorldSimulation CreateUniformWorld()
    {
        var simulation = HeadlessWorldRunner.Create(new WorldSettings(Seed: 77, Width: 5, Height: 5));
        for (var y = 0; y < simulation.World.Height; y++)
        for (var x = 0; x < simulation.World.Width; x++)
        {
            ref var cell = ref simulation.World.CellAt(x, y);
            cell.Terrain = TerrainType.Plains;
            cell.Biome = BiomeType.TemperateGrassland;
            cell.PlantBiomass = 1_000;
            cell.MaxPlantBiomass = 1_000;
            cell.BiomassRegenerationPerTick = 0;
        }
        return simulation;
    }
}

using LivingSim.Core;
using Xunit;

namespace LivingSim.Tests;

public sealed class SurvivalLoopTests
{
    [Fact]
    public void Animals_CannotLeaveWorldBounds()
    {
        var simulation = HeadlessWorldRunner.CreatePopulated(new WorldSettings(Seed: 23, Width: 9, Height: 7), herbivores: 12, predators: 4);

        simulation.Advance(500);

        Assert.All(simulation.Entities.Items.ToArray(), animal =>
        {
            Assert.InRange(animal.X, 0, simulation.World.Width - 1);
            Assert.InRange(animal.Y, 0, simulation.World.Height - 1);
        });
    }

    [Fact]
    public void Herbivores_ConsumePlantBiomass()
    {
        var simulation = CreateUniformWorld();
        simulation.SpawnAnimal(AnimalSpecies.Herbivore, 2, 2);

        simulation.Advance();

        Assert.True(simulation.World.CellAt(2, 2).PlantBiomass < 100);
        Assert.True(simulation.Entities.GetById(1).Energy > 0);
    }

    [Fact]
    public void Predators_KillValidNearbyPrey()
    {
        var simulation = CreateUniformWorld();
        var preyId = simulation.SpawnAnimal(AnimalSpecies.Herbivore, 2, 2);
        var predatorId = simulation.SpawnAnimal(AnimalSpecies.Predator, 2, 3);
        ConfigureForCloseCombat(simulation, preyId, predatorId);

        simulation.Advance(6);

        Assert.False(simulation.Entities.GetById(preyId).IsAlive);
        Assert.Equal(1, simulation.Carcasses.Count);
    }

    [Fact]
    public void Starvation_CausesDeathAndCreatesCarcass()
    {
        var simulation = CreateUniformWorld();
        var predatorId = simulation.SpawnAnimal(AnimalSpecies.Predator, 2, 2);
        simulation.Entities.SetEnergy(predatorId, 0);

        simulation.Advance();

        Assert.False(simulation.Entities.GetById(predatorId).IsAlive);
        Assert.Equal(1, simulation.Carcasses.Count);
    }

    [Fact]
    public void Carcasses_DecayAndPredatorsCanConsumeThem()
    {
        var simulation = CreateUniformWorld();
        var preyId = simulation.SpawnAnimal(AnimalSpecies.Herbivore, 2, 2);
        var predatorId = simulation.SpawnAnimal(AnimalSpecies.Predator, 2, 3);
        ConfigureForCloseCombat(simulation, preyId, predatorId);

        simulation.Advance(6);

        Assert.False(simulation.Entities.GetById(preyId).IsAlive);
        Assert.True(simulation.Entities.GetById(predatorId).Energy > 114);
        Assert.True(simulation.Carcasses.Items[0].Nutrition < 119);

        var decaySimulation = CreateUniformWorld();
        var starvingHerbivore = decaySimulation.SpawnAnimal(AnimalSpecies.Herbivore, 2, 2);
        decaySimulation.Entities.SetEnergy(starvingHerbivore, 0);
        decaySimulation.Advance(130);
        Assert.Equal(0, decaySimulation.Carcasses.Count);
    }

    [Fact]
    public void SameSeedAndPopulation_ProducesSameFullStateAfterTicks()
    {
        var settings = new WorldSettings(Seed: 919, Width: 32, Height: 24);
        var first = HeadlessWorldRunner.CreatePopulated(settings, herbivores: 20, predators: 5);
        var second = HeadlessWorldRunner.CreatePopulated(settings, herbivores: 20, predators: 5);

        first.Advance(1_000);
        second.Advance(1_000);

        Assert.Equal(SimulationStateHasher.Hash(first), SimulationStateHasher.Hash(second));
    }

    private static WorldSimulation CreateUniformWorld()
    {
        var simulation = HeadlessWorldRunner.Create(new WorldSettings(Seed: 1, Width: 5, Height: 5));
        for (var y = 0; y < simulation.World.Height; y++)
        {
            for (var x = 0; x < simulation.World.Width; x++)
            {
                ref var cell = ref simulation.World.CellAt(x, y);
                cell.Terrain = TerrainType.Plains;
                cell.Biome = BiomeType.TemperateGrassland;
                cell.PlantBiomass = 100;
                cell.MaxPlantBiomass = 100;
                cell.BiomassRegenerationPerTick = 0;
            }
        }

        return simulation;
    }

    private static void ConfigureForCloseCombat(WorldSimulation simulation, int preyId, int predatorId)
    {
        ref var prey = ref simulation.Entities.GetById(preyId);
        ref var predator = ref simulation.Entities.GetById(predatorId);
        prey.Traits = new AnimalTraits { Speed = 1, Metabolism = 1, Vision = 4, Size = 1, Fertility = 1 };
        predator.Traits = new AnimalTraits { Speed = 1, Metabolism = 1, Vision = 6, Size = 3, Fertility = 1 };
    }
}

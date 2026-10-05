using LivingSim.Core;
using Xunit;

namespace LivingSim.Tests;

public sealed class ReproductionTests
{
    [Fact]
    public void Juveniles_CannotReproduce()
    {
        var simulation = CreateUniformWorld();
        ConfigurePair(simulation, ageTicks: 0, energy: 200);

        simulation.Advance();

        Assert.Equal(2, simulation.Entities.Count);
    }

    [Fact]
    public void Reproduction_CreatesBoundedDeterministicOffspringAndChargesParents()
    {
        var simulation = CreateUniformWorld();
        var (motherId, fatherId) = ConfigurePair(simulation, ageTicks: 500, energy: 200);

        simulation.Advance();

        Assert.Equal(3, simulation.Entities.Count);
        var child = simulation.Entities.GetById(3);
        Assert.Equal(1, child.Generation);
        Assert.Equal(motherId, child.ParentAId);
        Assert.Equal(fatherId, child.ParentBId);
        Assert.InRange(child.Traits.Speed, 1, 3);
        Assert.InRange(child.Traits.Metabolism, 1, 3);
        Assert.InRange(child.Traits.Vision, 2, 8);
        Assert.InRange(child.Traits.Size, 1, 3);
        Assert.InRange(child.Traits.Fertility, 1, 3);
        Assert.True(simulation.Entities.GetById(motherId).Energy < 200);
        Assert.True(simulation.Entities.GetById(fatherId).Energy < 200);
        Assert.Equal(3, simulation.Metrics.HerbivoreTraits.Population);
        Assert.True(simulation.Metrics.HerbivoreTraits.Generation > 0);
    }

    [Fact]
    public void InheritanceAndMutation_AreDeterministic()
    {
        var first = CreateUniformWorld();
        var second = CreateUniformWorld();
        ConfigurePair(first, ageTicks: 500, energy: 200);
        ConfigurePair(second, ageTicks: 500, energy: 200);

        first.Advance(300);
        second.Advance(300);

        Assert.Equal(SimulationStateHasher.Hash(first), SimulationStateHasher.Hash(second));
    }

    [Fact]
    public void MultiGenerationPopulation_RemainsDeterministic()
    {
        var settings = new WorldSettings(Seed: 10, Width: 96, Height: 64);
        var first = HeadlessWorldRunner.CreatePopulated(settings, herbivores: 24, predators: 0);
        var second = HeadlessWorldRunner.CreatePopulated(settings, herbivores: 24, predators: 0);

        first.Advance(2_500);
        second.Advance(2_500);

        Assert.Contains(first.Entities.Items.ToArray(), animal => animal.Generation >= 2);
        Assert.True(first.Metrics.HerbivoreTraits.Generation > 0);
        Assert.Equal(SimulationStateHasher.Hash(first), SimulationStateHasher.Hash(second));
    }

    [Fact]
    public void WellFedPredatorFemale_ApproachesReadyMaleBeforeReproducing()
    {
        var simulation = HeadlessWorldRunner.Create(new WorldSettings(Seed: 1, Width: 32, Height: 24));
        var femaleId = simulation.SpawnAnimal(AnimalSpecies.Predator, 2, 2);
        var maleId = simulation.SpawnAnimal(AnimalSpecies.Predator, 22, 2);
        ref var female = ref simulation.Entities.GetById(femaleId);
        female.Sex = AnimalSex.Female;
        female.AgeTicks = 300;
        female.Energy = 2_000;
        female.Traits = new AnimalTraits { Speed = 2, Metabolism = 1, Vision = 6, Size = 3, Fertility = 1 };
        ref var male = ref simulation.Entities.GetById(maleId);
        male.Sex = AnimalSex.Male;
        male.AgeTicks = 300;
        male.Energy = 2_000;
        male.Traits = female.Traits;

        simulation.Advance(5);

        Assert.True(simulation.Entities.GetById(femaleId).X > 2);
        Assert.True(simulation.Entities.Count > 2);
    }

    private static (int MotherId, int FatherId) ConfigurePair(WorldSimulation simulation, int ageTicks, int energy)
    {
        var motherId = simulation.SpawnAnimal(AnimalSpecies.Herbivore, 2, 2);
        var fatherId = simulation.SpawnAnimal(AnimalSpecies.Herbivore, 2, 2);
        ref var mother = ref simulation.Entities.GetById(motherId);
        ref var father = ref simulation.Entities.GetById(fatherId);
        mother.Sex = AnimalSex.Female;
        father.Sex = AnimalSex.Male;
        mother.AgeTicks = ageTicks;
        father.AgeTicks = ageTicks;
        mother.Energy = energy;
        father.Energy = energy;
        mother.Traits = new AnimalTraits { Speed = 1, Metabolism = 1, Vision = 4, Size = 1, Fertility = 3 };
        father.Traits = new AnimalTraits { Speed = 1, Metabolism = 1, Vision = 4, Size = 1, Fertility = 3 };
        return (motherId, fatherId);
    }

    private static WorldSimulation CreateUniformWorld()
    {
        var simulation = HeadlessWorldRunner.Create(new WorldSettings(Seed: 77, Width: 5, Height: 5));
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
            }
        }

        return simulation;
    }
}

using LivingSim.Animals;
using LivingSim.Core;
using LivingSim.Environment;
using LivingSim.World;

namespace LivingSim.Tests;

public class SimulationRulesTests
{
    [Fact]
    public void SpawnAnimal_IgnoresOutOfBoundsCoordinates()
    {
        var manager = new AnimalManager(5, 5, new Random(123));

        manager.SpawnAnimal(AnimalType.Herbivore, -1, 0);
        manager.SpawnAnimal(AnimalType.Herbivore, 0, -1);
        manager.SpawnAnimal(AnimalType.Herbivore, 5, 0);
        manager.SpawnAnimal(AnimalType.Herbivore, 0, 5);
        manager.SpawnAnimal(AnimalType.Carnivore, 4, 4);

        var animals = manager.GetAnimals();
        Assert.Single(animals);
        Assert.Equal(4, animals[0].X);
        Assert.Equal(4, animals[0].Y);
    }

    [Fact]
    public void ReproductionConstraints_RespectAgeAndCrowdingThresholds()
    {
        var youngCarnivore = new Animal(Species.Wolf, 0, 0, new Random(1));

        Assert.False(youngCarnivore.IsReadyToReproduce());
        Assert.False(youngCarnivore.IsCrowded(4));
        Assert.True(youngCarnivore.IsCrowded(5));

        var youngOmnivore = new Animal(Species.Bear, 0, 0, new Random(2));
        Assert.False(youngOmnivore.IsCrowded(7));
        Assert.True(youngOmnivore.IsCrowded(8));

        var youngHerbivore = new Animal(Species.Deer, 0, 0, new Random(3));
        Assert.False(youngHerbivore.IsCrowded(14));
        Assert.True(youngHerbivore.IsCrowded(15));
    }

    [Fact]
    public void PlagueChecks_TriggerOnlyOnConfiguredCadence()
    {
        var random = new Random(99);
        var grid = new Grid(5, 5);
        var manager = new AnimalManager(
            width: 5,
            height: 5,
            random: random,
            plagueCheckInterval: 10,
            plagueThresholdRatio: 0.01f,
            plagueChance: 1.0f,
            plagueMortalityRate: 0.0f);

        for (int i = 0; i < 20; i++)
        {
            manager.SpawnAnimal(AnimalType.Herbivore, random.Next(5), random.Next(5));
        }

        for (int tick = 1; tick <= 9; tick++)
        {
            manager.Tick(grid, tick, isNight: false, currentSeason: Season.Spring);
        }

        Assert.Equal(0, manager.PlagueEventsTriggered);

        manager.Tick(grid, 10, isNight: false, currentSeason: Season.Spring);
        Assert.True(manager.PlagueEventsTriggered > 0);
    }

    [Fact]
    public void SimulationClock_ProgressesSeasonByDayBoundaries()
    {
        var clock = new SimulationClock();

        Assert.Equal(Season.Spring, clock.CurrentSeason);

        clock.AdvanceTicks(SimulationClock.TicksPerDay * SimulationClock.DaysPerSeason);
        Assert.Equal(Season.Summer, clock.CurrentSeason);

        clock.AdvanceTicks(SimulationClock.TicksPerDay * SimulationClock.DaysPerSeason);
        Assert.Equal(Season.Autumn, clock.CurrentSeason);

        clock.AdvanceTicks(SimulationClock.TicksPerDay * SimulationClock.DaysPerSeason);
        Assert.Equal(Season.Winter, clock.CurrentSeason);
    }
}

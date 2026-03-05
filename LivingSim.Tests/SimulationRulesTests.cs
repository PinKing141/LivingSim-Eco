using LivingSim.Animals;
using LivingSim.Core;
using LivingSim.Environment;
using LivingSim.Observation;
using LivingSim.World;
using System.Text;
using Xunit;

namespace LivingSim.Tests;

public class SimulationRulesTests
{
    [Fact]
    public void SpawnAnimal_IgnoresOutOfBoundsCoordinates()
    {
        var manager = new AnimalManager(5, 5, new Random(123), new DeterministicIdGenerator(123));

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
        var youngCarnivore = new Animal(Species.Wolf, 0, 0, new Random(1), new DeterministicIdGenerator(1), currentTick: 0);

        Assert.False(youngCarnivore.IsReadyToReproduce());
        Assert.False(youngCarnivore.IsCrowded(4));
        Assert.True(youngCarnivore.IsCrowded(5));

        var youngOmnivore = new Animal(Species.Bear, 0, 0, new Random(2), new DeterministicIdGenerator(2), currentTick: 0);
        Assert.False(youngOmnivore.IsCrowded(7));
        Assert.True(youngOmnivore.IsCrowded(8));

        var youngHerbivore = new Animal(Species.Deer, 0, 0, new Random(3), new DeterministicIdGenerator(3), currentTick: 0);
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
            idGenerator: new DeterministicIdGenerator(99),
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

    [Fact]
    public void GroupIds_AreDeterministicAcrossRunsWithSameSeed()
    {
        static List<string> RunAndCaptureGroupIds(int seed, int ticks)
        {
            var random = new Random(seed);
            var idGenerator = new DeterministicIdGenerator(seed);
            var grid = new Grid(8, 8);
            var manager = new AnimalManager(8, 8, random, idGenerator);

            for (int i = 0; i < 6; i++)
            {
                manager.SpawnAnimal(AnimalType.Herbivore, random.Next(8), random.Next(8));
                manager.SpawnAnimal(AnimalType.Carnivore, random.Next(8), random.Next(8));
            }

            var snapshots = new List<string>();
            for (int tick = 1; tick <= ticks; tick++)
            {
                manager.Tick(grid, tick, isNight: tick % 2 == 0, currentSeason: Season.Spring);
                var ids = manager.GetAnimals()
                    .Where(a => a.IsAlive)
                    .Select(a => a.GroupId)
                    .OrderBy(id => id)
                    .Select(id => id.ToString())
                    .ToArray();
                snapshots.Add(string.Join("|", ids));
            }

            return snapshots;
        }

        var runA = RunAndCaptureGroupIds(seed: 321, ticks: 50);
        var runB = RunAndCaptureGroupIds(seed: 321, ticks: 50);

        Assert.Equal(runA, runB);
    }

    [Fact]
    public void WorldManagerTick_DoesNotRequireConsoleOutput()
    {
        var originalOut = Console.Out;
        try
        {
            Console.SetOut(new ThrowOnWriteTextWriter());

            var clock = new SimulationClock();
            var grid = new Grid(4, 4);
            var environment = new EnvironmentTickSystem();
            var metrics = new MetricsCollector();
            var animals = new AnimalManager(4, 4, new Random(42), new DeterministicIdGenerator(42));
            var manager = new WorldManager(clock, grid, environment, metrics, animals);

            manager.Tick();

            Assert.Equal(1, clock.CurrentTick);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    private sealed class ThrowOnWriteTextWriter : TextWriter
    {
        public override Encoding Encoding => Encoding.UTF8;

        public override void Write(char value) =>
            throw new InvalidOperationException("Console output is not allowed in core tick execution.");

        public override void Write(string? value) =>
            throw new InvalidOperationException("Console output is not allowed in core tick execution.");

        public override void WriteLine(string? value) =>
            throw new InvalidOperationException("Console output is not allowed in core tick execution.");

        public override void WriteLine() =>
            throw new InvalidOperationException("Console output is not allowed in core tick execution.");
    }
}

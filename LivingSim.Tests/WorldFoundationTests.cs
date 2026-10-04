using LivingSim.Core;
using Xunit;

namespace LivingSim.Tests;

public sealed class WorldFoundationTests
{
    [Fact]
    public void SameSeed_GeneratesIdenticalWorldData()
    {
        var settings = new WorldSettings(Seed: 41, Width: 32, Height: 24);

        var first = WorldGenerator.Generate(settings);
        var second = WorldGenerator.Generate(settings);

        Assert.Equal(WorldStateHasher.Hash(first, 0), WorldStateHasher.Hash(second, 0));
    }

    [Fact]
    public void SameSeed_ProducesTheSameResourceStateAfterTicks()
    {
        var settings = new WorldSettings(Seed: -17, Width: 40, Height: 30);

        var first = HeadlessWorldRunner.Run(settings, ticks: 10_000);
        var second = HeadlessWorldRunner.Run(settings, ticks: 10_000);

        Assert.Equal(first.Tick, second.Tick);
        Assert.Equal(WorldStateHasher.Hash(first.World, first.Tick), WorldStateHasher.Hash(second.World, second.Tick));
    }

    [Fact]
    public void Regeneration_IsClampedToCellMaximum()
    {
        var simulation = HeadlessWorldRunner.Create(new WorldSettings(Seed: 5, Width: 12, Height: 12));

        simulation.Advance(10_000);

        Assert.All(simulation.World.Cells.ToArray(), cell => Assert.InRange(cell.PlantBiomass, 0, cell.MaxPlantBiomass));
    }

    [Fact]
    public void SimulationOrder_IsStableAndExplicit()
    {
        Assert.Equal(
        [SimulationSystem.Climate, SimulationSystem.ResourceRegeneration, SimulationSystem.Metrics],
        WorldSimulation.SystemOrder);
    }

    [Fact]
    public void HeadlessRunner_DoesNotRequireConsoleOutput()
    {
        var originalOut = Console.Out;
        try
        {
            Console.SetOut(new ThrowOnWriteTextWriter());
            var simulation = HeadlessWorldRunner.Run(new WorldSettings(Seed: 7, Width: 8, Height: 8), ticks: 100);
            Assert.Equal(100, simulation.Tick);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    private sealed class ThrowOnWriteTextWriter : TextWriter
    {
        public override System.Text.Encoding Encoding => System.Text.Encoding.UTF8;
        public override void Write(char value) => throw new InvalidOperationException("Core wrote to Console.");
        public override void Write(string? value) => throw new InvalidOperationException("Core wrote to Console.");
        public override void WriteLine(string? value) => throw new InvalidOperationException("Core wrote to Console.");
    }
}

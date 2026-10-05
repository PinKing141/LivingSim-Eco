using LivingSim.Core;
using Xunit;

namespace LivingSim.Tests;

public sealed class EngineeringGateTests
{
    [Fact]
    public void HeadlessDensityRun_MaintainsStateInvariants()
    {
        var scenario = ReleaseBenchmarks.Find("density");
        var simulation = HeadlessWorldRunner.CreateFoodWeb(scenario.Settings, scenario.Population.ToArray());

        for (var sample = 0; sample < 10; sample++)
        {
            simulation.Advance(100);
            SimulationInvariants.Validate(simulation);
        }
    }

    [Fact]
    public void GeographyGate_CapturesActualRegionalHistory()
    {
        var report = GeographyGateRunner.Run(seed: 3, ticks: 1_200);

        Assert.Equal(10, report.West.HistorySamples);
        Assert.Equal(report.West.HistorySamples, report.West.History.Count);
        Assert.Equal(report.East.HistorySamples, report.East.History.Count);
        Assert.Equal(120, report.West.History[0].Tick);
        Assert.Equal(1_200, report.West.History[^1].Tick);
        Assert.Equal(report.West.History[^1].Population, report.West.Population);
        Assert.Equal(report.East.History[^1].Population, report.East.Population);
        Assert.Contains(report.West.History, sample => sample.MeanSpeed != report.West.History[0].MeanSpeed);
    }
}

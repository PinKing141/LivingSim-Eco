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
}

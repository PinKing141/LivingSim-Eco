namespace LivingSim.Core;

public static class HeadlessWorldRunner
{
    public static WorldSimulation Create(WorldSettings settings) => new(WorldGenerator.Generate(settings));

    public static WorldSimulation Run(WorldSettings settings, int ticks)
    {
        var simulation = Create(settings);
        simulation.Advance(ticks);
        return simulation;
    }
}

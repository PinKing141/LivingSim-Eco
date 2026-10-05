using System.Diagnostics;

namespace LivingSim.Core;

public readonly record struct RegionalSnapshot(int Population, int Births, int MaximumGeneration, double MeanSpeed, double MeanMetabolism);

public readonly record struct GeographyGateReport(
    int Seed,
    int Ticks,
    RegionalSnapshot West,
    RegionalSnapshot East,
    int FinalPredators,
    double SpeedDifference,
    double MetabolismDifference,
    double ElapsedSeconds,
    string StateHash);

public static class GeographyGateRunner
{
    public static GeographyGateReport Run(int seed, int ticks = 12_000)
    {
        var simulation = HeadlessWorldRunner.Create(new WorldSettings(seed, 128, 96), ClimateMode.MildConstant);
        for (var index = 0; index < 96; index++)
        {
            simulation.SpawnAnimal(AnimalSpecies.Herbivore, 8 + index % 24, 12 + (index * 7) % 72);
            simulation.SpawnAnimal(AnimalSpecies.Herbivore, 96 + index % 24, 12 + (index * 11) % 72);
        }

        for (var index = 0; index < 2; index++)
            simulation.SpawnAnimal(AnimalSpecies.Predator, 12 + index * 3, 24 + index * 5);

        var watch = Stopwatch.StartNew();
        for (var elapsed = 0; elapsed < ticks;)
        {
            var step = Math.Min(120, ticks - elapsed);
            simulation.Advance(step);
            elapsed += step;
            SimulationInvariants.Validate(simulation);
        }

        watch.Stop();
        var west = Capture(simulation, x => x < simulation.World.Width / 2);
        var east = Capture(simulation, x => x >= simulation.World.Width / 2);
        return new GeographyGateReport(seed, ticks, west, east, simulation.Metrics.Predators,
            Math.Abs(west.MeanSpeed - east.MeanSpeed), Math.Abs(west.MeanMetabolism - east.MeanMetabolism),
            watch.Elapsed.TotalSeconds, SimulationStateHasher.Hash(simulation));
    }

    private static RegionalSnapshot Capture(WorldSimulation simulation, Func<int, bool> region)
    {
        var population = 0;
        var births = 0;
        var generation = 0;
        var speed = 0;
        var metabolism = 0;
        foreach (var animal in simulation.Entities.Items)
        {
            if (!animal.IsAlive || animal.Species != AnimalSpecies.Herbivore || !region(animal.X)) continue;
            population++;
            if (animal.ParentAId != 0) births++;
            generation = Math.Max(generation, animal.Generation);
            speed += animal.Traits.Speed;
            metabolism += animal.Traits.Metabolism;
        }

        return population == 0
            ? new RegionalSnapshot(0, births, generation, 0, 0)
            : new RegionalSnapshot(population, births, generation, (double)speed / population, (double)metabolism / population);
    }
}

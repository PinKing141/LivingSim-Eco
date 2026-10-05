using System.Diagnostics;

namespace LivingSim.Core;

public readonly record struct RegionalSnapshot(
    int Population,
    int Births,
    int MaximumGeneration,
    double InitialMeanSpeed,
    double FinalMeanSpeed,
    double InitialMeanMetabolism,
    double FinalMeanMetabolism,
    int HistorySamples)
{
    public IReadOnlyList<RegionalHistorySample> History { get; init; } = Array.Empty<RegionalHistorySample>();
}

public readonly record struct RegionalHistorySample(
    long Tick,
    int Population,
    int Predators,
    double MeanSpeed,
    double MeanMetabolism,
    long PlantBiomass);

public readonly record struct GeographyGateReport(
    int Seed,
    int Ticks,
    RegionalSnapshot West,
    RegionalSnapshot East,
    int WestPredators,
    int EastPredators,
    double SpeedDifference,
    double MetabolismDifference,
    double ElapsedSeconds,
    string StateHash);

public static class GeographyGateRunner
{
    public static GeographyGateReport Run(int seed, int ticks = 12_000)
    {
        var west = CreateRegion(seed, predators: 2);
        var east = CreateRegion(seed + 1, predators: 0);
        var westInitial = CaptureTraits(west);
        var eastInitial = CaptureTraits(east);
        var westHistory = new List<RegionalHistorySample>();
        var eastHistory = new List<RegionalHistorySample>();

        var watch = Stopwatch.StartNew();
        for (var elapsed = 0; elapsed < ticks;)
        {
            var step = Math.Min(120, ticks - elapsed);
            west.Advance(step);
            east.Advance(step);
            elapsed += step;
            SimulationInvariants.Validate(west);
            SimulationInvariants.Validate(east);
            westHistory.Add(CaptureHistory(west));
            eastHistory.Add(CaptureHistory(east));
        }

        watch.Stop();
        var westSnapshot = Capture(west, westInitial, westHistory);
        var eastSnapshot = Capture(east, eastInitial, eastHistory);
        return new GeographyGateReport(seed, ticks, westSnapshot, eastSnapshot, west.Metrics.Predators, east.Metrics.Predators,
            Math.Abs(westSnapshot.FinalMeanSpeed - eastSnapshot.FinalMeanSpeed),
            Math.Abs(westSnapshot.FinalMeanMetabolism - eastSnapshot.FinalMeanMetabolism),
            watch.Elapsed.TotalSeconds, $"{SimulationStateHasher.Hash(west)}:{SimulationStateHasher.Hash(east)}");
    }

    private static WorldSimulation CreateRegion(int seed, int predators)
    {
        var simulation = HeadlessWorldRunner.Create(new WorldSettings(seed, 64, 96), ClimateMode.MildConstant);
        for (var index = 0; index < 96; index++)
            simulation.SpawnAnimal(AnimalSpecies.Herbivore, 8 + index % 48, 8 + (index * 7) % 80);
        for (var index = 0; index < predators; index++)
            simulation.SpawnAnimal(AnimalSpecies.Predator, 12 + index * 3, 24 + index * 5);
        return simulation;
    }

    private static RegionalSnapshot Capture(
        WorldSimulation simulation,
        (int Population, double Speed, double Metabolism) initial,
        IReadOnlyList<RegionalHistorySample> history)
    {
        var population = 0;
        var births = 0;
        var generation = 0;
        foreach (var animal in simulation.Entities.Items)
        {
            if (!animal.IsAlive || animal.Species != AnimalSpecies.Herbivore) continue;
            population++;
            if (animal.ParentAId != 0) births++;
            generation = Math.Max(generation, animal.Generation);
        }

        return population == 0
            ? new RegionalSnapshot(0, births, generation, initial.Speed, 0, initial.Metabolism, 0, history.Count) { History = history }
            : new RegionalSnapshot(population, births, generation, initial.Speed, CaptureTraits(simulation).Speed,
                initial.Metabolism, CaptureTraits(simulation).Metabolism, history.Count) { History = history };
    }

    private static RegionalHistorySample CaptureHistory(WorldSimulation simulation)
    {
        var traits = CaptureTraits(simulation);
        long biomass = 0;
        foreach (var cell in simulation.World.Cells)
            biomass += cell.PlantBiomass;

        return new RegionalHistorySample(
            simulation.Tick,
            traits.Population,
            simulation.Metrics.Predators,
            traits.Speed,
            traits.Metabolism,
            biomass);
    }

    private static (int Population, double Speed, double Metabolism) CaptureTraits(WorldSimulation simulation)
    {
        var population = 0;
        var speed = 0;
        var metabolism = 0;
        foreach (var animal in simulation.Entities.Items)
        {
            if (!animal.IsAlive || animal.Species != AnimalSpecies.Herbivore) continue;
            population++;
            speed += animal.Traits.Speed;
            metabolism += animal.Traits.Metabolism;
        }

        return population == 0 ? (0, 0, 0) : (population, (double)speed / population, (double)metabolism / population);
    }
}

using System.Diagnostics;

namespace LivingSim.Core;

public readonly record struct SaveReloadGateReport(
    int Seed,
    int TotalTicks,
    int SaveTick,
    int InitialHerbivores,
    int UninterruptedFinalHerbivores,
    int ReloadedFinalHerbivores,
    int UninterruptedFinalPredators,
    int ReloadedFinalPredators,
    string UninterruptedStateHash,
    string ReloadedStateHash,
    bool HashesMatch,
    double ElapsedSeconds);

public static class SaveReloadGateRunner
{
    public static SaveReloadGateReport Run(
        int seed,
        int totalTicks = 480_000,
        int saveTick = 240_000,
        int herbivores = 96,
        int predators = 2,
        int width = 96,
        int height = 64)
    {
        if (totalTicks < 0) throw new ArgumentOutOfRangeException(nameof(totalTicks));
        if (saveTick < 0 || saveTick > totalTicks) throw new ArgumentOutOfRangeException(nameof(saveTick));

        var settings = new WorldSettings(seed, width, height);
        var watch = Stopwatch.StartNew();
        var uninterrupted = HeadlessWorldRunner.CreatePopulated(settings, herbivores, predators);
        uninterrupted.Advance(totalTicks);
        SimulationInvariants.Validate(uninterrupted);
        var uninterruptedHash = SimulationStateHasher.Hash(uninterrupted);

        var split = HeadlessWorldRunner.CreatePopulated(settings, herbivores, predators);
        split.Advance(saveTick);
        var path = Path.Combine(Path.GetTempPath(), $"livingsim-gate14-{Guid.NewGuid():N}.json");
        try
        {
            SimulationSaveService.Save(split, path);
            var reloaded = SimulationSaveService.Load(path);
            reloaded.Advance(totalTicks - saveTick);
            SimulationInvariants.Validate(reloaded);
            var reloadedHash = SimulationStateHasher.Hash(reloaded);
            watch.Stop();

            return new SaveReloadGateReport(
                seed,
                totalTicks,
                saveTick,
                herbivores,
                uninterrupted.Metrics.Herbivores,
                reloaded.Metrics.Herbivores,
                uninterrupted.Metrics.Predators,
                reloaded.Metrics.Predators,
                uninterruptedHash,
                reloadedHash,
                string.Equals(uninterruptedHash, reloadedHash, StringComparison.Ordinal),
                watch.Elapsed.TotalSeconds);
        }
        finally
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}

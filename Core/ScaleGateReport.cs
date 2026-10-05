using System.Diagnostics;

namespace LivingSim.Core;

public readonly record struct ScaleProfileSample(
    int Run,
    int AnimalsCreated,
    int LivingAnimals,
    TimeSpan Elapsed,
    long AllocatedBytes,
    string StateHash);

public readonly record struct ScaleGateReport(
    string Scenario,
    int Ticks,
    int WarmupRuns,
    IReadOnlyList<ScaleProfileSample> Samples,
    TimeSpan MinimumElapsed,
    TimeSpan MedianElapsed,
    TimeSpan MaximumElapsed,
    long MinimumAllocatedBytes,
    long MaximumAllocatedBytes,
    bool StateHashesMatch,
    double ElapsedSeconds);

public static class ScaleGateRunner
{
    public static ScaleGateReport Run(string scenarioName = "density", int ticks = 1_000, int warmupRuns = 1, int measuredRuns = 5)
    {
        if (ticks < 0) throw new ArgumentOutOfRangeException(nameof(ticks));
        if (warmupRuns < 0) throw new ArgumentOutOfRangeException(nameof(warmupRuns));
        if (measuredRuns < 1) throw new ArgumentOutOfRangeException(nameof(measuredRuns));

        var scenario = ReleaseBenchmarks.Find(scenarioName);
        for (var run = 0; run < warmupRuns; run++)
        {
            ReleaseBenchmarks.Run(scenario, ticks);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        var samples = new List<ScaleProfileSample>(measuredRuns);
        var watch = Stopwatch.StartNew();
        for (var run = 1; run <= measuredRuns; run++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            var result = ReleaseBenchmarks.Run(scenario, ticks);
            samples.Add(new ScaleProfileSample(run, result.AnimalsCreated, result.LivingAnimals,
                result.Elapsed, result.AllocatedBytes, result.StateHash));
        }
        watch.Stop();

        var elapsed = samples.Select(sample => sample.Elapsed).OrderBy(value => value).ToArray();
        var hashes = samples.Select(sample => sample.StateHash).Distinct(StringComparer.Ordinal).Count();
        return new ScaleGateReport(
            scenario.Name,
            ticks,
            warmupRuns,
            samples,
            elapsed[0],
            elapsed[elapsed.Length / 2],
            elapsed[^1],
            samples.Min(sample => sample.AllocatedBytes),
            samples.Max(sample => sample.AllocatedBytes),
            hashes == 1,
            watch.Elapsed.TotalSeconds);
    }
}

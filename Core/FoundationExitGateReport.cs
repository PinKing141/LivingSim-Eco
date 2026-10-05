namespace LivingSim.Core;

public readonly record struct FoundationExitGateReport(
    DiversityGateReport Diversity,
    LongRunGateReport LongRun,
    SaveReloadGateReport SaveReload,
    ScaleGateReport Scale,
    bool Passed);

public static class FoundationExitGateRunner
{
    public static FoundationExitGateReport Run(int ticks = 480_000)
    {
        var diversity = DiversityGateRunner.Run();
        var longRun = LongRunGateRunner.Run(seed: 3, ticks: ticks, herbivores: 96, predators: 2, width: 96, height: 64);
        var saveReload = SaveReloadGateRunner.Run(seed: 3, totalTicks: ticks, saveTick: ticks / 2, herbivores: 96, predators: 2, width: 96, height: 64);
        var scale = ScaleGateRunner.Run(ticks: 1_000);
        var passed =
            FoundationGateAcceptance.AcceptsDiversity(diversity) &&
            FoundationGateAcceptance.AcceptsLongRun(longRun) &&
            FoundationGateAcceptance.AcceptsSaveReload(saveReload) &&
            FoundationGateAcceptance.AcceptsScale(scale);
        return new FoundationExitGateReport(diversity, longRun, saveReload, scale, passed);
    }
}

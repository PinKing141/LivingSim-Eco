namespace LivingSim.Core;

/// <summary>
/// Executable foundation thresholds for the controlled ecology witnesses.
/// Predator extinction in the millennium witness remains a reported risk, not
/// a release blocker, because the foundation gate requires trophic feedback
/// evidence and prey viability rather than universal predator persistence.
/// </summary>
public static class FoundationGateAcceptance
{
    public const int MinimumCanonicalHerbivoreSurvivors = 10;
    public const int MinimumCanonicalPredatorSurvivors = 6;

    public static bool AcceptsDiversity(DiversityGateReport report)
    {
        var totalExtinctions = report.Seeds.Count(result =>
            result.FinalHerbivores == 0 && result.FinalPredators == 0);
        return report.HerbivoreSurvivors >= MinimumCanonicalHerbivoreSurvivors &&
            report.PredatorSurvivors >= MinimumCanonicalPredatorSurvivors &&
            totalExtinctions == 0;
    }

    public static bool AcceptsLongRun(LongRunGateReport report) =>
        report.FinalHerbivores > 0 &&
        report.LowestHerbivores > 0 &&
        report.PopulationReversals > 0 &&
        report.Checkpoints.All(checkpoint => checkpoint.PlantBiomass >= 0);

    public static bool AcceptsSaveReload(SaveReloadGateReport report) =>
        report.HashesMatch &&
        report.UninterruptedFinalHerbivores > 0 &&
        report.ReloadedFinalHerbivores > 0;

    public static bool AcceptsScale(ScaleGateReport report) =>
        report.StateHashesMatch &&
        report.Samples.Count > 0 &&
        report.Samples.All(sample => sample.AnimalsCreated > 0 && sample.LivingAnimals > 0);
}

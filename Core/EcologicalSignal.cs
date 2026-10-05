namespace LivingSim.Core;

[Flags]
public enum EcologicalSignal
{
    None = 0,
    HerbivoreExtinction = 1,
    PredatorExtinction = 2,
    BiomassDepleted = 4,
    RunawayPopulation = 8,
    StaticPopulation = 16,
}

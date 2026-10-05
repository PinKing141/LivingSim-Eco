namespace LivingSim.Core;

public readonly record struct TraitAverages(int Population, double Speed, double Metabolism, double Vision, double Size, double Fertility, double Generation);

public readonly record struct SimulationMetrics(
    int Herbivores,
    int Predators,
    int Carcasses,
    TraitAverages HerbivoreTraits,
    TraitAverages PredatorTraits);

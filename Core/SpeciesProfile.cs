namespace LivingSim.Core;

/// <summary>Species are parameter sets over shared simulation systems, not bespoke AI classes.</summary>
public readonly record struct SpeciesProfile(
    Diet Diet,
    bool HuntsPrey,
    SocialBehavior SocialBehavior,
    int StartingEnergy,
    int StartingHealth,
    int CarcassNutrition,
    AnimalTraits MinimumTraits,
    AnimalTraits MaximumTraits);

public static class SpeciesProfiles
{
    public static SpeciesProfile For(AnimalSpecies species) => species switch
    {
        AnimalSpecies.Herbivore => new(Diet.Plants, false, SocialBehavior.Herd, 120, 60, 120, Traits(1, 1, 2, 1, 1), Traits(3, 3, 8, 3, 3)),
        AnimalSpecies.LargeHerbivore => new(Diet.Plants, false, SocialBehavior.Herd, 180, 100, 180, Traits(1, 1, 2, 2, 1), Traits(2, 3, 6, 3, 2)),
        AnimalSpecies.SmallHerbivore => new(Diet.Plants, false, SocialBehavior.Herd, 90, 40, 80, Traits(2, 1, 3, 1, 2), Traits(3, 2, 8, 1, 3)),
        AnimalSpecies.Predator => new(Diet.Carcasses, true, SocialBehavior.Pack, 600, 80, 160, Traits(1, 1, 3, 1, 1), Traits(3, 3, 8, 3, 3)),
        AnimalSpecies.ApexPredator => new(Diet.Carcasses, true, SocialBehavior.Pack, 750, 120, 220, Traits(1, 1, 3, 2, 1), Traits(3, 3, 7, 3, 2)),
        AnimalSpecies.Omnivore => new(Diet.Plants | Diet.Carcasses, true, SocialBehavior.None, 240, 70, 140, Traits(1, 1, 3, 1, 1), Traits(3, 3, 8, 2, 3)),
        AnimalSpecies.Scavenger => new(Diet.Carcasses, false, SocialBehavior.None, 220, 50, 100, Traits(1, 1, 4, 1, 1), Traits(3, 2, 8, 2, 2)),
        _ => throw new ArgumentOutOfRangeException(nameof(species)),
    };

    public static bool IsPlantEater(AnimalSpecies species) => For(species).Diet.HasFlag(Diet.Plants);

    private static AnimalTraits Traits(int speed, int metabolism, int vision, int size, int fertility) => new()
    {
        Speed = speed,
        Metabolism = metabolism,
        Vision = vision,
        Size = size,
        Fertility = fertility,
    };
}

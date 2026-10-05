namespace LivingSim.Core;

public readonly record struct ClimateRecord(long Tick, ClimateState Climate, TraitAverages PlantEaterTraits, TraitAverages HunterTraits);

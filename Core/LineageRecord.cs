namespace LivingSim.Core;

/// <summary>Compact, persistent identity data for one animal.</summary>
public readonly record struct LineageRecord(
    int AnimalId,
    AnimalSpecies Species,
    int ParentAId,
    int ParentBId,
    int Generation,
    long BirthTick,
    int BirthX,
    int BirthY,
    long? DeathTick,
    int DeathX,
    int DeathY);

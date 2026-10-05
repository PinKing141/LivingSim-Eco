namespace LivingSim.Core;

public readonly record struct PopulationGroup(
    int GroupId,
    AnimalSpecies Species,
    int Members,
    int CenterX,
    int CenterY,
    bool IsMigrating,
    int MigrationTargetX,
    int MigrationTargetY);

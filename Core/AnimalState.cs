namespace LivingSim.Core;

/// <summary>Dense per-animal state. IDs are stable for the life of a simulation.</summary>
public struct AnimalState
{
    public int Id;
    public AnimalSpecies Species;
    public int X;
    public int Y;
    public int Energy;
    public int Health;
    public int AgeTicks;
    public int TargetEntityId;
    public int TargetX;
    public int TargetY;
    public bool IsAlive;
    public bool CarcassCreated;
}

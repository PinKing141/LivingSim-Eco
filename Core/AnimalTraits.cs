namespace LivingSim.Core;

/// <summary>
/// Bounded inheritable traits. Higher values carry an energy trade-off:
/// speed, metabolism, vision, size, and fertility all raise upkeep.
/// </summary>
public struct AnimalTraits
{
    public int Speed;
    public int Metabolism;
    public int Vision;
    public int Size;
    public int Fertility;
}

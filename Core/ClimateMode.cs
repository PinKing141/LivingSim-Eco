namespace LivingSim.Core;

/// <summary>Dynamic production climate or controlled constant conditions for ecology gates.</summary>
public enum ClimateMode : byte
{
    Dynamic,
    MildConstant,
    DroughtConstant,
    AbundanceConstant,
}

namespace LivingSim.Core;

/// <summary>Compact, fixed-point resource state for one world cell.</summary>
public struct WorldCell
{
    public TerrainType Terrain;
    public BiomeType Biome;
    public int PlantBiomass;
    public int MaxPlantBiomass;
    public int BiomassRegenerationPerTick;
    /// <summary>Local grazing pressure, from 0 (recovered) to 1000 (depleted).</summary>
    public int HabitatPressure;
}

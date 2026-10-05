namespace LivingSim.Core;

public static class WorldGenerator
{
    public static World Generate(WorldSettings settings)
    {
        settings = settings.Validate();
        var cells = new WorldCell[checked(settings.Width * settings.Height)];

        for (var y = 0; y < settings.Height; y++)
        {
            for (var x = 0; x < settings.Width; x++)
            {
                cells[y * settings.Width + x] = CreateCell(settings.Seed, x, y);
            }
        }

        return new World(settings, cells);
    }

    private static WorldCell CreateCell(int seed, int x, int y)
    {
        var selection = DeterministicHash.At(seed, x, y, 0) % 100;
        return selection switch
        {
            < 12 => new WorldCell { Terrain = TerrainType.Water, Biome = BiomeType.Ocean },
            < 24 => NewLandCell(TerrainType.Mountain, BiomeType.Alpine, 200, 2, seed, x, y),
            < 42 => NewLandCell(TerrainType.Desert, BiomeType.Arid, 550, 4, seed, x, y),
            < 68 => NewLandCell(TerrainType.Forest, BiomeType.TemperateForest, 1_500, 12, seed, x, y),
            _ => NewLandCell(TerrainType.Plains, BiomeType.TemperateGrassland, 1_000, 9, seed, x, y),
        };
    }

    private static WorldCell NewLandCell(TerrainType terrain, BiomeType biome, int maximum, int regeneration, int seed, int x, int y)
    {
        var startingBiomass = maximum / 2 + (int)(DeterministicHash.At(seed, x, y, 1) % (uint)(maximum / 2 + 1));
        return new WorldCell
        {
            Terrain = terrain,
            Biome = biome,
            PlantBiomass = startingBiomass,
            MaxPlantBiomass = maximum,
            BiomassRegenerationPerTick = regeneration,
        };
    }
}

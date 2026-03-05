using System;
using System.Collections.Generic;
using LivingSim.Animals;

namespace LivingSim.World
{
    /// <summary>
    /// Represents a single cell (tile) in the world grid.
    /// Tracks terrain, resources, and residents.
    /// </summary>
    public class WorldCell
    {
        private const float MaxResource = 10f;
        private readonly Action<float, float, float>? _resourceDeltaCallback;

        // Terrain type
        public TerrainType Terrain { get; set; }
        public Biome Biome { get; set; }

        // Resource properties (read-only outside)
        public float Food { get; private set; }
        public float Water { get; private set; }
        public float Timber { get; private set; }
        public Guid? TerritoryOwnerId { get; set; } // Null if unclaimed, otherwise the GroupId of the owner
        public float TerritoryStrength { get; set; }
        public long LastTerritoryRefreshTick { get; set; }
        public List<Scent> Scents { get; } = new List<Scent>();
        public List<Animal> Residents { get; } = new List<Animal>();

        // Residents placeholder
        public bool HasResidents { get; set; }

        public WorldCell(TerrainType terrain = TerrainType.Plains, Action<float, float, float>? resourceDeltaCallback = null)
        {
            _resourceDeltaCallback = resourceDeltaCallback;
            Terrain = terrain;
            Biome = Biome.Plains; // Default biome
            Food = 0;
            Water = 0;
            Timber = 0;
            TerritoryOwnerId = null;
            TerritoryStrength = 0f;
            LastTerritoryRefreshTick = -1;
            HasResidents = false;
        }

        // --- Resource Modifiers ---
        public void AddFood(float amount)
        {
            float previous = Food;
            Food = Math.Clamp(Food + amount, 0, MaxResource);
            _resourceDeltaCallback?.Invoke(Food - previous, 0f, 0f);
        }

        public void AddWater(float amount)
        {
            float previous = Water;
            Water = Math.Clamp(Water + amount, 0, MaxResource);
            _resourceDeltaCallback?.Invoke(0f, Water - previous, 0f);
        }

        public void AddTimber(float amount)
        {
            float previous = Timber;
            Timber = Math.Clamp(Timber + amount, 0, MaxResource);
            _resourceDeltaCallback?.Invoke(0f, 0f, Timber - previous);
        }

        // --- Resource Consumption ---
        public float ConsumeFood(float amount)
        {
            float consumed = Math.Min(Food, amount);
            Food -= consumed;
            _resourceDeltaCallback?.Invoke(-consumed, 0f, 0f);
            return consumed;
        }

        public float ConsumeWater(float amount)
        {
            float consumed = Math.Min(Water, amount);
            Water -= consumed;
            _resourceDeltaCallback?.Invoke(0f, -consumed, 0f);
            return consumed;
        }

        public void DecayResources()
        {
            AddFood(-0.01f);
            AddWater(-0.01f);
            AddTimber(-0.005f);
        }
    }
}

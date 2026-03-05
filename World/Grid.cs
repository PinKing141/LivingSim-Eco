using System.Collections.Generic;
using LivingSim.Animals;

namespace LivingSim.World
{
    /// <summary>
    /// The world grid stores all cells in a 2D layout.
    /// </summary>
    public class Grid
    {
        private readonly WorldCell[,] _cells;

        public int Width { get; }
        public int Height { get; }

        public float TotalFood { get; private set; }
        public float TotalWater { get; private set; }
        public float TotalTimber { get; private set; }

        public Grid(int width, int height)
        {
            Width = width;
            Height = height;
            _cells = new WorldCell[Width, Height];

            // Initialize all cells
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    _cells[x, y] = new WorldCell(resourceDeltaCallback: OnCellResourceDelta);
                }
            }
        }

        // Access a cell safely
        public WorldCell? GetCell(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return null;
            return _cells[x, y];
        }

        public bool TryGetCell(int x, int y, out WorldCell? cell)
        {
            cell = GetCell(x, y);
            return cell != null;
        }

        // Enumerate all cells
        public IEnumerable<WorldCell> GetAllCells()
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    yield return _cells[x, y];
        }

        public void RegisterAnimal(Animal animal)
        {
            var cell = GetCell(animal.X, animal.Y);
            if (cell == null) return;
            cell.Residents.Add(animal);
            cell.HasResidents = cell.Residents.Count > 0;
        }

        public void MoveAnimal(Animal animal, int oldX, int oldY, int newX, int newY)
        {
            var oldCell = GetCell(oldX, oldY);
            if (oldCell != null)
            {
                oldCell.Residents.Remove(animal);
                oldCell.HasResidents = oldCell.Residents.Count > 0;
            }

            var newCell = GetCell(newX, newY);
            if (newCell != null)
            {
                newCell.Residents.Add(animal);
                newCell.HasResidents = true;
            }
        }

        public void UnregisterAnimal(Animal animal)
        {
            var cell = GetCell(animal.X, animal.Y);
            if (cell == null) return;
            cell.Residents.Remove(animal);
            cell.HasResidents = cell.Residents.Count > 0;
        }

        private void OnCellResourceDelta(float foodDelta, float waterDelta, float timberDelta)
        {
            TotalFood += foodDelta;
            TotalWater += waterDelta;
            TotalTimber += timberDelta;
        }
    }
}

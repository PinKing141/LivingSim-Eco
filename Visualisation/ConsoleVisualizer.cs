using System;
using System.Collections.Generic;
using LivingSim.Core;
using LivingSim.World;
using LivingSim.Animals;

namespace LivingSim.Visualisation
{
    public class ConsoleVisualizer
    {
        private readonly Dictionary<Guid, ConsoleColor> _groupColors = new Dictionary<Guid, ConsoleColor>();
        private (char character, ConsoleColor fg, ConsoleColor bg)[,]? _displayGrid;
        private int _displayGridWidth;
        private int _displayGridHeight;

        public void Draw(Grid grid, IReadOnlyList<Animal> animals, SimulationClock clock, List<Dictionary<Species, int>> history, bool showStats)
        {
            Console.SetCursorPosition(0, 0);

            if (showStats)
            {
                DrawStatistics(history, grid.Width * 2, grid.Height);
            }
            else
            {
                EnsureDisplayGrid(grid.Width, grid.Height);

                for (int y = 0; y < grid.Height; y++)
                {
                    for (int x = 0; x < grid.Width; x++)
                    {
                        var cell = grid.GetCell(x, y);
                        _displayGrid![x, y] = GetTerrainDisplay(cell, clock.CurrentTick);
                    }
                }

                for (int i = 0; i < animals.Count; i++)
                {
                    var animal = animals[i];
                    if (animal.IsAlive || animal.CarcassFoodValue <= 0) continue;
                    if (animal.X >= 0 && animal.X < grid.Width && animal.Y >= 0 && animal.Y < grid.Height)
                    {
                        var existingBg = _displayGrid![animal.X, animal.Y].bg;
                        _displayGrid[animal.X, animal.Y] = ('x', ConsoleColor.DarkGray, existingBg);
                    }
                }

                for (int i = 0; i < animals.Count; i++)
                {
                    var animal = animals[i];
                    if (!animal.IsAlive) continue;
                    if (animal.X >= 0 && animal.X < grid.Width && animal.Y >= 0 && animal.Y < grid.Height)
                    {
                        var (character, speciesColor) = GetAnimalDisplay(animal);
                        var existingBg = _displayGrid![animal.X, animal.Y].bg;
                        _displayGrid[animal.X, animal.Y] = (character, speciesColor, existingBg);
                    }
                }

                for (int y = 0; y < grid.Height; y++)
                {
                    for (int x = 0; x < grid.Width; x++)
                    {
                        var cell = _displayGrid![x, y];
                        var fg = cell.fg;
                        var bg = cell.bg;

                        if (fg == bg) fg = ConsoleColor.White;

                        Console.ForegroundColor = fg;
                        Console.BackgroundColor = bg;
                        Console.Write(cell.character + " ");
                    }
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.WriteLine();
                }
                Console.ResetColor();
            }

            int totalAnimals = 0;
            var speciesCounts = new Dictionary<Species, int>();
            for (int i = 0; i < animals.Count; i++)
            {
                var animal = animals[i];
                if (!animal.IsAlive) continue;
                totalAnimals++;
                speciesCounts.TryGetValue(animal.Species, out int count);
                speciesCounts[animal.Species] = count + 1;
            }

            string tickInfo = $"Tick: {clock.CurrentTick,-5} | Day: {clock.CurrentDay,-3} | Season: {clock.CurrentSeason,-6}".PadRight(Console.WindowWidth > 0 ? Console.WindowWidth - 1 : 80);
            var speciesParts = new List<string>();
            foreach (Species species in Enum.GetValues<Species>())
            {
                if (speciesCounts.TryGetValue(species, out int count))
                {
                    speciesParts.Add($"{species}: {count}");
                }
            }

            string popInfo = $"Population: {totalAnimals,-4} | " + string.Join(" | ", speciesParts);

            Console.WriteLine(tickInfo);
            Console.WriteLine(popInfo.PadRight(Console.WindowWidth > 0 ? Console.WindowWidth - 1 : 80));
        }

        private void EnsureDisplayGrid(int width, int height)
        {
            if (_displayGrid == null || _displayGridWidth != width || _displayGridHeight != height)
            {
                _displayGrid = new (char character, ConsoleColor fg, ConsoleColor bg)[width, height];
                _displayGridWidth = width;
                _displayGridHeight = height;
            }
        }

        private void DrawStatistics(List<Dictionary<Species, int>> history, int width, int height)
        {
            int maxPop = 0;
            if (history.Count > 0)
            {
                for (int i = 0; i < history.Count; i++)
                {
                    foreach (var kvp in history[i])
                    {
                        if (kvp.Value > maxPop) maxPop = kvp.Value;
                    }
                }
            }
            if (maxPop == 0) maxPop = 1;

            int count = history.Count;
            int startIndex = Math.Max(0, count - width);

            var buffer = new (char c, ConsoleColor color)[width, height];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    buffer[x, y] = (' ', ConsoleColor.White);

            for (int i = startIndex; i < count; i++)
            {
                int x = i - startIndex;
                if (x >= width) break;

                var tickStats = history[i];
                foreach (var kvp in tickStats)
                {
                    Species species = kvp.Key;
                    int pop = kvp.Value;

                    int scaledY = (int)(((float)pop / maxPop) * (height - 1));
                    int y = height - 1 - scaledY;

                    if (y >= 0 && y < height)
                    {
                        var (c, _) = GetSpeciesDisplay(species);
                        buffer[x, y] = (c, GetSpeciesGraphColor(species));
                    }
                }
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Console.ForegroundColor = buffer[x, y].color;
                    Console.Write(buffer[x, y].c);
                }
                Console.WriteLine();
            }
            Console.ResetColor();
            Console.WriteLine($"Graph Max Y: {maxPop}".PadRight(width));
        }

        private (char, ConsoleColor, ConsoleColor) GetTerrainDisplay(WorldCell cell, long currentTick)
        {
            var fg = cell.Terrain switch
            {
                TerrainType.Water => ConsoleColor.Blue,
                TerrainType.Mountain => ConsoleColor.Gray,
                TerrainType.Forest => cell.Food > 5 ? ConsoleColor.Green : ConsoleColor.DarkGreen,
                TerrainType.Plains => cell.Food > 3 ? ConsoleColor.Green : ConsoleColor.DarkYellow,
                TerrainType.Desert => ConsoleColor.Yellow,
                TerrainType.Tundra => ConsoleColor.Cyan,
                TerrainType.Wetlands => ConsoleColor.DarkCyan,
                TerrainType.River => ConsoleColor.Blue,
                _ => ConsoleColor.White,
            };

            var bg = ConsoleColor.Black;
            if (cell.TerritoryOwnerId.HasValue)
            {
                ConsoleColor groupColor = GetGroupColor(cell.TerritoryOwnerId.Value);
                bg = ToDarkColor(groupColor);
            }

            var ch = cell.Terrain switch
            {
                TerrainType.Water => '~',
                TerrainType.Mountain => '^',
                TerrainType.Forest => 'T',
                TerrainType.Plains => '.',
                TerrainType.Desert => '.',
                TerrainType.Tundra => '-',
                TerrainType.Wetlands => '"',
                TerrainType.River => '≈',
                _ => '?',
            };

            return (ch, fg, bg);
        }

        private (char, ConsoleColor) GetAnimalDisplay(Animal animal) => GetSpeciesDisplay(animal.Species);

        private (char, ConsoleColor) GetSpeciesDisplay(Species species) => species switch
        {
            Species.Rabbit => ('r', ConsoleColor.White),
            Species.Deer => ('D', ConsoleColor.White),
            Species.Boar => ('b', ConsoleColor.DarkYellow),
            Species.Bear => ('B', ConsoleColor.Yellow),
            Species.Fox => ('f', ConsoleColor.Red),
            Species.Wolf => ('W', ConsoleColor.DarkRed),
            _ => ('?', ConsoleColor.Magenta),
        };

        private ConsoleColor GetSpeciesGraphColor(Species species) => species switch
        {
            Species.Rabbit => ConsoleColor.Cyan,
            Species.Deer => ConsoleColor.Green,
            Species.Boar => ConsoleColor.DarkYellow,
            Species.Bear => ConsoleColor.Yellow,
            Species.Fox => ConsoleColor.Red,
            Species.Wolf => ConsoleColor.DarkRed,
            _ => ConsoleColor.Magenta,
        };

        private ConsoleColor GetGroupColor(Guid groupId)
        {
            if (!_groupColors.TryGetValue(groupId, out ConsoleColor color))
            {
                ConsoleColor[] palette = {
                    ConsoleColor.Cyan, ConsoleColor.Yellow, ConsoleColor.Red,
                    ConsoleColor.Green, ConsoleColor.Magenta, ConsoleColor.White
                };
                color = palette[_groupColors.Count % palette.Length];
                _groupColors[groupId] = color;
            }
            return color;
        }

        private ConsoleColor ToDarkColor(ConsoleColor color) => color switch
        {
            ConsoleColor.White => ConsoleColor.DarkGray,
            ConsoleColor.Cyan => ConsoleColor.DarkCyan,
            ConsoleColor.Yellow => ConsoleColor.DarkYellow,
            ConsoleColor.Red => ConsoleColor.DarkRed,
            ConsoleColor.Green => ConsoleColor.DarkGreen,
            ConsoleColor.Magenta => ConsoleColor.DarkMagenta,
            ConsoleColor.Blue => ConsoleColor.DarkBlue,
            _ => color,
        };
    }
}

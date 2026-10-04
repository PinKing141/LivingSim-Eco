namespace LivingSim.Core;

public sealed class World
{
    private readonly WorldCell[] _cells;

    internal World(WorldSettings settings, WorldCell[] cells)
    {
        Settings = settings;
        _cells = cells;
    }

    public WorldSettings Settings { get; }
    public int Width => Settings.Width;
    public int Height => Settings.Height;
    public int CellCount => _cells.Length;
    public ReadOnlySpan<WorldCell> Cells => _cells;

    public ref WorldCell CellAt(int x, int y)
    {
        if ((uint)x >= (uint)Width || (uint)y >= (uint)Height)
        {
            throw new ArgumentOutOfRangeException($"({x}, {y}) is outside the world.");
        }

        return ref _cells[y * Width + x];
    }

    internal Span<WorldCell> MutableCells => _cells;
}

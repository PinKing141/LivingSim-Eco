namespace LivingSim.Core;

public sealed record WorldSettings(int Seed, int Width, int Height)
{
    public WorldSettings Validate()
    {
        if (Width is < 1 or > 4_096)
        {
            throw new ArgumentOutOfRangeException(nameof(Width), "World width must be between 1 and 4096.");
        }

        if (Height is < 1 or > 4_096)
        {
            throw new ArgumentOutOfRangeException(nameof(Height), "World height must be between 1 and 4096.");
        }

        return this;
    }
}

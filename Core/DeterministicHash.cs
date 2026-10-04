namespace LivingSim.Core;

internal static class DeterministicHash
{
    public static uint At(int seed, int x, int y, uint stream)
    {
        uint value = unchecked((uint)seed);
        value ^= unchecked((uint)x) * 0x9E3779B9u;
        value ^= unchecked((uint)y) * 0x85EBCA6Bu;
        value ^= stream * 0xC2B2AE35u;
        value ^= value >> 16;
        value *= 0x7FEB352Du;
        value ^= value >> 15;
        value *= 0x846CA68Bu;
        value ^= value >> 16;
        return value;
    }
}

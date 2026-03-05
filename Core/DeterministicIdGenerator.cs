using System;
using System.Buffers.Binary;
using System.Security.Cryptography;

namespace LivingSim.Core
{
    /// <summary>
    /// Generates deterministic GUID values from a world seed, simulation tick, and call sequence.
    /// </summary>
    public sealed class DeterministicIdGenerator
    {
        private readonly int _worldSeed;
        private long _sequence;

        public DeterministicIdGenerator(int worldSeed)
        {
            _worldSeed = worldSeed;
            _sequence = 0;
        }

        public Guid NextGuid(long tick)
        {
            long sequence = _sequence++;

            Span<byte> input = stackalloc byte[20];
            BinaryPrimitives.WriteInt32LittleEndian(input.Slice(0, 4), _worldSeed);
            BinaryPrimitives.WriteInt64LittleEndian(input.Slice(4, 8), tick);
            BinaryPrimitives.WriteInt64LittleEndian(input.Slice(12, 8), sequence);

            Span<byte> hash = stackalloc byte[32];
            SHA256.HashData(input, hash);

            Span<byte> guidBytes = stackalloc byte[16];
            hash.Slice(0, 16).CopyTo(guidBytes);
            return new Guid(guidBytes);
        }
    }
}

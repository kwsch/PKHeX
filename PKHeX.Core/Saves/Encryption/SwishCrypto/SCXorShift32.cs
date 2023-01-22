using System.Runtime.CompilerServices;

namespace PKHeX.Core;

/// <summary>
/// Self-mutating value that returns a crypto value to be xor-ed with another (unaligned) byte stream.
/// <para>
/// This implementation allows for yielding crypto bytes on demand.
/// </para>
/// </summary>
public ref struct SCXorShift32
{
    private int Counter;
    private uint Seed;

    public SCXorShift32(uint seed) => Seed = GetInitialSeed(seed);

    private static uint GetInitialSeed(uint seed)
    {
        var pop_count = System.Numerics.BitOperations.PopCount(seed);
        for (var i = 0; i < pop_count; i++)
            seed = XorshiftAdvance(seed);
        return seed;
    }

    /// <summary>
    /// Gets a <see cref="byte"/> from the current state.
    /// </summary>
    public uint Next()
    {
        var c = Counter;
        var result = (Seed >> (c << 3)) & 0xFF;
        if (c == 3)
        {
            Seed = XorshiftAdvance(Seed);
            Counter = 0;
        }
        else
        {
            ++Counter;
        }
        return result;
    }

    /// <summary>
    /// Gets a <see cref="uint"/> from the current state.
    /// </summary>
    public uint Next32()
    {
        return Next() | (Next() << 8) | (Next() << 16) | (Next() << 24);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint XorshiftAdvance(uint key)
    {
        key ^= key << 2;
        key ^= key >> 15;
        key ^= key << 13;
        return key;
    }
}

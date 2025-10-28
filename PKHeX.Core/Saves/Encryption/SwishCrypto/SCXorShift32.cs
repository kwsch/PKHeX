using System.Runtime.CompilerServices;

namespace PKHeX.Core;

/// <summary>
/// Self-mutating value that returns a crypto value to be xor-ed with another (unaligned) byte stream.
/// <para>
/// This implementation allows for yielding crypto bytes on demand.
/// </para>
/// </summary>
public ref struct SCXorShift32(uint seed)
{
    private int Counter;
    private uint State = GetInitialState(seed);

    private static uint GetInitialState(uint state)
    {
        var pop_count = System.Numerics.BitOperations.PopCount(state);
        for (var i = 0; i < pop_count; i++)
            state = XorshiftAdvance(state);
        return state;
    }

    /// <summary>
    /// Gets a <see cref="byte"/> from the current state.
    /// </summary>
    public byte Next()
    {
        var c = Counter;
        var result = (byte)(State >> (c << 3));
        if (c == 3)
        {
            State = XorshiftAdvance(State);
            Counter = 0;
        }
        else
        {
            ++Counter;
        }
        return result;
    }

    /// <summary>
    /// Gets a <see cref="int"/> from the current state.
    /// </summary>
    public int Next32() => Next() | (Next() << 8) | (Next() << 16) | (Next() << 24);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint XorshiftAdvance(uint state)
    {
        state ^= state << 2;
        state ^= state >> 15;
        state ^= state << 13;
        return state;
    }
}

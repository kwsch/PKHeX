using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Finds the index of the most recent save block for <see cref="SAV3"/> blocks.
/// </summary>
public static class SAV3BlockDetection
{
    private const int First = 0;
    private const int Second = 1;
    private const int Same = 2;

    /// <summary>
    /// Compares the footers of the two blocks to determine which is newest.
    /// </summary>
    /// <returns>0=Primary, 1=Secondary.</returns>
    public static int CompareFooters(ReadOnlySpan<byte> data, int offset1, int offset2)
    {
        const int ofsCounter = 0x0FFC; // last 4 bytes of a 0x1000 chunk
        var counter1 = ReadUInt32LittleEndian(data[(offset1 + ofsCounter)..]);
        var counter2 = ReadUInt32LittleEndian(data[(offset2 + ofsCounter)..]);
        var result = CompareCounters(counter1, counter2);
        return result == Second ? Second : First; // Same -> First, shouldn't happen for valid saves.
    }

    private static int CompareCounters(uint counter1, uint counter2)
    {
        // Uninitialized -- only continue if a rollover case (humanly impossible)
        if (counter1 == uint.MaxValue && counter2 != uint.MaxValue - 1)
            return Second;
        if (counter2 == uint.MaxValue && counter1 != uint.MaxValue - 1)
            return First;

        // Different
        if (counter1 > counter2)
            return First;
        if (counter1 < counter2)
            return Second;

        return Same;
    }
}

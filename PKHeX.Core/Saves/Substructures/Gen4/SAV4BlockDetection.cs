using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Finds the index of the most recent save block for <see cref="SAV4"/> blocks.
/// </summary>
public static class SAV4BlockDetection
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
        // Major Counters
        var major1 = ReadUInt32LittleEndian(data[offset1..]);
        var major2 = ReadUInt32LittleEndian(data[offset2..]);
        var result1 = CompareCounters(major1, major2);
        if (result1 != Same)
            return result1;

        // Minor Counters
        var minor1 = ReadUInt32LittleEndian(data[(offset1 + 4)..]);
        var minor2 = ReadUInt32LittleEndian(data[(offset2 + 4)..]);
        var result2 = CompareCounters(minor1, minor2);
        return result2 == Second ? Second : First; // Same -> First, shouldn't happen for valid saves.
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


    /// <summary>
    /// Compares two extra blocks to determine which is newest.
    /// </summary>
    /// <returns>0=Primary, 1=Secondary, -1=Uninitialized.</returns>
    public static int CompareExtra(ReadOnlySpan<byte> data1, ReadOnlySpan<byte> data2, BlockInfo4 block, uint? key)
    {
        // The Hall of Fame block uses a counter in the footer to determine which copy is used.
        // Entering the Hall of Fame overwrites both copies with the new data.
        if (block.ID == 0) 
        {
            var rev1 = block.GetRevision(data1);
            var rev2 = block.GetRevision(data2);
            if (rev1 != 0xFFFFFFFF && (rev1 >= rev2 || rev2 == 0xFFFFFFFF))
                return First;
            if (rev2 != 0xFFFFFFFF && (rev2 > rev1 || rev1 == 0xFFFFFFFF))
                return Second;
            return -1; // Uninitialized
        }

        // The Battle Hall/Battle Videos use a key in the General block to determine which copy is used.
        // The key for the previous copy is also stored in the General block.
        var key1 = block.GetKey(data1);
        var key2 = block.GetKey(data2);
        if (key != 0xFFFFFFFF && key == key1)
            return First;
        if (key != 0xFFFFFFFF && key == key2)
            return Second;
        return -1; // Uninitialized
    }
}

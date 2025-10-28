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
    public static int CompareExtra(ReadOnlySpan<byte> data1, ReadOnlySpan<byte> data2, BlockInfo4 block)
    {
        // The Hall of Fame block uses a counter in the footer to determine which copy is used.
        // Entering the Hall of Fame overwrites both copies with the new data.
        var rev1 = block.GetRevision(data1);
        var rev2 = block.GetRevision(data2);
        var valid1 = rev1 != 0xFFFFFFFF && block.SizeValid(data1);
        var valid2 = rev2 != 0xFFFFFFFF && block.SizeValid(data2);
        if (valid1 && (rev1 >= rev2 || !valid2))
            return First;
        if (valid2 && (rev2 > rev1 || !valid1))
            return Second;
        return -1; // Uninitialized
    }

    public static int CompareExtra(ReadOnlySpan<byte> data1, ReadOnlySpan<byte> data2, BlockInfo4 block, uint key, uint keyBackup, byte prefer)
    {
        // The Battle Hall/Battle Videos use a key in the General block to check if the block is valid.
        // If the key is 0xFFFFFFFF, the block is uninitialized or deleted.
        // Which partition is checked first is determined by a byte in the General block.
        var key1 = block.GetKey(data1);
        var key2 = block.GetKey(data2);
        var valid1 = key1 != 0xFFFFFFFF && block.SizeValid(data1);
        var valid2 = key2 != 0xFFFFFFFF && block.SizeValid(data2);
        if (prefer == First)
        {
            if (valid1 && key == key1)
                return First;
            if (valid2 && key == key2)
                return Second;
            if (valid1 && keyBackup == key1)
                return First;
            if (valid2 && keyBackup == key2)
                return Second;
        }
        else
        {
            if (valid2 && key == key2)
                return Second;
            if (valid1 && key == key1)
                return First;
            if (valid2 && keyBackup == key2)
                return Second;
            if (valid1 && keyBackup == key1)
                return First;
        }
        return -1; // Uninitialized
    }
}

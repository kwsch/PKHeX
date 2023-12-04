using System;
using static System.Buffers.Binary.BinaryPrimitives;
using static PKHeX.Core.PokeCrypto;

namespace PKHeX.Core;

public static class EntityDetection
{
    /// <summary>
    /// Determines if the given length is valid for a <see cref="PKM"/>.
    /// </summary>
    /// <param name="len">Data length of the file/array.</param>
    /// <returns>A <see cref="bool"/> indicating whether the length is valid for a <see cref="PKM"/>.</returns>
    public static bool IsSizePlausible(long len) => len is
        SIZE_1JLIST or SIZE_1ULIST or
        SIZE_2ULIST or SIZE_2JLIST or SIZE_2STADIUM or
        SIZE_3STORED or SIZE_3PARTY or
        SIZE_3CSTORED or SIZE_3XSTORED or
        SIZE_4STORED or SIZE_4PARTY or SIZE_4RSTORED or
        SIZE_5PARTY or
        SIZE_6STORED or SIZE_6PARTY or
        SIZE_8STORED or SIZE_8PARTY or
        SIZE_8ASTORED or SIZE_8APARTY or
        SIZE_9STORED or SIZE_9PARTY
        ;

    public static bool IsPresentGB(ReadOnlySpan<byte> data) => data[0] != 0; // Species non-zero
    public static bool IsPresentGC(ReadOnlySpan<byte> data) => ReadUInt16BigEndian(data) != 0; // Species non-zero
    public static bool IsPresentGBA(ReadOnlySpan<byte> data) => (data[0x13] & 0xFB) == 2; // ignore egg flag, must be FlagHasSpecies.
    public static bool IsPresentSAV4Ranch(ReadOnlySpan<byte> data) => ReadUInt32LittleEndian(data) != 0 && ReadUInt32BigEndian(data) != 0x28; // Species non-zero, ignore file end marker

    public static bool IsPresent(ReadOnlySpan<byte> data)
    {
        if (ReadUInt32LittleEndian(data) != 0) // PID
            return true;
        ushort species = ReadUInt16LittleEndian(data[8..]);
        return species != 0;
    }

    /// <summary>
    /// Gets a function that can check a byte array (at an offset) to see if a <see cref="PKM"/> is possibly present.
    /// </summary>
    /// <param name="blank"></param>
    /// <returns>Function that checks if a byte array (at an offset) has a <see cref="PKM"/> present</returns>
    public static Func<byte[], bool> GetFuncIsPresent(PKM blank)
    {
        if (blank.Format >= 4)
            return x => IsPresent(x);
        if (blank.Format <= 2)
            return x => IsPresentGB(x);
        if (blank.Data.Length <= SIZE_3PARTY)
            return x => IsPresentGBA(x);
        return x => IsPresentGC(x);
    }
}

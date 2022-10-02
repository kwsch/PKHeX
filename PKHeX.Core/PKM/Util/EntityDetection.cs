using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public static class EntityDetection
{
    private static readonly HashSet<int> Sizes = new()
    {
        PokeCrypto.SIZE_1JLIST,   PokeCrypto.SIZE_1ULIST,
        PokeCrypto.SIZE_2ULIST,   PokeCrypto.SIZE_2JLIST,   PokeCrypto.SIZE_2STADIUM,
        PokeCrypto.SIZE_3STORED,  PokeCrypto.SIZE_3PARTY,
        PokeCrypto.SIZE_3CSTORED, PokeCrypto.SIZE_3XSTORED,
        PokeCrypto.SIZE_4STORED,  PokeCrypto.SIZE_4PARTY,   PokeCrypto.SIZE_4RSTORED,
        PokeCrypto.SIZE_5PARTY,
        PokeCrypto.SIZE_6STORED,  PokeCrypto.SIZE_6PARTY,
        PokeCrypto.SIZE_8STORED,  PokeCrypto.SIZE_8PARTY,
        PokeCrypto.SIZE_8ASTORED, PokeCrypto.SIZE_8APARTY,
    };

    /// <summary>
    /// Determines if the given length is valid for a <see cref="PKM"/>.
    /// </summary>
    /// <param name="len">Data length of the file/array.</param>
    /// <returns>A <see cref="bool"/> indicating whether or not the length is valid for a <see cref="PKM"/>.</returns>
    public static bool IsSizePlausible(long len) => Sizes.Contains((int)len);

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
        if (blank.Data.Length <= PokeCrypto.SIZE_3PARTY)
            return x => IsPresentGBA(x);
        return x => IsPresentGC(x);
    }
}

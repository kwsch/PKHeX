using System;
using static System.Buffers.Binary.BinaryPrimitives;
using static PKHeX.Core.PokeCrypto;

namespace PKHeX.Core;

/// <summary>
/// Logic for detecting a <see cref="PKM"/> entity from a byte array or length.
/// </summary>
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
        SIZE_8ASTORED or SIZE_8APARTY
        ;

    /// <summary>
    /// Checks the first byte of the span to see if the species is non-zero.
    /// </summary>
    public static bool IsPresentGB(ReadOnlySpan<byte> data) => data[0] != 0;

    /// <summary>
    /// Checks the first two bytes of the span to see if the species is non-zero.
    /// </summary>
    public static bool IsPresentGC(ReadOnlySpan<byte> data) => ReadUInt16BigEndian(data) != 0;

    /// <summary>
    /// Checks the flag status of the span to see if it has the <see cref="PK3.FlagHasSpecies"/> indicator.
    /// </summary>
    public static bool IsPresentGBA(ReadOnlySpan<byte> data) => (data[0x13] & 0xFB) == 2; // ignore egg flag, must be FlagHasSpecies.

    /// <summary>
    /// Checks if the species is non-zero and the entity PID is not the "file end" marker of the save file.
    /// </summary>
    /// <remarks>
    /// Only useful when called from a <see cref="SAV4Ranch"/> file, not for a <see cref="PK4"/> entity dump.
    /// </remarks>
    public static bool IsPresentSAV4Ranch(ReadOnlySpan<byte> data) => IsPresent(data) && ReadUInt32BigEndian(data) != 0x28; // Species non-zero, ignore file end marker

    public static bool IsPresent(ReadOnlySpan<byte> data)
    {
        if (ReadUInt32LittleEndian(data) != 0) // PID
            return true;
        ushort species = ReadUInt16LittleEndian(data[8..]);
        return species != 0;
    }

    /// <summary>
    /// Gets a function that can check a span to see if a <see cref="PKM"/> is possibly present.
    /// </summary>
    /// <param name="blank"></param>
    /// <returns>Function that checks if a span has a <see cref="PKM"/> present</returns>
    public static Func<ReadOnlySpan<byte>, bool> GetFuncIsPresent(PKM blank) => blank switch
    {
        { Format: >= 4 } => IsPresent,
        { Format: <= 2 } => IsPresentGB,

        // Gen3; ^above handles all other formats.
        PK3 => IsPresentGBA,
        _ => IsPresentGC,
    };
}

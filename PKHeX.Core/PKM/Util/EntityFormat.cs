using System;
using static PKHeX.Core.PokeCrypto;
using static PKHeX.Core.EntityFormatDetected;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public static class EntityFormat
{
    /// <summary>
    /// Gets the generation of the Pokémon data.
    /// </summary>
    /// <param name="data">Raw data representing a Pokémon.</param>
    /// <returns>Enum indicating the generation of the PKM file, or <see cref="None"/> if the data is invalid.</returns>
    public static EntityFormatDetected GetFormat(ReadOnlySpan<byte> data)
    {
        if (!EntityDetection.IsSizePlausible(data.Length))
            return None;

        return GetFormatInternal(data);
    }

    private static EntityFormatDetected GetFormatInternal(ReadOnlySpan<byte> data) => data.Length switch
    {
        SIZE_1JLIST or SIZE_1ULIST    => FormatPK1,
        SIZE_2JLIST or SIZE_2ULIST    => FormatPK2,
        SIZE_2STADIUM                 => FormatSK2,
        SIZE_3PARTY or SIZE_3STORED   => FormatPK3,
        SIZE_3CSTORED                 => FormatCK3,
        SIZE_3XSTORED                 => FormatXK3,
        SIZE_4PARTY or SIZE_4STORED   => GetFormat45(data),
        SIZE_4RSTORED                 => FormatRK4,
        SIZE_5PARTY                   => FormatPK5,
        SIZE_6STORED                  => GetFormat67(data),
        SIZE_6PARTY                   => GetFormat67_PGT(data),
        SIZE_8PARTY  or SIZE_8STORED  => GetFormat89(data),
        SIZE_8APARTY or SIZE_8ASTORED => FormatPA8,
        _ => None,
    };

    private static EntityFormatDetected GetFormat67_PGT(ReadOnlySpan<byte> data)
    {
        if (!IsFormat67(data))
            return None; // PGT collision, same size.
        return GetFormat67(data);
    }

    private static bool IsFormat67(ReadOnlySpan<byte> data)
    {
        if (ReadUInt16LittleEndian(data[0x04..]) != 0) // Bad Sanity?
            return false; // PGT with non-zero ItemID

        // PGT files have the last 0x10 bytes 00; PK6/etc. will have data here.
        if (data[..^0x10].ContainsAnyExcept<byte>(0))
            return true;

        if (ReadUInt16LittleEndian(data[0x06..]) == Checksums.Add16(data[8..SIZE_6STORED]))
            return true; // decrypted
        return false;
    }

    // assumes decrypted state
    private static EntityFormatDetected GetFormat45(ReadOnlySpan<byte> data)
    {
        if (data.Length == SIZE_4RSTORED)
            return FormatRK4;
        if (ReadUInt16LittleEndian(data[0x4..]) != 0)
            return FormatBK4; // BK4 non-zero sanity
        if (data[0x5F] < 0x10 && ReadUInt16LittleEndian(data[0x80..]) < 0x3333)
            return FormatPK4; // Gen3/4 version origin, not Transporter
        if (ReadUInt16LittleEndian(data[0x46..]) != 0)
            return FormatPK4; // PK4.MetLocationExtended (unused in PK5)
        return FormatPK5;
    }

    /// <summary>
    /// Checks if the input <see cref="PK6"/> file is really a <see cref="PK7"/>.
    /// </summary>
    private static EntityFormatDetected GetFormat67(ReadOnlySpan<byte> data)
    {
        var pk = new PK6(data.ToArray());
        return IsFormatReally7(pk);
    }

    /// <summary>
    /// Checks if the input <see cref="PK8"/> file is really a <see cref="PB8"/>.
    /// </summary>
    private static EntityFormatDetected GetFormat89(ReadOnlySpan<byte> data)
    {
        var pk = new PK8(data.ToArray());
        if (IsFormatReally9(pk))
            return FormatPK9;
        return IsFormatReally8b(pk);
    }

    private static bool IsFormatReally9(PK8 pk)
    {
        // PK8: Unused Alignment, PK9: Obedience Level
        if (pk.Data[0x11F] != 0)
            return true;
        // PK8: Version, PK9: Unused -- Version relocated to 0xCE
        return pk.Data[0xDE] == 0;
        // No need to check for other usages being different.
    }

    /// <summary>
    /// Creates an instance of <see cref="PKM"/> from the given data.
    /// </summary>
    /// <param name="data">Raw data of the Pokémon file.</param>
    /// <param name="prefer">Optional identifier for the preferred generation.  Usually the generation of the destination save file.</param>
    /// <returns>An instance of <see cref="PKM"/> created from the given <paramref name="data"/>, or null if <paramref name="data"/> is invalid.</returns>
    public static PKM? GetFromBytes(byte[] data, EntityContext prefer = EntityContext.None)
    {
        var format = GetFormat(data);
        return GetFromBytes(data, format, prefer);
    }

    private static PKM? GetFromBytes(byte[] data, EntityFormatDetected format, EntityContext prefer) => format switch
    {
        FormatPK1 => PokeList1.ReadFromSingle(data),
        FormatPK2 => PokeList2.ReadFromSingle(data),
        FormatSK2 => new SK2(data),
        FormatPK3 => new PK3(data),
        FormatCK3 => new CK3(data),
        FormatXK3 => new XK3(data),
        FormatPK4 => new PK4(data),
        FormatBK4 => new BK4(data),
        FormatRK4 => new RK4(data),
        FormatPK5 => new PK5(data),
        FormatPK6 => new PK6(data),
        FormatPK7 => new PK7(data),
        FormatPB7 => new PB7(data),
        FormatPK8 => new PK8(data),
        FormatPA8 => new PA8(data),
        FormatPB8 => new PB8(data),
        Format6or7 => prefer == EntityContext.Gen6 ? new PK6(data) : new PK7(data),
        Format8or8b => prefer == EntityContext.Gen8b ? new PB8(data) : new PK8(data),
        FormatPK9 => new PK9(data),
        _ => null,
    };

    /// <summary>
    /// Checks if the input PK6 file is really a PK7.
    /// </summary>
    /// <param name="pk">PK6 to check</param>
    /// <returns>Boolean is a PK7</returns>
    private static EntityFormatDetected IsFormatReally7(PK6 pk)
    {
        if (pk.Version > Legal.MaxGameID_6)
        {
            if (pk.Version is (GameVersion.GP or GameVersion.GE or GameVersion.GO))
                return FormatPB7;
            return FormatPK7;
        }

        // Check Ranges
        if (pk.Species > Legal.MaxSpeciesID_6)
            return FormatPK7;

        const int maxMove = Legal.MaxMoveID_6_AO;
        if (pk.Move1 > maxMove || pk.Move2 > maxMove || pk.Move3 > maxMove || pk.Move4 > maxMove)
            return FormatPK7;
        if (pk.RelearnMove1 > maxMove || pk.RelearnMove2 > maxMove || pk.RelearnMove3 > maxMove || pk.RelearnMove4 > maxMove)
            return FormatPK7;
        if (pk.Ability > Legal.MaxAbilityID_6_AO)
            return FormatPK7;
        if (pk.HeldItem > Legal.MaxItemID_6_AO)
            return FormatPK7;

        // Ground Tile property is replaced with Hyper Training PK6->PK7
        var et = pk.GroundTile;
        if (et != 0)
        {
            if (pk.CurrentLevel < 100) // can't be hyper trained
                return FormatPK6;

            if (!pk.Gen4) // can't have GroundTile
                return FormatPK7;
            if (et > GroundTileType.Max_Pt) // invalid Gen4 GroundTile
                return FormatPK7;
        }

        int mb = ReadUInt16LittleEndian(pk.Data.AsSpan(0x16));
        if (mb > 0xAAA)
            return FormatPK6;
        for (int i = 0; i < 6; i++)
        {
            if (((mb >> (i << 1)) & 3) == 3) // markings are 10 or 01 (or 00), never 11
                return FormatPK6;
        }

        if (pk.Data[0x2A] > 20) // ResortEventStatus is always < 20
            return FormatPK6;

        return Format6or7;
    }

    private static EntityFormatDetected IsFormatReally8b(PK8 pk)
    {
        if (pk.Species > Legal.MaxSpeciesID_4)
            return FormatPK8;
        if (pk.DynamaxLevel != 0)
            return FormatPK8;
        if (pk.CanGigantamax)
            return FormatPK8;

        return Format8or8b;
    }
}

public enum EntityFormatDetected
{
    None = -1,

    FormatPK1,
    FormatPK2, FormatSK2,
    FormatPK3, FormatCK3, FormatXK3,
    FormatPK4, FormatBK4, FormatRK4, FormatPK5,
    FormatPK6, FormatPK7, FormatPB7,
    FormatPK8, FormatPA8, FormatPB8,
    FormatPK9,

    Format6or7,
    Format8or8b,
}

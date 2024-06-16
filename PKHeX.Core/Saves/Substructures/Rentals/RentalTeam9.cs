using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// A single Generation 9 Rental Team
/// </summary>
public sealed class RentalTeam9(byte[] Data) : IRentalTeam<PK9>, IPokeGroup
{
    private const int LEN_OT = 11; // char
    private const int LEN_TEAMNAME = 10; // char

    private const int LEN_META = sizeof(ushort) + (sizeof(char) * (LEN_OT + LEN_TEAMNAME)) + sizeof(uint);
    private const int LEN_STORED = PokeCrypto.SIZE_8STORED; // 0x148
    private const int LEN_POKE = PokeCrypto.SIZE_8PARTY; // 0x158
    private const int LEN_PARTYSTAT = LEN_POKE - PokeCrypto.SIZE_8STORED; // 0x10
    private const int COUNT_POKE = 6;

    private const int OFS_META = 0;
    private const int OFS_1 = OFS_META + LEN_META;
    private const int OFS_2 = OFS_1 + LEN_POKE;
    private const int OFS_3 = OFS_2 + LEN_POKE;
    private const int OFS_4 = OFS_3 + LEN_POKE;
    private const int OFS_5 = OFS_4 + LEN_POKE;
    private const int OFS_6 = OFS_5 + LEN_POKE;
    private const int OFS_END = OFS_6 + LEN_POKE;
    public const int SIZE = OFS_END + sizeof(uint); // 0x844

    public readonly byte[] Data = Data;

    // 2 bytes number
    public ushort ID
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(OFS_META + 0x00));
        set => WriteUInt16LittleEndian(Data.AsSpan(OFS_META + 0x00), value);
    }

    private Span<byte> OriginalTrainerTrash => Data.AsSpan(OFS_META + 0x02, LEN_OT * sizeof(char));

    private Span<byte> TeamNameTrash => Data.AsSpan(OFS_META + 0x18, LEN_TEAMNAME * sizeof(char));

    public uint Language
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(OFS_META + 0x2C));
        set => WriteUInt32LittleEndian(Data.AsSpan(OFS_META + 0x2C), value);
    }

    public string PlayerName
    {
        get => StringConverter8.GetString(OriginalTrainerTrash);
        set => StringConverter8.SetString(OriginalTrainerTrash, value, 10);
    }

    public string TeamName
    {
        get => StringConverter8.GetString(TeamNameTrash);
        set => StringConverter8.SetString(TeamNameTrash, value, LEN_TEAMNAME);
    }

    public uint EntityCount
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(OFS_END));
        set => WriteUInt32LittleEndian(Data.AsSpan(OFS_END), value);
    }

    public PK9 GetSlot(int slot)
    {
        var ofs = GetSlotOffset(slot);
        var data = Data.AsSpan(ofs, LEN_POKE);
        var pk9 = new PK9(data.ToArray());
        pk9.ResetPartyStats();
        return pk9;
    }

    public void SetSlot(int slot, PK9 pk)
    {
        var ofs = GetSlotOffset(slot);
        var data = pk.EncryptedPartyData;
        // Wipe Party Stats
        Array.Clear(data, LEN_STORED, LEN_PARTYSTAT);
        data.CopyTo(Data, ofs);
    }

    public PK9[] GetTeam()
    {
        var team = new PK9[COUNT_POKE];
        GetTeam(team);
        return team;
    }

    public void GetTeam(Span<PK9> team)
    {
        for (int i = 0; i < team.Length; i++)
            team[i] = GetSlot(i);
    }

    public void SetTeam(ReadOnlySpan<PK9> team)
    {
        for (int i = 0; i < team.Length; i++)
            SetSlot(i, team[i]);
    }

    public static int GetSlotOffset(int slot)
    {
        if ((uint)slot >= COUNT_POKE)
            throw new ArgumentOutOfRangeException(nameof(slot));
        return OFS_1 + (LEN_POKE * slot);
    }

    public IEnumerable<PKM> Contents => GetTeam();

    public static bool IsRentalTeam(byte[] data)
    {
        if (data.Length != SIZE)
            return false;
        var team = new RentalTeam9(data);
        for (int i = 0; i < 6; i++)
        {
            // Checksum can be invalid for whatever reason. Just sanity check a little.
            var pk = team.GetSlot(i);
            if (pk.Species == 0)
                continue;
            if (pk.Species > pk.MaxSpeciesID)
                return false;
            if (pk.Move1 > pk.MaxMoveID || pk.Move2 > pk.MaxMoveID || pk.Move3 > pk.MaxMoveID || pk.Move4 > pk.MaxMoveID)
                return false;
        }
        return true;
    }

    public static RentalTeam9 GetFrom(ReadOnlySpan<byte> data, int index)
    {
        var ofs = index * SIZE;
        var team = data.Slice(ofs, SIZE).ToArray();
        return new RentalTeam9(team);
    }

    public void WriteTo(Span<byte> data, int index) => Data.CopyTo(data[(index * SIZE)..]);

    private Span<byte> CheckSpan => Data.AsSpan(OFS_1, 6 * LEN_POKE);

    /// <summary>
    /// Simple checksum to detect team duplication.
    /// </summary>
    public ushort EntityChecksum => Checksums.CRC16_CCITT(CheckSpan);
}

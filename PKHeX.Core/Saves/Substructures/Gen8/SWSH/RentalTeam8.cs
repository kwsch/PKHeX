using System;
using System.Collections.Generic;
using System.Linq;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Container block for Generation 8 saved Rental Teams
/// </summary>
public sealed class RentalTeam8 : IRentalTeam<PK8>, IPokeGroup
{
    public const int SIZE = 0x880;

    private const int LEN_META = 8 + 0x1C + 2 + 0x16 + 0x1A;
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
    private const int POST_META = OFS_6 + LEN_POKE; // 0x866

    private readonly byte[] Data;

    public RentalTeam8(byte[] data) => Data = data;

    public ulong ID { get => ReadUInt64LittleEndian(GetMetadataStart()); set => WriteUInt64LittleEndian(GetMetadataStart(), value); }
    public string TeamID { get => StringConverter8.GetString(GetMetadataStart().Slice(8, 0x1C)); set => StringConverter8.SetString(GetMetadataStart().Slice(8, 0x1C), value.AsSpan(), 0x1C / 2); }
    // 2 unused bytes, probably null terminator for TeamID
    public string TeamName { get => StringConverter8.GetString(GetMetadataStart().Slice(0x26, 0x16)); set => StringConverter8.SetString(GetMetadataStart().Slice(0x26, 0x16), value.AsSpan(), 0x16 / 2); }
    public string PlayerName { get => StringConverter8.GetString(GetMetadataStart().Slice(0x3C, 0x1A)); set => StringConverter8.SetString(GetMetadataStart().Slice(0x3C, 0x1A), value.AsSpan(), 0x1A / 2); }

    public PK8 GetSlot(int slot)
    {
        var ofs = GetSlotOffset(slot);
        var data = Data.Slice(ofs, LEN_POKE);
        var pk8 = new PK8(data);
        pk8.ResetPartyStats();
        return pk8;
    }

    public void SetSlot(int slot, PK8 pk)
    {
        var ofs = GetSlotOffset(slot);
        var data = pk.EncryptedPartyData;
        // Wipe Party Stats
        Array.Clear(data, LEN_STORED, LEN_PARTYSTAT);
        data.CopyTo(Data, ofs);
    }

    public PK8[] GetTeam()
    {
        var team = new PK8[COUNT_POKE];
        for (int i = 0; i < team.Length; i++)
            team[i] = GetSlot(i);
        return team;
    }

    public void SetTeam(IReadOnlyList<PK8> team)
    {
        for (int i = 0; i < team.Count; i++)
            SetSlot(i, team[i]);
    }

    public static int GetSlotOffset(int slot)
    {
        if ((uint)slot >= COUNT_POKE)
            throw new ArgumentOutOfRangeException(nameof(slot));
        return OFS_1 + (LEN_POKE * slot);
    }

    public Span<byte> GetMetadataStart() => Data.AsSpan(OFS_META, LEN_META);
    public Span<byte> GetMetadataEnd() => Data.AsSpan(POST_META);

    public uint Unknown { get => ReadUInt32LittleEndian(GetMetadataEnd()); set => WriteUInt32LittleEndian(GetMetadataEnd(), value); }

    public GameVersion Version
    {
        get => (GameVersion)Data[0x86A];
        set => Data[0x86A] = (byte)value;
    }

    public LanguageID Language
    {
        get => (LanguageID)Data[0x86B];
        set => Data[0x86B] = (byte)value;
    }

    public byte Gender
    {
        get => Data[0x86C];
        set => Data[0x86C] = value;
    }

    public byte Sprite
    {
        get => Data[0x86D];
        set => Data[0x86D] = value;
    }
    // 0x12 bytes unused  to pad out to full size 0x880

    public IEnumerable<PKM> Contents => GetTeam();

    public static bool IsRentalTeam(byte[] data)
    {
        if (data.Length != SIZE)
            return false;
        var team = new RentalTeam8(data).GetTeam();
        return team.All(x => x.ChecksumValid);
    }
}

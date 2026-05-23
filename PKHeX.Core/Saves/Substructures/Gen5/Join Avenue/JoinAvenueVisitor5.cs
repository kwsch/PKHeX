using System;
using static System.Buffers.Binary.BinaryPrimitives;
using static PKHeX.Core.StringConverter5;

namespace PKHeX.Core;

public sealed class JoinAvenueVisitor5(Memory<byte> data) : IJoinAvenueEntity5
{
    public const int SIZE = 0xC4;
    private Span<byte> Data => data.Span;
    public string FileExtension => "jav5";
    public ReadOnlySpan<byte> Write() => Data;

    public const int TriviaCount = 0x10;
    public const int ActivityCount = 4;
    public const byte ShopMaxLevel = 10;

    public void CopyFrom(JoinAvenueVisitor5 other) => other.Data.CopyTo(Data);

    public string Name
    {
        get => GetString(Data[..0xE]); // no terminator
        set => SetString(Data[..0xE], value, 7, Language);
    }

    public byte Country { get => Data[0x0E]; set => Data[0x0E] = value; }
    public byte Subregion { get => Data[0x0F]; set => Data[0x0F] = value; }

    public string Shout
    {
        get => GetString(Data[0x10..0x20]);
        set => SetString(Data[0x10..0x20], value, 8, Language);
    }

    public byte Version { get => Data[0x20]; set => Data[0x20] = value; }
    public byte Language { get => Data[0x21]; set => Data[0x21] = value; }

    public byte Unknown22
    {
        get => (byte)(Data[0x22] & 0x0F);
        set => Data[0x22] = (byte)((Data[0x22] & 0xF0) | (value & 0x0F));
    }

    public byte Gender
    {
        get => (byte)(Data[0x22] >> 4);
        set => Data[0x22] = (byte)((Data[0x22] & 0x0F) | ((value & 0x0F) << 4));
    }

    public byte Unused23 { get => Data[0x23]; set => Data[0x23] = value; }

    public ushort TID16
    {
        get => ReadUInt16LittleEndian(Data[0x24..]);
        set => WriteUInt16LittleEndian(Data[0x24..], value);
    }

    public byte Unknown26 { get => Data[0x26]; set => Data[0x26] = value; }
    public byte Unknown27 { get => Data[0x27]; set => Data[0x27] = value; }

    public ushort PlayedTime
    {
        get => ReadUInt16LittleEndian(Data[0x28..]);
        set => WriteUInt16LittleEndian(Data[0x28..], value);
    }

    public ushort PlayedHours
    {
        get => (ushort)(PlayedTime & 0x03FF);
        set => PlayedTime = (ushort)((PlayedTime & ~0x03FF) | (value & 0x03FF));
    }

    public byte PlayedMinutes
    {
        get => (byte)(PlayedTime >> 10);
        set => PlayedTime = (ushort)((PlayedTime & 0x03FF) | ((value & 0x3F) << 10));
    }

    /// <summary>
    /// Overworld Sprite ID
    /// </summary>
    public ushort Sprite
    {
        get => ReadUInt16LittleEndian(Data[0x2A..]);
        set => WriteUInt16LittleEndian(Data[0x2A..], value);
    }

    public bool IsFlag2C // nothing sets this flag to true?
    {
        get => (Data[0x2C] & 1) != 0;
        set => Data[0x2C] = (byte)((Data[0x2C] & ~1) | (value ? 1 : 0));
    }

    public byte JoinAvenueLevel
    {
        get => (byte)(Data[0x2C] >> 1);
        set => Data[0x2C] = (byte)((Data[0x2C] & 1) | ((value & 0x7F) << 1));
    }

    public byte Unused2D { get => Data[0x2D]; set => Data[0x2D] = value; }

    public ushort DesiredShopType
    {
        get => ReadUInt16LittleEndian(Data[0x2E..]);
        set => WriteUInt16LittleEndian(Data[0x2E..], value);
    }

    private uint ShopCounts
    {
        get => ReadUInt32LittleEndian(Data[0x30..]);
        set => WriteUInt32LittleEndian(Data[0x30..], value);
    }

    public byte ShopCountRaffle  { get => (byte)(ShopCounts & 0xF); set => ShopCounts = (ShopCounts & ~0xFu) | (value & 0xFu); }
    public byte ShopCountSalon   { get => (byte)((ShopCounts >> 4) & 0xF); set => ShopCounts = (ShopCounts & ~(0xFu << 4)) | ((uint)(value & 0xF) << 4); }
    public byte ShopCountMarket  { get => (byte)((ShopCounts >> 8) & 0xF); set => ShopCounts = (ShopCounts & ~(0xFu << 8)) | ((uint)(value & 0xF) << 8); }
    public byte ShopCountFlorist { get => (byte)((ShopCounts >> 12) & 0xF); set => ShopCounts = (ShopCounts & ~(0xFu << 12)) | ((uint)(value & 0xF) << 12); }
    public byte ShopCountDojo    { get => (byte)((ShopCounts >> 16) & 0xF); set => ShopCounts = (ShopCounts & ~(0xFu << 16)) | ((uint)(value & 0xF) << 16); }
    public byte ShopCountNurse   { get => (byte)((ShopCounts >> 20) & 0xF); set => ShopCounts = (ShopCounts & ~(0xFu << 20)) | ((uint)(value & 0xF) << 20); }
    public byte ShopCountAntique { get => (byte)((ShopCounts >> 24) & 0xF); set => ShopCounts = (ShopCounts & ~(0xFu << 24)) | ((uint)(value & 0xF) << 24); }
    public byte ShopCountCafe    { get => (byte)((ShopCounts >> 28) & 0xF); set => ShopCounts = (ShopCounts & ~(0xFu << 28)) | ((uint)(value & 0xF) << 28); }

    private uint GameProgress
    {
        get => ReadUInt32LittleEndian(Data[0x34..]);
        set => WriteUInt32LittleEndian(Data[0x34..], value);
    }

    public ushort DexSeen
    {
        get => (ushort)(GameProgress & 0x03FF);
        set => GameProgress = (GameProgress & ~0x03FFu) | (value & 0x03FFu);
    }

    public ushort FavoriteSpecies
    {
        get => (ushort)((GameProgress >> 10) & 0x03FF);
        set => GameProgress = (GameProgress & ~(0x03FFu << 10)) | (((uint)value & 0x03FF) << 10);
    }

    public byte MedalRank
    {
        get => (byte)((GameProgress >> 20) & 0xFF);
        set => GameProgress = (GameProgress & ~(0xFFu << 20)) | (((uint)value & 0xFF) << 20);
    }

    public byte MedalHint { get => Data[0x38]; set => Data[0x38] = value; }
    public byte MedalCount { get => Data[0x39]; set => Data[0x39] = value; }

    public JoinAvenueDate5 Date1
    {
        get => new(ReadUInt16LittleEndian(Data[0x3A..]));
        set => WriteUInt16LittleEndian(Data[0x3A..], value.RawValue);
    }

    public JoinAvenueDate5 DateAdventureStart
    {
        get => new(ReadUInt16LittleEndian(Data[0x3C..]));
        set => WriteUInt16LittleEndian(Data[0x3C..], value.RawValue);
    }

    public JoinAvenueDate5 DateHallOfFame
    {
        get => new(ReadUInt16LittleEndian(Data[0x3E..]));
        set => WriteUInt16LittleEndian(Data[0x3E..], value.RawValue);
    }

    public uint GetRecord(JoinAvenueRecordIndex5 index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)JoinAvenueRecordIndex5.COUNT_MAX);
        return ReadUInt32LittleEndian(Data[(0x40 + ((int)index * sizeof(uint)))..]);
    }

    public void SetRecord(JoinAvenueRecordIndex5 index, uint value)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)JoinAvenueRecordIndex5.COUNT_MAX);
        WriteUInt32LittleEndian(Data[(0x40 + ((int)index * sizeof(uint)))..], value);
    }

    public byte GetTrivia(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, TriviaCount);
        return Data[0x60 + index];
    }

    public void SetTrivia(int index, byte value)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, TriviaCount);
        Data[0x60 + index] = value;
    }

    public byte GetActivity(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, ActivityCount);
        return Data[0x70 + index];
    }

    public void SetActivity(int index, byte value)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, ActivityCount);
        Data[0x70 + index] = value;
    }

    public JoinAvenueDate5 GetActivityDate(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, ActivityCount);
        return new(ReadUInt16LittleEndian(Data[(0x74 + (index * sizeof(ushort)))..]));
    }

    public void SetActivityDate(int index, JoinAvenueDate5 value)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, ActivityCount);
        WriteUInt16LittleEndian(Data[(0x74 + (index * sizeof(ushort)))..], value.RawValue);
    }

    public string Greeting
    {
        get => GetString(Data[0x80..0x90]);
        set => SetString(Data[0x80..0x90], value, 8, Language);
    }

    public string Farewell
    {
        get => GetString(Data[0x90..0xA0]);
        set => SetString(Data[0x90..0xA0], value, 8, Language);
    }

    public ushort Origin // 00 NPC, 01 Human Player
    {
        get => ReadUInt16LittleEndian(Data[0xA0..]);
        set => WriteUInt16LittleEndian(Data[0xA0..], value);
    }

    public byte UnusedA2 { get => Data[0xA2]; set => Data[0xA2] = value; }
    public byte MetYear { get => Data[0xA3]; set => Data[0xA3] = value; }
    public byte MetMonth { get => Data[0xA4]; set => Data[0xA4] = value; }
    public byte MetDay { get => Data[0xA5]; set => Data[0xA5] = value; }
    public byte MetHour { get => Data[0xA6]; set => Data[0xA6] = value; }
    public byte MetMinute { get => Data[0xA7]; set => Data[0xA7] = value; }
    public byte UnknownA8 { get => Data[0xA8]; set => Data[0xA8] = value; }

    public bool IsShopChangeAllowed
    {
        get => FlagUtil.GetFlag(Data, 0xA9, 0);
        set => FlagUtil.SetFlag(Data, 0xA9, 0, value);
    }

    public bool IsFlagA9_1
    {
        get => FlagUtil.GetFlag(Data, 0xA9, 1);
        set => FlagUtil.SetFlag(Data, 0xA9, 1, value);
    }

    public bool IsFlagA9_2
    {
        get => FlagUtil.GetFlag(Data, 0xA9, 2);
        set => FlagUtil.SetFlag(Data, 0xA9, 2, value);
    }

    // unused flags at 0xA9: 3,4,5,6

    public bool IsInteractedToday
    {
        get => FlagUtil.GetFlag(Data, 0xA9, 7);
        set => FlagUtil.SetFlag(Data, 0xA9, 7, value);
    }

    public bool IsFlagAA
    {
        get => FlagUtil.GetFlag(Data, 0xAA, 0);
        set => FlagUtil.SetFlag(Data, 0xAA, 0, value);
    }

    // unused flags at 0xAA: 1,2,3,4,5,6,7

    public byte JoinAvenueRank { get => Data[0xAB]; set => Data[0xAB] = value; }
    public byte UnknownAC { get => Data[0xAC]; set => Data[0xAC] = value; }
    public byte ShopLevel { get => Data[0xAD]; set => Data[0xAD] = Math.Min(ShopMaxLevel, value); }

    public ushort ShopExperience
    {
        get => ReadUInt16LittleEndian(Data[0xAE..]);
        set => WriteUInt16LittleEndian(Data[0xAE..], value);
    }

    public uint IsInventory // 0/1
    {
        get => ReadUInt32LittleEndian(Data[0xB0..]);
        set => WriteUInt32LittleEndian(Data[0xB0..], value);
    }

    public JoinAvenueShopType5 ShopType
    {
        get => (JoinAvenueShopType5)ReadUInt16LittleEndian(Data[0xB4..]);
        set => WriteUInt16LittleEndian(Data[0xB4..], (ushort)value);
    }

    public ushort ShopWork
    {
        get => ReadUInt16LittleEndian(Data[0xB6..]);
        set => WriteUInt16LittleEndian(Data[0xB6..], value);
    }

    public uint UnusedB8
    {
        get => ReadUInt32LittleEndian(Data[0xB8..]);
        set => WriteUInt32LittleEndian(Data[0xB8..], value);
    }

    private uint UnknownBits
    {
        get => ReadUInt32LittleEndian(Data[0xBC..]);
        set => WriteUInt32LittleEndian(Data[0xBC..], value);
    }

    public ushort UnknownBits0_8
    {
        get => (ushort)(UnknownBits & 0x1FF);
        set => UnknownBits = (UnknownBits & ~0x1FFu) | ((uint)value & 0x1FF);
    }

    public bool IsUnknownBits9
    {
        get => ((UnknownBits >> 9) & 1) != 0;
        set => UnknownBits = (UnknownBits & ~(1u << 9)) | ((value ? 1u : 0u) << 9);
    }

    public byte UnknownBits10
    {
        get => (byte)((UnknownBits >> 10) & 0x7);
        set => UnknownBits = (UnknownBits & ~(0x7u << 10)) | (((uint)value & 0x7) << 10);
    }

    public byte UnknownBits13_20
    {
        get => (byte)((UnknownBits >> 13) & 0xFF);
        set => UnknownBits = (UnknownBits & ~(0xFFu << 13)) | (((uint)value & 0xFF) << 13);
    }

    public byte UnknownBits21_27
    {
        get => (byte)((UnknownBits >> 21) & 0x7F);
        set => UnknownBits = (UnknownBits & ~(0x7Fu << 21)) | (((uint)value & 0x7F) << 21);
    }

    public byte UnknownBits28_31
    {
        get => (byte)((UnknownBits >> 28) & 0xF);
        set => UnknownBits = (UnknownBits & ~(0xFu << 28)) | (((uint)value & 0xF) << 28);
    }

    public uint Seed
    {
        get => ReadUInt32LittleEndian(Data[0xC0..]);
        set => WriteUInt32LittleEndian(Data[0xC0..], value);
    }
}

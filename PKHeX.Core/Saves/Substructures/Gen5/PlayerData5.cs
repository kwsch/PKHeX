using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Player data as well as ExtData flagging.
/// </summary>
public abstract class PlayerData5(SAV5 sav, Memory<byte> raw) : SaveBlock<SAV5>(sav, raw)
{
    private Span<byte> OriginalTrainerTrash => Data.Slice(4, 0x10);

    public string OT
    {
        get => SAV.GetString(OriginalTrainerTrash);
        set => SAV.SetString(OriginalTrainerTrash, value, SAV.MaxStringLengthTrainer, StringConverterOption.ClearZero);
    }

    public uint ID32
    {
        get => ReadUInt32LittleEndian(Data[0x14..]);
        set => WriteUInt32LittleEndian(Data[0x14..], value);
    }

    public ushort TID16
    {
        get => ReadUInt16LittleEndian(Data[(0x14 + 0)..]);
        set => WriteUInt16LittleEndian(Data[(0x14 + 0)..], value);
    }

    public ushort SID16
    {
        get => ReadUInt16LittleEndian(Data[(0x14 + 2)..]);
        set => WriteUInt16LittleEndian(Data[(0x14 + 2)..], value);
    }

    public uint GameSyncID
    {
        get => ReadUInt32LittleEndian(Data[0x18..]);
        set => WriteUInt32LittleEndian(Data[0x18..], value);
    }

    public int Country
    {
        get => Data[0x1C];
        set => Data[0x1C] = (byte)value;
    }

    public int Region
    {
        get => Data[0x1D];
        set => Data[0x1D] = (byte)value;
    }

    public int Language
    {
        get => Data[0x1E];
        set => Data[0x1E] = (byte)value;
    }

    public byte Game
    {
        get => Data[0x1F];
        set => Data[0x1F] = value;
    }

    public byte Gender
    {
        get => Data[0x21];
        set => Data[0x21] = value;
    }

    // 22,23 ??

    public int PlayedHours
    {
        get => ReadUInt16LittleEndian(Data[0x24..]);
        set => WriteUInt16LittleEndian(Data[0x24..], (ushort)value);
    }

    public int PlayedMinutes
    {
        get => Data[0x24 + 2];
        set => Data[0x24 + 2] = (byte)value;
    }

    public int PlayedSeconds
    {
        get => Data[0x24 + 3];
        set => Data[0x24 + 3] = (byte)value;
    }

    // 7 bits year
    // 4 bits month
    // 5 bits day
    // 5 bits hour
    // 6 bits minute
    public uint LastSaved
    {
        get => ReadUInt32LittleEndian(Data[0x28..]);
        set => WriteUInt32LittleEndian(Data[0x2C..], value);
    }

    public uint LastSavedYear
    {
        get => LastSaved & 0x7F;
        set => LastSaved = (LastSaved & 0xFFFFFF80) | (value & 0x7F);
    }

    public uint LastSavedMonth
    {
        get => (LastSaved >> 7) & 0x0F;
        set => LastSaved = (LastSaved & 0xFFFFF87F) | ((value & 0x0F) << 7);
    }

    public uint LastSavedDay
    {
        get => (LastSaved >> 11) & 0x1F;
        set => LastSaved = (LastSaved & 0xFFFFC7FF) | ((value & 0x1F) << 11);
    }

    public uint LastSavedHour
    {
        get => (LastSaved >> 16) & 0x1F;
        set => LastSaved = (LastSaved & 0xFFFE3FFF) | ((value & 0x1F) << 16);
    }

    public uint LastSavedMinute
    {
        get => (LastSaved >> 21) & 0x3F;
        set => LastSaved = (LastSaved & 0xFFF1FFFF) | ((value & 0x3F) << 21);
    }

    public DateTime LastSavedDateTime
    {
        get => new(2000 + (int)LastSavedYear, (int)LastSavedMonth, (int)LastSavedDay, (int)LastSavedHour, (int)LastSavedMinute, 0);
        set
        {
            LastSavedYear = (uint)(value.Year - 2000);
            LastSavedMonth = (uint)value.Month;
            LastSavedDay = (uint)value.Day;
            LastSavedHour = (uint)value.Hour;
            LastSavedMinute = (uint)value.Minute;
        }
    }

    // 0x2C-0x2F ???

    protected const ushort C21E = 0xC21E;

    public bool IsMagicBattleVideo0 { get => ReadUInt16LittleEndian(Data[0x30..]) == C21E; set => WriteUInt16LittleEndian(Data[0x30..], value ? C21E : (ushort)0); }
    public bool IsMagicBattleVideo1 { get => ReadUInt16LittleEndian(Data[0x32..]) == C21E; set => WriteUInt16LittleEndian(Data[0x32..], value ? C21E : (ushort)0); }
    public bool IsMagicBattleVideo2 { get => ReadUInt16LittleEndian(Data[0x34..]) == C21E; set => WriteUInt16LittleEndian(Data[0x34..], value ? C21E : (ushort)0); }
    public bool IsMagicBattleVideo3 { get => ReadUInt16LittleEndian(Data[0x36..]) == C21E; set => WriteUInt16LittleEndian(Data[0x36..], value ? C21E : (ushort)0); }
    public bool IsMagicCGearSkin    { get => ReadUInt16LittleEndian(Data[0x38..]) == C21E; set => WriteUInt16LittleEndian(Data[0x38..], value ? C21E : (ushort)0); }
    public bool IsMagicBattleTest   { get => ReadUInt16LittleEndian(Data[0x3A..]) == C21E; set => WriteUInt16LittleEndian(Data[0x3A..], value ? C21E : (ushort)0); }
    public bool IsMagicMusical      { get => ReadUInt16LittleEndian(Data[0x3C..]) == C21E; set => WriteUInt16LittleEndian(Data[0x3C..], value ? C21E : (ushort)0); }
    public bool IsMagicPokeDexSkin  { get => ReadUInt16LittleEndian(Data[0x3E..]) == C21E; set => WriteUInt16LittleEndian(Data[0x3E..], value ? C21E : (ushort)0); }
    public bool IsMagicHallOfFame   { get => ReadUInt16LittleEndian(Data[0x40..]) == C21E; set => WriteUInt16LittleEndian(Data[0x40..], value ? C21E : (ushort)0); }
    // 0x42 unused in B/W

    public abstract uint CountBattleVideo0 { get; set; }
    public abstract uint CountBattleVideo1 { get; set; }
    public abstract uint CountBattleVideo2 { get; set; }
    public abstract uint CountBattleVideo3 { get; set; }
    public abstract uint CountCGearSkin { get; set; }
    public abstract uint CountBattleTest { get; set; }
    public abstract uint CountMusical { get; set; }
    public abstract uint CountPokedexSkin { get; set; }
    public abstract uint CountHallOfFame { get; set; }

    public abstract void UpdateExtData(ExtDataSectionNote5 section, uint count);
    public abstract (ushort, uint) GetExtData(ExtDataSectionNote5 section);
}

public sealed class PlayerData5BW(SAV5 sav, Memory<byte> raw) : PlayerData5(sav, raw)
{
    public override uint CountBattleVideo0 { get => ReadUInt32LittleEndian(Data[0x44..]); set => WriteUInt32LittleEndian(Data[0x44..], value); }
    public override uint CountBattleVideo1 { get => ReadUInt32LittleEndian(Data[0x48..]); set => WriteUInt32LittleEndian(Data[0x48..], value); }
    public override uint CountBattleVideo2 { get => ReadUInt32LittleEndian(Data[0x4C..]); set => WriteUInt32LittleEndian(Data[0x4C..], value); }
    public override uint CountBattleVideo3 { get => ReadUInt32LittleEndian(Data[0x50..]); set => WriteUInt32LittleEndian(Data[0x50..], value); }
    public override uint CountCGearSkin    { get => ReadUInt32LittleEndian(Data[0x54..]); set => WriteUInt32LittleEndian(Data[0x54..], value); }
    public override uint CountBattleTest   { get => ReadUInt32LittleEndian(Data[0x58..]); set => WriteUInt32LittleEndian(Data[0x58..], value); }
    public override uint CountMusical      { get => ReadUInt32LittleEndian(Data[0x5C..]); set => WriteUInt32LittleEndian(Data[0x5C..], value); }
    public override uint CountPokedexSkin  { get => ReadUInt32LittleEndian(Data[0x60..]); set => WriteUInt32LittleEndian(Data[0x60..], value); }
    public override uint CountHallOfFame   { get => ReadUInt32LittleEndian(Data[0x64..]); set => WriteUInt32LittleEndian(Data[0x64..], value); }

    public override void UpdateExtData(ExtDataSectionNote5 section, uint count)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)section, (uint)ExtDataSectionNote5.HallOfFame, nameof(section));
        ushort value = count == 0 ? (ushort)0 : C21E;
        WriteUInt16LittleEndian(Data[(0x30 + ((int)section * 2))..], value);
        WriteUInt32LittleEndian(Data[(0x44 + ((int)section * 4))..], count);
    }

    public override (ushort, uint) GetExtData(ExtDataSectionNote5 section)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)section, (uint)ExtDataSectionNote5.HallOfFame, nameof(section));
        var state = ReadUInt16LittleEndian(Data[(0x30 + ((int)section * 2))..]);
        var count = ReadUInt32LittleEndian(Data[(0x44 + ((int)section * 4))..]);
        return (state, count);
    }
}

public sealed class PlayerData5B2W2(SAV5B2W2 sav, Memory<byte> raw) : PlayerData5(sav, raw)
{
    // Offsets are shifted due to the addition of new DLC blocks, which have the u16 and u32 counts.
    public bool IsMagicMovie1 { get => ReadUInt16LittleEndian(Data[0x42..]) == C21E; set => WriteUInt16LittleEndian(Data[0x42..], value ? C21E : (ushort)0); }
    public bool IsMagicMovie2 { get => ReadUInt16LittleEndian(Data[0x44..]) == C21E; set => WriteUInt16LittleEndian(Data[0x44..], value ? C21E : (ushort)0); }
    public bool IsMagicMovie3 { get => ReadUInt16LittleEndian(Data[0x46..]) == C21E; set => WriteUInt16LittleEndian(Data[0x46..], value ? C21E : (ushort)0); }
    public bool IsMagicMovie4 { get => ReadUInt16LittleEndian(Data[0x48..]) == C21E; set => WriteUInt16LittleEndian(Data[0x48..], value ? C21E : (ushort)0); }
    public bool IsMagicMovie5 { get => ReadUInt16LittleEndian(Data[0x4A..]) == C21E; set => WriteUInt16LittleEndian(Data[0x4A..], value ? C21E : (ushort)0); }
    public bool IsMagicMovie6 { get => ReadUInt16LittleEndian(Data[0x4C..]) == C21E; set => WriteUInt16LittleEndian(Data[0x4C..], value ? C21E : (ushort)0); }
    public bool IsMagicMovie7 { get => ReadUInt16LittleEndian(Data[0x4E..]) == C21E; set => WriteUInt16LittleEndian(Data[0x4E..], value ? C21E : (ushort)0); }
    public bool IsMagicMovie8 { get => ReadUInt16LittleEndian(Data[0x50..]) == C21E; set => WriteUInt16LittleEndian(Data[0x50..], value ? C21E : (ushort)0); }
    public bool IsMagicPWT1   { get => ReadUInt16LittleEndian(Data[0x52..]) == C21E; set => WriteUInt16LittleEndian(Data[0x52..], value ? C21E : (ushort)0); }
    public bool IsMagicPWT2   { get => ReadUInt16LittleEndian(Data[0x54..]) == C21E; set => WriteUInt16LittleEndian(Data[0x54..], value ? C21E : (ushort)0); }
    public bool IsMagicPWT3   { get => ReadUInt16LittleEndian(Data[0x56..]) == C21E; set => WriteUInt16LittleEndian(Data[0x56..], value ? C21E : (ushort)0); }
    public bool IsMagicUnused { get => ReadUInt16LittleEndian(Data[0x58..]) == C21E; set => WriteUInt16LittleEndian(Data[0x58..], value ? C21E : (ushort)0); }

    public override uint CountBattleVideo0 { get => ReadUInt32LittleEndian(Data[0x5C..]); set => WriteUInt32LittleEndian(Data[0x5C..], value); }
    public override uint CountBattleVideo1 { get => ReadUInt32LittleEndian(Data[0x60..]); set => WriteUInt32LittleEndian(Data[0x60..], value); }
    public override uint CountBattleVideo2 { get => ReadUInt32LittleEndian(Data[0x64..]); set => WriteUInt32LittleEndian(Data[0x64..], value); }
    public override uint CountBattleVideo3 { get => ReadUInt32LittleEndian(Data[0x68..]); set => WriteUInt32LittleEndian(Data[0x68..], value); }
    public override uint CountCGearSkin    { get => ReadUInt32LittleEndian(Data[0x6C..]); set => WriteUInt32LittleEndian(Data[0x6C..], value); }
    public override uint CountBattleTest   { get => ReadUInt32LittleEndian(Data[0x70..]); set => WriteUInt32LittleEndian(Data[0x70..], value); }
    public override uint CountMusical      { get => ReadUInt32LittleEndian(Data[0x74..]); set => WriteUInt32LittleEndian(Data[0x74..], value); }
    public override uint CountPokedexSkin  { get => ReadUInt32LittleEndian(Data[0x78..]); set => WriteUInt32LittleEndian(Data[0x78..], value); }
    public override uint CountHallOfFame   { get => ReadUInt32LittleEndian(Data[0x7C..]); set => WriteUInt32LittleEndian(Data[0x7C..], value); }
    public uint CountMovie1  { get => ReadUInt32LittleEndian(Data[0x80..]); set => WriteUInt32LittleEndian(Data[0x80..], value); }
    public uint CountMovie2  { get => ReadUInt32LittleEndian(Data[0x84..]); set => WriteUInt32LittleEndian(Data[0x84..], value); }
    public uint CountMovie3  { get => ReadUInt32LittleEndian(Data[0x88..]); set => WriteUInt32LittleEndian(Data[0x88..], value); }
    public uint CountMovie4  { get => ReadUInt32LittleEndian(Data[0x8C..]); set => WriteUInt32LittleEndian(Data[0x8C..], value); }
    public uint CountMovie5  { get => ReadUInt32LittleEndian(Data[0x90..]); set => WriteUInt32LittleEndian(Data[0x90..], value); }
    public uint CountMovie6  { get => ReadUInt32LittleEndian(Data[0x94..]); set => WriteUInt32LittleEndian(Data[0x94..], value); }
    public uint CountMovie7  { get => ReadUInt32LittleEndian(Data[0x98..]); set => WriteUInt32LittleEndian(Data[0x98..], value); }
    public uint CountMovie8  { get => ReadUInt32LittleEndian(Data[0x9C..]); set => WriteUInt32LittleEndian(Data[0x9C..], value); }
    public uint CountPWT1    { get => ReadUInt32LittleEndian(Data[0xA0..]); set => WriteUInt32LittleEndian(Data[0xA0..], value); }
    public uint CountPWT2    { get => ReadUInt32LittleEndian(Data[0xA4..]); set => WriteUInt32LittleEndian(Data[0xA4..], value); }
    public uint CountPWT3    { get => ReadUInt32LittleEndian(Data[0xA8..]); set => WriteUInt32LittleEndian(Data[0xA8..], value); }
    public uint CountUnused  { get => ReadUInt32LittleEndian(Data[0xAC..]); set => WriteUInt32LittleEndian(Data[0xAC..], value); }

    public override void UpdateExtData(ExtDataSectionNote5 section, uint count)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)section, (uint)ExtDataSectionNote5.KeyData, nameof(section));
        ushort value = count == 0 ? (ushort)0 : C21E;
        WriteUInt16LittleEndian(Data[(0x30 + ((int)section * 2))..], value);
        WriteUInt32LittleEndian(Data[(0x5C + ((int)section * 4))..], count);
    }

    public override (ushort, uint) GetExtData(ExtDataSectionNote5 section)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)section, (uint)ExtDataSectionNote5.HallOfFame, nameof(section));
        var state = ReadUInt16LittleEndian(Data[(0x30 + ((int)section * 2))..]);
        var count = ReadUInt32LittleEndian(Data[(0x5C + ((int)section * 4))..]);
        return (state, count);
    }
}

public enum ExtDataSectionNote5
{
    BattleVideo0 = 0,
    BattleVideo1 = 1,
    BattleVideo2 = 2,
    BattleVideo3 = 3,
    CGearSkin = 4,
    BattleTest = 5,
    Musical = 6,
    PokedexSkin = 7,
    HallOfFame = 8,

    // B2/W2 additions
    Movie1 = 9,
    Movie2 = 10,
    Movie3 = 11,
    Movie4 = 12,
    Movie5 = 13,
    Movie6 = 14,
    Movie7 = 15,
    Movie8 = 16,
    PWT1 = 17,
    PWT2 = 18,
    PWT3 = 19,
    KeyData = 20,
}

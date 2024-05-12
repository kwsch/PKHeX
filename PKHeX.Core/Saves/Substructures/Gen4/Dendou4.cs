using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class Dendou4(Memory<byte> raw, int language)
{
    private const int SIZE = 0x2AB0;
    private const int SIZE_FOOTER = 0x10;
    private const int SIZE_BLOCK = 3 * 0x1000;

    public const int MaxClears = 9999;
    public const int MaxRecords = 30;

    private Span<byte> Data => raw.Span;

    // Structure:
    // record[30] records
    // u32 NextIndexToOverwrite
    // u32 ClearCount

    public Dendou4Record this[int index] => GetRecord(index);

    private Dendou4Record GetRecord(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, MaxRecords);
        var slice = Data.Slice(index * Dendou4Record.SIZE, Dendou4Record.SIZE);
        return new Dendou4Record(slice, language);
    }

    private const int EndDataOffset = MaxRecords * Dendou4Record.SIZE; // 0x2AA8

    public uint IndexNextOverwrite { get => ReadUInt32LittleEndian(Data[EndDataOffset..]); set => WriteUInt32LittleEndian(Data[EndDataOffset..], value % MaxRecords); }
    public uint ClearCount { get => Math.Min(MaxClears, ReadUInt32LittleEndian(Data[(EndDataOffset + 4)..])); set => WriteUInt32LittleEndian(Data[(EndDataOffset + 4)..], Math.Min(MaxClears, value)); }

    #region Footer
    public bool SizeValid => BlockSize == SIZE;
    public bool ChecksumValid => Checksum == GetChecksum();
    public bool IsValid => SizeValid && ChecksumValid;

    public uint  Magic     { get => ReadUInt32LittleEndian(Footer); set => WriteUInt32LittleEndian(Footer, value); }
    public uint  Revision  { get => ReadUInt32LittleEndian(Footer[0x4..]); set => WriteUInt32LittleEndian(Footer[0x4..], value); }
    public int   BlockSize { get => ReadInt32LittleEndian (Footer[0x8..]); set => WriteInt32LittleEndian (Footer[0x8..], value); }
    public ushort BlockID  { get => ReadUInt16LittleEndian(Footer[0xC..]); set => WriteUInt16LittleEndian(Footer[0xC..], value); }
    public ushort Checksum { get => ReadUInt16LittleEndian(Footer[0xE..]); set => WriteUInt16LittleEndian(Footer[0xE..], value); }

    private ReadOnlySpan<byte> GetRegion() => Data[..(SIZE + SIZE_FOOTER)];
    private Span<byte> Footer => Data.Slice(SIZE, SIZE_FOOTER);
    private ushort GetChecksum() => Checksums.CRC16_CCITT(GetRegion()[..^2]);
    public void RefreshChecksum() => Checksum = GetChecksum();
    #endregion
}

public readonly ref struct Dendou4Record
{
    public const int Count = 6;
    public const int SIZE = (Dendou4Entity.SIZE * Count) + sizeof(uint); // 0x16C

    // structure:
    // Dendou4Entity[6]
    // u16 Year
    // u8 Month
    // u8 Day

    private readonly Span<byte> Data;
    private readonly int Language;

    // ReSharper disable once ConvertToPrimaryConstructor
    public Dendou4Record(Span<byte> data, int language)
    {
        Data = data;
        Language = language;
    }

    public Dendou4Entity this[int index] => GetEntity(index);

    private const int DateOffset = SIZE - 4;
    public ushort Year { get => ReadUInt16LittleEndian(Data[DateOffset..]); set => WriteUInt16LittleEndian(Data[DateOffset..], value); }
    public byte Month { get => Data[DateOffset + 2]; set => Data[DateOffset + 2] = value; }
    public byte Day   { get => Data[DateOffset + 3]; set => Data[DateOffset + 3] = value; }

    private Dendou4Entity GetEntity(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, Count);
        var slice = Data.Slice(index * Dendou4Entity.SIZE, Dendou4Entity.SIZE);
        return new Dendou4Entity(slice, Language);
    }
}

public readonly ref struct Dendou4Entity
{
    public const int SIZE = 0x3C;
    private readonly Span<byte> Data;
    private readonly int Language;

    // ReSharper disable once ConvertToPrimaryConstructor
    public Dendou4Entity(Span<byte> data, int language)
    {
        Data = data;
        Language = language;
    }

    public ushort Species { get => ReadUInt16LittleEndian(Data); set => WriteUInt16LittleEndian(Data, value); }
    public byte Level { get => Data[2]; set => Data[2] = value; }
    public byte Form  { get => Data[3]; set => Data[3] = value; }
    public uint PID  { get => ReadUInt32LittleEndian(Data[4..]); set => WriteUInt32LittleEndian(Data[4..], value); }
    public uint ID32 { get => ReadUInt32LittleEndian(Data[8..]); set => WriteUInt32LittleEndian(Data[8..], value); }

    public Span<byte> NicknameTrash => Data.Slice(0x0C, 22);
    public Span<byte> OriginalTrainerTrash => Data.Slice(0x22, 16);
    public string Nickname
    {
        get => StringConverter4.GetString(NicknameTrash);
        set => StringConverter4.SetString(NicknameTrash, value, 10, Language, StringConverterOption.None);
    }
    public string OriginalTrainerName
    {
        get => StringConverter4.GetString(OriginalTrainerTrash);
        set => StringConverter4.SetString(OriginalTrainerTrash, value, 7, 0, StringConverterOption.None);
    }

    public ushort Move1 { get => ReadUInt16LittleEndian(Data[0x32..]); set => WriteUInt16LittleEndian(Data[0x32..], value); }
    public ushort Move2 { get => ReadUInt16LittleEndian(Data[0x34..]); set => WriteUInt16LittleEndian(Data[0x34..], value); }
    public ushort Move3 { get => ReadUInt16LittleEndian(Data[0x36..]); set => WriteUInt16LittleEndian(Data[0x36..], value); }
    public ushort Move4 { get => ReadUInt16LittleEndian(Data[0x38..]); set => WriteUInt16LittleEndian(Data[0x38..], value); }
    // u16 alignment 0x3A
}

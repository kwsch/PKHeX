using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public abstract class EventWork7(SAV7 sav, Memory<byte> raw) : SaveBlock<SAV7>(sav, raw), IEventFlag37
{
    private readonly SAV7 sav7 = sav;
    protected abstract Span<byte> WorkSpan { get; }
    protected abstract Span<byte> FlagSpan { get; }
    protected abstract Memory<byte> FameSpan { get; }
    public HallOfFame7 Fame => new(FameSpan);
    public abstract int EventFlagCount { get; }
    public abstract int EventWorkCount { get; }
    public abstract int TotalZygardeCellCount { get; }

    public bool GetEventFlag(int flagNumber) => FlagUtil.GetFlag(FlagSpan, flagNumber);
    public void SetEventFlag(int flagNumber, bool value) => FlagUtil.SetFlag(FlagSpan, flagNumber, value);

    public ushort GetWork(int index) => ReadUInt16LittleEndian(WorkSpan[(index * sizeof(ushort))..]);
    public void SetWork(int index, ushort value) => WriteUInt16LittleEndian(WorkSpan[(index * sizeof(ushort))..], value);

    private const int cellstotal = 161;
    private const int cellscollected = 169;
    private const int celloffset = 198;

    public ushort ZygardeCellCount { get => GetWork(cellscollected); set => SetWork(cellscollected, value); }
    public ushort ZygardeCellTotal { get => GetWork(cellstotal); set => SetWork(cellstotal, value); }

    public ushort GetZygardeCell(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)TotalZygardeCellCount);
        return GetWork(celloffset + index);
    }

    public void SetZygardeCell(int index, ushort val)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)TotalZygardeCellCount);
        SetWork(celloffset + index, val);
    }

    public void UpdateQrConstants() => sav7.UpdateQrConstants();
}

public sealed class EventWork7SM(SAV7SM sav, Memory<byte> raw) : EventWork7(sav, raw)
{
    public const int WorkCount = 1000; // u16
    public const int FlagCount = 4000; // bits

    private const int OffsetWork = 0x0;
    private const int OffsetFlag = OffsetWork + (WorkCount * sizeof(ushort)); // 0x7D0

    // Hall of Fame
    private const int OffsetPostData = OffsetFlag + (FlagCount / 8); // 0x9C4

    public override int EventFlagCount => FlagCount;
    public override int EventWorkCount => WorkCount;
    public override int TotalZygardeCellCount => 95;
    protected override Span<byte> WorkSpan => Raw.Span[..(WorkCount * sizeof(ushort))];
    protected override Span<byte> FlagSpan => Raw.Span.Slice(OffsetFlag, FlagCount / 8);
    protected override Memory<byte> FameSpan => Raw.Slice(OffsetPostData, HallOfFame7.SIZE);

    public const int MagearnaEventFlag = 3100;
}

public sealed class EventWork7USUM(SAV7USUM sav, Memory<byte> raw) : EventWork7(sav, raw)
{
    public const int WorkCount = 1000; // u16
    public const int FlagCount = 4960;

    private const int OffsetWork = 0x0;
    private const int OffsetFlag = OffsetWork + (WorkCount * sizeof(ushort)); // 0x7D0

    // Hallf of Fame
    private const int OffsetPostData = OffsetFlag + (FlagCount / 8); // 0xA3C

    public override int EventFlagCount => FlagCount;
    public override int EventWorkCount => WorkCount;
    public override int TotalZygardeCellCount => 100;
    protected override Span<byte> WorkSpan => Raw.Span[..(EventWorkCount * sizeof(ushort))];
    protected override Span<byte> FlagSpan => Raw.Span.Slice(OffsetFlag, FlagCount / 8);
    protected override Memory<byte> FameSpan => Raw.Slice(OffsetPostData, HallOfFame7.SIZE);

    public const int MagearnaEventFlag = 4060;
    public const int CapPikachuEventFlag = 4562;
}

public sealed class HallOfFame7(Memory<byte> raw)
{
    // this HoF region is immediately after the Event Flags
    private const uint MaxCount = 12;

    public const int SIZE = 2 * (6 * sizeof(ushort)); // 24 bytes

    private Span<byte> Data => raw.Span;

    public ushort First1 { get => ReadUInt16LittleEndian(Data); set => WriteUInt16LittleEndian(Data, value); }
    public ushort First2 { get => ReadUInt16LittleEndian(Data[0x02..]); set => WriteUInt16LittleEndian(Data[0x02..], value); }
    public ushort First3 { get => ReadUInt16LittleEndian(Data[0x04..]); set => WriteUInt16LittleEndian(Data[0x04..], value); }
    public ushort First4 { get => ReadUInt16LittleEndian(Data[0x04..]); set => WriteUInt16LittleEndian(Data[0x04..], value); }
    public ushort First5 { get => ReadUInt16LittleEndian(Data[0x06..]); set => WriteUInt16LittleEndian(Data[0x06..], value); }
    public ushort First6 { get => ReadUInt16LittleEndian(Data[0x08..]); set => WriteUInt16LittleEndian(Data[0x08..], value); }

    public ushort Current1 { get => ReadUInt16LittleEndian(Data[0x0A..]); set => WriteUInt16LittleEndian(Data[0x0A..], value); }
    public ushort Current2 { get => ReadUInt16LittleEndian(Data[0x0C..]); set => WriteUInt16LittleEndian(Data[0x0C..], value); }
    public ushort Current3 { get => ReadUInt16LittleEndian(Data[0x0E..]); set => WriteUInt16LittleEndian(Data[0x0E..], value); }
    public ushort Current4 { get => ReadUInt16LittleEndian(Data[0x10..]); set => WriteUInt16LittleEndian(Data[0x10..], value); }
    public ushort Current5 { get => ReadUInt16LittleEndian(Data[0x12..]); set => WriteUInt16LittleEndian(Data[0x12..], value); }
    public ushort Current6 { get => ReadUInt16LittleEndian(Data[0x14..]); set => WriteUInt16LittleEndian(Data[0x14..], value); }

    public ushort GetEntry(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, MaxCount);
        if ((uint)index >= MaxCount)
            throw new ArgumentOutOfRangeException(nameof(index));
        return ReadUInt16LittleEndian(Data[(index * 2)..]);
    }

    public void SetEntry(int index, ushort value)
    {
        if ((uint)index >= MaxCount)
            throw new ArgumentOutOfRangeException(nameof(index));
        WriteUInt16LittleEndian(Data[(index * 2)..], value);
    }
}

using System;
using System.Buffers.Binary;

namespace PKHeX.Core;

public abstract class EventWork5(SAV5 sav, Memory<byte> raw) : SaveBlock<SAV5>(sav, raw), IEventFlag37
{
    protected abstract Span<byte> WorkSpan { get; }
    protected abstract Span<byte> FlagSpan { get; }

    public bool GetEventFlag(int flagNumber) => FlagUtil.GetFlag(FlagSpan, flagNumber);
    public void SetEventFlag(int flagNumber, bool value) => FlagUtil.SetFlag(FlagSpan, flagNumber, value);
    public ushort GetWork(int index) => BinaryPrimitives.ReadUInt16LittleEndian(WorkSpan[(index * sizeof(ushort))..]);
    public void SetWork(int index, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(WorkSpan[(index * sizeof(ushort))..], value);

    public abstract int EventWorkCount { get; }
    public abstract int EventFlagCount { get; }
}

public sealed class EventWork5BW(SAV5BW sav, Memory<byte> raw) : EventWork5(sav, raw)
{
    public const int OffsetEventWork = 0;
    public const int OffsetEventFlag = OffsetEventWork + (CountEventWork * sizeof(ushort));

    public const int CountEventWork = 0x13E;
    public const int CountEventFlag = 0xB60;

    protected override Span<byte> WorkSpan => Data[..(CountEventWork * sizeof(ushort))];
    protected override Span<byte> FlagSpan => Data.Slice(OffsetEventFlag, CountEventFlag / 8);

    public override int EventWorkCount => CountEventWork;
    public override int EventFlagCount => CountEventFlag;

    private const int WorkRoamer = 192;
    public ushort GetWorkRoamer() => GetWork(WorkRoamer);
    public void SetWorkRoamer(ushort value) => SetWork(WorkRoamer, value);
}

public sealed class EventWork5B2W2(SAV5B2W2 sav, Memory<byte> raw) : EventWork5(sav, raw)
{
    public const int OffsetEventWork = 0;
    public const int OffsetEventFlag = OffsetEventWork + (CountEventWork * sizeof(ushort));

    public const int CountEventWork = 0x1AF;
    public const int CountEventFlag = 0xBF8;

    protected override Span<byte> WorkSpan => Data[..(CountEventWork * sizeof(ushort))];
    protected override Span<byte> FlagSpan => Data.Slice(OffsetEventFlag, CountEventFlag / 8);

    public override int EventWorkCount => CountEventWork;
    public override int EventFlagCount => CountEventFlag;
}

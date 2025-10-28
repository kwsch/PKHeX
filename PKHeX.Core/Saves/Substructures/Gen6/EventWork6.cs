using System;
using System.Buffers.Binary;

namespace PKHeX.Core;

public sealed class EventWork6(SAV6 sav, Memory<byte> raw) : SaveBlock<SAV6>(sav, raw), IEventFlag37
{
    public const int OffsetEventWork = 0;
    public const int OffsetEventFlag = OffsetEventWork + (CountEventWork * sizeof(ushort));

    public const int CountEventWork = 0x178;
    public const int CountEventFlag = 0xD00; // 3328

    private Span<byte> WorkSpan => Data[..(CountEventWork * sizeof(ushort))];
    private Span<byte> FlagSpan => Data.Slice(OffsetEventFlag, CountEventFlag / 8);

    public int EventWorkCount => CountEventWork;
    public int EventFlagCount => CountEventFlag;
    public bool GetEventFlag(int flagNumber) => FlagUtil.GetFlag(FlagSpan, flagNumber);
    public void SetEventFlag(int flagNumber, bool value) => FlagUtil.SetFlag(FlagSpan, flagNumber, value);
    public ushort GetWork(int index) => BinaryPrimitives.ReadUInt16LittleEndian(WorkSpan[(index * sizeof(ushort))..]);
    public void SetWork(int index, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(WorkSpan[(index * sizeof(ushort))..], value);
}

using System;
using System.Diagnostics;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public abstract class RecordBlock6 : RecordBlock<SaveFile> // 6 or 7
{
    public const int RecordCount = 200;

    protected RecordBlock6(SaveFile sav, int offset) : base(sav) => Offset = offset;

    // Structure:
    //   uint[100];
    // ushort[100];

    public override int GetRecord(int recordID)
    {
        int ofs = Records.GetOffset(Offset, recordID);
        switch (recordID)
        {
            case < 100:
                return ReadInt32LittleEndian(Data.AsSpan(ofs));
            case < 200:
                return ReadInt16LittleEndian(Data.AsSpan(ofs));
            default:
                Trace.Fail(nameof(recordID));
                return 0;
        }
    }

    public override void SetRecord(int recordID, int value)
    {
        if ((uint)recordID >= RecordCount)
            throw new ArgumentOutOfRangeException(nameof(recordID));
        int ofs = GetRecordOffset(recordID);
        int max = GetRecordMax(recordID);
        if (value > max)
            value = max;
        switch (recordID)
        {
            case < 100:
                WriteInt32LittleEndian(Data.AsSpan(ofs), value);
                break;
            case < 200:
                WriteUInt16LittleEndian(Data.AsSpan(ofs), (ushort)value);
                break;
            default:
                Trace.Fail(nameof(recordID));
                break;
        }
    }
}

public sealed class RecordBlock6XY(SAV6XY sav, int offset) : RecordBlock6(sav, offset)
{
    protected override ReadOnlySpan<byte> RecordMax => MaxType_XY;

    private static ReadOnlySpan<byte> MaxType_XY =>
    [
        0, 0, 0, 0, 0, 0, 0, 2, 2, 2,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 2, 2, 2, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 3,
        3, 0, 0, 1, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 2,
        2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
        2, 2, 2, 2, 2, 2, 2, 2, 2, 2,

        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 7, 5, 4, 4, 4, 4, 4, 4, 4,
        4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
        4, 4, 4, 4, 4, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 6, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
    ];
}

public sealed class RecordBlock6AO : RecordBlock6
{
    public RecordBlock6AO(SAV6AO sav, int offset) : base(sav, offset) { }
    public RecordBlock6AO(SAV6AODemo sav, int offset) : base(sav, offset) { }
    protected override ReadOnlySpan<byte> RecordMax => MaxType_AO;

    private static ReadOnlySpan<byte> MaxType_AO =>
    [
        0, 0, 0, 0, 0, 0, 0, 2, 2, 2,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 2, 2, 2, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 3,
        3, 0, 0, 1, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 2, 2, 2, 2, 2, 2, 2, 2, 2,

        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 7, 5, 4, 4, 4, 4, 4, 4, 4,
        4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
        4, 4, 4, 4, 4, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 6, 4, 4,
        4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
        7, 7, 7, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
    ];
}

public sealed class RecordBlock7SM(SAV7SM sav, int offset) : RecordBlock6(sav, offset)
{
    protected override ReadOnlySpan<byte> RecordMax => MaxType_SM;

    private static ReadOnlySpan<byte> MaxType_SM =>
    [
        0, 0, 0, 0, 0, 0, 2, 2, 2, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 2, 2, 2, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 2, 2, 2, 0, 0, 0, 2, 2, 0,
        0, 2, 2, 2, 2, 2, 2, 2, 2, 2,
        2, 2, 2, 2, 2, 2, 1, 2, 2, 2,
        2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
        2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
        2, 2, 2, 2, 2, 2, 2, 2, 2, 2,

        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 6, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
    ];
}

public sealed class RecordBlock7USUM(SAV7USUM sav, int offset) : RecordBlock6(sav, offset)
{
    protected override ReadOnlySpan<byte> RecordMax => MaxType_USUM;

    private static ReadOnlySpan<byte> MaxType_USUM =>
    [
        0, 0, 0, 0, 0, 0, 2, 2, 2, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 2, 2, 2, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 2, 2, 2, 0, 0, 0, 2, 2, 0,
        0, 2, 2, 2, 2, 2, 2, 2, 2, 2,
        2, 2, 2, 2, 2, 2, 1, 2, 2, 2,
        0, 0, 0, 0, 0, 2, 2, 2, 2, 2,
        2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
        2, 2, 2, 2, 2, 2, 2, 2, 2, 2,

        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 6, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 4, 4, 4, 5, 5, 4, 5, 5,
    ];
}

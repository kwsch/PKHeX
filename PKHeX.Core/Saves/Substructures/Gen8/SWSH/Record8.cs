using System;
using System.Diagnostics;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class Record8(SAV8SWSH sav, SCBlock block) : RecordBlock<SAV8SWSH>(sav, block.Data)
{
    public const int RecordCount = 50;
    public const int WattTotal = 22;
    protected override ReadOnlySpan<byte> RecordMax => MaxType_SWSH;

    public override int GetRecord(int recordID)
    {
        int ofs = Records.GetOffset(recordID);
        if (recordID < RecordCount)
            return ReadInt32LittleEndian(Data[ofs..]);
        Trace.Fail(nameof(recordID));
        return 0;
    }

    public override void SetRecord(int recordID, int value)
    {
        if ((uint)recordID >= RecordCount)
            throw new ArgumentOutOfRangeException(nameof(recordID));
        int ofs = GetRecordOffset(recordID);
        int max = GetRecordMax(recordID);
        if (value > max)
            value = max;
        if (recordID < RecordCount)
            WriteInt32LittleEndian(Data[ofs..], value);
        else
            Trace.Fail(nameof(recordID));
    }

    private static ReadOnlySpan<byte> MaxType_SWSH =>
    [
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
    ];
}

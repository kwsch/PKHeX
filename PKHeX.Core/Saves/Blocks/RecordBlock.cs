using System;

namespace PKHeX.Core;

public abstract class RecordBlock<T> : SaveBlock<T>, IRecordStatStorage where T : SaveFile
{
    protected abstract ReadOnlySpan<byte> RecordMax { get; }
    public abstract int GetRecord(int recordID);
    public abstract void SetRecord(int recordID, int value);

    public int GetRecordMax(int recordID) => Records.GetMax(recordID, RecordMax);
    public int GetRecordOffset(int recordID) => Records.GetOffset(Offset, recordID);
    public void AddRecord(int recordID, int count = 1) => SetRecord(recordID, GetRecord(recordID) + count);

    protected RecordBlock(T sav) : base(sav)
    {
    }

    protected RecordBlock(T sav, byte[] data) : base(sav, data)
    {
    }
}

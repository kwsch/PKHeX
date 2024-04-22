using System;

namespace PKHeX.Core;

public abstract class RecordBlock<T>(T sav, Memory<byte> raw) : SaveBlock<T>(sav, raw), IRecordStatStorage
    where T : SaveFile
{
    protected abstract ReadOnlySpan<byte> RecordMax { get; }
    public abstract int GetRecord(int recordID);
    public abstract void SetRecord(int recordID, int value);

    public int GetRecordMax(int recordID) => Records.GetMax(recordID, RecordMax);
    public int GetRecordOffset(int recordID) => Records.GetOffset(recordID);
    public void AddRecord(int recordID, int count = 1) => SetRecord(recordID, GetRecord(recordID) + count);
}

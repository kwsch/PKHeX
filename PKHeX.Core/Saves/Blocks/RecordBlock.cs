using System.Collections.Generic;

namespace PKHeX.Core
{
    public abstract class RecordBlock : SaveBlock
    {
        protected abstract IReadOnlyList<byte> RecordMax { get; }
        public abstract int GetRecord(int recordID);
        public abstract void SetRecord(int recordID, int value);

        public int GetRecordMax(int recordID) => Records.GetMax(recordID, RecordMax);
        public int GetRecordOffset(int recordID) => Records.GetOffset(Offset, recordID);
        public void AddRecord(int recordID, int count = 1) => SetRecord(recordID, GetRecord(recordID) + count);

        protected RecordBlock(SaveFile sav) : base(sav)
        {
        }

        protected RecordBlock(SaveFile sav, byte[] data) : base(sav, data)
        {
        }
    }
}
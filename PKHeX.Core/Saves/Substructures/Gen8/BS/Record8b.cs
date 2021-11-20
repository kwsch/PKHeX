using System;

namespace PKHeX.Core
{
    public sealed class Record8b : SaveBlock, IRecordStatStorage
    {
        public const int RecordCount = 30;
        public const int RecordMaxValue = 999_999;

        public Record8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

        public int GetRecordOffset(int recordID)
        {
            if ((uint)recordID >= RecordCount)
                throw new ArgumentOutOfRangeException(nameof(recordID));
            return Offset + (sizeof(int) * recordID);
        }

        public int GetRecord(int recordID)
        {
            var value = BitConverter.ToInt32(Data, GetRecordOffset(recordID));
            return Math.Min(RecordMaxValue, value);
        }

        public void SetRecord(int recordID, int value)
        {
            var toWrite = Math.Min(RecordMaxValue, value);
            BitConverter.GetBytes(toWrite).CopyTo(Data, GetRecordOffset(recordID));
        }

        public void AddRecord(int recordID, int count = 1) => SetRecord(recordID, GetRecord(recordID) + count);
    }
}

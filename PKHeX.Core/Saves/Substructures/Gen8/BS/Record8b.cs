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
            if (recordID != 0)
                value = Math.Min(RecordMaxValue, value);
            return value;
        }

        public void SetRecord(int recordID, int value)
        {
            if (recordID != 0)
                value = Math.Min(RecordMaxValue, value);
            BitConverter.GetBytes(value).CopyTo(Data, GetRecordOffset(recordID));
        }

        public void AddRecord(int recordID, int count = 1) => SetRecord(recordID, GetRecord(recordID) + count);
    }
}

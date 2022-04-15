using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core
{
    /// <summary>
    /// Stores 12 different sets of record data, with the earliest entry being called the "head" record index.
    /// </summary>
    /// <remarks>size: 0x5A0 (12 * 4*30)</remarks>
    public sealed class Record8b : SaveBlock<SAV8BS>, IRecordStatStorage
    {
        public const int RecordIndexCount = 12; // There's a total of 12 uint[30] record entries. The head one is used, not sure about the others.
        public const int RecordCount = 30;
        public const int RecordMaxValue = 999_999;

        public Record8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

        public static int GetMax(int recordID) => Records.MaxValue_BDSP[recordID];

        private static int ClampRecord(int recordID, int value)
        {
            var max = Records.MaxValue_BDSP[recordID];
            return Math.Min(max, value);
        }

        public int GetRecordOffset(int recordID)
        {
            if ((uint)recordID >= RecordCount)
                throw new ArgumentOutOfRangeException(nameof(recordID));
            return Offset + (sizeof(int) * recordID);
        }

        public int GetRecord(int recordID)
        {
            var value = ReadInt32LittleEndian(Data.AsSpan(GetRecordOffset(recordID)));
            if (recordID != 0)
                value = ClampRecord(recordID, value);
            return value;
        }

        public void SetRecord(int recordID, int value)
        {
            if (recordID != 0)
                value = Math.Min(RecordMaxValue, value);
            WriteInt32LittleEndian(Data.AsSpan(GetRecordOffset(recordID)), value);
        }

        public void AddRecord(int recordID, int count = 1) => SetRecord(recordID, GetRecord(recordID) + count);
    }
}

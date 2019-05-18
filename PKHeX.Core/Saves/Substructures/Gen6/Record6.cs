using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    public class Record6 : SaveBlock
    {
        public const int RecordCount = 200;
        protected readonly IReadOnlyList<byte> RecordMax;

        public Record6(SAV6 sav, int offset, IReadOnlyList<byte> maxes) : base(sav)
        {
            Offset = offset;
            RecordMax = maxes;
        }

        public int GetRecord(int recordID)
        {
            int ofs = Records.GetOffset(Offset, recordID);
            if (recordID < 100)
                return BitConverter.ToInt32(Data, ofs);
            if (recordID < 200)
                return BitConverter.ToInt16(Data, ofs);
            return 0;
        }

        public void SetRecord(int recordID, int value)
        {
            int ofs = GetRecordOffset(recordID);
            int max = GetRecordMax(recordID);
            if (value > max)
                return; // out of range, don't set value
            if (recordID < 100)
                BitConverter.GetBytes(value).CopyTo(Data, ofs);
            if (recordID < 200)
                BitConverter.GetBytes((ushort)value).CopyTo(Data, ofs);
        }

        public int GetRecordMax(int recordID) => Records.GetMax(recordID, RecordMax);
        public int GetRecordOffset(int recordID) => Records.GetOffset(Offset, recordID);
        public void AddRecord(int recordID, int count = 1) => SetRecord(recordID, GetRecord(recordID) + count);
    }
}
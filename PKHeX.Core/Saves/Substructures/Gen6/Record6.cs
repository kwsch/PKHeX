using System;
using System.Collections.Generic;
using System.Diagnostics;

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

        public Record6(SAV7 sav, int offset, IReadOnlyList<byte> maxes) : base(sav)
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
            Trace.Fail(nameof(recordID));
            return 0;
        }

        public void SetRecord(int recordID, int value)
        {
            Debug.Assert(recordID < RecordCount);
            int ofs = GetRecordOffset(recordID);
            int max = GetRecordMax(recordID);
            if (value > max)
                value = max;
            if (recordID < 100)
                BitConverter.GetBytes(value).CopyTo(Data, ofs);
            else if (recordID < 200)
                BitConverter.GetBytes((ushort)value).CopyTo(Data, ofs);
            else
                Trace.Fail(nameof(recordID));
        }

        public int GetRecordMax(int recordID) => Records.GetMax(recordID, RecordMax);
        public int GetRecordOffset(int recordID) => Records.GetOffset(Offset, recordID);
        public void AddRecord(int recordID, int count = 1) => SetRecord(recordID, GetRecord(recordID) + count);
    }
}
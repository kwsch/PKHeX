using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PKHeX.Core
{
    public sealed class Record8 : RecordBlock
    {
        public const int RecordCount = 50;
        public const int WattTotal = 22;
        protected override IReadOnlyList<byte> RecordMax { get; }

        public Record8(SaveFile sav, SCBlock block, IReadOnlyList<byte> maxes) : base(sav, block.Data) =>
            RecordMax = maxes;

        public override int GetRecord(int recordID)
        {
            int ofs = Records.GetOffset(Offset, recordID);
            if (recordID < RecordCount)
                return BitConverter.ToInt32(Data, ofs);
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
                BitConverter.GetBytes(value).CopyTo(Data, ofs);
            else
                Trace.Fail(nameof(recordID));
        }
    }
}
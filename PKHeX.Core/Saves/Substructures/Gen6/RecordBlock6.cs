using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PKHeX.Core
{
    public sealed class RecordBlock6 : RecordBlock
    {
        public const int RecordCount = 200;
        protected override IReadOnlyList<byte> RecordMax { get; }

        public RecordBlock6(SAV6XY sav, int offset) : base(sav)
        {
            Offset = offset;
            RecordMax = Records.MaxType_XY;
        }

        public RecordBlock6(SAV6AO sav, int offset) : base(sav)
        {
            Offset = offset;
            RecordMax = Records.MaxType_AO;
        }

        public RecordBlock6(SAV6AODemo sav, int offset) : base(sav)
        {
            Offset = offset;
            RecordMax = Records.MaxType_AO;
        }

        public RecordBlock6(SAV7SM sav, int offset) : base(sav)
        {
            Offset = offset;
            RecordMax = Records.MaxType_SM;
        }

        public RecordBlock6(SAV7USUM sav, int offset) : base(sav)
        {
            Offset = offset;
            RecordMax = Records.MaxType_USUM;
        }

        public override int GetRecord(int recordID)
        {
            int ofs = Records.GetOffset(Offset, recordID);
            if (recordID < 100)
                return BitConverter.ToInt32(Data, ofs);
            if (recordID < 200)
                return BitConverter.ToInt16(Data, ofs);
            Trace.Fail(nameof(recordID));
            return 0;
        }

        public override void SetRecord(int recordID, int value)
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
    }
}
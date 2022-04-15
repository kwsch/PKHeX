using System;
using System.Collections.Generic;
using System.Diagnostics;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core
{
    public sealed class RecordBlock6 : RecordBlock<SaveFile> // 6 or 7
    {
        public const int RecordCount = 200;
        protected override IReadOnlyList<byte> RecordMax { get; }

        // Structure:
        //   uint[100];
        // ushort[100];

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
            switch (recordID)
            {
                case < 100:
                    return ReadInt32LittleEndian(Data.AsSpan(ofs));
                case < 200:
                    return ReadInt16LittleEndian(Data.AsSpan(ofs));
                default:
                    Trace.Fail(nameof(recordID));
                    return 0;
            }
        }

        public override void SetRecord(int recordID, int value)
        {
            if ((uint)recordID >= RecordCount)
                throw new ArgumentOutOfRangeException(nameof(recordID));
            int ofs = GetRecordOffset(recordID);
            int max = GetRecordMax(recordID);
            if (value > max)
                value = max;
            switch (recordID)
            {
                case < 100:
                    WriteInt32LittleEndian(Data.AsSpan(ofs), value);
                    break;
                case < 200:
                    WriteUInt16LittleEndian(Data.AsSpan(ofs), (ushort)value);
                    break;
                default:
                    Trace.Fail(nameof(recordID));
                    break;
            }
        }
    }
}

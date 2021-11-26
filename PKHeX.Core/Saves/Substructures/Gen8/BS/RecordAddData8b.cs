using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Stores additional record data.
    /// </summary>
    /// <remarks>size: 0x3C0</remarks>
    public sealed class RecordAddData8b : SaveBlock
    {
        // RECORD_ADD_DATA: 0x30-sized[12] (0x240 bytes), and 12*byte[32] (0x180), total 0x3C0
        public RecordAddData8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

        private const int COUNT_RECORD_ADD = 12;
        private const int COUNT_RECORD_RANKING = 12;
        private const int COUNT_RECORD_RANKING_FLAG = 32;

        public RecordAdd8b GetRecord(int index)
        {
            if ((uint)index >= COUNT_RECORD_ADD)
                throw new ArgumentOutOfRangeException(nameof(index));
            return new RecordAdd8b(Data, Offset + (index * RecordAdd8b.SIZE));
        }

        public RecordAdd8b[] GetRecords()
        {
            var result = new RecordAdd8b[COUNT_RECORD_ADD];
            for (int i = 0; i < result.Length; i++)
                result[i] = GetRecord(i);
            return result;
        }

        public void ReplaceOT(ITrainerInfo oldTrainer, ITrainerInfo newTrainer)
        {
            foreach (var r in GetRecords())
            {
                if (string.IsNullOrWhiteSpace(r.OT))
                    continue;

                if (oldTrainer.OT != r.OT || oldTrainer.TID != r.TID || oldTrainer.SID != r.SID)
                    continue;

                r.OT = newTrainer.OT;
                r.SID = newTrainer.SID;
                r.TID = newTrainer.TID;
            }
        }
    }

    public sealed class RecordAdd8b
    {
        public const int SIZE = 0x30;

        public readonly byte[] Data;
        private readonly int Offset;

        public RecordAdd8b(byte[] data, int offset)
        {
            Data = data;
            Offset = offset;
        }
        public string OT
        {
            get => StringConverter.GetString7b(Data, Offset + 0, 0x1A);
            set => StringConverter.SetString7b(value, 12, 12).CopyTo(Data, Offset);
        }
        // 1A reserved byte
        // 1B reserved byte

        public int Language
        {
            get => BitConverter.ToInt32(Data, Offset + 0x1C);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x1C);
        }

        public byte Gender { get => Data[Offset + 0x20]; set => Data[Offset + 0x20] = value; }
        // 21
        // 22
        // 23

        public int BodyType
        {
            get => BitConverter.ToInt32(Data, Offset + 0x24);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x24);
        }

        public int TID
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x28);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 0x28);
        }

        public int SID
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x2A);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 0x2A);
        }

        // 0x2C int32 reserved
    }
}

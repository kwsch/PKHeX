using System;
using System.Text;

namespace PKHeX.Core
{
    public class WB7Records : SaveBlock
    {
        public WB7Records(SaveFile sav) : base(sav)
        {
            Offset = ((SAV7b) sav).GetBlockOffset(BelugaBlockIndex.WB7Record);
        }

        private const int RecordMax = 10; // 0xE90 > (0x140 * 0xA = 0xC80), not sure what final 0x210 bytes are used for
        private const int FlagCountMax = 0x1C00; // (7168) end of the block?

        private int FlagStart => Offset + (RecordMax * WB7Record.SIZE);

        private int GetRecordOffset(int index)
        {
            if (index >= RecordMax)
                throw new ArgumentException(nameof(index));

            return Offset + (index * WB7Record.SIZE);
        }

        private int GetFlagOffset(int flag)
        {
            if (flag >= FlagCountMax)
                throw new ArgumentException(nameof(flag));
            return FlagStart + (flag / 8);
        }

        public WB7Record GetRecord(int index)
        {
            int ofs = GetRecordOffset(index);
            byte[] data = new byte[WB7Record.SIZE];
            Array.Copy(Data, ofs, data, 0, WB7Record.SIZE);
            return new WB7Record(data);
        }

        public void SetRecord(WB7Record record, int index)
        {
            int ofs = GetRecordOffset(index);
            record.Data.CopyTo(Data, ofs);
        }

        public WB7Record[] Records
        {
            get
            {
                var arr = new WB7Record[RecordMax];
                for (int i = 0; i < arr.Length; i++)
                    arr[i] = GetRecord(i);
                return arr;
            }
            set
            {
                for (int i = 0; i < value.Length; i++)
                    SetRecord(value[i], i);
            }
        }

        public bool GetFlag(int flag)
        {
            int ofs = GetFlagOffset(flag);
            var mask = 1 << (flag & 7);
            return (Data[ofs] & mask) != 0;
        }

        public void SetFlag(int flag)
        {
            int ofs = GetFlagOffset(flag);
            var mask = 1 << (flag & 7);
            Data[ofs] |= (byte)mask;
        }
    }

    public class WB7Record
    {
        public const int SIZE = 0x140;
        public readonly byte[] Data;

        public WB7Record(byte[] data) => Data = data;

        public uint Epoch
        {
            get => BitConverter.ToUInt32(Data, 0x00);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x00);
        }

        public DateTime Date => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Epoch);

        public ushort CardID
        {
            get => BitConverter.ToUInt16(Data, 0x08);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x08);
        }

        public ushort CardType
        {
            get => BitConverter.ToUInt16(Data, 0x0A);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x0A);
        }

        public WB7GiftType GiftType
        {
            get => (WB7GiftType)Data[0x0C];
            set => Data[0x0C] = (byte)value;
        }

        public int ItemCount
        {
            get => Data[0x0D];
            set => Data[0x0D] = (byte)value;
        }

        public ushort Species
        {
            get => BitConverter.ToUInt16(Data, 0x10C);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x10C);
        }

        public ushort ItemSetCount
        {
            get => BitConverter.ToUInt16(Data, 0x10E);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x10E);
        }

        public ushort ItemSet1Item  { get => BitConverter.ToUInt16(Data, 0x110); set => BitConverter.GetBytes(value).CopyTo(Data, 0x110); }
        public ushort ItemSet1Count { get => BitConverter.ToUInt16(Data, 0x112); set => BitConverter.GetBytes(value).CopyTo(Data, 0x112); }
        public ushort ItemSet2Item  { get => BitConverter.ToUInt16(Data, 0x114); set => BitConverter.GetBytes(value).CopyTo(Data, 0x114); }
        public ushort ItemSet2Count { get => BitConverter.ToUInt16(Data, 0x116); set => BitConverter.GetBytes(value).CopyTo(Data, 0x116); }
        public ushort ItemSet3Item  { get => BitConverter.ToUInt16(Data, 0x118); set => BitConverter.GetBytes(value).CopyTo(Data, 0x118); }
        public ushort ItemSet3Count { get => BitConverter.ToUInt16(Data, 0x11A); set => BitConverter.GetBytes(value).CopyTo(Data, 0x11A); }
        public ushort ItemSet4Item  { get => BitConverter.ToUInt16(Data, 0x11C); set => BitConverter.GetBytes(value).CopyTo(Data, 0x11C); }
        public ushort ItemSet4Count { get => BitConverter.ToUInt16(Data, 0x11E); set => BitConverter.GetBytes(value).CopyTo(Data, 0x11E); }
        public ushort ItemSet5Item  { get => BitConverter.ToUInt16(Data, 0x120); set => BitConverter.GetBytes(value).CopyTo(Data, 0x120); } // struct union overlaps OT Name data, beware!
        public ushort ItemSet5Count { get => BitConverter.ToUInt16(Data, 0x122); set => BitConverter.GetBytes(value).CopyTo(Data, 0x122); }
        public ushort ItemSet6Item  { get => BitConverter.ToUInt16(Data, 0x124); set => BitConverter.GetBytes(value).CopyTo(Data, 0x124); }
        public ushort ItemSet6Count { get => BitConverter.ToUInt16(Data, 0x126); set => BitConverter.GetBytes(value).CopyTo(Data, 0x126); }

        public string OT_Name
        {
            get => Util.TrimFromZero(Encoding.Unicode.GetString(Data, 0x120, 0x1A));
            set => Encoding.Unicode.GetBytes(value.PadRight(value.Length + 1, '\0')).CopyTo(Data, 0x120 + 0xB6); // careful with length
        }

        public LanguageID LanguageReceived
        {
            get => (LanguageID)Data[0x13A];
            set => Data[0x13A] = (byte)value;
        }
    }

    public enum WB7GiftType : byte
    {
        None = 0,
        Pokemon = 1,
        Item = 2,
    }
}

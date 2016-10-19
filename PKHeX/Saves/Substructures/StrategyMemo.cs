using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX
{
    public class StrategyMemo
    {
        private readonly bool XD;
        private const int SIZE_ENTRY = 12;
        private readonly List<StrategyMemoEntry> Entries = new List<StrategyMemoEntry>();
        private StrategyMemoEntry this[int Species] => Entries.FirstOrDefault(e => e.Species == Species);

        public StrategyMemo(byte[] input, int offset, bool xd)
        {
            XD = xd;
            int count = BigEndian.ToInt16(input, offset);
            if (count > 500)
                count = 500;
            for (int i = 0; i < count; i++)
            {
                byte[] data = new byte[SIZE_ENTRY];
                Array.Copy(input, 4 + offset + SIZE_ENTRY * i, data, 0, SIZE_ENTRY);
                Entries.Add(new StrategyMemoEntry(XD, data));
            }
        }
        public byte[] FinalData => BigEndian.GetBytes(Entries.Count) // count followed by populated entries
            .Concat(Entries.Where(entry => entry.Species != 0).SelectMany(entry => entry.Data)).ToArray();

        public StrategyMemoEntry GetEntry(int Species)
        {
            return this[Species] ?? new StrategyMemoEntry(XD);
        }
        public void SetEntry(StrategyMemoEntry entry)
        {
            int index = Array.FindIndex(Entries.ToArray(), ent => ent.Species == entry.Species);
            if (index > 0)
                Entries[index] = entry;
            else
                Entries.Add(entry);
        }
        public class StrategyMemoEntry
        {
            public readonly byte[] Data;
            private readonly bool XD;
            public StrategyMemoEntry(bool xd, byte[] data = null)
            {
                Data = data ?? new byte[SIZE_ENTRY];
                XD = xd;
            }

            public int Species
            {
                get
                {
                    int val = BigEndian.ToUInt16(Data, 0) & 0x1FF;
                    return PKX.getG4Species(val);
                }
                set
                {
                    value = PKX.getG3Species(value);
                    int cval = BigEndian.ToUInt16(Data, 0);
                    cval &= 0xE00; value &= 0x1FF; cval |= value;
                    BigEndian.GetBytes((ushort)cval).CopyTo(Data, 0);
                }
            }
            private bool Flag0 { get { return Data[0] >> 6 == 1; } set { Data[0] &= 0xBF; if (value) Data[0] |= 0x40; } } // Unused
            private bool Flag1 { get { return Data[0] >> 7 == 1; } set { Data[0] &= 0x7F; if (value) Data[0] |= 0x80; } } // Complete Entry
            public int SID { get { return BigEndian.ToUInt16(Data, 4); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 4); } }
            public int TID { get { return BigEndian.ToUInt16(Data, 6); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 6); } }
            public uint PID { get { return BigEndian.ToUInt32(Data, 8); } set { BigEndian.GetBytes(value).CopyTo(Data, 8); } }

            public bool Seen
            {
                get
                {
                    if (XD) return !Flag1;
                    return Species != 0;
                }
                set
                {
                    if (XD)
                        Flag1 = !value;
                    else if (!value)
                        new byte[SIZE_ENTRY].CopyTo(Data, 0);
                }
            }
            public bool Owned
            {
                get
                {
                    if (XD) return false;
                    return Flag0 | Flag1 == false;
                }
                set
                {
                    if (XD) return;
                    if (!value)
                        Flag1 = true;
                }
            }

            public bool IsEmpty => Species == 0;
            public bool Matches(int species, uint pid, int tid, int sid) => Species == species && PID == pid && TID == tid && SID == sid;
        }
    }
}

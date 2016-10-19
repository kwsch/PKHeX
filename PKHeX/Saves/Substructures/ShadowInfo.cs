using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX
{
    public class ShadowInfoTableXD
    {
        private readonly List<ShadowInfoEntryXD> Entries;
        private readonly int MaxLength;
        public ShadowInfoTableXD(byte[] data)
        {
            Entries = new List<ShadowInfoEntryXD>();
            MaxLength = data.Length;
            const int eSize = ShadowInfoEntryXD.SIZE_ENTRY;
            int eCount = data.Length/eSize;
            for (int i = 0; i < eCount; i++)
            {
                byte[] d = new byte[eSize];
                Array.Copy(data, i*eSize, d, 0, eSize);

                var entry = new ShadowInfoEntryXD(d);
                if (entry.Species != 0)
                    Entries.Add(entry);
            }
        }
        public byte[] FinalData => Entries.SelectMany(entry => entry.Data).Take(MaxLength).ToArray();
        public ShadowInfoEntryXD GetEntry(int Species, uint PID)
        {
            return Entries.FirstOrDefault(entry => entry.PID == PID && entry.Species == Species) ?? new ShadowInfoEntryXD();
        }
        public void SetEntry(ShadowInfoEntryXD Entry)
        {
            var entry = GetEntry(Entry.Species, Entry.PID);
            if (entry.IsEmpty)
                return;

            int index = Array.FindIndex(Entries.ToArray(), ent => ent.Species == entry.Species);
            if (index > 0)
                Entries[index] = entry;
            else
                Entries.Add(entry);
        }
        public ShadowInfoEntryXD this[int index] { get { return Entries[index]; } set { Entries[index] = value; } }
        public int Count => Entries.Count;
    }

    public class ShadowInfoEntryXD
    {
        public byte[] Data { get; private set; }
        internal const int SIZE_ENTRY = 72;
        public ShadowInfoEntryXD(byte[] data = null)
        {
            Data = (byte[])(data?.Clone() ?? new byte[SIZE_ENTRY]);
        }

        public bool IsSnagged => Data[0] >> 6 != 0;
        public bool IsPurified { get { return Data[0] >> 7 == 1; } set { Data[0] &= 0x7F; if (value) Data[0] |= 0x80; } }
        public int Species { get { return BigEndian.ToUInt16(Data, 0x1A); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x1A); } }
        public uint PID { get { return BigEndian.ToUInt32(Data, 0x1C); } set { BigEndian.GetBytes(value).CopyTo(Data, 0x1C); } }
        public int Purification { get { return BigEndian.ToInt32(Data, 0x24); } set { BigEndian.GetBytes(value).CopyTo(Data, 0x24); } }

        public uint EXP { get { return BigEndian.ToUInt32(Data, 0x04) >> 12; } set { BigEndian.GetBytes((BigEndian.ToUInt32(Data, 0x04) & 0xFFF) | (value << 12)).CopyTo(Data, 0x04); } }
        public bool IsEmpty => Species == 0;
    }

    public class ShadowInfoEntryColo
    {
        public byte[] Data { get; private set; }
        internal const int SIZE_ENTRY = 12;
        public ShadowInfoEntryColo(byte[] data = null)
        {
            Data = (byte[])(data?.Clone() ?? new byte[SIZE_ENTRY]);
        }
        public uint PID { get { return BigEndian.ToUInt32(Data, 0x00); } set { BigEndian.GetBytes(value).CopyTo(Data, 0x00); } }
        public int Met_Location { get { return BigEndian.ToUInt16(Data, 0x06); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x06); } }
        public uint _0x08 { get { return BigEndian.ToUInt32(Data, 0x08); } set { BigEndian.GetBytes(value).CopyTo(Data, 0x08); } }
    }
}
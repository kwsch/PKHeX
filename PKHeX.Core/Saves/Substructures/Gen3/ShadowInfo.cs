using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public sealed class ShadowInfoTableXD
    {
        private readonly List<ShadowInfoEntryXD> Entries;
        private readonly int MaxLength;
        private const int SIZE_ENTRY = ShadowInfoEntryXD.SIZE_ENTRY;

        public ShadowInfoTableXD(byte[] data)
        {
            MaxLength = data.Length;
            int eCount = data.Length/SIZE_ENTRY;
            Entries = new List<ShadowInfoEntryXD>(eCount);
            for (int i = 0; i < eCount; i++)
            {
                var entry = GetEntry(data, i);
                //if (entry.Species != 0)
                    Entries.Add(entry);
            }
        }

        public ShadowInfoTableXD() : this(new byte[SIZE_ENTRY * 200]) { }

        private static ShadowInfoEntryXD GetEntry(byte[] data, int i)
        {
            var d = new byte[SIZE_ENTRY];
            Array.Copy(data, i * SIZE_ENTRY, d, 0, SIZE_ENTRY);

            var entry = new ShadowInfoEntryXD(d);
            return entry;
        }

        public byte[] Write() => Entries.SelectMany(entry => entry.Data).Take(MaxLength).ToArray();

        public ShadowInfoEntryXD GetEntry(int Species, uint PID)
        {
            return Entries.Find(entry => entry.PID == PID && entry.Species == Species) ?? new ShadowInfoEntryXD();
        }

        public void SetEntry(ShadowInfoEntryXD Entry)
        {
            var entry = GetEntry(Entry.Species, Entry.PID);
            if (entry.IsEmpty)
                return;

            int index = Entries.FindIndex(ent => ent.Species == entry.Species);
            if (index >= 0)
                Entries[index] = entry;
            else
                Entries.Add(entry);
        }

        public ShadowInfoEntryXD this[int index] { get => Entries[index]; set => Entries[index] = value; }
        public int Count => Entries.Count;
    }

    public sealed class ShadowInfoEntryXD
    {
        public readonly byte[] Data;
        internal const int SIZE_ENTRY = 72;

        public ShadowInfoEntryXD() => Data = new byte[SIZE_ENTRY];
        public ShadowInfoEntryXD(byte[] data) => Data = data;

        public bool IsSnagged => Data[0] >> 6 != 0;
        public bool IsPurified { get => Data[0] >> 7 == 1; set { Data[0] &= 0x7F; if (value) Data[0] |= 0x80; } }
        public int Species { get => BigEndian.ToUInt16(Data, 0x1A); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x1A); }
        public uint PID { get => BigEndian.ToUInt32(Data, 0x1C); set => BigEndian.GetBytes(value).CopyTo(Data, 0x1C); }
        public int Purification { get => BigEndian.ToInt32(Data, 0x24); set => BigEndian.GetBytes(value).CopyTo(Data, 0x24); }

        public uint EXP { get => BigEndian.ToUInt32(Data, 0x04) >> 12; set => BigEndian.GetBytes((BigEndian.ToUInt32(Data, 0x04) & 0xFFF) | (value << 12)).CopyTo(Data, 0x04); }
        public bool IsEmpty => Species == 0;
    }

    public sealed class ShadowInfoEntryColo
    {
        public readonly byte[] Data;
        private const int SIZE_ENTRY = 12;

        public ShadowInfoEntryColo() => Data = new byte[SIZE_ENTRY];
        public ShadowInfoEntryColo(byte[] data) => Data = data;

        public uint PID { get => BigEndian.ToUInt32(Data, 0x00); set => BigEndian.GetBytes(value).CopyTo(Data, 0x00); }
        public int Met_Location { get => BigEndian.ToUInt16(Data, 0x06); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x06); }
        public uint Unk08 { get => BigEndian.ToUInt32(Data, 0x08); set => BigEndian.GetBytes(value).CopyTo(Data, 0x08); }
    }
}
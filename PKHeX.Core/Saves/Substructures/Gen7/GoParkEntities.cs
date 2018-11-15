using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace PKHeX.Core
{
    public sealed class GoParkEntities : SaveBlock, IEnumerable<GoPKM>
    {
        public GoParkEntities(SaveFile sav) : base(sav)
        {
            Offset = ((SAV7b)sav).GetBlockOffset(BelugaBlockIndex.GoParkEntities);
        }

        public const int SlotsPerArea = 50;
        public const int Areas = 20;
        public const int Count = SlotsPerArea * Areas; // 1000

        public GoPKM this[int index]
        {
            get
            {
                Debug.Assert(index < Count);
                return GoPKM.FromData(Data, Offset + (GoPKM.SIZE * index));
            }
            set
            {
                Debug.Assert(index < Count);
                value.WriteTo(Data, Offset + (GoPKM.SIZE * index));
            }
        }

        public GoPKM[] AllEntities
        {
            get
            {
                var value = new GoPKM[Count];
                for (int i = 0; i < value.Length; i++)
                    value[i] = this[i];
                return value;
            }
            set
            {
                Debug.Assert(value?.Length == Count);
                for (int i = 0; i < value.Length; i++)
                    this[i] = value[i];
            }
        }

        public IEnumerator<GoPKM> GetEnumerator() => (IEnumerator<GoPKM>)AllEntities.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => AllEntities.GetEnumerator();
    }

    public class GoPKM
    {
        public const int SIZE = 0x1B0;
        public byte[] Data { get; set; }
        public GoPKM() => Data = (byte[])Blank.Clone();
        public void WriteTo(byte[] data, int offset) => Data.CopyTo(data, offset);

        public static GoPKM FromData(byte[] data, int offset)
        {
            var gpkm = new GoPKM();
            Array.Copy(data, offset, gpkm.Data, 0, SIZE);
            return gpkm;
        }

        public static readonly byte[] Blank = GetBlank();

        private static readonly byte[] Blank20 =
        {
            0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x80, 0x3F, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x3F,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x3F, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x80, 0x3F,
            0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x85, 0xEC, 0x33, 0x01,
        };

        public static byte[] GetBlank()
        {
            byte[] data = new byte[SIZE];
            Blank20.CopyTo(data, 0x20);
            return data;
        }
    }
}
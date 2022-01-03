using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core
{
    public sealed class ItemInfo6 : SaveBlock
    {
        public ItemInfo6(SaveFile sav, int offset) : base(sav) => Offset = offset;

        private const int BoundItemCount = 4;
        private const int RecentItemCount = 12;

        public int[] SelectItems
        {
            // UP,RIGHT,DOWN,LEFT
            get
            {
                var span = Data.AsSpan(Offset + 10);
                int[] list = new int[BoundItemCount];
                for (int i = 0; i < list.Length; i++)
                    list[i] = ReadUInt16LittleEndian(span[(2 * i)..]);
                return list;
            }
            set
            {
                if (value.Length != BoundItemCount)
                    throw new ArgumentException(nameof(value));
                var span = Data.AsSpan(Offset + 10);
                for (int i = 0; i < value.Length; i++)
                    WriteUInt16LittleEndian(span[(2 * i)..], (ushort)value[i]);
            }
        }

        public int[] RecentItems
        {
            // Items recently interacted with (Give, Use)
            get
            {
                var span = Data.AsSpan(Offset + 20);
                int[] list = new int[RecentItemCount];
                for (int i = 0; i < list.Length; i++)
                    list[i] = ReadUInt16LittleEndian(span[(2 * i)..]);
                return list;
            }
            set
            {
                if (value.Length != RecentItemCount)
                    throw new ArgumentException(nameof(value));
                var span = Data.AsSpan(Offset + 20);
                for (int i = 0; i < value.Length; i++)
                    WriteUInt16LittleEndian(span[(2 * i)..], (ushort)value[i]);
            }
        }
    }
}

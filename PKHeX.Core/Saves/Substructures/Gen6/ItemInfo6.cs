using System;

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
                int[] list = new int[BoundItemCount];
                for (int i = 0; i < list.Length; i++)
                    list[i] = BitConverter.ToUInt16(Data, Offset + 10 + (2 * i));
                return list;
            }
            set
            {
                if (value.Length != BoundItemCount)
                    throw new ArgumentException(nameof(value));
                for (int i = 0; i < value.Length; i++)
                    BitConverter.GetBytes((ushort)value[i]).CopyTo(Data, Offset + 10 + (2 * i));
            }
        }

        public int[] RecentItems
        {
            // Items recently interacted with (Give, Use)
            get
            {
                int[] list = new int[RecentItemCount];
                for (int i = 0; i < list.Length; i++)
                    list[i] = BitConverter.ToUInt16(Data, Offset + 20 + (2 * i));
                return list;
            }
            set
            {
                if (value.Length != RecentItemCount)
                    throw new ArgumentException(nameof(value));
                for (int i = 0; i < value.Length; i++)
                    BitConverter.GetBytes((ushort)value[i]).CopyTo(Data, Offset + 20 + (2 * i));
            }
        }
    }
}
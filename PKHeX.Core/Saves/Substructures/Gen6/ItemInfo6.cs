using System;

namespace PKHeX.Core
{
    public sealed class ItemInfo6 : SaveBlock
    {
        public ItemInfo6(SaveFile sav, int offset) : base(sav) => Offset = offset;

        public int[] SelectItems
        {
            // UP,RIGHT,DOWN,LEFT
            get
            {
                int[] list = new int[4];
                for (int i = 0; i < list.Length; i++)
                    list[i] = BitConverter.ToUInt16(Data, Offset + 10 + (2 * i));
                return list;
            }
            set
            {
                if (value == null || value.Length > 4)
                    return;
                for (int i = 0; i < value.Length; i++)
                    BitConverter.GetBytes((ushort)value[i]).CopyTo(Data, Offset + 10 + (2 * i));
            }
        }

        public int[] RecentItems
        {
            // Items recently interacted with (Give, Use)
            get
            {
                int[] list = new int[12];
                for (int i = 0; i < list.Length; i++)
                    list[i] = BitConverter.ToUInt16(Data, Offset + 20 + (2 * i));
                return list;
            }
            set
            {
                if (value == null || value.Length > 12)
                    return;
                for (int i = 0; i < value.Length; i++)
                    BitConverter.GetBytes((ushort)value[i]).CopyTo(Data, Offset + 20 + (2 * i));
            }
        }
    }
}
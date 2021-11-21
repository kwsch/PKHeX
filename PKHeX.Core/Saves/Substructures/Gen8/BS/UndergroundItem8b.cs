using System;

namespace PKHeX.Core
{
    public sealed class UndergroundItem8b
    {
        private const int SIZE = 0xC;
        public readonly int Index; // not serialized
        public UgItemType Type => UgItemUtil.GetType(Index);
        public int MaxValue => UgItemUtil.GetMax(Index);

        public int Count { get; set; }
        public bool HideNewFlag { get; set; }
        public bool IsFavoriteFlag { get; set; }

        public UndergroundItem8b(byte[] data, int baseOffset, int index)
        {
            Index = index;
            var offset = baseOffset + (SIZE * index);
            Count = BitConverter.ToInt32(data, offset + 0);
            HideNewFlag = BitConverter.ToUInt32(data, offset + 4) == 1;
            IsFavoriteFlag = BitConverter.ToUInt32(data, offset + 8) == 1;
        }

        public void Write(byte[] data, int baseOffset)
        {
            var offset = baseOffset + (SIZE * Index);
            BitConverter.GetBytes(Count).CopyTo(data, offset + 0);
            BitConverter.GetBytes(HideNewFlag ? 1u : 0u).CopyTo(data, offset + 4);
            BitConverter.GetBytes(IsFavoriteFlag ? 1u : 0u).CopyTo(data, offset + 8);
        }
    }
}

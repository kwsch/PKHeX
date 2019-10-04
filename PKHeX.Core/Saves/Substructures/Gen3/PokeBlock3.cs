using System;

namespace PKHeX.Core
{
    public sealed class PokeBlock3
    {
        public const int SIZE = 7;
        private readonly byte[] Data;
        public PokeBlock3(byte[] data) => Data = data;

        public PokeBlock3Color Color { get => (PokeBlock3Color)Data[0]; set => Data[0] = (byte)value; }
        public int Spicy { get => Data[1]; set => Data[1] = (byte)value; }
        public int Dry { get => Data[2]; set => Data[2] = (byte)value; }
        public int Sweet { get => Data[3]; set => Data[3] = (byte)value; }
        public int Bitter { get => Data[4]; set => Data[4] = (byte)value; }
        public int Sour { get => Data[5]; set => Data[5] = (byte)value; }
        public int Feel { get => Data[6]; set => Data[6] = (byte)value; }

        public void Delete()
        {
            for (int i = 0; i < Data.Length; i++)
                Data[i] = 0;
        }

        public void Maximize(bool create = false)
        {
            if (Color == 0 && !create)
                return;
            Spicy = Dry = Sweet = Bitter = Sour = Feel = 255;
            Color = PokeBlock3Color.Gold;
        }

        public void SetBlock(byte[] data, int offset) => Data.CopyTo(data, offset);

        public static PokeBlock3 GetBlock(byte[] data, int offset)
        {
            byte[] result = new byte[SIZE];
            Array.Copy(data, offset, result, 0, SIZE);
            return new PokeBlock3(result);
        }
    }
}
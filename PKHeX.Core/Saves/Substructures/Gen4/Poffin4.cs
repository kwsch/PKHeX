using System;

namespace PKHeX.Core
{
    public class Poffin4
    {
        public const int SIZE = 8;
        public readonly byte[] Data;

        public Poffin4(byte[] data, int offset)
        {
            Data = new byte[SIZE];
            Array.Copy(data, offset, data, 0, SIZE);
        }

        public Poffin4(byte[] data) => Data = data;

        public PoffinFlavor4 Type{ get => (PoffinFlavor4)Data[0]; set => Data[0] = (byte)value; }
        public byte BoostSpicy  { get => Data[1]; set => Data[1] = value; }
        public byte BoostDry    { get => Data[2]; set => Data[2] = value; }
        public byte BoostSweet  { get => Data[3]; set => Data[3] = value; }
        public byte BoostBitter { get => Data[4]; set => Data[4] = value; }
        public byte BoostSour   { get => Data[5]; set => Data[5] = value; }
        public byte Smoothness  { get => Data[6]; set => Data[6] = value; }
        // public byte Unused   { get => Data[7]; set => Data[7] = value; }

        public bool IsManyStat => Type >= PoffinFlavor4.Rich;
        public int StatPrimary => IsManyStat ? -1 : (byte)Type / 5;
        public int StatSecondary => IsManyStat ? -1 : (byte)Type % 5;

        public void SetAll(byte value = 255, PoffinFlavor4 type = PoffinFlavor4.Rich)
        {
            Type = type;
            BoostSpicy = BoostDry = BoostSweet = BoostBitter = BoostSour = Smoothness = value;
        }

        public void SetStat(int stat, byte value)
        {
            if ((uint) stat > 5)
                throw new ArgumentException(nameof(stat));
            Data[1 + stat] = value;
        }

        public byte GetStat(int stat)
        {
            if ((uint)stat > 5)
                throw new ArgumentException(nameof(stat));
            return Data[1 + stat];
        }

        public void Delete() => SetAll(0, PoffinFlavor4.None);
    }
}

using System;

namespace PKHeX.Core
{
    public sealed class WC7Full
    {
        public const int Size = 0x310;
        private const int GiftStart = Size - WC7.Size;
        public readonly byte[] Data;
        public readonly WC7 Gift;

        public byte RestrictVersion { get => Data[0]; set => Data[0] = value; }
        public byte RestrictLanguage { get => Data[0x1FF]; set => Data[0x1FF] = value; }

        public WC7Full(byte[] data)
        {
            Data = data;
            var wc7 = data.SliceEnd(GiftStart);
            Gift = new WC7(wc7);
            var now = DateTime.Now;
            Gift.RawDate = WC7.SetDate((uint)now.Year, (uint)now.Month, (uint)now.Day);

            Gift.RestrictVersion = RestrictVersion;
            Gift.RestrictLanguage = RestrictLanguage;
        }

        public static WC7[] GetArray(byte[] wc7Full, byte[] data)
        {
            var countfull = wc7Full.Length / Size;
            var countgift = data.Length / WC7.Size;
            var result = new WC7[countfull + countgift];

            var now = DateTime.Now;
            for (int i = 0; i < countfull; i++)
                result[i] = ReadWC7(wc7Full, i * Size, now);
            for (int i = 0; i < countgift; i++)
                result[i + countfull] = ReadWC7Only(data, i * WC7.Size);

            return result;
        }

        private static WC7 ReadWC7(byte[] data, int ofs, DateTime date)
        {
            var slice = data.Slice(ofs + GiftStart, WC7.Size);
            return new WC7(slice)
            {
                RestrictVersion = data[ofs],
                RestrictLanguage = data[ofs + 0x1FF],
                RawDate = WC7.SetDate((uint) date.Year, (uint) date.Month, (uint) date.Day)
            };
        }

        private static WC7 ReadWC7Only(byte[] data, int ofs)
        {
            var slice = data.Slice(ofs, WC7.Size);
            return new WC7(slice);
        }
    }
}
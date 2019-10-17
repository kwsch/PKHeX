using System;

namespace PKHeX.Core
{
    public sealed class WC7Full
    {
        public const int Size = 0x310;
        public readonly byte[] Data;
        public readonly WC7 Gift;

        public byte RestrictVersion { get => Data[0]; set => Data[0] = value; }
        public byte RestrictLanguage { get => Data[0x1FF]; set => Data[0x1FF] = value; }

        public WC7Full(byte[] data)
        {
            Data = data;
            var wc7 = data.SliceEnd(Size - WC7.Size);
            Gift = new WC7(wc7);
            var now = DateTime.Now;
            Gift.RawDate = WC7.SetDate((uint)now.Year, (uint)now.Month, (uint)now.Day);

            Gift.RestrictVersion = RestrictVersion;
            Gift.RestrictLanguage = RestrictLanguage;
        }
    }
}
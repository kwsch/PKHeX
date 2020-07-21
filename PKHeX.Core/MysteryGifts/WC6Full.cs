using System;

namespace PKHeX.Core
{
    public sealed class WC6Full
    {
        public const int Size = 0x310;
        public readonly byte[] Data;
        public readonly WC6 Gift;

        public byte RestrictVersion { get => Data[0]; set => Data[0] = value; }
        public byte RestrictLanguage { get => Data[0x1FF]; set => Data[0x1FF] = value; }

        public WC6Full(byte[] data)
        {
            Data = data;
            var wc6 = data.SliceEnd(Size - WC6.Size);
            Gift = new WC6(wc6);
            var now = DateTime.Now;
            Gift.RawDate = WC6.SetDate((uint)now.Year, (uint)now.Month, (uint)now.Day);

            Gift.RestrictVersion = RestrictVersion;
            Gift.RestrictLanguage = RestrictLanguage;
        }
    }
}
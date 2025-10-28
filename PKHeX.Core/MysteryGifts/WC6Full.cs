using System;

namespace PKHeX.Core;

public sealed class WC6Full
{
    public const int Size = 0x310;
    private const int GiftStart = Size - WC6.Size;
    public readonly WC6 Gift;

    public readonly Memory<byte> Raw;
    public Span<byte> Data => Raw.Span;

    public byte RestrictVersion { get => Data[0]; set => Data[0] = value; }
    public byte RestrictLanguage { get => Data[0x1FF]; set => Data[0x1FF] = value; }

    public WC6Full(Memory<byte> raw)
    {
        Raw = raw[..Size];
        var wc6 = raw[GiftStart..];
        Gift = new WC6(wc6);
        var now = EncounterDate.GetDate3DS();
        Gift.RawDate = GetDate(now);

        Gift.RestrictVersion = RestrictVersion;
        Gift.RestrictLanguage = RestrictLanguage;
    }

    private static uint GetDate(DateOnly date) => WC6.SetDate((uint)date.Year, (uint)date.Month, (uint)date.Day);

    public static WC6[] GetArray(Memory<byte> WC6Full, Memory<byte> data)
    {
        var countfull = WC6Full.Length / Size;
        var countgift = data.Length / WC6.Size;
        var result = new WC6[countfull + countgift];

        var now = EncounterDate.GetDate3DS();
        for (int i = 0; i < countfull; i++)
            result[i] = ReadWC6(WC6Full, i * Size, now);
        for (int i = 0; i < countgift; i++)
            result[i + countfull] = ReadWC6Only(data, i * WC6.Size);

        return result;
    }

    private static WC6 ReadWC6(Memory<byte> data, int ofs, DateOnly date)
    {
        var slice = data.Slice(ofs + GiftStart, WC6.Size);
        var span = data.Span[ofs..];
        return new WC6(slice)
        {
            RestrictVersion = span[0],
            RestrictLanguage = span[0x1FF],
            RawDate = GetDate(date),
        };
    }

    private static WC6 ReadWC6Only(Memory<byte> data, int ofs)
    {
        var slice = data.Slice(ofs, WC6.Size);
        return new WC6(slice);
    }
}

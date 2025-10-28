using System;

namespace PKHeX.Core;

public sealed class WC7Full
{
    public const int Size = 0x310;
    private const int GiftStart = Size - WC7.Size;
    public readonly WC7 Gift;

    public readonly Memory<byte> Raw;
    public Span<byte> Data => Raw.Span;

    public byte RestrictVersion { get => Data[0]; set => Data[0] = value; }
    public byte RestrictLanguage { get => Data[0x1FF]; set => Data[0x1FF] = value; }

    public WC7Full(Memory<byte> raw)
    {
        Raw = raw[..Size];
        var wc7 = raw[GiftStart..];
        Gift = new WC7(wc7);
        var now = EncounterDate.GetDate3DS();
        Gift.RawDate = GetDate(now);

        Gift.RestrictVersion = RestrictVersion;
        Gift.RestrictLanguage = RestrictLanguage;
    }

    private static uint GetDate(DateOnly date) => WC7.SetDate((uint)date.Year, (uint)date.Month, (uint)date.Day);

    public static WC7[] GetArray(Memory<byte> wc7Full, Memory<byte> data)
    {
        var countfull = wc7Full.Length / Size;
        var countgift = data.Length / WC7.Size;
        var result = new WC7[countfull + countgift];

        var now = EncounterDate.GetDate3DS();
        for (int i = 0; i < countfull; i++)
            result[i] = ReadWC7(wc7Full, i * Size, now);
        for (int i = 0; i < countgift; i++)
            result[i + countfull] = ReadWC7Only(data, i * WC7.Size);

        return result;
    }

    private static WC7 ReadWC7(Memory<byte> data, int ofs, DateOnly date)
    {
        var slice = data.Slice(ofs + GiftStart, WC7.Size);
        var span = data.Span[ofs..];
        return new WC7(slice)
        {
            RestrictVersion = span[0],
            RestrictLanguage = span[0x1FF],
            RawDate = GetDate(date),
        };
    }

    private static WC7 ReadWC7Only(Memory<byte> data, int ofs)
    {
        var slice = data.Slice(ofs, WC7.Size);
        return new WC7(slice);
    }
}

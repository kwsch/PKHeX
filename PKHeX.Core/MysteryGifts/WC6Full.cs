using System;

namespace PKHeX.Core;

public sealed class WC6Full
{
    public const int Size = 0x310;
    private const int GiftStart = Size - WC6.Size;
    public readonly byte[] Data;
    public readonly WC6 Gift;

    public byte RestrictVersion { get => Data[0]; set => Data[0] = value; }
    public byte RestrictLanguage { get => Data[0x1FF]; set => Data[0x1FF] = value; }

    public WC6Full(byte[] data)
    {
        Data = data;
        var wc6 = data.AsSpan(GiftStart).ToArray();
        Gift = new WC6(wc6);
        var now = EncounterDate.GetDate3DS();
        Gift.RawDate = WC6.SetDate((uint)now.Year, (uint)now.Month, (uint)now.Day);

        Gift.RestrictVersion = RestrictVersion;
        Gift.RestrictLanguage = RestrictLanguage;
    }

    public static WC6[] GetArray(ReadOnlySpan<byte> WC6Full, ReadOnlySpan<byte> data)
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

    private static WC6 ReadWC6(ReadOnlySpan<byte> data, int ofs, DateOnly date)
    {
        var slice = data.Slice(ofs + GiftStart, WC6.Size).ToArray();
        return new WC6(slice)
        {
            RestrictVersion = data[ofs],
            RestrictLanguage = data[ofs + 0x1FF],
            RawDate = WC6.SetDate((uint)date.Year, (uint)date.Month, (uint)date.Day),
        };
    }

    private static WC6 ReadWC6Only(ReadOnlySpan<byte> data, int ofs)
    {
        var slice = data.Slice(ofs, WC6.Size).ToArray();
        return new WC6(slice);
    }
}

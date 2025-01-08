using System;

namespace PKHeX.Core;

internal static class Encounters2GBEra
{
    public static readonly EncounterGift2[] Gifts = GetGifts(Util.GetBinaryResource("event2.pkl"));

    private static EncounterGift2[] GetGifts(ReadOnlySpan<byte> bin)
    {
        const int size = EncounterGift2.SerializedSize;
        var result = new EncounterGift2[bin.Length / size];
        for (int i = 0; i < result.Length; i++)
        {
            var data = bin[..size];
            result[i] = new EncounterGift2(data);
            bin = bin[size..];
        }
        return result;
    }
}

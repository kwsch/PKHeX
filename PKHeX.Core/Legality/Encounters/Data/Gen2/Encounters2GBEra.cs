using System;

namespace PKHeX.Core;

internal static class Encounters2GBEra
{
    public static readonly EncounterGift2[] Gifts = GetGifts(Util.GetBinaryResource("event2.pkl"));

    private static EncounterGift2[] GetGifts(ReadOnlySpan<byte> bin)
    {
        const int Size = EncounterGift2.SerializedSize;
        var result = new EncounterGift2[bin.Length / Size];
        for (int i = 0; i < result.Length; i++)
        {
            var data = bin[..Size];
            result[i] = new EncounterGift2(data);
            bin = bin[Size..];
        }
        return result;
    }
}

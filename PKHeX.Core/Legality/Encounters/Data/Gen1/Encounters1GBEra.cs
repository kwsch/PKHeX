using System;

namespace PKHeX.Core;

internal static class Encounters1GBEra
{
    public static readonly EncounterGift1[] Gifts = GetGifts(Util.GetBinaryResource("event1.pkl"));

    private static EncounterGift1[] GetGifts(ReadOnlySpan<byte> bin)
    {
        const int Size = EncounterGift1.SerializedSize;
        var result = new EncounterGift1[bin.Length / Size];
        for (int i = 0; i < result.Length; i++)
        {
            var data = bin[..Size];
            result[i] = new EncounterGift1(data);
            bin = bin[Size..];
        }
        return result;
    }
}

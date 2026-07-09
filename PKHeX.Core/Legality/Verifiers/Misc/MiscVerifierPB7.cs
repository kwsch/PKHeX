using static System.BitConverter; // float->uint conversion for legality messages
using static PKHeX.Core.LegalityCheckResultCode;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core;

internal sealed class MiscVerifierPB7 : Verifier
{
    protected override CheckIdentifier Identifier => Misc;

    public override void Verify(LegalityAnalysis data)
    {
        if (data.Entity is PB7 pk)
            Verify(data, pk);
    }

    internal void Verify(LegalityAnalysis data, PB7 pk)
    {
        VerifyAbsoluteSizes(data, pk);

        var calc = pk.CalcCP;
        if (pk.Stat_CP != pk.CalcCP && !pk.IsStarter)
            data.AddLine(GetInvalid(Encounter, StatIncorrectCP_0, (uint)calc));

        VerifyReceivedDateTime(data, pk);
        VerifyMoodSpirit(data, pk);
    }

    private static void VerifyReceivedDateTime(LegalityAnalysis data, PB7 pk)
    {
        if (pk.ReceivedTime is null) // invalid 24h timestamp
            data.AddLine(GetInvalid(Misc, DateLocalInvalidTime));

        // HOME moving in and out will retain received date. ensure it matches if no HT data present.
        // Go Park captures will have different dates, as the GO met date is retained as Met Date.
        if (!IsReceivedDateValid(pk, data.EncounterOriginal))
            data.AddLine(GetInvalid(Misc, DateLocalInvalidDate));
    }

    private static bool IsReceivedDateValid(PB7 pk, IEncounterTemplate enc)
    {
        if (pk.ReceivedDate is not { } date)
            return false; // empty/invalid date
        if (!EncounterDate.IsValidDateSwitch(date))
            return false; // not obtainable on console
        if (pk.IsUntraded && enc is not EncounterSlot7GO && date != pk.MetDate)
            return false; // Must match met date if not traded; GO retains original met date (and can differ).
        return true;

    }

    private static void VerifyMoodSpirit(LegalityAnalysis data, PB7 pk)
    {
        if (data.IsStoredSlot(StorageSlotType.Party) || pk.IsStarter)
            return; // Can be changed in party, and starters retain moods/spirit.

        // At rest, these should be the default values.
        if (pk.Spirit is not PB7.InitialSpiritMood)
            data.AddLine(GetInvalid(Misc, G7BSocialShouldBe100Spirit, PB7.InitialSpiritMood));
        if (pk.Mood is not PB7.InitialSpiritMood)
            data.AddLine(GetInvalid(Misc, G7BSocialShouldBe100Mood, PB7.InitialSpiritMood));
    }
    internal static void VerifyAbsoluteSizes<T>(LegalityAnalysis data, T pk) where T : IScaledSizeValue
    {
        // Check if the size values are correct, with some edge cases where they are fixed and not calculated.
        // If not an edge case, must be calculated.
        switch (pk)
        {
            case PB7 pb7 when data.EncounterMatch is WB7 { IsHeightWeightFixed: true } enc:
                VerifyFixedSizes(data, pb7, enc);
                return;
        }

        VerifyCalculatedSizes(data, pk);
    }

    // ReSharper disable CompareOfFloatsByEqualityOperator -- THESE MUST MATCH EXACTLY
    private static void VerifyFixedSizes<T>(LegalityAnalysis data, T obj, WB7 enc) where T : IScaledSizeValue
    {
        // Unlike PLA, there is no way to force it to recalculate in-game.
        // The only encounter this applies to is Meltan, which cannot reach PLA for recalculation.
        var expectHeight = enc.GetHomeHeightAbsolute();
        if (obj.HeightAbsolute != expectHeight)
            data.AddLine(GetInvalid(Encounter, StatIncorrectHeight, SingleToUInt32Bits(expectHeight)));

        var expectWeight = enc.GetHomeWeightAbsolute();
        if (obj.WeightAbsolute != expectWeight)
            data.AddLine(GetInvalid(Encounter, StatIncorrectWeight, SingleToUInt32Bits(expectWeight)));
    }

    private static void VerifyCalculatedSizes<T>(LegalityAnalysis data, T obj) where T : IScaledSizeValue
    {
        var expectHeight = obj.CalcHeightAbsolute;
        if (obj.HeightAbsolute != expectHeight)
            data.AddLine(GetInvalid(Encounter, StatIncorrectHeight, SingleToUInt32Bits(expectHeight)));

        var expectWeight = obj.CalcWeightAbsolute;
        if (obj.WeightAbsolute != expectWeight)
            data.AddLine(GetInvalid(Encounter, StatIncorrectWeight, SingleToUInt32Bits(expectWeight)));
    }
    // ReSharper restore CompareOfFloatsByEqualityOperator
}

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
        MiscVerifierHelpers.VerifyAbsoluteSizes(data, pk);

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
}

using static System.BitConverter; // float->uint conversion for legality messages
using static PKHeX.Core.LegalityCheckResultCode;
using static PKHeX.Core.CheckIdentifier;
using static PKHeX.Core.CheckResult;
using static PKHeX.Core.Severity;

namespace PKHeX.Core;

internal static class MiscVerifierHelpers
{
    internal static void VerifyStatNature(LegalityAnalysis data, PKM pk)
    {
        // No encounters innately come with a different Stat Nature...
        // If it matches the Nature, it is valid. If it doesn't, it should be one of the mint natures.
        var statNature = pk.StatNature;
        if (statNature == pk.Nature)
            return;

        // Must be a valid mint nature.
        if (!statNature.IsMint())
            data.AddLine(Get(Invalid, Misc, StatNatureInvalid));
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
            case PA8 { Scale: 255 } pa8 when data.EncounterMatch is EncounterStatic8a { IsAlpha127: true }:
                VerifyFixedSizeAlpha127(data, pa8);
                return;
        }

        VerifyCalculatedSizes(data, pk);
    }

    internal static bool IsObedienceLevelValid<T>(T pk, byte currentObey, byte originalObey) where T : PKM
    {
        if (pk.IsUntraded)
            return currentObey == originalObey;

        // Trading will set equal to current level; can be any level up to the current.
        if (currentObey < originalObey)
            return false;
        return pk.CurrentLevel >= currentObey;
    }

    // ReSharper disable CompareOfFloatsByEqualityOperator -- THESE MUST MATCH EXACTLY
    private static void VerifyFixedSizes<T>(LegalityAnalysis data, T obj, WB7 enc) where T : IScaledSizeValue
    {
        // Unlike PLA, there is no way to force it to recalculate in-game.
        // The only encounter this applies to is Meltan, which cannot reach PLA for recalculation.
        var expectHeight = enc.GetHomeHeightAbsolute();
        if (obj.HeightAbsolute != expectHeight)
            data.AddLine(Get(Invalid, Encounter, StatIncorrectHeight, SingleToUInt32Bits(expectHeight)));

        var expectWeight = enc.GetHomeWeightAbsolute();
        if (obj.WeightAbsolute != expectWeight)
            data.AddLine(Get(Invalid, Encounter, StatIncorrectWeight, SingleToUInt32Bits(expectWeight)));
    }

    private static void VerifyFixedSizeAlpha127(LegalityAnalysis data, PA8 pk)
    {
        // HOME 3.0.1+ fixes the Height/Weight to 255, but doesn't update the float calculated sizes.
        // Putting it in party and putting it back in box did trigger them to update, so it can legally be two states:
        // Mutated (255 with 127-based-floats), or Updated (255 with 255-based-floats)
        // Since most players won't be triggering an update, it is more likely that it is only mutated.
        // Check for mutated first. If not matching mutated, must match updated.
        var pi = pk.PersonalInfo;
        var mutHeight = PA8.GetHeightAbsolute(pi, 127);
        if (pk.HeightAbsolute == mutHeight)
        {
            var mutWeight = PA8.GetWeightAbsolute(pi, 127, 127);
            if (pk.WeightAbsolute == mutWeight)
                return; // OK
        }
        // Since it does not match the mutated state, it must be the updated state (255 + matching floats)
        VerifyCalculatedSizes(data, pk);
    }

    private static void VerifyCalculatedSizes<T>(LegalityAnalysis data, T obj) where T : IScaledSizeValue
    {
        var expectHeight = obj.CalcHeightAbsolute;
        if (obj.HeightAbsolute != expectHeight)
            data.AddLine(Get(Invalid, Encounter, StatIncorrectHeight, SingleToUInt32Bits(expectHeight)));

        var expectWeight = obj.CalcWeightAbsolute;
        if (obj.WeightAbsolute != expectWeight)
            data.AddLine(Get(Invalid, Encounter, StatIncorrectWeight, SingleToUInt32Bits(expectWeight)));
    }
    // ReSharper restore CompareOfFloatsByEqualityOperator
}

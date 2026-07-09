using static PKHeX.Core.LegalityCheckResultCode;
using static PKHeX.Core.CheckIdentifier;
using static PKHeX.Core.CheckResult;
using static PKHeX.Core.Severity;

namespace PKHeX.Core;

internal static class MiscVerifierHelpers
{
    internal static void VerifyStatAlignment(LegalityAnalysis data, PKM pk)
    {
        // No encounters innately come with a different Stat Alignment...
        // If it matches the Nature, it is valid. If it doesn't, it should be one of the mint natures.
        var alignment = pk.StatAlignment;
        if (alignment == pk.Nature)
            return;

        // Must be a valid mint nature.
        if (!alignment.IsMint)
            data.AddLine(Get(Invalid, Misc, StatAlignmentInvalid));
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
}

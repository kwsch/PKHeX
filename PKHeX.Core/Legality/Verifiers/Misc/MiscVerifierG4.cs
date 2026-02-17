using static PKHeX.Core.LegalityCheckResultCode;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core;

internal sealed class MiscVerifierG4 : Verifier
{
    protected override CheckIdentifier Identifier => Misc;

    public override void Verify(LegalityAnalysis data)
    {
        if (data.Entity is G4PKM pk)
            Verify(data, pk);
    }

    internal void Verify(LegalityAnalysis data, G4PKM pk)
    {
        // Verify misc values that were introduced in HG/SS
        VerifySplitBall(data, pk);
        VerifyWalkingMood(data, pk);
        VerifyShinyLeaf(data, pk);
    }

    private void VerifyShinyLeaf(LegalityAnalysis data, G4PKM pk)
    {
        // Shiny leaf: cannot have crown (bit 5) without bits 0-4. Bits higher are invalid.
        // Eggs cannot receive a Shiny Leaf.
        var leaf = pk.ShinyLeaf;
        if (leaf is > 0b_11111 and not 0b_1_11111 || (leaf is not 0 && pk.IsEgg))
            data.AddLine(GetInvalid(G4ShinyLeafBitsInvalid));
    }

    private static void VerifySplitBall(LegalityAnalysis data, G4PKM pk)
    {
        // Version is a true match. If not from HG/SS, should not have HG/SS ball value set.
        if (pk.BallHGSS == 0 && pk.BallDPPt == pk.Ball)
            return;

        // Only set the HG/SS value if it originated in HG/SS and was not an event (pre-filled data only; not Manaphy egg!).
        if (!pk.HGSS || data.Info.EncounterOriginal is PCD) // PCD: not Manaphy via PGT
            data.AddLine(GetInvalid(CheckIdentifier.Ball, BallEncMismatch));
    }

    private static void VerifyWalkingMood(LegalityAnalysis data, G4PKM pk)
    {
        // Mood:
        // Range is [-127, 127]. Deduplicated unique adjustments are +8, +10, and -20.
        // Increment adjustments of -2 (-20 +8 +10) and +2 (+8*4 +10 -20) are possible.
        // Start from 0 or from clamped edge, with above adjustments, at least one mutation path can arrive at a given [-127, 127] value.
        // There are probably other scripted increments, but the above is just to prove that all values are possible via specific actions.
        // Eggs cannot have a value other than 0.
        // HG/SS resets when promoting to lead of party, or removing from party. Therefore, any party slot can retain a non-zero value.
        // Mood also resets for the lead Pokémon when booting the game, but it can be mutated immediately after (and saved/dumped).
        // Note: Trading to D/P/Pt does not clear (thus unchanged on side games too), so this check is only relevant for HG/SS slots.
        if (pk.WalkingMood == 0)
            return;

        if (pk.IsEgg)
            data.AddLine(GetInvalid(Egg, G4PartnerMoodEgg));
        else if (!data.IsStoredSlot(StorageSlotType.Party) && ParseSettings.ActiveTrainer is SAV4HGSS)
            data.AddLine(GetInvalid(Egg, G4PartnerMoodZero));
    }
}

using System;
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
        VerifyWalkingMood(data, pk);
        VerifyShinyLeaf(data, pk);

        var palPark = PalParkTransferOrigin.None;
        var enc = data.EncounterOriginal;
        if (enc.Generation == 3)
        {
            palPark = CheckPalParkTransfer(pk);
            if (palPark == PalParkTransferOrigin.Invalid)
                return;
            VerifyMetLocation3(data, pk, palPark);
        }
        VerifySplitBall(data, pk, palPark);
    }

    private void VerifyMetLocation3(LegalityAnalysis data, G4PKM pk, PalParkTransferOrigin palPark)
    {
        var extended = pk.MetLocationExtended;
        if (extended == 0 && palPark.HasFlag(PalParkTransferOrigin.DP))
            return; // OK
        if (extended == pk.MetLocationDP && palPark.HasFlag(PalParkTransferOrigin.PtHGSS))
            return; // OK

        data.AddLine(GetInvalid(TransferMetLocation, Locations.Transfer3));
    }

    private static PalParkTransferOrigin CheckPalParkTransfer(G4PKM pk)
    {
        // The Pt/HG/SS location value is unset; others will not be true.
        if (pk.MetLocationExtended == 0)
            return pk.PossiblyPalParkDP ? PalParkTransferOrigin.DP : PalParkTransferOrigin.Invalid;

        var result = PalParkTransferOrigin.None;
        if (pk.PossiblyPalParkPt)
            result |= PalParkTransferOrigin.Pt;
        if (pk.PossiblyPalParkHGSS)
            result |= PalParkTransferOrigin.HGSS;
        return result;
    }

    private void VerifyShinyLeaf(LegalityAnalysis data, G4PKM pk)
    {
        // Shiny leaf: cannot have crown (bit 5) without bits 0-4. Bits higher are invalid.
        // Eggs cannot receive a Shiny Leaf.
        var leaf = pk.ShinyLeaf;
        if (leaf is > 0b_11111 and not 0b_1_11111 || (leaf is not 0 && pk.IsEgg))
            data.AddLine(GetInvalid(G4ShinyLeafBitsInvalid));
    }

    private static void VerifySplitBall(LegalityAnalysis data, G4PKM pk, PalParkTransferOrigin palPark)
    {
        // Sanity check the D/P/Pt value.
        if (!IsValidBallDP(pk.BallDPPt))
            data.AddLine(GetInvalid(CheckIdentifier.Ball, BallG4Sinnoh));
        if (!IsValidBallHGSS(data, pk, palPark))
            data.AddLine(GetInvalid(CheckIdentifier.Ball, BallG4Johto));
    }

    private static bool IsValidBallHGSS(LegalityAnalysis data, G4PKM pk, PalParkTransferOrigin palPark)
    {
        var ball = pk.BallHGSS;
        var enc = data.EncounterOriginal;
        // If not from HG/SS, should not have HG/SS ball value set.
        // Battle Revolution does not copy this byte.
        if (!IsCreatedInHGSS(pk, enc, palPark) || pk is BK4)
            return ball == 0;

        // If it could have been D/P/Pt transfer, then allow the not-HG/SS state.
        if ((palPark & (PalParkTransferOrigin.DPPt)) != 0)
        {
            if (ball == 0)
                return true;
        }

        var dp = pk.BallDPPt;
        // Assume D/P/Pt ball is valid (flagged separately).
        // Ball being equal is OK.
        // Ball being HG/SS exclusive is OK if D/P/Pt is default (4).
        return ball == dp || (dp is (byte)Ball.Poke && !IsValidBallDP(ball));
    }

    // Any other ball value (HG/SS ball) isn't available/displayed correctly.
    private static bool IsValidBallDP(byte ball) => ball <= (int)Ball.Cherish;

    private static bool IsCreatedInHGSS(G4PKM pk, IEncounterTemplate enc, PalParkTransferOrigin palPark)
    {
        // Only set the HG/SS value if it originated in HG/SS and was not an event (pre-filled data only; not Manaphy egg!).
        // Pal Park transfers into HG/SS set this value as well.
        if (pk.HGSS)
            return enc is not PCD; // PCD: not Manaphy via PGT
        if (palPark.HasFlag(PalParkTransferOrigin.HGSS))
            return true;
        return false;
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

        const Severity severity = Severity.Fishy;
        if (pk.IsEgg)
            data.AddLine(Get(Egg, severity, G4PartnerMoodEgg));
        else if (!data.IsStoredSlot(StorageSlotType.Party) && ParseSettings.ActiveTrainer is SAV4HGSS)
            data.AddLine(Get(Misc, severity, G4PartnerMoodZero));
    }
}

[Flags]
public enum PalParkTransferOrigin
{
    None,
    DP = 1 << 0,
    Pt = 1 << 1,
    HGSS = 1 << 2,
    Invalid = 1 << 3,

    DPPt = DP | Pt,
    PtHGSS = Pt | HGSS,
}

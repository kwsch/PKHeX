using System;

namespace PKHeX.Core;

/// <summary>
/// Interface that exposes a method to <see cref="Rejuvenate"/> data after converting.
/// </summary>
public interface IEntityRejuvenator
{
    /// <summary>
    /// After converting, the method will attempt to auto-fill missing properties.
    /// </summary>
    /// <param name="result">Output data after conversion</param>
    /// <param name="original">Input data prior to conversion</param>
    void Rejuvenate(PKM result, PKM original);
}

/// <summary>
/// Uses <see cref="LegalityAnalysis"/> to auto-fill missing data after conversion.
/// </summary>
public sealed class LegalityRejuvenator : IEntityRejuvenator
{
    public void Rejuvenate(PKM result, PKM original)
    {
        // HOME transfers from PB8/PA8 => PK8 will sanitize Ball & Met/Egg Location.
        // Transferring back without a reference PB8/PA8, we need to guess the *original* values.
        if (original is not PK8 pk8)
        {
            ResetSideways(result);
            return;
        }

        var ver = result.Version;
        if (ver is (int)GameVersion.BD or (int)GameVersion.SP)
            RejuvenateBDSP(result, pk8);
        else if (ver is (int)GameVersion.PLA)
            RejuvenatePLA(result, pk8);
    }

    private static void ResetSideways(PKM result)
    {
        if (result is PA8 pa8)
        {
            // Won't work well for Alphas
            if (pa8.RibbonMarkAlpha)
                pa8.IsAlpha = true;
            var la = new LegalityAnalysis(pa8);
            var enc = la.EncounterMatch;
            ResetDataPLA(la, enc, pa8);
            if (pa8.LA)
                ResetBallPLA(pa8, enc);
        }
    }

    private static void RejuvenatePLA(PKM result, PK8 original)
    {
        var la = new LegalityAnalysis(original);
        var enc = la.EncounterOriginal;
        if (enc is EncounterInvalid)
            return; // Won't work for Alphas

        // No egg encounters. Always not-egg.
        {
            result.Met_Location = enc.Location;
            result.Egg_Location = 0;
        }

        // Try again with rectified locations.
        la = new LegalityAnalysis(result);
        enc = la.EncounterOriginal;
        if (result is PA8 pa8)
            ResetDataPLA(la, enc, pa8);

        ResetBallPLA(result, enc);
    }

    private static void ResetBallPLA(PKM result, IEncounterable enc)
    {
        if (result.Ball is >= (int)Ball.LAPoke and <= (int)Ball.LAOrigin)
            return;
        if (enc is IFixedBall { FixedBall: not Ball.None } f)
            result.Ball = (int)f.FixedBall;
        else
            result.Ball = result.Species == (int)Species.Unown ? (int)Ball.LAJet : (int)Ball.LAPoke;
    }

    private static void ResetDataPLA(LegalityAnalysis la, IEncounterable enc, PA8 pa8)
    {
        Span<ushort> relearn = stackalloc ushort[4];
        la.GetSuggestedRelearnMoves(relearn, enc);
        if (relearn[0] != 0)
            pa8.SetRelearnMoves(relearn);

        pa8.ClearMoveShopFlags();
        if (enc is IMasteryInitialMoveShop8 e)
            e.SetInitialMastery(pa8);
        pa8.SetMoveShopFlags(pa8);
    }

    private static void RejuvenateBDSP(PKM result, PK8 original)
    {
        var la = new LegalityAnalysis(original);
        var enc = la.EncounterOriginal;
        if (enc is EncounterInvalid)
            return;

        if (enc is { EggEncounter: true })
        {
            result.Met_Location = Locations.HatchLocation8b;
            result.Egg_Location = Locations.LinkTrade6NPC;
        }
        else
        {
            result.Met_Location = enc.Location;
            result.Egg_Location = Locations.Default8bNone;
        }

        // Try again with rectified locations.
        la = new LegalityAnalysis(result);
        Span<ushort> relearn = stackalloc ushort[4];
        la.GetSuggestedRelearnMoves(relearn);
        if (relearn[0] != 0)
            result.SetRelearnMoves(relearn);
    }
}

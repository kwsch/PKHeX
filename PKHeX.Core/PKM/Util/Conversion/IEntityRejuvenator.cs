using System;

namespace PKHeX.Core;

/// <summary>
/// Interface that exposes a method to <see cref="Rejuvenate"/> data after converting.
/// </summary>
public interface IEntityRejuvenator
{
    /// <summary>
    /// After converting, the method will attempt to autofill missing properties.
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
        if (original is PK8 pk8)
            ResetOutboundSWSH(result, pk8);
        ResetSideways(result);
    }

    private static void ResetOutboundSWSH(PKM result, PK8 pk8)
    {
        var version = result.Version;
        if (version is GameVersion.BD or GameVersion.SP)
            RejuvenateBDSP(result, pk8);
        else if (version is GameVersion.PLA)
            RejuvenatePLA(result, pk8);
        else if (version is GameVersion.SL or GameVersion.VL)
            RejuvenateSV(result, pk8);
    }

    private static void ResetSideways(PKM pk)
    {
        if (pk is PA8 pa8)
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
        else if (pk is PB8 { BDSP: true })
        {
            ResetRelearn(pk, new LegalityAnalysis(pk));
        }
        else if (pk is PK9 { SV: true } pk9)
        {
            var la = new LegalityAnalysis(pk);
            ResetRelearn(pk, la);

            // Try to restore original Tera type / override instead of HOME's double override to current Type1.
            TeraTypeUtil.ResetTeraType(pk9, la.EncounterMatch);
        }
        else if (pk is PK8 pk8 && !LocationsHOME.IsLocationSWSH(pk8.MetLocation))
        {
            // Gen8 and below (Gen6/7) need their original relearn moves
            // We can always set a Battle Version for non Gen8 origins, but most users won't be making stuff battle ready after.
            // Battle Version is always zero in this case, so be nice and give the original relearn moves.
            ResetRelearn(pk, new LegalityAnalysis(pk));
        }
    }

    private static void ResetRelearn(PKM pk, LegalityAnalysis la)
    {
        // Set suggested relearn moves.
        Span<ushort> m = stackalloc ushort[4];
        la.GetSuggestedRelearnMoves(m);
        pk.SetRelearnMoves(m);
    }

    private static void RejuvenatePLA(PKM result, PK8 original)
    {
        var la = new LegalityAnalysis(original);
        var enc = la.EncounterOriginal;
        if (enc is EncounterInvalid)
            return; // Won't work for Alphas

        // No egg encounters. Always not-egg.
        {
            result.MetLocation = enc.Location;
            result.EggLocation = 0;
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
        if (enc is { FixedBall: not Ball.None })
            result.Ball = (byte)enc.FixedBall;
        else
            result.Ball = result.Species == (int)Species.Unown ? (byte)Ball.LAJet : (byte)Ball.LAPoke;
    }

    private static void ResetDataPLA(LegalityAnalysis la, IEncounterable enc, PA8 pa8)
    {
        ResetRelearn(pa8, la);

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

        if (enc is { IsEgg: true })
        {
            result.MetLocation = Locations.HatchLocation8b;
            result.EggLocation = Locations.LinkTrade6NPC;
        }
        else
        {
            result.MetLocation = enc.Location;
            result.EggLocation = Locations.Default8bNone;
        }

        // Try again with rectified locations.
        la = new LegalityAnalysis(result);
        ResetRelearn(result, la);
    }

    private static void RejuvenateSV(PKM result, PK8 original)
    {
        var la = new LegalityAnalysis(original);
        var enc = la.EncounterOriginal;
        if (enc is EncounterInvalid)
            return;

        if (enc is { IsEgg: true })
        {
            result.MetLocation = Locations.HatchLocation9;
            result.EggLocation = Locations.LinkTrade6NPC;
        }
        else
        {
            result.MetLocation = enc.Location;
            result.EggLocation = 0;
        }

        // Try again with rectified locations.
        la = new LegalityAnalysis(result);
        ResetRelearn(result, la);
    }
}

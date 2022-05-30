namespace PKHeX.Core;

public interface IEntityRejuvenator
{
    public void Rejuvenate(PKM result, PKM original);
}

public class LegalityRejuvenator : IEntityRejuvenator
{
    public void Rejuvenate(PKM result, PKM original)
    {
        if (original is not PK8 pk8)
            return;

        var ver = result.Version;
        if (ver is (int)GameVersion.BD or (int)GameVersion.SP)
            RejuvenateBDSP(result, pk8);
        else if (ver is (int)GameVersion.PLA)
            RejuvenatePLA(result, pk8);
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
            result.Egg_Location = Locations.Default8bNone;
        }

        // Try again with rectified locations.
        la = new LegalityAnalysis(result);
        enc = la.EncounterOriginal;
        if (result is PA8 pa8)
        {
            var relearn = la.GetSuggestedRelearnMoves(enc);
            if (relearn.Count != 0)
            {
                for (int i = 0; i < relearn.Count; i++)
                    pa8.SetRelearnMove(i, relearn[i]);
            }

            pa8.ClearMoveShopFlags();
            if (enc is IMasteryInitialMoveShop8 e)
                e.SetInitialMastery(pa8);
            pa8.SetMoveShopFlags(pa8);
        }

        if (result.Ball is >= (int)Ball.LAPoke and <= (int)Ball.LAOrigin)
            return;
        if (enc is IFixedBall { FixedBall: not Ball.None } f)
            result.Ball = (int)f.FixedBall;
        else
            result.Ball = result.Species == (int)Species.Unown ? (int)Ball.LAJet : (int)Ball.LAPoke;
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
        enc = la.EncounterOriginal;
        var relearn = la.GetSuggestedRelearnMoves(enc);
        if (relearn.Count != 0)
        {
            for (int i = 0; i < relearn.Count; i++)
                result.SetRelearnMove(i, relearn[i]);
        }
    }
}

public sealed class EntityRejuvenatorDummy : IEntityRejuvenator
{
    public void Rejuvenate(PKM result, PKM original) { }
}

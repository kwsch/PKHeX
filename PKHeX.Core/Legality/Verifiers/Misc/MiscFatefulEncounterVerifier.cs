using static PKHeX.Core.LegalityCheckResultCode;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core;

internal sealed class MiscFatefulEncounterVerifier : Verifier
{
    protected override CheckIdentifier Identifier => Misc;

    public override void Verify(LegalityAnalysis data)
    {
        var pk = data.Entity;
        var enc = data.EncounterMatch;
        Verify(data, pk, enc);
    }

    internal void Verify(LegalityAnalysis data, PKM pk, IEncounterTemplate enc)
    {
        switch (enc)
        {
            case EncounterGift3 {FatefulEncounter: true} w:
                if (w.IsEgg)
                {
                    // Eggs hatched in RS clear the obedience flag!
                    // Hatching in Gen3 doesn't change the origin version.
                    if (pk.Format != 3)
                        return; // possible hatched in either game, don't bother checking
                    if (Locations.IsMetLocation3RS(pk.MetLocation)) // hatched in RS or Emerald
                        return; // possible hatched in either game, don't bother checking
                    // else, ensure fateful is active (via below)
                }
                VerifyFatefulActive(data, pk);
                VerifyGift3Shiny(data, w);
                return;
            case EncounterGift3 w:
                VerifyGift3Shiny(data, w);
                break;
            case MysteryGift g: // WC3 handled above
                VerifyReceivability(data, g);
                VerifyFatefulMysteryGift(data, g);
                return;
            case IFatefulEncounterReadOnly {FatefulEncounter: true}: // in-game fateful
                VerifyFatefulActive(data, pk);
                return;
        }

        VerifyFatefulInactive(data, pk);
    }

    private static void VerifyFatefulInactive(LegalityAnalysis data, PKM pk)
    {
        if (pk.FatefulEncounter)
            data.AddLine(GetInvalid(Fateful, FatefulInvalid));
    }

    private static void VerifyFatefulActive(LegalityAnalysis data, PKM pk)
    {
        if (!pk.FatefulEncounter)
            data.AddLine(GetInvalid(Fateful, FatefulMissing));
    }

    private static void VerifyFatefulMysteryGift(LegalityAnalysis data, MysteryGift g)
    {
        var pk = data.Entity;
        if (g is PGF {IsShiny: true})
        {
            var info = data.Info;
            info.PIDIV = MethodFinder.Analyze(pk);
            if (info.PIDIV.Type != PIDType.G5MGShiny)
            {
                var locToCheck = pk.IsEgg ? pk.MetLocation : pk.EggLocation;
                if (locToCheck is not (Locations.LinkTrade5 or Locations.LinkTrade5NPC))
                    data.AddLine(GetInvalid(PID, PIDTypeMismatch));
            }
        }

        bool shouldHave = g.FatefulEncounter;
        var result = pk.FatefulEncounter == shouldHave
            ? GetValid(Fateful, FatefulMystery)
            : GetInvalid(Fateful, shouldHave ? FatefulMysteryMissing : FatefulInvalid);
        data.AddLine(result);
    }

    private static void VerifyReceivability(LegalityAnalysis data, MysteryGift g)
    {
        var pk = data.Entity;
        switch (g)
        {
            case PCD pcd when !pcd.CanBeReceivedByVersion(pk.Version) && pcd.Gift.PK.Version == 0:
            case WC6 wc6 when !wc6.CanBeReceivedByVersion(pk.Version) && !pk.WasTradedEgg:
            case WC7 wc7 when !wc7.CanBeReceivedByVersion(pk.Version) && !pk.WasTradedEgg:
            case WC8 wc8 when !wc8.CanBeReceivedByVersion(pk.Version):
            case WB8 wb8 when !wb8.CanBeReceivedByVersion(pk.Version, pk):
            case WA8 wa8 when !wa8.CanBeReceivedByVersion(pk.Version, pk):
                data.AddLine(GetInvalid(GameOrigin, EncGiftVersionNotDistributed));
                return;
            case PGF pgf when pgf.RestrictLanguage != 0 && pk.Language != pgf.RestrictLanguage:
                data.AddLine(GetInvalid(CheckIdentifier.Language, EncGiftLanguageNotDistributed_0, (ushort)pgf.RestrictLanguage));
                return;
            case WC6 wc6 when wc6.RestrictLanguage != 0 && pk.Language != wc6.RestrictLanguage:
                data.AddLine(GetInvalid(CheckIdentifier.Language, EncGiftLanguageNotDistributed_0, (ushort)wc6.RestrictLanguage));
                return;
            case WC7 wc7 when wc7.RestrictLanguage != 0 && pk.Language != wc7.RestrictLanguage:
                data.AddLine(GetInvalid(CheckIdentifier.Language, EncGiftLanguageNotDistributed_0, (ushort)wc7.RestrictLanguage));
                return;
        }
    }

    private static void VerifyGift3Shiny(LegalityAnalysis data, EncounterGift3 g3)
    {
        // check for shiny locked gifts
        if (!g3.Shiny.IsValid(data.Entity))
            data.AddLine(GetInvalid(Fateful, EncGiftShinyMismatch));
    }
}

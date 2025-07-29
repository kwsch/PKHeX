using static PKHeX.Core.LegalityCheckResultCode;
using static PKHeX.Core.Species;

namespace PKHeX.Core;

/// <summary>
/// Verifies the <see cref="PKM.Form"/> value.
/// </summary>
public sealed class FormVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.Form;
    private static readonly FormArgumentVerifier FormArg = new();

    public override void Verify(LegalityAnalysis data)
    {
        var pk = data.Entity;
        if (pk.Format < 4)
            return; // no forms exist
        var result = VerifyForm(data);
        data.AddLine(result);

        FormArg.Verify(data);
    }

    private CheckResult VALID => GetValid(FormValid);

    private CheckResult VerifyForm(LegalityAnalysis data)
    {
        var pk = data.Entity;
        var pi = data.PersonalInfo;

        int count = pi.FormCount;
        var form = pk.Form;
        if (count <= 1 && form == 0)
            return VALID; // no forms to check

        var species = pk.Species;
        var enc = data.EncounterMatch;

        if (!pi.IsFormWithinRange(form) && !FormInfo.IsValidOutOfBoundsForm(species, form, enc.Generation))
            return GetInvalid(FormInvalidRangeLEQ_0F, (ushort)(count - 1));

        switch ((Species)species)
        {
            case Pikachu when enc.Generation == 6: // Cosplay
                if (enc is not EncounterStatic6 s6)
                {
                    if (form == 0)
                        break; // Regular Pikachu, OK.
                    return GetInvalid(FormPikachuCosplay);
                }
                if (form != s6.Form)
                    return GetInvalid(FormPikachuCosplayInvalid);
                if (pk.Format != 6)
                    return GetInvalid(TransferBad); // Can't transfer.
                break;

            // LGP/E: Can't get the other game's Starter form.
            case Pikachu when form is not 0 && ParseSettings.ActiveTrainer is SAV7b {Version:GameVersion.GE}:
            case Eevee when form is not 0 && ParseSettings.ActiveTrainer is SAV7b {Version:GameVersion.GP}:
                return GetInvalid(FormBattle);

            case Pikachu when enc.Generation >= 7: // Cap
                var expectForm = enc is EncounterInvalid or IEncounterEgg ? 0 : enc.Form;
                if (form != expectForm)
                {
                    bool gift = enc is MysteryGift g && g.Form != form;
                    var msg = gift ? FormPikachuEventInvalid : FormInvalidGame;
                    return GetInvalid(msg);
                }
                break;

            case Unown when enc.Generation == 2 && form >= 26:
                return GetInvalid(FormInvalidRangeLEQ_0F, 25);
            case Unown when enc.Generation == 3:
                var expectUnown = EntityPID.GetUnownForm3(pk.EncryptionConstant);
                if (expectUnown != form)
                    return GetInvalid(FormInvalidExpect_0, expectUnown);
                break;

            case Dialga or Palkia or Giratina or Arceus when form > 0 && pk is PA8: // can change forms with key items
                break;
            case Dialga   when pk.Format >= 9 && ((form == 1) != (pk.HeldItem == 1777)): // Origin Forme Dialga with Adamant Crystal
            case Palkia   when pk.Format >= 9 && ((form == 1) != (pk.HeldItem == 1778)): // Origin Forme Palkia with Lustrous Globe
            case Giratina when pk.Format >= 9 && ((form == 1) != (pk.HeldItem == 1779)): // Origin Forme Giratina with Griseous Core
            case Giratina when pk.Format <= 8 && ((form == 1) != (pk.HeldItem == 0112)): // Origin Forme Giratina with Griseous Orb
                return GetInvalid(FormItemInvalid);

            case Arceus:
                var arceus = FormItem.GetFormArceus(pk.HeldItem, pk.Format);
                return arceus != form ? GetInvalid(FormItemInvalid) : GetValid(FormItemMatches);
            case Keldeo when enc.Generation != 5 || pk.Format >= 8:
                // can mismatch in Gen5 via B/W tutor and transfer up
                // can mismatch in Gen8+ as the form activates in battle when knowing the move; outside of battle can be either state.
                // Generation 8 patched out the mismatch; always forced to match moves.
                bool hasSword = pk.HasMove((int) Move.SecretSword);
                bool isSword = pk.Form == 1;
                if (isSword != hasSword)
                    return GetInvalid(MoveKeldeoMismatch);
                break;
            case Genesect:
                var genesect = FormItem.GetFormGenesect(pk.HeldItem);
                return genesect != form ? GetInvalid(FormItemInvalid) : GetValid(FormItemMatches);
            case Greninja:
                if (form > 1) // Ash Battle Bond active
                    return GetInvalid(FormBattle);
                if (form != 0 && enc is not MysteryGift) // Form can not be bred for, MysteryGift already checked
                    return GetInvalid(FormInvalidRangeLEQ_0F, 0);
                break;

            case Scatterbug or Spewpa or Vivillon when enc.Context is EntityContext.Gen9:
                if (form > 18 && enc.Form != form) // Pokéball
                    return GetInvalid(FormVivillonEventPre);
                if (form != 18 && enc is IEncounterEgg) // Fancy
                    return GetInvalid(FormVivillonNonNative);
                break;
            case Scatterbug or Spewpa:
                if (form > Vivillon3DS.MaxWildFormID) // Fancy & Pokéball
                    return GetInvalid(FormVivillonEventPre);
                if (pk is not IRegionOrigin tr)
                    break;
                if (!Vivillon3DS.IsPatternValid(form, tr.ConsoleRegion))
                    return GetInvalid(FormVivillonInvalid);
                if (!Vivillon3DS.IsPatternNative(form, tr.Country, tr.Region))
                    data.AddLine(Get(Severity.Fishy, FormVivillonNonNative));
                break;
            case Vivillon:
                if (form > Vivillon3DS.MaxWildFormID) // Fancy & Pokéball
                {
                    if (enc is not MysteryGift)
                        return GetInvalid(FormVivillonInvalid);
                    return GetValid(FormVivillon);
                }
                if (pk is not IRegionOrigin trv)
                    break;
                if (!Vivillon3DS.IsPatternValid(form, trv.ConsoleRegion))
                    return GetInvalid(FormVivillonInvalid);
                if (!Vivillon3DS.IsPatternNative(form, trv.Country, trv.Region))
                    data.AddLine(Get(Severity.Fishy, FormVivillonNonNative));
                break;

            case Floette when form == 5: // Floette Eternal Flower -- Never Released
                if (enc is not MysteryGift)
                    return GetInvalid(FormEternalInvalid);
                return GetValid(FormEternal);
            case Meowstic when form != pk.Gender:
                return GetInvalid(GenderInvalidNone);

            case Silvally:
                var silvally = FormItem.GetFormSilvally(pk.HeldItem);
                return silvally != form ? GetInvalid(FormItemInvalid) : GetValid(FormItemMatches);

            // Form doesn't exist in SM; cannot originate from that game.
            case Rockruff when enc.Generation == 7 && form == 1 && pk.SM:
            case Lycanroc when enc.Generation == 7 && form == 2 && pk.SM:
                return GetInvalid(FormInvalidGame);

            // Toxel encounters have already been checked for the nature-specific evolution criteria.
            case Toxtricity when enc.Species == (int)Toxtricity:
                // The game enforces the Nature for Toxtricity encounters too!
                if (pk.Form != ToxtricityUtil.GetAmpLowKeyResult(pk.Nature))
                    return GetInvalid(FormInvalidNature);
                break;

            // Ogerpon's form changes depending on its held mask
            case Ogerpon when (form & 3) != FormItem.GetFormOgerpon(pk.HeldItem):
                return GetInvalid(FormItemInvalid);

            // Impossible Egg forms
            case Rotom when pk.IsEgg && form != 0:
            case Furfrou when pk.IsEgg && form != 0:
                return GetInvalid(EggSpecies);

            // Party Only Forms
            case Shaymin:
            case Furfrou:
            case Hoopa:
                if (form != 0 && !data.IsStoredSlot(StorageSlotType.Party) && pk.Format <= 6) // has form but stored in box
                    return GetInvalid(FormParty);
                break;
        }

        var format = pk.Format;
        if (FormInfo.IsBattleOnlyForm(species, form, format))
            return GetInvalid(FormBattle);

        return VALID;
    }
}

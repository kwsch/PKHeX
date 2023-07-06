using System;
using static PKHeX.Core.LegalityCheckStrings;
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

    private CheckResult VALID => GetValid(LFormValid);

    private CheckResult VerifyForm(LegalityAnalysis data)
    {
        var pk = data.Entity;
        var PersonalInfo = data.PersonalInfo;

        int count = PersonalInfo.FormCount;
        var form = pk.Form;
        if (count <= 1 && form == 0)
            return VALID; // no forms to check

        var species = pk.Species;
        var enc = data.EncounterMatch;
        var Info = data.Info;

        if (!PersonalInfo.IsFormWithinRange(form) && !FormInfo.IsValidOutOfBoundsForm(species, form, Info.Generation))
            return GetInvalid(string.Format(LFormInvalidRange, count - 1, form));

        switch ((Species)species)
        {
            case Pikachu when Info.Generation == 6: // Cosplay
                bool isStatic = enc is EncounterStatic6;
                bool validCosplay = form == (isStatic ? enc.Form : 0);
                if (!validCosplay)
                    return GetInvalid(isStatic ? LFormPikachuCosplayInvalid : LFormPikachuCosplay);
                break;

            case Pikachu when form is not 0 && ParseSettings.ActiveTrainer is SAV7b {Version:GameVersion.GE}:
            case Eevee when form is not 0 && ParseSettings.ActiveTrainer is SAV7b {Version:GameVersion.GP}:
                return GetInvalid(LFormBattle);

            case Pikachu when Info.Generation >= 7: // Cap
                bool validCap = form == (enc is EncounterInvalid or EncounterEgg ? 0 : enc.Form);
                if (!validCap)
                {
                    bool gift = enc is MysteryGift g && g.Form != form;
                    var msg = gift ? LFormPikachuEventInvalid : LFormInvalidGame;
                    return GetInvalid(msg);
                }
                break;
            case Unown when Info.Generation == 2 && form >= 26:
                return GetInvalid(string.Format(LFormInvalidRange, "Z", form == 26 ? "!" : "?"));
            case Unown when Info.Generation == 3 && form != EntityPID.GetUnownForm3(pk.EncryptionConstant):
                return GetInvalid(string.Format(LFormInvalidExpect_0, EntityPID.GetUnownForm3(pk.EncryptionConstant)));
            case Dialga or Palkia or Giratina or Arceus when form > 0 && pk is PA8: // can change forms with key items
                break;

            case Dialga   when pk.Format >= 9 && ((form == 1) != (pk.HeldItem == 1777)): // Origin Forme Dialga with Adamant Crystal
            case Palkia   when pk.Format >= 9 && ((form == 1) != (pk.HeldItem == 1778)): // Origin Forme Palkia with Lustrous Globe
            case Giratina when pk.Format >= 9 && ((form == 1) != (pk.HeldItem == 1779)): // Origin Forme Giratina with Griseous Core
            case Giratina when pk.Format <= 8 && ((form == 1) != (pk.HeldItem == 0112)): // Origin Forme Giratina with Griseous Orb
                return GetInvalid(LFormItemInvalid);

            case Arceus:
            {
                var arceus = GetArceusFormFromHeldItem(pk.HeldItem, pk.Format);
                return arceus != form ? GetInvalid(LFormItemInvalid) : GetValid(LFormItem);
            }
            case Keldeo when enc.Generation != 5 || pk.Format >= 8:
                // can mismatch in gen5 via BW tutor and transfer up
                // can mismatch in gen8+ as the form activates in battle when knowing the move; outside of battle can be either state.
                // Generation 8 patched out the mismatch; always forced to match moves.
                bool hasSword = pk.HasMove((int) Move.SecretSword);
                bool isSword = pk.Form == 1;
                if (isSword != hasSword)
                    return GetInvalid(LMoveKeldeoMismatch);
                break;
            case Genesect:
            {
                var genesect = GetGenesectFormFromHeldItem(pk.HeldItem);
                return genesect != form ? GetInvalid(LFormItemInvalid) : GetValid(LFormItem);
            }
            case Greninja:
                if (form > 1) // Ash Battle Bond active
                    return GetInvalid(LFormBattle);
                if (form != 0 && enc is not MysteryGift) // Forms are not breedable, MysteryGift already checked
                    return GetInvalid(string.Format(LFormInvalidRange, 0, form));
                break;

            case Scatterbug or Spewpa or Vivillon when GameVersion.SV.Contains(enc.Version):
                if (form > 18 && enc.Form != form) // Pokéball
                    return GetInvalid(LFormVivillonEventPre);
                if (form != 18 && enc is EncounterEgg) // Fancy
                    return GetInvalid(LFormVivillonNonNative);
                break;
            case Scatterbug or Spewpa:
                if (form > Vivillon3DS.MaxWildFormID) // Fancy & Pokéball
                    return GetInvalid(LFormVivillonEventPre);
                if (pk is not IRegionOrigin tr)
                    break;
                if (!Vivillon3DS.IsPatternValid(form, tr.ConsoleRegion))
                    return GetInvalid(LFormVivillonInvalid);
                if (!Vivillon3DS.IsPatternNative(form, tr.Country, tr.Region))
                    data.AddLine(Get(LFormVivillonNonNative, Severity.Fishy));
                break;
            case Vivillon:
                if (form > Vivillon3DS.MaxWildFormID) // Fancy & Pokéball
                {
                    if (enc is not MysteryGift)
                        return GetInvalid(LFormVivillonInvalid);
                    return GetValid(LFormVivillon);
                }
                if (pk is not IRegionOrigin trv)
                    break;
                if (!Vivillon3DS.IsPatternValid(form, trv.ConsoleRegion))
                    return GetInvalid(LFormVivillonInvalid);
                if (!Vivillon3DS.IsPatternNative(form, trv.Country, trv.Region))
                    data.AddLine(Get(LFormVivillonNonNative, Severity.Fishy));
                break;

            case Floette when form == 5: // Floette Eternal Flower -- Never Released
                if (enc is not MysteryGift)
                    return GetInvalid(LFormEternalInvalid);
                return GetValid(LFormEternal);
            case Meowstic when form != pk.Gender:
                return GetInvalid(LGenderInvalidNone);

            case Silvally:
            {
                var silvally = GetSilvallyFormFromHeldItem(pk.HeldItem);
                return silvally != form ? GetInvalid(LFormItemInvalid) : GetValid(LFormItem);
            }

            // Form doesn't exist in SM; cannot originate from that game.
            case Rockruff when enc.Generation == 7 && form == 1 && pk.SM:
            case Lycanroc when enc.Generation == 7 && form == 2 && pk.SM:
                return GetInvalid(LFormInvalidGame);

            // Toxel encounters have already been checked for the nature-specific evolution criteria.
            case Toxtricity when enc.Species == (int)Toxtricity:
            {
                // The game enforces the Nature for Toxtricity encounters too!
                if (pk.Form != EvolutionMethod.GetAmpLowKeyResult(pk.Nature))
                    return GetInvalid(LFormInvalidNature);
                break;
            }

            // Impossible Egg forms
            case Rotom when pk.IsEgg && form != 0:
            case Furfrou when pk.IsEgg && form != 0:
                return GetInvalid(LEggSpecies);

            // Party Only Forms
            case Shaymin:
            case Furfrou:
            case Hoopa:
                if (form != 0 && data.SlotOrigin is not SlotOrigin.Party && pk.Format <= 6) // has form but stored in box
                    return GetInvalid(LFormParty);
                break;
        }

        var format = pk.Format;
        if (FormInfo.IsBattleOnlyForm(species, form, format))
            return GetInvalid(LFormBattle);

        return VALID;
    }

    private static ReadOnlySpan<ushort> Arceus_PlateIDs => new ushort[] { 303, 306, 304, 305, 309, 308, 310, 313, 298, 299, 301, 300, 307, 302, 311, 312, 644 };
    private static ReadOnlySpan<ushort> Arceus_ZCrystal => new ushort[] { 782, 785, 783, 784, 788, 787, 789, 792, 777, 778, 780, 779, 786, 781, 790, 791, 793 };

    public static byte GetArceusFormFromHeldItem(int item, int format) => item switch
    {
        (>= 777 and <= 793)        => GetArceusFormFromZCrystal(item),
        (>= 298 and <= 313) or 644 => GetArceusFormFromPlate(item, format),
        _ => 0,
    };

    private static byte GetArceusFormFromZCrystal(int item)
    {
        return (byte)(Arceus_ZCrystal.IndexOf((ushort)item) + 1);
    }

    private static byte GetArceusFormFromPlate(int item, int format)
    {
        byte form = (byte)(Arceus_PlateIDs.IndexOf((ushort)item) + 1);
        if (format != 4) // No need to consider Curse type
            return form;
        if (form < 9)
            return form;
        return ++form; // ??? type Form shifts everything by 1
    }

    public static byte GetSilvallyFormFromHeldItem(int item)
    {
        if (item is >= 904 and <= 920)
            return (byte)(item - 903);
        return 0;
    }

    public static byte GetGenesectFormFromHeldItem(int item)
    {
        if (item is >= 116 and <= 119)
            return (byte)(item - 115);
        return 0;
    }
}

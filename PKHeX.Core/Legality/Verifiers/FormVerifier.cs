using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    public class FormVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.Form;

        public override void Verify(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            if (pkm.Format < 4)
                return; // no forms exist
            var result = VerifyForm(data);
            data.AddLine(result);
        }

        private CheckResult VALID => GetValid(V318);
        private CheckResult VerifyForm(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            var PersonalInfo = data.PersonalInfo;

            int count = PersonalInfo.FormeCount;
            if (count <= 1 && pkm.AltForm == 0)
                return VALID; // no forms to check

            var EncounterMatch = data.EncounterMatch;
            var Info = data.Info;

            if (!PersonalInfo.IsFormeWithinRange(pkm.AltForm) && !FormConverter.IsValidOutOfBoundsForme(pkm.Species, pkm.AltForm, Info.Generation))
                return GetInvalid(string.Format(V304, count - 1, pkm.AltForm));

            if (EncounterMatch is EncounterSlot w && w.Type == SlotType.FriendSafari)
                VerifyFormFriendSafari(data);
            else if (EncounterMatch is EncounterEgg)
            {
                if (FormConverter.IsTotemForm(pkm.Species, pkm.AltForm))
                    return GetInvalid(V317);
            }

            switch (pkm.Species)
            {
                case 25 when Info.Generation == 6: // Pikachu Cosplay
                    bool isStatic = EncounterMatch is EncounterStatic;
                    if (isStatic != (pkm.AltForm != 0))
                        return GetInvalid(isStatic ? V305 : V306);
                    break;

                case 25 when Info.Generation == 7: // Pikachu Cap
                    bool IsValidPikachuCap()
                    {
                        switch (EncounterMatch)
                        {
                            default: return pkm.AltForm == 0;
                            case WC7 wc7: return wc7.Form == pkm.AltForm;
                            case EncounterStatic s: return s.Form == pkm.AltForm;
                        }
                    }

                    if (!IsValidPikachuCap())
                    {
                        bool gift = EncounterMatch is WC7 g && g.Form != pkm.AltForm;
                        var msg = gift ? V307 : V317;
                        return GetInvalid(msg);
                    }
                    break;
                case 201 when Info.Generation == 2 && pkm.AltForm >= 26:
                    return GetInvalid(string.Format(V304, "Z", pkm.AltForm == 26 ? "!" : "?"));
                case 487 when pkm.AltForm == 1 ^ pkm.HeldItem == 112: // Giratina, Origin form only with Griseous Orb
                    return GetInvalid(V308);

                case 493: // Arceus
                    {
                        int form = GetArceusFormFromHeldItem(pkm.HeldItem, pkm.Format);
                        return form != pkm.AltForm ? GetInvalid(V308) : GetValid(V309);
                    }
                case 647: // Keldeo
                {
                    if (pkm.Gen5) // can mismatch in gen5 via BW tutor and transfer up
                        break;
                    int index = Array.IndexOf(pkm.Moves, 548); // Secret Sword
                    bool noSword = index < 0;
                    if (pkm.AltForm == 0 ^ noSword) // mismatch
                        Info.Moves[noSword ? 0 : index] = new CheckMoveResult(Info.Moves[noSword ? 0 : index], Severity.Invalid, V169, CheckIdentifier.Move);
                    break;
                }
                case 649: // Genesect
                    {
                        int form = GetGenesectFormFromHeldItem(pkm.HeldItem);
                        return form != pkm.AltForm ? GetInvalid(V308) : GetValid(V309);
                    }
                case 658: // Greninja
                    if (pkm.AltForm > 1) // Ash Battle Bond active
                        return GetInvalid(V310);
                    if (pkm.AltForm != 0 && !(EncounterMatch is MysteryGift)) // Formes are not breedable, MysteryGift already checked
                        return GetInvalid(string.Format(V304, 0, pkm.AltForm));
                    break;

                case 664: // Scatterbug
                case 665: // Spewpa
                    if (pkm.AltForm > 17) // Fancy & Pokéball
                        return GetInvalid(V311);
                    if (!Legal.CheckVivillonPattern(pkm.AltForm, pkm.Country, pkm.Region))
                        data.AddLine(Get(V312, Severity.Fishy));
                    break;
                case 666: // Vivillon
                    if (pkm.AltForm > 17) // Fancy & Pokéball
                    {
                        if (!(EncounterMatch is MysteryGift))
                            return GetInvalid(V312);
                        return GetValid(V313);
                    }
                    if (!Legal.CheckVivillonPattern(pkm.AltForm, pkm.Country, pkm.Region))
                        data.AddLine(Get(V312, Severity.Fishy));
                    break;

                case 670 when pkm.AltForm == 5: // Floette Eternal Flower -- Never Released
                    if (!(EncounterMatch is MysteryGift))
                        return GetInvalid(V314);
                    return GetValid(V315);
                case 678 when pkm.AltForm != pkm.Gender: // Meowstic
                    return GetInvalid(V203);

                case 773: // Silvally
                    {
                        int form = GetSilvallyFormFromHeldItem(pkm.HeldItem);
                        return form != pkm.AltForm ? GetInvalid(V308) : GetValid(V309);
                    }

                case 744 when Info.EncounterMatch.EggEncounter && pkm.AltForm == 1 && pkm.SM:
                case 745 when Info.EncounterMatch.EggEncounter && pkm.AltForm == 2 && pkm.SM:
                    return GetInvalid(V317);

                // Impossible Egg forms
                case 479 when pkm.IsEgg && pkm.AltForm != 0: // Rotom
                case 676 when pkm.IsEgg && pkm.AltForm != 0: // Furfrou
                    return GetInvalid(V50);

                // Party Only Forms
                case 492: // Shaymin
                case 676: // Furfrou
                case 720: // Hoopa
                    if (pkm.AltForm != 0 && pkm.Box > -1 && pkm.Format <= 6) // has form but stored in box
                        return GetInvalid(V316);
                    break;

                // Battle only Forms with other legal forms allowed
                case 718 when pkm.AltForm >= 4: // Zygarde Complete
                case 774 when pkm.AltForm < 7: // Minior Shield
                case 800 when pkm.AltForm == 3: // Ultra Necrozma
                    return GetInvalid(V310);
                case 800 when pkm.AltForm < 3: // Necrozma Fused forms & default
                case 778 when pkm.AltForm == 2: // Totem disguise Mimikyu
                    return VALID;
            }

            if (pkm.Format >= 7 && Info.Generation < 7 && pkm.AltForm != 0)
            {
                if (pkm.Species == 25 || Legal.AlolanOriginForms.Contains(pkm.Species) || Legal.AlolanVariantEvolutions12.Contains(data.EncounterOriginal.Species))
                    return GetInvalid(V317);
            }

            if (pkm.AltForm != 0 && BattleOnly.Any(arr => arr.Contains(pkm.Species)))
                return GetInvalid(V310);

            return VALID;
        }

        private static int GetArceusFormFromHeldItem(int item, int format)
        {
            if (777 <= item && item <= 793)
                return Array.IndexOf(Legal.Arceus_ZCrystal, item) + 1;

            int form = 0;
            if ((298 <= item && item <= 313) || item == 644)
                form = Array.IndexOf(Legal.Arceus_Plate, item) + 1;
            if (format == 4 && form >= 9)
                return form + 1; // ??? type Form shifts everything by 1
            return form;
        }

        private static int GetSilvallyFormFromHeldItem(int item)
        {
            if ((904 <= item && item <= 920) || item == 644)
                return item - 903;
            return 0;
        }

        private static int GetGenesectFormFromHeldItem(int item)
        {
            if (116 <= item && item <= 119)
                return item - 115;
            return 0;
        }

        private static readonly HashSet<int>[] BattleOnly = {Legal.BattleForms, Legal.BattleMegas, Legal.BattlePrimals};

        private static readonly HashSet<int> SafariFloette = new HashSet<int> {0, 1, 3}; // 0/1/3 - RBY
        private void VerifyFormFriendSafari(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            switch (pkm.Species)
            {
                case 670 when !SafariFloette.Contains(pkm.AltForm): // Floette
                case 671 when !SafariFloette.Contains(pkm.AltForm): // Florges
                    data.AddLine(GetInvalid(V64));
                    break;
                case 710 when pkm.AltForm != 0: // Pumpkaboo
                case 711 when pkm.AltForm != 0: // Goregeist Average
                    data.AddLine(GetInvalid(V6));
                    break;
                case 423 when pkm.AltForm != 0: // Gastrodon West
                    data.AddLine(GetInvalid(V64));
                    break;
                case 586 when pkm.AltForm != 0: // Sawsbuck
                    data.AddLine(GetInvalid(V65));
                    break;
            }
        }
    }
}

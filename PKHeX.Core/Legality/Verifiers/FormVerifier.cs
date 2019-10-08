using System;
using System.Collections.Generic;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    /// <summary>
    /// Verifies the <see cref="PKM.AltForm"/> value.
    /// </summary>
    public sealed class FormVerifier : Verifier
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

        private CheckResult VALID => GetValid(LFormValid);

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
                return GetInvalid(string.Format(LFormInvalidRange, count - 1, pkm.AltForm));

            if (EncounterMatch is EncounterSlot w && w.Type == SlotType.FriendSafari)
            {
                VerifyFormFriendSafari(data);
            }
            else if (EncounterMatch is EncounterEgg)
            {
                if (FormConverter.IsTotemForm(pkm.Species, pkm.AltForm))
                    return GetInvalid(LFormInvalidGame);
            }

            switch (pkm.Species)
            {
                case (int)Species.Pikachu when Info.Generation == 6: // Cosplay
                    bool isStatic = EncounterMatch is EncounterStatic;
                    if (isStatic != (pkm.AltForm != 0))
                        return GetInvalid(isStatic ? LFormPikachuCosplayInvalid : LFormPikachuCosplay);
                    break;

                case (int)Species.Pikachu when Info.Generation == 7: // Cap
                    bool IsValidPikachuCap()
                    {
                        return EncounterMatch switch
                        {
                            WC7 wc7 => (wc7.Form == pkm.AltForm),
                            EncounterStatic s => (s.Form == pkm.AltForm),
                            _ => (pkm.AltForm == 0)
                        };
                    }

                    if (!IsValidPikachuCap())
                    {
                        bool gift = EncounterMatch is WC7 g && g.Form != pkm.AltForm;
                        var msg = gift ? LFormPikachuEventInvalid : LFormInvalidGame;
                        return GetInvalid(msg);
                    }
                    break;
                case (int)Species.Unown when Info.Generation == 2 && pkm.AltForm >= 26:
                    return GetInvalid(string.Format(LFormInvalidRange, "Z", pkm.AltForm == 26 ? "!" : "?"));
                case (int)Species.Giratina when pkm.AltForm == 1 ^ pkm.HeldItem == 112: // Giratina, Origin form only with Griseous Orb
                    return GetInvalid(LFormItemInvalid);

                case (int)Species.Arceus:
                    {
                        int form = GetArceusFormFromHeldItem(pkm.HeldItem, pkm.Format);
                        return form != pkm.AltForm ? GetInvalid(LFormItemInvalid) : GetValid(LFormItem);
                    }
                case (int)Species.Keldeo:
                {
                    if (pkm.Gen5) // can mismatch in gen5 via BW tutor and transfer up
                        break;
                    int index = Array.IndexOf(pkm.Moves, 548); // Secret Sword
                    bool noSword = index < 0;
                    if (pkm.AltForm == 0 ^ noSword) // mismatch
                        Info.Moves[noSword ? 0 : index] = new CheckMoveResult(Info.Moves[noSword ? 0 : index], Severity.Invalid, LMoveKeldeoMismatch, CheckIdentifier.Move);
                    break;
                }
                case (int)Species.Genesect:
                    {
                        int form = GetGenesectFormFromHeldItem(pkm.HeldItem);
                        return form != pkm.AltForm ? GetInvalid(LFormItemInvalid) : GetValid(LFormItem);
                    }
                case (int)Species.Greninja:
                    if (pkm.AltForm > 1) // Ash Battle Bond active
                        return GetInvalid(LFormBattle);
                    if (pkm.AltForm != 0 && !(EncounterMatch is MysteryGift)) // Formes are not breedable, MysteryGift already checked
                        return GetInvalid(string.Format(LFormInvalidRange, 0, pkm.AltForm));
                    break;

                case (int)Species.Scatterbug:
                case (int)Species.Spewpa:
                    if (pkm.AltForm > 17) // Fancy & Pokéball
                        return GetInvalid(LFormVivillonEventPre);
                    if (!Legal.CheckVivillonPattern(pkm.AltForm, pkm.Country, pkm.Region))
                        data.AddLine(Get(LFormVivillonInvalid, Severity.Fishy));
                    break;
                case (int)Species.Vivillon:
                    if (pkm.AltForm > 17) // Fancy & Pokéball
                    {
                        if (!(EncounterMatch is MysteryGift))
                            return GetInvalid(LFormVivillonInvalid);
                        return GetValid(LFormVivillon);
                    }
                    if (!Legal.CheckVivillonPattern(pkm.AltForm, pkm.Country, pkm.Region))
                        data.AddLine(Get(LFormVivillonInvalid, Severity.Fishy));
                    break;

                case (int)Species.Floette when pkm.AltForm == 5: // Floette Eternal Flower -- Never Released
                    if (!(EncounterMatch is MysteryGift))
                        return GetInvalid(LFormEternalInvalid);
                    return GetValid(LFormEternal);
                case (int)Species.Meowstic when pkm.AltForm != pkm.Gender:
                    return GetInvalid(LGenderInvalidNone);

                case (int)Species.Silvally:
                    {
                        int form = GetSilvallyFormFromHeldItem(pkm.HeldItem);
                        return form != pkm.AltForm ? GetInvalid(LFormItemInvalid) : GetValid(LFormItem);
                    }

                case (int)Species.Lillipup when Info.EncounterMatch.EggEncounter && pkm.AltForm == 1 && pkm.SM:
                case (int)Species.Lycanroc when Info.EncounterMatch.EggEncounter && pkm.AltForm == 2 && pkm.SM:
                    return GetInvalid(LFormInvalidGame);

                // Impossible Egg forms
                case (int)Species.Rotom when pkm.IsEgg && pkm.AltForm != 0:
                case (int)Species.Furfrou when pkm.IsEgg && pkm.AltForm != 0:
                    return GetInvalid(LEggSpecies);

                // Party Only Forms
                case (int)Species.Shaymin:
                case (int)Species.Furfrou:
                case (int)Species.Hoopa:
                    if (pkm.AltForm != 0 && pkm.Box > -1 && pkm.Format <= 6) // has form but stored in box
                        return GetInvalid(LFormParty);
                    break;

                // Battle only Forms with other legal forms allowed
                case (int)Species.Zygarde when pkm.AltForm >= 4: // Zygarde Complete
                case (int)Species.Minior when pkm.AltForm < 7: // Minior Shield
                case (int)Species.Necrozma when pkm.AltForm == 3: // Ultra Necrozma
                    return GetInvalid(LFormBattle);
                case (int)Species.Necrozma when pkm.AltForm < 3: // Necrozma Fused forms & default
                case (int)Species.Mimikyu when pkm.AltForm == 2: // Totem disguise Mimikyu
                    return VALID;
            }

            if (pkm.Format >= 7 && Info.Generation < 7 && pkm.AltForm != 0)
            {
                if (pkm.Species == 25 || Legal.AlolanOriginForms.Contains(pkm.Species) || Legal.AlolanVariantEvolutions12.Contains(data.EncounterOriginal.Species))
                    return GetInvalid(LFormInvalidGame);
            }
            if (pkm.Format >= 8 && Info.Generation < 8 && pkm.AltForm != 0)
            {
                if (pkm.Species == 25 || Legal.GalarOriginForms.Contains(pkm.Species) || Legal.GalarVariantFormEvolutions.Contains(data.EncounterOriginal.Species))
                    return GetInvalid(LFormInvalidGame);
            }

            if (pkm.AltForm != 0 && BattleOnly.Contains(pkm.Species))
                return GetInvalid(LFormBattle);

            return VALID;
        }

        private static int GetArceusFormFromHeldItem(int item, int format)
        {
            if (777 <= item && item <= 793)
                return Array.IndexOf(Legal.Arceus_ZCrystal, (ushort)item) + 1;

            int form = 0;
            if ((298 <= item && item <= 313) || item == 644)
                form = Array.IndexOf(Legal.Arceus_Plate, (ushort)item) + 1;
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

        private static readonly HashSet<int> BattleOnly;
        private static readonly HashSet<int> SafariFloette = new HashSet<int> { 0, 1, 3 }; // 0/1/3 - RBY

        static FormVerifier()
        {
            BattleOnly = new HashSet<int>();
            BattleOnly.UnionWith(Legal.BattleForms);
            BattleOnly.UnionWith(Legal.BattleMegas);
            BattleOnly.UnionWith(Legal.BattlePrimals);
        }

        private void VerifyFormFriendSafari(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            switch (pkm.Species)
            {
                case (int)Species.Floette when !SafariFloette.Contains(pkm.AltForm): // Floette
                case (int)Species.Florges when !SafariFloette.Contains(pkm.AltForm): // Florges
                    data.AddLine(GetInvalid(LFormSafariFlorgesColor));
                    break;
                case 710 when pkm.AltForm != 0: // Pumpkaboo
                case (int)Species.Gourgeist when pkm.AltForm != 0: // Average
                    data.AddLine(GetInvalid(LFormSafariPumpkabooAverage));
                    break;
                case (int)Species.Gastrodon when pkm.AltForm != 0: // West
                    data.AddLine(GetInvalid(LFormSafariFlorgesColor));
                    break;
                case (int)Species.Sawsbuck when pkm.AltForm != 0: // Sawsbuck
                    data.AddLine(GetInvalid(LFormSafariSawsbuckSpring));
                    break;
            }
        }
    }
}

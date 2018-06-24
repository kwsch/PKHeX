using System;
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

            var PersonalInfo = data.PersonalInfo;
            var EncounterMatch = data.EncounterMatch;
            var Info = data.Info;

            int count = PersonalInfo.FormeCount;
            if (count <= 1 && pkm.AltForm == 0)
                return; // no forms to check

            if (!PersonalInfo.IsFormeWithinRange(pkm.AltForm) && !FormConverter.IsValidOutOfBoundsForme(pkm.Species, pkm.AltForm, Info.Generation))
            {
                data.AddLine(GetInvalid(string.Format(V304, count - 1, pkm.AltForm)));
                return;
            }

            if (EncounterMatch is EncounterSlot w && w.Type == SlotType.FriendSafari)
                VerifyFormFriendSafari(data);
            else if (EncounterMatch is EncounterEgg)
            {
                if (FormConverter.IsTotemForm(pkm.Species, pkm.AltForm))
                {
                    data.AddLine(GetInvalid(V317));
                    return;
                }
            }

            switch (pkm.Species)
            {
                case 25 when Info.Generation == 6: // Pikachu Cosplay
                    bool isStatic = EncounterMatch is EncounterStatic;
                    if (isStatic != (pkm.AltForm != 0))
                    {
                        string msg = isStatic ? V305 : V306;
                        data.AddLine(GetInvalid(msg));
                        return;
                    }
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
                        data.AddLine(GetInvalid(msg));
                        return;
                    }
                    break;
                case 201 when Info.Generation == 2 && pkm.AltForm >= 26:
                    data.AddLine(GetInvalid(string.Format(V304, "Z", pkm.AltForm == 26 ? "!" : "?")));
                    break;
                case 487: // Giratina
                    if (pkm.AltForm == 1 ^ pkm.HeldItem == 112) // Origin form only with Griseous Orb
                    {
                        data.AddLine(GetInvalid(V308));
                        return;
                    }
                    break;
                case 493: // Arceus
                {
                    int item = pkm.HeldItem;
                    int form = 0;
                    if (298 <= item && item <= 313 || item == 644)
                        form = Array.IndexOf(Legal.Arceus_Plate, item) + 1;
                    else if (777 <= item && item <= 793)
                        form = Array.IndexOf(Legal.Arceus_ZCrystal, item) + 1;
                    if (pkm.Format == 4 && form >= 9)
                        form++; // ??? type Form shifts everything by 1

                    if (form != pkm.AltForm)
                        data.AddLine(GetInvalid(V308));
                    else if (form != 0)
                        data.AddLine(GetValid(V309));
                }
                    break;
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
                    int item = pkm.HeldItem;
                    int form = 0;
                    if (116 <= item && item <= 119)
                        form = item - 115;

                    if (form != pkm.AltForm)
                        data.AddLine(GetInvalid(V308));
                    else
                        data.AddLine(GetValid(V309));
                }
                    break;
                case 658: // Greninja
                    if (pkm.AltForm > 1) // Ash Battle Bond active
                    {
                        data.AddLine(GetInvalid(V310));
                        return;
                    }
                    if (pkm.AltForm != 0 && !(EncounterMatch is MysteryGift)) // Formes are not breedable, MysteryGift already checked
                    {
                        data.AddLine(GetInvalid(string.Format(V304, 0, pkm.AltForm)));
                        return;
                    }
                    break;
                case 664: // Scatterbug
                case 665: // Spewpa
                    if (pkm.AltForm > 17) // Fancy & Pokéball
                    {
                        data.AddLine(GetInvalid(V311));
                        return;
                    }
                    if (!Legal.CheckVivillonPattern(pkm.AltForm, pkm.Country, pkm.Region))
                        data.AddLine(Get(V312, Severity.Fishy));
                    break;
                case 666: // Vivillon
                    if (pkm.AltForm > 17) // Fancy & Pokéball
                    {
                        if (!(EncounterMatch is MysteryGift))
                            data.AddLine(GetInvalid(V312));
                        else
                            data.AddLine(GetValid(V313));

                        return;
                    }
                    if (!Legal.CheckVivillonPattern(pkm.AltForm, pkm.Country, pkm.Region))
                        data.AddLine(Get(V312, Severity.Fishy));
                    break;
                case 670: // Floette
                    if (pkm.AltForm == 5) // Eternal Flower -- Never Released
                    {
                        if (!(EncounterMatch is MysteryGift))
                            data.AddLine(GetInvalid(V314));
                        else
                            data.AddLine(GetValid(V315));

                        return;
                    }
                    break;
                case 678: // Meowstic
                    if (pkm.AltForm != pkm.Gender)
                        data.AddLine(GetInvalid(V203));
                    break;
                case 773: // Silvally
                {
                    int item = pkm.HeldItem;
                    int form = 0;
                    if ((904 <= item && item <= 920) || item == 644)
                        form = item - 903;
                    if (form != pkm.AltForm)
                        data.AddLine(GetInvalid(V308));
                    else if (form != 0)
                        data.AddLine(GetValid(V309));
                    break;
                }

                case 744 when Info.EncounterMatch.EggEncounter && pkm.AltForm == 1 && pkm.SM:
                case 745 when Info.EncounterMatch.EggEncounter && pkm.AltForm == 2 && pkm.SM:
                    data.AddLine(GetInvalid(V317));
                    return;

                // Impossible Egg forms
                case 479 when pkm.IsEgg: // Rotom
                case 676 when pkm.IsEgg: // Furfrou
                    if (pkm.AltForm != 0) // has form
                    {
                        data.AddLine(GetInvalid(V50));
                        return;
                    }
                    break;

                // Party Only Forms
                case 492: // Shaymin
                case 676: // Furfrou
                case 720: // Hoopa
                    if (pkm.AltForm != 0 && pkm.Box > -1 && pkm.Format <= 6) // has form but stored in box
                    {
                        data.AddLine(GetInvalid(V316));
                        return;
                    }
                    break;

                // Battle only Forms with other legal forms allowed
                case 718 when pkm.AltForm >= 4: // Zygarde Complete
                case 774 when pkm.AltForm < 7: // Minior Shield
                case 800 when pkm.AltForm == 3: // Ultra Necrozma
                    data.AddLine(GetInvalid(V310));
                    return;
                case 800 when pkm.AltForm < 3: // Necrozma Fused forms & default
                case 778 when pkm.AltForm == 2: // Totem disguise Mimikyu
                    data.AddLine(GetValid(V318));
                    return;
            }

            if (pkm.Format >= 7 && Info.Generation < 7 && pkm.AltForm != 0)
            {
                if (pkm.Species == 25 || Legal.AlolanOriginForms.Contains(pkm.Species)
                    || Legal.AlolanVariantEvolutions12.Contains(data.EncounterOriginal.Species))
                { data.AddLine(GetInvalid(V317)); return; }
            }
            if (pkm.AltForm > 0 && new[] { Legal.BattleForms, Legal.BattleMegas, Legal.BattlePrimals }.Any(arr => arr.Contains(pkm.Species)))
            { data.AddLine(GetInvalid(V310)); return; }

            data.AddLine(GetValid(V318));
        }


        private void VerifyFormFriendSafari(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            switch (pkm.Species)
            {
                case 670: // Floette
                case 671: // Florges
                    if (!new[] { 0, 1, 3 }.Contains(pkm.AltForm)) // 0/1/3 - RBY
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

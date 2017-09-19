using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    public partial class LegalityAnalysis
    {
        private void VerifyGender()
        {
            if (pkm.PersonalInfo.Gender == 255 && pkm.Gender != 2)
                AddLine(Severity.Invalid, V203, CheckIdentifier.Gender);

            // Check for PID relationship to Gender & Nature if applicable
            int gen = Info.Generation;

            bool PIDGender = 3 <= gen && gen <= 5;
            if (!PIDGender)
                return;

            bool genderValid = pkm.IsGenderValid();
            if (!genderValid && pkm.Format > 5 && (pkm.Species == 183 || pkm.Species == 184))
            {
                var gv = pkm.PID & 0xFF;
                if (gv > 63 && pkm.Gender == 1) // evolved from azurill after transferring to keep gender
                    genderValid = true;
            }

            if (genderValid)
                AddLine(Severity.Valid, V250, CheckIdentifier.Gender);
            else
                AddLine(Severity.Invalid, V251, CheckIdentifier.Gender);

            bool PIDNature = gen != 5;
            if (!PIDNature)
                return;

            if (pkm.EncryptionConstant % 25 == pkm.Nature)
                AddLine(Severity.Valid, V252, CheckIdentifier.Nature);
            else
                AddLine(Severity.Invalid, V253, CheckIdentifier.Nature);
        }
        private void VerifyItem()
        {
            if (!Legal.IsHeldItemAllowed(pkm.HeldItem, pkm.Format))
                AddLine(Severity.Invalid, V204, CheckIdentifier.Form);
            if (pkm.Format == 3 && pkm.HeldItem == 175)
                VerifyEReaderBerry();
            if (pkm.IsEgg && pkm.HeldItem != 0)
                AddLine(Severity.Invalid, V419, CheckIdentifier.Egg);
        }
        private void VerifyEReaderBerry()
        {
            if (Legal.EReaderBerryIsEnigma) // no E-Reader berry data provided, can't hold berry.
            {
                AddLine(Severity.Invalid, V204, CheckIdentifier.Form);
                return;
            }

            var matchUSA = Legal.EReaderBerriesNames_USA.Contains(Legal.EReaderBerryName);
            var matchJP = Legal.EReaderBerriesNames_JP.Contains(Legal.EReaderBerryName);
            if (!matchJP && !matchUSA) // Does not match any released E-Reader berry
                AddLine(Severity.Invalid, V369, CheckIdentifier.Form);
            else if (matchJP && !Legal.SavegameJapanese) // E-Reader is region locked
                AddLine(Severity.Invalid, V370, CheckIdentifier.Form);
            else if (matchUSA && Legal.SavegameJapanese) // E-Reader is region locked
                AddLine(Severity.Invalid, V371, CheckIdentifier.Form);
        }
        private void VerifyECPID()
        {
            if (pkm.Format >= 6)
                VerifyEC();
            if (EncounterMatch.Species == 265)
                VerifyECPIDWurmple();

            if (pkm.PID == 0)
                AddLine(Severity.Fishy, V207, CheckIdentifier.PID);

            if (Info.Generation >= 6 && pkm.PID == pkm.EncryptionConstant)
                AddLine(Severity.Invalid, V208, CheckIdentifier.PID); // better to flag than 1:2^32 odds since RNG is not feasible to yield match

            switch (EncounterMatch)
            {
                case EncounterStatic s:
                    if (s.Shiny != null && (bool)s.Shiny ^ pkm.IsShiny)
                        AddLine(Severity.Invalid, V209, CheckIdentifier.Shiny);
                    
                    // gen5 correlation
                    if (Info.Generation != 5)
                        break;
                    if (s.Location == 75) // Entree Forest
                        break;
                    if (s.Gift || s.Roaming || s.Ability != 4)
                        break;
                    if (s.NSparkle)
                        break;
                    VerifyG5PID_IDCorrelation();
                    break;
                case EncounterSlot w:
                    if (pkm.IsShiny && w.Type == SlotType.HiddenGrotto)
                        AddLine(Severity.Invalid, V221, CheckIdentifier.Shiny);
                    if (Info.Generation == 5 && w.Type != SlotType.HiddenGrotto)
                        VerifyG5PID_IDCorrelation();
                    break;
                case PCD d: // fixed PID
                    if (d.Gift.PK.PID != 1 && pkm.EncryptionConstant != d.Gift.PK.PID)
                        AddLine(Severity.Invalid, V410, CheckIdentifier.Shiny);
                    break;
            }
        }
        private void VerifyG5PID_IDCorrelation()
        {
            var pid = pkm.EncryptionConstant;
            var result = (pid & 1) ^ (pid >> 31) ^ (pkm.TID & 1) ^ (pkm.SID & 1);
            if (result != 0)
                AddLine(Severity.Invalid, V411, CheckIdentifier.PID);
        }
        private void VerifyECPIDWurmple()
        {
            uint evoVal = PKX.GetWurmpleEvoVal(pkm.EncryptionConstant);

            if (pkm.Species == 265)
                AddLine(Severity.Valid, string.Format(V212, evoVal == 0 ? SpeciesStrings[267] : SpeciesStrings[269]), CheckIdentifier.EC);
            else if (evoVal != Array.IndexOf(Legal.WurmpleEvolutions, pkm.Species) / 2)
                AddLine(Severity.Invalid, V210, CheckIdentifier.EC);
        }
        private void VerifyEC()
        {
            if (pkm.EncryptionConstant == 0)
                AddLine(Severity.Fishy, V201, CheckIdentifier.EC);
            if (3 <= Info.Generation && Info.Generation <= 5)
                VerifyTransferEC();
            else
            {
                int xor = pkm.TSV ^ pkm.PSV;
                if (xor < 16 && xor >= 8 && (pkm.PID ^ 0x80000000) == pkm.EncryptionConstant)
                    AddLine(Severity.Fishy, V211, CheckIdentifier.EC);
            }
        }
        private void VerifyTransferEC()
        {
            // When transferred to Generation 6, the Encryption Constant is copied from the PID.
            // The PID is then checked to see if it becomes shiny with the new Shiny rules (>>4 instead of >>3)
            // If the PID is nonshiny->shiny, the top bit is flipped.

            // Check to see if the PID and EC are properly configured.
            bool xorPID = ((pkm.TID ^ pkm.SID ^ (int)(pkm.PID & 0xFFFF) ^ (int)(pkm.PID >> 16)) & ~0x7) == 8;
            bool valid = xorPID
                ? pkm.EncryptionConstant == (pkm.PID ^ 0x8000000)
                : pkm.EncryptionConstant == pkm.PID;

            if (!valid)
                AddLine(Severity.Invalid, xorPID ? V215 : V216, CheckIdentifier.EC);
        }
        #region verifyNickname
        private void VerifyNickname()
        {
            // If the Pokémon is not nicknamed, it should match one of the language strings.
            if (pkm.Nickname.Length == 0)
            {
                AddLine(Severity.Invalid, V2, CheckIdentifier.Nickname);
                return;
            }
            if (pkm.Species > PKX.SpeciesLang[0].Length)
            {
                AddLine(Severity.Indeterminate, V3, CheckIdentifier.Nickname);
                return;
            }

            if (pkm.VC && pkm.IsNicknamed)
            {
                VerifyG1NicknameWithinBounds(pkm.Nickname);
            }
            else if (EncounterMatch is MysteryGift m)
            {
                if (pkm.IsNicknamed && !m.IsEgg)
                    AddLine(Severity.Fishy, V0, CheckIdentifier.Nickname);
            }

            if (!Encounter.Valid)
                return;

            int maxLanguageID = Legal.GetMaxLanguageID(Info.Generation);
            if (pkm.Language == 6 || pkm.Language > maxLanguageID)
            {
                AddLine(Severity.Indeterminate, string.Format(V5, "<=" + maxLanguageID, pkm.Language), CheckIdentifier.Language);
                return;
            }

            if (Type == typeof(EncounterTrade))
            {
                VerifyNicknameTrade();
                return;
            }

            if (pkm.IsEgg)
            {
                VerifyNicknameEgg();
                return;
            }

            string nickname = pkm.Nickname.Replace("'", "’");
            if (pkm.IsNicknamed)
            {
                for (int i = 0; i < PKX.SpeciesLang.Length; i++)
                {
                    string[] lang = PKX.SpeciesLang[i];
                    int index = Array.IndexOf(lang, nickname);
                    if (index < 0)
                        continue;

                    AddLine(Severity.Fishy, index == pkm.Species && i != pkm.Language
                        ? V15
                        : V16, CheckIdentifier.Nickname);
                    return;
                }
                if (nickname.Any(c => 0x4E00 <= c && c <= 0x9FFF)) // East Asian Scripts
                {
                    AddLine(Severity.Invalid, V222, CheckIdentifier.Nickname);
                    return;
                }
                AddLine(Severity.Valid, V17, CheckIdentifier.Nickname);
            }
            else if (pkm.Format < 3)
            {
                // pk1/pk2 IsNicknamed getter checks for match, logic should only reach here if matches.
                AddLine(Severity.Valid, V18, CheckIdentifier.Nickname);
            }
            else
            {
                // Can't have another language name if it hasn't evolved or wasn't a language-traded egg.
                bool evolved = Legal.IsNotBaseSpecies(pkm);
                bool match = PKX.GetSpeciesNameGeneration(pkm.Species, pkm.Language, pkm.Format) == nickname;
                if (pkm.WasTradedEgg || evolved)
                    match |= !PKX.IsNicknamedAnyLanguage(pkm.Species, nickname, pkm.Format);
                if (!match && pkm.Format == 5 && !pkm.IsNative) // transfer
                {
                    if (evolved)
                        match |= !PKX.IsNicknamedAnyLanguage(pkm.Species, nickname, 4);
                    else
                        match |= PKX.GetSpeciesNameGeneration(pkm.Species, pkm.Language, 4) == nickname;
                }

                if (!match)
                {
                    if ((EncounterMatch as MysteryGift)?.CardID == 2046 && (pkm.SID << 16 | pkm.TID) == 0x79F57B49)
                        AddLine(Severity.Valid, V19, CheckIdentifier.Nickname);
                    else
                        AddLine(Severity.Invalid, V20, CheckIdentifier.Nickname);
                }
                else
                    AddLine(Severity.Valid, V18, CheckIdentifier.Nickname);
            }

            // Non-nicknamed strings have already been checked.
            if (Legal.CheckWordFilter && pkm.IsNicknamed && WordFilter.IsFiltered(nickname, out string bad))
                AddLine(Severity.Invalid, $"Wordfilter: {bad}", CheckIdentifier.Nickname);
        }
        private void VerifyNicknameEgg()
        {
            switch (pkm.Format)
            {
                case 4:
                    if (pkm.IsNicknamed) // gen4 doesn't use the nickname flag for eggs
                        AddLine(Severity.Invalid, V224, CheckIdentifier.Egg);
                    break;
                case 7:
                    if (EncounterMatch is EncounterStatic ^ !pkm.IsNicknamed) // gen7 doesn't use for ingame gifts
                        AddLine(Severity.Invalid, pkm.IsNicknamed ? V224 : V12, CheckIdentifier.Egg);
                    break;
                default:
                    if (!pkm.IsNicknamed)
                        AddLine(Severity.Invalid, V12, CheckIdentifier.Egg);
                    break;
            }

            if (pkm.Format == 2 && pkm.IsEgg && !PKX.IsNicknamedAnyLanguage(0, pkm.Nickname, 2))
                AddLine(Severity.Valid, V14, CheckIdentifier.Egg);
            else if (PKX.GetSpeciesNameGeneration(0, pkm.Language, Info.Generation) != pkm.Nickname)
                AddLine(Severity.Invalid, V13, CheckIdentifier.Egg);
            else
                AddLine(Severity.Valid, V14, CheckIdentifier.Egg);
        }
        private void VerifyNicknameTrade()
        {
            string[] validOT = new string[0];
            int index = -1;
            if (pkm.XY)
            {
                validOT = Encounters6.TradeXY[pkm.Language];
                index = Array.IndexOf(Encounters6.TradeGift_XY, EncounterMatch);
            }
            else if (pkm.AO)
            {
                validOT = Encounters6.TradeAO[pkm.Language];
                index = Array.IndexOf(Encounters6.TradeGift_AO, EncounterMatch);
            }
            else if (pkm.SM)
            {
                // TODO
                AddLine(Severity.Valid, V194, CheckIdentifier.Nickname);
                return;
            }
            else if (pkm.Format <= 2 || pkm.VC)
            {
                var et = (EncounterOriginalGB ?? EncounterMatch) as EncounterTrade;
                if (et?.TID == 0) // Gen1 Trade
                {
                    if (!EncounterGenerator.IsEncounterTrade1Valid(pkm))
                        AddLine(Severity.Invalid, V10, CheckIdentifier.Trainer);
                }
                else // Gen2
                {
                    return; // already checked all relevant properties when fetching with getValidEncounterTradeVC2
                }
                return;
            }
            else if (3 <= Info.Generation && Info.Generation <= 5)
            {
                // Trades for JPN games have language ID of 0, not 1.
                if (pkm.BW && pkm.Format == 5 && pkm.Language == 1)
                    AddLine(Severity.Invalid, string.Format(V5, 0, 1), CheckIdentifier.Language);

                // Suppressing temporarily
                return;
            }

            if (validOT.Length == 0)
            {
                AddLine(Severity.Indeterminate, V7, CheckIdentifier.Trainer);
                return;
            }
            if (index == -1 || validOT.Length < index * 2)
            {
                AddLine(Severity.Indeterminate, V8, CheckIdentifier.Trainer);
                return;
            }

            string nick = validOT[index];
            string OT = validOT[validOT.Length / 2 + index];

            if (nick != pkm.Nickname)
                AddLine(Severity.Fishy, V9, CheckIdentifier.Nickname);
            else if (OT != pkm.OT_Name)
                AddLine(Severity.Invalid, V10, CheckIdentifier.Trainer);
            else
                AddLine(Severity.Valid, V11, CheckIdentifier.Nickname);
        }
        #endregion
        private void VerifyEVs()
        {
            var evs = pkm.EVs;
            int sum = evs.Sum();
            if (sum > 0 && pkm.IsEgg)
                AddLine(Severity.Invalid, V22, CheckIdentifier.EVs);
            if (pkm.Format >= 3 && sum > 510)
                AddLine(Severity.Invalid, V25, CheckIdentifier.EVs);
            if (pkm.Format >= 6 && evs.Any(ev => ev > 252))
                AddLine(Severity.Invalid, V26, CheckIdentifier.EVs);
            if (pkm.Format == 4 && pkm.Gen4 && EncounterMatch.LevelMin == 100)
            {
                // Cannot EV train at level 100 -- Certain events are distributed at level 100.
                if (evs.Any(ev => ev > 100)) // EVs can only be increased by vitamins to a max of 100.
                    AddLine(Severity.Invalid, V367, CheckIdentifier.EVs);
            }
            else if (pkm.Format < 5)
            {
                var maxEV = pkm.Format <= 2 ? 25600 : 100; // Vitamin Max
                // Cannot EV train above 100 without increasing EXP
                if (PKX.GetEXP(EncounterMatch.LevelMin, pkm.Species) == pkm.EXP && evs.Any(ev => ev > maxEV))
                    AddLine(Severity.Invalid, string.Format(V418, maxEV), CheckIdentifier.EVs);

                if (pkm.Format < 3)
                    return;
            }

            // Only one of the following can be true: 0, 508, and x%6!=0
            if (sum == 0 && !EncounterMatch.IsWithinRange(pkm))
                AddLine(Severity.Fishy, V23, CheckIdentifier.EVs);
            else if (sum == 508)
                AddLine(Severity.Fishy, V24, CheckIdentifier.EVs);
            else if (evs[0] != 0 && evs.All(ev => evs[0] == ev))
                AddLine(Severity.Fishy, V27, CheckIdentifier.EVs);
        }
        private void VerifyIVs()
        {
            if (EncounterMatch is EncounterStatic s && s.IV3)
            {
                int IVCount = 3;
                if (s.Version == GameVersion.RBY && pkm.Species == 151)
                    IVCount = 5; // VC Mew
                if (pkm.IVs.Count(iv => iv == 31) < IVCount)
                {
                    AddLine(Severity.Invalid, string.Format(V28, IVCount), CheckIdentifier.IVs);
                    return;
                }
            }
            if (EncounterMatch is EncounterSlot w && w.Type == SlotType.FriendSafari)
            {
                if (pkm.IVs.Count(iv => iv == 31) < 2)
                {
                    AddLine(Severity.Invalid, V29, CheckIdentifier.IVs);
                    return;
                }
            }
            if (EncounterMatch is MysteryGift g)
            {
                int[] IVs;
                switch (g.Format)
                {
                    case 7: IVs = ((WC7)EncounterMatch).IVs; break;
                    case 6: IVs = ((WC6)EncounterMatch).IVs; break;
                    case 5: IVs = ((PGF)EncounterMatch).IVs; break;
                    default: IVs = null; break;
                }

                if (IVs != null)
                {
                    var pkIVs = pkm.IVs;
                    bool valid = true;
                    for (int i = 0; i < 6; i++)
                        if (IVs[i] <= 31 && IVs[i] != pkIVs[i])
                            valid = false;
                    if (!valid)
                        AddLine(Severity.Invalid, V30, CheckIdentifier.IVs);
                    bool IV3 = IVs[0] == 0xFE;
                    if (IV3 && pkm.IVs.Count(iv => iv == 31) < 3)
                        AddLine(Severity.Invalid, string.Format(V28, 3), CheckIdentifier.IVs);
                }
            }
            if (pkm.IVs.Sum() == 0)
                AddLine(Severity.Fishy, V31, CheckIdentifier.IVs);
            else if (pkm.IVs[0] < 30 && pkm.IVs.All(iv => pkm.IVs[0] == iv))
                AddLine(Severity.Fishy, V32, CheckIdentifier.IVs);
        }
        private void VerifyDVs()
        {
            // todo
        }
        #region VerifyOT
        private void VerifyOT()
        {
            if (Type == typeof(EncounterTrade))
                return; // Already matches Encounter information

            if (EncounterMatch is MysteryGift g && !g.IsEgg)
                return; // Already matches Encounter information

            if (EncounterMatch is EncounterStatic s && s.NSparkle)
                return; // Already checked by VerifyMisc

            if (pkm.TID == 0 && pkm.SID == 0)
                AddLine(Severity.Fishy, V33, CheckIdentifier.Trainer);
            else if (pkm.VC)
            {
                if (pkm.SID != 0)
                    AddLine(Severity.Invalid, V34, CheckIdentifier.Trainer);
            }
            else if (pkm.TID == pkm.SID)
                AddLine(Severity.Fishy, V35, CheckIdentifier.Trainer);
            else if (pkm.TID == 0)
                AddLine(Severity.Fishy, V36, CheckIdentifier.Trainer);
            else if (pkm.SID == 0)
                AddLine(Severity.Fishy, V37, CheckIdentifier.Trainer);
            else if (pkm.TID == 12345 && pkm.SID == 54321 || pkm.OT_Name.StartsWith("PKHeX"))
                AddLine(Severity.Fishy, V417, CheckIdentifier.Trainer);

            if (pkm.VC)
                VerifyG1OT();

            if (Legal.CheckWordFilter)
            {
                if (WordFilter.IsFiltered(pkm.OT_Name, out string bad))
                    AddLine(Severity.Invalid, $"Wordfilter: {bad}", CheckIdentifier.Nickname);
                if (WordFilter.IsFiltered(pkm.HT_Name, out bad))
                    AddLine(Severity.Invalid, $"Wordfilter: {bad}", CheckIdentifier.Nickname);
            }
        }
        private void VerifyG1OT()
        {
            string tr = pkm.OT_Name;

            VerifyG1OTWithinBounds(tr);
            if ((EncounterMatch as EncounterStatic)?.Version == GameVersion.Stadium)
                VerifyG1OTStadium(tr);

            if (pkm.Species == 151)
            {
                if (tr != "GF" && tr != "ゲーフリ") // if there are more events with special OTs, may be worth refactoring
                    AddLine(Severity.Invalid, V39, CheckIdentifier.Trainer);
            }

            if (pkm.OT_Gender == 1 && (pkm.Format == 2 && pkm.Met_Location == 0 || !Info.Game.Contains(GameVersion.C)))
                AddLine(Severity.Invalid, V408, CheckIdentifier.Trainer);
        }
        private void VerifyG1OTWithinBounds(string str)
        {
            if (StringConverter.GetIsG1English(str))
            {
                if (str.Length > 7)
                    AddLine(Severity.Invalid, V38, CheckIdentifier.Trainer);
            }
            else if (StringConverter.GetIsG1Japanese(str))
            {
                if (str.Length > 5)
                    AddLine(Severity.Invalid, V38, CheckIdentifier.Trainer);
            }
            else if (pkm is PK2 pk2 && pk2.Korean)
            {
                if (str.Length > 5)
                    AddLine(Severity.Invalid, V38, CheckIdentifier.Trainer);
            }
            else
            {
                AddLine(Severity.Invalid, V421, CheckIdentifier.Trainer);
            }
        }
        private void VerifyG1NicknameWithinBounds(string str)
        {
            if (StringConverter.GetIsG1English(str))
            {
                if (str.Length > 10)
                    AddLine(Severity.Invalid, V1, CheckIdentifier.Trainer);
            }
            else if (StringConverter.GetIsG1Japanese(str))
            {
                if (str.Length > 5)
                    AddLine(Severity.Invalid, V1, CheckIdentifier.Trainer);
            }
            else
            {
                AddLine(Severity.Invalid, V422, CheckIdentifier.Trainer);
            }
        }
        private void VerifyG1OTStadium(string tr)
        {
            bool jp = (pkm as PK1)?.Japanese ?? (pkm as PK2)?.Japanese ?? pkm.Language != 2;
            bool valid = GetIsStadiumOTIDValid(jp, tr);
            if (!valid)
                AddLine(Severity.Invalid, V402, CheckIdentifier.Trainer);
            else
                AddLine(Severity.Valid, jp ? V404 : V403, CheckIdentifier.Trainer);
        }
        private bool GetIsStadiumOTIDValid(bool jp, string tr)
        {
            if (jp)
                return tr == "スタジアム" && pkm.TID == 1999;
            return tr == "STADIUM" && pkm.TID == 2000;
        }
        #endregion
        private void VerifyHyperTraining()
        {
            if (pkm.Format < 7)
                return; // No Hyper Training before Gen7

            var IVs = new[] { pkm.IV_HP, pkm.IV_ATK, pkm.IV_DEF, pkm.IV_SPA, pkm.IV_SPD, pkm.IV_SPE };
            var HTs = new[] { pkm.HT_HP, pkm.HT_ATK, pkm.HT_DEF, pkm.HT_SPA, pkm.HT_SPD, pkm.HT_SPE };

            if (HTs.Any(ht => ht) && pkm.CurrentLevel != 100)
                AddLine(Severity.Invalid, V40, CheckIdentifier.IVs);

            if (IVs.All(iv => iv == 31) && HTs.Any(ht => ht))
                AddLine(Severity.Invalid, V41, CheckIdentifier.IVs);
            else
            {
                for (int i = 0; i < 6; i++) // Check individual IVs
                {
                    if (!HTs[i] || IVs[i] != 31)
                        continue;
                    AddLine(Severity.Invalid, V42, CheckIdentifier.IVs);
                    break;
                }
            }
        }
        #region VerifyEncounter
        private void VerifyFormFriendSafari()
        {
            switch (pkm.Species)
            {
                case 670: // Floette
                case 671: // Florges
                    if (!new[] { 0, 1, 3 }.Contains(pkm.AltForm)) // 0/1/3 - RBY
                        AddLine(Severity.Invalid, V64, CheckIdentifier.Form);
                    break;
                case 710: // Pumpkaboo
                case 711: // Goregeist
                    if (pkm.AltForm != 0) // Average
                        AddLine(Severity.Invalid, V6, CheckIdentifier.Form);
                    break;
                case 586: // Sawsbuck
                    if (pkm.AltForm != 0)
                        AddLine(Severity.Invalid, V65, CheckIdentifier.Form);
                    break;
            }
        }
        private void VerifyEncounterType()
        {
            if (pkm.Format >= 7)
                return;

            if (!Encounter.Valid)
                return;

            EncounterType type = EncounterType.None;
            // Encounter type data is only stored for gen 4 encounters
            // Gen 6 -> 7 transfer deletes encounter type data
            // All eggs have encounter type none, even if they are from static encounters
            if (pkm.Gen4 && !pkm.WasEgg)
            {
                if (EncounterMatch is EncounterSlot w)
                    // If there is more than one slot, the get wild encounter have filter for the pkm type encounter like safari/sports ball
                    type = w.TypeEncounter;
                if (EncounterMatch is EncounterStaticTyped s)
                    type = s.TypeEncounter;
            }

            if (!type.Contains(pkm.EncounterType))
                AddLine(Severity.Invalid, V381, CheckIdentifier.Encounter);
            else
                AddLine(Severity.Valid, V380, CheckIdentifier.Encounter);
        }

        private void VerifyTransferLegalityG3()
        {
            if (pkm.Format == 4 && pkm.Met_Location != 0x37) // Pal Park
                AddLine(Severity.Invalid, V60, CheckIdentifier.Encounter);
            if (pkm.Format != 4 && pkm.Met_Location != 30001)
                AddLine(Severity.Invalid, V61, CheckIdentifier.Encounter);
        }
        private void VerifyTransferLegalityG4()
        {
            // Transfer Legality
            int loc = pkm.Met_Location;
            if (loc != 30001) // PokéTransfer
            {
                // Crown
                switch (pkm.Species)
                {
                    case 251: // Celebi
                        if (loc != 30010 && loc != 30011) // unused || used
                            AddLine(Severity.Invalid, V351, CheckIdentifier.Encounter);
                        break;
                    case 243: // Raikou
                    case 244: // Entei
                    case 245: // Suicune
                        if (loc != 30012 && loc != 30013) // unused || used
                            AddLine(Severity.Invalid, V351, CheckIdentifier.Encounter);
                        break;
                    default:
                        AddLine(Severity.Invalid, V61, CheckIdentifier.Encounter);
                        break;
                }
            }
        }
        private static IEnumerable<CheckResult> VerifyVCEncounter(PKM pkm, int baseSpecies, GBEncounterData encounter, EncounterStatic transfer)
        {
            // Check existing EncounterMatch
            if (encounter == null || transfer == null)
                yield break; // Avoid duplicate invaild message

            if (encounter.Encounter is EncounterStatic v && (GameVersion.GBCartEraOnly.Contains(v.Version) || v.Version == GameVersion.VCEvents))
            {
                bool exceptions = false;
                exceptions |= v.Version == GameVersion.VCEvents && baseSpecies == 151 && pkm.TID == 22796;
                if (!exceptions)
                    yield return new CheckResult(Severity.Invalid, V79, CheckIdentifier.Encounter);
            }

            if (pkm.Met_Location != transfer.Location)
                yield return new CheckResult(Severity.Invalid, V81, CheckIdentifier.Encounter);
            if (pkm.Egg_Location != transfer.EggLocation)
                yield return new CheckResult(Severity.Invalid, V59, CheckIdentifier.Encounter);

            if (baseSpecies == 150 && pkm.Moves.Contains(6)) // pay day
                yield return new CheckResult(Severity.Invalid, V82, CheckIdentifier.Encounter);
        }
        #endregion
        private void VerifyLevel()
        {
            if (EncounterMatch is MysteryGift gift)
            {
                if (gift.Level != pkm.Met_Level && pkm.HasOriginalMetLocation)
                {
                    switch (gift)
                    {
                        case WC3 wc3 when wc3.Met_Level == pkm.Met_Level:
                            break;
                        case WC7 wc7 when wc7.MetLevel == pkm.Met_Level:
                            break;
                        default:
                            AddLine(new CheckResult(Severity.Invalid, V83, CheckIdentifier.Level));
                            return;
                    }
                }
                if (gift.Level > pkm.CurrentLevel)
                {
                    AddLine(new CheckResult(Severity.Invalid, V84, CheckIdentifier.Level));
                    return;
                }
            }

            if (pkm.IsEgg)
            {
                int elvl = Legal.GetEggHatchLevel(pkm);
                if (elvl != pkm.CurrentLevel)
                    AddLine(Severity.Invalid, string.Format(V52, elvl), CheckIdentifier.Level);
                return;
            }

            int lvl = pkm.CurrentLevel;
            if (lvl < pkm.Met_Level)
                AddLine(Severity.Invalid, V85, CheckIdentifier.Level);
            else if (!EncounterMatch.IsWithinRange(pkm) && lvl != 100 && pkm.EXP == PKX.GetEXP(lvl, pkm.Species))
                AddLine(Severity.Fishy, V87, CheckIdentifier.Level);
            else
                AddLine(Severity.Valid, V88, CheckIdentifier.Level);

            // There is no way to prevent a gen1 trade evolution as held items (everstone) did not exist.
            // Machoke, Graveler, Haunter and Kadabra captured in the second phase evolution, excluding in-game trades, are already checked
            if (pkm.Format <= 2 && Type != typeof (EncounterTrade) && EncounterMatch.Species == pkm.Species && Legal.Trade_Evolution1.Contains(EncounterMatch.Species))
                VerifyG1TradeEvo();
        }
        private void VerifyG1TradeEvo()
        {
            var mustevolve = pkm.TradebackStatus == TradebackType.WasTradeback || (pkm.Format == 1 && Legal.IsOutsider(pkm)) || Legal.IsTradedKadabraG1(pkm);
            if (!mustevolve)
                return;
            // Pokemon have been traded but it is not evolved, trade evos are sequential dex numbers
            var unevolved = SpeciesStrings[pkm.Species];
            var evolved = SpeciesStrings[pkm.Species + 1];
            AddLine(Severity.Invalid, string.Format(V405, unevolved, evolved), CheckIdentifier.Level);
        }
        #region VerifyMedals
        private void VerifyMedals()
        {
            if (pkm.Format < 6)
                return;
            
            VerifyMedalsRegular();
            VerifyMedalsEvent();
        }
        private void VerifyMedalsRegular()
        {
            uint data = BitConverter.ToUInt32(pkm.Data, 0x2C);
            if ((data & 3) != 0) // 2 unused flags
                AddLine(Severity.Invalid, V98, CheckIdentifier.Training);
            int TrainCount = pkm.SuperTrainingMedalCount();

            if (pkm.IsEgg && TrainCount > 0)
                AddLine(Severity.Invalid, V89, CheckIdentifier.Training);
            else if (TrainCount > 0 && Info.Generation > 6)
                AddLine(Severity.Invalid, V90, CheckIdentifier.Training);
            else
            {
                if (pkm.Format >= 7)
                {
                    if (pkm.SecretSuperTrainingUnlocked)
                        AddLine(Severity.Invalid, V91, CheckIdentifier.Training);
                    if (pkm.SecretSuperTrainingComplete)
                        AddLine(Severity.Invalid, V92, CheckIdentifier.Training);
                }
                else
                {
                    if (TrainCount == 30 ^ pkm.SecretSuperTrainingComplete)
                        AddLine(Severity.Invalid, V93, CheckIdentifier.Training);
                }
            }
        }

        private void VerifyMedalsEvent()
        {
            byte data = pkm.Data[0x3A];
            if ((data & 0xC0) != 0) // 2 unused flags highest bits
                AddLine(Severity.Invalid, V98, CheckIdentifier.Training);

            int TrainCount = 0;
            for (int i = 0; i < 6; i++)
            {
                if ((data & 1) != 0)
                    TrainCount++;
                data >>= 1;
            }
            if (pkm.IsEgg && TrainCount > 0)
            { AddLine(Severity.Invalid, V89, CheckIdentifier.Training); }
            else if (TrainCount > 0 && Info.Generation > 6)
            { AddLine(Severity.Invalid, V90, CheckIdentifier.Training); }
            else if (TrainCount > 0)
            { AddLine(Severity.Fishy, V94, CheckIdentifier.Training); }
        }
        #endregion
        private void VerifyRibbons()
        {
            if (!Encounter.Valid)
                return;

            // Check Unobtainable Ribbons
            var encounterContent = (EncounterMatch as MysteryGift)?.Content ?? EncounterMatch;
            if (pkm.IsEgg)
            {
                if (RibbonVerifier.GetIncorrectRibbonsEgg(pkm, encounterContent))
                    AddLine(Severity.Invalid, V603, CheckIdentifier.Ribbon);
                return;
            }

            List<string> result = RibbonVerifier.GetIncorrectRibbons(pkm, encounterContent, Info.Generation);
            if (result.Any())
                AddLine(Severity.Invalid, string.Join(Environment.NewLine, result.Where(s => !string.IsNullOrEmpty(s))), CheckIdentifier.Ribbon);
            else
                AddLine(Severity.Valid, V602, CheckIdentifier.Ribbon);
        }

        private void VerifyCXD()
        {
            if (EncounterMatch is EncounterStatic)
                VerifyCXDStarterCorrelation(Info.PIDIV);
            else if (pkm.WasEgg) // can't obtain eggs in CXD
                AddLine(Severity.Invalid, V80, CheckIdentifier.Encounter); // invalid encounter

            if (pkm.OT_Gender == 1)
                AddLine(Severity.Invalid, V407, CheckIdentifier.Trainer);
        }
        private void VerifyCXDStarterCorrelation(PIDIV pidiv)
        {
            if (pidiv.Type != PIDType.CXD)
                return;

            var spec = EncounterMatch.Species;
            int rev; // pidiv reversed 2x yields SID, 3x yields TID. shift by 7 if another PKM is generated prior
            switch (spec)
            {
                // XD
                case 133: // Eevee
                    rev = 2;
                    break;
                
                // Colosseum
                case 197: // Umbreon (generated before Espeon)
                    rev = 2;
                    break;
                case 196: // Espeon (generated after Umbreon)
                    rev = 2+7;
                    break;
                default:
                    return;
            }
            var seed = pidiv.OriginSeed;
            var SIDf = pidiv.RNG.Reverse(seed, rev);
            var TIDf = pidiv.RNG.Prev(SIDf);
            if (SIDf >> 16 != pkm.SID || TIDf >> 16 != pkm.TID)
                AddLine(Severity.Invalid, V400 + $" {TIDf>>16}/{SIDf>>16}", CheckIdentifier.PID);
        }

        private void VerifyAbility()
        {
            int[] abilities = pkm.PersonalInfo.Abilities;
            if (abilities[1] == 0)
                abilities[1] = abilities[0];
            int abilval = Array.IndexOf(abilities, pkm.Ability);
            if (abilval < 0)
            {
                AddLine(Severity.Invalid, V107, CheckIdentifier.Ability);
                return;
            }

            bool? AbilityUnchanged = true;
            // 3 states flag: true for unchanged, false for changed, null for uncertain/allowing PID mismatch
            // if true, check encounter ability
            // if true or false, check PID/AbilityNumber
            if (3 <= pkm.Format && pkm.Format <= 5 && abilities[0] != abilities[1]) // 3-5 and have 2 distinct ability now
                AbilityUnchanged = VerifyAbilityPreCapsule(abilities, abilval);

            if (EncounterMatch != null)
            {
                // Check Ability Mismatches
                int? EncounterAbility = (EncounterMatch as EncounterStatic)?.Ability ??
                                        (EncounterMatch as EncounterTrade)?.Ability ??
                                        (EncounterMatch as EncounterLink)?.Ability;

                if (EncounterAbility != null && VerifySetAbility(EncounterAbility, AbilityUnchanged, abilities, abilval))
                    return; // result added via VerifySetAbility

                switch (Info.Generation)
                {
                    case 5: VerifyAbility5(abilities); break;
                    case 6: VerifyAbility6(abilities); break;
                    case 7: VerifyAbility7(abilities); break;
                }
            }

            if (3 <= Info.Generation && Info.Generation <= 4 && pkm.AbilityNumber == 4)
                AddLine(Severity.Invalid, V112, CheckIdentifier.Ability);
            else if (AbilityUnchanged != null && abilities[pkm.AbilityNumber >> 1] != pkm.Ability)
                AddLine(Severity.Invalid, pkm.Format < 6 ? V113 : V114, CheckIdentifier.Ability);
            else
                AddLine(Severity.Valid, V115, CheckIdentifier.Ability);
        }
        private bool VerifySetAbility(int? EncounterAbility, bool? AbilityUnchanged, int[] abilities, int abilval)
        {
            if (pkm.AbilityNumber == 4 && EncounterAbility != 4)
            {
                AddLine(Severity.Invalid, V108, CheckIdentifier.Ability);
                return true;
            }

            if (!(AbilityUnchanged ?? false) || EncounterAbility == 0 || pkm.AbilityNumber == EncounterAbility)
                return false;

            if (EncounterMatch is EncounterTrade z && EncounterAbility == 1 << abilval && z.Species == pkm.Species) // Edge case (Static PID?)
                AddLine(Severity.Valid, V115, CheckIdentifier.Ability);
            else if (pkm.Format >= 6 && abilities[0] != abilities[1] && pkm.AbilityNumber < 4) // Ability Capsule
                AddLine(Severity.Valid, V109, CheckIdentifier.Ability);
            else
                AddLine(Severity.Invalid, V223, CheckIdentifier.Ability);
            return true;
        }
        private bool? VerifyAbilityPreCapsule(int[] abilities, int abilval)
        {
            // CXD pokemon could have any ability without maching PID
            if (pkm.Version == (int)GameVersion.CXD && pkm.Format == 3)
                return null;

            // gen3 native or gen4/5 origin
            if (pkm.Format == 3 || !pkm.InhabitedGeneration(3))
                return true;

            // Evovled in gen4/5
            if (pkm.Species > Legal.MaxSpeciesID_3)
                return false;

            // gen3Species will be zero for pokemon with illegal gen 3 encounters, like Infernape with gen 3 "origin"
            var gen3Species = Info.EvoChainsAllGens[3].FirstOrDefault()?.Species ?? 0;
            if (gen3Species == 0)
                return true;

            // Fall through when gen3 pkm transferred to gen4/5
            return VerifyAbilityGen3Transfer(abilities, abilval, gen3Species);
        }
        private bool? VerifyAbilityGen3Transfer(int[] abilities, int abilval, int Species_g3)
        {
            var abilities_g3 = PersonalTable.E[Species_g3].Abilities.Where(a => a != 0).Distinct().ToArray();
            if (abilities_g3.Length == 2) // Excluding Colosseum/XD, a gen3 pkm must match PID if it has 2 unique abilities
                return pkm.Version != (int)GameVersion.CXD;

            int Species_g4 = Info.EvoChainsAllGens[4].FirstOrDefault()?.Species ?? 0;
            int Species_g5 = pkm.Format == 5 ? Info.EvoChainsAllGens[5].FirstOrDefault()?.Species ?? 0 : 0;
            if (Math.Max(Species_g5, Species_g4) > Species_g3) // it has evolved in either gen 4 or gen 5; the ability must match PID
                return false;

            var Evolutions_g45 = Math.Max(Info.EvoChainsAllGens[4].Length, pkm.Format == 5 ? Info.EvoChainsAllGens[5].Length : 0);
            if (Evolutions_g45 > 1)
            {
                // Evolutions_g45 > 1 and Species_g45 = Species_g3 with means both options, evolve in gen 4-5 or not evolve, are possible
                if (pkm.Ability == abilities_g3[0])
                    // It could evolve in gen 4-5 an have generation 3 only ability
                    // that means it have not actually evolved in gen 4-5, ability do not need to match PID
                    return null;
                if (pkm.Ability == abilities[1])
                    // It could evolve in gen4-5 an have generation 4 second ability
                    // that means it have actually evolved in gen 4-5, ability must match PID
                    return false;
            }
            // Evolutions_g45 == 1 means it have not evolved in gen 4-5 games, 
            // ability do not need to match PID, but only generation 3 ability is allowed
            if (pkm.Ability != abilities_g3[0]) 
                // Not evolved in gen4-5 but do not have generation 3 only ability
                AddLine(Severity.Invalid, V373, CheckIdentifier.Ability);
            return null;
        }
        private void VerifyAbility5(int[] abilities)
        {
            switch (EncounterMatch)
            {
                case PGF g:
                    VerifyAbilityMG456(abilities, g.AbilityType);
                    break;

                case EncounterSlot w:
                    // Hidden Abilities for Wild Encounters are only available at a Hidden Grotto
                    bool grotto = w.Type == SlotType.HiddenGrotto;
                    if (pkm.AbilityNumber == 4 ^ grotto)
                        AddLine(Severity.Invalid, grotto ? V217 : V108, CheckIdentifier.Ability);
                    break;

                case EncounterEgg e when pkm.AbilityNumber == 4:
                    // Hidden Abilities for some are unbreedable (male only distribution)
                    if (Legal.MixedGenderBreeding.Contains(e.Species))
                        break; // from female
                    if ((pkm.PersonalInfo.Gender & 0xFF) == 0 || Legal.Ban_BreedHidden.Contains(e.Species))
                        AddLine(Severity.Invalid, V112, CheckIdentifier.Ability);
                    break;
            }
        }
        private void VerifyAbility6(int[] abilities)
        {
            if (EncounterMatch is EncounterSlot slot && pkm.AbilityNumber == 4)
            {
                bool valid = slot.Permissions.DexNav || slot.Type == SlotType.FriendSafari || slot.Type == SlotType.Horde;
                if (!valid)
                    AddLine(Severity.Invalid, V300, CheckIdentifier.Ability);
            }
            else if (EncounterMatch is WC6 g)
                VerifyAbilityMG456(abilities, g.AbilityType);
            else if (Legal.Ban_NoHidden6.Contains(pkm.SpecForm) && pkm.AbilityNumber == 4)
                AddLine(Severity.Invalid, V112, CheckIdentifier.Ability);
        }
        private void VerifyAbility7(int[] abilities)
        {
            if (EncounterMatch is EncounterSlot slot && pkm.AbilityNumber == 4)
            {
                bool valid = slot.Type == SlotType.SOS;
                if (!valid)
                    AddLine(Severity.Invalid, V111, CheckIdentifier.Ability);
            }
            else if (EncounterMatch is WC7 g)
                VerifyAbilityMG456(abilities, g.AbilityType);
            else if (Legal.Ban_NoHidden7.Contains(pkm.SpecForm) && pkm.AbilityNumber == 4)
                AddLine(Severity.Invalid, V112, CheckIdentifier.Ability);
        }
        private void VerifyAbilityMG456(int[] abilities, int cardtype)
        {
            int abilNumber = pkm.AbilityNumber;
            if (cardtype < 3 && abilNumber != 1 << cardtype) // set number
            {
                // Ability can be flipped 0/1 if Ability Capsule is available, is not Hidden Ability, and Abilities are different.
                if (pkm.Format >= 6 && cardtype < 2 && abilNumber < 3 && abilities[0] != abilities[1])
                    AddLine(Severity.Valid, V109, CheckIdentifier.Ability);
                else
                    AddLine(Severity.Invalid, V110, CheckIdentifier.Ability);
            }
            else if (cardtype == 3 && abilNumber == 4) // 1/2 only
                AddLine(Severity.Invalid, V110, CheckIdentifier.Ability);
        }
        #region VerifyBall
        private void VerifyBallEquals(params int[] balls)
        {
            int ball = pkm.Ball;
            if (balls.Any(b => b == ball))
                AddLine(Severity.Valid, V119, CheckIdentifier.Ball);
            else
                AddLine(Severity.Invalid, V118, CheckIdentifier.Ball);
        }
        private void VerifyBall()
        {
            if (pkm.Format < 3)
                return; // no ball info saved

            if (!Encounter.Valid)
                return;

            if (EncounterMatch is MysteryGift g)
            {
                if (pkm.Species == 490 && g.Ball == 0)
                    // there is no ball data in Manaphy Mystery Gift
                    VerifyBallEquals(4); // Pokeball
                else
                    VerifyBallEquals(g.Ball);
                return;
            }
            if (EncounterMatch is EncounterLink l)
            {
                VerifyBallEquals(l.Ball);
                return;
            }
            if (EncounterMatch is EncounterTrade t)
            {
                VerifyBallEquals(t.Ball); // Pokeball
                return;
            }

            if (pkm.Species == 292 && Info.Generation > 3) // Shedinja. For gen3, copy the ball from Nincada
            {
                VerifyBallEquals(4); // Pokeball Only
                return;
            }

            if (pkm.Ball == 0x14 && pkm.Gen7) // Heavy Ball
            {
                var lineage = Legal.GetLineage(pkm);
                if (lineage.Any(e => Legal.AlolanCaptureNoHeavyBall.Contains(e)))
                {
                    AddLine(Severity.Invalid, V116, CheckIdentifier.Ball);
                    return;
                }
            }

            if (EncounterMatch is EncounterStatic s)
            {
                if (s.Gift)
                    VerifyBallEquals(s.Ball);
                else if (pkm.Met_Location == 75 && pkm.Gen5) // DreamWorld
                    VerifyBallEquals(Legal.DreamWorldBalls);
                else
                    VerifyBallEquals(Legal.GetWildBalls(pkm));
                return;
            }
            if (EncounterMatch is EncounterSlot w)
            {
                if (pkm.Met_Location == 30016 && pkm.Gen7) // Poké Pelago
                    VerifyBallEquals(4); // Pokeball
                // For gen3/4 Safari Zones and BCC getValidWildEncounters already filter to not return
                // mixed possible encounters between safari, BCC and other encounters
                // That means is the first encounter is not safari then there is no safari encounter in the array
                else if (3 <= Info.Generation && Info.Generation <= 4 && EncounterGenerator.IsSafariSlot(w.Type))
                    VerifyBallEquals(5); // Safari Ball
                else if (Info.Generation == 4 && w.Type == SlotType.BugContest)
                    VerifyBallEquals(0x18); // Sport Ball
                else
                    VerifyBallEquals(Legal.GetWildBalls(pkm));
                return;
            }

            if (pkm.WasEgg)
            {
                VerifyBallEgg();
                return;
            }

            VerifyBallEquals(4); // Pokeball
        }
        private void VerifyBallEgg()
        {
            if (Info.Generation < 6) // No inheriting Balls
            {
                VerifyBallEquals(4); // Must be Pokéball -- no ball inheritance.
                return;
            }

            if (pkm.Ball == 0x01) // Master Ball
            { AddLine(Severity.Invalid, V117, CheckIdentifier.Ball); return; }
            if (pkm.Ball == 0x10) // Cherish Ball
            { AddLine(Severity.Invalid, V120, CheckIdentifier.Ball); return; }
            if (pkm.Ball == 0x04) // Poké Ball
            { AddLine(Severity.Valid, V119, CheckIdentifier.Ball); return; }

            switch (Info.Generation)
            {
                case 6: // Gen6 Inheritance Rules
                    VerifyBallEggGen6();
                    return;
                case 7: // Gen7 Inheritance Rules
                    VerifyBallEggGen7();
                    return;
            }
        }
        private void VerifyBallEggGen6()
        {
            if (pkm.Gender == 2) // Genderless
            {
                VerifyBallEquals(4); // Must be Pokéball as ball can only pass via mother (not Ditto!)
                return;
            }
            if (Legal.BreedMaleOnly.Contains(pkm.Species))
            {
                VerifyBallEquals(4); // Must be Pokéball as ball can only pass via mother (not Ditto!)
                return;
            }

            int ball = pkm.Ball;

            if (ball >= 26)
            {
                AddLine(Severity.Invalid, V126, CheckIdentifier.Ball);
                return;
            }
            if (ball == 0x05) // Safari Ball
            {
                if (Legal.GetLineage(pkm).All(e => !Legal.Inherit_Safari.Contains(e)))
                    AddLine(Severity.Invalid, V121, CheckIdentifier.Ball);
                else if (pkm.AbilityNumber == 4)
                    AddLine(Severity.Invalid, V122, CheckIdentifier.Ball);
                else
                    AddLine(Severity.Valid, V123, CheckIdentifier.Ball);

                return;
            }
            if (0x10 < ball && ball < 0x18) // Apricorn Ball
            {
                if (Legal.GetLineage(pkm).All(e => !Legal.Inherit_Apricorn6.Contains(e)))
                    AddLine(Severity.Invalid, V121, CheckIdentifier.Ball);
                if (pkm.AbilityNumber == 4)
                    AddLine(Severity.Invalid, V122, CheckIdentifier.Ball);
                else
                    AddLine(Severity.Valid, V123, CheckIdentifier.Ball);

                return;
            }
            if (ball == 0x18) // Sport Ball
            {
                if (Legal.GetLineage(pkm).All(e => !Legal.Inherit_Sport.Contains(e)))
                    AddLine(Severity.Invalid, V121, CheckIdentifier.Ball);
                else if (pkm.AbilityNumber == 4)
                    AddLine(Severity.Invalid, V122, CheckIdentifier.Ball);
                else
                    AddLine(Severity.Valid, V123, CheckIdentifier.Ball);

                return;
            }
            if (ball == 0x19) // Dream Ball
            {
                if (Legal.GetLineage(pkm).Any(e => Legal.Inherit_Dream.Contains(e)))
                    AddLine(Severity.Valid, V123, CheckIdentifier.Ball);
                else
                    AddLine(Severity.Invalid, V121, CheckIdentifier.Ball);

                if (pkm.AbilityNumber == 4 && Legal.Ban_DreamHidden.Contains(pkm.Species))
                    AddLine(Severity.Invalid, V122, CheckIdentifier.Ball);

                return;
            }
            if (0x0D <= ball && ball <= 0x0F)
            {
                if (!Legal.Ban_Gen4Ball_6.Contains(pkm.Species))
                    AddLine(Severity.Valid, V123, CheckIdentifier.Ball);
                else
                    AddLine(Severity.Invalid, V121, CheckIdentifier.Ball);

                return;
            }
            if (0x02 <= ball && ball <= 0x0C) // Don't worry, Ball # 0x05 was already checked.
            {
                if (Legal.Ban_Gen3Ball.Contains(pkm.Species))
                    AddLine(Severity.Invalid, V121, CheckIdentifier.Ball);
                else if (pkm.AbilityNumber == 4 && Legal.Ban_Gen3BallHidden.Contains(pkm.SpecForm))
                    AddLine(Severity.Invalid, V122, CheckIdentifier.Ball);
                else
                    AddLine(Severity.Valid, V123, CheckIdentifier.Ball);

                return;
            }

            if (pkm.Species > 650 && pkm.Species != 700) // Sylveon
            {
                if (Legal.GetWildBalls(pkm).Contains(pkm.Ball))
                    AddLine(Severity.Valid, V123, CheckIdentifier.Ball);
                else
                    AddLine(Severity.Invalid, V121, CheckIdentifier.Ball);
                return;
            }

            AddLine(Severity.Invalid, V125, CheckIdentifier.Ball);
        }
        private void VerifyBallEggGen7()
        {
            var Lineage = Legal.GetLineage(pkm).ToArray();
            if (722 <= pkm.Species && pkm.Species <= 730) // G7 Starters
            {
                VerifyBallEquals(4);
                return;
            }

            int ball = pkm.Ball;
            if (ball == 0x05) // Safari Ball
            {
                if (!Lineage.Any(e => Legal.Inherit_Safari.Contains(e) || Legal.Inherit_SafariMale.Contains(e)))
                    AddLine(Severity.Invalid, V121, CheckIdentifier.Ball);
                else if (pkm.AbilityNumber == 4 && Lineage.Any(e => Legal.Ban_SafariBallHidden_7.Contains(e)))
                    AddLine(Severity.Invalid, V122, CheckIdentifier.Ball);
                else
                    AddLine(Severity.Valid, V123, CheckIdentifier.Ball);

                return;
            }
            if (0x10 < ball && ball < 0x18) // Apricorn Ball
            {
                if (!Lineage.Any(e => Legal.Inherit_Apricorn7.Contains(e)))
                    AddLine(Severity.Invalid, V121, CheckIdentifier.Ball);
                else if (pkm.AbilityNumber == 4 && (Lineage.Contains(029) || Lineage.Contains(032))) // Nido
                    AddLine(Severity.Invalid, V122, CheckIdentifier.Ball);
                else
                    AddLine(Severity.Valid, V123, CheckIdentifier.Ball);

                return;
            }
            if (ball == 0x18) // Sport Ball
            {
                if (!Lineage.Any(e => Legal.Inherit_Sport.Contains(e)))
                    AddLine(Severity.Invalid, V121, CheckIdentifier.Ball);
                else if (pkm.AbilityNumber == 4 && (Lineage.Contains(313) || Lineage.Contains(314))) // Volbeat/Illumise
                    AddLine(Severity.Invalid, V122, CheckIdentifier.Ball);
                else
                    AddLine(Severity.Valid, V123, CheckIdentifier.Ball);

                return;
            }
            if (ball == 0x19) // Dream Ball
            {
                if (Lineage.Any(e => Legal.Inherit_Dream.Contains(e) || Legal.Inherit_DreamMale.Contains(e)))
                    AddLine(Severity.Valid, V123, CheckIdentifier.Ball);
                else
                    AddLine(Severity.Invalid, V121, CheckIdentifier.Ball);

                return;
            }
            if (0x0D <= ball && ball <= 0x0F) // Dusk Heal Quick
            {
                if (!Legal.Ban_Gen4Ball_7.Contains(pkm.Species))
                    AddLine(Severity.Valid, V123, CheckIdentifier.Ball);
                else
                    AddLine(Severity.Invalid, V121, CheckIdentifier.Ball);

                return;
            }
            if (0x02 <= ball && ball <= 0x0C) // Don't worry, Ball # 0x05 was already checked.
            {
                if (!Legal.Ban_Gen3Ball_7.Contains(pkm.Species))
                    AddLine(Severity.Valid, V123, CheckIdentifier.Ball);
                else
                    AddLine(Severity.Invalid, V121, CheckIdentifier.Ball);

                return;
            }

            if (ball == 26)
            {
                if ((pkm.Species > 731 && pkm.Species <= 785) || Lineage.Any(e => Legal.PastGenAlolanNatives.Contains(e) && !Legal.PastGenAlolanNativesUncapturable.Contains(e)))
                {
                    AddLine(Severity.Valid, V123, CheckIdentifier.Ball);
                    return;
                }
                if (Lineage.Any(e => Legal.PastGenAlolanScans.Contains(e)))
                {
                    AddLine(Severity.Valid, V123, CheckIdentifier.Ball);
                    return;
                }
                // next statement catches all new alolans
            }

            if (pkm.Species > 721)
            {
                VerifyBallEquals(Legal.GetWildBalls(pkm));
                return;
            }

            if (ball >= 27)
            {
                AddLine(Severity.Invalid, V126, CheckIdentifier.Ball);
                return;
            }
            AddLine(Severity.Invalid, V125, CheckIdentifier.Ball);
        }
        #endregion
        private CheckResult VerifyHistory()
        {
            if (!Encounter.Valid)
                return new CheckResult(Severity.Valid, V127, CheckIdentifier.History);

            if (Info.Generation < 6)
            {
                if (pkm.Format < 6)
                    return new CheckResult(Severity.Valid, V128, CheckIdentifier.History);

                if (pkm.OT_Affection > 0)
                    return new CheckResult(Severity.Invalid, V129, CheckIdentifier.History);
                if (pkm.OT_Memory > 0 || pkm.OT_Feeling > 0 || pkm.OT_Intensity > 0 || pkm.OT_TextVar > 0)
                    return new CheckResult(Severity.Invalid, V130, CheckIdentifier.History);
            }
            
            if (pkm.Format >= 6 && Info.Generation != pkm.Format && pkm.CurrentHandler != 1)
                return new CheckResult(Severity.Invalid, V124, CheckIdentifier.History);

            if (pkm.HT_Gender > 1)
                return new CheckResult(Severity.Invalid, string.Format(V131, pkm.HT_Gender), CheckIdentifier.History);
            
            if (EncounterMatch is WC6 wc6 && wc6.OT.Length > 0)
            {
                if (pkm.OT_Friendship != PersonalTable.AO[EncounterMatch.Species].BaseFriendship)
                    return new CheckResult(Severity.Invalid, V132, CheckIdentifier.History);
                if (pkm.OT_Affection != 0)
                    return new CheckResult(Severity.Invalid, V133, CheckIdentifier.History);
                if (pkm.CurrentHandler != 1)
                    return new CheckResult(Severity.Invalid, V134, CheckIdentifier.History);
            }
            else if (EncounterMatch is WC7 wc7 && wc7.OT.Length > 0)
            {
                if (pkm.OT_Friendship != PersonalTable.USUM[EncounterMatch.Species].BaseFriendship)
                    return new CheckResult(Severity.Invalid, V132, CheckIdentifier.History);
                if (pkm.OT_Affection != 0)
                    return new CheckResult(Severity.Invalid, V133, CheckIdentifier.History);
                if (pkm.CurrentHandler != 1)
                    return new CheckResult(Severity.Invalid, V134, CheckIdentifier.History);
            }
            else if (EncounterMatch is MysteryGift mg && mg.Format < 6 && pkm.Format >= 6)
            {
                if (pkm.OT_Affection != 0)
                    return new CheckResult(Severity.Invalid, V133, CheckIdentifier.History);
                if (pkm.CurrentHandler != 1)
                    return new CheckResult(Severity.Invalid, V134, CheckIdentifier.History);
            }
            
            // Geolocations
            var geo = new[]
            {
                pkm.Geo1_Country, pkm.Geo2_Country, pkm.Geo3_Country, pkm.Geo4_Country, pkm.Geo5_Country,
                pkm.Geo1_Region, pkm.Geo2_Region, pkm.Geo3_Region, pkm.Geo4_Region, pkm.Geo5_Region,
            };

            // Check sequential order (no zero gaps)
            bool geoEnd = false;
            for (int i = 0; i < 5; i++)
            {
                if (geoEnd && geo[i] != 0)
                    return new CheckResult(Severity.Invalid, V135, CheckIdentifier.History);

                if (geo[i] != 0)
                    continue;
                if (geo[i + 5] != 0)
                    return new CheckResult(Severity.Invalid, V136, CheckIdentifier.History);
                geoEnd = true;
            }
            if (pkm.Format >= 7)
                return VerifyHistory7(geo);

            // Determine if we should check for Handling Trainer Memories
            // A Pokémon is untraded if...
            bool untraded = pkm.HT_Name.Length == 0 || pkm.Geo1_Country == 0;
            if (Type == typeof(MysteryGift))
            {
                untraded |= !pkm.WasEventEgg;
                untraded &= pkm.WasEgg;
            }

            if (pkm.WasLink && (EncounterMatch as EncounterLink)?.OT == false)
                untraded = false;
            else if (Info.Generation < 6)
                untraded = false;

            if (untraded) // Is not Traded
            {
                if (pkm.HT_Name.Length != 0)
                    return new CheckResult(Severity.Invalid, V146, CheckIdentifier.History);
                if (pkm.Geo1_Country != 0)
                    return new CheckResult(Severity.Invalid, V147, CheckIdentifier.History);
                if (pkm.HT_Memory != 0)
                    return new CheckResult(Severity.Invalid, V148, CheckIdentifier.History);
                if (pkm.CurrentHandler != 0) // Badly edited; PKHeX doesn't trip this.
                    return new CheckResult(Severity.Invalid, V139, CheckIdentifier.History);
                if (pkm.HT_Friendship != 0)
                    return new CheckResult(Severity.Invalid, V140, CheckIdentifier.History);
                if (pkm.HT_Affection != 0)
                    return new CheckResult(Severity.Invalid, V141, CheckIdentifier.History);
                if (pkm.XY && pkm.CNTs.Any(stat => stat > 0))
                    return new CheckResult(Severity.Invalid, V138, CheckIdentifier.History);

                if (VerifyHistoryUntradedHandler(pkm, out CheckResult chk1))
                    return chk1;
                if (EncounterMatch.Species != pkm.Species && VerifyHistoryUntradedEvolution(pkm, out CheckResult chk2))
                    return chk2;
            }
            else // Is Traded
            {
                if (pkm.Format == 6 && pkm.HT_Memory == 0 && !pkm.IsEgg)
                    return new CheckResult(Severity.Invalid, V150, CheckIdentifier.History);
            }

            // Memory ChecksResult
            if (pkm.IsEgg)
            {
                if (pkm.HT_Memory != 0)
                    return new CheckResult(Severity.Invalid, V149, CheckIdentifier.History);
                if (pkm.OT_Memory != 0)
                    return new CheckResult(Severity.Invalid, V151, CheckIdentifier.History);
            }
            else if (!(EncounterMatch is WC6))
            {
                if (pkm.OT_Memory == 0 ^ !pkm.Gen6)
                    return new CheckResult(Severity.Invalid, V152, CheckIdentifier.History);
                if (Info.Generation < 6 && pkm.OT_Affection != 0)
                    return new CheckResult(Severity.Invalid, V129, CheckIdentifier.History);
            }
            // Unimplemented: Ingame Trade Memories

            return new CheckResult(Severity.Valid, V145, CheckIdentifier.History);
        }
        private CheckResult VerifyHistory7(int[] geo)
        {
            if (pkm.VC1)
            {
                var hasGeo = geo.Any(d => d != 0);

                if (!hasGeo)
                    return new CheckResult(Severity.Invalid, V137, CheckIdentifier.History);
            }

            if (Info.Generation >= 7 && pkm.CNTs.Any(stat => stat > 0))
                return new CheckResult(Severity.Invalid, V138, CheckIdentifier.History);

            if (!pkm.WasEvent && pkm.HT_Name.Length == 0) // Is not Traded
            {
                if (VerifyHistoryUntradedHandler(pkm, out CheckResult chk1))
                    return chk1;
                if (EncounterMatch.Species != pkm.Species && VerifyHistoryUntradedEvolution(pkm, out CheckResult chk2))
                    return chk2;
            }

            return new CheckResult(Severity.Valid, V145, CheckIdentifier.History);
        }
        private static bool VerifyHistoryUntradedHandler(PKM pkm, out CheckResult result)
        {
            result = null;
            if (pkm.CurrentHandler != 0) // Badly edited; PKHeX doesn't trip this.
                result = new CheckResult(Severity.Invalid, V139, CheckIdentifier.History);
            else if (pkm.HT_Friendship != 0)
                result = new CheckResult(Severity.Invalid, V140, CheckIdentifier.History);
            else if (pkm.HT_Affection != 0)
                result = new CheckResult(Severity.Invalid, V141, CheckIdentifier.History);
            else
                return false;

            return true;
        }
        private static bool VerifyHistoryUntradedEvolution(PKM pkm, out CheckResult result)
        {
            result = null;
            // Handling Trainer string is empty implying it has not been traded.
            // If it must be trade evolved, flag it.
            if (!Legal.IsTradeEvolved(pkm))
                return false;

            if (pkm.Species == 350) // Milotic
            {
                if (pkm.CNT_Beauty < 170) // Beauty Contest Stat Requirement
                    result = new CheckResult(Severity.Invalid, V143, CheckIdentifier.History);
                else if (pkm.CurrentLevel == 1)
                    result = new CheckResult(Severity.Invalid, V144, CheckIdentifier.History);
                else
                    return false;
            }
            else
                result = new CheckResult(Severity.Invalid, V142, CheckIdentifier.History);

            return true;
        }
        private CheckResult VerifyCommonMemory(int handler)
        {
            int m = 0;
            int t = 0;
            string resultPrefix = "";
            switch (handler)
            {
                case 0:
                    m = pkm.OT_Memory;
                    t = pkm.OT_TextVar;
                    resultPrefix = V205;
                    break;
                case 1:
                    m = pkm.HT_Memory;
                    t = pkm.HT_TextVar;
                    resultPrefix = V206;
                    break;
            }
            int matchingMoveMemory = Array.IndexOf(Legal.MoveSpecificMemories[0], m);
            if (matchingMoveMemory != -1 && pkm.Species != 235 && !Legal.GetCanLearnMachineMove(pkm, Legal.MoveSpecificMemories[1][matchingMoveMemory], 6))
                return new CheckResult(Severity.Invalid, string.Format(V153, resultPrefix), CheckIdentifier.Memory);

            if (m == 6 && !Legal.LocationsWithPKCenter[0].Contains(t))
                return new CheckResult(Severity.Invalid, string.Format(V154, resultPrefix), CheckIdentifier.Memory);

            if (m == 21) // {0} saw {2} carrying {1} on its back. {4} that {3}.
                if (!Legal.GetCanLearnMachineMove(new PK6 {Species = t, EXP = PKX.GetEXP(100, t)}, 19, 6))
                    return new CheckResult(Severity.Invalid, string.Format(V153, resultPrefix), CheckIdentifier.Memory);

            if ((m == 16 || m == 48) && (t == 0 || !Legal.GetCanKnowMove(pkm, t, 6)))
                return new CheckResult(Severity.Invalid, string.Format(V153, resultPrefix), CheckIdentifier.Memory);

            if (m == 49 && (t == 0 || !Legal.GetCanRelearnMove(pkm, t, 6))) // {0} was able to remember {2} at {1}'s instruction. {4} that {3}.
                return new CheckResult(Severity.Invalid, string.Format(V153, resultPrefix), CheckIdentifier.Memory);

            return new CheckResult(Severity.Valid, string.Format(V155, resultPrefix), CheckIdentifier.Memory);
        }

        private void VerifyOTMemoryIs(int[] values)
        {
            if (pkm.OT_Memory != values[0])
                AddLine(Severity.Invalid, string.Format(V197, V205, values[0]), CheckIdentifier.Memory);
            if (pkm.OT_Intensity != values[1])
                AddLine(Severity.Invalid, string.Format(V198, V205, values[1]), CheckIdentifier.Memory);
            if (pkm.OT_TextVar != values[2])
                AddLine(Severity.Invalid, string.Format(V199, V205, values[2]), CheckIdentifier.Memory);
            if (pkm.OT_Feeling != values[3])
                AddLine(Severity.Invalid, string.Format(V200, V205, values[3]), CheckIdentifier.Memory);
        }
        private void VerifyOTMemory()
        {
            if (pkm.Format < 6)
                return;

            if (!History.Valid)
                return;

            if (Info.Generation < 6)
            {
                VerifyOTMemoryIs(new [] {0, 0, 0, 0}); // empty
                return;
            }

            if (Type == typeof(EncounterTrade))
            {
                switch (Info.Generation)
                {
                    case 6:
                        break; // Undocumented, uncommon, and insignificant -- don't bother.
                    case 7:
                        VerifyOTMemoryIs(new[] { 1, 3, 40, 5 });
                        break;
                }
                return;
            }
            if (EncounterMatch is WC6 wc6)
            {
                var g = wc6;
                VerifyOTMemoryIs(new[] {g.OT_Memory, g.OT_Intensity, g.OT_TextVar, g.OT_Feeling});
                return;
            }
            if (EncounterMatch is WC7 wc7)
            {
                var g = wc7;
                VerifyOTMemoryIs(new[] {g.OT_Memory, g.OT_Intensity, g.OT_TextVar, g.OT_Feeling});
                return;
            }
            if (Info.Generation >= 7)
            {
                VerifyOTMemoryIs(new[] {0, 0, 0, 0}); // empty
                return;
            }

            switch (pkm.OT_Memory)
            {
                case 2: // {0} hatched from an Egg and saw {1} for the first time at... {2}. {4} that {3}.
                    if (!pkm.WasEgg && pkm.Egg_Location != 60004)
                        AddLine(Severity.Invalid, string.Format(V160, V205), CheckIdentifier.Memory);
                    break;

                case 4: // {0} became {1}’s friend when it arrived via Link Trade at... {2}. {4} that {3}.
                    AddLine(Severity.Invalid, string.Format(V161, V205), CheckIdentifier.Memory);
                    return;

                case 6: // {0} went to the Pokémon Center in {2} with {1} and had its tired body healed there. {4} that {3}.
                    int matchingOriginGame = Array.IndexOf(Legal.LocationsWithPKCenter[0], pkm.OT_TextVar);
                    if (matchingOriginGame != -1)
                    {
                        int gameID = Legal.LocationsWithPKCenter[1][matchingOriginGame];
                        if (pkm.XY && gameID != 0 || pkm.AO && gameID != 1)
                            AddLine(Severity.Invalid, string.Format(V162, V205), CheckIdentifier.Memory);
                    }
                    AddLine(VerifyCommonMemory(0));
                    return;

                case 14:
                    if (!Legal.GetCanBeCaptured(pkm.OT_TextVar, Info.Generation, (GameVersion)pkm.Version))
                        AddLine(Severity.Invalid, string.Format(V165, V205), CheckIdentifier.Memory);
                    else
                        AddLine(Severity.Valid, string.Format(V164, V205), CheckIdentifier.Memory);
                    return;
            }
            if (pkm.XY && Legal.Memory_NotXY.Contains(pkm.OT_Memory))
                AddLine(Severity.Invalid, string.Format(V163, V205), CheckIdentifier.Memory);
            if (pkm.AO && Legal.Memory_NotAO.Contains(pkm.OT_Memory))
                AddLine(Severity.Invalid, string.Format(V163, V205), CheckIdentifier.Memory);

            AddLine(VerifyCommonMemory(0));
        }
        private void VerifyHTMemory()
        {
            if (pkm.Format < 6)
                return;

            if (!History.Valid)
                return;

            if (pkm.Format >= 7)
            {
                /* 
                *  Bank Transfer adds in the Link Trade Memory.
                *  Trading 7<->7 between games (not Bank) clears this data.
                */
                if (pkm.HT_Memory == 0)
                {
                    if (pkm.HT_TextVar != 0 || pkm.HT_Intensity != 0 || pkm.HT_Feeling != 0)
                        AddLine(Severity.Invalid, V329, CheckIdentifier.Memory);
                    return;
                }

                // Transfer 6->7 & withdraw to same HT => keeps past gen memory
                // Don't require link trade memory for these past gen cases
                int gen = Info.Generation;
                if (3 <= gen && gen < 7 && pkm.CurrentHandler == 1) 
                    return;

                if (pkm.HT_Memory != 4)
                    AddLine(Severity.Invalid, V156, CheckIdentifier.Memory);
                if (pkm.HT_TextVar != 0)
                    AddLine(Severity.Invalid, V157, CheckIdentifier.Memory);
                if (pkm.HT_Intensity != 1)
                    AddLine(Severity.Invalid, V158, CheckIdentifier.Memory);
                if (pkm.HT_Feeling > 10)
                    AddLine(Severity.Invalid, V159, CheckIdentifier.Memory);
                return;
            }

            switch (pkm.HT_Memory)
            {
                case 0:
                    if (string.IsNullOrEmpty(pkm.HT_Name))
                        return;
                    AddLine(Severity.Invalid, V150, CheckIdentifier.Memory); return;
                case 1: // {0} met {1} at... {2}. {1} threw a Poké Ball at it, and they started to travel together. {4} that {3}.
                    AddLine(Severity.Invalid, string.Format(V202, V206), CheckIdentifier.Memory); return;

                case 2: // {0} hatched from an Egg and saw {1} for the first time at... {2}. {4} that {3}.
                    AddLine(Severity.Invalid, string.Format(V160, V206), CheckIdentifier.Memory); return;

                case 14:
                    if (Legal.GetCanBeCaptured(pkm.HT_TextVar, Info.Generation))
                        AddLine(Severity.Valid, string.Format(V164, V206), CheckIdentifier.Memory);
                    else
                        AddLine(Severity.Invalid, string.Format(V165, V206), CheckIdentifier.Memory);
                    return;
            }
            AddLine(VerifyCommonMemory(1));
        }
        private void VerifyConsoleRegion()
        {
            AddLine(VerifyConsoleRegion(pkm));
        }
        private static CheckResult VerifyConsoleRegion(PKM pkm)
        {
            int consoleRegion = pkm.ConsoleRegion;
            if (consoleRegion >= 7)
                return new CheckResult(Severity.Invalid, V301, CheckIdentifier.Geography);
            return IsConsoleRegionCountryValid(consoleRegion, pkm.Country) 
                ? new CheckResult(Severity.Valid, V303, CheckIdentifier.Geography) 
                : new CheckResult(Severity.Invalid, V302, CheckIdentifier.Geography);
        }
        private static bool IsConsoleRegionCountryValid(int consoleRegion, int country)
        {
            switch (consoleRegion)
            {
                case 0: // Japan
                    return country == 1;
                case 1: // Americas
                    return 8 <= country && country <= 52 || new[] {153, 156, 168, 174, 186}.Contains(country);
                case 2: // Europe
                    return 64 <= country && country <= 127 || new[] {169, 184, 185}.Contains(country);
                case 4: // China
                    return country == 144 || country == 160;
                case 5: // Korea
                    return country == 136;
                case 6: // Taiwan
                    return country == 128;
                default:
                    return false;
            }
        }
        private void VerifyForm()
        {
            if (!Encounter.Valid)
                return;

            if (pkm.Format < 4)
                return; // no forms exist

            int count = pkm.PersonalInfo.FormeCount;
            if (count == 1 && pkm.AltForm == 0)
                return; // no forms to check

            if (pkm.AltForm > count && !IsValidOutOfBoundsForme(pkm.Species, pkm.AltForm, Info.Generation))
            {
                AddLine(Severity.Invalid, string.Format(V304, count, pkm.AltForm), CheckIdentifier.Form);
                return;
            }

            if (EncounterMatch is EncounterSlot w && w.Type == SlotType.FriendSafari)
                VerifyFormFriendSafari();

            switch (pkm.Species)
            {
                case 25: // Pikachu
                    if (Info.Generation == 6 && pkm.AltForm != 0 ^ Type == typeof(EncounterStatic))
                    {
                        string msg = Type == typeof(EncounterStatic) ? V305 : V306;
                        AddLine(Severity.Invalid, msg, CheckIdentifier.Form);
                        return;
                    }
                    if (Info.Generation == 7 && pkm.AltForm != 0 ^ Type == typeof(MysteryGift))
                    {
                        if (EncounterMatch is WC7 gift && gift.Form != pkm.AltForm)
                        {
                            AddLine(Severity.Invalid, V307, CheckIdentifier.Form);
                            return;
                        }
                    }
                    break;
                case 487: // Giratina
                    if (pkm.AltForm == 1 ^ pkm.HeldItem == 112) // Origin form only with Griseous Orb
                    {
                        AddLine(Severity.Invalid, V308, CheckIdentifier.Form);
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

                        if (form != pkm.AltForm)
                            AddLine(Severity.Invalid, V308, CheckIdentifier.Form);
                        else if (form != 0)
                            AddLine(Severity.Valid, V309, CheckIdentifier.Form);
                    }
                    break;
                case 647: // Keldeo
                    {
                        if (pkm.Format == 5) // can mismatch in gen5 via BW tutor
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
                            AddLine(Severity.Invalid, V308, CheckIdentifier.Form);
                        else
                            AddLine(Severity.Valid, V309, CheckIdentifier.Form);
                    }
                    break;
                case 658: // Greninja
                    if (pkm.AltForm > 1) // Ash Battle Bond active
                    {
                        AddLine(Severity.Invalid, V310, CheckIdentifier.Form);
                        return;
                    }
                    if (pkm.AltForm != 0 && Type != typeof(MysteryGift)) // Formes are not breedable, MysteryGift already checked
                    {
                        AddLine(Severity.Invalid, string.Format(V304, 0, pkm.AltForm), CheckIdentifier.Form);
                        return;
                    }
                    break;
                case 664: // Scatterbug
                case 665: // Spewpa
                    if (pkm.AltForm > 17) // Fancy & Pokéball
                    {
                        AddLine(Severity.Invalid, V311, CheckIdentifier.Form);
                        return;
                    }
                    if (!Legal.CheckVivillonPattern(pkm.AltForm, pkm.Country, pkm.Region))
                        AddLine(Severity.Fishy, V312, CheckIdentifier.Form);
                    break;
                case 666: // Vivillon
                    if (pkm.AltForm > 17) // Fancy & Pokéball
                    {
                        if (Type != typeof(MysteryGift))
                            AddLine(Severity.Invalid, V312, CheckIdentifier.Form);
                        else
                            AddLine(Severity.Valid, V313, CheckIdentifier.Form);

                        return;
                    }
                    if (!Legal.CheckVivillonPattern(pkm.AltForm, pkm.Country, pkm.Region))
                        AddLine(Severity.Fishy, V312, CheckIdentifier.Form);
                    break;
                case 670: // Floette
                    if (pkm.AltForm == 5) // Eternal Flower -- Never Released
                    {
                        if (Type != typeof(MysteryGift))
                            AddLine(Severity.Invalid, V314, CheckIdentifier.Form);
                        else
                            AddLine(Severity.Valid, V315, CheckIdentifier.Form);

                        return;
                    }
                    break;
                case 718: // Zygarde
                    if (pkm.AltForm >= 4)
                    {
                        AddLine(Severity.Invalid, V310, CheckIdentifier.Form);
                        return;
                    }
                    break;
                case 773: // Silvally
                    {
                        int item = pkm.HeldItem;
                        int form = 0;
                        if ((904 <= item && item <= 920) || item == 644)
                            form = item - 903;
                        if (form != pkm.AltForm)
                            AddLine(Severity.Invalid, V308, CheckIdentifier.Form);
                        else if (form != 0)
                            AddLine(Severity.Valid, V309, CheckIdentifier.Form);
                        break;
                    }
                case 774: // Minior
                    if (pkm.AltForm < 7)
                    {
                        AddLine(Severity.Invalid, V310, CheckIdentifier.Form);
                        return;
                    }
                    break;

                // Party Only Forms
                case 492: // Shaymin
                case 676: // Furfrou
                case 720: // Hoopa
                    if (pkm.AltForm != 0 && pkm.Box > -1 && pkm.Format <= 6) // has form but stored in box
                    {
                        AddLine(Severity.Invalid, V316, CheckIdentifier.Form);
                        return;
                    }
                    break;
            }

            if (pkm.Format >= 7 && Info.Generation < 7 && pkm.AltForm != 0)
            {
                if (pkm.Species == 25 || Legal.AlolanOriginForms.Contains(pkm.Species))
                { AddLine(Severity.Invalid, V317, CheckIdentifier.Form); return; }
            }
            if (pkm.AltForm > 0 && new[] {Legal.BattleForms, Legal.BattleMegas, Legal.BattlePrimals}.Any(arr => arr.Contains(pkm.Species)))
            { AddLine(Severity.Invalid, V310, CheckIdentifier.Form); return; }

            AddLine(Severity.Valid, V318, CheckIdentifier.Form);
        }
        private static bool IsValidOutOfBoundsForme(int species, int form, int generation)
        {
            switch (species)
            {
                case 201: // Unown
                    return form < (generation == 2 ? 26 : 28); // A-Z : A-Z?!
                case 414: // Wormadam base form is kept
                    return form < 3;
                case 664: case 665: // Vivillon Pre-evolutions
                    return form < 18;
                default:
                    return false;
            }
        }
        private void VerifyMiscG1()
        {
            if (pkm.IsEgg)
            {
                VerifyMiscEggCommon();
                if (pkm.PKRS_Cured || pkm.PKRS_Infected)
                    AddLine(Severity.Invalid, V368, CheckIdentifier.Egg);
            }

            if (pkm.Format > 2)
            {
                // Female Shinies for a 12.5%-F species are not possible with the 'correct' correlation
                // Original Transporter code generated a random nature (VC1 only), so we can ignore in this case
                bool checkShiny = pkm.VC2 || pkm.TradebackStatus == TradebackType.WasTradeback && pkm.VC1;
                if (checkShiny && pkm.Gender == 1 && pkm.PersonalInfo.Gender == 31 && pkm.IsShiny)
                    AddLine(Severity.Invalid, V209, CheckIdentifier.PID);
            }

            if (!(pkm is PK1 pk1))
                return;

            VerifyMiscG1Types(pk1);
            VerifyMiscG1CatchRate(pk1);
        }
        private void VerifyMiscG1Types(PK1 pk1)
        {
            var Type_A = pk1.Type_A;
            var Type_B = pk1.Type_B;
            if (pkm.Species == 137)
            {
                // Porygon can have any type combination of any generation 1 species because of the move Conversion,
                // that change Porygon type to match the oponent types
                var Type_A_Match = Legal.Types_Gen1.Any(t => t == Type_A);
                var Type_B_Match = Legal.Types_Gen1.Any(t => t == Type_B);
                if (!Type_A_Match)
                    AddLine(Severity.Invalid, V386, CheckIdentifier.Misc);
                if (!Type_B_Match)
                    AddLine(Severity.Invalid, V387, CheckIdentifier.Misc);
                if (Type_A_Match && Type_B_Match)
                {
                    var TypesAB_Match = PersonalTable.RB.IsValidTypeCombination(Type_A, Type_B);
                    if (TypesAB_Match)
                        AddLine(Severity.Valid, V391, CheckIdentifier.Misc);
                    else
                        AddLine(Severity.Invalid, V388, CheckIdentifier.Misc);
                }
            }
            else // Types must match species types
            {
                var Type_A_Match = Type_A == PersonalTable.RB[pkm.Species].Types[0];
                var Type_B_Match = Type_B == PersonalTable.RB[pkm.Species].Types[1];

                AddLine(Type_A_Match ? Severity.Valid : Severity.Invalid, Type_A_Match ? V392 : V389, CheckIdentifier.Misc);
                AddLine(Type_B_Match ? Severity.Valid : Severity.Invalid, Type_B_Match ? V393 : V390, CheckIdentifier.Misc);
            }
        }
        private void VerifyMiscG1CatchRate(PK1 pk1)
        {
            var catch_rate = pk1.Catch_Rate;
            switch (pkm.TradebackStatus)
            {
                case TradebackType.Any:
                case TradebackType.WasTradeback:
                    if (catch_rate == 0 || Legal.HeldItems_GSC.Any(h => h == catch_rate))
                        AddLine(Severity.Valid, V394, CheckIdentifier.Misc);
                    else if (pkm.TradebackStatus == TradebackType.WasTradeback)
                        AddLine(Severity.Invalid, V395, CheckIdentifier.Misc);
                    else
                        goto case TradebackType.Gen1_NotTradeback;
                    break;
                case TradebackType.Gen1_NotTradeback:
                    if ((EncounterMatch as EncounterStatic)?.Version == GameVersion.Stadium || EncounterMatch is EncounterTradeCatchRate)
                        // Encounters detected by the catch rate, cant be invalid if match this encounters
                        AddLine(Severity.Valid, V398, CheckIdentifier.Misc);
                    if (pkm.Species == 149 && catch_rate == PersonalTable.Y[149].CatchRate || Legal.Species_NotAvailable_CatchRate.Contains(pkm.Species) && catch_rate == PersonalTable.RB[pkm.Species].CatchRate)
                        AddLine(Severity.Invalid, V396, CheckIdentifier.Misc);
                    else if (!Info.EvoChainsAllGens[1].Any(e => catch_rate == PersonalTable.RB[e.Species].CatchRate || catch_rate == PersonalTable.Y[e.Species].CatchRate))
                        AddLine(Severity.Invalid, pkm.Gen1_NotTradeback ? V397 : V399, CheckIdentifier.Misc);
                    else
                        AddLine(Severity.Valid, V398, CheckIdentifier.Misc);
                    break;
            }
        }
        private void VerifyMisc()
        {
            if (pkm.Format == 7 && ((PK7)pkm).PelagoEventStatus != 0)
            {
                // TODO: Figure out what PelagoEventStati are legal.
            }

            if (pkm.IsEgg)
            {
                VerifyMiscEggCommon();

                if (pkm.CNTs.Any(stat => stat > 0))
                    AddLine(Severity.Invalid, V320, CheckIdentifier.Egg);
                if (pkm is PK4 pk4)
                {
                    if (pk4.ShinyLeaf != 0)
                        AddLine(Severity.Invalid, V414, CheckIdentifier.Egg);
                    if (pk4.PokéathlonStat != 0)
                        AddLine(Severity.Invalid, V415, CheckIdentifier.Egg);
                }
            }

            if (!Encounter.Valid)
                return;

            if (Info.Generation == 5)
                VerifyNsPKM();

            switch (EncounterMatch)
            {
                case WC3 w when w.Fateful:
                    VerifyFatefulIngameActive();
                    return;
                case MysteryGift g when g.Format != 3: // WC3
                    VerifyFatefulMysteryGift(g);
                    return;
                case EncounterStatic s when s.Fateful: // ingame fateful
                case EncounterSlot _ when pkm.Version == 15: // ingame pokespot
                    VerifyFatefulIngameActive();
                    return;
                default:
                    if (pkm.FatefulEncounter)
                        AddLine(Severity.Invalid, V325, CheckIdentifier.Fateful);
                    return;
            }
        }
        private void VerifyMiscEggCommon()
        {
            if (new[] {pkm.Move1_PPUps, pkm.Move2_PPUps, pkm.Move3_PPUps, pkm.Move4_PPUps}.Any(ppup => ppup > 0))
                AddLine(Severity.Invalid, V319, CheckIdentifier.Egg);
            if (pkm.Move1_PP != pkm.GetMovePP(pkm.Move1, 0) || pkm.Move2_PP != pkm.GetMovePP(pkm.Move2, 0)
                || pkm.Move3_PP != pkm.GetMovePP(pkm.Move3, 0) || pkm.Move4_PP != pkm.GetMovePP(pkm.Move4, 0))
                AddLine(Severity.Invalid, V420, CheckIdentifier.Egg);

            var HatchCycles = (EncounterMatch as EncounterStatic)?.EggCycles;
            if (HatchCycles == 0 || HatchCycles == null)
                HatchCycles = pkm.PersonalInfo.HatchCycles;
            if (pkm.CurrentFriendship > HatchCycles)
                AddLine(Severity.Invalid, V374, CheckIdentifier.Egg);
        }
        private void VerifyFatefulMysteryGift(MysteryGift g)
        {
            if (g is PGF p && p.IsShiny)
            {
                Info.PIDIV = MethodFinder.Analyze(pkm);
                if (Info.PIDIV.Type != PIDType.G5MGShiny)
                    AddLine(Severity.Invalid, V411, CheckIdentifier.PID);
            }

            bool fatefulValid = false;
            if (g.Format == 3)
            {
                // obedience flag in gen3 is the fateful flag; met location stores the fateful info until transfer
                bool required = g.Species == 151 || g.Species == 386;
                required |= pkm.Format != 3 && !g.IsEgg;
                fatefulValid = !(required ^ pkm.FatefulEncounter);

                var g3 = (WC3)g; // shiny locked gifts
                if (g3.Shiny != null && g3.Shiny != pkm.IsShiny)
                    AddLine(Severity.Invalid, V409, CheckIdentifier.Fateful);
            }
            else
            {
                if (pkm.FatefulEncounter)
                    fatefulValid = true;
            }

            if (fatefulValid)
                AddLine(Severity.Valid, V321, CheckIdentifier.Fateful);
            else
                AddLine(Severity.Invalid, V322, CheckIdentifier.Fateful);
        }
        private void VerifyFatefulIngameActive()
        {
            if (pkm.Version == 15 && pkm is XK3 xk3 && Info.WasXD)
            {
                // can't have fateful until traded away, which clears ShadowID
                if (xk3.FatefulEncounter && xk3.ShadowID != 0)
                    AddLine(Severity.Invalid, V325, CheckIdentifier.Fateful);

                return; // fateful is set when transferred away
            }

            if (pkm.FatefulEncounter)
                AddLine(Severity.Valid, V323, CheckIdentifier.Fateful);
            else
                AddLine(Severity.Invalid, V324, CheckIdentifier.Fateful);
        }
        private void VerifyNsPKM()
        {
            bool req = EncounterMatch is EncounterStatic s && s.NSparkle;
            if (pkm.Format == 5)
            {
                bool has = ((PK5)pkm).NPokémon;
                if (req && !has)
                    AddLine(Severity.Invalid, V326, CheckIdentifier.Fateful);
                if (!req && has)
                    AddLine(Severity.Invalid, V327, CheckIdentifier.Fateful);
            }
            if (req)
            {
                if (pkm.IVs.Any(iv => iv != 30))
                    AddLine(Severity.Invalid, V218, CheckIdentifier.IVs);
                if (!VerifyNsPKMOTValid())
                    AddLine(Severity.Invalid, V219, CheckIdentifier.Trainer);
                if (pkm.IsShiny)
                    AddLine(Severity.Invalid, V220, CheckIdentifier.Shiny);
            }
        }
        private bool VerifyNsPKMOTValid()
        {
            if (pkm.TID != 00002 || pkm.SID != 00000)
                return false;

            switch (pkm.Language)
            {
                case 1: // jp
                    return pkm.OT_Name == "Ｎ";
                default:
                    return pkm.OT_Name == "N";
            }
        }
        private void VerifyVersionEvolution()
        {
            if (pkm.Format < 7)
                return;

            // No point using the evolution tree. Just handle certain species.
            switch (pkm.Species)
            {
                case 745: // Lycanroc
                    if (!pkm.WasEgg)
                        break;

                    if (pkm.AltForm == 0 && pkm.Version == 31 // Moon
                        || pkm.AltForm == 1 && pkm.Version == 30) // Sun
                        if (pkm.IsUntraded)
                            AddLine(Severity.Invalid, V328, CheckIdentifier.Evolution);
                    break;

                case 791: // Solgaleo
                    if (pkm.Version == 31 && pkm.IsUntraded)
                    {
                        if (EncounterMatch is MysteryGift g && g.Species == pkm.Species) // Gifted via Mystery Gift
                            break;
                        AddLine(Severity.Invalid, V328, CheckIdentifier.Evolution);
                    }
                    break;
                case 792: // Lunala
                    if (pkm.Version == 30 && pkm.IsUntraded)
                    {
                        if (EncounterMatch is MysteryGift g && g.Species == pkm.Species) // Gifted via Mystery Gift
                            break;
                        AddLine(Severity.Invalid, V328, CheckIdentifier.Evolution);
                    }
                    break;
            }
        }
        #region VerifyMoves
        #endregion
        public static string[] MoveStrings { internal get; set; } = Util.GetMovesList("en");
        public static string[] SpeciesStrings { internal get; set; } = Util.GetSpeciesList("en");
        internal static IEnumerable<string> getMoveNames(IEnumerable<int> moves) => moves.Select(m => m >= MoveStrings.Length ? V190 : MoveStrings[m]);
    }
}

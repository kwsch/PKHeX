using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    public enum Severity
    {
        Indeterminate = -2,
        Invalid = -1,
        Fishy = 0,
        Valid = 1,
        NotImplemented = 2,
    }
    internal enum CheckIdentifier
    {
        Move,
        RelearnMove,
        Encounter,
        History,
        ECPID,
        Shiny,
        EC,
        PID,
        Gender,
        EVs,
        Language,
        Nickname,
        Trainer,
        IVs,
        None,
        Level,
        Ball,
        Memory,
        Geography,
        Form,
        Egg,
        Misc,
        Fateful,
        Ribbon,
        Training,
        Ability,
        Evolution,
        Special,
        Nature
    }
    public class CheckResult
    {
        internal readonly Severity Judgement = Severity.Valid;
        internal string Comment = V;
        public bool Valid => Judgement >= Severity.Fishy;
        public bool Flag;
        internal readonly CheckIdentifier Identifier;

        internal CheckResult(CheckIdentifier i) { Identifier = i; }
        internal CheckResult(Severity s, string c, CheckIdentifier i)
        {
            Judgement = s;
            Comment = c;
            Identifier = i;
        }
    }
    public partial class LegalityAnalysis
    {
        private void verifyGender()
        {
            if (pkm.PersonalInfo.Gender == 255 && pkm.Gender != 2)
                AddLine(Severity.Invalid, V203, CheckIdentifier.Gender);

            // Check for PID relationship to Gender & Nature if applicable
            int gen = pkm.GenNumber;

            bool PIDGender = 3 <= gen && gen <= 5;
            if (!PIDGender)
                return;

            bool genderValid = pkm.getGenderIsValid();
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

            if (pkm.PID % 25 == pkm.Nature)
                AddLine(Severity.Valid, V252, CheckIdentifier.Nature);
            else
                AddLine(Severity.Invalid, V253, CheckIdentifier.Nature);
        }
        private void verifyItem()
        {
            if (!Legal.getHeldItemAllowed(pkm.Format, pkm.HeldItem))
                AddLine(Severity.Invalid, V204, CheckIdentifier.Form);
        }
        private void verifyECPID()
        {
            if (pkm.EncryptionConstant == 0)
                AddLine(Severity.Fishy, V201, CheckIdentifier.EC);

            if (pkm.PID == 0)
                AddLine(Severity.Fishy, V207, CheckIdentifier.PID);

            if (pkm.GenNumber >= 6 && pkm.PID == pkm.EncryptionConstant)
                AddLine(Severity.Fishy, V208, CheckIdentifier.PID);

            if (EncounterType == typeof (EncounterStatic))
            {
                var enc = (EncounterStatic) EncounterMatch;
                if (enc.Shiny != null && (bool) enc.Shiny ^ pkm.IsShiny)
                {
                    AddLine(Severity.Invalid, V209, CheckIdentifier.Shiny);
                    return;
                }
            }

            int wIndex = Array.IndexOf(Legal.WurmpleEvolutions, pkm.Species);
            if (pkm.GenNumber >= 6)
            {
                // Wurmple -> Silcoon/Cascoon
                if (wIndex > -1)
                {
                    // Check if Wurmple was the origin (only Egg and Wild Encounter)
                    if (pkm.WasEgg || (EncounterType == typeof(EncounterSlot[]) && (EncounterMatch as EncounterSlot[]).All(slot => slot.Species == 265)))
                        if ((pkm.EncryptionConstant >> 16)%10/5 != wIndex/2)
                        {
                            AddLine(Severity.Invalid, V210, CheckIdentifier.EC);
                            return;
                        }
                }
                else if (pkm.Species == 265)
                {
                    AddLine(Severity.Valid, string.Format(V212, (pkm.EncryptionConstant >> 16)%10/5 == 0 ? V213 : V214), CheckIdentifier.EC);
                }
                
                int xor = pkm.TSV ^ pkm.PSV;
                if (xor < 16 && xor >= 8 && (pkm.PID ^ 0x80000000) == pkm.EncryptionConstant)
                {
                    AddLine(Severity.Fishy, V211, CheckIdentifier.EC);
                    return;
                }
            }
            
            // abort if specimen wasn't transferred x->6
            if (pkm.Format < 6 || !(3 <= pkm.GenNumber && pkm.GenNumber <= 5))
                return;

            // When transferred to Generation 6, the Encryption Constant is copied from the PID.
            // The PID is then checked to see if it becomes shiny with the new Shiny rules (>>4 instead of >>3)
            // If the PID is nonshiny->shiny, the top bit is flipped.

            // Check to see if the PID and EC are properly configured.
            bool xorPID = ((pkm.TID ^ pkm.SID ^ (int)(pkm.PID & 0xFFFF) ^ (int)(pkm.PID >> 16)) & ~0x7) == 8;
            bool valid = xorPID
                ? pkm.EncryptionConstant == (pkm.PID ^ 0x8000000)
                : pkm.EncryptionConstant == pkm.PID;

            if (!valid)
            {
                AddLine(Severity.Invalid,
                    xorPID 
                    ? V215
                    : V216, CheckIdentifier.ECPID);
            }
        }

        private void verifyNickname()
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

            if (pkm.VC)
            {
                string pk = pkm.Nickname;
                var langset = PKX.SpeciesLang.FirstOrDefault(s => s.Contains(pk)) ?? PKX.SpeciesLang[2];
                int lang = Array.IndexOf(PKX.SpeciesLang, langset);

                if (pk.Length > (lang == 2 ? 10 : 5))
                    AddLine(Severity.Invalid, V1, CheckIdentifier.Nickname);
            }

            if (!Encounter.Valid)
                return;

            if (pkm.Format <= 6 && pkm.Language > 8)
            {
                AddLine(Severity.Indeterminate, V4, CheckIdentifier.Language);
                return;
            }
            if (pkm.Format <= 7 && pkm.Language > 10)
            {
                AddLine(Severity.Indeterminate, V5, CheckIdentifier.Language);
                return;
            }

            if (EncounterType == typeof(EncounterTrade))
            {
                verifyNicknameTrade();
                return;
            }

            if (pkm.IsEgg)
            {
                verifyNicknameEgg();
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
                bool match = PKX.getSpeciesNameGeneration(pkm.Species, pkm.Language, pkm.Format) == nickname;
                if (pkm.WasTradedEgg || Legal.getHasEvolved(pkm))
                    match |= !PKX.getIsNicknamedAnyLanguage(pkm.Species, nickname, pkm.Format);
                if (pkm.Format == 5 && !pkm.IsNative) // transfer
                    match |= PKX.getSpeciesNameGeneration(pkm.Species, pkm.Language, 4) == nickname;

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
        }
        private void verifyNicknameEgg()
        {
            if (!pkm.IsNicknamed && (pkm.Format != 7))
                AddLine(Severity.Invalid, V12, CheckIdentifier.Egg);
            else if (PKX.SpeciesLang[pkm.Language][0] != pkm.Nickname)
                AddLine(Severity.Invalid, V13, CheckIdentifier.Egg);
            else
                AddLine(Severity.Valid, V14, CheckIdentifier.Egg);
        }
        private void verifyNicknameTrade()
        {
            string[] validOT = new string[0];
            int index = -1;
            if (pkm.XY)
            {
                validOT = Legal.TradeXY[pkm.Language];
                index = Array.IndexOf(Legal.TradeGift_XY, EncounterMatch);
            }
            else if (pkm.AO)
            {
                validOT = Legal.TradeAO[pkm.Language];
                index = Array.IndexOf(Legal.TradeGift_AO, EncounterMatch);
            }
            else if (pkm.SM)
            {
                // TODO
                AddLine(Severity.Valid, V194, CheckIdentifier.Nickname);
                return;
            }
            else if (pkm.Format <= 2 || pkm.VC)
            {
                var et = EncounterOriginal as EncounterTrade;
                if (et?.TID == 0) // Gen1 Trade
                {
                    if (!Legal.getEncounterTrade1Valid(pkm))
                        AddLine(Severity.Invalid, V10, CheckIdentifier.Trainer);
                }
                else // Gen2
                {
                    return; // already checked all relevant properties when fetching with getValidEncounterTradeVC2
                }
                return;
            }
            else if (3 <= pkm.Format && pkm.Format <= 5)
            { 
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

        private void verifyEVs()
        {
            var evs = pkm.EVs;
            int sum = evs.Sum();
            if (pkm.IsEgg && sum > 0)
                AddLine(Severity.Invalid, V22, CheckIdentifier.EVs);
            else if (sum == 0 && pkm.CurrentLevel - Math.Max(1, pkm.Met_Level) > 0)
                AddLine(Severity.Fishy, V23, CheckIdentifier.EVs);
            else if (sum == 508)
                AddLine(Severity.Fishy, V24, CheckIdentifier.EVs);
            else if (sum > 510)
                AddLine(Severity.Invalid, V25, CheckIdentifier.EVs);
            else if (pkm.Format >= 6 && evs.Any(ev => ev > 252))
                AddLine(Severity.Invalid, V26, CheckIdentifier.EVs);
            else if (evs.All(ev => pkm.EVs[0] == ev) && evs[0] != 0)
                AddLine(Severity.Fishy, V27, CheckIdentifier.EVs);
        }
        private void verifyIVs()
        {
            var e = EncounterMatch as EncounterStatic;
            if ((EncounterMatch as EncounterStatic)?.IV3 == true)
            {
                int IVCount = 3;
                if (e.Version == GameVersion.RBY && pkm.Species == 151)
                    IVCount = 5; // VC Mew
                if (pkm.IVs.Count(iv => iv == 31) < IVCount)
                {
                    AddLine(Severity.Invalid, string.Format(V28, IVCount), CheckIdentifier.IVs);
                    return;
                }
            }
            if ((EncounterMatch as EncounterSlot[])?.All(slot => slot.Type == SlotType.FriendSafari) == true)
            {
                if (pkm.IVs.Count(iv => iv == 31) < 2)
                {
                    AddLine(Severity.Invalid, V29, CheckIdentifier.IVs);
                    return;
                }
            }
            if (EncounterIsMysteryGift)
            {
                int[] IVs;
                switch (((MysteryGift) EncounterMatch).Format)
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
                }
            }
            if (pkm.IVs.Sum() == 0)
                AddLine(Severity.Fishy, V31, CheckIdentifier.IVs);
            else if (pkm.IVs[0] < 30 && pkm.IVs.All(iv => pkm.IVs[0] == iv))
                AddLine(Severity.Fishy, V32, CheckIdentifier.IVs);
        }
        private void verifyDVs()
        {
            // todo
        }
        private void verifyOT()
        {
            if (EncounterType == typeof(EncounterTrade))
                return; // Already matches Encounter Trade information

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

            if (pkm.VC)
                verifyG1OT();
        }
        private void verifyG1OT()
        {
            string tr = pkm.OT_Name;
            string pk = pkm.Nickname;
            var langset = PKX.SpeciesLang.FirstOrDefault(s => s.Contains(pk)) ?? PKX.SpeciesLang[2];
            int lang = Array.IndexOf(PKX.SpeciesLang, langset);

            if (tr.Length > (lang == 2 ? 7 : 5))
                AddLine(Severity.Invalid, V38, CheckIdentifier.Trainer);
            if (pkm.Species == 151)
            {
                if (tr != "GF" && tr != "ゲーフリ") // if there are more events with special OTs, may be worth refactoring
                    AddLine(Severity.Invalid, V39, CheckIdentifier.Trainer);
            }
        }

        private void verifyHyperTraining()
        {
            if (pkm.Format < 7)
                return; // No Hyper Training before Gen VII

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
                    if ((IVs[i] == 31) && HTs[i])
                        AddLine(Severity.Invalid, V42, CheckIdentifier.IVs);
                }
            }
        }

        private CheckResult verifyEncounterLink()
        {
            // Should NOT be Fateful, and should be in Database
            EncounterLink enc = EncounterMatch as EncounterLink;
            if (enc == null)
                return new CheckResult(Severity.Invalid, V43, CheckIdentifier.Encounter);

            if (pkm.XY && !enc.XY)
                return new CheckResult(Severity.Invalid, V44, CheckIdentifier.Encounter);
            if (pkm.AO && !enc.ORAS)
                return new CheckResult(Severity.Invalid, V45, CheckIdentifier.Encounter);

            if (enc.Shiny != null && (bool)enc.Shiny ^ pkm.IsShiny)
                return new CheckResult(Severity.Invalid, V47, CheckIdentifier.Encounter);

            return pkm.FatefulEncounter
                ? new CheckResult(Severity.Invalid, V48, CheckIdentifier.Encounter)
                : new CheckResult(Severity.Valid, V49, CheckIdentifier.Encounter);
        }
        private CheckResult verifyEncounterEvent()
        {
            MysteryGift MatchedGift = EncounterMatch as MysteryGift;
            if (MatchedGift == null)
                return null;
            return new CheckResult(Severity.Valid, string.Format(V21, MatchedGift.CardID.ToString("0000"), MatchedGift.CardTitle), CheckIdentifier.Encounter);
        }

        private CheckResult verifyEncounterEgg()
        {
            // Check Species
            if (Legal.NoHatchFromEgg.Contains(pkm.Species) && EncounterMatch == null)
                return new CheckResult(Severity.Invalid, V50, CheckIdentifier.Encounter);
            if (pkm.WasGiftEgg && EncounterMatch == null)
                return new CheckResult(Severity.Invalid, V359, CheckIdentifier.Encounter);
            if (pkm.WasEventEgg && EncounterMatch == null)
                return new CheckResult(Severity.Invalid, V360, CheckIdentifier.Encounter);
            switch (pkm.GenNumber)
            {
                case 1:
                case 2: return new CheckResult(CheckIdentifier.Encounter); // no met location info
                case 3: return verifyEncounterEgg3();
                case 4: return pkm.IsEgg ? verifyUnhatchedEgg(2002) : verifyEncounterEgg4();
                case 5: return pkm.IsEgg ? verifyUnhatchedEgg(30002) : verifyEncounterEgg5();
                case 6: return pkm.IsEgg ? verifyUnhatchedEgg(30002) : verifyEncounterEgg6();
                case 7: return pkm.IsEgg ? verifyUnhatchedEgg(30002) : verifyEncounterEgg7();

                default: // none of the above
                    return new CheckResult(Severity.Invalid, V51, CheckIdentifier.Encounter);
            }
        }
        private CheckResult verifyEncounterEgg3()
        {
            return pkm.Format == 3 ? verifyEncounterEgg3Native() : verifyEncounterEgg3Transfer();
        }
        private CheckResult verifyEncounterEgg3Native()
        {
            if (pkm.Met_Level != 0)
                return new CheckResult(Severity.Invalid, string.Format(V52, 0), CheckIdentifier.Encounter);
            if (pkm.IsEgg)
            {
                var loc = pkm.FRLG ? Legal.ValidEggMet_FRLG : Legal.ValidEggMet_RSE;
                if (!loc.Contains(pkm.Met_Location)) 
                    return new CheckResult(Severity.Invalid, V55, CheckIdentifier.Encounter);
            }
            else
            {
                var locs = pkm.FRLG ? Legal.ValidMet_FRLG : pkm.E ? Legal.ValidMet_E : Legal.ValidMet_RS;
                if (locs.Contains(pkm.Met_Location))
                    return new CheckResult(Severity.Valid, V53, CheckIdentifier.Encounter);
                if (Legal.ValidMet_FRLG.Contains(pkm.Met_Location) || Legal.ValidMet_E.Contains(pkm.Met_Location) || Legal.ValidMet_RS.Contains(pkm.Met_Location))
                    return new CheckResult(Severity.Valid, V56, CheckIdentifier.Encounter);
                return new CheckResult(Severity.Invalid, V54, CheckIdentifier.Encounter);
            }
            return new CheckResult(Severity.Valid, V53, CheckIdentifier.Encounter);
        }
        private CheckResult verifyEncounterEgg3Transfer()
        {
            if (pkm.IsEgg)
                return new CheckResult(Severity.Invalid, V57, CheckIdentifier.Encounter);
            if (pkm.Met_Level < 5)
                return new CheckResult(Severity.Invalid, V58, CheckIdentifier.Encounter);
            if (pkm.Egg_Location != 0)
                return new CheckResult(Severity.Invalid, V59, CheckIdentifier.Encounter);
            if (pkm.Format == 4 && pkm.Met_Location != 0x37) // Pal Park
                return new CheckResult(Severity.Invalid, V60, CheckIdentifier.Encounter);
            if (pkm.Format != 4 && pkm.Met_Location != 30001)
                return new CheckResult(Severity.Invalid, V61, CheckIdentifier.Encounter);

            return new CheckResult(Severity.Valid, V53, CheckIdentifier.Encounter);
        }
        private CheckResult verifyEncounterEgg4()
        {
            if (pkm.Format == 4)
                return verifyEncounterEggLevelLoc(0, pkm.HGSS ? Legal.ValidMet_HGSS : pkm.Pt ? Legal.ValidMet_Pt : Legal.ValidMet_DP);
            if (pkm.IsEgg)
                return new CheckResult(Severity.Invalid, V57, CheckIdentifier.Encounter);
            // transferred
            if (pkm.Met_Level < 1)
                return new CheckResult(Severity.Invalid, V58, CheckIdentifier.Encounter);

            if (pkm.Met_Location != 30001)
                return new CheckResult(Severity.Invalid, V61, CheckIdentifier.Encounter);
            return new CheckResult(Severity.Valid, V63, CheckIdentifier.Encounter);
        }
        private CheckResult verifyEncounterEgg5()
        {
            return verifyEncounterEggLevelLoc(1, pkm.B2W2 ? Legal.ValidMet_B2W2 : Legal.ValidMet_BW);
        }
        private CheckResult verifyEncounterEgg6()
        {
            if (pkm.AO)
                return verifyEncounterEggLevelLoc(1, Legal.ValidMet_AO);

            if (pkm.Egg_Location == 318)
                return new CheckResult(Severity.Invalid, V55, CheckIdentifier.Encounter);

            return verifyEncounterEggLevelLoc(1, Legal.ValidMet_XY);
        }
        private CheckResult verifyEncounterEgg7()
        {
            if (pkm.SM)
                return verifyEncounterEggLevelLoc(1, Legal.ValidMet_SM);

            // no other games
            return new CheckResult(Severity.Invalid, V51, CheckIdentifier.Encounter);
        }
        private CheckResult verifyEncounterEggLevelLoc(int eggLevel, int[] MetLocations)
        {
            if (pkm.Met_Level != eggLevel)
                return new CheckResult(Severity.Invalid, string.Format(V52, eggLevel), CheckIdentifier.Encounter);
            return MetLocations.Contains(pkm.Met_Location)
                ? new CheckResult(Severity.Valid, V53, CheckIdentifier.Encounter)
                : new CheckResult(Severity.Invalid, V54, CheckIdentifier.Encounter);
        }
        private CheckResult verifyUnhatchedEgg(int tradeLoc)
        {
            if (pkm.Egg_Location == tradeLoc)
                return new CheckResult(Severity.Invalid, V62, CheckIdentifier.Encounter);

            if (pkm.Met_Location == tradeLoc)
                return new CheckResult(Severity.Valid, V56, CheckIdentifier.Encounter);
            return pkm.Met_Location == 0
                ? new CheckResult(Severity.Valid, V63, CheckIdentifier.Encounter)
                : new CheckResult(Severity.Invalid, V59, CheckIdentifier.Encounter);
        }

        private CheckResult verifyEncounterSafari()
        {
            switch (pkm.Species)
            {
                case 670: // Floette
                case 671: // Florges
                    if (!new[] {0, 1, 3}.Contains(pkm.AltForm)) // 0/1/3 - RBY
                        return new CheckResult(Severity.Invalid, V64, CheckIdentifier.Encounter);
                    break;
                case 710: // Pumpkaboo
                case 711: // Goregeist
                    if (pkm.AltForm != 0) // Average
                        return new CheckResult(Severity.Invalid, V6, CheckIdentifier.Encounter);
                    break;
                case 586: // Sawsbuck
                    if (pkm.AltForm != 0)
                        return new CheckResult(Severity.Invalid, V65, CheckIdentifier.Encounter);
                    break;
            }

            return new CheckResult(Severity.Valid, V66, CheckIdentifier.Encounter);
        }
        private CheckResult verifyEncounterWild()
        {
            EncounterSlot[] enc = (EncounterSlot[])EncounterMatch;

            if (enc.Any(slot => slot.Normal))
                return enc.All(slot => slot.Pressure)
                    ? new CheckResult(Severity.Valid, V67, CheckIdentifier.Encounter)
                    : new CheckResult(Severity.Valid, V68, CheckIdentifier.Encounter);

            // Decreased Level Encounters
            if (enc.Any(slot => slot.WhiteFlute))
                return enc.All(slot => slot.Pressure)
                    ? new CheckResult(Severity.Valid, V69, CheckIdentifier.Encounter)
                    : new CheckResult(Severity.Valid, V70, CheckIdentifier.Encounter);

            // Increased Level Encounters
            if (enc.Any(slot => slot.BlackFlute))
                return enc.All(slot => slot.Pressure)
                    ? new CheckResult(Severity.Valid, V71, CheckIdentifier.Encounter)
                    : new CheckResult(Severity.Valid, V72, CheckIdentifier.Encounter);

            if (enc.Any(slot => slot.Pressure))
                return new CheckResult(Severity.Valid, V67, CheckIdentifier.Encounter);

            return new CheckResult(Severity.Valid, V73, CheckIdentifier.Encounter);
        }
        private CheckResult verifyEncounterStatic()
        {
            var s = (EncounterStatic)EncounterMatch;

            // Check for Unreleased Encounters / Collisions
            switch (pkm.GenNumber)
            {
                case 3:
                    if (pkm.Species == 151 && s.Location == 201 && pkm.Language != 1) // Non-JP Mew (Old Sea Map)
                        return new CheckResult(Severity.Invalid, V353, CheckIdentifier.Encounter);
                    break;
                case 4:
                    if (pkm.Species == 493 && s.Location == 086) // Azure Flute Arceus
                        return new CheckResult(Severity.Invalid, V352, CheckIdentifier.Encounter);
                    if (pkm.Species == 492 && s.Location == 063 && !pkm.Pt) // DP Shaymin
                        return new CheckResult(Severity.Invalid, V354, CheckIdentifier.Encounter);
                    break;
                case 7:
                    if (s.EggLocation == 60002 && vRelearn.All(rl => rl.Valid))
                        return null; // not gift egg
                    break;
            }

            // Re-parse relearn moves
            if (pkm.Format >= 6)
            for (int i = 0; i < 4; i++)
                vRelearn[i] = pkm.RelearnMoves[i] != s.Relearn[i]
                    ? new CheckResult(Severity.Invalid, V74, CheckIdentifier.RelearnMove)
                    : new CheckResult(CheckIdentifier.RelearnMove);

            return new CheckResult(Severity.Valid, V75, CheckIdentifier.Encounter);
        }
        private CheckResult verifyEncounterTrade()
        {
            return new CheckResult(Severity.Valid, V76, CheckIdentifier.Encounter);
        }
        private CheckResult verifyEncounterG12()
        {
            var obj = Legal.getEncounter12(pkm, Legal.AllowGBCartEra && pkm.Format < 3);
            if (obj == null)
                return new CheckResult(Severity.Invalid, V80, CheckIdentifier.Encounter);

            EncounterMatch = obj.Item1;
            if (EncounterMatch is bool)
            {
                pkm.WasEgg = true;
                return verifyEncounterEgg();
            }
            if (EncounterMatch is EncounterSlot[])
                return verifyEncounterWild();
            if (EncounterMatch is EncounterStatic)
                return verifyEncounterStatic();
            if (EncounterMatch is EncounterTrade)
                return verifyEncounterTrade();

            // shouldn't ever hit, above 3*invalid check should abort
            Console.WriteLine($"Gen1 encounter fallthrough: {pkm.FileName}");
            return new CheckResult(Severity.Invalid, V80, CheckIdentifier.Encounter);
        }
        private CheckResult verifyEncounterVC()
        {
            int baseSpecies = Legal.getBaseSpecies(pkm);
            bool g1 = pkm.VC1 || pkm.Format == 1;

            if ((g1 && baseSpecies > Legal.MaxSpeciesID_1) || (baseSpecies > Legal.MaxSpeciesID_2))
                return new CheckResult(Severity.Invalid, V77, CheckIdentifier.Encounter);

            // Get EncounterMatch prior to parsing transporter legality
            var result = verifyEncounterG12();
            EncounterOriginal = EncounterMatch;

            if (pkm.Format > 2) // transported to 7+
                AddLine(verifyVCEncounter(baseSpecies));

            return result;
        }
        private CheckResult verifyEncounter()
        {
            // Special considerations have to be applied when encounter info is lost on transfer.
            // Generation 1/2 PKM do not reliably store met location or original version.
            if (pkm.VC || pkm.Format < 3)
                return verifyEncounterVC();
            
            // Generation 3/4 PKM do not retain met location when transferred.
            if (!pkm.HasOriginalMetLocation)
            {
                if (pkm.Gen3)
                    return verifyEncounterG3Transfer();
                if (pkm.Gen4)
                    return verifyEncounterG4Transfer();
            }

            if (pkm.WasLink)
                return verifyEncounterLink();

            bool wasEvent = pkm.WasEvent || pkm.WasEventEgg;
            if (wasEvent)
            {
                var result = verifyEncounterEvent();
                if (result != null)
                    return result;
            }

            if (null != (EncounterMatch = Legal.getValidStaticEncounter(pkm)))
            {
                var result = verifyEncounterStatic();
                if (result != null)
                    return result;

                EncounterMatch = null; // Reset Encounter Object, test for remaining encounters
            }

            if (pkm.WasEgg)
                return verifyEncounterEgg();
            
            if (null != (EncounterMatch = Legal.getValidFriendSafari(pkm)))
                return verifyEncounterSafari();
            
            if (null != (EncounterMatch = Legal.getValidWildEncounters(pkm)))
                return verifyEncounterWild();
            
            if (null != (EncounterMatch = Legal.getValidIngameTrade(pkm)))
                return verifyEncounterTrade();

            return wasEvent
                ? new CheckResult(Severity.Invalid, V78, CheckIdentifier.Encounter) 
                : new CheckResult(Severity.Invalid, V80, CheckIdentifier.Encounter);
        }

        private CheckResult verifyEncounterG3Transfer()
        {
            // WasEventEgg is not possible in gen 3 pal park pokemon, are indistinguible from normal eggs
            bool wasEvent = pkm.WasEvent;
            CheckResult EggResult = null;
            CheckResult G3Result = null;
            object G3Encounter = null;
            bool WasEgg = Legal.getWasEgg23(pkm) && !Legal.NoHatchFromEgg.Contains(pkm.Species);
            if (WasEgg)
            {
                pkm.WasEgg = true;
                EggResult = verifyEncounterEgg3Transfer();
                if (pkm.IsEgg)
                    return EggResult;
            }

            EncounterMatch = null;
            if (null != (EncounterMatch = Legal.getValidIngameTrade(pkm)))
            {
                if ((G3Result = verifyEncounterTrade())?.Valid ?? false)
                    G3Encounter = EncounterMatch;
            }
            else if (null != (EncounterMatch = Legal.getValidWildEncounters(pkm)))
            {
                if ((G3Result = verifyEncounterWild())?.Valid ?? false)
                    G3Encounter = EncounterMatch;
            }
            else if (null != (EncounterMatch = Legal.getValidStaticEncounter(pkm)))
            {
                if ((G3Result = verifyEncounterStatic())?.Valid ?? false)
                    G3Encounter = EncounterMatch;
            }

            // Check events after static, to match Mew/Deoxys static encounters
            if (wasEvent && G3Result == null && pkm.Species != 151 && pkm.Species != 386)
            {
                G3Result = verifyEncounterEvent() ?? new CheckResult(Severity.Invalid, V78, CheckIdentifier.Encounter);
            }

            // Now check Mew and Deoxys, they can be event or static encounters both with fatefull encounter
            if (pkm.Species == 151 || pkm.Species == 386)
            {
                var EventResult = verifyEncounterEvent();
                // Only return event if is valid, if not return result from static encounter
                if (EventResult?.Valid ?? false)
                    G3Result = EventResult;
            }

            // Even if EggResult is not returned WasEgg is keep true to check in verifymoves first the 
            // non egg encounter moves and after that egg encounter moves, because there is no way to tell 
            // what of the two encounters was the real origin
            if (EggResult != null && G3Result != null)
            {
                // keep the valid encounter, also if both are valid returns non egg information, because 
                // there is more data in the pokemon to found normal encounter
                if (EggResult.Valid && !G3Result.Valid)
                {
                    G3Result = EggResult;
                    G3Encounter = null;
                }
            }

            if (G3Result?.Valid ?? false)
                EncounterMatch = G3Encounter;

            if (pkm.Format == 4 && pkm.Met_Location != 0x37) // Pal Park
                AddLine(Severity.Invalid, V60, CheckIdentifier.Encounter);
            if (pkm.Format != 4 && pkm.Met_Location != 30001)
                AddLine(Severity.Invalid, V61, CheckIdentifier.Encounter);

            return G3Result ?? new CheckResult(Severity.Invalid, V80, CheckIdentifier.Encounter);
        }
        private CheckResult verifyEncounterG4Transfer()
        {
            CheckResult Gen4Result = null;
            
            bool wasEvent = pkm.WasEvent || pkm.WasEventEgg;
            if (wasEvent)
            {
                var result = verifyEncounterEvent();
                if (result != null)
                    Gen4Result = result;
            }

            if (Gen4Result == null && null != (EncounterMatch = Legal.getValidStaticEncounter(pkm)))
            {
                var result = verifyEncounterStatic();
                if (result != null)
                    return result;

                EncounterMatch = null; // Reset Encounter Object, test for remaining encounters
            }

            if (pkm.WasEgg) // Invalid transfer is already checked in encounter egg
                return verifyEncounterEgg();

            if (Gen4Result == null && null != (EncounterMatch = Legal.getValidFriendSafari(pkm)))
                Gen4Result = verifyEncounterSafari();

            if (Gen4Result == null && null != (EncounterMatch = Legal.getValidWildEncounters(pkm)))
                Gen4Result = verifyEncounterWild();

            if (Gen4Result == null && null != (EncounterMatch = Legal.getValidIngameTrade(pkm)))
                Gen4Result = verifyEncounterTrade();

            if (Gen4Result == null)
                Gen4Result = wasEvent
                           ? new CheckResult(Severity.Invalid, V78, CheckIdentifier.Encounter)
                           : new CheckResult(Severity.Invalid, V80, CheckIdentifier.Encounter);

            // Transfer Legality

            // PokéTransfer
            int loc = pkm.Met_Location;
            if (loc == 30001)
                return Gen4Result;

            // Crown
            switch (pkm.Species)
            {
                case 251: // Celebi
                    if (pkm.Met_Location == 30010 || pkm.Met_Location == 30011) // unused || used
                        return Gen4Result;
                    return new CheckResult(Severity.Invalid, V351, CheckIdentifier.Encounter);
                    
                case 243: // Raikou
                case 244: // Entei
                case 245: // Suicune
                    if (pkm.Met_Location == 30012 || pkm.Met_Location == 30013) // unused || used
                        return Gen4Result;
                    return new CheckResult(Severity.Invalid, V351, CheckIdentifier.Encounter);
            }

            // No Match
            return new CheckResult(Severity.Invalid, V61, CheckIdentifier.Encounter);
        }
        private CheckResult verifyVCEncounter(int baseSpecies)
        {
            // Sanitize Species to non-future species#
            int species = pkm.Species;
            if ((pkm.VC1 && species > Legal.MaxSpeciesID_1) ||
                (pkm.VC2 && species > Legal.MaxSpeciesID_2))
                species = baseSpecies;

            // Check existing EncounterMatch
            if ((EncounterOriginal ?? EncounterMatch) == null)
                return new CheckResult(Severity.Invalid, V80, CheckIdentifier.Encounter);

            var s = EncounterMatch as EncounterStatic;
            if (s != null && s.Version == GameVersion.GBCartEraOnly)
            {
                bool exceptions = false;
                exceptions |= baseSpecies == 151 && pkm.TID == 22796;
                if (!exceptions)
                    AddLine(new CheckResult(Severity.Invalid, V79, CheckIdentifier.Encounter));
            }

            EncounterMatch = Legal.getRBYStaticTransfer(species);
            var ematch = (EncounterStatic) EncounterMatch;

            if (pkm.Met_Location != ematch.Location)
                return new CheckResult(Severity.Invalid, V81, CheckIdentifier.Encounter);
            if (pkm.Egg_Location != ematch.EggLocation)
                return new CheckResult(Severity.Invalid, V59, CheckIdentifier.Encounter);

            if (species == 150 && pkm.Moves.Contains(6)) // pay day
                return new CheckResult(Severity.Invalid, V82, CheckIdentifier.Encounter);

            return new CheckResult(CheckIdentifier.Encounter);
        }

        private void verifyLevel()
        {
            MysteryGift MatchedGift = EncounterMatch as MysteryGift;
            if (MatchedGift != null && MatchedGift.Level != pkm.Met_Level)
            {
                if (pkm.HasOriginalMetLocation && (!(MatchedGift is WC7) || ((WC7) MatchedGift).MetLevel != pkm.Met_Level))
                {
                    AddLine(new CheckResult(Severity.Invalid, V83, CheckIdentifier.Level));
                    return;
                }
            }
            if (MatchedGift != null && MatchedGift.Level > pkm.CurrentLevel)
            {
                AddLine(new CheckResult(Severity.Invalid, V84, CheckIdentifier.Level));
                return;
            }

            int lvl = pkm.CurrentLevel;
            if (pkm.IsEgg)
            {
                int elvl = pkm.Format <= 3 ? 5 : 1;
                if (elvl != lvl)
                    AddLine(Severity.Invalid, string.Format(V52, elvl), CheckIdentifier.Level);
            }
            else if (lvl < pkm.Met_Level)
                AddLine(Severity.Invalid, V85, CheckIdentifier.Level);
            else if ((pkm.WasEgg || EncounterMatch == null) && !Legal.getEvolutionValid(pkm) && pkm.Species != 350)
                AddLine(Severity.Invalid, V86, CheckIdentifier.Level);
            else if (lvl > pkm.Met_Level && lvl > 1 && lvl != 100 && pkm.EXP == PKX.getEXP(pkm.Stat_Level, pkm.Species))
                AddLine(Severity.Fishy, V87, CheckIdentifier.Level);
            else
                AddLine(Severity.Valid, V88, CheckIdentifier.Level);
        }

        private void verifyMedals()
        {
            if (pkm.Format < 6)
                return;
            
            verifyMedalsRegular();
            verifyMedalsEvent();
        }
        private void verifyMedalsRegular()
        {
            var TrainNames = ReflectUtil.getPropertiesStartWithPrefix(pkm.GetType(), "SuperTrain").ToArray();
            var TrainCount = TrainNames.Count(MissionName => ReflectUtil.GetValue(pkm, MissionName) as bool? == true);
            if (pkm.IsEgg && TrainCount > 0)
            { AddLine(Severity.Invalid, V89, CheckIdentifier.Training); }
            else if (TrainCount > 0 && pkm.GenNumber > 6)
            { AddLine(Severity.Invalid, V90, CheckIdentifier.Training); }
            else
            {
                if (pkm.Format >= 7)
                {
                    if (pkm.SecretSuperTrainingUnlocked)
                    { AddLine(Severity.Invalid, V91, CheckIdentifier.Training); }
                    if (pkm.SecretSuperTrainingComplete)
                    { AddLine(Severity.Invalid, V92, CheckIdentifier.Training); }
                }
                else
                {
                    if (TrainCount == 30 ^ pkm.SecretSuperTrainingComplete)
                    { AddLine(Severity.Invalid, V93, CheckIdentifier.Training); }
                }
            }
        }
        private void verifyMedalsEvent()
        {
            var DistNames = ReflectUtil.getPropertiesStartWithPrefix(pkm.GetType(), "DistSuperTrain");
            var DistCount = DistNames.Count(MissionName => ReflectUtil.GetValue(pkm, MissionName) as bool? == true);
            if (pkm.IsEgg && DistCount > 0)
            { AddLine(Severity.Invalid, V89, CheckIdentifier.Training); }
            else if (DistCount > 0 && pkm.GenNumber > 6)
            { AddLine(Severity.Invalid, V90, CheckIdentifier.Training); }
            else if (DistCount > 0)
            { AddLine(Severity.Fishy, V94, CheckIdentifier.Training); }
        }

        private void verifyRibbons()
        {
            if (!Encounter.Valid)
                return;

            List<string> missingRibbons = new List<string>();
            List<string> invalidRibbons = new List<string>();
            
            if (pkm.IsEgg)
            {
                var RibbonNames = ReflectUtil.getPropertiesStartWithPrefix(pkm.GetType(), "Ribbon");
                foreach (object RibbonValue in RibbonNames.Select(RibbonName => ReflectUtil.GetValue(pkm, RibbonName)))
                {
                    if (RibbonValue as bool? == true) // Boolean
                    { AddLine(Severity.Invalid, V95, CheckIdentifier.Ribbon); return; }
                    if ((RibbonValue as int?) > 0) // Count
                    { AddLine(Severity.Invalid, V95, CheckIdentifier.Ribbon); return; }
                }
                return;
            }

            // Check Event Ribbons
            var RibbonData = ReflectUtil.getPropertiesStartWithPrefix(pkm.GetType(), "Ribbon");
            MysteryGift MatchedGift = EncounterMatch as MysteryGift;
            string[] EventRib =
            {
                nameof(PK6.RibbonCountry), nameof(PK6.RibbonNational), nameof(PK6.RibbonEarth), nameof(PK6.RibbonWorld), nameof(PK6.RibbonClassic),
                nameof(PK6.RibbonPremier), nameof(PK6.RibbonEvent), nameof(PK6.RibbonBirthday), nameof(PK6.RibbonSpecial), nameof(PK6.RibbonSouvenir),
                nameof(PK6.RibbonWishing), nameof(PK6.RibbonChampionBattle), nameof(PK6.RibbonChampionRegional), nameof(PK6.RibbonChampionNational), nameof(PK6.RibbonChampionWorld)
            };
            if (MatchedGift != null) // Wonder Card
            {
                var mgRibbons = MatchedGift.Format == 4 ? EventRib : ReflectUtil.getPropertiesStartWithPrefix(MatchedGift.Content.GetType(), "Ribbon");
                var commonRibbons = mgRibbons.Intersect(RibbonData).ToArray();

                foreach (string r in commonRibbons)
                {
                    bool? pk = ReflectUtil.getBooleanState(pkm, r);
                    bool? mg = ReflectUtil.getBooleanState(MatchedGift.Content, r);
                    if (pk != mg) // Mismatch
                    {
                        if (pk ?? false)
                            missingRibbons.Add(r);
                        else
                            invalidRibbons.Add(r);
                    }
                }
            }
            else if (EncounterType == typeof(EncounterLink))
            {
                // No Event Ribbons except Classic (unless otherwise specified, ie not for Demo)
                for (int i = 0; i < EventRib.Length; i++)
                {
                    if (i == 4)
                        continue;

                    if (ReflectUtil.getBooleanState(pkm, EventRib[i]) == true)
                        invalidRibbons.Add(EventRibName[i]);
                }

                bool classic = ReflectUtil.getBooleanState(pkm, EventRib[4]) == true;
                if (classic ^ ((EncounterLink)EncounterMatch).Classic)
                    (classic ? invalidRibbons : missingRibbons).Add(EventRibName[4]);
            }
            else if (EncounterType == typeof(EncounterStatic))
            {
                // No Event Ribbons except Wishing (which is only for Magearna)
                for (int i = 0; i < EventRib.Length; i++)
                {
                    if (i == 10)
                        continue;

                    if (ReflectUtil.getBooleanState(pkm, EventRib[i]) == true)
                        invalidRibbons.Add(EventRibName[i]);
                }

                bool wishing = ReflectUtil.getBooleanState(pkm, EventRib[10]) == true;
                if (wishing ^ ((EncounterStatic)EncounterMatch).RibbonWishing)
                    (wishing ? invalidRibbons : missingRibbons).Add(EventRibName[10]);
            }
            else // No ribbons
            {
                for (int i = 0; i < EventRib.Length; i++)
                    if (ReflectUtil.getBooleanState(pkm, EventRib[i]) == true)
                        invalidRibbons.Add(EventRibName[i]);
            }
            
            // Unobtainable ribbons for Gen Origin
            if (pkm.GenNumber > 3)
            {
                if (ReflectUtil.getBooleanState(pkm, nameof(PK3.RibbonChampionG3Hoenn)) == true)
                    invalidRibbons.Add(V96); // RSE HoF
                if (ReflectUtil.getBooleanState(pkm, nameof(PK3.RibbonArtist)) == true)
                    invalidRibbons.Add(V97); // RSE Master Rank Portrait
                if (ReflectUtil.getBooleanState(pkm, nameof(PK3.RibbonNational)) == true && pkm.Version != (int)GameVersion.CXD)
                    invalidRibbons.Add(V98); // RSE HoF
            }
            if (pkm.GenNumber > 4)
            {
                if (ReflectUtil.getBooleanState(pkm, nameof(PK4.RibbonChampionSinnoh)) == true)
                    invalidRibbons.Add(V99); // DPPt HoF
                if (ReflectUtil.getBooleanState(pkm, nameof(PK4.RibbonLegend)) == true)
                    invalidRibbons.Add(V100); // HGSS Defeat Red @ Mt.Silver
            }
            if (pkm.Format >= 6 && pkm.GenNumber >= 6)
            {
                if (ReflectUtil.getBooleanState(pkm, nameof(PK6.RibbonCountMemoryContest)) == true)
                    invalidRibbons.Add(V106); // Gen3/4 Contest
                if (ReflectUtil.getBooleanState(pkm, nameof(PK6.RibbonCountMemoryBattle)) == true)
                    invalidRibbons.Add(V105); // Gen3/4 Battle
            }
            if (ReflectUtil.getBooleanState(pkm, nameof(PK6.RibbonRecord)) == true)
                invalidRibbons.Add(V104); // Unobtainable
            
            if (missingRibbons.Count + invalidRibbons.Count == 0)
            {
                AddLine(Severity.Valid, V103, CheckIdentifier.Ribbon);
                return;
            }

            string[] result = new string[2];
            if (missingRibbons.Count > 0)
                result[0] = string.Format(V101, string.Join(", ", missingRibbons));
            if (invalidRibbons.Count > 0)
                result[1] = string.Format(V102, string.Join(", ", invalidRibbons));
            AddLine(Severity.Invalid, string.Join(Environment.NewLine, result.Where(s => !string.IsNullOrEmpty(s))), CheckIdentifier.Ribbon);
        }
        private void verifyAbility()
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

            if (EncounterMatch != null)
            {
                // Check Hidden Ability Mismatches
                if (pkm.GenNumber >= 5)
                {
                    bool valid = true;
                    if (EncounterType == typeof(EncounterStatic))
                    {
                        if (pkm.AbilityNumber == 4 ^ ((EncounterStatic) EncounterMatch).Ability == 4)
                            valid = false;
                    }
                    else if (EncounterType == typeof(EncounterTrade))
                    {
                        if (pkm.AbilityNumber == 4 ^ ((EncounterTrade) EncounterMatch).Ability == 4)
                            valid = false;
                    }
                    else if (EncounterType == typeof(EncounterLink))
                    {
                        if (pkm.AbilityNumber != ((EncounterLink)EncounterMatch).Ability)
                            valid = false;
                    }
                    if (!valid)
                    {
                        AddLine(Severity.Invalid, V108, CheckIdentifier.Ability);
                        return;
                    }
                }
                if (pkm.GenNumber == 5)
                {
                    if (EncounterType == typeof(EncounterSlot[]))
                    {
                        bool grotto = ((EncounterSlot[])EncounterMatch).All(slot => slot.Type == SlotType.HiddenGrotto); //encounter only at HiddenGrotto
                        if (pkm.AbilityNumber == 4 ^ grotto)
                        {
                            AddLine(Severity.Invalid, grotto ? V217 : V108, CheckIdentifier.Ability);
                            return;
                        }
                    }
                }
                if (pkm.GenNumber == 6)
                {
                    if (EncounterIsMysteryGift)
                    {
                        var wc = EncounterMatch as WC6;
                        var type = wc?.AbilityType;
                        int abilNumber = pkm.AbilityNumber;
                        if (type < 3 && abilNumber != 1 << type) // set number
                        {
                            if (type < 2 && abilNumber < 3 && abilities[0] != abilities[1]) // 0/1 required, not hidden, and ability can be changed
                                AddLine(Severity.Valid, V109, CheckIdentifier.Ability);
                            else
                                AddLine(Severity.Invalid, V110, CheckIdentifier.Ability);
                        }
                        else if (type == 3 && abilNumber == 4) // 1/2 only
                            AddLine(Severity.Invalid, V110, CheckIdentifier.Ability);
                    }
                    if (EncounterType == typeof(EncounterSlot[]) && pkm.AbilityNumber == 4)
                    {
                        var slots = (EncounterSlot[])EncounterMatch;
                        bool valid = slots.Any(slot => slot.DexNav ||
                            slot.Type == SlotType.FriendSafari ||
                            slot.Type == SlotType.Horde);

                        if (!valid)
                        {
                            AddLine(Severity.Invalid, V300, CheckIdentifier.Ability);
                            return;
                        }
                    }
                }
                if (pkm.GenNumber == 7)
                {
                    if (EncounterIsMysteryGift)
                    {
                        var wc = EncounterMatch as WC7;
                        var type = wc?.AbilityType;
                        int abilNumber = pkm.AbilityNumber;
                        if (type < 3 && abilNumber != 1 << type) // set number
                        {
                            if (type < 2 && abilNumber < 3 && abilities[0] != abilities[1]) // 0/1 required, not hidden, and ability can be changed
                                AddLine(Severity.Valid, V109, CheckIdentifier.Ability);
                            else
                                AddLine(Severity.Invalid, V110, CheckIdentifier.Ability);
                        }
                        else if (type == 3 && abilNumber == 4) // 1/2 only
                            AddLine(Severity.Invalid, V110, CheckIdentifier.Ability);
                    }
                    if (EncounterType == typeof(EncounterSlot[]) && pkm.AbilityNumber == 4)
                    {
                        var slots = (EncounterSlot[])EncounterMatch;
                        bool valid = slots.Any(slot => slot.Type == SlotType.SOS);

                        if (!valid)
                        {
                            AddLine(Severity.Invalid, V111, CheckIdentifier.Ability);
                            return;
                        }
                    }
                    if (Legal.Ban_NoHidden7.Contains(pkm.Species) && pkm.AbilityNumber == 4)
                    {
                        AddLine(Severity.Invalid, V112, CheckIdentifier.Ability);
                        return;
                    }

                }
            }

            if (3 <= pkm.Format && pkm.Format <= 5) // 3-5
            {
                if (pkm.Version != (int) GameVersion.CXD && abilities[0] != abilities[1] && pkm.AbilityNumber != 1 << abilval)
                {
                    AddLine(Severity.Invalid, V113, CheckIdentifier.Ability);
                    return;
                }
            }
            else
            {
                if (abilities[pkm.AbilityNumber >> 1] != pkm.Ability)
                {
                    AddLine(Severity.Invalid, V114, CheckIdentifier.Ability);
                    return;
                }
            }

            AddLine(Severity.Valid, V115, CheckIdentifier.Ability);
        }

        private void verifyBallEquals(params int[] balls)
        {
            int ball = pkm.Ball;
            if (balls.Any(b => b == ball))
                AddLine(Severity.Valid, V119, CheckIdentifier.Ball);
            else
                AddLine(Severity.Invalid, V118, CheckIdentifier.Ball);
        }
        private void verifyBall()
        {
            if (pkm.Format < 3)
                return; // no ball info saved

            if (!Encounter.Valid)
                return;

            if (EncounterIsMysteryGift)
            {
                if (pkm.Species == 490 && ((MysteryGift)EncounterMatch).Ball == 0)
                    // there is no ball data in Manaphy Mystery Gift
                    verifyBallEquals(4); // Pokeball
                else
                    verifyBallEquals(((MysteryGift)EncounterMatch).Ball);
                return;
            }
            if (EncounterType == typeof (EncounterLink))
            {
                verifyBallEquals(((EncounterLink)EncounterMatch).Ball);
                return;
            }
            if (EncounterType == typeof (EncounterTrade))
            {
                verifyBallEquals(4); // Pokeball
                return;
            }

            if (pkm.Ball == 0x14 && pkm.Gen7) // Heavy Ball
            {
                var lineage = Legal.getLineage(pkm);
                if (lineage.Any(e => Legal.AlolanCaptureNoHeavyBall.Contains(e)))
                {
                    AddLine(Severity.Invalid, V116, CheckIdentifier.Ball);
                    return;
                }
            }

            if (EncounterType == typeof(EncounterStatic))
            {
                EncounterStatic enc = EncounterMatch as EncounterStatic;
                if (enc?.Gift ?? false)
                    verifyBallEquals(enc.Ball);
                else if (pkm.Met_Location == 75 && pkm.Gen5) // DreamWorld
                    verifyBallEquals(Legal.DreamWorldBalls);
                else
                    verifyBallEquals(Legal.getWildBalls(pkm));
                return;
            }
            if (EncounterType == typeof (EncounterSlot[]))
            {
                EncounterSlot[] enc = EncounterMatch as EncounterSlot[];

                if (pkm.Met_Location == 30016 && pkm.Gen7) // Poké Pelago
                    verifyBallEquals(4); // Pokeball
                // For gen3/4 safari zones and BCC getValidWildEncounters already filter to not return
                // mixed possible encounters between safari, BCC and other encounters
                // That means is the first encounter is not safari then there is no safari encounter in the array
                else if (3 <= pkm.GenNumber && pkm.GenNumber <= 4 && Legal.IsSafariSlot(enc.First().Type))
                    verifyBallEquals(5); // Safari Ball
                else if (pkm.GenNumber == 4 && enc.First().Type == SlotType.BugContest)
                    verifyBallEquals(0x18); // Sport Ball
                else
                    verifyBallEquals(Legal.getWildBalls(pkm));
                return;
            }

            if (pkm.WasEgg)
            {
                verifyBallEgg();
                return;
            }

            verifyBallEquals(4); // Pokeball
        }
        private void verifyBallEgg()
        {
            if (pkm.GenNumber < 6) // No inheriting Balls
            {
                verifyBallEquals(4); // Must be Pokéball -- no ball inheritance.
                return;
            }

            if (pkm.Ball == 0x01) // Master Ball
            { AddLine(Severity.Invalid, V117, CheckIdentifier.Ball); return; }
            if (pkm.Ball == 0x10) // Cherish Ball
            { AddLine(Severity.Invalid, V120, CheckIdentifier.Ball); return; }
            if (pkm.Ball == 0x04) // Poké Ball
            { AddLine(Severity.Valid, V119, CheckIdentifier.Ball); return; }

            switch (pkm.GenNumber)
            {
                case 6: // Gen6 Inheritance Rules
                    verifyBallEggGen6();
                    return;
                case 7: // Gen7 Inheritance Rules
                    verifyBallEggGen7();
                    return;
            }
        }
        private void verifyBallEggGen6()
        {
            if (pkm.Gender == 2) // Genderless
            {
                verifyBallEquals(4); // Must be Pokéball as ball can only pass via mother (not Ditto!)
                return;
            }
            if (Legal.BreedMaleOnly.Contains(pkm.Species))
            {
                verifyBallEquals(4); // Must be Pokéball as ball can only pass via mother (not Ditto!)
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
                if (Legal.getLineage(pkm).All(e => !Legal.Inherit_Safari.Contains(e)))
                    AddLine(Severity.Invalid, V121, CheckIdentifier.Ball);
                else if (pkm.AbilityNumber == 4)
                    AddLine(Severity.Invalid, V122, CheckIdentifier.Ball);
                else
                    AddLine(Severity.Valid, V123, CheckIdentifier.Ball);

                return;
            }
            if (0x10 < ball && ball < 0x18) // Apricorn Ball
            {
                if (Legal.getLineage(pkm).All(e => !Legal.Inherit_Apricorn6.Contains(e)))
                    AddLine(Severity.Invalid, V121, CheckIdentifier.Ball);
                if (pkm.AbilityNumber == 4)
                    AddLine(Severity.Invalid, V122, CheckIdentifier.Ball);
                else
                    AddLine(Severity.Valid, V123, CheckIdentifier.Ball);

                return;
            }
            if (ball == 0x18) // Sport Ball
            {
                if (Legal.getLineage(pkm).All(e => !Legal.Inherit_Sport.Contains(e)))
                    AddLine(Severity.Invalid, V121, CheckIdentifier.Ball);
                else if (pkm.AbilityNumber == 4)
                    AddLine(Severity.Invalid, V122, CheckIdentifier.Ball);
                else
                    AddLine(Severity.Valid, V123, CheckIdentifier.Ball);

                return;
            }
            if (ball == 0x19) // Dream Ball
            {
                if (Legal.getLineage(pkm).Any(e => Legal.Inherit_Dream.Contains(e)))
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
                else if (pkm.AbilityNumber == 4 && 152 <= pkm.Species && pkm.Species <= 160)
                    AddLine(Severity.Invalid, V122, CheckIdentifier.Ball);
                else
                    AddLine(Severity.Valid, V123, CheckIdentifier.Ball);

                return;
            }

            if (pkm.Species > 650 && pkm.Species != 700) // Sylveon
            {
                if (Legal.getWildBalls(pkm).Contains(pkm.Ball))
                    AddLine(Severity.Valid, V123, CheckIdentifier.Ball);
                else
                    AddLine(Severity.Invalid, V121, CheckIdentifier.Ball);
                return;
            }

            AddLine(Severity.Invalid, V125, CheckIdentifier.Ball);
        }
        private void verifyBallEggGen7()
        {
            var Lineage = Legal.getLineage(pkm).ToArray();
            if (722 <= pkm.Species && pkm.Species <= 730) // G7 Starters
            {
                verifyBallEquals(4);
                return;
            }

            int ball = pkm.Ball;
            if (ball == 0x05) // Safari Ball
            {
                if (Lineage.Any(e => Legal.Inherit_Safari.Contains(e) || Legal.Inherit_SafariMale.Contains(e)))
                    AddLine(Severity.Valid, V123, CheckIdentifier.Ball);
                else
                    AddLine(Severity.Invalid, V121, CheckIdentifier.Ball);

                return;
            }
            if (0x10 < ball && ball < 0x18) // Apricorn Ball
            {
                if (Lineage.Any(e => Legal.Inherit_Apricorn7.Contains(e))) // past gen
                    AddLine(Severity.Valid, V123, CheckIdentifier.Ball);
                else
                    AddLine(Severity.Invalid, V121, CheckIdentifier.Ball);

                return;
            }
            if (ball == 0x18) // Sport Ball
            {
                if (Lineage.Any(e => Legal.Inherit_Sport.Contains(e)))
                    AddLine(Severity.Valid, V123, CheckIdentifier.Ball);
                else
                    AddLine(Severity.Invalid, V121, CheckIdentifier.Ball);

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
                if ((pkm.Species > 731 && pkm.Species <= 785) || Lineage.Any(e => Legal.PastGenAlolanNatives.Contains(e)))
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
                verifyBallEquals(Legal.getWildBalls(pkm));
                return;
            }

            if (ball >= 27)
            {
                AddLine(Severity.Invalid, V126, CheckIdentifier.Ball);
                return;
            }
            AddLine(Severity.Invalid, V125, CheckIdentifier.Ball);
        }

        private CheckResult verifyHistory()
        {
            if (!Encounter.Valid)
                return new CheckResult(Severity.Valid, V127, CheckIdentifier.History);

            if (pkm.GenNumber < 6)
            {
                if (pkm.Format < 6)
                    return new CheckResult(Severity.Valid, V128, CheckIdentifier.History);

                if (pkm.OT_Affection > 0)
                    return new CheckResult(Severity.Invalid, V129, CheckIdentifier.History);
                if (pkm.OT_Memory > 0 || pkm.OT_Feeling > 0 || pkm.OT_Intensity > 0 || pkm.OT_TextVar > 0)
                    return new CheckResult(Severity.Invalid, V130, CheckIdentifier.History);
            }
            
            if (pkm.Format >= 6 && pkm.GenNumber != pkm.Format && pkm.CurrentHandler != 1)
                return new CheckResult(Severity.Invalid, V124, CheckIdentifier.History);

            if (pkm.HT_Gender > 1)
                return new CheckResult(Severity.Invalid, string.Format(V131, pkm.HT_Gender), CheckIdentifier.History);
            
            MysteryGift mg = EncounterMatch as MysteryGift;
            WC6 MatchedWC6 = EncounterMatch as WC6;
            WC7 MatchedWC7 = EncounterMatch as WC7;
            if (MatchedWC6?.OT.Length > 0) // Has Event OT -- null propagation yields false if MatchedWC6=null
            {
                if (pkm.OT_Friendship != PersonalTable.AO[MatchedWC6.Species].BaseFriendship)
                    return new CheckResult(Severity.Invalid, V132, CheckIdentifier.History);
                if (pkm.OT_Affection != 0)
                    return new CheckResult(Severity.Invalid, V133, CheckIdentifier.History);
                if (pkm.CurrentHandler != 1)
                    return new CheckResult(Severity.Invalid, V134, CheckIdentifier.History);
            }
            else if (MatchedWC7?.OT.Length > 0) // Has Event OT -- null propagation yields false if MatchedWC7=null
            {
                if (pkm.OT_Friendship != PersonalTable.SM[MatchedWC7.Species].BaseFriendship)
                    return new CheckResult(Severity.Invalid, V132, CheckIdentifier.History);
                if (pkm.OT_Affection != 0)
                    return new CheckResult(Severity.Invalid, V133, CheckIdentifier.History);
                if (pkm.CurrentHandler != 1)
                    return new CheckResult(Severity.Invalid, V134, CheckIdentifier.History);
            }
            else if (mg != null && mg.Format < 6 && pkm.Format >= 6)
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
            {
                if (pkm.VC1)
                {
                    var hasGeo = geo.Any(d => d != 0);

                    if (!hasGeo)
                        return new CheckResult(Severity.Invalid, V137, CheckIdentifier.History);
                }
                
                if (pkm.GenNumber >= 7 && pkm.CNTs.Any(stat => stat > 0))
                    return new CheckResult(Severity.Invalid, V138, CheckIdentifier.History);
                
                if (!pkm.WasEvent && pkm.HT_Name.Length == 0) // Is not Traded
                {
                    if (pkm.CurrentHandler != 0) // Badly edited; PKHeX doesn't trip this.
                        return new CheckResult(Severity.Invalid, V139, CheckIdentifier.History);
                    if (pkm.HT_Friendship != 0)
                        return new CheckResult(Severity.Invalid, V140, CheckIdentifier.History);
                    if (pkm.HT_Affection != 0)
                        return new CheckResult(Severity.Invalid, V141, CheckIdentifier.History);

                    // We know it is untraded (HT is empty), if it must be trade evolved flag it.
                    if (Legal.getHasTradeEvolved(pkm) // if evo chain requires a trade
                        && (EncounterMatch as EncounterSlot[])?.Any(slot => slot.Species == pkm.Species) != true // Wild Encounter
                        && (EncounterMatch as EncounterStatic)?.Species != pkm.Species) // Static Encounter
                    {
                        if (pkm.Species != 350) // Milotic
                            return new CheckResult(Severity.Invalid, V142, CheckIdentifier.History);
                        if (pkm.CNT_Beauty < 170) // Beauty Contest Stat Requirement
                            return new CheckResult(Severity.Invalid, V143, CheckIdentifier.History);
                        if (pkm.CurrentLevel == 1)
                            return new CheckResult(Severity.Invalid, V144, CheckIdentifier.History);
                    }
                }

                return new CheckResult(Severity.Valid, V145, CheckIdentifier.History);
            }

            // Determine if we should check for Handling Trainer Memories
            // A Pokémon is untraded if...
            bool untraded = pkm.HT_Name.Length == 0 || pkm.Geo1_Country == 0;
            if (EncounterIsMysteryGift)
            {
                untraded |= !pkm.WasEventEgg;
                untraded &= pkm.WasEgg;
            }

            if (pkm.WasLink && (EncounterMatch as EncounterLink)?.OT == false)
                untraded = false;
            else if (pkm.GenNumber < 6)
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

                // We know it is untraded (HT is empty), if it must be trade evolved flag it.
                if (Legal.getHasTradeEvolved(pkm) && (EncounterMatch as EncounterSlot[])?.Any(slot => slot.Species == pkm.Species) != true)
                {
                    if (pkm.Species != 350) // Milotic
                        return new CheckResult(Severity.Invalid, V142, CheckIdentifier.History);
                    if (pkm.CNT_Beauty < 170) // Beauty Contest Stat Requirement
                        return new CheckResult(Severity.Invalid, V143, CheckIdentifier.History);
                    if (pkm.CurrentLevel == 1)
                        return new CheckResult(Severity.Invalid, V144, CheckIdentifier.History);
                }
            }
            else // Is Traded
            {
                if (pkm.Format == 6 && pkm.HT_Memory == 0)
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
            else if (EncounterType != typeof(WC6))
            {
                if (pkm.OT_Memory == 0 ^ !pkm.Gen6)
                    return new CheckResult(Severity.Invalid, V152, CheckIdentifier.History);
                if (pkm.GenNumber < 6 && pkm.OT_Affection != 0)
                    return new CheckResult(Severity.Invalid, V129, CheckIdentifier.History);
            }
            // Unimplemented: Ingame Trade Memories

            return new CheckResult(Severity.Valid, V145, CheckIdentifier.History);
        }
        private CheckResult verifyCommonMemory(int handler)
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
            if (matchingMoveMemory != -1 && pkm.Species != 235 && !Legal.getCanLearnMachineMove(pkm, Legal.MoveSpecificMemories[1][matchingMoveMemory], 6))
                return new CheckResult(Severity.Invalid, string.Format(V153, resultPrefix), CheckIdentifier.Memory);

            if (m == 6 && !Legal.LocationsWithPKCenter[0].Contains(t))
                return new CheckResult(Severity.Invalid, string.Format(V154, resultPrefix), CheckIdentifier.Memory);

            if (m == 21) // {0} saw {2} carrying {1} on its back. {4} that {3}.
                if (!Legal.getCanLearnMachineMove(new PK6 {Species = t, EXP = PKX.getEXP(100, t)}, 19, 6))
                    return new CheckResult(Severity.Invalid, string.Format(V153, resultPrefix), CheckIdentifier.Memory);

            if ((m == 16 || m == 48) && (t == 0 || !Legal.getCanKnowMove(pkm, t, 6)))
                return new CheckResult(Severity.Invalid, string.Format(V153, resultPrefix), CheckIdentifier.Memory);

            if (m == 49 && (t == 0 || !Legal.getCanRelearnMove(pkm, t, 6))) // {0} was able to remember {2} at {1}'s instruction. {4} that {3}.
                return new CheckResult(Severity.Invalid, string.Format(V153, resultPrefix), CheckIdentifier.Memory);

            return new CheckResult(Severity.Valid, string.Format(V155, resultPrefix), CheckIdentifier.Memory);
        }

        private void verifyOTMemoryIs(int[] values)
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
        private void verifyOTMemory()
        {
            if (pkm.Format < 6)
                return;

            if (!History.Valid)
                return;

            if (pkm.GenNumber < 6)
            {
                verifyOTMemoryIs(new [] {0,0,0,0}); // empty
                return;
            }

            if (EncounterType == typeof(EncounterTrade))
            {
                // Undocumented, uncommon, and insignificant -- don't bother.
                return;
            }
            if (EncounterType == typeof(WC6))
            {
                WC6 g = EncounterMatch as WC6;
                verifyOTMemoryIs(new[] {g.OT_Memory, g.OT_Intensity, g.OT_TextVar, g.OT_Feeling});
                return;
            }
            if (EncounterType == typeof(WC7))
            {
                WC7 g = EncounterMatch as WC7;
                verifyOTMemoryIs(new[] {g.OT_Memory, g.OT_Intensity, g.OT_TextVar, g.OT_Feeling});
                return;
            }
            if (pkm.GenNumber >= 7)
            {
                verifyOTMemoryIs(new[] {0, 0, 0, 0}); // empty
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
                    AddLine(verifyCommonMemory(0));
                    return;

                case 14:
                    if (!Legal.getCanBeCaptured(pkm.OT_TextVar, pkm.GenNumber, (GameVersion)pkm.Version))
                        AddLine(Severity.Invalid, string.Format(V165, V205), CheckIdentifier.Memory);
                    else
                        AddLine(Severity.Valid, string.Format(V164, V205), CheckIdentifier.Memory);
                    return;
            }
            if (pkm.XY && Legal.Memory_NotXY.Contains(pkm.OT_Memory))
                AddLine(Severity.Invalid, string.Format(V163, V205), CheckIdentifier.Memory);
            if (pkm.AO && Legal.Memory_NotAO.Contains(pkm.OT_Memory))
                AddLine(Severity.Invalid, string.Format(V163, V205), CheckIdentifier.Memory);

            AddLine(verifyCommonMemory(0));
        }
        private void verifyHTMemory()
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
                    if (Legal.getCanBeCaptured(pkm.HT_TextVar, pkm.GenNumber))
                        AddLine(Severity.Valid, string.Format(V164, V206), CheckIdentifier.Memory);
                    else
                        AddLine(Severity.Invalid, string.Format(V165, V206), CheckIdentifier.Memory);
                    return;
            }
            AddLine(verifyCommonMemory(1));
        }
        private void verifyRegion()
        {
            if (pkm.Format < 6)
                return;

            bool pass;
            switch (pkm.ConsoleRegion)
            {
                case 0: // Japan
                    pass = pkm.Country == 1;
                    break;
                case 1: // Americas
                    pass = 8 <= pkm.Country && pkm.Country <= 52 || new[] {153, 156, 168, 174, 186}.Contains(pkm.Country);
                    break;
                case 2: // Europe
                    pass = 64 <= pkm.Country && pkm.Country <= 127 || new[] {169, 184, 185}.Contains(pkm.Country);
                    break;
                case 4: // China
                    pass = pkm.Country == 144 || pkm.Country == 160;
                    break;
                case 5: // Korea
                    pass = pkm.Country == 136;
                    break;
                case 6: // Taiwan
                    pass = pkm.Country == 128;
                    break;
                default:
                    AddLine(new CheckResult(Severity.Invalid, V301, CheckIdentifier.Geography));
                    return;
            }

            if (!pass)
                AddLine(Severity.Invalid, V302, CheckIdentifier.Geography);
            else
                AddLine(Severity.Valid, V303, CheckIdentifier.Geography);
        }
        private void verifyForm()
        {
            if (!Encounter.Valid)
                return;

            if (pkm.Format < 4)
                return;

            if (pkm.AltForm > pkm.PersonalInfo.FormeCount)
            {
                bool valid = false;
                int species = pkm.Species;
                if (species == 201) // Unown
                {
                    int maxCount = pkm.GenNumber == 2 ? 26 : 28; // A-Z : A-Z?!
                    if (pkm.AltForm < maxCount)
                        valid = true;
                }
                if (species == 414 && pkm.AltForm < 3) // Wormadam base form kept
                    valid = true;

                if ((species == 664 || species == 665) && pkm.AltForm < 18) // Vivillon Pre-evolutions
                    valid = true;

                if (!valid) // ignore list
                { AddLine(Severity.Invalid, string.Format(V304, pkm.PersonalInfo.FormeCount, pkm.AltForm), CheckIdentifier.Form); return; }
            }

            switch (pkm.Species)
            {
                case 25: // Pikachu
                    if (pkm.Format == 6 && pkm.AltForm != 0 ^ EncounterType == typeof(EncounterStatic))
                    {
                        string msg = EncounterType == typeof (EncounterStatic) ? V305 : V306;
                        AddLine(Severity.Invalid, msg, CheckIdentifier.Form);
                        return;
                    }
                    if (pkm.Format == 7 && pkm.AltForm != 0 ^ EncounterIsMysteryGift)
                    {
                        var gift = EncounterMatch as WC7;
                        if (gift != null && gift.Form != pkm.AltForm)
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
                        if ((298 <= item && item <= 313) || item == 644)
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
                        int index = Array.IndexOf(pkm.Moves, 548); // Secret Sword
                        bool noSword = index < 0;
                        if (pkm.AltForm == 0 ^ noSword) // mismatch
                            vMoves[noSword ? 0 : index] = new CheckResult(Severity.Invalid, V169, CheckIdentifier.Move);
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
                        if (!EncounterIsMysteryGift)
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
                        if (!EncounterIsMysteryGift)
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
                    if (pkm.AltForm != 0 && pkm.Box > -1) // has form but stored in box
                    {
                        AddLine(Severity.Invalid, V316, CheckIdentifier.Form);
                        return;
                    }
                    break;
            }

            if (pkm.Format >= 7 && pkm.GenNumber < 7 && pkm.AltForm != 0)
            {
                if (pkm.Species == 25 || Legal.AlolanOriginForms.Contains(pkm.Species))
                { AddLine(Severity.Invalid, V317, CheckIdentifier.Form); return; }
            }
            if (pkm.AltForm > 0 && new[] {Legal.BattleForms, Legal.BattleMegas, Legal.BattlePrimals}.Any(arr => arr.Contains(pkm.Species)))
            { AddLine(Severity.Invalid, V310, CheckIdentifier.Form); return; }

            AddLine(Severity.Valid, V318, CheckIdentifier.Form);
        }
        private void verifyMisc()
        {
            if (pkm.Format == 7 && ((PK7)pkm).PelagoEventStatus != 0)
            {
                // TODO: Figure out what PelagoEventStati are legal.
            }

            if (pkm.IsEgg)
            {
                if (new[] {pkm.Move1_PPUps, pkm.Move2_PPUps, pkm.Move3_PPUps, pkm.Move4_PPUps}.Any(ppup => ppup > 0))
                { AddLine(Severity.Invalid, V319, CheckIdentifier.Misc); return; }
                if (pkm.CNTs.Any(stat => stat > 0))
                { AddLine(Severity.Invalid, V320, CheckIdentifier.Misc); return; }
            }

            if (Encounter.Valid)
            {
                if (EncounterIsMysteryGift)
                {
                    if (pkm.FatefulEncounter)
                        AddLine(Severity.Valid, V321, CheckIdentifier.Fateful);
                    else
                        AddLine(Severity.Invalid, V322, CheckIdentifier.Fateful);
                    return;
                }
                if (EncounterType == typeof (EncounterStatic))
                {
                    var enc = EncounterMatch as EncounterStatic;
                    if (enc.Fateful)
                    {
                        if (pkm.FatefulEncounter)
                            AddLine(Severity.Valid, V323, CheckIdentifier.Fateful);
                        else
                            AddLine(Severity.Invalid, V324, CheckIdentifier.Fateful);
                    }
                    else if (pkm.FatefulEncounter)
                        AddLine(Severity.Invalid, V325, CheckIdentifier.Fateful);
                    return;
                }
                if (pkm.FatefulEncounter)
                    AddLine(Severity.Invalid, V325, CheckIdentifier.Fateful);
                
                if (pkm.Format == 5)
                {
                    var enc = EncounterMatch as EncounterStatic;
                    bool req = enc?.NSparkle ?? false;
                    bool has = ((PK5) pkm).NPokémon;
                    if (req && !has)
                        AddLine(Severity.Invalid, V326, CheckIdentifier.Fateful);
                    if (!req && has)
                        AddLine(Severity.Invalid, V327, CheckIdentifier.Fateful);
                }
            }
        }
        private void verifyVersionEvolution()
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
                        if (EncounterIsMysteryGift && (EncounterMatch as MysteryGift).Species == pkm.Species) // Gifted via Mystery Gift
                            break;
                        AddLine(Severity.Invalid, V328, CheckIdentifier.Evolution);
                    }
                    break;
                case 792: // Lunala
                    if (pkm.Version == 30 && pkm.IsUntraded)
                    {
                        if (EncounterIsMysteryGift && (EncounterMatch as MysteryGift).Species == pkm.Species) // Gifted via Mystery Gift
                            break;
                        AddLine(Severity.Invalid, V328, CheckIdentifier.Evolution);
                    }
                    break;
            }
        }

        private CheckResult[] verifyMoves(GameVersion game = GameVersion.Any)
        {
            var validLevelMoves = Legal.getValidMovesAllGens(pkm, EvoChainsAllGens, Tutor: false, Machine: false, RemoveTransferHM:false);
            var validTMHM = Legal.getValidMovesAllGens(pkm, EvoChainsAllGens, LVL: false, Tutor: false, MoveReminder: false, RemoveTransferHM: false);
            var validTutor = Legal.getValidMovesAllGens(pkm, EvoChainsAllGens, LVL: false, Machine: false, MoveReminder: false, RemoveTransferHM: false);
            Legal.RemoveFutureMoves(pkm, EvoChainsAllGens, ref validLevelMoves, ref validTMHM, ref validTutor);
            CheckResult[] res;
            int[] Moves = pkm.Moves;
            if (!pkm.IsEgg && pkm.Species == 235) // Smeargle can have any move except a few
                res = parseMovesSketch(Moves);
            else if (pkm.GenNumber < 6)
                res = parseMovesPre3DS(game, validLevelMoves, validTMHM, validTutor, Moves);
            else if (EventGiftMatch?.Count > 1) // Multiple possible Mystery Gifts matched, get the best match too
                res = parseMovesGetGift(Moves, validLevelMoves, validTMHM, validTutor);
            else if (pkm.WasEgg && Legal.SplitBreed.Contains(pkm.Species))
                res = parseMovesRelearnSplitBreed(Moves, validLevelMoves, validTMHM, validTutor, game);
            else // Everything else
                res = parseMovesRelearn(Moves, validLevelMoves, validTMHM, validTutor, 0, game);

            // Duplicate Moves Check
            verifyNoEmptyDuplicates(Moves, res);
            if (Moves[0] == 0) // Can't have an empty moveslot for the first move.
                res[0] = new CheckResult(Severity.Invalid, V167, CheckIdentifier.Move);

            return res;
        }
        private GameVersion[] getBaseMovesIsEggGames()
        {
            GameVersion[] Games = { };
            switch (pkm.GenNumber)
            {
                case 1:
                case 2:
                    Games = new[] { GameVersion.GS, GameVersion.C };
                    break;
                case 3:
                    switch ((GameVersion)pkm.Version)
                    {
                        case GameVersion.R:
                        case GameVersion.S:
                            Games = new[] { GameVersion.RS};
                            break;
                        case GameVersion.E:
                            Games = new[] { GameVersion.E };
                            break;
                        case GameVersion.FR:
                        case GameVersion.LG:
                            Games = new[] { GameVersion.FRLG };
                            break;
                    }
                    break;
                case 4:
                    switch ((GameVersion)pkm.Version)
                    {
                        case GameVersion.D:
                        case GameVersion.P:
                            Games = new[] { GameVersion.DP };
                            break;
                        case GameVersion.Pt:
                            Games = new[] { GameVersion.Pt };
                            break;
                        case GameVersion.HG:
                        case GameVersion.SS:
                            Games = new[] { GameVersion.HGSS };
                            break;
                    }
                    break;
                case 5:
                    switch ((GameVersion)pkm.Version)
                    {
                        case GameVersion.B:
                        case GameVersion.W:
                            Games = new[] { GameVersion.BW };
                            break;
                        case GameVersion.Pt:
                            Games = new[] { GameVersion.Pt };
                            break;
                        case GameVersion.B2:
                        case GameVersion.W2:
                            Games = new[] { GameVersion.B2W2 };
                            break;
                    }
                    break;
            }
            return Games;
        }
        private CheckResult[] parseMovesIsEggPreRelearn(int[] Moves, int[] SpecialMoves, bool allowinherited)
        {
            CheckResult[] res = new CheckResult[4];
            var ValidSpecialMoves = SpecialMoves.Where(m => m != 0).ToList();
            // Some games can have different egg movepools. Have to check all situations.
            GameVersion[] Games = getBaseMovesIsEggGames();
            int splitctr = Legal.getSplitBreedGeneration(pkm).Contains(pkm.Species) ? 1 : 0;
            foreach (var ver in Games)
            {
                for (int i = 0; i <= splitctr; i++)
                {
                    var baseEggMoves = Legal.getBaseEggMoves(pkm, i, ver, pkm.GenNumber < 4 ? 5 : 1)?.ToList() ?? new List<int>();
                    var InheritedLvlMoves = Legal.getBaseEggMoves(pkm, i, ver, 100)?.ToList() ?? new List<int>();
                    var EggMoves = Legal.getEggMoves(pkm, i, ver)?.ToList() ?? new List<int>();
                    var InheritedTutorMoves = ver == GameVersion.C ? Legal.getTutorMoves(pkm, pkm.Species, pkm.AltForm, false, 2)?.ToList() : new List<int>();
                    // Only TM Hm moves from the source game of the egg, not any other games from the same generation
                    var InheritedTMHMMoves = Legal.getTMHM(pkm, pkm.Species, pkm.AltForm, pkm.GenNumber, ver, false)?.ToList();
                    InheritedLvlMoves.RemoveAll(x => baseEggMoves.Contains(x));

                    if (pkm.Format > 2 || SpecialMoves.Any()) 
                    {
                        // For gen 2 is not possible to difference normal eggs from event eggs
                        // If there is no special moves assume normal egg
                        res = verifyPreRelearnEggBase(Moves, baseEggMoves, EggMoves, InheritedLvlMoves, InheritedTMHMMoves, InheritedTutorMoves, ValidSpecialMoves, allowinherited, ver);
                        if (res.All(r => r.Valid)) // moves is satisfactory
                            return res;
                    }
                    if (pkm.Format != 2)
                        continue;
                    
                    // For gen 2 if does not match special egg check for normal egg too
                    res = verifyPreRelearnEggBase(Moves, baseEggMoves, EggMoves, InheritedLvlMoves, InheritedTMHMMoves, InheritedTutorMoves, new List<int>(), true, ver);
                    if (res.All(r => r.Valid)) // moves are satisfactory
                        return res;
                }
            }
            return res;
        }
        private CheckResult[] parseMovesWasEggPreRelearn(int[] Moves, List<int>[] validLevelMoves, List<int>[] validTMHM, List<int>[] validTutor)
        {
            CheckResult[] res = new CheckResult[4];

            // Gen 3 could have an egg origin and a non-egg origin, check first non-egg origin
            if (pkm.GenNumber == 3 && !pkm.HasOriginalMetLocation && EncounterMatch !=null)
            {
                res = EventGiftMatch?.Count > 1 
                    ? parseMovesGetGift(Moves, validLevelMoves, validTMHM, validTutor) // Multiple possible Mystery Gifts matched, get the best match too
                    : parseMovesPreRelearnEncounter(Moves, validLevelMoves, validTMHM, validTutor, GameVersion.Any); // Everything else, non-egg encounters only
                if (res.All(r => r.Valid)) // moves are satisfactory
                    return res;
                // If non-egg encounter is not valid check egg-encounter with eggmoves and without special moves
            }

            // Some games can have different egg movepools. Have to check all situations.
            GameVersion[] Games = { };
            switch (pkm.GenNumber)
            {
                case 1:
                case 2:
                    Games = new[] { GameVersion.GS, GameVersion.C };
                    break;
                case 3: // Generation 3 does not overwrite source game after pokemon hatched
                    Games = getBaseMovesIsEggGames();
                    break;
                case 4:
                    Games = new[] { GameVersion.DP, GameVersion.Pt, GameVersion.HGSS };
                    break;
                case 5:
                    Games = new[] { GameVersion.BW, GameVersion.B2W2 };
                    break;
            }
            int splitctr = Legal.SplitBreed.Contains(pkm.Species) ? 1 : 0;
            foreach (var ver in Games)
            {
                var EventEggMoves = pkm.WasEgg && !pkm.WasGiftEgg ? Legal.getSpecialEggMoves(pkm, ver).ToArray() : new int[0];
                for (int i = 0; i <= splitctr; i++)
                {
                    var baseEggMoves = Legal.getBaseEggMoves(pkm, i, ver, 100)?.ToArray() ?? new int[0];
                    var EggMoves = pkm.WasEgg && !pkm.WasGiftEgg ? Legal.getEggMoves(pkm, i, ver).ToArray() : new int[0];

                    res = parseMoves(Moves, validLevelMoves, new int[0], validTMHM, validTutor, new int[0], baseEggMoves, EggMoves, EventEggMoves);

                    if (res.All(r => r.Valid)) // moves is satisfactory
                        return res;
                }
            }
            return res;
        }
        private CheckResult[] parseMovesIsEggPreRelearnEvent(int[] Moves)
        {
            foreach (MysteryGift mg in EventGiftMatch)
            {
                int[] SpecialMoves = mg.Moves;
                CheckResult[] res = parseMovesIsEggPreRelearn(Moves, SpecialMoves, false);
                if (res.Any(r => !r.Valid))
                    continue;

                // Match Found
                EncounterMatch = mg;
                return res;
            }
            // no Mystery Gifts matched
            return parseMovesIsEggPreRelearn(Moves, new int[0], false);
        }
        private CheckResult[] parseMovesSketch(int[] Moves)
        {
            CheckResult[] res = new CheckResult[4];
            for (int i = 0; i < 4; i++)
                res[i] = Legal.InvalidSketch.Contains(Moves[i])
                    ? new CheckResult(Severity.Invalid, V166, CheckIdentifier.Move)
                    : new CheckResult(CheckIdentifier.Move);
            return res;
        }
        private CheckResult[] parseMovesPre3DS(GameVersion game, List<int>[] validLevelMoves, List<int>[] validTMHM, List<int>[] validTutor, int[] Moves)
        {
            if (pkm.IsEgg)
            {
                if (EventGiftMatch?.Count > 0)
                    return parseMovesIsEggPreRelearnEvent(Moves);

                int[] SpecialMoves = (EncounterMatch as MysteryGift)?.Moves ??
                                   (EncounterMatch as EncounterStatic)?.Moves ??
                                   (EncounterMatch as EncounterTrade)?.Moves;
                var allowinherited = SpecialMoves == null && !pkm.WasGiftEgg && !pkm.WasEventEgg;
                return parseMovesIsEggPreRelearn(Moves, SpecialMoves ?? new int[0], allowinherited);
            }
            if (pkm.WasEgg)
                return parseMovesWasEggPreRelearn(Moves, validLevelMoves, validTMHM, validTutor);

            if (EventGiftMatch?.Count > 1)
                // Multiple possible non-egg Mystery Gifts matched, get the best match too
                return parseMovesGetGift(Moves, validLevelMoves, validTMHM, validTutor);

            return parseMovesPreRelearnEncounter(Moves, validLevelMoves, validTMHM, validTutor, game);
        }
        private CheckResult[] parseMovesGetGift(int[] Moves, List<int>[] validLevelMoves, List<int>[] validTMHM, List<int>[] validTutor)
        {
            int[] RelearnMoves = pkm.RelearnMoves;
            foreach (MysteryGift mg in EventGiftMatch)
            {
                int[] SpecialMoves = mg.Moves;
                CheckResult[] res = parseMoves(Moves, validLevelMoves, RelearnMoves, validTMHM, validTutor, SpecialMoves, new int[0], new int[0], new int[0]);
                if (res.Any(r => !r.Valid))
                    continue;

                // Match Found
                EncounterMatch = mg;
                RelearnBase = mg.RelearnMoves;
                return res;
            }

            // no Mystery Gifts matched
            return parseMoves(Moves, validLevelMoves, RelearnMoves, validTMHM, validTutor, new int[0], new int[0], new int[0], new int[0]);
        }
        private CheckResult[] parseMovesPreRelearnEncounter(int[] Moves, List<int>[] validLevelMoves, List<int>[] validTMHM, List<int>[] validTutor, GameVersion game)
        {
            int[] SpecialMoves = (EncounterMatch as MysteryGift)?.Moves ??
                                 (EncounterMatch as EncounterStatic)?.Moves ??
                                 (EncounterMatch as EncounterTrade)?.Moves ??
                                 new int[0];

            return parseMoves(Moves, validLevelMoves, new int[0], validTMHM, validTutor, SpecialMoves, new int[0], new int[0], new int[0]);
        }
        private CheckResult[] parseMovesRelearnSplitBreed(int[] Moves, List<int>[] validLevelMoves, List<int>[] validTMHM, List<int>[] validTutor, GameVersion game)
        {
            CheckResult[] res = new CheckResult[4];
            int splitctr = Legal.SplitBreed.Contains(pkm.Species) ? 1 : 0;
        
            for (int i = 0; i <= splitctr; i++)
            {
                res = parseMovesRelearn(Moves, validLevelMoves, validTMHM, validTutor, i, game);
                if (res.All(r => r.Valid)) // moves are satisfactory
                    break;
            }
           
            return res;
        }
        private CheckResult[] parseMovesRelearn(int[] Moves, List<int>[] validLevelMoves, List<int>[] validTMHM, List<int>[] validTutor, int SkipOption, GameVersion game)
        {
            int[] EggMoves = pkm.WasEgg && !pkm.WasGiftEgg? Legal.getEggMoves(pkm, SkipOption, game).ToArray() : new int[0];
            int[] EventEggMoves =  pkm.WasEgg && !pkm.WasGiftEgg ? Legal.getSpecialEggMoves(pkm, game).ToArray() : new int[0];
            int[] RelearnMoves = pkm.RelearnMoves;
            int[] SpecialMoves = (EncounterMatch as MysteryGift)?.Moves ??
                                 (EncounterMatch as EncounterStatic)?.Moves ??
                                 (EncounterMatch as EncounterTrade)?.Moves ??
                                 new int[0];

            CheckResult[] res = parseMoves(Moves, validLevelMoves, RelearnMoves, validTMHM, validTutor, SpecialMoves, new int[0], EggMoves, EventEggMoves);

            for (int i = 0; i < 4; i++)
                if ((pkm.IsEgg || res[i].Flag) && !RelearnMoves.Contains(Moves[i]))
                    res[i] = new CheckResult(Severity.Invalid, string.Format(V170, res[i].Comment), res[i].Identifier);

            return res;
        }
        private CheckResult[] parseMoves(int[] moves, List<int>[] learn, int[] relearn, List<int>[] tmhm, List<int>[] tutor, int[] special, int[] baseegg, int[] egg, int[] eventegg)
        {
            CheckResult[] res = new CheckResult[4];
            var Gen1MovesLearned = new List<int>();
            var EggMovesLearned = new List<int>();
            var BaseEggMovesLearned = new List<int>();
            var EventEggMovesLearned = new List<int>();
            var IsGen2Pkm = pkm.Format == 2 || pkm.VC2;
            // Check none moves and relearn moves before generation moves
            for (int m = 0; m < 4; m++)
            {
                if (moves[m] == 0)
                    res[m] = new CheckResult(Severity.Valid, V167, CheckIdentifier.Move);
                else if (relearn.Contains(moves[m]))
                    res[m] = new CheckResult(Severity.Valid, V172, CheckIdentifier.Move) { Flag = true };
            }

            if (res.All(r => r != null))
                return res;

            bool MixedGen1NonTradebackGen2 = false;
            // Check moves going backwards, marking the move valid in the most current generation when it can be learned
            int[] generations = getGenMovesCheckOrder(pkm);
            foreach (var gen in generations)
            {
                if (!pkm.InhabitedGeneration(gen))
                    continue;

                var HMLearned = new int[0];
                // Check if pokemon knows HM moves from generation 3 and 4 but are not valid yet, that means it cant learn the HMs in future generations
                bool KnowDefogWhirlpool = false;
                if (gen == 4 && pkm.Format > 4)
                {
                    // Copy to array the hm found or else the list will be emptied when the legal status of moves changes in the current generation
                    HMLearned = moves.Where((m,i) => !(res[i]?.Valid ?? false) && Legal.HM_4_RemovePokeTransfer.Any(l => l == m)).Select((m, i) => i).ToArray();
                    // Defog and Whirlpool at the same time, also both can't be learned in future generations or else they will be valid
                    KnowDefogWhirlpool = moves.Where((m, i) => (m == 250 || m == 432) && !(res[i]?.Valid ?? false)).Count() == 2;
                }
                else if (gen == 3 && pkm.Format > 3)
                    HMLearned = moves.Select((m, i) => i).Where(i => !(res[i]?.Valid ?? false) && Legal.HM_3.Any(l => l == moves[i])).ToArray();

                bool native = gen == pkm.Format;
                for (int m = 0; m < 4; m++)
                {
                    if (res[m]?.Valid ?? false)
                        continue;

                    if (learn[gen].Contains(moves[m]))
                        res[m] = new CheckResult(Severity.Valid, native ? V177 : string.Format(V330, gen), CheckIdentifier.Move);
                    else if (tmhm[gen].Contains(moves[m]))
                        res[m] = new CheckResult(Severity.Valid, native ? V173 : string.Format(V331, gen), CheckIdentifier.Move);
                    else if (tutor[gen].Contains(moves[m]))
                        res[m] = new CheckResult(Severity.Valid, native ? V173 : string.Format(V332, gen), CheckIdentifier.Move);
                    else if (gen == pkm.GenNumber && special.Contains(moves[m]))
                        res[m] = new CheckResult(Severity.Valid, V175, CheckIdentifier.Move);

                    if (res[m] == null)
                        continue;
                    if (res[m].Valid && gen == 1)
                        Gen1MovesLearned.Add(m);
                }

                if (gen == generations.Last())
                {
                    // Check base egg moves after all the moves but just before egg moves to different it from normal level up moves
                    // Also check if the base egg moves is a non tradeback move
                    for (int m = 0; m < 4; m++)
                    {
                        if (!baseegg.Contains(moves[m]))
                            continue;

                        if (IsGen2Pkm && Gen1MovesLearned.Any() && moves[m] > Legal.MaxMoveID_1)
                        {
                            res[m] = new CheckResult(Severity.Invalid, V334, CheckIdentifier.Move);
                            MixedGen1NonTradebackGen2 = true;
                        }
                        else
                            res[m] = new CheckResult(Severity.Valid, V345, CheckIdentifier.Move);
                        BaseEggMovesLearned.Add(m);
                    }

                    // Check egg moves after all the generations and all the moves, every move that can't be learned in another source should have preference
                    // the moves that can only be learned from egg moves should in the future check if the move combinations can be breed in gens 2 to 5
                    for (int m = 0; m < 4; m++)
                    {
                        if (res[m]?.Valid ?? false)
                            continue;
                        if (egg.Contains(moves[m]))
                        {
                            if (IsGen2Pkm && Gen1MovesLearned.Any() && moves[m] > Legal.MaxMoveID_1)
                            {
                                // To learn exclusive generation 1 moves the pokemon was tradeback, but it can't be trade to generation 1
                                // without removing moves above MaxMoveID_1, egg moves above MaxMoveID_1 and gen 1 moves are incompatible
                                res[m] = new CheckResult(Severity.Invalid, V334, CheckIdentifier.Move) { Flag = true };
                                MixedGen1NonTradebackGen2 = true;
                            }
                            else
                                res[m] = new CheckResult(Severity.Valid, V171, CheckIdentifier.Move) { Flag = true };
                            EggMovesLearned.Add(m);
                        }
                        if (!eventegg.Contains(moves[m]))
                            continue;

                        if (!egg.Contains(moves[m]))
                        {
                            if (IsGen2Pkm && Gen1MovesLearned.Any() && moves[m] > Legal.MaxMoveID_1)
                            {
                                res[m] = new CheckResult(Severity.Invalid, V334, CheckIdentifier.Move) { Flag = true };
                                MixedGen1NonTradebackGen2 = true;
                            }
                            else
                                res[m] = new CheckResult(Severity.Valid, V333, CheckIdentifier.Move) { Flag = true };
                        }
                        EventEggMovesLearned.Add(m);
                    }

                    // A pokemon could have normal egg moves and regular egg moves
                    // Only if all regular egg moves are event egg moves or all event egg moves are regular egg moves
                    var RegularEggMovesLearned = EggMovesLearned.Union(BaseEggMovesLearned).ToList();
                    if (RegularEggMovesLearned.Any() && EventEggMovesLearned.Any())
                    {
                        // Moves that are egg moves or event egg moves but not both
                        var IncompatibleEggMoves = RegularEggMovesLearned.Except(EventEggMovesLearned).Union(EventEggMovesLearned.Except(RegularEggMovesLearned)).ToList();
                        if (IncompatibleEggMoves.Any())
                        {
                            foreach(int m in IncompatibleEggMoves)
                            {
                                if (EventEggMovesLearned.Contains(m) && !EggMovesLearned.Contains(m))
                                    res[m] = new CheckResult(Severity.Invalid, V337, CheckIdentifier.Move);
                                else if (!EventEggMovesLearned.Contains(m) && EggMovesLearned.Contains(m))
                                    res[m] = new CheckResult(Severity.Invalid, V336, CheckIdentifier.Move);
                                else if (!EventEggMovesLearned.Contains(m) && BaseEggMovesLearned.Contains(m))
                                    res[m] = new CheckResult(Severity.Invalid, V358, CheckIdentifier.Move);
                            }
                        }
                    }
                }
                
                if (3 <= gen && gen <= 4 && pkm.Format > gen)
                {
                    // After all the moves from the generations 3 and 4, 
                    // including egg moves if is the origin generation because some hidden moves are also special egg moves in gen 3
                    // Check if the marked hidden moves that were invalid at the start are now marked as valid, that means 
                    // the hidden move was learned in gen 3 or 4 but was not removed when transfer to 4 or 5
                    if (KnowDefogWhirlpool)
                    {
                        int invalidCount = moves.Where((m, i) => (m == 250 || m == 432) && (res[i]?.Valid ?? false)).Count();
                        if (invalidCount == 2) // can't know both at the same time
                            for (int i = 0; i < 4; i++) // flag both moves
                                if (moves[i] == 250 || moves[i] == 432)
                                    res[i] = new CheckResult(Severity.Invalid, V338, CheckIdentifier.Move);
                    }
                    
                    for (int i = 0; i < HMLearned.Length; i++)
                        if (res[i]?.Valid ?? false)
                            res[i] = new CheckResult(Severity.Invalid, string.Format(V339, gen, gen + 1), CheckIdentifier.Move);
                }

                // Mark the gen 1 exclusive moves as illegal because the pokemon also have Non tradeback egg moves.
                if (MixedGen1NonTradebackGen2)
                    foreach (int m in Gen1MovesLearned)
                        res[m] = new CheckResult(Severity.Invalid, V335, CheckIdentifier.Move);

                if (res.All(r => r != null))
                    return res;
            }

            if (pkm.Species == 292)
            {
                // Check Shedinja evolved moves from Ninjask after egg moves
                // Those moves could also be inherited egg moves
                ParseShedinjaEvolveMoves(moves, ref res);
            }

            for (int m = 0; m < 4; m++)
            {
                if (res[m] == null)
                    res[m] = new CheckResult(Severity.Invalid, V176, CheckIdentifier.Move);
            }
            return res;
        }
        private void ParseShedinjaEvolveMoves(int[] moves, ref CheckResult[] res)
        {
            List<int>[] ShedinjaEvoMoves = Legal.getShedinjaEvolveMoves(pkm);
            var ShedinjaEvoMovesLearned = new List<int>();
            for (int gen = 4; gen >= 3; gen--)
            {
                bool native = gen == pkm.Format;
                for (int m = 0; m < 4; m++)
                {
                    if (res[m]?.Valid ?? false)
                        continue;

                    if (!ShedinjaEvoMoves[gen].Contains(moves[m]))
                        continue;

                    res[m] = new CheckResult(Severity.Valid, native ? V355 : string.Format(V356, gen), CheckIdentifier.Move);
                    ShedinjaEvoMovesLearned.Add(m);
                }
            }

            if (ShedinjaEvoMovesLearned.Count <= 1)
                return;

            foreach (int m in ShedinjaEvoMovesLearned)
                res[m] = new CheckResult(Severity.Invalid, V357, CheckIdentifier.Move);
        }

        private void verifyPreRelearn()
        {
            // For origins prior to relearn moves, need to try to match a mystery gift if applicable.

            if (pkm.WasEvent || pkm.WasEventEgg)
            {
                EventGiftMatch = new List<MysteryGift>(Legal.getValidGifts(pkm));
                EncounterMatch = EventGiftMatch.FirstOrDefault();
            }
        }
        private CheckResult[] verifyRelearn()
        {
            RelearnBase = null;

            if (pkm.GenNumber < 6 || pkm.VC1)
                return verifyRelearnNone();

            if (pkm.WasLink)
                return verifyRelearnLink();

            if (pkm.WasEvent || pkm.WasEventEgg)
                return verifyRelearnMysteryGift();

            if (pkm.WasEgg && !Legal.NoHatchFromEgg.Contains(pkm.Species))
                return verifyRelearnEgg();

            if (pkm.RelearnMove1 != 0 && Legal.getDexNavValid(pkm))
                return verifyRelearnDexNav();

            return verifyRelearnNone();
        }
        private CheckResult[] verifyRelearnMysteryGift()
        {
            CheckResult[] res = new CheckResult[4];
            int[] RelearnMoves = pkm.RelearnMoves;
            // Get gifts that match

            EventGiftMatch = new List<MysteryGift>(Legal.getValidGifts(pkm));
            foreach (MysteryGift mg in EventGiftMatch.ToArray())
            {
                int[] moves = mg.RelearnMoves;
                for (int i = 0; i < 4; i++)
                    res[i] = moves[i] != RelearnMoves[i]
                        ? new CheckResult(Severity.Invalid, string.Format(V178, movelist[moves[i]]), CheckIdentifier.RelearnMove)
                        : new CheckResult(CheckIdentifier.RelearnMove);
                if (res.Any(r => !r.Valid))
                    EventGiftMatch.Remove(mg);
            }
            if (EventGiftMatch.Count > 1)
                return res;

            if (EventGiftMatch.Count == 1)
            {
                EncounterMatch = EventGiftMatch[0];
                RelearnBase = EventGiftMatch[0].RelearnMoves;
                return res;
            }

            // No gift match, thus no relearn moves
            EncounterMatch = EncounterType = null;
            return verifyRelearnNone(); 
        }
        private CheckResult[] verifyRelearnDexNav()
        {
            CheckResult[] res = new CheckResult[4];
            int[] RelearnMoves = pkm.RelearnMoves;

            // DexNav Pokémon can have 1 random egg move as a relearn move.
            res[0] = !Legal.getValidRelearn(pkm, 0).Contains(RelearnMoves[0])
                    ? new CheckResult(Severity.Invalid, V183, CheckIdentifier.RelearnMove)
                    : new CheckResult(CheckIdentifier.RelearnMove);

            // All other relearn moves must be empty.
            for (int i = 1; i < 4; i++)
                res[i] = RelearnMoves[i] != 0
                    ? new CheckResult(Severity.Invalid, V184, CheckIdentifier.RelearnMove)
                    : new CheckResult(CheckIdentifier.RelearnMove);

            // Update the relearn base moves if the first relearn move is okay.
            if (res[0].Valid)
                RelearnBase = new[] { RelearnMoves[0], 0, 0, 0 };

            return res;
        }
        private CheckResult[] verifyRelearnNone()
        {
            CheckResult[] res = new CheckResult[4];
            int[] RelearnMoves = pkm.RelearnMoves;
            
            // No relearn moves should be present.
            for (int i = 0; i < 4; i++)
                res[i] = RelearnMoves[i] != 0
                    ? new CheckResult(Severity.Invalid, V184, CheckIdentifier.RelearnMove)
                    : new CheckResult(CheckIdentifier.RelearnMove);

            return res;
        }
        private CheckResult[] verifyRelearnLink()
        {
            // The WasLink check indicated that it was from the Pokémon Link
            var Link = Legal.getValidLinkGifts(pkm);

            // But no encounter was able to be matched. Abort!
            if (Link == null)
                return verifyRelearnNone();

            EncounterMatch = Link;
            CheckResult[] res = new CheckResult[4];
            int[] RelearnMoves = pkm.RelearnMoves;
            int[] LinkRelearn = ((EncounterLink)EncounterMatch).RelearnMoves;
            
            // Pokémon Link encounters should have their relearn moves match exactly.
            RelearnBase = LinkRelearn;
            for (int i = 0; i < 4; i++)
                res[i] = LinkRelearn[i] != RelearnMoves[i]
                    ? new CheckResult(Severity.Invalid, string.Format(V178, movelist[LinkRelearn[i]]), CheckIdentifier.RelearnMove)
                    : new CheckResult(CheckIdentifier.RelearnMove);

            return res;
        }
        private CheckResult[] verifyRelearnEgg()
        {
            CheckResult[] res = new CheckResult[4];
            int[] RelearnMoves = pkm.RelearnMoves;

            // Some games can have different egg movepools. Have to check all situations.
            GameVersion[] Games = { };
            switch (pkm.GenNumber)
            {
                case 6:
                    Games = new[] { GameVersion.XY, GameVersion.ORAS };
                    break;
                case 7:
                    Games = new[] { GameVersion.SM };
                    break;
            }

            bool checkAllGames = pkm.WasTradedEgg;
            bool splitBreed = Legal.getSplitBreedGeneration(pkm).Contains(pkm.Species);
            int iterate = (checkAllGames ? Games.Length : 1) * (splitBreed ? 2 : 1);

            for (int i = 0; i < iterate; i++)
            {
                // Obtain parameters for the Egg's Base Moves
                int gameSource = !checkAllGames ? -1 : i % iterate / (splitBreed ? 2 : 1);
                int skipOption = splitBreed && iterate / 2 <= i ? 1 : 0;
                GameVersion ver = gameSource == -1 ? GameVersion.Any : Games[gameSource];

                // Generate & Analyze compatibility
                res = verifyRelearnEggBase(RelearnMoves, skipOption, ver);
                if (res.All(r => r.Valid)) // egg is satisfactory
                    break;
            }

            verifyNoEmptyDuplicates(RelearnMoves, res);
            return res;
        }
        private CheckResult[] verifyRelearnEggBase(int[] RelearnMoves, int skipOption, GameVersion ver)
        {
            CheckResult[] res = new CheckResult[4];

            // Obtain level1 moves
            List<int> baseMoves = new List<int>(Legal.getBaseEggMoves(pkm, skipOption, ver, 1));
            int baseCt = baseMoves.Count;
            if (baseCt > 4) baseCt = 4;

            // Obtain Inherited moves
            var inheritMoves = Legal.getValidRelearn(pkm, skipOption).ToList();
            var inherited = RelearnMoves.Where(m => m != 0 && (!baseMoves.Contains(m) || inheritMoves.Contains(m))).ToList();
            int inheritCt = inherited.Count;

            // Get required amount of base moves
            int unique = baseMoves.Concat(inherited).Distinct().Count();
            int reqBase = inheritCt == 4 || baseCt + inheritCt > 4 ? 4 - inheritCt : baseCt;
            if (RelearnMoves.Where(m => m != 0).Count() < Math.Min(4, baseMoves.Count))
                reqBase = Math.Min(4, unique);

            // Check if the required amount of Base Egg Moves are present.
            for (int i = 0; i < reqBase; i++)
            {
                if (baseMoves.Contains(RelearnMoves[i]))
                    res[i] = new CheckResult(Severity.Valid, V179, CheckIdentifier.RelearnMove);
                else
                {
                    // mark remaining base egg moves missing
                    for (int z = i; z < reqBase; z++)
                        res[z] = new CheckResult(Severity.Invalid, V180, CheckIdentifier.RelearnMove);

                    // provide the list of suggested base moves for the last required slot
                    string em = string.Join(", ", baseMoves.Select(m => m >= movelist.Length ? V190 : movelist[m]));
                    res[reqBase - 1].Comment += string.Format(Environment.NewLine + V181, em);
                    break;
                }
            }

            // Non-Base moves that can magically appear in the regular movepool
            if (Legal.LightBall.Contains(pkm.Species))
                inheritMoves.Add(344);

            // Inherited moves appear after the required base moves.
            for (int i = reqBase; i < 4; i++)
            {
                if (RelearnMoves[i] == 0) // empty
                    res[i] = new CheckResult(Severity.Valid, V167, CheckIdentifier.RelearnMove);
                else if (inheritMoves.Contains(RelearnMoves[i])) // inherited
                    res[i] = new CheckResult(Severity.Valid, V172, CheckIdentifier.RelearnMove);
                else // not inheritable, flag
                    res[i] = new CheckResult(Severity.Invalid, V182, CheckIdentifier.RelearnMove);
            }

            RelearnBase = baseMoves.ToArray();
            return res;
        }

        /* Similar to verifyRelearnEgg but in pre relearn generation is the moves what should match the expected order
         but only if the pokemon is inside an egg */
        private CheckResult[] verifyPreRelearnEggBase(int[] Moves, List<int> baseMoves, List<int> eggmoves, List<int> lvlmoves, List<int> tmhmmoves, List<int> tutormoves, List<int> specialmoves, bool AllowInherited, GameVersion ver)
        {
            CheckResult[] res = new CheckResult[4];

            // Obtain level1 moves
            int baseCt = baseMoves.Count;
            if (baseCt > 4) baseCt = 4;

            // Obtain Inherited moves
            var inherited = Moves.Where(m => m != 0 && (!baseMoves.Contains(m) || specialmoves.Contains(m) || eggmoves.Contains(m) || lvlmoves.Contains(m) || tmhmmoves.Contains(m) || tutormoves.Contains(m))).ToList();
            int inheritCt = inherited.Count;

            // Get required amount of base moves
            int unique = baseMoves.Concat(inherited).Distinct().Count();
            int reqBase = inheritCt == 4 || baseCt + inheritCt > 4 ? 4 - inheritCt : baseCt;
            if (Moves.Where(m => m != 0).Count() < Math.Min(4, baseMoves.Count))
                reqBase = Math.Min(4, unique);

            var em = string.Empty;
            var moveoffset = 0;
            // Check if the required amount of Base Egg Moves are present.
            for (int i = moveoffset; i < reqBase; i++)
            {
                if (baseMoves.Contains(Moves[i]))
                    res[i] = new CheckResult(Severity.Valid, V179, CheckIdentifier.Move);
                else
                {
                    // mark remaining base egg moves missing
                    for (int z = i; z < reqBase; z++)
                        res[z] = new CheckResult(Severity.Invalid, V180, CheckIdentifier.Move);

                    // provide the list of suggested base moves for the last required slot
                    em = string.Join(", ", baseMoves.Select(m => m >= movelist.Length ? V190 : movelist[m]));
                    break;
                }
            }

            moveoffset += reqBase;

            // Check also if the required amount of Special Egg Moves are present, ir are after base moves
            for (int i = moveoffset; i < moveoffset + specialmoves.Count; i++)
            {
                if (specialmoves.Contains(Moves[i]))
                    res[i] = new CheckResult(Severity.Valid, V333, CheckIdentifier.Move);
                else
                {
                    // mark remaining special egg moves missing
                    for (int z = i; z < moveoffset + specialmoves.Count; z++)
                        res[z] = new CheckResult(Severity.Invalid, V342, CheckIdentifier.Move);

                    // provide the list of suggested base moves and specia moves for the last required slot
                    if (!string.IsNullOrEmpty(em)) em += ",";
                    else
                        em = string.Join(", ", baseMoves.Select(m => m >= movelist.Length ? V190 : movelist[m])) + ",";
                    em += string.Join(", ", specialmoves.Select(m => m >= movelist.Length ? V190 : movelist[m]));
                    break;
                }
            }

            if(!string.IsNullOrEmpty(em))
                res[reqBase > 0 ? reqBase - 1 : 0].Comment = string.Format(Environment.NewLine + V343, em);
            // Non-Base moves that can magically appear in the regular movepool
            if (pkm.GenNumber >=3 && Legal.LightBall.Contains(pkm.Species))
                eggmoves.Add(344);

            // Inherited moves appear after the required base moves.
            var AllowInheritedSeverity = AllowInherited ? Severity.Valid : Severity.Invalid;
            for (int i = reqBase + specialmoves.Count; i < 4; i++)
            {
                if (Moves[i] == 0) // empty
                    res[i] = new CheckResult(Severity.Valid, V167, CheckIdentifier.Move);
                else if (eggmoves.Contains(Moves[i])) // inherited egg move
                    res[i] = new CheckResult(AllowInheritedSeverity, AllowInherited ? V344 : V341, CheckIdentifier.Move);
                else if (lvlmoves.Contains(Moves[i])) // inherited lvl moves
                    res[i] = new CheckResult(AllowInheritedSeverity, AllowInherited ? V345 : V347, CheckIdentifier.Move);
                else if (tmhmmoves.Contains(Moves[i])) // inherited TMHM moves
                    res[i] = new CheckResult(AllowInheritedSeverity, AllowInherited ? V349 : V350, CheckIdentifier.Move);
                else if (tutormoves.Contains(Moves[i])) // inherited tutor moves
                    res[i] = new CheckResult(AllowInheritedSeverity, AllowInherited ? V346 : V348, CheckIdentifier.Move);
                else // not inheritable, flag
                    res[i] = new CheckResult(Severity.Invalid, V340, CheckIdentifier.Move);
            }
            
            return res;
        }

        private void verifyNoEmptyDuplicates(int[] Moves, CheckResult[] res)
        {
            bool emptySlot = false;
            for (int i = 0; i < 4; i++)
            {
                if (Moves[i] == 0)
                    emptySlot = true;
                else if (emptySlot)
                    res[i] = new CheckResult(Severity.Invalid, V167, res[i].Identifier);
                else if (Moves.Count(m => m == Moves[i]) > 1)
                    res[i] = new CheckResult(Severity.Invalid, V168, res[i].Identifier);
            }
        }

        private CheckResult verifyEggMoves()
        {
            if (!pkm.WasEgg || vMoves.All(m => m.Valid))
                return new CheckResult(CheckIdentifier.Egg);

            // todo: egg move breeding legality
            switch (pkm.GenNumber)
            {
                case 1:
                case 2:
                    // Check Both Egg Moves -- egg moves are initially checked with no game as the base.
                    foreach (var game in new[] {GameVersion.GS, GameVersion.C})
                    {
                        vMoves = verifyMoves(game);
                        if (vMoves.Any(m => !m.Valid))
                            continue;

                        // todo: check compatibility of parents (chain wise)
                        return new CheckResult(Severity.Valid, string.Format(V185, game), CheckIdentifier.Egg);
                    }
                    break;

                case 3:
                    return new CheckResult(Severity.NotImplemented, V186, CheckIdentifier.Egg);

                case 4:
                    return new CheckResult(Severity.NotImplemented, V186, CheckIdentifier.Egg);
            }
            return new CheckResult(CheckIdentifier.Egg);
        }

        public static string[] movelist = Util.getMovesList("en");
        private static readonly string[] EventRibName =
        {
            "Country", "National", "Earth", "World", "Classic",
            "Premier", "Event", "Birthday", "Special", "Souvenir",
            "Wishing", "Battle Champ", "Regional Champ", "National Champ", "World Champ"
        };

        /// <summary>
        /// Converts a Check result Severity determination (Valid/Invalid/etc) to the localized string.
        /// </summary>
        /// <param name="s"><see cref="Severity"/> value to convert to string.</param>
        /// <returns>Localized <see cref="string"/>.</returns>
        private static string getString(Severity s)
        {
            switch (s)
            {
                case Severity.Indeterminate: return V500;
                case Severity.Invalid: return V501;
                case Severity.Fishy: return V502;
                case Severity.Valid: return V503;
                default: return V504;
            }
        }
    }
}

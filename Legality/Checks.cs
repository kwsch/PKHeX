using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX
{
    public enum Severity
    {
        Indeterminate = -2,
        Invalid = -1,
        Fishy = 0,
        Valid = 1,
        NotImplemented = 2,
    }
    public class LegalityCheck
    {
        public Severity Judgement = Severity.Valid;
        public readonly string Comment = "Valid";
        public bool Valid => Judgement >= Severity.Fishy;
        public bool Flag;

        public LegalityCheck() { }
        public LegalityCheck(Severity s, string c)
        {
            Judgement = s;
            Comment = c;
        }
    }
    public partial class LegalityAnalysis
    {
        private LegalityCheck verifyECPID()
        {
            // Secondary Checks
            if (pk6.EncryptionConstant == 0)
                return new LegalityCheck(Severity.Fishy, "Encryption Constant is not set.");

            if (pk6.PID == 0)
                return new LegalityCheck(Severity.Fishy, "PID is not set.");

            if (pk6.Gen6)
                return new LegalityCheck();

            // When transferred to Generation 6, the Encryption Constant is copied from the PID.
            // The PID is then checked to see if it becomes shiny with the new Shiny rules (>>4 instead of >>3)
            // If the PID is nonshiny->shiny, the top bit is flipped.

            // Check to see if the PID and EC are properly configured.
            bool xorPID = ((pk6.TID ^ pk6.SID ^ (int)(pk6.PID & 0xFFFF) ^ (int)(pk6.PID >> 16)) & 0x7) == 8;
            bool valid = xorPID
                ? pk6.EncryptionConstant == (pk6.PID ^ 0x8000000)
                : pk6.EncryptionConstant == pk6.PID;

            if (!valid)
                if (xorPID)
                    return new LegalityCheck(Severity.Invalid, "PID should be equal to EC [with top bit flipped]!");
                else
                    return new LegalityCheck(Severity.Invalid, "PID should be equal to EC!");

            return new LegalityCheck();
        }
        private LegalityCheck verifyNickname()
        {
            // If the Pokémon is not nicknamed, it should match one of the language strings.
            if (pk6.Nickname.Length == 0)
                return new LegalityCheck(Severity.Indeterminate, "Nickname is empty.");
            if (pk6.Species > PKX.SpeciesLang[0].Length)
                return new LegalityCheck(Severity.Indeterminate, "Species index invalid for Nickname comparison.");
            if (!Encounter.Valid)
                return new LegalityCheck(Severity.Valid, "Skipped Nickname check due to other check being invalid.");
            
            if (pk6.Language > 8)
                return new LegalityCheck(Severity.Indeterminate, "Language ID > 8.");
            
            if (EncounterType == typeof(EncounterTrade))
            {
                string[] validOT = new string[0];
                int index = -1;
                if (pk6.XY)
                {
                    validOT = Legal.TradeXY[pk6.Language];
                    index = Array.IndexOf(Legal.TradeGift_XY, EncounterMatch);
                }
                else if (pk6.AO)
                {
                    validOT = Legal.TradeAO[pk6.Language];
                    index = Array.IndexOf(Legal.TradeGift_AO, EncounterMatch);
                }
                if (validOT.Length == 0)
                    return new LegalityCheck(Severity.Indeterminate, "Ingame Trade invalid version?");
                if (index == -1 || validOT.Length < index * 2)
                    return new LegalityCheck(Severity.Indeterminate, "Ingame Trade invalid lookup?");

                string nick = validOT[index];
                string OT = validOT[validOT.Length/2 + index];

                if (nick != pk6.Nickname)
                    return new LegalityCheck(Severity.Fishy, "Ingame Trade nickname has been altered.");
                if (OT != pk6.OT_Name)
                    return new LegalityCheck(Severity.Invalid, "Ingame Trade OT has been altered.");

                return new LegalityCheck(Severity.Valid, "Ingame Trade OT/Nickname have not been altered.");
            }

            if (pk6.IsEgg)
            {
                if (!pk6.IsNicknamed)
                    return new LegalityCheck(Severity.Invalid, "Eggs must be nicknamed.");
                return PKX.SpeciesLang[pk6.Language][0] == pk6.Nickname
                    ? new LegalityCheck(Severity.Valid, "Egg matches language Egg name.")
                    : new LegalityCheck(Severity.Invalid, "Egg name does not match language Egg name.");
            }
            string nickname = pk6.Nickname.Replace("'", "’");
            if (pk6.IsNicknamed)
            {
                for (int i = 0; i < PKX.SpeciesLang.Length; i++)
                {
                    string[] lang = PKX.SpeciesLang[i];
                    int index = Array.IndexOf(lang, nickname);
                    if (index < 0)
                        continue;

                    return index == pk6.Species && i != pk6.Language
                        ? new LegalityCheck(Severity.Fishy, "Nickname matches another species name (+language).")
                        : new LegalityCheck(Severity.Fishy, "Nickname flagged, matches species name.");
                }
                return new LegalityCheck(Severity.Valid, "Nickname does not match another species name.");
            }
            // else
            {
                // Can't have another language name if it hasn't evolved.
                return Legal.getHasEvolved(pk6) && PKX.SpeciesLang.Any(lang => lang[pk6.Species] == nickname)
                       || PKX.SpeciesLang[pk6.Language][pk6.Species] == nickname
                    ? new LegalityCheck(Severity.Valid, "Nickname matches species name.")
                    : new LegalityCheck(Severity.Invalid, "Nickname does not match species name.");
            }
        }
        private LegalityCheck verifyEVs()
        {
            var evs = pk6.EVs;
            int sum = evs.Sum();
            if (sum == 0 && pk6.Met_Level != pk6.Stat_Level && pk6.Stat_Level > 1)
                return new LegalityCheck(Severity.Fishy, "All EVs are zero, but leveled above Met Level");
            if (sum == 508)
                return new LegalityCheck(Severity.Fishy, "2 EVs remaining.");
            if (sum > 510)
                return new LegalityCheck(Severity.Invalid, "EV total cannot be above 510.");
            if (evs.Any(ev => ev > 252))
                return new LegalityCheck(Severity.Invalid, "EVs cannot go above 252.");
            if (evs.All(ev => pk6.EVs[0] == ev) && evs[0] != 0)
                return new LegalityCheck(Severity.Fishy, "EVs are all equal.");

            return new LegalityCheck();
        }
        private LegalityCheck verifyIVs()
        {
            if (pk6.IVs.Sum() == 0)
                return new LegalityCheck(Severity.Fishy, "All IVs are zero.");
            if (pk6.IVs[0] < 30 && pk6.IVs.All(iv => pk6.IVs[0] == iv))
                return new LegalityCheck(Severity.Fishy, "All IVs are equal.");
            return new LegalityCheck();
        }
        private LegalityCheck verifyID()
        {
            if (pk6.TID == 0 && pk6.SID == 0)
                return new LegalityCheck(Severity.Fishy, "TID and SID are zero.");
            if (pk6.TID == 0)
                return new LegalityCheck(Severity.Fishy, "TID is zero.");
            if (pk6.SID == 0)
                return new LegalityCheck(Severity.Fishy, "SID is zero.");
            return new LegalityCheck();
        }
        private LegalityCheck verifyEncounter()
        {
            if (!pk6.Gen6)
                return new LegalityCheck {Judgement = Severity.NotImplemented};

            if (pk6.WasLink)
            {
                // Should NOT be Fateful, and should be in Database
                EncounterLink enc = EncounterMatch as EncounterLink;
                if (enc == null)
                    return new LegalityCheck(Severity.Invalid, "Not a valid Link gift -- unable to find matching gift.");
                
                if (pk6.XY && !enc.XY)
                    return new LegalityCheck(Severity.Invalid, "Not a valid Link gift -- can't obtain in XY.");
                if (pk6.AO && !enc.ORAS)
                    return new LegalityCheck(Severity.Invalid, "Not a valid Link gift -- can't obtain in ORAS.");

                return pk6.FatefulEncounter 
                    ? new LegalityCheck(Severity.Invalid, "Not a valid Link gift -- should not be Fateful Encounter.") 
                    : new LegalityCheck(Severity.Valid, "Valid Link gift.");
            }
            if (pk6.WasEvent || pk6.WasEventEgg)
            {
                WC6 MatchedWC6 = EncounterMatch as WC6;
                return MatchedWC6 != null // Matched in RelearnMoves check.
                    ? new LegalityCheck(Severity.Valid, $"Matches #{MatchedWC6.CardID.ToString("0000")} ({MatchedWC6.CardTitle})") 
                    : new LegalityCheck(Severity.Invalid, "Not a valid Wonder Card gift.");
            }

            EncounterMatch = null; // Reset object
            if (pk6.WasEgg)
            {
                // Check Hatch Locations
                if (pk6.Met_Level != 1)
                    return new LegalityCheck(Severity.Invalid, "Invalid met level, expected 1.");
                if (pk6.IsEgg)
                {
                    return pk6.Met_Location == 0
                        ? new LegalityCheck(Severity.Valid, "Valid un-hatched egg.")
                        : new LegalityCheck(Severity.Invalid, "Invalid location for un-hatched egg (expected ID:0)");
                }
                if (pk6.XY)
                {
                    return Legal.ValidMet_XY.Contains(pk6.Met_Location)
                        ? new LegalityCheck(Severity.Valid, "Valid X/Y hatched egg.")
                        : new LegalityCheck(Severity.Invalid, "Invalid X/Y location for hatched egg.");
                }
                if (pk6.AO)
                {
                    return Legal.ValidMet_AO.Contains(pk6.Met_Location)
                        ? new LegalityCheck(Severity.Valid, "Valid OR/AS hatched egg.")
                        : new LegalityCheck(Severity.Invalid, "Invalid OR/AS location for hatched egg.");
                }
                return new LegalityCheck(Severity.Invalid, "Invalid location for hatched egg.");
            }

            EncounterMatch = Legal.getValidStaticEncounter(pk6);
            if (EncounterMatch != null)
                return new LegalityCheck(Severity.Valid, "Valid gift/static encounter.");

            if (Legal.getIsFossil(pk6))
            {
                return pk6.AbilityNumber != 4
                    ? new LegalityCheck(Severity.Valid, "Valid revived fossil.")
                    : new LegalityCheck(Severity.Invalid, "Hidden ability on revived fossil.");
            }
            EncounterMatch = Legal.getValidFriendSafari(pk6);
            if (EncounterMatch != null)
            {
                if (pk6.Species == 670 || pk6.Species == 671) // Floette
                    if (pk6.AltForm % 2 != 0) // 0/2/4
                        return new LegalityCheck(Severity.Invalid, "Friend Safari: Not valid color.");
                else if (pk6.Species == 710 || pk6.Species == 711) // Pumpkaboo
                    if (pk6.AltForm != 1) // Average
                        return new LegalityCheck(Severity.Invalid, "Friend Safari: Not average sized.");
                else if (pk6.Species == 586) // Sawsbuck
                    if (pk6.AltForm != 0)
                        return new LegalityCheck(Severity.Invalid, "Friend Safari: Not Spring form.");

                return new LegalityCheck(Severity.Valid, "Valid friend safari encounter.");
            }

            EncounterMatch = Legal.getValidWildEncounters(pk6);
            if (EncounterMatch != null)
            {
                return ((EncounterSlot[]) EncounterMatch).Any(slot => !slot.DexNav) 
                    ? new LegalityCheck(Severity.Valid, "Valid encounter at location.") 
                    : new LegalityCheck(Severity.Valid, "Valid DexNav encounter at location.");
            }
            EncounterMatch = Legal.getValidIngameTrade(pk6);
            if (EncounterMatch != null)
            {
                return new LegalityCheck(Severity.Valid, "Valid ingame trade.");
            }
            return new LegalityCheck(Severity.Invalid, "Not a valid encounter.");
        }
        private LegalityCheck verifyLevel()
        {
            WC6 MatchedWC6 = EncounterMatch as WC6;
            if (MatchedWC6 != null && MatchedWC6.Level != pk6.Met_Level)
                return new LegalityCheck(Severity.Invalid, "Met Level does not match Wonder Card level.");
            return pk6.CurrentLevel < pk6.Met_Level
                ? new LegalityCheck(Severity.Invalid, "Current level is below met level.")
                : new LegalityCheck(Severity.Valid, "Current level is not below met level.");
        }
        private LegalityCheck verifyRibbons()
        {
            if (!Encounter.Valid)
                return new LegalityCheck(Severity.Valid, "Skipped Ribbon check due to other check being invalid.");

            List<string> missingRibbons = new List<string>();
            List<string> invalidRibbons = new List<string>();

            // Check Event Ribbons
            bool[] EventRib =
            {
                pk6.RIB2_6, pk6.RIB2_7, pk6.RIB3_0, pk6.RIB3_1, pk6.RIB3_2,
                pk6.RIB3_3, pk6.RIB3_4, pk6.RIB3_5, pk6.RIB3_6, pk6.RIB3_7,
                pk6.RIB4_0, pk6.RIB4_1, pk6.RIB4_2, pk6.RIB4_3, pk6.RIB4_4
            };
            WC6 MatchedWC6 = EncounterMatch as WC6;
            if (MatchedWC6 != null) // Wonder Card
            {
                bool[] wc6rib =
                {
                    MatchedWC6.RIB0_3, MatchedWC6.RIB0_4, MatchedWC6.RIB0_5, MatchedWC6.RIB0_6, MatchedWC6.RIB1_5,
                    MatchedWC6.RIB1_6, MatchedWC6.RIB0_7, MatchedWC6.RIB1_1, MatchedWC6.RIB1_2, MatchedWC6.RIB1_3,
                    MatchedWC6.RIB1_4, MatchedWC6.RIB0_0, MatchedWC6.RIB0_1, MatchedWC6.RIB0_2, MatchedWC6.RIB1_0
                };
                for (int i = 0; i < EventRib.Length; i++)
                    if (EventRib[i] ^ wc6rib[i]) // Mismatch
                        (wc6rib[i] ? missingRibbons : invalidRibbons).Add(EventRibName[i]);
            }
            else if (EncounterType == typeof(EncounterLink))
            {
                // No Event Ribbons except Classic (unless otherwise specified, ie not for Demo)
                for (int i = 0; i < EventRib.Length; i++)
                    if (i != 4 && EventRib[i])
                        invalidRibbons.Add(EventRibName[i]);

                if (EventRib[4] ^ ((EncounterLink)EncounterMatch).Classic)
                    (EventRib[4] ? invalidRibbons : missingRibbons).Add(EventRibName[4]);
            }
            else // No ribbons
            {
                for (int i = 0; i < EventRib.Length; i++)
                    if (EventRib[i])
                        invalidRibbons.Add(EventRibName[i]);
            }

            // Unobtainable ribbons for Gen6 Origin
            if (pk6.RIB0_1)
                invalidRibbons.Add("GBA Champion"); // RSE HoF
            if (pk6.RIB0_2)
                invalidRibbons.Add("Sinnoh Champ"); // DPPt HoF
            if (pk6.RIB2_2)
                invalidRibbons.Add("Artist"); // RSE Master Rank Portrait
            if (pk6.RIB2_4)
                invalidRibbons.Add("Record"); // Unobtainable
            if (pk6.RIB2_5)
                invalidRibbons.Add("Legend"); // HGSS Defeat Red @ Mt.Silver
            if (pk6.Memory_ContestCount > 0)
                invalidRibbons.Add("Contest Memory"); // Gen3/4 Contest
            if (pk6.Memory_BattleCount > 0)
                invalidRibbons.Add("Battle Memory"); // Gen3/4 Battle
            
            if (missingRibbons.Count + invalidRibbons.Count == 0)
                return new LegalityCheck(Severity.Valid, "All ribbons accounted for.");

            string[] result = new string[2];
            if (missingRibbons.Count > 0)
                result[0] = "Missing Ribbons: " + string.Join(", ", missingRibbons);
            if (invalidRibbons.Count > 0)
                result[1] = "Invalid Ribbons: " + string.Join(", ", invalidRibbons);
            return new LegalityCheck(Severity.Invalid, string.Join(Environment.NewLine, result.Where(s=>!string.IsNullOrEmpty(s))));
        }
        private LegalityCheck verifyAbility()
        {
            int index = Legal.PersonalAO[pk6.Species].FormeIndex(pk6.Species, pk6.AltForm);
            byte[] abilities = Legal.PersonalAO[index].Abilities;
            int abilval = Array.IndexOf(abilities, (byte)pk6.Ability);
            if (abilval < 0)
                return new LegalityCheck(Severity.Invalid, "Ability is not valid for species/form.");

            if (EncounterMatch != null)
            {
                if (EncounterType == typeof(EncounterStatic))
                    if (pk6.AbilityNumber == 4 ^ ((EncounterStatic)EncounterMatch).Ability == 4)
                        return new LegalityCheck(Severity.Invalid, "Hidden Ability mismatch for static encounter.");
                if (EncounterType == typeof(EncounterTrade))
                    if (pk6.AbilityNumber == 4 ^ ((EncounterTrade)EncounterMatch).Ability == 4)
                        return new LegalityCheck(Severity.Invalid, "Hidden Ability mismatch for ingame trade.");
                if (EncounterType == typeof (EncounterSlot[]) && pk6.AbilityNumber == 4)
                    if (((EncounterSlot[]) EncounterMatch).All(slot => slot.Type != SlotType.FriendSafari) &&
                        ((EncounterSlot[]) EncounterMatch).All(slot => slot.Type != SlotType.Horde))
                        return new LegalityCheck(Severity.Invalid, "Hidden Ability on non-horde/friend safari wild encounter.");
            }

            return abilities[pk6.AbilityNumber >> 1] != pk6.Ability
                ? new LegalityCheck(Severity.Invalid, "Ability does not match ability number.")
                : new LegalityCheck(Severity.Valid, "Ability matches ability number.");
        }
        private LegalityCheck verifyBall()
        {
            if (!pk6.Gen6)
                return new LegalityCheck();
            if (!Encounter.Valid)
                return new LegalityCheck(Severity.Valid, "Skipped Ball check due to other check being invalid.");
            
            if (EncounterType == typeof(WC6))
                return pk6.Ball != ((WC6)EncounterMatch).Pokéball
                    ? new LegalityCheck(Severity.Invalid, "Ball does not match specified Wonder Card Ball.")
                    : new LegalityCheck(Severity.Valid, "Ball matches Wonder Card.");
            if (EncounterType == typeof(EncounterLink))
                return ((EncounterLink)EncounterMatch).Ball != pk6.Ball
                    ? new LegalityCheck(Severity.Invalid, "Incorrect ball on Link gift.")
                    : new LegalityCheck(Severity.Valid, "Correct ball on Link gift.");
            if (EncounterType == typeof(EncounterTrade))
                return pk6.Ball != 4 // Pokeball
                    ? new LegalityCheck(Severity.Invalid, "Incorrect ball on ingame trade encounter.")
                    : new LegalityCheck(Severity.Valid, "Correct ball on ingame trade encounter.");

            if (pk6.Ball == 0x04) // Poké Ball
                return new LegalityCheck(Severity.Valid, "Standard Poké Ball.");

            if (EncounterType == typeof(EncounterStatic))
                return !Legal.WildPokeballs.Contains(pk6.Ball)
                    ? new LegalityCheck(Severity.Invalid, "Incorrect ball on ingame static encounter.")
                    : new LegalityCheck(Severity.Valid, "Correct ball on ingame static encounter.");
            if (EncounterType == typeof(EncounterSlot[]))
                return !Legal.WildPokeballs.Contains(pk6.Ball)
                    ? new LegalityCheck(Severity.Invalid, "Incorrect ball on ingame encounter.")
                    : new LegalityCheck(Severity.Valid, "Correct ball on ingame encounter.");

            if (pk6.WasEgg)
            {
                if (pk6.Ball == 0x01) // Master Ball
                    return new LegalityCheck(Severity.Invalid, "Master Ball on egg origin.");
                if (pk6.Ball == 0x10) // Cherish Ball
                    return new LegalityCheck(Severity.Invalid, "Cherish Ball on non-event.");

                if (pk6.Gender == 2) // Genderless
                    return pk6.Ball != 0x04 // Must be Pokéball as ball can only pass via mother (not Ditto!)
                        ? new LegalityCheck(Severity.Invalid, "Non-Pokéball on genderless egg.")
                        : new LegalityCheck(Severity.Valid, "Pokéball on genderless egg.");
                if (Legal.BreedMaleOnly.Contains(pk6.Species))
                    return pk6.Ball != 0x04 // Must be Pokéball as ball can only pass via mother (not Ditto!)
                        ? new LegalityCheck(Severity.Invalid, "Non-Pokéball on Male-Only egg.")
                        : new LegalityCheck(Severity.Valid, "Pokéball on Male-Only egg.");

                if (pk6.Ball == 0x05) // Safari Ball
                {
                    if (Legal.getLineage(pk6).All(e => !Legal.Inherit_Safari.Contains(e)))
                        return new LegalityCheck(Severity.Invalid, "Safari Ball not possible for species.");
                    if (pk6.AbilityNumber == 4)
                        return new LegalityCheck(Severity.Invalid, "Safari Ball with Hidden Ability.");

                    return new LegalityCheck(Severity.Valid, "Safari Ball possible for species.");
                }
                if (0x10 < pk6.Ball && pk6.Ball < 0x18) // Apricorn Ball
                {
                    if (Legal.getLineage(pk6).All(e => !Legal.Inherit_Apricorn.Contains(e)))
                        return new LegalityCheck(Severity.Invalid, "Apricorn Ball not possible for species.");
                    if (pk6.AbilityNumber == 4)
                        return new LegalityCheck(Severity.Invalid, "Apricorn Ball with Hidden Ability.");

                    return new LegalityCheck(Severity.Valid, "Apricorn Ball possible for species.");
                }
                if (pk6.Ball == 0x18) // Sport Ball
                {
                    if (Legal.getLineage(pk6).All(e => !Legal.Inherit_Sport.Contains(e)))
                        return new LegalityCheck(Severity.Invalid, "Sport Ball not possible for species.");
                    if (pk6.AbilityNumber == 4)
                        return new LegalityCheck(Severity.Invalid, "Sport Ball with Hidden Ability.");

                    return new LegalityCheck(Severity.Valid, "Sport Ball possible for species.");
                }
                if (pk6.Ball == 0x19) // Dream Ball
                {
                    if (Legal.getLineage(pk6).All(e => !Legal.Inherit_Dream.Contains(e)))
                        return new LegalityCheck(Severity.Invalid, "Dream Ball not possible for species.");

                    return new LegalityCheck(Severity.Valid, "Dream Ball possible for species.");
                }

                if (pk6.Species > 650 && pk6.Species != 700) // Sylveon
                    return !Legal.WildPokeballs.Contains(pk6.Ball)
                        ? new LegalityCheck(Severity.Invalid, "Unobtainable ball for Kalos origin.")
                        : new LegalityCheck(Severity.Valid, "Obtainable ball for Kalos origin.");
                
                if (0x0D <= pk6.Ball && pk6.Ball <= 0x0F)
                {
                    if (Legal.Ban_Gen4Ball.Contains(pk6.Species))
                        return new LegalityCheck(Severity.Invalid, "Unobtainable capture for Gen4 Ball.");

                    return new LegalityCheck(Severity.Valid, "Obtainable capture for Gen4 Ball.");
                }
                if (0x02 <= pk6.Ball && pk6.Ball <= 0x0C) // Don't worry, Ball # 0x05 was already checked.
                {
                    if (Legal.Ban_Gen3Ball.Contains(pk6.Species))
                        return new LegalityCheck(Severity.Invalid, "Unobtainable capture for Gen4 Ball.");

                    return new LegalityCheck(Severity.Valid, "Obtainable capture for Gen4 Ball.");
                }
            }

            return new LegalityCheck(Severity.Invalid, "No ball check satisfied, assuming illegal.");
        }
        private LegalityCheck verifyHandlerMemories()
        {
            if (!Encounter.Valid)
                return new LegalityCheck(Severity.Valid, "Skipped Memory check due to other check being invalid.");

            WC6 MatchedWC6 = EncounterMatch as WC6;
            if (MatchedWC6?.OT.Length > 0) // Has Event OT -- null propagation yields false if MatchedWC6=null
            {
                if (pk6.OT_Friendship != PKX.getBaseFriendship(pk6.Species))
                    return new LegalityCheck(Severity.Invalid, "Event OT Friendship does not match base friendship.");
                if (pk6.OT_Affection != 0)
                    return new LegalityCheck(Severity.Invalid, "Event OT Affection should be zero.");
                if (pk6.CurrentHandler != 1)
                    return new LegalityCheck(Severity.Invalid, "Current handler should not be Event OT.");
            }
            if (!pk6.WasEvent && (pk6.HT_Name.Length == 0 || pk6.Geo1_Country == 0)) // Is not Traded
            {
                if (pk6.HT_Name.Length != 0)
                    return new LegalityCheck(Severity.Invalid, "GeoLocation -- HT Name present but has no previous Country.");
                if (pk6.Geo1_Country != 0)
                    return new LegalityCheck(Severity.Invalid, "GeoLocation -- Previous country of residence but no Handling Trainer.");
                if (pk6.HT_Memory != 0)
                    return new LegalityCheck(Severity.Invalid, "Memory -- Handling Trainer memory present but no Handling Trainer.");
                if (pk6.CurrentHandler != 0) // Badly edited; PKHeX doesn't trip this.
                    return new LegalityCheck(Severity.Invalid, "Untraded -- Current handler should not be the Handling Trainer.");
                if (pk6.HT_Friendship != 0)
                    return new LegalityCheck(Severity.Invalid, "Untraded -- Handling Trainer Friendship should be zero.");
                if (pk6.HT_Affection != 0)
                    return new LegalityCheck(Severity.Invalid, "Untraded -- Handling Trainer Affection should be zero.");

                // We know it is untraded (HT is empty), if it must be trade evolved flag it.
                if (Legal.getHasTradeEvolved(pk6))
                {
                    if (pk6.Species != 350) // Milotic
                        return new LegalityCheck(Severity.Invalid, "Untraded -- requires a trade evolution.");
                    if (pk6.CNT_Beauty < 170) // Beauty Contest Stat Requirement
                        return new LegalityCheck(Severity.Invalid, "Untraded -- Beauty is not high enough for Levelup Evolution.");
                }
            }
            else // Is Traded
            {
                if (pk6.HT_Memory == 0)
                    return new LegalityCheck(Severity.Invalid, "Memory -- missing Handling Trainer Memory.");
            }
            
            // Unimplemented: Ingame Trade Memories

            return new LegalityCheck(Severity.Valid, "History is valid.");
        }
        private LegalityCheck[] verifyMoves()
        {
            int[] Moves = pk6.Moves;
            LegalityCheck[] res = new LegalityCheck[4];
            for (int i = 0; i < 4; i++)
                res[i] = new LegalityCheck();
            if (!pk6.Gen6)
                return res;

            var validMoves = Legal.getValidMoves(pk6).ToArray();
            if (pk6.Species == 235)
            {
                for (int i = 0; i < 4; i++)
                    res[i] = Legal.InvalidSketch.Contains(Moves[i])
                        ? new LegalityCheck(Severity.Invalid, "Invalid Sketch move.")
                        : new LegalityCheck();
            }
            else
            {
                int[] RelearnMoves = pk6.RelearnMoves;
                WC6 MatchedWC6 = EncounterMatch as WC6;
                int[] WC6Moves = MatchedWC6?.Moves ?? new int[0];
                for (int i = 0; i < 4; i++)
                {
                    if (Moves[i] == Legal.Struggle)
                        res[i] = new LegalityCheck(Severity.Invalid, "Invalid Move: Struggle.");
                    else if (validMoves.Contains(Moves[i]))
                        res[i] = new LegalityCheck(Severity.Valid, "Level-up.");
                    else if (RelearnMoves.Contains(Moves[i]))
                        res[i] = new LegalityCheck(Severity.Valid, "Relearn Move.") {Flag = true};
                    else if (WC6Moves.Contains(Moves[i]))
                        res[i] = new LegalityCheck(Severity.Valid, "Wonder Card Non-Relearn Move.");
                    else
                        res[i] = new LegalityCheck(Severity.Invalid, "Invalid Move.");
                }
            }
            if (Moves[0] == 0)
                res[0] = new LegalityCheck(Severity.Invalid, "Invalid Move.");


            if (pk6.Species == 647) // Keldeo
                if (pk6.AltForm == 1 ^ pk6.Moves.Contains(548))
                    res[0] = new LegalityCheck(Severity.Invalid, "Secret Sword / Resolute Keldeo Mismatch.");

            // Duplicate Moves Check
            for (int i = 0; i < 4; i++)
                if (Moves.Count(m => m != 0 && m == Moves[i]) > 1)
                    res[i] = new LegalityCheck(Severity.Invalid, "Duplicate Move.");

            return res;
        }
        private LegalityCheck[] verifyRelearn()
        {
            RelearnBase = null;
            LegalityCheck[] res = new LegalityCheck[4];
            
            int[] Moves = pk6.RelearnMoves;
            if (!pk6.Gen6)
                goto noRelearn;
            if (pk6.WasLink)
            {
                var Link = Legal.getValidLinkGifts(pk6);
                if (Link == null)
                {
                    for (int i = 0; i < 4; i++)
                        res[i] = new LegalityCheck();
                    return res;
                }
                EncounterMatch = Link;

                int[] moves = ((EncounterLink)EncounterMatch).RelearnMoves;
                RelearnBase = moves;
                for (int i = 0; i < 4; i++)
                    res[i] = moves[i] != Moves[i]
                        ? new LegalityCheck(Severity.Invalid, $"Expected ID: {moves[i]}.")
                        : new LegalityCheck();
                return res;
            }
            if (pk6.WasEvent || pk6.WasEventEgg)
            {
                // Get WC6's that match
                IEnumerable<WC6> vwc6 = Legal.getValidWC6s(pk6);
                foreach (var wc in vwc6)
                {
                    int[] moves = wc.RelearnMoves;
                    for (int i = 0; i < 4; i++)
                        res[i] = moves[i] != Moves[i]
                            ? new LegalityCheck(Severity.Invalid, $"Expected ID: {moves[i]}.")
                            : new LegalityCheck(Severity.Valid, $"Matched WC #{wc.CardID.ToString("0000")}");
                    if (res.All(r => r.Valid))
                    { EncounterMatch = wc; RelearnBase = moves; return res; }
                }
                EncounterMatch = EncounterType = null;
                goto noRelearn; // No WC match
            }

            if (pk6.WasEgg)
            {
                const int games = 2;
                bool checkAllGames = pk6.WasTradedEgg;
                bool splitBreed = Legal.SplitBreed.Contains(pk6.Species);

                int iterate = (checkAllGames ? games : 1) * (splitBreed ? 2 : 1);
                for (int i = 0; i < iterate; i++)
                {
                    int gameSource = !checkAllGames ? -1 : i % iterate / (splitBreed ? 2 : 1);
                    int skipOption = splitBreed && iterate / 2 <= i ? 1 : 0;

                    // Obtain level1 moves
                    List<int> baseMoves = new List<int>(Legal.getBaseEggMoves(pk6, skipOption, gameSource));
                    int baseCt = baseMoves.Count;
                    if (baseCt > 4) baseCt = 4;

                    // Obtain Nonstandard moves
                    var relearnMoves = Legal.getValidRelearn(pk6, skipOption).ToArray();
                    var relearn = pk6.RelearnMoves.Where(move => move != 0 
                        && (!baseMoves.Contains(move) || relearnMoves.Contains(move))
                        ).ToArray();
                    int relearnCt = relearn.Length;

                    // Get Move Window
                    List<int> window = new List<int>(baseMoves);
                    window.AddRange(relearn);
                    int[] moves = window.Skip(baseCt + relearnCt - 4).Take(4).ToArray();
                    Array.Resize(ref moves, 4);

                    int req;
                    if (relearnCt == 4)
                        req = 0;
                    else if (baseCt + relearnCt > 4)
                        req = 4 - relearnCt;
                    else
                        req = baseCt;

                    // Movepool finalized! Check validity.
                    
                    int[] rl = pk6.RelearnMoves;
                    string em = string.Join(", ", baseMoves);
                    RelearnBase = baseMoves.ToArray();
                    // Base Egg Move
                    for (int j = 0; j < req; j++)
                        res[j] = !baseMoves.Contains(rl[j])
                            ? new LegalityCheck(Severity.Invalid, $"Base egg move missing; expected one of: {em}.")
                            : new LegalityCheck(Severity.Valid, "Base egg move.");

                    // Non-Base
                    if (Legal.LightBall.Contains(pk6.Species))
                        relearnMoves = relearnMoves.Concat(new[] { 344 }).ToArray();
                    for (int j = req; j < 4; j++)
                        res[j] = !relearnMoves.Contains(rl[j])
                            ? new LegalityCheck(Severity.Invalid, "Not an expected relearn move.")
                            : new LegalityCheck(Severity.Valid, "Relearn move.");

                    if (res.All(r => r.Valid))
                        break;
                }
                return res;
            }
            if (Moves[0] != 0) // DexNav only?
            {
                // Check DexNav
                if (!Legal.getDexNavValid(pk6))
                    goto noRelearn;

                res[0] = !Legal.getValidRelearn(pk6, 0).Contains(Moves[0])
                        ? new LegalityCheck(Severity.Invalid, "Not an expected DexNav move.")
                        : new LegalityCheck();
                for (int i = 1; i < 4; i++)
                    res[i] = Moves[i] != 0
                        ? new LegalityCheck(Severity.Invalid, "Expected no Relearn Move in slot.")
                        : new LegalityCheck();

                if (res[0].Valid)
                    RelearnBase = new[] { Moves[0], 0, 0, 0 };
                return res;
            }

            // Should have no relearn moves.
            noRelearn:
            for (int i = 0; i < 4; i++)
                res[i] = Moves[i] != 0
                    ? new LegalityCheck(Severity.Invalid, "Expected no Relearn Moves.")
                    : new LegalityCheck();
            return res;
        }

        private static readonly string[] EventRibName =
        {
            "Country", "National", "Earth", "World", "Classic",
            "Premier", "Event", "Birthday", "Special", "Souvenir",
            "Wishing", "Battle Champ", "Regional Champ", "National Champ", "World Champ"
        };
    }
}

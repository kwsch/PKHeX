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
                return PKX.SpeciesLang.Any(lang => lang.Contains(nickname))
                    ? new LegalityCheck(Severity.Invalid, "Nickname matches another species name (+language).")
                    : new LegalityCheck(Severity.Valid, "Nickname does not match another species name.");
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
                return pk6.FatefulEncounter || Legal.getLinkMoves(pk6).Length == 0 
                    ? new LegalityCheck(Severity.Invalid, "Not a valid Link gift.")
                    : new LegalityCheck(Severity.Valid, "Valid Link gift.");
            }
            if (pk6.WasEvent || pk6.WasEventEgg)
            {
                return MatchedWC6 != null // Matched in RelearnMoves check.
                    ? new LegalityCheck(Severity.Valid, $"Matches #{MatchedWC6.CardID.ToString("0000")} ({MatchedWC6.CardTitle})") 
                    : new LegalityCheck(Severity.Invalid, "Not a valid Wonder Card gift.");
            }
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

            EncounterStatic z = Legal.getStaticEncounter(pk6);
            if (z != null)
                return new LegalityCheck(Severity.Valid, "Valid gift/static encounter.");

            if (Legal.getIsFossil(pk6))
            {
                return pk6.AbilityNumber != 4
                    ? new LegalityCheck(Severity.Valid, "Valid revived fossil.")
                    : new LegalityCheck(Severity.Invalid, "Hidden ability on revived fossil.");
            }
            int FriendSafari = Legal.getFriendSafariValid(pk6);
            if (FriendSafari > 0)
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
            
            if (Legal.getDexNavValid(pk6))
                return new LegalityCheck(Severity.Valid, "Valid (DexNav) encounter at location.");
            if (Legal.getWildEncounterValid(pk6))
            {
                return pk6.AbilityNumber != 4
                    ? new LegalityCheck(Severity.Valid, "Valid encounter at location.")
                    : new LegalityCheck(Severity.Invalid, "Hidden ability on valid encounter.");
            }
            EncounterTrade t = Legal.getIngameTrade(pk6);
            if (t != null)
            {
                EncounterMatch = t; // Check in individual methods
                return new LegalityCheck(Severity.Valid, "Valid ingame trade.");
            }
            return new LegalityCheck(Severity.Invalid, "Not a valid encounter.");
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
                int[] WC6Moves = MatchedWC6?.Moves ?? new int[0];
                for (int i = 0; i < 4; i++)
                {
                    if (Moves[i] == Legal.Struggle)
                        res[i] = new LegalityCheck(Severity.Invalid, "Invalid Move: Struggle.");
                    else if (validMoves.Contains(Moves[i]))
                        res[i] = new LegalityCheck(Severity.Valid, "Level-up.");
                    else if (RelearnMoves.Contains(Moves[i]))
                        res[i] = new LegalityCheck(Severity.Valid, "Relearn Move.");
                    else if (WC6Moves.Contains(Moves[i]))
                        res[i] = new LegalityCheck(Severity.Valid, "Wondercard Non-Relearn Move.");
                    else
                        res[i] = new LegalityCheck(Severity.Invalid, "Invalid Move.");
                }
            }
            if (Moves[0] == 0)
                res[0] = new LegalityCheck(Severity.Invalid, "Invalid Move.");


            if (pk6.Species == 647) // Keldeo
                if (pk6.AltForm == 1 ^ pk6.Moves.Contains(533))
                    res[0] = new LegalityCheck(Severity.Invalid, "Sacred Sword / Resolute Keldeo Mismatch.");

            // Duplicate Moves Check
            for (int i = 0; i < 4; i++)
                if (Moves.Count(m => m != 0 && m == Moves[i]) > 1)
                    res[i] = new LegalityCheck(Severity.Invalid, "Duplicate Move.");

            return res;
        }
        private LegalityCheck[] verifyRelearn()
        {
            LegalityCheck[] res = new LegalityCheck[4];
            MatchedWC6 = null; // Reset
            int[] Moves = pk6.RelearnMoves;
            if (!pk6.Gen6)
                goto noRelearn;
            if (pk6.WasLink)
            {
                int[] moves = Legal.getLinkMoves(pk6);
                for (int i = 0; i < 4; i++)
                    res[i] = moves[i] != Moves[i]
                        ? new LegalityCheck(Severity.Invalid, $"Expected ID:{moves[i]}.")
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
                            ? new LegalityCheck(Severity.Invalid, $"Expected ID:{moves[i]}.")
                            : new LegalityCheck(Severity.Valid, $"Matched WC #{wc.CardID.ToString("0000")}");
                    if (res.All(r => r.Valid))
                    { MatchedWC6 = wc; return res; }
                }
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
    }
}

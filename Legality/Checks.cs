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
        public string Comment = "Valid";
        public bool Valid => Judgement >= Severity.Fishy;

        public LegalityCheck() { }
        public LegalityCheck(Severity s, string c)
        {
            Judgement = s;
            Comment = c;
        }
        public static LegalityCheck verifyECPID(PK6 pk)
        {
            // Secondary Checks
            if (pk.EncryptionConstant == 0)
                return new LegalityCheck(Severity.Fishy, "Encryption Constant is not set.");

            if (pk.PID == 0)
                return new LegalityCheck(Severity.Fishy, "PID is not set.");

            if (pk.Gen6)
                return new LegalityCheck();

            // When transferred to Generation 6, the Encryption Constant is copied from the PID.
            // The PID is then checked to see if it becomes shiny with the new Shiny rules (>>4 instead of >>3)
            // If the PID is nonshiny->shiny, the top bit is flipped.

            // Check to see if the PID and EC are properly configured.
            bool xorPID = ((pk.TID ^ pk.SID ^ (int)(pk.PID & 0xFFFF) ^ (int)(pk.PID >> 16)) & 0x7) == 8;
            bool valid = xorPID
                ? pk.EncryptionConstant == (pk.PID ^ 0x8000000)
                : pk.EncryptionConstant == pk.PID;

            if (!valid)
                if (xorPID)
                    return new LegalityCheck(Severity.Invalid, "PID should be equal to EC [with top bit flipped]!");
                else
                    return new LegalityCheck(Severity.Invalid, "PID should be equal to EC!");

            return new LegalityCheck();
        }
        public static LegalityCheck verifyNickname(PK6 pk)
        {
            LegalityCheck r = new LegalityCheck { Judgement = Severity.NotImplemented };
            // If the Pokémon is not nicknamed, it should match one of the language strings.
            if (pk.Nickname.Length == 0)
            {
                r.Judgement = Severity.Indeterminate;
                r.Comment = "Pokémon nickname is empty.";
            }
            return r;
        }
        public static LegalityCheck verifyEVs(PK6 pk)
        {
            var EVs = pk.EVs;
            if (EVs.Sum() == 0 && pk.Met_Level != pk.Stat_Level && pk.Stat_Level > 1)
                return new LegalityCheck(Severity.Fishy, "All EVs are zero, but leveled above Met Level");
            if (EVs.Sum() == 508)
                return new LegalityCheck(Severity.Fishy, "2 EVs remaining.");
            if (EVs.Sum() > 510)
                return new LegalityCheck(Severity.Invalid, "EV total cannot be above 510.");
            if (EVs.Any(ev => ev > 252))
                return new LegalityCheck(Severity.Invalid, "EVs cannot go above 252.");
            if (EVs.All(ev => pk.EVs[0] == ev) && EVs[0] != 0)
                return new LegalityCheck(Severity.Fishy, "EVs are all equal.");

            return new LegalityCheck();
        }
        public static LegalityCheck verifyIVs(PK6 pk)
        {
            if (pk.IVs.Sum() == 0)
                return new LegalityCheck(Severity.Fishy, "All IVs are zero.");
            if (pk.IVs[0] < 30 && pk.IVs.All(iv => pk.IVs[0] == iv))
                return new LegalityCheck(Severity.Fishy, "All IVs are equal.");
            return new LegalityCheck();
        }
        public static LegalityCheck verifyID(PK6 pk)
        {
            if (pk.TID == 0 && pk.SID == 0)
                return new LegalityCheck(Severity.Fishy, "TID and SID are zero.");
            if (pk.TID == 0)
                return new LegalityCheck(Severity.Fishy, "TID is zero.");
            if (pk.SID == 0)
                return new LegalityCheck(Severity.Fishy, "SID is zero.");
            return new LegalityCheck();
        }
        public static LegalityCheck verifyEncounter(PK6 pk)
        {
            if (!pk.Gen6)
                return new LegalityCheck {Judgement = Severity.NotImplemented};

            if (pk.WasLink)
            {
                if (pk.FatefulEncounter || Legal.getLinkMoves(pk).Length == 0) // Should NOT be Fateful, and should be in Database
                    return new LegalityCheck(Severity.Invalid, "Not a valid Link gift.");
            }
            if (pk.WasEvent || pk.WasEventEgg)
            {
                var vwc6 = Legal.getValidWC6s(pk);
                try
                {
                    WC6 card = vwc6.First(wc6 => wc6.RelearnMoves.SequenceEqual(pk.RelearnMoves));
                    return new LegalityCheck(Severity.Valid, $"Matches #{card.CardID.ToString("0000")} ({card.CardTitle})");
                }
                catch { /* No card. */ }
                return new LegalityCheck(Severity.Invalid, "Not a valid Wonder Card gift.");
            }
            if (pk.WasEgg)
            {
                // Check Hatch Locations
                if (pk.Met_Level != 1)
                    return new LegalityCheck(Severity.Invalid, "Invalid met level, expected 1.");
                if (pk.IsEgg)
                {
                    var lc = pk.Met_Location == 0
                        ? new LegalityCheck(Severity.Valid, "Valid un-hatched egg.")
                        : new LegalityCheck(Severity.Invalid, "Invalid location for un-hatched egg (expected ID:0)");
                    return lc;
                }
                if (pk.Version < 26) // XY
                {
                    var lc = Legal.ValidMet_XY.Contains(pk.Met_Location)
                        ? new LegalityCheck(Severity.Valid, "Valid X/Y hatched egg.")
                        : new LegalityCheck(Severity.Invalid, "Invalid X/Y location for hatched egg.");
                    return lc;
                }
                if (pk.Version < 28)
                {
                    var lc = Legal.ValidMet_AO.Contains(pk.Met_Location)
                        ? new LegalityCheck(Severity.Valid, "Valid OR/AS hatched egg.")
                        : new LegalityCheck(Severity.Invalid, "Invalid OR/AS location for hatched egg.");
                    return lc;
                }
                return new LegalityCheck(Severity.Invalid, "Invalid location for hatched egg.");
            }


            EncounterStatic z = Legal.getStaticEncounter(pk);
            if (z != null)
                return new LegalityCheck(Severity.Valid, "Valid gift/static encounter.");

            int FriendSafari = Legal.getFriendSafariValid(pk);
            if (FriendSafari > 0)
                return new LegalityCheck(Severity.Valid, "Valid friend safari encounter.");

            // Not Implemented: In-Game Trades

            if (Legal.getWildEncounterValid(pk))
            {
                var lc = Legal.ValidMet_AO.Contains(pk.Met_Location)
                    ? new LegalityCheck(Severity.Valid, "Valid encounter at location.")
                    : new LegalityCheck(Severity.Invalid, "Hidden ability on valid encounter.");
                return lc;
            }
            return new LegalityCheck(Severity.Invalid, "Not a valid encounter.");
        }
        public static LegalityCheck[] verifyMoves(PK6 pk6)
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
                for (int i = 0; i < 4; i++)
                {
                    if (Moves[i] == Legal.Struggle)
                        res[i] = new LegalityCheck(Severity.Invalid, "Invalid Move: Struggle.");
                    else if (validMoves.Contains(Moves[i]))
                        res[i] = new LegalityCheck(Severity.Valid, "Level-up.");
                    else if (RelearnMoves.Contains(Moves[i]))
                        res[i] = new LegalityCheck(Severity.Valid, "Relearn Move.");
                    else
                        res[i] = new LegalityCheck(Severity.Invalid, "Invalid Move.");
                }
            }
            if (Moves[0] == 0)
                res[0] = new LegalityCheck(Severity.Invalid, "Invalid Move.");

            return res;
        }
        public static LegalityCheck[] verifyRelearn(PK6 pk6)
        {
            LegalityCheck[] res = new LegalityCheck[4];
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
                        return res;
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

using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX
{
    public class LegalityAnalysis
    {
        private readonly PK6 pk6;
        public bool[] vMoves = new bool[4];
        public bool[] vRelearn = new bool[4];
        public LegalityAnalysis(PK6 pk)
        {
            pk6 = pk;
            updateRelearnLegality();
            updateMoveLegality();
        }
        public void updateRelearnLegality()
        {
            vRelearn = getRelearnValidity(pk6.RelearnMoves);
        }
        public void updateMoveLegality()
        {
            vMoves = getMoveValidity(pk6.Moves, pk6.RelearnMoves);
        }

        public bool[] getMoveValidity(int[] Moves, int[] RelearnMoves)
        {
            if (Moves.Length != 4)
                return new bool[4];

            bool[] res = { true, true, true, true };
            if (!pk6.Gen6)
                return res;

            int[] validMoves = Legal.getValidMoves(pk6);
            if (pk6.Species == 235)
            {
                for (int i = 0; i < 4; i++)
                    res[i] = !Legal.InvalidSketch.Contains(Moves[i]);
            }
            else
            {
                for (int i = 0; i < 4; i++)
                    res[i] = Moves[i] != Legal.Struggle && validMoves.Concat(RelearnMoves).Contains(Moves[i]);
            }
            if (Moves[0] == 0)
                res[0] = false;

            return res;
        }
        public bool[] getRelearnValidity(int[] Moves)
        {
            bool[] res = {true, true, true, true};
            if (!pk6.Gen6)
                goto noRelearn;

            if (Moves.Length != 4)
                return new bool[4];
            if (pk6.WasLink)
            {
                if (pk6.FatefulEncounter) // Should NOT be Fateful
                    return new bool[4]; // False
                int[] moves = Legal.getLinkMoves(pk6);
                return moves.SequenceEqual(Moves) ? res : new bool[4];
            }
            if (pk6.WasEvent || pk6.WasEventEgg)
            {
                // Get WC6's that match
                IEnumerable<WC6> vwc6 = Legal.getValidWC6s(pk6);
                if (vwc6.Any(wc6 => wc6.RelearnMoves.SequenceEqual(Moves)))
                    return res; // all true

                goto noRelearn; // No WC match
            }

            int[] relearnMoves = Legal.getValidRelearn(pk6, 0);
            if (pk6.WasEgg)
            {
                if (Legal.SplitBreed.Contains(pk6.Species))
                {
                    res = new bool[4];
                    for (int i = 0; i < 4; i++)
                        res[i] = relearnMoves.Contains(Moves[i]);
                    if (!res.Any(move => !move))
                        return res;

                    // Try Next Species up
                    Legal.getValidRelearn(pk6, 1);
                    for (int i = 0; i < 4; i++)
                        res[i] = relearnMoves.Contains(Moves[i]);
                    return res;
                }

                if (Legal.LightBall.Contains(pk6.Species))
                    relearnMoves = relearnMoves.Concat(new[] {344}).ToArray();
                for (int i = 0; i < 4; i++)
                    res[i] &= relearnMoves.Contains(Moves[i]);
                return res;
            }
            if (Moves[0] != 0) // DexNav only?
            {
                // Check DexNav
                for (int i = 0; i < 4; i++)
                    res[i] &= Moves[i] == 0;
                if (Legal.getDexNavValid(pk6))
                    res[0] = relearnMoves.Contains(Moves[0]);

                return res;
            }

            // Should have no relearn moves.
          noRelearn:
            for (int i = 0; i < 4; i++)
                res[i] = Moves[i] == 0;
            return res;
        }
        
        public LegalityCheck EC, Nickname, PID, IDs, IVs, EVs, Encounter;
        public string Report => getLegalityReport();
        private string getLegalityReport()
        {
            if (!pk6.Gen6)
                return "Analysis only available for Pokémon that originate from X/Y & OR/AS.";

            EC = LegalityCheck.verifyECPID(pk6);
            Nickname = LegalityCheck.verifyNickname(pk6);
            PID = LegalityCheck.verifyECPID(pk6);
            IVs = LegalityCheck.verifyIVs(pk6);
            EVs = LegalityCheck.verifyEVs(pk6);
            IDs = LegalityCheck.verifyID(pk6);
            Encounter = LegalityCheck.verifyEncounter(pk6);

            var chks = new[] {EC, Nickname, PID, IVs, EVs, IDs, Encounter};

            string r = "";
            for (int i = 0; i < 4; i++)
                if (!vMoves[i])
                    r += $"Invalid: Move {i + 1}{Environment.NewLine}";
            for (int i = 0; i < 4; i++)
                if (!vRelearn[i])
                    r += $"Invalid: Relearn Move {i + 1}{Environment.NewLine}";

            if (r.Length == 0 && chks.All(chk => chk.Valid))
                return "Legal!";

            // Build result string...
            r += chks.Where(chk => !chk.Valid).Aggregate("", (current, chk) => current + $"{chk.Judgement}: {chk.Comment}{Environment.NewLine}");

            return r;
        }
    }
}

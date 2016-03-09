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
        public Severity Judgement = Severity.Invalid;
        public string Comment;
        public bool Valid => Judgement >= Severity.Fishy;

        public LegalityCheck() { }
        public LegalityCheck(Severity s, string c)
        {
            Judgement = s;
            Comment = c;
        }
    }
    public class LegalityAnalysis
    {
        private readonly PK6 pk6;
        public LegalityAnalysis(PK6 pk)
        {
            pk6 = pk;
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

            bool link = pk6.Met_Location == 30011;
            if (link)
            {
                if (pk6.FatefulEncounter) // Should NOT be Fateful
                    return new bool[4]; // False
                int[] moves = Legal.getLinkMoves(pk6);
                return moves.SequenceEqual(Moves) ? res : new bool[4];
            }
            bool egg = Legal.EggLocations.Contains(pk6.Egg_Location) && pk6.Met_Level == 1;
            bool evnt = pk6.FatefulEncounter && pk6.Met_Location > 40000;
            bool eventEgg = pk6.FatefulEncounter && (pk6.Egg_Location > 40000 || pk6.Egg_Location == 30002) && pk6.Met_Level == 1;
            int[] relearnMoves = Legal.getValidRelearn(pk6, 0);
            if (evnt || eventEgg)
            {
                // Get WC6's that match
                IEnumerable<WC6> vwc6 = Legal.getValidWC6s(pk6);
                if (vwc6.Any(wc6 => wc6.RelearnMoves.SequenceEqual(Moves)))
                    return res; // all true
            }
            else if (egg)
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
            else if (Moves[0] != 0) // DexNav only?
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

        public bool Valid = false;
        public LegalityCheck EC, Nickname, PID, IDs, IVs, EVs;
        public string Report => getLegalityReport();
        private string getLegalityReport()
        {
            return null;
        }
    }
}

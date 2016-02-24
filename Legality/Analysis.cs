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
        public bool Valid = false;
        public LegalityCheck EC, Nickname, PID, IDs, IVs, EVs;
        public int[] ValidMoves => Legal.getValidMoves(pk6.Species, pk6.CurrentLevel);
        public int[] ValidRelearnMoves => Legal.getValidRelearn(pk6.Species);
        public string Report => getLegalityReport();

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

            if (pk6.Species == 235)
            {
                for (int i = 0; i < 4; i++)
                    res[i] = !Legal.InvalidSketch.Contains(Moves[i]);
            }
            else
            {
                for (int i = 0; i < 4; i++)
                    res[i] = Moves[i] != Legal.Struggle && ValidMoves.Concat(RelearnMoves).Contains(Moves[i]);
            }
            if (Moves[0] == 0)
                res[0] = false;

            return res;
        }
        public bool[] getRelearnValidity(int[] Moves)
        {
            if (Moves.Length != 4)
                return new bool[4];

            bool[] res = {true, true, true, true};
            if (!pk6.Gen6)
                goto noRelearn;

            bool egg = pk6.Egg_Location > 1000 || pk6.Egg_Location == 318;
            bool evnt = pk6.Met_Location > 40000 || pk6.Egg_Location > 40000;
            int[] relearnMoves = ValidRelearnMoves;
            if (evnt)
            {
                // Check Event Info
                // Not Implemented
            }
            if (egg)
            {
                for (int i = 0; i < 4; i++)
                    res[i] &= relearnMoves.Contains(Moves[i]);
                return res;
            }
            bool dexnav = false;
            if (dexnav) // not implemented
            {
                return res;
            }

            // Should have no relearn moves.
          noRelearn:
            for (int i = 0; i < 4; i++)
                res[i] &= Moves[i] == 0;
            return res;
        }
        private string getLegalityReport()
        {
            return null;
        }
    }
}

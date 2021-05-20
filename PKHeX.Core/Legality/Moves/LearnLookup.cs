using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.Legal;

namespace PKHeX.Core
{
    /// <summary>
    /// Level-Up Lookup object
    /// </summary>
    public sealed class LearnLookup
    {
        private readonly GameVersion Version;
        private readonly PersonalTable Table;
        private readonly Learnset[] Learn;

        public LearnLookup(PersonalTable table, Learnset[] learn, GameVersion version)
        {
            Version = version;
            Table = table;
            Learn = learn;
        }

        public List<int> AddMovesIndex(List<int> moves, int index, int max, int min)
        {
            if (index <= 0)
                return moves;
            return Learn[index].AddMoves(moves, max, min);
        }

        public List<int> AddMoves(List<int> moves, int species, int form, int max, int min = 0)
        {
            int index = Table.GetFormIndex(species, form);
            return AddMovesIndex(moves, index, max, min);
        }

        public List<int> AddMoves1(List<int> moves, int species, int form, int max, int min)
        {
            int index = Table.GetFormIndex(species, form);
            return AddMovesIndex1(moves, index, max, min);
        }

        public List<int> AddMovesIndex1(List<int> moves, int index, int max, int min)
        {
            if (min == 1)
                moves.AddRange(((PersonalInfoG1)Table[index]).Moves);
            return AddMovesIndex(moves, index, max, min);
        }

        public List<int> GetMoves(int species, int form, int min, int max)
        {
            int index = Table.GetFormIndex(species, form);
            return Learn[index].GetMoveList(max, min);
        }

        public LearnVersion GetIsLevelUp(int species, int form, int move)
        {
            int index = Table.GetFormIndex(species, form);
            if (index <= 0)
                return LearnNONE;
            var lv = Learn[index].GetLevelLearnMove(move);
            if (lv >= 0)
                return new LearnVersion(lv, Version);
            return LearnNONE;
        }

        public LearnVersion GetIsLevelUp(int species, int form, int move, int max)
        {
            int index = Table.GetFormIndex(species, form);
            if (index <= 0)
                return LearnNONE;
            var lv = Learn[index].GetLevelLearnMove(move);
            if (lv >= 0 && lv <= max)
                return new LearnVersion(lv, Version);
            return LearnNONE;
        }

        public LearnVersion GetIsLevelUpMin(int species, int move, int max, int min, int form)
        {
            int index = Table.GetFormIndex(species, form);
            if (index <= 0)
                return LearnNONE;
            var lv = Learn[index].GetLevelLearnMove(move, min);
            if (lv >= min && lv <= max)
                return new LearnVersion(lv, Version);
            return LearnNONE;
        }

        public LearnVersion GetIsLevelUpG1(int species, int form, int move, int max, int min = 0)
        {
            int index = PersonalTable.RB.GetFormIndex(species, form);
            if (index == 0)
                return LearnNONE;

            // No Move re-learner -- have to be learned on level-up
            var lv = Learn[index].GetLevelLearnMove(move, min);
            if (lv >= 0 && lv <= max)
                return new LearnVersion(lv, Version);

            if (min >= 1)
                return LearnNONE;

            var pi = (PersonalInfoG1)Table[index];
            var i = Array.IndexOf(pi.Moves, move);

            // Check if move was not overwritten by higher level moves before it was encountered
            if (i >= 0)
            {
                var unique = Learn[index].GetUniqueMovesLearned(pi.Moves.Where(z => z != 0), max);
                if (unique.Count - i <= 4)
                    return new LearnVersion(0, Version);
            }
            return LearnNONE;
        }
    }
}

using System.Collections.Generic;
using System.Linq;

using static PKHeX.Core.Legal;

namespace PKHeX.Core
{
    internal static class MoveLevelUp
    {
        public static LearnVersion GetIsLevelUpMove(PKM pkm, int species, int minlvlG1, int minlvlG2, int lvl, int form, int Generation, int move, GameVersion Version = GameVersion.Any)
        {
            switch (Generation)
            {
                case 1: return GetIsLevelUp1(species, move, lvl, minlvlG1);
                case 2: return GetIsLevelUp2(species, move, lvl, minlvlG2, pkm.Korean);
                case 3: return GetIsLevelUp3(species, move, lvl, form);
                case 4: return GetIsLevelUp4(species, move, lvl, form);
                case 5: return GetIsLevelUp5(species, move, lvl, form);
                case 6: return GetIsLevelUp6(species, move, lvl, form, Version);
                case 7: return GetIsLevelUp7(species, move, form, Version);
            }
            return LearnNONE;
        }

        private static LearnVersion GetIsLevelUp1(int species, int move, int max, int min)
        {
            int index = PersonalTable.RB.GetFormeIndex(species, 0);
            if (index == 0)
                return LearnNONE;

            if (min == 1)
            {
                var pi_rb = (PersonalInfoG1)PersonalTable.RB[index];
                var pi_y = (PersonalInfoG1)PersonalTable.Y[index];
                if (pi_rb.Moves.Contains(move))
                    return new LearnVersion(0, GameVersion.RB);
                if (pi_y.Moves.Contains(move))
                    return new LearnVersion(0, GameVersion.YW);
            }

            // No relearner -- have to be learned on levelup
            var lv = LevelUpRB[index].GetLevelLearnMove(move, min);
            if (lv >= 0 && lv <= max)
                return new LearnVersion(lv, GameVersion.RB);

            // No relearner -- have to be learned on levelup
            lv = LevelUpY[index].GetLevelLearnMove(move, min);
            if (lv >= 0 && lv <= max)
                return new LearnVersion(lv, GameVersion.YW);

            return LearnNONE;
        }
        private static LearnVersion GetIsLevelUp2(int species, int move, int max, int min, bool korean)
        {
            if (move > MaxMoveID_1)
                return LearnNONE;

            int index = PersonalTable.C.GetFormeIndex(species, 0);
            if (index == 0)
                return LearnNONE;

            // No relearner -- have to be learned on levelup
            var lv = LevelUpGS[index].GetLevelLearnMove(move, min);
            if (lv >= 0 && lv <= max)
                return new LearnVersion(lv, GameVersion.GS);

            if (korean) // No Korean Crystal
                return LearnNONE;

            // No relearner -- have to be learned on levelup
            lv = LevelUpC[index].GetLevelLearnMove(move, min);
            if (lv >= 0 && lv <= max)
                return new LearnVersion(lv, GameVersion.C);

            return LearnNONE;
        }
        private static LearnVersion GetIsLevelUp3(int species, int move, int lvl, int form)
        {

            int index = PersonalTable.E.GetFormeIndex(species, 0);
            if (index == 0)
                return LearnNONE;
            if (index == 386)
                return GetIsLevelUp3Deoxys(form, move, lvl);

            // Emerald level up table are equals to R/S level up tables
            var lv = LevelUpE[index].GetLevelLearnMove(move);
            if (lv >= 0 && lv <= lvl)
                return new LearnVersion(lv, GameVersion.RSE);

            // fire red and leaf green are equals between each other but different than RSE
            // Do not use FR Levelup table. It have 67 moves for charmander but Leaf Green moves table is correct
            lv = LevelUpLG[index].GetLevelLearnMove(move);
            if (lv >= 0 && lv <= lvl)
                return new LearnVersion(lv, GameVersion.FRLG);

            return LearnNONE;
        }
        private static LearnVersion GetIsLevelUp4(int species, int move, int lvl, int form)
        {
            int index = PersonalTable.HGSS.GetFormeIndex(species, form);
            if (index == 0)
                return LearnNONE;

            var lv = LevelUpDP[index].GetLevelLearnMove(move);
            if (lv >= 0 && lv <= lvl)
                return new LearnVersion(lv, GameVersion.DP);

            lv = LevelUpPt[index].GetLevelLearnMove(move);
            if (lv >= 0 && lv <= lvl)
                return new LearnVersion(lv, GameVersion.Pt);

            lv = LevelUpHGSS[index].GetLevelLearnMove(move);
            if (lv >= 0 && lv <= lvl)
                return new LearnVersion(lv, GameVersion.HGSS);

            return LearnNONE;
        }
        private static LearnVersion GetIsLevelUp5(int species, int form, int move, int lvl)
        {
            int index1 = PersonalTable.BW.GetFormeIndex(species, form);
            int index2 = PersonalTable.B2W2.GetFormeIndex(species, form);
            if (index1 == 0 && index2 == 0)
                return LearnNONE;
            if (index1 != 0)
            {
                var lv = LevelUpBW[index1].GetLevelLearnMove(move);
                if (lv >= 0 && lv <= lvl)
                    return new LearnVersion(lv, GameVersion.BW);
            }
            if (index2 != 0)
            {
                var lv = LevelUpB2W2[index2].GetLevelLearnMove(move);
                if (lv >= 0 && lv <= lvl)
                    return new LearnVersion(lv, GameVersion.B2W2);
            }
            return LearnNONE;
        }
        private static LearnVersion GetIsLevelUp6(int species, int move, int lvl, int form, GameVersion ver)
        {
            switch (ver)
            {
                case GameVersion.SN:
                case GameVersion.MN:
                case GameVersion.SM:
                    return GetIsLevelUp6XY(species, form, move, lvl);
                case GameVersion.Any:
                case GameVersion.US:
                case GameVersion.UM:
                case GameVersion.USUM:
                    return GetIsLevelUp6AO(species, form, move, lvl);
            }
            return LearnNONE;
        }
        private static LearnVersion GetIsLevelUp7(int species, int move, int form, GameVersion ver)
        {
            switch (ver)
            {
                case GameVersion.SN:
                case GameVersion.MN:
                case GameVersion.SM:
                    return GetIsLevelUp7SM(species, form, move);
                case GameVersion.Any:
                case GameVersion.US:
                case GameVersion.UM:
                case GameVersion.USUM:
                    return GetIsLevelUp7USUM(species, form, move);
            }
            return LearnNONE;
        }

        private static LearnVersion GetIsLevelUp3Deoxys(int form, int move, int lvl)
        {
            var moveset = GetDeoxysLearn3(form);
            if (moveset == null)
                return LearnNONE;
            var lv = moveset.GetLevelLearnMove(move);
            if (lv >= 0 && lv <= lvl)
                return new LearnVersion(lv, GetDeoxysGameVersion3(form));
            return LearnNONE;
        }
        private static LearnVersion GetIsLevelUp6XY(int species, int form, int move, int lvl)
        {
            int index = PersonalTable.XY.GetFormeIndex(species, form);
            if (index == 0)
                return LearnNONE;

            var lv = LevelUpXY[index].GetLevelLearnMove(move);
            if (lv >= 0 && lv <= lvl)
                return new LearnVersion(lv, GameVersion.XY);

            return LearnNONE;
        }
        private static LearnVersion GetIsLevelUp6AO(int species, int form, int move, int lvl)
        {
            int index = PersonalTable.AO.GetFormeIndex(species, form);
            if (index == 0)
                return LearnNONE;

            var lv = LevelUpAO[index].GetLevelLearnMove(move);
            if (lv >= 0 && lv <= lvl)
                return new LearnVersion(lv, GameVersion.ORAS);

            return GetIsLevelUp7SM(species, form, move);
        }
        private static LearnVersion GetIsLevelUp7SM(int species, int form, int move)
        {
            if (species > MaxSpeciesID_7)
                return LearnNONE;
            int index = PersonalTable.SM.GetFormeIndex(species, form);
            if (index == 0)
                return LearnNONE;

            var lv = LevelUpSM[index].GetLevelLearnMove(move);
            if (lv >= 0)
                return new LearnVersion(lv, GameVersion.SM);

            return LearnNONE;
        }
        private static LearnVersion GetIsLevelUp7USUM(int species, int form, int move)
        {
            if (species > MaxSpeciesID_7_USUM)
                return LearnNONE;
            int index = PersonalTable.USUM.GetFormeIndex(species, form);
            if (index == 0)
                return LearnNONE;

            var lv = LevelUpUSUM[index].GetLevelLearnMove(move);
            if (lv >= 0)
                return new LearnVersion(lv, GameVersion.USUM);

            return GetIsLevelUp7SM(species, form, move);
        }

        private static GameVersion GetDeoxysGameVersion3(int form)
        {
            switch (form)
            {
                case 0: return GameVersion.RS;
                case 1: return GameVersion.FR;
                case 2: return GameVersion.LG;
                case 3: return GameVersion.E;
                default:
                    return GameVersion.Invalid;
            }
        }
        private static Learnset GetDeoxysLearn3(int form)
        {
            switch (form)
            {
                case 0: return LevelUpRS[386]; // Normal
                case 1: return LevelUpFR[386]; // Attack
                case 2: return LevelUpLG[386]; // Defense
                case 3: return LevelUpE[386]; // Speed
                default: return null;
            }
        }

        public static IEnumerable<int> GetMovesLevelUp(PKM pkm, int species, int minlvlG1, int minlvlG2, int lvl, int form, GameVersion version, bool MoveReminder, int Generation)
        {
            switch (Generation)
            {
                case 1: return GetMovesLevelUp1(species, lvl, minlvlG1);
                case 2: return GetMovesLevelUp2(species, lvl, minlvlG2, pkm.Korean, pkm.Format);
                case 3: return GetMovesLevelUp3(species, lvl, form);
                case 4: return GetMovesLevelUp4(species, lvl, form);
                case 5: return GetMovesLevelUp5(species, lvl, form);
                case 6: return GetMovesLevelUp6(species, lvl, form, version);
                case 7: return GetMovesLevelUp7(species, lvl, form, version, MoveReminder);
            }
            return null;
        }

        internal static List<int> GetMovesLevelUp1(int species, int max, int min)
        {
            List<int> moves = new List<int>();
            int index = PersonalTable.RB.GetFormeIndex(species, 0);
            if (index == 0)
                return moves;

            if (min == 1)
            {
                var pi_rb = (PersonalInfoG1)PersonalTable.RB[index];
                var pi_y = (PersonalInfoG1)PersonalTable.Y[index];
                moves.AddRange(pi_rb.Moves);
                moves.AddRange(pi_y.Moves);
            }
            moves.AddRange(LevelUpRB[index].GetMoves(max, min));
            moves.AddRange(LevelUpY[index].GetMoves(max, min));
            return moves;
        }
        private static List<int> GetMovesLevelUp2(int species, int max, int min, bool korean, int format)
        {
            var r = new List<int>();
            int index = PersonalTable.C.GetFormeIndex(species, 0);
            if (index == 0)
                return r;
            r.AddRange(LevelUpGS[index].GetMoves(max, min));
            if (!korean)
                r.AddRange(LevelUpC[index].GetMoves(max, min));
            if (format == 1) //tradeback gen 2 -> gen 1
                r = r.Where(m => m <= MaxMoveID_1).ToList();
            return r;
        }
        private static List<int> GetMovesLevelUp3(int species, int lvl, int form)
        {
            var moves = new List<int>();
            if (species == 386)
            {
                var learn = GetDeoxysLearn3(form);
                if (learn != null)
                    moves.AddRange(learn.GetMoves(lvl));
                return moves;
            }

            int index = PersonalTable.E.GetFormeIndex(species, 0);
            if (index == 0)
                return moves;

            // Emerald level up table are equals to R/S level up tables
            moves.AddRange(LevelUpE[index].GetMoves(lvl));
            // fire red and leaf green are equals between each other but different than RSE
            // Do not use FR Levelup table. It have 67 moves for charmander but Leaf Green moves table is correct
            moves.AddRange(LevelUpLG[index].GetMoves(lvl));
            return moves;
        }
        private static List<int> GetMovesLevelUp4(int species, int lvl, int form)
        {
            var moves = new List<int>();
            int index = PersonalTable.HGSS.GetFormeIndex(species, form);
            if (index == 0)
                return moves;
            if (index < LevelUpDP.Length)
                moves.AddRange(LevelUpDP[index].GetMoves(lvl));
            moves.AddRange(LevelUpPt[index].GetMoves(lvl));
            moves.AddRange(LevelUpHGSS[index].GetMoves(lvl));
            return moves;
        }
        private static List<int> GetMovesLevelUp5(int species, int lvl, int form)
        {
            var moves = new List<int>();
            int index1 = PersonalTable.BW.GetFormeIndex(species, form);
            if (index1 != 0)
                moves.AddRange(LevelUpBW[index1].GetMoves(lvl));

            int index2 = PersonalTable.B2W2.GetFormeIndex(species, form);
            if (index2 != 0)
                moves.AddRange(LevelUpB2W2[index2].GetMoves(lvl));
            return moves;
        }
        private static List<int> GetMovesLevelUp6(int species, int lvl, int form, GameVersion ver)
        {
            var moves = new List<int>();
            switch (ver)
            {
                case GameVersion.Any:
                    AddMovesLevelUp6XY(moves, species, lvl, form);
                    AddMovesLevelUp6AO(moves, species, lvl, form);
                    break;
                case GameVersion.X:
                case GameVersion.Y:
                case GameVersion.XY:
                    AddMovesLevelUp6XY(moves, species, lvl, form);
                    break;
                case GameVersion.AS:
                case GameVersion.OR:
                case GameVersion.ORAS:
                    AddMovesLevelUp6AO(moves, species, lvl, form);
                    break;
            }
            return moves;
        }
        private static void AddMovesLevelUp6XY(List<int> moves, int species, int lvl, int form)
        {
            int index = PersonalTable.XY.GetFormeIndex(species, form);
            if (index == 0)
                return;
            moves.AddRange(LevelUpXY[index].GetMoves(lvl));
        }
        private static void AddMovesLevelUp6AO(List<int> moves, int species, int lvl, int form)
        {
            int index = PersonalTable.AO.GetFormeIndex(species, form);
            if (index == 0)
                return;
            moves.AddRange(LevelUpAO[index].GetMoves(lvl));
        }
        private static List<int> GetMovesLevelUp7(int species, int lvl, int form, GameVersion ver, bool MoveReminder)
        {
            var moves = new List<int>();
            switch (ver)
            {
                case GameVersion.Any:
                    AddMovesLevelUp7SM(moves, species, lvl, form, MoveReminder);
                    AddMovesLevelUp7USUM(moves, species, lvl, form, MoveReminder);
                    break;
                case GameVersion.SN:
                case GameVersion.MN:
                case GameVersion.SM:
                    AddMovesLevelUp7SM(moves, species, lvl, form, MoveReminder);
                    break;
                case GameVersion.US:
                case GameVersion.UM:
                case GameVersion.USUM:
                    AddMovesLevelUp7USUM(moves, species, lvl, form, MoveReminder);
                    break;
            }
            return moves;
        }
        private static void AddMovesLevelUp7SM(List<int> moves, int species, int lvl, int form, bool MoveReminder)
        {
            if (species > MaxSpeciesID_7)
                return;
            int index = PersonalTable.SM.GetFormeIndex(species, form);
            if (MoveReminder)
                lvl = 100; // Move reminder can teach any level in movepool now!

            moves.AddRange(LevelUpSM[index].GetMoves(lvl));
        }
        private static void AddMovesLevelUp7USUM(List<int> moves, int species, int lvl, int form, bool MoveReminder)
        {
            if (species > MaxSpeciesID_7_USUM)
                return;
            int index = PersonalTable.USUM.GetFormeIndex(species, form);
            if (MoveReminder)
                lvl = 100; // Move reminder can teach any level in movepool now!

            moves.AddRange(LevelUpUSUM[index].GetMoves(lvl));
        }

        public static int[] GetEncounterMoves(PKM pk, int level, GameVersion version)
        {
            return GetEncounterMoves(pk.Species, pk.AltForm, level, version);
        }
        public static int[] GetEncounterMoves(int species, int form, int level, GameVersion version)
        {
            var learn = GameData.GetLearnsets(version);
            var table = GameData.GetPersonal(version);
            var index = table.GetFormeIndex(species, form);
            return learn[index].GetEncounterMoves(level);
        }
    }
}

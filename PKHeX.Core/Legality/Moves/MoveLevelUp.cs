using System;
using System.Collections.Generic;
using System.Linq;

using static PKHeX.Core.Legal;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core
{
    internal static class MoveLevelUp
    {
        private static readonly LearnLookup LearnSM, LearnUSUM,
            LearnXY, LearnAO,
            LearnBW, LearnB2W2,
            LearnDP, LearnPt, LearnHGSS,
            LearnRSE, LearnFRLG,
            LearnGS, LearnC,
            LearnRB, LearnY;

        static MoveLevelUp()
        {
            LearnSM = new LearnLookup(PersonalTable.SM, LevelUpSM, SM);
            LearnUSUM = new LearnLookup(PersonalTable.USUM, LevelUpUSUM, USUM);
            LearnXY = new LearnLookup(PersonalTable.XY, LevelUpXY, XY);
            LearnAO = new LearnLookup(PersonalTable.AO, LevelUpAO, ORAS);
            LearnBW = new LearnLookup(PersonalTable.BW, LevelUpBW, BW);
            LearnB2W2 = new LearnLookup(PersonalTable.B2W2, LevelUpB2W2, B2W2);
            LearnDP = new LearnLookup(PersonalTable.DP, LevelUpDP, DP);
            LearnPt = new LearnLookup(PersonalTable.Pt, LevelUpPt, Pt);
            LearnHGSS = new LearnLookup(PersonalTable.HGSS, LevelUpHGSS, HGSS);
            LearnRSE = new LearnLookup(PersonalTable.RS, LevelUpRS, RSE);
            LearnFRLG = new LearnLookup(PersonalTable.LG, LevelUpLG, FRLG);
            LearnGS = new LearnLookup(PersonalTable.GS, LevelUpGS, GS);
            LearnC = new LearnLookup(PersonalTable.C, LevelUpC, C);
            LearnRB = new LearnLookup(PersonalTable.RB, LevelUpRB, RB);
            LearnY = new LearnLookup(PersonalTable.Y, LevelUpY, YW);
        }

        public static LearnVersion GetIsLevelUpMove(PKM pkm, int species, int minlvlG1, int minlvlG2, int lvl, int form, int Generation, int move, GameVersion Version = Any)
        {
            switch (Generation)
            {
                case 1: return GetIsLevelUp1(species, move, lvl, minlvlG1, Version);
                case 2: return GetIsLevelUp2(species, move, lvl, minlvlG2, pkm.Korean, Version);
                case 3: return GetIsLevelUp3(species, move, lvl, form, Version);
                case 4: return GetIsLevelUp4(species, move, lvl, form, Version);
                case 5: return GetIsLevelUp5(species, move, lvl, form, Version);
                case 6: return GetIsLevelUp6(species, move, lvl, form, Version);
                case 7: return GetIsLevelUp7(species, move, form, Version);
            }
            return LearnNONE;
        }

        private static LearnVersion GetIsLevelUp1(int species, int move, int max, int min, GameVersion ver = Any)
        {
            switch (ver)
            {
                case Any:
                case RBY:
                    var first = LearnRB.GetIsLevelUpG1(species, move, max, min);
                    if (first.IsLevelUp)
                        return first;
                    return LearnY.GetIsLevelUpG1(species, move, max, min);

                case RD: case BU: case GN: case RB:
                    return LearnRB.GetIsLevelUpG1(species, move, max, min);
                case YW:
                    return LearnY.GetIsLevelUpG1(species, move, max, min);
            }

            return LearnNONE;
        }
        private static LearnVersion GetIsLevelUp2(int species, int move, int max, int min, bool korean, GameVersion ver = Any)
        {
            if (move > MaxMoveID_1)
                return LearnNONE;

            // No Korean Crystal
            switch (ver)
            {
                case Any:
                case GSC:
                    var first = LearnGS.GetIsLevelUpMin(species, move, max, min);
                    if (first.IsLevelUp || korean)
                        return first;
                    return LearnC.GetIsLevelUpMin(species, move, max, min);

                case GD: case SV: case GS:
                    return LearnGS.GetIsLevelUpMin(species, move, max, min);
                case C when !korean:
                    return LearnC.GetIsLevelUpMin(species, move, max, min);
            }
            return LearnNONE;
        }
        private static LearnVersion GetIsLevelUp3(int species, int move, int lvl, int form, GameVersion ver = Any)
        {
            if (species == 386)
                return GetIsLevelUp3Deoxys(form, move, lvl);

            // Emerald level up tables are equal to R/S level up tables
            switch (ver)
            {
                case Any:
                    var first = LearnRSE.GetIsLevelUp(species, form, move, lvl);
                    if (first.IsLevelUp)
                        return first;
                    return LearnFRLG.GetIsLevelUp(species, form, move, lvl);

                case R: case S: case E: case RS: case RSE:
                    return LearnRSE.GetIsLevelUp(species, form, move, lvl);
                case FR: case LG: case FRLG:
                    return LearnFRLG.GetIsLevelUp(species, form, move, lvl);
            }
            return LearnNONE;
        }
        private static LearnVersion GetIsLevelUp4(int species, int move, int lvl, int form, GameVersion ver = Any)
        {
            switch (ver)
            {
                case Any:
                case DPPt:
                    var first = LearnDP.GetIsLevelUp(species, form, move, lvl);
                    if (first.IsLevelUp)
                        return first;
                    var second = LearnPt.GetIsLevelUp(species, form, move, lvl);
                    if (second.IsLevelUp)
                        return second;
                    if (ver == DPPt) // stop here
                        return LearnNONE;
                    return LearnHGSS.GetIsLevelUp(species, form, move, lvl);

                case D: case P: case DP:
                    return LearnDP.GetIsLevelUp(species, form, move, lvl);
                case Pt:
                    return LearnPt.GetIsLevelUp(species, form, move, lvl);
                case HG: case SS: case HGSS:
                    return LearnHGSS.GetIsLevelUp(species, form, move, lvl);
            }
            return LearnNONE;
        }
        private static LearnVersion GetIsLevelUp5(int species, int form, int move, int lvl, GameVersion ver = Any)
        {
            switch (ver)
            {
                case Any:
                    var first = LearnBW.GetIsLevelUp(species, form, move, lvl);
                    if (first.IsLevelUp)
                        return first;
                    return LearnB2W2.GetIsLevelUp(species, form, move, lvl);
                case B: case W: case BW:
                    return LearnBW.GetIsLevelUp(species, form, move, lvl);
                case B2: case W2: case B2W2:
                    return LearnB2W2.GetIsLevelUp(species, form, move, lvl);
            }
            return LearnNONE;
        }
        private static LearnVersion GetIsLevelUp6(int species, int move, int lvl, int form, GameVersion ver = Any)
        {
            switch (ver)
            {
                case Any:
                    var first = LearnXY.GetIsLevelUp(species, form, move, lvl);
                    if (first.IsLevelUp)
                        return first;
                    return LearnAO.GetIsLevelUp(species, form, move, lvl);

                case X: case Y: case XY:
                    return LearnXY.GetIsLevelUp(species, form, move, lvl);
                case OR: case AS: case ORAS:
                    return LearnAO.GetIsLevelUp(species, form, move, lvl);
            }
            return LearnNONE;
        }
        private static LearnVersion GetIsLevelUp7(int species, int move, int form, GameVersion ver = Any)
        {
            switch (ver)
            {
                case Any:
                    if (species > MaxSpeciesID_7)
                        return LearnNONE;
                    var first = LearnSM.GetIsLevelUp(species, form, move);
                    if (first.IsLevelUp)
                        return first;
                    if (species > MaxSpeciesID_7_USUM)
                        return LearnNONE;
                    return LearnUSUM.GetIsLevelUp(species, form, move);

                case SN: case MN: case SM:
                    if (species > MaxSpeciesID_7)
                        return LearnNONE;
                    return LearnSM.GetIsLevelUp(species, form, move);

                case US: case UM: case USUM:
                    if (species > MaxSpeciesID_7_USUM)
                        return LearnNONE;
                    return LearnUSUM.GetIsLevelUp(species, form, move);
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

        private static GameVersion GetDeoxysGameVersion3(int form)
        {
            switch (form)
            {
                case 0: return RS;
                case 1: return FR;
                case 2: return LG;
                case 3: return E;
                default:
                    return Invalid;
            }
        }
        private static Learnset GetDeoxysLearn3(int form, GameVersion ver = Any)
        {
            if (ver == Any)
            switch (form)
            {
                case 0: return LevelUpRS[386]; // Normal
                case 1: return LevelUpFR[386]; // Attack
                case 2: return LevelUpLG[386]; // Defense
                case 3: return LevelUpE[386]; // Speed
                default: return null;
            }
            var gen = ver.GetGeneration();
            if (gen != 3)
                return GetDeoxysLearn3(form);
            return GameData.GetLearnsets(ver)[386];
        }

        public static IEnumerable<int> GetMovesLevelUp(PKM pkm, int species, int minlvlG1, int minlvlG2, int lvl, int form, GameVersion version, bool MoveReminder, int Generation)
        {
            switch (Generation)
            {
                case 1: return GetMovesLevelUp1(species, lvl, minlvlG1, version);
                case 2: return GetMovesLevelUp2(species, lvl, version, minlvlG2, pkm.Korean, pkm.Format);
                case 3: return GetMovesLevelUp3(species, lvl, version, form);
                case 4: return GetMovesLevelUp4(species, lvl, version, form);
                case 5: return GetMovesLevelUp5(species, lvl, version, form);
                case 6: return GetMovesLevelUp6(species, lvl, version, form);
                case 7: return GetMovesLevelUp7(species, lvl, version, form, MoveReminder);
            }
            return null;
        }

        internal static List<int> GetMovesLevelUp1(int species, int max, int min, GameVersion ver = Any)
        {
            var moves = new List<int>();
            int index = PersonalTable.RB.GetFormeIndex(species, 0);
            if (index == 0)
                return moves;

            switch (ver)
            {
                case Any: case RBY:
                    AddMovesLevelUp1RBG(moves, max, min, index);
                    AddMovesLevelUp1Y(moves, max, min, index);
                    return moves;
                case RB:
                case RD: case BU: case GN:
                    AddMovesLevelUp1RBG(moves, max, min, index);
                    return moves;
                case YW:
                    AddMovesLevelUp1Y(moves, max, min, index);
                    return moves;
            }
            AddMovesLevelUp1RBG(moves, max, min, index);
            return moves;
        }

        private static List<int> GetMovesLevelUp2(int species, int max, GameVersion ver, int min, bool korean, int format)
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
        private static List<int> GetMovesLevelUp3(int species, int lvl, GameVersion ver, int form)
        {
            var moves = new List<int>();
            if (species == 386)
            {
                var learn = GetDeoxysLearn3(form, ver);
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
        private static List<int> GetMovesLevelUp4(int species, int lvl, GameVersion ver, int form)
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
        private static List<int> GetMovesLevelUp5(int species, int lvl, GameVersion ver, int form)
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
        private static List<int> GetMovesLevelUp6(int species, int lvl, GameVersion ver, int form)
        {
            var moves = new List<int>();
            switch (ver)
            {
                case Any:
                    AddMovesLevelUp6XY(moves, species, lvl, form);
                    AddMovesLevelUp6AO(moves, species, lvl, form);
                    break;
                case X: case Y: case XY:
                    AddMovesLevelUp6XY(moves, species, lvl, form);
                    break;
                case AS: case OR: case ORAS:
                    AddMovesLevelUp6AO(moves, species, lvl, form);
                    break;
            }
            return moves;
        }


        private static void AddMovesLevelUp1RBG(List<int> moves, int max, int min, int index)
        {
            if (min == 1)
                moves.AddRange(((PersonalInfoG1)PersonalTable.RB[index]).Moves);
            moves.AddRange(LevelUpRB[index].GetMoves(max, min));
        }
        private static void AddMovesLevelUp1Y(List<int> moves, int max, int min, int index)
        {
            if (min == 1)
                moves.AddRange(((PersonalInfoG1)PersonalTable.Y[index]).Moves);
            moves.AddRange(LevelUpY[index].GetMoves(max, min));
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
        private static List<int> GetMovesLevelUp7(int species, int lvl, GameVersion ver, int form, bool MoveReminder)
        {
            var moves = new List<int>();
            switch (ver)
            {
                case Any:
                    AddMovesLevelUp7SM(moves, species, lvl, form, MoveReminder);
                    AddMovesLevelUp7USUM(moves, species, lvl, form, MoveReminder);
                    break;
                case SN: case MN: case SM:
                    AddMovesLevelUp7SM(moves, species, lvl, form, MoveReminder);
                    break;
                case US: case UM: case USUM:
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
            if (RBY.Contains(version))
                return GetEncounterMoves1(pk.Species, level, version);
            return GetEncounterMoves(pk.Species, pk.AltForm, level, version);
        }

        private static int[] GetEncounterMoves1(int species, int level, GameVersion version)
        {
            var learn = GameData.GetLearnsets(version);
            var table = GameData.GetPersonal(version);
            var index = table.GetFormeIndex(species, 0);
            var lvl0 = (int[])((PersonalInfoG1) table[index]).Moves.Clone();
            int start = Math.Max(0, Array.FindIndex(lvl0, z => z == 0));

            return learn[index].GetEncounterMoves(level, lvl0, start);
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

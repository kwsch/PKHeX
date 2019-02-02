using System;
using System.Collections.Generic;

using static PKHeX.Core.Legal;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core
{
    internal static class MoveLevelUp
    {
        private static readonly LearnLookup
            LearnSM = new LearnLookup(PersonalTable.SM, LevelUpSM, SM),
            LearnUSUM = new LearnLookup(PersonalTable.USUM, LevelUpUSUM, USUM),
            LearnGG = new LearnLookup(PersonalTable.GG, LevelUpGG, GG),
            LearnXY = new LearnLookup(PersonalTable.XY, LevelUpXY, XY),
            LearnAO = new LearnLookup(PersonalTable.AO, LevelUpAO, ORAS),
            LearnBW = new LearnLookup(PersonalTable.BW, LevelUpBW, BW),
            LearnB2W2 = new LearnLookup(PersonalTable.B2W2, LevelUpB2W2, B2W2),
            LearnDP = new LearnLookup(PersonalTable.DP, LevelUpDP, DP),
            LearnPt = new LearnLookup(PersonalTable.Pt, LevelUpPt, Pt),
            LearnHGSS = new LearnLookup(PersonalTable.HGSS, LevelUpHGSS, HGSS),
            LearnRSE = new LearnLookup(PersonalTable.RS, LevelUpRS, RSE),
            LearnFRLG = new LearnLookup(PersonalTable.LG, LevelUpLG, FRLG),
            LearnGS = new LearnLookup(PersonalTable.GS, LevelUpGS, GS),
            LearnC = new LearnLookup(PersonalTable.C, LevelUpC, C),
            LearnRB = new LearnLookup(PersonalTable.RB, LevelUpRB, RB),
            LearnY = new LearnLookup(PersonalTable.Y, LevelUpY, YW);

        public static LearnVersion GetIsLevelUpMove(PKM pkm, int species, int form, int lvl, int generation, int move, int minlvlG1, int minlvlG2, GameVersion version = Any)
        {
            if (pkm.IsMovesetRestricted())
                version = (GameVersion)pkm.Version;

            switch (generation)
            {
                case 1: return GetIsLevelUp1(species, move, lvl, form, minlvlG1, version);
                case 2: if (move > MaxMoveID_1 && pkm.LearnMovesNew2Disallowed()) return LearnNONE;
                        return GetIsLevelUp2(species, move, lvl, form, minlvlG2, pkm.Korean, version);
                case 3: return GetIsLevelUp3(species, move, lvl, form, version);
                case 4: return GetIsLevelUp4(species, move, lvl, form, version);
                case 5: return GetIsLevelUp5(species, move, lvl, form, version);
                case 6: return GetIsLevelUp6(species, move, lvl, form, version);
                case 7: return GetIsLevelUp7(species, move, form, version);
            }
            return LearnNONE;
        }

        public static LearnVersion GetIsLevelUp1(int species, int move, int max, int form, int min, GameVersion ver = Any)
        {
            if (move > MaxMoveID_1)
                return LearnNONE;

            switch (ver)
            {
                case Any: case RBY:
                    var first = LearnRB.GetIsLevelUpG1(species, form, move, max, min);
                    var second = LearnY.GetIsLevelUpG1(species, form, move, max, min);
                    if (!first.IsLevelUp)
                        return second;
                    if (!second.IsLevelUp)
                        return first;
                    return first.Level > second.Level ? second : first;

                case RD: case BU: case GN: case RB:
                    return LearnRB.GetIsLevelUpG1(species, form, move, max, min);
                case YW:
                    return LearnY.GetIsLevelUpG1(species, form, move, max, min);
            }

            return LearnNONE;
        }

        private static LearnVersion GetIsLevelUp2(int species, int move, int max, int form, int min, bool korean, GameVersion ver = Any)
        {
            // No Korean Crystal
            switch (ver)
            {
                case Any: case GSC:
                    var first = LearnGS.GetIsLevelUpMin(species, move, max, min, form);
                    if (first.IsLevelUp || korean)
                        return first;
                    return LearnC.GetIsLevelUpMin(species, move, max, min, form);

                case GD: case SV: case GS:
                    return LearnGS.GetIsLevelUpMin(species, move, max, min, form);
                case C when !korean:
                    return LearnC.GetIsLevelUpMin(species, move, max, min, form);
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
                case Any: case DPPt:
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

        private static LearnVersion GetIsLevelUp5(int species, int move, int lvl, int form, GameVersion ver = Any)
        {
            switch (ver)
            {
                case Any:
                    var first = LearnBW.GetIsLevelUp(species, form, move, lvl);
                    if (first.IsLevelUp && species != 646)  // Kyurem moves are same for both versions, but forme movepool not present.
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
                case GP: case GE: case GG: case GO:
                    return LearnGG.GetIsLevelUp(species, form, move);

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

            var gen = ver.GetGeneration();
            if (gen != 3)
                return GetDeoxysLearn3(form);
            return GameData.GetLearnsets(ver)[386];
        }

        public static IEnumerable<int> GetMovesLevelUp(PKM pkm, int species, int minlvlG1, int minlvlG2, int lvl, int form, GameVersion version, bool MoveReminder, int Generation)
        {
            if (pkm.IsMovesetRestricted())
                version = (GameVersion)pkm.Version;
            switch (Generation)
            {
                case 1: return GetMovesLevelUp1(species, form, lvl, minlvlG1, version);
                case 2: return GetMovesLevelUp2(species, form, lvl, minlvlG2, pkm.Korean, pkm.LearnMovesNew2Disallowed(), version);
                case 3: return GetMovesLevelUp3(species, form, lvl, version);
                case 4: return GetMovesLevelUp4(species, form, lvl, version);
                case 5: return GetMovesLevelUp5(species, form, lvl, version);
                case 6: return GetMovesLevelUp6(species, form, lvl, version);
                case 7: return GetMovesLevelUp7(species, form, lvl, MoveReminder, version);
            }
            return Array.Empty<int>();
        }

        private static bool LearnMovesNew2Disallowed(this PKM pkm) => pkm.Format == 1 || (pkm.Format >= 7 && pkm.VC);

        internal static List<int> GetMovesLevelUp1(int species, int form, int max, int min, GameVersion ver = Any)
        {
            return AddMovesLevelUp1(new List<int>(), ver, species, form, max, min);
        }

        private static List<int> GetMovesLevelUp2(int species, int form, int max, int min, bool korean, bool removeNewGSCMoves, GameVersion ver = Any)
        {
            var moves = AddMovesLevelUp2(new List<int>(), ver, species, form, max, min, korean);
            if (removeNewGSCMoves)
                moves.RemoveAll(m => m > MaxMoveID_1);
            return moves;
        }

        private static List<int> GetMovesLevelUp3(int species, int form, int max, GameVersion ver = Any)
        {
            return AddMovesLevelUp3(new List<int>(), ver, species, max, form);
        }

        private static List<int> GetMovesLevelUp4(int species, int form, int max, GameVersion ver = Any)
        {
            return AddMovesLevelUp4(new List<int>(), ver, species, max, form);
        }

        private static List<int> GetMovesLevelUp5(int species, int form, int max, GameVersion ver = Any)
        {
            return AddMovesLevelUp5(new List<int>(), ver, species, max, form);
        }

        private static List<int> GetMovesLevelUp6(int species, int form, int max, GameVersion ver = Any)
        {
            return AddMovesLevelUp6(new List<int>(), ver, species, max, form);
        }

        private static List<int> GetMovesLevelUp7(int species, int form, int max, bool MoveReminder, GameVersion ver = Any)
        {
            return AddMovesLevelUp7(new List<int>(), ver, species, max, form, MoveReminder);
        }

        private static List<int> AddMovesLevelUp1(List<int> moves, GameVersion ver, int species, int form, int max, int min)
        {
            switch (ver)
            {
                case Any: case RBY:
                    LearnRB.AddMoves1(moves, species, form, max, min);
                    return LearnY.AddMoves1(moves, species, form, max, min);

                case RD: case BU: case GN: case RB:
                    return LearnRB.AddMoves1(moves, species, form, max, min);
                case YW:
                    return LearnY.AddMoves1(moves, species, form, max, min);
            }
            return moves;
        }

        private static List<int> AddMovesLevelUp2(List<int> moves, GameVersion ver, int species, int form, int max, int min, bool korean)
        {
            switch (ver)
            {
                case Any: case GSC:
                    LearnGS.AddMoves(moves, species, form, max, min);
                    if (korean)
                        return moves;
                    return LearnC.AddMoves(moves, species, form, max, min);

                case GD: case SV: case GS:
                    return LearnGS.AddMoves(moves, species, form, max, min);
                case C when !korean:
                    return LearnC.AddMoves(moves, species, form, max, min);
            }
            return moves;
        }

        private static List<int> AddMovesLevelUp3(List<int> moves, GameVersion ver, int species, int max, int form)
        {
            if (species == 386)
            {
                var learn = GetDeoxysLearn3(form, ver);
                if (learn != null)
                    moves.AddRange(learn.GetMoves(max));
                return moves;
            }

            // Emerald level up tables are equal to R/S level up tables
            switch (ver)
            {
                case Any:
                    LearnRSE.AddMoves(moves, species, form, max);
                    return LearnFRLG.AddMoves(moves, species, form, max);

                case R: case S: case E: case RS: case RSE:
                    return LearnRSE.AddMoves(moves, species, form, max);
                case FR: case LG: case FRLG:
                    return LearnFRLG.AddMoves(moves, species, form, max);
            }
            return moves;
        }

        private static List<int> AddMovesLevelUp4(List<int> moves, GameVersion ver, int species, int max, int form)
        {
            switch (ver)
            {
                case Any: case DPPt:
                    LearnDP.AddMoves(moves, species, form, max);
                    LearnPt.AddMoves(moves, species, form, max);
                    if (ver == DPPt) // stop here
                        return moves;
                    return LearnHGSS.AddMoves(moves, species, form, max);

                case D: case P: case DP:
                    return LearnDP.AddMoves(moves, species, form, max);
                case Pt:
                    return LearnPt.AddMoves(moves, species, form, max);
                case HG: case SS: case HGSS:
                    return LearnHGSS.AddMoves(moves, species, form, max);
            }
            return moves;
        }

        private static List<int> AddMovesLevelUp5(List<int> moves, GameVersion ver, int species, int max, int form)
        {
            switch (ver)
            {
                case Any:
                    if (species != 646) // Kyurem moves are same for both versions, but forme movepool not present.
                        LearnBW.AddMoves(moves, species, form, max);
                    return LearnB2W2.AddMoves(moves, species, form, max);

                case B: case W: case BW:
                    return LearnBW.AddMoves(moves, species, form, max);
                case B2: case W2: case B2W2:
                    return LearnB2W2.AddMoves(moves, species, form, max);
            }
            return moves;
        }

        private static List<int> AddMovesLevelUp6(List<int> moves, GameVersion ver, int species, int max, int form)
        {
            switch (ver)
            {
                case Any:
                    LearnXY.AddMoves(moves, species, form, max);
                    return LearnAO.AddMoves(moves, species, form, max);

                case X: case Y: case XY:
                    return LearnXY.AddMoves(moves, species, form, max);
                case AS: case OR: case ORAS:
                    return LearnAO.AddMoves(moves, species, form, max);
            }
            return moves;
        }

        private static List<int> AddMovesLevelUp7(List<int> moves, GameVersion ver, int species, int max, int form, bool MoveReminder)
        {
            if (MoveReminder)
                max = 100; // Move reminder can teach any level in movepool now!
            switch (ver)
            {
                case GP: case GE: case GG: case GO:
                    return LearnGG.AddMoves(moves, species, form, max);

                case Any:
                    if (species > MaxSpeciesID_7_USUM)
                        return moves;
                    LearnUSUM.AddMoves(moves, species, form, max);
                    if (species > MaxSpeciesID_7)
                        return moves;
                    return LearnSM.AddMoves(moves, species, form, max);

                case SN: case MN: case SM:
                    if (species > MaxSpeciesID_7)
                        return moves;
                    return LearnSM.AddMoves(moves, species, form, max);

                case US: case UM: case USUM:
                    if (species > MaxSpeciesID_7_USUM)
                        return moves;
                    LearnUSUM.AddMoves(moves, species, form, max);
                    break;
            }
            return moves;
        }

        public static int[] GetEncounterMoves(PKM pk, int level, GameVersion version)
        {
            if (version <= 0)
                version = (GameVersion)pk.Version;
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

        private static int[] GetEncounterMoves2(int species, int level, GameVersion version)
        {
            var learn = GameData.GetLearnsets(version);
            var table = GameData.GetPersonal(version);
            var index = table.GetFormeIndex(species, 0);
            var lvl0 = learn[species].GetEncounterMoves(1);
            int start = Math.Max(0, Array.FindIndex(lvl0, z => z == 0));

            return learn[index].GetEncounterMoves(level, lvl0, start);
        }

        public static int[] GetEncounterMoves(int species, int form, int level, GameVersion version)
        {
            if (RBY.Contains(version))
                return GetEncounterMoves1(species, level, version);
            if (GSC.Contains(version))
                return GetEncounterMoves2(species, level, version);
            var learn = GameData.GetLearnsets(version);
            var table = GameData.GetPersonal(version);
            var index = table.GetFormeIndex(species, form);
            return learn[index].GetEncounterMoves(level);
        }
    }
}

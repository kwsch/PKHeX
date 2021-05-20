using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static PKHeX.Core.EggSource2;

namespace PKHeX.Core
{
    public static class MoveBreed2
    {
        private const int level = 5;

        public static EggSource2[] Validate(int species, GameVersion version, int[] moves, out bool valid)
        {
            var count = Array.IndexOf(moves, 0);
            if (count == 0)
            {
                valid = false; // empty moveset
                return Array.Empty<EggSource2>();
            }
            if (count == -1)
                count = moves.Length;

            var learn = GameData.GetLearnsets(version);
            var table = GameData.GetPersonal(version);
            var learnset = learn[species];
            var pi = table[species];
            var egg = (version == GameVersion.C ? Legal.EggMovesC : Legal.EggMovesGS)[species].Moves;

            var value = new BreedInfo<EggSource2>(count, learnset, moves, level);
            {
                bool inherit = Breeding.GetCanInheritMoves(species);
                MarkMovesForOrigin(value, egg, count, inherit, pi, version);
                valid = RecurseMovesForOrigin(value, count - 1);
            }

            if (!valid)
                CleanResult(value.Actual, value.Possible);
            return value.Actual;
        }

        private static void CleanResult(EggSource2[] valueActual, byte[] valuePossible)
        {
            for (int i = 0; i < valueActual.Length; i++)
            {
                if (valueActual[i] != 0)
                    continue;
                var poss = valuePossible[i];
                if (poss == 0)
                    continue;

                for (int j = 0; j < (int) Max; j++)
                {
                    if ((poss & (1 << j)) == 0)
                        continue;
                    valueActual[i] = (EggSource2)j;
                    break;
                }
            }
        }

        private static bool RecurseMovesForOrigin(in BreedInfo<EggSource2> info, int start, EggSource2 type = Max)
        {
            int i = start;
            do
            {
                if (type != Base)
                {
                    if (RecurseMovesForOrigin(info, i, Base))
                        return true;
                }

                var flag = 1 << (int)Base;
                if (type != Base)
                    flag = ~flag;

                var permit = info.Possible[i];
                if ((permit & flag) == 0)
                    return false;

                info.Actual[i] = type == Base ? Base : GetFirstType(permit);
            } while (--i >= 0);

            return VerifyBaseMoves(info);
        }

        private static EggSource2 GetFirstType(byte permit)
        {
            for (var type = FatherEgg; type < Max; type++)
            {
                if ((permit & (1 << (int)type)) != 0)
                    return type;
            }
            throw new ArgumentOutOfRangeException(nameof(permit), permit, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool VerifyBaseMoves(in BreedInfo<EggSource2> info)
        {
            var count = 0;
            foreach (var x in info.Actual)
            {
                if (x == Base)
                    count++;
                else
                    break;
            }

            var moves = info.Moves;
            if (count == -1)
                return moves[^1] != 0;

            var baseMoves = info.Learnset.GetBaseEggMoves(info.Level);
            if (baseMoves.Length < count)
                return false;
            if (moves[^1] == 0 && count != baseMoves.Length)
                return false;

            for (int i = count - 1, b = baseMoves.Length - 1; i >= 0; i--, b--)
            {
                var move = moves[i];
                var expect = baseMoves[b];
                if (expect != move)
                    return false;
            }

            // A low-index base egg move may be nudged out, but can only reappear if sufficient non-base moves are before it.
            if (baseMoves.Length == count)
                return true;

            for (int i = count; i < info.Actual.Length; i++)
            {
                var isBase = (info.Possible[i] & (1 << (int)Base)) != 0;
                if (!isBase)
                    continue;

                var baseIndex = baseMoves.IndexOf(moves[i]);
                var min = moves.Length - baseMoves.Length + baseIndex;
                if (i <= min + count)
                    return false;
            }

            return true;
        }

        private static void MarkMovesForOrigin(in BreedInfo<EggSource2> value, ICollection<int> eggMoves, int count, bool inheritLevelUp, PersonalInfo info, GameVersion version)
        {
            var possible = value.Possible;
            var learn = value.Learnset;
            var baseEgg = value.Learnset.GetBaseEggMoves(value.Level);
            var tm = info.TMHM;

            var moves = value.Moves;
            for (int i = 0; i < count; i++)
            {
                var move = moves[i];

                if (baseEgg.IndexOf(move) != -1)
                    possible[i] |= 1 << (int)Base;

                if (inheritLevelUp && learn.GetLevelLearnMove(move) != -1)
                    possible[i] |= 1 << (int)ParentLevelUp;

                if (eggMoves.Contains(move))
                    possible[i] |= 1 << (int)FatherEgg;

                var tmIndex = Array.IndexOf(Legal.TMHM_GSC, move, 0, 50);
                if (tmIndex != -1 && tm[tmIndex])
                    possible[i] |= 1 << (int)FatherTM;

                if (version is GameVersion.C)
                {
                    var tutorIndex = Array.IndexOf(Legal.Tutors_GSC, move);
                    if (tutorIndex != -1 && tm[57 + tutorIndex])
                        possible[i] |= 1 << (int)Tutor;
                }
            }
        }
    }
}

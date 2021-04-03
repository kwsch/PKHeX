using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PKHeX.Core
{
    public static class MoveBreed
    {
        #region Gen2-5
        public static bool Process25(int generation, int species, int form, GameVersion version, int[] moves, int level)
        {
            var count = Array.IndexOf(moves, 0);
            if (count == 0)
                return false;
            if (count == -1)
                count = moves.Length;

            var learn = GameData.GetLearnsets(version);
            var table = GameData.GetPersonal(version);
            var index = table.GetFormIndex(species, form);
            var learnset = learn[index];
            var pi = table[index];
            var egg = MoveEgg.GetEggMoves(generation, species, form, version);

            var value = new ValueStorage<EggSource25>(count, learnset, moves, level);
            if (moves[count - 1] is (int)Move.VoltTackle && (generation > 3 || version == GameVersion.E))
            {
                if (--count == 0)
                    return false;
                value.Actual[count] = EggSource25.Special;
            }

            bool inherit = Breeding.GetCanInheritMoves(species);
            MarkMovesForOrigin(value, egg, count, inherit, pi, generation, version);

            if (generation == 2)
                return RecurseMovesForOriginG2(value, count - 1);

            return RecurseMovesForOrigin(value, count - 1);
        }

        private static bool RecurseMovesForOriginG2(ValueStorage<EggSource25> info, int start, EggSource25 type = EggSource25.Max)
        {
            int i = start;
            do
            {
                if (type != EggSource25.Base)
                {
                    if (RecurseMovesForOriginG2(info, i, EggSource25.Base))
                        return true;
                }

                var flag = 1 << (int) EggSource25.Base;
                if (type != EggSource25.Base)
                    flag = ~flag;

                var permit = info.Origins[i];
                if ((permit & flag) == 0)
                    return false;

                info.Actual[i] = type;
            } while (--i >= 0);

            return VerifyBaseMoves(info);
        }

        private static bool RecurseMovesForOrigin(ValueStorage<EggSource25> info, int start, EggSource25 type = EggSource25.Max - 1)
        {
            int i = start;
            do
            {
                var unpeel = type - 1;
                if (unpeel != 0 && RecurseMovesForOrigin(info, i, unpeel))
                    return true;

                var permit = info.Origins[i];
                if ((permit & (1 << (int)type)) == 0)
                    return false;

                info.Actual[i] = type;
            } while (--i >= 0);

            return VerifyBaseMoves(info);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool VerifyBaseMoves(ValueStorage<EggSource25> info)
        {
            var count = 0;
            foreach (var x in info.Actual)
            {
                if (x == EggSource25.Base)
                    count++;
                else
                    break;
            }
            if (count == -1)
                return info.Moves[info.Moves.Length - 1] != 0;

            var baseMoves = info.Learnset.GetBaseEggMoves(info.Level);
            if (baseMoves.Length < count)
                return false;
            if (info.Moves[info.Moves.Length - 1] == 0 && count != baseMoves.Length)
                return false;

            for (int i = count - 1, b = baseMoves.Length - 1; i >= 0; i--, b--)
            {
                var move = info.Moves[i];
                var expect = baseMoves[b];
                if (expect != move)
                    return false;
            }

            // A low-index base egg move may be nudged out, but can only reappear if sufficient non-base moves are before it.
            if (baseMoves.Length == count)
                return true;

            for (int i = count; i < info.Actual.Length; i++)
            {
                var isBase = (info.Origins[i] & (1 << (int)EggSource25.Base)) != 0;
                if (!isBase)
                    continue;

                var baseIndex = baseMoves.IndexOf(info.Moves[i]);
                var min = info.Moves.Length - baseMoves.Length + baseIndex;
                if (i <= min + count)
                    return false;
            }

            return true;
        }

        private static void MarkMovesForOrigin(ValueStorage<EggSource25> value, ICollection<int> eggMoves, int count, bool inheritLevelUp, PersonalInfo info, int generation, GameVersion version)
        {
            var possible = value.Origins;
            var learn = value.Learnset;
            var baseEgg = value.Learnset.GetBaseEggMoves(value.Level);
            var tm = info.TMHM;
            var tmlist = GetHeritableTMList(generation);

            var moves = value.Moves;
            for (int i = 0; i < count; i++)
            {
                var move = moves[i];

                if (baseEgg.Contains(move))
                    possible[i] |= 1 << (int) EggSource25.Base;

                if (inheritLevelUp && learn.GetLevelLearnMove(move) != -1)
                    possible[i] |= 1 << (int)EggSource25.ParentLevelUp;

                if (eggMoves.Contains(move))
                    possible[i] |= 1 << (int)EggSource25.FatherEgg;

                var tmIndex = tmlist.IndexOf(move);
                if (tmIndex != -1 && tm[tmIndex])
                    possible[i] |= 1 << (int)EggSource25.FatherTM;

                if (version is GameVersion.C)
                {
                    var tutorIndex = Array.IndexOf(Legal.Tutors_GSC, move);
                    if (tutorIndex != -1 && tm[57 + tutorIndex])
                        possible[i] |= 1 << (int)EggSource25.FatherTM; // don't bother differentiating Crystal tutor from TMs
                }
            }
        }

        #endregion

        #region Gen6-8+
        public static bool Process6(int generation, int species, int form, GameVersion version, int[] moves, int level)
        {
            var count = Array.IndexOf(moves, 0);
            if (count == 0)
                return false;
            if (count == -1)
                count = moves.Length;

            var learn = GameData.GetLearnsets(version);
            var table = GameData.GetPersonal(version);
            var index = table.GetFormIndex(species, form);
            var learnset = learn[index];
            var egg = MoveEgg.GetEggMoves(generation, species, form, version);

            var value = new ValueStorage<EggSource6>(count, learnset, moves, level);
            if (moves[count - 1] is (int)Move.VoltTackle)
            {
                if (--count == 0)
                    return false;
                value.Actual[count] = EggSource6.Special;
            }

            bool inherit = Breeding.GetCanInheritMoves(species);
            MarkMovesForOrigin(value, egg, count, inherit);

            return RecurseMovesForOrigin(value, count - 1);
        }

        private static bool RecurseMovesForOrigin(ValueStorage<EggSource6> info, int start, EggSource6 type = EggSource6.Max - 1)
        {
            int i = start;
            do
            {
                var unpeel = type - 1;
                if (unpeel != 0 && RecurseMovesForOrigin(info, i, unpeel))
                    return true;

                var permit = info.Origins[i];
                if ((permit & (1 << (int)type)) == 0)
                    return false;

                info.Actual[i] = type;
            } while (--i >= 0);

            return VerifyBaseMoves(info);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool VerifyBaseMoves(ValueStorage<EggSource6> info)
        {
            var count = 0;
            foreach (var x in info.Actual)
            {
                if (x == EggSource6.Base)
                    count++;
                else
                    break;
            }
            if (count == -1)
                return info.Moves[info.Moves.Length - 1] != 0;

            var baseMoves = info.Learnset.GetBaseEggMoves(info.Level);
            if (baseMoves.Length < count)
                return false;
            if (info.Moves[info.Moves.Length - 1] == 0 && count != baseMoves.Length)
                return false;

            for (int i = count - 1, b = baseMoves.Length - 1; i >= 0; i--, b--)
            {
                var move = info.Moves[i];
                var expect = baseMoves[b];
                if (expect != move)
                    return false;
            }

            // A low-index base egg move may be nudged out, but can only reappear if sufficient non-base moves are before it.
            if (baseMoves.Length == count)
                return true;

            for (int i = count; i < info.Actual.Length; i++)
            {
                var isBase = (info.Origins[i] & (1 << (int)EggSource6.Base)) != 0;
                if (!isBase)
                    continue;

                var baseIndex = baseMoves.IndexOf(info.Moves[i]);
                var min = info.Moves.Length - baseMoves.Length + baseIndex;
                if (i <= min + count)
                    return false;
            }

            return true;
        }

        private static void MarkMovesForOrigin(ValueStorage<EggSource6> value, ICollection<int> eggMoves, int count, bool inheritLevelUp)
        {
            var possible = value.Origins;
            var learn = value.Learnset;
            var baseEgg = value.Learnset.GetBaseEggMoves(value.Level);

            var moves = value.Moves;
            for (int i = 0; i < count; i++)
            {
                var move = moves[i];

                if (baseEgg.Contains(move))
                    possible[i] |= 1 << (int)EggSource6.Base;

                if (inheritLevelUp && learn.GetLevelLearnMove(move) != -1)
                    possible[i] |= 1 << (int)EggSource6.ParentLevelUp;

                if (eggMoves.Contains(move))
                    possible[i] |= 1 << (int)EggSource6.ParentEgg;
            }
        }

        #endregion

        private readonly ref struct ValueStorage<T> where T : Enum
        {
            public readonly T[] Actual;
            public readonly byte[] Origins;
            public readonly Learnset Learnset;
            public readonly int[] Moves;
            public readonly int Level;

            public ValueStorage(int count, Learnset learnset, int[] moves, int level)
            {
                Origins = new byte[count];
                Actual = new T[count];
                Learnset = learnset;
                Moves = moves;
                Level = level;
            }
        }

        private static bool Contains(this ReadOnlySpan<int> moves, int move)
        {
            foreach (var m in moves)
            {
                if (m == move)
                    return true;
            }
            return false;
        }

        private static ReadOnlySpan<int> GetHeritableTMList(int generation) => generation switch
        {
            2 => Legal.TMHM_GSC.AsSpan(0, 50),
            3 => Legal.TM_3.AsSpan(0, 50),
            4 => Legal.TM_4.AsSpan(0, 92),
            5 => Legal.TMHM_BW.AsSpan(0, 95), // actually 96, but TM96 is unavailable (Snarl - Lock Capsule)
            _ => throw new Exception(),
        };
    }

    public enum EggSource25 : byte
    {
        None,
        Base,
        FatherEgg,
        FatherTM,
        ParentLevelUp,

        Max,

        Special,
    }

    public enum EggSource6 : byte
    {
        None,
        Base,
        ParentLevelUp,
        ParentEgg,

        Max,

        Special,
    }
}

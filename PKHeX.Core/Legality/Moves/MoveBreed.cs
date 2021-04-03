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
            if (moves[count - 1] is (int)Move.VoltTackle)
            {
                if (--count == 0)
                    return false;
                value.Actual[count] = EggSource25.Special;
            }
            MarkMovesForOrigin(value, moves, egg, pi, generation, count);

            return RecurseMovesForOrigin(value, count, EggSource25.Max);
        }

        private static bool RecurseMovesForOrigin(ValueStorage<EggSource25> info, int start, EggSource25 type)
        {
            int i = start;
            do
            {
                if (type != 0 && RecurseMovesForOrigin(info, start, type - 1))
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
            var count = Array.LastIndexOf(info.Actual, EggSource25.Base);
            if (count == -1)
                return true;

            var baseMoves = info.Learnset.GetBaseEggMoves(info.Level);
            if (baseMoves.Length < count)
                return false;

            for (int i = count - 1, b = 0; i >= 0; i--, b++)
            {
                var move = info.Moves[i];
                var expect = baseMoves[baseMoves.Length - 1 - b];
                if (expect != move)
                    return false;
            }
            return true;
        }

        private static void MarkMovesForOrigin(ValueStorage<EggSource25> value, IReadOnlyList<int> moves, ICollection<int> eggMoves, PersonalInfo info, int generation, int count)
        {
            var possible = value.Origins;
            var learn = value.Learnset;
            var baseEgg = value.Learnset.GetBaseEggMoves(value.Level);
            var tm = info.TMHM;

            var tmlist = GetHeritableTMList(generation);
            for (int i = 0; i < count; i++)
            {
                var move = moves[i];

                if (baseEgg.Contains(move))
                    possible[i] |= 1 << (int) EggSource25.Base;

                if (learn.GetLevelLearnMove(move) != -1)
                    possible[i] |= 1 << (int)EggSource25.ParentLevelUp;

                if (eggMoves.Contains(move))
                    possible[i] |= 1 << (int)EggSource25.FatherEgg;

                var tmIndex = tmlist.IndexOf(move);
                if (tmIndex != -1 && tm[i])
                    possible[i] |= 1 << (int)EggSource25.FatherTM;
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
            MarkMovesForOrigin(value, moves, egg, count);

            return RecurseMovesForOrigin(value, count, EggSource6.Max);
        }

        private static bool RecurseMovesForOrigin(ValueStorage<EggSource6> info, int start, EggSource6 type)
        {
            int i = start;
            do
            {
                if (type != 0 && RecurseMovesForOrigin(info, start, type - 1))
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
            var count = Array.LastIndexOf(info.Actual, EggSource6.Base);
            if (count == -1)
                return true;

            var baseMoves = info.Learnset.GetBaseEggMoves(info.Level);
            if (baseMoves.Length < count)
                return false;

            for (int i = count - 1, b = 0; i >= 0; i--, b++)
            {
                var move = info.Moves[i];
                var expect = baseMoves[baseMoves.Length - 1 - b];
                if (expect != move)
                    return false;
            }
            return true;
        }

        private static void MarkMovesForOrigin(ValueStorage<EggSource6> value, IReadOnlyList<int> moves, ICollection<int> eggMoves, int count)
        {
            var possible = value.Origins;
            var learn = value.Learnset;
            var baseEgg = value.Learnset.GetBaseEggMoves(value.Level);

            for (int i = 0; i < count; i++)
            {
                var move = moves[i];

                if (baseEgg.Contains(move))
                    possible[i] |= 1 << (int)EggSource6.Base;

                if (learn.GetLevelLearnMove(move) != -1)
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

        Max = ParentLevelUp,

        Special,
    }

    public enum EggSource6 : byte
    {
        None,
        Base,
        ParentLevelUp,
        ParentEgg,

        Max = ParentEgg,

        Special,
    }
}

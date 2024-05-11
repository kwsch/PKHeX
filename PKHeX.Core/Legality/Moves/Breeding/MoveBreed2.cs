using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static PKHeX.Core.EggSource2;
using static PKHeX.Core.LearnSource2;

namespace PKHeX.Core;

/// <summary>
/// Inheritance logic for Generation 2.
/// </summary>
/// <remarks>Refer to <see cref="EggSource2"/> for inheritance ordering.</remarks>
public static class MoveBreed2
{
    private const int level = 5;

    /// <inheritdoc cref="MoveBreed.Validate"/>
    public static bool Validate(ushort species, GameVersion version, ReadOnlySpan<ushort> moves, Span<byte> origins)
    {
        var count = moves.IndexOf((ushort)0);
        if (count == 0)
            return false;
        if (count == -1)
            count = moves.Length;

        ILearnSource<PersonalInfo2> ls = version == GameVersion.C ? LearnSource2C.Instance : LearnSource2GS.Instance;
        if (!ls.TryGetPersonal(species, 0, out var pi))
            return false;

        var learnset = ls.GetLearnset(species, 0);
        var eggMoves = version == GameVersion.C
            ? LearnSource2C.Instance.GetEggMoves(species, 0)
            : LearnSource2GS.Instance.GetEggMoves(species, 0);

        var actual = MemoryMarshal.Cast<byte, EggSource2>(origins);
        Span<byte> possible = stackalloc byte[count];
        var value = new BreedInfo<EggSource2>(actual, possible, learnset, moves, level);

        bool inherit = Breeding.GetCanInheritMoves(species);
        MarkMovesForOrigin(value, eggMoves, count, inherit, pi, version);
        var valid = RecurseMovesForOrigin(value, count - 1);
        if (!valid)
            CleanResult(actual, possible);
        return valid;
    }

    private static void CleanResult(Span<EggSource2> valueActual, Span<byte> valuePossible)
    {
        for (int i = 0; i < valuePossible.Length; i++)
        {
            var poss = valuePossible[i];
            if (poss == 0)
                continue;
            if (valueActual[i] != 0)
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
            if (i < min + count)
                return false;
        }

        return true;
    }

    private static void MarkMovesForOrigin(in BreedInfo<EggSource2> value, ReadOnlySpan<ushort> eggMoves, int count, bool inheritLevelUp, PersonalInfo2 info, GameVersion version)
    {
        var possible = value.Possible;
        var learn = value.Learnset;
        var baseEgg = value.Learnset.GetBaseEggMoves(value.Level);

        var moves = value.Moves;
        for (int i = 0; i < count; i++)
        {
            var move = moves[i];
            if (move > Legal.MaxMoveID_2) // byte
                continue;

            if (baseEgg.Contains(move))
                possible[i] |= 1 << (int)Base;

            if (inheritLevelUp && learn.GetIsLearn(move))
                possible[i] |= 1 << (int)ParentLevelUp;

            if (eggMoves.Contains(move))
                possible[i] |= 1 << (int)FatherEgg;

            if (info.GetIsLearnTM(TMHM_GSC.IndexOf((byte)move)))
                possible[i] |= 1 << (int)FatherTM;

            if (version is GameVersion.C)
            {
                if (info.GetIsLearnTutorType(Tutors_GSC.IndexOf((byte)move)))
                    possible[i] |= 1 << (int)Tutor;
            }
        }
    }
}

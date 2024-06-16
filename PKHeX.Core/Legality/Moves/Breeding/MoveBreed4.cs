using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static PKHeX.Core.EggSource34;
using static PKHeX.Core.GameVersion;
using static PKHeX.Core.LearnSource4;

namespace PKHeX.Core;

/// <summary>
/// Inheritance logic for Generation 4.
/// </summary>
/// <remarks>Refer to <see cref="EggSource34"/> for inheritance ordering.</remarks>
public static class MoveBreed4
{
    private const int level = 1;

    /// <inheritdoc cref="MoveBreed.Validate"/>
    public static bool Validate(ushort species, GameVersion version, ReadOnlySpan<ushort> moves, Span<byte> origins)
    {
        var count = moves.IndexOf((ushort)0);
        if (count == 0)
            return false;
        if (count == -1)
            count = moves.Length;

        var learnset = version switch
        {
            HG or SS => LearnSource4HGSS.Instance.GetLearnset(species, 0),
            D or P => LearnSource4DP.Instance.GetLearnset(species, 0),
            _ => LearnSource4Pt.Instance.GetLearnset(species, 0),
        };
        var table = version switch
        {
            HG or SS => PersonalTable.HGSS,
            D or P => PersonalTable.DP,
            _ => PersonalTable.Pt,
        };
        var pi = table[species];

        var actual = MemoryMarshal.Cast<byte, EggSource34>(origins);
        Span<byte> possible = stackalloc byte[count];
        var value = new BreedInfo<EggSource34>(actual, possible, learnset, moves, level);
        if (species is (int)Species.Pichu && moves[count - 1] is (int)Move.VoltTackle)
            actual[--count] = VoltTackle;

        bool valid;
        if (count == 0)
        {
            valid = VerifyBaseMoves(value);
        }
        else
        {
            bool inherit = Breeding.GetCanInheritMoves(species);
            var eggMoves = version is HG or SS
                ? LearnSource4HGSS.Instance.GetEggMoves(species, 0)
                : LearnSource4DP.Instance.GetEggMoves(species, 0);
            MarkMovesForOrigin(value, eggMoves, count, inherit, pi, version);
            valid = RecurseMovesForOrigin(value, count - 1);
        }

        if (!valid)
            CleanResult(actual, possible);
        return valid;
    }

    private static void CleanResult(Span<EggSource34> valueActual, Span<byte> valuePossible)
    {
        for (int i = 0; i < valuePossible.Length; i++)
        {
            var poss = valuePossible[i];
            if (poss == 0)
                continue;
            if (valueActual[i] != 0)
                continue;

            for (int j = 0; j < (int)Max; j++)
            {
                if ((poss & (1 << j)) == 0)
                    continue;
                valueActual[i] = (EggSource34)j;
                break;
            }
        }
    }

    private static bool RecurseMovesForOrigin(in BreedInfo<EggSource34> info, int start, EggSource34 type = Max - 1)
    {
        int i = start;
        do
        {
            var unpeel = type - 1;
            if (unpeel != 0 && RecurseMovesForOrigin(info, i, unpeel))
                return true;

            var permit = info.Possible[i];
            if ((permit & (1 << (int)type)) == 0)
                return false;

            info.Actual[i] = type;
        } while (--i >= 0);

        return VerifyBaseMoves(info);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool VerifyBaseMoves(in BreedInfo<EggSource34> info)
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

    private static void MarkMovesForOrigin(in BreedInfo<EggSource34> value, ReadOnlySpan<ushort> eggMoves, int count, bool inheritLevelUp, PersonalInfo4 info, GameVersion gameVersion)
    {
        var possible = value.Possible;
        var learn = value.Learnset;
        var baseEgg = value.Learnset.GetBaseEggMoves(value.Level);
        var tmlist = TM_4;
        var hmlist = gameVersion is HG or SS ? HM_HGSS : HM_DPPt;

        var moves = value.Moves;
        for (int i = 0; i < count; i++)
        {
            var move = moves[i];

            if (baseEgg.Contains(move))
                possible[i] |= 1 << (int)Base;

            if (inheritLevelUp && learn.GetIsLearn(move))
                possible[i] |= 1 << (int)ParentLevelUp;

            if (eggMoves.Contains(move))
                possible[i] |= 1 << (int)FatherEgg;

            var tmIndex = tmlist.IndexOf(move);
            if (tmIndex != -1 && info.GetIsLearnTM(tmIndex))
                possible[i] |= 1 << (int)FatherTM;

            var hmIndex = hmlist.IndexOf(move);
            if (hmIndex != -1 && info.GetIsLearnHM(hmIndex))
                possible[i] |= 1 << (int)FatherTM;
        }
    }
}

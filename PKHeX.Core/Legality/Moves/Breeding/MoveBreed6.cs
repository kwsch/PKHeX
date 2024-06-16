using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static PKHeX.Core.EggSource6;

namespace PKHeX.Core;

/// <summary>
/// Inheritance logic for Generations 6+.
/// </summary>
/// <remarks>Refer to <see cref="EggSource6"/> for inheritance ordering.</remarks>
public static class MoveBreed6
{
    private const int level = 1;

    /// <inheritdoc cref="MoveBreed.Validate"/>
    public static bool Validate(byte generation, ushort species, byte form, GameVersion version, ReadOnlySpan<ushort> moves, Span<byte> origins)
    {
        var count = moves.IndexOf((ushort)0);
        if (count == 0)
            return false;
        if (count == -1)
            count = moves.Length;

        var learn = GameData.GetLearnSource(version);
        var learnset = learn.GetLearnset(species, form);

        var actual = MemoryMarshal.Cast<byte, EggSource6>(origins);
        Span<byte> possible = stackalloc byte[count];
        var value = new BreedInfo<EggSource6>(actual, possible, learnset, moves, level);
        if (species is (int)Species.Pichu && moves[count - 1] is (int)Move.VoltTackle)
            actual[--count] = VoltTackle;

        bool valid;
        if (count == 0)
        {
            valid = VerifyBaseMoves(value);
        }
        else
        {
            var egg = learn.GetEggMoves(species, form);
            bool inherit = Breeding.GetCanInheritMoves(species);
            MarkMovesForOrigin(value, egg, count, inherit);
            valid = RecurseMovesForOrigin(value, count - 1);
        }

        if (!valid)
            CleanResult(actual, possible);
        return valid;
    }

    private static void CleanResult(Span<EggSource6> valueActual, Span<byte> valuePossible)
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
                valueActual[i] = (EggSource6)j;
                break;
            }
        }
    }

    private static bool RecurseMovesForOrigin(in BreedInfo<EggSource6> info, int start, EggSource6 type = Max - 1)
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
    private static bool VerifyBaseMoves(in BreedInfo<EggSource6> info)
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

            var move = info.Moves[i];
            var baseIndex = baseMoves.IndexOf(move);
            var min = info.Moves.Length - baseMoves.Length + baseIndex;
            if (i < min + count)
                return false;
        }

        return true;
    }

    private static void MarkMovesForOrigin(in BreedInfo<EggSource6> value, ReadOnlySpan<ushort> eggMoves, int count, bool inheritLevelUp)
    {
        var possible = value.Possible;
        var learn = value.Learnset;
        var baseEgg = value.Learnset.GetBaseEggMoves(value.Level);

        var moves = value.Moves;
        for (int i = 0; i < count; i++)
        {
            var move = moves[i];

            if (baseEgg.Contains(move))
                possible[i] |= 1 << (int)Base;

            if (inheritLevelUp && learn.GetIsLearn(move))
                possible[i] |= 1 << (int)ParentLevelUp;

            if (eggMoves.Contains(move))
                possible[i] |= 1 << (int)ParentEgg;
        }
    }
}

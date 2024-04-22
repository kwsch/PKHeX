using System;
using System.Runtime.CompilerServices;

namespace PKHeX.Core;

/// <summary>
/// Logic to verify the current <see cref="PKM.RelearnMoves"/>.
/// </summary>
public static class LearnVerifierRelearn
{
    public static void Verify(Span<MoveResult> result, IEncounterTemplate enc, PKM pk)
    {
        if (ShouldNotHaveRelearnMoves(enc, pk))
            VerifyRelearnNone(pk, result);
        else if (enc is IRelearn {Relearn: {HasMoves: true} x})
            VerifyRelearnSpecifiedMoveset(pk, x, result);
        else if (enc is EncounterEgg e)
            VerifyEggMoveset(e, result, pk);
        else if (enc is EncounterSlot6AO { CanDexNav: true } z && pk.RelearnMove1 != 0)
            VerifyRelearnDexNav(pk, result, z);
        else if (enc is EncounterSlot8b { IsUnderground: true } u)
            VerifyRelearnUnderground(pk, result, u);
        else
            VerifyRelearnNone(pk, result);
    }

    public static bool ShouldNotHaveRelearnMoves(IGeneration enc, PKM pk) => enc.Generation < 6 || pk.IsOriginalMovesetDeleted();

    private static void VerifyRelearnSpecifiedMoveset(PKM pk, Moveset required, Span<MoveResult> result)
    {
        result[3] = ParseExpect(pk.RelearnMove4, required.Move4);
        result[2] = ParseExpect(pk.RelearnMove3, required.Move3);
        result[1] = ParseExpect(pk.RelearnMove2, required.Move2);
        result[0] = ParseExpect(pk.RelearnMove1, required.Move1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static MoveResult ParseExpect(ushort move, ushort expect = 0)
    {
        if (move != expect)
            return MoveResult.Unobtainable(expect);
        if (move == 0)
            return MoveResult.Empty;
        return MoveResult.Relearn;
    }

    private static void VerifyRelearnDexNav(PKM pk, Span<MoveResult> result, EncounterSlot6AO slot)
    {
        // Only has one relearn move from the encounter. Every other relearn move must be empty.
        result[3] = ParseExpect(pk.RelearnMove4);
        result[2] = ParseExpect(pk.RelearnMove3);
        result[1] = ParseExpect(pk.RelearnMove2);

        // DexNav Pokémon can have 1 random egg move as a relearn move.
        result[0] = slot.CanBeDexNavMove(pk.RelearnMove1) ? MoveResult.Relearn : MoveResult.Unobtainable(); // DexNav
    }

    private static void VerifyRelearnUnderground(PKM pk, Span<MoveResult> result, EncounterSlot8b slot)
    {
        // Only has one relearn move from the encounter. Every other relearn move must be empty.
        result[3] = ParseExpect(pk.RelearnMove4);
        result[2] = ParseExpect(pk.RelearnMove3);
        result[1] = ParseExpect(pk.RelearnMove2);

        // Underground Pokémon can have 1 random egg move as a relearn move.
        result[0] = slot.CanBeUndergroundMove(pk.RelearnMove1) ? MoveResult.Relearn : MoveResult.Unobtainable(); // Underground
    }

    private static void VerifyRelearnNone(PKM pk, Span<MoveResult> result)
    {
        // No relearn moves should be present.
        result[3] = ParseExpect(pk.RelearnMove4);
        result[2] = ParseExpect(pk.RelearnMove3);
        result[1] = ParseExpect(pk.RelearnMove2);
        result[0] = ParseExpect(pk.RelearnMove1);
    }

    private static void VerifyEggMoveset(EncounterEgg e, Span<MoveResult> result, PKM pk)
    {
        Span<ushort> moves = stackalloc ushort[4];
        pk.GetRelearnMoves(moves);
        VerifyEggMoveset(e, result, moves);
    }

    internal static void VerifyEggMoveset(EncounterEgg e, Span<MoveResult> result, ReadOnlySpan<ushort> moves)
    {
        var gen = e.Generation;
        Span<byte> origins = stackalloc byte[moves.Length];
        var valid = MoveBreed.Validate(gen, e.Species, e.Form, e.Version, moves, origins);
        if (valid)
        {
            for (int i = result.Length - 1; i >= 0; i--)
            {
                if (moves[i] == 0)
                    result[i] = MoveResult.Empty;
                else
                    result[i] = new(EggSourceUtil.GetSource(origins[i], gen), GameData.GetLearnSource(e.Version).Environment);
            }
        }
        else
        {
            Span<ushort> expected = stackalloc ushort[moves.Length];
            _ = MoveBreed.GetExpectedMoves(moves, e, expected);
            _ = MoveBreed.Validate(gen, e.Species, e.Form, e.Version, expected, origins);
            for (int i = moves.Length - 1; i >= 0; i--)
            {
                var current = moves[i];
                var expect = expected[i];
                if (current != expect)
                    result[i] = MoveResult.Unobtainable(expect);
                else if (current == 0)
                    result[i] = MoveResult.Empty;
                else
                    result[i] = new(EggSourceUtil.GetSource(origins[i], gen), GameData.GetLearnSource(e.Version).Environment);
            }
        }

        var dupe = IsAnyMoveDuplicate(moves);
        if (dupe != NO_DUPE)
            result[dupe] = MoveResult.Duplicate;
    }

    private const int NO_DUPE = -1;

    private static int IsAnyMoveDuplicate(ReadOnlySpan<ushort> move)
    {
        int m1 = move[0];
        int m2 = move[1];

        if (m1 != 0 && m1 == m2)
            return 1;
        int m3 = move[2];
        if (m3 != 0 && (m1 == m3 || m2 == m3))
            return 2;
        int m4 = move[3];
        if (m4 != 0 && (m1 == m4 || m2 == m4 || m3 == m4))
            return 3;
        return NO_DUPE;
    }
}

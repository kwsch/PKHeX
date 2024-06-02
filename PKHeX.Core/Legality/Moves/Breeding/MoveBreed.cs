using System;

namespace PKHeX.Core;

/// <summary>
/// Inheritance logic for bred eggs.
/// </summary>
/// <remarks>Refer to the associated Egg Source enums used by the associated child classes for inheritance ordering.</remarks>
public static class MoveBreed
{
    /// <summary>
    /// Verifies the input <see cref="moves"/> using the breeding rules of the associated <see cref="generation"/> and <see cref="version"/>.
    /// </summary>
    /// <param name="generation">Generation rule-set the egg was created in</param>
    /// <param name="species">Entity species in the egg</param>
    /// <param name="form">Entity form in the egg</param>
    /// <param name="version">Version the egg was created in</param>
    /// <param name="moves">Moves the egg supposedly originated with</param>
    /// <param name="origins">Output buffer indicating the origin of each index within <see cref="moves"/></param>
    /// <returns>True if the moves are ordered correctly, without missing moves.</returns>
    public static bool Validate(byte generation, ushort species, byte form, GameVersion version, ReadOnlySpan<ushort> moves, Span<byte> origins) => generation switch
    {
        2 => MoveBreed2.Validate(species, version, moves, origins),
        3 => MoveBreed3.Validate(species, version, moves, origins),
        4 => MoveBreed4.Validate(species, version, moves, origins),
        5 => MoveBreed5.Validate(species, version, moves, origins),
        _ => MoveBreed6.Validate(generation, species, form, version, moves, origins),
    };

    /// <summary>
    /// Gets the expected moves the egg should come with, using an input of requested <see cref="moves"/> that are requested to be in the output.
    /// </summary>
    /// <param name="moves">Moves requested to be in the expected moves result</param>
    /// <param name="enc">Encounter detail interface wrapper; should always be <see cref="EncounterEgg"/>.</param>
    /// <param name="result">Result moves that are valid</param>
    /// <remarks>Validates the requested moves first prior to trying a more expensive computation.</remarks>
    /// <returns>True if the <see cref="result"/> is valid using the input <see cref="moves"/>. If not valid, the <see cref="result"/> will be base egg moves, probably valid.</returns>
    public static bool GetExpectedMoves(ReadOnlySpan<ushort> moves, IEncounterTemplate enc, Span<ushort> result)
    {
        Span<byte> origins = stackalloc byte[moves.Length];
        var valid = Validate(enc.Generation, enc.Species, enc.Form, enc.Version, moves, origins);
        if (valid)
        {
            moves.CopyTo(result);
            return true;
        }
        return GetExpectedMoves(enc.Generation, enc.Species, enc.Form, enc.Version, moves, origins, result);
    }

    /// <summary>
    /// A more expensive method of getting the expected moves the egg should come with, using an input of requested <see cref="moves"/> that are requested to be in the output.
    /// </summary>
    /// <remarks>Uses inputs calculated from <see cref="Validate"/>. Don't call this directly unless already parsed the input as invalid.</remarks>
    /// <returns>Expected moves for the encounter</returns>
    /// <inheritdoc cref="Validate"/>
    public static bool GetExpectedMoves(byte generation, ushort species, byte form, GameVersion version, ReadOnlySpan<ushort> moves, Span<byte> origins, Span<ushort> result)
    {
        // Try rearranging the order of the moves.
        // Group and order moves by their possible origin flags.
        Span<MoveOrder> expected = stackalloc MoveOrder[moves.Length];
        GetSortedMoveOrder(generation, moves, origins, expected);
        // Don't mutate the expected list anymore.

        // Temp buffer for the validation origin flags, unused in current scope but used inside the called method.
        Span<byte> temp = stackalloc byte[moves.Length];

        // Try checking if the rearranged order from above is valid.
        for (int i = 0; i < moves.Length; i++)
            result[i] = expected[i].Move;
        var valid = Validate(generation, species, form, version, result, temp);
        if (valid) // If true, the result buffer is now valid.
            return true;

        // Well, that didn't work; probably because one or more moves aren't valid.
        // Let's remove all present base moves, and get a fresh set of base moves.
        var learn = GameData.GetLearnSource(version);
        var learnset = learn.GetLearnset(species, form);
        var eggLevel = EggStateLegality.GetEggLevel(generation);
        var baseMoves = learnset.GetBaseEggMoves(eggLevel);

        RebuildMoves(baseMoves, expected, result);

        // Check if that worked...
        temp.Clear();
        valid = Validate(generation, species, form, version, result, temp);
        if (valid) // If true, the result buffer is now valid.
            return true;

        // Total failure; just return the base moves.
        for (int i = 0; i < baseMoves.Length; i++)
            result[i] = baseMoves[i];
        for (int i = baseMoves.Length; i < result.Length; i++)
            result[i] = 0;
        return false;
    }

    private static void GetSortedMoveOrder(byte generation, ReadOnlySpan<ushort> moves, Span<byte> origins, Span<MoveOrder> expected)
    {
        if (generation == 2)
        {
            GetSortedMoveOrder2(moves, origins, expected);
            return;
        }

        int count = 0;
        for (int i = moves.Length - 1; i >= 0; i--)
        {
            var origin = origins[i];
            if (origin == 0) // invalid/empty
                continue;
            int insertIndex = GetInsertIndex(expected, origin, count);
            if (insertIndex < count)
                ShiftAllItems(expected, insertIndex);
            expected[insertIndex] = new MoveOrder(moves[i], origin);
            count++;
        }
    }

    private static void ShiftAllItems(Span<MoveOrder> details, int insertIndex)
    {
        // Shifts all indexes starting at insertIndex to the right by one.
        // Empty slot at the end is overwritten, but that slot was zero (unused).
        for (int i = details.Length - 1; i > insertIndex; i--)
            details[i] = details[i - 1];
    }

    private static void GetSortedMoveOrder2(ReadOnlySpan<ushort> moves, Span<byte> origins, Span<MoveOrder> expected)
    {
        // Base moves first, then non-base.
        // Empty/invalid move slots are ignored -- default struct value is an empty move.
        int baseMoves = origins.Count((byte)EggSource2.Base);
        int ctrBase = 0;
        int ctrNonBase = 0;
        for (int i = 0; i < moves.Length; i++)
        {
            var origin = origins[i];
            if (origin == 0) // invalid/empty
                continue;

            int index = origin == (byte)EggSource2.Base ? ctrBase++ : baseMoves + ctrNonBase++;
            expected[index] = new MoveOrder(moves[i], origin);
        }
    }

    private static int GetInsertIndex(ReadOnlySpan<MoveOrder> expected, byte origin, int count)
    {
        // Return the first index that has an origin lower than the entry present in the slot.
        // If no such index exists, return the current count (insert at the end).
        int i = 0;
        for (; i < count; i++)
        {
            if (origin < expected[i].Origin)
                break;
        }
        return i;
    }

    private static void RebuildMoves(ReadOnlySpan<ushort> baseMoves, ReadOnlySpan<MoveOrder> expected, Span<ushort> result)
    {
        // Build a list of moves that are not present in the base moves list.
        // Use the expected order (sorted by origin flags) when assembling the result.
        Span<ushort> notBase = stackalloc ushort[expected.Length];
        int notBaseCount = 0;
        foreach (var (move, origin) in expected)
        {
            if (origin == 0) // invalid/empty
                continue;
            if (!baseMoves.Contains(move))
                notBase[notBaseCount++] = move;
        }

        int baseCount = expected.Length - notBaseCount;
        if (baseCount > baseMoves.Length)
            baseCount = baseMoves.Length;

        int ctr = 0;
        // Start with base moves
        for (; ctr < baseCount; ctr++)
            result[ctr] = baseMoves[baseMoves.Length - baseCount + ctr];
        // Then with non-base moves
        for (var i = 0; i < notBaseCount; i++)
            result[ctr++] = notBase[i];
        // Then clear the remainder
        result[ctr..].Clear();
    }

    /// <summary>
    /// Simple tuple to track the move and its possible origin flags.
    /// </summary>
    private readonly record struct MoveOrder(ushort Move, byte Origin);
}

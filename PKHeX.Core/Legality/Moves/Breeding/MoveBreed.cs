using System;
using System.Collections.Generic;
using System.Linq;

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
    public static bool Validate(int generation, int species, int form, GameVersion version, ReadOnlySpan<int> moves, Span<byte> origins) => generation switch
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
    /// <remarks>Validates the requested moves first prior to trying a more expensive computation.</remarks>
    public static int[] GetExpectedMoves(ReadOnlySpan<int> moves, IEncounterTemplate enc)
    {
        Span<byte> origins = stackalloc byte[4];
        var valid = Validate(enc.Generation, enc.Species, enc.Form, enc.Version, moves, origins);
        if (valid)
            return moves.ToArray();
        return GetExpectedMoves(enc.Generation, enc.Species, enc.Form, enc.Version, moves, origins);
    }

    /// <summary>
    /// A more expensive method of getting the expected moves the egg should come with, using an input of requested <see cref="moves"/> that are requested to be in the output.
    /// </summary>
    /// <remarks>Uses inputs calculated from <see cref="Validate"/>. Don't call this directly unless already parsed the input as invalid.</remarks>
    /// <returns>Expected moves for the encounter</returns>
    /// <inheritdoc cref="Validate"/>
    public static int[] GetExpectedMoves(int generation, int species, int form, GameVersion version, ReadOnlySpan<int> moves, Span<byte> origins)
    {
        // Try rearranging the order of the moves.
        // Build an info table
        var details = new MoveOrder[moves.Length];
        for (int i = moves.Length - 1; i >= 0; i--)
            details[i] = new MoveOrder((ushort) moves[i], origins[i]);

        // Kick empty slots to the end, then order by source priority.
        IOrderedEnumerable<MoveOrder> expect = generation != 2
            ? details.OrderBy(z => z.Move == 0).ThenBy(z => z.Source)
            : details.OrderBy(z => z.Move == 0).ThenBy(z => z.Source != (byte) EggSource2.Base);

        // Reorder the moves.
        var reorder1 = new int[moves.Length];
        var exp = expect.ToList();
        for (int i = 0; i < moves.Length; i++)
            reorder1[i] = exp[i].Move;

        // Check if that worked...
        Span<byte> temp = stackalloc byte[4];
        var valid = Validate(generation, species, form, version, reorder1, temp);
        if (valid)
            return reorder1;

        // Well, that didn't work; probably because the moves aren't valid. Let's remove all the base moves, and get a fresh set.
        var reorder2 = reorder1; // reuse instead of reallocate
        var learn = GameData.GetLearnsets(version);
        var table = GameData.GetPersonal(version);
        var index = table.GetFormIndex(species, form);
        var learnset = learn[index];
        var eggLevel = EggStateLegality.GetEggLevel(generation);
        var baseMoves = learnset.GetBaseEggMoves(eggLevel);

        RebuildMoves(baseMoves, exp, reorder2);

        // Check if that worked...
        temp.Clear();
        valid = Validate(generation, species, form, version, reorder2, temp);
        if (valid)
            return reorder2;

        // Total failure; just return the base moves.
        baseMoves.CopyTo(reorder2);
        for (int i = baseMoves.Length; i < reorder2.Length; i++)
            reorder2[i] = 0;
        return reorder2;
    }

    private static void RebuildMoves(ReadOnlySpan<int> baseMoves, List<MoveOrder> exp, int[] result)
    {
        var notBase = new List<int>();
        foreach (var m in exp)
        {
            if (m.Source == 0)
                continue; // invalid
            int move = m.Move;
            if (baseMoves.IndexOf(move) == -1)
                notBase.Add(move);
        }

        int baseCount = 4 - notBase.Count;
        if (baseCount > baseMoves.Length)
            baseCount = baseMoves.Length;
        int ctr = 0;
        for (; ctr < baseCount; ctr++)
            result[ctr] = baseMoves[baseMoves.Length - baseCount + ctr];
        foreach (var m in notBase)
            result[ctr++] = m;

        for (int i = ctr; i < result.Length; i++)
            result[i] = 0;
    }

    private readonly record struct MoveOrder(ushort Move, byte Source);
}

using System;
using System.Buffers;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Logic for suggesting a moveset.
/// </summary>
public static class MoveListSuggest
{
    private static ushort[] GetSuggestedMoves(PKM pk, EvolutionHistory evoChains, MoveSourceType types, IEncounterTemplate enc)
    {
        if (pk.IsEgg && pk.Format <= 5) // pre relearn
        {
            var moves = new ushort[4];
            MoveList.GetCurrentMoves(pk, pk.Species, 0, (GameVersion)pk.Version, pk.CurrentLevel, moves);
            return moves;
        }

        if (types != MoveSourceType.None)
            return GetValidMoves(pk, enc, evoChains, types);

        // try to give current moves
        if (enc.Generation <= 2)
        {
            var lvl = pk.Format >= 7 ? pk.Met_Level : pk.CurrentLevel;
            var ver = enc.Version;
            return MoveLevelUp.GetEncounterMoves(enc.Species, 0, lvl, ver);
        }

        if (pk.Species == enc.Species)
        {
            return MoveLevelUp.GetEncounterMoves(pk.Species, pk.Form, pk.CurrentLevel, (GameVersion)pk.Version);
        }

        return GetValidMoves(pk, enc, evoChains, types);
    }

    private static ushort[] GetValidMoves(PKM pk, IEncounterTemplate enc, EvolutionHistory evoChains, MoveSourceType types = MoveSourceType.ExternalSources)
    {
        var length = pk.MaxMoveID + 1;
        bool[] rent = ArrayPool<bool>.Shared.Rent(length);
        var span = rent.AsSpan(0, length);
        LearnPossible.Get(pk, enc, evoChains, span, types);

        var count = span[1..].Count(true);
        var result = new ushort[count];
        int ctr = 0;
        for (ushort i = 1; i < span.Length; i++)
        {
            if (rent[i])
                result[ctr++] = i;
        }

        span.Clear();
        ArrayPool<bool>.Shared.Return(rent);
        return result;
    }

    /// <summary>
    /// Gets four moves which can be learned depending on the input arguments.
    /// </summary>
    /// <param name="analysis">Parse information to generate a moveset for.</param>
    /// <param name="types">Allowed move sources for populating the result array</param>
    public static ushort[] GetSuggestedCurrentMoves(this LegalityAnalysis analysis, MoveSourceType types = MoveSourceType.All)
    {
        if (!analysis.Parsed)
            return new ushort[4];
        var pk = analysis.Entity;
        if (pk.IsEgg && pk.Format >= 6)
            return pk.RelearnMoves;

        if (pk.IsEgg)
            types = types.ClearNonEggSources();

        var info = analysis.Info;
        return GetSuggestedMoves(pk, info.EvoChainsAllGens, types, info.EncounterOriginal);
    }

    /// <summary>
    /// Gets the current <see cref="PKM.RelearnMoves"/> array of four moves that might be legal.
    /// </summary>
    /// <remarks>Use <see cref="GetSuggestedRelearnMovesFromEncounter"/> instead of calling directly; this method just puts default values in without considering the final moveset.</remarks>
    public static IReadOnlyList<ushort> GetSuggestedRelearn(this IEncounterTemplate enc, PKM pk)
    {
        if (LearnVerifierRelearn.ShouldNotHaveRelearnMoves(enc, pk))
            return Empty;

        return GetSuggestedRelearnInternal(enc, pk);
    }

    // Invalid encounters won't be recognized as an EncounterEgg; check if it *should* be a bred egg.
    private static IReadOnlyList<ushort> GetSuggestedRelearnInternal(this IEncounterTemplate enc, PKM pk) => enc switch
    {
        IRelearn { Relearn: { HasMoves: true } r } => r.ToArray(),
        EncounterEgg or EncounterInvalid {EggEncounter: true} => GetSuggestedRelearnEgg(enc, pk),
        _ => Empty,
    };

    private static ushort[] GetSuggestedRelearnEgg(IEncounterTemplate enc, PKM pk)
    {
        Span<ushort> current = stackalloc ushort[4];
        pk.GetRelearnMoves(current);
        Span<ushort> expected = stackalloc ushort[current.Length];
        _ = MoveBreed.GetExpectedMoves(current, enc, expected);
        return expected.ToArray();
    }

    private static readonly IReadOnlyList<ushort> Empty = new ushort[4];

    /// <summary>
    /// Gets the current <see cref="PKM.RelearnMoves"/> array of four moves that might be legal.
    /// </summary>
    public static IReadOnlyList<ushort> GetSuggestedRelearnMovesFromEncounter(this LegalityAnalysis analysis, IEncounterTemplate? enc = null)
    {
        var info = analysis.Info;
        enc ??= info.EncounterOriginal;
        var pk = analysis.Entity;

        if (LearnVerifierRelearn.ShouldNotHaveRelearnMoves(enc, pk))
            return Empty;

        if (enc is EncounterEgg or EncounterInvalid {EggEncounter: true})
            return enc.GetSuggestedRelearnEgg(info.Moves, pk);
        return enc.GetSuggestedRelearnInternal(pk);
    }

    private static IReadOnlyList<ushort> GetSuggestedRelearnEgg(this IEncounterTemplate enc, ReadOnlySpan<MoveResult> parse, PKM pk)
    {
        var result = enc.GetEggRelearnMoves(parse, pk);
        int generation = enc.Generation;
        if (generation <= 5) // gen2 does not have splitbreed, <=5 do not have relearn moves and shouldn't even be here.
            return result;

        // Split-breed species like Budew & Roselia may be legal for one, and not the other.
        // If we're not a split-breed or are already legal, return.
        var split = Breeding.GetSplitBreedGeneration(generation);
        if (!split.Contains(enc.Species))
            return result;

        var tmp = pk.Clone();
        tmp.SetRelearnMoves(result);
        var la = new LegalityAnalysis(tmp);
        var moves = la.Info.Moves;
        if (MoveResult.AllValid(moves))
            return result;

        // Try again with the other split-breed species if possible.
        var other = EncounterEggGenerator.GenerateEggs(tmp, generation);
        foreach (var incense in other)
        {
            if (incense.Species != enc.Species)
                return incense.GetEggRelearnMoves(parse, pk);
        }
        return result;
    }

    private static ushort[] GetEggRelearnMoves(this IEncounterTemplate enc, ReadOnlySpan<MoveResult> parse, PKM pk)
    {
        // Extract a list of the moves that should end up in the relearn move list.
        Span<ushort> moves = stackalloc ushort[parse.Length];
        LoadRelearnFlagged(moves, parse, pk);

        Span<ushort> expected = stackalloc ushort[moves.Length];
        _ = MoveBreed.GetExpectedMoves(moves, enc, expected);
        return expected.ToArray();
    }

    private static void LoadRelearnFlagged(Span<ushort> moves, ReadOnlySpan<MoveResult> parse, PKM pk)
    {
        // Loads only indexes that are flagged as relearn moves
        int count = 0;
        for (int index = 0; index < parse.Length; index++)
        {
            var move = parse[index];
            if (move.ShouldBeInRelearnMoves())
                moves[count++] = pk.GetMove(index);
        }
    }
}

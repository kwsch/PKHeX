using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core;

public static class MoveListSuggest
{
    private static int[] GetSuggestedMoves(PKM pk, EvolutionHistory evoChains, MoveSourceType types, IEncounterTemplate enc)
    {
        if (pk.IsEgg && pk.Format <= 5) // pre relearn
            return MoveList.GetBaseEggMoves(pk, pk.Species, 0, (GameVersion)pk.Version, pk.CurrentLevel);

        if (types != MoveSourceType.None)
            return GetValidMoves(pk, evoChains, types).Skip(1).ToArray(); // skip move 0

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

        return GetValidMoves(pk, evoChains, types).Skip(1).ToArray(); // skip move 0
    }

    private static IEnumerable<int> GetValidMoves(PKM pk, EvolutionHistory evoChains, MoveSourceType types = MoveSourceType.ExternalSources, bool RemoveTransferHM = true)
    {
        var (_, version) = pk.IsMovesetRestricted();
        return GetValidMoves(pk, version, evoChains, types: types, RemoveTransferHM: RemoveTransferHM);
    }

    private static IEnumerable<int> GetValidMoves(PKM pk, GameVersion version, EvolutionHistory evoChains, MoveSourceType types = MoveSourceType.Reminder, bool RemoveTransferHM = true)
    {
        var r = new List<int> { 0 };
        if (types.HasFlagFast(MoveSourceType.RelearnMoves) && pk.Format >= 6)
            r.AddRange(pk.RelearnMoves);

        int start = pk.Generation;
        if (start < 0)
            start = pk.Format; // be generous instead of returning nothing
        if (pk is IBattleVersion b)
            start = Math.Max(0, b.GetMinGeneration());

        for (int generation = start; generation <= pk.Format; generation++)
        {
            var chain = evoChains[generation];
            if (chain.Length == 0)
                continue;
            r.AddRange(MoveList.GetValidMoves(pk, version, chain, generation, types: types, RemoveTransferHM: RemoveTransferHM));
        }

        return r.Distinct();
    }

    private static IEnumerable<int> AllSuggestedMoves(this LegalityAnalysis analysis)
    {
        if (!analysis.Parsed)
            return new int[4];
        return analysis.GetSuggestedCurrentMoves();
    }

    private static IEnumerable<int> AllSuggestedRelearnMoves(this LegalityAnalysis analysis)
    {
        if (!analysis.Parsed)
            return new int[4];
        var pk = analysis.Entity;
        var enc = analysis.EncounterMatch;
        return MoveList.GetValidRelearn(pk, enc.Species, enc.Form, (GameVersion)pk.Version).ToArray();
    }

    public static int[] GetSuggestedMovesAndRelearn(this LegalityAnalysis analysis)
    {
        if (!analysis.Parsed)
            return new int[4];
        return analysis.AllSuggestedMoves().Concat(analysis.AllSuggestedRelearnMoves()).ToArray();
    }

    /// <summary>
    /// Gets four moves which can be learned depending on the input arguments.
    /// </summary>
    /// <param name="analysis">Parse information to generate a moveset for.</param>
    /// <param name="types">Allowed move sources for populating the result array</param>
    public static int[] GetSuggestedCurrentMoves(this LegalityAnalysis analysis, MoveSourceType types = MoveSourceType.All)
    {
        if (!analysis.Parsed)
            return new int[4];
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
    public static IReadOnlyList<int> GetSuggestedRelearn(this IEncounterTemplate enc, PKM pk)
    {
        if (LearnVerifierRelearn.ShouldNotHaveRelearnMoves(enc, pk))
            return Empty;

        return GetSuggestedRelearnInternal(enc, pk);
    }

    // Invalid encounters won't be recognized as an EncounterEgg; check if it *should* be a bred egg.
    private static IReadOnlyList<int> GetSuggestedRelearnInternal(this IEncounterTemplate enc, PKM pk) => enc switch
    {
        IRelearn s when s.Relearn.Count > 0 => s.Relearn,
        EncounterEgg or EncounterInvalid {EggEncounter: true} => MoveBreed.GetExpectedMoves(pk.RelearnMoves, enc).ToArray(),
        _ => Empty,
    };

    private static readonly IReadOnlyList<int> Empty = new int[4];

    /// <summary>
    /// Gets the current <see cref="PKM.RelearnMoves"/> array of four moves that might be legal.
    /// </summary>
    public static IReadOnlyList<int> GetSuggestedRelearnMovesFromEncounter(this LegalityAnalysis analysis, IEncounterTemplate? enc = null)
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

    private static IReadOnlyList<int> GetSuggestedRelearnEgg(this IEncounterTemplate enc, MoveResult[] parse, PKM pk)
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
        if (la.Info.Moves.All(z => z.Valid))
            return result;

        // Try again with the other split-breed species if possible.
        var incense = EncounterEggGenerator.GenerateEggs(tmp, generation).FirstOrDefault();
        if (incense is null || incense.Species == enc.Species)
            return result;

        return incense.GetEggRelearnMoves(parse, pk);
    }

    private static IReadOnlyList<int> GetEggRelearnMoves(this IEncounterTemplate enc, MoveResult[] parse, PKM pk)
    {
        // Extract a list of the moves that should end up in the relearn move list.
        int ctr = 0;
        var moves = new int[4];
        for (var i = 0; i < parse.Length; i++)
        {
            var m = parse[i];
            if (!m.ShouldBeInRelearnMoves())
                continue;
            moves[ctr++] = pk.GetMove(i);
        }

        // Swap Volt Tackle to the end of the list.
        int volt = Array.IndexOf(moves, (int) Move.VoltTackle, 0, ctr);
        if (volt != -1)
        {
            var dest = ctr - 1;
            moves[volt] = moves[dest];
            moves[dest] = (int) Move.VoltTackle;
        }
        return MoveBreed.GetExpectedMoves(moves, enc);
    }
}

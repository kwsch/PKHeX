using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace PKHeX.Core.AutoMod;

public static class BattleTemplateLegality
{
    public static string ANALYSIS_INVALID { get; set; } = "Specific analysis for this set is unavailable.";
    public static string EXHAUSTED_ENCOUNTERS { get; set; } = "No valid matching encounter available: (Exhausted {0}/{1} possible encounters).";
    public static string SPECIES_UNAVAILABLE_FORM { get; set; } = "{0} with form {1} is unavailable in this game.";
    public static string SPECIES_UNAVAILABLE { get; set; } = "{0} is unavailable in the game.";
    public static string INVALID_MOVES { get; set; } = "{0} cannot learn the following move(s) in this game: {1}.";
    public static string ALL_MOVES_INVALID { get; set; } = "All the requested moves for this Pokémon are invalid.";
    public static string LEVEL_INVALID { get; set; } = "Requested level is lower than the minimum possible level for {0}. Minimum required level is {1}.";
    public static string SHINY_INVALID { get; set; } = "Requested shiny value (ShinyType.{0}) is not possible for the given set.";
    public static string ALPHA_INVALID { get; set; } = "Requested Pokémon cannot be an Alpha.";
    public static string BALL_INVALID { get; set; } = "{0} Ball is not possible for the given set.";
    public static string ONLY_HIDDEN_ABILITY_AVAILABLE { get; set; } = "You can only obtain {0} with hidden ability in this game.";
    public static string HIDDEN_ABILITY_UNAVAILABLE { get; set; } = "You cannot obtain {0} with hidden ability in this game.";
    public static string HOME_TRANSFER_ONLY { get; set; } = "{0} is only available in this game through Home Transfer.";
    public static string BAD_WORDS { get; set; } = "{0}'s nickname, OT or HT contains a filtered word.";

    public static string SetAnalysis(this IBattleTemplate set, ITrainerInfo sav, PKM failed)
    {
        if (failed.Version == 0)
            failed.Version = sav.Version;

        var species_name = SpeciesName.GetSpeciesNameGeneration(set.Species, (int)LanguageID.English, sav.Generation);

        // Species checks
        var gv = sav.Version;
        if (!gv.ExistsInGame(set.Species, set.Form))
        {
            // Species does not exist in the game
            return set.Form == 0 ? string.Format(SPECIES_UNAVAILABLE, species_name) : string.Format(SPECIES_UNAVAILABLE_FORM, species_name, set.FormName);
        }

        // Species exists -- check if it has at least one move.
        // If it has no moves, and it didn't generate, that makes the mon still illegal in game (moves are set to legal ones)
        Memory<ushort> moves = set.Moves;
        var empty = moves.Span.IndexOf<ushort>(0);
        if (empty != -1)
            moves = moves[..empty];

        // Reusable data
        var destVer = sav.Version;
        if (destVer <= 0 && sav is SaveFile s)
            destVer = s.Version;

        var versions = APILegality.FilteredGameList(failed, destVer, APILegality.AllowBatchCommands, set);

        // Move checks
        var bestCombination = GetValidMovesetWithMostPresent(set, sav, moves, failed, versions);
        if (bestCombination.Length != moves.Length)
        {
            if (bestCombination.Length == 0)
                return ALL_MOVES_INVALID;
            var sb = new StringBuilder();
            AddMovesNotPresentIn(moves.Span, bestCombination, sb);
            return string.Format(INVALID_MOVES, species_name, sb);
        }

        // All moves possible, get encounters
        var encounters = EncounterMovesetGenerator.GenerateEncounters(pk: failed, moves, versions).ToList();
        var initialcount = encounters.Count;
        if (set is RegenTemplate { Regen.EncounterFilters: { } x })
            encounters.RemoveAll(enc => !BatchEditingUtil.IsFilterMatch(x, enc));

        // No available encounters
        if (encounters.Count == 0)
            return string.Format(EXHAUSTED_ENCOUNTERS, initialcount, initialcount);

        // Level checks, check if level is impossible to achieve
        if (encounters.All(z => !APILegality.IsRequestedLevelValid(set, z)))
            return string.Format(LEVEL_INVALID, species_name, encounters.Min(z => z.LevelMin));

        encounters.RemoveAll(enc => !APILegality.IsRequestedLevelValid(set, enc));

        // Shiny checks, check if shiny is impossible to achieve
        Shiny shinytype = set.Shiny ? Shiny.Always : Shiny.Never;
        if (set is RegenTemplate { Regen.HasExtraSettings: true } ret)
            shinytype = ret.Regen.Extra.ShinyType;

        if (encounters.All(z => !APILegality.IsRequestedShinyValid(set, z)))
            return string.Format(SHINY_INVALID, shinytype);

        encounters.RemoveAll(enc => !APILegality.IsRequestedShinyValid(set, enc));

        // Alpha checks
        if (encounters.All(z => !APILegality.IsRequestedAlphaValid(set, z)))
            return ALPHA_INVALID;

        encounters.RemoveAll(enc => !APILegality.IsRequestedAlphaValid(set, enc));
        if (WordFilter.IsFiltered(failed.Nickname, failed.Context, out _, out _) || WordFilter.IsFiltered(failed.OriginalTrainerName, failed.Context, out _, out _) || WordFilter.IsFiltered(failed.HandlingTrainerName, failed.Context, out _, out _))
            return string.Format(BAD_WORDS, species_name);

        // Ability checks
        var abilityreq = APILegality.GetRequestedAbility(failed, set);
        if (abilityreq == AbilityRequest.NotHidden && encounters.All(z => z is { Ability: AbilityPermission.OnlyHidden }))
            return string.Format(ONLY_HIDDEN_ABILITY_AVAILABLE, species_name);

        if (abilityreq == AbilityRequest.Hidden && encounters.All(z => z.Generation is 3 or 4) && destVer.Generation < 8)
            return string.Format(HIDDEN_ABILITY_UNAVAILABLE, species_name);

        // Home Checks
        if (!APILegality.AllowHOME)
        {
            if (encounters.All(z => HomeTrackerUtil.IsRequired(z, failed)))
                return string.Format(HOME_TRANSFER_ONLY, species_name);

            encounters.RemoveAll(enc => HomeTrackerUtil.IsRequired(enc, failed));
        }

        // Ball checks
        if (set is RegenTemplate { Regen.HasExtraSettings: true } regt)
        {
            var ball = regt.Regen.Extra.Ball;
            if (encounters.All(z => !APILegality.IsRequestedBallValid(set, z)))
                return string.Format(BALL_INVALID, ball);

            encounters.RemoveAll(enc => !APILegality.IsRequestedBallValid(set, enc));
        }

        return string.Format(EXHAUSTED_ENCOUNTERS, initialcount - encounters.Count, initialcount);
    }

    private static void AddMovesNotPresentIn(ReadOnlySpan<ushort> check, ReadOnlySpan<ushort> set, StringBuilder sb)
    {
        foreach (var move in check)
        {
            if (set.Contains(move))
                continue;
            if (move == 0)
                continue;
            if (sb.Length > 0)
                sb.Append(", ");
            sb.Append($"{(Move)move}");
        }
    }

    private static ReadOnlySpan<ushort> GetValidMovesetWithMostPresent(IBattleTemplate set, ITrainerInfo sav, Memory<ushort> moves, PKM blank, ReadOnlyMemory<GameVersion> versions)
    {
        if (sav.Generation <= 2)
            blank.EXP = 0; // no relearn moves in gen 1/2 so pass level 1 to generator

        // Eager check: current moveset is valid
        if (HasAnyEncounterForMoves(set, blank, moves, versions))
            return moves.Span;

        // Okay, at least one move is invalid. Recursively permute combinations to find the moveset with most moves valid.
        moves = moves.ToArray(); // copy to not disturb the original array.
        var count = Recurse(set, moves, blank, versions, [..moves.Span]);
        // The moves array is now the most-populated combination of moves that are valid.
        return moves.Span[..count];
    }

    private static int Recurse(IBattleTemplate set, Memory<ushort> request, PKM blank, ReadOnlyMemory<GameVersion> versions, List<ushort> moves)
    {
        if (moves.Count <= 1)
            return 0;

        // Breadth first search to find the most valid moveset -- remove one move and check, and restore if not.
        request = request[..(moves.Count - 1)];
        for (int i = 0; i < moves.Count; i++)
        {
            // Original order doesn't matter, skip an array copy shift when reinserting
            // This essentially cycles them like a queue
            var move = moves[0];
            moves.RemoveAt(0);
            moves.CopyTo(request.Span);
            if (HasAnyEncounterForMoves(set, blank, request, versions))
                return moves.Count;
            moves.Add(move);
        }

        // If above failed, recurse the same as above with more moves removed.
        for (int i = 0; i < moves.Count; i++)
        {
            var move = moves[0];
            moves.RemoveAt(0);
            var count = Recurse(set, request, blank, versions, moves);
            if (count != 0) // ignore 0, the removed move might be valid in a different combination
                return count;
            moves.Add(move);
        }
        return 0;
    }

    private static bool HasAnyEncounterForMoves(IBattleTemplate set, PKM blank, ReadOnlyMemory<ushort> moves, ReadOnlyMemory<GameVersion> versions)
    {
        // Do we even need to set the moves to the template?
        Span<ushort> tmp = stackalloc ushort[4];
        moves.Span.CopyTo(tmp);
        blank.SetMoves(tmp);

        var encounters = EncounterMovesetGenerator.GenerateEncounters(blank, moves, versions);
        if (set is not RegenTemplate { Regen.EncounterFilters: { Count: not 0 } x })
            return encounters.Any();
        encounters = encounters.Where(enc => BatchEditingUtil.IsFilterMatch(x, enc));
        return encounters.Any();
    }
}

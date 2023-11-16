using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core;

/// <summary>
/// Logic to check if things can learn specific moves.
/// </summary>
public static class EncounterLearn
{
    /// <summary>
    /// Default response if there are no matches.
    /// </summary>
    public const string NoMatches = "None";

    private const string DefaultLanguage = GameLanguage.DefaultLanguage;

    /// <summary>
    /// Checks if a <see cref="species"/> can learn all input <see cref="moves"/>.
    /// </summary>
    public static bool CanLearn(string species, IEnumerable<string> moves, byte form = 0, string lang = DefaultLanguage)
    {
        var encounters = GetLearn(species, moves, form, lang);
        return encounters.Any();
    }

    /// <summary>
    /// Gets a summary of all encounters where a <see cref="species"/> can learn all input <see cref="moves"/>.
    /// </summary>
    public static IEnumerable<string> GetLearnSummary(string species, IEnumerable<string> moves, byte form = 0, string lang = DefaultLanguage)
    {
        var encounters = GetLearn(species, moves, form, lang);
        var msg = Summarize(encounters).ToList();
        if (msg.Count == 0)
            msg.Add(NoMatches);
        return msg;
    }

    /// <summary>
    /// Gets all encounters where a <see cref="species"/> can learn all input <see cref="moves"/>.
    /// </summary>
    public static IEnumerable<IEncounterable> GetLearn(string species, IEnumerable<string> moves, byte form = 0, string lang = DefaultLanguage)
    {
        var str = GameInfo.GetStrings(lang);

        var speciesID = StringUtil.FindIndexIgnoreCase(str.specieslist, species);
        if (speciesID <= 0)
            return [];

        var allMoves = moves.ToList();
        var span = new ushort[allMoves.Count];
        for (int i = 0; i < span.Length; i++)
        {
            var move = allMoves[i];
            var index = StringUtil.FindIndexIgnoreCase(str.movelist, move);
            if (index <= 0)
                return [];
            span[i] = (ushort)index;
        }

        return GetLearn((ushort)speciesID, span, form);
    }

    /// <summary>
    /// Gets all encounters where a <see cref="species"/> can learn all input <see cref="moves"/>.
    /// </summary>
    public static IEnumerable<IEncounterable> GetLearn(ushort species, ushort[] moves, byte form = 0)
    {
        if (species == 0)
            return [];
        if (moves.AsSpan().Contains<ushort>(0))
            return [];

        var vers = GameUtil.GameVersions;
        return GetLearnInternal(species, form, moves, vers);
    }

    private static IEnumerable<IEncounterable> GetLearnInternal(ushort species, byte form, ushort[] moves, GameVersion[] vers)
    {
        bool iterated = false;
        if (PersonalTable.SV.IsPresentInGame(species, form))
        {
            var blank = new PK9 { Species = species, Form = form };
            var encs = EncounterMovesetGenerator.GenerateEncounters(blank, moves, vers);
            foreach (var enc in encs)
                yield return enc;
        }
        if (PersonalTable.LA.IsPresentInGame(species, form))
        {
            var blank = new PA8 { Species = species, Form = form };
            var encs = EncounterMovesetGenerator.GenerateEncounters(blank, moves, GameVersion.PLA);
            foreach (var enc in encs)
                yield return enc;
            iterated = true;
        }
        if (PersonalTable.BDSP.IsPresentInGame(species, form))
        {
            var blank = new PB8 { Species = species, Form = form };
            var encs = EncounterMovesetGenerator.GenerateEncounters(blank, moves, GameVersion.BD, GameVersion.SP);
            foreach (var enc in encs)
                yield return enc;
            iterated = true;
        }
        if (PersonalTable.SWSH.IsPresentInGame(species, form))
        {
            var blank = new PK8 { Species = species, Form = form };
            var v = vers.Where(z => z <= GameVersion.SH).ToArray();
            var encs = EncounterMovesetGenerator.GenerateEncounters(blank, moves, v);
            foreach (var enc in encs)
                yield return enc;
            iterated = true;
        }
        if (!iterated && PersonalTable.USUM.IsPresentInGame(species, form))
        {
            var blank = new PK7 { Species = species, Form = form };
            var encs = EncounterMovesetGenerator.GenerateEncounters(blank, moves, vers);
            foreach (var enc in encs)
                yield return enc;
        }
    }

    /// <summary>
    /// Summarizes all encounters by grouping by <see cref="IEncounterable.Name"/>.
    /// </summary>
    public static IEnumerable<string> Summarize(IEnumerable<IEncounterable> encounters, bool advanced = false)
    {
        var types = encounters.GroupBy(z => z.Name);
        return Summarize(types, advanced);
    }

    /// <summary>
    /// Summarizes groups of encounters.
    /// </summary>
    public static IEnumerable<string> Summarize(IEnumerable<IGrouping<string, IEncounterable>> types, bool advanced = false)
    {
        return types.SelectMany(g => EncounterSummary.SummarizeGroup(g, g.Key, advanced));
    }
}

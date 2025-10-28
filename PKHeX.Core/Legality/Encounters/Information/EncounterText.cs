using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Logic for creating a formatted text summary of an encounter.
/// </summary>
public static class EncounterText
{
    private static EncounterDisplayContext GetContext(string language = GameLanguage.DefaultLanguage) => new()
    {
        Localization = EncounterDisplayLocalization.Get(language),
        Strings = GameInfo.GetStrings(language),
    };

    public static IReadOnlyList<string> GetTextLines(this IEncounterInfo enc, bool verbose = false, string language = GameLanguage.DefaultLanguage) => GetTextLines(enc, GetContext(language), verbose);

    public static IReadOnlyList<string> GetTextLines(this IEncounterInfo enc, EncounterDisplayContext ctx, bool verbose = false)
    {
        var lines = new List<string>();
        var loc = ctx.Localization;
        var name = ctx.GetSpeciesName(enc);
        lines.Add(string.Format(loc.Format, loc.EncounterType, name));

        if (enc is MysteryGift mg)
        {
            lines.AddRange(mg.GetDescription());
        }
        else if (enc is IMoveset m)
        {
            var moves = m.Moves;
            if (moves.HasMoves)
                lines.Add(ctx.GetMoveset(moves));
        }

        var location = enc.GetEncounterLocation(enc.Generation, enc.Version);
        if (!string.IsNullOrEmpty(location))
            lines.Add(string.Format(loc.Format, loc.Location, location));

        lines.Add(ctx.GetVersionDisplay(enc));
        lines.Add(ctx.GetLevelDisplay(enc));

        if (!verbose)
            return lines;

        // Record types! Can get a nice summary.
        // Won't work neatly for Mystery Gift types since those aren't record types, plus they have way too many properties.
        if (enc is not MysteryGift)
        {
            // ReSharper disable once ConstantNullCoalescingCondition
            var raw = enc.ToString() ?? throw new ArgumentNullException(nameof(enc));
            lines.AddRange(raw.Split(',', '}', '{'));
        }
        return lines;
    }
}

public readonly record struct EncounterDisplayContext
{
    public required EncounterDisplayLocalization Localization { get; init; }
    public required GameStrings Strings { get; init; }

    public string GetSpeciesName(IEncounterTemplate enc)
    {
        var encSpecies = enc.Species;
        var str = Strings.Species;
        var name = (uint)encSpecies < str.Count ? str[encSpecies] : encSpecies.ToString();
        var EncounterName = $"{(enc is IEncounterable ie ? ie.LongName : "Special")} ({name})";
        return EncounterName;
    }

    public string GetMoveset(Moveset moves) => moves.GetMovesetLine(Strings.movelist);

    public string GetVersionDisplay(IEncounterTemplate enc)
    {
        var version = enc.Version;
        var versionName = enc.Version.IsValidSavedVersion()
            ? Strings.gamelist[(int)enc.Version]
            : enc.Version.ToString();

        return string.Format(Localization.Format, Localization.Version, versionName);
    }

    public string GetLevelDisplay(IEncounterTemplate enc)
    {
        if (enc.LevelMin == enc.LevelMax)
            return string.Format(Localization.Format, Localization.Level, enc.LevelMin);
        return string.Format(Localization.Format, Localization.LevelRange, enc.LevelMin, enc.LevelMax);
    }
}

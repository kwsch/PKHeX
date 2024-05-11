using System;
using System.Collections.Generic;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core;

/// <summary>
/// Logic for creating a formatted text summary of an encounter.
/// </summary>
public static class EncounterText
{
    public static IReadOnlyList<string> GetTextLines(this IEncounterInfo enc) => GetTextLines(enc, GameInfo.Strings);

    public static IReadOnlyList<string> GetTextLines(this IEncounterInfo enc, GameStrings strings)
    {
        var lines = new List<string>();
        var str = strings.Species;
        var name = (uint)enc.Species < str.Count ? str[enc.Species] : enc.Species.ToString();
        var EncounterName = $"{(enc is IEncounterable ie ? ie.LongName : "Special")} ({name})";
        lines.Add(string.Format(L_FEncounterType_0, EncounterName));
        if (enc is MysteryGift mg)
        {
            lines.AddRange(mg.GetDescription());
        }
        else if (enc is IMoveset m)
        {
            var moves = m.Moves;
            if (moves.HasMoves)
            {
                string result = moves.GetMovesetLine(strings.movelist);
                lines.Add(result);
            }
        }

        var loc = enc.GetEncounterLocation(enc.Generation, enc.Version);
        if (!string.IsNullOrEmpty(loc))
            lines.Add(string.Format(L_F0_1, "Location", loc));

        var game = enc.Version.IsValidSavedVersion() ? strings.gamelist[(int)enc.Version] : enc.Version.ToString();
        lines.Add(string.Format(L_F0_1, nameof(GameVersion), game));
        lines.Add(enc.LevelMin == enc.LevelMax
            ? $"Level: {enc.LevelMin}"
            : $"Level: {enc.LevelMin}-{enc.LevelMax}");

#if DEBUG
        // Record types! Can get a nice summary.
        // Won't work neatly for Mystery Gift types since those aren't record types.
        if (enc is not MysteryGift)
        {
            // ReSharper disable once ConstantNullCoalescingCondition
            var raw = enc.ToString() ?? throw new ArgumentNullException(nameof(enc));
            lines.AddRange(raw.Split(',', '}', '{'));
        }
#endif
        return lines;
    }
}

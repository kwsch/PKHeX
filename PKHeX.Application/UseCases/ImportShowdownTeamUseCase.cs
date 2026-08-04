using PKHeX.Core;

namespace PKHeX.Application.UseCases;

public readonly record struct ImportShowdownTeamResult(int Imported, IReadOnlyList<string> SetErrors, string? FatalError)
{
    public bool Success => FatalError is null;
}

/// <summary>
/// Parses multi-set Showdown text (blank-line separated) and places each Pokémon into the first
/// empty slots of the target box. All-or-nothing on space: if the valid sets outnumber the empty
/// slots, nothing is written. Malformed sets are skipped and reported individually.
/// </summary>
public sealed class ImportShowdownTeamUseCase
{
    public ImportShowdownTeamResult Execute(SaveFile sav, string? text, int box)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new ImportShowdownTeamResult(0, [], "Clipboard is empty.");

        var lines = text.Split('\n').Select(l => l.TrimEnd('\r'));
        var sets = ShowdownParsing.GetShowdownSets(lines).ToList();
        if (sets.Count == 0)
            return new ImportShowdownTeamResult(0, [], "No Showdown sets found in the text.");

        var errors = new List<string>();
        var pokemon = new List<PKM>();
        for (int i = 0; i < sets.Count; i++)
        {
            var set = sets[i];
            if (set.Species <= 0)
            {
                var reason = set.InvalidLines.Count > 0
                    ? string.Join("; ", set.InvalidLines.Select(z => z.ToString()))
                    : "Unrecognized species.";
                errors.Add($"Set {i + 1}: {reason}");
                continue;
            }

            var pk = sav.BlankPKM;
            pk.ApplySetDetails(set);
            if (pk.Format >= 8)
                pk.Nature = pk.StatAlignment;
            pk.SetPIDGender(pk.Gender);
            pokemon.Add(pk);
        }

        if (pokemon.Count == 0)
            return new ImportShowdownTeamResult(0, errors, "No valid Showdown sets found.");

        var emptySlots = new List<int>();
        for (int slot = 0; slot < sav.BoxSlotCount; slot++)
        {
            if (sav.GetBoxSlotAtIndex(box, slot).Species == 0)
                emptySlots.Add(slot);
        }

        if (pokemon.Count > emptySlots.Count)
            return new ImportShowdownTeamResult(0, errors,
                $"Not enough empty slots in the current box: need {pokemon.Count}, have {emptySlots.Count}. Nothing was imported.");

        for (int i = 0; i < pokemon.Count; i++)
            sav.SetBoxSlotAtIndex(pokemon[i], box, emptySlots[i]);

        return new ImportShowdownTeamResult(pokemon.Count, errors, null);
    }
}

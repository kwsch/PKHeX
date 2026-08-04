using PKHeX.Core;

namespace PKHeX.Application.UseCases;

public readonly record struct ImportShowdownResult(bool Success, PKM? Pokemon, string? Error);

/// <summary>
/// Parses Showdown set text into a Pokémon for the given save. Application business rule extracted
/// from the main window's "Import Showdown" command.
/// </summary>
public sealed class ImportShowdownSetUseCase
{
    public ImportShowdownResult Execute(SaveFile sav, string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new ImportShowdownResult(false, null, "Clipboard is empty.");

        var set = new ShowdownSet(text.Trim());
        if (set.Species <= 0)
            return new ImportShowdownResult(false, null, "Invalid Showdown set text.");

        var pk = sav.BlankPKM;
        pk.ApplySetDetails(set);
        if (pk.Format >= 8)
            pk.Nature = pk.StatAlignment;
        pk.SetPIDGender(pk.Gender);
        return new ImportShowdownResult(true, pk, null);
    }
}

using PKHeX.Core;

namespace PKHeX.Application.UseCases;

/// <summary>Converts a Pokémon to Showdown set text. Returns null for an empty slot.</summary>
public sealed class ExportShowdownSetUseCase
{
    public string? Execute(PKM pk) => pk.Species == 0 ? null : new ShowdownSet(pk).Text;
}

using static PKHeX.Core.LegalityCheckResultCode;
using static PKHeX.Core.CheckIdentifier;
using static PKHeX.Core.Species;

namespace PKHeX.Core;

/// <summary>
/// Special case handling for <see cref="EvolutionType"/> for Trade evolutions that have version-specific branches.
/// </summary>
internal sealed class MiscEvolutionVerifier : Verifier
{
    protected override CheckIdentifier Identifier => Misc;

    public override void Verify(LegalityAnalysis data) => Verify(data, data.Entity);

    internal void Verify(LegalityAnalysis data, PKM pk)
    {
        if (pk.Format < 7)
            return; // Version specific evolutions did not exist yet.

        // Only relevant if evolved.
        if (data.EncounterMatch.Species == pk.Species)
            return;

        // No point using the evolution tree. Just handle certain species.
        // If it has evolved, check if it was an evolution from another game.
        // If it was untraded, then it couldn't have evolved, so it's illegal.
        switch (pk.Species)
        {
            // Rockruff only evolves into:
            // Midday Form Lycanroc in Pokémon Sun and Ultra Sun,
            // Midnight Form Lycanroc in Pokémon Moon and Ultra Moon.
            // Future games do proper Time-Of-Day evolutions, so this is only a Gen 7 issue.
            case (int)Lycanroc when pk.Format == 7 && ((pk.Form == 0 && Moon()) || (pk.Form == 1 && Sun())):

            // Cosmog only evolves into:
            case (int)Solgaleo when Moon():
            case (int)Lunala when Sun():
                bool Sun()  => ((uint)pk.Version & 1) == 0;
                bool Moon() => ((uint)pk.Version & 1) == 1;
                if (pk.IsUntraded)
                    data.AddLine(GetInvalid(Evolution, EvoTradeRequired));
                break;
        }
    }
}

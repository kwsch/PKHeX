using System.Collections.Generic;
using PKHeX.Core;

namespace PKHeX.Application.UseCases;

/// <summary>
/// Independently re-verifies every candidate Pokémon a generator (e.g. the Living Dex engine, issue #123)
/// produced, following the same "never trust the engine's own success flag" pattern used by the
/// Auto-Legality Mod adapter (issue #89): a candidate is only accepted if it passes
/// <see cref="LegalityAnalysis"/> on its own. Candidates that fail are reported by species/form name
/// instead of being silently included or causing a crash. Pure Core-only logic, so it can be exercised
/// without invoking the vendored engine.
/// </summary>
public sealed class LivingDexVerificationUseCase
{
    public LivingDexVerificationResult Verify(IReadOnlyList<PKM> candidates)
    {
        var accepted = new List<PKM>(candidates.Count);
        var skipped = new List<string>();
        var strings = GameInfo.Strings;

        foreach (var pk in candidates)
        {
            bool valid;
            try
            {
                valid = new LegalityAnalysis(pk).Valid;
            }
            catch
            {
                valid = false;
            }

            if (valid)
                accepted.Add(pk);
            else
                skipped.Add(GetDisplayName(pk.Species, pk.Form, strings));
        }

        return new LivingDexVerificationResult(accepted, skipped);
    }

    /// <summary>Shared species/form display-name formatting, also used when generation itself fails a candidate.</summary>
    public static string GetDisplayName(ushort species, byte form, GameStrings strings)
    {
        var name = species < strings.Species.Count ? strings.Species[species] : $"species {species}";
        return form == 0 ? name : $"{name} (form {form})";
    }
}

/// <summary>Outcome of <see cref="LivingDexVerificationUseCase.Verify"/>.</summary>
/// <param name="Accepted">Candidates that independently passed <see cref="LegalityAnalysis"/>.</param>
/// <param name="SkippedSpeciesNames">Display names of candidates that failed re-verification.</param>
public sealed record LivingDexVerificationResult(IReadOnlyList<PKM> Accepted, IReadOnlyList<string> SkippedSpeciesNames);

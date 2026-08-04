using System;
using PKHeX.Application.Abstractions;
using PKHeX.Core;
using PKHeX.Core.AutoMod;

namespace PKHeX.Infrastructure.AutoLegality;

/// <summary>
/// Thin adapter over the vendored <c>PKHeX.AutoMod</c> legalization engine, implementing the
/// Application-layer <see cref="IAutoLegalityService"/> port. This is the single point where the
/// engine's static API is invoked; the rest of the app stays engine-agnostic.
/// </summary>
public sealed class AutoLegalityService : IAutoLegalityService
{
    static AutoLegalityService()
    {
        // The engine can fall back to a random "meme" species when it fails to legalize a set
        // (EnableEasterEggs). For a service that must report success/failure honestly, disable that so a
        // failure surfaces as a failure instead of silently returning an unrelated Pokémon.
        Legalizer.EnableEasterEggs = false;

        // Use the destination save's own trainer data so generated Pokémon match the loaded save
        // (OT/TID/SID/language), rather than the engine's built-in "ALM" default trainer.
        APILegality.UseTrainerData = true;
    }

    public AutoLegalityResult TryLegalizeShowdownSet(SaveFile sav, string? showdownText)
    {
        ArgumentNullException.ThrowIfNull(sav);

        if (string.IsNullOrWhiteSpace(showdownText))
            return AutoLegalityResult.Failure(LegalizationStatus.InvalidSet, null, "No Showdown set text was provided.");

        ShowdownSet set;
        try
        {
            set = new ShowdownSet(showdownText.Trim());
        }
        catch (Exception ex)
        {
            return AutoLegalityResult.Failure(LegalizationStatus.InvalidSet, null, $"Could not parse the Showdown set: {ex.Message}");
        }

        if (set.Species == 0)
            return AutoLegalityResult.Failure(LegalizationStatus.InvalidSet, null, "The Showdown set does not name a recognizable species.");

        if (set.InvalidLines.Count > 0)
        {
            var lines = string.Join(", ", set.InvalidLines);
            return AutoLegalityResult.Failure(LegalizationStatus.InvalidSet, null, $"The Showdown set has invalid lines: {lines}");
        }

        APILegality.AsyncLegalizationResult engineResult;
        try
        {
            // SaveFile implements ITrainerInfo; the engine builds a blank entity for the save's context,
            // so the result respects the loaded save's generation and format.
            engineResult = ((ITrainerInfo)sav).GetLegalFromSet(set);
        }
        catch (Exception ex)
        {
            return AutoLegalityResult.Failure(LegalizationStatus.Error, null, $"The legalization engine threw an error: {ex.Message}");
        }

        return engineResult.Status switch
        {
            LegalizationResult.Regenerated => Verify(engineResult.Created),
            LegalizationResult.Timeout => AutoLegalityResult.Failure(
                LegalizationStatus.Timeout, null,
                "The engine timed out before it could build a legal Pokémon for this set."),
            LegalizationResult.VersionMismatch => AutoLegalityResult.Failure(
                LegalizationStatus.VersionMismatch, null,
                "The vendored AutoMod engine reports a PKHeX.Core version mismatch."),
            _ => AutoLegalityResult.Failure(
                LegalizationStatus.Failed, null,
                "The engine could not find a legal encounter that produces this set."),
        };
    }

    /// <summary>
    /// Enforces the contract that a successful result MUST pass <see cref="LegalityAnalysis"/>. The engine
    /// reports <see cref="LegalizationResult.Regenerated"/> when it believes it succeeded; we independently
    /// verify and downgrade to a failure (carrying the report for diagnostics) if analysis disagrees.
    /// </summary>
    private static AutoLegalityResult Verify(PKM pk)
    {
        LegalityAnalysis la;
        try
        {
            la = new LegalityAnalysis(pk);
        }
        catch (Exception ex)
        {
            return AutoLegalityResult.Failure(LegalizationStatus.Error, null, $"Legality analysis threw an error: {ex.Message}");
        }

        var report = la.Report();
        if (la.Valid)
            return AutoLegalityResult.Ok(pk, report);

        return AutoLegalityResult.Failure(
            LegalizationStatus.Failed, report,
            "The engine produced a Pokémon, but it did not pass legality analysis.");
    }
}

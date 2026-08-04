using System;
using System.Collections.Generic;
using System.Threading;
using PKHeX.Core;

namespace PKHeX.Application.Abstractions;

/// <summary>
/// Application-layer port over the Living Dex generator (Auto-Legality Mod Phase 2, issue #123). Produces
/// one legal <see cref="PKM"/> per species obtainable in the loaded save's game. The concrete
/// implementation lives in the Infrastructure layer; it drives the loop itself over the vendored
/// <c>PKHeX.AutoMod</c> engine's per-set legalization path (the same one the "Auto Legality Mod" single-set
/// tool uses) rather than the engine's own <c>ModLogic.GenerateLivingDex</c>, which has a defect that makes
/// it return nothing (see the Infrastructure implementation for details). This interface keeps the
/// engine's types out of the Application and Presentation layers.
/// </summary>
public interface ILivingDexService
{
    /// <summary>
    /// Generates a living dex for the given save file.
    /// </summary>
    /// <param name="sav">Save file supplying trainer data, generation, and format context.</param>
    /// <param name="options">Generation options (include forms / shiny).</param>
    /// <param name="progress">Optional progress reporter, updated after each species is processed.</param>
    /// <param name="cancellationToken">
    /// Cooperative cancellation, checked before each species (and each of its forms) is processed. Since
    /// the per-species loop lives in this adapter (not inside the vendored engine), cancellation takes
    /// effect promptly rather than only between whole-dex passes.
    /// </param>
    /// <param name="maxSpeciesId">
    /// Test-only: caps the species ID range attempted (still subject to <paramref name="sav"/>'s own
    /// dex). Leave <see langword="null"/> for the full living dex — that is what the UI always requests.
    /// </param>
    /// <remarks>
    /// This call is CPU-bound and can run for tens of seconds to a few minutes for a full modern-gen dex
    /// (one legality attempt per species/form in the game). Callers on a UI thread should offload it (e.g.
    /// <c>Task.Run</c>). Never throws for a species that cannot be legalized; those are reported in
    /// <see cref="LivingDexGenerationResult.SkippedSpeciesNames"/> rather than included in the result.
    /// </remarks>
    LivingDexGenerationResult Generate(SaveFile sav, LivingDexOptions options, IProgress<LivingDexGenerationProgress>? progress = null, CancellationToken cancellationToken = default, int? maxSpeciesId = null);
}

/// <summary>Living Dex generation options exposed to the user.</summary>
/// <param name="IncludeForms">Generate every alternate form of a species, not just its base form.</param>
/// <param name="SetShiny">Request shiny specimens where the species/form is not shiny-locked.</param>
public readonly record struct LivingDexOptions(bool IncludeForms, bool SetShiny);

/// <summary>Progress notification for <see cref="ILivingDexService.Generate"/>: species processed so far out of the total.</summary>
public readonly record struct LivingDexGenerationProgress(int Completed, int Total);

/// <summary>
/// Result of an <see cref="ILivingDexService.Generate"/> call.
/// </summary>
public sealed class LivingDexGenerationResult
{
    private LivingDexGenerationResult(bool cancelled, IReadOnlyList<PKM> pokemon, IReadOnlyList<string> skippedSpeciesNames)
    {
        Cancelled = cancelled;
        Pokemon = pokemon;
        SkippedSpeciesNames = skippedSpeciesNames;
    }

    /// <summary>True if the caller's <see cref="CancellationToken"/> was honored (no changes were computed/applied).</summary>
    public bool Cancelled { get; }

    /// <summary>
    /// Every generated Pokémon in this list independently passes <see cref="LegalityAnalysis"/>; nothing
    /// here should ever fail a subsequent legality check.
    /// </summary>
    public IReadOnlyList<PKM> Pokemon { get; }

    /// <summary>
    /// Human-readable names of species/forms the engine attempted but could not legalize (or that failed
    /// independent re-verification). Reported rather than silently dropped.
    /// </summary>
    public IReadOnlyList<string> SkippedSpeciesNames { get; }

    public static LivingDexGenerationResult Ok(IReadOnlyList<PKM> pokemon, IReadOnlyList<string> skippedSpeciesNames) =>
        new(cancelled: false, pokemon, skippedSpeciesNames);

    public static LivingDexGenerationResult Cancel() => new(cancelled: true, [], []);
}

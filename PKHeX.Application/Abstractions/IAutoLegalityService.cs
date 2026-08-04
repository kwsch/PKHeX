using System.Collections.Generic;
using PKHeX.Core;

namespace PKHeX.Application.Abstractions;

/// <summary>
/// Application-layer port over the Auto-Legality (ALM) engine. Turns a Pokémon Showdown set into a
/// legal <see cref="PKM"/> for the loaded save's generation/format. The concrete implementation lives
/// in the Infrastructure layer and adapts the vendored <c>PKHeX.AutoMod</c> engine; this interface keeps
/// the engine's types out of the Application and Presentation layers.
/// </summary>
public interface IAutoLegalityService
{
    /// <summary>
    /// Attempts to generate a legal Pokémon from a Showdown set for the given save file.
    /// </summary>
    /// <param name="sav">Destination save file. Supplies trainer data, generation and format context.</param>
    /// <param name="showdownText">Raw Pokémon Showdown set text (single set).</param>
    /// <returns>
    /// A result describing success or failure. On success, <see cref="AutoLegalityResult.Pokemon"/> is a
    /// Pokémon that passes <see cref="LegalityAnalysis"/> for <paramref name="sav"/>.
    /// </returns>
    /// <remarks>
    /// This call is CPU-bound and may run for several seconds (the engine has an internal timeout).
    /// Callers on a UI thread should offload it (e.g. <c>Task.Run</c>). Never throws for bad input or a
    /// failed generation; failures are reported via the returned result.
    /// </remarks>
    AutoLegalityResult TryLegalizeShowdownSet(SaveFile sav, string? showdownText);
}

/// <summary>Outcome classification for a legalization attempt.</summary>
public enum LegalizationStatus
{
    /// <summary>A Pokémon was generated and it passes <see cref="LegalityAnalysis"/>.</summary>
    Success,

    /// <summary>The Showdown text was empty or could not be parsed into a species.</summary>
    InvalidSet,

    /// <summary>The engine could not find a legal encounter that satisfies the requested set.</summary>
    Failed,

    /// <summary>The engine exhausted its time budget before finding a legal result.</summary>
    Timeout,

    /// <summary>The engine reported a PKHeX.Core version mismatch (vendored engine out of sync with Core).</summary>
    VersionMismatch,

    /// <summary>An unexpected error occurred inside the engine.</summary>
    Error,
}

/// <summary>
/// Result of an <see cref="IAutoLegalityService.TryLegalizeShowdownSet"/> call.
/// </summary>
public sealed class AutoLegalityResult
{
    private AutoLegalityResult(LegalizationStatus status, PKM? pokemon, string? legalityReport, IReadOnlyList<string> messages)
    {
        Status = status;
        Pokemon = pokemon;
        LegalityReport = legalityReport;
        Messages = messages;
    }

    /// <summary>Whether a legal Pokémon was produced.</summary>
    public bool Success => Status == LegalizationStatus.Success;

    /// <summary>Outcome classification.</summary>
    public LegalizationStatus Status { get; }

    /// <summary>The generated Pokémon. Non-null only when <see cref="Success"/> is <see langword="true"/>.</summary>
    public PKM? Pokemon { get; }

    /// <summary>Full human-readable legality report for <see cref="Pokemon"/>, when available.</summary>
    public string? LegalityReport { get; }

    /// <summary>Human-readable messages (parse errors, failure reasons).</summary>
    public IReadOnlyList<string> Messages { get; }

    /// <summary>Convenience single-line summary of <see cref="Messages"/>.</summary>
    public string MessageText => Messages.Count == 0 ? string.Empty : string.Join("\n", Messages);

    public static AutoLegalityResult Ok(PKM pokemon, string legalityReport) =>
        new(LegalizationStatus.Success, pokemon, legalityReport, []);

    /// <summary>A failure. <paramref name="legalityReport"/> may carry diagnostics (e.g. why a generated result was still illegal); no Pokémon is returned.</summary>
    public static AutoLegalityResult Failure(LegalizationStatus status, string? legalityReport, params string[] messages) =>
        new(status, null, legalityReport, messages);
}

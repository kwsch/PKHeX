using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Provides functionality for managing and localizing external legality checkers.
/// </summary>
/// <remarks>
/// <see cref="LegalityAnalysis"/> will automatically use the registered external checkers after its legality checks are complete.
/// </remarks>
public static class ExternalLegalityCheck
{
    /// <summary>
    /// Gets the collection of external legality checkers indexed by their unique identifier.
    /// </summary>
    internal static Dictionary<ushort, IExternalLegalityChecker> ExternalCheckers { get; } = [];

    /// <summary>
    /// Registers an external legality checker for use in validation processes.
    /// </summary>
    /// <param name="checker">The legality checker to register. Must have a unique identity.</param>
    /// <exception cref="ArgumentException">Thrown if a legality checker with the same identity is already registered.</exception>
    public static void RegisterChecker(IExternalLegalityChecker checker)
    {
        if (!ExternalCheckers.TryAdd(checker.Identity, checker))
            throw new ArgumentException($"A legality checker with the argument {checker.Identity} is already registered.");
    }

    public static void UnregisterChecker(IExternalLegalityChecker checker)
    {
        if (!ExternalCheckers.Remove(checker.Identity))
            throw new ArgumentException($"A legality checker with the argument {checker.Identity} is not registered.");
    }

    /// <summary>
    /// Retrieves a localized string representation of the specified check result.
    /// </summary>
    /// <param name="chk">The check result to be localized, containing the argument to be processed.</param>
    /// <param name="localization">The localization settings that determine how the check result should be translated.</param>
    /// <param name="data">Legality analysis data that may be used for additional context in localization.</param>
    /// <returns>A localized string that represents the check result according to the specified settings.</returns>
    public static string Localize(CheckResult chk, LegalityLocalizationSet localization, LegalityAnalysis data)
    {
        if (ExternalCheckers.Count == 0)
            return localization.Lines.NotImplemented;

        if (!ExternalCheckers.TryGetValue(chk.Argument, out var checker))
            return localization.Lines.NotImplemented;

        return checker.Localize(chk, localization, data);
    }
}

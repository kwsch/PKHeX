using System;
using System.Collections.Generic;

namespace PKHeX.Core.Bulk;

public static class ExternalBulkCheck
{
    /// <summary>
    /// Gets the collection of external legality checkers indexed by their unique identifier.
    /// </summary>
    internal static Dictionary<ushort, IExternalBulkChecker> ExternalCheckers { get; } = [];
    /// <summary>
    /// Registers an external bulk legality checker for use in validation processes.
    /// </summary>
    /// <param name="checker">The bulk legality checker to register. Must have a unique identity.</param>
    /// <exception cref="ArgumentException">Thrown if a legality checker with the same identity is already registered.</exception>
    public static void RegisterChecker(IExternalBulkChecker checker)
    {
        if (!ExternalCheckers.TryAdd(checker.Identity, checker))
            throw new ArgumentException($"A bulk legality checker with the argument {checker.Identity} is already registered.");
    }
    public static void UnregisterChecker(IExternalBulkChecker checker)
    {
        if (!ExternalCheckers.Remove(checker.Identity))
            throw new ArgumentException($"A bulk legality checker with the argument {checker.Identity} is not registered.");
    }

    public static string Localize(CheckResult chk, LegalityLocalizationSet localization, IReadOnlyList<LegalityAnalysis> allAnalysis, int index1, int index2)
    {
        if (ExternalCheckers.Count == 0)
            return localization.Lines.NotImplemented;
        if (!ExternalCheckers.TryGetValue(chk.Argument, out var checker))
            return localization.Lines.NotImplemented;

        return (index1, index2) switch
        {

            (_, BulkCheckResult.NoIndex) => checker.Localize(chk, localization, allAnalysis[index1]),
            _ => checker.Localize(chk, localization, allAnalysis[index1], allAnalysis[index2]),
        };
    }
}

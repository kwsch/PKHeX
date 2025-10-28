using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

/// <summary>
/// String Translation Utility
/// </summary>
public class RibbonStrings
{
    private readonly Dictionary<string, string> RibbonNames = [];

    /// <summary>
    /// Resets the Ribbon Dictionary to use the supplied set of Ribbon (Property) Names.
    /// </summary>
    /// <param name="lines">Array of strings that are tab separated with Property Name, \t, and Display Name.</param>
    public RibbonStrings(ReadOnlySpan<string> lines)
    {
        RibbonNames.EnsureCapacity(lines.Length);

        // Don't clear existing keys on reset; only update.
        // A language will have the same keys (hopefully), only with differing values.
        foreach (var line in lines)
        {
            var index = line.IndexOf('\t');
            if (index < 0)
                continue;
            var text = line[(index + 1)..];
            var name = line[..index];
            RibbonNames[name] = text;
        }
    }

    /// <summary>
    /// Returns the Ribbon Display Name for the corresponding <see cref="PKM"/> ribbon property name.
    /// </summary>
    /// <param name="propertyName">Ribbon property name</param>
    /// <param name="result">Ribbon localized name</param>
    /// <returns>True if exists</returns>
    public bool GetNameSafe(string propertyName, [NotNullWhen(true)] out string? result) => RibbonNames.TryGetValue(propertyName, out result);

    /// <returns>Ribbon display name</returns>
    /// <inheritdoc cref="GetNameSafe"/>
    public string GetName(string propertyName)
    {
        // Throw an exception with the requested property name as the message, rather than an ambiguous "key not present" message.
        // We should ALWAYS have the key present as the input arguments are not user-defined, rather, they are from PKM property names.
        if (!GetNameSafe(propertyName, out var value))
            throw new KeyNotFoundException(propertyName);
        return value;
    }
}

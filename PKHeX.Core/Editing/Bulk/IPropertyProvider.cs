using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

/// <summary>
/// Interface for retrieving properties from a <see cref="PKM"/>.
/// </summary>
public interface IPropertyProvider
{
    /// <summary>
    /// Attempts to retrieve a property's value (as string) from a <see cref="PKM"/> instance.
    /// </summary>
    /// <param name="pk">Entity to retrieve the property from.</param>
    /// <param name="prop">Property name to retrieve.</param>
    /// <param name="result">Property value as string.</param>
    /// <returns><c>true</c> if the property was found and retrieved successfully; otherwise, <c>false</c>.</returns>
    bool TryGetProperty(PKM pk, string prop, [NotNullWhen(true)] out string? result);
}

public sealed class DefaultPropertyProvider : IPropertyProvider
{
    public static readonly DefaultPropertyProvider Instance = new();

    public bool TryGetProperty(PKM pk, string prop, [NotNullWhen(true)] out string? result)
    {
        result = null;
        if (!BatchEditing.TryGetHasProperty(pk, prop, out var pi))
            return false;
        try
        {
            var value = pi.GetValue(pk);
            result = value?.ToString();
            return result is not null;
        }
        catch
        {
            return false;
        }
    }
}

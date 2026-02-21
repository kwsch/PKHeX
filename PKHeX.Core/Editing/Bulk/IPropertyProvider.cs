using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

/// <summary>
/// Interface for retrieving properties from a <see cref="T"/>.
/// </summary>
public interface IPropertyProvider<in T> where T : notnull
{
    /// <summary>
    /// Attempts to retrieve a property's value (as string) from an entity instance.
    /// </summary>
    /// <param name="obj">Entity to retrieve the property from.</param>
    /// <param name="prop">Property name to retrieve.</param>
    /// <param name="result">Property value as string.</param>
    /// <returns><see langword="true"/> if the property was found and retrieved successfully; otherwise, <see langword="false"/>.</returns>
    bool TryGetProperty(T obj, string prop, [NotNullWhen(true)] out string? result);
}

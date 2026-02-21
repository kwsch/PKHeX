using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace PKHeX.Core;

/// <summary>
/// Provides batch editing helpers for an entity type.
/// </summary>
public interface IBatchEditor<TObject> where TObject : notnull
{
    /// <summary>
    /// Gets the list of supported entity types.
    /// </summary>
    IReadOnlyList<Type> Types { get; }

    /// <summary>
    /// Gets the property names, indexed by <see cref="Types"/>.
    /// </summary>
    string[][] Properties { get; }

    /// <summary>
    /// Tries to fetch the entity property from the cache of available properties.
    /// </summary>
    bool TryGetHasProperty(TObject entity, ReadOnlySpan<char> name, [NotNullWhen(true)] out PropertyInfo? pi);

    /// <summary>
    /// Tries to fetch the entity property from the cache of available properties.
    /// </summary>
    bool TryGetHasProperty(Type type, ReadOnlySpan<char> name, [NotNullWhen(true)] out PropertyInfo? pi);

    /// <summary>
    /// Gets a list of entity types that implement the requested property.
    /// </summary>
    IEnumerable<string> GetTypesImplementing(string property);

    /// <summary>
    /// Gets the type of the entity property using the saved cache of properties.
    /// </summary>
    bool TryGetPropertyType(string propertyName, [NotNullWhen(true)] out string? result, int typeIndex = 0);

    /// <summary>
    /// Checks if the entity is filtered by the provided filters.
    /// </summary>
    bool IsFilterMatch(IEnumerable<StringInstruction> filters, TObject entity);

    /// <summary>
    /// Tries to modify the entity.
    /// </summary>
    bool TryModifyIsSuccess(TObject entity, IEnumerable<StringInstruction> filters, IEnumerable<StringInstruction> modifications, Func<TObject, bool>? modifier = null);

    /// <summary>
    /// Tries to modify the entity using instructions and a custom modifier delegate.
    /// </summary>
    ModifyResult TryModify(TObject entity, IEnumerable<StringInstruction> filters, IEnumerable<StringInstruction> modifications, Func<TObject, bool>? modifier = null);
}

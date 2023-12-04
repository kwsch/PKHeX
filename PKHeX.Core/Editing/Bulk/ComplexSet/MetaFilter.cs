using System;

namespace PKHeX.Core;

/// <inheritdoc cref="IComplexFilter"/>
public sealed class MetaFilter(
    string Property,
    Func<object, StringInstruction, bool> FilterPKM)
    : IComplexFilterMeta
{
    public bool IsMatch(string prop) => prop == Property;
    public bool IsFiltered(object pk, StringInstruction value) => FilterPKM(pk, value);
}

using System;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

/// <inheritdoc cref="IComplexFilter"/>
public sealed class MetaFilter(
    [ConstantExpected] string Property,
    Func<object, StringInstruction, bool> FilterPKM)
    : IComplexFilterMeta
{
    public bool IsMatch(ReadOnlySpan<char> prop) => prop.SequenceEqual(Property);
    public bool IsFiltered(object pk, StringInstruction value) => FilterPKM(pk, value);
}

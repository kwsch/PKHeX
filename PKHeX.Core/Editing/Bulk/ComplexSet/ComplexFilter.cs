using System;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

/// <inheritdoc cref="IComplexFilter"/>
public sealed class ComplexFilter(
    [ConstantExpected] string Property,
    Func<PKM, StringInstruction, bool> FilterPKM,
    Func<BatchInfo, StringInstruction, bool> FilterBulk)
    : IComplexFilter
{
    public bool IsMatch(ReadOnlySpan<char> prop) => prop.SequenceEqual(Property);
    public bool IsFiltered(PKM pk, StringInstruction value) => FilterPKM(pk, value);
    public bool IsFiltered(BatchInfo info, StringInstruction value) => FilterBulk(info, value);
}

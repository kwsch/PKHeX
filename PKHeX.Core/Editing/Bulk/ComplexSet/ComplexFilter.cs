using System;

namespace PKHeX.Core;

/// <inheritdoc cref="IComplexFilter"/>
public sealed class ComplexFilter(
    string Property,
    Func<PKM, StringInstruction, bool> FilterPKM,
    Func<BatchInfo, StringInstruction, bool> FilterBulk)
    : IComplexFilter
{
    public bool IsMatch(string prop) => prop == Property;
    public bool IsFiltered(PKM pk, StringInstruction value) => FilterPKM(pk, value);
    public bool IsFiltered(BatchInfo info, StringInstruction value) => FilterBulk(info, value);
}

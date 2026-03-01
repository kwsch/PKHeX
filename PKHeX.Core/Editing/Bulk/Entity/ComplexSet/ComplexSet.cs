using System;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

/// <inheritdoc cref="IComplexSet"/>
public sealed class ComplexSet([ConstantExpected] string PropertyName, Action<PKM, StringInstruction> Action) : IComplexSet
{
    public readonly string PropertyName = PropertyName;
    public readonly Func<ReadOnlySpan<char>, bool> IsValueCompatible = _ => true;

    public ComplexSet([ConstantExpected] string PropertyName, Func<ReadOnlySpan<char>, bool> criteria, Action<PKM, StringInstruction> Action)
        : this(PropertyName, Action) => IsValueCompatible = criteria;

    public bool IsMatch(ReadOnlySpan<char> name, ReadOnlySpan<char> value)
        => name.SequenceEqual(PropertyName) && IsValueCompatible(value);

    public void Modify(PKM pk, StringInstruction instr) => Action(pk, instr);
}

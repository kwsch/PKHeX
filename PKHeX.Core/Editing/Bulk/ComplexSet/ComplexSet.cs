using System;

namespace PKHeX.Core;

/// <inheritdoc cref="IComplexSet"/>
public sealed class ComplexSet(string PropertyName, Action<PKM, StringInstruction> Action) : IComplexSet
{
    public readonly string PropertyName = PropertyName;
    public readonly Func<string, bool> IsValueCompatible = _ => true;

    public ComplexSet(string PropertyName, Func<string, bool> criteria, Action<PKM, StringInstruction> Action) : this(PropertyName, Action) => IsValueCompatible = criteria;

    public bool IsMatch(string name, string value) => name == PropertyName && IsValueCompatible(value);

    public void Modify(PKM pk, StringInstruction instr) => Action(pk, instr);
}

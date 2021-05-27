using System;

namespace PKHeX.Core
{
    /// <inheritdoc cref="IComplexSet"/>
    public class ComplexSet : IComplexSet
    {
        public readonly string PropertyName;
        public readonly Func<string, bool> IsValueCompatible = _ => true;
        private readonly Action<PKM, StringInstruction> Action;

        public ComplexSet(string propertyName, Action<PKM, StringInstruction> modify)
        {
            PropertyName = propertyName;
            Action = modify;
        }

        public ComplexSet(string propertyName, Func<string, bool> criteria, Action<PKM, StringInstruction> modify) : this(propertyName, modify) => IsValueCompatible = criteria;

        public bool IsMatch(string name, string value) => name == PropertyName && IsValueCompatible(value);

        public void Modify(PKM pk, StringInstruction instr) => Action(pk, instr);
    }
}

using System.Collections.Generic;

namespace PKHeX.Core;

public sealed record NamedEventWork(string Name, int Index, NamedEventType Type, IReadOnlyList<NamedEventConst> PredefinedValues) : NamedEventValue(Name, Index, Type);

public sealed record NamedEventConst(string Name, ushort Value)
{
    public bool IsCustom => Value == CustomMagicValue;
    public const ushort CustomMagicValue = ushort.MaxValue;
}

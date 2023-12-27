using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class EventVarGroup(EventVarType type)
{
    public readonly EventVarType Type = type;
    public readonly List<EventVar> Vars = [];
}

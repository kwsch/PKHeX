using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Event Flag that toggles certain features / entities on and off.
/// </summary>
public sealed class EventFlag(int index, EventVarType t, IReadOnlyList<string> pieces) : EventVar(index, t, pieces[1])
{
    public bool Flag;
}

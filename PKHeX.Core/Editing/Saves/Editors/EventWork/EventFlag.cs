using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Event Flag that toggles certain features / entities on and off.
    /// </summary>
    public sealed class EventFlag : EventVar
    {
        public bool Flag;

        public EventFlag(int index, EventVarType t, IReadOnlyList<string> pieces) : base(index, t, pieces[1])
        {
        }
    }
}
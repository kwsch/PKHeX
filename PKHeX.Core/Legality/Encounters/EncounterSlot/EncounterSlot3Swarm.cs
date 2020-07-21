using System.Collections.Generic;

namespace PKHeX.Core
{
    internal sealed class EncounterSlot3Swarm : EncounterSlot, IMoveset
    {
        public IReadOnlyList<int> Moves { get; }

        public EncounterSlot3Swarm(int[] moves) => Moves = moves;
    }
}
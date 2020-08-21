using System.Collections.Generic;

namespace PKHeX.Core
{
    internal sealed class EncounterSlot3Swarm : EncounterSlot3, IMoveset
    {
        public override int Generation => 3;
        public IReadOnlyList<int> Moves { get; }

        public EncounterSlot3Swarm(IReadOnlyList<int> moves) => Moves = moves;

        protected override void SetEncounterMoves(PKM pk, GameVersion version, int level)
        {
            var moves = Moves;
            pk.SetMoves(moves);
            pk.SetMaximumPPCurrent(moves);
        }
    }
}

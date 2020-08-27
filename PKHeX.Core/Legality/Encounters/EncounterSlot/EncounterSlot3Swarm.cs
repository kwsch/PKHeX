using System.Collections.Generic;

namespace PKHeX.Core
{
    internal sealed class EncounterSlot3Swarm : EncounterSlot3, IMoveset
    {
        public override int Generation => 3;
        public IReadOnlyList<int> Moves { get; }

        public EncounterSlot3Swarm(EncounterArea3 area, int species, int min, int max, int slot, GameVersion game,
            IReadOnlyList<int> moves) : base(area, species, 0, min, max, slot, 0, 0, 0, 0, game) => Moves = moves;

        protected override void SetEncounterMoves(PKM pk, GameVersion version, int level)
        {
            var moves = Moves;
            pk.SetMoves(moves);
            pk.SetMaximumPPCurrent(moves);
        }
    }
}

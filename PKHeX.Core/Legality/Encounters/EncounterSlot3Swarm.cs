namespace PKHeX.Core
{
    internal sealed class EncounterSlot3Swarm : EncounterSlot, IMoveset
    {
        public int[] Moves { get; }

        public EncounterSlot3Swarm(int[] moves) => Moves = moves;
    }
}
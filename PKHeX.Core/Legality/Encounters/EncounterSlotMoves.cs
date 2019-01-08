namespace PKHeX.Core
{
    internal sealed class EncounterSlotMoves : EncounterSlot, IMoveset
    {
        public int[] Moves { get; set; }
    }
}
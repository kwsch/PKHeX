namespace PKHeX.Core
{
    public class EncounterSlot3 : EncounterSlot, IMagnetStatic, INumberedSlot
    {
        public override int Generation => 3;

        public int StaticIndex { get; set; } = -1;
        public int MagnetPullIndex { get; set; } = -1;
        public int StaticCount { get; set; }
        public int MagnetPullCount { get; set; }

        public int SlotNumber { get; set; }
    }
}

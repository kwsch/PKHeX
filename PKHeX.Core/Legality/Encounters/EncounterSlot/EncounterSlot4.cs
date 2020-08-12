namespace PKHeX.Core
{
    public sealed class EncounterSlot4 : EncounterSlot, IMagnetStatic, INumberedSlot
    {
        public override int Generation => 4;
        public EncounterType TypeEncounter { get; set; } = EncounterType.None;

        public int StaticIndex { get; set; }
        public int MagnetPullIndex { get; set; }
        public int StaticCount { get; set; }
        public int MagnetPullCount { get; set; }

        public int SlotNumber { get; set; }

        protected override void SetFormatSpecificData(PKM pk) => ((PK4)pk).EncounterType = TypeEncounter.GetIndex();
    }
}

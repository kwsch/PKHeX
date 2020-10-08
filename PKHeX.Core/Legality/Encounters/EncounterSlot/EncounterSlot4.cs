namespace PKHeX.Core
{
    public sealed class EncounterSlot4 : EncounterSlot, IMagnetStatic, INumberedSlot, IEncounterTypeTile
    {
        public override int Generation => 4;
        public EncounterType TypeEncounter => ((EncounterArea4)Area).TypeEncounter;

        public int StaticIndex { get; set; }
        public int MagnetPullIndex { get; set; }
        public int StaticCount { get; set; }
        public int MagnetPullCount { get; set; }

        public int SlotNumber { get; set; }

        public EncounterSlot4(EncounterArea4 area, int species, int form, int min, int max, int slot, int mpi, int mpc, int sti, int stc) : base(area)
        {
            Species = species;
            Form = form;
            LevelMin = min;
            LevelMax = max;
            SlotNumber = slot;

            MagnetPullIndex = mpi;
            MagnetPullCount = mpc;

            StaticIndex = sti;
            StaticCount = stc;
        }

        protected override void SetFormatSpecificData(PKM pk) => ((PK4)pk).EncounterType = TypeEncounter.GetIndex();
    }
}

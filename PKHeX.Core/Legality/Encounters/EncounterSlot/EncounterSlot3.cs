namespace PKHeX.Core
{
    public class EncounterSlot3 : EncounterSlot, IMagnetStatic, INumberedSlot
    {
        public override int Generation => 3;

        public int StaticIndex { get; set; }
        public int MagnetPullIndex { get; set; }
        public int StaticCount { get; set; }
        public int MagnetPullCount { get; set; }

        public int SlotNumber { get; set; }

        public EncounterSlot3(EncounterArea3 area, int species, int form, int min, int max, int slot, int mpi, int mpc, int sti, int stc, GameVersion game) : base(area)
        {
            Species = species;
            Form = form;
            LevelMin = min;
            LevelMax = max;
            SlotNumber = slot;
            Version = game;

            MagnetPullIndex = mpi;
            MagnetPullCount = mpc;

            StaticIndex = sti;
            StaticCount = stc;
        }
    }
}

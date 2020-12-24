namespace PKHeX.Core
{
    /// <summary>
    /// Encounter Slot found in <see cref="GameVersion.Gen3"/>.
    /// </summary>
    /// <inheritdoc cref="EncounterSlot"/>
    public record EncounterSlot3 : EncounterSlot, IMagnetStatic, INumberedSlot
    {
        public sealed override int Generation => 3;

        public int StaticIndex { get; }
        public int MagnetPullIndex { get; }
        public int StaticCount { get; }
        public int MagnetPullCount { get; }

        public int SlotNumber { get; }

        public EncounterSlot3(EncounterArea3 area, int species, int form, int min, int max, int slot, int mpi, int mpc, int sti, int stc) : base(area)
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
    }
}

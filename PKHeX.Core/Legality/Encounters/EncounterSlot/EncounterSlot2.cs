namespace PKHeX.Core
{
    /// <summary>
    /// Generation 2 Wild Encounter Slot data
    /// </summary>
    /// <remarks>
    /// Contains Time data which is present in <see cref="GameVersion.C"/> origin data.
    /// </remarks>
    public sealed class EncounterSlot2 : EncounterSlot, INumberedSlot
    {
        public override int Generation => 2;
        public int SlotNumber { get; set; }

        public EncounterSlot2(EncounterArea2 area, int species, int min, int max, int slot, GameVersion game)
        {
            Area = area;
            Species = species;
            LevelMin = min;
            LevelMax = max;
            SlotNumber = slot;
            Version = game;
        }

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(sav, criteria, pk);

            var pk2 = (PK2)pk;
            if (Version == GameVersion.C)
                pk2.Met_TimeOfDay = ((EncounterArea2)Area!).Time.RandomValidTime();
        }
    }
}

namespace PKHeX.Core
{
    /// <summary>
    /// Encounter Slot found in <see cref="GameVersion.Gen2"/>.
    /// </summary>
    /// <remarks>
    /// Referenced Area object contains Time data which is used for <see cref="GameVersion.C"/> origin data.
    /// </remarks>
    /// <inheritdoc cref="EncounterSlot"/>
    public sealed record EncounterSlot2 : EncounterSlot, INumberedSlot
    {
        public override int Generation => 2;
        public int SlotNumber { get; }

        public EncounterSlot2(EncounterArea2 area, int species, int min, int max, int slot) : base(area, species, 0, min, max)
        {
            SlotNumber = slot;
        }

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(sav, criteria, pk);

            var pk2 = (PK2)pk;
            if (Version == GameVersion.C)
                pk2.Met_TimeOfDay = ((EncounterArea2)Area).Time.RandomValidTime();
        }
    }
}

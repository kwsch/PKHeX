namespace PKHeX.Core
{
    /// <summary>
    /// Encounter Slot found in <see cref="GameVersion.Gen1"/>.
    /// </summary>
    /// <inheritdoc cref="EncounterSlot"/>
    public sealed record EncounterSlot1 : EncounterSlot, INumberedSlot
    {
        public override int Generation => 1;
        public int SlotNumber { get; }

        public EncounterSlot1(EncounterArea1 area, int species, int min, int max, int slot) : base(area, species, 0, min, max)
        {
            SlotNumber = slot;
        }

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(sav, criteria, pk);

            var pk1 = (PK1)pk;
            if (Version == GameVersion.YW)
            {
                // Since we don't keep track of Yellow's Personal Data, just handle any differences here.
                pk1.Catch_Rate = Species switch
                {
                    (int) Core.Species.Kadabra => 96,
                    (int) Core.Species.Dragonair => 27,
                    _ => PersonalTable.RB[Species].CatchRate
                };
            }
            else
            {
                pk1.Catch_Rate = PersonalTable.RB[Species].CatchRate; // RB
            }
        }
    }
}

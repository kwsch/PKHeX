namespace PKHeX.Core
{
    /// <summary>
    /// Generation 1 Wild Encounter Slot data
    /// </summary>
    public sealed class EncounterSlot1 : EncounterSlot
    {
        public readonly int Rate;

        public EncounterSlot1(int species, int min, int max, int rate, SlotType type, int slot)
        {
            Species = species;
            LevelMin = min;
            LevelMax = max;
            Rate = rate;
            Type = type;
            SlotNumber = slot;
        }

        /// <summary>
        /// Deserializes Gen1 Encounter Slots from data.
        /// </summary>
        /// <param name="data">Byte array containing complete slot data table.</param>
        /// <param name="ofs">Offset to start reading from.</param>
        /// <param name="count">Amount of slots to read.</param>
        /// <param name="type">Type of encounter slot table.</param>
        /// <param name="rate">Slot type encounter rate.</param>
        /// <returns>Array of encounter slots.</returns>
        public static EncounterSlot1[] ReadSlots(byte[] data, ref int ofs, int count, SlotType type, int rate)
        {
            var bump = type == SlotType.Surf ? 4 : 0;
            var slots = new EncounterSlot1[count];
            for (int slot = 0; slot < count; slot++)
            {
                int min = data[ofs++];
                int species = data[ofs++];
                int max = min + bump;
                slots[slot] = new EncounterSlot1(species, min, max, rate, type, slot);
            }
            return slots;
        }

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(sav, criteria, pk);

            var pk1 = (PK1)pk;
            if (Version == GameVersion.YW)
            {
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

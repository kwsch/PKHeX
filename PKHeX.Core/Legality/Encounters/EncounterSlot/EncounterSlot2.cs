namespace PKHeX.Core
{
    /// <summary>
    /// Generation 2 Wild Encounter Slot data
    /// </summary>
    /// <remarks>
    /// Contains Time data which is present in <see cref="GameVersion.C"/> origin data.
    /// </remarks>
    public sealed class EncounterSlot2 : EncounterSlot
    {
        public int Rate;
        internal EncounterTime Time;

        public EncounterSlot2(int species, int min, int max, int rate, SlotType type, int slot)
        {
            Species = species;
            LevelMin = min;
            LevelMax = max;
            Rate = rate;
            Type = type;
            SlotNumber = slot;
        }

        /// <summary>
        /// Deserializes Gen2 Encounter Slots from data.
        /// </summary>
        /// <param name="data">Byte array containing complete slot data table.</param>
        /// <param name="ofs">Offset to start reading from.</param>
        /// <param name="count">Amount of slots to read.</param>
        /// <param name="type">Type of encounter slot table.</param>
        /// <param name="rate">Slot type encounter rate.</param>
        /// <returns>Array of encounter slots.</returns>
        public static EncounterSlot2[] ReadSlots(byte[] data, ref int ofs, int count, SlotType type, int rate)
        {
            var bump = type == SlotType.Surf ? 4 : 0;
            var slots = new EncounterSlot2[count];
            for (int slot = 0; slot < count; slot++)
            {
                int min = data[ofs++];
                int species = data[ofs++];
                int max = min + bump;
                slots[slot] = new EncounterSlot2(species, min, max, rate, type, slot);
            }
            return slots;
        }

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(sav, criteria, pk);

            var pk2 = (PK2)pk;
            if (Version == GameVersion.C)
                pk2.Met_TimeOfDay = Time.RandomValidTime();
        }
    }
}

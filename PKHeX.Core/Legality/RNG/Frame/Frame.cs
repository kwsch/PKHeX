namespace PKHeX.Core
{
    public class Frame
    {
        public readonly uint Seed;
        public readonly LeadRequired Lead;

        private readonly FrameType FrameType;
        private readonly RNG RNG;

        public uint RandLevel { get; set; }
        public uint RandESV { get; set; }
        public uint OriginSeed { get; set; }

        public bool LevelSlotModified => Lead > LeadRequired.SynchronizeFail;

        public Frame(uint seed, FrameType type, RNG rng, LeadRequired lead)
        {
            Seed = seed;
            Lead = lead;
            FrameType = type;
            RNG = rng;
        }

        /// <summary>
        /// Checks the Encounter Slot for RNG calls before the Nature loop.
        /// </summary>
        /// <param name="slot">Slot Data</param>
        /// <param name="pkm">Ancillary pkm data for determining how to check level.</param>
        /// <returns>Slot number for this frame & lead value.</returns>
        public bool IsSlotCompatibile(EncounterSlot slot, PKM pkm)
        {
            // Level is before Nature, but usually isn't varied. Check ESV calc first.
            int s = GetSlot(slot);
            if (s != slot.SlotNumber)
                return false;

            // Check Level Now
            int lvl = SlotRange.GetLevel(slot, Lead, RandLevel);
            if (pkm.HasOriginalMetLocation)
            {
                if (lvl != pkm.Met_Level)
                    return false;
            }
            else
            {
                if (lvl > pkm.Met_Level)
                    return false;
            }

            // Check if the slot is actually encounterable (considering Sweet Scent)
            bool encounterable = SlotRange.GetIsEncounterable(slot, FrameType, 0, Lead);
            return encounterable;
        }

        /// <summary>
        /// Gets the slot value for the input slot.
        /// </summary>
        /// <param name="slot">Slot Data</param>
        /// <returns>Slot number for this frame & lead value.</returns>
        private int GetSlot(EncounterSlot slot)
        {
            uint esv = RandESV;
            if (FrameType != FrameType.MethodH && !slot.FixedLevel)
                esv = RNG.Prev(RandLevel) >> 16;

            // Static and Magnet Pull do a slot search rather than slot mapping 0-99.
            return Lead != LeadRequired.StaticMagnet 
                ? SlotRange.GetSlot(slot.Type, esv, FrameType) 
                : SlotRange.GetSlotStaticMagnet(slot, esv);
        }

        /// <summary>
        /// Only use this for test methods.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public int GetSlot(SlotType t) => SlotRange.GetSlot(t, RandESV, FrameType);
    }
}

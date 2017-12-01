namespace PKHeX.Core
{
    public class Frame
    {
        public readonly uint Seed;
        public readonly LeadRequired Lead;

        private readonly FrameType FrameType;
        private readonly RNG RNG;

        public uint RandLevel { get; set; }
        public uint ESV { get; set; }

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
            // Static and Magnet Pull do a slot search rather than slot mapping 0-99.
            return Lead != LeadRequired.StaticMagnet 
                ? GetSlot(slot.Type) 
                : GetSlotStaticMagnet(slot);
        }

        /// <summary>
        /// Checks both Static and Magnet Pull ability type selection encounters to see if the encounter can be selected.
        /// </summary>
        /// <param name="slot">Slot Data</param>
        /// <returns>Slot number from the slot data if the slot is selected on this frame, else an invalid slot value.</returns>
        private int GetSlotStaticMagnet(EncounterSlot slot)
        {
            if (slot.Permissions.StaticIndex >= 0)
            {
                var index = ESV % slot.Permissions.StaticCount;
                if (index == slot.Permissions.StaticIndex)
                    return slot.SlotNumber;
            }
            if (slot.Permissions.MagnetPullIndex >= 0)
            {
                var index = ESV % slot.Permissions.MagnetPullCount;
                if (index == slot.Permissions.MagnetPullIndex)
                    return slot.SlotNumber;
            }
            return -1;
        }

        /// <summary>
        /// Only use this for test methods.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public int GetSlot(SlotType t) => SlotRange.GetSlot(t, ESV, FrameType, Seed);
    }
}

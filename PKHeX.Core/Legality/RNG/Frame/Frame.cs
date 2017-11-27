namespace PKHeX.Core
{
    public class Frame
    {
        public readonly uint Seed;
        public readonly LeadRequired Lead;

        private readonly FrameType FrameType;
        private readonly RNG RNG;

        public uint ESV { get; set; }
        public void SetOriginSeed(int Offset) => OriginSeed = RNG.Reverse(Seed, Offset);
        public bool LevelSlotModified => Lead > LeadRequired.SynchronizeFail;

        public uint OriginSeed;

        public Frame(uint seed, FrameType type, RNG rng, LeadRequired lead)
        {
            Seed = seed;
            Lead = lead;
            FrameType = type;
            RNG = rng;
        }

        public int EncounterSlot(SlotType t, EncounterSlot slot)
        {
            if (Lead == LeadRequired.StaticMagnet)
            {
                if (slot.Permissions.MagnetPullIndex >= 0)
                {
                    var index = ESV % slot.Permissions.MagnetPullCount;
                    return index == slot.Permissions.MagnetPullIndex ? slot.SlotNumber : -1;
                }

                if (slot.Permissions.StaticIndex >= 0)
                {
                    var index = ESV % slot.Permissions.StaticCount;
                    return index == slot.Permissions.StaticIndex ? slot.SlotNumber : -1;
                }
                return -1;
            }
            return EncounterSlot(t);
        }
        /// <summary>
        /// Only use this for test methods.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public int EncounterSlot(SlotType t) => SlotRange.GetSlot(t, ESV, FrameType, Seed);
    }
}

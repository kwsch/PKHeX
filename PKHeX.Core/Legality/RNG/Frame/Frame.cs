namespace PKHeX.Core
{
    public class Frame
    {
        public readonly uint Seed;
        public readonly LeadRequired Lead;

        private readonly FrameType FrameType;
        private readonly RNG RNG;

        public uint ESV { get; set; }
        public int EncounterSlot(SlotType t) => SlotRange.GetSlot(t, ESV, FrameType);
        public void setOriginSeed(int Offset) => OriginSeed = RNG.Reverse(Seed, Offset);
        public bool LevelSlotModified => Lead > LeadRequired.SynchronizeFail;

        public uint OriginSeed;

        public Frame(uint seed, FrameType type, RNG rng, LeadRequired lead)
        {
            Seed = seed;
            Lead = lead;
            FrameType = type;
            RNG = rng;
        }
    }
}

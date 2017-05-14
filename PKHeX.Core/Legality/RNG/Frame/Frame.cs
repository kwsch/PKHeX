namespace PKHeX.Core
{
    public class Frame
    {
        public readonly FrameType FrameType;

        public uint Seed { get; set; }
        public readonly LeadRequired Lead;
        public bool CuteCharm { get; set; }

        public uint ESV { get; set; }
        public bool SuctionCups { get; set; }

        public Frame(uint seed, FrameType type, LeadRequired lead)
        {
            Seed = seed;
            Lead = lead;
            FrameType = type;
        }
    }
}

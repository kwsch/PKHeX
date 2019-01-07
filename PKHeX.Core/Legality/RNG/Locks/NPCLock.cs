namespace PKHeX.Core
{
    /// <summary>
    /// Locks associated to a given NPC PKM that appears before a <see cref="EncounterStaticShadow"/>.
    /// </summary>
    public sealed class NPCLock
    {
        public readonly short Species;
        public readonly byte Nature;
        public readonly byte Gender;
        public readonly byte Ratio;
        public readonly bool Shadow;
        public readonly bool Seen;

        public NPCLock(short s, byte n, byte g, byte r)
        {
            Species = s;
            Nature = n;
            Gender = g;
            Ratio = r;
        }

        public NPCLock(short s, bool seen = false)
        {
            Species = s;
            Nature = 25;
            Shadow = true;
            Seen = seen;
        }

        public bool MatchesLock(uint PID)
        {
            if (Shadow)
                return true;
            if (Gender != 2 && Gender != ((PID & 0xFF) < Ratio ? 1 : 0))
                return false;
            if (Nature != PID % 25)
                return false;
            return true;
        }

        internal NPCLock Clone() => (NPCLock)MemberwiseClone();
    }
}
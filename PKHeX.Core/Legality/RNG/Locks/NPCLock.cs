namespace PKHeX.Core
{
    /// <summary>
    /// Locks associated to a given NPC PKM that appears before a <see cref="EncounterStaticShadow"/>.
    /// </summary>
    public sealed class NPCLock
    {
        public readonly int Species;
        public readonly uint Nature;
        public readonly uint Gender;
        public readonly uint Ratio;
        public readonly bool Shadow;
        public readonly bool Seen;

        public NPCLock(int s, uint n, uint g, uint r)
        {
            Species = s;
            Nature = n;
            Gender = g;
            Ratio = r;
        }

        public NPCLock(int s, bool seen = false)
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
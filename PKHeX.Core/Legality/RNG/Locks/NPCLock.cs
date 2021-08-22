namespace PKHeX.Core
{
    /// <summary>
    /// Locks associated to a given NPC PKM that appears before a <see cref="EncounterStaticShadow"/>.
    /// </summary>
    public readonly struct NPCLock
    {
        public readonly short Species;
        public readonly byte Nature;
        public readonly byte Gender;
        public readonly byte Ratio;
        public readonly bool Shadow;
        public readonly bool Seen;

        public int FramesConsumed => Seen ? 5 : 7;

        // Not-Shadow
        public NPCLock(short s, byte n, byte g, byte r)
        {
            Species = s;
            Nature = n;
            Gender = g;
            Ratio = r;
            Shadow = false;
            Seen = false;
        }

        // Shadow
        public NPCLock(short s, bool seen = false)
        {
            Species = s;
            Nature = 0;
            Gender = 0;
            Ratio = 0;
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

        public override bool Equals(object obj) => false;
        public override int GetHashCode() => 0;
        public static bool operator ==(NPCLock left, NPCLock right) => left.Equals(right);
        public static bool operator !=(NPCLock left, NPCLock right) => !(left == right);

#if DEBUG
        public override string ToString()
        {
            var sb = new System.Text.StringBuilder(64);
            sb.Append((Species)Species);
            if (Shadow)
                sb.Append(" (Shadow)");
            if (Seen)
                sb.Append(" [Seen]");
            sb.Append(" - ");
            sb.Append("Nature: ").Append((Nature)Nature);
            if (Gender != 2)
                sb.Append(", ").Append("Gender: ").Append(Gender);
            return sb.ToString();
        }
#endif
    }
}

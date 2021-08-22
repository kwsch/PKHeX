namespace PKHeX.Core
{
    /// <summary>
    /// Stores details about a <see cref="PKM.EncryptionConstant"/> (PID) and any associated details being traced to a known correlation.
    /// </summary>
    public readonly struct PIDIV
    {
        internal static readonly PIDIV None = new();
        internal static readonly PIDIV CuteCharm = new(PIDType.CuteCharm);
        internal static readonly PIDIV Pokewalker = new(PIDType.Pokewalker);
        internal static readonly PIDIV G5MGShiny = new(PIDType.G5MGShiny);

        /// <summary> The RNG seed which immediately generates the PIDIV (starting with PID or IVs, whichever comes first). </summary>
        public readonly uint OriginSeed;

        /// <summary> Indicates that there is no <see cref="OriginSeed"/> to refer to. </summary>
        /// <remarks> Some PIDIVs may be generated without a single seed, but may follow a traceable pattern. </remarks>
        public bool NoSeed => Type is PIDType.None or PIDType.CuteCharm or PIDType.Pokewalker or PIDType.G5MGShiny;

        /// <summary> Type of PIDIV correlation </summary>
        public readonly PIDType Type;

        private PIDIV(PIDType type)
        {
            OriginSeed = 0;
            Type = type;
        }

        public PIDIV(PIDType type, uint seed)
        {
            OriginSeed = seed;
            Type = type;
        }

        public bool Equals(PIDIV pid) => pid.Type == Type && pid.OriginSeed == OriginSeed;
        public override bool Equals(object pid) => pid is PIDIV p && Equals(p);
        public override int GetHashCode() => 0;
        public static bool operator ==(PIDIV left, PIDIV right) => left.Equals(right);
        public static bool operator !=(PIDIV left, PIDIV right) => !(left == right);

#if DEBUG
        public override string ToString() => NoSeed ? Type.ToString() : $"{Type} - 0x{OriginSeed:X8}";
#endif
    }
}

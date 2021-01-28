namespace PKHeX.Core
{
    public sealed record PIDIV
    {
        internal static readonly PIDIV None = new();
        internal static readonly PIDIV CuteCharm = new(PIDType.CuteCharm);
        internal static readonly PIDIV Pokewalker = new(PIDType.Pokewalker);
        internal static readonly PIDIV G5MGShiny = new(PIDType.G5MGShiny);

        /// <summary> The RNG seed which immediately generates the PIDIV (starting with PID or IVs, whichever comes first). </summary>
        public readonly uint OriginSeed;

        /// <summary> Indicates that there is no <see cref="OriginSeed"/> to refer to. </summary>
        /// <remarks> Some PIDIVs may be generated without a single seed, but may follow a traceable pattern. </remarks>
        public readonly bool NoSeed;

        /// <summary> Type of PIDIV correlation </summary>
        public readonly PIDType Type;

        private PIDIV(PIDType type = PIDType.None)
        {
            NoSeed = true;
            Type = type;
        }

        public PIDIV(PIDType type, uint seed)
        {
            OriginSeed = seed;
            Type = type;
        }
    }
}

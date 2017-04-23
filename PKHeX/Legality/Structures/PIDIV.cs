namespace PKHeX.Core
{
    public class PIDIV
    {
        /// <summary> The RNG that generated the PKM from the <see cref="OriginSeed"/> </summary>
        public RNG RNG;

        /// <summary> The RNG seed which immediately generates the PIDIV (starting with PID or IVs, whichever comes first). </summary>
        public uint OriginSeed;
    }
}

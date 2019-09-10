using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Shadow Pokémon Encounter found in <see cref="GameVersion.CXD"/>
    /// </summary>
    public sealed class EncounterStaticShadow : EncounterStatic
    {
        /// <summary>
        /// Team Specification with required <see cref="Species"/>, <see cref="Nature"/> and Gender.
        /// </summary>
        public TeamLock[] Locks { get; internal set; } = Array.Empty<TeamLock>();

        /// <summary>
        /// Initial Shadow Gauge value.
        /// </summary>
        public int Gauge { get; internal set; }

        /// <summary>
        /// Originates from the EReader scans (Japanese Only)
        /// </summary>
        public bool EReader { get; set; }

        internal override EncounterStatic Clone()
        {
            var result = (EncounterStaticShadow)base.Clone();

            if (Locks.Length == 0)
                return result;

            result.Locks = new TeamLock[Locks.Length];
            for (int i = 0; i < Locks.Length; i++)
                result.Locks[i] = Locks[i].Clone();
            return result;
        }
    }
}
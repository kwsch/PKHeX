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
        public readonly TeamLock[] Locks;

        /// <summary>
        /// Initial Shadow Gauge value.
        /// </summary>
        public int Gauge { get; internal set; }

        /// <summary>
        /// Originates from the EReader scans (Japanese Only)
        /// </summary>
        public bool EReader { get; internal set; }

        public EncounterStaticShadow(TeamLock[] locks) => Locks = locks;
        public EncounterStaticShadow() => Locks = Array.Empty<TeamLock>();
    }
}
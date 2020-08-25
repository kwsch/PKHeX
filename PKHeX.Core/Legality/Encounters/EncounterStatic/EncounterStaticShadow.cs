using System;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Shadow Pokémon Encounter found in <see cref="GameVersion.CXD"/>
    /// </summary>
    public sealed class EncounterStaticShadow : EncounterStatic3
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

        private static readonly int[] MirorBXDLocations =
        {
            090, // Rock
            091, // Oasis
            092, // Cave
            113, // Pyrite Town
            059, // Realgam Tower
        };

        protected override bool IsMatchLocation(PKM pkm)
        {
            if (pkm.Format != 3)
                return true; // transfer location verified later

            var met = pkm.Met_Location;
            if (Version == GameVersion.XD)
            {
                if (met == Location)
                    return true;
                return MirorBXDLocations.Contains(met);
            }

            return met == Location;
        }
    }
}

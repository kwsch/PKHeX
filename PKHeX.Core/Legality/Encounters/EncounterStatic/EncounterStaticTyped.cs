using System;
using System.Linq;

namespace PKHeX.Core
{
    public sealed class EncounterStaticTyped : EncounterStatic4
    {
        public bool Roaming { get; set; }

        /// <summary>
        /// <see cref="PK4.EncounterType"/> values permitted for the encounter.
        /// </summary>
        public EncounterType TypeEncounter { get; internal set; } = EncounterType.None;

        protected override bool IsMatchLocation(PKM pkm)
        {
            if (!Roaming)
                return base.IsMatchLocation(pkm);

            if (!(pkm is G4PKM pk4))
                return true;

            var locs = GetRoamLocations(Species, pk4.EncounterType);
            return locs.Contains(pkm.Met_Location);
        }

        protected override void SetMetData(PKM pk, int level, DateTime today)
        {
            var pk4 = (PK4)pk;
            var type = pk4.EncounterType = TypeEncounter.GetIndex();
            pk.Met_Location = Roaming ? GetRoamLocations(Species, type)[0] : Location;
            pk.Met_Level = level;
            pk.MetDate = today;
        }

        private int[] GetRoamLocations(int species, int type)
        {
            switch (species)
            {
                case 481: case 488: case 144: case 145: case 146:
                    return 1 << type == (int)EncounterType.TallGrass
                        ? Roaming_MetLocation_DPPt_Grass
                        : Roaming_MetLocation_DPPt_Surf;
                case 243: case 244:
                    return 1 << type == (int)EncounterType.TallGrass
                        ? Roaming_MetLocation_HGSS_Johto_Grass
                        : Roaming_MetLocation_HGSS_Johto_Surf;
                case 380: case 381:
                    return 1 << type == (int)EncounterType.TallGrass
                        ? Roaming_MetLocation_HGSS_Kanto_Grass
                        : Roaming_MetLocation_HGSS_Kanto_Surf;
                default: throw new IndexOutOfRangeException(nameof(species));
            }
        }

        private static readonly int[] Roaming_MetLocation_DPPt_Grass =
        {
            // Routes 201-218, 221-222 can be encountered in grass
            16, 17, 18, 19, 20, 21, 22, 23, 24, 25,
            26, 27, 28, 29, 30, 31, 32, 33, 36, 37,
            47,     // Valley Windworks
            49,     // Fuego Ironworks
        };

        private static readonly int[] Roaming_MetLocation_DPPt_Surf =
        {
            // Routes 203-205, 208-210, 212-214, 218-222 can be encountered in water
            18, 19, 20, 23, 24, 25, 27, 28, 29, 33,
            34, 35, 36, 37,
            47,     // Valley Windworks
            49,     // Fuego Ironworks
        };

        // Grass 29-39, 42-46, 47, 48
        // Surf 30-32 34-35, 40-45, 47
        // Route 45 inaccessible surf
        private static readonly int[] Roaming_MetLocation_HGSS_Johto_Grass =
        {
            // Routes 29-48 can be encountered in grass
            // Won't go to routes 40,41,47,48
            177, 178, 179, 180, 181, 182, 183, 184, 185, 186,
            187,                     190, 191, 192, 193, 194,
        };

        private static readonly int[] Roaming_MetLocation_HGSS_Johto_Surf =
        {
            // Routes 30-32,34-35,40-45 and 47 can be encountered in water
            // Won't go to routes 40,41,47,48
            178, 179, 180, 182, 183, 190, 191, 192, 193
        };

        private static readonly int[] Roaming_MetLocation_HGSS_Kanto_Grass =
        {
            // Route 01-18,21,22,24,26 and 28 can be encountered in grass
            // Won't go to route 23 25 27
            149, 150, 151, 152, 153, 154, 155, 156, 157, 158,
            159, 160, 161, 162, 163, 164, 165, 166,
            169, 170,      172,      174,      176,
        };

        private static readonly int[] Roaming_MetLocation_HGSS_Kanto_Surf =
        {
            // Route 4,6,9,10,12,13,19-22,24,26 and 28 can be encountered in water
            // Won't go to route 23 25 27
            152, 154, 157, 158, 160, 161, 167, 168, 169, 170,
            172,      174,      176,
        };
    }
}
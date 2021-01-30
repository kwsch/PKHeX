using System;
using System.Linq;
using static PKHeX.Core.EncounterType;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 4 Static Encounter
    /// </summary>
    /// <inheritdoc cref="EncounterStatic"/>
    public sealed record EncounterStatic4 : EncounterStatic, IEncounterTypeTile
    {
        public override int Generation => 4;

        /// <summary> Indicates if the encounter is a Roamer (variable met location) </summary>
        public bool Roaming { get; init; }

        /// <summary> <see cref="PK4.EncounterType"/> values permitted for the encounter. </summary>
        public EncounterType TypeEncounter { get; init; } = None;

        public EncounterStatic4(GameVersion game) : base(game) { }

        protected override bool IsMatchLocation(PKM pkm)
        {
            if (!Roaming)
                return base.IsMatchLocation(pkm);

            // Met location is lost on transfer
            if (pkm is not G4PKM pk4)
                return true;

            var locs = GetRoamLocations(Species, pk4.EncounterType);
            return locs.Contains(pkm.Met_Location);
        }

        protected override bool IsMatchEggLocation(PKM pkm)
        {
            if (pkm.Egg_Location == EggLocation)
            {
                if (EggLocation == 0)
                    return true;

                // Check the inverse scenario for 4->5 eggs
                if (!Locations.IsPtHGSSLocationEgg(EggLocation))
                    return true;
                return pkm.Format == 4;
            }

            if (pkm.IsEgg) // unhatched
            {
                if (EggLocation != pkm.Met_Location)
                    return false;
                return pkm.Egg_Location == 0;
            }

            // Only way to mismatch is to be a Link Traded egg, or traded to Pt/HG/SS and hatched there.
            if (pkm.Egg_Location == Locations.LinkTrade4)
                return true;

            // check Pt/HGSS data
            if (pkm.Format == 4)
                return false;

            if (!Locations.IsPtHGSSLocationEgg(EggLocation)) // non-Pt/HG/SS egg gift
                return false;

            // transferring 4->5 clears Pt/HG/SS location value and keeps Faraway Place
            return pkm.Egg_Location == Locations.Faraway4;
        }

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(sav, criteria, pk);
            SanityCheckVersion(pk);
        }

        private void SanityCheckVersion(PKM pk)
        {
            // Unavailable encounters in DP, morph them to Pt so they're legal.
            switch (Species)
            {
                case (int)Core.Species.Darkrai when Location == 079: // DP Darkrai
                case (int)Core.Species.Shaymin when Location == 063: // DP Shaymin
                    pk.Version = (int)GameVersion.Pt;
                    return;
            }
        }

        protected override bool IsMatchLevel(PKM pkm, DexLevel evo)
        {
            if (pkm.Format != 4) // Met Level lost on PK3=>PK4
                return Level <= evo.Level;

            return pkm.Met_Level == (EggEncounter ? 0 : Level);
        }

        protected override bool IsMatchPartial(PKM pkm)
        {
            if (Gift && pkm.Ball != Ball)
                return true;
            return base.IsMatchPartial(pkm);
        }

        protected override void SetMetData(PKM pk, int level, DateTime today)
        {
            var pk4 = (PK4)pk;
            var type = pk4.EncounterType = TypeEncounter.GetIndex();
            pk.Met_Location = Roaming ? GetRoamLocations(Species, type)[0] : Location;
            pk.Met_Level = level;
            pk.MetDate = today;
        }

        private static int[] GetRoamLocations(int species, int type) => species switch
        {
            481 or 488 or 144 or 145 or 146 => 1 << type == (int)TallGrass ? Roaming_MetLocation_DPPt_Grass : Roaming_MetLocation_DPPt_Surf,
            243 or 244 => 1 << type == (int)TallGrass ? Roaming_MetLocation_HGSS_Johto_Grass : Roaming_MetLocation_HGSS_Johto_Surf,
            380 or 381 => 1 << type == (int)TallGrass ? Roaming_MetLocation_HGSS_Kanto_Grass : Roaming_MetLocation_HGSS_Kanto_Surf,
            _ => throw new IndexOutOfRangeException(nameof(species)),
        };

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
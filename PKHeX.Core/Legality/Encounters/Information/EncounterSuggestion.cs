using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic for providing suggested property values with respect to the input data.
    /// </summary>
    public static class EncounterSuggestion
    {
        public static EncounterStatic? GetSuggestedMetInfo(PKM pkm)
        {
            int loc = GetSuggestedTransferLocation(pkm);

            if (pkm.WasEgg)
                return GetSuggestedEncounterEgg(pkm, loc);

            var w = EncounterSlotGenerator.GetCaptureLocation(pkm);
            if (w != null)
                return GetSuggestedEncounterWild(w, loc);

            var s = EncounterStaticGenerator.GetStaticLocation(pkm);
            if (s != null)
                return GetSuggestedEncounterStatic(s, loc);

            return null;
        }

        private static EncounterStatic GetSuggestedEncounterEgg(PKM pkm, int loc = -1)
        {
            int lvl = GetSuggestedEncounterEggMetLevel(pkm);
            return new EncounterStatic
            {
                Species = Legal.GetBaseSpecies(pkm),
                Location = loc != -1 ? loc : GetSuggestedEggMetLocation(pkm),
                Level = lvl,
            };
        }

        public static int GetSuggestedEncounterEggMetLevel(PKM pkm)
        {
            if (!pkm.IsNative && pkm.GenNumber < 5)
                return pkm.CurrentLevel; // be generous with transfer conditions
            if (pkm.Format < 5) // and native
                return pkm.Format == 2 && pkm.Met_Location != 0 ? 1 : 0;
            return 1; // gen5+
        }

        public static int GetSuggestedEncounterEggLocationEgg(PKM pkm, bool traded = false)
        {
            switch (pkm.GenNumber)
            {
                case 1:
                case 2:
                case 3:
                    return 0;
                case 4:
                    return traded ? Locations.LinkTrade4 : Locations.Daycare4;
                case 5:
                    return traded ? Locations.LinkTrade5 : Locations.Daycare5;
                default:
                    return traded ? Locations.LinkTrade6 : Locations.Daycare5;
            }
        }

        private static EncounterStatic GetSuggestedEncounterWild(EncounterArea area, int loc = -1)
        {
            var slots = area.Slots.OrderBy(s => s.LevelMin);
            var first = slots.First();
            return new EncounterStatic
            {
                Location = loc != -1 ? loc : area.Location,
                Species = first.Species,
                Level = first.LevelMin,
            };
        }

        private static EncounterStatic GetSuggestedEncounterStatic(EncounterStatic s, int loc = -1)
        {
            if (loc == -1)
                loc = s.Location;

            // don't leak out the original EncounterStatic object
            var encounter = s.Clone(loc);
            return encounter;
        }

        /// <summary>
        /// Gets a valid Egg hatch location for the origin game.
        /// </summary>
        /// <param name="pkm">Pokémon data to suggest for</param>
        public static int GetSuggestedEggMetLocation(PKM pkm)
        {
            // Return one of legal hatch locations for game
            switch ((GameVersion)pkm.Version)
            {
                case GameVersion.R:
                case GameVersion.S:
                case GameVersion.E:
                case GameVersion.FR:
                case GameVersion.LG:
                    return pkm.Format switch
                    {
                        3 => (pkm.FRLG ? Locations.HatchLocationFRLG : Locations.HatchLocationRSE),
                        4 => Locations.Transfer3, // Pal Park
                        _ => Locations.Transfer4,
                    };

                case GameVersion.D:
                case GameVersion.P:
                case GameVersion.Pt:
                    return pkm.Format > 4 ? Locations.Transfer4 /* Transporter */ : Locations.HatchLocationDPPt;
                case GameVersion.HG:
                case GameVersion.SS:
                    return pkm.Format > 4 ? Locations.Transfer4 /* Transporter */ : Locations.HatchLocationHGSS;

                case GameVersion.B:
                case GameVersion.W:
                case GameVersion.B2:
                case GameVersion.W2:
                    return Locations.HatchLocation5;

                case GameVersion.X:
                case GameVersion.Y:
                    return Locations.HatchLocation6XY;
                case GameVersion.AS:
                case GameVersion.OR:
                    return Locations.HatchLocation6AO;

                case GameVersion.SN:
                case GameVersion.MN:
                case GameVersion.US:
                case GameVersion.UM:
                    return Locations.HatchLocation7;

                case GameVersion.SW:
                case GameVersion.SH:
                    return Locations.HatchLocation8;

                case GameVersion.GD:
                case GameVersion.SV:
                case GameVersion.C:
                case GameVersion.GSC:
                case GameVersion.RBY:
                    return pkm.Format > 2 ? Legal.Transfer2 : pkm.Met_Level == 0 ? 0 : Locations.HatchLocationC;
            }
            return -1;
        }

        /// <summary>
        /// Gets the correct Transfer Met location for the origin game.
        /// </summary>
        /// <param name="pkm">Pokémon data to suggest for</param>
        /// <remarks>
        /// Returns -1 if the met location is not overriden with a transfer location
        /// </remarks>
        public static int GetSuggestedTransferLocation(PKM pkm)
        {
            if (pkm.HasOriginalMetLocation)
                return -1;
            if (pkm.Version == (int) GameVersion.GO)
                return 30012;
            if (pkm.VC1)
                return Legal.Transfer1;
            if (pkm.VC2)
                return Legal.Transfer2;
            if (pkm.Format == 4) // Pal Park
                return Locations.Transfer3;

            if (pkm.GenNumber >= 5)
                return -1;

            if (pkm.Format >= 5) // Transporter
                return Legal.GetTransfer45MetLocation(pkm);
            return -1;
        }
    }
}

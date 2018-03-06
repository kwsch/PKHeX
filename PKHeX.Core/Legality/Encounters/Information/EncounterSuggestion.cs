using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic for providing suggested property values with respect to the input data.
    /// </summary>
    internal static class EncounterSuggestion
    {
        public static EncounterStatic GetSuggestedMetInfo(PKM pkm)
        {
            if (pkm == null)
                return null;

            int loc = GetSuggestedTransferLocation(pkm);

            if (pkm.WasEgg)
                return GetSuggestedEncounterEgg(pkm, loc);

            var w = EncounterGenerator.GetCaptureLocation(pkm);
            if (w != null)
                return GetSuggestedEncounterWild(w, loc);

            var s = EncounterGenerator.GetStaticLocation(pkm);
            if (s != null)
                return GetSuggestedEncounterStatic(s, loc);

            return null;
        }
        private static EncounterStatic GetSuggestedEncounterEgg(PKM pkm, int loc = -1)
        {
            int lvl = 1; // gen5+
            if (!pkm.IsNative && pkm.GenNumber < 5)
                lvl = pkm.CurrentLevel; // be generous with transfer conditions
            else if (pkm.Format < 5) // and native
                lvl = 0;
            return new EncounterStatic
            {
                Species = Legal.GetBaseSpecies(pkm),
                Location = loc != -1 ? loc : GetSuggestedEggMetLocation(pkm),
                Level = lvl,
            };
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
                    switch (pkm.Format)
                    {
                        case 3:
                            return pkm.FRLG ? 146 /* Four Island */ : 32; // Route 117
                        case 4:
                            return Legal.Transfer3; // Pal Park
                        default:
                            return Legal.Transfer4; // Transporter
                    }

                case GameVersion.D:
                case GameVersion.P:
                case GameVersion.Pt:
                    return pkm.Format > 4 ? Legal.Transfer4 /* Transporter */ : 4; // Solaceon Town
                case GameVersion.HG:
                case GameVersion.SS:
                    return pkm.Format > 4 ? Legal.Transfer4 /* Transporter */ : 182; // Route 34

                case GameVersion.B:
                case GameVersion.W:
                case GameVersion.B2:
                case GameVersion.W2:
                    return 16; // Route 3

                case GameVersion.X:
                case GameVersion.Y:
                    return 38; // Route 7
                case GameVersion.AS:
                case GameVersion.OR:
                    return 318; // Battle Resort

                case GameVersion.SN:
                case GameVersion.MN:
                case GameVersion.US:
                case GameVersion.UM:
                    return 50; // Route 4
            }
            return -1;
        }
        /// <summary> 
        /// Gets the correct Transfer Met location for the origin game.
        /// </summary>
        /// <remarks>
        /// Returns -1 if the met location is not overriden with a transfer location
        /// </remarks>
        private static int GetSuggestedTransferLocation(PKM pkm)
        {
            if (pkm.HasOriginalMetLocation)
                return -1;
            if (pkm.VC1)
                return Legal.Transfer1;
            if (pkm.VC2)
                return Legal.Transfer2;
            if (pkm.Format == 4) // Pal Park
                return Legal.Transfer3;
            if (pkm.Format >= 5) // Transporter
            {
                return pkm.Gen4 && pkm.FatefulEncounter && Legal.CrownBeasts.Contains(pkm.Species)
                    ? (pkm.Species == 251 ? Legal.Transfer4_CelebiUnused : Legal.Transfer4_CrownUnused) // Celebi : Beast
                    : Legal.Transfer4; // Pokétransfer (not Crown)
            }
            return -1;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic for providing suggested property values with respect to the input data.
    /// </summary>
    public static class EncounterSuggestion
    {
        /// <summary>
        /// Gets an object containing met data properties that might be legal.
        /// </summary>
        public static EncounterSuggestionData? GetSuggestedMetInfo(PKM pkm)
        {
            int loc = GetSuggestedTransferLocation(pkm);

            if (pkm.WasEgg)
                return GetSuggestedEncounterEgg(pkm, loc);

            var w = EncounterSlotGenerator.GetCaptureLocation(pkm);
            if (w != null)
                return GetSuggestedEncounterWild(pkm, w, loc);

            var s = EncounterStaticGenerator.GetStaticLocation(pkm);
            if (s != null)
                return GetSuggestedEncounterStatic(pkm, s, loc);

            return null;
        }

        private static EncounterSuggestionData GetSuggestedEncounterEgg(PKM pkm, int loc = -1)
        {
            int lvl = GetSuggestedEncounterEggMetLevel(pkm);
            var met = loc != -1 ? loc : GetSuggestedEggMetLocation(pkm);
            return new EncounterSuggestionData(pkm, met, lvl);
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

        private static EncounterSuggestionData GetSuggestedEncounterWild(PKM pkm, EncounterArea area, int loc = -1)
        {
            var slots = area.Slots.OrderBy(s => s.LevelMin);
            var first = slots.First();
            var met = loc != -1 ? loc : area.Location;
            return new EncounterSuggestionData(pkm, first, met);
        }

        private static EncounterSuggestionData GetSuggestedEncounterStatic(PKM pkm, EncounterStatic s, int loc = -1)
        {
            var met = loc != -1 ? loc : s.Location;
            return new EncounterSuggestionData(pkm, s, met);
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

        public static int GetLowestLevel(PKM pkm, int startLevel)
        {
            if (startLevel == -1)
                startLevel = 100;

            var table = EvolutionTree.GetEvolutionTree(pkm, pkm.Format);
            int count = 1;
            for (int i = 100; i >= startLevel; i--)
            {
                var evos = table.GetValidPreEvolutions(pkm, maxLevel: i, minLevel: startLevel, skipChecks: true);
                if (evos.Count < count) // lost an evolution, prior level was minimum current level
                    return evos.Max(evo => evo.Level) + 1;
                count = evos.Count;
            }
            return startLevel;
        }

        public static int GetSuggestedMetLevel(PKM pkm, int minLevel)
        {
            var clone = pkm.Clone();
            int minMove = -1;
            for (int i = clone.CurrentLevel; i >= minLevel; i--)
            {
                clone.Met_Level = i;
                var la = new LegalityAnalysis(clone);
                if (la.Valid)
                    return i;
                if (la.Info.Moves.All(z => z.Valid))
                    minMove = i;
            }
            return Math.Max(minMove, minLevel);
        }
    }

    public sealed class EncounterSuggestionData : IRelearn
    {
        private readonly IEncounterable? Encounter;

        public IReadOnlyList<int> Relearn => Encounter is IRelearn r ? r.Relearn : Array.Empty<int>();

        public EncounterSuggestionData(PKM pkm, IEncounterable enc, int met)
        {
            Encounter = enc;
            Species = pkm.Species;
            Form = pkm.AltForm;
            Location = met;

            LevelMin = enc.LevelMin;
            LevelMax = enc.LevelMax;
        }

        public EncounterSuggestionData(PKM pkm, int met, int lvl)
        {
            Species = pkm.Species;
            Form = pkm.AltForm;
            Location = met;

            LevelMin = lvl;
            LevelMax = lvl;
        }

        public int Species { get; }
        public int Form { get; }
        public int Location { get; }

        public int LevelMin { get; }
        public int LevelMax { get; }

        public int GetSuggestedMetLevel(PKM pkm) => EncounterSuggestion.GetSuggestedMetLevel(pkm, LevelMin);
    }
}

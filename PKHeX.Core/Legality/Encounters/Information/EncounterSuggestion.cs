using System;
using System.Collections.Generic;
using System.Linq;

using static PKHeX.Core.GameVersion;

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
            if (!pkm.IsNative && pkm.Generation < 5)
                return pkm.CurrentLevel; // be generous with transfer conditions
            if (pkm.Format < 5) // and native
                return pkm.Format == 2 && pkm.Met_Location != 0 ? 1 : 0;
            return 1; // gen5+
        }

        public static int GetSuggestedEncounterEggLocationEgg(PKM pkm, bool traded = false)
        {
            return GetSuggestedEncounterEggLocationEgg(pkm.Generation, traded);
        }

        public static int GetSuggestedEncounterEggLocationEgg(int generation, bool traded = false) => generation switch
        {
            1 or 2 or 3 => 0,
            4 => traded ? Locations.LinkTrade4 : Locations.Daycare4,
            5 => traded ? Locations.LinkTrade5 : Locations.Daycare5,
            _ => traded ? Locations.LinkTrade6 : Locations.Daycare5,
        };

        private static EncounterSuggestionData GetSuggestedEncounterWild(PKM pkm, EncounterSlot first, int loc = -1)
        {
            var met = loc != -1 ? loc : first.Location;
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
        public static int GetSuggestedEggMetLocation(PKM pkm) => (GameVersion)pkm.Version switch
        {
            R or S or E or FR or LG => pkm.Format switch
            {
                3 => (pkm.FRLG ? Locations.HatchLocationFRLG : Locations.HatchLocationRSE),
                4 => Locations.Transfer3, // Pal Park
                _ => Locations.Transfer4,
            },

            D or P or Pt => pkm.Format > 4 ? Locations.Transfer4 : Locations.HatchLocationDPPt,
            HG or SS => pkm.Format > 4 ? Locations.Transfer4 : Locations.HatchLocationHGSS,

            B or W or B2 or W2 => Locations.HatchLocation5,

            X or Y => Locations.HatchLocation6XY,
            AS or OR => Locations.HatchLocation6AO,

            SN or MN or US or UM => Locations.HatchLocation7,
            RD or BU or GN or YW => Locations.Transfer1,
            GD or SV or C => Locations.Transfer2,
            GSC or RBY => pkm.Met_Level == 0 ? 0 : Locations.HatchLocationC,

            SW or SH => Locations.HatchLocation8,
            _ => -1,
        };

        /// <summary>
        /// Gets the correct Transfer Met location for the origin game.
        /// </summary>
        /// <param name="pkm">Pokémon data to suggest for</param>
        /// <remarks>
        /// Returns -1 if the met location is not overriden with a transfer location
        /// </remarks>
        public static int GetSuggestedTransferLocation(PKM pkm)
        {
            if (pkm.Version == (int)GO)
                return Locations.GO8;
            if (pkm.HasOriginalMetLocation)
                return -1;
            if (pkm.VC1)
                return Locations.Transfer1;
            if (pkm.VC2)
                return Locations.Transfer2;
            if (pkm.Format == 4) // Pal Park
                return Locations.Transfer3;
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

        /// <summary>
        /// Gets the suggested <see cref="PKM.Met_Level"/> based on a baseline <see cref="minLevel"/> and the <see cref="pkm"/>'s current moves.
        /// </summary>
        /// <param name="pkm">Entity to calculate for</param>
        /// <param name="minLevel">Encounter minimum level to calculate for</param>
        /// <returns>Minimum level the <see cref="pkm"/> can have for its <see cref="PKM.Met_Level"/></returns>
        /// <remarks>Brute-forces the value by cloning the <see cref="pkm"/> and adjusting the <see cref="PKM.Met_Level"/> and returning the lowest valid value.</remarks>
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

    public sealed class EncounterSuggestionData : ISpeciesForm, IRelearn
    {
        private readonly IEncounterable? Encounter;

        public IReadOnlyList<int> Relearn => Encounter is IRelearn r ? r.Relearn : Array.Empty<int>();

        public EncounterSuggestionData(PKM pkm, IEncounterable enc, int met)
        {
            Encounter = enc;
            Species = pkm.Species;
            Form = pkm.Form;
            Location = met;

            LevelMin = enc.LevelMin;
            LevelMax = enc.LevelMax;
        }

        public EncounterSuggestionData(PKM pkm, int met, int lvl)
        {
            Species = pkm.Species;
            Form = pkm.Form;
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
        public int GetSuggestedEncounterType() => Encounter is IEncounterTypeTile t ? t.TypeEncounter.GetIndex() : 0;
        public bool HasEncounterType(int format) => Encounter is IEncounterTypeTile t && t.HasTypeEncounter(format);
    }
}

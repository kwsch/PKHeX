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

            var chain = EvolutionChain.GetValidPreEvolutions(pkm, maxLevel: 100, skipChecks: true);
            var w = EncounterSlotGenerator.GetCaptureLocation(pkm, chain);
            var s = EncounterStaticGenerator.GetStaticLocation(pkm, chain);
            if (w is null)
                return s is null ? null : GetSuggestedEncounter(pkm, s, loc);
            if (s is null)
                return GetSuggestedEncounter(pkm, w, loc);

            bool isDefinitelySlot = chain.Any(z => z.Species == w.Species && z.Form == w.Form);
            bool isDefinitelyStatic = chain.Any(z => z.Species == s.Species && z.Form == s.Form);
            IEncounterable obj = (isDefinitelySlot || !isDefinitelyStatic) ? w : s;
            return GetSuggestedEncounter(pkm, obj, loc);
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
            return GetSuggestedEncounterEggLocationEgg(pkm.Generation, (GameVersion)pkm.Version, traded);
        }

        public static int GetSuggestedEncounterEggLocationEgg(int generation, GameVersion version, bool traded = false) => generation switch
        {
            1 or 2 or 3 => 0,
            4 => traded ? Locations.LinkTrade4 : Locations.Daycare4,
            5 => traded ? Locations.LinkTrade5 : Locations.Daycare5,
            8 when BDSP.Contains(version)=> traded ? Locations.LinkTrade6NPC : Locations.Daycare8b,
            _ => traded ? Locations.LinkTrade6 : Locations.Daycare5,
        };

        private static EncounterSuggestionData GetSuggestedEncounter(PKM pkm, IEncounterable enc, int loc = -1)
        {
            var met = loc != -1 ? loc : enc.Location;
            return new EncounterSuggestionData(pkm, enc, met);
        }

        /// <inheritdoc cref="EggStateLegality.GetEggHatchLocation"/>
        public static int GetSuggestedEggMetLocation(PKM pkm) => EggStateLegality.GetEggHatchLocation((GameVersion)pkm.Version, pkm.Format);

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

        public static bool IterateMinimumCurrentLevel(PKM pk, bool isLegal, int max = 100)
        {
            var original = pk.CurrentLevel;
            var originalEXP = pk.EXP;
            if (isLegal)
            {
                if (original == 1)
                    return false;
                max = original - 1;
            }

            for (int i = max; i != 0; i--)
            {
                pk.CurrentLevel = i;
                var la = new LegalityAnalysis(pk);
                var valid = la.Valid;
                if (valid)
                    continue;

                var revert = Math.Min(100, i + 1);
                if (revert == original)
                {
                    pk.EXP = originalEXP;
                    return false;
                }

                pk.CurrentLevel = revert;
                return true;
            }
            return true; // 1
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
        public GroundTileType GetSuggestedGroundTile() => Encounter is IGroundTypeTile t ? t.GroundTile.GetIndex() : 0;
        public bool HasGroundTile(int format) => Encounter is IGroundTypeTile t && t.HasGroundTile(format);
    }
}

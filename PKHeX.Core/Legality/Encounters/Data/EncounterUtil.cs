using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Miscellaneous setup utility for legality checking <see cref="IEncounterable"/> data sources.
    /// </summary>
    internal static class EncounterUtil
    {
        /// <summary>
        /// Gets the relevant <see cref="EncounterStatic"/> objects that appear in the relevant game.
        /// </summary>
        /// <param name="source">Table of valid encounters that appear for the game pairing</param>
        /// <param name="game">Game to filter for</param>
        /// <returns>Array of encounter objects that are encounterable on the input game</returns>
        internal static EncounterStatic[] GetStaticEncounters(IEnumerable<EncounterStatic> source, GameVersion game)
        {
            return source.Where(s => s.Version.Contains(game)).ToArray();
        }

        /// <summary>
        /// Direct fetch for <see cref="EncounterArea"/> data; can also be used to fetch supplementary encounter streams.
        /// </summary>
        /// <param name="ident">Unpacking identification ASCII characters (first two bytes of binary)</param>
        /// <param name="resource">Resource name (will be prefixed with "encounter_"</param>
        /// <returns>Array of encounter areas</returns>
        internal static T[] GetEncounterTables<T>(string ident, string resource) where T : EncounterArea32, new()
        {
            byte[] mini = Util.GetBinaryResource($"encounter_{resource}.pkl");
            return EncounterArea32.GetArray<T>(BinLinker.Unpack(mini, ident));
        }

        /// <summary>
        /// Direct fetch for <see cref="EncounterArea"/> data; can also be used to fetch supplementary encounter streams.
        /// </summary>
        /// <param name="ident">Unpacking identification ASCII characters (first two bytes of binary)</param>
        /// <param name="resource">Resource name (will be prefixed with "encounter_"</param>
        /// <returns>Array of encounter areas</returns>
        internal static T[] GetEncounterTables8<T>(string ident, string resource) where T : EncounterAreaSH, new()
        {
            byte[] mini = Util.GetBinaryResource($"encounter_{resource}.pkl");
            return EncounterAreaSH.GetArray<T>(BinLinker.Unpack(mini, ident));
        }

        /// <summary>
        /// Combines <see cref="EncounterArea"/> slot arrays with the same <see cref="EncounterArea.Location"/>.
        /// </summary>
        /// <param name="tables">Input encounter areas to combine</param>
        /// <returns>Combined Array of encounter areas. No duplicate location IDs will be present.</returns>
        internal static T[] AddExtraTableSlots<T>(params T[][] tables) where T : EncounterArea, new()
        {
            return tables.SelectMany(s => s).GroupBy(l => l.Location)
                .Select(t => t.Count() == 1
                    ? t.First() // only one table, just return the area
                    : new T { Location = t.Key, Slots = t.SelectMany(s => s.Slots).ToArray() })
                .ToArray();
        }

        /// <summary>
        /// Marks Encounter Slots for party lead's ability slot influencing.
        /// </summary>
        /// <remarks>Magnet Pull attracts Steel type slots, and Static attracts Electric</remarks>
        /// <param name="Areas">Encounter Area array for game</param>
        /// <param name="t">Personal data for use with a given species' type</param>
        internal static void MarkEncountersStaticMagnetPull(IEnumerable<EncounterArea> Areas, PersonalTable t)
        {
            foreach (EncounterArea Area in Areas)
            {
                foreach (var grp in Area.Slots.GroupBy(z => z.Type))
                    MarkEncountersStaticMagnetPull(grp, t);
            }
        }

        internal static void MarkEncountersStaticMagnetPull(IEnumerable<EncounterSlot> grp, PersonalTable t)
        {
            GetStaticMagnet(t, grp, out List<EncounterSlot> s, out List<EncounterSlot> m);
            for (var i = 0; i < s.Count; i++)
            {
                var slot = s[i];
                slot.Permissions.StaticIndex = i;
                slot.Permissions.StaticCount = s.Count;
            }
            for (var i = 0; i < m.Count; i++)
            {
                var slot = m[i];
                slot.Permissions.MagnetPullIndex = i;
                slot.Permissions.MagnetPullCount = m.Count;
            }
        }

        internal static void MarkEncountersStaticMagnetPullPermutation(IEnumerable<EncounterSlot> grp, PersonalTable t, List<EncounterSlot> permuted)
        {
            GetStaticMagnet(t, grp, out List<EncounterSlot> s, out List<EncounterSlot> m);

            // Apply static/magnet values; if any permutation has a unique slot combination, add it to the slot list.
            for (int i = 0; i < s.Count; i++)
            {
                var slot = s[i];
                if (slot.Permissions.StaticIndex >= 0) // already has unique data
                {
                    if (slot.IsMatchStatic(i, s.Count))
                        continue; // same values, no permutation
                    if (permuted.Any(z => z.SlotNumber == slot.SlotNumber && z.IsMatchStatic(i, s.Count) && z.Species == slot.Species))
                        continue; // same values, previously permuted

                    s[i] = slot = slot.Clone();
                    permuted.Add(slot);
                }
                slot.Permissions.StaticIndex = i;
                slot.Permissions.StaticCount = s.Count;
            }
            for (int i = 0; i < m.Count; i++)
            {
                var slot = m[i];
                if (slot.Permissions.MagnetPullIndex >= 0) // already has unique data
                {
                    if (slot.IsMatchStatic(i, m.Count))
                        continue; // same values, no permutation
                    if (permuted.Any(z => z.SlotNumber == slot.SlotNumber && z.IsMatchMagnet(i, m.Count) && z.Species == slot.Species))
                        continue; // same values, previously permuted

                    m[i] = slot = slot.Clone();
                    permuted.Add(slot);
                }
                slot.Permissions.MagnetPullIndex = i;
                slot.Permissions.MagnetPullCount = m.Count;
            }
        }

        private static void GetStaticMagnet(PersonalTable t, IEnumerable<EncounterSlot> grp, out List<EncounterSlot> s, out List<EncounterSlot> m)
        {
            const int steel = (int)MoveType.Steel;
            const int electric = (int)MoveType.Electric + 1; // offset by 1 in gen3/4 for the ??? type
            s = new List<EncounterSlot>();
            m = new List<EncounterSlot>();
            foreach (EncounterSlot Slot in grp)
            {
                var p = t[Slot.Species];
                if (p.IsType(steel))
                    m.Add(Slot);
                if (p.IsType(electric))
                    s.Add(Slot);
            }
        }

        /// <summary>
        /// Sets the <see cref="EncounterSlot1.Version"/> value, for use in determining split-generation origins.
        /// </summary>
        /// <remarks>Only used for Gen 1 &amp; 2, as <see cref="PKM.Version"/> data is not present.</remarks>
        /// <param name="Areas">Ingame encounter data</param>
        /// <param name="Version">Version ID to set</param>
        internal static void MarkEncountersVersion(IEnumerable<EncounterArea> Areas, GameVersion Version)
        {
            foreach (EncounterArea Area in Areas)
            {
                foreach (var Slot in Area.Slots)
                    Slot.Version = Version;
            }
        }

        /// <summary>
        /// Sets the <see cref="IGeneration.Generation"/> value.
        /// </summary>
        /// <param name="Generation">Generation number to set</param>
        /// <param name="Encounters">Ingame encounter data</param>
        internal static void MarkEncountersGeneration(int Generation, params IEnumerable<IGeneration>[] Encounters)
        {
            foreach (var table in Encounters)
                MarkEncountersGeneration(Generation, table);
        }

        /// <summary>
        /// Sets the <see cref="IGeneration.Generation"/> value, for use in determining split-generation origins.
        /// </summary>
        /// <param name="Generation">Generation number to set</param>
        /// <param name="Areas">Ingame encounter data</param>
        internal static void MarkEncountersGeneration(int Generation, params IEnumerable<EncounterArea>[] Areas)
        {
            foreach (var table in Areas)
            {
                foreach (var area in table)
                    MarkEncountersGeneration(Generation, area.Slots);
            }
        }

        private static void MarkEncountersGeneration(int Generation, IEnumerable<IGeneration> Encounters)
        {
            foreach (IGeneration enc in Encounters)
                enc.Generation = Generation;
        }

        /// <summary>
        /// Groups areas by location id, raw data has areas with different slots but the same location id.
        /// </summary>
        /// <remarks>Similar to <see cref="AddExtraTableSlots{T}"/>, this method combines a single array.</remarks>
        /// <param name="Areas">Ingame encounter data</param>
        internal static void ReduceAreasSize<T>(ref T[] Areas) where T : EncounterArea, new()
        {
            Areas = Areas.GroupBy(a => a.Location).Select(a => new T
            {
                Location = a.Key,
                Slots = a.SelectMany(m => m.Slots).ToArray()
            }).ToArray();
        }

        internal static T[] ConcatAll<T>(params IEnumerable<T>[] arr) => arr.SelectMany(z => z).ToArray();

        internal static void MarkEncounterAreaArray<T>(params T[][] areas) where T : EncounterArea
        {
            foreach (var area in areas)
                MarkEncounterAreas(area);
        }

        private static void MarkEncounterAreas<T>(params T[] areas) where T : EncounterArea
        {
            foreach (var area in areas)
            {
                foreach (var slot in area.Slots)
                    slot.Area = area;
            }
        }

        internal static EncounterStatic Clone(this EncounterStatic s, int location)
        {
            var result = s.Clone();
            result.Location = location;
            return result;
        }

        internal static EncounterStatic[] Clone(this EncounterStatic s, int[] locations)
        {
            EncounterStatic[] Encounters = new EncounterStatic[locations.Length];
            for (int i = 0; i < locations.Length; i++)
                Encounters[i] = s.Clone(locations[i]);
            return Encounters;
        }

        internal static IEnumerable<EncounterStatic> DreamRadarClone(this EncounterStatic s)
        {
            for (int i = 0; i < 8; i++)
                yield return s.DreamRadarClone((5 * i) + 5);  // Level from 5->40 depends on the number of badges
        }

        private static EncounterStatic DreamRadarClone(this EncounterStatic s, int level)
        {
            var result = s.Clone(level);
            result.Level = level;
            result.Location = 30015;// Pokemon Dream Radar
            result.Gift = true;     // Only
            result.Ball = 25;       // Dream Ball
            return result;
        }

        internal static void MarkEncounterTradeStrings(EncounterTrade[] table, string[][] strings)
        {
            int half = strings[1].Length / 2;
            for (int i = 0; i < half; i++)
            {
                var t = table[i];
                t.Nicknames = getNames(i, strings);
                t.TrainerNames = getNames(i + half, strings);
            }
            string[] getNames(int i, IEnumerable<string[]> names) => names.Select(z => z.Length > i ? z[i] : string.Empty).ToArray();
        }

        internal static void MarkEncounterGame(IEnumerable<IVersion> table, GameVersion version)
        {
            foreach (var t in table.Where(z => z.Version == GameVersion.Any))
                t.Version = version;
        }
    }
}

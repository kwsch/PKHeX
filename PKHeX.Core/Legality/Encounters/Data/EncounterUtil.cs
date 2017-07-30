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
        /// Gets the <see cref="EncounterArea"/> data for the input game via the program's resource streams.
        /// </summary>
        /// <param name="game">Game to fetch for</param>
        /// <remarks> <see cref="EncounterSlot.SlotNumber"/> data is not marked, as the RNG seed is 64 bits (permitting sufficient randomness).</remarks>
        /// <returns>Array of areas that are encounterable on the input game.</returns>
        internal static EncounterArea[] GetEncounterTables(GameVersion game)
        {
            switch (game)
            {
                case GameVersion.B:  return GetEncounterTables("51", "b");
                case GameVersion.W:  return GetEncounterTables("51", "w");
                case GameVersion.B2: return GetEncounterTables("52", "b2");
                case GameVersion.W2: return GetEncounterTables("52", "w2");
                case GameVersion.X:  return GetEncounterTables("xy", "x");
                case GameVersion.Y:  return GetEncounterTables("xy", "y");
                case GameVersion.AS: return GetEncounterTables("ao", "a");
                case GameVersion.OR: return GetEncounterTables("ao", "o");
                case GameVersion.SN: return GetEncounterTables("sm", "sn");
                case GameVersion.MN: return GetEncounterTables("sm", "mn");
            }
            return null; // bad request
        }

        /// <summary>
        /// Direct fetch for <see cref="EncounterArea"/> data; can also be used to fetch supplementary encounter streams.
        /// </summary>
        /// <param name="ident">Unpacking identification ASCII characters (first two bytes of binary)</param>
        /// <param name="resource">Resource name (will be prefixed with "encounter_"</param>
        /// <returns>Array of encounter areas</returns>
        internal static EncounterArea[] GetEncounterTables(string ident, string resource)
        {
            byte[] mini = Util.GetBinaryResource($"encounter_{resource}.pkl");
            return EncounterArea.GetArray(Data.UnpackMini(mini, ident));
        }

        /// <summary>
        /// Combines <see cref="EncounterArea"/> slot arrays with the same <see cref="EncounterArea.Location"/>.
        /// </summary>
        /// <param name="tables">Input encounter areas to combine</param>
        /// <returns>Combined Array of encounter areas. No duplicate location IDs will be present.</returns>
        internal static EncounterArea[] AddExtraTableSlots(params EncounterArea[][] tables)
        {
            return tables.SelectMany(s => s).GroupBy(l => l.Location)
                .Select(t => t.Count() == 1
                    ? t.First() // only one table, just return the area
                    : new EncounterArea { Location = t.First().Location, Slots = t.SelectMany(s => s.Slots).ToArray() })
                .ToArray();
        }

        /// <summary>
        /// Marks Encounter Slots for party lead's ability slot influencing.
        /// </summary>
        /// <remarks>Magnet Pull attracts Steel type slots, and Static attracts Electric</remarks>
        /// <param name="Areas">Encounter Area array for game</param>
        /// <param name="t">Personal data for use with a given species' type</param>
        internal static void MarkEncountersStaticMagnetPull(ref EncounterArea[] Areas, PersonalTable t)
        {
            const int steel = 8;
            const int electric = 12;
            foreach (EncounterArea Area in Areas)
            {
                var s = new List<EncounterSlot>(); // Static
                var m = new List<EncounterSlot>(); // Magnet Pull
                foreach (EncounterSlot Slot in Area.Slots)
                {
                    var types = t[Slot.Species].Types;
                    if (types[0] == steel || types[1] == steel)
                        m.Add(Slot);
                    if (types[0] == electric || types[1] == electric)
                        s.Add(Slot);
                }
                foreach (var slot in s)
                {
                    slot.Permissions.Static = true;
                    slot.Permissions.StaticCount = s.Count;
                }
                foreach (var slot in m)
                {
                    slot.Permissions.MagnetPull = true;
                    slot.Permissions.MagnetPullCount = s.Count;
                }
            }
        }

        /// <summary>
        /// Sets the <see cref="EncounterStatic.Generation"/> value, for use in determining split-generation origins.
        /// </summary>
        /// <remarks>Only used for Gen 1 & 2, as <see cref="PKM.Version"/> data is not present.</remarks>
        /// <param name="Encounters">Ingame encounter data</param>
        /// <param name="Generation">Generation number to set</param>
        internal static void MarkEncountersGeneration(IEnumerable<EncounterStatic> Encounters, int Generation)
        {
            foreach (EncounterStatic Encounter in Encounters)
                Encounter.Generation = Generation;
        }

        /// <summary>
        /// Sets the <see cref="EncounterSlot1.Version"/> value, for use in determining split-generation origins.
        /// </summary>
        /// <remarks>Only used for Gen 1 & 2, as <see cref="PKM.Version"/> data is not present.</remarks>
        /// <param name="Areas">Ingame encounter data</param>
        /// <param name="Version">Version ID to set</param>
        internal static void MarkEncountersVersion(IEnumerable<EncounterArea> Areas, GameVersion Version)
        {
            foreach (EncounterArea Area in Areas)
            foreach (var Slot in Area.Slots.OfType<EncounterSlot1>())
                Slot.Version = Version;
        }

        /// <summary>
        /// Sets the <see cref="EncounterStatic.Generation"/> value, for use in determining split-generation origins.
        /// </summary>
        /// <remarks>Only used for Gen 1 & 2, as <see cref="PKM.Version"/> data is not present.</remarks>
        /// <param name="Areas">Ingame encounter data</param>
        /// <param name="Generation">Generation number to set</param>
        internal static void MarkEncountersGeneration(IEnumerable<EncounterArea> Areas, int Generation)
        {
            foreach (EncounterArea Area in Areas)
            foreach (EncounterSlot Slot in Area.Slots)
                Slot.Generation = Generation;
        }

        /// <summary>
        /// Groups areas by location id, raw data has areas with different slots but the same location id.
        /// </summary>
        /// <remarks>Similar to <see cref="AddExtraTableSlots"/>, this method combines a single array.</remarks>
        /// <param name="Areas">Ingame encounter data</param>
        internal static void ReduceAreasSize(ref EncounterArea[] Areas)
        {
            Areas = Areas.GroupBy(a => a.Location).Select(a => new EncounterArea
            {
                Location = a.First().Location,
                Slots = a.SelectMany(m => m.Slots).ToArray()
            }).ToArray();
        }

        /// <summary>
        /// Sets the <see cref="EncounterArea.Location"/> to the <see cref="EncounterSlot.Location"/> for identifying where the slot is encountered.
        /// </summary>
        /// <remarks>Some games / transferred <see cref="PKM"/> data do not contain original encounter location IDs; is mainly for info purposes.</remarks>
        /// <param name="Areas">Ingame encounter data</param>
        internal static void MarkSlotLocation(ref EncounterArea[] Areas)
        {
            foreach (EncounterArea Area in Areas)
            foreach (EncounterSlot Slot in Area.Slots)
                Slot.Location = Area.Location;
        }
    }
}

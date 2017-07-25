using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    internal static class EncounterUtil
    {
        internal static EncounterArea[] GetEncounterTables(GameVersion Game)
        {
            switch (Game)
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

        internal static EncounterArea[] GetEncounterTables(string ident, string resource)
        {
            byte[] mini = Util.GetBinaryResource($"encounter_{resource}.pkl");
            return EncounterArea.GetArray(Data.UnpackMini(mini, ident));
        }

        internal static EncounterArea[] AddExtraTableSlots(params EncounterArea[][] tables)
        {
            return tables.SelectMany(s => s).GroupBy(l => l.Location)
                .Select(t => t.Count() == 1
                    ? t.First() // only one table, just return the area
                    : new EncounterArea { Location = t.First().Location, Slots = t.SelectMany(s => s.Slots).ToArray() })
                .ToArray();
        }

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

        internal static void MarkEncountersGeneration(EncounterStatic[] Encounters, int Generation)
        {
            foreach (EncounterStatic Encounter in Encounters)
                Encounter.Generation = Generation;
        }

        internal static void MarkEncountersVersion(EncounterArea[] Areas, GameVersion Version)
        {
            foreach (EncounterArea Area in Areas)
            foreach (var Slot in Area.Slots.OfType<EncounterSlot1>())
                Slot.Version = Version;
        }

        internal static void MarkEncountersGeneration(IEnumerable<EncounterArea> Areas, int Generation)
        {
            foreach (EncounterArea Area in Areas)
            foreach (EncounterSlot Slot in Area.Slots)
                Slot.Generation = Generation;
        }

        internal static void ReduceAreasSize(ref EncounterArea[] Areas)
        {
            // Group areas by location id, the raw data have areas with different slots but the same location id
            Areas = Areas.GroupBy(a => a.Location).Select(a => new EncounterArea
            {
                Location = a.First().Location,
                Slots = a.SelectMany(m => m.Slots).ToArray()
            }).ToArray();
        }

        internal static void MarkSlotLocation(ref EncounterArea[] Areas)
        {
            foreach (EncounterArea Area in Areas)
            {
                foreach (EncounterSlot Slot in Area.Slots)
                {
                    Slot.Location = Area.Location;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="GameVersion.SWSH"/> encounter area
    /// </summary>
    public sealed class EncounterArea8 : EncounterAreaSH
    {
        /// <inheritdoc />
        public override bool IsMatchLocation(int location)
        {
            if (Location == location)
                return true;

            if (!PermitCrossover)
                return false;

            // Get all other areas that the Location can bleed encounters to
            if (!ConnectingArea8.TryGetValue(Location, out var others))
                return false;

            // Check if any of the other areas are the met location
            return others.Contains((byte)location);
        }

        protected override IEnumerable<EncounterSlot> GetMatchFromEvoLevel(PKM pkm, IEnumerable<EvoCriteria> vs, int minLevel)
        {
            var loc = Location;
            if (IsWildArea8(loc)) // wild area gets boosted up to level 60 post-game
            {
                const int boostTo = 60;
                if (pkm.Met_Level == boostTo)
                {
                    var boost = Slots.Where(slot => vs.Any(evo => IsMatch(evo, slot) && evo.Level >= boostTo));
                    return boost.Where(s => s.LevelMax < boostTo || s.IsLevelWithinRange(minLevel));
                }
            }
            var slots = Slots.Where(slot => vs.Any(evo => IsMatch(evo, slot) && evo.Level >= slot.LevelMin));

            // Get slots where pokemon can exist with respect to level constraints
            return slots.Where(s => s.IsLevelWithinRange(minLevel));
        }

        private static bool IsMatch(DexLevel evo, EncounterSlot slot)
        {
            if (evo.Species != slot.Species)
                return false;
            if (evo.Form == slot.Form)
                return true;
            if (Legal.FormChange.Contains(evo.Species))
                return true;
            return false;
        }

        protected override IEnumerable<EncounterSlot> GetFilteredSlots(PKM pkm, IEnumerable<EncounterSlot> slots, int minLevel) => slots;

        public static bool IsWildArea8(int loc) => 122 <= loc && loc <= 154; // Rolling Fields -> Lake of Outrage

        // Location, and areas that can feed encounters into it.
        public static readonly IReadOnlyDictionary<int, IReadOnlyList<byte>> ConnectingArea8 = new Dictionary<int, IReadOnlyList<byte>>
        {
            // Rolling Fields
            // Dappled Grove, East Lake Axewell, West Lake Axewell
            // Also connects to South Lake Miloch but too much of a stretch
            {122, new byte[] {124, 128, 130}},

            // Dappled Grove
            // Rolling Fields, Watchtower Ruins
            {124, new byte[] {122, 126}},

            // Watchtower Ruins
            // Dappled Grove, West Lake Axewell
            {126, new byte[] {124, 130}},

            // East Lake Axewell
            // Rolling Fields, West Lake Axewell, Axew's Eye, North Lake Miloch
            {128, new byte[] {122, 130, 132, 138}},

            // West Lake Axewell
            // Rolling Fields, Watchtower Ruins, East Lake Axewell, Axew's Eye
            {130, new byte[] {122, 126, 128, 132}},

            // Axew's Eye
            // East Lake Axewell, West Lake Axewell
            {132, new byte[] {128, 130}},

            // South Lake Miloch
            // Giant's Seat, North Lake Miloch
            {134, new byte[] {136, 138}},

            // Giant's Seat
            // South Lake Miloch, North Lake Miloch
            {136, new byte[] {134, 138}},

            // North Lake Miloch
            // East Lake Axewell, South Lake Miloch, Giant's Seat
            // Also connects to Motostoke Riverbank but too much of a stretch
            {138, new byte[] {134, 136}},

            // Motostoke Riverbank
            // Bridge Field
            {140, new byte[] {142}},

            // Bridge Field
            // Motostoke Riverbank, Stony Wilderness
            {142, new byte[] {140, 144}},

            // Stony Wilderness
            // Bridge Field, Dusty Bowl, Giant's Mirror, Giant's Cap
            {144, new byte[] {142, 146, 148, 152}},

            // Dusty Bowl
            // Stony Wilderness, Giant's Mirror, Hammerlocke Hills
            {146, new byte[] {144, 148, 150}},

            // Giant's Mirror
            // Stony Wilderness, Dusty Bowl, Hammerlocke Hills
            {148, new byte[] {144, 146, 148}},

            // Hammerlocke Hills
            // Dusty Bowl, Giant's Mirror, Giant's Cap
            {150, new byte[] {146, 148, 152}},

            // Giant's Cap
            // Stony Wilderness, Giant's Cap
            // Also connects to Lake of Outrage but too much of a stretch
            {152, new byte[] {144, 150}},

            // Lake of Outrage is just itself.
        };
    }

    public abstract class EncounterAreaSH : EncounterArea
    {
        /// <summary>
        /// Slots from this area can cross over to another area, resulting in a different met location.
        /// </summary>
        public bool PermitCrossover { get; internal set; }

        /// <summary>
        /// Gets an array of areas from an array of raw area data
        /// </summary>
        /// <param name="entries">Simplified raw format of an Area</param>
        /// <returns>Array of areas</returns>
        public static T[] GetArray<T>(byte[][] entries) where T : EncounterAreaSH, new()
        {
            T[] data = new T[entries.Length];
            for (int i = 0; i < data.Length; i++)
            {
                var loc = data[i] = new T();
                loc.LoadSlots(entries[i]);
            }
            return data;
        }

        private void LoadSlots(byte[] areaData)
        {
            Location = areaData[0];
            Slots = new EncounterSlot[areaData[1]];

            int ctr = 0;
            int ofs = 2;
            do
            {
                var flags = (AreaWeather8)BitConverter.ToUInt16(areaData, ofs);
                var min = areaData[ofs + 2];
                var max = areaData[ofs + 3];
                var count = areaData[ofs + 4];
                // ofs+5 reserved
                ofs += 6;
                for (int i = 0; i < count; i++, ctr++, ofs += 2)
                {
                    var specForm = BitConverter.ToUInt16(areaData, ofs);
                    Slots[ctr] = new EncounterSlot8(specForm, min, max, flags);
                }
            } while (ctr != Slots.Length);
            foreach (var slot in Slots)
                slot.Area = this;
        }
    }

    /// <summary>
    /// Encounter Conditions for <see cref="GameVersion.SWSH"/>
    /// </summary>
    /// <remarks>Values above <see cref="All"/> are for Shaking/Fishing hidden encounters only.</remarks>
    [Flags]
    public enum AreaWeather8
    {
        None,
        Normal = 1,
        Overcast = 1 << 1,
        Raining = 1 << 2,
        Thunderstorm = 1 << 3,
        Intense_Sun = 1 << 4,
        Snowing = 1 << 5,
        Snowstorm = 1 << 6,
        Sandstorm = 1 << 7,
        Heavy_Fog = 1 << 8,

        All = Normal | Overcast | Raining | Thunderstorm | Intense_Sun | Snowing | Snowstorm | Sandstorm | Heavy_Fog,

        Shaking_Trees = 1 << 9,
        Fishing = 1 << 10,

        NotWeather = Shaking_Trees | Fishing,
    }

    /// <summary>
    /// Encounter Slot found in <see cref="GameVersion.SWSH"/>
    /// </summary>
    public sealed class EncounterSlot8 : EncounterSlot
    {
        public readonly AreaWeather8 Weather;
        public override string LongName => Weather == AreaWeather8.All ? wild : $"{wild} - {Weather.ToString().Replace("_", string.Empty)}";

        public EncounterSlot8(int specForm, int min, int max, AreaWeather8 weather)
        {
            Species = specForm & 0x7FF;
            Form = specForm >> 11;
            LevelMin = min;
            LevelMax = max;

            Weather = weather;
        }
    }
}

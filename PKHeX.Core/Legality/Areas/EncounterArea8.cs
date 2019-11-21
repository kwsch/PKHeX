using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="GameVersion.SWSH"/> encounter area
    /// </summary>
    public sealed class EncounterArea8 : EncounterArea32
    {
        /// <inheritdoc />
        public override bool IsMatchLocation(int location)
        {
            if (Location == location)
                return true;

            // get all other areas that can bleed encounters to the met location
            if (!ConnectingArea8.TryGetValue(location, out var others))
                return false;

            return others.Contains((byte) location);
        }

        protected override IEnumerable<EncounterSlot> GetMatchFromEvoLevel(PKM pkm, IEnumerable<EvoCriteria> vs, int minLevel)
        {
            var loc = Location;
            if (IsWildArea8(loc)) // wild area gets boosted up to level 60 post-game
            {
                const int boostTo = 60;
                if (pkm.Met_Level == boostTo)
                {
                    var boost = Slots.Where(slot => vs.Any(evo => evo.Species == slot.Species && evo.Form == slot.Form && evo.Level >= boostTo));
                    return boost.Where(s => s.LevelMax < boostTo || s.IsLevelWithinRange(minLevel));
                }
            }
            var slots = Slots.Where(slot => vs.Any(evo => evo.Species == slot.Species && evo.Form == slot.Form && evo.Level >= (slot.LevelMin)));

            // Get slots where pokemon can exist with respect to level constraints
            return slots.Where(s => s.IsLevelWithinRange(minLevel));
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
            // Dappled Grove, Watchtower Ruins
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
}
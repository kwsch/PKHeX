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
        protected override IEnumerable<EncounterSlot> GetMatchFromEvoLevel(PKM pkm, IEnumerable<EvoCriteria> vs, int minLevel)
        {
            var loc = Location;
            if (IsWildArea8(loc)) // wild area gets boosted up to level 60 postgame
            {
                const int boostTo = 60;
                if (pkm.Met_Level == boostTo)
                {
                    var boost = Slots.Where(slot => vs.Any(evo => evo.Species == slot.Species && evo.Form == slot.Form && evo.Level >= boostTo));
                    return boost.Where(s => s.LevelMax < 60 || s.IsLevelWithinRange(minLevel));
                }
            }
            var slots = Slots.Where(slot => vs.Any(evo => evo.Species == slot.Species && evo.Form == slot.Form && evo.Level >= (slot.LevelMin)));

            // Get slots where pokemon can exist with respect to level constraints
            return slots.Where(s => s.IsLevelWithinRange(minLevel));
        }

        protected override IEnumerable<EncounterSlot> GetFilteredSlots(PKM pkm, IEnumerable<EncounterSlot> slots, int minLevel) => slots;

        public static bool IsWildArea8(int loc) => 120 <= loc && loc <= 154;

        public static int[] ConnectingArea8(int loc)
        {
            if (!IsWildArea8(loc))
            {
                return new int[] { loc }; // Single
            }

            switch (loc)
            {
                case 122: // Rolling Fields
                    // Dappled Grove, East Lake Axewell, West Lake Axewell
                    // Also connects to South Lake Miloch but too much of a stretch
                    return new int[] { 124, 128, 130 };

                case 124: // Dappled Grove
                    // Dappled Grove, Watchtower Ruins
                    return new int[] { 122, 126 };

                case 126: // Watchtower Ruins
                    // Dappled Grove, West Lake Axewell
                    return new int[] { 124, 130 };

                case 128: // East Lake Axewell
                    // Rolling Fields, West Lake Axewell, Axew's Eye, North Lake Miloch
                    return new int[] { 122, 130, 132, 138 };

                case 130: // West Lake Axewell
                    // Rolling Fields, Watchtower Ruins, East Lake Axewell, Axew's Eye
                    return new int[] { 122, 126, 128, 132 };

                case 132: // Axew's Eye
                    // East Lake Axewell, West Lake Axewell
                    return new int[] { 128, 130 };

                case 134: // South Lake Miloch
                    // Giant's Seat, North Lake Miloch
                    return new int[] { 136, 138 };

                case 136: // Giant's Seat
                    // South Lake Miloch, North Lake Miloch
                    return new int[] { 134, 138 };

                case 138: // North Lake Miloch
                    // East Lake Axewell, South Lake Miloch, Giant's Seat
                    // Also connects to Motostoke Riverbank but too much of a stretch
                    return new int[] { 128, 134, 136 };

                case 140: // Motostoke Riverbank
                    // Bridge Field
                    return new int[] { 142 };

                case 142: // Bridge Field
                    // Motostoke Riverbank, Stony Wilderness
                    return new int[] { 140, 144 };

                case 144: // Stony Wilderness
                    // Bridge Field, Dusty Bowl, Giant's Mirror, Giant's Cap
                    return new int[] { 142, 146, 148, 152 };

                case 146: // Dusty Bowl
                    // Stony Wilderness, Giant's Mirror, Hammerlocke Hills
                    return new int[] { 144, 148, 150 };

                case 148: // Giant's Mirror
                    // Stony Wilderness, Dusty Bowl, Hammerlocke Hills
                    return new int[] { 144, 146, 148 };

                case 150: // Hammerlocke Hills
                    // Dusty Bowl, Giant's Mirror, Giant's Cap
                    return new int[] { 146, 148, 152 };

                case 152: // Giant's Cap
                    // Stony Wilderness, Giant's Cap
                    // Also connects to Lake of Outrage but too much of a stretch
                    return new int[] { 144, 150 };

                case 154: // Lake of Outrage
                    return new int[] { loc }; // Single

                default:
                    return new int[] { loc }; // Single
            }
        }
    }
}
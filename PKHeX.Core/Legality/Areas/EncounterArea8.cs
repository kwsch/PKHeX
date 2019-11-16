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
            if (120 <= loc && loc <= 154) // wild area gets boosted up to level 60 postgame
            {
                const int boostTo = 60;
                if (pkm.Met_Level == boostTo)
                {
                    var boost = Slots.Where(slot => vs.Any(evo => evo.Species == slot.Species && evo.Form == slot.Form && evo.Level >= boostTo));
                    return boost.Where(s => s.LevelMax < 60 || s.IsLevelWithinRange(minLevel));
                }
            }
            var slots = Slots.Where(slot => vs.Any(evo => evo.Species == slot.Species && evo.Level >= (slot.LevelMin)));

            // Get slots where pokemon can exist with respect to level constraints
            return slots.Where(s => s.IsLevelWithinRange(minLevel));
        }

        protected override IEnumerable<EncounterSlot> GetFilteredSlots(PKM pkm, IEnumerable<EncounterSlot> slots, int minLevel) => slots;
    }
}
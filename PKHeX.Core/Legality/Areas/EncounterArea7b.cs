using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="GameVersion.GG"/> encounter area
    /// </summary>
    public sealed class EncounterArea7b : EncounterArea32
    {
        private const int CatchComboBonus = 1;

        protected override IEnumerable<EncounterSlot> GetMatchFromEvoLevel(PKM pkm, IReadOnlyList<DexLevel> chain, int minLevel)
        {
            var slots = Slots.Where(slot => chain.Any(evo => evo.Species == slot.Species && evo.Form == slot.Form && evo.Level >= (slot.LevelMin - CatchComboBonus)));

            // Get slots where pokemon can exist with respect to level constraints
            return slots.Where(s => s.IsLevelWithinRange(minLevel, minLevel, 0, CatchComboBonus));
        }

        protected override IEnumerable<EncounterSlot> GetFilteredSlots(PKM pkm, IEnumerable<EncounterSlot> slots, int minLevel)
        {
            return slots;
        }
    }
}

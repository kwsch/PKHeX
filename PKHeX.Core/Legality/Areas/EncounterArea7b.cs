using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="GameVersion.GG"/> encounter area
    /// </summary>
    public sealed class EncounterArea7b : EncounterArea32
    {
        private const int CatchComboBonus = 1;

        public override IEnumerable<EncounterSlot> GetMatchingSlots(PKM pkm, IReadOnlyList<EvoCriteria> chain)
        {
            foreach (var slot in Slots)
            {
                foreach (var evo in chain)
                {
                    if (slot.Species != evo.Species)
                        continue;

                    if (!slot.IsLevelWithinRange(evo.MinLevel, evo.Level, CatchComboBonus))
                        break;
                    if (slot.Form != evo.Form)
                        break;

                    yield return slot;
                    break;
                }
            }
        }
    }
}

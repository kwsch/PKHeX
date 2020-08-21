using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="GameVersion.GO"/> encounter area for <see cref="GameVersion.GG"/>
    /// </summary>
    public sealed class EncounterArea7g : EncounterArea32
    {
        public override IEnumerable<EncounterSlot> GetMatchingSlots(PKM pkm, IReadOnlyList<EvoCriteria> chain)
        {
            foreach (var slot in Slots)
            {
                foreach (var evo in chain)
                {
                    if (slot.Species != evo.Species)
                        continue;

                    if (!slot.IsLevelWithinRange(pkm.Met_Level))
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
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="GameVersion.Gen7"/> encounter area
    /// </summary>
    public sealed class EncounterArea7 : EncounterArea32
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

                    if (slot.Form != evo.Form && !Legal.WildChangeFormAfter.Contains(slot.Species))
                    {
                        if (slot.Species != (int)Species.Minior) // Random Color, edge case
                            break;
                    }

                    yield return slot;
                    break;
                }
            }
        }
    }
}

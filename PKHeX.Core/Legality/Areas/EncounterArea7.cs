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
            // Edge Case Handling
            switch (pkm.Species)
            {
                case 744 when pkm.AltForm == 1: // Rockruff Event
                case 745 when pkm.AltForm == 2: // Lycanroc Event
                    yield break;
            }

            foreach (var slot in Slots)
            {
                foreach (var evo in chain)
                {
                    if (slot.Species != evo.Species)
                        continue;

                    if (!slot.IsLevelWithinRange(evo.MinLevel, evo.Level))
                        break;

                    if (slot.Form != evo.Form && !Legal.WildChangeFormAfter.Contains(slot.Species))
                    {
                        if (!Legal.WildForms.Contains(slot.Species))
                            break;
                    }

                    yield return slot;
                }
            }
        }
    }
}

using System.Collections.Generic;
using System.Linq;

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
                        continue;

                    if (slot.Form != evo.Form)
                    {
                        if (!Legal.WildForms.Contains(pkm.Species))
                            continue;

                        var maxLevel = Slots.Where(z => z.Species == evo.Species).Max(z => z.LevelMax);
                        if (maxLevel != pkm.Met_Level)
                            continue;

                        var s = (EncounterSlot7)slot.Clone();
                        s.Form = pkm.AltForm;
                        s.Pressure = true;
                        yield return s;
                        continue;
                    }

                    yield return slot;
                }
            }
        }
    }
}

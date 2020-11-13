using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.Species;

namespace PKHeX.Core
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="GameVersion.GO"/> encounter area for <see cref="GameVersion.GG"/>
    /// </summary>
    public sealed class EncounterArea7g : EncounterArea
    {
        private EncounterArea7g() : base(GameVersion.GO) { }

        internal static EncounterArea7g[] GetArea(HashSet<int> raid15)
        {
            var noForm = Enumerable.Range(1, 150).Concat(Enumerable.Range(808, 2)); // count : 152
            var forms = new[]
            {
                (byte)Rattata,
                (byte)Raticate,
                (byte)Raichu,
                (byte)Sandshrew,
                (byte)Sandslash,
                (byte)Vulpix,
                (byte)Ninetales,
                (byte)Diglett,
                (byte)Dugtrio,
                (byte)Meowth,
                (byte)Persian,
                (byte)Geodude,
                (byte)Graveler,
                (byte)Golem,
                (byte)Grimer,
                (byte)Muk,
                (byte)Exeggutor,
                (byte)Marowak,
            };

            var area = new EncounterArea7g { Location = 50, Type = SlotType.GoPark };
            EncounterSlot7GO GetSlot(EncounterArea7g a, int species, int form)
            {
                var min = raid15.Contains(species | (form << 11)) ? 15 : 1;
                return new EncounterSlot7GO(a, species, form, min, 40);
            }

            var regular = noForm.Select(z => GetSlot(area, z, 0));
            var alolan = forms.Select(z => GetSlot(area, z, 1));
            var slots = regular.Concat(alolan).ToArray();

            area.Slots = slots;
            return new[] { area };
        }

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

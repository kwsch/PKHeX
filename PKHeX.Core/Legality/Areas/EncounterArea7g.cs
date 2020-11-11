using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="GameVersion.GO"/> encounter area for <see cref="GameVersion.GG"/>
    /// </summary>
    public sealed class EncounterArea7g : EncounterArea
    {
        private EncounterArea7g() : base(GameVersion.GO) { }

        internal static EncounterArea7g[] GetArea()
        {
            var area = new EncounterArea7g { Location = 50, Type = SlotType.GoPark };
            static EncounterSlot7GO GetSlot(EncounterArea7g area, int species, int form)
            {
                return new EncounterSlot7GO(area, species, form, 1, 40);
            }

            var obtainable = Enumerable.Range(1, 150).Concat(Enumerable.Range(808, 2)); // count : 152
            var AlolanKanto = new byte[]
            {
                // Level 1+
                019, // Rattata
                020, // Raticate
                027, // Sandshrew
                028, // Sandslash
                037, // Vulpix
                038, // Ninetales
                050, // Diglett
                051, // Dugtrio
                052, // Meowth
                053, // Persian
                074, // Geodude
                075, // Graveler
                076, // Golem
                088, // Grimer
                089, // Muk
                103, // Exeggutor
                105, // Marowak

                // Level 15+
                026, // Raichu
            };

            var regular = obtainable.Select(z => GetSlot(area, z, 0));
            var alolan = AlolanKanto.Select(z => GetSlot(area, z, 1));
            var slots = regular.Concat(alolan).ToArray();

            slots[slots.Length - 1].ClampMinRaid(15); // Raichu
            slots[(int)Species.Mewtwo - 1].ClampMinRaid(15);
            slots[(int)Species.Articuno - 1].ClampMinRaid(15);
            slots[(int)Species.Zapdos - 1].ClampMinRaid(15);
            slots[(int)Species.Moltres - 1].ClampMinRaid(15);

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

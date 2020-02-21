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

        protected override IEnumerable<EncounterSlot> GetMatchFromEvoLevel(PKM pkm, IEnumerable<EvoCriteria> vs, int minLevel)
        {
            var slots = Slots.Where(slot => vs.Any(evo => evo.Species == slot.Species && evo.Level >= (slot.LevelMin - CatchComboBonus)));

            // Get slots where pokemon can exist with respect to level constraints
            return slots.Where(s => s.IsLevelWithinRange(minLevel, minLevel, 0, CatchComboBonus));
        }

        protected override IEnumerable<EncounterSlot> GetFilteredSlots(PKM pkm, IEnumerable<EncounterSlot> slots, int minLevel)
        {
            int species = pkm.Species;

            int form = pkm.AltForm;
            if (Legal.AlolanVariantEvolutions12.Contains(species) || Legal.GalarVariantFormEvolutions.Contains(species)) // match form if same species, else form 0.
            {
                if (pkm.AltForm != 0 && pkm is PB7)
                    yield break; // can't get Alolan forms from wild

                foreach (var slot in slots)
                {
                    if (species == slot.Species ? slot.Form == form : slot.Form == 0)
                        yield return slot;
                }
            }
            else if (form == 0)
            {
                // enforce no form
                foreach (var slot in slots)
                {
                    yield return slot;
                }
            }
        }
    }
}
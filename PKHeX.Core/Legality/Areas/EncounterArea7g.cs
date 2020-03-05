using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="GameVersion.GO"/> encounter area for <see cref="GameVersion.GG"/>
    /// </summary>
    public sealed class EncounterArea7g : EncounterArea32
    {
        protected override IEnumerable<EncounterSlot> GetFilteredSlots(PKM pkm, IEnumerable<EncounterSlot> slots, int minLevel)
        {
            int species = pkm.Species;
            int form = pkm.AltForm;

            if (Legal.AlolanVariantEvolutions12.Contains(species) || Legal.GalarVariantFormEvolutions.Contains(species)) // match form if same species, else form 0.
            {
                foreach (var slot in slots)
                {
                    if (species == slot.Species ? slot.Form == form : slot.Form == 0)
                        yield return slot;
                }
            }
            else if (Legal.AlolanOriginForms.Contains(species)) // match slot form
            {
                foreach (var slot in slots)
                {
                    if (slot.Form == form)
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
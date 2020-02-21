using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="GameVersion.Gen7"/> encounter area
    /// </summary>
    public sealed class EncounterArea7 : EncounterArea32
    {
        protected override IEnumerable<EncounterSlot> GetFilteredSlots(PKM pkm, IEnumerable<EncounterSlot> slots, int minLevel)
        {
            int species = pkm.Species;
            int form = pkm.AltForm;

            // Edge Case Handling
            switch (species)
            {
                case 744 when form == 1: // Rockruff Event
                case 745 when form == 2: // Lycanroc Event
                    yield break;
            }

            EncounterSlot? slotMax = null;
            void CachePressureSlot(EncounterSlot s)
            {
                if (slotMax != null && s.LevelMax > slotMax.LevelMax)
                    slotMax = s;
            }

            if (Legal.AlolanVariantEvolutions12.Contains(species) || Legal.GalarVariantFormEvolutions.Contains(species)) // match form if same species, else form 0.
            {
                foreach (var slot in slots)
                {
                    if (species == slot.Species ? slot.Form == form : slot.Form == 0)
                        yield return slot;
                    CachePressureSlot(slot);
                }
            }
            else if (ShouldMatchSlotForm()) // match slot form
            {
                foreach (var slot in slots)
                {
                    if (slot.Form == form)
                        yield return slot;
                    CachePressureSlot(slot);
                }
            }
            else
            {
                foreach (var slot in slots)
                {
                    yield return slot; // no form checking
                    CachePressureSlot(slot);
                }
            }

            // Filter for Form Specific
            // Pressure Slot
            if (slotMax == null)
                yield break;

            if (Legal.AlolanVariantEvolutions12.Contains(species) || Legal.GalarVariantFormEvolutions.Contains(species)) // match form if same species, else form 0.
            {
                if (species == slotMax.Species ? slotMax.Form == form : slotMax.Form == 0)
                    yield return GetPressureSlot(slotMax, pkm);
            }
            else if (ShouldMatchSlotForm()) // match slot form
            {
                if (slotMax.Form == form)
                    yield return GetPressureSlot(slotMax, pkm);
            }
            else
            {
                yield return GetPressureSlot(slotMax, pkm);
            }

            bool ShouldMatchSlotForm() => Legal.WildForms.Contains(species) || Legal.AlolanOriginForms.Contains(species) || FormConverter.IsTotemForm(species, form, 7);
        }
    }
}
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="GameVersion.XY"/> encounter area
    /// </summary>
    public sealed class EncounterArea6XY : EncounterArea32
    {
        protected override IEnumerable<EncounterSlot> GetFilteredSlots(PKM pkm, IEnumerable<EncounterSlot> slots, int minLevel)
        {
            EncounterSlot? slotMax = null;
            void CachePressureSlot(EncounterSlot s)
            {
                if (slotMax == null || s.LevelMax > slotMax.LevelMax)
                    slotMax = s;
            }

            int species = pkm.Species;
            int form = pkm.AltForm;
            bool ShouldMatchSlotForm() => Legal.WildForms.Contains(species);

            if (ShouldMatchSlotForm()) // match slot form
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

            if (ShouldMatchSlotForm()) // match slot form
            {
                if (slotMax.Form == form)
                    yield return GetPressureSlot(slotMax, pkm);
            }
            else
            {
                yield return GetPressureSlot(slotMax, pkm);
            }
        }

        public static bool WasFriendSafari(PKM pkm)
        {
            if (!pkm.XY)
                return false;
            if (pkm.Met_Location != 148)
                return false;
            if (pkm.Met_Level != 30)
                return false;
            if (pkm.Egg_Location != 0)
                return false;
            return true;
        }

        public static IEnumerable<EncounterSlot> GetValidFriendSafari(PKM pkm)
        {
            var vs = EvolutionChain.GetValidPreEvolutions(pkm);
            return GetValidFriendSafari(vs);
        }

        public static IEnumerable<EncounterSlot> GetValidFriendSafari(IEnumerable<EvoCriteria> vs)
        {
            var evos = vs.Where(d => d.Level >= 30);
            return evos.SelectMany(z => Encounters6.FriendSafari[z.Species]);
        }
    }
}
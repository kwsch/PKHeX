using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="GameVersion.ORAS"/> encounter area
    /// </summary>
    public sealed class EncounterArea6AO : EncounterArea32
    {
        private const int FluteBoostMin = 4; // White Flute decreases levels.
        private const int FluteBoostMax = 4; // Black Flute increases levels.
        private const int DexNavBoost = 30; // Maximum DexNav chain

        protected override IEnumerable<EncounterSlot> GetMatchFromEvoLevel(PKM pkm, IEnumerable<EvoCriteria> vs, int minLevel)
        {
            var slots = Slots.Where(slot => vs.Any(evo => evo.Species == slot.Species && evo.Level >= (slot.LevelMin - FluteBoostMax)));

            // note: it's probably possible to determine a reduced DexNav boost based on the flawless IV count (no flawless = not chained)
            // if someone wants to implement that logic to have the below method return a calculated max DexNavBoost, send a pull request :)
            static int getMaxLevelBoost(EncounterSlot s) => s.Type != SlotType.Rock_Smash ? DexNavBoost : FluteBoostMax; // DexNav encounters most likely

            // Get slots where pokemon can exist with respect to level constraints
            return slots.Where(s => s.IsLevelWithinRange(minLevel, minLevel, FluteBoostMin, getMaxLevelBoost(s)));
        }

        protected override IEnumerable<EncounterSlot> GetFilteredSlots(PKM pkm, IEnumerable<EncounterSlot> slots, int minLevel)
        {
            EncounterSlot? slotMax = null;
            foreach (EncounterSlot s in slots)
            {
                if (Legal.WildForms.Contains(pkm.Species) && s.Form != pkm.AltForm)
                {
                    CachePressureSlot(s);
                    continue;
                }
                bool nav = s.Permissions.AllowDexNav && (pkm.RelearnMove1 != 0 || pkm.AbilityNumber == 4);
                EncounterSlot slot = s.Clone();
                slot.Permissions.DexNav = nav;

                if (slot.LevelMin > minLevel)
                    slot.Permissions.WhiteFlute = true;
                if (slot.LevelMax + 1 <= minLevel && minLevel <= slot.LevelMax + FluteBoostMax)
                    slot.Permissions.BlackFlute = true;
                if (slot.LevelMax != minLevel && slot.Permissions.AllowDexNav)
                    slot.Permissions.DexNav = true;
                yield return slot;

                CachePressureSlot(slot);
            }

            void CachePressureSlot(EncounterSlot s)
            {
                if (slotMax != null && s.LevelMax > slotMax.LevelMax)
                    slotMax = s;
            }
            // Pressure Slot
            if (slotMax != null)
                yield return GetPressureSlot(slotMax, pkm);
        }
    }
}
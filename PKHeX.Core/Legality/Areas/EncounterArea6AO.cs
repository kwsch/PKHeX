using System;
using System.Collections.Generic;

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

        public override IEnumerable<EncounterSlot> GetMatchingSlots(PKM pkm, IReadOnlyList<EvoCriteria> chain)
        {
            foreach (var slot in Slots)
            {
                foreach (var evo in chain)
                {
                    if (slot.Species != evo.Species)
                        continue;

                    var boostMax = slot.Type != SlotType.Rock_Smash ? DexNavBoost : FluteBoostMax;
                    const int boostMin = FluteBoostMin;
                    if (!slot.IsLevelWithinRange(evo.MinLevel, evo.Level, boostMin, boostMax))
                        break;

                    if (slot.Form != evo.Form)
                    {
                        if (!Legal.WildForms.Contains(slot.Species))
                            break;

                        if (!ExistsPressureSlot(evo, out var maxLevel))
                            break;

                        if (maxLevel != pkm.Met_Level)
                            break;

                        var clone = (EncounterSlot6AO)slot.Clone();
                        clone.Form = pkm.AltForm;
                        clone.Pressure = true;
                        MarkSlotDetails(pkm, clone, evo);
                        yield return clone;
                    }
                    else
                    {
                        var clone = (EncounterSlot6AO)slot.Clone();
                        MarkSlotDetails(pkm, clone, evo);
                        yield return clone;
                    }
                    break;
                }
            }
        }

        private bool ExistsPressureSlot(DexLevel evo, out int maxLevel)
        {
            maxLevel = 0;
            bool existsForm = false;
            foreach (var z in Slots)
            {
                if (z.Species != evo.Species)
                    continue;
                if (z.Form == evo.Form)
                    continue;
                maxLevel = Math.Max(maxLevel, z.LevelMax);
                existsForm = true;
            }
            return existsForm;
        }

        private static void MarkSlotDetails(PKM pkm, EncounterSlot6AO clone, EvoCriteria evo)
        {
            bool nav = clone.AllowDexNav && (pkm.RelearnMove1 != 0 || pkm.AbilityNumber == 4);
            clone.DexNav = nav;

            if (clone.LevelMin > evo.MinLevel)
                clone.WhiteFlute = true;
            if (clone.LevelMax + 1 <= evo.MinLevel && evo.MinLevel <= clone.LevelMax + FluteBoostMax)
                clone.BlackFlute = true;
            if (clone.LevelMax != evo.MinLevel && clone.AllowDexNav)
                clone.DexNav = true;
        }
    }
}
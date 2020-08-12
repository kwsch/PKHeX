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
                        continue;

                    if (slot.Form != evo.Form)
                    {
                        if (!Legal.WildForms.Contains(pkm.Species))
                            continue;

                        var maxLevel = Slots.Where(z => z.Species == evo.Species).Max(z => z.LevelMax);
                        if (maxLevel != pkm.Met_Level)
                            continue;

                        var s = (EncounterSlot6AO)slot.Clone();
                        s.Form = pkm.AltForm;
                        s.Pressure = true;
                        MarkSlotDetails(pkm, (EncounterSlot6AO)slot, s, evo);
                        yield return s;
                    }
                    else
                    {
                        var s = (EncounterSlot6AO)slot.Clone();
                        MarkSlotDetails(pkm, (EncounterSlot6AO)slot, s, evo);
                        yield return s;
                    }
                }
            }
        }

        private static void MarkSlotDetails(PKM pkm, EncounterSlot6AO original, EncounterSlot6AO clone, EvoCriteria evo)
        {
            bool nav = original.AllowDexNav && (pkm.RelearnMove1 != 0 || pkm.AbilityNumber == 4);
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
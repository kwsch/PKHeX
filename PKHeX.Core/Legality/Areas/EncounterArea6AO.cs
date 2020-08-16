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

        private const int RandomForm = 31;

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

                    if (slot.Form != evo.Form && slot.Form != RandomForm)
                        break;

                    var clone = (EncounterSlot6AO)slot.Clone();
                    MarkSlotDetails(pkm, clone, evo);
                    yield return clone;
                    break;
                }
            }
        }

        private static void MarkSlotDetails(PKM pkm, EncounterSlot6AO slot, EvoCriteria evo)
        {
            bool nav = slot.AllowDexNav && (pkm.RelearnMove1 != 0 || pkm.AbilityNumber == 4);
            slot.DexNav = nav;

            if (slot.LevelMin > evo.MinLevel)
                slot.WhiteFlute = true;
            if (slot.LevelMax + 1 <= evo.MinLevel && evo.MinLevel <= slot.LevelMax + FluteBoostMax)
                slot.BlackFlute = true;
            if (slot.LevelMax != evo.MinLevel && slot.AllowDexNav)
                slot.DexNav = true;
        }
    }
}

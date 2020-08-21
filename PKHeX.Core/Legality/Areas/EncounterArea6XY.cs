using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="GameVersion.XY"/> encounter area
    /// </summary>
    public sealed class EncounterArea6XY : EncounterArea32
    {
        private const int RandomForm = 31;
        private const int RandomFormVivillon = RandomForm - 1;

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

                    if (slot.Form != evo.Form && slot.Form < RandomFormVivillon && !Legal.WildChangeFormAfter.Contains(slot.Species))
                    {
                        if (slot.Species != (int)Species.Flabébé)
                            break;

                        var maxLevel = slot.LevelMax;
                        if (!ExistsPressureSlot(evo, ref maxLevel))
                            break;

                        if (maxLevel != pkm.Met_Level)
                            break;

                        var clone = (EncounterSlot6XY)slot.Clone();
                        clone.Form = evo.Form;
                        clone.Pressure = true;
                        yield return clone;
                        break;
                    }

                    yield return slot;
                    break;
                }
            }
        }

        private bool ExistsPressureSlot(DexLevel evo, ref int level)
        {
            bool existsForm = false;
            foreach (var z in Slots)
            {
                if (z.Species != evo.Species)
                    continue;
                if (z.Form == evo.Form)
                    continue;
                if (z.LevelMax < level)
                    continue;
                level = z.LevelMax;
                existsForm = true;
            }
            return existsForm;
        }
    }
}

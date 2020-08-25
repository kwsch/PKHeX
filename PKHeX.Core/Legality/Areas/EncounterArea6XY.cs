using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="GameVersion.XY"/> encounter area
    /// </summary>
    public sealed class EncounterArea6XY : EncounterArea32
    {
        public static EncounterArea6XY[] GetAreas(byte[][] input, GameVersion game)
        {
            var result = new EncounterArea6XY[input.Length];
            for (int i = 0; i < input.Length; i++)
                result[i] = new EncounterArea6XY(input[i], game);
            return result;
        }

        private EncounterArea6XY(byte[] data, GameVersion game)
        {
            Location = data[0] | (data[1] << 8);
            Type = (SlotType)data[2];

            Slots = ReadSlots(data, game);
        }

        private EncounterSlot6XY[] ReadSlots(byte[] data, GameVersion game)
        {
            const int size = 4;
            int count = (data.Length - 4) / size;
            var slots = new EncounterSlot6XY[count];
            for (int i = 0; i < slots.Length; i++)
            {
                int offset = 4 + (size * i);
                ushort SpecForm = BitConverter.ToUInt16(data, offset);
                int species = SpecForm & 0x3FF;
                int form = SpecForm >> 11;
                int min = data[offset + 2];
                int max = data[offset + 3];
                slots[i] = new EncounterSlot6XY(this, species, form, min, max, game);
            }

            return slots;
        }

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

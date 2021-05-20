using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <inheritdoc cref="EncounterArea" />
    /// <summary>
    /// <see cref="GameVersion.Gen5"/> encounter area
    /// </summary>
    public sealed record EncounterArea5 : EncounterArea
    {
        public static EncounterArea5[] GetAreas(byte[][] input, GameVersion game)
        {
            var result = new EncounterArea5[input.Length];
            for (int i = 0; i < input.Length; i++)
                result[i] = new EncounterArea5(input[i], game);
            return result;
        }

        private EncounterArea5(byte[] data, GameVersion game) : base(game)
        {
            Location = data[0] | (data[1] << 8);
            Type = (SlotType)data[2];

            Slots = ReadSlots(data);
        }

        private EncounterSlot5[] ReadSlots(byte[] data)
        {
            const int size = 4;
            int count = (data.Length - 4) / size;
            var slots = new EncounterSlot5[count];
            for (int i = 0; i < slots.Length; i++)
            {
                int offset = 4 + (size * i);
                ushort SpecForm = BitConverter.ToUInt16(data, offset);
                int species = SpecForm & 0x3FF;
                int form = SpecForm >> 11;
                int min = data[offset + 2];
                int max = data[offset + 3];
                slots[i] = new EncounterSlot5(this, species, form, min, max);
            }

            return slots;
        }

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

                    if (slot.Form != evo.Form && !FormInfo.WildChangeFormAfter.Contains(slot.Species))
                        break;

                    yield return slot;
                    break;
                }
            }
        }
    }
}
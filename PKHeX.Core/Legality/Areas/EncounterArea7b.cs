using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <inheritdoc cref="EncounterArea" />
    /// <summary>
    /// <see cref="GameVersion.GG"/> encounter area
    /// </summary>
    public sealed record EncounterArea7b : EncounterArea
    {
        public static EncounterArea7b[] GetAreas(byte[][] input, GameVersion game)
        {
            var result = new EncounterArea7b[input.Length];
            for (int i = 0; i < input.Length; i++)
                result[i] = new EncounterArea7b(input[i], game);
            return result;
        }

        private EncounterArea7b(byte[] data, GameVersion game) : base(game)
        {
            Location = data[0] | (data[1] << 8);
            Slots = ReadSlots(data);
        }

        private EncounterSlot7b[] ReadSlots(byte[] data)
        {
            const int size = 4;
            int count = (data.Length - 2) / size;
            var slots = new EncounterSlot7b[count];
            for (int i = 0; i < slots.Length; i++)
            {
                int offset = 2 + (size * i);
                ushort SpecForm = BitConverter.ToUInt16(data, offset);
                int species = SpecForm & 0x3FF;
                int min = data[offset + 2];
                int max = data[offset + 3];
                slots[i] = new EncounterSlot7b(this, species, min, max);
            }

            return slots;
        }

        private const int CatchComboBonus = 1;

        public override IEnumerable<EncounterSlot> GetMatchingSlots(PKM pkm, IReadOnlyList<EvoCriteria> chain)
        {
            foreach (var slot in Slots)
            {
                foreach (var evo in chain)
                {
                    if (slot.Species != evo.Species)
                        continue;

                    var met = pkm.Met_Level;
                    if (!slot.IsLevelWithinRange(met, 0, CatchComboBonus))
                        break;
                    if (slot.Form != evo.Form)
                        break;

                    yield return slot;
                    break;
                }
            }
        }
    }
}

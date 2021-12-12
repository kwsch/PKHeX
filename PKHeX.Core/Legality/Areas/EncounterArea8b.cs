using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <inheritdoc cref="EncounterArea" />
    /// <summary>
    /// <see cref="GameVersion.BDSP"/> encounter area
    /// </summary>
    public sealed record EncounterArea8b : EncounterArea
    {
        public readonly EncounterSlot8b[] Slots;

        protected override IReadOnlyList<EncounterSlot> Raw => Slots;

        public static EncounterArea8b[] GetAreas(byte[][] input, GameVersion game)
        {
            var result = new EncounterArea8b[input.Length];
            for (int i = 0; i < input.Length; i++)
                result[i] = new EncounterArea8b(input[i], game);
            return result;
        }

        private EncounterArea8b(byte[] data, GameVersion game) : base(game)
        {
            Location = data[0] | (data[1] << 8);
            Type = (SlotType)data[2];

            Slots = ReadSlots(data);
        }

        private EncounterSlot8b[] ReadSlots(byte[] data)
        {
            const int size = 4;
            int count = (data.Length - 4) / size;
            var slots = new EncounterSlot8b[count];
            for (int i = 0; i < slots.Length; i++)
            {
                int offset = 4 + (size * i);
                ushort SpecForm = BitConverter.ToUInt16(data, offset);
                int species = SpecForm & 0x3FF;
                int form = SpecForm >> 11;
                int min = data[offset + 2];
                int max = data[offset + 3];
                slots[i] = new EncounterSlot8b(this, species, form, min, max);
            }
            return slots;
        }

        public override bool IsMatchLocation(int location)
        {
            if (base.IsMatchLocation(location))
                return true;
            return CanCrossoverTo(location);
        }

        private bool CanCrossoverTo(int location)
        {
            if (Type is SlotType.Surf)
            {
                return Location switch
                {
                    486 => location is 167, // Route 223 -> Pokémon League
                    167 => location is 486, // Pokémon League -> Route 223
                    420 => location is 489, // Route 229 -> Route 230
                    489 => location is 420, // Route 230 -> Route 229

                    // All other crossover surf locations are identical slot lists.
                    _ => false,
                };
            }

            // No crossover
            return false;
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

                    if (slot.Form != evo.Form && slot.Species is not (int)Species.Burmy)
                        break;

                    yield return slot;
                    break;
                }
            }
        }
    }
}

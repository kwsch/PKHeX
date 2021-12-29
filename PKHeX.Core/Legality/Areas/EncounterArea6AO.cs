using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core
{
    /// <inheritdoc cref="EncounterArea" />
    /// <summary>
    /// <see cref="GameVersion.ORAS"/> encounter area
    /// </summary>
    public sealed record EncounterArea6AO : EncounterArea
    {
        public readonly EncounterSlot6AO[] Slots;

        protected override IReadOnlyList<EncounterSlot> Raw => Slots;

        public static EncounterArea6AO[] GetAreas(byte[][] input, GameVersion game)
        {
            var result = new EncounterArea6AO[input.Length];
            for (int i = 0; i < input.Length; i++)
                result[i] = new EncounterArea6AO(input[i], game);
            return result;
        }

        private EncounterArea6AO(ReadOnlySpan<byte> data, GameVersion game) : base(game)
        {
            Location = ReadInt16LittleEndian(data);
            Type = (SlotType)data[2];

            Slots = ReadSlots(data);
        }

        private EncounterSlot6AO[] ReadSlots(ReadOnlySpan<byte> data)
        {
            const int size = 4;
            int count = (data.Length - 4) / size;
            var slots = new EncounterSlot6AO[count];
            for (int i = 0; i < slots.Length; i++)
            {
                int offset = 4 + (size * i);
                var entry = data.Slice(offset, size);
                slots[i] = ReadSlot(entry);
            }

            return slots;
        }

        private EncounterSlot6AO ReadSlot(ReadOnlySpan<byte> entry)
        {
            ushort SpecForm = ReadUInt16LittleEndian(entry);
            int species = SpecForm & 0x3FF;
            int form = SpecForm >> 11;
            int min = entry[2];
            int max = entry[3];
            return new EncounterSlot6AO(this, species, form, min, max);
        }

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

                    var boostMax = Type != SlotType.Rock_Smash ? DexNavBoost : FluteBoostMax;
                    const int boostMin = FluteBoostMin;
                    if (!slot.IsLevelWithinRange(pkm.Met_Level, boostMin, boostMax))
                        break;

                    if (slot.Form != evo.Form && !slot.IsRandomUnspecificForm)
                        break;

                    // Track some metadata about how this slot was matched.
                    var clone = slot with
                    {
                        WhiteFlute = evo.MinLevel < slot.LevelMin,
                        BlackFlute = evo.MinLevel > slot.LevelMax && evo.MinLevel <= slot.LevelMax + FluteBoostMax,
                        DexNav = slot.CanDexNav && (evo.MinLevel != slot.LevelMax || pkm.RelearnMove1 != 0 || pkm.AbilityNumber == 4),
                    };
                    yield return clone;
                    break;
                }
            }
        }
    }
}

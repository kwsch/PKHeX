using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <inheritdoc cref="EncounterArea" />
    /// <summary>
    /// <see cref="GameVersion.Gen3"/> encounter area
    /// </summary>
    public sealed record EncounterArea3 : EncounterArea
    {
        public readonly int Rate;

        public static EncounterArea3[] GetAreas(byte[][] input, GameVersion game)
        {
            var result = new EncounterArea3[input.Length];
            for (int i = 0; i < input.Length; i++)
                result[i] = new EncounterArea3(input[i], game);
            return result;
        }

        public static EncounterArea3[] GetAreasSwarm(byte[][] input, GameVersion game)
        {
            var result = new EncounterArea3[input.Length];
            for (int i = 0; i < input.Length; i++)
                result[i] = new EncounterArea3(input[i], game, SlotType.Swarm | SlotType.Grass);
            return result;
        }

        private EncounterArea3(byte[] data, GameVersion game) : base(game)
        {
            Location = data[0] | (data[1] << 8);
            Type = (SlotType)data[2];
            Rate = data[3];

            Slots = ReadRegularSlots(data);
        }

        private EncounterArea3(byte[] data, GameVersion game, SlotType type) : base(game)
        {
            Location = data[0] | (data[1] << 8);
            Type = type;
            Rate = data[3];

            Slots = ReadSwarmSlots(data);
        }

        private EncounterSlot3[] ReadRegularSlots(byte[] data)
        {
            const int size = 10;
            int count = (data.Length - 4) / size;
            var slots = new EncounterSlot3[count];
            for (int i = 0; i < slots.Length; i++)
            {
                int offset = 4 + (size * i);

                int species = BitConverter.ToUInt16(data, offset + 0);
                int form = data[offset + 2];
                int slotNum = data[offset + 3];
                int min = data[offset + 4];
                int max = data[offset + 5];

                int mpi = data[offset + 6];
                int mpc = data[offset + 7];
                int sti = data[offset + 8];
                int stc = data[offset + 9];
                slots[i] = new EncounterSlot3(this, species, form, min, max, slotNum, mpi, mpc, sti, stc);
            }

            return slots;
        }

        private EncounterSlot3[] ReadSwarmSlots(byte[] data)
        {
            const int size = 14;
            int count = (data.Length - 4) / size;
            var slots = new EncounterSlot3[count];
            for (int i = 0; i < slots.Length; i++)
            {
                int offset = 4 + (size * i);

                int species = BitConverter.ToUInt16(data, offset + 0);
                // form always 0
                int slotNum = data[offset + 3];
                int min = data[offset + 4];
                int max = data[offset + 5];

                int[] moves =
                {
                    BitConverter.ToUInt16(data, offset + 6),
                    BitConverter.ToUInt16(data, offset + 8),
                    BitConverter.ToUInt16(data, offset + 10),
                    BitConverter.ToUInt16(data, offset + 12),
                };

                slots[i] = new EncounterSlot3Swarm(this, species, min, max, slotNum, moves);
            }

            return slots;
        }

        public override IEnumerable<EncounterSlot> GetMatchingSlots(PKM pkm, IReadOnlyList<EvoCriteria> chain)
        {
            if (pkm.Format != 3) // Met Location and Met Level are changed on PK3->PK4
                return GetSlotsFuzzy(chain);
            if (pkm.Met_Location != Location)
                return Array.Empty<EncounterSlot>();
            return GetSlotsMatching(chain, pkm.Met_Level);
        }

        private IEnumerable<EncounterSlot> GetSlotsMatching(IReadOnlyList<EvoCriteria> chain, int lvl)
        {
            foreach (var slot in Slots)
            {
                foreach (var evo in chain)
                {
                    if (slot.Species != evo.Species)
                        continue;

                    if (slot.Form != evo.Form)
                        break;
                    if (!slot.IsLevelWithinRange(lvl))
                        break;

                    yield return slot;
                    break;
                }
            }
        }

        private IEnumerable<EncounterSlot> GetSlotsFuzzy(IReadOnlyList<EvoCriteria> chain)
        {
            foreach (var slot in Slots)
            {
                foreach (var evo in chain)
                {
                    if (slot.Species != evo.Species)
                        continue;

                    if (slot.Form != evo.Form)
                        break;
                    if (slot.LevelMin > evo.Level)
                        break;

                    yield return slot;
                    break;
                }
            }
        }
    }
}
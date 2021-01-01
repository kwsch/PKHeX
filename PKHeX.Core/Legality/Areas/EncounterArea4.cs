using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <inheritdoc cref="EncounterArea" />
    /// <summary>
    /// <see cref="GameVersion.Gen4"/> encounter area
    /// </summary>
    public sealed record EncounterArea4 : EncounterArea
    {
        public readonly EncounterType TypeEncounter;
        public readonly int Rate;

        public static EncounterArea4[] GetAreas(byte[][] input, GameVersion game)
        {
            var result = new EncounterArea4[input.Length];
            for (int i = 0; i < input.Length; i++)
                result[i] = new EncounterArea4(input[i], game);
            return result;
        }

        private EncounterArea4(byte[] data, GameVersion game) : base(game)
        {
            Location = data[0] | (data[1] << 8);
            Type = (SlotType)data[2];
            Rate = data[3];
            TypeEncounter = (EncounterType) BitConverter.ToUInt16(data, 4);

            Slots = ReadRegularSlots(data);
        }

        private EncounterSlot4[] ReadRegularSlots(byte[] data)
        {
            const int size = 10;
            int count = (data.Length - 6) / size;
            var slots = new EncounterSlot4[count];
            for (int i = 0; i < slots.Length; i++)
            {
                int offset = 6 + (size * i);

                int species = BitConverter.ToUInt16(data, offset + 0);
                int form = data[offset + 2];
                int slotNum = data[offset + 3];
                int min = data[offset + 4];
                int max = data[offset + 5];

                int mpi = data[offset + 6];
                int mpc = data[offset + 7];
                int sti = data[offset + 8];
                int stc = data[offset + 9];
                slots[i] = new EncounterSlot4(this, species, form, min, max, slotNum, mpi, mpc, sti, stc);
            }

            return slots;
        }

        public override IEnumerable<EncounterSlot> GetMatchingSlots(PKM pkm, IReadOnlyList<EvoCriteria> chain)
        {
            if (pkm.Format != 4) // Met Location and Met Level are changed on PK4->PK5
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

                    if (slot.Form != evo.Form && !FormInfo.WildChangeFormAfter.Contains(slot.Species) && slot.Species != (int)Species.Unown)
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

                    if (slot.Form != evo.Form && !FormInfo.WildChangeFormAfter.Contains(slot.Species) && slot.Species != (int)Species.Unown)
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
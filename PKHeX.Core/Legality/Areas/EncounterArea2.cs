using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <inheritdoc cref="EncounterArea" />
    /// <summary>
    /// <see cref="GameVersion.GSC"/> encounter area
    /// </summary>
    public sealed record EncounterArea2 : EncounterArea
    {
        private static readonly byte[] BCC_SlotRates = { 20, 20, 10, 10, 05, 05, 10, 10, 05, 05 };
        private static readonly byte[] RatesGrass = { 30, 30, 20, 10, 5, 4, 1 };
        private static readonly byte[] RatesSurf = { 60, 30, 10 };

        internal readonly EncounterTime Time;
        public readonly int Rate;
        public readonly IReadOnlyList<byte> Rates;

        public static EncounterArea2[] GetAreas(byte[][] input, GameVersion game)
        {
            var result = new EncounterArea2[input.Length];
            for (int i = 0; i < input.Length; i++)
                result[i] = new EncounterArea2(input[i], game);
            return result;
        }

        private EncounterArea2(byte[] data, GameVersion game) : base(game)
        {
            Location = data[0];
            Time = (EncounterTime)data[1];
            var type = (Type = (SlotType)data[2]) & (SlotType)0xF;
            var rate = data[3];

            if (type > SlotType.Surf) // Not Grass/Surf
            {
                const int size = 5;
                int count = (data.Length - 4) / size;

                var rates = new byte[count];
                for (int i = 0; i < rates.Length; i++)
                    rates[i] = data[4 + i];

                Rates = rates;
                Slots = ReadSlots(data, count, 4 + count);
            }
            else
            {
                Rate = rate;

                const int size = 4;
                int count = (data.Length - 4) / size;
                Rates = type switch
                {
                    SlotType.BugContest => BCC_SlotRates,
                    SlotType.Grass => RatesGrass,
                    _ => RatesSurf
                };
                Slots = ReadSlots(data, count, 4);
            }
        }

        private EncounterSlot2[] ReadSlots(byte[] data, int count, int start)
        {
            var slots = new EncounterSlot2[count];
            for (int i = 0; i < slots.Length; i++)
            {
                int offset = start + (4 * i);
                int species = data[offset + 0];
                int slotNum = data[offset + 1];
                int min = data[offset + 2];
                int max = data[offset + 3];
                slots[i] = new EncounterSlot2(this, species, min, max, slotNum);
            }

            return slots;
        }

        public override IEnumerable<EncounterSlot> GetMatchingSlots(PKM pkm, IReadOnlyList<EvoCriteria> chain)
        {
            if (pkm is not ICaughtData2 pk2 || pk2.CaughtData == 0)
                return GetSlotsFuzzy(chain);

            if (pk2.Met_Location != Location)
                return Array.Empty<EncounterSlot>();
            return GetSlotsSpecificLevelTime(chain, pk2.Met_TimeOfDay, pk2.Met_Level);
        }

        private IEnumerable<EncounterSlot> GetSlotsSpecificLevelTime(IReadOnlyList<EvoCriteria> chain, int time, int lvl)
        {
            foreach (var slot in Slots)
            {
                foreach (var evo in chain)
                {
                    if (slot.Species != evo.Species)
                        continue;

                    if (slot.Form != evo.Form)
                    {
                        if (slot.Species != (int)Species.Unown || evo.Form >= 26) // Don't yield !? forms
                            break;
                    }

                    if (!slot.IsLevelWithinRange(lvl))
                        break;

                    if (!Time.Contains(time))
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
                    {
                        if (slot.Species != (int) Species.Unown || evo.Form >= 26) // Don't yield !? forms
                            break;
                    }
                    if (slot.LevelMin > evo.Level)
                        break;

                    yield return slot;
                    break;
                }
            }
        }
    }
}

using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <inheritdoc cref="EncounterArea" />
    /// <summary>
    /// <see cref="GameVersion.RBY"/> encounter area
    /// </summary>
    public sealed record EncounterArea1 : EncounterArea
    {
        public readonly int Rate;

        public static EncounterArea1[] GetAreas(byte[][] input, GameVersion game)
        {
            var result = new EncounterArea1[input.Length];
            for (int i = 0; i < input.Length; i++)
                result[i] = new EncounterArea1(input[i], game);
            return result;
        }

        private EncounterArea1(byte[] data, GameVersion game) : base(game)
        {
            Location = data[0];
            // 1 byte unused
            Type = (SlotType)data[2];
            Rate = data[3];

            int count = (data.Length - 4) / 4;
            var slots = new EncounterSlot1[count];
            for (int i = 0; i < slots.Length; i++)
            {
                int offset = 4 + (4 * i);
                int species = data[offset + 0];
                int slotNum = data[offset + 1];
                int min = data[offset + 2];
                int max = data[offset + 3];
                slots[i] = new EncounterSlot1(this, species, min, max, slotNum);
            }
            Slots = slots;
        }

        public override IEnumerable<EncounterSlot> GetMatchingSlots(PKM pkm, IReadOnlyList<EvoCriteria> chain)
        {
            int rate = pkm is PK1 {Gen1_NotTradeback: true} pk1 ? pk1.Catch_Rate : -1;
            foreach (var slot in Slots)
            {
                foreach (var evo in chain)
                {
                    if (slot.Species != evo.Species)
                        continue;

                    if (slot.LevelMin > evo.Level)
                        break;
                    if (slot.Form != evo.Form)
                        break;

                    if (rate != -1)
                    {
                        var expect = (slot.Version == GameVersion.YW ? PersonalTable.Y : PersonalTable.RB)[slot.Species].CatchRate;
                        if (expect != rate)
                            break;
                    }
                    yield return slot;
                    break;
                }
            }
        }
    }
}

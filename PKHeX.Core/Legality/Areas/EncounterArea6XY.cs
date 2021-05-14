using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <inheritdoc cref="EncounterArea" />
    /// <summary>
    /// <see cref="GameVersion.XY"/> encounter area
    /// </summary>
    public sealed record EncounterArea6XY : EncounterArea
    {
        public static EncounterArea6XY[] GetAreas(byte[][] input, GameVersion game, EncounterArea6XY safari)
        {
            var result = new EncounterArea6XY[input.Length + 1];
            for (int i = 0; i < input.Length; i++)
                result[i] = new EncounterArea6XY(input[i], game);
            result[^1] = safari;
            return result;
        }

        public EncounterArea6XY() : base(GameVersion.XY)
        {
            Location = 148; // Friend Safari
            Type = SlotType.FriendSafari;

            Slots = LoadSafariSlots();
        }

        private EncounterArea6XY(byte[] data, GameVersion game) : base(game)
        {
            Location = data[0] | (data[1] << 8);
            Type = (SlotType)data[2];

            Slots = ReadSlots(data);
        }

        private EncounterSlot6XY[] LoadSafariSlots()
        {
            const int SpeciesFormSlots = 4;

            // Single form species
            ushort[] species =
            {
                002, 005, 008, 012, 014, 016, 021, 025, 027, 035,
                038, 039, 043, 044, 046, 049, 049, 051, 056, 058,
                061, 063, 067, 077, 082, 083, 084, 087, 089, 091,
                095, 096, 098, 101, 105, 112, 113, 114, 125, 126,
                127, 130, 131, 132, 133, 148, 163, 165, 168, 175,
                178, 184, 190, 191, 194, 195, 202, 203, 205, 206,
                209, 213, 214, 215, 215, 216, 218, 219, 221, 222,
                224, 225, 227, 231, 235, 236, 247, 262, 267, 268,
                274, 281, 284, 286, 290, 294, 297, 299, 302, 303,
                303, 307, 310, 313, 314, 317, 323, 326, 328, 332,
                336, 342, 352, 353, 356, 357, 359, 361, 363, 372,
                375, 400, 404, 415, 417, 419, 423, 426, 437, 442,
                444, 447, 452, 454, 459, 506, 510, 511, 513, 515,
                517, 520, 523, 525, 527, 530, 531, 536, 538, 539,
                541, 544, 548, 551, 556, 557, 561, 569, 572, 575,
                578, 581, 586, 587, 596, 597, 600, 608, 611, 614,
                618, 619, 621, 623, 624, 627, 629, 636, 651, 654,
                657, 660, 662, 662, 668, 673, 674, 677, 682, 684,
                686, 689, 694, 701, 702, 702, 705, 707, 708, 710,
                712, 714
            };

            var slots = new EncounterSlot6XY[species.Length + SpeciesFormSlots];
            int i = 0;
            for (; i < species.Length; i++)
                slots[i] = new EncounterSlot6XY(this, species[i], 0, 30, 30);

            // Floette has 3 separate forms (RBY)
            slots[i++] = new EncounterSlot6XY(this, (int)Species.Floette, 0, 30, 30);
            slots[i++] = new EncounterSlot6XY(this, (int)Species.Floette, 1, 30, 30);
            slots[i++] = new EncounterSlot6XY(this, (int)Species.Floette, 3, 30, 30);

            // Region Random Vivillon
            slots[i] = new EncounterSlot6XY(this, (int)Species.Vivillon, 30, 30, 30);
            return slots;
        }

        private EncounterSlot6XY[] ReadSlots(byte[] data)
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
                slots[i] = new EncounterSlot6XY(this, species, form, min, max);
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

                    if (slot.Form != evo.Form && slot.Form < RandomFormVivillon && !FormInfo.WildChangeFormAfter.Contains(slot.Species))
                    {
                        if (slot.Species != (int)Species.Flabébé)
                            break;

                        var maxLevel = slot.LevelMax;
                        if (!ExistsPressureSlot(evo, ref maxLevel))
                            break;

                        if (maxLevel != pkm.Met_Level)
                            break;

                        yield return ((EncounterSlot6XY)slot).CreatePressureFormCopy(evo.Form);
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

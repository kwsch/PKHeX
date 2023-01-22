using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <inheritdoc cref="EncounterArea" />
/// <summary>
/// <see cref="GameVersion.SV"/> encounter area
/// </summary>
public sealed record EncounterArea9 : EncounterArea
{
    public readonly EncounterSlot9[] Slots;

    public ushort CrossFrom { get; }

    protected override IReadOnlyList<EncounterSlot9> Raw => Slots;

    public static EncounterArea9[] GetAreas(BinLinkerAccessor input, GameVersion game)
    {
        var result = new EncounterArea9[input.Length];
        for (int i = 0; i < result.Length; i++)
            result[i] = new EncounterArea9(input[i], game);
        return result;
    }

    private EncounterArea9(ReadOnlySpan<byte> areaData, GameVersion game) : base(game)
    {
        Location = areaData[0];
        CrossFrom = areaData[2];
        Slots = ReadSlots(areaData[4..]);
    }

    private EncounterSlot9[] ReadSlots(ReadOnlySpan<byte> areaData)
    {
        const int size = 8;
        var result = new EncounterSlot9[areaData.Length / size];
        for (int i = 0; i < result.Length; i++)
        {
            var slot = areaData[(i * size)..];
            var species = ReadUInt16LittleEndian(slot);
            var form = slot[2];
            var gender = (sbyte)slot[3];

            var min = slot[4];
            var max = slot[5];
            var time = slot[6];

            result[i] = new EncounterSlot9(this, species, form, min, max, gender, time);
        }
        return result;
    }

    public override IEnumerable<EncounterSlot9> GetMatchingSlots(PKM pk, EvoCriteria[] chain)
    {
        var lvl = pk.Met_Level;
        foreach (var slot in Slots)
        {
            foreach (var evo in chain)
            {
                if (slot.Species != evo.Species)
                    continue;
                if (slot.Form != evo.Form && slot.Species is not ((int)Species.Rotom or (int)Species.Deerling or (int)Species.Sawsbuck or (int)Species.Oricorio))
                    break;
                if (slot.Gender != -1 && pk.Gender != slot.Gender)
                    break;
                if (!slot.IsLevelWithinRange(lvl))
                    break;

                if (pk is ITeraType t)
                {
                    var orig = (byte)t.TeraTypeOriginal;
                    var pi = PersonalTable.SV[slot.Species, slot.Form];
                    if (pi.Type1 != orig && pi.Type2 != orig)
                        break;
                }

                yield return slot;
                break;
            }
        }
    }
}

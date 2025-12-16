using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// <see cref="GameVersion.ZA"/> encounter area
/// </summary>
public sealed record EncounterArea9a : IEncounterArea<EncounterSlot9a>, IAreaLocation
{
    public const ushort LocationHyperspace = 273;

    public EncounterSlot9a[] Slots { get; }

    public readonly ushort Location;
    public readonly SlotType9a Type;
    public bool IsMatchLocation(ushort location) => Location == location; // no crossovers!

    public static EncounterArea9a[] GetAreas(BinLinkerAccessor16 input, SlotType9a type)
    {
        var result = new EncounterArea9a[input.Length];
        for (int i = 0; i < result.Length; i++)
            result[i] = new EncounterArea9a(input[i], type);
        return result;
    }

    private EncounterArea9a(ReadOnlySpan<byte> areaData, SlotType9a type)
    {
        Location = ReadUInt16LittleEndian(areaData);
        Type = type;
        // 2..3 reserved
        Slots = ReadSlots(areaData[4..]);
    }

    private EncounterSlot9a[] ReadSlots(ReadOnlySpan<byte> areaData)
    {
        const int size = 8;
        var result = new EncounterSlot9a[areaData.Length / size];
        for (int i = 0; i < result.Length; i++)
        {
            var entry = areaData[(i * size)..];
            var shiny = (Shiny)entry[7];
            var alpha = entry[6] == 1;
            var max = entry[5];
            var min = entry[4];
            var gender = entry[3];
            var form = entry[2];
            var species = ReadUInt16LittleEndian(entry);

            result[i] = new EncounterSlot9a(this, species, form, min, max, alpha, gender, shiny);
        }
        return result;
    }
}

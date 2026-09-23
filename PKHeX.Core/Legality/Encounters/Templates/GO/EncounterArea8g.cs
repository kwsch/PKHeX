using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// <see cref="GameVersion.GO"/> encounter area for direct-to-HOME transfers.
/// </summary>
public sealed record EncounterArea8g : ISpeciesForm
{
    /// <summary> Species for the area </summary>
    /// <remarks> Due to how the encounter data is packaged by PKHeX, each species-form is grouped together. </remarks>
    public ushort Species { get; }
    /// <summary> Form of the Species </summary>
    public byte Form { get; }
    public readonly EncounterSlot8GO[] Slots;

    private EncounterArea8g(ushort species, byte form, EncounterSlot8GO[] slots)
    {
        Species = species;
        Form = form;
        Slots = slots;
    }

    internal static EncounterArea8g[] GetArea(BinLinkerAccessor data)
    {
        var areas = new EncounterArea8g[data.Length];
        for (int i = 0; i < areas.Length; i++)
            areas[i] = GetArea(data[i]);
        return areas;
    }

    private const int meta = 4;
    private const int entrySize = (2 * sizeof(ushort)) + 8;

    private static EncounterArea8g GetArea(ReadOnlySpan<byte> data)
    {
        var species = ReadUInt16LittleEndian(data);
        var form = data[2];
        var import = (PogoImportFormat)data[3];

        data = data[meta..];
        var result = new EncounterSlot8GO[data.Length / entrySize];
        var area = new EncounterArea8g(species, form, result);
        for (int i = 0; i < result.Length; i++)
        {
            var offset = i * entrySize;
            var entry = data.Slice(offset, entrySize);
            result[i] = ReadSlot(entry, species, form, import);
        }

        return area;
    }

    private static EncounterSlot8GO ReadSlot(ReadOnlySpan<byte> entry, ushort species, byte form, PogoImportFormat format)
    {
        ushort start = ReadUInt16LittleEndian(entry);
        ushort end = ReadUInt16LittleEndian(entry[2..]);

        var shiny = (Shiny)entry[4];
        var gender = (Gender)entry[5];
        var type = (PogoType)entry[6];
        var ball = (PogoBallRestriction)entry[7];

        var flags = (PogoFlags)entry[8];
        var minLevel = entry[9];
        var minIV = entry[10];
        return new EncounterSlot8GO(start, end, species, form, minLevel, minIV, shiny, gender, type, ball, format, flags);
    }
}

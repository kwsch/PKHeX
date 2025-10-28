using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// <see cref="GameVersion.GO"/> encounter area for <see cref="GameVersion.GG"/>
/// </summary>
public sealed record EncounterArea7g : ISpeciesForm
{
    /// <summary> Species for the area </summary>
    /// <remarks> Due to how the encounter data is packaged by PKHeX, each species-form is grouped together. </remarks>
    public ushort Species { get; }
    /// <summary> Form of the Species </summary>
    public byte Form { get; }
    public readonly EncounterSlot7GO[] Slots;

    private EncounterArea7g(ushort species, byte form, EncounterSlot7GO[] slots)
    {
        Species = species;
        Form = form;
        Slots = slots;
    }

    internal static EncounterArea7g[] GetArea(BinLinkerAccessor data)
    {
        var areas = new EncounterArea7g[data.Length];
        for (int i = 0; i < areas.Length; i++)
            areas[i] = GetArea(data[i]);
        return areas;
    }

    private const int meta = 4;
    private const int entrySize = (2 * sizeof(int)) + 2;

    private static EncounterArea7g GetArea(ReadOnlySpan<byte> data)
    {
        var species = ReadUInt16LittleEndian(data);
        var form = data[2];
        //var import = (EntityFormatDetected)data[3];

        data = data[meta..];
        var result = new EncounterSlot7GO[data.Length / entrySize];
        var area = new EncounterArea7g(species, form, result);
        for (int i = 0; i < result.Length; i++)
        {
            var offset = i * entrySize;
            var entry = data.Slice(offset, entrySize);
            result[i] = ReadSlot(entry, species, form);
        }

        return area;
    }

    private static EncounterSlot7GO ReadSlot(ReadOnlySpan<byte> entry, ushort species, byte form)
    {
        int start = ReadInt32LittleEndian(entry);
        int end = ReadInt32LittleEndian(entry[4..]);
        var sg = entry[8];
        var shiny = (Shiny)(sg & 0x3F);
        var gender = (Gender)(sg >> 6);
        var type = (PogoType)entry[9];
        return new EncounterSlot7GO(start, end, species, form, type.GetMinLevel(), EncountersGO.MAX_LEVEL, shiny, gender, type);
    }
}

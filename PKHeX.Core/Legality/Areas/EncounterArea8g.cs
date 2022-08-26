using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <inheritdoc cref="EncounterArea" />
/// <summary>
/// <see cref="GameVersion.GO"/> encounter area for direct-to-HOME transfers.
/// </summary>
public sealed record EncounterArea8g : EncounterArea, ISpeciesForm
{
    /// <summary> Species for the area </summary>
    /// <remarks> Due to how the encounter data is packaged by PKHeX, each species-form is grouped together. </remarks>
    public ushort Species { get; }
    /// <summary> Form of the Species </summary>
    public byte Form { get; }
    public readonly EncounterSlot8GO[] Slots;

    protected override IReadOnlyList<EncounterSlot> Raw => Slots;

    private EncounterArea8g(ushort species, byte form, EncounterSlot8GO[] slots) : base(GameVersion.GO)
    {
        Species = species;
        Form = form;
        Location = Locations.GO8;
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
    private const int entrySize = (2 * sizeof(int)) + 2;

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
            result[i] = ReadSlot(entry, area, species, form, import);
        }

        return area;
    }

    private static EncounterSlot8GO ReadSlot(ReadOnlySpan<byte> entry, EncounterArea8g area, ushort species, byte form, PogoImportFormat format)
    {
        int start = ReadInt32LittleEndian(entry);
        int end = ReadInt32LittleEndian(entry[4..]);
        var sg = entry[8];
        var shiny = (Shiny)(sg & 0x3F);
        var gender = (Gender)(sg >> 6);
        var type = (PogoType)entry[9];
        return new EncounterSlot8GO(area, species, form, start, end, shiny, gender, type, format);
    }

    public override IEnumerable<EncounterSlot> GetMatchingSlots(PKM pk, EvoCriteria[] chain)
    {
        if (pk.TSV == 0) // HOME doesn't assign TSV=0 to accounts.
            yield break;

        // Find the first chain that has slots defined.
        // Since it is possible to evolve before transferring, we only need the highest evolution species possible.
        // PoGoEncTool has already extrapolated the evolutions to separate encounters!
        var sf = FindCriteriaToIterate(pk, chain);
        if (sf == default)
            yield break;

        var species = pk.Species;
        var ball = (Ball)pk.Ball;
        var met = Math.Max(sf.LevelMin, pk.Met_Level);
        EncounterSlot8GO? deferredIV = null;

        foreach (var slot in Slots)
        {
            if (!slot.IsLevelWithinRange(met))
                continue;
            if (!slot.IsBallValid(ball, species))
                continue;
            if (!slot.Shiny.IsValid(pk))
                continue;
            if (slot.Gender != Gender.Random && (int)slot.Gender != pk.Gender)
                continue;

            if (!slot.GetIVsValid(pk))
            {
                deferredIV ??= slot;
                continue;
            }

            yield return slot;
        }

        if (deferredIV != null)
            yield return deferredIV;
    }

    private EvoCriteria FindCriteriaToIterate(PKM pk, EvoCriteria[] chain)
    {
        foreach (var evo in chain)
        {
            if (evo.Species != Species)
                continue;

            if (evo.Form == Form)
                return evo;

            // Check for form mismatches
            if (FormInfo.IsFormChangeable(Species, Form, evo.Form, pk.Format))
                return evo;
            if (Species == (int)Core.Species.Burmy)
                return evo;
            break;
        }
        return default;
    }
}

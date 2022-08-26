using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <inheritdoc cref="EncounterArea" />
/// <summary>
/// <see cref="GameVersion.GO"/> encounter area for <see cref="GameVersion.GG"/>
/// </summary>
public sealed record EncounterArea7g : EncounterArea, ISpeciesForm
{
    /// <summary> Species for the area </summary>
    /// <remarks> Due to how the encounter data is packaged by PKHeX, each species-form is grouped together. </remarks>
    public ushort Species { get; }
    /// <summary> Form of the Species </summary>
    public byte Form { get; }
    public readonly EncounterSlot7GO[] Slots;

    protected override IReadOnlyList<EncounterSlot> Raw => Slots;

    private EncounterArea7g(ushort species, byte form, EncounterSlot7GO[] slots) : base(GameVersion.GO)
    {
        Species = species;
        Form = form;
        Location = Locations.GO7;
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
            result[i] = ReadSlot(entry, area, species, form);
        }

        return area;
    }

    private static EncounterSlot7GO ReadSlot(ReadOnlySpan<byte> entry, EncounterArea7g area, ushort species, byte form)
    {
        int start = ReadInt32LittleEndian(entry);
        int end = ReadInt32LittleEndian(entry[4..]);
        var sg = entry[8];
        var shiny = (Shiny)(sg & 0x3F);
        var gender = (Gender)(sg >> 6);
        var type = (PogoType)entry[9];
        return new EncounterSlot7GO(area, species, form, start, end, shiny, gender, type);
    }

    public override IEnumerable<EncounterSlot> GetMatchingSlots(PKM pk, EvoCriteria[] chain)
    {
        // Find the first chain that has slots defined.
        // Since it is possible to evolve before transferring, we only need the highest evolution species possible.
        // PoGoEncTool has already extrapolated the evolutions to separate encounters!
        var sf = Array.Find(chain, z => z.Species == Species && z.Form == Form);
        if (sf == default)
            yield break;

        var stamp = EncounterSlotGO.GetTimeStamp(pk.Met_Year + 2000, pk.Met_Month, pk.Met_Day);
        var met = Math.Max(sf.LevelMin, pk.Met_Level);
        EncounterSlot7GO? deferredIV = null;

        foreach (var slot in Slots)
        {
            if (!slot.IsLevelWithinRange(met))
                continue;
            //if (!slot.IsBallValid(ball)) -- can have any of the in-game balls due to re-capture
            //    continue;
            if (!slot.Shiny.IsValid(pk))
                continue;
            //if (slot.Gender != Gender.Random && (int) slot.Gender != pk.Gender)
            //    continue;
            if (!slot.IsWithinStartEnd(stamp))
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
}

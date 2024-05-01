using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public enum SlotType8a : byte
{
    Standard = 0,
    Distortion = 1,
    Landmark = 2,
    MassOutbreakRegular = 3,
    MassOutbreakMassive = 4,
}

/// <summary>
/// <see cref="GameVersion.PLA"/> encounter area
/// </summary>
public sealed record EncounterArea8a : IEncounterArea<EncounterSlot8a>, IAreaLocation
{
    public EncounterSlot8a[] Slots { get; }
    public GameVersion Version => GameVersion.PLA;

    private readonly byte[] Locations;
    public readonly SlotType8a Type;

    public ushort Location => Locations[0];

    public bool IsMatchLocation(ushort location) => Locations.AsSpan().Contains((byte)location);

    public static EncounterArea8a[] GetAreas(BinLinkerAccessor input)
    {
        var result = new EncounterArea8a[input.Length];
        for (int i = 0; i < result.Length; i++)
            result[i] = new EncounterArea8a(input[i]);
        return result;
    }

    private EncounterArea8a(ReadOnlySpan<byte> areaData)
    {
        // Area Metadata
        var locationCount = areaData[0];
        Locations = areaData.Slice(1, locationCount).ToArray();

        var align = (locationCount + 1);
        align += align & 1; // ensure alignment is even
        areaData = areaData[align..];
        Type = (SlotType8a)areaData[0];
        var count = areaData[1];

        var slots = areaData[2..];
        Slots = ReadSlots(slots, count);
    }

    private EncounterSlot8a[] ReadSlots(ReadOnlySpan<byte> areaData, byte slotCount)
    {
        var slots = new EncounterSlot8a[slotCount];
        const int bpe = 8;
        for (int i = 0; i < slotCount; i++)
        {
            var ofs = i * bpe;
            var entry = areaData.Slice(ofs, bpe);
            byte flawless = entry[7];
            var gender = (Gender)entry[6];
            byte max = entry[5];
            byte min = entry[4];
            var alpha = entry[3];
            var form = entry[2];
            var species = ReadUInt16LittleEndian(entry);

            slots[i] = new EncounterSlot8a(this, species, form, min, max, alpha, flawless, gender);
        }
        return slots;
    }
}

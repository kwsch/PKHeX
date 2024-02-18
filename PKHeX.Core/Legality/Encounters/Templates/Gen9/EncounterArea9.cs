using System;
using System.Diagnostics.CodeAnalysis;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// <see cref="GameVersion.SV"/> encounter area
/// </summary>
public sealed record EncounterArea9 : IEncounterArea<EncounterSlot9>, IAreaLocation
{
    public EncounterSlot9[] Slots { get; }
    public GameVersion Version { get; }

    public readonly byte Location;
    public readonly byte CrossFrom; // Actual location if not default
    public byte ActualLocation() => CrossFrom == 0 ? Location : CrossFrom;

    public bool IsMatchLocation(ushort location) => Location == location;

    public static EncounterArea9[] GetAreas(BinLinkerAccessor input, [ConstantExpected] GameVersion game)
    {
        var result = new EncounterArea9[input.Length];
        for (int i = 0; i < result.Length; i++)
            result[i] = new EncounterArea9(input[i], game);
        return result;
    }

    private EncounterArea9(ReadOnlySpan<byte> areaData, [ConstantExpected] GameVersion game)
    {
        Location = areaData[0];
        CrossFrom = areaData[2];
        Version = game;
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
            var gender = slot[3];

            var min = slot[4];
            var max = slot[5];
            var time = slot[6];

            result[i] = new EncounterSlot9(this, species, form, min, max, gender, time);
        }
        return result;
    }
}

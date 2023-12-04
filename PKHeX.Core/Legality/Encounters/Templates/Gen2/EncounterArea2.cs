using System;

namespace PKHeX.Core;

/// <summary>
/// <see cref="GameVersion.GSC"/> encounter area
/// </summary>
public sealed record EncounterArea2 : IEncounterArea<EncounterSlot2>, IAreaLocation
{
    public EncounterSlot2[] Slots { get; }
    public GameVersion Version { get; }

    private static ReadOnlySpan<byte> BCC_SlotRates => [ 20, 20, 10, 10, 05, 05, 10, 10, 05, 05 ];
    private static ReadOnlySpan<byte> RatesGrass => [ 30, 30, 20, 10, 5, 4, 1 ];
    private static ReadOnlySpan<byte> RatesSurf => [ 60, 30, 10 ];

    public readonly byte[]? Rates;
    internal readonly EncounterTime Time;
    public readonly byte Rate;
    public readonly byte Location;
    public readonly SlotType Type;

    public bool IsMatchLocation(int location) => location == Location;

    public static EncounterArea2[] GetAreas(BinLinkerAccessor input, GameVersion game)
    {
        var result = new EncounterArea2[input.Length];
        for (int i = 0; i < result.Length; i++)
            result[i] = new EncounterArea2(input[i], game);
        return result;
    }

    private EncounterArea2(ReadOnlySpan<byte> data, GameVersion game)
    {
        Location = data[0];
        Time = (EncounterTime)data[1];
        var type = (Type = (SlotType)data[2]) & (SlotType)0xF;
        Rate = data[3];

        Version = game;

        var next = data[4..];
        if (type is > SlotType.Surf and not SlotType.BugContest) // Not Grass/Surf
        {
            const int size = 5;
            int count = next.Length / size;
            Rates = next[..count].ToArray();
            Slots = ReadSlots(next[count..], count);
        }
        else
        {
            const int size = 4;
            int count = next.Length / size;
            Rates = null; // fetch as needed.
            Slots = ReadSlots(next, count);
        }
    }

    private EncounterSlot2[] ReadSlots(ReadOnlySpan<byte> data, int count)
    {
        const int size = 4;
        var slots = new EncounterSlot2[count];
        for (int i = 0; i < slots.Length; i++)
        {
            var entry = data.Slice(i * size, size);
            byte max = entry[3];
            byte min = entry[2];
            byte slotNum = entry[1];
            byte species = entry[0];
            var form = species == (int)Species.Unown ? EncounterUtil1.FormRandom : (byte)0;
            slots[i] = new EncounterSlot2(this, species, form, min, max, slotNum);
        }
        return slots;
    }
}

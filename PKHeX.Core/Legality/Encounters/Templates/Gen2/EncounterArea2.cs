using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.SlotType2;

namespace PKHeX.Core;

/// <summary>
/// <see cref="GameVersion.GSC"/> encounter area
/// </summary>
public sealed record EncounterArea2 : IEncounterArea<EncounterSlot2>, IAreaLocation
{
    public EncounterSlot2[] Slots { get; }
    public GameVersion Version { get; }

    private static ReadOnlySpan<byte> RatesGrass => [ 30, 30, 20, 10, 5, 4, 1 ];
    private static ReadOnlySpan<byte> RatesSurf => [ 60, 30, 10 ];

    private readonly byte[] Rates; // Slot specific rates
    internal readonly EncounterTime Time;
    public readonly byte Rate; // Area Rate
    public readonly byte Location;
    public readonly SlotType2 Type;

    public bool IsMatchLocation(ushort location) => location == Location;

    public ReadOnlySpan<byte> GetSlotRates() => Type switch
    {
        Grass => RatesGrass,
        Surf => RatesSurf,
        _ => Rates,
    };

    public static EncounterArea2[] GetAreas(BinLinkerAccessor input, [ConstantExpected] GameVersion game)
    {
        var result = new EncounterArea2[input.Length];
        for (int i = 0; i < result.Length; i++)
            result[i] = new EncounterArea2(input[i], game);
        return result;
    }

    private EncounterArea2(ReadOnlySpan<byte> data, [ConstantExpected] GameVersion game)
    {
        Location = data[0];
        Time = (EncounterTime)data[1];
        Type = (SlotType2)data[2];
        Rate = data[3];

        Version = game;

        var slotData = data[4..];
        if (Type > Surf) // Not Grass/Surf
        {
            int count = slotData.Length / (SlotSize + 1); // each slot has a rate
            Rates = slotData[..count].ToArray();
            Slots = ReadSlots(slotData[count..], count);
        }
        else
        {
            int count = slotData.Length / SlotSize; // shared rate value
            Rates = []; // fetch as needed.
            Slots = ReadSlots(slotData, count);
        }
    }

    private const int SlotSize = 4;

    private EncounterSlot2[] ReadSlots(ReadOnlySpan<byte> data, int count)
    {
        var slots = new EncounterSlot2[count];
        for (int i = 0; i < slots.Length; i++)
        {
            var entry = data.Slice(i * SlotSize, SlotSize);
            byte max = entry[3];
            byte min = entry[2];
            byte slotNum = entry[1];
            byte species = entry[0];
            var form = species == (int)Species.Unown ? EncounterUtil.FormRandom : (byte)0;
            slots[i] = new EncounterSlot2(this, species, form, min, max, slotNum);
        }
        return slots;
    }
}

public enum SlotType2 : byte
{
    Grass = 0,
    Surf = 1,
    Old_Rod = 2,
    Good_Rod = 3,
    Super_Rod = 4,
    Rock_Smash = 5,

    Headbutt = 6,
    HeadbuttSpecial = 7,
    BugContest = 8,
}

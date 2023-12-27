using System;

namespace PKHeX.Core;

/// <summary>
/// <see cref="GameVersion.RBY"/> encounter area
/// </summary>
public sealed record EncounterArea1 : IEncounterArea<EncounterSlot1>
{
    public EncounterSlot1[] Slots { get; }
    public GameVersion Version { get; }

    public readonly byte Location;
    public readonly SlotType Type;
    public readonly byte Rate;

    public static EncounterArea1[] GetAreas(BinLinkerAccessor input, GameVersion game)
    {
        var result = new EncounterArea1[input.Length];
        for (int i = 0; i < result.Length; i++)
            result[i] = new EncounterArea1(input[i], game);
        return result;
    }

    private EncounterArea1(ReadOnlySpan<byte> data, GameVersion game)
    {
        Location = data[0];
        // 1 byte unused
        Type = (SlotType)data[2];
        Rate = data[3];
        Version = game;

        var next = data[4..];
        int count = next.Length / 4;
        var slots = new EncounterSlot1[count];
        for (int i = 0; i < slots.Length; i++)
        {
            const int size = 4;
            var entry = next.Slice(i * size, size);
            byte max = entry[3];
            byte min = entry[2];
            byte slotNum = entry[1];
            byte species = entry[0];
            slots[i] = new EncounterSlot1(this, species, min, max, slotNum);
        }
        Slots = slots;
    }
}

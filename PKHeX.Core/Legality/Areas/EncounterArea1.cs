using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <inheritdoc cref="EncounterArea" />
/// <summary>
/// <see cref="GameVersion.RBY"/> encounter area
/// </summary>
public sealed record EncounterArea1 : EncounterArea
{
    public readonly int Rate;
    public readonly EncounterSlot1[] Slots;

    protected override IReadOnlyList<EncounterSlot> Raw => Slots;

    public static EncounterArea1[] GetAreas(BinLinkerAccessor input, GameVersion game)
    {
        var result = new EncounterArea1[input.Length];
        for (int i = 0; i < result.Length; i++)
            result[i] = new EncounterArea1(input[i], game);
        return result;
    }

    private EncounterArea1(ReadOnlySpan<byte> data, GameVersion game) : base(game)
    {
        Location = data[0];
        // 1 byte unused
        Type = (SlotType)data[2];
        Rate = data[3];

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

    public override IEnumerable<EncounterSlot> GetMatchingSlots(PKM pk, EvoCriteria[] chain)
    {
        (bool useCatchRate, byte rate) = pk is PK1 pk1 ? (true, pk1.Catch_Rate) : (false, (byte)0);
        foreach (var slot in Slots)
        {
            foreach (var evo in chain)
            {
                if (slot.Species != evo.Species)
                    continue;

                if (slot.LevelMin > evo.LevelMax)
                    break;
                if (slot.Form != evo.Form)
                    break;

                if (useCatchRate)
                {
                    var expect = (slot.Version == GameVersion.YW ? PersonalTable.Y : PersonalTable.RB)[slot.Species].CatchRate;
                    if (expect != rate && !(ParseSettings.AllowGen1Tradeback && GBRestrictions.IsTradebackCatchRate(rate)))
                        break;
                }
                yield return slot;
                break;
            }
        }
    }
}

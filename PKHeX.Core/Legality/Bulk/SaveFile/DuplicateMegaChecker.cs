using System;
using System.Collections.Generic;
using static PKHeX.Core.LegalityCheckResultCode;

namespace PKHeX.Core.Bulk;

public sealed class DuplicateMegaChecker : IBulkAnalyzer
{
    private const CheckIdentifier Identifier = CheckIdentifier.HeldItem;

    public void Analyze(BulkAnalysis input)
    {
        if (input.Trainer is SAV9ZA za)
            AnalyzeInventory(input, za);
        else
            AnalyzeNoInventory(input);
    }

    private static void AnalyzeInventory(BulkAnalysis input, SAV9ZA za)
    {
        var items = za.Items;

        // Rule: Only 1 Mega Stone of a given Item ID may be held across all Pokémon, or present in the bag.
        Dictionary<ushort, int> seenStones = []; // ushort item, int index
        for (var i = 0; i < input.AllData.Count; i++)
        {
            var slot = input.AllData[i];
            var pk = slot.Entity;

            var stone = (ushort)pk.HeldItem;
            if (!ItemStorage9ZA.IsMegaStone(stone))
                continue;

            var item = items.GetItem(stone);
            if (item.Count == 0) // Not acquired by the save file, thus not able to be held.
                input.AddLine(slot, Identifier, BulkCheckResult.NoIndex, BulkNotAcquiredMegaStoneInventory, stone);
            else if (!item.IsHeld) // Not marked as held, so it's still "in the bag" (not given).
                input.AddLine(slot, Identifier, BulkCheckResult.NoIndex, BulkDuplicateMegaStoneInventory, stone);
            else if (seenStones.TryGetValue(stone, out var otherIndex)) // Already given to another slot.
                input.AddLine(slot, input.AllData[otherIndex], Identifier, i, index2: otherIndex, BulkDuplicateMegaStoneSlot, stone);
            else // First time seeing this Mega Stone, all good.
                seenStones[stone] = i;
        }

        CheckOtherUnassigned(input, items, ItemStorage9ZA.MegaStones, seenStones);
    }

    private static void CheckOtherUnassigned(BulkAnalysis input, MyItem9a items, ReadOnlySpan<ushort> megaStones, IReadOnlyDictionary<ushort, int> seenStones)
    {
        // Check that all other un-assigned stones are not marked as assigned.
        foreach (var stone in megaStones)
        {
            if (seenStones.ContainsKey(stone))
                continue;
            var item = items.GetItem(stone);
            if (item.IsHeld)
                input.AddMessage(string.Empty, CheckResult.Get(Severity.Invalid, CheckIdentifier.HeldItem, BulkAssignedMegaStoneNotFound_0, stone));
        }
    }

    private static void AnalyzeNoInventory(BulkAnalysis input)
    {
        // Rule: Only 1 Mega Stone of a given Item ID may be held across all Pokémon, or present in the bag.
        Dictionary<ushort, int> seenStones = []; // ushort item, int index
        for (var i = 0; i < input.AllData.Count; i++)
        {
            var slot = input.AllData[i];
            var pk = slot.Entity;

            var stone = (ushort)pk.HeldItem;
            if (!ItemStorage9ZA.IsMegaStone(stone))
                continue;

            if (seenStones.TryGetValue(stone, out var otherIndex)) // Already given to another slot.
                input.AddLine(slot, input.AllData[otherIndex], Identifier, i, index2: otherIndex, BulkDuplicateMegaStoneSlot, stone);
            else // First time seeing this Mega Stone, all good.
                seenStones[stone] = i;
        }
    }
}

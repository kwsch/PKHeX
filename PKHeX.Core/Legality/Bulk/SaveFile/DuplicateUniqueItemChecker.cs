using System;
using System.Collections.Generic;
using static PKHeX.Core.LegalityCheckResultCode;

namespace PKHeX.Core.Bulk;

public sealed class DuplicateUniqueItemChecker : IBulkAnalyzer
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

        // Rule: Only 1 of a unique Item ID may be held across all Pokémon, or present in the bag.
        Dictionary<ushort, int> seenStones = []; // ushort item, int index
        for (var i = 0; i < input.AllData.Count; i++)
        {
            var slot = input.AllData[i];
            var pk = slot.Entity;

            var stone = (ushort)pk.HeldItem;
            if (!ItemStorage9ZA.IsUniqueHeldItem(stone))
                continue;

            var item = items.GetItem(stone);
            if (item.Count == 0) // Not acquired by the save file, thus not able to be held.
                input.AddLine(slot, Identifier, BulkCheckResult.NoIndex, BulkHeldItemInventoryNotAcquired_0, stone);
            else if (!item.IsHeld) // Not marked as held, so it's still "in the bag" (not given).
                input.AddLine(slot, Identifier, BulkCheckResult.NoIndex, BulkHeldItemInventoryUnassigned_0, stone);
            else if (seenStones.TryGetValue(stone, out var otherIndex)) // Already given to another slot.
                input.AddLine(slot, input.AllData[otherIndex], Identifier, i, index2: otherIndex, BulkHeldItemInventoryMultipleSlots_0, stone);
            else // First time seeing this item, all good.
                seenStones[stone] = i;
        }

        CheckOtherUnassigned(input, items, ItemStorage9ZA.GetAllUniqueHeldItems(), seenStones);
    }

    private static void CheckOtherUnassigned(BulkAnalysis input, MyItem9a items, ReadOnlySpan<ushort> uniqueItems, IReadOnlyDictionary<ushort, int> seenItems)
    {
        // Check that all other un-assigned stones are not marked as assigned.
        foreach (var item in uniqueItems)
        {
            if (seenItems.ContainsKey(item))
                continue;
            var entry = items.GetItem(item);
            if (entry.IsHeld)
                input.AddMessage(string.Empty, CheckResult.Get(Severity.Invalid, CheckIdentifier.HeldItem, BulkHeldItemInventoryAssignedNoneHeld_0, item));
        }
    }

    private static void AnalyzeNoInventory(BulkAnalysis input)
    {
        // Rule: Only 1 of a unique Item ID may be held across all Pokémon, or present in the bag.
        Dictionary<ushort, int> seenStones = []; // ushort item, int index
        for (var i = 0; i < input.AllData.Count; i++)
        {
            var slot = input.AllData[i];
            var pk = slot.Entity;

            var stone = (ushort)pk.HeldItem;
            if (!ItemStorage9ZA.IsUniqueHeldItem(stone))
                continue;

            if (seenStones.TryGetValue(stone, out var otherIndex)) // Already given to another slot.
                input.AddLine(slot, input.AllData[otherIndex], Identifier, i, index2: otherIndex, BulkHeldItemInventoryMultipleSlots_0, stone);
            else // First time seeing this item, all good.
                seenStones[stone] = i;
        }
    }
}

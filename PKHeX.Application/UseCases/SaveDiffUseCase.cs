using PKHeX.Core;

namespace PKHeX.Application.UseCases;

/// <summary>Broad bucket a detected difference falls into, for grouping in the diff viewer.</summary>
public enum SaveDiffCategory
{
    Trainer,
    Inventory,
    BoxParty,

    /// <summary>
    /// Reserved for future well-known flag/record comparisons (e.g. badges, Pokédex flags) as they're
    /// added per generation. Deliberately not backed by a raw byte-diff — that flagged incidental,
    /// non-meaningful engine bookkeeping (checksums, auto-set Pokédex bits, etc.) as false positives,
    /// which conflicts with showing users an exact, meaningful diff. Full binary diffing already
    /// exists in the Block Editor.
    /// </summary>
    Misc,
}

/// <summary>One detected difference between two saves.</summary>
public readonly record struct SaveDiffChange(SaveDiffCategory Category, string Description, string? OldValue, string? NewValue);

/// <summary>
/// Result of comparing two saves. <see cref="Success"/> is <see langword="false"/> (with <see cref="Error"/>
/// set) when the saves are not comparable at all, e.g. different games — callers must fail gracefully
/// rather than produce a misleading diff.
/// </summary>
public readonly record struct SaveDiffResult(bool Success, string? Error, IReadOnlyList<SaveDiffChange> Changes);

/// <summary>
/// Produces a categorized, high-level diff between two <see cref="SaveFile"/> instances by comparing
/// well-known accessors (trainer info, inventory, box/party slots). Deliberately not an exhaustive
/// byte-level diff — that already exists in the Block Editor — so this stays maintainable as new
/// generations are added.
/// </summary>
public sealed class SaveDiffUseCase
{
    public SaveDiffResult Execute(SaveFile left, SaveFile right)
    {
        if (!IsComparable(left, right))
        {
            return new SaveDiffResult(false,
                $"These saves are from different games ({left.Version} vs {right.Version}) and cannot be compared.",
                []);
        }

        var changes = new List<SaveDiffChange>();
        CompareTrainer(left, right, changes);
        CompareInventory(left, right, changes);
        CompareBoxAndParty(left, right, changes);
        return new SaveDiffResult(true, null, changes);
    }

    /// <summary>
    /// Two saves are comparable if they're the same concrete save-file implementation (which implies
    /// same generation/layout) and the same game version. Different versions within a compatible pair
    /// (e.g. Sword vs Shield) can still have divergent held-item/box layouts, so require an exact match.
    /// </summary>
    private static bool IsComparable(SaveFile left, SaveFile right)
        => left.GetType() == right.GetType() && left.Version == right.Version;

    private static void CompareTrainer(SaveFile left, SaveFile right, List<SaveDiffChange> changes)
    {
        AddIfChanged(changes, SaveDiffCategory.Trainer, "Trainer Name", left.OT, right.OT);
        AddIfChanged(changes, SaveDiffCategory.Trainer, "Trainer ID", left.TID16.ToString(), right.TID16.ToString());
        AddIfChanged(changes, SaveDiffCategory.Trainer, "Secret ID", left.SID16.ToString(), right.SID16.ToString());
        AddIfChanged(changes, SaveDiffCategory.Trainer, "Trainer Gender", left.Gender.ToString(), right.Gender.ToString());
        AddIfChanged(changes, SaveDiffCategory.Trainer, "Money", left.Money.ToString(), right.Money.ToString());

        var leftTime = $"{left.PlayedHours}h {left.PlayedMinutes}m {left.PlayedSeconds}s";
        var rightTime = $"{right.PlayedHours}h {right.PlayedMinutes}m {right.PlayedSeconds}s";
        AddIfChanged(changes, SaveDiffCategory.Trainer, "Play Time", leftTime, rightTime);
    }

    private static void CompareInventory(SaveFile left, SaveFile right, List<SaveDiffChange> changes)
    {
        var leftPouches = left.Inventory.Pouches;
        var rightPouches = right.Inventory.Pouches;

        foreach (var leftPouch in leftPouches)
        {
            InventoryPouch? rightPouch = null;
            foreach (var candidate in rightPouches)
            {
                if (candidate.Type == leftPouch.Type)
                {
                    rightPouch = candidate;
                    break;
                }
            }
            if (rightPouch is null)
                continue;

            var leftItems = ToCountByIndex(leftPouch);
            var rightItems = ToCountByIndex(rightPouch);

            var itemIds = new SortedSet<int>(leftItems.Keys);
            itemIds.UnionWith(rightItems.Keys);

            foreach (var itemId in itemIds)
            {
                leftItems.TryGetValue(itemId, out var leftCount);
                rightItems.TryGetValue(itemId, out var rightCount);
                if (leftCount == rightCount)
                    continue;

                var name = Services.StringResourceLookup.Item(itemId);
                changes.Add(new SaveDiffChange(SaveDiffCategory.Inventory, $"{name} ({leftPouch.Type})",
                    leftCount.ToString(), rightCount.ToString()));
            }
        }
    }

    private static Dictionary<int, int> ToCountByIndex(InventoryPouch pouch)
    {
        var dict = new Dictionary<int, int>();
        foreach (var item in pouch.Items)
        {
            if (item.Index == 0 || item.Count == 0)
                continue;
            dict[item.Index] = item.Count;
        }
        return dict;
    }

    private static void CompareBoxAndParty(SaveFile left, SaveFile right, List<SaveDiffChange> changes)
    {
        CompareSlots(left.BoxData, right.BoxData, left.BoxSlotCount, "Box", changes);
        CompareSlots(left.PartyData, right.PartyData, left.PartyData.Count, "Party", changes);
    }

    private static void CompareSlots(IList<PKM> leftSlots, IList<PKM> rightSlots, int slotsPerGroup, string groupLabel, List<SaveDiffChange> changes)
    {
        var count = Math.Min(leftSlots.Count, rightSlots.Count);
        for (int i = 0; i < count; i++)
        {
            var leftPk = leftSlots[i];
            var rightPk = rightSlots[i];

            var leftEmpty = leftPk.Species == 0;
            var rightEmpty = rightPk.Species == 0;
            if (leftEmpty && rightEmpty)
                continue;

            if (SlotsEqual(leftPk, rightPk))
                continue;

            var position = slotsPerGroup > 0
                ? $"{groupLabel} {i / slotsPerGroup + 1}, Slot {i % slotsPerGroup + 1}"
                : $"{groupLabel} Slot {i + 1}";

            var leftDesc = DescribeSlot(leftPk, leftEmpty);
            var rightDesc = DescribeSlot(rightPk, rightEmpty);
            changes.Add(new SaveDiffChange(SaveDiffCategory.BoxParty, position, leftDesc, rightDesc));
        }
    }

    private static bool SlotsEqual(PKM left, PKM right)
    {
        if (left.Species != right.Species)
            return false;
        if (left.Species == 0)
            return true; // both empty, already short-circuited above but keep this safe
        return left.Data.SequenceEqual(right.Data);
    }

    private static string DescribeSlot(PKM pk, bool empty)
    {
        if (empty)
            return "(empty)";
        var name = Services.StringResourceLookup.Species(pk.Species);
        return name.Length > 0
            ? $"{name}{(pk.IsShiny ? " (Shiny)" : string.Empty)}"
            : $"Species #{pk.Species}";
    }

    private static void AddIfChanged(List<SaveDiffChange> changes, SaveDiffCategory category, string description, string oldValue, string newValue)
    {
        if (oldValue == newValue)
            return;
        changes.Add(new SaveDiffChange(category, description, oldValue, newValue));
    }
}

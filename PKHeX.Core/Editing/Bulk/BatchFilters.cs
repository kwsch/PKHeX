using System.Collections.Generic;
using static PKHeX.Core.BatchEditing;

namespace PKHeX.Core;

/// <summary>
/// Filters for Batch Editing
/// </summary>
public static class BatchFilters
{
    public static readonly List<IComplexFilter> FilterMods = new()
    {
        new ComplexFilter(PROP_LEGAL,
            (pk, cmd) => bool.TryParse(cmd.PropertyValue, out var b) && cmd.Comparer.IsCompareEquivalence(b == new LegalityAnalysis(pk).Valid),
            (info, cmd) => bool.TryParse(cmd.PropertyValue, out var b) && cmd.Comparer.IsCompareEquivalence(b == info.Legality.Valid)),

        new ComplexFilter(PROP_TYPENAME,
            (pk, cmd) => cmd.Comparer.IsCompareEquivalence(pk.GetType().Name == cmd.PropertyValue),
            (info, cmd) => cmd.Comparer.IsCompareEquivalence(info.Entity.GetType().Name == cmd.PropertyValue)),
    };

    public static readonly List<IComplexFilterMeta> FilterMeta = new()
    {
        new MetaFilter(IdentifierContains,
            (obj, cmd) => obj is SlotCache s && cmd.Comparer.IsCompareEquivalence(s.Identify().Contains(cmd.PropertyValue))),

        new MetaFilter(nameof(SlotInfoBox.Box),
            (obj, cmd) => obj is SlotCache { Source: SlotInfoBox b } && int.TryParse(cmd.PropertyValue, out var box) && b.Box + 1 == box),

        new MetaFilter(nameof(ISlotInfo.Slot),
            (obj, cmd) => obj is SlotCache s && int.TryParse(cmd.PropertyValue, out var slot) && s.Source.Slot + 1 == slot),
    };
}

using System.Collections.Generic;
using static PKHeX.Core.BatchEditing;

namespace PKHeX.Core;

/// <summary>
/// Filters for Batch Editing
/// </summary>
public static class BatchFilters
{
    /// <summary>
    /// Filters to use for <see cref="BatchEditing"/> that are derived from the <see cref="PKM"/> data.
    /// </summary>
    public static readonly List<IComplexFilter> FilterMods =
    [
        new ComplexFilter(PROP_LEGAL,
            (pk, cmd) => bool.TryParse(cmd.PropertyValue, out var b) && cmd.Comparer.IsCompareEquivalence(b == new LegalityAnalysis(pk).Valid),
            (info, cmd) => bool.TryParse(cmd.PropertyValue, out var b) && cmd.Comparer.IsCompareEquivalence(b == info.Legality.Valid)),

        new ComplexFilter(PROP_TYPENAME,
            (pk, cmd) => cmd.Comparer.IsCompareEquivalence(pk.GetType().Name == cmd.PropertyValue),
            (info, cmd) => cmd.Comparer.IsCompareEquivalence(info.Entity.GetType().Name == cmd.PropertyValue)),

        new ComplexFilter(PROP_TYPE1,
            (pk, cmd) => byte.TryParse(cmd.PropertyValue, out var b) && cmd.Comparer.IsCompareEquivalence(b == pk.PersonalInfo.Type1),
            (info, cmd) => byte.TryParse(cmd.PropertyValue, out var b) && cmd.Comparer.IsCompareEquivalence(b == info.Entity.PersonalInfo.Type1)),

        new ComplexFilter(PROP_TYPE2,
            (pk, cmd) => byte.TryParse(cmd.PropertyValue, out var b) && cmd.Comparer.IsCompareEquivalence(b == pk.PersonalInfo.Type2),
            (info, cmd) => byte.TryParse(cmd.PropertyValue, out var b) && cmd.Comparer.IsCompareEquivalence(b == info.Entity.PersonalInfo.Type2)),

        new ComplexFilter(PROP_TYPEEITHER,
            (pk, cmd) => byte.TryParse(cmd.PropertyValue, out var b) && cmd.Comparer.IsCompareEquivalence(pk.PersonalInfo.IsType(b)),
            (info, cmd) => byte.TryParse(cmd.PropertyValue, out var b) && cmd.Comparer.IsCompareEquivalence(info.Entity.PersonalInfo.IsType(b))),
    ];

    /// <summary>
    /// Filters to use for <see cref="BatchEditing"/> that are derived from the <see cref="PKM"/> source.
    /// </summary>
    public static readonly List<IComplexFilterMeta> FilterMeta =
    [
        new MetaFilter(IdentifierContains,
            (obj, cmd) => obj is SlotCache s && cmd.Comparer.IsCompareEquivalence(s.Identify().Contains(cmd.PropertyValue))),

        new MetaFilter(nameof(SlotInfoBox.Box),
            (obj, cmd) => obj is SlotCache { Source: SlotInfoBox b } && int.TryParse(cmd.PropertyValue, out var box) && cmd.Comparer.IsCompareOperator((b.Box + 1).CompareTo(box))),

        new MetaFilter(nameof(ISlotInfo.Slot),
            (obj, cmd) => obj is SlotCache s && int.TryParse(cmd.PropertyValue, out var slot) && cmd.Comparer.IsCompareOperator((s.Source.Slot + 1).CompareTo(slot))),
    ];
}

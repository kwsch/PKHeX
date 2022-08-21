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
            (pk, cmd) => bool.TryParse(cmd.PropertyValue, out var b) && (b == new LegalityAnalysis(pk).Valid) == cmd.Evaluator,
            (info, cmd) => bool.TryParse(cmd.PropertyValue, out var b) && (b == info.Legality.Valid) == cmd.Evaluator),

        new ComplexFilter(PROP_TYPENAME,
            (pk, cmd) => (pk.GetType().Name == cmd.PropertyValue) == cmd.Evaluator,
            (info, cmd) => (info.Entity.GetType().Name == cmd.PropertyValue) == cmd.Evaluator),
    };

    public static readonly List<IComplexFilterMeta> FilterMeta = new()
    {
        new MetaFilter(IdentifierContains,
            (obj, cmd) => obj is SlotCache s && s.Identify().Contains(cmd.PropertyValue) == cmd.Evaluator),

        new MetaFilter(nameof(SlotInfoBox.Box),
            (obj, cmd) => obj is SlotCache { Source: SlotInfoBox b } && int.TryParse(cmd.PropertyValue, out var box) && b.Box + 1 == box),

        new MetaFilter(nameof(ISlotInfo.Slot),
            (obj, cmd) => obj is SlotCache s && int.TryParse(cmd.PropertyValue, out var slot) && s.Source.Slot + 1 == slot),
    };
}

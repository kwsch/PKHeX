namespace PKHeX.Core.Bulk;

/// <summary>
/// Tuple wrapper to store a legality analysis result and the slot it was generated from.
/// </summary>
public sealed record CombinedReference(SlotCache Slot, LegalityAnalysis Analysis);

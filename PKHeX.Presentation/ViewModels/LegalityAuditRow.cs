using PKHeX.Application.UseCases;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// One row of the Legality Audit grid: an <see cref="EntitySummary"/> for display plus the
/// verdict/location/failure text computed by <see cref="LegalityAuditUseCase"/> (the same
/// <see cref="LegalityAnalysis"/> the editor's legality panel uses, so the verdict never diverges).
/// </summary>
public sealed class LegalityAuditRow : EntitySummary
{
    private readonly LegalityAuditEntry _entry;

    /// <summary>True if this slot came from the party rather than a box.</summary>
    public bool IsParty => _entry.IsParty;

    /// <summary>Zero-based box index. Meaningless when <see cref="IsParty"/> is true.</summary>
    public int Box => _entry.Box;

    /// <summary>Zero-based slot index within the box, or the party index when <see cref="IsParty"/> is true.</summary>
    public int Slot => _entry.Slot;

    public override string Position => IsParty ? $"Party {Slot + 1}" : $"B{Box + 1:00}:{Slot + 1:00}";

    /// <summary>Whether this entity passed every legality check.</summary>
    public bool Valid => _entry.Valid;

    /// <summary>"Legal" or "Illegal", for grid/CSV display.</summary>
    public string Verdict => Valid ? "Legal" : "Illegal";

    /// <summary>Semicolon-joined, single-line rendering of the failed checks (empty when <see cref="Valid"/>).</summary>
    public string FailureSummary => _entry.FailureSummary;

    /// <summary>The full standard PKHeX legality report text, identical to the editor's legality panel.</summary>
    public string ReportText => _entry.ReportText;

    public LegalityAuditRow(LegalityAuditEntry entry, GameStrings strings) : base(entry.Entity, strings)
    {
        _entry = entry;
    }
}

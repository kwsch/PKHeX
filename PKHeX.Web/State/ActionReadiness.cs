namespace PKHeX.Web.State;

/// <summary>Why Apply cannot apply the draft yet, in the order the reasons are listed.</summary>
public enum ApplyBlocker
{
    /// <summary>Another operation (a download) is running.</summary>
    Busy,

    /// <summary>The draft's position is not written by this release (see <see cref="EditorDraft.CanApply"/>).</summary>
    NotWritable,

    /// <summary>The draft does not differ from its slot, so there is nothing to apply.</summary>
    NoChanges,

    /// <summary>The last edit was refused (<see cref="WorkspaceState.DraftRefusal"/>).</summary>
    FieldRefused,

    /// <summary>A species or form change is previewed but not made.</summary>
    SpeciesPreviewPending,

    /// <summary>The draft as it is now has no legality result yet.</summary>
    LegalityWaiting,

    /// <summary>The draft's Invalid or Unavailable result is not acknowledged.</summary>
    LegalityNotAcknowledged,
}

/// <summary>Why Download cannot download the save yet, in the order the reasons are listed.</summary>
public enum DownloadBlocker
{
    /// <summary>Another download is running.</summary>
    Busy,

    /// <summary>The last edit was refused (<see cref="WorkspaceState.DraftRefusal"/>).</summary>
    FieldRefused,

    /// <summary>A species or form change is previewed but not made.</summary>
    SpeciesPreviewPending,

    /// <summary>The draft has changes that are not applied, which the download would not hold.</summary>
    DraftNotApplied,

    /// <summary>Applied changes that legality flagged are not acknowledged for this download (<see cref="SaveSession.ExportNeedsAcknowledgement"/>).</summary>
    ExportNotAcknowledged,
}

/// <summary>
/// Whether Apply and Download can act now, and if not, every reason why. The buttons' state, their guards and the error summary shown when
/// one is activated all come from here, so they cannot disagree.
/// </summary>
public static class ActionReadiness
{
    /// <summary>The reasons Apply cannot apply the draft; empty when it can.</summary>
    /// <param name="busy">True while another operation runs.</param>
    /// <param name="canApply">True when the draft's position is written by this release.</param>
    /// <param name="dirty">True when the draft differs from its slot.</param>
    /// <param name="refusal">The last refused edit, or null.</param>
    /// <param name="previewing">True while a species or form change is previewed but not made.</param>
    /// <param name="gate">What legality asks of the draft as it is now (<see cref="DraftLegality.Gate"/>).</param>
    public static IReadOnlyList<ApplyBlocker> ForApply(bool busy, bool canApply, bool dirty, FieldRefusal? refusal, bool previewing, LegalityGate gate)
    {
        var blockers = new List<ApplyBlocker>();
        if (busy)
        {
            blockers.Add(ApplyBlocker.Busy);
        }
        if (!canApply)
        {
            // Nothing else can make an unwritable position applicable, so no other reason is worth listing.
            blockers.Add(ApplyBlocker.NotWritable);
            return blockers;
        }
        if (refusal is not null)
        {
            blockers.Add(ApplyBlocker.FieldRefused);
        }
        else if (!dirty)
        {
            blockers.Add(ApplyBlocker.NoChanges);
        }
        if (previewing)
        {
            blockers.Add(ApplyBlocker.SpeciesPreviewPending);
        }
        // Legality is asked of a valid changed draft only: a refused or unchanged one has no result to wait for.
        if (refusal is null && dirty)
        {
            switch (gate)
            {
                case LegalityGate.Waiting:
                    blockers.Add(ApplyBlocker.LegalityWaiting);
                    break;
                case LegalityGate.NeedsAcknowledgement:
                    blockers.Add(ApplyBlocker.LegalityNotAcknowledged);
                    break;
            }
        }
        return blockers;
    }

    /// <summary>The reasons Download cannot download the save; empty when it can.</summary>
    /// <param name="busy">True while another operation runs.</param>
    /// <param name="dirty">True when the draft differs from its slot.</param>
    /// <param name="refusal">The last refused edit, or null.</param>
    /// <param name="previewing">True while a species or form change is previewed but not made.</param>
    /// <param name="exportNeedsAcknowledgement">True when flagged changes are not acknowledged for this download.</param>
    public static IReadOnlyList<DownloadBlocker> ForDownload(bool busy, bool dirty, FieldRefusal? refusal, bool previewing, bool exportNeedsAcknowledgement)
    {
        var blockers = new List<DownloadBlocker>();
        if (busy)
        {
            blockers.Add(DownloadBlocker.Busy);
        }
        if (refusal is not null)
        {
            blockers.Add(DownloadBlocker.FieldRefused);
        }
        if (previewing)
        {
            blockers.Add(DownloadBlocker.SpeciesPreviewPending);
        }
        if (dirty)
        {
            blockers.Add(DownloadBlocker.DraftNotApplied);
        }
        if (exportNeedsAcknowledgement)
        {
            blockers.Add(DownloadBlocker.ExportNotAcknowledged);
        }
        return blockers;
    }
}

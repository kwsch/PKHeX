namespace PKHeX.Web.State;

/// <summary>Which pane a stacked (phone-width) layout shows: the party and boxes, or the selected Pokémon's editor.</summary>
public enum WorkspacePane
{
    /// <summary>The overview, party and boxes.</summary>
    Storage,

    /// <summary>The selected Pokémon's editor.</summary>
    Editor,
}

/// <summary>
/// The layout choices of the open workspace that are independent of the screen width: which pane a stacked layout shows, and whether a
/// medium-width layout has folded the storage pane away to give the editor room.
/// </summary>
/// <remarks>
/// The width decides which of these the page applies, in CSS alone: a wide layout shows both panes and ignores both choices, a medium one
/// honours <see cref="StorageCollapsed"/>, and a narrow one shows only <see cref="Pane"/>. Resizing therefore never changes them, so it cannot
/// lose the selection, the box shown or the draft.
/// </remarks>
public sealed class WorkspaceView
{
    /// <summary>The pane a stacked layout shows. It is the editor only while a draft is open.</summary>
    public WorkspacePane Pane { get; private set; } = WorkspacePane.Storage;

    /// <summary>True when a medium-width layout hides the storage pane. It is offered, and holds, only while a draft is open.</summary>
    public bool StorageCollapsed { get; private set; }

    /// <summary>True while a draft is open, which the editor pane and both choices depend on.</summary>
    public bool HasDraft { get; private set; }

    /// <summary>A slot was opened (or opened again): a stacked layout moves to its editor.</summary>
    public void ShowEditor()
    {
        HasDraft = true;
        Pane = WorkspacePane.Editor;
    }

    /// <summary>The return action: a stacked layout goes back to the party and boxes, keeping the draft.</summary>
    public void ShowStorage() => Pane = WorkspacePane.Storage;

    /// <summary>Folds the storage pane away, or brings it back. It does nothing without a draft, since storage is then all there is to show.</summary>
    public void ToggleStorage()
    {
        if (HasDraft)
        {
            StorageCollapsed = !StorageCollapsed;
        }
    }

    /// <summary>
    /// The draft was closed, or a new session was opened: only the party and boxes are left to show, so both choices go back to showing them.
    /// </summary>
    public void Reset()
    {
        HasDraft = false;
        Pane = WorkspacePane.Storage;
        StorageCollapsed = false;
    }
}

using PKHeX.Web.State;

namespace PKHeX.Web.Components;

/// <summary>
/// The responsive workspace's fixed names: the width at which panes stack, the pane actions' text, and the elements that focus moves to
/// when a pane change hides the one that had it (WEB-APP-003, WEB-A11Y-002).
/// </summary>
public static class WorkspaceLayout
{
    /// <summary>
    /// The media query under which one pane is shown at a time. <c>wwwroot/app.css</c> and <c>wwwroot/browser.js</c> use this same text
    /// (pinned by a test), so focus moves exactly when the layout hides the element that had it.
    /// </summary>
    public const string NarrowQuery = "(max-width: 39.99rem)";

    /// <summary>The media query from which both panes are shown side by side; also in <c>app.css</c> and <c>browser.js</c>.</summary>
    public const string WideQuery = "(min-width: 75rem)";

    /// <summary>The narrow layout's return action.</summary>
    public const string ReturnToStorage = "Back to party and boxes";

    /// <summary>The narrow layout's way back from the party and boxes to the open draft.</summary>
    public const string ResumeEditor = "Back to the selected Pokémon";

    /// <summary>The medium layout's disclosure for the storage pane; <c>aria-expanded</c> says whether it is shown.</summary>
    public const string StorageToggle = "Party and boxes";

    /// <summary>The editor's heading, focused when a slot is opened on a narrow screen.</summary>
    public const string EditorHeadingId = "draft-title";

    /// <summary>The storage pane's heading, focused on return when the selected slot is not shown (its box is not the one shown).</summary>
    public const string StorageHeadingId = "storage-title";

    /// <summary>
    /// The element id of <paramref name="slot"/>'s button in the storage browser (see <see cref="StorageBrowser"/>), or null when it is not
    /// shown: a box slot of another box than <paramref name="currentBox"/>.
    /// </summary>
    /// <param name="slot">The slot.</param>
    /// <param name="asList">True when the party and box are shown as lists, whose buttons are the Open buttons.</param>
    /// <param name="currentBox">The box the storage browser shows.</param>
    public static string? SlotElementId(SlotRef slot, bool asList, int currentBox)
    {
        var view = asList ? "list" : "grid";
        if (slot.IsParty)
        {
            return $"party-{view}-{slot.Slot}";
        }
        return slot.Box == currentBox ? $"box-{view}-{slot.Slot}" : null;
    }
}

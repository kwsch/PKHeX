namespace PKHeX.Web.State;

/// <summary>Why the open session is being left.</summary>
public enum ExitIntent
{
    /// <summary>Another file was opened and waits in <see cref="SessionExit.Candidate"/>.</summary>
    Replace,

    /// <summary>The user asked to close the save and return to the start screen.</summary>
    Close,
}

/// <summary>What still has to be resolved before the open session can be left; see <see cref="WorkspaceState.ExitStage"/>.</summary>
public enum ExitStage
{
    /// <summary>No exit is in progress.</summary>
    None,

    /// <summary>The draft has unapplied or refused changes: apply it, discard it, or cancel the exit.</summary>
    ResolveDraft,

    /// <summary>The session has changes that the current revision's download does not cover: download them, discard the session, or cancel.</summary>
    ResolveSession,

    /// <summary>The current revision was downloaded: the user must confirm they checked the file, download again, or cancel.</summary>
    ConfirmExport,

    /// <summary>
    /// Nothing would be lost any more (for example the draft was cancelled in the editor), but leaving still waits for the user,
    /// so an editor action never closes or replaces the session by itself.
    /// </summary>
    Ready,
}

/// <summary>A request to leave the open session, held until every loss it would cause has been resolved or it is cancelled.</summary>
/// <param name="Intent">Replace or close.</param>
/// <param name="Candidate">The parsed and checked replacement for <see cref="ExitIntent.Replace"/>; null for <see cref="ExitIntent.Close"/>.</param>
public sealed record SessionExit(ExitIntent Intent, SaveSession? Candidate)
{
    /// <summary>Leave by replacing the session with <paramref name="candidate"/>.</summary>
    public static SessionExit Replace(SaveSession candidate) => new(ExitIntent.Replace, candidate);

    /// <summary>Leave by closing the session.</summary>
    public static SessionExit Close { get; } = new(ExitIntent.Close, null);
}

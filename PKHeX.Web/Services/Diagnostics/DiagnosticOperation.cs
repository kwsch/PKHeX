namespace PKHeX.Web.Services.Diagnostics;

/// <summary>
/// What the user was doing when a failure was recorded. Named in a diagnostic report; it carries nothing from the save.
/// </summary>
public enum DiagnosticOperation
{
    /// <summary>Opening a file (picker or drop).</summary>
    Open,

    /// <summary>Opening a party position or box slot in the editor.</summary>
    Slot,

    /// <summary>Editing a field of the draft.</summary>
    Edit,

    /// <summary>Analysing the draft's legality.</summary>
    Analyze,

    /// <summary>Applying the draft to the session.</summary>
    Apply,

    /// <summary>Building and starting a download of the save.</summary>
    Export,

    /// <summary>Closing, resetting, discarding or replacing the session, or one of their steps.</summary>
    Exit,

    /// <summary>Acknowledging a legality result or flagged changes.</summary>
    Acknowledge,

    /// <summary>Moving focus to a section of the page.</summary>
    Focus,

    /// <summary>Rendering the workspace; caught by the fault boundary.</summary>
    Render,

    /// <summary>Loading the sprite atlas at startup, in builds that include it.</summary>
    Sprites,
}

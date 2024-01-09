namespace PKHeX.Core;

public enum TrashMatch
{
    /// <summary>
    /// Expected under-layer of trash was not found.
    /// </summary>
    NotPresent,

    /// <summary>
    /// Expected under-layer of trash was found.
    /// </summary>
    Present,

    /// <summary>
    /// Displayed string is too long, with all bytes covering the initial trash.
    /// </summary>
    TooLongToTell,

    /// <summary>
    /// Ignored due to other issues that would be flagged by other checks.
    /// </summary>
    Skipped,
}

namespace PKHeX.Core;

public enum TrashMatch
{
    /// <summary>
    /// Expected under-layer of trash was not found.
    /// </summary>
    NotPresent,

    NotEmpty,

    /// <summary>
    /// Displayed string is too long, with all bytes covering the initial trash.
    /// </summary>
    TooLongToTell,

    /// <summary>
    /// Expected under-layer of trash was found.
    /// </summary>
    Present,

    PresentNone,

    PresentSingle,

    PresentMulti,
}

public static class TrashMatchExtensions
{
    public static bool IsPresent(this TrashMatch match) => match >= TrashMatch.Present;
    public static bool IsInvalid(this TrashMatch match) => match < TrashMatch.TooLongToTell;
}

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
    extension(TrashMatch match)
    {
        public bool IsPresent => match >= TrashMatch.Present;
        public bool IsInvalid => match < TrashMatch.TooLongToTell;
    }
}

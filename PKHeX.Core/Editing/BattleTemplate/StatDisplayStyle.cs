namespace PKHeX.Core;

/// <summary>
/// Style to display stat names.
/// </summary>
public enum StatDisplayStyle : sbyte
{
    Custom = -1,

    /// <summary>
    /// Stat names are displayed in abbreviated (2-3 characters) localized text.
    /// </summary>
    Abbreviated,

    /// <summary>
    /// Stat names are displayed in full localized text.
    /// </summary>
    Full,

    /// <summary>
    /// Stat names are displayed as a single character.
    /// </summary>
    /// <remarks>
    /// This is the typical format used by the Japanese community; HABCDS.
    /// </remarks>
    HABCDS,

    /// <summary>
    /// Stat names are displayed without localization; X/X/X/X/X/X
    /// </summary>
    Raw,

    /// <summary>
    /// Stat names are displayed without localization; XX/XX/XX/XX/XX/XX
    /// </summary>
    Raw00,
}

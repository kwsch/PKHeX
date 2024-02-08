namespace PKHeX.Core;

/// <summary>
/// Types of Method H sub-patterns for Gen 3.
/// </summary>
public enum MethodHCondition : byte
{
    /// <summary>
    /// Invalid sentinel value.
    /// </summary>
    Empty = 0,

    /// <summary>
    /// Disallow <see cref="LeadRequired"/> logic to be used.
    /// </summary>
    /// <remarks>Used by <see cref="GameVersion.RS"/> and <see cref="GameVersion.FRLG"/>.</remarks>
    Regular,

    /// <summary>
    /// Allow <see cref="LeadRequired"/> logic to be used.
    /// </summary>
    Emerald,

    /// <summary>
    /// <see cref="Species.Unown"/>
    /// </summary>
    /// <remarks>Only available in <see cref="GameVersion.FRLG"/>, so never <see cref="Emerald"/>.</remarks>
    Unown,
}

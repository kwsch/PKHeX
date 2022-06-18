namespace PKHeX.Core;

/// <summary>
/// Batch Editor Modification result for an individual <see cref="PKM"/>.
/// </summary>
public enum ModifyResult
{
    /// <summary>
    /// The <see cref="PKM"/> has invalid data and is not a suitable candidate for modification.
    /// </summary>
    Invalid,

    /// <summary>
    /// An error was occurred while iterating modifications for this <see cref="PKM"/>.
    /// </summary>
    Error,

    /// <summary>
    /// The <see cref="PKM"/> was skipped due to a matching Filter.
    /// </summary>
    Filtered,

    /// <summary>
    /// The <see cref="PKM"/> was modified.
    /// </summary>
    Modified,
}

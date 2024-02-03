namespace PKHeX.Core;

/// <summary>
/// Stores the previous timestamp and the current timestamp.
/// </summary>
public interface ISecureValueStorage
{
    /// <summary>
    /// Timestamp that the file was previously saved at.
    /// </summary>
    /// <remarks>Useful for remembering the timestamp of a backup, if present.</remarks>
    ulong TimeStampPrevious { get; set; }

    /// <summary>
    /// Timestamp that the file was last saved at.
    /// </summary>
    ulong TimeStampCurrent { get; set; }
}

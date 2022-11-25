namespace PKHeX.Core;

/// <summary>
/// Exposes info about Dynamax potential
/// </summary>
public interface IDynamaxLevel : IDynamaxLevelReadOnly
{
    /// <summary>
    /// Dynamax Level used by <see cref="GameVersion.SWSH"/> format entity data.
    /// </summary>
    new byte DynamaxLevel { get; set; }
}

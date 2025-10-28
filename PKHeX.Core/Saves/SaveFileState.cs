namespace PKHeX.Core;

/// <summary>
/// Tracks information about modifications made to a <see cref="SaveFile"/>
/// </summary>
/// <param name="Exportable">
/// Toggle determining if the save file can be exported.
/// </param>
public sealed record SaveFileState(bool Exportable)
{
    /// <summary>
    /// Mutable value tracking if the save file has been changed. This is set manually by modifications, and not for all modifications.
    /// </summary>
    public bool Edited { get; set; }
}

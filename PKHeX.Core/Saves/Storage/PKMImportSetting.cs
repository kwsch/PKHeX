namespace PKHeX.Core;

/// <summary>
/// Setting to conditionally update PKM properties when importing to a save file.
/// </summary>
public enum PKMImportSetting
{
    /// <summary>
    /// Use whatever the global setting is.
    /// </summary>
    UseDefault,

    /// <summary>
    /// Always update the PKM properties to match the save file.
    /// </summary>
    Update,

    /// <summary>
    /// Never update the PKM properties to match the save file.
    /// </summary>
    Skip,
}

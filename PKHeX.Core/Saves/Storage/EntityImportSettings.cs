using static PKHeX.Core.EntityImportOption;

namespace PKHeX.Core;

/// <summary>
/// Settings to conditionally update entity and save file properties when adapting or importing to a save file.
/// </summary>
public readonly record struct EntityImportSettings(
    EntityImportOption UpdateToSaveFile,
    EntityImportOption UpdatePokeDex,
    EntityImportOption UpdateRecord)
{
    public static EntityImportSettings None => new(Disable, Disable, Disable);
    public static EntityImportSettings All => new(Enable, Enable, Enable);
}

/// <summary>
/// Option to enable/disable conditional updates when adapting or importing to a save file.
/// </summary>
public enum EntityImportOption : byte
{
    /// <summary>
    /// Use whatever the global setting is.
    /// </summary>
    UseDefault,
    Enable,
    Disable,
}

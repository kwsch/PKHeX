namespace PKHeX.Core;

/// <summary>
/// Stores the result from a save detection.
/// </summary>
/// <param name="Type">The save file type detected</param>
/// <param name="SubVersion">Specific game version within the type, or Any if not distinguished</param>
public readonly record struct SaveTypeInfo(SaveFileType Type, GameVersion SubVersion = default, LanguageID Language = default)
{
    /// <summary>
    /// Implicit conversion from SaveTypeInfo to SaveFileType for convenience.
    /// </summary>
    public static implicit operator SaveFileType(SaveTypeInfo info) => info.Type;

    public static implicit operator SaveTypeInfo(SaveFileType type) => new(type);

    /// <summary>
    /// Returns Invalid save type info.
    /// </summary>
    public static SaveTypeInfo Invalid => default;
}

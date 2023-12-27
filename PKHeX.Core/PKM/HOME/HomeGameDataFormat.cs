namespace PKHeX.Core;

/// <summary>
/// Enumerates the various side game data formats that can enter HOME.
/// </summary>
public enum HomeGameDataFormat : byte
{
    None = 0,
    PB7 = 1,
    PK8 = 2,
    PA8 = 3,
    PB8 = 4,
    PK9 = 5,
}

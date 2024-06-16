namespace PKHeX.Core;

/// <summary>
/// Metadata indicating the maximums (and minimums) a type of value can be.
/// </summary>
public interface IGameValueLimit
{
    /// <summary>
    /// Maximum species ID value that can exist.
    /// </summary>
    ushort MaxSpeciesID { get; }

    /// <summary>
    /// Maximum move ID value that can exist.
    /// </summary>
    ushort MaxMoveID { get; }

    /// <summary>
    /// Maximum item ID value that can exist.
    /// </summary>
    int MaxItemID { get; }

    /// <summary>
    /// Maximum ability ID value that can exist.
    /// </summary>
    int MaxAbilityID { get; }

    /// <summary>
    /// Maximum ball ID value that can exist.
    /// </summary>
    int MaxBallID { get; }

    /// <summary>
    /// Maximum Version ID value that can exist.
    /// </summary>
    GameVersion MaxGameID { get; }

    /// <summary>
    /// Minimum Version ID value that can exist.
    /// </summary>
    GameVersion MinGameID { get; }

    /// <summary>
    /// Maximum IV value that is possible.
    /// </summary>
    int MaxIV { get; }

    /// <summary>
    /// Minimum IV value that is possible.
    /// </summary>
    int MaxEV { get; }

    /// <summary>
    /// Maximum length of a string field for a Trainer Name.
    /// </summary>
    int MaxStringLengthTrainer { get; }

    /// <summary>
    /// Maximum length of a string field for a Pokémon Nickname.
    /// </summary>
    int MaxStringLengthNickname { get; }
}

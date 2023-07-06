namespace PKHeX.Core;

/// <summary>
/// Exposes info about all <see cref="IPersonalInfo"/> contained in the object.
/// </summary>
public interface IPersonalTable
{
    /// <summary>
    /// Max Species ID (National Dex) that is stored in the table.
    /// </summary>
    ushort MaxSpeciesID { get; }

    /// <summary>
    /// Gets an index from the inner array.
    /// </summary>
    /// <remarks>Has built in length checks; returns empty (0) entry if out of range.</remarks>
    /// <param name="index">Index to retrieve</param>
    /// <returns>Requested index entry</returns>
    PersonalInfo this[int index] { get; }

    /// <summary>
    /// Alternate way of fetching <see cref="GetFormEntry"/>.
    /// </summary>
    PersonalInfo this[ushort species, byte form] { get; }

    /// <summary>
    /// Gets the <see cref="PersonalInfo"/> entry index for a given <see cref="PKM.Species"/> and <see cref="PKM.Form"/>.
    /// </summary>
    /// <param name="species"><see cref="PKM.Species"/></param>
    /// <param name="form"><see cref="PKM.Form"/></param>
    /// <returns>Entry index for the input criteria</returns>
    int GetFormIndex(ushort species, byte form);

    /// <summary>
    /// Gets the <see cref="PersonalInfo"/> entry for a given <see cref="PKM.Species"/> and <see cref="PKM.Form"/>.
    /// </summary>
    /// <param name="species"><see cref="PKM.Species"/></param>
    /// <param name="form"><see cref="PKM.Form"/></param>
    /// <returns>Entry for the input criteria</returns>
    PersonalInfo GetFormEntry(ushort species, byte form);

    /// <summary>
    /// Checks if the <see cref="PKM.Species"/> is within the bounds of the table.
    /// </summary>
    /// <param name="species"><see cref="PKM.Species"/></param>
    /// <returns>True if present in game</returns>
    bool IsSpeciesInGame(ushort species);

    /// <summary>
    /// Checks if the <see cref="PKM.Species"/> and <see cref="PKM.Form"/> is within the bounds of the table.
    /// </summary>
    /// <param name="species"><see cref="PKM.Species"/></param>
    /// <param name="form"><see cref="PKM.Form"/></param>
    /// <returns>True if present in game</returns>
    bool IsPresentInGame(ushort species, byte form);
}

/// <summary>
/// Generic interface for exposing specific <see cref="IPersonalInfo"/> retrieval methods.
/// </summary>
/// <typeparam name="T">Specific type of <see cref="IPersonalInfo"/> the table contains.</typeparam>
public interface IPersonalTable<out T> where T : IPersonalInfo
{
    T this[int index] { get; }
    T this[ushort species, byte form] { get; }
    T GetFormEntry(ushort species, byte form);
}

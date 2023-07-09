namespace PKHeX.Core;

/// <summary>
/// Interface for a HOME storage system.
/// </summary>
public interface IHomeStorage
{
    /// <summary>
    /// Checks if the given tracker exists in the storage system.
    /// </summary>
    /// <param name="tracker">Tracker to check</param>
    /// <returns>True if the tracker exists, false otherwise.</returns>
    bool Exists(ulong tracker);

    /// <summary>
    /// Gets the HOME entity for the given <see cref="PKM"/>.
    /// </summary>
    /// <typeparam name="T">Input PKM type</typeparam>
    /// <param name="pk">PKM to get the entity for</param>
    /// <returns>HOME entity for the given <see cref="PKM"/>.</returns>
    PKH GetEntity<T>(T pk) where T : PKM;
}

/// <summary>
/// Facade for a HOME storage system.
/// </summary>
public sealed class HomeStorageFacade : IHomeStorage
{
    public bool Exists(ulong tracker) => false;
    public PKH GetEntity<T>(T pk) where T : PKM => PKH.ConvertFromPKM(pk);
}

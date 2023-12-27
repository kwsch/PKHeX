namespace PKHeX.Core;

/// <summary>
/// Simple interface representing a Save File viewer.
/// </summary>
public interface ISaveFileProvider
{
    /// <summary>
    /// Retrieves the save file the <see cref="ISaveFileProvider"/> has control over.
    /// </summary>
    SaveFile SAV { get; }

    /// <summary>
    /// Retrieves the current box the <see cref="ISaveFileProvider"/> has control over.
    /// </summary>
    int CurrentBox { get; }

    /// <summary>
    /// Triggers a refresh of any individual <see cref="PKM"/> view slots.
    /// </summary>
    void ReloadSlots();
}

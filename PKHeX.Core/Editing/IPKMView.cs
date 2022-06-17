namespace PKHeX.Core;

/// <summary>
/// Simple interface representing a <see cref="PKM"/> viewer.
/// </summary>
public interface IPKMView
{
    /// <summary>
    /// Fetches the currently loaded <see cref="PKM"/> data from the viewer.
    /// </summary>
    PKM Data { get; }

    /// <summary>
    /// Indicates if the Viewer supports using Unicode characters or not.
    /// </summary>
    bool Unicode { get; }

    /// <summary>
    /// Indicates if the Viewer is providing extra flexibility or not.
    /// </summary>
    bool HaX { get; }

    /// <summary>
    /// Indicates if the Viewer's controls are changing their values and should avoid triggering other updates.
    /// </summary>
    bool ChangingFields { get; set; }

    /// <summary>
    /// Fetches the currently loaded <see cref="PKM"/> data from the viewer by finishing any pending changes or auto-modifications.
    /// </summary>
    /// <param name="click">Cause the viewer to do extra actions to force validation of its children.</param>
    /// <returns>Prepared <see cref="PKM"/> data from the viewer.</returns>
    PKM PreparePKM(bool click = true);

    /// <summary>
    /// Indicates if the currently loaded <see cref="PKM"/> data is ready for exporting.
    /// </summary>
    bool EditsComplete { get; }

    /// <summary>
    /// Loads a given <see cref="PKM"/> data to the viewer.
    /// </summary>
    /// <param name="pk">Pokémon data to load.</param>
    /// <param name="focus">Cause the viewer to give focus to itself.</param>
    /// <param name="skipConversionCheck">Cause the viewer to skip converting the data. Faster if it is known that the format is the same as the previous format.</param>
    void PopulateFields(PKM pk, bool focus = true, bool skipConversionCheck = false);
}

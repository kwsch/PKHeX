namespace PKHeX.Core;

/// <summary>
/// Plugin interface used by an editor to notify third-party code providers.
/// </summary>
public interface IPlugin
{
    /// <summary>
    /// Plugin Name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Plugin Loading Priority (lowest is initialized first).
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Entry point for the parent to initialize the plugin with provided arguments.
    /// </summary>
    /// <param name="args">Arguments containing objects useful for initializing the plugin.</param>
    void Initialize(params object[] args);

    /// <summary>
    /// Notifies the plugin that a save file was just loaded.
    /// </summary>
    void NotifySaveLoaded();

    /// <summary>
    /// Notifies the plugin that the display language has changed.
    /// </summary>
    /// <param name="language">Short code for language name</param>
    /// <remarks>Useful to translate controls if any added.</remarks>
    void NotifyDisplayLanguageChanged(string language) { }

    /// <summary>
    /// Attempts to load a file using the plugin.
    /// </summary>
    /// <param name="filePath">Path to file to be loaded.</param>
    /// <returns>True if the plugin has handled the file.</returns>
    bool TryLoadFile(string filePath);

    /// <summary>
    /// Retrieves the <see cref="ISaveFileProvider"/> object which can provide a <see cref="SaveFile"/>.
    /// </summary>
    ISaveFileProvider SaveFileEditor { get; }
}

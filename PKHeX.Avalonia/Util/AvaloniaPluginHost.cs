using System;
using PKHeX.Core;

namespace PKHeX.Avalonia.Util;

/// <summary>
/// Provides an <see cref="ISaveFileProvider"/> implementation for plugins in the Avalonia frontend.
/// Acts as a bridge between the plugin system and the Avalonia ViewModel layer.
/// </summary>
public sealed class AvaloniaPluginHost : ISaveFileProvider
{
    private SaveFile _sav;
    private readonly Func<int> _getCurrentBox;
    private readonly Action _reloadSlots;

    /// <summary>
    /// Creates a new plugin host wrapping the current save file state.
    /// </summary>
    /// <param name="sav">The initial save file (can be a blank).</param>
    /// <param name="getCurrentBox">Delegate returning the currently selected box index.</param>
    /// <param name="reloadSlots">Delegate to trigger a full slot refresh in the UI.</param>
    public AvaloniaPluginHost(SaveFile sav, Func<int> getCurrentBox, Action reloadSlots)
    {
        _sav = sav;
        _getCurrentBox = getCurrentBox;
        _reloadSlots = reloadSlots;
    }

    /// <inheritdoc />
    public SaveFile SAV => _sav;

    /// <inheritdoc />
    public int CurrentBox => _getCurrentBox();

    /// <inheritdoc />
    public void ReloadSlots() => _reloadSlots();

    /// <summary>
    /// Updates the hosted save file reference when a new save is loaded.
    /// </summary>
    public void UpdateSaveFile(SaveFile sav) => _sav = sav;
}

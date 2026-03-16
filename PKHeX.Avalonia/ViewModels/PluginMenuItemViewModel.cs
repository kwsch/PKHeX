using System;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels;

/// <summary>
/// Represents a single plugin entry in the Tools > Plugins menu.
/// Exposes the plugin name and a command that calls <see cref="IPlugin.NotifySaveLoaded"/>
/// as a generic "activate" action. Plugins that provide their own UI will hook into
/// <see cref="IPlugin.Initialize"/> to register behaviour.
/// </summary>
public sealed partial class PluginMenuItemViewModel
{
    private readonly IPlugin _plugin;

    /// <summary>
    /// Display name shown in the menu.
    /// </summary>
    public string Name => _plugin.Name;

    public PluginMenuItemViewModel(IPlugin plugin)
    {
        _plugin = plugin;
    }

    /// <summary>
    /// Command invoked when the user clicks the plugin menu item.
    /// Calls <see cref="IPlugin.NotifySaveLoaded"/> which is the standard
    /// WinForms convention for re-activating a plugin's UI.
    /// </summary>
    [RelayCommand]
    private void Activate()
    {
        try
        {
            _plugin.NotifySaveLoaded();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Plugin {_plugin.Name} activation failed: {ex.Message}");
        }
    }
}

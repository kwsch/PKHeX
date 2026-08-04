using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Presentation.Localization;

namespace PKHeX.Presentation.ViewModels;

public partial class AboutViewModel : ViewModelBase
{
    private readonly UpdateCheckCoordinator? _updateCoordinator;

    public string UIVersion { get; }
    public string CoreVersion { get; }
    public string AvaloniaVersion { get; }

    /// <summary>Status line for the manual "Check for Updates" button; empty until a check runs.</summary>
    [ObservableProperty] private string _updateCheckStatus = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CheckForUpdatesCommand))]
    private bool _isCheckingForUpdates;

    /// <summary>Parameterless ctor kept for the XAML designer / ViewLocator preview.</summary>
    public AboutViewModel() : this(null)
    {
    }

    public AboutViewModel(UpdateCheckCoordinator? updateCoordinator)
    {
        _updateCoordinator = updateCoordinator;

        // UI version comes from the host assembly (PKHeX.Avalonia); look it up by name so this
        // Presentation-layer type never references the host or the Avalonia framework directly.
        UIVersion = StripMeta(GetInformationalVersion("PKHeX.Avalonia")
                    ?? typeof(AboutViewModel).Assembly.GetName().Version?.ToString(3)
                    ?? "unknown");

        var coreAssembly = typeof(PKHeX.Core.PKM).Assembly;
        CoreVersion = coreAssembly.GetName().Version?.ToString(3) ?? "unknown";

        AvaloniaVersion = StripMeta(GetInformationalVersion("Avalonia.Base")
                          ?? GetInformationalVersion("Avalonia")
                          ?? "unknown");
    }

    private bool CanCheckForUpdates => !IsCheckingForUpdates && _updateCoordinator is not null;

    [RelayCommand(CanExecute = nameof(CanCheckForUpdates))]
    private async Task CheckForUpdatesAsync()
    {
        if (_updateCoordinator is null)
            return;

        IsCheckingForUpdates = true;
        UpdateCheckStatus = LocalizedStrings.Instance["Update_Checking"];
        try
        {
            var result = await _updateCoordinator.CheckNowAsync();
            UpdateCheckStatus = result.Message;
        }
        finally
        {
            IsCheckingForUpdates = false;
        }
    }

    private static string? GetInformationalVersion(string assemblyName)
    {
        var asm = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => string.Equals(a.GetName().Name, assemblyName, StringComparison.Ordinal));
        return asm?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
               ?? asm?.GetName().Version?.ToString(3);
    }

    private static string StripMeta(string version)
        => version.Contains('+') ? version[..version.IndexOf('+')] : version;
}

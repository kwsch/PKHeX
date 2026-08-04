using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Application.Services;
using PKHeX.Core;
using PKHeX.Presentation.Localization;

namespace PKHeX.Presentation.ViewModels;

public partial class SettingsViewModel : ViewModelBase, ICloseableDialog
{
    private readonly AppSettings _settings;
    private readonly ISettingsStore _settingsStore;
    private readonly IThemeService _themeService;
    private readonly LanguageService _languageService;
    private readonly UpdateCheckCoordinator _updateCoordinator;
    private bool _isLoading;

    public Action? CloseRequested { get; set; }

    /// <summary>Exposed so the settings screen can host a UI-language selector that switches live.</summary>
    public LanguageService LanguageService => _languageService;

    public SettingsViewModel(
        AppSettings settings,
        ISettingsStore settingsStore,
        IThemeService themeService,
        LanguageService languageService,
        UpdateCheckCoordinator updateCoordinator)
    {
        _settings = settings;
        _settingsStore = settingsStore;
        _themeService = themeService;
        _languageService = languageService;
        _updateCoordinator = updateCoordinator;
        Load();
    }

    /// <summary>Status line for the manual "Check for Updates" button; empty until a check runs.</summary>
    [ObservableProperty] private string _updateCheckStatus = string.Empty;

    /// <summary>Disables the button and shows the "checking…" state while a manual check is in flight.</summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CheckForUpdatesCommand))]
    private bool _isCheckingForUpdates;

    private bool CanCheckForUpdates => !IsCheckingForUpdates;

    [RelayCommand(CanExecute = nameof(CanCheckForUpdates))]
    private async Task CheckForUpdatesAsync()
    {
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

    // Startup
    [ObservableProperty] private GameVersion _defaultSaveVersion;
    public IReadOnlyList<GameVersion> GameVersions { get; } = Enum.GetValues<GameVersion>();

    [ObservableProperty] private SaveFileLoadSetting _autoLoadMode;
    public IReadOnlyList<SaveFileLoadSetting> LoadModes { get; } = Enum.GetValues<SaveFileLoadSetting>();

    [ObservableProperty] private bool _forceHaX;
    [ObservableProperty] private bool _showChangelog;
    [ObservableProperty] private bool _checkForUpdatesOnStartup;

    // Backup
    [ObservableProperty] private bool _bakEnabled;
    [ObservableProperty] private bool _bakPrompt;

    // SlotWrite
    [ObservableProperty] private bool _setUpdateDex;
    [ObservableProperty] private bool _setUpdatePKM;
    [ObservableProperty] private bool _setUpdateRecords;
    [ObservableProperty] private bool _modifyUnset;

    // Sprites
    [ObservableProperty] private SpritePreference _spritePreference;
    public IReadOnlyList<SpritePreference> SpritePreferences { get; } = Enum.GetValues<SpritePreference>();

    // Appearance
    [ObservableProperty] private AppTheme _selectedTheme;
    public IReadOnlyList<AppTheme> Themes { get; } = Enum.GetValues<AppTheme>();

    partial void OnSelectedThemeChanged(AppTheme value)
    {
        // Apply (and persist) immediately so the picker previews live, without needing Save.
        // Skip during Load(), which sets the initial value from the already-applied preference.
        if (!_isLoading)
            _themeService.ApplyTheme(value);
    }

    private void Load()
    {
        _isLoading = true;

        DefaultSaveVersion = _settings.Startup.DefaultSaveVersion;
        AutoLoadMode = _settings.Startup.AutoLoadSaveOnStartup;
        ForceHaX = _settings.Startup.ForceHaXOnLaunch;
        ShowChangelog = _settings.Startup.ShowChangelogOnUpdate;
        CheckForUpdatesOnStartup = _settings.Startup.CheckForUpdatesOnStartup;

        BakEnabled = _settings.Backup.BAKEnabled;
        BakPrompt = _settings.Backup.BAKPrompt;

        SetUpdateDex = _settings.SlotWrite.SetUpdateDex;
        SetUpdatePKM = _settings.SlotWrite.SetUpdatePKM;
        SetUpdateRecords = _settings.SlotWrite.SetUpdateRecords;
        ModifyUnset = _settings.SlotWrite.ModifyUnset;

        SpritePreference = _settings.Sprite.SpritePreference;
        SelectedTheme = _themeService.CurrentTheme;

        _isLoading = false;
    }

    [RelayCommand]
    private void Save()
    {
        _settings.Startup.DefaultSaveVersion = DefaultSaveVersion;
        _settings.Startup.AutoLoadSaveOnStartup = AutoLoadMode;
        _settings.Startup.ForceHaXOnLaunch = ForceHaX;
        _settings.Startup.ShowChangelogOnUpdate = ShowChangelog;
        _settings.Startup.CheckForUpdatesOnStartup = CheckForUpdatesOnStartup;

        _settings.Backup.BAKEnabled = BakEnabled;
        _settings.Backup.BAKPrompt = BakPrompt;

        _settings.SlotWrite.SetUpdateDex = SetUpdateDex;
        _settings.SlotWrite.SetUpdatePKM = SetUpdatePKM;
        _settings.SlotWrite.SetUpdateRecords = SetUpdateRecords;
        _settings.SlotWrite.ModifyUnset = ModifyUnset;

        _settings.Sprite.SpritePreference = SpritePreference;

        _settingsStore.Save(_settings);
        _settings.InitializeCore();

        CloseRequested?.Invoke();
    }
}

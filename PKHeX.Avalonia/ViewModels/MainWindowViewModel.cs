using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Avalonia.Services;
using PKHeX.Avalonia.Util;
using PKHeX.Avalonia.ViewModels.Subforms;
using PKHeX.Avalonia.Views.Subforms;
using PKHeX.Avalonia.Settings;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite.Avalonia;

namespace PKHeX.Avalonia.ViewModels;

/// <summary>
/// ViewModel for the main application window. Handles file operations, menu commands, and lifecycle.
/// </summary>
public partial class MainWindowViewModel : ObservableObject
{
    private readonly IDialogService _dialogService;

    /// <summary>
    /// Tracks open non-modal sub-windows (e.g. BoxViewer) so they can be closed on save reload.
    /// </summary>
    private readonly List<Window> _openSubWindows = new();

    /// <summary>
    /// Guards against concurrent invocations of <see cref="LoadFileAsync"/>.
    /// </summary>
    private bool _isLoading;

    /// <summary>
    /// The file path from which the current save file was loaded.
    /// Used to create an automatic backup before overwriting.
    /// </summary>
    private string? _loadedFilePath;

    [ObservableProperty]
    private string _title = "PKHeX - Cross-Platform";

    [ObservableProperty]
    private SaveFile? _saveFile;

    [ObservableProperty]
    private PKMEditorViewModel? _pkmEditor;

    [ObservableProperty]
    private SAVEditorViewModel? _savEditor;

    [ObservableProperty]
    private bool _hasSaveFile;

    [ObservableProperty]
    private string _statusMessage = "Ready. Open a save file to begin.";

    /// <summary>
    /// Indicates whether the currently loaded save file has been modified since it was last saved or loaded.
    /// </summary>
    [ObservableProperty]
    private bool _hasUnsavedChanges;

    /// <summary>
    /// The currently active UI language code, matching <see cref="GameLanguage"/> codes.
    /// </summary>
    [ObservableProperty]
    private string _currentLanguage = GameInfo.CurrentLanguage;

    /// <summary>
    /// All supported UI language codes for the language picker.
    /// </summary>
    public IReadOnlyList<LanguageOption> AvailableLanguages { get; } =
    [
        new("English",     "en"),
        new("日本語",       "ja"),
        new("Français",    "fr"),
        new("Italiano",    "it"),
        new("Deutsch",     "de"),
        new("Español",     "es"),
        new("Español (LA)","es-419"),
        new("한국어",       "ko"),
        new("中文简体",     "zh-Hans"),
        new("中文繁體",     "zh-Hant"),
    ];

    partial void OnCurrentLanguageChanged(string value)
    {
        // Update GameInfo strings and filtered sources
        GameInfo.CurrentLanguage = value;
        if (SaveFile is not null)
            LocalizeUtil.InitializeStrings(value, SaveFile);
        else
            LocalizeUtil.InitializeStrings(value);

        // Persist language preference
        App.Settings.Startup.Language = value;

        // Refresh all combo lists in the PKM editor
        RefreshGameDataAfterLanguageChange();

        StatusMessage = $"Language changed to {value}.";
    }

    private void RefreshGameDataAfterLanguageChange()
    {
        if (PkmEditor is null)
            return;

        // If a save is loaded, re-create the filtered sources and re-initialize
        if (SaveFile is not null)
        {
            GameInfo.FilteredSources = new FilteredGameDataSource(SaveFile, GameInfo.Sources);
            // Notify the editor that list sources changed
            PkmEditor.NotifyListsChanged();
            // Re-populate the current PKM so display names refresh
            if (PkmEditor.Entity is { } pk)
                PkmEditor.PopulateFields(pk);
        }
    }

    /// <summary>
    /// Loaded plugin instances.
    /// </summary>
    public List<IPlugin> Plugins { get; } = [];

    /// <summary>
    /// Plugin load result for managing plugin lifecycle.
    /// </summary>
    private PluginLoadResult? _pluginLoadResult;

    /// <summary>
    /// Plugin host that provides ISaveFileProvider to plugins.
    /// </summary>
    private AvaloniaPluginHost? _pluginHost;

    public MainWindowViewModel() : this(new AvaloniaDialogService())
    {
    }

    public MainWindowViewModel(IDialogService dialogService)
    {
        _dialogService = dialogService;
        PkmEditor = new PKMEditorViewModel();
        SavEditor = new SAVEditorViewModel
        {
            SlotSelected = pk => PkmEditor.PopulateFields(pk),
            GetEditorPKM = () => PkmEditor.PreparePKM(),
            SetStatusMessage = msg => StatusMessage = msg,
            OnModified = () => HasUnsavedChanges = true,
            OpenSettingsEditorCommand = OpenSettingsEditorCommand,
            OpenDatabaseCommand = OpenDatabaseCommand,
            OpenBatchEditorCommand = OpenBatchEditorCommand,
            OpenEncountersCommand = OpenEncountersCommand,
            OpenReportGridCommand = OpenReportGridCommand,
            OpenBoxViewerCommand = OpenBoxViewerCommand,
            OpenMysteryGiftDBCommand = OpenMysteryGiftDBCommand,
            OpenRibbonEditorCommand = OpenRibbonEditorCommand,
            OpenMemoryAmieCommand = OpenMemoryAmieCommand,
            OpenTechRecordEditorCommand = OpenTechRecordEditorCommand,
        };
        LoadPlugins();
    }

    /// <summary>
    /// Loads plugins from the plugins directory relative to the working directory.
    /// </summary>
    private void LoadPlugins()
    {
        if (!App.Settings.Startup.PluginLoadEnable)
            return;

        var pluginPath = Path.Combine(App.WorkingDirectory, "plugins");
        try
        {
            _pluginLoadResult = PluginLoader.LoadPlugins<IPlugin>(pluginPath, Plugins, App.Settings.Startup.PluginLoadMerged);

            // Create the plugin host that provides ISaveFileProvider to plugins
            var blankSav = BlankSaveFile.Get(App.Settings.Startup.DefaultSaveVersion, null);
            _pluginHost = new AvaloniaPluginHost(
                blankSav,
                () => SavEditor?.CurrentBox ?? 0,
                () => SavEditor?.ReloadSlots()
            );

            foreach (var plugin in Plugins.OrderBy(p => p.Priority))
            {
                try
                {
                    plugin.Initialize(_pluginHost, App.CurrentVersion);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to initialize plugin {plugin.Name}: {ex.Message}");
                }
            }
            if (Plugins.Count > 0)
                StatusMessage = $"Loaded {Plugins.Count} plugin(s).";
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to load plugins: {ex.Message}");
        }
    }

    /// <summary>
    /// Notifies all loaded plugins that a save file was loaded.
    /// </summary>
    private void NotifyPluginsSaveLoaded()
    {
        foreach (var plugin in Plugins)
        {
            try
            {
                plugin.NotifySaveLoaded();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Plugin {plugin.Name} failed on NotifySaveLoaded: {ex.Message}");
            }
        }
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task OpenFileAsync()
    {
        var path = await _dialogService.OpenFileAsync("Open Save File");
        if (path is null)
            return;

        await LoadFileAsync(path);
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task SaveFileAsync()
    {
        if (SaveFile is null)
            return;

        var path = await _dialogService.SaveFileAsync("Save File", SaveFile.Metadata.FileName ?? "save");
        if (path is null)
            return;

        try
        {
            // Create an automatic backup of the original file before overwriting
            CreateAutoBackup(path);

            ExportSAV(SaveFile, path);
            HasUnsavedChanges = false;
            StatusMessage = $"Saved to {Path.GetFileName(path)}";
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync("Save Error", ex.Message);
        }
    }

    /// <summary>
    /// Creates an automatic backup of the file at the specified path before it is overwritten.
    /// If the file does not exist yet, no backup is created.
    /// </summary>
    private static void CreateAutoBackup(string originalPath)
    {
        try
        {
            if (!File.Exists(originalPath))
                return;

            var backupDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "PKHeX", "backups");
            Directory.CreateDirectory(backupDir);

            var filename = $"{Path.GetFileNameWithoutExtension(originalPath)}_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
            File.Copy(originalPath, Path.Combine(backupDir, filename), overwrite: true);
        }
        catch
        {
            // Silently fail backup — saving should proceed regardless
        }
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task ExportPKMAsync()
    {
        if (PkmEditor?.Entity is not { } pk)
            return;

        var ext = pk.Extension;
        var path = await _dialogService.SaveFileAsync("Export PKM", $"exported.{ext}");
        if (path is null)
            return;

        try
        {
            File.WriteAllBytes(path, pk.DecryptedBoxData);
            StatusMessage = $"Exported PKM to {Path.GetFileName(path)}";
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync("Export Error", ex.Message);
        }
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task ExportSAVAsync()
    {
        if (SaveFile is null)
            return;

        var path = await _dialogService.SaveFileAsync("Export SAV", SaveFile.Metadata.FileName ?? "save");
        if (path is null)
            return;

        try
        {
            ExportSAV(SaveFile, path);
            StatusMessage = $"Exported SAV to {Path.GetFileName(path)}";
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync("Export Error", ex.Message);
        }
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task DumpAllBoxesAsync()
    {
        if (SaveFile is null)
            return;

        try
        {
            var folder = await _dialogService.OpenFolderAsync("Select output folder for all boxes");
            if (string.IsNullOrEmpty(folder))
                return;

            int dumped = 0;
            for (int box = 0; box < SaveFile.BoxCount; box++)
            {
                var boxDir = Path.Combine(folder, $"Box {box + 1:00}");
                Directory.CreateDirectory(boxDir);
                for (int slot = 0; slot < SaveFile.BoxSlotCount; slot++)
                {
                    var pk = SaveFile.GetBoxSlotAtIndex(box, slot);
                    if (pk is null || pk.Species == 0)
                        continue;

                    var fileName = $"{pk.Species:000}_{pk.Nickname}.{pk.Extension}";
                    foreach (var c in Path.GetInvalidFileNameChars())
                        fileName = fileName.Replace(c, '_');

                    await File.WriteAllBytesAsync(Path.Combine(boxDir, fileName), pk.DecryptedBoxData);
                    dumped++;
                }
            }

            StatusMessage = $"Dumped {dumped} Pokemon from all boxes.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Dump All Boxes error: {ex.Message}";
        }
    }

    /// <summary>
    /// Loads the initial save file and entity from startup arguments.
    /// Called by App.axaml.cs after the main window is shown.
    /// </summary>
    public void LoadInitialSave(SaveFile sav, PKM? entity, string path)
    {
        LoadSaveFile(sav, path);
        if (entity is not null)
            PkmEditor?.PopulateFields(entity);
    }

    public async Task LoadFileAsync(string path)
    {
        if (_isLoading)
            return;
        _isLoading = true;
        try
        {
            var data = await File.ReadAllBytesAsync(path);
            var sav = SaveUtil.GetSaveFile(data);
            if (sav is not null)
            {
                LoadSaveFile(sav, path);
                return;
            }

            // Try loading as PKM
            var pk = EntityFormat.GetFromBytes(data);
            if (pk is not null)
            {
                PkmEditor?.PopulateFields(pk);
                StatusMessage = $"Loaded {pk.Species} from {Path.GetFileName(path)}";
                return;
            }

            await _dialogService.ShowAlertAsync("Unsupported File", "The file format is not recognized.");
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync("Load Error", ex.Message);
        }
        finally
        {
            _isLoading = false;
        }
    }

    /// <summary>
    /// Closes all tracked non-modal sub-windows (e.g. BoxViewer) that may hold references to the old save.
    /// </summary>
    private void CloseSubWindows()
    {
        foreach (var w in _openSubWindows.ToArray())
            w.Close();
        _openSubWindows.Clear();
    }

    private void LoadSaveFile(SaveFile sav, string path)
    {
        CloseSubWindows();
        SaveFile = sav;
        HasSaveFile = true;
        HasUnsavedChanges = false;
        _loadedFilePath = path;

        SpriteUtil.Initialize(sav);
        SavEditor?.LoadSaveFile(sav);
        PkmEditor?.Initialize(sav);

        // Update the plugin host with the new save file
        _pluginHost?.UpdateSaveFile(sav);

        App.Settings.Startup.LoadSaveFile(path);

        Title = $"PKHeX - {sav.GetType().Name} ({Path.GetFileName(path)})";
        StatusMessage = $"Loaded {sav.GetType().Name} - {Path.GetFileName(path)}";

        NotifyPluginsSaveLoaded();
    }

    private static void ExportSAV(SaveFile sav, string path)
    {
        var data = sav.Write();
        File.WriteAllBytes(path, data.ToArray());
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task OpenDatabaseAsync()
    {
        if (SaveFile is null)
            return;

        try
        {
            var dbPath = GetDatabasePath();
            var vm = new DatabaseViewModel(SaveFile, dbPath)
            {
                SlotClicked = pk => PkmEditor?.PopulateFields(pk),
            };
            var view = new DatabaseView { DataContext = vm };

            var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (mainWindow is not null)
            {
                // Start loading in the background, then show
                _ = vm.LoadDatabaseAsync();
                await view.ShowDialog(mainWindow);
                vm.CancelLoad();
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Database error: {ex.Message}";
        }
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task OpenBatchEditorAsync()
    {
        if (SaveFile is null)
            return;

        try
        {
            var vm = new BatchEditorViewModel(SaveFile)
            {
                CurrentBox = SavEditor?.CurrentBox ?? 0,
            };
            var view = new BatchEditorView { DataContext = vm };

            var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (mainWindow is not null)
                await view.ShowDialog(mainWindow);

            if (vm.Modified)
            {
                SavEditor?.ReloadSlots();
                HasUnsavedChanges = true;
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Batch Editor error: {ex.Message}";
        }
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task OpenEncountersAsync()
    {
        if (SaveFile is null)
            return;

        try
        {
            var vm = new EncountersViewModel(SaveFile)
            {
                SlotClicked = pk => PkmEditor?.PopulateFields(pk),
            };
            var view = new EncountersView { DataContext = vm };

            var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (mainWindow is not null)
                await view.ShowDialog(mainWindow);

            vm.CancelSearch();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Encounters error: {ex.Message}";
        }
    }

    private static string GetDatabasePath()
    {
        var pokemon = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PKHeX");
        return Path.Combine(pokemon, "pkmdb");
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task OpenRibbonEditorAsync()
    {
        if (PkmEditor?.Entity is not { } pk)
            return;

        try
        {
            var vm = new RibbonEditorViewModel(pk);
            var view = new RibbonEditorView { DataContext = vm };

            var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (mainWindow is not null)
                await view.ShowDialog(mainWindow);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ribbon Editor error: {ex.Message}";
        }
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task OpenReportGridAsync()
    {
        if (SaveFile is null)
            return;

        try
        {
            var vm = new ReportGridViewModel(SaveFile)
            {
                GetExportPath = () => _dialogService.SaveFileAsync("Export CSV", "report.csv"),
            };

            // Auto-load all boxes
            vm.LoadDataCommand.Execute(null);

            var view = new ReportGridView { DataContext = vm };

            var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (mainWindow is not null)
                await view.ShowDialog(mainWindow);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Report Grid error: {ex.Message}";
        }
    }

    [RelayCommand]
    private void OpenBoxViewer()
    {
        if (SaveFile is null)
            return;

        try
        {
            var slotManager = SavEditor?.SlotManager;
            var vm = new BoxViewerViewModel(SaveFile, SavEditor?.CurrentBox ?? 0)
            {
                SlotSelected = pk => PkmEditor?.PopulateFields(pk),
                SlotManager = slotManager,
            };
            slotManager?.RegisterBoxViewer(vm);

            var view = new BoxViewerView { DataContext = vm };

            var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (mainWindow is not null)
            {
                view.Show(mainWindow); // non-modal
                _openSubWindows.Add(view);
                view.Closed += (_, _) =>
                {
                    _openSubWindows.Remove(view);
                    slotManager?.UnregisterBoxViewer(vm);
                };
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Box Viewer error: {ex.Message}";
        }
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task OpenSettingsEditorAsync()
    {
        try
        {
            var settings = App.Settings;
            var vm = new SettingsEditorViewModel(settings);
            var view = new SettingsEditorView { DataContext = vm };

            var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (mainWindow is not null)
                await view.ShowDialog(mainWindow);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Settings error: {ex.Message}";
        }
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task OpenMysteryGiftDBAsync()
    {
        if (SaveFile is null)
            return;

        try
        {
            var vm = new MysteryGiftDBViewModel(SaveFile)
            {
                SlotClicked = pk => PkmEditor?.PopulateFields(pk),
            };
            var view = new MysteryGiftDBView { DataContext = vm };

            var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (mainWindow is not null)
            {
                _ = vm.LoadDatabaseAsync();
                await view.ShowDialog(mainWindow);
                vm.CancelLoad();
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Mystery Gift DB error: {ex.Message}";
        }
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task OpenMemoryAmieAsync()
    {
        if (PkmEditor?.Entity is not { } pk)
            return;

        try
        {
            if (pk is not ITrainerMemories && pk is not IAffection && pk is not IFullnessEnjoyment)
            {
                await _dialogService.ShowAlertAsync("Not Supported", "This Pokemon does not support memories or affection.");
                return;
            }

            var vm = new MemoryAmieViewModel(pk);
            var view = new MemoryAmieView { DataContext = vm };

            var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (mainWindow is not null)
                await view.ShowDialog(mainWindow);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Memory/Amie error: {ex.Message}";
        }
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task OpenTechRecordEditorAsync()
    {
        if (PkmEditor?.Entity is not { } pk)
            return;

        try
        {
            if (pk is not ITechRecord record)
            {
                await _dialogService.ShowAlertAsync("Not Supported", "This Pokemon does not support Tech Records.");
                return;
            }

            var vm = new TechRecordEditorViewModel(record, pk);
            var view = new TechRecordEditorView { DataContext = vm };

            var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (mainWindow is not null)
                await view.ShowDialog(mainWindow);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Tech Record error: {ex.Message}";
        }
    }

    #region Language

    [RelayCommand]
    private void ChangeLanguage(string? lang)
    {
        if (lang is null || lang == CurrentLanguage)
            return;
        CurrentLanguage = lang;
    }

    #endregion

    #region Undo/Redo

    [RelayCommand]
    private void UndoSlot() => SavEditor?.Undo();

    [RelayCommand]
    private void RedoSlot() => SavEditor?.Redo();

    #endregion

    #region Showdown Import/Export

    private static IClipboard? GetClipboard()
    {
        var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        var mainWindow = lifetime?.MainWindow;
        if (mainWindow is null)
            return null;
        return TopLevel.GetTopLevel(mainWindow)?.Clipboard;
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task ImportShowdownAsync()
    {
        try
        {
            var clipboard = GetClipboard();
            if (clipboard is null)
            {
                await _dialogService.ShowAlertAsync("Clipboard Error", "Could not access clipboard.");
                return;
            }

            var text = await clipboard.GetTextAsync();
            if (string.IsNullOrWhiteSpace(text))
            {
                await _dialogService.ShowAlertAsync("Clipboard Empty", "No text found on the clipboard.");
                return;
            }

            var sets = BattleTemplateTeams.TryGetSets(text);
            var set = sets.FirstOrDefault() ?? new ShowdownSet(string.Empty);

            if (set.Species == 0)
            {
                await _dialogService.ShowAlertAsync("Import Failed", "No valid Showdown set found on the clipboard.");
                return;
            }

            var reformatted = set.Text;
            var confirm = await _dialogService.ShowConfirmAsync("Import Showdown Set?", reformatted);
            if (!confirm)
                return;

            if (PkmEditor?.Entity is null)
            {
                await _dialogService.ShowAlertAsync("No Pokemon", "Load a save file first to import a Showdown set.");
                return;
            }

            var pk = PkmEditor.PreparePKM();
            if (pk is null)
                return;

            pk.ApplySetDetails(set);
            PkmEditor.PopulateFields(pk);
            StatusMessage = "Imported Showdown set from clipboard.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Import Showdown error: {ex.Message}";
        }
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task ExportShowdownAsync()
    {
        try
        {
            if (PkmEditor?.Entity is not { } pk || pk.Species == 0)
            {
                await _dialogService.ShowAlertAsync("No Pokemon", "No Pokemon data to export.");
                return;
            }

            var text = ShowdownParsing.GetShowdownText(pk);
            if (string.IsNullOrWhiteSpace(text))
            {
                await _dialogService.ShowAlertAsync("Export Failed", "Could not generate Showdown text.");
                return;
            }

            var clipboard = GetClipboard();
            if (clipboard is null)
            {
                await _dialogService.ShowAlertAsync("Clipboard Error", "Could not access clipboard.");
                return;
            }

            await clipboard.SetTextAsync(text);
            StatusMessage = "Exported Showdown set to clipboard.";
            await _dialogService.ShowAlertAsync("Showdown Export", text);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Export Showdown error: {ex.Message}";
        }
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task ExportPartyShowdownAsync()
    {
        if (SaveFile is null)
            return;

        try
        {
            var party = SaveFile.PartyData;
            if (party is null) return;
            var text = string.Join("\n\n", party.Where(p => p.Species > 0).Select(ShowdownParsing.GetShowdownText));
            if (string.IsNullOrWhiteSpace(text))
            {
                StatusMessage = "No party Pokemon to export.";
                return;
            }

            var clipboard = GetClipboard();
            if (clipboard is not null)
            {
                try
                {
                    await clipboard.SetTextAsync(text);
                    StatusMessage = "Party exported to clipboard.";
                }
                catch { StatusMessage = "Clipboard unavailable."; }
            }
            else
            {
                StatusMessage = "Clipboard unavailable.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Export Party error: {ex.Message}";
        }
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task ExportBoxShowdownAsync()
    {
        if (SaveFile is null || SavEditor is null)
            return;

        try
        {
            var box = SaveFile.GetBoxData(SavEditor.CurrentBox);
            var text = string.Join("\n\n", box.Where(p => p is not null && p.Species > 0).Select(ShowdownParsing.GetShowdownText));
            if (string.IsNullOrWhiteSpace(text))
            {
                StatusMessage = "No Pokemon in box to export.";
                return;
            }

            var clipboard = GetClipboard();
            if (clipboard is not null)
            {
                try
                {
                    await clipboard.SetTextAsync(text);
                    StatusMessage = $"Box {SavEditor.CurrentBox + 1} exported to clipboard.";
                }
                catch { StatusMessage = "Clipboard unavailable."; }
            }
            else
            {
                StatusMessage = "Clipboard unavailable.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Export Box error: {ex.Message}";
        }
    }

    #endregion

    #region Folder / About

    [RelayCommand]
    private void OpenFolder()
    {
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PKHeX");
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task ShowAboutAsync()
    {
        var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        if (mainWindow is null)
            return;

        var coreVersion = typeof(PKM).Assembly.GetName().Version?.ToString() ?? "unknown";

        var dialog = new Window
        {
            Title = "About PKHeX",
            Width = 320,
            Height = 220,
            CanResize = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new StackPanel
            {
                Margin = new Thickness(24),
                Spacing = 8,
                Children =
                {
                    new TextBlock { Text = "PKHeX", FontSize = 22, FontWeight = global::Avalonia.Media.FontWeight.Bold },
                    new TextBlock { Text = "Pokemon Save Editor", FontSize = 14 },
                    new TextBlock { Text = "Cross-Platform Avalonia Port", FontSize = 12, Opacity = 0.7 },
                    new TextBlock { Text = $"Core Version: {coreVersion}", FontSize = 11 },
                    new TextBlock { Text = "https://github.com/kwsch/PKHeX", FontSize = 11, Opacity = 0.6 },
                }
            }
        };
        await dialog.ShowDialog(mainWindow);
    }

    #endregion

    #region QR Code

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task OpenQRDialogAsync()
    {
        try
        {
            if (PkmEditor?.Entity is not { } pk || pk.Species == 0)
            {
                await _dialogService.ShowAlertAsync("No Pokemon", "No Pokemon data to generate QR code.");
                return;
            }

            var vm = new QRDialogViewModel(pk);
            var view = new QRDialogView { DataContext = vm };

            var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (mainWindow is not null)
                await view.ShowDialog(mainWindow);
        }
        catch (Exception ex)
        {
            StatusMessage = $"QR Code error: {ex.Message}";
        }
    }

    #endregion

    public void HandleFileDrop(string[] files)
    {
        if (files.Length == 0 || _isLoading)
            return;

        _ = LoadFileAsync(files[0]);
    }

    #region Exit

    [RelayCommand]
    private void Exit()
    {
        var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        lifetime?.MainWindow?.Close();
    }

    #endregion

    #region Load Boxes / Dump Box

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task LoadBoxesAsync()
    {
        if (SaveFile is null || SavEditor is null)
            return;

        try
        {
            var paths = await _dialogService.OpenFilesAsync("Select PKM files to load into boxes");
            if (paths is null || paths.Length == 0)
                return;

            int loaded = 0;
            int currentBox = SavEditor.CurrentBox;
            int slot = 0;

            // Find first empty slot in current box
            for (int i = 0; i < SaveFile.BoxSlotCount; i++)
            {
                var existing = SaveFile.GetBoxSlotAtIndex(currentBox, i);
                if (existing is null || existing.Species == 0)
                {
                    slot = i;
                    break;
                }
                slot = i + 1;
            }

            foreach (var path in paths)
            {
                if (slot >= SaveFile.BoxSlotCount)
                    break;

                var data = await File.ReadAllBytesAsync(path);
                var pk = EntityFormat.GetFromBytes(data);
                if (pk is null)
                    continue;

                var converted = EntityConverter.ConvertToType(pk, SaveFile.PKMType, out _);
                if (converted is null)
                    continue;

                SaveFile.SetBoxSlotAtIndex(converted, currentBox, slot);
                loaded++;
                slot++;
            }

            SavEditor.ReloadSlots();
            if (loaded > 0)
                HasUnsavedChanges = true;
            StatusMessage = $"Loaded {loaded} Pokemon into Box {currentBox + 1}.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Load Boxes error: {ex.Message}";
        }
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task DumpBoxAsync()
    {
        if (SaveFile is null || SavEditor is null)
            return;

        try
        {
            var folder = await _dialogService.OpenFolderAsync("Select output folder for box dump");
            if (string.IsNullOrEmpty(folder))
                return;

            var box = SavEditor.CurrentBox;
            int dumped = 0;

            for (int i = 0; i < SaveFile.BoxSlotCount; i++)
            {
                var pk = SaveFile.GetBoxSlotAtIndex(box, i);
                if (pk is null || pk.Species == 0)
                    continue;

                var fileName = $"{pk.Species:000}_{pk.Nickname}.{pk.Extension}";
                // Sanitize filename
                foreach (var c in Path.GetInvalidFileNameChars())
                    fileName = fileName.Replace(c, '_');

                var filePath = Path.Combine(folder, fileName);
                await File.WriteAllBytesAsync(filePath, pk.DecryptedBoxData);
                dumped++;
            }

            StatusMessage = $"Dumped {dumped} Pokemon from Box {box + 1}.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Dump Box error: {ex.Message}";
        }
    }

    #endregion
}

/// <summary>
/// Represents a selectable UI language with a display name and language code.
/// </summary>
public sealed record LanguageOption(string DisplayName, string Code)
{
    public override string ToString() => DisplayName;
}

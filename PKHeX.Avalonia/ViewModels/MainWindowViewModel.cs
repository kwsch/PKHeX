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
    /// Loaded plugin instances.
    /// </summary>
    public List<IPlugin> Plugins { get; } = [];

    /// <summary>
    /// Plugin load result for managing plugin lifecycle.
    /// </summary>
    private PluginLoadResult? _pluginLoadResult;

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
            OpenInventoryCommand = OpenInventoryCommand,
            OpenBoxLayoutCommand = OpenBoxLayoutCommand,
            OpenWondercardCommand = OpenWondercardCommand,
            OpenEventFlagsCommand = OpenEventFlagsCommand,
            OpenSettingsEditorCommand = OpenSettingsEditorCommand,
        };
        LoadPlugins();
    }

    /// <summary>
    /// Loads plugins from the plugins directory relative to the working directory.
    /// </summary>
    private void LoadPlugins()
    {
        var pluginPath = Path.Combine(App.WorkingDirectory, "plugins");
        try
        {
            _pluginLoadResult = PluginLoader.LoadPlugins<IPlugin>(pluginPath, Plugins, false);
            foreach (var plugin in Plugins.OrderBy(p => p.Priority))
            {
                try
                {
                    plugin.Initialize(this);
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

    [RelayCommand]
    private async Task OpenFileAsync()
    {
        var path = await _dialogService.OpenFileAsync("Open Save File");
        if (path is null)
            return;

        await LoadFileAsync(path);
    }

    [RelayCommand]
    private async Task SaveFileAsync()
    {
        if (SaveFile is null)
            return;

        var path = await _dialogService.SaveFileAsync("Save File", SaveFile.Metadata.FileName ?? "save");
        if (path is null)
            return;

        try
        {
            ExportSAV(SaveFile, path);
            StatusMessage = $"Saved to {Path.GetFileName(path)}";
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync("Save Error", ex.Message);
        }
    }

    [RelayCommand]
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

    public async Task LoadFileAsync(string path)
    {
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
    }

    private void LoadSaveFile(SaveFile sav, string path)
    {
        SaveFile = sav;
        HasSaveFile = true;

        SpriteUtil.Initialize(sav);
        SavEditor?.LoadSaveFile(sav);
        PkmEditor?.Initialize(sav);

        Title = $"PKHeX - {sav.GetType().Name} ({Path.GetFileName(path)})";
        StatusMessage = $"Loaded {sav.GetType().Name} - {Path.GetFileName(path)}";

        NotifyPluginsSaveLoaded();
    }

    private static void ExportSAV(SaveFile sav, string path)
    {
        var data = sav.Write();
        File.WriteAllBytes(path, data.ToArray());
    }

    [RelayCommand]
    private async Task OpenInventoryAsync()
    {
        if (SaveFile is null)
            return;

        var vm = new InventoryViewModel(SaveFile);
        var view = new InventoryView { DataContext = vm };

        var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        if (mainWindow is not null)
            await view.ShowDialog(mainWindow);

        if (vm.Modified)
            SavEditor?.ReloadSlots();
    }

    [RelayCommand]
    private async Task OpenDatabaseAsync()
    {
        if (SaveFile is null)
            return;

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

    [RelayCommand]
    private async Task OpenBatchEditorAsync()
    {
        if (SaveFile is null)
            return;

        var vm = new BatchEditorViewModel(SaveFile)
        {
            CurrentBox = SavEditor?.CurrentBox ?? 0,
        };
        var view = new BatchEditorView { DataContext = vm };

        var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        if (mainWindow is not null)
            await view.ShowDialog(mainWindow);

        if (vm.Modified)
            SavEditor?.ReloadSlots();
    }

    [RelayCommand]
    private async Task OpenEncountersAsync()
    {
        if (SaveFile is null)
            return;

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

    private static string GetDatabasePath()
    {
        var pokemon = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PKHeX");
        return Path.Combine(pokemon, "pkmdb");
    }

    [RelayCommand]
    private async Task OpenRibbonEditorAsync()
    {
        if (PkmEditor?.Entity is not { } pk)
            return;

        var vm = new RibbonEditorViewModel(pk);
        var view = new RibbonEditorView { DataContext = vm };

        var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        if (mainWindow is not null)
            await view.ShowDialog(mainWindow);
    }

    [RelayCommand]
    private async Task OpenEventFlagsAsync()
    {
        if (SaveFile is not IEventFlagProvider37 provider)
            return;

        var vm = new EventFlagsViewModel(SaveFile, provider.EventWork, SaveFile.Version);
        var view = new EventFlagsView { DataContext = vm };

        var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        if (mainWindow is not null)
            await view.ShowDialog(mainWindow);
    }

    [RelayCommand]
    private async Task OpenWondercardAsync()
    {
        if (SaveFile is not IMysteryGiftStorageProvider)
            return;

        var vm = new WondercardViewModel(SaveFile);
        var view = new WondercardView { DataContext = vm };

        var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        if (mainWindow is not null)
            await view.ShowDialog(mainWindow);

        if (vm.Modified)
            SavEditor?.ReloadSlots();
    }

    [RelayCommand]
    private async Task OpenBoxLayoutAsync()
    {
        if (SaveFile is null)
            return;

        var vm = new BoxLayoutViewModel(SaveFile);
        var view = new BoxLayoutView { DataContext = vm };

        var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        if (mainWindow is not null)
            await view.ShowDialog(mainWindow);

        if (vm.Modified)
            SavEditor?.ReloadSlots();
    }

    [RelayCommand]
    private async Task OpenReportGridAsync()
    {
        if (SaveFile is null)
            return;

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

    [RelayCommand]
    private void OpenBoxViewer()
    {
        if (SaveFile is null)
            return;

        var vm = new BoxViewerViewModel(SaveFile, SavEditor?.CurrentBox ?? 0)
        {
            SlotSelected = pk => PkmEditor?.PopulateFields(pk),
        };
        var view = new BoxViewerView { DataContext = vm };

        var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        if (mainWindow is not null)
            view.Show(mainWindow); // non-modal
    }

    [RelayCommand]
    private async Task OpenSettingsEditorAsync()
    {
        var settings = App.Settings;
        var vm = new SettingsEditorViewModel(settings);
        var view = new SettingsEditorView { DataContext = vm };

        var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        if (mainWindow is not null)
            await view.ShowDialog(mainWindow);
    }

    [RelayCommand]
    private async Task OpenEventFlags2Async()
    {
        if (SaveFile is null)
            return;

        // Gen 2: IEventFlagArray + IEventWorkArray<byte>
        if (SaveFile is IEventFlagArray flagArray and IEventWorkArray<byte> workArray and SAV2)
        {
            var vm = new EventFlags2ViewModel(SaveFile, flagArray, workArray, SaveFile.Version);
            var view = new EventFlags2View { DataContext = vm };
            var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (mainWindow is not null)
                await view.ShowDialog(mainWindow);
            return;
        }

        // Gen 8 (BDSP): SAV8BS has FlagWork8b implementing IEventFlag + ISystemFlag + IEventWork<int>
        if (SaveFile is SAV8BS bdsp)
        {
            var fw = bdsp.FlagWork;
            var vm = new EventFlags2ViewModel(SaveFile, fw, fw, fw, SaveFile.Version);
            var view = new EventFlags2View { DataContext = vm };
            var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (mainWindow is not null)
                await view.ShowDialog(mainWindow);
            return;
        }
    }

    [RelayCommand]
    private async Task OpenMysteryGiftDBAsync()
    {
        if (SaveFile is null)
            return;

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

    [RelayCommand]
    private async Task OpenMemoryAmieAsync()
    {
        if (PkmEditor?.Entity is not { } pk)
            return;

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

    [RelayCommand]
    private async Task OpenTechRecordEditorAsync()
    {
        if (PkmEditor?.Entity is not { } pk)
            return;

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

    #region Showdown Import/Export

    private static IClipboard? GetClipboard()
    {
        var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        var mainWindow = lifetime?.MainWindow;
        if (mainWindow is null)
            return null;
        return TopLevel.GetTopLevel(mainWindow)?.Clipboard;
    }

    [RelayCommand]
    private async Task ImportShowdownAsync()
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

    [RelayCommand]
    private async Task ExportShowdownAsync()
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

    #endregion

    #region QR Code

    [RelayCommand]
    private async Task OpenQRDialogAsync()
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

    #endregion

    public void HandleFileDrop(string[] files)
    {
        if (files.Length == 0)
            return;

        _ = LoadFileAsync(files[0]);
    }
}

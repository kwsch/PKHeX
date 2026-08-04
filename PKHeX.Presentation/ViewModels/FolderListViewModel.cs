using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class FolderListViewModel : ViewModelBase
{
    private readonly ISaveFileGateway _saveFileService;
    private readonly AppSettings _settings;
    private readonly IDialogService _dialogService;
    private readonly CancellationTokenSource _cts = new();

    [ObservableProperty]
    private ObservableCollection<SaveFilePreviewViewModel> _recentSaves = [];

    [ObservableProperty]
    private ObservableCollection<SaveFilePreviewViewModel> _backupSaves = [];

    [ObservableProperty]
    private SaveFilePreviewViewModel? _selectedRecentSave;
    
    [ObservableProperty]
    private SaveFilePreviewViewModel? _selectedBackupSave;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusText = "Ready";
    
    // Filter
    [ObservableProperty]
    private string _filterText = string.Empty;

    public FolderListViewModel(ISaveFileGateway saveFileService, AppSettings settings, IDialogService dialogService)
    {
        _saveFileService = saveFileService;
        _settings = settings;
        _dialogService = dialogService;

        LoadSavesAsync();
    }

    partial void OnFilterTextChanged(string value)
    {
        // TODO: Implement filtering
    }

    private async void LoadSavesAsync()
    {
        IsLoading = true;
        StatusText = "Scanning for save files...";

        try
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var backupPath = Path.Combine(baseDir, "bak");
            var recentFiles = _settings.Startup.RecentlyLoaded;
            var extraPaths = _settings.Backup.OtherBackupPaths;

            var (validRecents, validBackups) = await Task.Run(() =>
            {
                // 1. Recent Saves
                var validRecents = new List<SaveFilePreviewViewModel>();
                foreach (var path in recentFiles)
                {
                    if (File.Exists(path))
                    {
                        var info = new FileInfo(path);
                        if (SaveUtil.IsSizeValid(info.Length))
                        {
                            var sav = SaveUtil.GetSaveFile(path);
                            if (sav != null)
                                validRecents.Add(new SaveFilePreviewViewModel(sav));
                        }
                    }
                }

                // 2. Backup Saves
                var validBackups = new List<SaveFilePreviewViewModel>();
                var allBackupPaths = new List<string> { backupPath };
                allBackupPaths.AddRange(extraPaths);
                
                foreach (var folder in allBackupPaths)
                {
                    if (!Directory.Exists(folder)) continue;
                    
                    var files = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);
                    foreach (var f in files)
                    {
                         var info = new FileInfo(f);
                         if (SaveUtil.IsSizeValid(info.Length))
                         {
                             var sav = SaveUtil.GetSaveFile(f);
                             if (sav != null)
                                validBackups.Add(new SaveFilePreviewViewModel(sav));
                         }
                    }
                }

                return (validRecents, validBackups);
            });

            // Back on the UI thread (async command continuation) — assign the observable collections.
            RecentSaves = new ObservableCollection<SaveFilePreviewViewModel>(validRecents);
            BackupSaves = new ObservableCollection<SaveFilePreviewViewModel>(validBackups);
            StatusText = $"Loaded {RecentSaves.Count} recent, {BackupSaves.Count} backups.";
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task OpenSave(SaveFilePreviewViewModel? vm)
    {
        if (vm == null) return;
        
        // Use SaveFileService to load
        // But we already loaded it? 
        // SaveFileService usually takes a path and loads it.
        // Or we can pass the object if we have it fully loaded.
        // SaveUtil.GetSaveFile returns 'SaveFile' or 'SaveFile<T>'.
        
        // If we kept the SaveFile object in ViewModel (we should), pass it.
        // But usually we want to set it as CurrentSave.
        // Assuming _saveFileService has a method to set current save or load from path.
        
        await _saveFileService.LoadSaveFileAsync(vm.FilePath);
        
        // Close dialog/window? 
        // This is likely a dialog or a tab. If dialog, we can signal close.
        CloseRequested?.Invoke();
    }
    
    public event Action? CloseRequested;

    [RelayCommand]
    private void OpenFolder(SaveFilePreviewViewModel? vm)
    {
        if (vm == null || string.IsNullOrEmpty(vm.FilePath)) return;
        
        var folder = Path.GetDirectoryName(vm.FilePath);
        if (Directory.Exists(folder))
        {
            // Cross platform open folder?
            // Process.Start... or ILauncher?
            // For now simple Process.Start(folder) might work on Windows, less on Mac.
            // Avalonia usually needs a launcher service.
            // We'll leave a TODO or try simple dotnet ways.
            try 
            {
                if (OperatingSystem.IsWindows())
                    Process.Start("explorer.exe", folder);
                else if (OperatingSystem.IsMacOS())
                    Process.Start("open", folder);
                else if (OperatingSystem.IsLinux())
                    Process.Start("xdg-open", folder);
            }
            catch {}
        }
    }
}

public class SaveFilePreviewViewModel : ViewModelBase
{
    public string FileName { get; }
    public string FilePath { get; }
    public string Version { get; }
    public string TrainerName { get; }
    public string PlayTime { get; }
    public DateTime LastModified { get; }
    public string BadgeCount { get; }
    
    public SaveFilePreviewViewModel(SaveFile sav)
    {
        FilePath = sav.Metadata.FilePath ?? "Unknown";
        FileName = Path.GetFileName(FilePath);
        Version = sav.Version.ToString();
        TrainerName = sav.OT;
        // PlayTime = sav.PlayTime;
        PlayTime = ""; // sav.PlayTime not available in SaveFile base?
        LastModified = File.Exists(FilePath) ? File.GetLastWriteTime(FilePath) : DateTime.MinValue;
        // BadgeCount? 
        // Not in simple metadata usually? Or implied.
        BadgeCount = ""; 
    }
}

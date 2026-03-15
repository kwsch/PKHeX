using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a folder entry in the folder list.
/// </summary>
public class FolderEntry
{
    public string DisplayName { get; }
    public string FullPath { get; }
    public bool Exists { get; }

    public FolderEntry(string displayName, string fullPath)
    {
        DisplayName = displayName;
        FullPath = fullPath;
        Exists = Directory.Exists(fullPath);
    }
}

/// <summary>
/// Model for a save file preview entry.
/// </summary>
public class SaveFileEntry
{
    public string FileName { get; }
    public string FilePath { get; }
    public string FileSize { get; }
    public string LastModified { get; }

    public SaveFileEntry(string filePath)
    {
        FilePath = filePath;
        FileName = Path.GetFileName(filePath);
        var info = new FileInfo(filePath);
        FileSize = info.Exists ? $"{info.Length:N0} bytes" : "N/A";
        LastModified = info.Exists ? info.LastWriteTime.ToString("g") : "N/A";
    }
}

/// <summary>
/// ViewModel for the Folder List browser subform.
/// Shows save file locations and recent/backup saves.
/// </summary>
public partial class FolderListViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _modified;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private int _selectedTabIndex;

    public ObservableCollection<FolderEntry> Folders { get; } = [];

    public ObservableCollection<SaveFileEntry> RecentFiles { get; } = [];

    [ObservableProperty]
    private ObservableCollection<SaveFileEntry> _filteredRecentFiles = [];

    public ObservableCollection<SaveFileEntry> BackupFiles { get; } = [];

    [ObservableProperty]
    private ObservableCollection<SaveFileEntry> _filteredBackupFiles = [];

    public string WindowTitle => "Save File Folder List";

    /// <summary>
    /// Creates the view model with the given folder paths and backup directory.
    /// </summary>
    public FolderListViewModel(string[] folderPaths, string backupPath)
    {
        foreach (var path in folderPaths)
        {
            if (Directory.Exists(path))
                Folders.Add(new FolderEntry(Path.GetFileName(path), path));
        }

        // Load recent files from common save locations
        foreach (var folder in Folders)
        {
            if (!Directory.Exists(folder.FullPath))
                continue;
            try
            {
                foreach (var file in Directory.EnumerateFiles(folder.FullPath, "*", SearchOption.TopDirectoryOnly).Take(50))
                    RecentFiles.Add(new SaveFileEntry(file));
            }
            catch (UnauthorizedAccessException) { }
            catch (IOException) { }
        }

        // Load backup files
        if (Directory.Exists(backupPath))
        {
            try
            {
                foreach (var file in Directory.EnumerateFiles(backupPath, "*", SearchOption.AllDirectories).Take(100))
                    BackupFiles.Add(new SaveFileEntry(file));
            }
            catch (UnauthorizedAccessException) { }
            catch (IOException) { }
        }

        FilteredRecentFiles = new ObservableCollection<SaveFileEntry>(RecentFiles);
        FilteredBackupFiles = new ObservableCollection<SaveFileEntry>(BackupFiles);
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            FilteredRecentFiles = new ObservableCollection<SaveFileEntry>(RecentFiles);
            FilteredBackupFiles = new ObservableCollection<SaveFileEntry>(BackupFiles);
            return;
        }

        FilteredRecentFiles = new ObservableCollection<SaveFileEntry>(
            RecentFiles.Where(f => f.FileName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                                || f.FilePath.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
        FilteredBackupFiles = new ObservableCollection<SaveFileEntry>(
            BackupFiles.Where(f => f.FileName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                                || f.FilePath.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
    }

    /// <summary>
    /// Opens the selected folder in the system file manager.
    /// </summary>
    [RelayCommand]
    private void OpenFolder(FolderEntry? folder)
    {
        if (folder is null || !Directory.Exists(folder.FullPath))
            return;

        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = folder.FullPath,
                UseShellExecute = true,
            });
        }
        catch { /* ignore failures to open folder */ }
    }
}

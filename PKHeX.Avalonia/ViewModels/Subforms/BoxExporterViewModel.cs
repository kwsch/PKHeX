using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the Box Exporter subform.
/// Exports box contents to files with configurable naming and scope.
/// </summary>
public partial class BoxExporterViewModel : ObservableObject
{
    private readonly SaveFile _sav;
    private readonly IFileNamer<PKM>[] _namers = [.. EntityFileNamer.AvailableNamers];

    [ObservableProperty]
    private bool _modified;

    [ObservableProperty]
    private int _selectedNamerIndex;

    [ObservableProperty]
    private bool _exportAll = true;

    [ObservableProperty]
    private string _statusText = string.Empty;

    /// <summary>Available file namer names for the combo box.</summary>
    public ObservableCollection<string> NamerNames { get; } = [];

    /// <summary>Callback to request a folder path from the view.</summary>
    public Func<Task<string?>>? GetFolderPath { get; set; }

    public BoxExporterViewModel(SaveFile sav)
    {
        _sav = sav;
        foreach (var namer in _namers)
            NamerNames.Add(namer.Name);
        if (NamerNames.Count > 0)
            SelectedNamerIndex = 0;
    }

    /// <summary>
    /// Exports boxes to the selected folder.
    /// </summary>
    [RelayCommand]
    private async Task ExportAsync()
    {
        if (GetFolderPath is null)
            return;

        var folder = await GetFolderPath();
        if (string.IsNullOrEmpty(folder))
            return;

        var namer = _namers[SelectedNamerIndex];
        var settings = new BoxExportSettings
        {
            Scope = ExportAll ? BoxExportScope.All : BoxExportScope.Current,
        };

        int ctr = BoxExport.Export(_sav, folder, namer, settings);
        if (ctr < 0)
        {
            StatusText = "Export failed: invalid box data.";
            return;
        }

        StatusText = $"Exported {ctr} files to {folder}";
        Modified = true;
    }
}

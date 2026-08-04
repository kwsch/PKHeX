using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;
using PKHeX.Presentation.Localization;

namespace PKHeX.Presentation.ViewModels;

public partial class BoxExporterViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly IDialogService _dialogService;
    private readonly IFileNamer<PKM>[] _namers;
    private readonly BoxExportSettings _settings;

    [ObservableProperty]
    private IEnumerable<string> _namerList;

    [ObservableProperty]
    private string _selectedNamer;

    // Export Options exposed as properties
    [ObservableProperty]
    private bool _exportPrivate;
    
    [ObservableProperty]
    private bool _regenPID;

    public Action? CloseRequested { get; set; }

    public BoxExporterViewModel(SaveFile sav, IDialogService dialogService)
    {
        _sav = sav;
        _dialogService = dialogService;
        _namers = EntityFileNamer.AvailableNamers.ToArray();
        _namerList = _namers.Select(n => n.Name);
        _selectedNamer = _namers[0].Name;

        // Load settings?
        // Using manual defaults for now, or could link to AppSettings if we exposed SlotExport there
        _settings = new BoxExportSettings();

        // Bind initial values from settings if needed
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task ExportAsync()
    {
        // Get folder
        var folder = await _dialogService.OpenFolderAsync("Select Export Folder");
        if (string.IsNullOrEmpty(folder)) return;

        var namer = _namers.FirstOrDefault(n => n.Name == SelectedNamer);
        if (namer is null)
        {
            await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["BoxExporter_ExportErrorTitle"], LocalizedStrings.Instance["BoxExporter_NoNamerSelected"]);
            return;
        }

        // Update settings object
        // Assuming BoxExportSettings is compatible or we use a manual implementation
        // BoxExport.Export uses BoxExportSettings
        // We can just construct one
        var settings = new BoxExportSettings
        {
             // Map properties if we added them to UI
        };

        try
        {
            int count = BoxExport.Export(_sav, folder, namer, settings);
            await _dialogService.ShowInformationAsync(LocalizedStrings.Instance["BoxExporter_ExportCompleteTitle"], LocalizedStrings.Instance.Format("BoxExporter_ExportCompleteMessage", count, folder));
            CloseRequested?.Invoke();
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["BoxExporter_ExportErrorTitle"], ex.Message);
        }
    }
}

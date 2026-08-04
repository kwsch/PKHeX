using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Application.Abstractions;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Helps load save files that fail auto-detection by letting the user force a specific
/// <see cref="SaveFileType"/>, <see cref="GameVersion"/>, <see cref="LanguageID"/>, and recognition
/// handler. Mirrors the upstream WinForms <c>SaveHandlerTroubleshooter</c>, using
/// <see cref="SaveUtil.TryGetSaveFileHandler(Memory{byte}, out SaveFile, string, ISaveHandler, SaveTypeInfo)"/>
/// as the force-load entry point. Intentionally not gated on a loaded save — its whole purpose is to
/// open a save the normal loader cannot.
/// </summary>
public partial class SaveHandlerTroubleshooterViewModel : ViewModelBase, ICloseableDialog
{
    private readonly IDialogService _dialogService;
    private readonly ISaveFileGateway _saveFileService;

    public Action? CloseRequested { get; set; }

    /// <summary>The force-loaded save, exposed so callers/tests can read the result after a successful load.</summary>
    public SaveFile? LoadedSave { get; private set; }

    public SaveHandlerTroubleshooterViewModel(IDialogService dialogService, ISaveFileGateway saveFileService)
    {
        _dialogService = dialogService;
        _saveFileService = saveFileService;

        SaveTypes = BuildSaveTypes();
        Handlers = BuildHandlers();
        Languages = BuildLanguages();

        SelectedSaveType = SaveTypes[0];
        SelectedHandler = Handlers[0];
        SelectedLanguage = Languages.FirstOrDefault(l => l.Value == LanguageID.English) ?? Languages[0];

        UpdateVersionChoices();
    }

    // — File selection —

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoadCommand))]
    private string _filePath = string.Empty;

    public string FileName => string.IsNullOrWhiteSpace(FilePath) ? "(no file selected)" : Path.GetFileName(FilePath);

    partial void OnFilePathChanged(string value) => OnPropertyChanged(nameof(FileName));

    // — Combo sources —

    public IReadOnlyList<SaveTypeOption> SaveTypes { get; }
    public IReadOnlyList<HandlerOption> Handlers { get; }
    public IReadOnlyList<LanguageOption> Languages { get; }

    [ObservableProperty] private ObservableCollection<VersionOption> _versions = [];

    // — Selected values —

    [ObservableProperty] private SaveTypeOption _selectedSaveType = null!;
    [ObservableProperty] private VersionOption? _selectedVersion;
    [ObservableProperty] private HandlerOption _selectedHandler = null!;
    [ObservableProperty] private LanguageOption _selectedLanguage = null!;

    partial void OnSelectedSaveTypeChanged(SaveTypeOption value) => UpdateVersionChoices();

    // — Status —

    [ObservableProperty] private string _status = "Select a save file and (optionally) force a type, version, language, or handler, then press Load.";
    [ObservableProperty] private bool _lastLoadSucceeded;

    private void UpdateVersionChoices()
    {
        var type = SelectedSaveType?.Value ?? SaveFileType.None;
        var list = new List<VersionOption> { new("Any", GameVersion.Any) };
        if (type != SaveFileType.None)
        {
            list.AddRange(GameUtil.GameVersions
                .Where(v => v.SaveFileType == type)
                .Distinct()
                .Select(v => new VersionOption(GetVersionDisplayName(v), v)));
        }

        Versions = new ObservableCollection<VersionOption>(list);
        SelectedVersion = Versions[0];
    }

    [RelayCommand]
    private async Task BrowseAsync()
    {
        var path = await _dialogService.OpenFileAsync("Select Save File to Troubleshoot", FileDialogFilters.OpenSaveFile);
        if (!string.IsNullOrEmpty(path))
            FilePath = path;
    }

    private bool CanLoad() => !string.IsNullOrWhiteSpace(FilePath);

    [RelayCommand(CanExecute = nameof(CanLoad))]
    private async Task LoadAsync()
    {
        LastLoadSucceeded = false;
        LoadedSave = null;

        var path = FilePath.Trim();
        if (string.IsNullOrWhiteSpace(path))
        {
            Status = "No file selected.";
            return;
        }

        if (!File.Exists(path))
        {
            Status = $"File does not exist:\n{path}";
            return;
        }

        byte[] data;
        try
        {
            data = File.ReadAllBytes(path);
        }
        catch (Exception ex)
        {
            Status = $"Could not read the file (it may be in use):\n{path}\n\n{ex.Message}";
            return;
        }

        SaveFile? sav;
        try
        {
            sav = ForceLoad(data, path);
        }
        catch (Exception ex)
        {
            Status = $"The forced parameters threw while constructing the save:\n{ex.Message}";
            return;
        }

        if (sav is null)
        {
            Status = "Force-load failed: no save file could be built from the chosen handler/type. " +
                     "Try a different Type, Version, or Handler. " +
                     $"(File size: {data.Length} bytes.)";
            return;
        }

        LoadedSave = sav;
        LastLoadSucceeded = true;
        Status = $"Success — loaded as {sav.GetType().Name} ({sav.Version}). Opening it in the main window…";

        // Integrate: hand the constructed save off to the main open flow.
        _saveFileService.OpenLoadedSave(sav, path);

        await Task.Yield();
        CloseRequested?.Invoke();
    }

    /// <summary>
    /// Force-constructs a <see cref="SaveFile"/> from the chosen handler and type info. Returns null on failure.
    /// Public so callers/tests can exercise the exact force-load path the Load command uses.
    /// </summary>
    public SaveFile? ForceLoad(Memory<byte> data, string? path)
    {
        var handler = SelectedHandler?.Handler ?? new AutoSaveHandler();

        // "Default/auto" type → defer to full auto-detection (typeInfo carries no concrete type to build).
        if (SelectedSaveType is null || SelectedSaveType.Value == SaveFileType.None)
            return SaveUtil.TryGetSaveFileHandler(data, out var auto, path, handler) ? auto : null;

        var typeInfo = new SaveTypeInfo(
            SelectedSaveType.Value,
            SelectedVersion?.Value ?? GameVersion.Any,
            SelectedLanguage?.Value ?? LanguageID.None);

        return SaveUtil.TryGetSaveFileHandler(data, out var result, path, handler, typeInfo) ? result : null;
    }

    private static IReadOnlyList<SaveTypeOption> BuildSaveTypes()
    {
        var list = new List<SaveTypeOption> { new("Default (auto-detect)", SaveFileType.None) };
        list.AddRange(Enum.GetValues<SaveFileType>()
            .Where(t => t != SaveFileType.None)
            .Select(t => new SaveTypeOption(t.ToString(), t)));
        return list;
    }

    private static IReadOnlyList<HandlerOption> BuildHandlers()
    {
        var list = new List<HandlerOption> { new("Default (no special handling)", new AutoSaveHandler()) };
        list.AddRange(SaveUtil.Handlers.Select(h => new HandlerOption(GetHandlerDisplayName(h), h)));
        return list;
    }

    private static IReadOnlyList<LanguageOption> BuildLanguages() =>
        Enum.GetValues<LanguageID>().Select(l => new LanguageOption(l.ToString(), l)).ToList();

    private static string GetVersionDisplayName(GameVersion version)
    {
        var name = GameInfo.GetVersionName(version);
        return string.IsNullOrWhiteSpace(name) ? version.ToString() : name;
    }

    private static string GetHandlerDisplayName(ISaveHandler handler)
    {
        const string prefix = "SaveHandler";
        var name = handler.GetType().Name;
        return name.StartsWith(prefix, StringComparison.Ordinal) ? name[prefix.Length..] : name;
    }

    // — Option records (ToString drives ComboBox display) —

    public sealed record SaveTypeOption(string Text, SaveFileType Value)
    {
        public override string ToString() => Text;
    }

    public sealed record VersionOption(string Text, GameVersion Value)
    {
        public override string ToString() => Text;
    }

    public sealed record HandlerOption(string Text, ISaveHandler Handler)
    {
        public override string ToString() => Text;
    }

    public sealed record LanguageOption(string Text, LanguageID Value)
    {
        public override string ToString() => Text;
    }

    /// <summary>
    /// Pass-through handler that recognizes any size and returns the input unsplit. Equivalent to the
    /// upstream <c>SaveHandlerDefault</c>; selecting it means "no special header/footer handling".
    /// </summary>
    private sealed class AutoSaveHandler : ISaveHandler
    {
        public bool IsRecognized(long size) => true;
        public SaveHandlerSplitResult TrySplit(Memory<byte> input) => new(input, default, default, this);
        public void Finalize(Span<byte> input) { }
    }
}

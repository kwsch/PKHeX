using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;
using PKHeX.Presentation.Localization;

namespace PKHeX.Presentation.ViewModels;

public partial class MedalEditorViewModel : ViewModelBase
{
    private const int MaxMedals = 255;
    private const string DateFormat = "yyyy-MM-dd";
    private const string MedalListExtension = "ml5";

    private readonly SaveFile _sav;
    private readonly IDialogService? _dialogService;
    private readonly MedalList5? _medals;
    private HabitatList5? _habitat;

    private bool _suppressSettingsWrite;

    public MedalEditorViewModel(SaveFile sav) : this(sav, null) { }

    public MedalEditorViewModel(SaveFile sav, IDialogService? dialogService)
    {
        _sav = sav;
        _dialogService = dialogService;

        if (sav is SAV5B2W2 b2w2)
        {
            _medals = b2w2.Medals;
            _habitat = _medals.HabitatList;
            IsSupported = true;
            BuildEnumChoices();
            LoadData();
        }
    }

    public bool IsSupported { get; }
    public string GameInfo => $"{_sav.Version} - Medals (255)";

    public string[] StateChoices { get; private set; } = [];
    public string[] HabitatCompletionChoices { get; private set; } = [];
    public string[] RankChoices { get; private set; } = [];
    public string[] EncounterTypeChoices { get; private set; } = [];

    [ObservableProperty]
    private ObservableCollection<MedalCategoryViewModel> _categories = [];

    [ObservableProperty]
    private ObservableCollection<MedalItemViewModel> _medalRows = [];

    [ObservableProperty]
    private ObservableCollection<HabitatRowViewModel> _habitatRows = [];

    [ObservableProperty]
    private int _totalObtained;

    // Medal list settings.
    [ObservableProperty]
    private int _pinnedMedal;

    [ObservableProperty]
    private int _rankIndex;

    [ObservableProperty]
    private bool _isTutorialComplete;

    // Habitat settings.
    [ObservableProperty]
    private bool _habitatTutorialViewed;

    [ObservableProperty]
    private bool _habitatTutorialCompleteCapture;

    [ObservableProperty]
    private int _lastEncounterTypeIndex;

    private void BuildEnumChoices()
    {
        StateChoices = Enum.GetNames<MedalState5>();
        HabitatCompletionChoices = Enum.GetNames<HabitatCompletion5>();
        RankChoices = Enum.GetNames<MedalRank5>();
        EncounterTypeChoices = Enum.GetNames<HabitatEncounterType5>();
    }

    private void LoadData()
    {
        LoadMedalData();
        LoadHabitatData();
        LoadHabitatSettings();
    }

    private void LoadMedalData()
    {
        if (_medals is null) return;

        Categories.Clear();
        MedalRows.Clear();
        TotalObtained = 0;

        var ranges = new (int start, int end, string name)[]
        {
            (0, 6, "Special"),
            (7, 104, "Adventure"),
            (105, 160, "Battle"),
            (161, 235, "Entertainment"),
            (236, 254, "Challenge"),
        };

        foreach (var (start, end, name) in ranges)
        {
            var cat = new MedalCategoryViewModel(name);
            for (int i = start; i <= end; i++)
            {
                var vm = new MedalItemViewModel(this, i);
                cat.Medals.Add(vm);
                MedalRows.Add(vm);
                if (vm.IsObtained) TotalObtained++;
            }
            Categories.Add(cat);
        }

        _suppressSettingsWrite = true;
        PinnedMedal = _medals.PinnedMedal;
        RankIndex = (int)_medals.Rank;
        IsTutorialComplete = _medals.IsTutorialComplete;
        _suppressSettingsWrite = false;
    }

    private void LoadHabitatData()
    {
        if (_habitat is null) return;

        HabitatRows.Clear();
        for (int i = 0; i < HabitatList5.HabitatCount; i++)
            HabitatRows.Add(new HabitatRowViewModel(this, i));
    }

    private void LoadHabitatSettings()
    {
        if (_habitat is not { } habitat) return;

        _suppressSettingsWrite = true;
        HabitatTutorialViewed = habitat.IsTutorialViewed;
        HabitatTutorialCompleteCapture = habitat.IsTutorialCompleteCapture;
        LastEncounterTypeIndex = (int)habitat.LastEncounterType;
        _suppressSettingsWrite = false;
    }

    internal Medal5 GetMedal(int index) => _medals![index];

    internal HabitatStatus5 GetHabitat(int index) => _habitat!.GetHabitat(index);

    internal void MarkEdited() => _sav.State.Edited = true;

    internal void RecountObtained()
    {
        if (_medals is null) return;
        TotalObtained = _medals.GetCountObtained();
    }

    partial void OnPinnedMedalChanged(int value)
    {
        if (_suppressSettingsWrite || _medals is null) return;
        if (value is < 0 or > MaxMedals) return;
        _medals.PinnedMedal = (byte)value;
        MarkEdited();
    }

    partial void OnRankIndexChanged(int value)
    {
        if (_suppressSettingsWrite || _medals is null) return;
        if (value < 0) return;
        _medals.Rank = (MedalRank5)value;
        MarkEdited();
    }

    partial void OnIsTutorialCompleteChanged(bool value)
    {
        if (_suppressSettingsWrite || _medals is null) return;
        _medals.IsTutorialComplete = value;
        MarkEdited();
    }

    partial void OnHabitatTutorialViewedChanged(bool value)
    {
        if (_suppressSettingsWrite || _habitat is not { } habitat) return;
        habitat.IsTutorialViewed = value;
        MarkEdited();
    }

    partial void OnHabitatTutorialCompleteCaptureChanged(bool value)
    {
        if (_suppressSettingsWrite || _habitat is not { } habitat) return;
        habitat.IsTutorialCompleteCapture = value;
        MarkEdited();
    }

    partial void OnLastEncounterTypeIndexChanged(int value)
    {
        if (_suppressSettingsWrite || _habitat is not { } habitat) return;
        if (value < 0) return;
        habitat.LastEncounterType = (HabitatEncounterType5)value;
        MarkEdited();
    }

    [RelayCommand]
    private void GiveAll()
    {
        if (_medals is null) return;
        _medals.GiveAll(EncounterDate.GetDateNDS(), unread: true);
        MarkEdited();
        LoadMedalData();
        RefreshMedalRows();
    }

    [RelayCommand]
    private void ClearAll()
    {
        if (_medals is null) return;
        for (int i = 0; i < MaxMedals; i++)
            _medals[i].Clear();
        MarkEdited();
        LoadMedalData();
        RefreshMedalRows();
    }

    [RelayCommand]
    private void RecalculateRank()
    {
        if (_medals is null) return;
        RankIndex = (int)_medals.CalculateRank();
        // RankIndex setter persists + marks edited.
    }

    [RelayCommand]
    private void HabitatCompleteAll()
    {
        if (_habitat is not { } habitat) return;
        habitat.CompleteAll();
        MarkEdited();
        RefreshHabitatRows();
    }

    [RelayCommand]
    private void HabitatClearAll()
    {
        if (_habitat is not { } habitat) return;
        for (int i = 0; i < HabitatList5.HabitatCount; i++)
            habitat.GetHabitat(i).Clear();
        MarkEdited();
        RefreshHabitatRows();
    }

    [RelayCommand]
    private void Refresh() => LoadData();

    [RelayCommand]
    private async Task ExportAllAsync()
    {
        if (_medals is null) return;
        if (_dialogService is null) return;

        var path = await _dialogService.SaveFileAsync(LocalizedStrings.Instance["Medal_ExportMedalListTitle"], GetDefaultFileName(), [MedalListExtension]);
        if (string.IsNullOrEmpty(path)) return;

        try
        {
            await File.WriteAllBytesAsync(path, _medals.AllMedals.ToArray());
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["Medal_ExportErrorTitle"], ex.Message);
        }
    }

    [RelayCommand]
    private async Task ImportAllAsync()
    {
        if (_medals is null) return;
        if (_dialogService is null) return;

        var path = await _dialogService.OpenFileAsync(LocalizedStrings.Instance["Medal_ImportMedalListTitle"], [MedalListExtension]);
        if (string.IsNullOrEmpty(path)) return;

        try
        {
            var data = await File.ReadAllBytesAsync(path);
            if (data.Length != MedalList5.LengthAllMedals)
            {
                await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["Medal_ImportErrorTitle"],
                    LocalizedStrings.Instance.Format("Medal_SizeMismatch", MedalList5.LengthAllMedals, data.Length));
                return;
            }

            data.AsSpan().CopyTo(_medals.AllMedals);
            MarkEdited();
            LoadMedalData();
            RefreshMedalRows();
            await _dialogService.ShowInformationAsync(LocalizedStrings.Instance["Medal_ImportSuccessfulTitle"], LocalizedStrings.Instance["Medal_ListImported"]);
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["Medal_ImportErrorTitle"], ex.Message);
        }
    }

    private void RefreshMedalRows()
    {
        foreach (var row in MedalRows)
            row.RefreshFromSave();
        RecountObtained();
    }

    private void RefreshHabitatRows()
    {
        foreach (var row in HabitatRows)
            row.RefreshFromSave();
    }

    private string GetDefaultFileName()
    {
        var name = PathUtil.CleanFileName($"{_sav.OT} {_sav.Version}.{MedalListExtension}");
        return string.IsNullOrWhiteSpace(name) ? $"Medals.{MedalListExtension}" : name;
    }

    internal static bool TryParseDate(string? text, out DateOnly date)
    {
        date = default;
        if (string.IsNullOrWhiteSpace(text))
            return false;

        text = text.Trim();
        if (!DateOnly.TryParse(text, System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out date) &&
            !DateOnly.TryParseExact(text, DateFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date))
        {
            return false;
        }

        return EncounterDate.IsValidDateNDS(date);
    }

    internal static string FormatDate(DateOnly date) => date.ToString(DateFormat, System.Globalization.CultureInfo.InvariantCulture);
}

public partial class MedalCategoryViewModel : ViewModelBase
{
    public MedalCategoryViewModel(string name)
    {
        Name = name;
    }

    public string Name { get; }
    public int ObtainedCount => Medals.Count(m => m.IsObtained);
    public string Summary => $"{ObtainedCount}/{Medals.Count}";

    [ObservableProperty]
    private ObservableCollection<MedalItemViewModel> _medals = [];
}

public partial class MedalItemViewModel : ViewModelBase
{
    private readonly MedalEditorViewModel _parent;
    private bool _suppressWrite;

    public MedalItemViewModel(MedalEditorViewModel parent, int index)
    {
        _parent = parent;
        Index = index;
        Type = MedalList5.GetMedalType(index).ToString();
        RefreshFromSave();
    }

    public int Index { get; }
    public string Type { get; }

    [ObservableProperty]
    private int _stateIndex;

    [ObservableProperty]
    private bool _isUnread;

    [ObservableProperty]
    private string _date = string.Empty;

    [ObservableProperty]
    private bool _canHaveDate;

    [ObservableProperty]
    private bool _isObtained;

    [ObservableProperty]
    private string _stateText = string.Empty;

    public string[] StateChoices => _parent.StateChoices;

    internal void RefreshFromSave()
    {
        var medal = _parent.GetMedal(Index);
        _suppressWrite = true;
        StateIndex = (int)medal.State;
        IsUnread = medal.IsUnread;
        CanHaveDate = medal.CanHaveDate;
        Date = medal is { CanHaveDate: true, HasDate: true } ? MedalEditorViewModel.FormatDate(medal.Date) : string.Empty;
        IsObtained = medal.IsObtained;
        StateText = StateToText((MedalState5)StateIndex);
        _suppressWrite = false;
    }

    partial void OnStateIndexChanged(int value)
    {
        if (_suppressWrite || value < 0) return;
        var medal = _parent.GetMedal(Index);
        medal.State = (MedalState5)value;
        // When transitioning to a state that can carry a date but lacks one, stamp now (mirrors upstream).
        if (medal is { CanHaveDate: true, HasDate: false })
            medal.Date = EncounterDate.GetDateNDS();
        _parent.MarkEdited();

        // Re-read derived fields (date / obtained / canHaveDate) without re-triggering writes.
        _suppressWrite = true;
        CanHaveDate = medal.CanHaveDate;
        Date = medal is { CanHaveDate: true, HasDate: true } ? MedalEditorViewModel.FormatDate(medal.Date) : string.Empty;
        IsObtained = medal.IsObtained;
        StateText = StateToText((MedalState5)value);
        _suppressWrite = false;

        _parent.RecountObtained();
    }

    partial void OnIsUnreadChanged(bool value)
    {
        if (_suppressWrite) return;
        var medal = _parent.GetMedal(Index);
        medal.IsUnread = value;
        _parent.MarkEdited();
    }

    partial void OnDateChanged(string value)
    {
        if (_suppressWrite) return;
        var medal = _parent.GetMedal(Index);
        if (!medal.CanHaveDate) return;
        if (!MedalEditorViewModel.TryParseDate(value, out var date)) return;

        medal.Date = date;
        _parent.MarkEdited();

        _suppressWrite = true;
        Date = MedalEditorViewModel.FormatDate(date);
        _suppressWrite = false;
    }

    private static string StateToText(MedalState5 state) => state switch
    {
        MedalState5.Unobtained => LocalizedStrings.Instance["Medal_StateNotObtained"],
        MedalState5.HintReady => LocalizedStrings.Instance["Medal_StateHintReady"],
        MedalState5.HintObtained => LocalizedStrings.Instance["Medal_StateHintObtained"],
        MedalState5.ObtainReady => LocalizedStrings.Instance["Medal_StateReadyToObtain"],
        MedalState5.Obtained => LocalizedStrings.Instance["Medal_StateObtained"],
        _ => state.ToString(),
    };
}

public partial class HabitatRowViewModel : ViewModelBase
{
    private readonly MedalEditorViewModel _parent;
    private bool _suppressWrite;

    public HabitatRowViewModel(MedalEditorViewModel parent, int index)
    {
        _parent = parent;
        Index = index;
        RefreshFromSave();
    }

    public int Index { get; }

    public string[] HabitatCompletionChoices => _parent.HabitatCompletionChoices;

    [ObservableProperty]
    private bool _isComplete;

    [ObservableProperty]
    private int _grassIndex;

    [ObservableProperty]
    private int _surfIndex;

    [ObservableProperty]
    private int _fishIndex;

    internal void RefreshFromSave()
    {
        var habitat = _parent.GetHabitat(Index);
        _suppressWrite = true;
        IsComplete = habitat.IsComplete;
        GrassIndex = (int)habitat.GetStatus(HabitatEncounterType5.Grass);
        SurfIndex = (int)habitat.GetStatus(HabitatEncounterType5.Surf);
        FishIndex = (int)habitat.GetStatus(HabitatEncounterType5.Fish);
        _suppressWrite = false;
    }

    partial void OnIsCompleteChanged(bool value)
    {
        if (_suppressWrite) return;
        var habitat = _parent.GetHabitat(Index);
        habitat.IsComplete = value;
        _parent.MarkEdited();
    }

    partial void OnGrassIndexChanged(int value) => SetStatus(HabitatEncounterType5.Grass, value);
    partial void OnSurfIndexChanged(int value) => SetStatus(HabitatEncounterType5.Surf, value);
    partial void OnFishIndexChanged(int value) => SetStatus(HabitatEncounterType5.Fish, value);

    private void SetStatus(HabitatEncounterType5 type, int value)
    {
        if (_suppressWrite || value < 0) return;
        var habitat = _parent.GetHabitat(Index);
        habitat.SetStatus(type, (HabitatCompletion5)value);
        _parent.MarkEdited();
    }
}

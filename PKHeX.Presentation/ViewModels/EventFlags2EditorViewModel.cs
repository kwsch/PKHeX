using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Event flags and work values editor specifically for Generation 2 saves.
/// Uses EventWorkspace for proper label support.
/// </summary>
public partial class EventFlags2EditorViewModel : ViewModelBase
{
    private readonly SAV2 _sav;
    private readonly EventWorkspace<SAV2, byte> _workspace;

    [ObservableProperty]
    private ObservableCollection<EventFlag2ViewModel> _flags = [];

    [ObservableProperty]
    private ObservableCollection<EventWork2ViewModel> _workValues = [];

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private ObservableCollection<EventFlag2ViewModel> _filteredFlags = [];

    [ObservableProperty]
    private int _customFlagIndex;

    [ObservableProperty]
    private bool _customFlagValue;

    [ObservableProperty]
    private int _customWorkIndex;

    [ObservableProperty]
    private byte _customWorkValue;

    public int MaxFlagIndex => _workspace.Flags.Length - 1;
    public int MaxWorkIndex => _workspace.Values.Length - 1;
    public bool IsSupported => true;
    public string VersionText => $"Event Flags ({_sav.Version})";

    public EventFlags2EditorViewModel(SAV2 sav)
    {
        _sav = sav;
        _workspace = new EventWorkspace<SAV2, byte>(sav, sav.Version);

        LoadFlags();
        LoadWorkValues();
    }

    private void LoadFlags()
    {
        Flags.Clear();
        var labels = _workspace.Labels.Flag;

        // Add labeled flags first
        foreach (var label in labels.OrderByDescending(l => l.Type))
        {
            var index = label.Index;
            if (index >= 0 && index < _workspace.Flags.Length)
            {
                Flags.Add(new EventFlag2ViewModel(index, _workspace.Flags[index], label.Name));
            }
        }

        FilterFlags();
    }

    private void LoadWorkValues()
    {
        WorkValues.Clear();
        var labels = _workspace.Labels.Work;

        foreach (var label in labels.OrderByDescending(l => l.Type))
        {
            var index = label.Index;
            if (index >= 0 && index < _workspace.Values.Length)
            {
                var predefined = label.PredefinedValues.Select(p => new ComboItem(p.Name, p.Value)).ToList();
                WorkValues.Add(new EventWork2ViewModel(index, _workspace.Values[index], label.Name, predefined));
            }
        }
    }

    partial void OnSearchTextChanged(string value)
    {
        FilterFlags();
    }

    private void FilterFlags()
    {
        FilteredFlags.Clear();
        var search = SearchText.ToLowerInvariant();

        foreach (var flag in Flags)
        {
            if (string.IsNullOrEmpty(search) ||
                flag.Index.ToString().Contains(search) ||
                flag.Label.Contains(search, System.StringComparison.OrdinalIgnoreCase))
            {
                FilteredFlags.Add(flag);
            }
        }
    }

    partial void OnCustomFlagIndexChanged(int value)
    {
        if (value >= 0 && value < _workspace.Flags.Length)
        {
            CustomFlagValue = _workspace.Flags[value];
        }
    }

    partial void OnCustomFlagValueChanged(bool value)
    {
        if (CustomFlagIndex >= 0 && CustomFlagIndex < _workspace.Flags.Length)
        {
            _workspace.Flags[CustomFlagIndex] = value;

            // Update in collection if exists
            var existing = Flags.FirstOrDefault(f => f.Index == CustomFlagIndex);
            if (existing is not null)
                existing.IsSet = value;
        }
    }

    partial void OnCustomWorkIndexChanged(int value)
    {
        if (value >= 0 && value < _workspace.Values.Length)
        {
            CustomWorkValue = _workspace.Values[value];
        }
    }

    partial void OnCustomWorkValueChanged(byte value)
    {
        if (CustomWorkIndex >= 0 && CustomWorkIndex < _workspace.Values.Length)
        {
            _workspace.Values[CustomWorkIndex] = value;

            // Update in collection if exists
            var existing = WorkValues.FirstOrDefault(w => w.Index == CustomWorkIndex);
            if (existing is not null)
                existing.Value = value;
        }
    }

    [RelayCommand]
    private void Save()
    {
        // Sync flags from view models back to workspace
        foreach (var flag in Flags)
        {
            if (flag.Index >= 0 && flag.Index < _workspace.Flags.Length)
                _workspace.Flags[flag.Index] = flag.IsSet;
        }

        // Sync work values from view models back to workspace
        foreach (var work in WorkValues)
        {
            if (work.Index >= 0 && work.Index < _workspace.Values.Length)
                _workspace.Values[work.Index] = work.Value;
        }

        _workspace.Save();
    }

    [RelayCommand]
    private void Reset()
    {
        // Reload from save
        var flags = _sav.GetEventFlags();
        var values = _sav.GetAllEventWork();

        for (int i = 0; i < _workspace.Flags.Length && i < flags.Length; i++)
            _workspace.Flags[i] = flags[i];
        for (int i = 0; i < _workspace.Values.Length && i < values.Length; i++)
            _workspace.Values[i] = values[i];

        LoadFlags();
        LoadWorkValues();
    }
}

public partial class EventFlag2ViewModel : ViewModelBase
{
    public int Index { get; }
    public string Label { get; }
    public string DisplayText => string.IsNullOrEmpty(Label) ? $"Flag {Index:D4}" : $"{Index:D4}: {Label}";

    [ObservableProperty]
    private bool _isSet;

    public EventFlag2ViewModel(int index, bool isSet, string label)
    {
        Index = index;
        Label = label;
        _isSet = isSet;
    }
}

public partial class EventWork2ViewModel : ViewModelBase
{
    public int Index { get; }
    public string Label { get; }
    public string DisplayText => string.IsNullOrEmpty(Label) ? $"Work {Index:D4}" : $"{Index:D4}: {Label}";
    public ObservableCollection<ComboItem> PredefinedValues { get; }
    public bool HasPredefinedValues => PredefinedValues.Count > 0;

    [ObservableProperty]
    private byte _value;

    [ObservableProperty]
    private int _selectedPredefinedIndex = -1;

    public EventWork2ViewModel(int index, byte value, string label, System.Collections.Generic.List<ComboItem> predefined)
    {
        Index = index;
        Label = label;
        _value = value;
        PredefinedValues = new ObservableCollection<ComboItem>(predefined);

        // Find matching predefined value
        var match = predefined.FindIndex(p => p.Value == value);
        if (match >= 0)
            _selectedPredefinedIndex = match;
    }

    partial void OnSelectedPredefinedIndexChanged(int value)
    {
        if (value >= 0 && value < PredefinedValues.Count)
        {
            Value = (byte)PredefinedValues[value].Value;
        }
    }

    partial void OnValueChanged(byte value)
    {
        // Update predefined selection if it matches
        var match = PredefinedValues.ToList().FindIndex(p => p.Value == value);
        if (match >= 0 && match != SelectedPredefinedIndex)
            SelectedPredefinedIndex = match;
    }
}

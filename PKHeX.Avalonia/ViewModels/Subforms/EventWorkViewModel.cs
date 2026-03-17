using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single event flag entry (SAV7b).
/// </summary>
public partial class EventWorkFlagModel : ObservableObject
{
    public int Index { get; }
    public string Label { get; }

    [ObservableProperty]
    private bool _value;

    public EventWorkFlagModel(int index, bool value)
    {
        Index = index;
        _value = value;
        Label = $"Flag {index:D4}";
    }
}

/// <summary>
/// Model for a single event work constant (SAV7b).
/// </summary>
public partial class EventWorkConstModel : ObservableObject
{
    public int Index { get; }
    public string Label { get; }

    [ObservableProperty]
    private int _value;

    public EventWorkConstModel(int index, int value)
    {
        Index = index;
        _value = value;
        Label = $"Work {index:D4}";
    }
}

/// <summary>
/// ViewModel for the Event Work editor (SAV7b / Let's Go).
/// Edits event flags and work constants.
/// </summary>
public partial class EventWorkViewModel : SaveEditorViewModelBase
{
    private readonly SaveFile _origin;
    private readonly SaveFile _clone;
    private readonly EventWork7b _eventWork;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private int _selectedTabIndex;

    public ObservableCollection<EventWorkFlagModel> AllFlags { get; } = [];

    [ObservableProperty]
    private ObservableCollection<EventWorkFlagModel> _filteredFlags = [];

    public ObservableCollection<EventWorkConstModel> AllWorks { get; } = [];

    [ObservableProperty]
    private ObservableCollection<EventWorkConstModel> _filteredWorks = [];

    public bool HasWorks => AllWorks.Count > 0;
    public string WindowTitle { get; }

    public EventWorkViewModel(SaveFile sav, EventWork7b eventWork) : base(sav)
    {
        _origin = sav;
        _clone = sav.Clone();
        _eventWork = ((SAV7b)_clone).EventWork;
        WindowTitle = $"Event Work ({sav.Version})";

        LoadFlags();
        LoadWorks();

        FilteredFlags = new ObservableCollection<EventWorkFlagModel>(AllFlags);
        FilteredWorks = new ObservableCollection<EventWorkConstModel>(AllWorks);
    }

    private void LoadFlags()
    {
        for (int i = 0; i < _eventWork.CountFlag; i++)
            AllFlags.Add(new EventWorkFlagModel(i, _eventWork.GetFlag(i)));
    }

    private void LoadWorks()
    {
        for (int i = 0; i < _eventWork.CountWork; i++)
            AllWorks.Add(new EventWorkConstModel(i, _eventWork.GetWork(i)));
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            FilteredFlags = new ObservableCollection<EventWorkFlagModel>(AllFlags);
            FilteredWorks = new ObservableCollection<EventWorkConstModel>(AllWorks);
            return;
        }

        FilteredFlags = new ObservableCollection<EventWorkFlagModel>(
            AllFlags.Where(f => f.Label.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
        FilteredWorks = new ObservableCollection<EventWorkConstModel>(
            AllWorks.Where(w => w.Label.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
    }

    [RelayCommand]
    private void Save()
    {
        foreach (var flag in AllFlags)
            _eventWork.SetFlag(flag.Index, flag.Value);

        foreach (var work in AllWorks)
            _eventWork.SetWork(work.Index, work.Value);

        _origin.CopyChangesFrom(_clone);
        Modified = true;
    }
}

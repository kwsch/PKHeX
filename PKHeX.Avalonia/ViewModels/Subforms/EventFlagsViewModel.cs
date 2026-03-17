using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single event flag (boolean).
/// </summary>
public partial class EventFlagModel : ObservableObject
{
    public int Index { get; }
    public string Label { get; }

    [ObservableProperty]
    private bool _value;

    public EventFlagModel(int index, bool value, string label)
    {
        Index = index;
        _value = value;
        Label = string.IsNullOrEmpty(label) ? $"Flag {index:D4}" : $"{index:D4}: {label}";
    }
}

/// <summary>
/// Model for a single event work (numeric value).
/// </summary>
public partial class EventWorkModel : ObservableObject
{
    public int Index { get; }
    public string Label { get; }

    [ObservableProperty]
    private ushort _value;

    public EventWorkModel(int index, ushort value, string label)
    {
        Index = index;
        _value = value;
        Label = string.IsNullOrEmpty(label) ? $"Work {index:D4}" : $"{index:D4}: {label}";
    }
}

/// <summary>
/// ViewModel for the Event Flags editor subform.
/// Supports Gen 3-7 event flags (booleans) and event works (numeric values).
/// </summary>
public partial class EventFlagsViewModel : SaveEditorViewModelBase
{
    private readonly SaveFile _origin;
    private readonly SaveFile _clone;
    private readonly IEventFlag37 _eventWork;
    private readonly bool[] _flags;
    private readonly ushort[] _works;
    private readonly EventLabelCollection _labels;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private int _selectedTabIndex;

    /// <summary>All flag models.</summary>
    public ObservableCollection<EventFlagModel> AllFlags { get; } = [];

    /// <summary>Filtered flag models.</summary>
    [ObservableProperty]
    private ObservableCollection<EventFlagModel> _filteredFlags = [];

    /// <summary>All work models.</summary>
    public ObservableCollection<EventWorkModel> AllWorks { get; } = [];

    /// <summary>Filtered work models.</summary>
    [ObservableProperty]
    private ObservableCollection<EventWorkModel> _filteredWorks = [];

    /// <summary>Whether this save has event works.</summary>
    public bool HasWorks => AllWorks.Count > 0;

    /// <summary>Display title including the game version.</summary>
    public string WindowTitle { get; }

    public EventFlagsViewModel(SaveFile sav, IEventFlag37 eventWork, GameVersion version) : base(sav)
    {
        _origin = sav;
        _clone = sav.Clone();
        _eventWork = _clone is IEventFlagProvider37 provider ? provider.EventWork : (IEventFlag37)_clone;
        _flags = eventWork.GetEventFlags();
        _works = eventWork.GetAllEventWork();

        WindowTitle = $"Event Flags ({version})";

        var game = GetResourceSuffix(version);
        _labels = new EventLabelCollection(game, _flags.Length, _works.Length);

        LoadFlags();
        LoadWorks();

        FilteredFlags = new ObservableCollection<EventFlagModel>(AllFlags);
        FilteredWorks = new ObservableCollection<EventWorkModel>(AllWorks);
    }

    private void LoadFlags()
    {
        // Add labeled flags first, then remaining
        var labeledIndices = _labels.Flag.Select(f => f.Index).ToHashSet();

        // Add labeled flags
        foreach (var label in _labels.Flag)
        {
            if (label.Index >= 0 && label.Index < _flags.Length)
                AllFlags.Add(new EventFlagModel(label.Index, _flags[label.Index], label.Name));
        }

        // Add remaining unlabeled flags
        for (int i = 0; i < _flags.Length; i++)
        {
            if (!labeledIndices.Contains(i))
                AllFlags.Add(new EventFlagModel(i, _flags[i], string.Empty));
        }
    }

    private void LoadWorks()
    {
        var labeledIndices = _labels.Work.Select(w => w.Index).ToHashSet();

        foreach (var label in _labels.Work)
        {
            if (label.Index >= 0 && label.Index < _works.Length)
                AllWorks.Add(new EventWorkModel(label.Index, _works[label.Index], label.Name));
        }

        for (int i = 0; i < _works.Length; i++)
        {
            if (!labeledIndices.Contains(i))
                AllWorks.Add(new EventWorkModel(i, _works[i], string.Empty));
        }
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            FilteredFlags = new ObservableCollection<EventFlagModel>(AllFlags);
            FilteredWorks = new ObservableCollection<EventWorkModel>(AllWorks);
            return;
        }

        FilteredFlags = new ObservableCollection<EventFlagModel>(
            AllFlags.Where(f => f.Label.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
        FilteredWorks = new ObservableCollection<EventWorkModel>(
            AllWorks.Where(w => w.Label.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
    }

    /// <summary>
    /// Saves event flag and work changes back to the save file.
    /// </summary>
    [RelayCommand]
    private void Save()
    {
        // Write flags back
        foreach (var flag in AllFlags)
            _flags[flag.Index] = flag.Value;

        // Write works back
        foreach (var work in AllWorks)
            _works[work.Index] = work.Value;

        _eventWork.SetEventFlags(_flags);
        _eventWork.SetAllEventWork(_works);

        _origin.CopyChangesFrom(_clone);
        Modified = true;
    }

    private static string GetResourceSuffix(GameVersion version) => version switch
    {
        GameVersion.X or GameVersion.Y or GameVersion.XY => "xy",
        GameVersion.OR or GameVersion.AS or GameVersion.ORAS => "oras",
        GameVersion.SN or GameVersion.MN or GameVersion.SM => "sm",
        GameVersion.US or GameVersion.UM or GameVersion.USUM => "usum",
        GameVersion.D or GameVersion.P or GameVersion.DP => "dp",
        GameVersion.Pt or GameVersion.DPPt => "pt",
        GameVersion.HG or GameVersion.SS or GameVersion.HGSS => "hgss",
        GameVersion.B or GameVersion.W or GameVersion.BW => "bw",
        GameVersion.B2 or GameVersion.W2 or GameVersion.B2W2 => "b2w2",
        GameVersion.R or GameVersion.S or GameVersion.RS => "rs",
        GameVersion.E => "e",
        GameVersion.FR or GameVersion.LG or GameVersion.FRLG => "frlg",
        GameVersion.C => "c",
        GameVersion.GD or GameVersion.SI or GameVersion.GS => "gs",
        _ => throw new ArgumentOutOfRangeException(nameof(version), version, null),
    };
}

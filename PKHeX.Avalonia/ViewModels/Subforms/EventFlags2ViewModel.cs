using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single flag in Gen 8+ event editors (IEventFlag).
/// </summary>
public partial class EventFlag8Model : ObservableObject
{
    public int Index { get; }
    public string Label { get; }

    [ObservableProperty]
    private bool _value;

    public EventFlag8Model(int index, bool value, string label)
    {
        Index = index;
        _value = value;
        Label = string.IsNullOrEmpty(label) ? $"Flag {index:D4}" : $"{index:D4}: {label}";
    }
}

/// <summary>
/// Model for a single work value in Gen 8+ event editors (IEventWork).
/// </summary>
public partial class EventWork8Model : ObservableObject
{
    public int Index { get; }
    public string Label { get; }

    [ObservableProperty]
    private int _value;

    /// <summary>Predefined named values for this work entry.</summary>
    public ObservableCollection<NamedEventConst> PredefinedValues { get; }

    public EventWork8Model(int index, int value, string label, IReadOnlyList<NamedEventConst>? predefined = null)
    {
        Index = index;
        _value = value;
        Label = string.IsNullOrEmpty(label) ? $"Work {index:D4}" : $"{index:D4}: {label}";
        PredefinedValues = predefined is { Count: > 0 }
            ? new ObservableCollection<NamedEventConst>(predefined)
            : [];
    }
}

/// <summary>
/// ViewModel for Gen 8+ event flags/works/system flags editor.
/// Supports SAV8BS (BDSP) which implements IEventFlag + ISystemFlag + IEventWork&lt;int&gt;.
/// Also supports SAV2 (Gen 2) which implements IEventFlagArray + IEventWorkArray&lt;byte&gt;.
/// </summary>
public partial class EventFlags2ViewModel : SaveEditorViewModelBase
{
    private readonly IEventFlag? _eventFlags;
    private readonly ISystemFlag? _systemFlags;
    private readonly IEventWork<int>? _eventWork;

    // Gen 2 support
    private readonly IEventFlagArray? _flagArray;
    private readonly IEventWorkArray<byte>? _workArray;

    private readonly bool _isGen2;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private int _selectedTabIndex;

    public ObservableCollection<EventFlag8Model> AllFlags { get; } = [];

    [ObservableProperty]
    private ObservableCollection<EventFlag8Model> _filteredFlags = [];

    public ObservableCollection<EventFlag8Model> AllSystemFlags { get; } = [];

    [ObservableProperty]
    private ObservableCollection<EventFlag8Model> _filteredSystemFlags = [];

    public ObservableCollection<EventWork8Model> AllWorks { get; } = [];

    [ObservableProperty]
    private ObservableCollection<EventWork8Model> _filteredWorks = [];

    public bool HasSystemFlags => AllSystemFlags.Count > 0;
    public bool HasWorks => AllWorks.Count > 0;
    public string WindowTitle { get; }

    /// <summary>
    /// Constructor for SAV8BS (BDSP) - IEventFlag + ISystemFlag + IEventWork&lt;int&gt;.
    /// </summary>
    public EventFlags2ViewModel(SaveFile sav, IEventFlag eventFlags, ISystemFlag? systemFlags, IEventWork<int>? eventWork, GameVersion version) : base(sav)
    {
        _eventFlags = eventFlags;
        _systemFlags = systemFlags;
        _eventWork = eventWork;
        _isGen2 = false;
        WindowTitle = $"Event Flags ({version})";

        var game = GetGameFilePrefix(version);
        var labels = new EventLabelCollectionSystem(game, eventFlags.CountFlag - 1,
            (systemFlags?.CountSystem ?? 0) - 1, (eventWork?.CountWork ?? 0) - 1);

        LoadFlags(labels);
        LoadSystemFlags(labels);
        LoadWorks(labels);

        FilteredFlags = new ObservableCollection<EventFlag8Model>(AllFlags);
        FilteredSystemFlags = new ObservableCollection<EventFlag8Model>(AllSystemFlags);
        FilteredWorks = new ObservableCollection<EventWork8Model>(AllWorks);
    }

    /// <summary>
    /// Constructor for SAV2 (Gen 2) - IEventFlagArray + IEventWorkArray&lt;byte&gt;.
    /// </summary>
    public EventFlags2ViewModel(SaveFile sav, IEventFlagArray flagArray, IEventWorkArray<byte> workArray, GameVersion version) : base(sav)
    {
        _flagArray = flagArray;
        _workArray = workArray;
        _isGen2 = true;
        WindowTitle = $"Event Flags ({version})";

        var game = GetResourceSuffix(version);
        var labels = new EventLabelCollection(game, flagArray.EventFlagCount, workArray.EventWorkCount);

        LoadGen2Flags(labels, flagArray);
        LoadGen2Works(labels, workArray);

        FilteredFlags = new ObservableCollection<EventFlag8Model>(AllFlags);
        FilteredWorks = new ObservableCollection<EventWork8Model>(AllWorks);
    }

    private void LoadFlags(EventLabelCollectionSystem labels)
    {
        var labeledIndices = labels.Flag.Select(f => f.Index).ToHashSet();
        foreach (var label in labels.Flag)
        {
            if (label.Index >= 0 && label.Index < _eventFlags!.CountFlag)
                AllFlags.Add(new EventFlag8Model(label.Index, _eventFlags.GetFlag(label.Index), label.Name));
        }
        for (int i = 0; i < _eventFlags!.CountFlag; i++)
        {
            if (!labeledIndices.Contains(i))
                AllFlags.Add(new EventFlag8Model(i, _eventFlags.GetFlag(i), string.Empty));
        }
    }

    private void LoadSystemFlags(EventLabelCollectionSystem labels)
    {
        if (_systemFlags is null)
            return;
        var labeledIndices = labels.System.Select(f => f.Index).ToHashSet();
        foreach (var label in labels.System)
        {
            if (label.Index >= 0 && label.Index < _systemFlags.CountSystem)
                AllSystemFlags.Add(new EventFlag8Model(label.Index, _systemFlags.GetSystemFlag(label.Index), label.Name));
        }
        for (int i = 0; i < _systemFlags.CountSystem; i++)
        {
            if (!labeledIndices.Contains(i))
                AllSystemFlags.Add(new EventFlag8Model(i, _systemFlags.GetSystemFlag(i), string.Empty));
        }
    }

    private void LoadWorks(EventLabelCollectionSystem labels)
    {
        if (_eventWork is null)
            return;
        var labeledIndices = labels.Work.Select(w => w.Index).ToHashSet();
        foreach (var label in labels.Work)
        {
            if (label.Index >= 0 && label.Index < _eventWork.CountWork)
                AllWorks.Add(new EventWork8Model(label.Index, _eventWork.GetWork(label.Index), label.Name, label.PredefinedValues));
        }
        for (int i = 0; i < _eventWork.CountWork; i++)
        {
            if (!labeledIndices.Contains(i))
                AllWorks.Add(new EventWork8Model(i, _eventWork.GetWork(i), string.Empty));
        }
    }

    private void LoadGen2Flags(EventLabelCollection labels, IEventFlagArray flagArray)
    {
        var flags = flagArray.GetEventFlags();
        var labeledIndices = labels.Flag.Select(f => f.Index).ToHashSet();
        foreach (var label in labels.Flag)
        {
            if (label.Index >= 0 && label.Index < flags.Length)
                AllFlags.Add(new EventFlag8Model(label.Index, flags[label.Index], label.Name));
        }
        for (int i = 0; i < flags.Length; i++)
        {
            if (!labeledIndices.Contains(i))
                AllFlags.Add(new EventFlag8Model(i, flags[i], string.Empty));
        }
    }

    private void LoadGen2Works(EventLabelCollection labels, IEventWorkArray<byte> workArray)
    {
        var works = workArray.GetAllEventWork();
        var labeledIndices = labels.Work.Select(w => w.Index).ToHashSet();
        foreach (var label in labels.Work)
        {
            if (label.Index >= 0 && label.Index < works.Length)
                AllWorks.Add(new EventWork8Model(label.Index, works[label.Index], label.Name, label.PredefinedValues));
        }
        for (int i = 0; i < works.Length; i++)
        {
            if (!labeledIndices.Contains(i))
                AllWorks.Add(new EventWork8Model(i, works[i], string.Empty));
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
            FilteredFlags = new ObservableCollection<EventFlag8Model>(AllFlags);
            FilteredSystemFlags = new ObservableCollection<EventFlag8Model>(AllSystemFlags);
            FilteredWorks = new ObservableCollection<EventWork8Model>(AllWorks);
            return;
        }

        FilteredFlags = new ObservableCollection<EventFlag8Model>(
            AllFlags.Where(f => f.Label.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
        FilteredSystemFlags = new ObservableCollection<EventFlag8Model>(
            AllSystemFlags.Where(f => f.Label.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
        FilteredWorks = new ObservableCollection<EventWork8Model>(
            AllWorks.Where(w => w.Label.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
    }

    [RelayCommand]
    private void Save()
    {
        if (_isGen2)
        {
            SaveGen2();
        }
        else
        {
            SaveGen8();
        }
        Modified = true;
    }

    private void SaveGen8()
    {
        foreach (var flag in AllFlags)
            _eventFlags!.SetFlag(flag.Index, flag.Value);

        if (_systemFlags is not null)
        {
            foreach (var flag in AllSystemFlags)
                _systemFlags.SetSystemFlag(flag.Index, flag.Value);
        }

        if (_eventWork is not null)
        {
            foreach (var work in AllWorks)
                _eventWork.SetWork(work.Index, work.Value);
        }
    }

    private void SaveGen2()
    {
        if (_flagArray is not null)
        {
            foreach (var flag in AllFlags)
                _flagArray.SetEventFlag(flag.Index, flag.Value);
        }

        if (_workArray is not null)
        {
            foreach (var work in AllWorks)
                _workArray.SetWork(work.Index, (byte)Math.Clamp(work.Value, byte.MinValue, byte.MaxValue));
        }
    }

    private static string GetGameFilePrefix(GameVersion version) => version switch
    {
        GameVersion.BD or GameVersion.SP or GameVersion.BDSP => "bdsp",
        _ => throw new ArgumentOutOfRangeException(nameof(version), version, null),
    };

    private static string GetResourceSuffix(GameVersion version) => version switch
    {
        GameVersion.C => "c",
        GameVersion.GD or GameVersion.SI or GameVersion.GS => "gs",
        _ => throw new ArgumentOutOfRangeException(nameof(version), version, null),
    };
}

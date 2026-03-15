using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a flag entry in the BDSP flag/work editor.
/// </summary>
public partial class FlagEntry8bModel : ObservableObject
{
    public string Name { get; }
    public int Index { get; }

    [ObservableProperty] private bool _value;

    public FlagEntry8bModel(string name, int index, bool value)
    {
        Name = name;
        Index = index;
        _value = value;
    }
}

/// <summary>
/// Model for a work (constant) entry in the BDSP flag/work editor.
/// </summary>
public partial class WorkEntry8bModel : ObservableObject
{
    public string Name { get; }
    public int Index { get; }

    [ObservableProperty] private int _value;

    public WorkEntry8bModel(string name, int index, int value)
    {
        Name = name;
        Index = index;
        _value = value;
    }
}

/// <summary>
/// ViewModel for the BDSP flag/work editor.
/// Edits event flags, system flags, and work values.
/// </summary>
public partial class FlagWork8bViewModel : SaveEditorViewModelBase
{
    private readonly SAV8BS _origin;
    private readonly SAV8BS _sav;

    [ObservableProperty] private string _flagSearchText = string.Empty;
    [ObservableProperty] private string _systemSearchText = string.Empty;
    [ObservableProperty] private string _workSearchText = string.Empty;

    // Custom flag/system/work
    [ObservableProperty] private int _customFlagIndex;
    [ObservableProperty] private bool _customFlagValue;
    [ObservableProperty] private int _customSystemIndex;
    [ObservableProperty] private bool _customSystemValue;
    [ObservableProperty] private int _customWorkIndex;
    [ObservableProperty] private int _customWorkValue;

    public int MaxFlagIndex { get; }
    public int MaxSystemIndex { get; }
    public int MaxWorkIndex { get; }

    public ObservableCollection<FlagEntry8bModel> AllFlags { get; } = [];
    public ObservableCollection<FlagEntry8bModel> AllSystemFlags { get; } = [];
    public ObservableCollection<WorkEntry8bModel> AllWork { get; } = [];

    [ObservableProperty] private ObservableCollection<FlagEntry8bModel> _filteredFlags = [];
    [ObservableProperty] private ObservableCollection<FlagEntry8bModel> _filteredSystemFlags = [];
    [ObservableProperty] private ObservableCollection<WorkEntry8bModel> _filteredWork = [];

    public FlagWork8bViewModel(SAV8BS sav) : base(sav)
    {
        _sav = (SAV8BS)(sav).Clone();
        _origin = sav;

        var obj = _sav.FlagWork;
        MaxFlagIndex = obj.CountFlag - 1;
        MaxSystemIndex = obj.CountSystem - 1;
        MaxWorkIndex = obj.CountWork - 1;

        var game = GetGameFilePrefix(_sav.Version);
        var editor = new EventLabelCollectionSystem(game, obj.CountFlag - 1, obj.CountSystem - 1, obj.CountWork - 1);

        // Load flags
        foreach (var entry in editor.Flag)
            AllFlags.Add(new FlagEntry8bModel(entry.Name, entry.Index, _sav.FlagWork.GetFlag(entry.Index)));

        // Load system flags
        foreach (var entry in editor.System)
            AllSystemFlags.Add(new FlagEntry8bModel(entry.Name, entry.Index, _sav.FlagWork.GetSystemFlag(entry.Index)));

        // Load work
        foreach (var entry in editor.Work)
            AllWork.Add(new WorkEntry8bModel(entry.Name, entry.Index, _sav.FlagWork.GetWork(entry.Index)));

        FilteredFlags = new ObservableCollection<FlagEntry8bModel>(AllFlags);
        FilteredSystemFlags = new ObservableCollection<FlagEntry8bModel>(AllSystemFlags);
        FilteredWork = new ObservableCollection<WorkEntry8bModel>(AllWork);

        CustomFlagValue = obj.GetFlag(0);
        CustomSystemValue = obj.GetSystemFlag(0);
        CustomWorkValue = obj.GetWork(0);
    }

    partial void OnFlagSearchTextChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            FilteredFlags = new ObservableCollection<FlagEntry8bModel>(AllFlags);
        else
            FilteredFlags = new ObservableCollection<FlagEntry8bModel>(
                AllFlags.Where(f => f.Name.Contains(value, StringComparison.OrdinalIgnoreCase)));
    }

    partial void OnSystemSearchTextChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            FilteredSystemFlags = new ObservableCollection<FlagEntry8bModel>(AllSystemFlags);
        else
            FilteredSystemFlags = new ObservableCollection<FlagEntry8bModel>(
                AllSystemFlags.Where(f => f.Name.Contains(value, StringComparison.OrdinalIgnoreCase)));
    }

    partial void OnWorkSearchTextChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            FilteredWork = new ObservableCollection<WorkEntry8bModel>(AllWork);
        else
            FilteredWork = new ObservableCollection<WorkEntry8bModel>(
                AllWork.Where(w => w.Name.Contains(value, StringComparison.OrdinalIgnoreCase)));
    }

    partial void OnCustomFlagIndexChanged(int value)
    {
        if (value >= 0 && value <= MaxFlagIndex)
            CustomFlagValue = _sav.FlagWork.GetFlag(value);
    }

    partial void OnCustomSystemIndexChanged(int value)
    {
        if (value >= 0 && value <= MaxSystemIndex)
            CustomSystemValue = _sav.FlagWork.GetSystemFlag(value);
    }

    partial void OnCustomWorkIndexChanged(int value)
    {
        if (value >= 0 && value <= MaxWorkIndex)
            CustomWorkValue = _sav.FlagWork.GetWork(value);
    }

    [RelayCommand]
    private void ApplyFlag()
    {
        _sav.FlagWork.SetFlag(CustomFlagIndex, CustomFlagValue);
        var match = AllFlags.FirstOrDefault(f => f.Index == CustomFlagIndex);
        if (match != null)
            match.Value = CustomFlagValue;
    }

    [RelayCommand]
    private void ApplySystemFlag()
    {
        _sav.FlagWork.SetSystemFlag(CustomSystemIndex, CustomSystemValue);
        var match = AllSystemFlags.FirstOrDefault(f => f.Index == CustomSystemIndex);
        if (match != null)
            match.Value = CustomSystemValue;
    }

    [RelayCommand]
    private void ApplyWork()
    {
        _sav.FlagWork.SetWork(CustomWorkIndex, CustomWorkValue);
        var match = AllWork.FirstOrDefault(w => w.Index == CustomWorkIndex);
        if (match != null)
            match.Value = CustomWorkValue;
    }

    private static string GetGameFilePrefix(GameVersion version) => version switch
    {
        BD or SP or BDSP => "bdsp",
        _ => "bdsp",
    };

    [RelayCommand]
    private void Save()
    {
        // Apply all flag changes
        foreach (var flag in AllFlags)
            _sav.FlagWork.SetFlag(flag.Index, flag.Value);

        foreach (var sys in AllSystemFlags)
            _sav.FlagWork.SetSystemFlag(sys.Index, sys.Value);

        foreach (var work in AllWork)
            _sav.FlagWork.SetWork(work.Index, work.Value);

        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }
}

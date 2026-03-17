using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single event work grid row (flag or value).
/// </summary>
public partial class EventWork64RowModel : ObservableObject
{
    public int Index { get; }

    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private bool _flagValue;
    [ObservableProperty] private string _workValue = "0";

    public bool IsFlag { get; }

    public EventWork64RowModel(int index, bool isFlag)
    {
        Index = index;
        IsFlag = isFlag;
    }
}

/// <summary>
/// Model for an event work tab.
/// </summary>
public partial class EventWork64TabModel : ObservableObject
{
    public string Name { get; }
    public bool IsFlag { get; }
    public ObservableCollection<EventWork64RowModel> AllRows { get; } = [];

    [ObservableProperty]
    private ObservableCollection<EventWork64RowModel> _filteredRows = [];

    [ObservableProperty] private string _searchText = string.Empty;

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    public void ApplyFilter()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            FilteredRows = new ObservableCollection<EventWork64RowModel>(AllRows);
            return;
        }
        FilteredRows = new ObservableCollection<EventWork64RowModel>(
            AllRows.Where(r => r.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
    }

    public EventWork64TabModel(string name, bool isFlag)
    {
        Name = name;
        IsFlag = isFlag;
    }
}

/// <summary>
/// ViewModel for the ZA event work grid editor (EventWorkGrid64).
/// Provides a tabbed interface for editing flags and values.
/// </summary>
public partial class EventWorkGrid64ViewModel : SaveEditorViewModelBase
{
    private readonly SAV9ZA _origin;
    private readonly SAV9ZA _sav;
    private readonly Dictionary<ulong, string> _lookup = new() { { FnvHash.HashEmpty, "" } };
    private readonly Dictionary<string, ulong> _reverse;

    public ObservableCollection<EventWork64TabModel> Tabs { get; } = [];

    [ObservableProperty] private EventWork64TabModel? _selectedTab;

    // Storage references for save
    private readonly List<(EventWork64TabModel Tab, object Storage)> _storageMap = [];

    public EventWorkGrid64ViewModel(SAV9ZA sav) : base(sav)
    {
        _origin = sav;
        _sav = (SAV9ZA)sav.Clone();

        var path = Path.Combine(AppContext.BaseDirectory, $"{sav.GetType().Name}_flagwork.txt");
        if (File.Exists(path))
            SCBlockMetadata.AddExtraKeyNames64(_lookup, File.ReadLines(path));

        _reverse = new Dictionary<string, ulong>();
        foreach (var kv in _lookup)
            _reverse[kv.Value] = kv.Key;

        AddFlagTab(nameof(_sav.Blocks.Flags), _sav.Blocks.Flags);
        AddFlagTab(nameof(_sav.Blocks.Event), _sav.Blocks.Event);
        AddValueTab(nameof(_sav.Blocks.Work), _sav.Blocks.Work);
        AddValueTab(nameof(_sav.Blocks.Quest), _sav.Blocks.Quest);
        AddValueTab(nameof(_sav.Blocks.WorkMable), _sav.Blocks.WorkMable);
        AddValueTab(nameof(_sav.Blocks.CountMable), _sav.Blocks.CountMable);
        AddValueTab(nameof(_sav.Blocks.CountTitle), _sav.Blocks.CountTitle);
        AddValueTab(nameof(_sav.Blocks.WorkSpawn), _sav.Blocks.WorkSpawn);
        AddFlagTab(nameof(_sav.Blocks.FieldItems), _sav.Blocks.FieldItems);

        if (Tabs.Count > 0)
            SelectedTab = Tabs[0];
    }

    private string GetName(ulong hash)
    {
        if (_lookup.TryGetValue(hash, out var name))
            return name;
        return hash.ToString("X16");
    }

    private ulong GetHash(string nameOrHex)
    {
        if (_reverse.TryGetValue(nameOrHex, out var h))
            return h;
        if (ulong.TryParse(nameOrHex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var parsed))
            return parsed;
        return FnvHash.HashFnv1a_64(nameOrHex);
    }

    private void AddFlagTab(string tabName, EventWorkFlagStorage storage)
    {
        var tab = new EventWork64TabModel(tabName, true);
        var count = storage.Count;
        for (int i = 0; i < count; i++)
        {
            var hash = storage.GetKey(i);
            if (hash == FnvHash.HashEmpty)
                break;
            var row = new EventWork64RowModel(i, true)
            {
                Name = GetName(hash),
                FlagValue = storage.GetValue(i),
            };
            tab.AllRows.Add(row);
        }
        tab.FilteredRows = new ObservableCollection<EventWork64RowModel>(tab.AllRows);
        Tabs.Add(tab);
        _storageMap.Add((tab, storage));
    }

    private void AddValueTab(string tabName, EventWorkValueStorage storage)
    {
        var tab = new EventWork64TabModel(tabName, false);
        var count = storage.Count;
        for (int i = 0; i < count; i++)
        {
            var hash = storage.GetKey(i);
            if (hash == FnvHash.HashEmpty)
                break;
            var row = new EventWork64RowModel(i, false)
            {
                Name = GetName(hash),
                WorkValue = storage.GetValue(i).ToString(),
            };
            tab.AllRows.Add(row);
        }
        tab.FilteredRows = new ObservableCollection<EventWork64RowModel>(tab.AllRows);
        Tabs.Add(tab);
        _storageMap.Add((tab, storage));
    }

    [RelayCommand]
    private void Save()
    {
        foreach (var (tab, storage) in _storageMap)
        {
            if (storage is EventWorkFlagStorage flags)
            {
                var count = Math.Min(flags.Count, tab.AllRows.Count);
                for (int i = 0; i < count; i++)
                {
                    var row = tab.AllRows[i];
                    var hash = GetHash(row.Name.Trim());
                    flags.SetKey(i, hash);
                    flags.SetValue(i, row.FlagValue);
                }
                flags.Compress();
            }
            else if (storage is EventWorkValueStorage values)
            {
                var count = Math.Min(values.Count, tab.AllRows.Count);
                for (int i = 0; i < count; i++)
                {
                    var row = tab.AllRows[i];
                    var hash = GetHash(row.Name.Trim());
                    values.SetKey(i, hash);
                    var v = ulong.TryParse(row.WorkValue, CultureInfo.InvariantCulture, out var val) ? val : 0UL;
                    values.SetValue(i, v);
                }
                values.Compress();
            }
        }
        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }
}

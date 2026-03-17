using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single flag/work entry in the ZA event work editor.
/// </summary>
public partial class FlagWork9aEntryModel : ObservableObject
{
    public int Index { get; }
    public string Name { get; }
    public bool IsFlag { get; }

    [ObservableProperty] private bool _flagValue;
    [ObservableProperty] private string _workValue = "0";

    public FlagWork9aEntryModel(int index, string name, bool isFlag)
    {
        Index = index;
        Name = name;
        IsFlag = isFlag;
    }
}

/// <summary>
/// Model for a tab of event work entries.
/// </summary>
public partial class FlagWork9aTabModel : ObservableObject
{
    public string Name { get; }
    public bool IsFlag { get; }
    public ObservableCollection<FlagWork9aEntryModel> AllEntries { get; } = [];

    [ObservableProperty]
    private ObservableCollection<FlagWork9aEntryModel> _filteredEntries = [];

    [ObservableProperty] private string _searchText = string.Empty;

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    public void ApplyFilter()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            FilteredEntries = new ObservableCollection<FlagWork9aEntryModel>(AllEntries);
            return;
        }
        FilteredEntries = new ObservableCollection<FlagWork9aEntryModel>(
            AllEntries.Where(e => e.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
    }

    public FlagWork9aTabModel(string name, bool isFlag)
    {
        Name = name;
        IsFlag = isFlag;
    }
}

/// <summary>
/// ViewModel for the ZA flag/work editor.
/// Provides a simplified view of flag and value storage blocks.
/// </summary>
public partial class FlagWork9aViewModel : SaveEditorViewModelBase
{
    private readonly SAV9ZA _sav;
    private readonly Dictionary<ulong, string> _lookup = new() { { FnvHash.HashEmpty, "" } };

    public ObservableCollection<FlagWork9aTabModel> Tabs { get; } = [];

    [ObservableProperty] private FlagWork9aTabModel? _selectedTab;

    public FlagWork9aViewModel(SAV9ZA sav) : base(sav)
    {
        _sav = sav;

        // Load any custom name definitions
        var settingsPath = Path.Combine(AppContext.BaseDirectory, $"{sav.GetType().Name}_flagwork.txt");
        if (File.Exists(settingsPath))
            SCBlockMetadata.AddExtraKeyNames64(_lookup, File.ReadLines(settingsPath));

        LoadFlags(nameof(sav.Blocks.Flags), sav.Blocks.Flags);
        LoadFlags(nameof(sav.Blocks.Event), sav.Blocks.Event);
        LoadValues(nameof(sav.Blocks.Work), sav.Blocks.Work);
        LoadValues(nameof(sav.Blocks.Quest), sav.Blocks.Quest);
        LoadValues(nameof(sav.Blocks.WorkMable), sav.Blocks.WorkMable);
        LoadValues(nameof(sav.Blocks.CountMable), sav.Blocks.CountMable);
        LoadValues(nameof(sav.Blocks.CountTitle), sav.Blocks.CountTitle);
        LoadValues(nameof(sav.Blocks.WorkSpawn), sav.Blocks.WorkSpawn);
        LoadFlags(nameof(sav.Blocks.FieldItems), sav.Blocks.FieldItems);

        if (Tabs.Count > 0)
            SelectedTab = Tabs[0];
    }

    private string GetName(ulong hash)
    {
        if (_lookup.TryGetValue(hash, out var name))
            return name;
        return hash.ToString("X16");
    }

    private void LoadFlags(string tabName, EventWorkFlagStorage storage)
    {
        var tab = new FlagWork9aTabModel(tabName, true);
        var count = storage.Count;
        for (int i = 0; i < count; i++)
        {
            var hash = storage.GetKey(i);
            if (hash == FnvHash.HashEmpty)
                break;
            var name = GetName(hash);
            var value = storage.GetValue(i);
            tab.AllEntries.Add(new FlagWork9aEntryModel(i, name, true) { FlagValue = value });
        }
        tab.FilteredEntries = new ObservableCollection<FlagWork9aEntryModel>(tab.AllEntries);
        Tabs.Add(tab);
    }

    private void LoadValues(string tabName, EventWorkValueStorage storage)
    {
        var tab = new FlagWork9aTabModel(tabName, false);
        var count = storage.Count;
        for (int i = 0; i < count; i++)
        {
            var hash = storage.GetKey(i);
            if (hash == FnvHash.HashEmpty)
                break;
            var name = GetName(hash);
            var value = storage.GetValue(i);
            tab.AllEntries.Add(new FlagWork9aEntryModel(i, name, false) { WorkValue = value.ToString() });
        }
        tab.FilteredEntries = new ObservableCollection<FlagWork9aEntryModel>(tab.AllEntries);
        Tabs.Add(tab);
    }

    [RelayCommand]
    private void Save()
    {
        // Write model values back to the underlying storage objects.
        // Storage objects write directly to SAV memory, so no clone/CopyChangesFrom is needed.
        foreach (var tab in Tabs)
        {
            if (tab.IsFlag)
            {
                var storage = GetFlagStorage(tab.Name);
                foreach (var entry in tab.AllEntries)
                    storage.SetValue(entry.Index, entry.FlagValue);
            }
            else
            {
                var storage = GetValueStorage(tab.Name);
                foreach (var entry in tab.AllEntries)
                {
                    if (ulong.TryParse(entry.WorkValue, out var val))
                        storage.SetValue(entry.Index, val);
                }
            }
        }

        Modified = true;
    }

    private EventWorkFlagStorage GetFlagStorage(string tabName) => tabName switch
    {
        nameof(_sav.Blocks.Flags) => _sav.Blocks.Flags,
        nameof(_sav.Blocks.Event) => _sav.Blocks.Event,
        nameof(_sav.Blocks.FieldItems) => _sav.Blocks.FieldItems,
        _ => throw new ArgumentOutOfRangeException(nameof(tabName), tabName, "Unknown flag tab"),
    };

    private EventWorkValueStorage GetValueStorage(string tabName) => tabName switch
    {
        nameof(_sav.Blocks.Work) => _sav.Blocks.Work,
        nameof(_sav.Blocks.Quest) => _sav.Blocks.Quest,
        nameof(_sav.Blocks.WorkMable) => _sav.Blocks.WorkMable,
        nameof(_sav.Blocks.CountMable) => _sav.Blocks.CountMable,
        nameof(_sav.Blocks.CountTitle) => _sav.Blocks.CountTitle,
        nameof(_sav.Blocks.WorkSpawn) => _sav.Blocks.WorkSpawn,
        _ => throw new ArgumentOutOfRangeException(nameof(tabName), tabName, "Unknown value tab"),
    };
}

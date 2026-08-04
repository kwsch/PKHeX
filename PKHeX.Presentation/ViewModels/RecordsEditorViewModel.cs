using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class RecordsEditorViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly ITrainerStatRecord? _storage;
    private readonly Dictionary<int, string>? _recordNames;

    public RecordsEditorViewModel(SaveFile sav)
    {
        _sav = sav;
        _storage = sav as ITrainerStatRecord;
        _recordNames = GetRecordList(sav.Generation);

        LoadRecords();
    }

    public bool HasRecords => _storage is not null && _recordNames is not null;

    [ObservableProperty]
    private ObservableCollection<RecordItemViewModel> _records = [];

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private ObservableCollection<RecordItemViewModel> _filteredRecords = [];

    partial void OnSearchTextChanged(string value)
    {
        FilterRecords();
    }

    private void LoadRecords()
    {
        Records.Clear();
        
        if (_storage is null || _recordNames is null) return;

        foreach (var kvp in _recordNames.OrderBy(x => x.Key))
        {
            if (kvp.Key >= _storage.RecordCount) continue; // skip IDs outside save's valid range
            var value = _storage.GetRecord(kvp.Key);
            var vm = new RecordItemViewModel(kvp.Key, kvp.Value, value, (ITrainerStatRecord)_storage);
            Records.Add(vm);
        }

        FilterRecords();
    }

    private void FilterRecords()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            FilteredRecords = new ObservableCollection<RecordItemViewModel>(Records);
        }
        else
        {
            var search = SearchText.ToLowerInvariant();
            FilteredRecords = new ObservableCollection<RecordItemViewModel>(
                Records.Where(r => r.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                   r.Id.ToString().Contains(search)));
        }
    }

    [RelayCommand]
    private void RefreshRecords()
    {
        LoadRecords();
    }

    private static Dictionary<int, string>? GetRecordList(byte generation) => generation switch
    {
        5 => RecordLists.RecordList_5,
        6 => RecordLists.RecordList_6,
        7 => RecordLists.RecordList_7,
        8 => RecordLists.RecordList_8,
        _ => null
    };
}

public partial class RecordItemViewModel : ViewModelBase
{
    private readonly ITrainerStatRecord _storage;

    public RecordItemViewModel(int id, string name, int value, ITrainerStatRecord storage)
    {
        Id = id;
        Name = name;
        _value = value;
        _storage = storage;
    }

    public int Id { get; }
    public string Name { get; }

    [ObservableProperty]
    private int _value;

    partial void OnValueChanged(int value)
    {
        _storage.SetRecord(Id, value);
    }
}

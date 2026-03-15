using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single trainer stat record.
/// </summary>
public partial class TrainerStatModel : ObservableObject
{
    public int Index { get; }
    public string Label { get; }
    public string OffsetText { get; }
    public int MaxValue { get; }

    [ObservableProperty]
    private int _value;

    public TrainerStatModel(int index, string label, int value, int offset, int max)
    {
        Index = index;
        Label = label;
        _value = value;
        OffsetText = $"0x{offset:X3}";
        MaxValue = max;
    }
}

/// <summary>
/// ViewModel for the Trainer Stat Records subform.
/// Displays and edits trainer statistics/records.
/// </summary>
public partial class TrainerStatViewModel : SaveEditorViewModelBase
{
    private readonly ITrainerStatRecord _record;

    [ObservableProperty]
    private string _searchText = string.Empty;

    public ObservableCollection<TrainerStatModel> AllRecords { get; } = [];

    [ObservableProperty]
    private ObservableCollection<TrainerStatModel> _filteredRecords = [];

    public string WindowTitle { get; }

    public TrainerStatViewModel(SaveFile sav, ITrainerStatRecord record, Dictionary<int, string> recordNames) : base(sav)
    {
        _record = record;
        WindowTitle = $"Trainer Stats ({sav.Version})";

        for (int i = 0; i < record.RecordCount; i++)
        {
            if (!recordNames.TryGetValue(i, out var name))
                name = $"{i:D3}";

            var value = record.GetRecord(i);
            var offset = record.GetRecordOffset(i);
            var max = record.GetRecordMax(i);
            AllRecords.Add(new TrainerStatModel(i, name, value, offset, max));
        }

        FilteredRecords = new ObservableCollection<TrainerStatModel>(AllRecords);
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            FilteredRecords = new ObservableCollection<TrainerStatModel>(AllRecords);
            return;
        }

        FilteredRecords = new ObservableCollection<TrainerStatModel>(
            AllRecords.Where(r => r.Label.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase)));
    }

    [RelayCommand]
    private void Save()
    {
        foreach (var rec in AllRecords)
        {
            var clamped = System.Math.Clamp(rec.Value, 0, rec.MaxValue);
            _record.SetRecord(rec.Index, clamped);
        }

        Modified = true;
    }
}

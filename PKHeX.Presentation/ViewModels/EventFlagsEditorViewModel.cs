using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class EventFlagsEditorViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly IEventFlagArray? _flagArray;

    public EventFlagsEditorViewModel(SaveFile sav)
    {
        _sav = sav;
        _flagArray = GetFlagArray(sav);

        if (_flagArray is not null)
        {
            FlagCount = _flagArray.EventFlagCount;
            IsSupported = true;
            LoadFlags();
        }
        else
        {
            FlagCount = 0;
            IsSupported = false;
        }
    }

    private static IEventFlagArray? GetFlagArray(SaveFile sav)
    {
        // Try to get event flags from various save types
        if (sav is IEventFlagProvider37 provider)
            return provider.EventWork;
        if (sav is IEventFlagArray flagArray)
            return flagArray;
        return null;
    }

    public int FlagCount { get; }
    public bool IsSupported { get; }

    [ObservableProperty]
    private ObservableCollection<EventFlagViewModel> _flags = [];

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private ObservableCollection<EventFlagViewModel> _filteredFlags = [];

    [ObservableProperty]
    private int _selectedFlagIndex;

    [ObservableProperty]
    private bool _selectedFlagValue;

    partial void OnSearchTextChanged(string value)
    {
        FilterFlags();
    }

    partial void OnSelectedFlagIndexChanged(int value)
    {
        if (_flagArray is not null && value >= 0 && value < FlagCount)
        {
            SelectedFlagValue = _flagArray.GetEventFlag(value);
        }
    }

    partial void OnSelectedFlagValueChanged(bool value)
    {
        if (_flagArray is not null && SelectedFlagIndex >= 0 && SelectedFlagIndex < FlagCount)
        {
            _flagArray.SetEventFlag(SelectedFlagIndex, value);
            // Update the flag in the collection if it exists
            var flag = Flags.FirstOrDefault(f => f.Index == SelectedFlagIndex);
            if (flag is not null)
                flag.IsSet = value;
        }
    }

    private void LoadFlags()
    {
        if (_flagArray is null) return;

        Flags.Clear();
        for (int i = 0; i < FlagCount; i++)
        {
            var isSet = _flagArray.GetEventFlag(i);
            Flags.Add(new EventFlagViewModel(i, isSet));
        }
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
                flag.HexIndex.Contains(search, StringComparison.OrdinalIgnoreCase))
            {
                FilteredFlags.Add(flag);
            }
        }
    }

    [RelayCommand]
    private void Save()
    {
        if (_flagArray is null) return;

        foreach (var flag in Flags)
        {
            _flagArray.SetEventFlag(flag.Index, flag.IsSet);
        }
    }

    [RelayCommand]
    private void Reset()
    {
        LoadFlags();
    }

    [RelayCommand]
    private void SetAll()
    {
        foreach (var flag in Flags)
        {
            flag.IsSet = true;
        }
    }

    [RelayCommand]
    private void ClearAll()
    {
        foreach (var flag in Flags)
        {
            flag.IsSet = false;
        }
    }
}

public partial class EventFlagViewModel : ViewModelBase
{
    public EventFlagViewModel(int index, bool isSet)
    {
        Index = index;
        _isSet = isSet;
        HexIndex = $"0x{index:X4}";
    }

    public int Index { get; }
    public string HexIndex { get; }

    [ObservableProperty]
    private bool _isSet;
}

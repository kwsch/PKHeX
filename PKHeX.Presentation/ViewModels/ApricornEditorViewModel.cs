using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class ApricornEditorViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly SAV4HGSS? _hgss;

    private const int Count = 7;
    private const int ItemNameBase = 485; // Red Apricorn

    private static ReadOnlySpan<byte> ItemNameOffset =>
    [
        0, // 485: Red
        2, // 487: Yellow
        1, // 486: Blue
        3, // 488: Green
        4, // 489: Pink
        5, // 490: White
        6, // 491: Black
    ];

    public ApricornEditorViewModel(SaveFile sav)
    {
        _sav = sav;
        _hgss = sav as SAV4HGSS;
        IsSupported = _hgss is not null;

        if (IsSupported)
            LoadApricorns();
    }

    public bool IsSupported { get; }

    [ObservableProperty]
    private ObservableCollection<ApricornSlotViewModel> _apricorns = [];

    private void LoadApricorns()
    {
        Apricorns.Clear();
        if (_hgss is null) return;

        var itemNames = GameInfo.Strings.itemlist;
        for (int i = 0; i < Count; i++)
        {
            var itemId = ItemNameBase + ItemNameOffset[i];
            var name = itemNames[itemId];
            var count = _hgss.GetApricornCount(i);
            Apricorns.Add(new ApricornSlotViewModel(i, name, count, SetApricornCount));
        }
    }

    private void SetApricornCount(int index, int count)
    {
        _hgss?.SetApricornCount(index, Math.Min(byte.MaxValue, count));
    }

    [RelayCommand]
    private void GiveAll()
    {
        if (_hgss is null) return;
        for (int i = 0; i < Count; i++)
            _hgss.SetApricornCount(i, 99);
        LoadApricorns();
    }

    [RelayCommand]
    private void ClearAll()
    {
        if (_hgss is null) return;
        for (int i = 0; i < Count; i++)
            _hgss.SetApricornCount(i, 0);
        LoadApricorns();
    }
}

public partial class ApricornSlotViewModel : ViewModelBase
{
    private readonly Action<int, int> _onChanged;

    public ApricornSlotViewModel(int index, string name, int count, Action<int, int> onChanged)
    {
        Index = index;
        Name = name;
        _count = count;
        _onChanged = onChanged;
    }

    public int Index { get; }
    public string Name { get; }

    [ObservableProperty]
    private int _count;

    partial void OnCountChanged(int value) => _onChanged(Index, value);
}

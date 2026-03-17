using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single seal sticker entry.
/// </summary>
public partial class SealSticker8bModel : ObservableObject
{
    public int Index { get; }
    public string Name { get; }

    [ObservableProperty] private int _count;
    [ObservableProperty] private int _totalCount;
    [ObservableProperty] private bool _isObtained;

    public SealSticker8bModel(int index, string name)
    {
        Index = index;
        Name = name;
    }
}

/// <summary>
/// ViewModel for the BDSP seal sticker editor.
/// </summary>
public partial class SealStickers8bViewModel : SaveEditorViewModelBase
{
    private readonly SAV8BS _origin;
    private readonly SAV8BS _sav;
    private readonly IReadOnlyList<SealSticker8b> _allItems;
    private readonly string[] _itemNames;

    public ObservableCollection<SealSticker8bModel> Stickers { get; } = [];

    public SealStickers8bViewModel(SAV8BS sav) : base(sav)
    {
        _sav = (SAV8BS)(_origin = sav).Clone();
        var names = new string[200];
        for (int i = 0; i < names.Length; i++)
            names[i] = $"Sticker {i}";
        _itemNames = names;
        _allItems = _sav.SealList.ReadItems();
        LoadItems();
    }

    private void LoadItems()
    {
        Stickers.Clear();
        foreach (var item in _allItems)
        {
            var index = item.Index;
            if ((uint)index >= _itemNames.Length || _itemNames[index].Length == 0)
                continue;

            Stickers.Add(new SealSticker8bModel(index, _itemNames[index])
            {
                Count = item.Count,
                TotalCount = item.TotalCount,
                IsObtained = item.IsGet,
            });
        }
    }

    [RelayCommand]
    private void GiveAll()
    {
        foreach (var sticker in Stickers)
        {
            const int max = SealSticker8b.MaxValue;
            var increment = max - sticker.TotalCount;
            sticker.Count += increment;
            sticker.TotalCount = max;
            sticker.IsObtained = true;
        }
    }

    [RelayCommand]
    private void ClearAll()
    {
        foreach (var sticker in Stickers)
        {
            sticker.Count = 0;
            sticker.TotalCount = 0;
            sticker.IsObtained = false;
        }
    }

    [RelayCommand]
    private void Save()
    {
        foreach (var model in Stickers)
        {
            var item = _allItems.FirstOrDefault(x => x.Index == model.Index);
            if (item == null)
                continue;
            item.Count = model.Count;
            item.TotalCount = model.TotalCount;
            item.IsGet = model.IsObtained;
        }
        _sav.SealList.WriteItems(_allItems);
        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }
}

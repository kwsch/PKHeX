using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class Underground8bEditorViewModel : ViewModelBase
{
    private readonly SAV8BS? _sav;
    private readonly IReadOnlyList<UndergroundItem8b>? _allItems;
    private readonly string[] _itemNames;

    public Underground8bEditorViewModel(SaveFile sav)
    {
        _sav = sav as SAV8BS;
        IsSupported = _sav is not null;
        _itemNames = Util.GetStringList("ug_item", GameInfo.CurrentLanguage);

        if (_sav is not null)
        {
            _allItems = _sav.Underground.ReadItems();
            LoadItems();
        }
    }

    public bool IsSupported { get; }

    [ObservableProperty]
    private ObservableCollection<UndergroundItem8bViewModel> _items = [];

    private void LoadItems()
    {
        if (_allItems is null) return;

        Items.Clear();
        foreach (var item in _allItems)
        {
            if (item.Type == UgItemType.None)
                continue;

            var name = item.Index < _itemNames.Length ? _itemNames[item.Index] : $"Item {item.Index}";
            Items.Add(new UndergroundItem8bViewModel(item, name));
        }
    }

    [RelayCommand]
    private void SetAllMax()
    {
        foreach (var item in Items)
            item.Count = item.MaxValue;
    }

    [RelayCommand]
    private void SetAllNone()
    {
        foreach (var item in Items)
            item.Count = 0;
    }

    [RelayCommand]
    private void Save()
    {
        if (_sav is null || _allItems is null) return;
        _sav.Underground.WriteItems(_allItems);
    }
}

public partial class UndergroundItem8bViewModel : ViewModelBase
{
    private readonly UndergroundItem8b _item;

    public UndergroundItem8bViewModel(UndergroundItem8b item, string name)
    {
        _item = item;
        Name = name;
        _count = item.Count;
        _isNew = !item.HideNewFlag;
        _isFavorite = item.IsFavoriteFlag;
    }

    public int Index => _item.Index;
    public string Type => _item.Type.ToString();
    public string Name { get; }
    public int MaxValue => _item.MaxValue;

    [ObservableProperty]
    private int _count;

    partial void OnCountChanged(int value) => _item.Count = value;

    [ObservableProperty]
    private bool _isNew;

    partial void OnIsNewChanged(bool value) => _item.HideNewFlag = !value;

    [ObservableProperty]
    private bool _isFavorite;

    partial void OnIsFavoriteChanged(bool value) => _item.IsFavoriteFlag = value;
}

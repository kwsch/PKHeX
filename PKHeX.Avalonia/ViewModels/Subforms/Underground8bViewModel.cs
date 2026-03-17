using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single underground item entry.
/// </summary>
public partial class UndergroundItem8bModel : ObservableObject
{
    public int Index { get; }
    public string TypeName { get; }
    public string Name { get; }
    public int MaxValue { get; }

    [ObservableProperty] private int _count;
    [ObservableProperty] private bool _isNew;
    [ObservableProperty] private bool _isFavorite;

    public UndergroundItem8bModel(int index, string typeName, string name, int maxValue)
    {
        Index = index;
        TypeName = typeName;
        Name = name;
        MaxValue = maxValue;
    }
}

/// <summary>
/// ViewModel for the BDSP underground item editor.
/// </summary>
public partial class Underground8bViewModel : SaveEditorViewModelBase
{
    private readonly SAV8BS _origin;
    private readonly SAV8BS _sav;
    private readonly IReadOnlyList<UndergroundItem8b> _allItems;
    private readonly string[] _itemNames;

    public ObservableCollection<UndergroundItem8bModel> Items { get; } = [];

    public Underground8bViewModel(SAV8BS sav) : base(sav)
    {
        _sav = (SAV8BS)(_origin = sav).Clone();
        var names = new string[300];
        for (int i = 0; i < names.Length; i++)
            names[i] = $"Item {i}";
        _itemNames = names;
        _allItems = _sav.Underground.ReadItems();
        LoadItems();
    }

    private void LoadItems()
    {
        Items.Clear();
        foreach (var item in _allItems)
        {
            if (item.Type == UgItemType.None)
                continue;

            var name = item.Index < _itemNames.Length ? _itemNames[item.Index] : $"Item {item.Index}";
            Items.Add(new UndergroundItem8bModel(item.Index, item.Type.ToString(), name, item.MaxValue)
            {
                Count = item.Count,
                IsNew = !item.HideNewFlag,
                IsFavorite = item.IsFavoriteFlag,
            });
        }
    }

    [RelayCommand]
    private void GiveAll()
    {
        foreach (var model in Items)
            model.Count = model.MaxValue;
    }

    [RelayCommand]
    private void ClearAll()
    {
        foreach (var model in Items)
        {
            model.Count = 0;
            model.IsNew = true;
            model.IsFavorite = false;
        }
    }

    [RelayCommand]
    private void Save()
    {
        foreach (var model in Items)
        {
            var item = _allItems.FirstOrDefault(x => x.Index == model.Index);
            if (item == null)
                continue;
            item.Count = model.Count;
            item.HideNewFlag = !model.IsNew;
            item.IsFavoriteFlag = model.IsFavorite;
        }
        _sav.Underground.WriteItems(_allItems);
        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }
}

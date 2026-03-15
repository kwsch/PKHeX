using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single poffin entry.
/// </summary>
public partial class Poffin8bModel : ObservableObject
{
    public int Index { get; }

    [ObservableProperty] private string _typeName = string.Empty;
    [ObservableProperty] private int _level;
    [ObservableProperty] private int _taste;
    [ObservableProperty] private bool _isNew;
    [ObservableProperty] private int _spicy;
    [ObservableProperty] private int _dry;
    [ObservableProperty] private int _sweet;
    [ObservableProperty] private int _bitter;
    [ObservableProperty] private int _sour;

    public Poffin8bModel(int index)
    {
        Index = index;
    }
}

/// <summary>
/// ViewModel for the BDSP poffin editor.
/// </summary>
public partial class Poffin8bViewModel : SaveEditorViewModelBase
{
    private readonly SAV8BS _sav;
    private readonly IReadOnlyList<Poffin8b> _allItems;
    private readonly string[] _itemNames;

    public ObservableCollection<Poffin8bModel> Poffins { get; } = [];

    public Poffin8bViewModel(SAV8BS sav) : base(sav)
    {
        _sav = sav;
        var names = new string[256];
        for (int i = 0; i < names.Length; i++)
            names[i] = $"Poffin {i}";
        names[0] = GameInfo.Strings.Item[0];
        _itemNames = names;

        _allItems = _sav.Poffins.GetPoffins();
        LoadItems();
    }

    private void LoadItems()
    {
        Poffins.Clear();
        for (var i = 0; i < _allItems.Count; i++)
        {
            var item = _allItems[i];
            var model = new Poffin8bModel(i)
            {
                TypeName = GetPoffinName(item.MstID),
                Level = item.Level,
                Taste = item.Taste,
                IsNew = item.IsNew,
                Spicy = item.FlavorSpicy,
                Dry = item.FlavorDry,
                Sweet = item.FlavorSweet,
                Bitter = item.FlavorBitter,
                Sour = item.FlavorSour,
            };
            Poffins.Add(model);
        }
    }

    private string GetPoffinName(byte itemMstId)
    {
        var index = (int)(byte)(itemMstId + 1);
        if ((uint)index >= _itemNames.Length)
            index = 0;
        return _itemNames[index];
    }

    [RelayCommand]
    private void GiveAll()
    {
        foreach (var poffin in _allItems)
        {
            poffin.MstID = 0x1C;
            poffin.Level = 60;
            poffin.Taste = 0xFF;
            poffin.FlavorSpicy = poffin.FlavorBitter = poffin.FlavorDry = poffin.FlavorSour = poffin.FlavorSweet = 0xFF;
        }
        LoadItems();
    }

    [RelayCommand]
    private void ClearAll()
    {
        foreach (var poffin in _allItems)
            poffin.ToNull();
        LoadItems();
    }

    [RelayCommand]
    private void Save()
    {
        // Write back from models to data
        for (int i = 0; i < Poffins.Count && i < _allItems.Count; i++)
        {
            var model = Poffins[i];
            var item = _allItems[model.Index];
            item.Level = (byte)model.Level;
            item.Taste = (byte)model.Taste;
            item.IsNew = model.IsNew;
            item.FlavorSpicy = (byte)model.Spicy;
            item.FlavorDry = (byte)model.Dry;
            item.FlavorSweet = (byte)model.Sweet;
            item.FlavorBitter = (byte)model.Bitter;
            item.FlavorSour = (byte)model.Sour;
        }
        _sav.Poffins.SetPoffins(_allItems);
        Modified = true;
    }
}

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class Poffin8bEditorViewModel : ViewModelBase
{
    private readonly SAV8BS? _sav;
    private readonly Poffin8b[]? _allItems;
    private readonly string[] _poffinNames;

    public Poffin8bEditorViewModel(SaveFile sav)
    {
        _sav = sav as SAV8BS;
        IsSupported = _sav is not null;
        _poffinNames = Util.GetStringList("poffin", GameInfo.CurrentLanguage);

        if (_sav is not null)
        {
            _allItems = _sav.Poffins.GetPoffins();
            LoadPoffins();
        }
    }

    public bool IsSupported { get; }

    [ObservableProperty]
    private ObservableCollection<Poffin8bItemViewModel> _poffins = [];

    [ObservableProperty]
    private Poffin8bItemViewModel? _selectedPoffin;

    private void LoadPoffins()
    {
        if (_allItems is null) return;

        Poffins.Clear();
        for (int i = 0; i < _allItems.Length; i++)
        {
            var poffin = _allItems[i];
            Poffins.Add(new Poffin8bItemViewModel(i, poffin, _poffinNames));
        }

        if (Poffins.Count > 0)
            SelectedPoffin = Poffins[0];
    }

    [RelayCommand]
    private void Save()
    {
        if (_sav is null || _allItems is null) return;
        _sav.Poffins.SetPoffins(_allItems);
    }
}

public partial class Poffin8bItemViewModel : ViewModelBase
{
    private readonly Poffin8b _poffin;
    private readonly string[] _poffinNames;

    public Poffin8bItemViewModel(int index, Poffin8b poffin, string[] poffinNames)
    {
        Index = index;
        _poffin = poffin;
        _poffinNames = poffinNames;

        _mstID = poffin.MstID;
        _level = poffin.Level;
        _taste = poffin.Taste;
        _isNew = poffin.IsNew;
        _spicy = poffin.FlavorSpicy;
        _dry = poffin.FlavorDry;
        _sweet = poffin.FlavorSweet;
        _bitter = poffin.FlavorBitter;
        _sour = poffin.FlavorSour;
    }

    public int Index { get; }
    public string DisplayName => $"Poffin {Index + 1}";

    [ObservableProperty]
    private byte _mstID;

    partial void OnMstIDChanged(byte value)
    {
        _poffin.MstID = value;
        OnPropertyChanged(nameof(PoffinName));
    }

    public string PoffinName => MstID < _poffinNames.Length ? _poffinNames[MstID] : $"(ID: {MstID})";

    [ObservableProperty]
    private byte _level;

    partial void OnLevelChanged(byte value) => _poffin.Level = value;

    [ObservableProperty]
    private byte _taste;

    partial void OnTasteChanged(byte value) => _poffin.Taste = value;

    [ObservableProperty]
    private bool _isNew;

    partial void OnIsNewChanged(bool value) => _poffin.IsNew = value;

    [ObservableProperty]
    private byte _spicy;

    partial void OnSpicyChanged(byte value) => _poffin.FlavorSpicy = value;

    [ObservableProperty]
    private byte _dry;

    partial void OnDryChanged(byte value) => _poffin.FlavorDry = value;

    [ObservableProperty]
    private byte _sweet;

    partial void OnSweetChanged(byte value) => _poffin.FlavorSweet = value;

    [ObservableProperty]
    private byte _bitter;

    partial void OnBitterChanged(byte value) => _poffin.FlavorBitter = value;

    [ObservableProperty]
    private byte _sour;

    partial void OnSourChanged(byte value) => _poffin.FlavorSour = value;
}

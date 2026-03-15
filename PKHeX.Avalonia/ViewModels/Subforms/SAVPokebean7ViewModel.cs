using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single Pokebean slot row.
/// </summary>
public partial class PokebeanSlotModel : ObservableObject
{
    public string Name { get; }

    [ObservableProperty]
    private int _count;

    public PokebeanSlotModel(string name, int count)
    {
        Name = name;
        _count = count;
    }
}

/// <summary>
/// ViewModel for the Gen 7 Poke Bean collection editor.
/// </summary>
public partial class SAVPokebean7ViewModel : SaveEditorViewModelBase
{
    private readonly SaveFile _origin;
    private readonly SAV7 _sav;

    public ObservableCollection<PokebeanSlotModel> Slots { get; } = [];

    public SAVPokebean7ViewModel(SAV7 sav) : base(sav)
    {
        _sav = (SAV7)(_origin = sav).Clone();
        LoadValues();
    }

    private void LoadValues()
    {
        Slots.Clear();
        var names = ResortSave7.GetBeanIndexNames();
        var beans = _sav.ResortSave.GetBeans();
        for (int i = 0; i < beans.Length; i++)
            Slots.Add(new PokebeanSlotModel(names[i], beans[i]));
    }

    [RelayCommand]
    private void GiveAll()
    {
        _sav.ResortSave.FillBeans();
        LoadValues();
    }

    [RelayCommand]
    private void ClearAll()
    {
        _sav.ResortSave.ClearBeans();
        LoadValues();
    }

    [RelayCommand]
    private void Save()
    {
        var beans = _sav.ResortSave.GetBeans();
        for (int i = 0; i < Slots.Count && i < beans.Length; i++)
            beans[i] = (byte)Math.Min(byte.MaxValue, Math.Max(0, Slots[i].Count));
        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }
}

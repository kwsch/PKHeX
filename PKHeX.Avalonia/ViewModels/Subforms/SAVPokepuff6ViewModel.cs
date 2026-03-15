using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single Pokepuff slot.
/// </summary>
public partial class PokepuffSlotModel : ObservableObject
{
    public int Index { get; }

    [ObservableProperty]
    private int _selectedPuffIndex;

    public string DisplayName => $"Slot {Index + 1}";

    public PokepuffSlotModel(int index, int puffIndex)
    {
        Index = index;
        _selectedPuffIndex = puffIndex;
    }
}

/// <summary>
/// ViewModel for the Gen 6 Pokepuff collection editor.
/// </summary>
public partial class SAVPokepuff6ViewModel : SaveEditorViewModelBase
{
    private readonly ISaveBlock6Main _sav;

    public ObservableCollection<PokepuffSlotModel> Slots { get; } = [];
    public List<string> PuffNames { get; }

    public SAVPokepuff6ViewModel(ISaveBlock6Main sav) : base((SaveFile)sav)
    {
        _sav = sav;
        PuffNames = [.. GameInfo.Strings.puffs];

        LoadPuffs();
    }

    private void LoadPuffs()
    {
        Slots.Clear();
        var puffs = _sav.Puff.GetPuffs();
        for (int i = 0; i < puffs.Length; i++)
        {
            int puffVal = puffs[i];
            if (puffVal >= PuffNames.Count)
                puffVal = 0;
            Slots.Add(new PokepuffSlotModel(i, puffVal));
        }
    }

    [RelayCommand]
    private void GiveAll()
    {
        _sav.Puff.MaxCheat(false);
        LoadPuffs();
    }

    [RelayCommand]
    private void GiveBest()
    {
        _sav.Puff.MaxCheat(true);
        LoadPuffs();
    }

    [RelayCommand]
    private void ClearAll()
    {
        _sav.Puff.Reset();
        LoadPuffs();
    }

    [RelayCommand]
    private void Sort()
    {
        SavePuffs();
        _sav.Puff.Sort(false);
        LoadPuffs();
    }

    [RelayCommand]
    private void SortReverse()
    {
        SavePuffs();
        _sav.Puff.Sort(true);
        LoadPuffs();
    }

    private void SavePuffs()
    {
        var puffs = new byte[Slots.Count];
        for (int i = 0; i < Slots.Count; i++)
            puffs[i] = (byte)Slots[i].SelectedPuffIndex;
        _sav.Puff.SetPuffs(puffs);
        _sav.Puff.PuffCount = puffs.Length;
    }

    [RelayCommand]
    private void Save()
    {
        SavePuffs();
        Modified = true;
    }
}

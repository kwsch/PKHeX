using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Editor for Gen 6 Poké Puffs.
/// </summary>
public partial class PokepuffEditorViewModel : ViewModelBase
{
    private readonly ISaveBlock6Main _sav;
    
    public PokepuffEditorViewModel(ISaveBlock6Main sav)
    {
        _sav = sav;
        LoadPuffs();
    }

    [ObservableProperty]
    private ObservableCollection<PokepuffSlotViewModel> _puffs = [];

    public string[] PuffList { get; } = GameInfo.Strings.puffs;

    private void LoadPuffs()
    {
        var currentPuffs = _sav.Puff.GetPuffs();
        Puffs.Clear();
        for (int i = 0; i < currentPuffs.Length; i++)
        {
            Puffs.Add(new PokepuffSlotViewModel(i, currentPuffs[i]));
        }
    }

    private void SavePuffs()
    {
        var puffData = new byte[Puffs.Count];
        for (int i = 0; i < Puffs.Count; i++)
        {
            puffData[i] = (byte)Puffs[i].PuffIndex;
        }
        _sav.Puff.SetPuffs(puffData);
        _sav.Puff.PuffCount = puffData.Length;
    }

    [RelayCommand]
    private void Save()
    {
        SavePuffs();
    }

    [RelayCommand]
    private void GiveAllBest()
    {
        _sav.Puff.MaxCheat(true); // Best puffs
        LoadPuffs();
    }

    [RelayCommand]
    private void GiveAllVaried()
    {
        _sav.Puff.MaxCheat(false); // Varied puffs
        LoadPuffs();
    }

    [RelayCommand]
    private void RemoveAll()
    {
        _sav.Puff.Reset();
        LoadPuffs();
    }

    [RelayCommand]
    private void Sort()
    {
        // Must save current state first to sort correctly
        SavePuffs(); 
        _sav.Puff.Sort(false); // Default sort
        LoadPuffs();
    }
}

public partial class PokepuffSlotViewModel : ObservableObject
{
    public PokepuffSlotViewModel(int slotIndex, int puffIndex)
    {
        SlotIndex = slotIndex;
        PuffIndex = puffIndex;
    }

    public int SlotIndex { get; }
    public int DisplayIndex => SlotIndex + 1;

    [ObservableProperty]
    private int _puffIndex;
}

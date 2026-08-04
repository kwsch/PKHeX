using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Misc editor for Legends Arceus (SAV8LA) saves.
/// </summary>
public partial class Misc8aEditorViewModel : ViewModelBase
{
    private readonly SAV8LA _sav;

    public Misc8aEditorViewModel(SAV8LA sav)
    {
        _sav = sav;
        LoadData();
    }

    #region Currency

    [ObservableProperty] private uint _money;
    [ObservableProperty] private uint _meritCurrent;
    [ObservableProperty] private uint _meritEarned;

    private void LoadData()
    {
        Money = _sav.Money;
        MeritCurrent = (uint)_sav.Blocks.GetBlockValue(SaveBlockAccessor8LA.KMeritCurrent);
        MeritEarned = (uint)_sav.Blocks.GetBlockValue(SaveBlockAccessor8LA.KMeritEarnedTotal);
        Rank = (uint)_sav.Blocks.GetBlockValue(SaveBlockAccessor8LA.KExpeditionTeamRank);
        SatchelUpgrades = (uint)_sav.Blocks.GetBlockValue(SaveBlockAccessor8LA.KSatchelUpgrades);
    }

    private void SaveData()
    {
        _sav.Money = Money;
        _sav.Blocks.SetBlockValue(SaveBlockAccessor8LA.KMeritCurrent, MeritCurrent);
        _sav.Blocks.SetBlockValue(SaveBlockAccessor8LA.KMeritEarnedTotal, MeritEarned);
        _sav.Blocks.SetBlockValue(SaveBlockAccessor8LA.KExpeditionTeamRank, Rank);
        _sav.Blocks.SetBlockValue(SaveBlockAccessor8LA.KSatchelUpgrades, SatchelUpgrades);
    }

    [RelayCommand]
    private void MaxMoney()
    {
        Money = (uint)_sav.MaxMoney;
    }

    #endregion

    #region Expedition

    [ObservableProperty] private uint _rank;
    [ObservableProperty] private uint _satchelUpgrades;

    [RelayCommand]
    private void MaxRank()
    {
        Rank = 10; // Max rank in Legends Arceus
    }

    [RelayCommand]
    private void MaxSatchel()
    {
        SatchelUpgrades = 37; // Max satchel upgrades
    }

    #endregion

    #region Commands

    [RelayCommand]
    private void Save()
    {
        SaveData();
    }

    #endregion
}

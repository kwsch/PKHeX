using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Misc editor for BDSP saves.
/// Provides event unlocks for mythical Pokémon and other features.
/// </summary>
public partial class Misc8bEditorViewModel : ViewModelBase
{
    private readonly SAV8BS _sav;
    private readonly SAV8BS _clone;
    private readonly EventUnlocker8b _unlocker;

    [ObservableProperty]
    private bool _canUnlockSpiritomb;

    [ObservableProperty]
    private bool _canUnlockDarkrai;

    [ObservableProperty]
    private bool _canUnlockShaymin;

    [ObservableProperty]
    private bool _canUnlockArceus;

    [ObservableProperty]
    private bool _canUnlockBoxLegend;

    [ObservableProperty]
    private bool _canRespawnRoamer;

    [ObservableProperty]
    private bool _canRebattle;

    [ObservableProperty]
    private bool _canDefeatAll;

    [ObservableProperty]
    private bool _canUnlockZones = true;

    [ObservableProperty]
    private bool _canUnlockFashion = true;

    public Misc8bEditorViewModel(SAV8BS sav)
    {
        _sav = sav;
        _clone = (SAV8BS)sav.Clone();
        _unlocker = new EventUnlocker8b(_clone);

        RefreshStates();
    }

    private void RefreshStates()
    {
        CanUnlockSpiritomb = _unlocker.UnlockReadySpiritomb;
        CanUnlockDarkrai = _unlocker.UnlockReadyDarkrai;
        CanUnlockShaymin = _unlocker.UnlockReadyShaymin;
        CanUnlockArceus = _unlocker.UnlockReadyArceus;
        CanUnlockBoxLegend = _unlocker.UnlockReadyBoxLegend;
        CanRespawnRoamer = _unlocker.ResetReadyRoamerMesprit || _unlocker.ResetReadyRoamerCresselia;
        CanRebattle = _clone.BattleTrainer.AnyDefeated;
        CanDefeatAll = _clone.BattleTrainer.AnyUndefeated;
    }

    [RelayCommand]
    private void UnlockSpiritomb()
    {
        _unlocker.UnlockSpiritomb();
        CanUnlockSpiritomb = _unlocker.UnlockReadySpiritomb;
    }

    [RelayCommand]
    private void UnlockDarkrai()
    {
        _unlocker.UnlockDarkrai();
        CanUnlockDarkrai = _unlocker.UnlockReadyDarkrai;
    }

    [RelayCommand]
    private void UnlockShaymin()
    {
        _unlocker.UnlockShaymin();
        CanUnlockShaymin = _unlocker.UnlockReadyShaymin;
    }

    [RelayCommand]
    private void UnlockArceus()
    {
        _unlocker.UnlockArceus();
        CanUnlockArceus = _unlocker.UnlockReadyArceus;
    }

    [RelayCommand]
    private void UnlockBoxLegend()
    {
        _unlocker.UnlockBoxLegend();
        CanUnlockBoxLegend = _unlocker.UnlockReadyBoxLegend;
    }

    [RelayCommand]
    private void RespawnRoamers()
    {
        _unlocker.RespawnRoamer();
        CanRespawnRoamer = _unlocker.ResetReadyRoamerMesprit || _unlocker.ResetReadyRoamerCresselia;
    }

    [RelayCommand]
    private void UnlockZones()
    {
        _unlocker.UnlockZones();
        CanUnlockZones = false;
    }

    [RelayCommand]
    private void UnlockFashion()
    {
        _unlocker.UnlockFashion();
        CanUnlockFashion = false;
    }

    [RelayCommand]
    private void DefeatAllTrainers()
    {
        _clone.BattleTrainer.DefeatAll();
        CanDefeatAll = false;
        CanRebattle = true;
    }

    [RelayCommand]
    private void RebattleAllTrainers()
    {
        _clone.BattleTrainer.RebattleAll();
        CanRebattle = false;
        CanDefeatAll = true;
    }

    [RelayCommand]
    private void Save()
    {
        _sav.CopyChangesFrom(_clone);
    }
}

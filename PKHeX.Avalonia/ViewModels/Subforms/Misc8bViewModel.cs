using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the BDSP misc editor.
/// Provides buttons for unlocking events (Spiritomb, Darkrai, Shaymin, Arceus, etc.).
/// </summary>
public partial class Misc8bViewModel : SaveEditorViewModelBase
{
    private readonly SAV8BS _origin;
    private readonly SAV8BS _sav;
    private readonly EventUnlocker8b _unlocker;

    [ObservableProperty] private bool _spiritombEnabled;
    [ObservableProperty] private bool _darkraiEnabled;
    [ObservableProperty] private bool _shayminEnabled;
    [ObservableProperty] private bool _arceusEnabled;
    [ObservableProperty] private bool _boxLegendEnabled;
    [ObservableProperty] private bool _roamerEnabled;
    [ObservableProperty] private bool _zonesEnabled = true;
    [ObservableProperty] private bool _fashionEnabled = true;
    [ObservableProperty] private bool _defeatEyecatchEnabled;
    [ObservableProperty] private bool _rebattleEyecatchEnabled;

    public Misc8bViewModel(SAV8BS sav) : base(sav)
    {
        _sav = (SAV8BS)(_origin = sav).Clone();
        _unlocker = new EventUnlocker8b(_sav);
        LoadState();
    }

    private void LoadState()
    {
        SpiritombEnabled = _unlocker.UnlockReadySpiritomb;
        DarkraiEnabled = _unlocker.UnlockReadyDarkrai;
        ShayminEnabled = _unlocker.UnlockReadyShaymin;
        ArceusEnabled = _unlocker.UnlockReadyArceus;
        BoxLegendEnabled = _unlocker.UnlockReadyBoxLegend;
        RoamerEnabled = _unlocker.ResetReadyRoamerMesprit || _unlocker.ResetReadyRoamerCresselia;
        DefeatEyecatchEnabled = _sav.BattleTrainer.AnyUndefeated;
        RebattleEyecatchEnabled = _sav.BattleTrainer.AnyDefeated;
    }

    [RelayCommand]
    private void UnlockSpiritomb()
    {
        _unlocker.UnlockSpiritomb();
        SpiritombEnabled = _unlocker.UnlockReadySpiritomb;
    }

    [RelayCommand]
    private void UnlockDarkrai()
    {
        _unlocker.UnlockDarkrai();
        DarkraiEnabled = _unlocker.UnlockReadyDarkrai;
    }

    [RelayCommand]
    private void UnlockShaymin()
    {
        _unlocker.UnlockShaymin();
        ShayminEnabled = _unlocker.UnlockReadyShaymin;
    }

    [RelayCommand]
    private void UnlockArceus()
    {
        _unlocker.UnlockArceus();
        ArceusEnabled = _unlocker.UnlockReadyArceus;
    }

    [RelayCommand]
    private void UnlockBoxLegend()
    {
        _unlocker.UnlockBoxLegend();
        BoxLegendEnabled = _unlocker.UnlockReadyBoxLegend;
    }

    [RelayCommand]
    private void RespawnRoamer()
    {
        _unlocker.RespawnRoamer();
        RoamerEnabled = _unlocker.ResetReadyRoamerMesprit || _unlocker.ResetReadyRoamerCresselia;
    }

    [RelayCommand]
    private void UnlockZones()
    {
        _unlocker.UnlockZones();
        ZonesEnabled = false;
    }

    [RelayCommand]
    private void UnlockFashion()
    {
        _unlocker.UnlockFashion();
        FashionEnabled = false;
    }

    [RelayCommand]
    private void DefeatEyecatch()
    {
        _sav.BattleTrainer.DefeatAll();
        DefeatEyecatchEnabled = false;
        RebattleEyecatchEnabled = true;
    }

    [RelayCommand]
    private void RebattleEyecatch()
    {
        _sav.BattleTrainer.RebattleAll();
        RebattleEyecatchEnabled = false;
        DefeatEyecatchEnabled = true;
    }

    [RelayCommand]
    private void Save()
    {
        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }
}

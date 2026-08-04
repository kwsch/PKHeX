using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class UnityTower5EditorViewModel : ViewModelBase
{
    private readonly SAV5? _sav5;
    private readonly UnityTower5? _unityTower;

    public UnityTower5EditorViewModel(SaveFile sav)
    {
        _sav5 = sav as SAV5;
        IsSupported = _sav5?.UnityTower is not null;

        if (_sav5 is not null)
            _unityTower = _sav5.UnityTower;

        _globalFlag = _unityTower?.GlobalFlag ?? false;
        _unityTowerFlag = _unityTower?.UnityTowerFlag ?? false;
    }

    public bool IsSupported { get; }

    [ObservableProperty]
    private bool _globalFlag;

    [ObservableProperty]
    private bool _unityTowerFlag;

    partial void OnGlobalFlagChanged(bool value)
    {
        if (_unityTower is not null)
            _unityTower.GlobalFlag = value;
    }

    partial void OnUnityTowerFlagChanged(bool value)
    {
        if (_unityTower is not null)
            _unityTower.UnityTowerFlag = value;
    }

    [RelayCommand]
    private void SetAllLocations()
    {
        if (_unityTower is null) return;
        _unityTower.SetAll();
        GlobalFlag = _unityTower.GlobalFlag;
        UnityTowerFlag = _unityTower.UnityTowerFlag;
    }

    [RelayCommand]
    private void SetAllLegalLocations()
    {
        if (_unityTower is null) return;
        _unityTower.SetAllLegal();
        GlobalFlag = _unityTower.GlobalFlag;
        UnityTowerFlag = _unityTower.UnityTowerFlag;
    }

    [RelayCommand]
    private void ClearAllLocations()
    {
        if (_unityTower is null) return;
        _unityTower.ClearAll();
        GlobalFlag = _unityTower.GlobalFlag;
        UnityTowerFlag = _unityTower.UnityTowerFlag;
    }
}

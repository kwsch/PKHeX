using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class Geonet4EditorViewModel : ViewModelBase
{
    private readonly SAV4? _sav4;
    private readonly Geonet4? _geonet;

    public Geonet4EditorViewModel(SaveFile sav)
    {
        _sav4 = sav as SAV4;
        IsSupported = _sav4 is not null;

        if (_sav4 is not null)
            _geonet = new Geonet4(_sav4);
        
        _globalFlag = _geonet?.GlobalFlag ?? false;
    }

    public bool IsSupported { get; }

    [ObservableProperty]
    private bool _globalFlag;

    partial void OnGlobalFlagChanged(bool value)
    {
        if (_geonet is not null)
            _geonet.GlobalFlag = value;
    }

    [RelayCommand]
    private void SetAllLocations()
    {
        if (_geonet is null) return;
        _geonet.SetAll();
        GlobalFlag = _geonet.GlobalFlag;
    }

    [RelayCommand]
    private void SetAllLegalLocations()
    {
        if (_geonet is null) return;
        _geonet.SetAllLegal();
        GlobalFlag = _geonet.GlobalFlag;
    }

    [RelayCommand]
    private void ClearAllLocations()
    {
        if (_geonet is null) return;
        _geonet.ClearAll();
        GlobalFlag = _geonet.GlobalFlag;
    }

    [RelayCommand]
    private void Save()
    {
        _geonet?.Save();
    }
}

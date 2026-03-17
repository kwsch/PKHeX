using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the Gen 2 Misc editor.
/// Provides GS Ball event activation for Crystal saves.
/// </summary>
public partial class SAVMisc2ViewModel : SaveEditorViewModelBase
{
    private readonly SAV2 _origin;
    private readonly SAV2 _sav;

    [ObservableProperty]
    private bool _showGsBallButton;

    [ObservableProperty]
    private bool _gsBallEnabled;

    public SAVMisc2ViewModel(SAV2 sav) : base(sav)
    {
        _origin = sav;
        _sav = (SAV2)sav.Clone();
        _showGsBallButton = _sav.Version is GameVersion.C;
        _gsBallEnabled = !_sav.IsEnabledGSBallMobileEvent;
    }

    [RelayCommand]
    private void EnableGsBall()
    {
        _sav.EnableGSBallMobileEvent();
        GsBallEnabled = false;
    }

    [RelayCommand]
    private void Save()
    {
        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }
}

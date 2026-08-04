using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Misc editor for Gen 2 saves (Gold/Silver/Crystal).
/// Provides GS Ball event unlock for Crystal Virtual Console.
/// </summary>
public partial class Misc2EditorViewModel : ViewModelBase
{
    private readonly SAV2 _sav;
    private readonly SAV2 _clone;

    public bool IsCrystal => _clone.Version is GameVersion.C;

    [ObservableProperty]
    private bool _canEnableGsBall;

    [ObservableProperty]
    private bool _gsBallEnabled;

    public Misc2EditorViewModel(SAV2 sav)
    {
        _sav = sav;
        _clone = (SAV2)sav.Clone();

        GsBallEnabled = _clone.IsEnabledGSBallMobileEvent;
        CanEnableGsBall = IsCrystal && !GsBallEnabled;
    }

    [RelayCommand]
    private void EnableGsBall()
    {
        if (!IsCrystal || GsBallEnabled)
            return;

        _clone.EnableGSBallMobileEvent();
        GsBallEnabled = true;
        CanEnableGsBall = false;
    }

    [RelayCommand]
    private void Save()
    {
        _sav.CopyChangesFrom(_clone);
    }
}

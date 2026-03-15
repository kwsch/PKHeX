using CommunityToolkit.Mvvm.ComponentModel;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the C-Gear skin image display (Gen 5).
/// Shows the current C-Gear background skin dimensions and status.
/// </summary>
public partial class CGearImage5ViewModel : SaveEditorViewModelBase
{
    [ObservableProperty] private bool _hasSkin;
    [ObservableProperty] private string _skinStatus = "No C-Gear skin loaded";
    [ObservableProperty] private int _width;
    [ObservableProperty] private int _height;

    public CGearImage5ViewModel(SAV5 sav) : base(sav)
    {
        var data = sav.CGearSkinData;
        CGearBackground bg = sav is SAV5BW ? new CGearBackgroundBW(data) : new CGearBackgroundB2W2(data);

        _width = CGearBackground.Width;
        _height = CGearBackground.Height;
        _hasSkin = !bg.IsUninitialized;
        _skinStatus = _hasSkin
            ? $"C-Gear skin loaded ({_width}x{_height})"
            : "No C-Gear skin loaded";
    }
}

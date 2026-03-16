using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the Gen 2 Real-Time Clock editor.
/// Gen 2 Crystal has an RTC flag that can be reset to force re-initialization.
/// </summary>
public partial class SAVRTC2ViewModel : SaveEditorViewModelBase
{
    private readonly SAV2 _sav;

    [ObservableProperty]
    private string _statusText = "RTC is running normally. Use Reset to force clock re-initialization on next boot.";

    public SAVRTC2ViewModel(SAV2 sav) : base(sav)
    {
        _sav = sav;
    }

    [RelayCommand]
    private void ResetRtc()
    {
        _sav.ResetRTC();
        StatusText = "RTC reset flag has been SET. The clock will be re-initialized on next boot.";
    }

    [RelayCommand]
    private void Save()
    {
        Modified = true;
    }
}

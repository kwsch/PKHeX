using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the Gen 3 Real-Time Clock editor.
/// Edits initial and elapsed clock values for RSE saves.
/// </summary>
public partial class SAVRTC3ViewModel : SaveEditorViewModelBase
{
    private readonly SAV3 _sav;
    private readonly ISaveBlock3SmallHoenn _small;
    private readonly RTC3 _clockInitial;
    private readonly RTC3 _clockElapsed;

    // Initial clock
    [ObservableProperty]
    private int _initialDay;

    [ObservableProperty]
    private int _initialHour;

    [ObservableProperty]
    private int _initialMinute;

    [ObservableProperty]
    private int _initialSecond;

    // Elapsed clock
    [ObservableProperty]
    private int _elapsedDay;

    [ObservableProperty]
    private int _elapsedHour;

    [ObservableProperty]
    private int _elapsedMinute;

    [ObservableProperty]
    private int _elapsedSecond;

    public SAVRTC3ViewModel(SAV3 sav) : base(sav)
    {
        _sav = sav;
        _small = (ISaveBlock3SmallHoenn)sav.SmallBlock;
        _clockInitial = _small.ClockInitial;
        _clockElapsed = _small.ClockElapsed;

        LoadData();
    }

    private void LoadData()
    {
        InitialDay = _clockInitial.Day;
        InitialHour = Math.Min(23, _clockInitial.Hour);
        InitialMinute = Math.Min(59, _clockInitial.Minute);
        InitialSecond = Math.Min(59, _clockInitial.Second);

        ElapsedDay = _clockElapsed.Day;
        ElapsedHour = Math.Min(23, _clockElapsed.Hour);
        ElapsedMinute = Math.Min(59, _clockElapsed.Minute);
        ElapsedSecond = Math.Min(59, _clockElapsed.Second);
    }

    [RelayCommand]
    private void Reset()
    {
        InitialDay = InitialHour = InitialMinute = InitialSecond = 0;
        ElapsedDay = ElapsedHour = ElapsedMinute = ElapsedSecond = 0;
    }

    [RelayCommand]
    private void BerryFix()
    {
        ElapsedDay = Math.Max((2 * 366) + 2, ElapsedDay);
    }

    [RelayCommand]
    private void Save()
    {
        _clockInitial.Day = InitialDay;
        _clockInitial.Hour = InitialHour;
        _clockInitial.Minute = InitialMinute;
        _clockInitial.Second = InitialSecond;

        _clockElapsed.Day = ElapsedDay;
        _clockElapsed.Hour = ElapsedHour;
        _clockElapsed.Minute = ElapsedMinute;
        _clockElapsed.Second = ElapsedSecond;

        _small.ClockInitial = _clockInitial;
        _small.ClockElapsed = _clockElapsed;

        Modified = true;
    }
}

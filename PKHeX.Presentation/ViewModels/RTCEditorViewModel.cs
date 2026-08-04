using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class RTCEditorViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly ISaveBlock3SmallHoenn? _small;
    private RTC3? _clockInitial;
    private RTC3? _clockElapsed;

    public RTCEditorViewModel(SaveFile sav)
    {
        _sav = sav;
        _small = sav switch
        {
            SAV3RS rs => rs.SmallBlock,
            SAV3E e => e.SmallBlock,
            _ => null,
        };
        IsSupported = _small is not null;
        if (IsSupported) LoadData();
    }

    public bool IsSupported { get; }

    [ObservableProperty] private int _initialDay;
    [ObservableProperty] private int _initialHour;
    [ObservableProperty] private int _initialMinute;
    [ObservableProperty] private int _initialSecond;

    [ObservableProperty] private int _elapsedDay;
    [ObservableProperty] private int _elapsedHour;
    [ObservableProperty] private int _elapsedMinute;
    [ObservableProperty] private int _elapsedSecond;

    private void LoadData()
    {
        if (_small is null) return;
        _clockInitial = _small.ClockInitial;
        _clockElapsed = _small.ClockElapsed;

        InitialDay    = _clockInitial.Day;
        InitialHour   = Math.Min(23, _clockInitial.Hour);
        InitialMinute = Math.Min(59, _clockInitial.Minute);
        InitialSecond = Math.Min(59, _clockInitial.Second);

        ElapsedDay    = _clockElapsed.Day;
        ElapsedHour   = Math.Min(23, _clockElapsed.Hour);
        ElapsedMinute = Math.Min(59, _clockElapsed.Minute);
        ElapsedSecond = Math.Min(59, _clockElapsed.Second);
    }

    [RelayCommand]
    private void Save()
    {
        if (_small is null || _clockInitial is null || _clockElapsed is null) return;

        _clockInitial.Day    = (ushort)InitialDay;
        _clockInitial.Hour   = (byte)InitialHour;
        _clockInitial.Minute = (byte)InitialMinute;
        _clockInitial.Second = (byte)InitialSecond;

        _clockElapsed.Day    = (ushort)ElapsedDay;
        _clockElapsed.Hour   = (byte)ElapsedHour;
        _clockElapsed.Minute = (byte)ElapsedMinute;
        _clockElapsed.Second = (byte)ElapsedSecond;

        _small.ClockInitial = _clockInitial;
        _small.ClockElapsed = _clockElapsed;
    }

    [RelayCommand]
    private void Reset() => InitialDay = InitialHour = InitialMinute = InitialSecond =
                            ElapsedDay = ElapsedHour = ElapsedMinute = ElapsedSecond = 0;

    [RelayCommand]
    private void BerryFix() => ElapsedDay = Math.Max((2 * 366) + 2, ElapsedDay);

    [RelayCommand]
    private void Refresh() => LoadData();
}

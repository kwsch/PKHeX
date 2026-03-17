using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the Simple Trainer editor (Gen 1-5).
/// Edits basic trainer info: Name, TID, SID, Money, Gender, Badges, Play Time.
/// </summary>
public partial class SimpleTrainerViewModel : SaveEditorViewModelBase
{
    private readonly SaveFile _clone;

    [ObservableProperty]
    private string _otName = string.Empty;

    [ObservableProperty]
    private ushort _tid;

    [ObservableProperty]
    private ushort _sid;

    [ObservableProperty]
    private uint _money;

    [ObservableProperty]
    private int _gender;

    [ObservableProperty]
    private int _playedHours;

    [ObservableProperty]
    private int _playedMinutes;

    [ObservableProperty]
    private int _playedSeconds;

    [ObservableProperty]
    private bool _badge1;

    [ObservableProperty]
    private bool _badge2;

    [ObservableProperty]
    private bool _badge3;

    [ObservableProperty]
    private bool _badge4;

    [ObservableProperty]
    private bool _badge5;

    [ObservableProperty]
    private bool _badge6;

    [ObservableProperty]
    private bool _badge7;

    [ObservableProperty]
    private bool _badge8;

    public int MaxNameLength { get; }
    public uint MaxMoney { get; }
    public bool HasSid { get; }
    public bool HasGender { get; }
    public string[] GenderSymbols { get; }

    public SimpleTrainerViewModel(SaveFile sav) : base(sav)
    {
        _clone = sav;

        MaxNameLength = sav.MaxStringLengthTrainer;
        MaxMoney = (uint)sav.MaxMoney;
        HasSid = sav.Generation > 2;
        HasGender = sav.Generation > 1;
        GenderSymbols = ["Male", "Female"];

        // Load values
        _otName = sav.OT;
        _tid = sav.TID16;
        _sid = sav.SID16;
        _money = sav.Money;
        _gender = sav.Gender;
        _playedHours = sav.PlayedHours;
        _playedMinutes = sav.PlayedMinutes;
        _playedSeconds = sav.PlayedSeconds;

        // Load badges
        int badgeval = GetBadgeValue(sav);
        _badge1 = (badgeval & (1 << 0)) != 0;
        _badge2 = (badgeval & (1 << 1)) != 0;
        _badge3 = (badgeval & (1 << 2)) != 0;
        _badge4 = (badgeval & (1 << 3)) != 0;
        _badge5 = (badgeval & (1 << 4)) != 0;
        _badge6 = (badgeval & (1 << 5)) != 0;
        _badge7 = (badgeval & (1 << 6)) != 0;
        _badge8 = (badgeval & (1 << 7)) != 0;
    }

    private static int GetBadgeValue(SaveFile sav) => sav switch
    {
        SAV1 s => s.Badges,
        SAV2 s => s.Badges,
        SAV3 s => s.Badges,
        SAV4 s => s.Badges,
        SAV5 s => s.Misc.Badges,
        _ => 0,
    };

    [RelayCommand]
    private void MaxCash()
    {
        Money = MaxMoney;
    }

    [RelayCommand]
    private void Save()
    {
        var sav = _clone;
        sav.OT = OtName;
        sav.TID16 = Tid;
        sav.SID16 = Sid;
        sav.Money = Math.Min(Money, MaxMoney);
        if (HasGender)
            sav.Gender = (byte)Math.Clamp(Gender, 0, 255);

        sav.PlayedHours = (ushort)Math.Clamp(PlayedHours, 0, ushort.MaxValue);
        sav.PlayedMinutes = (ushort)Math.Clamp(PlayedMinutes, 0, 59);
        sav.PlayedSeconds = (ushort)Math.Clamp(PlayedSeconds, 0, 59);

        // Save badges
        int badgeval = 0;
        if (Badge1) badgeval |= 1 << 0;
        if (Badge2) badgeval |= 1 << 1;
        if (Badge3) badgeval |= 1 << 2;
        if (Badge4) badgeval |= 1 << 3;
        if (Badge5) badgeval |= 1 << 4;
        if (Badge6) badgeval |= 1 << 5;
        if (Badge7) badgeval |= 1 << 6;
        if (Badge8) badgeval |= 1 << 7;

        SetBadgeValue(sav, badgeval);
        Modified = true;
    }

    private static void SetBadgeValue(SaveFile sav, int val)
    {
        switch (sav)
        {
            case SAV1 s: s.Badges = val & 0xFF; break;
            case SAV2 s: s.Badges = val & 0xFF; break;
            case SAV3 s: s.Badges = val & 0xFF; break;
            case SAV4 s: s.Badges = val & 0xFF; break;
            case SAV5 s: s.Misc.Badges = val & 0xFF; break;
        }
    }
}

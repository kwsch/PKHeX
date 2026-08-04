using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Simple trainer editor for older generation saves (Gen 1-5).
/// </summary>
public partial class SimpleTrainerEditorViewModel : ViewModelBase
{
    private readonly SaveFile _sav;

    public SimpleTrainerEditorViewModel(SaveFile sav)
    {
        _sav = sav;
        IsGen1 = sav is SAV1;
        IsGen2 = sav is SAV2;
        IsGen3 = sav is SAV3;
        IsGen4 = sav is SAV4;
        IsGen5 = sav is SAV5;
        HasSID = sav.Generation > 2;
        HasCoins = sav.Generation < 3;
        LoadData();
    }

    public bool IsGen1 { get; }
    public bool IsGen2 { get; }
    public bool IsGen3 { get; }
    public bool IsGen4 { get; }
    public bool IsGen5 { get; }
    public bool HasSID { get; }
    public bool HasCoins { get; }

    #region Trainer Info

    [ObservableProperty] private string _trainerName = string.Empty;
    [ObservableProperty] private int _gender;
    [ObservableProperty] private ushort _tid;
    [ObservableProperty] private ushort _sid;
    [ObservableProperty] private uint _money;
    [ObservableProperty] private ushort _coins;

    [ObservableProperty] private ushort _playedHours;
    [ObservableProperty] private byte _playedMinutes;
    [ObservableProperty] private byte _playedSeconds;

    private void LoadData()
    {
        TrainerName = _sav.OT;
        Gender = _sav.Gender;
        Tid = _sav.TID16;
        Sid = _sav.SID16;
        Money = _sav.Money;
        PlayedHours = (ushort)_sav.PlayedHours;
        PlayedMinutes = (byte)_sav.PlayedMinutes;
        PlayedSeconds = (byte)_sav.PlayedSeconds;

        if (_sav is SAV1 sav1)
            Coins = (ushort)sav1.Coin;
        else if (_sav is SAV2 sav2)
            Coins = (ushort)sav2.Coin;
    }

    private void SaveData()
    {
        _sav.OT = TrainerName;
        _sav.Gender = (byte)Gender;
        _sav.TID16 = Tid;
        _sav.SID16 = Sid;
        _sav.Money = Money;
        _sav.PlayedHours = PlayedHours;
        _sav.PlayedMinutes = PlayedMinutes;
        _sav.PlayedSeconds = PlayedSeconds;

        if (_sav is SAV1 sav1)
            sav1.Coin = Math.Min(Coins, (ushort)_sav.MaxCoins);
        else if (_sav is SAV2 sav2)
            sav2.Coin = Math.Min(Coins, (ushort)_sav.MaxCoins);
    }

    [RelayCommand]
    private void MaxMoney()
    {
        Money = (uint)_sav.MaxMoney;
    }

    [RelayCommand]
    private void MaxCoins()
    {
        Coins = (ushort)_sav.MaxCoins;
    }

    #endregion

    #region Commands

    [RelayCommand]
    private void Save()
    {
        SaveData();
    }

    #endregion
}

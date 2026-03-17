using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the BDSP trainer editor.
/// Edits OT name, rival, money, BP, play time, badges, etc.
/// </summary>
public partial class Trainer8bViewModel : SaveEditorViewModelBase
{
    private readonly SaveFile _origin;
    private readonly SAV8BS _sav;

    public string[] GenderChoices { get; } = ["Male", "Female"];
    public string[] GameChoices { get; } = ["Brilliant Diamond", "Shining Pearl"];

    [ObservableProperty] private string _otName = string.Empty;
    [ObservableProperty] private string _rival = string.Empty;
    [ObservableProperty] private string _money = "0";
    [ObservableProperty] private int _gender;
    [ObservableProperty] private int _gameIndex;
    [ObservableProperty] private int _bp;

    [ObservableProperty] private int _playedHours;
    [ObservableProperty] private int _playedMinutes;
    [ObservableProperty] private int _playedSeconds;

    [ObservableProperty] private bool _badge1;
    [ObservableProperty] private bool _badge2;
    [ObservableProperty] private bool _badge3;
    [ObservableProperty] private bool _badge4;
    [ObservableProperty] private bool _badge5;
    [ObservableProperty] private bool _badge6;
    [ObservableProperty] private bool _badge7;
    [ObservableProperty] private bool _badge8;

    public Trainer8bViewModel(SAV8BS sav) : base(sav)
    {
        _sav = (SAV8BS)(_origin = sav).Clone();
        LoadData();
    }

    private void LoadData()
    {
        GameIndex = Math.Clamp((byte)_sav.Version - (byte)GameVersion.BD, 0, 1);
        Gender = _sav.Gender;
        OtName = _sav.OT;
        Rival = _sav.Rival;
        Money = _sav.Money.ToString();
        Bp = (int)_sav.BattleTower.BP;

        PlayedHours = _sav.PlayedHours;
        PlayedMinutes = _sav.PlayedMinutes;
        PlayedSeconds = _sav.PlayedSeconds;

        Badge1 = _sav.FlagWork.GetSystemFlag(124);
        Badge2 = _sav.FlagWork.GetSystemFlag(125);
        Badge3 = _sav.FlagWork.GetSystemFlag(126);
        Badge4 = _sav.FlagWork.GetSystemFlag(127);
        Badge5 = _sav.FlagWork.GetSystemFlag(128);
        Badge6 = _sav.FlagWork.GetSystemFlag(129);
        Badge7 = _sav.FlagWork.GetSystemFlag(130);
        Badge8 = _sav.FlagWork.GetSystemFlag(131);
    }

    [RelayCommand]
    private void MaxCash() => Money = _sav.MaxMoney.ToString();

    [RelayCommand]
    private void Save()
    {
        _sav.Version = (GameVersion)(GameIndex + (int)GameVersion.BD);
        _sav.Gender = (byte)Gender;
        _sav.OT = OtName;
        _sav.Rival = Rival;
        _sav.Money = uint.TryParse(Money, out var money) ? money : 0u;
        _sav.BattleTower.BP = (uint)Math.Max(0, Bp);

        _sav.PlayedHours = (ushort)Math.Clamp(PlayedHours, 0, ushort.MaxValue);
        _sav.PlayedMinutes = (ushort)Math.Clamp(PlayedMinutes, 0, 59);
        _sav.PlayedSeconds = (ushort)Math.Clamp(PlayedSeconds, 0, 59);

        _sav.FlagWork.SetSystemFlag(124, Badge1);
        _sav.FlagWork.SetSystemFlag(125, Badge2);
        _sav.FlagWork.SetSystemFlag(126, Badge3);
        _sav.FlagWork.SetSystemFlag(127, Badge4);
        _sav.FlagWork.SetSystemFlag(128, Badge5);
        _sav.FlagWork.SetSystemFlag(129, Badge6);
        _sav.FlagWork.SetSystemFlag(130, Badge7);
        _sav.FlagWork.SetSystemFlag(131, Badge8);

        // Cannot have an all-zero ID.
        if (_sav is { TID16: 0, SID16: 0 })
            _sav.SID16 = 1;

        // Trickle down changes to the extra record block.
        if (_sav.HasFirstSaveFileExpansion && (_sav.OT != _origin.OT || _sav.TID16 != _origin.TID16 || _sav.SID16 != _origin.SID16))
            _sav.RecordAdd.ReplaceOT(_origin, _sav);

        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }
}

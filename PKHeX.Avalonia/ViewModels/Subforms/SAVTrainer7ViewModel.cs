using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the Gen 7 SM/USUM trainer editor.
/// Edits OT name, TID/SID, money, play time, BP, FC, Poke Finder stats,
/// Battle Tree streaks, stamps, unlocks, and USUM-specific data.
/// </summary>
public partial class SAVTrainer7ViewModel : SaveEditorViewModelBase
{
    private readonly SaveFile _origin;
    private readonly SAV7 _sav;

    public string[] GenderChoices { get; } = ["Male", "Female"];

    // Basic Info
    [ObservableProperty] private string _otName = string.Empty;
    [ObservableProperty] private int _gender;
    [ObservableProperty] private string _money = "0";
    [ObservableProperty] private int _language;
    public List<ComboItem> LanguageList { get; }

    // Play Time
    [ObservableProperty] private int _playedHours;
    [ObservableProperty] private int _playedMinutes;
    [ObservableProperty] private int _playedSeconds;

    // Currency
    [ObservableProperty] private int _bp;
    [ObservableProperty] private int _fc;

    // Poke Finder
    [ObservableProperty] private int _snapCount;
    [ObservableProperty] private int _thumbsTotal;
    [ObservableProperty] private int _thumbsRecord;
    [ObservableProperty] private int _cameraVersion;
    [ObservableProperty] private bool _gyroFlag;
    public string[] CameraVersions { get; } = ["1", "2", "3", "4", "5"];

    // Battle Tree (regular)
    [ObservableProperty] private int _rcStreak0;
    [ObservableProperty] private int _rcStreak1;
    [ObservableProperty] private int _rcStreak2;
    [ObservableProperty] private int _rmStreak0;
    [ObservableProperty] private int _rmStreak1;
    [ObservableProperty] private int _rmStreak2;

    // Battle Tree (super)
    [ObservableProperty] private int _scStreak0;
    [ObservableProperty] private int _scStreak1;
    [ObservableProperty] private int _scStreak2;
    [ObservableProperty] private int _smStreak0;
    [ObservableProperty] private int _smStreak1;
    [ObservableProperty] private int _smStreak2;

    // Unlocks
    [ObservableProperty] private bool _megaUnlocked;
    [ObservableProperty] private bool _zMoveUnlocked;
    [ObservableProperty] private bool _unlockSuperSingles;
    [ObservableProperty] private bool _unlockSuperDoubles;
    [ObservableProperty] private bool _unlockSuperMulti;

    // Plaza
    [ObservableProperty] private string _plazaName = string.Empty;

    // USUM-specific
    [ObservableProperty] private bool _isUsum;
    [ObservableProperty] private int _surf0;
    [ObservableProperty] private int _surf1;
    [ObservableProperty] private int _surf2;
    [ObservableProperty] private int _surf3;
    [ObservableProperty] private string _rotomOt = string.Empty;
    [ObservableProperty] private int _rotomAffection;
    [ObservableProperty] private bool _rotoLoto1;
    [ObservableProperty] private bool _rotoLoto2;

    public SAVTrainer7ViewModel(SAV7 sav) : base(sav)
    {
        _sav = (SAV7)(_origin = sav).Clone();
        IsUsum = _sav is SAV7USUM;
        LanguageList = GameInfo.LanguageDataSource(_sav.Generation, _sav.Context).ToList();

        LoadData();
    }

    private void LoadData()
    {
        OtName = _sav.OT;
        Gender = _sav.Gender;
        Money = _sav.Money.ToString();
        Language = _sav.Language;

        PlayedHours = _sav.PlayedHours;
        PlayedMinutes = _sav.PlayedMinutes;
        PlayedSeconds = _sav.PlayedSeconds;

        Bp = (int)Math.Min(9999, _sav.Misc.BP);
        Fc = Math.Min(9999999, _sav.Festa.FestaCoins);

        // Poke Finder
        SnapCount = (int)Math.Min(9999999, _sav.PokeFinder.SnapCount);
        ThumbsTotal = (int)Math.Min(9999999, _sav.PokeFinder.ThumbsTotalValue);
        ThumbsRecord = (int)Math.Min(9999999, _sav.PokeFinder.ThumbsHighValue);
        CameraVersion = Math.Min(4, (int)_sav.PokeFinder.CameraVersion);
        GyroFlag = _sav.PokeFinder.GyroFlag;

        // Battle Tree
        var bt = _sav.BattleTree;
        RcStreak0 = bt.GetTreeStreak(0, super: false, max: false);
        RcStreak1 = bt.GetTreeStreak(1, super: false, max: false);
        RcStreak2 = bt.GetTreeStreak(2, super: false, max: false);
        RmStreak0 = bt.GetTreeStreak(0, super: false, max: true);
        RmStreak1 = bt.GetTreeStreak(1, super: false, max: true);
        RmStreak2 = bt.GetTreeStreak(2, super: false, max: true);

        ScStreak0 = bt.GetTreeStreak(0, super: true, max: false);
        ScStreak1 = bt.GetTreeStreak(1, super: true, max: false);
        ScStreak2 = bt.GetTreeStreak(2, super: true, max: false);
        SmStreak0 = bt.GetTreeStreak(0, super: true, max: true);
        SmStreak1 = bt.GetTreeStreak(1, super: true, max: true);
        SmStreak2 = bt.GetTreeStreak(2, super: true, max: true);

        // Unlocks
        MegaUnlocked = _sav.MyStatus.MegaUnlocked;
        ZMoveUnlocked = _sav.MyStatus.ZMoveUnlocked;
        UnlockSuperSingles = _sav.EventWork.GetEventFlag(333);
        UnlockSuperDoubles = _sav.EventWork.GetEventFlag(334);
        UnlockSuperMulti = _sav.EventWork.GetEventFlag(335);

        PlazaName = _sav.Festa.FestivalPlazaName;

        // USUM
        if (_sav is SAV7USUM)
        {
            Surf0 = _sav.Misc.GetSurfScore(0);
            Surf1 = _sav.Misc.GetSurfScore(1);
            Surf2 = _sav.Misc.GetSurfScore(2);
            Surf3 = _sav.Misc.GetSurfScore(3);
            RotomOt = _sav.FieldMenu.RotomOT;
            RotomAffection = Math.Min(1000, (int)_sav.FieldMenu.RotomAffection);
            RotoLoto1 = _sav.FieldMenu.RotomLoto1;
            RotoLoto2 = _sav.FieldMenu.RotomLoto2;
        }
    }

    [RelayCommand]
    private void MaxCash()
    {
        Money = "9999999";
    }

    [RelayCommand]
    private void Save()
    {
        _sav.OT = OtName;
        _sav.Gender = (byte)Gender;
        _sav.Money = uint.TryParse(Money, out var m) ? m : 0u;
        _sav.Language = Language;

        _sav.PlayedHours = (ushort)Math.Clamp(PlayedHours, 0, ushort.MaxValue);
        _sav.PlayedMinutes = (ushort)Math.Clamp(PlayedMinutes, 0, 59);
        _sav.PlayedSeconds = (ushort)Math.Clamp(PlayedSeconds, 0, 59);

        _sav.Misc.BP = (uint)Math.Max(0, Bp);
        _sav.Festa.FestaCoins = Fc;

        // Poke Finder
        _sav.PokeFinder.SnapCount = (uint)Math.Max(0, SnapCount);
        _sav.PokeFinder.ThumbsTotalValue = (uint)Math.Max(0, ThumbsTotal);
        _sav.PokeFinder.ThumbsHighValue = (uint)Math.Max(0, ThumbsRecord);
        _sav.PokeFinder.CameraVersion = (ushort)Math.Clamp(CameraVersion, 0, ushort.MaxValue);
        _sav.PokeFinder.GyroFlag = GyroFlag;

        // Battle Tree
        var bt = _sav.BattleTree;
        bt.SetTreeStreak(RcStreak0, 0, super: false, max: false);
        bt.SetTreeStreak(RcStreak1, 1, super: false, max: false);
        bt.SetTreeStreak(RcStreak2, 2, super: false, max: false);
        bt.SetTreeStreak(RmStreak0, 0, super: false, max: true);
        bt.SetTreeStreak(RmStreak1, 1, super: false, max: true);
        bt.SetTreeStreak(RmStreak2, 2, super: false, max: true);

        bt.SetTreeStreak(ScStreak0, 0, super: true, max: false);
        bt.SetTreeStreak(ScStreak1, 1, super: true, max: false);
        bt.SetTreeStreak(ScStreak2, 2, super: true, max: false);
        bt.SetTreeStreak(SmStreak0, 0, super: true, max: true);
        bt.SetTreeStreak(SmStreak1, 1, super: true, max: true);
        bt.SetTreeStreak(SmStreak2, 2, super: true, max: true);

        // Unlocks
        _sav.MyStatus.MegaUnlocked = MegaUnlocked;
        _sav.MyStatus.ZMoveUnlocked = ZMoveUnlocked;
        _sav.EventWork.SetEventFlag(333, UnlockSuperSingles);
        _sav.EventWork.SetEventFlag(334, UnlockSuperDoubles);
        _sav.EventWork.SetEventFlag(335, UnlockSuperMulti);

        _sav.Festa.FestivalPlazaName = PlazaName;

        // USUM
        if (_sav is SAV7USUM)
        {
            _sav.Misc.SetSurfScore(0, Surf0);
            _sav.Misc.SetSurfScore(1, Surf1);
            _sav.Misc.SetSurfScore(2, Surf2);
            _sav.Misc.SetSurfScore(3, Surf3);
            _sav.FieldMenu.RotomOT = RotomOt;
            _sav.FieldMenu.RotomAffection = (ushort)Math.Clamp(RotomAffection, 0, ushort.MaxValue);
            _sav.FieldMenu.RotomLoto1 = RotoLoto1;
            _sav.FieldMenu.RotomLoto2 = RotoLoto2;
        }

        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }
}

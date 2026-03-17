using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the Sword/Shield trainer editor.
/// Edits OT name, money, watts, BP, play time, battle tower stats, etc.
/// </summary>
public partial class Trainer8ViewModel : SaveEditorViewModelBase
{
    private readonly SaveFile _origin;
    private readonly SAV8SWSH _sav;

    public string[] GenderChoices { get; } = ["Male", "Female"];
    public string[] GameChoices { get; } = ["Sword", "Shield"];
    public string[] FashionChoices { get; } = ["Base Fashion", "Full Legal", "Everything"];

    [ObservableProperty] private string _otName = string.Empty;
    [ObservableProperty] private string _trainerCardName = string.Empty;
    [ObservableProperty] private string _trainerCardNumber = string.Empty;
    [ObservableProperty] private string _trainerCardId = "000000";
    [ObservableProperty] private string _rotoRallyScore = "0";
    [ObservableProperty] private string _money = "0";
    [ObservableProperty] private string _watt = "0";
    [ObservableProperty] private int _gender;
    [ObservableProperty] private int _gameIndex;
    [ObservableProperty] private int _bp;

    [ObservableProperty] private int _playedHours;
    [ObservableProperty] private int _playedMinutes;
    [ObservableProperty] private int _playedSeconds;

    [ObservableProperty] private string _battleTowerSinglesWin = "0";
    [ObservableProperty] private string _battleTowerDoublesWin = "0";
    [ObservableProperty] private string _battleTowerSinglesStreak = "0";
    [ObservableProperty] private string _battleTowerDoublesStreak = "0";

    [ObservableProperty] private int _fashionIndex = 1;
    [ObservableProperty] private bool _showCollectDiglett;

    public Trainer8ViewModel(SAV8SWSH sav) : base(sav)
    {
        _sav = (SAV8SWSH)(_origin = sav).Clone();
        ShowCollectDiglett = _sav.SaveRevision != 0;
        LoadData();
    }

    private void LoadData()
    {
        GameIndex = _sav.Version - GameVersion.SW;
        Gender = _sav.Gender;
        OtName = _sav.OT;
        TrainerCardName = _sav.Blocks.TrainerCard.OT;
        TrainerCardNumber = _sav.Blocks.TrainerCard.Number;
        TrainerCardId = _sav.Blocks.TrainerCard.TrainerID.ToString("000000");
        RotoRallyScore = _sav.Blocks.TrainerCard.RotoRallyScore.ToString();
        Money = _sav.Money.ToString();
        Watt = _sav.MyStatus.Watt.ToString();

        Bp = (int)Math.Min(_sav.Misc.BP, 9999);

        PlayedHours = _sav.PlayedHours;
        PlayedMinutes = _sav.PlayedMinutes;
        PlayedSeconds = _sav.PlayedSeconds;

        BattleTowerSinglesWin = _sav.GetValue<uint>(SaveBlockAccessor8SWSH.KBattleTowerSinglesVictory).ToString();
        BattleTowerDoublesWin = _sav.GetValue<uint>(SaveBlockAccessor8SWSH.KBattleTowerDoublesVictory).ToString();
        BattleTowerSinglesStreak = _sav.GetValue<ushort>(SaveBlockAccessor8SWSH.KBattleTowerSinglesStreak).ToString();
        BattleTowerDoublesStreak = _sav.GetValue<ushort>(SaveBlockAccessor8SWSH.KBattleTowerDoublesStreak).ToString();
    }

    [RelayCommand]
    private void MaxCash() => Money = _sav.MaxMoney.ToString();

    [RelayCommand]
    private void MaxWatt() => Watt = MyStatus8.MaxWatt.ToString();

    [RelayCommand]
    private void CollectAllDiglett()
    {
        _sav.UnlockAllDiglett();
    }

    [RelayCommand]
    private void ApplyFashion()
    {
        _sav.Fashion.Clear();
        switch (FashionIndex)
        {
            case 0: _sav.Fashion.Reset(); break;
            case 1: _sav.Fashion.UnlockAllLegal(); break;
            case 2: _sav.Fashion.UnlockAll(); break;
        }
    }

    [RelayCommand]
    private void CopyPartyToTrainerCard()
    {
        _sav.Blocks.TrainerCard.SetPartyData();
    }

    [RelayCommand]
    private void CopyPartyToTitleScreen()
    {
        _sav.Blocks.TitleScreen.SetPartyData();
    }

    [RelayCommand]
    private void Save()
    {
        _sav.Version = (GameVersion)(GameIndex + (int)GameVersion.SW);
        _sav.Gender = (byte)Math.Clamp(Gender, 0, 255);
        _sav.OT = OtName;
        _sav.Blocks.TrainerCard.OT = TrainerCardName;
        _sav.Blocks.MyStatus.Number = _sav.Blocks.TrainerCard.Number = TrainerCardNumber;
        _sav.Blocks.TrainerCard.TrainerID = int.TryParse(TrainerCardId, out var tcid) ? tcid : 0;
        _sav.Blocks.TrainerCard.RotoRallyScore = int.TryParse(RotoRallyScore, out var rr) ? rr : 0;

        _sav.Money = uint.TryParse(Money, out var money) ? (uint)Math.Min(money, (uint)_sav.MaxMoney) : 0u;

        var watt = uint.TryParse(Watt, out var w) ? w : 0u;
        _sav.MyStatus.Watt = watt;
        if (_sav.GetRecord(Record8.WattTotal) < watt)
            _sav.SetRecord(Record8.WattTotal, (int)Math.Min(watt, int.MaxValue));

        _sav.Misc.BP = Bp;

        _sav.PlayedHours = (ushort)Math.Clamp(PlayedHours, 0, ushort.MaxValue);
        _sav.PlayedMinutes = (ushort)Math.Clamp(PlayedMinutes, 0, 59);
        _sav.PlayedSeconds = (ushort)Math.Clamp(PlayedSeconds, 0, 59);

        // Battle Tower
        var singles = Math.Min(9_999_999u, uint.TryParse(BattleTowerSinglesWin, out var sv) ? sv : 0u);
        var doubles = Math.Min(9_999_999u, uint.TryParse(BattleTowerDoublesWin, out var dv) ? dv : 0u);
        _sav.SetValue(SaveBlockAccessor8SWSH.KBattleTowerSinglesVictory, singles);
        _sav.SetValue(SaveBlockAccessor8SWSH.KBattleTowerDoublesVictory, doubles);
        _sav.SetValue(SaveBlockAccessor8SWSH.KBattleTowerSinglesStreak, (ushort)Math.Min(300, uint.TryParse(BattleTowerSinglesStreak, out var ss) ? ss : 0u));
        _sav.SetValue(SaveBlockAccessor8SWSH.KBattleTowerDoublesStreak, (ushort)Math.Min(300, uint.TryParse(BattleTowerDoublesStreak, out var ds) ? ds : 0u));

        _sav.SetRecord(RecordLists.G8BattleTowerSingleWin, (int)singles);
        _sav.SetRecord(RecordLists.G8BattleTowerDoubleWin, (int)doubles);

        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }
}

using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Misc editor for Gen 8 Sword/Shield saves covering Battle Tower, Watts, BP.
/// </summary>
public partial class Misc8EditorViewModel : ViewModelBase
{
    private readonly SAV8SWSH _sav;

    public Misc8EditorViewModel(SAV8SWSH sav)
    {
        _sav = sav;
        IsIoA = sav.SaveRevision >= 1; // Isle of Armor+
        LoadMisc();
        LoadBattleTower();
    }

    public bool IsIoA { get; }

    #region Misc

    [ObservableProperty] private uint _watts;
    [ObservableProperty] private int _bp;

    private void LoadMisc()
    {
        Watts = _sav.MyStatus.Watt;
        Bp = _sav.Misc.BP;
    }

    private void SaveMisc()
    {
        _sav.MyStatus.Watt = Watts;
        if (_sav.GetRecord(Record8.WattTotal) < (int)Watts)
            _sav.SetRecord(Record8.WattTotal, (int)Watts);
        _sav.Misc.BP = Bp;
    }

    [RelayCommand]
    private void MaxWatts()
    {
        Watts = MyStatus8.MaxWatt;
    }

    [RelayCommand]
    private void MaxBP()
    {
        Bp = 9999;
    }

    #endregion

    #region Battle Tower

    [ObservableProperty] private uint _singlesWins;
    [ObservableProperty] private uint _doublesWins;
    [ObservableProperty] private ushort _singlesStreak;
    [ObservableProperty] private ushort _doublesStreak;

    private void LoadBattleTower()
    {
        SinglesWins = _sav.GetValue<uint>(SaveBlockAccessor8SWSH.KBattleTowerSinglesVictory);
        DoublesWins = _sav.GetValue<uint>(SaveBlockAccessor8SWSH.KBattleTowerDoublesVictory);
        SinglesStreak = _sav.GetValue<ushort>(SaveBlockAccessor8SWSH.KBattleTowerSinglesStreak);
        DoublesStreak = _sav.GetValue<ushort>(SaveBlockAccessor8SWSH.KBattleTowerDoublesStreak);
    }

    private void SaveBattleTower()
    {
        var singles = Math.Min(9_999_999u, SinglesWins);
        var doubles = Math.Min(9_999_999u, DoublesWins);
        _sav.SetValue(SaveBlockAccessor8SWSH.KBattleTowerSinglesVictory, singles);
        _sav.SetValue(SaveBlockAccessor8SWSH.KBattleTowerDoublesVictory, doubles);
        _sav.SetValue(SaveBlockAccessor8SWSH.KBattleTowerSinglesStreak, (ushort)Math.Min(300, (int)SinglesStreak));
        _sav.SetValue(SaveBlockAccessor8SWSH.KBattleTowerDoublesStreak, (ushort)Math.Min(300, (int)DoublesStreak));

        _sav.SetRecord(RecordLists.G8BattleTowerSingleWin, (int)singles);
        _sav.SetRecord(RecordLists.G8BattleTowerDoubleWin, (int)doubles);
    }

    #endregion

    #region Fashion

    [RelayCommand]
    private void UnlockAllFashion()
    {
        _sav.Fashion.UnlockAllLegal();
    }

    #endregion

    #region Diglett (IoA)

    [RelayCommand]
    private void CollectAllDiglett()
    {
        if (IsIoA)
            _sav.UnlockAllDiglett();
    }

    #endregion

    #region Commands

    [RelayCommand]
    private void Save()
    {
        SaveMisc();
        SaveBattleTower();
    }

    #endregion
}

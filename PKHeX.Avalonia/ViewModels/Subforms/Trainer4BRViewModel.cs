using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the Battle Revolution trainer editor.
/// Edits trainer info, records, and colosseum unlocks.
/// </summary>
public partial class Trainer4BRViewModel : SaveEditorViewModelBase
{
    private readonly SAV4BR SAV4BR;

    [ObservableProperty] private string _otName = string.Empty;
    [ObservableProperty] private string _birthMonth = string.Empty;
    [ObservableProperty] private string _birthDay = string.Empty;
    [ObservableProperty] private string _selfIntroduction = string.Empty;
    [ObservableProperty] private string _playerIdHex = string.Empty;
    [ObservableProperty] private uint _money;
    [ObservableProperty] private ushort _tid16;
    [ObservableProperty] private ushort _sid16;
    [ObservableProperty] private ushort _playedHours;
    [ObservableProperty] private ushort _playedMinutes;
    [ObservableProperty] private ushort _playedSeconds;

    // Records
    [ObservableProperty] private uint _recordTotalBattles;
    [ObservableProperty] private uint _recordColosseumBattles;
    [ObservableProperty] private uint _recordFreeBattles;
    [ObservableProperty] private uint _recordWiFiBattles;
    [ObservableProperty] private byte _recordGatewayColosseumClears;
    [ObservableProperty] private byte _recordMainStreetColosseumClears;
    [ObservableProperty] private byte _recordWaterfallColosseumClears;
    [ObservableProperty] private byte _recordNeonColosseumClears;
    [ObservableProperty] private byte _recordCrystalColosseumClears;
    [ObservableProperty] private byte _recordSunnyParkColosseumClears;
    [ObservableProperty] private byte _recordMagmaColosseumClears;
    [ObservableProperty] private byte _recordCourtyardColosseumClears;
    [ObservableProperty] private byte _recordSunsetColosseumClears;
    [ObservableProperty] private byte _recordStargazerColosseumClears;

    // Colosseum Unlocks
    [ObservableProperty] private bool _unlockedGatewayColosseum;
    [ObservableProperty] private bool _unlockedMainStreetColosseum;
    [ObservableProperty] private bool _unlockedWaterfallColosseum;
    [ObservableProperty] private bool _unlockedNeonColosseum;
    [ObservableProperty] private bool _unlockedCrystalColosseum;
    [ObservableProperty] private bool _unlockedSunnyParkColosseum;
    [ObservableProperty] private bool _unlockedMagmaColosseum;
    [ObservableProperty] private bool _unlockedSunsetColosseum;
    [ObservableProperty] private bool _unlockedCourtyardColosseum;
    [ObservableProperty] private bool _unlockedStargazerColosseum;
    [ObservableProperty] private bool _unlockedPostGame;

    public Trainer4BRViewModel(SAV4BR sav) : base(sav)
    {
        SAV4BR = sav;

        _otName = sav.CurrentOT;
        _birthMonth = sav.BirthMonth;
        _birthDay = sav.BirthDay;
        _selfIntroduction = sav.SelfIntroduction.TrimStart(StringConverter4GC.Proportional).Replace(StringConverter4GC.LineBreak, '\n');
        _playerIdHex = sav.PlayerID.ToString("X16");
        _money = sav.Money;
        _tid16 = sav.TID16;
        _sid16 = sav.SID16;
        _playedHours = (ushort)sav.PlayedHours;
        _playedMinutes = (ushort)sav.PlayedMinutes;
        _playedSeconds = (ushort)sav.PlayedSeconds;

        _recordTotalBattles = sav.RecordTotalBattles;
        _recordColosseumBattles = sav.RecordColosseumBattles;
        _recordFreeBattles = sav.RecordFreeBattles;
        _recordWiFiBattles = sav.RecordWiFiBattles;
        _recordGatewayColosseumClears = sav.RecordGatewayColosseumClears;
        _recordMainStreetColosseumClears = sav.RecordMainStreetColosseumClears;
        _recordWaterfallColosseumClears = sav.RecordWaterfallColosseumClears;
        _recordNeonColosseumClears = sav.RecordNeonColosseumClears;
        _recordCrystalColosseumClears = sav.RecordCrystalColosseumClears;
        _recordSunnyParkColosseumClears = sav.RecordSunnyParkColosseumClears;
        _recordMagmaColosseumClears = sav.RecordMagmaColosseumClears;
        _recordCourtyardColosseumClears = sav.RecordCourtyardColosseumClears;
        _recordSunsetColosseumClears = sav.RecordSunsetColosseumClears;
        _recordStargazerColosseumClears = sav.RecordStargazerColosseumClears;

        _unlockedGatewayColosseum = sav.UnlockedGatewayColosseum;
        _unlockedMainStreetColosseum = sav.UnlockedMainStreetColosseum;
        _unlockedWaterfallColosseum = sav.UnlockedWaterfallColosseum;
        _unlockedNeonColosseum = sav.UnlockedNeonColosseum;
        _unlockedCrystalColosseum = sav.UnlockedCrystalColosseum;
        _unlockedSunnyParkColosseum = sav.UnlockedSunnyParkColosseum;
        _unlockedMagmaColosseum = sav.UnlockedMagmaColosseum;
        _unlockedSunsetColosseum = sav.UnlockedSunsetColosseum;
        _unlockedCourtyardColosseum = sav.UnlockedCourtyardColosseum;
        _unlockedStargazerColosseum = sav.UnlockedStargazerColosseum;
        _unlockedPostGame = sav.UnlockedPostGame;
    }

    [RelayCommand]
    private void MaxCash() => Money = (uint)SAV4BR.MaxMoney;

    [RelayCommand]
    private void Save()
    {
        if (SAV4BR.CurrentOT != OtName)
            SAV4BR.CurrentOT = OtName;

        SAV4BR.BirthMonth = BirthMonth;
        SAV4BR.BirthDay = BirthDay;
        SAV4BR.SelfIntroduction = SelfIntroduction.Replace("\n", StringConverter4GC.LineBreak.ToString());
        SAV4BR.PlayerID = ulong.TryParse(PlayerIdHex, System.Globalization.NumberStyles.HexNumber, null, out var playerId) ? playerId : 0;
        SAV4BR.Money = Money;
        SAV4BR.TID16 = Tid16;
        SAV4BR.SID16 = Sid16;
        SAV4BR.PlayedHours = PlayedHours;
        SAV4BR.PlayedMinutes = (ushort)(PlayedMinutes % 60);
        SAV4BR.PlayedSeconds = (ushort)(PlayedSeconds % 60);

        SAV4BR.RecordTotalBattles = RecordTotalBattles;
        SAV4BR.RecordColosseumBattles = RecordColosseumBattles;
        SAV4BR.RecordFreeBattles = RecordFreeBattles;
        SAV4BR.RecordWiFiBattles = RecordWiFiBattles;
        SAV4BR.RecordGatewayColosseumClears = RecordGatewayColosseumClears;
        SAV4BR.RecordMainStreetColosseumClears = RecordMainStreetColosseumClears;
        SAV4BR.RecordWaterfallColosseumClears = RecordWaterfallColosseumClears;
        SAV4BR.RecordNeonColosseumClears = RecordNeonColosseumClears;
        SAV4BR.RecordCrystalColosseumClears = RecordCrystalColosseumClears;
        SAV4BR.RecordSunnyParkColosseumClears = RecordSunnyParkColosseumClears;
        SAV4BR.RecordMagmaColosseumClears = RecordMagmaColosseumClears;
        SAV4BR.RecordCourtyardColosseumClears = RecordCourtyardColosseumClears;
        SAV4BR.RecordSunsetColosseumClears = RecordSunsetColosseumClears;
        SAV4BR.RecordStargazerColosseumClears = RecordStargazerColosseumClears;

        SAV4BR.UnlockedGatewayColosseum = UnlockedGatewayColosseum;
        SAV4BR.UnlockedMainStreetColosseum = UnlockedMainStreetColosseum;
        SAV4BR.UnlockedWaterfallColosseum = UnlockedWaterfallColosseum;
        SAV4BR.UnlockedNeonColosseum = UnlockedNeonColosseum;
        SAV4BR.UnlockedCrystalColosseum = UnlockedCrystalColosseum;
        SAV4BR.UnlockedSunnyParkColosseum = UnlockedSunnyParkColosseum;
        SAV4BR.UnlockedMagmaColosseum = UnlockedMagmaColosseum;
        SAV4BR.UnlockedSunsetColosseum = UnlockedSunsetColosseum;
        SAV4BR.UnlockedCourtyardColosseum = UnlockedCourtyardColosseum;
        SAV4BR.UnlockedStargazerColosseum = UnlockedStargazerColosseum;
        SAV4BR.UnlockedPostGame = UnlockedPostGame;

        SAV.State.Edited = true;
        Modified = true;
    }
}

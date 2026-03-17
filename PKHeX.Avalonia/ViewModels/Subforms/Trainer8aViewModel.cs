using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the Legends Arceus trainer editor.
/// Edits OT name, money, play time, merit points, rank, satchel upgrades, etc.
/// </summary>
public partial class Trainer8aViewModel : SaveEditorViewModelBase
{
    private readonly SaveFile _origin;
    private readonly SAV8LA _sav;

    public string[] GenderChoices { get; } = ["Male", "Female"];

    [ObservableProperty] private string _otName = string.Empty;
    [ObservableProperty] private string _money = "0";
    [ObservableProperty] private int _gender;

    [ObservableProperty] private int _playedHours;
    [ObservableProperty] private int _playedMinutes;
    [ObservableProperty] private int _playedSeconds;

    [ObservableProperty] private uint _meritCurrent;
    [ObservableProperty] private uint _meritEarned;
    [ObservableProperty] private uint _rank;
    [ObservableProperty] private uint _satchelUpgrades;

    public Trainer8aViewModel(SAV8LA sav) : base(sav)
    {
        _sav = (SAV8LA)(_origin = sav).Clone();
        LoadData();
    }

    private void LoadData()
    {
        Gender = _sav.Gender;
        OtName = _sav.OT;
        Money = _sav.Money.ToString();

        PlayedHours = _sav.PlayedHours;
        PlayedMinutes = _sav.PlayedMinutes;
        PlayedSeconds = _sav.PlayedSeconds;

        MeritCurrent = LoadClamp(SaveBlockAccessor8LA.KMeritCurrent, 999999);
        MeritEarned = LoadClamp(SaveBlockAccessor8LA.KMeritEarnedTotal, 999999);
        Rank = LoadClamp(SaveBlockAccessor8LA.KExpeditionTeamRank, 999);
        SatchelUpgrades = LoadClamp(SaveBlockAccessor8LA.KSatchelUpgrades, 999);
    }

    private uint LoadClamp(uint key, uint max)
    {
        var actual = (uint)_sav.Blocks.GetBlockValue(key);
        return Math.Min(actual, max);
    }

    [RelayCommand]
    private void MaxCash() => Money = _sav.MaxMoney.ToString();

    [RelayCommand]
    private void Save()
    {
        _sav.Gender = (byte)Gender;
        _sav.OT = OtName;
        _sav.Money = uint.TryParse(Money, out var money) ? money : 0u;

        _sav.PlayedHours = (ushort)Math.Clamp(PlayedHours, 0, ushort.MaxValue);
        _sav.PlayedMinutes = (ushort)Math.Clamp(PlayedMinutes, 0, 59);
        _sav.PlayedSeconds = (ushort)Math.Clamp(PlayedSeconds, 0, 59);

        _sav.Blocks.SetBlockValue(SaveBlockAccessor8LA.KMeritCurrent, MeritCurrent);
        _sav.Blocks.SetBlockValue(SaveBlockAccessor8LA.KMeritEarnedTotal, MeritEarned);
        _sav.Blocks.SetBlockValue(SaveBlockAccessor8LA.KExpeditionTeamRank, Rank);
        _sav.Blocks.SetBlockValue(SaveBlockAccessor8LA.KSatchelUpgrades, SatchelUpgrades);

        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }
}

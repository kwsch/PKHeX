using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the Let's Go trainer editor.
/// Edits OT name, rival name, TID/SID, money, play time, coordinates, etc.
/// </summary>
public partial class SAVTrainer7GGViewModel : SaveEditorViewModelBase
{
    private readonly SaveFile _origin;
    private readonly SAV7b _sav;

    public string[] GenderChoices { get; } = ["Male", "Female"];
    public List<ComboItem> LanguageList { get; }
    public List<ComboItem> GameList { get; }

    // Basic Info
    [ObservableProperty] private string _otName = string.Empty;
    [ObservableProperty] private string _rivalName = string.Empty;
    [ObservableProperty] private int _gender;
    [ObservableProperty] private string _money = "0";
    [ObservableProperty] private int _language;
    [ObservableProperty] private int _game;

    // Play Time
    [ObservableProperty] private int _playedHours;
    [ObservableProperty] private int _playedMinutes;
    [ObservableProperty] private int _playedSeconds;

    public SAVTrainer7GGViewModel(SAV7b sav) : base(sav)
    {
        _sav = (SAV7b)(_origin = sav).Clone();
        LanguageList = GameInfo.LanguageDataSource(_sav.Generation, _sav.Context).ToList();
        GameList = GameInfo.Sources.VersionDataSource
            .Where(z => (GameVersion)z.Value is GameVersion.GP or GameVersion.GE)
            .ToList();

        LoadData();
    }

    private void LoadData()
    {
        OtName = _sav.OT;
        RivalName = _sav.Blocks.Misc.RivalName;
        Gender = _sav.Gender;
        Money = _sav.Blocks.Misc.Money.ToString();
        Language = _sav.Language;
        Game = (int)_sav.Version;

        PlayedHours = _sav.PlayedHours;
        PlayedMinutes = _sav.PlayedMinutes;
        PlayedSeconds = _sav.PlayedSeconds;
    }

    [RelayCommand]
    private void MaxCash()
    {
        Money = "9999999";
    }

    [RelayCommand]
    private void UnlockAllTitles()
    {
        _sav.Blocks.EventWork.UnlockAllTitleFlags();
    }

    [RelayCommand]
    private void UnlockAllFashion()
    {
        _sav.Blocks.FashionPlayer.UnlockAllAccessoriesPlayer();
        _sav.Blocks.FashionStarter.UnlockAllAccessoriesStarter();
    }

    [RelayCommand]
    private void Save()
    {
        _sav.Version = (GameVersion)Game;
        _sav.Gender = (byte)Gender;
        _sav.Money = uint.TryParse(Money, out var m) ? m : 0u;
        _sav.Language = Language;
        _sav.OT = OtName;
        _sav.Blocks.Misc.RivalName = RivalName;

        _sav.PlayedHours = (ushort)Math.Clamp(PlayedHours, 0, ushort.MaxValue);
        _sav.PlayedMinutes = (ushort)Math.Clamp(PlayedMinutes, 0, 59);
        _sav.PlayedSeconds = (ushort)Math.Clamp(PlayedSeconds, 0, 59);

        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }
}

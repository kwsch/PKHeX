using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;
using static PKHeX.Core.SaveBlockAccessor9SV;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the Scarlet/Violet trainer editor.
/// </summary>
public partial class Trainer9ViewModel : SaveEditorViewModelBase
{
    private readonly SaveFile _origin;
    private readonly SAV9SV _sav;

    public string[] GenderChoices { get; } = ["Male", "Female"];
    public string[] GameChoices { get; }

    [ObservableProperty] private string _otName = string.Empty;
    [ObservableProperty] private string _money = "0";
    [ObservableProperty] private string _leaguePoints = "0";
    [ObservableProperty] private string _blueberryPointsText = "0";
    [ObservableProperty] private int _gender;
    [ObservableProperty] private int _gameIndex;

    [ObservableProperty] private int _playedHours;
    [ObservableProperty] private int _playedMinutes;
    [ObservableProperty] private int _playedSeconds;

    // Map
    [ObservableProperty] private decimal _mapX;
    [ObservableProperty] private decimal _mapY;
    [ObservableProperty] private decimal _mapZ;
    [ObservableProperty] private decimal _mapR;
    [ObservableProperty] private bool _mapEnabled = true;

    // Blueberry
    [ObservableProperty] private bool _showBlueberry;
    [ObservableProperty] private string _bbqSolo = "0";
    [ObservableProperty] private string _bbqGroup = "0";

    public Trainer9ViewModel(SAV9SV sav) : base(sav)
    {
        _sav = (SAV9SV)(_origin = sav).Clone();

        var games = GameInfo.Strings.gamelist;
        GameChoices = [games[(int)GameVersion.SL], games[(int)GameVersion.VL]];

        LoadData();
    }

    private void LoadData()
    {
        GameIndex = _sav.Version - GameVersion.SL;
        Gender = _sav.Gender;
        OtName = _sav.OT;
        Money = _sav.Money.ToString();
        LeaguePoints = _sav.LeaguePoints.ToString();

        PlayedHours = _sav.PlayedHours;
        PlayedMinutes = _sav.PlayedMinutes;
        PlayedSeconds = _sav.PlayedSeconds;

        try
        {
            MapX = (decimal)(double)_sav.X;
            MapY = (decimal)(double)_sav.Y;
            MapZ = (decimal)(double)_sav.Z;
            MapR = (decimal)(Math.Atan2(_sav.RZ, _sav.RW) * 360.0 / Math.PI);
        }
        catch
        {
            MapEnabled = false;
        }

        ShowBlueberry = _sav.SaveRevision >= 2;
        if (ShowBlueberry)
        {
            BlueberryPointsText = _sav.BlueberryPoints.ToString();
            var bbq = _sav.BlueberryQuestRecord;
            BbqSolo = bbq.QuestsDoneSolo.ToString();
            BbqGroup = bbq.QuestsDoneGroup.ToString();
        }
    }

    [RelayCommand]
    private void MaxCash() => Money = _sav.MaxMoney.ToString();

    [RelayCommand]
    private void MaxLP() => LeaguePoints = _sav.MaxMoney.ToString();

    [RelayCommand]
    private void MaxBP() => BlueberryPointsText = _sav.MaxMoney.ToString();

    [RelayCommand]
    private void UnlockFlyLocations()
    {
        var accessor = _sav.Accessor;
        ReadOnlySpan<uint> flyHashes =
        [
            FSYS_YMAP_FLY_01, FSYS_YMAP_FLY_02, FSYS_YMAP_FLY_03, FSYS_YMAP_FLY_04,
            FSYS_YMAP_FLY_05, FSYS_YMAP_FLY_06, FSYS_YMAP_FLY_07, FSYS_YMAP_FLY_08,
            FSYS_YMAP_FLY_09, FSYS_YMAP_FLY_10, FSYS_YMAP_FLY_11, FSYS_YMAP_FLY_12,
            FSYS_YMAP_FLY_13, FSYS_YMAP_FLY_14, FSYS_YMAP_FLY_15, FSYS_YMAP_FLY_16,
            FSYS_YMAP_FLY_17, FSYS_YMAP_FLY_18, FSYS_YMAP_FLY_19, FSYS_YMAP_FLY_20,
            FSYS_YMAP_FLY_21, FSYS_YMAP_FLY_22, FSYS_YMAP_FLY_23, FSYS_YMAP_FLY_24,
            FSYS_YMAP_FLY_25, FSYS_YMAP_FLY_26, FSYS_YMAP_FLY_27, FSYS_YMAP_FLY_28,
            FSYS_YMAP_FLY_29, FSYS_YMAP_FLY_30, FSYS_YMAP_FLY_31, FSYS_YMAP_FLY_32,
            FSYS_YMAP_FLY_33, FSYS_YMAP_FLY_34, FSYS_YMAP_FLY_35,
            FSYS_YMAP_FLY_MAGATAMA, FSYS_YMAP_FLY_MOKKAN, FSYS_YMAP_FLY_TSURUGI, FSYS_YMAP_FLY_UTSUWA,
            FSYS_YMAP_POKECEN_02, FSYS_YMAP_POKECEN_03, FSYS_YMAP_POKECEN_04, FSYS_YMAP_POKECEN_05,
            FSYS_YMAP_POKECEN_06, FSYS_YMAP_POKECEN_07, FSYS_YMAP_POKECEN_08, FSYS_YMAP_POKECEN_09,
            FSYS_YMAP_POKECEN_10, FSYS_YMAP_POKECEN_11, FSYS_YMAP_POKECEN_12, FSYS_YMAP_POKECEN_13,
            FSYS_YMAP_POKECEN_14, FSYS_YMAP_POKECEN_15, FSYS_YMAP_POKECEN_16, FSYS_YMAP_POKECEN_17,
            FSYS_YMAP_POKECEN_18, FSYS_YMAP_POKECEN_19, FSYS_YMAP_POKECEN_20, FSYS_YMAP_POKECEN_21,
            FSYS_YMAP_POKECEN_22, FSYS_YMAP_POKECEN_23, FSYS_YMAP_POKECEN_24, FSYS_YMAP_POKECEN_25,
            FSYS_YMAP_POKECEN_26, FSYS_YMAP_POKECEN_27, FSYS_YMAP_POKECEN_28, FSYS_YMAP_POKECEN_29,
            FSYS_YMAP_POKECEN_30, FSYS_YMAP_POKECEN_31, FSYS_YMAP_POKECEN_32, FSYS_YMAP_POKECEN_33,
            FSYS_YMAP_POKECEN_34, FSYS_YMAP_POKECEN_35,
            FSYS_YMAP_MAGATAMA, FSYS_YMAP_MOKKAN, FSYS_YMAP_TSURUGI, FSYS_YMAP_UTSUWA,
            FSYS_YMAP_SU1MAP_CHANGE,
            FSYS_YMAP_FLY_SU1_AREA10, FSYS_YMAP_FLY_SU1_BUSSTOP, FSYS_YMAP_FLY_SU1_CENTER01,
            FSYS_YMAP_FLY_SU1_PLAZA, FSYS_YMAP_FLY_SU1_SPOT01, FSYS_YMAP_FLY_SU1_SPOT02,
            FSYS_YMAP_FLY_SU1_SPOT03, FSYS_YMAP_FLY_SU1_SPOT04, FSYS_YMAP_FLY_SU1_SPOT05,
            FSYS_YMAP_FLY_SU1_SPOT06,
            FSYS_YMAP_S2_MAPCHANGE_ENABLE,
            FSYS_YMAP_FLY_SU2_DRAGON, FSYS_YMAP_FLY_SU2_ENTRANCE, FSYS_YMAP_FLY_SU2_FAIRY,
            FSYS_YMAP_FLY_SU2_HAGANE, FSYS_YMAP_FLY_SU2_HONOO,
            FSYS_YMAP_FLY_SU2_SPOT01, FSYS_YMAP_FLY_SU2_SPOT02, FSYS_YMAP_FLY_SU2_SPOT03,
            FSYS_YMAP_FLY_SU2_SPOT04, FSYS_YMAP_FLY_SU2_SPOT05, FSYS_YMAP_FLY_SU2_SPOT06,
            FSYS_YMAP_FLY_SU2_SPOT07, FSYS_YMAP_FLY_SU2_SPOT08, FSYS_YMAP_FLY_SU2_SPOT09,
            FSYS_YMAP_FLY_SU2_SPOT10, FSYS_YMAP_FLY_SU2_SPOT11,
            FSYS_YMAP_POKECEN_SU02,
        ];
        foreach (var hash in flyHashes)
        {
            if (accessor.TryGetBlock(hash, out var block))
                block.ChangeBooleanType(SCTypeCode.Bool2);
        }
    }

    [RelayCommand]
    private void CollectAllStakes() => _sav.CollectAllStakes();

    [RelayCommand]
    private void UnlockTMRecipes() => _sav.UnlockAllTMRecipes();

    [RelayCommand]
    private void ActivateSnacksworthLegendaries() => _sav.ActivateSnacksworthLegendaries();

    [RelayCommand]
    private void UnlockCoaches() => _sav.UnlockAllCoaches();

    [RelayCommand]
    private void UnlockBikeUpgrades()
    {
        string[] blocks =
        [
            "FSYS_RIDE_DASH_ENABLE", "FSYS_RIDE_SWIM_ENABLE",
            "FSYS_RIDE_HIJUMP_ENABLE", "FSYS_RIDE_GLIDE_ENABLE", "FSYS_RIDE_CLIMB_ENABLE",
        ];
        var accessor = _sav.Accessor;
        foreach (var b in blocks)
            accessor.GetBlock(b).ChangeBooleanType(SCTypeCode.Bool2);
        if (accessor.TryGetBlock("FSYS_RIDE_FLIGHT_ENABLE", out var fly))
            fly.ChangeBooleanType(SCTypeCode.Bool2);
    }

    [RelayCommand]
    private void UnlockClothing()
    {
        PlayerFashionUnlock9.UnlockBase(_sav.Accessor, _sav.Gender);
    }

    [RelayCommand]
    private void UnlockThrowStyles() => _sav.UnlockAllThrowStyles();

    [RelayCommand]
    private void Save()
    {
        _sav.Version = (GameVersion)(GameIndex + (byte)GameVersion.SL);
        _sav.Gender = (byte)Gender;
        _sav.OT = OtName;
        _sav.Money = uint.TryParse(Money, out var m) ? m : 0u;
        _sav.LeaguePoints = uint.TryParse(LeaguePoints, out var lp) ? lp : 0u;

        _sav.PlayedHours = (ushort)PlayedHours;
        _sav.PlayedMinutes = (ushort)(PlayedMinutes % 60);
        _sav.PlayedSeconds = (ushort)(PlayedSeconds % 60);

        if (MapEnabled)
        {
            _sav.SetCoordinates((float)MapX, (float)MapY, (float)MapZ);
            var angle = (double)MapR * Math.PI / 360.0;
            _sav.SetPlayerRotation(0, (float)Math.Sin(angle), 0, (float)Math.Cos(angle));
        }

        if (ShowBlueberry)
        {
            if (_sav.Blocks.TryGetBlock(KBlueberryPoints, out var bpBlock))
                bpBlock.SetValue(uint.TryParse(BlueberryPointsText, out var bp) ? bp : 0u);
            var bbq = _sav.BlueberryQuestRecord;
            bbq.QuestsDoneSolo = uint.TryParse(BbqSolo, out var solo) ? solo : 0u;
            bbq.QuestsDoneGroup = uint.TryParse(BbqGroup, out var group) ? group : 0u;
        }

        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }
}

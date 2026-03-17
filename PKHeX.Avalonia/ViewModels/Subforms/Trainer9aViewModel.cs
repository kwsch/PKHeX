using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;
using static PKHeX.Core.SaveBlockAccessor9ZA;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the Legends Z-A trainer editor.
/// </summary>
public partial class Trainer9aViewModel : SaveEditorViewModelBase
{
    private readonly SaveFile _origin;
    private readonly SAV9ZA _sav;

    public string[] GenderChoices { get; } = ["Male", "Female"];

    [ObservableProperty] private string _otName = string.Empty;
    [ObservableProperty] private string _money = "0";
    [ObservableProperty] private string _royaleRegular = "0";
    [ObservableProperty] private string _royaleInfinite = "0";
    [ObservableProperty] private int _gender;

    [ObservableProperty] private int _playedHours;
    [ObservableProperty] private int _playedMinutes;
    [ObservableProperty] private int _playedSeconds;

    // Map
    [ObservableProperty] private decimal _mapX;
    [ObservableProperty] private decimal _mapY;
    [ObservableProperty] private decimal _mapZ;
    [ObservableProperty] private decimal _mapR;
    [ObservableProperty] private string _mapName = string.Empty;
    [ObservableProperty] private bool _mapEnabled = true;

    // DLC
    [ObservableProperty] private bool _showDlc;
    [ObservableProperty] private string _hyperspaceSurveyPoints = "0";
    [ObservableProperty] private string _streetName = string.Empty;

    public Trainer9aViewModel(SAV9ZA sav) : base(sav)
    {
        _sav = (SAV9ZA)(_origin = sav).Clone();
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

        RoyaleRegular = _sav.TicketPointsRoyale.ToString();
        RoyaleInfinite = _sav.TicketPointsRoyaleInfinite.ToString();

        try
        {
            MapX = (decimal)(double)_sav.Coordinates.X;
            MapY = (decimal)(double)_sav.Coordinates.Y;
            MapZ = (decimal)(double)_sav.Coordinates.Z;
            MapR = (decimal)_sav.Coordinates.Rotation;
            MapName = _sav.Coordinates.Map;
        }
        catch
        {
            MapEnabled = false;
        }

        ShowDlc = _sav.SaveRevision != 0;
        if (ShowDlc)
        {
            HyperspaceSurveyPoints = _sav.GetValue<uint>(KHyperspaceSurveyPoints).ToString();
            StreetName = _sav.GetString(_sav.Blocks.GetBlock(KStreetName).Data);
        }
    }

    [RelayCommand]
    private void MaxCash() => Money = _sav.MaxMoney.ToString();

    [RelayCommand]
    private void MaxRoyaleRegular() => RoyaleRegular = 310_000.ToString();

    [RelayCommand]
    private void MaxRoyaleInfinite() => RoyaleInfinite = 50_000.ToString();

    [RelayCommand]
    private void MaxHyperspaceSurvey() => HyperspaceSurveyPoints = 100_000.ToString();

    [RelayCommand]
    private void CollectTechnicalMachines()
    {
        TechnicalMachine9a.SetAllTechnicalMachines(_sav, true);
    }

    [RelayCommand]
    private void CollectScrews()
    {
        ColorfulScrew9a.CollectScrews(_sav);
    }

    [RelayCommand]
    private void Save()
    {
        if (_sav.Gender != (byte)Gender)
        {
            _sav.Gender = (byte)Gender;
            _sav.PlayerFashion.Reset();
        }

        _sav.OT = OtName;
        _sav.Money = uint.TryParse(Money, out var m) ? (uint)Math.Min(m, (uint)_sav.MaxMoney) : 0u;

        _sav.PlayedHours = (ushort)Math.Clamp(PlayedHours, 0, ushort.MaxValue);
        _sav.PlayedMinutes = (ushort)Math.Clamp(PlayedMinutes, 0, 59);
        _sav.PlayedSeconds = (ushort)Math.Clamp(PlayedSeconds, 0, 59);

        _sav.TicketPointsRoyale = uint.TryParse(RoyaleRegular, out var rr) ? rr : 0u;
        _sav.TicketPointsRoyaleInfinite = uint.TryParse(RoyaleInfinite, out var ri) ? ri : 0u;

        if (MapEnabled)
        {
            _sav.Coordinates.SetCoordinates((float)MapX, (float)MapY, (float)MapZ);
            _sav.Coordinates.SetPlayerRotation((double)MapR);
            _sav.Coordinates.Map = MapName;
        }

        if (ShowDlc)
        {
            _sav.SetValue(KHyperspaceSurveyPoints, uint.TryParse(HyperspaceSurveyPoints, out var hsp) ? hsp : 0u);
            _sav.SetString(_sav.Blocks.GetBlock(KStreetName).Data, StreetName, 18, StringConverterOption.ClearZero);
        }

        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }
}

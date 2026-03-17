using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the Gen 6 trainer editor.
/// Edits OT name, TID/SID, money, play time, badges, BP, pokemiles, maison records, etc.
/// </summary>
public partial class SAVTrainer6ViewModel : SaveEditorViewModelBase
{
    private readonly SaveFile _origin;
    private readonly SAV6 _sav;

    public string[] GenderChoices { get; } = ["Male", "Female"];

    [ObservableProperty]
    private string _otName = string.Empty;

    [ObservableProperty]
    private string _tid = "00000";

    [ObservableProperty]
    private string _sid = "00000";

    [ObservableProperty]
    private string _money = "0";

    [ObservableProperty]
    private int _gender;

    [ObservableProperty]
    private int _playedHours;

    [ObservableProperty]
    private int _playedMinutes;

    [ObservableProperty]
    private int _playedSeconds;

    [ObservableProperty]
    private string _bp = "0";

    [ObservableProperty]
    private string _pokemiles = "0";

    [ObservableProperty]
    private string _style = "0";

    [ObservableProperty]
    private bool _megaUnlocked;

    [ObservableProperty]
    private bool _megaRayquazaUnlocked;

    [ObservableProperty]
    private bool _showStyle;

    [ObservableProperty]
    private bool _showMegaRayquaza;

    // Badges
    [ObservableProperty]
    private bool _badge1;

    [ObservableProperty]
    private bool _badge2;

    [ObservableProperty]
    private bool _badge3;

    [ObservableProperty]
    private bool _badge4;

    [ObservableProperty]
    private bool _badge5;

    [ObservableProperty]
    private bool _badge6;

    [ObservableProperty]
    private bool _badge7;

    [ObservableProperty]
    private bool _badge8;

    // Sayings
    [ObservableProperty]
    private string _saying1 = string.Empty;

    [ObservableProperty]
    private string _saying2 = string.Empty;

    [ObservableProperty]
    private string _saying3 = string.Empty;

    [ObservableProperty]
    private string _saying4 = string.Empty;

    [ObservableProperty]
    private string _saying5 = string.Empty;

    /// <summary>Whether the save contains JPEG trainer photo data.</summary>
    public bool HasJpegData => _sav.GetJPEGData().Length > 0;

    /// <summary>Suggested filename for the JPEG export.</summary>
    public string JpegFileName => $"{_sav.JPEGTitle}'s picture";

    /// <summary>The raw JPEG bytes from the save, or empty.</summary>
    public byte[] GetJpegBytes() => _sav.GetJPEGData().ToArray();

    /// <summary>Status text shown after JPEG export attempt.</summary>
    [ObservableProperty]
    private string _jpegExportStatus = string.Empty;

    public SAVTrainer6ViewModel(SAV6 sav) : base(sav)
    {
        _sav = (SAV6)(_origin = sav).Clone();

        ShowStyle = _sav is SAV6XY;
        ShowMegaRayquaza = _sav is SAV6AO;

        LoadData();
    }

    private void LoadData()
    {
        OtName = _sav.OT;
        Tid = _sav.TID16.ToString("00000");
        Sid = _sav.SID16.ToString("00000");
        Money = _sav.Money.ToString();
        Gender = _sav.Gender;

        PlayedHours = _sav.PlayedHours;
        PlayedMinutes = _sav.PlayedMinutes;
        PlayedSeconds = _sav.PlayedSeconds;

        Bp = _sav.BP.ToString();
        Pokemiles = _sav.GetRecord(63).ToString();

        var sit = _sav.Situation;
        Style = sit.Style.ToString();

        int badgeval = _sav.Badges;
        Badge1 = (badgeval & (1 << 0)) != 0;
        Badge2 = (badgeval & (1 << 1)) != 0;
        Badge3 = (badgeval & (1 << 2)) != 0;
        Badge4 = (badgeval & (1 << 3)) != 0;
        Badge5 = (badgeval & (1 << 4)) != 0;
        Badge6 = (badgeval & (1 << 5)) != 0;
        Badge7 = (badgeval & (1 << 6)) != 0;
        Badge8 = (badgeval & (1 << 7)) != 0;

        var status = _sav.Status;
        Saying1 = status.Saying1;
        Saying2 = status.Saying2;
        Saying3 = status.Saying3;
        Saying4 = status.Saying4;
        Saying5 = status.Saying5;

        MegaUnlocked = status.IsMegaEvolutionUnlocked;
        MegaRayquazaUnlocked = status.IsMegaRayquazaUnlocked;
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
        _sav.TID16 = ushort.TryParse(Tid, out var tid) ? tid : (ushort)0;
        _sav.SID16 = ushort.TryParse(Sid, out var sid) ? sid : (ushort)0;
        _sav.Money = uint.TryParse(Money, out var money) ? money : 0u;
        _sav.Gender = (byte)Gender;
        _sav.Overworld.ResetPlayerModel();

        _sav.PlayedHours = (ushort)Math.Clamp(PlayedHours, 0, ushort.MaxValue);
        _sav.PlayedMinutes = (ushort)Math.Clamp(PlayedMinutes, 0, 59);
        _sav.PlayedSeconds = (ushort)Math.Clamp(PlayedSeconds, 0, 59);

        _sav.BP = ushort.TryParse(Bp, out var bp) ? bp : (ushort)0;
        _sav.SetRecord(63, int.TryParse(Pokemiles, out var pm1) ? pm1 : 0);
        _sav.SetRecord(64, int.TryParse(Pokemiles, out var pm2) ? pm2 : 0);

        var sit = _sav.Situation;
        if (byte.TryParse(Style, out var style))
            sit.Style = style;

        int badgeval = 0;
        if (Badge1) badgeval |= 1 << 0;
        if (Badge2) badgeval |= 1 << 1;
        if (Badge3) badgeval |= 1 << 2;
        if (Badge4) badgeval |= 1 << 3;
        if (Badge5) badgeval |= 1 << 4;
        if (Badge6) badgeval |= 1 << 5;
        if (Badge7) badgeval |= 1 << 6;
        if (Badge8) badgeval |= 1 << 7;
        _sav.Badges = badgeval;

        var status = _sav.Status;
        status.Saying1 = Saying1;
        status.Saying2 = Saying2;
        status.Saying3 = Saying3;
        status.Saying4 = Saying4;
        status.Saying5 = Saying5;

        status.IsMegaEvolutionUnlocked = MegaUnlocked;
        status.IsMegaRayquazaUnlocked = MegaRayquazaUnlocked;

        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }
}

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class RaidEditorViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private RaidSpawnList8? _raids;

    public RaidEditorViewModel(SaveFile sav, string region = "Galar")
    {
        _sav = sav;
        Region = region;

        if (sav is SAV8SWSH swsh)
        {
            IsSupported = true;
            LoadRaids(swsh, region);
        }
    }

    public bool IsSupported { get; }
    public string Region { get; }

    [ObservableProperty]
    private ObservableCollection<RaidViewModel> _dens = [];

    [ObservableProperty]
    private RaidViewModel? _selectedDen;

    private void LoadRaids(SAV8SWSH swsh, string region)
    {
        _raids = region switch
        {
            "Galar" => swsh.RaidGalar,
            "Isle of Armor" => swsh.RaidArmor,
            "Crown Tundra" => swsh.RaidCrown,
            _ => swsh.RaidGalar
        };

        Dens.Clear();
        for (int i = 0; i < _raids.CountUsed; i++)
        {
            var raid = _raids.GetRaid(i);
            Dens.Add(new RaidViewModel(i, raid));
        }

        if (Dens.Count > 0)
            SelectedDen = Dens[0];
    }

    [RelayCommand]
    private void Refresh()
    {
        if (_sav is SAV8SWSH swsh)
            LoadRaids(swsh, Region);
    }
}

public partial class RaidViewModel : ViewModelBase
{
    private readonly RaidSpawnDetail _raid;

    public RaidViewModel(int index, RaidSpawnDetail raid)
    {
        Index = index;
        _raid = raid;

        _isEvent = raid.IsEvent;
        _isRare = raid.IsRare;
        _isWishingPiece = raid.IsWishingPiece;
        _seed = raid.Seed;
        _stars = raid.Stars;
        _randRoll = raid.RandRoll;
        _flags = raid.Flags;
    }

    public int Index { get; }
    public string DisplayName => $"Den {Index + 1:000}";

    // IsActive is derived from DenType, read-only
    public bool IsActive => _raid.IsActive;

    [ObservableProperty]
    private bool _isEvent;

    partial void OnIsEventChanged(bool value)
    {
        _raid.IsEvent = value;
        OnPropertyChanged(nameof(IsActive));
    }

    [ObservableProperty]
    private bool _isRare;

    partial void OnIsRareChanged(bool value)
    {
        _raid.IsRare = value;
        OnPropertyChanged(nameof(IsActive));
    }

    [ObservableProperty]
    private bool _isWishingPiece;

    partial void OnIsWishingPieceChanged(bool value)
    {
        _raid.IsWishingPiece = value;
        OnPropertyChanged(nameof(IsActive));
    }

    [ObservableProperty]
    private ulong _seed;

    partial void OnSeedChanged(ulong value) => _raid.Seed = value;

    public string SeedHex
    {
        get => Seed.ToString("X16");
        set
        {
            if (ulong.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out var parsed))
                Seed = parsed;
        }
    }

    [ObservableProperty]
    private byte _stars;

    partial void OnStarsChanged(byte value) => _raid.Stars = value;

    [ObservableProperty]
    private byte _randRoll;

    partial void OnRandRollChanged(byte value) => _raid.RandRoll = value;

    [ObservableProperty]
    private byte _flags;

    partial void OnFlagsChanged(byte value) => _raid.Flags = value;

    [RelayCommand]
    private void Deactivate()
    {
        _raid.Deactivate();
        Stars = 0;
        RandRoll = 0;
        OnPropertyChanged(nameof(IsActive));
    }
}

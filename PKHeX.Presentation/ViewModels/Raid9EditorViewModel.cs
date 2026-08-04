using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class Raid9EditorViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly SAV9SV? _sv;
    private RaidSpawnList9? _raids;

    public Raid9EditorViewModel(SaveFile sav, string region = "Paldea")
    {
        _sav = sav;
        _sv = sav as SAV9SV;
        Region = region;
        IsSupported = _sv is not null;

        if (IsSupported)
            LoadRaids(region);
    }

    public bool IsSupported { get; }
    public string Region { get; }

    [ObservableProperty]
    private string _currentSeedHex = string.Empty;

    partial void OnCurrentSeedHexChanged(string value)
    {
        if (_raids is null || !_raids.HasSeeds) return;
        if (ulong.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out var seed))
            _raids.CurrentSeed = seed;
    }

    [ObservableProperty]
    private string _tomorrowSeedHex = string.Empty;

    partial void OnTomorrowSeedHexChanged(string value)
    {
        if (_raids is null || !_raids.HasSeeds) return;
        if (ulong.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out var seed))
            _raids.TomorrowSeed = seed;
    }

    public bool HasSeeds => _raids?.HasSeeds ?? false;

    [ObservableProperty]
    private ObservableCollection<Raid9ViewModel> _raids9 = [];

    [ObservableProperty]
    private Raid9ViewModel? _selectedRaid;

    private void LoadRaids(string region)
    {
        if (_sv is null) return;

        _raids = region switch
        {
            "Paldea" => _sv.RaidPaldea,
            "Kitakami" => _sv.RaidKitakami,
            "Blueberry" => _sv.RaidBlueberry,
            _ => _sv.RaidPaldea
        };

        if (_raids.HasSeeds)
        {
            CurrentSeedHex = _raids.CurrentSeed.ToString("X16");
            TomorrowSeedHex = _raids.TomorrowSeed.ToString("X16");
        }

        Raids9.Clear();
        for (int i = 0; i < _raids.CountUsed; i++)
        {
            var raid = _raids.GetRaid(i);
            Raids9.Add(new Raid9ViewModel(i, raid));
        }

        if (Raids9.Count > 0)
            SelectedRaid = Raids9[0];

        OnPropertyChanged(nameof(HasSeeds));
    }

    [RelayCommand]
    private void Refresh() => LoadRaids(Region);

    [RelayCommand]
    private void CopyToOthers()
    {
        if (_raids is null || SelectedRaid is null) return;
        _raids.Propagate(SelectedRaid.Index, seedToo: false);
        LoadRaids(Region);
    }
}

public partial class Raid9ViewModel : ViewModelBase
{
    private readonly TeraRaidDetail _raid;

    public Raid9ViewModel(int index, TeraRaidDetail raid)
    {
        Index = index;
        _raid = raid;

        _isEnabled = raid.IsEnabled;
        _area = raid.AreaID;
        _seed = raid.Seed;
        _content = (int)raid.Content;
    }

    public int Index { get; }
    public string DisplayName => $"Raid {Index + 1:000}";

    [ObservableProperty]
    private bool _isEnabled;

    partial void OnIsEnabledChanged(bool value) => _raid.IsEnabled = value;

    [ObservableProperty]
    private uint _area;

    partial void OnAreaChanged(uint value) => _raid.AreaID = value;

    [ObservableProperty]
    private uint _seed;

    partial void OnSeedChanged(uint value) => _raid.Seed = value;

    public string SeedHex
    {
        get => Seed.ToString("X8");
        set
        {
            if (uint.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out var parsed))
                Seed = parsed;
        }
    }

    [ObservableProperty]
    private int _content;

    partial void OnContentChanged(int value) => _raid.Content = (TeraRaidContentType)value;

    public string ContentName => Content switch
    {
        0 => "Base (5★)",
        1 => "Black (6★)",
        2 => "Distribution",
        3 => "Mighty (7★)",
        _ => $"Unknown ({Content})"
    };
}

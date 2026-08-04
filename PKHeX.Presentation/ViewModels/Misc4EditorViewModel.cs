using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Misc editor for Gen 4 saves covering coins, BP, fly destinations,
/// Walker (HGSS), Pokéathlon (HGSS), and Battle Frontier prints.
/// </summary>
public partial class Misc4EditorViewModel : ViewModelBase
{
    private readonly SAV4 _sav;

    public Misc4EditorViewModel(SAV4 sav)
    {
        _sav = sav;

        LoadMain();
        LoadFlyDestinations();
        LoadWalker();
        LoadBattleFrontier();
        LoadPokeRadar();
    }

    #region Main Properties

    [ObservableProperty]
    private uint _coins;

    [ObservableProperty]
    private ushort _bp;

    [ObservableProperty]
    private uint _undergroundFlags;

    [ObservableProperty]
    private uint _pokeathlonPoints;

    [ObservableProperty]
    private int _mapUnlockState;

    [ObservableProperty]
    private bool _pokeRadar;

    public bool IsSinnoh => _sav is SAV4Sinnoh;
    public bool IsHGSS => _sav is SAV4HGSS;
    public bool IsWalkerVisible => IsHGSS;
    public bool IsBattleFrontierVisible => _sav is not SAV4DP;
    public bool IsPokeRadarVisible => _sav is SAV4DP or SAV4Pt;

    public string[] MapStates { get; } = ["Map Johto", "Map Johto+", "Map Johto & Kanto"];

    #endregion

    #region Fly Destinations

    [ObservableProperty]
    private ObservableCollection<FlyDestinationViewModel> _flyDestinations = [];

    private static ReadOnlySpan<byte> FlyWorkFlagSinnoh => [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 67, 68];
    private static ReadOnlySpan<byte> LocationIDsSinnoh => [1, 2, 3, 4, 5, 82, 83, 6, 7, 8, 9, 10, 11, 12, 13, 14, 54, 81, 55, 15];
    private static ReadOnlySpan<byte> FlyWorkFlagHGSS => [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 27, 30, 33, 35];
    private static ReadOnlySpan<byte> LocationIDsHGSS => [138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 229, 227, 221, 225];

    private const int FlyFlagStart = 2480;

    private void LoadFlyDestinations()
    {
        var locations = IsSinnoh ? LocationIDsSinnoh : LocationIDsHGSS;
        var flags = IsSinnoh ? FlyWorkFlagSinnoh : FlyWorkFlagHGSS;
        var metStrings = GameInfo.Strings.Gen4.Met0;

        FlyDestinations.Clear();
        for (int i = 0; i < locations.Length; i++)
        {
            var flagIndex = FlyFlagStart + flags[i];
            var state = _sav.GetEventFlag(flagIndex);
            var locationID = locations[i];
            var name = metStrings[locationID];
            FlyDestinations.Add(new FlyDestinationViewModel(flagIndex, name, state));
        }
    }

    private void SaveFlyDestinations()
    {
        foreach (var dest in FlyDestinations)
            _sav.SetEventFlag(dest.FlagIndex, dest.IsUnlocked);
    }

    [RelayCommand]
    private void UnlockAllFlyDestinations()
    {
        foreach (var dest in FlyDestinations)
            dest.IsUnlocked = true;
    }

    #endregion

    #region Walker (HGSS only)

    [ObservableProperty]
    private ObservableCollection<WalkerCourseViewModel> _walkerCourses = [];

    [ObservableProperty]
    private uint _walkerWatts;

    [ObservableProperty]
    private uint _walkerSteps;

    private void LoadWalker()
    {
        if (_sav is not SAV4HGSS hgss) return;

        var courseNames = GameInfo.Sources.Strings.walkercourses;
        Span<bool> courses = stackalloc bool[SAV4HGSS.PokewalkerCourseFlagCount];
        hgss.GetPokewalkerCoursesUnlocked(courses);

        WalkerCourses.Clear();
        for (int i = 0; i < courseNames.Length && i < SAV4HGSS.PokewalkerCourseFlagCount; i++)
            WalkerCourses.Add(new WalkerCourseViewModel(i, courseNames[i], courses[i]));

        WalkerWatts = hgss.PokewalkerWatts;
        WalkerSteps = hgss.PokewalkerSteps;
    }

    private void SaveWalker()
    {
        if (_sav is not SAV4HGSS hgss) return;

        Span<bool> courses = stackalloc bool[SAV4HGSS.PokewalkerCourseFlagCount];
        for (int i = 0; i < WalkerCourses.Count && i < courses.Length; i++)
            courses[i] = WalkerCourses[i].IsUnlocked;
        hgss.SetPokewalkerCoursesUnlocked(courses);

        hgss.PokewalkerWatts = WalkerWatts;
        hgss.PokewalkerSteps = WalkerSteps;
    }

    [RelayCommand]
    private void UnlockAllWalkerCourses()
    {
        if (_sav is not SAV4HGSS hgss) return;
        hgss.PokewalkerCoursesUnlockAll();
        
        Span<bool> courses = stackalloc bool[SAV4HGSS.PokewalkerCourseFlagCount];
        hgss.GetPokewalkerCoursesUnlocked(courses);
        for (int i = 0; i < WalkerCourses.Count && i < courses.Length; i++)
            WalkerCourses[i].IsUnlocked = courses[i];
    }

    #endregion

    #region Battle Frontier (Pt/HGSS)

    [ObservableProperty]
    private ObservableCollection<FrontierPrintViewModel> _frontierPrints = [];

    private int PrintIndexStart => _sav switch
    {
        SAV4Pt => 79,
        SAV4HGSS => 77,
        _ => 0
    };

    private void LoadBattleFrontier()
    {
        if (_sav is SAV4DP) return;

        string[] facilityNames = ["Tower", "Factory", "Hall", "Castle", "Arcade"];
        FrontierPrints.Clear();

        for (int i = 0; i < facilityNames.Length; i++)
        {
            var workIndex = PrintIndexStart + i;
            var value = _sav.GetWork(workIndex);
            var status = (BattleFrontierPrintStatus4)value;
            FrontierPrints.Add(new FrontierPrintViewModel(workIndex, facilityNames[i], status));
        }
    }

    private void SaveBattleFrontier()
    {
        if (_sav is SAV4DP) return;

        foreach (var print in FrontierPrints)
            _sav.SetWork(print.WorkIndex, (ushort)print.Status);
    }

    [RelayCommand]
    private void GiveAllPrints()
    {
        foreach (var print in FrontierPrints)
            print.Status = BattleFrontierPrintStatus4.SecondReceived;
    }

    [RelayCommand]
    private void ClearAllPrints()
    {
        foreach (var print in FrontierPrints)
            print.Status = BattleFrontierPrintStatus4.None;
    }

    #endregion

    #region Main Data

    private void LoadMain()
    {
        Coins = (uint)Math.Min(_sav.MaxCoins, _sav.Coin);
        Bp = Math.Min((ushort)9999, (ushort)_sav.BP);

        if (_sav is SAV4Sinnoh sinnoh)
            UndergroundFlags = sinnoh.UG_FlagsCaptured;

        if (_sav is SAV4HGSS hgss)
        {
            PokeathlonPoints = hgss.Pokeathlon.Points;
            var mapState = (int)hgss.MapUnlockState;
            MapUnlockState = mapState >= MapStates.Length ? MapStates.Length - 1 : mapState;
        }
    }

    private void SaveMain()
    {
        _sav.Coin = Coins;
        _sav.BP = Bp;

        if (_sav is SAV4Sinnoh sinnoh)
            sinnoh.UG_FlagsCaptured = UndergroundFlags;

        if (_sav is SAV4HGSS hgss)
        {
            hgss.Pokeathlon.Points = PokeathlonPoints;
            hgss.MapUnlockState = (MapUnlockState4)MapUnlockState;
        }
    }

    private const int PokeRadarItem = 431;

    private void LoadPokeRadar()
    {
        if (!IsPokeRadarVisible) return;
        PokeRadar = _sav.Inventory.Pouches.First(p => p.Type is InventoryType.KeyItems).HasItem(PokeRadarItem);
    }

    private void SavePokeRadar()
    {
        if (!IsPokeRadarVisible) return;

        var bag = _sav.Inventory;
        var pouch = bag.Pouches.First(p => p.Type is InventoryType.KeyItems);
        var hasRadar = pouch.HasItem(PokeRadarItem);
        if (PokeRadar == hasRadar)
            return;

        if (PokeRadar)
        {
            var item = pouch.Items.FirstOrDefault(it => it.Index == 0);
            if (item == null)
            {
                // No empty slot to place the radar; revert the checkbox to the save's actual state.
                LoadPokeRadar();
                return;
            }
            item.Index = PokeRadarItem;
            item.Count = 1;
        }
        else
        {
            var item = pouch.Items.FirstOrDefault(it => it.Index == PokeRadarItem);
            if (item != null)
            {
                item.Index = 0;
                item.Count = 0;

                // The Gen 4 bag pouch is a null-terminated array: leaving a zeroed slot in the
                // middle would strand any items after it behind the hole, making them invisible
                // in-game. Compact so all remaining non-empty entries stay contiguous from the
                // start, preserving their relative order.
                pouch.ClearCount0();
            }
        }

        bag.CopyTo(_sav);
    }

    #endregion

    #region Commands

    [RelayCommand]
    private void Save()
    {
        SaveMain();
        SaveFlyDestinations();
        SaveWalker();
        SaveBattleFrontier();
        SavePokeRadar();
    }

    #endregion
}

/// <summary>
/// ViewModel for a fly destination.
/// </summary>
public partial class FlyDestinationViewModel : ObservableObject
{
    public FlyDestinationViewModel(int flagIndex, string name, bool isUnlocked)
    {
        FlagIndex = flagIndex;
        Name = name;
        _isUnlocked = isUnlocked;
    }

    public int FlagIndex { get; }
    public string Name { get; }

    [ObservableProperty]
    private bool _isUnlocked;
}

/// <summary>
/// ViewModel for a Pokéwalker course.
/// </summary>
public partial class WalkerCourseViewModel : ObservableObject
{
    public WalkerCourseViewModel(int index, string name, bool isUnlocked)
    {
        Index = index;
        Name = name;
        _isUnlocked = isUnlocked;
    }

    public int Index { get; }
    public string Name { get; }

    [ObservableProperty]
    private bool _isUnlocked;
}

/// <summary>
/// ViewModel for a Battle Frontier print.
/// </summary>
public partial class FrontierPrintViewModel : ObservableObject
{
    public FrontierPrintViewModel(int workIndex, string name, BattleFrontierPrintStatus4 status)
    {
        WorkIndex = workIndex;
        Name = name;
        _status = status;
    }

    public int WorkIndex { get; }
    public string Name { get; }

    [ObservableProperty]
    private BattleFrontierPrintStatus4 _status;

    public string StatusText => Status switch
    {
        BattleFrontierPrintStatus4.None => "None",
        BattleFrontierPrintStatus4.FirstReady => "Silver Ready",
        BattleFrontierPrintStatus4.FirstReceived => "Silver",
        BattleFrontierPrintStatus4.SecondReady => "Gold Ready",
        BattleFrontierPrintStatus4.SecondReceived => "Gold",
        _ => "Unknown"
    };

    [RelayCommand]
    private void CycleStatus()
    {
        Status = Status switch
        {
            BattleFrontierPrintStatus4.None => BattleFrontierPrintStatus4.FirstReceived,
            BattleFrontierPrintStatus4.FirstReady => BattleFrontierPrintStatus4.FirstReceived,
            BattleFrontierPrintStatus4.FirstReceived => BattleFrontierPrintStatus4.SecondReceived,
            BattleFrontierPrintStatus4.SecondReady => BattleFrontierPrintStatus4.SecondReceived,
            BattleFrontierPrintStatus4.SecondReceived => BattleFrontierPrintStatus4.None,
            _ => BattleFrontierPrintStatus4.None
        };
    }
}

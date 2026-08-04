using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Misc editor for Gen 7 saves covering Battle Tree, Poké Finder, Stamps, and Fly destinations.
/// </summary>
public partial class Misc7EditorViewModel : ViewModelBase
{
    private readonly SAV7 _sav;

    public Misc7EditorViewModel(SAV7 sav)
    {
        _sav = sav;
        LoadBattleTree();
        LoadPokeFinder();
        LoadStamps();
        LoadFlyDestinations();
        if (sav is SAV7USUM)
            LoadUltraData();
    }

    public bool IsUSUM => _sav is SAV7USUM;
    public bool IsSM => _sav is SAV7SM;

    #region Battle Tree

    // Regular mode (current/max)
    [ObservableProperty] private int _singleCurrentStreak;
    [ObservableProperty] private int _singleMaxStreak;
    [ObservableProperty] private int _doubleCurrentStreak;
    [ObservableProperty] private int _doubleMaxStreak;
    [ObservableProperty] private int _multiCurrentStreak;
    [ObservableProperty] private int _multiMaxStreak;

    // Super mode (current/max)
    [ObservableProperty] private int _superSingleCurrentStreak;
    [ObservableProperty] private int _superSingleMaxStreak;
    [ObservableProperty] private int _superDoubleCurrentStreak;
    [ObservableProperty] private int _superDoubleMaxStreak;
    [ObservableProperty] private int _superMultiCurrentStreak;
    [ObservableProperty] private int _superMultiMaxStreak;

    // Unlock flags
    [ObservableProperty] private bool _superSingleUnlocked;
    [ObservableProperty] private bool _superDoubleUnlocked;
    [ObservableProperty] private bool _superMultiUnlocked;

    private void LoadBattleTree()
    {
        var bt = _sav.BattleTree;

        // Regular
        SingleCurrentStreak = bt.GetTreeStreak(0, super: false, max: false);
        SingleMaxStreak = bt.GetTreeStreak(0, super: false, max: true);
        DoubleCurrentStreak = bt.GetTreeStreak(1, super: false, max: false);
        DoubleMaxStreak = bt.GetTreeStreak(1, super: false, max: true);
        MultiCurrentStreak = bt.GetTreeStreak(2, super: false, max: false);
        MultiMaxStreak = bt.GetTreeStreak(2, super: false, max: true);

        // Super
        SuperSingleCurrentStreak = bt.GetTreeStreak(0, super: true, max: false);
        SuperSingleMaxStreak = bt.GetTreeStreak(0, super: true, max: true);
        SuperDoubleCurrentStreak = bt.GetTreeStreak(1, super: true, max: false);
        SuperDoubleMaxStreak = bt.GetTreeStreak(1, super: true, max: true);
        SuperMultiCurrentStreak = bt.GetTreeStreak(2, super: true, max: false);
        SuperMultiMaxStreak = bt.GetTreeStreak(2, super: true, max: true);

        // Unlock flags
        SuperSingleUnlocked = _sav.EventWork.GetEventFlag(333);
        SuperDoubleUnlocked = _sav.EventWork.GetEventFlag(334);
        SuperMultiUnlocked = _sav.EventWork.GetEventFlag(335);
    }

    private void SaveBattleTree()
    {
        var bt = _sav.BattleTree;

        // Regular
        bt.SetTreeStreak(SingleCurrentStreak, 0, super: false, max: false);
        bt.SetTreeStreak(SingleMaxStreak, 0, super: false, max: true);
        bt.SetTreeStreak(DoubleCurrentStreak, 1, super: false, max: false);
        bt.SetTreeStreak(DoubleMaxStreak, 1, super: false, max: true);
        bt.SetTreeStreak(MultiCurrentStreak, 2, super: false, max: false);
        bt.SetTreeStreak(MultiMaxStreak, 2, super: false, max: true);

        // Super
        bt.SetTreeStreak(SuperSingleCurrentStreak, 0, super: true, max: false);
        bt.SetTreeStreak(SuperSingleMaxStreak, 0, super: true, max: true);
        bt.SetTreeStreak(SuperDoubleCurrentStreak, 1, super: true, max: false);
        bt.SetTreeStreak(SuperDoubleMaxStreak, 1, super: true, max: true);
        bt.SetTreeStreak(SuperMultiCurrentStreak, 2, super: true, max: false);
        bt.SetTreeStreak(SuperMultiMaxStreak, 2, super: true, max: true);

        // Unlock flags
        _sav.EventWork.SetEventFlag(333, SuperSingleUnlocked);
        _sav.EventWork.SetEventFlag(334, SuperDoubleUnlocked);
        _sav.EventWork.SetEventFlag(335, SuperMultiUnlocked);
    }

    [RelayCommand]
    private void UnlockAllBattleTreeModes()
    {
        SuperSingleUnlocked = true;
        SuperDoubleUnlocked = true;
        SuperMultiUnlocked = true;
    }

    #endregion

    #region Poké Finder

    [ObservableProperty] private uint _snapCount;
    [ObservableProperty] private uint _thumbsTotal;
    [ObservableProperty] private uint _thumbsRecord;
    [ObservableProperty] private int _cameraVersion;
    [ObservableProperty] private bool _gyroEnabled;

    public string[] CameraVersions { get; } = ["Pokemon Finder", "Pokemon Finder 2.0", "Pokemon Finder 2.1 (Max)"];

    private void LoadPokeFinder()
    {
        SnapCount = _sav.PokeFinder.SnapCount;
        ThumbsTotal = _sav.PokeFinder.ThumbsTotalValue;
        ThumbsRecord = _sav.PokeFinder.ThumbsHighValue;
        CameraVersion = Math.Min((int)_sav.PokeFinder.CameraVersion, 2);
        GyroEnabled = _sav.PokeFinder.GyroFlag;
    }

    private void SavePokeFinder()
    {
        _sav.PokeFinder.SnapCount = SnapCount;
        _sav.PokeFinder.ThumbsTotalValue = ThumbsTotal;
        _sav.PokeFinder.ThumbsHighValue = ThumbsRecord;
        _sav.PokeFinder.CameraVersion = (ushort)CameraVersion;
        _sav.PokeFinder.GyroFlag = GyroEnabled;
    }

    [RelayCommand]
    private void MaxPokeFinder()
    {
        SnapCount = 999999;
        ThumbsTotal = 9999999;
        ThumbsRecord = 9999999;
        CameraVersion = 2;
    }

    #endregion

    #region Stamps

    [ObservableProperty]
    private ObservableCollection<StampViewModel> _stamps = [];

    private void LoadStamps()
    {
        var stampNames = Enum.GetNames<Stamp7>();
        uint stampBits = _sav.Misc.Stamps;

        Stamps.Clear();
        for (int i = 0; i < stampNames.Length; i++)
        {
            bool obtained = (stampBits & (1u << i)) != 0;
            Stamps.Add(new StampViewModel(i, stampNames[i], obtained));
        }
    }

    private void SaveStamps()
    {
        uint bits = 0;
        for (int i = 0; i < Stamps.Count; i++)
        {
            if (Stamps[i].IsObtained)
                bits |= 1u << i;
        }
        _sav.Misc.Stamps = bits;
    }

    [RelayCommand]
    private void UnlockAllStamps()
    {
        foreach (var stamp in Stamps)
            stamp.IsObtained = true;
    }

    #endregion

    #region Fly Destinations

    [ObservableProperty]
    private ObservableCollection<FlyDestination7ViewModel> _flyDestinations = [];

    private int _skipFlag;
    private int[] _flyDestFlagOfs = [];

    private void LoadFlyDestinations()
    {
        _skipFlag = _sav is SAV7USUM ? 4160 : 3200;

        _flyDestFlagOfs = [
            44, 43, 45, 40, 41, 49, 42, 47, 46, 48,
            50, 54, 39, 57, 51, 55, 59, 52, 58, 53, 61, 60, 56,
            62, 66, 67, 64, 65, 273, 270, 37, 38,
            69, 74, 72, 71, 276, 73, 70,
            75, 332, 334,
            331, 333, 335, 336
        ];

        string[] flyDestNames = [
            "My House", "Route 1", "Hau'oli Outskirts", "Iki Town", "Route 2",
            "Hau'oli City", "Route 3", "Heahea City", "Route 4", "Paniola Town",
            "Route 5", "Royal Avenue", "Hauoli Cemetery", "Route 6", "Konikoni City",
            "Diglett's Tunnel", "Memorial Hill", "Route 7", "Wela Volcano Park", "Route 8",
            "Lush Jungle", "Route 9", "Aether Base",
            "Malie City", "Route 10", "Mount Hokulani", "Route 11", "Route 12",
            "Secluded Shore (USUM)", "Tapu Village", "Route 13", "Route 14",
            "Seafolk Village", "Exeggutor Island", "Vast Poni Canyon", "Altar of the Sunne/Moone",
            "Poni Grove (USUM)", "Ancient Poni Path", "Poni Wilds",
            "Battle Tree", "Photo Club (Hauoli)", "Photo Club (Konikoni)",
            "Battle Agency (USUM)", "Big Wave Beach (USUM)", "Sandy Cave (USUM)", "Poni Beach (USUM)"
        ];

        int count = _sav is SAV7USUM ? flyDestNames.Length : flyDestNames.Length - 6;

        FlyDestinations.Clear();
        for (int i = 0; i < count; i++)
        {
            bool unlocked = _sav.EventWork.GetEventFlag(_skipFlag + _flyDestFlagOfs[i]);
            FlyDestinations.Add(new FlyDestination7ViewModel(i, flyDestNames[i], unlocked));
        }
    }

    private void SaveFlyDestinations()
    {
        for (int i = 0; i < FlyDestinations.Count && i < _flyDestFlagOfs.Length; i++)
        {
            _sav.EventWork.SetEventFlag(_skipFlag + _flyDestFlagOfs[i], FlyDestinations[i].IsUnlocked);
        }
    }

    [RelayCommand]
    private void UnlockAllFlyDestinations()
    {
        foreach (var dest in FlyDestinations)
            dest.IsUnlocked = true;
    }

    #endregion

    #region Ultra Data (USUM only)

    [ObservableProperty] private int _mantineSurf0;
    [ObservableProperty] private int _mantineSurf1;
    [ObservableProperty] private int _mantineSurf2;
    [ObservableProperty] private int _mantineSurf3;

    private void LoadUltraData()
    {
        MantineSurf0 = _sav.Misc.GetSurfScore(0);
        MantineSurf1 = _sav.Misc.GetSurfScore(1);
        MantineSurf2 = _sav.Misc.GetSurfScore(2);
        MantineSurf3 = _sav.Misc.GetSurfScore(3);
    }

    private void SaveUltraData()
    {
        if (_sav is not SAV7USUM) return;

        _sav.Misc.SetSurfScore(0, MantineSurf0);
        _sav.Misc.SetSurfScore(1, MantineSurf1);
        _sav.Misc.SetSurfScore(2, MantineSurf2);
        _sav.Misc.SetSurfScore(3, MantineSurf3);
    }

    #endregion

    #region Commands

    [RelayCommand]
    private void Save()
    {
        SaveBattleTree();
        SavePokeFinder();
        SaveStamps();
        SaveFlyDestinations();
        SaveUltraData();
    }

    #endregion
}

public partial class StampViewModel : ObservableObject
{
    public StampViewModel(int index, string name, bool isObtained)
    {
        Index = index;
        Name = name;
        _isObtained = isObtained;
    }

    public int Index { get; }
    public string Name { get; }

    [ObservableProperty]
    private bool _isObtained;
}

public partial class FlyDestination7ViewModel : ObservableObject
{
    public FlyDestination7ViewModel(int index, string name, bool isUnlocked)
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

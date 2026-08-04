using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Misc editor for Gen 3 saves covering records, coins, BP, joyful minigames,
/// Battle Frontier (Emerald), and ferry features (Emerald).
/// </summary>
public partial class Misc3EditorViewModel : ViewModelBase
{
    private readonly SAV3 _sav;

    public Misc3EditorViewModel(SAV3 sav)
    {
        _sav = sav;

        LoadRecords();
        LoadMain();
        LoadJoyful();
        LoadFerry();
        LoadBattleFrontier();
    }

    #region Main Properties

    [ObservableProperty]
    private ushort _coins;

    [ObservableProperty]
    private uint _bp;

    public bool IsBpVisible => _sav is SAV3E;
    public bool IsFrlg => _sav is SAV3FRLG;

    [ObservableProperty]
    private string _rivalName = string.Empty;

    #endregion

    #region Other (R/S/E only)

    public bool CanForceMirageIsland => _sav.SmallBlock is ISaveBlock3SmallHoenn;

    [RelayCommand]
    private void ForceMirageIsland()
    {
        if (_sav.SmallBlock is not ISaveBlock3SmallHoenn) return;
        var party1 = _sav.LargeBlock.PartyBuffer;
        if (party1.Length < 2) return;
        var pidLow = System.Buffers.Binary.BinaryPrimitives.ReadUInt16LittleEndian(party1);
        _sav.SetWork(0x24, pidLow); // mirage island rand value is work 0x24; matching party slot-0 PID-low triggers the island
    }

    #endregion

    #region Records

    [ObservableProperty]
    private ObservableCollection<ComboItem> _recordList = [];

    [ObservableProperty]
    private ComboItem? _selectedRecord;

    [ObservableProperty]
    private uint _recordValue;

    private void LoadRecords()
    {
        var items = Record3.GetItems(_sav);
        RecordList.Clear();
        foreach (var item in items)
            RecordList.Add(item);

        if (RecordList.Count > 0)
            SelectedRecord = RecordList[0];
    }

    partial void OnSelectedRecordChanged(ComboItem? value)
    {
        if (value is null) return;
        RecordValue = _sav.GetRecord(value.Value);
    }

    partial void OnRecordValueChanged(uint value)
    {
        if (SelectedRecord is null) return;
        _sav.SetRecord(SelectedRecord.Value, value);
    }

    #endregion

    #region Joyful Minigames

    private static ISaveBlock3SmallExpansion? GetExpansionSmall(SaveFile sav) => sav switch
    {
        SAV3E e => e.SmallBlock,
        SAV3FRLG frlg => frlg.SmallBlock,
        _ => null,
    };

    public bool IsJoyfulVisible => GetExpansionSmall(_sav) is not null;

    [ObservableProperty]
    private ushort _joyfulJumpInRow;

    [ObservableProperty]
    private ushort _joyfulJumpScore;

    [ObservableProperty]
    private ushort _joyfulJump5InRow;

    [ObservableProperty]
    private ushort _joyfulJumpMaxPlayers;

    [ObservableProperty]
    private ushort _joyfulBerriesInRow;

    [ObservableProperty]
    private ushort _joyfulBerriesScore;

    [ObservableProperty]
    private ushort _joyfulBerries5InRow;

    [ObservableProperty]
    private uint _berryPowder;

    private void LoadJoyful()
    {
        if (GetExpansionSmall(_sav) is not { } j) return;

        JoyfulJumpInRow = Math.Min((ushort)9999, j.JoyfulJumpInRow);
        JoyfulJumpScore = Math.Min((ushort)9999, (ushort)j.JoyfulJumpScore);
        JoyfulJump5InRow = Math.Min((ushort)9999, j.JoyfulJump5InRow);
        JoyfulJumpMaxPlayers = Math.Min((ushort)9999, j.JoyfulJumpGamesMaxPlayers);
        JoyfulBerriesInRow = Math.Min((ushort)9999, j.JoyfulBerriesInRow);
        JoyfulBerriesScore = Math.Min((ushort)9999, (ushort)j.JoyfulBerriesScore);
        JoyfulBerries5InRow = Math.Min((ushort)9999, j.JoyfulBerries5InRow);
        BerryPowder = Math.Min(99999u, j.BerryPowder);
    }

    private void SaveJoyful()
    {
        if (GetExpansionSmall(_sav) is not { } j) return;

        j.JoyfulJumpInRow = JoyfulJumpInRow;
        j.JoyfulJumpScore = JoyfulJumpScore;
        j.JoyfulJump5InRow = JoyfulJump5InRow;
        j.JoyfulJumpGamesMaxPlayers = JoyfulJumpMaxPlayers;
        j.JoyfulBerriesInRow = JoyfulBerriesInRow;
        j.JoyfulBerriesScore = JoyfulBerriesScore;
        j.JoyfulBerries5InRow = JoyfulBerries5InRow;
        j.BerryPowder = BerryPowder;
    }

    #endregion

    #region Ferry (Emerald only)

    public bool IsFerryVisible => _sav is SAV3E;

    // Catchable flags
    [ObservableProperty]
    private bool _mewCatchable;

    // Reach flags  
    [ObservableProperty]
    private bool _reachSouthernIsland;

    [ObservableProperty]
    private bool _reachBirthIsland;

    [ObservableProperty]
    private bool _reachFarawayIsland;

    [ObservableProperty]
    private bool _reachNavelRock;

    [ObservableProperty]
    private bool _reachBattleFrontier;

    // Initial visit flags
    [ObservableProperty]
    private bool _initialSouthernIsland;

    [ObservableProperty]
    private bool _initialBirthIsland;

    [ObservableProperty]
    private bool _initialFarawayIsland;

    [ObservableProperty]
    private bool _initialNavelRock;

    private void LoadFerry()
    {
        if (_sav is not SAV3E) return;

        MewCatchable = _sav.GetEventFlag(0x864);
        ReachSouthernIsland = _sav.GetEventFlag(0x8B3);
        ReachBirthIsland = _sav.GetEventFlag(0x8D5);
        ReachFarawayIsland = _sav.GetEventFlag(0x8D6);
        ReachNavelRock = _sav.GetEventFlag(0x8E0);
        ReachBattleFrontier = _sav.GetEventFlag(0x1D0);
        InitialSouthernIsland = _sav.GetEventFlag(0x1AE);
        InitialBirthIsland = _sav.GetEventFlag(0x1AF);
        InitialFarawayIsland = _sav.GetEventFlag(0x1B0);
        InitialNavelRock = _sav.GetEventFlag(0x1DB);
    }

    private void SaveFerry()
    {
        if (_sav is not SAV3E) return;

        _sav.SetEventFlag(0x864, MewCatchable);
        _sav.SetEventFlag(0x8B3, ReachSouthernIsland);
        _sav.SetEventFlag(0x8D5, ReachBirthIsland);
        _sav.SetEventFlag(0x8D6, ReachFarawayIsland);
        _sav.SetEventFlag(0x8E0, ReachNavelRock);
        _sav.SetEventFlag(0x1D0, ReachBattleFrontier);
        _sav.SetEventFlag(0x1AE, InitialSouthernIsland);
        _sav.SetEventFlag(0x1AF, InitialBirthIsland);
        _sav.SetEventFlag(0x1B0, InitialFarawayIsland);
        _sav.SetEventFlag(0x1DB, InitialNavelRock);
    }

    [RelayCommand]
    private void UnlockAllFerryDestinations()
    {
        ReachSouthernIsland = true;
        ReachBirthIsland = true;
        ReachFarawayIsland = true;
        ReachNavelRock = true;
        ReachBattleFrontier = true;
        InitialSouthernIsland = true;
        InitialBirthIsland = true;
        InitialFarawayIsland = true;
        InitialNavelRock = true;
    }

    #endregion

    #region Battle Frontier (Emerald only)

    public bool IsBattleFrontierVisible => _sav is SAV3E;

    [ObservableProperty]
    private bool _frontierPassActivated;

    // Symbol statuses: 0 = None, 1 = Silver, 2 = Gold
    [ObservableProperty]
    private ObservableCollection<FrontierSymbolViewModel> _frontierSymbols = [];

    private void LoadBattleFrontier()
    {
        if (_sav is not SAV3E) return;

        FrontierPassActivated = _sav.GetEventFlag(BattleFrontier3.FrontierPassFlagIndex);

        FrontierSymbols.Clear();
        foreach (BattleFrontierFacility3 facility in Enum.GetValues<BattleFrontierFacility3>())
        {
            var silver = _sav.GetEventFlag(BattleFrontier3.GetSymbolSilverFlagIndex(facility));
            var gold = _sav.GetEventFlag(BattleFrontier3.GetSymbolGoldFlagIndex(facility));
            var status = silver ? (gold ? 2 : 1) : 0;
            FrontierSymbols.Add(new FrontierSymbolViewModel(facility, status));
        }
    }

    private void SaveBattleFrontier()
    {
        if (_sav is not SAV3E) return;

        _sav.SetEventFlag(BattleFrontier3.FrontierPassFlagIndex, FrontierPassActivated);

        foreach (var symbol in FrontierSymbols)
        {
            var silver = symbol.Status >= 1;
            var gold = symbol.Status >= 2;
            _sav.SetEventFlag(BattleFrontier3.GetSymbolSilverFlagIndex(symbol.Facility), silver);
            _sav.SetEventFlag(BattleFrontier3.GetSymbolGoldFlagIndex(symbol.Facility), gold);
        }
    }

    [RelayCommand]
    private void GiveAllSymbols()
    {
        foreach (var symbol in FrontierSymbols)
            symbol.Status = 2; // Gold
    }

    [RelayCommand]
    private void ClearAllSymbols()
    {
        foreach (var symbol in FrontierSymbols)
            symbol.Status = 0; // None
    }

    #endregion

    #region Main Data

    private void LoadMain()
    {
        Coins = Math.Min((ushort)9999, (ushort)_sav.Coin);

        if (_sav is SAV3E e)
            Bp = Math.Min(9999u, e.SmallBlock.BP);

        if (_sav is SAV3FRLG frlg)
            RivalName = frlg.RivalName;
    }

    private void SaveMain()
    {
        _sav.Coin = Coins;

        if (_sav is SAV3E e)
            e.SmallBlock.BP = (ushort)Bp;

        if (_sav is SAV3FRLG frlg)
            frlg.RivalName = RivalName;
    }

    #endregion

    #region Commands

    [RelayCommand]
    private void Save()
    {
        SaveMain();
        SaveJoyful();
        SaveFerry();
        SaveBattleFrontier();
        // Changes are applied directly to _sav, no copy needed
    }

    #endregion
}

/// <summary>
/// ViewModel for a Battle Frontier symbol.
/// </summary>
public partial class FrontierSymbolViewModel : ObservableObject
{
    public FrontierSymbolViewModel(BattleFrontierFacility3 facility, int status)
    {
        Facility = facility;
        _status = status;
    }

    public BattleFrontierFacility3 Facility { get; }
    public string Name => Facility.ToString();

    [ObservableProperty]
    private int _status; // 0 = None, 1 = Silver, 2 = Gold

    public string StatusText => Status switch
    {
        0 => "None",
        1 => "Silver",
        2 => "Gold",
        _ => "Unknown"
    };

    [RelayCommand]
    private void CycleStatus()
    {
        Status = (Status + 1) % 3;
    }
}

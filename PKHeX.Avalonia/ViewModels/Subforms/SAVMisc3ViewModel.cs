using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the Gen 3 Misc editor.
/// Covers Records, Coins, Joyful minigames, Ferry tickets, and FRLG Rival name.
/// </summary>
public partial class SAVMisc3ViewModel : SaveEditorViewModelBase
{
    private readonly SAV3 _sav;

    // Records
    public IList<ComboItem> RecordItems { get; }

    [ObservableProperty]
    private int _selectedRecordIndex;

    [ObservableProperty]
    private uint _recordValue;

    [ObservableProperty]
    private ushort _fameHours;

    [ObservableProperty]
    private byte _fameMinutes;

    [ObservableProperty]
    private byte _fameSeconds;

    [ObservableProperty]
    private bool _showFameTime;

    // General
    [ObservableProperty]
    private ushort _coins;

    // BP (Emerald only)
    [ObservableProperty]
    private bool _showBp;

    [ObservableProperty]
    private ushort _bp;

    [ObservableProperty]
    private ushort _bpEarned;

    // Joyful (Emerald + FRLG)
    [ObservableProperty]
    private bool _showJoyful;

    [ObservableProperty]
    private ushort _joyfulJumpInRow;

    [ObservableProperty]
    private uint _joyfulJumpScore;

    [ObservableProperty]
    private ushort _joyfulJump5InRow;

    [ObservableProperty]
    private ushort _joyfulJumpMaxPlayers;

    [ObservableProperty]
    private ushort _joyfulBerriesInRow;

    [ObservableProperty]
    private uint _joyfulBerriesScore;

    [ObservableProperty]
    private ushort _joyfulBerries5InRow;

    [ObservableProperty]
    private uint _berryPowder;

    // Ferry (Emerald only)
    [ObservableProperty]
    private bool _showFerry;

    [ObservableProperty]
    private bool _catchable;

    [ObservableProperty]
    private bool _reachSouthern;

    [ObservableProperty]
    private bool _reachBirth;

    [ObservableProperty]
    private bool _reachFaraway;

    [ObservableProperty]
    private bool _reachNavel;

    [ObservableProperty]
    private bool _reachBf;

    [ObservableProperty]
    private bool _initialSouthern;

    [ObservableProperty]
    private bool _initialBirth;

    [ObservableProperty]
    private bool _initialFaraway;

    [ObservableProperty]
    private bool _initialNavel;

    // FRLG
    [ObservableProperty]
    private bool _showFrlg;

    [ObservableProperty]
    private string _rivalName = string.Empty;

    public SAVMisc3ViewModel(SAV3 sav) : base(sav)
    {
        _sav = sav;

        // Records
        RecordItems = Record3.GetItems(sav);
        _coins = (ushort)Math.Min(9999, sav.Coin);

        // BP (Emerald)
        if (sav is SAV3E em)
        {
            _showBp = true;
            _bp = (ushort)Math.Min((int)9999, (int)em.SmallBlock.BP);
            _bpEarned = em.SmallBlock.BPEarned;
        }

        // Joyful
        if (sav.SmallBlock is ISaveBlock3SmallExpansion j)
        {
            _showJoyful = true;
            _joyfulJumpInRow = Math.Min((ushort)9999, j.JoyfulJumpInRow);
            _joyfulJumpScore = Math.Min(99990u, j.JoyfulJumpScore);
            _joyfulJump5InRow = Math.Min((ushort)9999, j.JoyfulJump5InRow);
            _joyfulJumpMaxPlayers = Math.Min((ushort)9999, j.JoyfulJumpGamesMaxPlayers);
            _joyfulBerriesInRow = Math.Min((ushort)9999, j.JoyfulBerriesInRow);
            _joyfulBerriesScore = Math.Min(99990u, j.JoyfulBerriesScore);
            _joyfulBerries5InRow = Math.Min((ushort)9999, j.JoyfulBerries5InRow);
            _berryPowder = Math.Min(99999u, j.BerryPowder);
        }

        // Ferry (Emerald)
        if (sav is SAV3E)
        {
            _showFerry = true;
            _catchable = sav.GetEventFlag(0x864);
            _reachSouthern = sav.GetEventFlag(0x8B3);
            _reachBirth = sav.GetEventFlag(0x8D5);
            _reachFaraway = sav.GetEventFlag(0x8D6);
            _reachNavel = sav.GetEventFlag(0x8E0);
            _reachBf = sav.GetEventFlag(0x1D0);
            _initialSouthern = sav.GetEventFlag(0x1AE);
            _initialBirth = sav.GetEventFlag(0x1AF);
            _initialFaraway = sav.GetEventFlag(0x1B0);
            _initialNavel = sav.GetEventFlag(0x1DB);
        }

        // FRLG
        if (sav is SAV3FRLG frlg)
        {
            _showFrlg = true;
            _rivalName = frlg.RivalName;
        }

        // Load first record
        if (RecordItems.Count > 0)
            LoadRecord(0);
    }

    partial void OnSelectedRecordIndexChanged(int value)
    {
        if (value >= 0 && value < RecordItems.Count)
            LoadRecord(value);
    }

    private void LoadRecord(int index)
    {
        var recordId = RecordItems[index].Value;
        RecordValue = _sav.GetRecord(recordId);
        ShowFameTime = recordId == 1;
        if (ShowFameTime)
            SetFameTime(RecordValue);
    }

    private void SetFameTime(uint val)
    {
        FameHours = (ushort)Math.Min((int)9999, (int)(val >> 16));
        FameMinutes = (byte)Math.Min((int)59, (int)(byte)(val >> 8));
        FameSeconds = (byte)Math.Min((int)59, (int)(byte)val);
    }

    private uint GetFameTime()
    {
        var hrs = Math.Min((uint)9999, FameHours);
        var min = Math.Min((uint)59, FameMinutes);
        var sec = Math.Min((uint)59, FameSeconds);
        return (hrs << 16) | (min << 8) | sec;
    }

    [RelayCommand]
    private void UpdateRecord()
    {
        if (SelectedRecordIndex < 0 || SelectedRecordIndex >= RecordItems.Count)
            return;
        var recordId = RecordItems[SelectedRecordIndex].Value;
        if (ShowFameTime)
            RecordValue = GetFameTime();
        _sav.SetRecord(recordId, RecordValue);
    }

    [RelayCommand]
    private void Save()
    {
        // Save current record
        UpdateRecord();

        // Coins
        _sav.Coin = Coins;

        // BP (Emerald)
        if (_sav is SAV3E se)
        {
            se.SmallBlock.BP = Bp;
            se.SmallBlock.BPEarned = BpEarned;
        }

        // Joyful
        if (_sav.SmallBlock is ISaveBlock3SmallExpansion j)
        {
            j.JoyfulJumpInRow = JoyfulJumpInRow;
            j.JoyfulJumpScore = JoyfulJumpScore;
            j.JoyfulJump5InRow = JoyfulJump5InRow;
            j.JoyfulJumpGamesMaxPlayers = JoyfulJumpMaxPlayers;
            j.JoyfulBerriesInRow = JoyfulBerriesInRow;
            j.JoyfulBerriesScore = JoyfulBerriesScore;
            j.JoyfulBerries5InRow = JoyfulBerries5InRow;
            j.BerryPowder = BerryPowder;
        }

        // Ferry (Emerald)
        if (_sav is SAV3E)
        {
            _sav.SetEventFlag(0x864, Catchable);
            _sav.SetEventFlag(0x8B3, ReachSouthern);
            _sav.SetEventFlag(0x8D5, ReachBirth);
            _sav.SetEventFlag(0x8D6, ReachFaraway);
            _sav.SetEventFlag(0x8E0, ReachNavel);
            _sav.SetEventFlag(0x1D0, ReachBf);
            _sav.SetEventFlag(0x1AE, InitialSouthern);
            _sav.SetEventFlag(0x1AF, InitialBirth);
            _sav.SetEventFlag(0x1B0, InitialFaraway);
            _sav.SetEventFlag(0x1DB, InitialNavel);
        }

        // FRLG
        if (_sav is SAV3FRLG frlg)
            frlg.RivalName = RivalName;

        Modified = true;
    }
}

using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a fly destination entry (Gen 5).
/// </summary>
public partial class FlyDest5Model : ObservableObject
{
    public int FlagBit { get; }
    public string Name { get; }

    [ObservableProperty]
    private bool _unlocked;

    public FlyDest5Model(int flagBit, string name, bool unlocked)
    {
        FlagBit = flagBit;
        Name = name;
        _unlocked = unlocked;
    }
}

/// <summary>
/// Model for a Key System entry (B2W2).
/// </summary>
public partial class KeyEntry5Model : ObservableObject
{
    public int Index { get; }
    public string Label { get; }
    public bool IsObtain { get; }

    [ObservableProperty]
    private bool _enabled;

    public KeyEntry5Model(int index, string label, bool isObtain, bool enabled)
    {
        Index = index;
        Label = label;
        IsObtain = isObtain;
        _enabled = enabled;
    }
}

/// <summary>
/// Model for a Musical prop entry.
/// </summary>
public partial class PropModel : ObservableObject
{
    public int Index { get; }
    public string Name { get; }

    [ObservableProperty]
    private bool _obtained;

    public PropModel(int index, string name, bool obtained)
    {
        Index = index;
        Name = name;
        _obtained = obtained;
    }
}

/// <summary>
/// ViewModel for the Gen 5 Misc editor.
/// Covers Fly Destinations, Roamer (BW), Key System (B2W2), Musical Props, Records, and Battle Subway.
/// </summary>
public partial class Misc5ViewModel : SaveEditorViewModelBase
{
    private readonly SAV5 SAV5;
    private readonly BattleSubway5 Subway;
    private readonly BattleSubwayPlay5 SubwayPlay;
    private readonly Record5 Record;

    // Fly
    public ObservableCollection<FlyDest5Model> FlyDestinations { get; } = [];
    private int _flyOffset;
    private int[] _flyDestC = [];

    // Roamer (BW only)
    public bool ShowRoamer { get; }

    [ObservableProperty] private int _roamer641State;
    [ObservableProperty] private int _roamer642State;
    [ObservableProperty] private int _roamStatus;
    [ObservableProperty] private bool _libertyPass;

    public string[] RoamerStates { get; } = ["Not roamed", "Roaming", "Defeated", "Captured"];
    public string[] RoamStatusStates { get; } = ["Not happened", "Go to route 7", "Unknown (2)", "Event finished"];

    // Key System (B2W2 only)
    public bool ShowKeySystem { get; }
    public ObservableCollection<KeyEntry5Model> KeyEntries { get; } = [];

    // Musical Props
    public ObservableCollection<PropModel> Props { get; } = [];

    // Battle Subway
    [ObservableProperty] private int _subwayCurrentType;
    [ObservableProperty] private int _subwayCurrentBattle;

    // Subway flags
    [ObservableProperty] private bool _subwayFlag0;
    [ObservableProperty] private bool _subwayFlag1;
    [ObservableProperty] private bool _subwayFlag2;
    [ObservableProperty] private bool _subwayFlag3;
    [ObservableProperty] private bool _superSingle;
    [ObservableProperty] private bool _superDouble;
    [ObservableProperty] private bool _superMulti;
    [ObservableProperty] private bool _subwayFlag7;
    [ObservableProperty] private bool _npcMet;

    // Subway records
    [ObservableProperty] private int _singlePast;
    [ObservableProperty] private int _singleRecord;
    [ObservableProperty] private int _doublePast;
    [ObservableProperty] private int _doubleRecord;
    [ObservableProperty] private int _multiNpcPast;
    [ObservableProperty] private int _multiNpcRecord;
    [ObservableProperty] private int _multiFriendsPast;
    [ObservableProperty] private int _multiFriendsRecord;
    [ObservableProperty] private int _superSinglePast;
    [ObservableProperty] private int _superSingleRecord;
    [ObservableProperty] private int _superDoublePast;
    [ObservableProperty] private int _superDoubleRecord;
    [ObservableProperty] private int _superMultiNpcPast;
    [ObservableProperty] private int _superMultiNpcRecord;
    [ObservableProperty] private int _superMultiFriendsPast;
    [ObservableProperty] private int _superMultiFriendsRecord;

    // Records
    [ObservableProperty] private int _record16Index;
    [ObservableProperty] private int _record16Value;
    [ObservableProperty] private int _record32Index;
    [ObservableProperty] private uint _record32Value;

    public Misc5ViewModel(SAV5 sav) : base(sav)
    {
        SAV5 = sav;
        Subway = sav.BattleSubway;
        SubwayPlay = sav.BattleSubwayPlay;
        Record = sav.Records;

        ReadFly();
        ReadRoamer();
        ReadKeySystem();
        ReadMusical();
        ReadSubway();
        ReadRecords();

        ShowRoamer = sav is SAV5BW;
        ShowKeySystem = sav is SAV5B2W2;
    }

    private void ReadFly()
    {
        string[] flyNames;
        switch (SAV5.Version)
        {
            case GameVersion.B or GameVersion.W or GameVersion.BW:
                _flyOffset = 0x204B2;
                flyNames = [
                    "Nuvema Town", "Accumula Town", "Striaton City", "Nacrene City",
                    "Castelia City", "Nimbasa City", "Driftveil City", "Mistralton City",
                    "Icirrus City", "Opelucid City", "Victory Road", "Pokemon League",
                    "Lacunosa Town", "Undella Town", "Black City/White Forest", "(Unity Tower)",
                ];
                _flyDestC = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 15, 11, 10, 13, 12, 14];
                break;
            case GameVersion.B2 or GameVersion.W2 or GameVersion.B2W2:
                _flyOffset = 0x20392;
                flyNames = [
                    "Aspertia City", "Floccesy Town", "Virbank City",
                    "Nuvema Town", "Accumula Town", "Striaton City", "Nacrene City",
                    "Castelia City", "Nimbasa City", "Driftveil City", "Mistralton City",
                    "Icirrus City", "Opelucid City",
                    "Lacunosa Town", "Undella Town", "Black City/White Forest",
                    "Lentimas Town", "Humilau City", "Victory Road", "Pokemon League",
                    "Pokestar Studios", "Join Avenue", "PWT", "(Unity Tower)",
                ];
                _flyDestC = [24, 27, 25, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 21, 20, 28, 26, 66, 19, 5, 6, 7, 22];
                break;
            default:
                flyNames = [];
                break;
        }

        uint valFly = ReadUInt32LittleEndian(SAV5.Data[_flyOffset..]);
        for (int i = 0; i < flyNames.Length; i++)
        {
            bool isSet;
            if (_flyDestC[i] < 32)
                isSet = (valFly & (1u << _flyDestC[i])) != 0;
            else
                isSet = (SAV5.Data[_flyOffset + (_flyDestC[i] >> 3)] & (1 << (_flyDestC[i] & 7))) != 0;
            FlyDestinations.Add(new FlyDest5Model(_flyDestC[i], flyNames[i], isSet));
        }
    }

    private void ReadRoamer()
    {
        if (SAV5 is SAV5BW bw)
        {
            var encount = bw.Encount;
            _roamer641State = Math.Clamp((int)encount.GetRoamerState(1), (int)0, (int)3);
            _roamer642State = Math.Clamp((int)encount.GetRoamerState(0), (int)0, (int)3);
            _roamStatus = Math.Clamp((int)bw.EventWork.GetWorkRoamer(), (int)0, (int)3);
            _libertyPass = bw.Misc.IsLibertyTicketActivated;
        }
    }

    private void ReadKeySystem()
    {
        if (SAV5 is SAV5B2W2 b2w2)
        {
            var keys = b2w2.Keys;
            string[] keyNames = ["EasyKey", "ChallengeKey", "CityKey", "IronKey", "IcebergKey"];
            for (int i = 0; i < 5; i++)
            {
                KeyEntries.Add(new KeyEntry5Model(i, $"Obtain {keyNames[i]}", true, keys.GetIsKeyObtained((KeyType5)i)));
                KeyEntries.Add(new KeyEntry5Model(i, $"Unlock {keyNames[i]}", false, keys.GetIsKeyUnlocked((KeyType5)i)));
            }
        }
    }

    private void ReadMusical()
    {
        var musical = SAV5.Musical;
        // Musical has 100 props
        for (int i = 0; i < 100; i++)
        {
            var name = $"Prop {i:000}";
            Props.Add(new PropModel(i, name, musical.GetHasProp(i)));
        }
    }

    private void ReadSubway()
    {
        _subwayCurrentType = SubwayPlay.CurrentType;
        _subwayCurrentBattle = SubwayPlay.CurrentBattle;

        _subwayFlag0 = Subway.Flag0;
        _subwayFlag1 = Subway.Flag1;
        _subwayFlag2 = Subway.Flag2;
        _subwayFlag3 = Subway.Flag3;
        _superSingle = Subway.SuperSingle;
        _superDouble = Subway.SuperDouble;
        _superMulti = Subway.SuperMulti;
        _subwayFlag7 = Subway.Flag7;
        _npcMet = Subway.NPCMet;

        _singlePast = Subway.SinglePast;
        _singleRecord = Subway.SingleRecord;
        _doublePast = Subway.DoublePast;
        _doubleRecord = Subway.DoubleRecord;
        _multiNpcPast = Subway.MultiNPCPast;
        _multiNpcRecord = Subway.MultiNPCRecord;
        _multiFriendsPast = Subway.MultiFriendsPast;
        _multiFriendsRecord = Subway.MultiFriendsRecord;
        _superSinglePast = Subway.SuperSinglePast;
        _superSingleRecord = Subway.SuperSingleRecord;
        _superDoublePast = Subway.SuperDoublePast;
        _superDoubleRecord = Subway.SuperDoubleRecord;
        _superMultiNpcPast = Subway.SuperMultiNPCPast;
        _superMultiNpcRecord = Subway.SuperMultiNPCRecord;
        _superMultiFriendsPast = Subway.SuperMultiFriendsPast;
        _superMultiFriendsRecord = Subway.SuperMultiFriendsRecord;
    }

    private void ReadRecords()
    {
        _record16Value = Record.GetRecord16(0);
        _record32Value = Record.GetRecord32(0);
    }

    partial void OnRecord16IndexChanged(int value)
    {
        if (value >= 0 && value < Record5.Record16)
            Record16Value = Record.GetRecord16(value);
    }

    partial void OnRecord32IndexChanged(int value)
    {
        if (value >= 0 && value < Record5.Record32)
            Record32Value = Record.GetRecord32(value);
    }

    [RelayCommand]
    private void UnlockAllFlyDest()
    {
        foreach (var dest in FlyDestinations)
            dest.Unlocked = true;
    }

    [RelayCommand]
    private void UnlockAllKeys()
    {
        foreach (var entry in KeyEntries)
            entry.Enabled = true;
    }

    [RelayCommand]
    private void UnlockAllProps()
    {
        foreach (var prop in Props)
            prop.Obtained = true;
    }

    [RelayCommand]
    private void Save()
    {
        SaveFly();
        SaveRoamer();
        SaveKeySystem();
        SaveMusical();
        SaveSubway();
        SaveRecords();

        SAV.State.Edited = true;
        Modified = true;
    }

    private void SaveFly()
    {
        uint valFly = ReadUInt32LittleEndian(SAV5.Data[_flyOffset..]);
        for (int i = 0; i < FlyDestinations.Count; i++)
        {
            var dest = FlyDestinations[i];
            if (_flyDestC[i] < 32)
            {
                if (dest.Unlocked)
                    valFly |= 1u << _flyDestC[i];
                else
                    valFly &= ~(1u << _flyDestC[i]);
            }
            else
            {
                var ofs = _flyOffset + (_flyDestC[i] >> 3);
                SAV5.Data[ofs] = (byte)((SAV5.Data[ofs] & ~(1 << (_flyDestC[i] & 7))) | ((dest.Unlocked ? 1 : 0) << (_flyDestC[i] & 7)));
            }
        }
        WriteUInt32LittleEndian(SAV5.Data[_flyOffset..], valFly);
    }

    private void SaveRoamer()
    {
        if (SAV5 is SAV5BW bw)
        {
            var encount = bw.Encount;
            encount.SetRoamerState(1, (byte)Math.Clamp(Roamer641State, 0, 3));
            encount.SetRoamerState(0, (byte)Math.Clamp(Roamer642State, 0, 3));
            bw.EventWork.SetWorkRoamer((ushort)Math.Clamp(RoamStatus, 0, ushort.MaxValue));

            if (LibertyPass != bw.Misc.IsLibertyTicketActivated)
                bw.Misc.IsLibertyTicketActivated = LibertyPass;
        }
    }

    private void SaveKeySystem()
    {
        if (SAV5 is SAV5B2W2 b2w2)
        {
            var keys = b2w2.Keys;
            for (int i = 0; i < 5; i++)
            {
                var obtainIndex = i * 2;
                var unlockIndex = obtainIndex + 1;
                keys.SetIsKeyObtained((KeyType5)i, KeyEntries[obtainIndex].Enabled);
                keys.SetIsKeyUnlocked((KeyType5)i, KeyEntries[unlockIndex].Enabled);
            }
        }
    }

    private void SaveMusical()
    {
        var musical = SAV5.Musical;
        foreach (var prop in Props)
            musical.SetHasProp(prop.Index, prop.Obtained);
    }

    private void SaveSubway()
    {
        SubwayPlay.CurrentType = SubwayCurrentType;
        SubwayPlay.CurrentBattle = SubwayCurrentBattle;

        Subway.Flag0 = SubwayFlag0;
        Subway.Flag1 = SubwayFlag1;
        Subway.Flag2 = SubwayFlag2;
        Subway.Flag3 = SubwayFlag3;
        Subway.SuperSingle = SuperSingle;
        Subway.SuperDouble = SuperDouble;
        Subway.SuperMulti = SuperMulti;
        Subway.Flag7 = SubwayFlag7;
        Subway.NPCMet = NpcMet;

        Subway.SinglePast = SinglePast;
        Subway.SingleRecord = SingleRecord;
        Subway.DoublePast = DoublePast;
        Subway.DoubleRecord = DoubleRecord;
        Subway.MultiNPCPast = MultiNpcPast;
        Subway.MultiNPCRecord = MultiNpcRecord;
        Subway.MultiFriendsPast = MultiFriendsPast;
        Subway.MultiFriendsRecord = MultiFriendsRecord;
        Subway.SuperSinglePast = SuperSinglePast;
        Subway.SuperSingleRecord = SuperSingleRecord;
        Subway.SuperDoublePast = SuperDoublePast;
        Subway.SuperDoubleRecord = SuperDoubleRecord;
        Subway.SuperMultiNPCPast = SuperMultiNpcPast;
        Subway.SuperMultiNPCRecord = SuperMultiNpcRecord;
        Subway.SuperMultiFriendsPast = SuperMultiFriendsPast;
        Subway.SuperMultiFriendsRecord = SuperMultiFriendsRecord;
    }

    private void SaveRecords()
    {
        Record.SetRecord16(Record16Index, (ushort)Math.Clamp(Record16Value, 0, ushort.MaxValue));
        Record.SetRecord32(Record32Index, Record32Value);
        Record.EndAccess();
    }
}

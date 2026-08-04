using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Misc editor for Gen 5 saves covering fly destinations, Battle Subway,
/// Key System (B2W2), roamers (BW), and Liberty Pass (BW).
/// </summary>
public partial class Misc5EditorViewModel : ViewModelBase
{
    private readonly SAV5 _sav;
    private readonly BattleSubway5 _subway;
    private readonly int _ofsFly;
    private readonly int[] _flyDestC;

    public Misc5EditorViewModel(SAV5 sav)
    {
        _sav = sav;
        _subway = sav.BattleSubway;

        // Setup fly destinations based on version
        if (sav.Version is GameVersion.B or GameVersion.W or GameVersion.BW)
        {
            _ofsFly = 0x204B2;
            _flyDestC = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 15, 11, 10, 13, 12, 14];
        }
        else // B2W2
        {
            _ofsFly = 0x20392;
            _flyDestC = [24, 27, 25, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 21, 20, 28, 26, 66, 19, 5, 6, 7, 22];
        }

        LoadMain();
        LoadFlyDestinations();
        LoadSubway();
        LoadKeySystem();
        LoadRoamer();
    }

    #region Properties

    public bool IsBW => _sav is SAV5BW;
    public bool IsB2W2 => _sav is SAV5B2W2;

    #endregion

    #region Fly Destinations

    [ObservableProperty]
    private ObservableCollection<FlyDestination5ViewModel> _flyDestinations = [];

    private void LoadFlyDestinations()
    {
        string[] flyDestNames = IsBW
            ? ["Nuvema Town", "Accumula Town", "Striaton City", "Nacrene City",
               "Castelia City", "Nimbasa City", "Driftveil City", "Mistralton City",
               "Icirrus City", "Opelucid City", "Victory Road", "Pokémon League",
               "Lacunosa Town", "Undella Town", "Black City/White Forest", "(Unity Tower)"]
            : ["Aspertia City", "Floccesy Town", "Virbank City",
               "Nuvema Town", "Accumula Town", "Striaton City", "Nacrene City",
               "Castelia City", "Nimbasa City", "Driftveil City", "Mistralton City",
               "Icirrus City", "Opelucid City",
               "Lacunosa Town", "Undella Town", "Black City/White Forest",
               "Lentimas Town", "Humilau City", "Victory Road", "Pokémon League",
               "Pokéstar Studios", "Join Avenue", "PWT", "(Unity Tower)"];

        uint valFly = ReadUInt32LittleEndian(_sav.Data.Slice(_ofsFly));

        FlyDestinations.Clear();
        for (int i = 0; i < flyDestNames.Length && i < _flyDestC.Length; i++)
        {
            bool state;
            if (_flyDestC[i] < 32)
                state = (valFly & (1u << _flyDestC[i])) != 0;
            else
                state = (_sav.Data[_ofsFly + (_flyDestC[i] >> 3)] & (1 << (_flyDestC[i] & 7))) != 0;

            FlyDestinations.Add(new FlyDestination5ViewModel(i, flyDestNames[i], state));
        }
    }

    private void SaveFlyDestinations()
    {
        uint valFly = ReadUInt32LittleEndian(_sav.Data.Slice(_ofsFly));

        for (int i = 0; i < FlyDestinations.Count && i < _flyDestC.Length; i++)
        {
            if (_flyDestC[i] < 32)
            {
                if (FlyDestinations[i].IsUnlocked)
                    valFly |= 1u << _flyDestC[i];
                else
                    valFly &= ~(1u << _flyDestC[i]);
            }
            else
            {
                var ofs = _ofsFly + (_flyDestC[i] >> 3);
                _sav.Data[ofs] = (byte)((_sav.Data[ofs] & ~(1 << (_flyDestC[i] & 7))) |
                    ((FlyDestinations[i].IsUnlocked ? 1 : 0) << (_flyDestC[i] & 7)));
            }
        }
        WriteUInt32LittleEndian(_sav.Data.Slice(_ofsFly), valFly);
    }

    [RelayCommand]
    private void UnlockAllFlyDestinations()
    {
        foreach (var dest in FlyDestinations)
            dest.IsUnlocked = true;
    }

    #endregion

    #region Battle Subway

    // Normal mode records
    [ObservableProperty] private int _singlePast;
    [ObservableProperty] private int _singleRecord;
    [ObservableProperty] private int _doublePast;
    [ObservableProperty] private int _doubleRecord;
    [ObservableProperty] private int _multiNpcPast;
    [ObservableProperty] private int _multiNpcRecord;
    [ObservableProperty] private int _multiFriendsPast;
    [ObservableProperty] private int _multiFriendsRecord;

    // Super mode records
    [ObservableProperty] private int _superSinglePast;
    [ObservableProperty] private int _superSingleRecord;
    [ObservableProperty] private int _superDoublePast;
    [ObservableProperty] private int _superDoubleRecord;
    [ObservableProperty] private int _superMultiNpcPast;
    [ObservableProperty] private int _superMultiNpcRecord;
    [ObservableProperty] private int _superMultiFriendsPast;
    [ObservableProperty] private int _superMultiFriendsRecord;

    // Super mode unlock flags
    [ObservableProperty] private bool _superSingleUnlocked;
    [ObservableProperty] private bool _superDoubleUnlocked;
    [ObservableProperty] private bool _superMultiUnlocked;

    private void LoadSubway()
    {
        // Normal
        SinglePast = _subway.SinglePast;
        SingleRecord = _subway.SingleRecord;
        DoublePast = _subway.DoublePast;
        DoubleRecord = _subway.DoubleRecord;
        MultiNpcPast = _subway.MultiNPCPast;
        MultiNpcRecord = _subway.MultiNPCRecord;
        MultiFriendsPast = _subway.MultiFriendsPast;
        MultiFriendsRecord = _subway.MultiFriendsRecord;

        // Super
        SuperSinglePast = _subway.SuperSinglePast;
        SuperSingleRecord = _subway.SuperSingleRecord;
        SuperDoublePast = _subway.SuperDoublePast;
        SuperDoubleRecord = _subway.SuperDoubleRecord;
        SuperMultiNpcPast = _subway.SuperMultiNPCPast;
        SuperMultiNpcRecord = _subway.SuperMultiNPCRecord;
        SuperMultiFriendsPast = _subway.SuperMultiFriendsPast;
        SuperMultiFriendsRecord = _subway.SuperMultiFriendsRecord;

        // Unlock flags
        SuperSingleUnlocked = _subway.SuperSingle;
        SuperDoubleUnlocked = _subway.SuperDouble;
        SuperMultiUnlocked = _subway.SuperMulti;
    }

    private void SaveSubway()
    {
        // Normal
        _subway.SinglePast = SinglePast;
        _subway.SingleRecord = SingleRecord;
        _subway.DoublePast = DoublePast;
        _subway.DoubleRecord = DoubleRecord;
        _subway.MultiNPCPast = MultiNpcPast;
        _subway.MultiNPCRecord = MultiNpcRecord;
        _subway.MultiFriendsPast = MultiFriendsPast;
        _subway.MultiFriendsRecord = MultiFriendsRecord;

        // Super
        _subway.SuperSinglePast = SuperSinglePast;
        _subway.SuperSingleRecord = SuperSingleRecord;
        _subway.SuperDoublePast = SuperDoublePast;
        _subway.SuperDoubleRecord = SuperDoubleRecord;
        _subway.SuperMultiNPCPast = SuperMultiNpcPast;
        _subway.SuperMultiNPCRecord = SuperMultiNpcRecord;
        _subway.SuperMultiFriendsPast = SuperMultiFriendsPast;
        _subway.SuperMultiFriendsRecord = SuperMultiFriendsRecord;

        // Unlock flags
        _subway.SuperSingle = SuperSingleUnlocked;
        _subway.SuperDouble = SuperDoubleUnlocked;
        _subway.SuperMulti = SuperMultiUnlocked;
    }

    [RelayCommand]
    private void UnlockAllSuperModes()
    {
        SuperSingleUnlocked = true;
        SuperDoubleUnlocked = true;
        SuperMultiUnlocked = true;
    }

    #endregion

    #region Key System (B2W2 only)

    [ObservableProperty]
    private ObservableCollection<KeySystemViewModel> _keys = [];

    private void LoadKeySystem()
    {
        if (_sav is not SAV5B2W2 b2w2) return;

        var keyBlock = b2w2.Keys;
        string[] keyNames = ["EasyKey", "ChallengeKey", "CityKey", "IronKey", "IcebergKey"];

        Keys.Clear();
        for (int i = 0; i < 5; i++)
        {
            var keyType = (KeyType5)i;
            Keys.Add(new KeySystemViewModel(
                keyNames[i],
                keyType,
                keyBlock.GetIsKeyObtained(keyType),
                keyBlock.GetIsKeyUnlocked(keyType)));
        }
    }

    private void SaveKeySystem()
    {
        if (_sav is not SAV5B2W2 b2w2) return;

        var keyBlock = b2w2.Keys;
        foreach (var key in Keys)
        {
            keyBlock.SetIsKeyObtained(key.KeyType, key.IsObtained);
            keyBlock.SetIsKeyUnlocked(key.KeyType, key.IsUnlocked);
        }
    }

    [RelayCommand]
    private void UnlockAllKeys()
    {
        foreach (var key in Keys)
        {
            key.IsObtained = true;
            key.IsUnlocked = true;
        }
    }

    #endregion

    #region Roamer (BW only)

    [ObservableProperty]
    private bool _libertyPassActivated;

    [ObservableProperty]
    private ObservableCollection<RoamerViewModel> _roamers = [];

    public string[] RoamerStates { get; } = ["Not roamed", "Roaming", "Defeated", "Captured"];

    private void LoadRoamer()
    {
        if (_sav is not SAV5BW bw) return;

        LibertyPassActivated = bw.Misc.IsLibertyTicketActivated;

        string[] roamerNames = ["Thundurus", "Tornadus"];
        var encount = bw.Encount;

        Roamers.Clear();
        for (int i = 0; i < 2; i++)
        {
            var state = encount.GetRoamerState(i);
            Roamers.Add(new RoamerViewModel(i, roamerNames[i], Math.Min((int)state, 3)));
        }
    }

    private void SaveRoamer()
    {
        if (_sav is not SAV5BW bw) return;

        bw.Misc.IsLibertyTicketActivated = LibertyPassActivated;

        var encount = bw.Encount;
        foreach (var roamer in Roamers)
        {
            var current = encount.GetRoamerState(roamer.Index);
            if (current != roamer.State)
            {
                encount.SetRoamerState(roamer.Index, (byte)roamer.State);
                if (current == 1) // Was roaming, clear roamer data
                {
                    var roamerData = roamer.Index == 0 ? encount.Roamer1 : encount.Roamer2;
                    roamerData.Clear();
                    encount.SetRoamerState2C(roamer.Index, 0);
                }
            }
        }
    }

    #endregion

    #region Main

    private void LoadMain()
    {
        // Future expansion
    }

    #endregion

    #region Commands

    [RelayCommand]
    private void Save()
    {
        SaveFlyDestinations();
        SaveSubway();
        SaveKeySystem();
        SaveRoamer();
    }

    #endregion
}

/// <summary>
/// ViewModel for a Gen 5 fly destination.
/// </summary>
public partial class FlyDestination5ViewModel : ObservableObject
{
    public FlyDestination5ViewModel(int index, string name, bool isUnlocked)
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
/// ViewModel for a B2W2 Key System entry.
/// </summary>
public partial class KeySystemViewModel : ObservableObject
{
    public KeySystemViewModel(string name, KeyType5 keyType, bool isObtained, bool isUnlocked)
    {
        Name = name;
        KeyType = keyType;
        _isObtained = isObtained;
        _isUnlocked = isUnlocked;
    }

    public string Name { get; }
    public KeyType5 KeyType { get; }

    [ObservableProperty]
    private bool _isObtained;

    [ObservableProperty]
    private bool _isUnlocked;
}

/// <summary>
/// ViewModel for a BW roamer (Thundurus/Tornadus).
/// </summary>
public partial class RoamerViewModel : ObservableObject
{
    public RoamerViewModel(int index, string name, int state)
    {
        Index = index;
        Name = name;
        _state = state;
    }

    public int Index { get; }
    public string Name { get; }

    [ObservableProperty]
    private int _state;

    public string StateText => State switch
    {
        0 => "Not roamed",
        1 => "Roaming",
        2 => "Defeated",
        3 => "Captured",
        _ => "Unknown"
    };
}

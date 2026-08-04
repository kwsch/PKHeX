using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class OPowerEditorViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly OPower6? _block;

    public OPowerEditorViewModel(SaveFile sav)
    {
        _sav = sav;

        if (sav is ISaveBlock6Main main)
        {
            _block = main.OPower;
            IsSupported = true;
            LoadData();
        }
    }

    public bool IsSupported { get; }

    [ObservableProperty]
    private byte _points;

    partial void OnPointsChanged(byte value)
    {
        if (_block is not null)
            _block.Points = value;
    }

    [ObservableProperty]
    private ObservableCollection<OPowerFieldViewModel> _fieldPowers = [];

    [ObservableProperty]
    private ObservableCollection<OPowerBattleViewModel> _battlePowers = [];

    [ObservableProperty]
    private ObservableCollection<OPowerUnlockViewModel> _unlocks = [];

    private void LoadData()
    {
        if (_block is null) return;

        Points = _block.Points;

        // Field Powers
        FieldPowers.Clear();
        var fieldNames = new[] { "Hatching", "Bargain", "Prize Money", "Exp. Point", "Capture", "Encounter", "Stealth", "HP Restoring", "PP Restoring", "Befriending" };
        for (int i = 0; i < fieldNames.Length; i++)
        {
            var type = (OPower6FieldType)i;
            FieldPowers.Add(new OPowerFieldViewModel(fieldNames[i], type, _block));
        }

        // Battle Powers
        BattlePowers.Clear();
        var battleNames = new[] { "Attack", "Defense", "Sp. Atk", "Sp. Def", "Speed", "Critical", "Accuracy" };
        for (int i = 0; i < battleNames.Length; i++)
        {
            var type = (OPower6BattleType)i;
            BattlePowers.Add(new OPowerBattleViewModel(battleNames[i], type, _block));
        }

        // Unlocks
        Unlocks.Clear();
        var unlockNames = Enum.GetNames<OPower6Index>();
        for (int i = 0; i < (int)OPower6Index.Count; i++)
        {
            var idx = (OPower6Index)i;
            var isUnlocked = _block.GetState(idx) == OPowerFlagState.Unlocked;
            Unlocks.Add(new OPowerUnlockViewModel(unlockNames[i], idx, isUnlocked, _block));
        }
    }

    [RelayCommand]
    private void UnlockAll()
    {
        _block?.UnlockAll();
        LoadData();
    }

    [RelayCommand]
    private void ClearAll()
    {
        _block?.ClearAll();
        LoadData();
    }
}

public partial class OPowerFieldViewModel : ViewModelBase
{
    private readonly OPower6FieldType _type;
    private readonly OPower6 _block;

    public OPowerFieldViewModel(string name, OPower6FieldType type, OPower6 block)
    {
        Name = name;
        _type = type;
        _block = block;

        _level1 = block.GetLevel1(type);
        _level2 = block.GetLevel2(type);
    }

    public string Name { get; }

    [ObservableProperty]
    private byte _level1;

    partial void OnLevel1Changed(byte value) => _block.SetLevel1(_type, value);

    [ObservableProperty]
    private byte _level2;

    partial void OnLevel2Changed(byte value) => _block.SetLevel2(_type, value);
}

public partial class OPowerBattleViewModel : ViewModelBase
{
    private readonly OPower6BattleType _type;
    private readonly OPower6 _block;

    public OPowerBattleViewModel(string name, OPower6BattleType type, OPower6 block)
    {
        Name = name;
        _type = type;
        _block = block;

        _level1 = block.GetLevel1(type);
        _level2 = block.GetLevel2(type);
    }

    public string Name { get; }

    [ObservableProperty]
    private byte _level1;

    partial void OnLevel1Changed(byte value) => _block.SetLevel1(_type, value);

    [ObservableProperty]
    private byte _level2;

    partial void OnLevel2Changed(byte value) => _block.SetLevel2(_type, value);
}

public partial class OPowerUnlockViewModel : ViewModelBase
{
    private readonly OPower6Index _idx;
    private readonly OPower6 _block;

    public OPowerUnlockViewModel(string name, OPower6Index idx, bool isUnlocked, OPower6 block)
    {
        Name = name;
        _idx = idx;
        _isUnlocked = isUnlocked;
        _block = block;
    }

    public string Name { get; }

    [ObservableProperty]
    private bool _isUnlocked;

    partial void OnIsUnlockedChanged(bool value)
    {
        _block.SetState(_idx, value ? OPowerFlagState.Unlocked : OPowerFlagState.Locked);
    }
}

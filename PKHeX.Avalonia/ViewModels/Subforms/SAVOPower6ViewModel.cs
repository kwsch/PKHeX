using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for an unlock flag item.
/// </summary>
public partial class OPowerUnlockModel : ObservableObject
{
    public string Name { get; }
    public int Index { get; }

    [ObservableProperty]
    private bool _isUnlocked;

    public OPowerUnlockModel(string name, int index, bool unlocked)
    {
        Name = name;
        Index = index;
        _isUnlocked = unlocked;
    }
}

/// <summary>
/// Model for a field/battle level row.
/// </summary>
public partial class OPowerLevelModel : ObservableObject
{
    public string Name { get; }
    public int Index { get; }

    [ObservableProperty]
    private byte _level1;

    [ObservableProperty]
    private byte _level2;

    public OPowerLevelModel(string name, int index, byte level1, byte level2)
    {
        Name = name;
        Index = index;
        _level1 = level1;
        _level2 = level2;
    }
}

/// <summary>
/// ViewModel for the Gen 6 O-Power editor.
/// Edits unlock states, field/battle levels, and points.
/// </summary>
public partial class SAVOPower6ViewModel : SaveEditorViewModelBase
{
    private readonly SaveFile _origin;
    private readonly SaveFile _clone;
    private readonly OPower6 _block;

    public ObservableCollection<OPowerUnlockModel> UnlockFlags { get; } = [];
    public ObservableCollection<OPowerLevelModel> FieldLevels { get; } = [];
    public ObservableCollection<OPowerLevelModel> BattleLevels { get; } = [];

    [ObservableProperty]
    private byte _points;

    public SAVOPower6ViewModel(ISaveBlock6Main sav) : base((SaveFile)sav)
    {
        _origin = (SaveFile)sav;
        _clone = _origin.Clone();
        _block = ((ISaveBlock6Main)_clone).OPower;

        LoadData();
    }

    private void LoadData()
    {
        UnlockFlags.Clear();
        FieldLevels.Clear();
        BattleLevels.Clear();

        // Unlock flags - use enum names
        var indexNames = Enum.GetNames<OPower6Index>();
        int indexCount = (int)OPower6Index.Count;
        for (int i = 0; i < indexCount; i++)
        {
            var name = i < indexNames.Length ? indexNames[i] : $"Index {i}";
            bool unlocked = _block.GetState((OPower6Index)i) == OPowerFlagState.Unlocked;
            UnlockFlags.Add(new OPowerUnlockModel(name, i, unlocked));
        }

        // Field levels
        var fieldNames = Enum.GetNames<OPower6FieldType>();
        int fieldCount = (int)OPower6FieldType.Count;
        for (int i = 0; i < fieldCount; i++)
        {
            var name = i < fieldNames.Length ? fieldNames[i] : $"Field {i}";
            FieldLevels.Add(new OPowerLevelModel(name, i,
                _block.GetLevel1((OPower6FieldType)i),
                _block.GetLevel2((OPower6FieldType)i)));
        }

        // Battle levels
        var battleNames = Enum.GetNames<OPower6BattleType>();
        int battleCount = (int)OPower6BattleType.Count;
        for (int i = 0; i < battleCount; i++)
        {
            var name = i < battleNames.Length ? battleNames[i] : $"Battle {i}";
            BattleLevels.Add(new OPowerLevelModel(name, i,
                _block.GetLevel1((OPower6BattleType)i),
                _block.GetLevel2((OPower6BattleType)i)));
        }

        Points = _block.Points;
    }

    [RelayCommand]
    private void GiveAll()
    {
        _block.UnlockAll();
        LoadData();
    }

    [RelayCommand]
    private void ClearAll()
    {
        _block.ClearAll();
        LoadData();
    }

    [RelayCommand]
    private void Save()
    {
        foreach (var flag in UnlockFlags)
            _block.SetState((OPower6Index)flag.Index, flag.IsUnlocked ? OPowerFlagState.Unlocked : OPowerFlagState.Locked);
        foreach (var field in FieldLevels)
        {
            _block.SetLevel1((OPower6FieldType)field.Index, field.Level1);
            _block.SetLevel2((OPower6FieldType)field.Index, field.Level2);
        }
        foreach (var battle in BattleLevels)
        {
            _block.SetLevel1((OPower6BattleType)battle.Index, battle.Level1);
            _block.SetLevel2((OPower6BattleType)battle.Index, battle.Level2);
        }
        _block.Points = Points;
        _origin.CopyChangesFrom(_clone);
        Modified = true;
    }
}

using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the Honey Tree editor (Gen 4 Sinnoh).
/// Edits honey tree encounter data.
/// </summary>
public partial class HoneyTree4ViewModel : SaveEditorViewModelBase
{
    private readonly SAV4Sinnoh SAV4S;
    private HoneyTreeValue? _tree;
    private int _treeIndex;

    public string[] TreeNames { get; }
    public string MunchlaxTreesText { get; }

    [ObservableProperty]
    private int _selectedTreeIndex;

    [ObservableProperty]
    private uint _time;

    [ObservableProperty]
    private int _shake;

    [ObservableProperty]
    private int _group;

    [ObservableProperty]
    private int _slot;

    [ObservableProperty]
    private string _speciesLabel = string.Empty;

    public HoneyTree4ViewModel(SAV4Sinnoh sav) : base(sav)
    {
        SAV4S = sav;

        // Build tree name list
        TreeNames =
        [
            "Route 205 (South)", "Route 205 (North)", "Route 206",
            "Route 207", "Route 208", "Route 209",
            "Route 210 (South)", "Route 210 (North)", "Route 211",
            "Route 212 (North)", "Route 212 (South)", "Route 213",
            "Route 214", "Route 215", "Route 218",
            "Route 221", "Route 222",
            "Valley Windworks", "Eterna Forest",
            "Fuego Ironworks", "Floaroma Meadow",
        ];

        // Calculate munchlax trees
        byte[] munchlaxTrees = new byte[4];
        HoneyTreeUtil.CalculateMunchlaxTrees(sav.ID32, munchlaxTrees);
        MunchlaxTreesText = string.Join(", ", Array.ConvertAll(munchlaxTrees, t => t < TreeNames.Length ? TreeNames[t] : $"Tree {t}"));

        _selectedTreeIndex = 0;
        LoadTree(0);
    }

    partial void OnSelectedTreeIndexChanged(int value)
    {
        SaveTree();
        LoadTree(value);
    }

    private void LoadTree(int index)
    {
        _treeIndex = index;
        _tree = SAV4S.GetHoneyTree(index);
        Time = _tree.Time;
        Shake = _tree.Shake;
        Group = _tree.Group;
        Slot = _tree.Slot;
        UpdateSpeciesLabel();
    }

    partial void OnGroupChanged(int value) => UpdateSpeciesLabel();
    partial void OnSlotChanged(int value) => UpdateSpeciesLabel();

    private void UpdateSpeciesLabel()
    {
        var species = SAV4S.GetHoneyTreeSpecies(Group, Slot);
        var speciesNames = GameInfo.Strings.specieslist;
        SpeciesLabel = species < speciesNames.Length ? speciesNames[species] : $"Species {species}";
    }

    private void SaveTree()
    {
        if (_tree is null)
            return;
        _tree.Time = Time;
        _tree.Shake = Shake;
        _tree.Group = Group;
        _tree.Slot = Slot;
        SAV4S.SetHoneyTree(_tree, _treeIndex);
    }

    [RelayCommand]
    private void SetCatchable() => Time = 1080;

    [RelayCommand]
    private void Save()
    {
        SaveTree();
        SAV.State.Edited = true;
        Modified = true;
    }
}

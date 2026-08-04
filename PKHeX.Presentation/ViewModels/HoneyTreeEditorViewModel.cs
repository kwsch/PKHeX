using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class HoneyTreeEditorViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly SAV4Sinnoh? _sinnoh;

    private static readonly string[] TreeNames =
    [
        "Route 205 (Eterna)", "Route 205 (Floaroma)", "Route 206", "Route 207",
        "Route 208", "Route 209", "Route 210 (Solaceon)", "Route 210 (Celestic)",
        "Route 211", "Route 212 (Hearthome)", "Route 212 (Pastoria)", "Route 213",
        "Route 214", "Route 215", "Route 218", "Route 221", "Route 222",
        "Valley Windworks", "Eterna Forest", "Fuego Ironworks", "Floaroma Meadow"
    ];

    public HoneyTreeEditorViewModel(SaveFile sav)
    {
        _sav = sav;
        _sinnoh = sav as SAV4Sinnoh;
        IsSupported = _sinnoh is not null;

        if (IsSupported)
        {
            LoadTrees();
            CalculateMunchlaxTrees();
        }
    }

    public bool IsSupported { get; }

    [ObservableProperty]
    private ObservableCollection<HoneyTreeViewModel> _trees = [];

    [ObservableProperty]
    private HoneyTreeViewModel? _selectedTree;

    [ObservableProperty]
    private string _munchlaxTreesText = string.Empty;

    private void LoadTrees()
    {
        Trees.Clear();
        if (_sinnoh is null) return;

        for (int i = 0; i < TreeNames.Length; i++)
        {
            var tree = _sinnoh.GetHoneyTree(i);
            Trees.Add(new HoneyTreeViewModel(i, TreeNames[i], tree, _sinnoh));
        }

        if (Trees.Count > 0)
            SelectedTree = Trees[0];
    }

    private void CalculateMunchlaxTrees()
    {
        if (_sinnoh is null) return;

        var munchlaxTrees = new byte[4];
        HoneyTreeUtil.CalculateMunchlaxTrees(_sinnoh.ID32, munchlaxTrees);

        var lines = new System.Text.StringBuilder();
        lines.AppendLine("Munchlax can appear in:");
        foreach (var idx in munchlaxTrees)
        {
            if (idx < TreeNames.Length)
                lines.AppendLine($"  • {TreeNames[idx]}");
        }
        MunchlaxTreesText = lines.ToString();
    }

    [RelayCommand]
    private void Refresh() => LoadTrees();
}

public partial class HoneyTreeViewModel : ViewModelBase
{
    private readonly HoneyTreeValue _tree;
    private readonly SAV4Sinnoh _sav;
    private readonly int _index;

    public HoneyTreeViewModel(int index, string name, HoneyTreeValue tree, SAV4Sinnoh sav)
    {
        _index = index;
        Name = name;
        _tree = tree;
        _sav = sav;

        _time = (int)tree.Time;
        _shake = tree.Shake;
        _group = tree.Group;
        _slot = tree.Slot;
    }

    public string Name { get; }

    [ObservableProperty]
    private int _time;

    partial void OnTimeChanged(int value)
    {
        _tree.Time = (uint)value;
        SaveTree();
    }

    [ObservableProperty]
    private int _shake;

    partial void OnShakeChanged(int value)
    {
        _tree.Shake = value;
        SaveTree();
    }

    [ObservableProperty]
    private int _group;

    partial void OnGroupChanged(int value)
    {
        _tree.Group = value;
        SaveTree();
        OnPropertyChanged(nameof(SpeciesName));
    }

    [ObservableProperty]
    private int _slot;

    partial void OnSlotChanged(int value)
    {
        _tree.Slot = value;
        SaveTree();
        OnPropertyChanged(nameof(SpeciesName));
    }

    public string SpeciesName
    {
        get
        {
            var species = _sav.GetHoneyTreeSpecies(Group, Slot);
            if (species == 0) return "(None)";
            var names = GameInfo.Strings.Species;
            return species < names.Count ? names[species] : $"Species #{species}";
        }
    }

    private void SaveTree() => _sav.SetHoneyTree(_tree, _index);

    [RelayCommand]
    private void MakeCatchable()
    {
        Time = 1080; // Makes Pokemon catchable
    }
}

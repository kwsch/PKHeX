using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single PokeBlock entry in ORAS.
/// </summary>
public partial class PokeBlock6Model : ObservableObject
{
    public int Index { get; }
    public string Label { get; }

    [ObservableProperty]
    private uint _count;

    public PokeBlock6Model(int index, string label, uint count)
    {
        Index = index;
        Label = label;
        _count = count;
    }
}

/// <summary>
/// ViewModel for the Gen 6 ORAS PokeBlock editor.
/// Edits the 12 pokeblock counts.
/// </summary>
public partial class SAVPokeBlockORASViewModel : SaveEditorViewModelBase
{
    private readonly SAV6AO _origin;
    private readonly SAV6AO _sav;

    public ObservableCollection<PokeBlock6Model> Blocks { get; } = [];

    public SAVPokeBlockORASViewModel(SAV6AO sav) : base(sav)
    {
        _sav = (SAV6AO)(_origin = sav).Clone();

        var contest = _sav.Contest;
        var blockNames = GameInfo.Strings.pokeblocks;
        for (int i = 0; i < Contest6.CountBlock; i++)
        {
            var label = (94 + i < blockNames.Length) ? blockNames[94 + i] : $"Block {i}";
            Blocks.Add(new PokeBlock6Model(i, label, contest.GetBlockCount(i)));
        }
    }

    [RelayCommand]
    private void GiveAll()
    {
        foreach (var block in Blocks)
            block.Count = 999;
    }

    [RelayCommand]
    private void ClearAll()
    {
        foreach (var block in Blocks)
            block.Count = 0;
    }

    [RelayCommand]
    private void Save()
    {
        var contest = _sav.Contest;
        foreach (var block in Blocks)
            contest.SetBlockCount(block.Index, block.Count);
        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }
}

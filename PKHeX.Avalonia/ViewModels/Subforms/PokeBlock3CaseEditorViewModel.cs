using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single PokeBlock entry in the case.
/// </summary>
public partial class PokeBlock3Model : ObservableObject
{
    private readonly PokeBlock3 _block;
    public int Index { get; }

    [ObservableProperty]
    private PokeBlock3Color _color;

    [ObservableProperty]
    private byte _spicy;

    [ObservableProperty]
    private byte _dry;

    [ObservableProperty]
    private byte _sweet;

    [ObservableProperty]
    private byte _bitter;

    [ObservableProperty]
    private byte _sour;

    [ObservableProperty]
    private byte _feel;

    public string DisplayName => $"{Index + 1:00} - {Color}";

    public PokeBlock3Model(PokeBlock3 block, int index)
    {
        _block = block;
        Index = index;
        _color = block.Color;
        _spicy = block.Spicy;
        _dry = block.Dry;
        _sweet = block.Sweet;
        _bitter = block.Bitter;
        _sour = block.Sour;
        _feel = block.Feel;
    }

    public void SaveToBlock()
    {
        _block.Color = Color;
        _block.Spicy = Spicy;
        _block.Dry = Dry;
        _block.Sweet = Sweet;
        _block.Bitter = Bitter;
        _block.Sour = Sour;
        _block.Feel = Feel;
    }

    public void LoadFromBlock()
    {
        Color = _block.Color;
        Spicy = _block.Spicy;
        Dry = _block.Dry;
        Sweet = _block.Sweet;
        Bitter = _block.Bitter;
        Sour = _block.Sour;
        Feel = _block.Feel;
        OnPropertyChanged(nameof(DisplayName));
    }
}

/// <summary>
/// ViewModel for the Gen 3 PokeBlock Case editor.
/// Allows viewing and editing all 40 PokeBlock slots.
/// </summary>
public partial class PokeBlock3CaseEditorViewModel : SaveEditorViewModelBase
{
    private readonly SaveFile _origin;
    private readonly SAV3 _sav;
    private readonly ISaveBlock3LargeHoenn _hoenn;
    private readonly PokeBlock3Case _case;

    public ObservableCollection<PokeBlock3Model> Blocks { get; } = [];
    public Array ColorValues { get; } = Enum.GetValues<PokeBlock3Color>();

    [ObservableProperty]
    private PokeBlock3Model? _selectedBlock;

    [ObservableProperty]
    private int _selectedBlockIndex = -1;

    public PokeBlock3CaseEditorViewModel(SAV3 sav, ISaveBlock3LargeHoenn hoenn) : base(sav)
    {
        _origin = sav;
        _sav = (SAV3)sav.Clone();
        _hoenn = (ISaveBlock3LargeHoenn)_sav.LargeBlock;
        _case = _hoenn.PokeBlocks;

        for (int i = 0; i < _case.Blocks.Length; i++)
            Blocks.Add(new PokeBlock3Model(_case.Blocks[i], i));

        if (Blocks.Count > 0)
            SelectedBlockIndex = 0;
    }

    partial void OnSelectedBlockIndexChanged(int value)
    {
        if (value >= 0 && value < Blocks.Count)
            SelectedBlock = Blocks[value];
    }

    [RelayCommand]
    private void MaximizeAll()
    {
        _case.MaximizeAll(true);
        foreach (var block in Blocks)
            block.LoadFromBlock();
    }

    [RelayCommand]
    private void DeleteAll()
    {
        _case.DeleteAll();
        foreach (var block in Blocks)
            block.LoadFromBlock();
    }

    [RelayCommand]
    private void Save()
    {
        foreach (var block in Blocks)
            block.SaveToBlock();
        _hoenn.PokeBlocks = _case;
        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }
}

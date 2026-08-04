using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class PokeBlock3CaseEditorViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private PokeBlock3Case? _case;

    private static readonly string[] ColorNames = ["None", "Red", "Blue", "Pink", "Green", "Yellow", "Purple", "Indigo", "Brown", "Lite Blue", "Olive", "Gray", "Black", "White", "Gold"];

    public PokeBlock3CaseEditorViewModel(SaveFile sav)
    {
        _sav = sav;

        if (sav is SAV3E e)
        {
            _case = e.LargeBlock.PokeBlocks;
            IsSupported = true;
        }
        else if (sav is SAV3RS rs)
        {
            _case = rs.LargeBlock.PokeBlocks;
            IsSupported = true;
        }

        if (IsSupported)
            LoadData();
    }

    public bool IsSupported { get; }
    public string GameInfo => $"{_sav.Version} - Pokéblock Case (40 slots)";

    [ObservableProperty]
    private ObservableCollection<PokeBlock3ViewModel> _blocks = [];

    [ObservableProperty]
    private int _filledCount;

    private void LoadData()
    {
        if (_case is null) return;

        Blocks.Clear();
        FilledCount = 0;

        for (int i = 0; i < _case.Blocks.Length; i++)
        {
            var block = _case.Blocks[i];
            var vm = new PokeBlock3ViewModel(i, block);
            Blocks.Add(vm);
            if (block.Color != PokeBlock3Color.NoBlock) FilledCount++;
        }
    }

    [RelayCommand]
    private void MaximizeAll()
    {
        _case?.MaximizeAll(createMissing: true);
        SaveToGame();
        LoadData();
    }

    [RelayCommand]
    private void MaximizeExisting()
    {
        _case?.MaximizeAll(createMissing: false);
        SaveToGame();
        LoadData();
    }

    [RelayCommand]
    private void DeleteAll()
    {
        _case?.DeleteAll();
        SaveToGame();
        LoadData();
    }

    private void SaveToGame()
    {
        if (_case is null) return;
        
        if (_sav is SAV3E e)
            e.LargeBlock.PokeBlocks = _case;
        else if (_sav is SAV3RS rs)
            rs.LargeBlock.PokeBlocks = _case;
    }

    [RelayCommand]
    private void Refresh() => LoadData();
}

public class PokeBlock3ViewModel
{
    private readonly PokeBlock3 _block;

    public PokeBlock3ViewModel(int slot, PokeBlock3 block)
    {
        Slot = slot;
        _block = block;
    }

    public int Slot { get; }
    public string SlotLabel => $"#{Slot + 1:00}";

    public string ColorName => _block.Color.ToString();
    public byte Level => _block.Level;
    public byte Feel => _block.Feel;

    public byte Spicy => _block.Spicy;
    public byte Dry => _block.Dry;
    public byte Sweet => _block.Sweet;
    public byte Bitter => _block.Bitter;
    public byte Sour => _block.Sour;

    public bool IsEmpty => _block.Color == PokeBlock3Color.NoBlock;
}

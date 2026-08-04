using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class PokeBlockEditorViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly Contest6? _contest;

    private static readonly string[] BlockNames =
    [
        "Red", "Blue", "Pink", "Green", "Yellow", "Rainbow",
        "Red+", "Blue+", "Pink+", "Green+", "Yellow+", "Rainbow+"
    ];

    public PokeBlockEditorViewModel(SaveFile sav)
    {
        _sav = sav;

        if (sav is SAV6AO ao)
        {
            _contest = ao.Contest;
            IsSupported = true;
            LoadData();
        }
    }

    public bool IsSupported { get; }

    [ObservableProperty]
    private ObservableCollection<PokeBlockSlotViewModel> _blocks = [];

    private void LoadData()
    {
        if (_contest is null) return;

        Blocks.Clear();
        var pokeblockNames = GameInfo.Strings.pokeblocks;

        for (int i = 0; i < BlockNames.Length; i++)
        {
            var name = i + 94 < pokeblockNames.Length ? pokeblockNames[i + 94] : BlockNames[i];
            var count = _contest.GetBlockCount(i);
            Blocks.Add(new PokeBlockSlotViewModel(i, name, count, SetBlockCount));
        }
    }

    private void SetBlockCount(int index, uint count)
    {
        _contest?.SetBlockCount(index, count);
    }

    [RelayCommand]
    private void GiveAll()
    {
        if (_contest is null) return;
        for (int i = 0; i < BlockNames.Length; i++)
            _contest.SetBlockCount(i, 999);
        LoadData();
    }

    [RelayCommand]
    private void ClearAll()
    {
        if (_contest is null) return;
        for (int i = 0; i < BlockNames.Length; i++)
            _contest.SetBlockCount(i, 0);
        LoadData();
    }

    [RelayCommand]
    private void Refresh() => LoadData();
}

public partial class PokeBlockSlotViewModel : ViewModelBase
{
    private readonly System.Action<int, uint> _onChanged;

    public PokeBlockSlotViewModel(int index, string name, uint count, System.Action<int, uint> onChanged)
    {
        Index = index;
        Name = name;
        _count = count;
        _onChanged = onChanged;
    }

    public int Index { get; }
    public string Name { get; }

    [ObservableProperty]
    private uint _count;

    partial void OnCountChanged(uint value) => _onChanged(Index, value);
}

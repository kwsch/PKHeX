using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class RaidSevenStar9EditorViewModel : ViewModelBase
{
    private readonly SAV9SV? _sav9;
    private readonly RaidSevenStar9? _raidsData;

    public RaidSevenStar9EditorViewModel(SaveFile sav)
    {
        _sav9 = sav as SAV9SV;
        IsSupported = _sav9?.RaidSevenStar is not null;

        if (_sav9 is not null)
        {
            _raidsData = _sav9.RaidSevenStar;
            LoadRaids();
        }
    }

    public bool IsSupported { get; }

    [ObservableProperty]
    private ObservableCollection<RaidSevenStarItem> _raidItems = [];

    [ObservableProperty]
    private RaidSevenStarItem? _selectedRaid;

    private void LoadRaids()
    {
        if (_raidsData is null) return;

        RaidItems.Clear();
        for (int i = 0; i < _raidsData.CountAll; i++)
        {
            var raid = _raidsData.GetRaid(i);
            RaidItems.Add(new RaidSevenStarItem(i, raid));
        }

        if (RaidItems.Count > 0)
            SelectedRaid = RaidItems[0];
    }
}


public class RaidSevenStarItem
{
    private readonly SevenStarRaidDetail _raid;

    public RaidSevenStarItem(int index, SevenStarRaidDetail raid)
    {
        Index = index;
        _raid = raid;
    }

    public int Index { get; }
    public string DisplayName => $"Raid {Index + 1:0000}";

    public bool Captured
    {
        get => _raid.Captured;
        set => _raid.Captured = value;
    }

    public bool Defeated
    {
        get => _raid.Defeated;
        set => _raid.Defeated = value;
    }
}

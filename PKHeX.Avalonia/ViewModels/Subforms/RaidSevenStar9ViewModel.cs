using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single Seven Star Raid entry.
/// </summary>
public partial class SevenStarRaid9Model : ObservableObject
{
    public int Index { get; }
    public string Label { get; }
    public SevenStarRaidDetail Detail { get; }

    public SevenStarRaid9Model(int index, SevenStarRaidDetail detail)
    {
        Index = index;
        Label = $"Raid {index + 1:0000}";
        Detail = detail;
    }
}

/// <summary>
/// ViewModel for the Scarlet/Violet Seven Star Raid editor.
/// </summary>
public partial class RaidSevenStar9ViewModel : SaveEditorViewModelBase
{
    private readonly SaveFile _origin;
    private readonly SAV9SV _sav;
    private readonly RaidSevenStar9 _raids;

    public ObservableCollection<SevenStarRaid9Model> Raids { get; } = [];

    [ObservableProperty] private SevenStarRaid9Model? _selectedRaid;

    public RaidSevenStar9ViewModel(SAV9SV sav) : base(sav)
    {
        _sav = (SAV9SV)(_origin = sav).Clone();
        _raids = _sav.RaidSevenStar;

        for (int i = 0; i < _raids.CountAll; i++)
        {
            var detail = _raids.GetRaid(i);
            Raids.Add(new SevenStarRaid9Model(i, detail));
        }

        if (Raids.Count > 0)
            SelectedRaid = Raids[0];
    }

    [RelayCommand]
    private void Save()
    {
        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }
}

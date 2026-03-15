using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the Gen 6 Roaming Pokemon editor.
/// Edits species, roam state, and times encountered.
/// </summary>
public partial class SAVRoamer6ViewModel : SaveEditorViewModelBase
{
    private readonly SAV6XY _origin;
    private readonly SAV6XY _sav;
    private readonly Roamer6 _roamer;

    public List<string> SpeciesChoices { get; }
    public List<string> RoamStateChoices { get; } = ["Inactive", "Roaming", "Stationary", "Defeated", "Captured"];

    [ObservableProperty]
    private int _selectedSpeciesIndex;

    [ObservableProperty]
    private int _selectedRoamStateIndex;

    [ObservableProperty]
    private uint _timesEncountered;

    public SAVRoamer6ViewModel(SAV6XY sav) : base(sav)
    {
        _sav = (SAV6XY)(_origin = sav).Clone();
        _roamer = _sav.Encount.Roamer;

        var species = GameInfo.Strings.specieslist;
        SpeciesChoices =
        [
            species[(int)Species.Articuno],
            species[(int)Species.Zapdos],
            species[(int)Species.Moltres],
        ];

        SelectedSpeciesIndex = GetInitialIndex();
        TimesEncountered = _roamer.TimesEncountered;
        SelectedRoamStateIndex = (int)_roamer.RoamStatus;
    }

    private int GetInitialIndex()
    {
        const int speciesOffset = 144;
        const int starterChoiceIndex = 48;
        if (_roamer.Species != 0)
            return _roamer.Species - speciesOffset;
        return _sav.EventWork.GetWork(starterChoiceIndex);
    }

    [RelayCommand]
    private void Save()
    {
        const int speciesOffset = 144;
        _roamer.Species = (ushort)(speciesOffset + SelectedSpeciesIndex);
        _roamer.TimesEncountered = TimesEncountered;
        _roamer.RoamStatus = (Roamer6State)SelectedRoamStateIndex;
        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }
}

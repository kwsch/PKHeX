using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Editor for Gen 6 Roaming Pokémon (Articuno, Zapdos, Moltres).
/// </summary>
public partial class Roamer6EditorViewModel : ViewModelBase
{
    private readonly SAV6XY _sav;
    private readonly Roamer6 _roamer;
    private const int SpeciesOffset = 144; // Articuno is 144
    private const int StarterChoiceIndex = 48; // Offset in EventWork to find starter choice if roamer not set

    public Roamer6EditorViewModel(SAV6XY sav)
    {
        _sav = sav;
        _roamer = sav.Encount.Roamer;

        LoadRoamer();
    }

    [ObservableProperty]
    private int _selectedSpeciesIndex;

    [ObservableProperty]
    private int _roamStateIndex;

    [ObservableProperty]
    private uint _timesEncountered;

    public string[] SpeciesList { get; } =
    [
        GameInfo.Strings.Species[144], // Articuno
        GameInfo.Strings.Species[145], // Zapdos
        GameInfo.Strings.Species[146]  // Moltres
    ];

    public string[] RoamStates { get; } = ["Inactive", "Roaming", "Stationary", "Defeated", "Captured"];

    private void LoadRoamer()
    {
        SelectedSpeciesIndex = GetInitialIndex();
        RoamStateIndex = Math.Min((int)_roamer.RoamStatus, RoamStates.Length - 1);
        TimesEncountered = _roamer.TimesEncountered;
    }

    private int GetInitialIndex()
    {
        if (_roamer.Species != 0)
            return Math.Max(0, _roamer.Species - SpeciesOffset);
        
        // Roamer Species is not set if the player hasn't beaten the league so derive the species from the starter choice
        var starterChoice = _sav.EventWork.GetWork(StarterChoiceIndex);
        return Math.Clamp((int)starterChoice, 0, 2);
    }

    [RelayCommand]
    private void Save()
    {
        _roamer.Species = (ushort)(SpeciesOffset + SelectedSpeciesIndex);
        _roamer.RoamStatus = (Roamer6State)RoamStateIndex;
        _roamer.TimesEncountered = TimesEncountered;
    }
}

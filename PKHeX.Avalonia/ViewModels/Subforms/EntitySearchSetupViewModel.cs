using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the Entity Search Setup subform.
/// Provides search filter configuration for species, type, move, and ability filtering.
/// </summary>
public partial class EntitySearchSetupViewModel : ObservableObject
{
    private readonly SaveFile _sav;

    [ObservableProperty]
    private int _selectedSpecies;

    [ObservableProperty]
    private int _selectedMove;

    [ObservableProperty]
    private int _selectedAbility;

    [ObservableProperty]
    private string _instructionText = string.Empty;

    [ObservableProperty]
    private bool _hasResults;

    [ObservableProperty]
    private string _statusText = string.Empty;

    /// <summary>Species list for combo box binding.</summary>
    public ObservableCollection<string> SpeciesList { get; } = [];

    /// <summary>Move list for combo box binding.</summary>
    public ObservableCollection<string> MoveList { get; } = [];

    /// <summary>Ability list for combo box binding.</summary>
    public ObservableCollection<string> AbilityList { get; } = [];

    /// <summary>Callback invoked when the search is executed.</summary>
    public Func<PKM, bool>? SearchFilter { get; private set; }

    /// <summary>Event raised when search is requested.</summary>
    public event EventHandler? SearchRequested;

    /// <summary>Event raised when reset is requested.</summary>
    public event EventHandler? ResetRequested;

    public EntitySearchSetupViewModel(SaveFile sav)
    {
        _sav = sav;
        var strings = GameInfo.Strings;
        foreach (var s in strings.specieslist)
            SpeciesList.Add(s);
        foreach (var m in strings.movelist)
            MoveList.Add(m);
        foreach (var a in strings.abilitylist)
            AbilityList.Add(a);
    }

    /// <summary>
    /// Builds a search filter from the current criteria and fires the search event.
    /// </summary>
    [RelayCommand]
    private void Search()
    {
        var species = (ushort)SelectedSpecies;
        var move = (ushort)SelectedMove;
        var ability = (ushort)SelectedAbility;
        var text = InstructionText;

        SearchFilter = pk =>
        {
            if (species > 0 && pk.Species != species)
                return false;
            if (move > 0 && !HasMove(pk, move))
                return false;
            if (ability > 0 && pk.Ability != ability)
                return false;
            return true;
        };

        HasResults = true;
        StatusText = "Search filter applied.";
        SearchRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Resets all search filters.
    /// </summary>
    [RelayCommand]
    private void Reset()
    {
        SelectedSpecies = 0;
        SelectedMove = 0;
        SelectedAbility = 0;
        InstructionText = string.Empty;
        SearchFilter = null;
        HasResults = false;
        StatusText = string.Empty;
        ResetRequested?.Invoke(this, EventArgs.Empty);
    }

    private static bool HasMove(PKM pk, ushort move)
    {
        return pk.Move1 == move || pk.Move2 == move || pk.Move3 == move || pk.Move4 == move;
    }
}

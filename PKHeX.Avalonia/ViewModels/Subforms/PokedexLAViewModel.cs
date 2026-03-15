using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single Legends Arceus Pokedex species entry.
/// </summary>
public partial class PokedexLAEntryModel : ObservableObject
{
    public ushort Species { get; }
    public ushort DexNumber { get; }
    public string Label { get; }

    [ObservableProperty] private bool _seen;
    [ObservableProperty] private bool _complete;
    [ObservableProperty] private bool _perfect;
    [ObservableProperty] private int _researchLevel;

    public PokedexLAEntryModel(ushort species, ushort dexNumber, string label)
    {
        Species = species;
        DexNumber = dexNumber;
        Label = label;
    }
}

/// <summary>
/// ViewModel for the Legends Arceus Pokedex editor.
/// Provides a simplified species list with seen/complete/perfect status.
/// </summary>
public partial class PokedexLAViewModel : SaveEditorViewModelBase
{
    private readonly SAV8LA _origin;
    private readonly SAV8LA _sav;
    private readonly PokedexSave8a _dex;

    private readonly ushort[] _speciesToDex;
    private readonly ushort[] _dexToSpecies;

    [ObservableProperty] private string _searchText = string.Empty;

    public ObservableCollection<PokedexLAEntryModel> AllEntries { get; } = [];

    [ObservableProperty]
    private ObservableCollection<PokedexLAEntryModel> _filteredEntries = [];

    public PokedexLAViewModel(SAV8LA sav) : base(sav)
    {
        _sav = (SAV8LA)(_origin = sav).Clone();
        _dex = _sav.Blocks.PokedexSave;

        _speciesToDex = new ushort[_sav.Personal.MaxSpeciesID + 1];
        var maxDex = 0;
        for (ushort s = 1; s <= _sav.Personal.MaxSpeciesID; s++)
        {
            var hisuiDex = PokedexSave8a.GetDexIndex(PokedexType8a.Hisui, s);
            if (hisuiDex == 0)
                continue;
            _speciesToDex[s] = hisuiDex;
            if (hisuiDex > maxDex)
                maxDex = hisuiDex;
        }

        _dexToSpecies = new ushort[maxDex + 1];
        for (ushort s = 1; s <= _sav.Personal.MaxSpeciesID; s++)
        {
            if (_speciesToDex[s] != 0)
                _dexToSpecies[_speciesToDex[s]] = s;
        }

        var speciesNames = GameInfo.Strings.Species;
        for (var d = 1; d < _dexToSpecies.Length; d++)
        {
            var species = _dexToSpecies[d];
            var name = species < speciesNames.Count ? speciesNames[species] : $"Species {species}";
            var label = $"{d:000} - {name}";

            var reportedRate = _dex.GetPokeResearchRate(species);
            var entry = new PokedexLAEntryModel(species, (ushort)d, label)
            {
                Seen = _dex.HasPokeEverBeenUpdated(species),
                Complete = _dex.IsComplete(species),
                Perfect = _dex.IsPerfect(species),
                ResearchLevel = reportedRate,
            };
            AllEntries.Add(entry);
        }

        FilteredEntries = new ObservableCollection<PokedexLAEntryModel>(AllEntries);
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    private void ApplyFilter()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            FilteredEntries = new ObservableCollection<PokedexLAEntryModel>(AllEntries);
            return;
        }
        FilteredEntries = new ObservableCollection<PokedexLAEntryModel>(
            AllEntries.Where(e => e.Label.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
    }

    [RelayCommand]
    private void Save()
    {
        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }
}

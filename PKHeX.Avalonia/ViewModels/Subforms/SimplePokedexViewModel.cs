using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single Pokedex species entry.
/// </summary>
public partial class PokedexEntryModel : ObservableObject
{
    public ushort Species { get; }
    public string Label { get; }

    [ObservableProperty]
    private bool _seen;

    [ObservableProperty]
    private bool _caught;

    public PokedexEntryModel(ushort species, string label, bool seen, bool caught)
    {
        Species = species;
        Label = label;
        _seen = seen;
        _caught = caught;
    }
}

/// <summary>
/// ViewModel for the Simple Pokedex editor (Gen 1-5).
/// Edits seen/caught status per species.
/// </summary>
public partial class SimplePokedexViewModel : SaveEditorViewModelBase
{
    private readonly SaveFile _origin;
    private readonly SaveFile _clone;

    [ObservableProperty]
    private string _searchText = string.Empty;

    public ObservableCollection<PokedexEntryModel> AllEntries { get; } = [];

    [ObservableProperty]
    private ObservableCollection<PokedexEntryModel> _filteredEntries = [];

    public SimplePokedexViewModel(SaveFile sav) : base(sav)
    {
        _origin = sav;
        _clone = (SaveFile)sav.Clone();
        var count = sav.MaxSpeciesID;
        var speciesNames = GameInfo.Strings.specieslist;

        for (int i = 0; i < count; i++)
        {
            ushort species = (ushort)(i + 1);
            var name = species < speciesNames.Length ? speciesNames[species] : $"Species {species}";
            var label = $"{species:000} - {name}";
            AllEntries.Add(new PokedexEntryModel(species, label, sav.GetSeen(species), sav.GetCaught(species)));
        }

        FilteredEntries = new ObservableCollection<PokedexEntryModel>(AllEntries);
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            FilteredEntries = new ObservableCollection<PokedexEntryModel>(AllEntries);
            return;
        }

        FilteredEntries = new ObservableCollection<PokedexEntryModel>(
            AllEntries.Where(e => e.Label.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
    }

    [RelayCommand]
    private void SeenAll()
    {
        foreach (var entry in AllEntries)
            entry.Seen = true;
    }

    [RelayCommand]
    private void SeenNone()
    {
        foreach (var entry in AllEntries)
            entry.Seen = false;
    }

    [RelayCommand]
    private void CaughtAll()
    {
        foreach (var entry in AllEntries)
            entry.Caught = true;
    }

    [RelayCommand]
    private void CaughtNone()
    {
        foreach (var entry in AllEntries)
            entry.Caught = false;
    }

    [RelayCommand]
    private void Save()
    {
        foreach (var entry in AllEntries)
        {
            _clone.SetSeen(entry.Species, entry.Seen);
            _clone.SetCaught(entry.Species, entry.Caught);
        }

        // Sanity checks for gen 3
        if (_clone is SAV3 s3)
            s3.MirrorSeenFlags();

        _origin.CopyChangesFrom(_clone);
        Modified = true;
    }
}

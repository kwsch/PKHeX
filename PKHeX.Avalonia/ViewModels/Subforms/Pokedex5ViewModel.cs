using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single Gen 5 Pokedex species entry.
/// </summary>
public partial class Pokedex5EntryModel : ObservableObject
{
    public ushort Species { get; }
    public string Label { get; }

    // Caught
    [ObservableProperty] private bool _caught;

    // Seen: Male, Female, Male Shiny, Female Shiny
    [ObservableProperty] private bool _seenMale;
    [ObservableProperty] private bool _seenFemale;
    [ObservableProperty] private bool _seenMaleShiny;
    [ObservableProperty] private bool _seenFemaleShiny;

    // Displayed: Male, Female, Male Shiny, Female Shiny
    [ObservableProperty] private bool _dispMale;
    [ObservableProperty] private bool _dispFemale;
    [ObservableProperty] private bool _dispMaleShiny;
    [ObservableProperty] private bool _dispFemaleShiny;

    // Language flags (7 languages: JPN, ENG, FRE, ITA, GER, SPA, KOR)
    [ObservableProperty] private bool[] _languages;

    public bool HasLanguages { get; }

    public Pokedex5EntryModel(ushort species, string label, bool hasLanguages)
    {
        Species = species;
        Label = label;
        HasLanguages = hasLanguages;
        _languages = new bool[7];
    }
}

/// <summary>
/// ViewModel for the Gen 5 Pokedex editor.
/// Edits seen/caught/language/form status per species.
/// </summary>
public partial class Pokedex5ViewModel : SaveEditorViewModelBase
{
    private readonly SAV5 SAV5;
    private readonly Zukan5 Dex;
    private const int LangCount = 7;

    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private bool _nationalDexUnlocked;
    [ObservableProperty] private bool _nationalDexActive;
    [ObservableProperty] private string _spindaPid = "00000000";

    public ObservableCollection<Pokedex5EntryModel> AllEntries { get; } = [];

    [ObservableProperty]
    private ObservableCollection<Pokedex5EntryModel> _filteredEntries = [];

    public Pokedex5ViewModel(SAV5 sav) : base(sav)
    {
        SAV5 = sav;
        Dex = sav.Zukan;

        _nationalDexUnlocked = Dex.IsNationalDexUnlocked;
        _nationalDexActive = Dex.IsNationalDexMode;
        _spindaPid = Dex.Spinda.ToString("X8");

        var speciesNames = GameInfo.Strings.specieslist;
        for (ushort i = 1; i <= sav.MaxSpeciesID; i++)
        {
            var name = i < speciesNames.Length ? speciesNames[i] : $"Species {i}";
            var label = $"{i:000} - {name}";
            bool hasLangs = i <= 493; // Language flags only for Gen 1-4 species

            var entry = new Pokedex5EntryModel(i, label, hasLangs)
            {
                Caught = Dex.GetCaught(i),
                SeenMale = Dex.GetSeen(i, 0),
                SeenFemale = Dex.GetSeen(i, 1),
                SeenMaleShiny = Dex.GetSeen(i, 2),
                SeenFemaleShiny = Dex.GetSeen(i, 3),
                DispMale = Dex.GetDisplayed(i, 0),
                DispFemale = Dex.GetDisplayed(i, 1),
                DispMaleShiny = Dex.GetDisplayed(i, 2),
                DispFemaleShiny = Dex.GetDisplayed(i, 3),
            };

            if (hasLangs)
            {
                var langs = new bool[LangCount];
                for (int l = 0; l < LangCount; l++)
                    langs[l] = Dex.GetLanguageFlag(i, l);
                entry.Languages = langs;
            }

            AllEntries.Add(entry);
        }

        FilteredEntries = new ObservableCollection<Pokedex5EntryModel>(AllEntries);
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    private void ApplyFilter()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            FilteredEntries = new ObservableCollection<Pokedex5EntryModel>(AllEntries);
            return;
        }
        FilteredEntries = new ObservableCollection<Pokedex5EntryModel>(
            AllEntries.Where(e => e.Label.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
    }

    [RelayCommand]
    private void SeenAll()
    {
        foreach (var entry in AllEntries)
        {
            entry.SeenMale = true;
            entry.SeenFemale = true;
            if (!entry.DispMale && !entry.DispFemale && !entry.DispMaleShiny && !entry.DispFemaleShiny)
                entry.DispMale = true;
        }
    }

    [RelayCommand]
    private void CaughtAll()
    {
        foreach (var entry in AllEntries)
        {
            entry.Caught = true;
            if (entry.HasLanguages)
            {
                var langs = new bool[LangCount];
                for (int i = 0; i < LangCount; i++)
                    langs[i] = true;
                entry.Languages = langs;
            }
        }
    }

    [RelayCommand]
    private void ClearAll()
    {
        foreach (var entry in AllEntries)
        {
            entry.Caught = false;
            entry.SeenMale = false;
            entry.SeenFemale = false;
            entry.SeenMaleShiny = false;
            entry.SeenFemaleShiny = false;
            entry.DispMale = false;
            entry.DispFemale = false;
            entry.DispMaleShiny = false;
            entry.DispFemaleShiny = false;
            if (entry.HasLanguages)
                entry.Languages = new bool[LangCount];
        }
    }

    [RelayCommand]
    private void CompleteAll()
    {
        SeenAll();
        CaughtAll();
    }

    [RelayCommand]
    private void Save()
    {
        foreach (var entry in AllEntries)
        {
            var species = entry.Species;
            Dex.SetCaught(species, entry.Caught);
            Dex.SetSeen(species, 0, entry.SeenMale);
            Dex.SetSeen(species, 1, entry.SeenFemale);
            Dex.SetSeen(species, 2, entry.SeenMaleShiny);
            Dex.SetSeen(species, 3, entry.SeenFemaleShiny);
            Dex.SetDisplayed(species, 0, entry.DispMale);
            Dex.SetDisplayed(species, 1, entry.DispFemale);
            Dex.SetDisplayed(species, 2, entry.DispMaleShiny);
            Dex.SetDisplayed(species, 3, entry.DispFemaleShiny);

            if (entry.HasLanguages)
            {
                for (int l = 0; l < LangCount; l++)
                    Dex.SetLanguageFlag(species, l, entry.Languages[l]);
            }
        }

        Dex.IsNationalDexUnlocked = NationalDexUnlocked;
        Dex.IsNationalDexMode = NationalDexActive;
        if (uint.TryParse(SpindaPid, System.Globalization.NumberStyles.HexNumber, null, out var spinda))
            Dex.Spinda = spinda;

        SAV.State.Edited = true;
        Modified = true;
    }
}

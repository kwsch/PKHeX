using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single form flag entry in the Gen 5 Pokedex.
/// Each entry represents one form in one of the 4 regions (Seen, SeenShiny, Displayed, DisplayedShiny).
/// </summary>
public partial class Pokedex5FormFlagModel : ObservableObject
{
    public string Label { get; }
    public int FormIndex { get; }
    public int Region { get; }

    [ObservableProperty] private bool _isChecked;

    public Pokedex5FormFlagModel(string label, int formIndex, int region, bool isChecked)
    {
        Label = label;
        FormIndex = formIndex;
        Region = region;
        _isChecked = isChecked;
    }
}

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

    /// <summary>Whether this species has alternate form data in the dex.</summary>
    public bool HasForms { get; }

    /// <summary>Form index start (from GetFormIndex).</summary>
    public byte FormIndexStart { get; }

    /// <summary>Number of forms in the dex.</summary>
    public byte FormCount { get; }

    /// <summary>
    /// Form seen flags (non-shiny + shiny interleaved).
    /// First [FormCount] entries are non-shiny seen, next [FormCount] are shiny seen.
    /// </summary>
    public ObservableCollection<Pokedex5FormFlagModel> FormsSeen { get; } = [];

    /// <summary>
    /// Form displayed flags (non-shiny + shiny interleaved).
    /// First [FormCount] entries are non-shiny displayed, next [FormCount] are shiny displayed.
    /// </summary>
    public ObservableCollection<Pokedex5FormFlagModel> FormsDisplayed { get; } = [];

    public Pokedex5EntryModel(ushort species, string label, bool hasLanguages, bool hasForms, byte formIndexStart, byte formCount)
    {
        Species = species;
        Label = label;
        HasLanguages = hasLanguages;
        HasForms = hasForms;
        FormIndexStart = formIndexStart;
        FormCount = formCount;
        _languages = new bool[7];
    }
}

/// <summary>
/// ViewModel for the Gen 5 Pokedex editor.
/// Edits seen/caught/language/form status per species.
/// </summary>
public partial class Pokedex5ViewModel : SaveEditorViewModelBase
{
    private readonly SAV5 _origin;
    private readonly SAV5 SAV5;
    private readonly Zukan5 Dex;
    private const int LangCount = 7;

    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private bool _nationalDexUnlocked;
    [ObservableProperty] private bool _nationalDexActive;
    [ObservableProperty] private string _spindaPid = "00000000";

    /// <summary>
    /// The currently selected entry for detail editing (forms).
    /// </summary>
    [ObservableProperty]
    private Pokedex5EntryModel? _selectedEntry;

    public ObservableCollection<Pokedex5EntryModel> AllEntries { get; } = [];

    [ObservableProperty]
    private ObservableCollection<Pokedex5EntryModel> _filteredEntries = [];

    public Pokedex5ViewModel(SAV5 sav) : base(sav)
    {
        _origin = sav;
        SAV5 = (SAV5)sav.Clone();
        Dex = SAV5.Zukan;

        _nationalDexUnlocked = Dex.IsNationalDexUnlocked;
        _nationalDexActive = Dex.IsNationalDexMode;
        _spindaPid = Dex.Spinda.ToString("X8");

        var speciesNames = GameInfo.Strings.specieslist;
        for (ushort i = 1; i <= sav.MaxSpeciesID; i++)
        {
            var name = i < speciesNames.Length ? speciesNames[i] : $"Species {i}";
            var label = $"{i:000} - {name}";
            bool hasLangs = i <= 493; // Language flags only for Gen 1-4 species

            var (formIndex, formCount) = Dex.GetFormIndex(i);
            bool hasForms = formCount > 0;

            var entry = new Pokedex5EntryModel(i, label, hasLangs, hasForms, formIndex, formCount)
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

            // Load form flags
            if (hasForms)
                LoadFormFlags(entry, i, formIndex, formCount);

            AllEntries.Add(entry);
        }

        FilteredEntries = new ObservableCollection<Pokedex5EntryModel>(AllEntries);
    }

    private void LoadFormFlags(Pokedex5EntryModel entry, ushort species, byte formIndex, byte formCount)
    {
        var forms = FormConverter.GetFormList(species, GameInfo.Strings.types, GameInfo.Strings.forms, SAV5.Context);
        if (forms.Length < 1)
            return;

        // Seen: non-shiny forms (region 0), then shiny forms (region 1)
        for (int i = 0; i < forms.Length; i++)
        {
            bool val = Dex.GetFormFlag(formIndex + i, 0);
            entry.FormsSeen.Add(new Pokedex5FormFlagModel(forms[i], formIndex + i, 0, val));
        }
        for (int i = 0; i < forms.Length; i++)
        {
            bool val = Dex.GetFormFlag(formIndex + i, 1);
            entry.FormsSeen.Add(new Pokedex5FormFlagModel($"* {forms[i]}", formIndex + i, 1, val));
        }

        // Displayed: non-shiny forms (region 2), then shiny forms (region 3)
        for (int i = 0; i < forms.Length; i++)
        {
            bool val = Dex.GetFormFlag(formIndex + i, 2);
            entry.FormsDisplayed.Add(new Pokedex5FormFlagModel(forms[i], formIndex + i, 2, val));
        }
        for (int i = 0; i < forms.Length; i++)
        {
            bool val = Dex.GetFormFlag(formIndex + i, 3);
            entry.FormsDisplayed.Add(new Pokedex5FormFlagModel($"* {forms[i]}", formIndex + i, 3, val));
        }
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

            // Clear form flags
            foreach (var f in entry.FormsSeen)
                f.IsChecked = false;
            foreach (var f in entry.FormsDisplayed)
                f.IsChecked = false;
        }
    }

    [RelayCommand]
    private void CompleteAll()
    {
        SeenAll();
        CaughtAll();

        // Complete all form flags (seen non-shiny for each species)
        foreach (var entry in AllEntries)
        {
            if (!entry.HasForms)
                continue;

            int half = entry.FormsSeen.Count / 2;
            // Set all non-shiny seen forms
            for (int i = 0; i < half; i++)
                entry.FormsSeen[i].IsChecked = true;

            // Ensure at least one displayed form is set
            bool hasDisplayed = entry.FormsDisplayed.Any(f => f.IsChecked);
            if (!hasDisplayed && entry.FormsDisplayed.Count > 0)
                entry.FormsDisplayed[0].IsChecked = true;
        }
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

            // Save form flags
            if (entry.HasForms)
            {
                int half = entry.FormsSeen.Count / 2;
                for (int i = 0; i < half; i++)
                    Dex.SetFormFlag(entry.FormsSeen[i].FormIndex, 0, entry.FormsSeen[i].IsChecked);
                for (int i = 0; i < half; i++)
                    Dex.SetFormFlag(entry.FormsSeen[half + i].FormIndex, 1, entry.FormsSeen[half + i].IsChecked);

                int halfDisp = entry.FormsDisplayed.Count / 2;
                for (int i = 0; i < halfDisp; i++)
                    Dex.SetFormFlag(entry.FormsDisplayed[i].FormIndex, 2, entry.FormsDisplayed[i].IsChecked);
                for (int i = 0; i < halfDisp; i++)
                    Dex.SetFormFlag(entry.FormsDisplayed[halfDisp + i].FormIndex, 3, entry.FormsDisplayed[halfDisp + i].IsChecked);
            }
        }

        // Save InitialSpecies from the selected entry (or first caught species)
        if (SelectedEntry is { Species: not 0 })
            Dex.InitialSpecies = SelectedEntry.Species;

        Dex.IsNationalDexUnlocked = NationalDexUnlocked;
        Dex.IsNationalDexMode = NationalDexActive;
        if (uint.TryParse(SpindaPid, System.Globalization.NumberStyles.HexNumber, null, out var spinda))
            Dex.Spinda = spinda;

        _origin.CopyChangesFrom(SAV5);
        Modified = true;
    }
}

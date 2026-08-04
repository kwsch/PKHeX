using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class Pokedex6EditorViewModel : ViewModelBase
{
    private readonly Zukan6 _zukan;
    private readonly SAV6 _sav;
    
    public Pokedex6EditorViewModel(SAV6 sav)
    {
        _sav = sav;
        _zukan = sav is SAV6XY xy ? xy.Zukan : ((SAV6AO)sav).Zukan;
        IsXY = sav is SAV6XY;
        IsORAS = !IsXY;

        var species = GameInfo.Strings.Species;
        _allSpecies = Enumerable.Range(1, _sav.MaxSpeciesID)
            .Select(i => new ComboItem(species[i], i))
            .ToList();
        _filteredSpecies = new ObservableCollection<ComboItem>(_allSpecies);

        // Initialize Global Settings
        NationalDexUnlocked = _zukan.IsNationalDexUnlocked;
        NationalDexMode = _zukan.IsNationalDexMode;
        Spinda = _zukan.Spinda;

        // Select initial
        SelectedSpecies = _zukan.InitialSpecies > 0 && _zukan.InitialSpecies <= _sav.MaxSpeciesID 
            ? _allSpecies[_zukan.InitialSpecies - 1] 
            : _allSpecies[0];
    }

    public bool IsXY { get; }
    public bool IsORAS { get; }

    private readonly List<ComboItem> _allSpecies;
    
    [ObservableProperty]
    private ObservableCollection<ComboItem> _filteredSpecies;

    [ObservableProperty]
    private string _searchText = "";

    partial void OnSearchTextChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            FilteredSpecies = new ObservableCollection<ComboItem>(_allSpecies);
        }
        else
        {
            FilteredSpecies = new ObservableCollection<ComboItem>(
                _allSpecies.Where(x => x.Text.Contains(value, StringComparison.OrdinalIgnoreCase)));
        }
    }

    [ObservableProperty]
    private ComboItem _selectedSpecies;

    partial void OnSelectedSpeciesChanged(ComboItem? oldValue, ComboItem newValue)
    {
        if (oldValue != null)
            SaveEntry(oldValue.Value);
        
        if (newValue != null)
            LoadEntry(newValue.Value);
    }
    
    // Global Settings
    [ObservableProperty] private bool _nationalDexUnlocked;
    [ObservableProperty] private bool _nationalDexMode;
    [ObservableProperty] private uint _spinda;

    partial void OnNationalDexUnlockedChanged(bool value) => _zukan.IsNationalDexUnlocked = value;
    partial void OnNationalDexModeChanged(bool value) => _zukan.IsNationalDexMode = value;
    partial void OnSpindaChanged(uint value) => _zukan.Spinda = value;

    // Entry Properties
    [ObservableProperty] private bool _caught;
    
    // Seen Flags
    [ObservableProperty] private bool _seenMale;
    [ObservableProperty] private bool _seenFemale;
    [ObservableProperty] private bool _seenShinyMale;
    [ObservableProperty] private bool _seenShinyFemale;

    // Displayed Flags
    [ObservableProperty] private bool _displayedMale;
    [ObservableProperty] private bool _displayedFemale;
    [ObservableProperty] private bool _displayedShinyMale;
    [ObservableProperty] private bool _displayedShinyFemale;

    // Language Flags
    [ObservableProperty] private bool _langJapanese;
    [ObservableProperty] private bool _langEnglish;
    [ObservableProperty] private bool _langFrench;
    [ObservableProperty] private bool _langItalian;
    [ObservableProperty] private bool _langGerman;
    [ObservableProperty] private bool _langSpanish;
    [ObservableProperty] private bool _langKorean;

    // XY Specific
    [ObservableProperty] private bool _foreign;
    public bool HasForeign => IsXY && SelectedSpecies.Value < (int)Species.Genesect;

    // ORAS Specific
    [ObservableProperty] private ushort _seenCount;
    [ObservableProperty] private ushort _obtainedCount;

    // Forms
    [ObservableProperty]
    private ObservableCollection<PokedexFormViewModel> _forms = [];

    // UI State
    [ObservableProperty] private bool _canBeFemale;
    [ObservableProperty] private bool _canBeMale;

    private void LoadEntry(int species)
    {
        var uspecies = (ushort)species;
        
        Caught = _zukan.GetCaught(uspecies);
        
        SeenMale = _zukan.GetSeen(uspecies, 0);
        SeenFemale = _zukan.GetSeen(uspecies, 1);
        SeenShinyMale = _zukan.GetSeen(uspecies, 2);
        SeenShinyFemale = _zukan.GetSeen(uspecies, 3);

        DisplayedMale = _zukan.GetDisplayed(uspecies, 0);
        DisplayedFemale = _zukan.GetDisplayed(uspecies, 1);
        DisplayedShinyMale = _zukan.GetDisplayed(uspecies, 2);
        DisplayedShinyFemale = _zukan.GetDisplayed(uspecies, 3);

        LangJapanese = _zukan.GetLanguageFlag(uspecies, 0);
        LangEnglish = _zukan.GetLanguageFlag(uspecies, 1);
        LangFrench = _zukan.GetLanguageFlag(uspecies, 2);
        LangItalian = _zukan.GetLanguageFlag(uspecies, 3);
        LangGerman = _zukan.GetLanguageFlag(uspecies, 4);
        LangSpanish = _zukan.GetLanguageFlag(uspecies, 5);
        LangKorean = _zukan.GetLanguageFlag(uspecies, 6);

        if (IsXY && _zukan is Zukan6XY xy)
        {
            Foreign = HasForeign && xy.GetForeignFlag(uspecies);
        }
        else if (IsORAS && _zukan is Zukan6AO ao)
        {
            SeenCount = ao.GetCountSeen(uspecies);
            ObtainedCount = ao.GetCountObtained(uspecies);
        }

        // Gender constraints
        var pi = _sav.Personal[uspecies];
        CanBeFemale = !pi.OnlyMale && !pi.Genderless;
        CanBeMale = !pi.OnlyFemale; // Genderless treated as male-ish for flags (Region 0/2)

        LoadForms(uspecies);
        OnPropertyChanged(nameof(HasForeign));
    }

    private void SaveEntry(int species)
    {
        var uspecies = (ushort)species;
        
        _zukan.SetCaught(uspecies, Caught);
        
        _zukan.SetSeen(uspecies, 0, SeenMale);
        _zukan.SetSeen(uspecies, 1, SeenFemale);
        _zukan.SetSeen(uspecies, 2, SeenShinyMale);
        _zukan.SetSeen(uspecies, 3, SeenShinyFemale);

        _zukan.SetDisplayed(uspecies, 0, DisplayedMale);
        _zukan.SetDisplayed(uspecies, 1, DisplayedFemale);
        _zukan.SetDisplayed(uspecies, 2, DisplayedShinyMale);
        _zukan.SetDisplayed(uspecies, 3, DisplayedShinyFemale);

        _zukan.SetLanguageFlag(uspecies, 0, LangJapanese);
        _zukan.SetLanguageFlag(uspecies, 1, LangEnglish);
        _zukan.SetLanguageFlag(uspecies, 2, LangFrench);
        _zukan.SetLanguageFlag(uspecies, 3, LangItalian);
        _zukan.SetLanguageFlag(uspecies, 4, LangGerman);
        _zukan.SetLanguageFlag(uspecies, 5, LangSpanish);
        _zukan.SetLanguageFlag(uspecies, 6, LangKorean);

        if (IsXY && _zukan is Zukan6XY xy && HasForeign)
        {
            xy.SetForeignFlag(uspecies, Foreign);
        }
        else if (IsORAS && _zukan is Zukan6AO ao)
        {
            ao.SetCountSeen(uspecies, SeenCount);
            ao.SetCountObtained(uspecies, ObtainedCount);
        }

        SaveForms(uspecies);
        
        // Update Initial Species preference
        if (species != 0)
            _zukan.InitialSpecies = uspecies;
    }

    private void LoadForms(ushort species)
    {
        Forms.Clear();
        var (index, count) = _zukan.GetFormIndex(species);
        if (count <= 0) return;

        var formNames = FormConverter.GetFormList(species, GameInfo.Strings.Types, GameInfo.Strings.forms, GameInfo.GenderSymbolASCII, _sav.Context);
        
        // Safety check for length mismatch, though Core should handle it.
        // If formNames length != count, typically FormConverter returns count strings?
        // FormConverter.GetFormList logic can be complex.
        // WinForms loop: i < forms.Length. 
        // We trust count from Zukan matches or is close. 
        // Zukan.GetFormIndex returns the START index in the blob and the COUNT.
        // FormConverter returns names.
        
        int limit = Math.Min(count, formNames.Length);

        for (int i = 0; i < limit; i++)
        {
            int formIndex = index + i;
            bool seen = _zukan.GetFormFlag(formIndex, 0);
            bool seenShiny = _zukan.GetFormFlag(formIndex, 1);
            bool disp = _zukan.GetFormFlag(formIndex, 2);
            bool dispShiny = _zukan.GetFormFlag(formIndex, 3);
            
            Forms.Add(new PokedexFormViewModel(formNames[i], seen, seenShiny, disp, dispShiny));
        }
    }

    private void SaveForms(ushort species)
    {
        var (index, count) = _zukan.GetFormIndex(species);
        if (count <= 0) return;
        
        for (int i = 0; i < Forms.Count; i++)
        {
            int formIndex = index + i;
            var f = Forms[i];
            _zukan.SetFormFlag(formIndex, 0, f.Seen);
            _zukan.SetFormFlag(formIndex, 1, f.SeenShiny);
            _zukan.SetFormFlag(formIndex, 2, f.Displayed);
            _zukan.SetFormFlag(formIndex, 3, f.DisplayedShiny);
        }
    }
    
    public void SaveCurrent()
    {
        if (SelectedSpecies != null)
            SaveEntry(SelectedSpecies.Value);
    }
}

public partial class PokedexFormViewModel : ViewModelBase
{
    public string Name { get; }
    
    [ObservableProperty] private bool _seen;
    [ObservableProperty] private bool _seenShiny;
    [ObservableProperty] private bool _displayed;
    [ObservableProperty] private bool _displayedShiny;

    public PokedexFormViewModel(string name, bool seen, bool seenShiny, bool disp, bool dispShiny)
    {
        Name = name;
        Seen = seen;
        SeenShiny = seenShiny;
        Displayed = disp;
        DisplayedShiny = dispShiny;
    }
}

using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class Pokedex5EditorViewModel : ViewModelBase
{
    private readonly SAV5? _sav;
    private readonly Zukan5? _zukan;
    
    public Pokedex5EditorViewModel(SaveFile sav)
    {
        _sav = sav as SAV5;
        _zukan = _sav?.Zukan;
        IsSupported = _sav is not null && _zukan is not null;
        
        if (IsSupported)
        {
            var speciesList = GameInfo.Strings.Species;
            Species = new ObservableCollection<ComboItem>(
                Enumerable.Range(1, _sav!.MaxSpeciesID)
                    .Select(i => new ComboItem(speciesList[i], i)));
            
            _selectedSpecies = Species.FirstOrDefault(s => s.Value == 1) ?? Species.FirstOrDefault();
            if (_selectedSpecies != null)
                LoadEntry((ushort)_selectedSpecies.Value);
        }
    }

    public bool IsSupported { get; }
    public ObservableCollection<ComboItem> Species { get; } = [];
    
    [ObservableProperty]
    private ComboItem? _selectedSpecies;

    [ObservableProperty]
    private string _searchText = "";

    partial void OnSearchTextChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return;
        var match = Species.FirstOrDefault(s => s.Text.Contains(value, StringComparison.OrdinalIgnoreCase));
        if (match != null)
            SelectedSpecies = match;
    }

    partial void OnSelectedSpeciesChanged(ComboItem? value)
    {
        if (value is not null)
            LoadEntry((ushort)value.Value);
    }
    
    // Global Settings
    [ObservableProperty] private bool _nationalDexUnlocked;
    [ObservableProperty] private bool _nationalDexMode;
    [ObservableProperty] private string _spindaPID = "00000000";

    partial void OnNationalDexUnlockedChanged(bool value) 
    {
        if (_zukan is not null) _zukan.IsNationalDexUnlocked = value;
        if (!value) NationalDexMode = false;
    }
    partial void OnNationalDexModeChanged(bool value) 
    { 
        if (_zukan is not null) _zukan.IsNationalDexMode = value; 
    }
    partial void OnSpindaPIDChanged(string value)
    {
        if (_zukan is not null && uint.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out var pid))
            _zukan.Spinda = pid;
    }

    // Entry Data
    [ObservableProperty] private bool _caught;
    
    // Seen flags (Male, Female, Male Shiny, Female Shiny)
    [ObservableProperty] private bool _seenMale;
    [ObservableProperty] private bool _seenFemale;
    [ObservableProperty] private bool _seenMaleShiny;
    [ObservableProperty] private bool _seenFemaleShiny;
    
    // Displayed flags
    [ObservableProperty] private bool _displayedMale;
    [ObservableProperty] private bool _displayedFemale;
    [ObservableProperty] private bool _displayedMaleShiny;
    [ObservableProperty] private bool _displayedFemaleShiny;
    
    // Gender availability
    [ObservableProperty] private bool _canBeMale;
    [ObservableProperty] private bool _canBeFemale;
    
    // Language flags (only for species 1-493)
    public bool HasLanguages => SelectedSpecies != null && SelectedSpecies.Value <= 493;
    
    [ObservableProperty] private bool _langJPN;
    [ObservableProperty] private bool _langENG;
    [ObservableProperty] private bool _langFRA;
    [ObservableProperty] private bool _langGER;
    [ObservableProperty] private bool _langITA;
    [ObservableProperty] private bool _langSPA;
    [ObservableProperty] private bool _langKOR;
    
    // Forms
    public bool HasForms => FormsSeen.Count > 0;
    public ObservableCollection<FormFlagViewModel> FormsSeen { get; } = [];
    public ObservableCollection<FormFlagViewModel> FormsDisplayed { get; } = [];

    private void LoadEntry(ushort species)
    {
        if (_zukan is null || _sav is null) return;
        
        // Global settings (only load once, not per species)
        NationalDexUnlocked = _zukan.IsNationalDexUnlocked;
        NationalDexMode = _zukan.IsNationalDexMode;
        SpindaPID = _zukan.Spinda.ToString("X8");
        
        Caught = _zukan.GetCaught(species);
        
        // Seen: regions 0=Male, 1=Female, 2=MaleShiny, 3=FemaleShiny
        SeenMale = _zukan.GetSeen(species, 0);
        SeenFemale = _zukan.GetSeen(species, 1);
        SeenMaleShiny = _zukan.GetSeen(species, 2);
        SeenFemaleShiny = _zukan.GetSeen(species, 3);
        
        // Displayed
        DisplayedMale = _zukan.GetDisplayed(species, 0);
        DisplayedFemale = _zukan.GetDisplayed(species, 1);
        DisplayedMaleShiny = _zukan.GetDisplayed(species, 2);
        DisplayedFemaleShiny = _zukan.GetDisplayed(species, 3);
        
        // Gender availability
        var pi = _sav.Personal[species];
        CanBeMale = !pi.OnlyFemale;
        CanBeFemale = !(pi.OnlyMale || pi.Genderless);
        
        // Languages
        LoadLanguages(species);
        
        // Forms
        LoadForms(species);
        
        OnPropertyChanged(nameof(HasLanguages));
        OnPropertyChanged(nameof(HasForms));
    }
    
    private void LoadLanguages(ushort species)
    {
        if (_zukan is null) return;
        
        if (species <= 493)
        {
            LangJPN = _zukan.GetLanguageFlag(species, 0);
            LangENG = _zukan.GetLanguageFlag(species, 1);
            LangFRA = _zukan.GetLanguageFlag(species, 2);
            LangGER = _zukan.GetLanguageFlag(species, 3);
            LangITA = _zukan.GetLanguageFlag(species, 4);
            LangSPA = _zukan.GetLanguageFlag(species, 5);
            LangKOR = _zukan.GetLanguageFlag(species, 6);
        }
        else
        {
            LangJPN = LangENG = LangFRA = LangGER = LangITA = LangSPA = LangKOR = false;
        }
    }
    
    private void LoadForms(ushort species)
    {
        FormsSeen.Clear();
        FormsDisplayed.Clear();
        
        if (_zukan is null || _sav is null) return;
        
        var (index, count) = _zukan.GetFormIndex(species);
        if (count == 0) return;
        
        var formNames = FormConverter.GetFormList(species, GameInfo.Strings.types, GameInfo.Strings.forms, GameInfo.GenderSymbolUnicode, _sav.Context);
        if (formNames.Length < 1) return;
        
        for (int i = 0; i < formNames.Length; i++)
        {
            FormsSeen.Add(new FormFlagViewModel(formNames[i], false, _zukan.GetFormFlag(index + i, 0)));
            FormsSeen.Add(new FormFlagViewModel(formNames[i], true, _zukan.GetFormFlag(index + i, 1)));
            
            FormsDisplayed.Add(new FormFlagViewModel(formNames[i], false, _zukan.GetFormFlag(index + i, 2)));
            FormsDisplayed.Add(new FormFlagViewModel(formNames[i], true, _zukan.GetFormFlag(index + i, 3)));
        }
    }
    
    [RelayCommand]
    private void Save()
    {
        if (_zukan is null || _sav is null || SelectedSpecies is null) return;
        
        var species = (ushort)SelectedSpecies.Value;
        
        _zukan.SetCaught(species, Caught);
        
        _zukan.SetSeen(species, 0, SeenMale);
        _zukan.SetSeen(species, 1, SeenFemale);
        _zukan.SetSeen(species, 2, SeenMaleShiny);
        _zukan.SetSeen(species, 3, SeenFemaleShiny);
        
        _zukan.SetDisplayed(species, 0, DisplayedMale);
        _zukan.SetDisplayed(species, 1, DisplayedFemale);
        _zukan.SetDisplayed(species, 2, DisplayedMaleShiny);
        _zukan.SetDisplayed(species, 3, DisplayedFemaleShiny);
        
        // Languages
        if (species <= 493)
        {
            _zukan.SetLanguageFlag(species, 0, LangJPN);
            _zukan.SetLanguageFlag(species, 1, LangENG);
            _zukan.SetLanguageFlag(species, 2, LangFRA);
            _zukan.SetLanguageFlag(species, 3, LangGER);
            _zukan.SetLanguageFlag(species, 4, LangITA);
            _zukan.SetLanguageFlag(species, 5, LangSPA);
            _zukan.SetLanguageFlag(species, 6, LangKOR);
        }
        
        // Forms
        var (index, count) = _zukan.GetFormIndex(species);
        if (count > 0)
        {
            int formCount = FormsSeen.Count / 2;
            for (int i = 0; i < formCount; i++)
            {
                _zukan.SetFormFlag(index + i, 0, FormsSeen[i].IsChecked); // Seen
                _zukan.SetFormFlag(index + i, 1, FormsSeen[i + formCount].IsChecked); // Seen Shiny
                _zukan.SetFormFlag(index + i, 2, FormsDisplayed[i].IsChecked); // Displayed
                _zukan.SetFormFlag(index + i, 3, FormsDisplayed[i + formCount].IsChecked); // Displayed Shiny
            }
        }
        
        _sav.State.Edited = true;
    }
}

public partial class FormFlagViewModel : ObservableObject
{
    public FormFlagViewModel(string name, bool isShiny, bool isChecked)
    {
        Name = isShiny ? $"★ {name}" : name;
        IsShiny = isShiny;
        _isChecked = isChecked;
    }
    
    public string Name { get; }
    public bool IsShiny { get; }
    
    [ObservableProperty]
    private bool _isChecked;
}

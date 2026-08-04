using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class Pokedex7EditorViewModel : ViewModelBase
{
    private readonly SAV7? _sav;
    private readonly Zukan7? _zukan;
    
    public Pokedex7EditorViewModel(SaveFile sav)
    {
        _sav = sav as SAV7;
        _zukan = _sav?.Zukan as Zukan7;
        IsSupported = _sav is not null && _zukan is not null;
        
        if (IsSupported)
        {
            var speciesNames = GameInfo.Strings.Species;
            var names = _zukan!.GetEntryNames(speciesNames);
            Entries = new ObservableCollection<ComboItem>(
                names.Select((n, i) => new ComboItem(n, i)));
            
            _selectedEntry = Entries.FirstOrDefault();
            if (_selectedEntry != null)
                LoadEntry(_selectedEntry.Value);
        }
    }

    public bool IsSupported { get; }
    public ObservableCollection<ComboItem> Entries { get; } = [];
    
    [ObservableProperty]
    private ComboItem? _selectedEntry;

    [ObservableProperty]
    private string _searchText = "";

    partial void OnSearchTextChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return;
        var match = Entries.FirstOrDefault(s => s.Text.Contains(value, StringComparison.OrdinalIgnoreCase));
        if (match != null)
            SelectedEntry = match;
    }

    partial void OnSelectedEntryChanged(ComboItem? value)
    {
        if (value is not null)
            LoadEntry(value.Value);
    }
    
    // Entry Data
    [ObservableProperty] private bool _caught;
    [ObservableProperty] private bool _isCaughtEnabled;
    
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
    
    // Language flags (9 languages in Gen 7)
    public bool HasLanguages => _selectedIndex <= _sav!.MaxSpeciesID;
    
    [ObservableProperty] private bool _langJPN;
    [ObservableProperty] private bool _langENG;
    [ObservableProperty] private bool _langFRA;
    [ObservableProperty] private bool _langGER;
    [ObservableProperty] private bool _langITA;
    [ObservableProperty] private bool _langSPA;
    [ObservableProperty] private bool _langKOR;
    [ObservableProperty] private bool _langCHT;
    [ObservableProperty] private bool _langCHS;
    
    private int _selectedIndex = -1;

    private void LoadEntry(int index)
    {
        if (_zukan is null || _sav is null) return;
        
        _selectedIndex = index;
        var species = (ushort)(index + 1);
        bool isSpeciesEntry = species <= _sav.MaxSpeciesID;
        
        IsCaughtEnabled = isSpeciesEntry;
        Caught = isSpeciesEntry && _zukan.GetCaught(species);
        
        // Gender availability
        var gt = _zukan.GetBaseSpeciesGenderValue(index);
        CanBeMale = gt != PersonalInfo.RatioMagicFemale;
        CanBeFemale = gt is not (PersonalInfo.RatioMagicMale or PersonalInfo.RatioMagicGenderless);
        
        // Seen: regions 0=Male, 1=Female, 2=MaleShiny, 3=FemaleShiny
        SeenMale = _zukan.GetSeen(species, 0);
        SeenFemale = _zukan.GetSeen(species, 1);
        SeenMaleShiny = _zukan.GetSeen(species, 2);
        SeenFemaleShiny = _zukan.GetSeen(species, 3);
        
        // Displayed
        DisplayedMale = _zukan.GetDisplayed(index, 0);
        DisplayedFemale = _zukan.GetDisplayed(index, 1);
        DisplayedMaleShiny = _zukan.GetDisplayed(index, 2);
        DisplayedFemaleShiny = _zukan.GetDisplayed(index, 3);
        
        // Languages
        LoadLanguages(index, isSpeciesEntry);
        
        OnPropertyChanged(nameof(HasLanguages));
    }
    
    private void LoadLanguages(int index, bool isSpeciesEntry)
    {
        if (_zukan is null || !isSpeciesEntry)
        {
            LangJPN = LangENG = LangFRA = LangGER = LangITA = LangSPA = LangKOR = LangCHT = LangCHS = false;
            return;
        }
        
        LangJPN = _zukan.GetLanguageFlag(index, 0);
        LangENG = _zukan.GetLanguageFlag(index, 1);
        LangFRA = _zukan.GetLanguageFlag(index, 2);
        LangGER = _zukan.GetLanguageFlag(index, 3);
        LangITA = _zukan.GetLanguageFlag(index, 4);
        LangSPA = _zukan.GetLanguageFlag(index, 5);
        LangKOR = _zukan.GetLanguageFlag(index, 6);
        LangCHT = _zukan.GetLanguageFlag(index, 7);
        LangCHS = _zukan.GetLanguageFlag(index, 8);
    }
    
    [RelayCommand]
    private void Save()
    {
        if (_zukan is null || _sav is null || _selectedIndex < 0) return;
        
        var index = _selectedIndex;
        var species = (ushort)(index + 1);
        bool isSpeciesEntry = species <= _sav.MaxSpeciesID;
        
        _zukan.SetSeen(species, 0, SeenMale);
        _zukan.SetSeen(species, 1, SeenFemale);
        _zukan.SetSeen(species, 2, SeenMaleShiny);
        _zukan.SetSeen(species, 3, SeenFemaleShiny);
        
        _zukan.SetDisplayed(index, 0, DisplayedMale);
        _zukan.SetDisplayed(index, 1, DisplayedFemale);
        _zukan.SetDisplayed(index, 2, DisplayedMaleShiny);
        _zukan.SetDisplayed(index, 3, DisplayedFemaleShiny);
        
        if (!isSpeciesEntry)
        {
            _sav.State.Edited = true;
            return;
        }
        
        _zukan.SetCaught(species, Caught);
        
        // Languages
        _zukan.SetLanguageFlag(index, 0, LangJPN);
        _zukan.SetLanguageFlag(index, 1, LangENG);
        _zukan.SetLanguageFlag(index, 2, LangFRA);
        _zukan.SetLanguageFlag(index, 3, LangGER);
        _zukan.SetLanguageFlag(index, 4, LangITA);
        _zukan.SetLanguageFlag(index, 5, LangSPA);
        _zukan.SetLanguageFlag(index, 6, LangKOR);
        _zukan.SetLanguageFlag(index, 7, LangCHT);
        _zukan.SetLanguageFlag(index, 8, LangCHS);
        
        _sav.State.Edited = true;
    }
}

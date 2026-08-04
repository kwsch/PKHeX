using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class Pokedex8EditorViewModel : ViewModelBase
{
    private readonly SAV8SWSH _sav;
    private readonly Zukan8 _zukan;
    private readonly List<Zukan8EntryInfo> _allEntries;

    public Pokedex8EditorViewModel(SAV8SWSH sav)
    {
        _sav = sav;
        _zukan = sav.Blocks.Zukan;

        // Load Indexes
        // Zukan8.GetRawIndexes might be static or instance? WinForms calls it static.
        // It requires PersonalTable.SWSH.
        // If PersonalTable.SWSH is not directly accessible, we might need GameInfo or similar.
        // Assuming PersonalTable.SWSH is available as per WinForms code.
        var indexes = Zukan8.GetRawIndexes(PersonalTable.SWSH, _zukan.GetRevision(), Zukan8Index.TotalCount);
        
        var speciesNames = GameInfo.Strings.Species;
        _allEntries = indexes.OrderBy(z => z.GetEntryName(speciesNames)).ToList();

        var comboItems = _allEntries.Select((z, index) => 
        {
            var name = z.GetEntryName(speciesNames);
            // Add asterisk if DexType mismatches (from WinForms logic)
            if (_zukan.DexLookup[z.Species].DexType != z.Entry.DexType)
                name += "***";
            return new ComboItem(name, index);
        }).ToList();

        _allSpecies = comboItems;
        _filteredSpecies = new ObservableCollection<ComboItem>(_allSpecies);

        // Select first
        if (_filteredSpecies.Count > 0)
            SelectedSpecies = _filteredSpecies[0];
    }

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
    private ComboItem? _selectedSpecies;

    partial void OnSelectedSpeciesChanged(ComboItem? oldValue, ComboItem? newValue)
    {
        if (oldValue != null)
            SaveEntry(oldValue.Value);
        
        if (newValue != null)
            LoadEntry(newValue.Value);
    }

    // Entry Properties
    [ObservableProperty] private bool _caught;
    [ObservableProperty] private bool _caughtGigantamax;
    [ObservableProperty] private bool _caughtGigantamax1; // Urshifu
    [ObservableProperty] private bool _isUrshifu;

    // Display Properties
    [ObservableProperty] private bool _displayDynamax;
    [ObservableProperty] private bool _displayShiny;
    [ObservableProperty] private int _displayGender; // 0=Male, 1=Female, 2=Genderless? Enum?
    [ObservableProperty] private int _displayForm;
    [ObservableProperty] private uint _battledCount;

    // Languages
    [ObservableProperty] private bool _langJapanese;
    [ObservableProperty] private bool _langEnglish;
    [ObservableProperty] private bool _langFrench;
    [ObservableProperty] private bool _langItalian;
    [ObservableProperty] private bool _langGerman;
    [ObservableProperty] private bool _langSpanish;
    [ObservableProperty] private bool _langKorean;
    [ObservableProperty] private bool _langChineseS;
    [ObservableProperty] private bool _langChineseT;

    // Forms
    [ObservableProperty]
    private ObservableCollection<Pokedex8FormViewModel> _forms = [];

    private void LoadEntry(int index)
    {
        if (index < 0 || index >= _allEntries.Count) return;

        var entryInfo = _allEntries[index];
        var entry = entryInfo.Entry;
        if (entry.DexType == Zukan8Type.None)
            return; // Should disable editing?

        Caught = _zukan.GetCaught(entry);
        CaughtGigantamax = _zukan.GetCaughtGigantamaxed(entry);
        
        IsUrshifu = entryInfo.Species == (int)Species.Urshifu;
        if (IsUrshifu)
            CaughtGigantamax1 = _zukan.GetCaughtGigantamax1(entry);
        else
            CaughtGigantamax1 = false;

        DisplayDynamax = _zukan.GetDisplayDynamaxInstead(entry);
        DisplayShiny = _zukan.GetDisplayShiny(entry);
        DisplayGender = (int)_zukan.GetGenderDisplayed(entry);
        DisplayForm = (int)_zukan.GetFormDisplayed(entry);
        BattledCount = _zukan.GetBattledCount(entry);

        LangJapanese = _zukan.GetIsLanguageIndexObtained(entry, 0);
        LangEnglish = _zukan.GetIsLanguageIndexObtained(entry, 1);
        LangFrench = _zukan.GetIsLanguageIndexObtained(entry, 2);
        LangItalian = _zukan.GetIsLanguageIndexObtained(entry, 3);
        LangGerman = _zukan.GetIsLanguageIndexObtained(entry, 4);
        LangSpanish = _zukan.GetIsLanguageIndexObtained(entry, 5);
        LangKorean = _zukan.GetIsLanguageIndexObtained(entry, 6);
        LangChineseS = _zukan.GetIsLanguageIndexObtained(entry, 7);
        LangChineseT = _zukan.GetIsLanguageIndexObtained(entry, 8);

        LoadForms(entryInfo.Species, entry);
    }

    private void LoadForms(ushort species, Zukan8Index entry)
    {
        Forms.Clear();
        var formNames = GetFormList(species);
        
        // We need to display relevant forms.
        // WinForms displays 0-63, but names are only valid for forms.Length.
        // Form 63 is Gigantamax.
        // Form 62 is Gigantamax (Urshifu).

        // Add standard forms
        for (int i = 0; i < formNames.Length; i++)
        {
            var f = CreateFormVM(entry, i, formNames[i]);
            Forms.Add(f);
        }

        // Check specifically for GMax forms if not already covered
        // Note: formNames might not include "Gigantamax" if it's not a standard form index?
        // WinForms hardcodes items[63] = "Gigantamax".
        
        if (species == (int)Species.Urshifu)
        {
            Forms.Add(CreateFormVM(entry, 62, "Gmax-Rapid Strike")); // Name guessed based on WinForms logic
            Forms.Add(CreateFormVM(entry, 63, "Gmax-Single Strike"));
        }
        else
        {
            // For others, check if we should show GMax (index 63)
            // Typically GMax flag is at 63.
            Forms.Add(CreateFormVM(entry, 63, "Gigantamax"));
        }
    }

    private Pokedex8FormViewModel CreateFormVM(Zukan8Index entry, int formIndex, string name)
    {
        var f = new Pokedex8FormViewModel(name, (byte)formIndex);
        f.SeenMale = _zukan.GetSeenRegion(entry, (byte)formIndex, 0);
        f.SeenFemale = _zukan.GetSeenRegion(entry, (byte)formIndex, 1);
        f.SeenShinyMale = _zukan.GetSeenRegion(entry, (byte)formIndex, 2);
        f.SeenShinyFemale = _zukan.GetSeenRegion(entry, (byte)formIndex, 3);
        return f;
    }

    private void SaveEntry(int index)
    {
        if (index < 0 || index >= _allEntries.Count) return;

        var entryInfo = _allEntries[index];
        var entry = entryInfo.Entry;
        if (entry.DexType == Zukan8Type.None) return;

        _zukan.SetCaught(entry, Caught);
        _zukan.SetCaughtGigantamax(entry, CaughtGigantamax);
        
        if (IsUrshifu)
            _zukan.SetCaughtGigantamax1(entry, CaughtGigantamax1);

        _zukan.SetDisplayDynamaxInstead(entry, DisplayDynamax);
        _zukan.SetDisplayShiny(entry, DisplayShiny);
        _zukan.SetGenderDisplayed(entry, (uint)DisplayGender);
        _zukan.SetFormDisplayed(entry, (uint)DisplayForm);
        _zukan.SetBattledCount(entry, BattledCount);

        _zukan.SetIsLanguageIndexObtained(entry, 0, LangJapanese);
        _zukan.SetIsLanguageIndexObtained(entry, 1, LangEnglish);
        _zukan.SetIsLanguageIndexObtained(entry, 2, LangFrench);
        _zukan.SetIsLanguageIndexObtained(entry, 3, LangItalian);
        _zukan.SetIsLanguageIndexObtained(entry, 4, LangGerman);
        _zukan.SetIsLanguageIndexObtained(entry, 5, LangSpanish);
        _zukan.SetIsLanguageIndexObtained(entry, 6, LangKorean);
        _zukan.SetIsLanguageIndexObtained(entry, 7, LangChineseS);
        _zukan.SetIsLanguageIndexObtained(entry, 8, LangChineseT);

        foreach (var f in Forms)
        {
            _zukan.SetSeenRegion(entry, f.FormIndex, 0, f.SeenMale);
            _zukan.SetSeenRegion(entry, f.FormIndex, 1, f.SeenFemale);
            _zukan.SetSeenRegion(entry, f.FormIndex, 2, f.SeenShinyMale);
            _zukan.SetSeenRegion(entry, f.FormIndex, 3, f.SeenShinyFemale);
        }
    }

    private static string[] GetFormList(ushort species)
    {
        var s = GameInfo.Strings;
        if (species == (int)Species.Alcremie)
            return FormConverter.GetAlcremieFormList(s.forms);
        return FormConverter.GetFormList(species, s.Types, s.forms, GameInfo.GenderSymbolASCII, EntityContext.Gen8);
    }
    
    public void SaveCurrent()
    {
        if (SelectedSpecies != null)
            SaveEntry(SelectedSpecies.Value);
    }

    [RelayCommand]
    private void SeenAll()
    {
        if (SelectedSpecies == null) return;
        SaveEntry(SelectedSpecies.Value); // Save current UI state first
        _zukan.SeenAll(false); // Helper method on Zukan8? WinForms uses Dex.SeenAll(shiny)
        LoadEntry(SelectedSpecies.Value);
    }

    [RelayCommand]
    private void CaughtAll()
    {
        if (SelectedSpecies == null) return;
        SaveEntry(SelectedSpecies.Value);
        _zukan.CaughtAll(false);
        LoadEntry(SelectedSpecies.Value);
    }
    
    [RelayCommand]
    private void CompleteDex()
    {
        if (SelectedSpecies == null) return;
        SaveEntry(SelectedSpecies.Value);
        _zukan.CompleteDex(false);
        LoadEntry(SelectedSpecies.Value);
    }
}

public partial class Pokedex8FormViewModel : ViewModelBase
{
    public string Name { get; }
    public byte FormIndex { get; }

    [ObservableProperty] private bool _seenMale;
    [ObservableProperty] private bool _seenFemale;
    [ObservableProperty] private bool _seenShinyMale;
    [ObservableProperty] private bool _seenShinyFemale;

    public Pokedex8FormViewModel(string name, byte formIndex)
    {
        Name = name;
        FormIndex = formIndex;
    }
}

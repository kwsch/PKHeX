using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class Pokedex8bEditorViewModel : ViewModelBase
{
    private readonly SAV8BS _sav;
    private readonly Zukan8b _zukan;
    private readonly List<ComboItem> _allSpecies;

    public Pokedex8bEditorViewModel(SAV8BS sav)
    {
        _sav = sav;
        _zukan = sav.Zukan;

        var speciesNames = GameInfo.Strings.Species;
        _allSpecies = Enumerable.Range(1, 493)
            .Select(i => new ComboItem(speciesNames[i], i))
            .ToList();

        _filteredSpecies = new ObservableCollection<ComboItem>(_allSpecies);

        // Global Flags
        HasRegionalDex = _zukan.HasRegionalDex;
        HasNationalDex = _zukan.HasNationalDex;

        // Select first
        SelectedSpecies = _allSpecies[0];
    }

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

    // Global Flags
    [ObservableProperty] private bool _hasRegionalDex;
    [ObservableProperty] private bool _hasNationalDex;

    partial void OnHasRegionalDexChanged(bool value) => _zukan.HasRegionalDex = value;
    partial void OnHasNationalDexChanged(bool value) => _zukan.HasNationalDex = value;

    // Entry Properties
    [ObservableProperty] private int _state; // 0=None, 1=Heard, 2=Seen, 3=Caught
    public string[] States => ["None", "Heard Of", "Seen", "Caught"];

    [ObservableProperty] private bool _seenMale;
    [ObservableProperty] private bool _seenFemale;
    [ObservableProperty] private bool _seenShinyMale;
    [ObservableProperty] private bool _seenShinyFemale;

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
    private ObservableCollection<Pokedex8bFormViewModel> _forms = [];

    // UI State
    [ObservableProperty] private bool _canBeFemale;
    [ObservableProperty] private bool _canBeMale;

    private void LoadEntry(int species)
    {
        var uspecies = (ushort)species;
        State = (int)_zukan.GetState(uspecies);
        
        _zukan.GetGenderFlags(uspecies, out bool m, out bool f, out bool ms, out bool fs);
        SeenMale = m;
        SeenFemale = f;
        SeenShinyMale = ms;
        SeenShinyFemale = fs;

        LangJapanese = _zukan.GetLanguageFlag(uspecies, (int)LanguageID.Japanese);
        LangEnglish = _zukan.GetLanguageFlag(uspecies, (int)LanguageID.English);
        LangFrench = _zukan.GetLanguageFlag(uspecies, (int)LanguageID.French);
        LangItalian = _zukan.GetLanguageFlag(uspecies, (int)LanguageID.Italian);
        LangGerman = _zukan.GetLanguageFlag(uspecies, (int)LanguageID.German);
        LangSpanish = _zukan.GetLanguageFlag(uspecies, (int)LanguageID.Spanish);
        LangKorean = _zukan.GetLanguageFlag(uspecies, (int)LanguageID.Korean);
        LangChineseS = _zukan.GetLanguageFlag(uspecies, (int)LanguageID.ChineseS);
        LangChineseT = _zukan.GetLanguageFlag(uspecies, (int)LanguageID.ChineseT);

        var pi = _sav.Personal[uspecies];
        CanBeFemale = !pi.OnlyMale && !pi.Genderless;
        CanBeMale = !pi.OnlyFemale;

        LoadForms(uspecies);
    }

    private void SaveEntry(int species)
    {
        var uspecies = (ushort)species;
        _zukan.SetState(uspecies, (ZukanState8b)State);
        _zukan.SetGenderFlags(uspecies, SeenMale, SeenFemale, SeenShinyMale, SeenShinyFemale);

        _zukan.SetLanguageFlag(uspecies, (int)LanguageID.Japanese, LangJapanese);
        _zukan.SetLanguageFlag(uspecies, (int)LanguageID.English, LangEnglish);
        _zukan.SetLanguageFlag(uspecies, (int)LanguageID.French, LangFrench);
        _zukan.SetLanguageFlag(uspecies, (int)LanguageID.Italian, LangItalian);
        _zukan.SetLanguageFlag(uspecies, (int)LanguageID.German, LangGerman);
        _zukan.SetLanguageFlag(uspecies, (int)LanguageID.Spanish, LangSpanish);
        _zukan.SetLanguageFlag(uspecies, (int)LanguageID.Korean, LangKorean);
        _zukan.SetLanguageFlag(uspecies, (int)LanguageID.ChineseS, LangChineseS);
        _zukan.SetLanguageFlag(uspecies, (int)LanguageID.ChineseT, LangChineseT);

        SaveForms(uspecies);
    }

    private void LoadForms(ushort species)
    {
        Forms.Clear();
        var count = Zukan8b.GetFormCount(species);
        if (count == 0) return;

        var formNames = FormConverter.GetFormList(species, GameInfo.Strings.Types, GameInfo.Strings.forms, GameInfo.GenderSymbolASCII, _sav.Context);
        for (int i = 0; i < count; i++)
        {
            var name = i < formNames.Length ? formNames[i] : $"Form {i}";
            var seen = _zukan.GetHasFormFlag(species, (byte)i, false);
            var seenShiny = _zukan.GetHasFormFlag(species, (byte)i, true);
            Forms.Add(new Pokedex8bFormViewModel(name, (byte)i, seen, seenShiny));
        }
    }

    private void SaveForms(ushort species)
    {
        foreach (var f in Forms)
        {
            _zukan.SetHasFormFlag(species, f.FormIndex, false, f.Seen);
            _zukan.SetHasFormFlag(species, f.FormIndex, true, f.SeenShiny);
        }
    }

    public void SaveCurrent()
    {
        if (SelectedSpecies != null)
            SaveEntry(SelectedSpecies.Value);
        _sav.State.Edited = true;
    }

    [RelayCommand]
    private void SeenAll()
    {
        _zukan.SeenAll(false);
        if (SelectedSpecies != null) LoadEntry(SelectedSpecies.Value);
    }

    [RelayCommand]
    private void CaughtAll()
    {
        _zukan.CaughtAll(false);
        if (SelectedSpecies != null) LoadEntry(SelectedSpecies.Value);
    }

    [RelayCommand]
    private void CompleteDex()
    {
        _zukan.CompleteDex(false);
        if (SelectedSpecies != null) LoadEntry(SelectedSpecies.Value);
    }

    [RelayCommand]
    private void ModifyAll(string command)
    {
        switch (command)
        {
            case "SeenNone": _zukan.SeenNone(); break;
            case "CaughtNone": _zukan.CaughtNone(); break;
        }
        if (SelectedSpecies != null) LoadEntry(SelectedSpecies.Value);
    }
}

public partial class Pokedex8bFormViewModel : ViewModelBase
{
    public string Name { get; }
    public byte FormIndex { get; }

    [ObservableProperty] private bool _seen;
    [ObservableProperty] private bool _seenShiny;

    public Pokedex8bFormViewModel(string name, byte formIndex, bool seen, bool seenShiny)
    {
        Name = name;
        FormIndex = formIndex;
        Seen = seen;
        SeenShiny = seenShiny;
    }
}

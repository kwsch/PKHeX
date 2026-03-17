using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the Gen 6 XY Pokedex editor.
/// Edits seen/caught/language flags per species.
/// </summary>
public partial class SAVPokedexXYViewModel : SaveEditorViewModelBase
{
    private readonly SaveFile _origin;
    private readonly SAV6XY _sav;
    private readonly Zukan6XY _zukan;
    private bool _editing;
    private ushort _species = ushort.MaxValue;

    public ObservableCollection<string> SpeciesNames { get; } = [];

    [ObservableProperty]
    private int _selectedSpeciesIndex = -1;

    // Caught
    [ObservableProperty]
    private bool _caught;

    // Seen M/F/MS/FS
    [ObservableProperty]
    private bool _seenMale;

    [ObservableProperty]
    private bool _seenFemale;

    [ObservableProperty]
    private bool _seenMaleShiny;

    [ObservableProperty]
    private bool _seenFemaleShiny;

    // Displayed M/F/MS/FS
    [ObservableProperty]
    private bool _displayedMale;

    [ObservableProperty]
    private bool _displayedFemale;

    [ObservableProperty]
    private bool _displayedMaleShiny;

    [ObservableProperty]
    private bool _displayedFemaleShiny;

    // Language flags
    [ObservableProperty]
    private bool _langJPN;

    [ObservableProperty]
    private bool _langENG;

    [ObservableProperty]
    private bool _langFRE;

    [ObservableProperty]
    private bool _langITA;

    [ObservableProperty]
    private bool _langGER;

    [ObservableProperty]
    private bool _langSPA;

    [ObservableProperty]
    private bool _langKOR;

    [ObservableProperty]
    private bool _nationalDexUnlocked;

    [ObservableProperty]
    private bool _nationalDexActive;

    [ObservableProperty]
    private string _spindaPid = "00000000";

    public SAVPokedexXYViewModel(SAV6XY sav) : base(sav)
    {
        _sav = (SAV6XY)(_origin = sav).Clone();
        _zukan = _sav.Zukan;

        for (int i = 1; i <= _sav.MaxSpeciesID; i++)
            SpeciesNames.Add($"{i:000} - {GameInfo.Strings.specieslist[i]}");

        NationalDexUnlocked = _zukan.IsNationalDexUnlocked;
        NationalDexActive = _zukan.IsNationalDexMode;
        SpindaPid = _zukan.Spinda.ToString("X8");

        SelectedSpeciesIndex = 0;
    }

    partial void OnSelectedSpeciesIndexChanged(int value)
    {
        if (value < 0 || _editing)
            return;
        SaveEntry();
        _editing = true;
        _species = (ushort)(value + 1);
        LoadEntry();
        _editing = false;
    }

    private void LoadEntry()
    {
        Caught = _zukan.GetCaught(_species);
        SeenMale = _zukan.GetSeen(_species, 0);
        SeenFemale = _zukan.GetSeen(_species, 1);
        SeenMaleShiny = _zukan.GetSeen(_species, 2);
        SeenFemaleShiny = _zukan.GetSeen(_species, 3);
        DisplayedMale = _zukan.GetDisplayed(_species, 0);
        DisplayedFemale = _zukan.GetDisplayed(_species, 1);
        DisplayedMaleShiny = _zukan.GetDisplayed(_species, 2);
        DisplayedFemaleShiny = _zukan.GetDisplayed(_species, 3);
        LangJPN = _zukan.GetLanguageFlag(_species, 0);
        LangENG = _zukan.GetLanguageFlag(_species, 1);
        LangFRE = _zukan.GetLanguageFlag(_species, 2);
        LangITA = _zukan.GetLanguageFlag(_species, 3);
        LangGER = _zukan.GetLanguageFlag(_species, 4);
        LangSPA = _zukan.GetLanguageFlag(_species, 5);
        LangKOR = _zukan.GetLanguageFlag(_species, 6);
    }

    private void SaveEntry()
    {
        if (_species is 0 or > 721)
            return;

        _zukan.SetCaught(_species, Caught);
        _zukan.SetSeen(_species, 0, SeenMale);
        _zukan.SetSeen(_species, 1, SeenFemale);
        _zukan.SetSeen(_species, 2, SeenMaleShiny);
        _zukan.SetSeen(_species, 3, SeenFemaleShiny);
        _zukan.SetDisplayed(_species, 0, DisplayedMale);
        _zukan.SetDisplayed(_species, 1, DisplayedFemale);
        _zukan.SetDisplayed(_species, 2, DisplayedMaleShiny);
        _zukan.SetDisplayed(_species, 3, DisplayedFemaleShiny);
        _zukan.SetLanguageFlag(_species, 0, LangJPN);
        _zukan.SetLanguageFlag(_species, 1, LangENG);
        _zukan.SetLanguageFlag(_species, 2, LangFRE);
        _zukan.SetLanguageFlag(_species, 3, LangITA);
        _zukan.SetLanguageFlag(_species, 4, LangGER);
        _zukan.SetLanguageFlag(_species, 5, LangSPA);
        _zukan.SetLanguageFlag(_species, 6, LangKOR);
    }

    [RelayCommand]
    private void SeenAll()
    {
        SaveEntry();
        _zukan.SeenAll(shinyToo: false);
        LoadEntry();
    }

    [RelayCommand]
    private void SeenNone()
    {
        SaveEntry();
        _zukan.SeenNone();
        LoadEntry();
    }

    [RelayCommand]
    private void CaughtAll()
    {
        SaveEntry();
        var language = (LanguageID)_sav.Language;
        _zukan.CaughtAll(language, allLanguages: false);
        LoadEntry();
    }

    [RelayCommand]
    private void CaughtNone()
    {
        SaveEntry();
        _zukan.CaughtNone();
        LoadEntry();
    }

    [RelayCommand]
    private void Complete()
    {
        SaveEntry();
        var language = (LanguageID)_sav.Language;
        _zukan.SeenAll(shinyToo: false);
        _zukan.CaughtAll(language, allLanguages: false);
        LoadEntry();
    }

    [RelayCommand]
    private void Save()
    {
        SaveEntry();
        _zukan.IsNationalDexUnlocked = NationalDexUnlocked;
        _zukan.IsNationalDexMode = NationalDexActive;
        _zukan.Spinda = uint.TryParse(SpindaPid, System.Globalization.NumberStyles.HexNumber, null, out var spinda) ? spinda : 0;
        if (_species is not 0)
            _zukan.InitialSpecies = _species;
        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }
}

using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the Gen 7 SM/USUM Pokedex editor.
/// Edits caught, seen (M/F normal/shiny), displayed, and language flags per species.
/// </summary>
public partial class SAVPokedexSMViewModel : SaveEditorViewModelBase
{
    private readonly SaveFile _origin;
    private readonly SAV7 _sav;
    private readonly Zukan7 _dex;
    private bool _editing;
    private int _currentIndex = -1;

    public ObservableCollection<string> SpeciesNames { get; } = [];

    [ObservableProperty] private int _selectedSpeciesIndex = -1;

    // Caught
    [ObservableProperty] private bool _caught;
    [ObservableProperty] private bool _caughtEnabled;

    // Seen: Male Normal, Female Normal, Male Shiny, Female Shiny
    [ObservableProperty] private bool _seenMaleNormal;
    [ObservableProperty] private bool _seenFemaleNormal;
    [ObservableProperty] private bool _seenMaleShiny;
    [ObservableProperty] private bool _seenFemaleShiny;
    [ObservableProperty] private bool _maleEnabled;
    [ObservableProperty] private bool _femaleEnabled;

    // Displayed
    [ObservableProperty] private bool _displayedMaleNormal;
    [ObservableProperty] private bool _displayedFemaleNormal;
    [ObservableProperty] private bool _displayedMaleShiny;
    [ObservableProperty] private bool _displayedFemaleShiny;

    // Language flags (9 languages)
    [ObservableProperty] private bool _lang1;
    [ObservableProperty] private bool _lang2;
    [ObservableProperty] private bool _lang3;
    [ObservableProperty] private bool _lang4;
    [ObservableProperty] private bool _lang5;
    [ObservableProperty] private bool _lang6;
    [ObservableProperty] private bool _lang7;
    [ObservableProperty] private bool _lang8;
    [ObservableProperty] private bool _lang9;
    [ObservableProperty] private bool _langEnabled;

    public SAVPokedexSMViewModel(SAV7 sav) : base(sav)
    {
        _sav = (SAV7)(_origin = sav).Clone();
        _dex = _sav.Zukan;

        _editing = true;
        var species = GameInfo.Strings.Species;
        var names = _dex.GetEntryNames(species);
        foreach (var n in names)
            SpeciesNames.Add(n);
        _editing = false;

        SelectedSpeciesIndex = 0;
    }

    partial void OnSelectedSpeciesIndexChanged(int value)
    {
        if (_editing || value < 0)
            return;
        SaveEntry();
        _currentIndex = value;
        LoadEntry();
    }

    private void LoadEntry()
    {
        if (_currentIndex < 0)
            return;
        _editing = true;
        var index = _currentIndex;
        var species = (ushort)(index + 1);
        bool isSpeciesEntry = species <= _sav.MaxSpeciesID;

        CaughtEnabled = isSpeciesEntry;
        Caught = _dex.GetCaught(species);

        var gt = _dex.GetBaseSpeciesGenderValue(index);
        MaleEnabled = gt != PersonalInfo.RatioMagicFemale;
        FemaleEnabled = gt is not (PersonalInfo.RatioMagicMale or PersonalInfo.RatioMagicGenderless);

        SeenMaleNormal = _dex.GetSeen(species, 0);
        SeenFemaleNormal = _dex.GetSeen(species, 1);
        SeenMaleShiny = _dex.GetSeen(species, 2);
        SeenFemaleShiny = _dex.GetSeen(species, 3);

        DisplayedMaleNormal = _dex.GetDisplayed(index, 0);
        DisplayedFemaleNormal = _dex.GetDisplayed(index, 1);
        DisplayedMaleShiny = _dex.GetDisplayed(index, 2);
        DisplayedFemaleShiny = _dex.GetDisplayed(index, 3);

        LangEnabled = isSpeciesEntry;
        Lang1 = isSpeciesEntry && _dex.GetLanguageFlag(index, 0);
        Lang2 = isSpeciesEntry && _dex.GetLanguageFlag(index, 1);
        Lang3 = isSpeciesEntry && _dex.GetLanguageFlag(index, 2);
        Lang4 = isSpeciesEntry && _dex.GetLanguageFlag(index, 3);
        Lang5 = isSpeciesEntry && _dex.GetLanguageFlag(index, 4);
        Lang6 = isSpeciesEntry && _dex.GetLanguageFlag(index, 5);
        Lang7 = isSpeciesEntry && _dex.GetLanguageFlag(index, 6);
        Lang8 = isSpeciesEntry && _dex.GetLanguageFlag(index, 7);
        Lang9 = isSpeciesEntry && _dex.GetLanguageFlag(index, 8);

        _editing = false;
    }

    private void SaveEntry()
    {
        if (_currentIndex < 0)
            return;
        var index = _currentIndex;
        var species = (ushort)(index + 1);
        bool isSpeciesEntry = species <= _sav.MaxSpeciesID;

        _dex.SetSeen(species, 0, SeenMaleNormal);
        _dex.SetSeen(species, 1, SeenFemaleNormal);
        _dex.SetSeen(species, 2, SeenMaleShiny);
        _dex.SetSeen(species, 3, SeenFemaleShiny);

        _dex.SetDisplayed(index, 0, DisplayedMaleNormal);
        _dex.SetDisplayed(index, 1, DisplayedFemaleNormal);
        _dex.SetDisplayed(index, 2, DisplayedMaleShiny);
        _dex.SetDisplayed(index, 3, DisplayedFemaleShiny);

        if (!isSpeciesEntry)
            return;

        _dex.SetCaught(species, Caught);
        _dex.SetLanguageFlag(index, 0, Lang1);
        _dex.SetLanguageFlag(index, 1, Lang2);
        _dex.SetLanguageFlag(index, 2, Lang3);
        _dex.SetLanguageFlag(index, 3, Lang4);
        _dex.SetLanguageFlag(index, 4, Lang5);
        _dex.SetLanguageFlag(index, 5, Lang6);
        _dex.SetLanguageFlag(index, 6, Lang7);
        _dex.SetLanguageFlag(index, 7, Lang8);
        _dex.SetLanguageFlag(index, 8, Lang9);
    }

    [RelayCommand]
    private void GiveAllCurrent()
    {
        if (LangEnabled)
        {
            Lang1 = Lang2 = Lang3 = Lang4 = Lang5 = Lang6 = Lang7 = Lang8 = Lang9 = true;
        }
        if (CaughtEnabled)
            Caught = true;
        if (MaleEnabled)
        {
            SeenMaleNormal = true;
            SeenMaleShiny = true;
        }
        if (FemaleEnabled)
        {
            SeenFemaleNormal = true;
            SeenFemaleShiny = true;
        }
        if (!(DisplayedMaleNormal || DisplayedFemaleNormal || DisplayedMaleShiny || DisplayedFemaleShiny))
        {
            if (MaleEnabled) DisplayedMaleNormal = true;
            else if (FemaleEnabled) DisplayedFemaleNormal = true;
        }
    }

    [RelayCommand]
    private void SeenAll()
    {
        SaveEntry();
        for (ushort i = 1; i <= _sav.MaxSpeciesID; i++)
        {
            var idx = i - 1;
            var gt = _dex.GetBaseSpeciesGenderValue(idx);
            bool canMale = gt != PersonalInfo.RatioMagicFemale;
            bool canFemale = gt is not (PersonalInfo.RatioMagicMale or PersonalInfo.RatioMagicGenderless);

            if (canMale) { _dex.SetSeen(i, 0, true); _dex.SetSeen(i, 2, true); }
            if (canFemale) { _dex.SetSeen(i, 1, true); _dex.SetSeen(i, 3, true); }
            if (!(_dex.GetDisplayed(idx, 0) || _dex.GetDisplayed(idx, 1) || _dex.GetDisplayed(idx, 2) || _dex.GetDisplayed(idx, 3)))
                _dex.SetDisplayed(idx, canMale ? 0 : 1, true);
        }
        LoadEntry();
    }

    [RelayCommand]
    private void CaughtAll()
    {
        SaveEntry();
        int lang = _sav.Language;
        if (lang > 5) lang--;
        lang--;
        for (ushort i = 1; i <= _sav.MaxSpeciesID; i++)
        {
            var idx = i - 1;
            var gt = _dex.GetBaseSpeciesGenderValue(idx);
            bool canMale = gt != PersonalInfo.RatioMagicFemale;
            bool canFemale = gt is not (PersonalInfo.RatioMagicMale or PersonalInfo.RatioMagicGenderless);

            _dex.SetCaught(i, true);
            if (canMale) { _dex.SetSeen(i, 0, true); _dex.SetSeen(i, 2, true); }
            if (canFemale) { _dex.SetSeen(i, 1, true); _dex.SetSeen(i, 3, true); }
            if (!(_dex.GetDisplayed(idx, 0) || _dex.GetDisplayed(idx, 1) || _dex.GetDisplayed(idx, 2) || _dex.GetDisplayed(idx, 3)))
                _dex.SetDisplayed(idx, canMale ? 0 : 1, true);

            for (int j = 0; j < 9; j++)
                _dex.SetLanguageFlag(idx, j, j == lang);
        }
        LoadEntry();
    }

    [RelayCommand]
    private void ClearAll()
    {
        SaveEntry();
        for (ushort i = 1; i <= _sav.MaxSpeciesID; i++)
        {
            var idx = i - 1;
            _dex.SetCaught(i, false);
            for (int j = 0; j < 4; j++)
            {
                _dex.SetSeen(i, j, false);
                _dex.SetDisplayed(idx, j, false);
            }
            for (int j = 0; j < 9; j++)
                _dex.SetLanguageFlag(idx, j, false);
        }
        LoadEntry();
    }

    [RelayCommand]
    private void Save()
    {
        SaveEntry();
        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }
}

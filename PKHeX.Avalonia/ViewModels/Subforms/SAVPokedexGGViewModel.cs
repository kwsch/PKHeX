using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the Let's Go Pokedex editor.
/// Edits caught, seen, displayed, language, and size record flags per species.
/// </summary>
public partial class SAVPokedexGGViewModel : SaveEditorViewModelBase
{
    private readonly SaveFile _origin;
    private readonly SAV7b _sav;
    private readonly Zukan7b _dex;
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

    // Size records
    [ObservableProperty] private bool _showSizeRecords;
    [ObservableProperty] private bool _hasMinHeight;
    [ObservableProperty] private bool _hasMaxHeight;
    [ObservableProperty] private bool _hasMinWeight;
    [ObservableProperty] private bool _hasMaxWeight;
    [ObservableProperty] private int _minHeight;
    [ObservableProperty] private int _minHeightWeight;
    [ObservableProperty] private int _maxHeight;
    [ObservableProperty] private int _maxHeightWeight;
    [ObservableProperty] private int _minWeightHeight;
    [ObservableProperty] private int _minWeight;
    [ObservableProperty] private int _maxWeightHeight;
    [ObservableProperty] private int _maxWeight;

    public SAVPokedexGGViewModel(SAV7b sav) : base(sav)
    {
        _sav = (SAV7b)(_origin = sav).Clone();
        _dex = _sav.Blocks.Zukan;

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
        Caught = CaughtEnabled && _dex.GetCaught(species);

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

        // Size records
        var recordSpecies = isSpeciesEntry ? species : _dex.GetBaseSpecies(index);
        int formIndex = Math.Max(0, isSpeciesEntry ? 0 : index - (_sav.MaxSpeciesID - 1));
        bool hasRecord = Zukan7b.TryGetSizeEntryIndex(recordSpecies, (byte)formIndex, out var sizeIdx);
        ShowSizeRecords = hasRecord;
        if (hasRecord)
        {
            HasMinHeight = _dex.GetSizeData(DexSizeType.MinHeight, sizeIdx, out byte h1, out byte w1, out _);
            MinHeight = h1; MinHeightWeight = w1;
            HasMaxHeight = _dex.GetSizeData(DexSizeType.MaxHeight, sizeIdx, out byte h2, out byte w2, out _);
            MaxHeight = h2; MaxHeightWeight = w2;
            HasMinWeight = _dex.GetSizeData(DexSizeType.MinWeight, sizeIdx, out byte h3, out byte w3, out _);
            MinWeightHeight = h3; MinWeight = w3;
            HasMaxWeight = _dex.GetSizeData(DexSizeType.MaxWeight, sizeIdx, out byte h4, out byte w4, out _);
            MaxWeightHeight = h4; MaxWeight = w4;
        }

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

        if (isSpeciesEntry)
        {
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

        // Save size records
        var recordSpecies = isSpeciesEntry ? species : _dex.GetBaseSpecies(index);
        int formIndex = Math.Max(0, isSpeciesEntry ? 0 : index - (_sav.MaxSpeciesID - 1));
        if (Zukan7b.TryGetSizeEntryIndex(recordSpecies, (byte)formIndex, out var sizeIdx))
        {
            byte defH = Zukan7b.DefaultEntryValueH;
            byte defW = Zukan7b.DefaultEntryValueW;
            _dex.SetSizeData(DexSizeType.MinHeight, sizeIdx, HasMinHeight ? (byte)MinHeight : defH, HasMinHeight ? (byte)MinHeightWeight : defW, false);
            _dex.SetSizeData(DexSizeType.MaxHeight, sizeIdx, HasMaxHeight ? (byte)MaxHeight : defH, HasMaxHeight ? (byte)MaxHeightWeight : defW, false);
            _dex.SetSizeData(DexSizeType.MinWeight, sizeIdx, HasMinWeight ? (byte)MinWeightHeight : defH, HasMinWeight ? (byte)MinWeight : defW, false);
            _dex.SetSizeData(DexSizeType.MaxWeight, sizeIdx, HasMaxWeight ? (byte)MaxWeightHeight : defH, HasMaxWeight ? (byte)MaxWeight : defW, false);
        }
    }

    [RelayCommand]
    private void GiveAllCurrent()
    {
        if (LangEnabled)
            Lang1 = Lang2 = Lang3 = Lang4 = Lang5 = Lang6 = Lang7 = Lang8 = Lang9 = true;
        if (CaughtEnabled)
            Caught = true;
        if (MaleEnabled) { SeenMaleNormal = true; SeenMaleShiny = true; }
        if (FemaleEnabled) { SeenFemaleNormal = true; SeenFemaleShiny = true; }
        if (!(DisplayedMaleNormal || DisplayedFemaleNormal || DisplayedMaleShiny || DisplayedFemaleShiny))
        {
            if (MaleEnabled) DisplayedMaleNormal = true;
            else if (FemaleEnabled) DisplayedFemaleNormal = true;
        }
    }

    [RelayCommand]
    private void ClearAll()
    {
        SaveEntry();
        for (int i = 0; i < SpeciesNames.Count; i++)
        {
            var species = (ushort)(i + 1);
            bool isSpecies = species <= _sav.MaxSpeciesID;
            for (int j = 0; j < 4; j++)
            {
                _dex.SetSeen(species, j, false);
                _dex.SetDisplayed(i, j, false);
            }
            if (isSpecies)
            {
                _dex.SetCaught(species, false);
                for (int j = 0; j < 9; j++)
                    _dex.SetLanguageFlag(i, j, false);
            }
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

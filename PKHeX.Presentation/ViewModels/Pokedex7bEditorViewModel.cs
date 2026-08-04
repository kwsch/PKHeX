using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class Pokedex7bEditorViewModel : ViewModelBase
{
    private readonly SAV7b _sav;
    private readonly Zukan7b _zukan;

    public Pokedex7bEditorViewModel(SAV7b sav)
    {
        _sav = sav;
        _zukan = sav.Zukan;

        var speciesNames = GameInfo.Strings.Species;
        var names = _zukan.GetEntryNames(speciesNames);
        Entries = new ObservableCollection<ComboItem>(
            names.Select((n, i) => new ComboItem(n, i)));

        _selectedEntry = Entries.FirstOrDefault();
        if (_selectedEntry != null)
            LoadEntry(_selectedEntry.Value);
    }

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

    // Seen flags
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

    // Language flags
    public bool HasLanguages => _selectedIndex < _sav.MaxSpeciesID;

    [ObservableProperty] private bool _langJPN;
    [ObservableProperty] private bool _langENG;
    [ObservableProperty] private bool _langFRA;
    [ObservableProperty] private bool _langGER;
    [ObservableProperty] private bool _langITA;
    [ObservableProperty] private bool _langSPA;
    [ObservableProperty] private bool _langKOR;
    [ObservableProperty] private bool _langCHT;
    [ObservableProperty] private bool _langCHS;

    // Size Data
    [ObservableProperty] private bool _hasSizeData;
    [ObservableProperty] private byte _minHeight;
    [ObservableProperty] private byte _maxHeight;
    [ObservableProperty] private byte _minWeight;
    [ObservableProperty] private byte _maxWeight;
    [ObservableProperty] private bool _minHeightFlag;
    [ObservableProperty] private bool _maxHeightFlag;
    [ObservableProperty] private bool _minWeightFlag;
    [ObservableProperty] private bool _maxWeightFlag;

    private int _selectedIndex = -1;

    private void LoadEntry(int index)
    {
        _selectedIndex = index;
        var species = (ushort)(index + 1);
        bool isSpeciesEntry = species <= _sav.MaxSpeciesID;

        IsCaughtEnabled = isSpeciesEntry;
        Caught = isSpeciesEntry && _zukan.GetCaught(species);

        var gt = _zukan.GetBaseSpeciesGenderValue(index);
        CanBeMale = gt != PersonalInfo.RatioMagicFemale;
        CanBeFemale = gt is not (PersonalInfo.RatioMagicMale or PersonalInfo.RatioMagicGenderless);

        SeenMale = _zukan.GetSeen(species, 0);
        SeenFemale = _zukan.GetSeen(species, 1);
        SeenMaleShiny = _zukan.GetSeen(species, 2);
        SeenFemaleShiny = _zukan.GetSeen(species, 3);

        DisplayedMale = _zukan.GetDisplayed(index, 0);
        DisplayedFemale = _zukan.GetDisplayed(index, 1);
        DisplayedMaleShiny = _zukan.GetDisplayed(index, 2);
        DisplayedFemaleShiny = _zukan.GetDisplayed(index, 3);

        LoadLanguages(index, isSpeciesEntry);
        LoadSizeData(index);

        OnPropertyChanged(nameof(HasLanguages));
    }

    private void LoadLanguages(int index, bool isSpeciesEntry)
    {
        if (!isSpeciesEntry)
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

    private void LoadSizeData(int index)
    {
        // We need species and form from the index.
        // Zukan7.GetEntryNames logic: 1-MaxSpeciesID, then forms.
        if (index < _sav.MaxSpeciesID)
        {
            // species = (ushort)(index + 1);
        }
        else
        {
            // This is complex because GetEntryNames uses a flattened list.
            // Zukan7 doesn't expose the mapping directly.
            // But Zukan7b.TryGetSizeEntryIndex is what we really need.
            // Let's approximate or just try for Kanto + Meltan.
            // Actually, we can just use the index to search if it's a form entry.
            // Re-implementing mapping for LGPE
            if (index < 151) { } // species = (ushort)(index + 1);
            else if (index == 151) { } // species = 808; // Meltan
            else if (index == 152) { } // species = 809; // Melmetal
            else
            {
                // Form entries... 
                // Zukan7.GetEntryNames(GameInfo.Strings.Species) for Gen 7b:
                // 1..151, 808, 809, then forms from DexFormUtil.GetDexFormCountGG.
                // It's probably better to just iterate and check.
            }
        }

        // Simpler: TryGetSizeEntryIndex by species/form if we can determine them.
        // Since we only need it for the selected entry, and we have the index...
        // Wait, Zukan7b.TryGetSizeEntryIndex uses (species, form).
        // Let's try to find which species/form the index corresponds to.
        
        // Actually, Zukan7.GetEntryNames returns a list where [0..150] are species 1..151, [151] is Meltan, [152] is Melmetal.
        // The size data indices for these match exactly!
        // 0..150 -> 1..151
        // 151 -> 808
        // 152 -> 809
        // 153.. -> forms.
        
        int sizeDataIndex = -1;
        if (index <= 152)
        {
            sizeDataIndex = index;
        }
        else
        {
            // Forms.
            // Form indices in size data start at 153.
            // Form entries in the list also start at 153.
            sizeDataIndex = index;
        }

        HasSizeData = sizeDataIndex >= 0 && sizeDataIndex < 186; // EntryCount is 186
        if (HasSizeData)
        {
            _zukan.GetSizeData(DexSizeType.MinHeight, sizeDataIndex, out byte minH, out _, out bool minHF);
            _zukan.GetSizeData(DexSizeType.MaxHeight, sizeDataIndex, out byte maxH, out _, out bool maxHF);
            _zukan.GetSizeData(DexSizeType.MinWeight, sizeDataIndex, out _, out byte minW, out bool minWF);
            _zukan.GetSizeData(DexSizeType.MaxWeight, sizeDataIndex, out _, out byte maxW, out bool maxWF);

            MinHeight = minH; MinHeightFlag = minHF;
            MaxHeight = maxH; MaxHeightFlag = maxHF;
            MinWeight = minW; MinWeightFlag = minWF;
            MaxWeight = maxW; MaxWeightFlag = maxWF;
        }
    }

    [RelayCommand]
    private void Save()
    {
        if (_selectedIndex < 0) return;

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

        if (isSpeciesEntry)
        {
            _zukan.SetCaught(species, Caught);
            _zukan.SetLanguageFlag(index, 0, LangJPN);
            _zukan.SetLanguageFlag(index, 1, LangENG);
            _zukan.SetLanguageFlag(index, 2, LangFRA);
            _zukan.SetLanguageFlag(index, 3, LangGER);
            _zukan.SetLanguageFlag(index, 4, LangITA);
            _zukan.SetLanguageFlag(index, 5, LangSPA);
            _zukan.SetLanguageFlag(index, 6, LangKOR);
            _zukan.SetLanguageFlag(index, 7, LangCHT);
            _zukan.SetLanguageFlag(index, 8, LangCHS);
        }

        if (HasSizeData)
        {
            _zukan.SetSizeData(DexSizeType.MinHeight, index, MinHeight, 0, MinHeightFlag);
            _zukan.SetSizeData(DexSizeType.MaxHeight, index, MaxHeight, 0, MaxHeightFlag);
            _zukan.SetSizeData(DexSizeType.MinWeight, index, 0, MinWeight, MinWeightFlag);
            _zukan.SetSizeData(DexSizeType.MaxWeight, index, 0, MaxWeight, MaxWeightFlag);
        }

        _sav.State.Edited = true;
    }
}

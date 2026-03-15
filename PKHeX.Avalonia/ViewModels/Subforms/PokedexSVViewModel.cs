using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single SV Paldea Pokedex species entry.
/// </summary>
public partial class PokedexSVEntryModel : ObservableObject
{
    public ushort Species { get; }
    public string Label { get; }

    [ObservableProperty] private int _state;
    [ObservableProperty] private bool _isNew;
    [ObservableProperty] private bool _seenMale;
    [ObservableProperty] private bool _seenFemale;
    [ObservableProperty] private bool _seenGenderless;
    [ObservableProperty] private bool _seenShiny;
    [ObservableProperty] private int _displayGender;
    [ObservableProperty] private bool _displayShiny;
    [ObservableProperty] private int _displayForm;

    // Language flags
    [ObservableProperty] private bool _langJPN;
    [ObservableProperty] private bool _langENG;
    [ObservableProperty] private bool _langFRE;
    [ObservableProperty] private bool _langITA;
    [ObservableProperty] private bool _langGER;
    [ObservableProperty] private bool _langSPA;
    [ObservableProperty] private bool _langKOR;
    [ObservableProperty] private bool _langCHS;
    [ObservableProperty] private bool _langCHT;

    public PokedexSVEntryModel(ushort species, string label)
    {
        Species = species;
        Label = label;
    }
}

/// <summary>
/// ViewModel for the Scarlet/Violet Paldea Pokedex editor.
/// </summary>
public partial class PokedexSVViewModel : SaveEditorViewModelBase
{
    private readonly SAV9SV _origin;
    private readonly SAV9SV _sav;
    private readonly Zukan9 _dex;

    [ObservableProperty] private string _searchText = string.Empty;

    public ObservableCollection<PokedexSVEntryModel> AllEntries { get; } = [];

    [ObservableProperty]
    private ObservableCollection<PokedexSVEntryModel> _filteredEntries = [];

    [ObservableProperty] private PokedexSVEntryModel? _selectedEntry;

    public PokedexSVViewModel(SAV9SV sav) : base(sav)
    {
        _sav = (SAV9SV)(_origin = sav).Clone();
        _dex = _sav.Blocks.Zukan;

        var filtered = GameInfo.FilteredSources;
        const int maxSpecies = (int)Species.IronLeaves; // 1010
        var species = filtered.Species.Where(z => z.Value <= maxSpecies).ToArray();

        foreach (var sp in species)
        {
            var s = (ushort)sp.Value;
            var entry = _dex.DexPaldea.Get(s);
            var model = new PokedexSVEntryModel(s, sp.Text);
            LoadEntry(model, entry);
            AllEntries.Add(model);
        }

        FilteredEntries = new ObservableCollection<PokedexSVEntryModel>(AllEntries);
    }

    private static void LoadEntry(PokedexSVEntryModel model, PokeDexEntry9Paldea entry)
    {
        model.State = (int)entry.GetState();
        model.IsNew = entry.GetDisplayIsNew();
        model.SeenMale = entry.GetIsGenderSeen(0);
        model.SeenFemale = entry.GetIsGenderSeen(1);
        model.SeenGenderless = entry.GetIsGenderSeen(2);
        model.SeenShiny = entry.GetSeenIsShiny();
        model.DisplayGender = (int)entry.GetDisplayGender();
        model.DisplayShiny = entry.GetDisplayIsShiny();
        model.DisplayForm = (int)entry.GetDisplayForm();

        model.LangJPN = entry.GetLanguageFlag((int)LanguageID.Japanese);
        model.LangENG = entry.GetLanguageFlag((int)LanguageID.English);
        model.LangFRE = entry.GetLanguageFlag((int)LanguageID.French);
        model.LangITA = entry.GetLanguageFlag((int)LanguageID.Italian);
        model.LangGER = entry.GetLanguageFlag((int)LanguageID.German);
        model.LangSPA = entry.GetLanguageFlag((int)LanguageID.Spanish);
        model.LangKOR = entry.GetLanguageFlag((int)LanguageID.Korean);
        model.LangCHS = entry.GetLanguageFlag((int)LanguageID.ChineseS);
        model.LangCHT = entry.GetLanguageFlag((int)LanguageID.ChineseT);
    }

    private static void SaveEntry(PokedexSVEntryModel model, PokeDexEntry9Paldea entry)
    {
        entry.SetState((uint)model.State);
        entry.SetDisplayIsNew(model.IsNew);
        entry.SetIsGenderSeen(0, model.SeenMale);
        entry.SetIsGenderSeen(1, model.SeenFemale);
        entry.SetIsGenderSeen(2, model.SeenGenderless);
        entry.SetSeenIsShiny(model.SeenShiny);
        entry.SetDisplayGender(model.DisplayGender);
        entry.SetDisplayIsShiny(model.DisplayShiny);
        entry.SetDisplayForm((uint)model.DisplayForm);

        entry.SetLanguageFlag((int)LanguageID.Japanese, model.LangJPN);
        entry.SetLanguageFlag((int)LanguageID.English, model.LangENG);
        entry.SetLanguageFlag((int)LanguageID.French, model.LangFRE);
        entry.SetLanguageFlag((int)LanguageID.Italian, model.LangITA);
        entry.SetLanguageFlag((int)LanguageID.German, model.LangGER);
        entry.SetLanguageFlag((int)LanguageID.Spanish, model.LangSPA);
        entry.SetLanguageFlag((int)LanguageID.Korean, model.LangKOR);
        entry.SetLanguageFlag((int)LanguageID.ChineseS, model.LangCHS);
        entry.SetLanguageFlag((int)LanguageID.ChineseT, model.LangCHT);
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    private void ApplyFilter()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            FilteredEntries = new ObservableCollection<PokedexSVEntryModel>(AllEntries);
            return;
        }
        FilteredEntries = new ObservableCollection<PokedexSVEntryModel>(
            AllEntries.Where(e => e.Label.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
    }

    private void SaveAllEntries()
    {
        foreach (var model in AllEntries)
        {
            var entry = _dex.DexPaldea.Get(model.Species);
            SaveEntry(model, entry);
        }
    }

    private void ReloadAllEntries()
    {
        foreach (var model in AllEntries)
        {
            var entry = _dex.DexPaldea.Get(model.Species);
            LoadEntry(model, entry);
        }
        ApplyFilter();
    }

    [RelayCommand]
    private void SeenAll()
    {
        SaveAllEntries();
        _dex.SeenAll(false);
        ReloadAllEntries();
    }

    [RelayCommand]
    private void CaughtAll()
    {
        SaveAllEntries();
        _dex.CaughtAll(false);
        ReloadAllEntries();
    }

    [RelayCommand]
    private void SeenNone()
    {
        SaveAllEntries();
        _dex.SeenNone();
        ReloadAllEntries();
    }

    [RelayCommand]
    private void CaughtNone()
    {
        SaveAllEntries();
        _dex.CaughtNone();
        ReloadAllEntries();
    }

    [RelayCommand]
    private void CompleteDex()
    {
        SaveAllEntries();
        _dex.CompleteDex(false);
        ReloadAllEntries();
    }

    [RelayCommand]
    private void Save()
    {
        SaveAllEntries();
        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }
}

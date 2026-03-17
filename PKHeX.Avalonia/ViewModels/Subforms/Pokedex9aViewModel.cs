using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single ZA Pokedex species entry.
/// </summary>
public partial class Pokedex9aEntryModel : ObservableObject
{
    public ushort Species { get; }
    public string Label { get; }

    [ObservableProperty] private bool _isNew;
    [ObservableProperty] private bool _seenMale;
    [ObservableProperty] private bool _seenFemale;
    [ObservableProperty] private bool _seenGenderless;
    [ObservableProperty] private bool _seenAlpha;
    [ObservableProperty] private bool _seenMega0;
    [ObservableProperty] private bool _seenMega1;
    [ObservableProperty] private bool _seenMega2;
    [ObservableProperty] private int _displayForm;
    [ObservableProperty] private int _displayGender;
    [ObservableProperty] private bool _displayShiny;

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
    [ObservableProperty] private bool _langLATAM;

    public Pokedex9aEntryModel(ushort species, string label)
    {
        Species = species;
        Label = label;
    }
}

/// <summary>
/// ViewModel for the Legends Z-A Pokedex editor.
/// </summary>
public partial class Pokedex9aViewModel : SaveEditorViewModelBase
{
    private readonly SAV9ZA _origin;
    private readonly SAV9ZA _sav;
    private readonly Zukan9a _dex;

    [ObservableProperty] private string _searchText = string.Empty;

    public ObservableCollection<Pokedex9aEntryModel> AllEntries { get; } = [];

    [ObservableProperty]
    private ObservableCollection<Pokedex9aEntryModel> _filteredEntries = [];

    [ObservableProperty] private Pokedex9aEntryModel? _selectedEntry;

    public Pokedex9aViewModel(SAV9ZA sav) : base(sav)
    {
        _sav = (SAV9ZA)(_origin = sav).Clone();
        _dex = _sav.Blocks.Zukan;

        var filtered = GameInfo.FilteredSources;
        int maxSpecies = sav.MaxSpeciesID;
        var species = filtered.Species.Where(z => z.Value <= maxSpecies).ToArray();

        foreach (var sp in species)
        {
            var s = (ushort)sp.Value;
            var entry = _dex.GetEntry(s);
            var model = new Pokedex9aEntryModel(s, sp.Text);
            LoadEntry(model, entry);
            AllEntries.Add(model);
        }

        FilteredEntries = new ObservableCollection<Pokedex9aEntryModel>(AllEntries);
    }

    private static void LoadEntry(Pokedex9aEntryModel model, PokeDexEntry9a entry)
    {
        model.IsNew = entry.GetDisplayIsNew();
        model.SeenMale = entry.GetIsGenderSeen(0);
        model.SeenFemale = entry.GetIsGenderSeen(1);
        model.SeenGenderless = entry.GetIsGenderSeen(2);
        model.SeenAlpha = entry.GetIsSeenAlpha();
        model.SeenMega0 = entry.GetIsSeenMega(0);
        model.SeenMega1 = entry.GetIsSeenMega(1);
        model.SeenMega2 = entry.GetIsSeenMega(2);
        model.DisplayForm = Math.Clamp((int)entry.DisplayForm, 0, 255);
        model.DisplayGender = Math.Clamp((int)entry.DisplayGender, 0, 2);
        model.DisplayShiny = entry.GetDisplayIsShiny();

        model.LangJPN = entry.GetLanguageFlag((int)LanguageID.Japanese);
        model.LangENG = entry.GetLanguageFlag((int)LanguageID.English);
        model.LangFRE = entry.GetLanguageFlag((int)LanguageID.French);
        model.LangITA = entry.GetLanguageFlag((int)LanguageID.Italian);
        model.LangGER = entry.GetLanguageFlag((int)LanguageID.German);
        model.LangSPA = entry.GetLanguageFlag((int)LanguageID.Spanish);
        model.LangKOR = entry.GetLanguageFlag((int)LanguageID.Korean);
        model.LangCHS = entry.GetLanguageFlag((int)LanguageID.ChineseS);
        model.LangCHT = entry.GetLanguageFlag((int)LanguageID.ChineseT);
        model.LangLATAM = entry.GetLanguageFlag((int)LanguageID.SpanishL);
    }

    private static void SaveEntry(Pokedex9aEntryModel model, PokeDexEntry9a entry)
    {
        entry.SetDisplayIsNew(model.IsNew);
        entry.SetIsGenderSeen(0, model.SeenMale);
        entry.SetIsGenderSeen(1, model.SeenFemale);
        entry.SetIsGenderSeen(2, model.SeenGenderless);
        entry.SetIsSeenAlpha(model.SeenAlpha);
        entry.SetIsSeenMega(0, model.SeenMega0);
        entry.SetIsSeenMega(1, model.SeenMega1);
        entry.SetIsSeenMega(2, model.SeenMega2);
        entry.DisplayForm = (byte)Math.Clamp(model.DisplayForm, 0, 255);
        entry.DisplayGender = (DisplayGender9a)Math.Clamp(model.DisplayGender, 0, 2);
        entry.SetDisplayIsShiny(model.DisplayShiny);

        entry.SetLanguageFlag((int)LanguageID.Japanese, model.LangJPN);
        entry.SetLanguageFlag((int)LanguageID.English, model.LangENG);
        entry.SetLanguageFlag((int)LanguageID.French, model.LangFRE);
        entry.SetLanguageFlag((int)LanguageID.Italian, model.LangITA);
        entry.SetLanguageFlag((int)LanguageID.German, model.LangGER);
        entry.SetLanguageFlag((int)LanguageID.Spanish, model.LangSPA);
        entry.SetLanguageFlag((int)LanguageID.Korean, model.LangKOR);
        entry.SetLanguageFlag((int)LanguageID.ChineseS, model.LangCHS);
        entry.SetLanguageFlag((int)LanguageID.ChineseT, model.LangCHT);
        entry.SetLanguageFlag((int)LanguageID.SpanishL, model.LangLATAM);
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    private void ApplyFilter()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            FilteredEntries = new ObservableCollection<Pokedex9aEntryModel>(AllEntries);
            return;
        }
        FilteredEntries = new ObservableCollection<Pokedex9aEntryModel>(
            AllEntries.Where(e => e.Label.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
    }

    private void SaveAllEntries()
    {
        foreach (var model in AllEntries)
        {
            var entry = _dex.GetEntry(model.Species);
            SaveEntry(model, entry);
        }
    }

    private void ReloadAllEntries()
    {
        foreach (var model in AllEntries)
        {
            var entry = _dex.GetEntry(model.Species);
            LoadEntry(model, entry);
        }
        ApplyFilter();
    }

    [RelayCommand] private void SeenAll() { SaveAllEntries(); _dex.SeenAll(false); ReloadAllEntries(); }
    [RelayCommand] private void CaughtAll() { SaveAllEntries(); _dex.CaughtAll(false); ReloadAllEntries(); }
    [RelayCommand] private void SeenNone() { SaveAllEntries(); _dex.SeenNone(); ReloadAllEntries(); }
    [RelayCommand] private void CaughtNone() { SaveAllEntries(); _dex.CaughtNone(); ReloadAllEntries(); }
    [RelayCommand] private void CompleteDex() { SaveAllEntries(); _dex.CompleteDex(false); ReloadAllEntries(); }

    [RelayCommand]
    private void Save()
    {
        SaveAllEntries();
        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }
}

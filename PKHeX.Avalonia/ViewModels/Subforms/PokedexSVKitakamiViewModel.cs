using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single SV Kitakami Pokedex species entry.
/// </summary>
public partial class PokedexSVKitakamiEntryModel : ObservableObject
{
    public ushort Species { get; }
    public string Label { get; }

    [ObservableProperty] private bool _seenMale;
    [ObservableProperty] private bool _seenFemale;
    [ObservableProperty] private bool _seenGenderless;
    [ObservableProperty] private bool _seenShiny;

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

    // Local dex display
    [ObservableProperty] private int _paldeaForm;
    [ObservableProperty] private int _paldeaGender;
    [ObservableProperty] private bool _paldeaShiny;
    [ObservableProperty] private int _kitakamiForm;
    [ObservableProperty] private int _kitakamiGender;
    [ObservableProperty] private bool _kitakamiShiny;
    [ObservableProperty] private int _blueberryForm;
    [ObservableProperty] private int _blueberryGender;
    [ObservableProperty] private bool _blueberryShiny;

    public PokedexSVKitakamiEntryModel(ushort species, string label)
    {
        Species = species;
        Label = label;
    }
}

/// <summary>
/// ViewModel for the Scarlet/Violet Kitakami DLC Pokedex editor.
/// </summary>
public partial class PokedexSVKitakamiViewModel : SaveEditorViewModelBase
{
    private readonly SAV9SV _origin;
    private readonly SAV9SV _sav;
    private readonly Zukan9 _dex;

    [ObservableProperty] private string _searchText = string.Empty;

    public ObservableCollection<PokedexSVKitakamiEntryModel> AllEntries { get; } = [];

    [ObservableProperty]
    private ObservableCollection<PokedexSVKitakamiEntryModel> _filteredEntries = [];

    [ObservableProperty] private PokedexSVKitakamiEntryModel? _selectedEntry;

    public PokedexSVKitakamiViewModel(SAV9SV sav) : base(sav)
    {
        _sav = (SAV9SV)(_origin = sav).Clone();
        _dex = _sav.Blocks.Zukan;

        var source = GameInfo.FilteredSources;
        var species = source.Species.ToArray();

        foreach (var sp in species)
        {
            var s = (ushort)sp.Value;
            var entry = _dex.DexKitakami.Get(s);
            var model = new PokedexSVKitakamiEntryModel(s, sp.Text);
            LoadEntry(model, entry);
            AllEntries.Add(model);
        }

        FilteredEntries = new ObservableCollection<PokedexSVKitakamiEntryModel>(AllEntries);
    }

    private static void LoadEntry(PokedexSVKitakamiEntryModel model, PokeDexEntry9Kitakami entry)
    {
        model.SeenMale = entry.GetIsGenderSeen(0);
        model.SeenFemale = entry.GetIsGenderSeen(1);
        model.SeenGenderless = entry.GetIsGenderSeen(2);
        model.SeenShiny = entry.GetIsModelSeen(true);

        model.LangJPN = entry.GetLanguageFlag((int)LanguageID.Japanese);
        model.LangENG = entry.GetLanguageFlag((int)LanguageID.English);
        model.LangFRE = entry.GetLanguageFlag((int)LanguageID.French);
        model.LangITA = entry.GetLanguageFlag((int)LanguageID.Italian);
        model.LangGER = entry.GetLanguageFlag((int)LanguageID.German);
        model.LangSPA = entry.GetLanguageFlag((int)LanguageID.Spanish);
        model.LangKOR = entry.GetLanguageFlag((int)LanguageID.Korean);
        model.LangCHS = entry.GetLanguageFlag((int)LanguageID.ChineseS);
        model.LangCHT = entry.GetLanguageFlag((int)LanguageID.ChineseT);

        model.PaldeaForm = entry.DisplayedPaldeaForm;
        model.PaldeaGender = entry.DisplayedPaldeaGender;
        model.PaldeaShiny = entry.DisplayedPaldeaShiny != 0;
        model.KitakamiForm = entry.DisplayedKitakamiForm;
        model.KitakamiGender = entry.DisplayedKitakamiGender;
        model.KitakamiShiny = entry.DisplayedKitakamiShiny != 0;
        model.BlueberryForm = entry.DisplayedBlueberryForm;
        model.BlueberryGender = entry.DisplayedBlueberryGender;
        model.BlueberryShiny = entry.DisplayedBlueberryShiny != 0;
    }

    private static void SaveEntry(PokedexSVKitakamiEntryModel model, PokeDexEntry9Kitakami entry)
    {
        entry.SetIsGenderSeen(0, model.SeenMale);
        entry.SetIsGenderSeen(1, model.SeenFemale);
        entry.SetIsGenderSeen(2, model.SeenGenderless);
        entry.SetIsModelSeen(true, model.SeenShiny);

        entry.SetLanguageFlag((int)LanguageID.Japanese, model.LangJPN);
        entry.SetLanguageFlag((int)LanguageID.English, model.LangENG);
        entry.SetLanguageFlag((int)LanguageID.French, model.LangFRE);
        entry.SetLanguageFlag((int)LanguageID.Italian, model.LangITA);
        entry.SetLanguageFlag((int)LanguageID.German, model.LangGER);
        entry.SetLanguageFlag((int)LanguageID.Spanish, model.LangSPA);
        entry.SetLanguageFlag((int)LanguageID.Korean, model.LangKOR);
        entry.SetLanguageFlag((int)LanguageID.ChineseS, model.LangCHS);
        entry.SetLanguageFlag((int)LanguageID.ChineseT, model.LangCHT);

        entry.SetLocalPaldea((byte)model.PaldeaForm, (byte)model.PaldeaGender, model.PaldeaShiny ? (byte)1 : (byte)0);
        entry.SetLocalKitakami((byte)model.KitakamiForm, (byte)model.KitakamiGender, model.KitakamiShiny ? (byte)1 : (byte)0);
        entry.SetLocalBlueberry((byte)model.BlueberryForm, (byte)model.BlueberryGender, model.BlueberryShiny ? (byte)1 : (byte)0);
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    private void ApplyFilter()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            FilteredEntries = new ObservableCollection<PokedexSVKitakamiEntryModel>(AllEntries);
            return;
        }
        FilteredEntries = new ObservableCollection<PokedexSVKitakamiEntryModel>(
            AllEntries.Where(e => e.Label.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
    }

    private void SaveAllEntries()
    {
        foreach (var model in AllEntries)
        {
            var entry = _dex.DexKitakami.Get(model.Species);
            SaveEntry(model, entry);
        }
    }

    private void ReloadAllEntries()
    {
        foreach (var model in AllEntries)
        {
            var entry = _dex.DexKitakami.Get(model.Species);
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

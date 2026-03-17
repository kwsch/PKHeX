using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;
using static PKHeX.Core.Zukan4;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single Gen 4 Pokedex species entry.
/// </summary>
public partial class Pokedex4EntryModel : ObservableObject
{
    public ushort Species { get; }
    public string Label { get; }

    [ObservableProperty]
    private bool _seen;

    [ObservableProperty]
    private bool _caught;

    [ObservableProperty]
    private bool[] _languages;

    public Pokedex4EntryModel(ushort species, string label, bool seen, bool caught, bool[] languages)
    {
        Species = species;
        Label = label;
        _seen = seen;
        _caught = caught;
        _languages = languages;
    }
}

/// <summary>
/// ViewModel for the Gen 4 Pokedex editor.
/// Edits seen/caught/language status per species.
/// </summary>
public partial class Pokedex4ViewModel : SaveEditorViewModelBase
{
    private readonly SAV4 _origin;
    private readonly SAV4 SAV4;
    private const int LangCount = 6;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private int _dexUpgraded;

    public string[] DexModes { get; }

    public ObservableCollection<Pokedex4EntryModel> AllEntries { get; } = [];

    [ObservableProperty]
    private ObservableCollection<Pokedex4EntryModel> _filteredEntries = [];

    public Pokedex4ViewModel(SAV4 sav) : base(sav)
    {
        _origin = sav;
        SAV4 = (SAV4)sav.Clone();
        var speciesNames = GameInfo.Strings.specieslist;

        for (ushort i = 1; i <= sav.MaxSpeciesID; i++)
        {
            var name = i < speciesNames.Length ? speciesNames[i] : $"Species {i}";
            var label = $"{i:000} - {name}";
            var dex = sav.Dex;
            var langs = new bool[LangCount];
            if (dex.HasLanguage(i))
            {
                for (int l = 0; l < LangCount; l++)
                    langs[l] = dex.GetLanguageBitIndex(i, l);
            }
            AllEntries.Add(new Pokedex4EntryModel(i, label, dex.GetSeen(i), dex.GetCaught(i), langs));
        }

        FilteredEntries = new ObservableCollection<Pokedex4EntryModel>(AllEntries);

        string[] dexMode = ["not given", "simple mode", "detect forms", "national dex", "other languages"];
        if (sav is SAV4HGSS)
            dexMode = dexMode.Where((_, i) => i != 2).ToArray();
        DexModes = dexMode;
        _dexUpgraded = Math.Clamp(sav.DexUpgraded, 0, DexModes.Length - 1);
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    private void ApplyFilter()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            FilteredEntries = new ObservableCollection<Pokedex4EntryModel>(AllEntries);
            return;
        }
        FilteredEntries = new ObservableCollection<Pokedex4EntryModel>(
            AllEntries.Where(e => e.Label.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
    }

    [RelayCommand]
    private void CompleteAll()
    {
        var lang = GetGen4LanguageBitIndex(SAV4.Language);
        foreach (var entry in AllEntries)
        {
            entry.Seen = true;
            entry.Caught = true;
            var langs = new bool[LangCount];
            for (int i = 0; i < LangCount; i++)
                langs[i] = true;
            entry.Languages = langs;
        }
    }

    [RelayCommand]
    private void ClearAll()
    {
        foreach (var entry in AllEntries)
        {
            entry.Seen = false;
            entry.Caught = false;
            entry.Languages = new bool[LangCount];
        }
    }

    [RelayCommand]
    private void Save()
    {
        var dex = SAV4.Dex;
        foreach (var entry in AllEntries)
        {
            dex.SetCaught(entry.Species, entry.Caught);
            dex.SetSeen(entry.Species, entry.Seen);
            if (dex.HasLanguage(entry.Species))
            {
                for (int i = 0; i < LangCount; i++)
                    dex.SetLanguageBitIndex(entry.Species, i, entry.Languages[i]);
            }
        }
        if (DexUpgraded >= 0)
            SAV4.DexUpgraded = DexUpgraded;

        _origin.CopyChangesFrom(SAV4);
        Modified = true;
    }
}

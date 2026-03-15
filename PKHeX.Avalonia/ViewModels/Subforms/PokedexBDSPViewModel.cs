using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single BDSP Pokedex species entry.
/// </summary>
public partial class PokedexBDSPEntryModel : ObservableObject
{
    public ushort Species { get; }
    public string Label { get; }

    [ObservableProperty] private int _state;
    [ObservableProperty] private bool _seenMale;
    [ObservableProperty] private bool _seenFemale;
    [ObservableProperty] private bool _seenMaleShiny;
    [ObservableProperty] private bool _seenFemaleShiny;

    // Language flags
    [ObservableProperty] private bool _langJpn;
    [ObservableProperty] private bool _langEng;
    [ObservableProperty] private bool _langFre;
    [ObservableProperty] private bool _langIta;
    [ObservableProperty] private bool _langGer;
    [ObservableProperty] private bool _langSpa;
    [ObservableProperty] private bool _langKor;
    [ObservableProperty] private bool _langChs;
    [ObservableProperty] private bool _langCht;

    public PokedexBDSPEntryModel(ushort species, string label)
    {
        Species = species;
        Label = label;
    }
}

/// <summary>
/// ViewModel for the BDSP Pokedex editor.
/// </summary>
public partial class PokedexBDSPViewModel : SaveEditorViewModelBase
{
    private readonly SaveFile _origin;
    private readonly SAV8BS _sav;
    private readonly Zukan8b _zukan;

    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private bool _nationalDex;

    public string[] StateChoices { get; } = ["None", "Heard", "Seen", "Caught"];

    public ObservableCollection<PokedexBDSPEntryModel> AllEntries { get; } = [];

    [ObservableProperty]
    private ObservableCollection<PokedexBDSPEntryModel> _filteredEntries = [];

    public PokedexBDSPViewModel(SAV8BS sav) : base(sav)
    {
        _sav = (SAV8BS)(_origin = sav).Clone();
        _zukan = _sav.Zukan;
        _nationalDex = _zukan.HasNationalDex;

        var speciesNames = GameInfo.Strings.specieslist;
        for (ushort i = 1; i <= _sav.MaxSpeciesID; i++)
        {
            var name = i < speciesNames.Length ? speciesNames[i] : $"Species {i}";
            var label = $"{i:000} - {name}";

            _zukan.GetGenderFlags(i, out var m, out var f, out var ms, out var fs);

            var entry = new PokedexBDSPEntryModel(i, label)
            {
                State = (int)_zukan.GetState(i),
                SeenMale = m,
                SeenFemale = f,
                SeenMaleShiny = ms,
                SeenFemaleShiny = fs,
                LangJpn = _zukan.GetLanguageFlag(i, (int)LanguageID.Japanese),
                LangEng = _zukan.GetLanguageFlag(i, (int)LanguageID.English),
                LangFre = _zukan.GetLanguageFlag(i, (int)LanguageID.French),
                LangIta = _zukan.GetLanguageFlag(i, (int)LanguageID.Italian),
                LangGer = _zukan.GetLanguageFlag(i, (int)LanguageID.German),
                LangSpa = _zukan.GetLanguageFlag(i, (int)LanguageID.Spanish),
                LangKor = _zukan.GetLanguageFlag(i, (int)LanguageID.Korean),
                LangChs = _zukan.GetLanguageFlag(i, (int)LanguageID.ChineseS),
                LangCht = _zukan.GetLanguageFlag(i, (int)LanguageID.ChineseT),
            };
            AllEntries.Add(entry);
        }

        FilteredEntries = new ObservableCollection<PokedexBDSPEntryModel>(AllEntries);
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    private void ApplyFilter()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            FilteredEntries = new ObservableCollection<PokedexBDSPEntryModel>(AllEntries);
            return;
        }
        FilteredEntries = new ObservableCollection<PokedexBDSPEntryModel>(
            AllEntries.Where(e => e.Label.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
    }

    [RelayCommand]
    private void SeenAll()
    {
        SaveAllEntries();
        _zukan.SetAllSeen(false);
        ReloadAllEntries();
    }

    [RelayCommand]
    private void CaughtAll()
    {
        SaveAllEntries();
        _zukan.CaughtAll();
        ReloadAllEntries();
    }

    [RelayCommand]
    private void CaughtNone()
    {
        SaveAllEntries();
        _zukan.CaughtNone();
        ReloadAllEntries();
    }

    [RelayCommand]
    private void CompleteDex()
    {
        SaveAllEntries();
        _zukan.CompleteDex(false);
        ReloadAllEntries();
    }

    private void ReloadAllEntries()
    {
        foreach (var entry in AllEntries)
        {
            var s = entry.Species;
            entry.State = (int)_zukan.GetState(s);
            _zukan.GetGenderFlags(s, out var m, out var f, out var ms, out var fs);
            entry.SeenMale = m;
            entry.SeenFemale = f;
            entry.SeenMaleShiny = ms;
            entry.SeenFemaleShiny = fs;
            entry.LangJpn = _zukan.GetLanguageFlag(s, (int)LanguageID.Japanese);
            entry.LangEng = _zukan.GetLanguageFlag(s, (int)LanguageID.English);
            entry.LangFre = _zukan.GetLanguageFlag(s, (int)LanguageID.French);
            entry.LangIta = _zukan.GetLanguageFlag(s, (int)LanguageID.Italian);
            entry.LangGer = _zukan.GetLanguageFlag(s, (int)LanguageID.German);
            entry.LangSpa = _zukan.GetLanguageFlag(s, (int)LanguageID.Spanish);
            entry.LangKor = _zukan.GetLanguageFlag(s, (int)LanguageID.Korean);
            entry.LangChs = _zukan.GetLanguageFlag(s, (int)LanguageID.ChineseS);
            entry.LangCht = _zukan.GetLanguageFlag(s, (int)LanguageID.ChineseT);
        }
        ApplyFilter();
    }

    private void SaveAllEntries()
    {
        foreach (var entry in AllEntries)
        {
            var s = entry.Species;
            if (s > 493)
                continue;

            _zukan.SetState(s, (ZukanState8b)entry.State);
            _zukan.SetGenderFlags(s, entry.SeenMale, entry.SeenFemale, entry.SeenMaleShiny, entry.SeenFemaleShiny);
            _zukan.SetLanguageFlag(s, (int)LanguageID.Japanese, entry.LangJpn);
            _zukan.SetLanguageFlag(s, (int)LanguageID.English, entry.LangEng);
            _zukan.SetLanguageFlag(s, (int)LanguageID.French, entry.LangFre);
            _zukan.SetLanguageFlag(s, (int)LanguageID.Italian, entry.LangIta);
            _zukan.SetLanguageFlag(s, (int)LanguageID.German, entry.LangGer);
            _zukan.SetLanguageFlag(s, (int)LanguageID.Spanish, entry.LangSpa);
            _zukan.SetLanguageFlag(s, (int)LanguageID.Korean, entry.LangKor);
            _zukan.SetLanguageFlag(s, (int)LanguageID.ChineseS, entry.LangChs);
            _zukan.SetLanguageFlag(s, (int)LanguageID.ChineseT, entry.LangCht);
        }
    }

    [RelayCommand]
    private void Save()
    {
        SaveAllEntries();
        _zukan.HasNationalDex = NationalDex;
        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }
}

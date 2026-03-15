using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single SWSH Pokedex species entry.
/// </summary>
public partial class PokedexSWSHEntryModel : ObservableObject
{
    public string Label { get; }
    public Zukan8EntryInfo Info { get; }

    [ObservableProperty] private bool _caught;
    [ObservableProperty] private bool _gigantamaxed;
    [ObservableProperty] private bool _displayDynamax;
    [ObservableProperty] private bool _displayShiny;
    [ObservableProperty] private int _genderDisplayed;
    [ObservableProperty] private uint _formDisplayed;
    [ObservableProperty] private uint _battledCount;

    // Language flags
    [ObservableProperty] private bool _lang1;
    [ObservableProperty] private bool _lang2;
    [ObservableProperty] private bool _lang3;
    [ObservableProperty] private bool _lang4;
    [ObservableProperty] private bool _lang5;
    [ObservableProperty] private bool _lang6;
    [ObservableProperty] private bool _lang7;
    [ObservableProperty] private bool _lang8;
    [ObservableProperty] private bool _lang9;

    public PokedexSWSHEntryModel(string label, Zukan8EntryInfo info)
    {
        Label = label;
        Info = info;
    }
}

/// <summary>
/// ViewModel for the Sword/Shield Pokedex editor.
/// </summary>
public partial class PokedexSWSHViewModel : SaveEditorViewModelBase
{
    private readonly SAV8SWSH _origin;
    private readonly SAV8SWSH _sav;
    private readonly Zukan8 _dex;

    [ObservableProperty] private string _searchText = string.Empty;

    public ObservableCollection<PokedexSWSHEntryModel> AllEntries { get; } = [];

    [ObservableProperty]
    private ObservableCollection<PokedexSWSHEntryModel> _filteredEntries = [];

    [ObservableProperty] private PokedexSWSHEntryModel? _selectedEntry;

    public PokedexSWSHViewModel(SAV8SWSH sav) : base(sav)
    {
        _sav = (SAV8SWSH)(_origin = sav).Clone();
        _dex = _sav.Blocks.Zukan;

        var indexes = Zukan8.GetRawIndexes(PersonalTable.SWSH, _dex.GetRevision(), Zukan8Index.TotalCount);
        var speciesNames = GameInfo.Strings.Species;
        var sorted = indexes.OrderBy(z => z.GetEntryName(speciesNames)).ToArray();

        foreach (var info in sorted)
        {
            var entry = info.Entry;
            if (entry.DexType == Zukan8Type.None)
                continue;

            var label = info.GetEntryName(speciesNames);
            var model = new PokedexSWSHEntryModel(label, info)
            {
                Caught = _dex.GetCaught(entry),
                Gigantamaxed = _dex.GetCaughtGigantamaxed(entry),
                DisplayDynamax = _dex.GetDisplayDynamaxInstead(entry),
                DisplayShiny = _dex.GetDisplayShiny(entry),
                GenderDisplayed = (int)_dex.GetGenderDisplayed(entry),
                FormDisplayed = _dex.GetFormDisplayed(entry),
                BattledCount = _dex.GetBattledCount(entry),
            };

            // Language flags
            model.Lang1 = _dex.GetIsLanguageIndexObtained(entry, 0);
            model.Lang2 = _dex.GetIsLanguageIndexObtained(entry, 1);
            model.Lang3 = _dex.GetIsLanguageIndexObtained(entry, 2);
            model.Lang4 = _dex.GetIsLanguageIndexObtained(entry, 3);
            model.Lang5 = _dex.GetIsLanguageIndexObtained(entry, 4);
            model.Lang6 = _dex.GetIsLanguageIndexObtained(entry, 5);
            model.Lang7 = _dex.GetIsLanguageIndexObtained(entry, 6);
            model.Lang8 = _dex.GetIsLanguageIndexObtained(entry, 7);
            model.Lang9 = _dex.GetIsLanguageIndexObtained(entry, 8);

            AllEntries.Add(model);
        }

        FilteredEntries = new ObservableCollection<PokedexSWSHEntryModel>(AllEntries);
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    private void ApplyFilter()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            FilteredEntries = new ObservableCollection<PokedexSWSHEntryModel>(AllEntries);
            return;
        }
        FilteredEntries = new ObservableCollection<PokedexSWSHEntryModel>(
            AllEntries.Where(e => e.Label.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
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

    private void ReloadAllEntries()
    {
        foreach (var model in AllEntries)
        {
            var entry = model.Info.Entry;
            if (entry.DexType == Zukan8Type.None)
                continue;

            model.Caught = _dex.GetCaught(entry);
            model.Gigantamaxed = _dex.GetCaughtGigantamaxed(entry);
            model.DisplayDynamax = _dex.GetDisplayDynamaxInstead(entry);
            model.DisplayShiny = _dex.GetDisplayShiny(entry);
            model.GenderDisplayed = (int)_dex.GetGenderDisplayed(entry);
            model.FormDisplayed = _dex.GetFormDisplayed(entry);
            model.BattledCount = _dex.GetBattledCount(entry);

            model.Lang1 = _dex.GetIsLanguageIndexObtained(entry, 0);
            model.Lang2 = _dex.GetIsLanguageIndexObtained(entry, 1);
            model.Lang3 = _dex.GetIsLanguageIndexObtained(entry, 2);
            model.Lang4 = _dex.GetIsLanguageIndexObtained(entry, 3);
            model.Lang5 = _dex.GetIsLanguageIndexObtained(entry, 4);
            model.Lang6 = _dex.GetIsLanguageIndexObtained(entry, 5);
            model.Lang7 = _dex.GetIsLanguageIndexObtained(entry, 6);
            model.Lang8 = _dex.GetIsLanguageIndexObtained(entry, 7);
            model.Lang9 = _dex.GetIsLanguageIndexObtained(entry, 8);
        }
        ApplyFilter();
    }

    private void SaveAllEntries()
    {
        foreach (var model in AllEntries)
        {
            var entry = model.Info.Entry;
            if (entry.DexType == Zukan8Type.None)
                continue;

            _dex.SetCaught(entry, model.Caught);
            _dex.SetCaughtGigantamax(entry, model.Gigantamaxed);
            _dex.SetDisplayDynamaxInstead(entry, model.DisplayDynamax);
            _dex.SetDisplayShiny(entry, model.DisplayShiny);
            _dex.SetGenderDisplayed(entry, (uint)model.GenderDisplayed);
            _dex.SetFormDisplayed(entry, model.FormDisplayed);
            _dex.SetBattledCount(entry, model.BattledCount);

            _dex.SetIsLanguageIndexObtained(entry, 0, model.Lang1);
            _dex.SetIsLanguageIndexObtained(entry, 1, model.Lang2);
            _dex.SetIsLanguageIndexObtained(entry, 2, model.Lang3);
            _dex.SetIsLanguageIndexObtained(entry, 3, model.Lang4);
            _dex.SetIsLanguageIndexObtained(entry, 4, model.Lang5);
            _dex.SetIsLanguageIndexObtained(entry, 5, model.Lang6);
            _dex.SetIsLanguageIndexObtained(entry, 6, model.Lang7);
            _dex.SetIsLanguageIndexObtained(entry, 7, model.Lang8);
            _dex.SetIsLanguageIndexObtained(entry, 8, model.Lang9);
        }
    }

    [RelayCommand]
    private void Save()
    {
        SaveAllEntries();
        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }
}

using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class PokedexGen9EditorViewModel : ViewModelBase
{
    private readonly SAV9SV _sav;
    private readonly Zukan9 _zukan;
    
    public ObservableCollection<ComboItem> SpeciesList { get; } = [];
    public ObservableCollection<ComboItem> FilteredSpeciesList { get; private set; } = [];

    private string _searchText = "";
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
                FilterList();
        }
    }

    private ComboItem? _selectedSpecies;
    public ComboItem? SelectedSpecies
    {
        get => _selectedSpecies;
        set
        {
            if (SetProperty(ref _selectedSpecies, value))
                LoadEntry();
        }
    }
    
    // Entry Properties
    [ObservableProperty] private bool _isCaught; // Derived or manually managed?
    [ObservableProperty] private bool _isSeenMale;
    [ObservableProperty] private bool _isSeenFemale;
    [ObservableProperty] private bool _isSeenGenderless;
    [ObservableProperty] private bool _isSeenShiny;
    [ObservableProperty] private bool _isNew;
    [ObservableProperty] private int _displayGender; // 0=M, 1=F, 2=G
    [ObservableProperty] private bool _displayShiny;
    [ObservableProperty] private int _displayForm;
    [ObservableProperty] private bool _displayGenderDiff;
    
    // Languages
    [ObservableProperty] private bool _langJPN;
    [ObservableProperty] private bool _langENG;
    [ObservableProperty] private bool _langFRE;
    [ObservableProperty] private bool _langITA;
    [ObservableProperty] private bool _langGER;
    [ObservableProperty] private bool _langSPA;
    [ObservableProperty] private bool _langKOR;
    [ObservableProperty] private bool _langCHS;
    [ObservableProperty] private bool _langCHT;

    public ObservableCollection<string> Forms { get; } = [];
    
    public PokedexGen9EditorViewModel(SAV9SV sav)
    {
        _sav = sav;
        _zukan = sav.Blocks.Zukan;
        
        LoadSpeciesList();
    }

    private void LoadSpeciesList()
    {
        SpeciesList.Clear();
        var species = GameInfo.FilteredSources.Species
            .Where(z => z.Value <= (int)Species.IronLeaves) // Limit to Gen 9 max? GameInfo usually has correct limits
            .ToList();

        foreach (var s in species)
            SpeciesList.Add(s);
            
        FilterList();
        if (FilteredSpeciesList.Count > 0)
            SelectedSpecies = FilteredSpeciesList[0];
    }

    private void FilterList()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            FilteredSpeciesList = new ObservableCollection<ComboItem>(SpeciesList);
        }
        else
        {
            FilteredSpeciesList = new ObservableCollection<ComboItem>(
                SpeciesList.Where(x => x.Text.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
        }
        OnPropertyChanged(nameof(FilteredSpeciesList));
    }

    private void LoadEntry()
    {
        if (SelectedSpecies is null) return;
        
        ushort species = (ushort)SelectedSpecies.Value;
        // Paldea Dex is primary? Or check which dex it is in?
        // Zukan9 usually abstracts accessing the correct "Dex" via Get?
        // Actually SAV_PokedexSV uses `SAV.Zukan.DexPaldea.Get(species)` directly.
        // It seems most data is in DexPaldea even for DLC mons in SV?
        
        var entry = _zukan.DexPaldea.Get(species);
        
        // Load Forms
        Forms.Clear();
        var formList = FormConverter.GetFormList(species, GameInfo.Strings.Types, GameInfo.Strings.forms, GameInfo.GenderSymbolASCII, EntityContext.Gen9);
        foreach(var f in formList) Forms.Add(f);
        
        // Properties
        IsNew = entry.GetDisplayIsNew();
        IsSeenMale = entry.GetIsGenderSeen(0);
        IsSeenFemale = entry.GetIsGenderSeen(1);
        IsSeenGenderless = entry.GetIsGenderSeen(2);
        IsSeenShiny = entry.GetSeenIsShiny();
        
        DisplayGender = (int)entry.GetDisplayGender();
        DisplayShiny = entry.GetDisplayIsShiny();
        DisplayGenderDiff = entry.GetDisplayGenderIsDifferent();
        DisplayForm = (int)entry.GetDisplayForm();
        if (DisplayForm >= Forms.Count) DisplayForm = 0;

        // Languages
        LangJPN = entry.GetLanguageFlag((int)LanguageID.Japanese);
        LangENG = entry.GetLanguageFlag((int)LanguageID.English);
        LangFRE = entry.GetLanguageFlag((int)LanguageID.French);
        LangITA = entry.GetLanguageFlag((int)LanguageID.Italian);
        LangGER = entry.GetLanguageFlag((int)LanguageID.German);
        LangSPA = entry.GetLanguageFlag((int)LanguageID.Spanish);
        LangKOR = entry.GetLanguageFlag((int)LanguageID.Korean);
        LangCHS = entry.GetLanguageFlag((int)LanguageID.ChineseS);
        LangCHT = entry.GetLanguageFlag((int)LanguageID.ChineseT);
    }

    [RelayCommand]
    private void SaveCurrent()
    {
        if (SelectedSpecies is null) return;
        ushort species = (ushort)SelectedSpecies.Value;
        var entry = _zukan.DexPaldea.Get(species);

        entry.SetDisplayIsNew(IsNew);
        entry.SetIsGenderSeen(0, IsSeenMale);
        entry.SetIsGenderSeen(1, IsSeenFemale);
        entry.SetIsGenderSeen(2, IsSeenGenderless);
        entry.SetSeenIsShiny(IsSeenShiny);
        
        entry.SetDisplayGender(DisplayGender);
        entry.SetDisplayIsShiny(DisplayShiny);
        entry.SetDisplayGenderIsDifferent(DisplayGenderDiff);
        entry.SetDisplayForm((uint)DisplayForm);
        
        entry.SetLanguageFlag((int)LanguageID.Japanese, LangJPN);
        entry.SetLanguageFlag((int)LanguageID.English, LangENG);
        entry.SetLanguageFlag((int)LanguageID.French, LangFRE);
        entry.SetLanguageFlag((int)LanguageID.Italian, LangITA);
        entry.SetLanguageFlag((int)LanguageID.German, LangGER);
        entry.SetLanguageFlag((int)LanguageID.Spanish, LangSPA);
        entry.SetLanguageFlag((int)LanguageID.Korean, LangKOR);
        entry.SetLanguageFlag((int)LanguageID.ChineseS, LangCHS);
        entry.SetLanguageFlag((int)LanguageID.ChineseT, LangCHT);
        
        _sav.State.Edited = true;
    }

    // Batch commands (Apply to current only, or all?)
    // Typically WinForms has "Seen None/All" for CURRENT species, and "Modify All" menu for entire dex.
    
    [RelayCommand]
    private void SeenAll()
    {
        if (SelectedSpecies is null) return;
        _zukan.SetDexEntryAll((ushort)SelectedSpecies.Value, false);
        LoadEntry();
    }
    
    [RelayCommand]
    private void CaughtAll() // Actually Caught implies Seen
    {
        // ... Zukan9Helper?
        // WinForms uses Dex.CaughtAll(shiny) for ALL species.
        // For single species, it sets flags manually.
    }
}

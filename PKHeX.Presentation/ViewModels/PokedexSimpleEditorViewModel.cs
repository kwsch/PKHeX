using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class PokedexSimpleEditorViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    public int MaxSpeciesID { get; }

    public ObservableCollection<PokedexEntryViewModel> Entries { get; } = [];
    public ObservableCollection<PokedexEntryViewModel> FilteredEntries { get; private set; } = [];

    private string _searchText = "";
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
                FilterEntries();
        }
    }

    public PokedexSimpleEditorViewModel(SaveFile sav)
    {
        _sav = sav;
        MaxSpeciesID = sav.MaxSpeciesID;
        Load();
    }

    private void Load()
    {
        Entries.Clear();
        var speciesNames = GameInfo.Strings.Species;
        for (int i = 1; i <= MaxSpeciesID; i++)
        {
            if (i >= speciesNames.Count) break;
            var name = speciesNames[i];
            var seen = _sav.GetSeen((ushort)i);
            var caught = _sav.GetCaught((ushort)i);
            Entries.Add(new PokedexEntryViewModel((ushort)i, name, seen, caught));
        }
        FilterEntries();
    }

    private void FilterEntries()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            FilteredEntries = new ObservableCollection<PokedexEntryViewModel>(Entries);
        }
        else
        {
            FilteredEntries = new ObservableCollection<PokedexEntryViewModel>(
                Entries.Where(x => x.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
        }
        OnPropertyChanged(nameof(FilteredEntries));
    }

    [RelayCommand]
    private void Save()
    {
        foreach (var entry in Entries)
        {
            _sav.SetSeen(entry.Species, entry.IsSeen);
            _sav.SetCaught(entry.Species, entry.IsCaught);
        }
        _sav.State.Edited = true;
    }

    [RelayCommand]
    private void SeenAll()
    {
        foreach (var entry in Entries)
            entry.IsSeen = true;
    }

    [RelayCommand]
    private void SeenNone()
    {
        foreach (var entry in Entries)
            entry.IsSeen = false;
    }

    [RelayCommand]
    private void CaughtAll()
    {
        foreach (var entry in Entries)
            entry.IsCaught = true;
    }

    [RelayCommand]
    private void CaughtNone()
    {
        foreach (var entry in Entries)
            entry.IsCaught = false;
    }
}

public partial class PokedexEntryViewModel : ViewModelBase
{
    public ushort Species { get; }
    public string Name { get; }

    [ObservableProperty]
    private bool _isSeen;

    [ObservableProperty]
    private bool _isCaught;

    public PokedexEntryViewModel(ushort species, string name, bool seen, bool caught)
    {
        Species = species;
        Name = name;
        IsSeen = seen;
        IsCaught = caught;
    }
    
    partial void OnIsCaughtChanged(bool value)
    {
        if (value && !IsSeen)
            IsSeen = true;
    }
}

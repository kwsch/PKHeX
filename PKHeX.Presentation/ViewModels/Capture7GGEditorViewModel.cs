using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Capture records editor for Let's Go Pikachu/Eevee.
/// Tracks caught and transferred counts per species.
/// </summary>
public partial class Capture7GGEditorViewModel : ViewModelBase
{
    private readonly SAV7b _sav;
    private readonly SAV7b _clone;
    private readonly Zukan7b _dex;
    private readonly CaptureRecords _captured;

    [ObservableProperty]
    private ObservableCollection<ComboItem> _speciesList = [];

    [ObservableProperty]
    private ObservableCollection<CaptureEntryViewModel> _entries = [];

    [ObservableProperty]
    private CaptureEntryViewModel? _selectedEntry;

    [ObservableProperty]
    private int _selectedSpeciesIndex;

    [ObservableProperty]
    private uint _capturedCount;

    [ObservableProperty]
    private uint _transferredCount;

    [ObservableProperty]
    private uint _totalCaptured;

    [ObservableProperty]
    private uint _totalTransferred;

    private ushort _currentIndex;

    public Capture7GGEditorViewModel(SAV7b sav)
    {
        _sav = sav;
        _clone = (SAV7b)sav.Clone();
        _dex = _clone.Blocks.Zukan;
        _captured = _clone.Blocks.Captured;

        LoadSpeciesList();
        LoadEntries();
        LoadTotals();

        if (Entries.Count > 0)
        {
            SelectedEntry = Entries[0];
            _currentIndex = 0;
        }
    }

    private static bool IsLegalSpecies(int species) => species is >= 1 and (<= 151 or 808 or 809);

    private void LoadSpeciesList()
    {
        SpeciesList.Clear();
        foreach (var species in GameInfo.FilteredSources.Species.Where(z => IsLegalSpecies(z.Value)))
            SpeciesList.Add(species);
    }

    private void LoadEntries()
    {
        Entries.Clear();
        foreach (var species in SpeciesList.OrderBy(s => s.Value))
        {
            var index = CaptureRecords.GetSpeciesIndex((ushort)species.Value);
            var captured = _captured.GetCapturedCountIndex(index);
            var transferred = _captured.GetTransferredCountIndex(index);
            Entries.Add(new CaptureEntryViewModel((ushort)species.Value, species.Text, captured, transferred));
        }
    }

    private void LoadTotals()
    {
        TotalCaptured = _captured.TotalCaptured;
        TotalTransferred = _captured.TotalTransferred;
    }

    partial void OnSelectedEntryChanged(CaptureEntryViewModel? value)
    {
        if (value is null)
            return;

        SaveCurrentEntry();

        _currentIndex = CaptureRecords.GetSpeciesIndex(value.Species);
        CapturedCount = _captured.GetCapturedCountIndex(_currentIndex);
        TransferredCount = _captured.GetTransferredCountIndex(_currentIndex);

        // Update species combo
        for (int i = 0; i < SpeciesList.Count; i++)
        {
            if (SpeciesList[i].Value == value.Species)
            {
                SelectedSpeciesIndex = i;
                break;
            }
        }
    }

    partial void OnSelectedSpeciesIndexChanged(int value)
    {
        if (value < 0 || value >= SpeciesList.Count)
            return;

        var species = (ushort)SpeciesList[value].Value;
        var entry = Entries.FirstOrDefault(e => e.Species == species);
        if (entry is not null && entry != SelectedEntry)
        {
            SelectedEntry = entry;
        }
    }

    private void SaveCurrentEntry()
    {
        if (_currentIndex > CaptureRecords.MaxIndex)
            return;

        _captured.SetCapturedCountIndex(_currentIndex, CapturedCount);
        _captured.SetTransferredCountIndex(_currentIndex, TransferredCount);

        // Update entry in list
        var species = CaptureRecords.GetIndexSpecies(_currentIndex);
        var entry = Entries.FirstOrDefault(e => e.Species == species);
        if (entry is not null)
        {
            entry.CapturedCount = CapturedCount;
            entry.TransferredCount = TransferredCount;
        }
    }

    [RelayCommand]
    private void SetAllCaptured()
    {
        SaveCurrentEntry();
        _captured.TotalCaptured = TotalCaptured;
        _captured.TotalTransferred = TotalTransferred;
        _captured.SetAllCaptured(CapturedCount, _dex);
        _captured.SetAllTransferred(TransferredCount, _dex);
        LoadEntries();

        // Re-select current entry
        if (_currentIndex <= CaptureRecords.MaxIndex)
        {
            var species = CaptureRecords.GetIndexSpecies(_currentIndex);
            SelectedEntry = Entries.FirstOrDefault(e => e.Species == species);
        }
    }

    [RelayCommand]
    private void SumTotals()
    {
        SaveCurrentEntry();
        TotalCaptured = _captured.CalculateTotalCaptured();
        TotalTransferred = _captured.CalculateTotalTransferred();
    }

    [RelayCommand]
    private void Save()
    {
        SaveCurrentEntry();
        _captured.TotalCaptured = TotalCaptured;
        _captured.TotalTransferred = TotalTransferred;
        _sav.CopyChangesFrom(_clone);
    }
}

public partial class CaptureEntryViewModel : ViewModelBase
{
    public ushort Species { get; }
    public string SpeciesName { get; }
    public string DisplayText => $"{Species:000}: {SpeciesName}";

    [ObservableProperty]
    private uint _capturedCount;

    [ObservableProperty]
    private uint _transferredCount;

    public CaptureEntryViewModel(ushort species, string name, uint captured, uint transferred)
    {
        Species = species;
        SpeciesName = name;
        _capturedCount = captured;
        _transferredCount = transferred;
    }
}

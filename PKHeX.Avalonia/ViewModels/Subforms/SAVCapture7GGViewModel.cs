using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the Let's Go capture record editor.
/// Edits per-species captured/transferred counts and totals.
/// </summary>
public partial class SAVCapture7GGViewModel : SaveEditorViewModelBase
{
    private readonly SaveFile _origin;
    private readonly SAV7b _sav;
    private readonly CaptureRecords _captured;
    private readonly Zukan7b _dex;
    private bool _loading;
    private ushort _index;

    public ObservableCollection<string> SpeciesEntries { get; } = [];
    public List<ComboItem> SpeciesList { get; }

    [ObservableProperty] private int _selectedSpeciesIndex = -1;
    [ObservableProperty] private int _selectedSpeciesCombo;

    [ObservableProperty] private uint _speciesCaptured;
    [ObservableProperty] private uint _speciesTransferred;
    [ObservableProperty] private uint _totalCaptured;
    [ObservableProperty] private uint _totalTransferred;

    public SAVCapture7GGViewModel(SAV7b sav) : base(sav)
    {
        _sav = (SAV7b)(_origin = sav).Clone();
        _dex = _sav.Blocks.Zukan;
        _captured = _sav.Blocks.Captured;

        var species = GameInfo.FilteredSources.Species.Where(z => IsLegalSpecies(z.Value)).ToList();
        SpeciesList = species;

        foreach (var (text, value) in species.OrderBy(z => z.Value))
            SpeciesEntries.Add($"{value:000}: {text}");

        TotalCaptured = _captured.TotalCaptured;
        TotalTransferred = _captured.TotalTransferred;

        _loading = true;
        SelectedSpeciesIndex = 0;
        _index = 0;
        LoadEntry();
        _loading = false;
    }

    private static bool IsLegalSpecies(int species) => species is >= 1 and (<= 151 or 808 or 809);

    partial void OnSelectedSpeciesIndexChanged(int value)
    {
        if (_loading || value < 0)
            return;
        SaveEntry();
        _index = (ushort)value;
        _loading = true;
        SelectedSpeciesCombo = (int)CaptureRecords.GetIndexSpecies(_index);
        LoadEntry();
        _loading = false;
    }

    partial void OnSelectedSpeciesComboChanged(int value)
    {
        if (_loading || value <= 0)
            return;
        SaveEntry();
        _index = CaptureRecords.GetSpeciesIndex((ushort)value);
        _loading = true;
        if (_index <= CaptureRecords.MaxIndex)
            SelectedSpeciesIndex = _index;
        LoadEntry();
        _loading = false;
    }

    private void LoadEntry()
    {
        if (_index > CaptureRecords.MaxIndex)
            return;
        SpeciesCaptured = _captured.GetCapturedCountIndex(_index);
        SpeciesTransferred = _captured.GetTransferredCountIndex(_index);
    }

    private void SaveEntry()
    {
        if (_index > CaptureRecords.MaxIndex)
            return;
        _captured.SetCapturedCountIndex(_index, SpeciesCaptured);
        _captured.SetTransferredCountIndex(_index, SpeciesTransferred);
    }

    [RelayCommand]
    private void ModifyAll()
    {
        SaveTotals();
        _captured.SetAllCaptured(SpeciesCaptured, _dex);
        _captured.SetAllTransferred(SpeciesTransferred, _dex);
        LoadEntry();
    }

    [RelayCommand]
    private void SumTotal()
    {
        SaveEntry();
        TotalCaptured = _captured.CalculateTotalCaptured();
        TotalTransferred = _captured.CalculateTotalTransferred();
    }

    private void SaveTotals()
    {
        _captured.TotalCaptured = TotalCaptured;
        _captured.TotalTransferred = TotalTransferred;
    }

    [RelayCommand]
    private void Save()
    {
        SaveEntry();
        SaveTotals();
        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the Gen 7 Hall of Fame viewer/editor.
/// Shows first clear (6 entries) and current team (6 entries),
/// plus starter EC for USUM.
/// </summary>
public partial class SAVHallOfFame7ViewModel : SaveEditorViewModelBase
{
    private readonly SAV7 _origin;
    private readonly SAV7 _sav;
    private readonly HallOfFame7 _fame;

    public List<ComboItem> SpeciesList { get; }

    // First clear team (6 species)
    [ObservableProperty] private int _first1;
    [ObservableProperty] private int _first2;
    [ObservableProperty] private int _first3;
    [ObservableProperty] private int _first4;
    [ObservableProperty] private int _first5;
    [ObservableProperty] private int _first6;

    // Current team (6 species)
    [ObservableProperty] private int _current1;
    [ObservableProperty] private int _current2;
    [ObservableProperty] private int _current3;
    [ObservableProperty] private int _current4;
    [ObservableProperty] private int _current5;
    [ObservableProperty] private int _current6;

    // Starter EC (USUM only)
    [ObservableProperty] private string _starterEc = "00000000";
    [ObservableProperty] private bool _showStarterEc;

    public SAVHallOfFame7ViewModel(SAV7 sav) : base(sav)
    {
        _origin = sav;
        _sav = (SAV7)sav.Clone();
        _fame = _sav.EventWork.Fame;
        SpeciesList = GameInfo.FilteredSources.Species.ToList();
        ShowStarterEc = sav is SAV7USUM;

        LoadData();
    }

    private void LoadData()
    {
        First1 = _fame.GetEntry(0);
        First2 = _fame.GetEntry(1);
        First3 = _fame.GetEntry(2);
        First4 = _fame.GetEntry(3);
        First5 = _fame.GetEntry(4);
        First6 = _fame.GetEntry(5);

        Current1 = _fame.GetEntry(6);
        Current2 = _fame.GetEntry(7);
        Current3 = _fame.GetEntry(8);
        Current4 = _fame.GetEntry(9);
        Current5 = _fame.GetEntry(10);
        Current6 = _fame.GetEntry(11);

        if (_sav is SAV7USUM uu)
            StarterEc = uu.Misc.StarterEncryptionConstant.ToString("X8");
    }

    [RelayCommand]
    private void Save()
    {
        _fame.SetEntry(0, (ushort)First1);
        _fame.SetEntry(1, (ushort)First2);
        _fame.SetEntry(2, (ushort)First3);
        _fame.SetEntry(3, (ushort)First4);
        _fame.SetEntry(4, (ushort)First5);
        _fame.SetEntry(5, (ushort)First6);

        _fame.SetEntry(6, (ushort)Current1);
        _fame.SetEntry(7, (ushort)Current2);
        _fame.SetEntry(8, (ushort)Current3);
        _fame.SetEntry(9, (ushort)Current4);
        _fame.SetEntry(10, (ushort)Current5);
        _fame.SetEntry(11, (ushort)Current6);

        if (_sav is SAV7USUM uu)
        {
            if (uint.TryParse(StarterEc, System.Globalization.NumberStyles.HexNumber, null, out var ec))
                uu.Misc.StarterEncryptionConstant = ec;
        }

        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }
}

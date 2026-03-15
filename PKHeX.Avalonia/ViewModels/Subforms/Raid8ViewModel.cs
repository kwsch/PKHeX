using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single raid den entry.
/// </summary>
public partial class RaidDen8Model : ObservableObject
{
    public int Index { get; }
    public string Label { get; }
    public RaidSpawnDetail Detail { get; }

    [ObservableProperty] private bool _isActive;
    [ObservableProperty] private bool _isEvent;
    [ObservableProperty] private bool _isRare;
    [ObservableProperty] private int _stars;
    [ObservableProperty] private int _randRoll;

    public RaidDen8Model(int index, RaidSpawnDetail detail)
    {
        Index = index;
        Label = $"Den {index + 1:000}";
        Detail = detail;
        _isActive = detail.IsActive;
        _isEvent = detail.IsEvent;
        _isRare = detail.IsRare;
        _stars = detail.Stars;
        _randRoll = detail.RandRoll;
    }
}

/// <summary>
/// ViewModel for the SwSh Raid Den editor.
/// </summary>
public partial class Raid8ViewModel : SaveEditorViewModelBase
{
    private readonly SaveFile _origin;
    private readonly SAV8SWSH _sav;
    private readonly RaidSpawnList8 _raids;

    public ObservableCollection<RaidDen8Model> Dens { get; } = [];

    [ObservableProperty] private RaidDen8Model? _selectedDen;

    public Raid8ViewModel(SAV8SWSH sav, MaxRaidOrigin raidOrigin) : base(sav)
    {
        _sav = (SAV8SWSH)(_origin = sav).Clone();
        _raids = raidOrigin switch
        {
            MaxRaidOrigin.Galar => _sav.RaidGalar,
            MaxRaidOrigin.IsleOfArmor => _sav.RaidArmor,
            MaxRaidOrigin.CrownTundra => _sav.RaidCrown,
            _ => throw new ArgumentOutOfRangeException(nameof(raidOrigin)),
        };

        for (int i = 0; i < _raids.CountUsed; i++)
        {
            var detail = _raids.GetRaid(i);
            Dens.Add(new RaidDen8Model(i, detail));
        }

        if (Dens.Count > 0)
            SelectedDen = Dens[0];
    }

    [RelayCommand]
    private void Save()
    {
        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }
}

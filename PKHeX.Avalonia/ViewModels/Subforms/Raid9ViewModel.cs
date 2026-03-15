using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single Tera Raid entry.
/// </summary>
public partial class TeraRaid9Model : ObservableObject
{
    public int Index { get; }
    public string Label { get; }
    public TeraRaidDetail Detail { get; }

    public TeraRaid9Model(int index, TeraRaidDetail detail)
    {
        Index = index;
        Label = $"Raid {index + 1:000}";
        Detail = detail;
    }
}

/// <summary>
/// ViewModel for the Scarlet/Violet Tera Raid editor.
/// </summary>
public partial class Raid9ViewModel : SaveEditorViewModelBase
{
    private readonly SaveFile _origin;
    private readonly SAV9SV _sav;
    private readonly RaidSpawnList9 _raids;

    public ObservableCollection<TeraRaid9Model> Raids { get; } = [];

    [ObservableProperty] private TeraRaid9Model? _selectedRaid;

    [ObservableProperty] private string _seedToday = string.Empty;
    [ObservableProperty] private string _seedTomorrow = string.Empty;
    [ObservableProperty] private bool _hasSeeds;

    public Raid9ViewModel(SAV9SV sav, TeraRaidOrigin raidOrigin) : base(sav)
    {
        _sav = (SAV9SV)(_origin = sav).Clone();
        _raids = raidOrigin switch
        {
            TeraRaidOrigin.Paldea => _sav.RaidPaldea,
            TeraRaidOrigin.Kitakami => _sav.RaidKitakami,
            TeraRaidOrigin.BlueberryAcademy => _sav.RaidBlueberry,
            _ => throw new ArgumentOutOfRangeException(nameof(raidOrigin)),
        };

        for (int i = 0; i < _raids.CountUsed; i++)
        {
            var detail = _raids.GetRaid(i);
            Raids.Add(new TeraRaid9Model(i, detail));
        }

        if (Raids.Count > 0)
            SelectedRaid = Raids[0];

        HasSeeds = _raids.HasSeeds;
        if (HasSeeds)
        {
            SeedToday = _raids.CurrentSeed.ToString("X16");
            SeedTomorrow = _raids.TomorrowSeed.ToString("X16");
        }
    }

    [RelayCommand]
    private void CopyToOthers()
    {
        if (SelectedRaid is null)
            return;
        _raids.Propagate(SelectedRaid.Index, seedToo: false);
    }

    [RelayCommand]
    private void Save()
    {
        if (HasSeeds)
        {
            if (ulong.TryParse(SeedToday, System.Globalization.NumberStyles.HexNumber, null, out var st))
                _raids.CurrentSeed = st;
            if (ulong.TryParse(SeedTomorrow, System.Globalization.NumberStyles.HexNumber, null, out var sm))
                _raids.TomorrowSeed = sm;
        }

        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }
}


using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class PokemonEditorViewModel
{
    // Met Info
    [ObservableProperty]
    private int _originGame;

    [ObservableProperty]
    private int _metLocation;

    [ObservableProperty]
    private int _eggLocation;

    [ObservableProperty]
    private int _metLevel;

    [ObservableProperty]
    private DateTime? _metDate;
    [ObservableProperty]
    private DateTime? _eggDate;

    [ObservableProperty] private ObservableCollection<ComboItem> _metLocationList = [];
    [ObservableProperty] private ObservableCollection<ComboItem> _eggLocationList = [];

    public bool HasMetDate => _sav.Generation >= 4;

    partial void OnOriginGameChanged(int value)
    {
        if (_isLoading) return;
        UpdateMetDataLists();
    }

    private void UpdateMetDataLists(bool preserveSelection = true)
    {
        var currentMetLocation = MetLocation;
        var currentEggLocation = EggLocation;

        var context = _sav.Context;
        MetLocationList = new ObservableCollection<ComboItem>(
            GameInfo.Sources.Met.GetLocationList((GameVersion)OriginGame, context));
        EggLocationList = new ObservableCollection<ComboItem>(
            GameInfo.Sources.Met.GetLocationList((GameVersion)OriginGame, context, egg: true));

        if (_isLoading || !preserveSelection) return;

        MetLocation = MetLocationList.Any(l => l.Value == currentMetLocation)
            ? currentMetLocation
            : MetLocationList.Count > 0 ? MetLocationList[0].Value : 0;

        EggLocation = EggLocationList.Any(l => l.Value == currentEggLocation)
            ? currentEggLocation
            : EggLocationList.Count > 0 ? EggLocationList[0].Value : 0;
    }

    partial void OnMetLocationChanged(int value) { if (!_isLoading) Validate(); }
    partial void OnMetLevelChanged(int value) { if (!_isLoading) Validate(); }
    partial void OnMetDateChanged(DateTime? value) { if (!_isLoading) Validate(); }
    partial void OnEggLocationChanged(int value) { if (!_isLoading) Validate(); }
    partial void OnEggDateChanged(DateTime? value) { if (!_isLoading) Validate(); }
    partial void OnIsFatefulEncounterChanged(bool value) { if (!_isLoading) Validate(); }
}

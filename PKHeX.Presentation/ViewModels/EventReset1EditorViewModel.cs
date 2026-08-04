using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Event reset editor for Gen 1 saves.
/// Allows resetting overworld Pokémon encounters.
/// </summary>
public partial class EventReset1EditorViewModel : ViewModelBase
{
    private readonly G1OverworldSpawner _overworld;

    [ObservableProperty]
    private ObservableCollection<OverworldEventViewModel> _events = [];

    public EventReset1EditorViewModel(SAV1 sav)
    {
        _overworld = new G1OverworldSpawner(sav);
        LoadEvents();
    }

    private void LoadEvents()
    {
        Events.Clear();

        var pairs = _overworld.GetFlagPairs().OrderBy(z => z.Name);
        foreach (var pair in pairs)
        {
            var name = pair.Name.AsSpan(G1OverworldSpawner.FlagPropertyPrefix.Length);
            var index = name.IndexOf('_');
            var specName = index == -1 ? name : name[..index];

            // Convert species name to localized name
            SpeciesName.TryGetSpecies(specName, (int)LanguageID.English, out var species);
            var localized = GameInfo.Strings.specieslist[species];
            if (index != -1)
                localized += $" {name[(index + 1)..]}";

            Events.Add(new OverworldEventViewModel(pair, localized));
        }
    }

    [RelayCommand]
    private void ResetAll()
    {
        foreach (var ev in Events.Where(e => e.CanReset))
        {
            ev.Reset();
        }
    }

    [RelayCommand]
    private void Save()
    {
        _overworld.Save();
    }
}

public partial class OverworldEventViewModel : ViewModelBase
{
    private readonly FlagPairG1Detail _pair;

    public string Name { get; }

    [ObservableProperty]
    private bool _canReset;

    public OverworldEventViewModel(FlagPairG1Detail pair, string name)
    {
        _pair = pair;
        Name = name;
        CanReset = pair.IsHidden;
    }

    [RelayCommand]
    public void Reset()
    {
        if (!CanReset) return;
        _pair.Reset();
        CanReset = false;
    }
}

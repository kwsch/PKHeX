using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single overworld spawner flag pair entry.
/// </summary>
public partial class FlagPairModel : ObservableObject
{
    public string Name { get; }
    public FlagPairG1Detail Backing { get; }

    [ObservableProperty]
    private bool _isEnabled;

    public FlagPairModel(FlagPairG1Detail pair, string localizedName)
    {
        Backing = pair;
        Name = localizedName;
        _isEnabled = pair.IsHidden;
    }

    [RelayCommand]
    private void Reset()
    {
        Backing.Reset();
        IsEnabled = false;
    }
}

/// <summary>
/// ViewModel for the Gen 1 Event Reset editor.
/// Resets overworld spawner event data in Gen 1 saves.
/// </summary>
public partial class SAVEventReset1ViewModel : SaveEditorViewModelBase
{
    private readonly G1OverworldSpawner _overworld;

    public ObservableCollection<FlagPairModel> FlagPairs { get; } = [];

    public SAVEventReset1ViewModel(SAV1 sav) : base(sav)
    {
        _overworld = new G1OverworldSpawner(sav);

        var pairs = _overworld.GetFlagPairs().OrderBy(z => z.Name);
        foreach (var pair in pairs)
        {
            var nameSpan = pair.Name.AsSpan(G1OverworldSpawner.FlagPropertyPrefix.Length);
            var index = nameSpan.IndexOf('_');
            var specName = index == -1 ? nameSpan : nameSpan[..index];

            SpeciesName.TryGetSpecies(specName, (int)LanguageID.English, out var species);
            var localized = GameInfo.Strings.specieslist[species];
            if (index != -1)
                localized += $" {nameSpan[(index + 1)..].ToString()}";

            FlagPairs.Add(new FlagPairModel(pair, localized));
        }
    }

    [RelayCommand]
    private void Save()
    {
        _overworld.Save();
        Modified = true;
    }
}

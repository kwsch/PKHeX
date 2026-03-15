using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a Battle Video entry.
/// </summary>
public class BattleVideo5Entry
{
    public int Index { get; }
    public string Label { get; }

    public BattleVideo5Entry(int index, string label)
    {
        Index = index;
        Label = label;
    }
}

/// <summary>
/// Model for a PWT entry.
/// </summary>
public class PWT5Entry
{
    public int Index { get; }
    public string Label { get; }

    public PWT5Entry(int index, string label)
    {
        Index = index;
        Label = label;
    }
}

/// <summary>
/// Model for a Pokestar movie entry.
/// </summary>
public class Pokestar5Entry
{
    public int Index { get; }
    public string Label { get; }

    public Pokestar5Entry(int index, string label)
    {
        Index = index;
        Label = label;
    }
}

/// <summary>
/// ViewModel for the Gen 5 DLC content editor.
/// Manages C-Gear skins, Battle Videos, PWT, Pokestar Movies, Musical Shows, Memory Links, and Pokedex Skins.
/// </summary>
public partial class DLC5ViewModel : SaveEditorViewModelBase
{
    private readonly SAV5 SAV5;

    // C-Gear
    [ObservableProperty] private bool _hasCGear;
    [ObservableProperty] private string _cGearStatus = "Not loaded";

    // Battle Videos
    public ObservableCollection<BattleVideo5Entry> BattleVideos { get; } = [];

    [ObservableProperty]
    private int _selectedBattleVideoIndex;

    // PWT (B2W2 only)
    public bool ShowPWT { get; }
    public ObservableCollection<PWT5Entry> PWTEntries { get; } = [];

    [ObservableProperty]
    private int _selectedPWTIndex;

    // Pokestar (B2W2 only)
    public bool ShowPokestar { get; }
    public ObservableCollection<Pokestar5Entry> PokestarMovies { get; } = [];

    [ObservableProperty]
    private int _selectedPokestarIndex;

    // Musical
    [ObservableProperty] private string _musicalName = string.Empty;

    // Memory Link
    [ObservableProperty] private string _link1Status = "Link 1";
    [ObservableProperty] private string _link2Status = "Link 2";

    public DLC5ViewModel(SAV5 sav) : base(sav)
    {
        SAV5 = sav;
        ShowPWT = sav is SAV5B2W2;
        ShowPokestar = sav is SAV5B2W2;

        LoadCGear();
        LoadBattleVideos();
        LoadPWT();
        LoadPokestar();
        LoadMusical();
    }

    private void LoadCGear()
    {
        var data = SAV5.CGearSkinData;
        var bg = SAV5 is SAV5BW ? (CGearBackground)new CGearBackgroundBW(data) : new CGearBackgroundB2W2(data);
        _hasCGear = !bg.IsUninitialized;
        _cGearStatus = _hasCGear ? "C-Gear skin loaded" : "No C-Gear skin";
    }

    private void LoadBattleVideos()
    {
        for (int i = 0; i < 4; i++)
        {
            var data = SAV5.GetBattleVideo(i);
            var bvid = new BattleVideo5(data);
            var name = bvid.IsUninitialized ? "Empty" : bvid.GetTrainerNames();
            BattleVideos.Add(new BattleVideo5Entry(i, $"{i:00} - {name}"));
        }
    }

    private void LoadPWT()
    {
        if (SAV5 is not SAV5B2W2 b2w2)
            return;
        for (int i = 0; i < SAV5B2W2.PWTCount; i++)
        {
            var data = b2w2.GetPWT(i);
            var pwt = new WorldTournament5(data);
            var name = pwt.Name;
            if (string.IsNullOrWhiteSpace(name))
                name = "Empty";
            PWTEntries.Add(new PWT5Entry(i, $"{i + 1:00} - {name}"));
        }
    }

    private void LoadPokestar()
    {
        if (SAV5 is not SAV5B2W2 b2w2)
            return;
        for (int i = 0; i < SAV5B2W2.PokestarCount; i++)
        {
            var data = b2w2.GetPokestarMovie(i);
            var movie = new PokestarMovie5(data);
            PokestarMovies.Add(new Pokestar5Entry(i, $"{i + 1:00} - {movie.Name}"));
        }
    }

    private void LoadMusical()
    {
        _musicalName = SAV5.Musical.MusicalName;
    }

    [RelayCommand]
    private void Save()
    {
        SAV5.Musical.MusicalName = MusicalName;
        SAV.State.Edited = true;
        Modified = true;
    }
}

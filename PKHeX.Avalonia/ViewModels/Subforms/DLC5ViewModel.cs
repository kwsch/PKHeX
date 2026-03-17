using System;
using System.Collections.ObjectModel;
using System.IO;
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
    public string Label { get; set; }

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
    public string Label { get; set; }

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
    public string Label { get; set; }

    public Pokestar5Entry(int index, string label)
    {
        Index = index;
        Label = label;
    }
}

/// <summary>
/// ViewModel for the Gen 5 DLC content editor.
/// Manages C-Gear skins, Battle Videos, PWT, Pokestar Movies, Musical Shows, Memory Links, Pokedex Skins, and Battle Tests.
/// </summary>
public partial class DLC5ViewModel : SaveEditorViewModelBase
{
    private readonly SAV5 _origin;
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

    // Status
    [ObservableProperty] private string _statusText = string.Empty;

    // C-Gear extension info
    public string CGearExtension => SAV5 is SAV5BW ? CGearBackgroundBW.Extension : CGearBackgroundB2W2.Extension;

    public DLC5ViewModel(SAV5 sav) : base(sav)
    {
        _origin = sav;
        SAV5 = (SAV5)sav.Clone();
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
        HasCGear = !bg.IsUninitialized;
        CGearStatus = HasCGear ? "C-Gear skin loaded" : "No C-Gear skin";
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
        MusicalName = SAV5.Musical.MusicalName;
    }

    // ========== C-Gear Import/Export ==========

    [RelayCommand]
    private void ImportCGear(string? path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            StatusText = "File not found.";
            return;
        }

        var len = new FileInfo(path).Length;
        if (len != CGearBackground.SIZE)
        {
            StatusText = $"Incorrect size, got {len} bytes, expected {CGearBackground.SIZE} bytes.";
            return;
        }

        byte[] data = File.ReadAllBytes(path);
        bool isBW = SAV5 is SAV5BW;
        CGearBackground temp = isBW ? new CGearBackgroundBW(data) : new CGearBackgroundB2W2(data);
        bool isPSK = PaletteTileSelection.IsPaletteShiftFormat(temp.Arrange);

        try
        {
            if (isBW && !isPSK)
                PaletteTileSelection.ConvertToShiftFormat<CGearBackgroundBW>(temp.Arrange);
            else if (!isBW && isPSK)
                PaletteTileSelection.ConvertFromShiftFormat(temp.Arrange);
        }
        catch (Exception ex)
        {
            StatusText = $"C-Gear conversion error: {ex.Message}";
            return;
        }

        SAV5.SetCGearSkin(temp.Data);
        HasCGear = true;
        CGearStatus = "C-Gear skin loaded";
        StatusText = "C-Gear skin imported.";
    }

    [RelayCommand]
    private void ExportCGear(string? path)
    {
        if (string.IsNullOrEmpty(path))
            return;

        var data = SAV5.CGearSkinData;
        var bg = SAV5 is SAV5BW ? (CGearBackground)new CGearBackgroundBW(data) : new CGearBackgroundB2W2(data);
        File.WriteAllBytes(path, bg.Data);
        StatusText = "C-Gear skin exported.";
    }

    // ========== Battle Video Import/Export ==========

    [RelayCommand]
    private void ImportBattleVideo(string? path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            StatusText = "File not found.";
            return;
        }

        var len = new FileInfo(path).Length;
        if (len != BattleVideo5.SIZE)
        {
            StatusText = $"Incorrect size, got {len} bytes, expected {BattleVideo5.SIZE} bytes.";
            return;
        }

        byte[] data = File.ReadAllBytes(path);
        bool decrypted = BattleVideo5.GetIsDecrypted(data);
        var bvid = new BattleVideo5(data) { IsDecrypted = decrypted };
        bvid.Encrypt();
        if (!bvid.IsUninitialized)
            bvid.RefreshChecksums();

        var index = SelectedBattleVideoIndex;
        if (index < 0 || index >= BattleVideos.Count)
            index = 0;
        SAV5.SetBattleVideo(index, data);
        var name = bvid.IsUninitialized ? "Empty" : bvid.GetTrainerNames();
        BattleVideos[index].Label = $"{index:00} - {name}";
        // Force refresh
        var temp = BattleVideos[index];
        BattleVideos.RemoveAt(index);
        BattleVideos.Insert(index, temp);
        SelectedBattleVideoIndex = index;
        StatusText = "Battle Video imported.";
    }

    [RelayCommand]
    private void ExportBattleVideo(string? path)
    {
        if (string.IsNullOrEmpty(path))
            return;

        var index = SelectedBattleVideoIndex;
        if (index < 0 || index >= BattleVideos.Count)
            index = 0;
        var data = SAV5.GetBattleVideo(index);
        File.WriteAllBytes(path, data.Span.ToArray());
        StatusText = "Battle Video exported.";
    }

    // ========== PWT Import/Export (B2W2 only) ==========

    [RelayCommand]
    private void ImportPWT(string? path)
    {
        if (SAV5 is not SAV5B2W2 b2w2)
            return;
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            StatusText = "File not found.";
            return;
        }

        var len = new FileInfo(path).Length;
        const int pporg = 0x1314;
        if (len != WorldTournament5.SIZE && len != pporg)
        {
            StatusText = $"Incorrect size, got {len} bytes, expected {WorldTournament5.SIZE} bytes.";
            return;
        }

        byte[] data = File.ReadAllBytes(path);
        if (data.Length == pporg)
            Array.Resize(ref data, WorldTournament5.SIZE);

        var index = SelectedPWTIndex;
        if (index < 0 || index >= PWTEntries.Count)
            index = 0;
        b2w2.SetPWT(index, data);

        var pwt = new WorldTournament5(data);
        var name = pwt.Name;
        if (string.IsNullOrWhiteSpace(name))
            name = "Empty";
        PWTEntries[index].Label = $"{index + 1:00} - {name}";
        var temp = PWTEntries[index];
        PWTEntries.RemoveAt(index);
        PWTEntries.Insert(index, temp);
        SelectedPWTIndex = index;
        StatusText = "PWT data imported.";
    }

    [RelayCommand]
    private void ExportPWT(string? path)
    {
        if (SAV5 is not SAV5B2W2 b2w2)
            return;
        if (string.IsNullOrEmpty(path))
            return;

        var index = SelectedPWTIndex;
        if (index < 0 || index >= PWTEntries.Count)
            index = 0;
        var data = b2w2.GetPWT(index);
        File.WriteAllBytes(path, data.Span.ToArray());
        StatusText = "PWT data exported.";
    }

    // ========== Pokestar Import/Export (B2W2 only) ==========

    [RelayCommand]
    private void ImportPokestar(string? path)
    {
        if (SAV5 is not SAV5B2W2 b2w2)
            return;
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            StatusText = "File not found.";
            return;
        }

        var len = new FileInfo(path).Length;
        if (len != PokestarMovie5.SIZE)
        {
            StatusText = $"Incorrect size, got {len} bytes, expected {PokestarMovie5.SIZE} bytes.";
            return;
        }

        byte[] data = File.ReadAllBytes(path);
        var index = SelectedPokestarIndex;
        if (index < 0 || index >= PokestarMovies.Count)
            index = 0;
        b2w2.SetPokestarMovie(index, data);

        var movie = new PokestarMovie5(data);
        PokestarMovies[index].Label = $"{index + 1:00} - {movie.Name}";
        var temp = PokestarMovies[index];
        PokestarMovies.RemoveAt(index);
        PokestarMovies.Insert(index, temp);
        SelectedPokestarIndex = index;
        StatusText = "Pokestar movie imported.";
    }

    [RelayCommand]
    private void ExportPokestar(string? path)
    {
        if (SAV5 is not SAV5B2W2 b2w2)
            return;
        if (string.IsNullOrEmpty(path))
            return;

        var index = SelectedPokestarIndex;
        if (index < 0 || index >= PokestarMovies.Count)
            index = 0;
        var data = b2w2.GetPokestarMovie(index);
        File.WriteAllBytes(path, data.Span.ToArray());
        StatusText = "Pokestar movie exported.";
    }

    // ========== Musical Import/Export ==========

    [RelayCommand]
    private void ImportMusical(string? path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            StatusText = "File not found.";
            return;
        }

        var expectedSize = SAV5 is SAV5B2W2 ? MusicalShow5.SIZE_B2W2 : MusicalShow5.SIZE_BW;
        const int pporg = 0x17D78;
        var len = new FileInfo(path).Length;
        if (len != expectedSize && len != pporg)
        {
            StatusText = $"Incorrect size, got {len} bytes, expected {expectedSize} bytes.";
            return;
        }

        byte[] data = File.ReadAllBytes(path);
        if (data.Length == pporg)
            Array.Resize(ref data, expectedSize);

        var musical = new MusicalShow5(data);
        SAV5.SetMusical(data);
        SAV5.Musical.MusicalName = musical.IsUninitialized ? "" : Path.GetFileNameWithoutExtension(path).Trim();
        MusicalName = SAV5.Musical.MusicalName;
        StatusText = "Musical show imported.";
    }

    [RelayCommand]
    private void ExportMusical(string? path)
    {
        if (string.IsNullOrEmpty(path))
            return;

        var data = SAV5.MusicalDownloadData;
        File.WriteAllBytes(path, data.Span.ToArray());
        StatusText = "Musical show exported.";
    }

    // ========== Memory Link Import/Export ==========

    [RelayCommand]
    private void ImportLink1(string? path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            StatusText = "File not found.";
            return;
        }

        var len = new FileInfo(path).Length;
        if (len != SAV5.Link1Data.Length)
        {
            StatusText = $"Incorrect size, got {len} bytes, expected {SAV5.Link1Data.Length} bytes.";
            return;
        }

        byte[] data = File.ReadAllBytes(path);
        SAV5.SetLink1Data(data);
        StatusText = "Memory Link 1 imported.";
    }

    [RelayCommand]
    private void ExportLink1(string? path)
    {
        if (string.IsNullOrEmpty(path))
            return;
        File.WriteAllBytes(path, SAV5.Link1Data.Span.ToArray());
        StatusText = "Memory Link 1 exported.";
    }

    [RelayCommand]
    private void ImportLink2(string? path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            StatusText = "File not found.";
            return;
        }

        var len = new FileInfo(path).Length;
        if (len != SAV5.Link2Data.Length)
        {
            StatusText = $"Incorrect size, got {len} bytes, expected {SAV5.Link2Data.Length} bytes.";
            return;
        }

        byte[] data = File.ReadAllBytes(path);
        SAV5.SetLink2Data(data);
        StatusText = "Memory Link 2 imported.";
    }

    [RelayCommand]
    private void ExportLink2(string? path)
    {
        if (string.IsNullOrEmpty(path))
            return;
        File.WriteAllBytes(path, SAV5.Link2Data.Span.ToArray());
        StatusText = "Memory Link 2 exported.";
    }

    // ========== Pokedex Skin Import/Export ==========

    [RelayCommand]
    private void ImportPokedexSkin(string? path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            StatusText = "File not found.";
            return;
        }

        var len = new FileInfo(path).Length;
        const int pporg = 0x6200;
        if (len != PokeDexSkin5.SIZE && len != pporg)
        {
            StatusText = $"Incorrect size, got {len} bytes, expected {PokeDexSkin5.SIZE} bytes.";
            return;
        }

        byte[] data = File.ReadAllBytes(path);
        if (data.Length == pporg)
            Array.Resize(ref data, PokeDexSkin5.SIZE);

        SAV5.SetPokeDexSkin(data);
        StatusText = "Pokedex Skin imported.";
    }

    [RelayCommand]
    private void ExportPokedexSkin(string? path)
    {
        if (string.IsNullOrEmpty(path))
            return;
        File.WriteAllBytes(path, SAV5.PokedexSkinData.Span.ToArray());
        StatusText = "Pokedex Skin exported.";
    }

    // ========== Battle Test Import/Export ==========

    [RelayCommand]
    private void ImportBattleTest(string? path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            StatusText = "File not found.";
            return;
        }

        var len = new FileInfo(path).Length;
        if (len != BattleTest5.SIZE)
        {
            StatusText = $"Incorrect size, got {len} bytes, expected {BattleTest5.SIZE} bytes.";
            return;
        }

        byte[] data = File.ReadAllBytes(path);
        var test = new BattleTest5(data);
        if (!test.IsUninitialized)
        {
            test.Magic = BattleTest5.Sentinel;
            test.RefreshChecksums();
        }
        SAV5.SetBattleTest(data);
        StatusText = "Battle Test imported.";
    }

    [RelayCommand]
    private void ExportBattleTest(string? path)
    {
        if (string.IsNullOrEmpty(path))
            return;
        File.WriteAllBytes(path, SAV5.BattleTest.Span.ToArray());
        StatusText = "Battle Test exported.";
    }

    // ========== Save ==========

    [RelayCommand]
    private void Save()
    {
        SAV5.Musical.MusicalName = MusicalName;
        _origin.CopyChangesFrom(SAV5);
        Modified = true;
    }
}

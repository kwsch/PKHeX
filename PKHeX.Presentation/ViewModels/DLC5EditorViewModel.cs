using System;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;
using PKHeX.Presentation.Localization;

namespace PKHeX.Presentation.ViewModels;

public partial class DLC5EditorViewModel : ViewModelBase
{
    private readonly SAV5 _sav;
    private readonly IDialogService _dialogService;

    public bool IsB2W2 { get; }
    public bool IsBW { get; }

    public DLC5EditorViewModel(SAV5 sav, IDialogService dialogService)
    {
        _sav = sav;
        _dialogService = dialogService;
        
        IsB2W2 = sav is SAV5B2W2;
        IsBW = sav is SAV5BW;

        RefreshLists();
    }

    // PWT (B2W2 Only)
    public string[] PWTItems { get; private set; } = [];
    private int _pwtIndex;
    public int PWTIndex { get => _pwtIndex; set => SetProperty(ref _pwtIndex, value); }

    // Pokestar (B2W2 Only)
    public string[] PokestarItems { get; private set; } = [];
    private int _pokestarIndex;
    public int PokestarIndex { get => _pokestarIndex; set => SetProperty(ref _pokestarIndex, value); }
    
    // Battle Video
    public string[] BattleVideoItems { get; private set; } = [];
    private int _battleVideoIndex;
    public int BattleVideoIndex { get => _battleVideoIndex; set => SetProperty(ref _battleVideoIndex, value); }

    private void RefreshLists()
    {
        // PWT
        if (_sav is SAV5B2W2 b2w2)
        {
            var pwtList = new string[SAV5B2W2.PWTCount];
            for (int i = 0; i < SAV5B2W2.PWTCount; i++)
            {
                var data = b2w2.GetPWT(i);
                var pwt = new WorldTournament5(data);
                var name = string.IsNullOrWhiteSpace(pwt.Name) ? "Empty" : pwt.Name;
                pwtList[i] = $"{i + 1:00} - {name}";
            }
            PWTItems = pwtList;
            OnPropertyChanged(nameof(PWTItems));
            
            // Pokestar
            var pokestarList = new string[SAV5B2W2.PokestarCount];
            for (int i = 0; i < SAV5B2W2.PokestarCount; i++)
            {
                var data = b2w2.GetPokestarMovie(i);
                var movie = new PokestarMovie5(data);
                pokestarList[i] = $"{i + 1:00} - {movie.Name}";
            }
            PokestarItems = pokestarList;
            OnPropertyChanged(nameof(PokestarItems));
        }

        // Battle Videos
        var bvList = new string[4];
        for (int i = 0; i < 4; i++)
        {
            var data = _sav.GetBattleVideo(i);
            var bvid = new BattleVideo5(data);
            var name = bvid.IsUninitialized ? "Empty" : bvid.GetTrainerNames();
            bvList[i] = $"{i:00} - {name}";
        }
        BattleVideoItems = bvList;
        OnPropertyChanged(nameof(BattleVideoItems));
    }

    /// <summary>Reads a file, surfacing an error dialog (instead of throwing) if the read fails.</summary>
    private async Task<byte[]?> TryReadAllBytesAsync(string path)
    {
        try
        {
            return await File.ReadAllBytesAsync(path);
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["Common_Error"], LocalizedStrings.Instance.Format("File_CouldNotReadFile", ex.Message));
            return null;
        }
    }

    /// <summary>Writes a file, surfacing an error dialog (instead of throwing) if the write fails.</summary>
    private async Task<bool> TryWriteAllBytesAsync(string path, byte[] data)
    {
        try
        {
            await File.WriteAllBytesAsync(path, data);
            return true;
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["Common_Error"], LocalizedStrings.Instance.Format("File_CouldNotWriteFile", ex.Message));
            return false;
        }
    }

    [RelayCommand]
    private async Task ImportCGearAsync()
    {
        var result = await _dialogService.OpenFileAsync(LocalizedStrings.Instance["DLC5Editor_ImportCGearTitle"], ["cgb", "png"]);
        if (result is null) return;

        var data = await TryReadAllBytesAsync(result);
        if (data is null) return;
        if (data.Length != CGearBackground.SIZE)
        {
            await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["Common_Error"], LocalizedStrings.Instance.Format("DLC5Editor_CGearSizeMismatch", data.Length, CGearBackground.SIZE));
            return;
        }

        // Adjust format if needed (BW vs B2W2)
        CGearBackground temp = IsBW ? new CGearBackgroundBW(data) : new CGearBackgroundB2W2(data);
        bool isPSK = PaletteTileSelection.IsPaletteShiftFormat(temp.Arrange);

        // Auto-convert format if mismatch
        try 
        {
            if (IsBW && !isPSK)
                PaletteTileSelection.ConvertToShiftFormat<CGearBackgroundBW>(temp.Arrange);
            else if (!IsBW && isPSK)
                PaletteTileSelection.ConvertFromShiftFormat(temp.Arrange);
        }
        catch (Exception ex)
        {
             await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["Common_Error"], ex.Message);
             return;
        }

        _sav.SetCGearSkin(temp.Data);
        await _dialogService.ShowInformationAsync(LocalizedStrings.Instance["DLC5Editor_SuccessTitle"], LocalizedStrings.Instance["DLC5Editor_CGearImportSuccess"]);
    }

    [RelayCommand]
    private async Task ExportCGearAsync()
    {
        string ext = IsBW ? CGearBackgroundBW.Extension : CGearBackgroundB2W2.Extension;
        var path = await _dialogService.SaveFileAsync("Export C-Gear Skin", "CGear_Skin." + ext, [ext]);
        if (path is null) return;

        await TryWriteAllBytesAsync(path, _sav.CGearSkinData.ToArray());
    }
    
    [RelayCommand]
    private async Task ImportMusicalAsync()
    {
        int size = IsB2W2 ? MusicalShow5.SIZE_B2W2 : MusicalShow5.SIZE_BW;
        var result = await _dialogService.OpenFileAsync("Import Musical", [MusicalShow5.Extension]);
        if (result is null) return;

        var data = await TryReadAllBytesAsync(result);
        if (data is null) return;
        if (data.Length != size)
        {
             await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["Common_Error"], LocalizedStrings.Instance.Format("DLC5Editor_InvalidFileSizeBytes", size));
             return;
        }

        _sav.SetMusical(data);
        var musical = new MusicalShow5(data);
        _sav.Musical.MusicalName = musical.IsUninitialized ? "" : Path.GetFileNameWithoutExtension(result).Trim();
        await _dialogService.ShowInformationAsync(LocalizedStrings.Instance["DLC5Editor_SuccessTitle"], LocalizedStrings.Instance["DLC5Editor_MusicalImported"]);
    }

    [RelayCommand]
    private async Task ExportMusicalAsync()
    {
        string name = string.IsNullOrWhiteSpace(_sav.Musical.MusicalName) ? "Musical" : _sav.Musical.MusicalName;
        var path = await _dialogService.SaveFileAsync("Export Musical", $"{name}.{MusicalShow5.Extension}", [MusicalShow5.Extension]);
        if (path is null) return;

        await TryWriteAllBytesAsync(path, _sav.MusicalDownloadData.ToArray());
    }

    [RelayCommand]
    private async Task ImportPokedexSkinAsync()
    {
        var result = await _dialogService.OpenFileAsync("Import Pokédex Skin", [PokeDexSkin5.Extension]);
        if (result is null) return;

        var data = await TryReadAllBytesAsync(result);
        if (data is null) return;
        if (data.Length != _sav.PokedexSkinData.Length)
        {
             await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["Common_Error"], LocalizedStrings.Instance.Format("DLC5Editor_InvalidFileSizeBytes", _sav.PokedexSkinData.Length));
             return;
        }

        _sav.SetPokeDexSkin(data);
        await _dialogService.ShowInformationAsync(LocalizedStrings.Instance["DLC5Editor_SuccessTitle"], LocalizedStrings.Instance["DLC5Editor_PokedexImported"]);
    }

    [RelayCommand]
    private async Task ExportPokedexSkinAsync()
    {
        var path = await _dialogService.SaveFileAsync("Export Pokédex Skin", $"PokedexSkin.{PokeDexSkin5.Extension}", [PokeDexSkin5.Extension]);
        if (path is null) return;

        await TryWriteAllBytesAsync(path, _sav.PokedexSkinData.ToArray());
    }

    [RelayCommand]
    private async Task ImportPWTAsync()
    {
        if (_sav is not SAV5B2W2 b2w2) return;
        
        var result = await _dialogService.OpenFileAsync("Import PWT", [WorldTournament5.Extension]);
        if (result is null) return;

        var data = await TryReadAllBytesAsync(result);
        if (data is null) return;
        if (data.Length != WorldTournament5.SIZE)
        {
             await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["Common_Error"], LocalizedStrings.Instance.Format("DLC5Editor_InvalidFileSizeBytes", WorldTournament5.SIZE));
             return;
        }

        b2w2.SetPWT(PWTIndex, data);
        RefreshLists();
    }

    [RelayCommand]
    private async Task ExportPWTAsync()
    {
        if (_sav is not SAV5B2W2 b2w2) return;
        var data = b2w2.GetPWT(PWTIndex).ToArray();
        var name = new WorldTournament5(data).Name;
        if (string.IsNullOrWhiteSpace(name)) name = "Empty";
        
        var path = await _dialogService.SaveFileAsync("Export PWT", $"{name}.{WorldTournament5.Extension}", [WorldTournament5.Extension]);
        if (path is null) return;

        await TryWriteAllBytesAsync(path, data);
    }

    [RelayCommand]
    private async Task ImportPokestarAsync()
    {
        if (_sav is not SAV5B2W2 b2w2) return;

        var result = await _dialogService.OpenFileAsync("Import Pokestar Movie", [PokestarMovie5.Extension]);
        if (result is null) return;

        var data = await TryReadAllBytesAsync(result);
        if (data is null) return;
        if (data.Length != PokestarMovie5.SIZE)
        {
             await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["Common_Error"], LocalizedStrings.Instance.Format("DLC5Editor_InvalidFileSizeBytes", PokestarMovie5.SIZE));
             return;
        }

        b2w2.SetPokestarMovie(PokestarIndex, data);
        RefreshLists();
    }

    [RelayCommand]
    private async Task ExportPokestarAsync()
    {
        if (_sav is not SAV5B2W2 b2w2) return;
        var data = b2w2.GetPokestarMovie(PokestarIndex).ToArray();
        var name = new PokestarMovie5(data).Name;
        
        var path = await _dialogService.SaveFileAsync("Export Pokestar Movie", $"{name}.{PokestarMovie5.Extension}", [PokestarMovie5.Extension]);
        if (path is null) return;

        await TryWriteAllBytesAsync(path, data);
    }

    [RelayCommand]
    private async Task ImportBattleVideoAsync()
    {
        var result = await _dialogService.OpenFileAsync("Import Battle Video", [BattleVideo5.Extension]);
        if (result is null) return;

        var data = await TryReadAllBytesAsync(result);
        if (data is null) return;
        if (data.Length != BattleVideo5.SIZE)
        {
             await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["Common_Error"], LocalizedStrings.Instance.Format("DLC5Editor_InvalidFileSizeBytes", BattleVideo5.SIZE));
             return;
        }

        bool decrypted = BattleVideo5.GetIsDecrypted(data);
        var bvid = new BattleVideo5(data) { IsDecrypted = decrypted };
        bvid.Encrypt();
        if (!bvid.IsUninitialized)
            bvid.RefreshChecksums();

        _sav.SetBattleVideo(BattleVideoIndex, data);
        RefreshLists();
    }

    [RelayCommand]
    private async Task ExportBattleVideoAsync()
    {
        var path = await _dialogService.SaveFileAsync("Export Battle Video", $"BattleVideo_{BattleVideoIndex}.{BattleVideo5.Extension}", [BattleVideo5.Extension]);
        if (path is null) return;

        await TryWriteAllBytesAsync(path, _sav.GetBattleVideo(BattleVideoIndex).ToArray());
    }
    
    [RelayCommand]
    private async Task ImportMemoryLink1Async()
    {
        await ImportMemoryLinkAsync(_sav.Link1Data.Length, d => _sav.SetLink1Data(d));
    }

    [RelayCommand]
    private async Task ImportMemoryLink2Async()
    {
        await ImportMemoryLinkAsync(_sav.Link2Data.Length, d => _sav.SetLink2Data(d));
    }

    private async Task ImportMemoryLinkAsync(int size, Action<byte[]> setter)
    {
        var result = await _dialogService.OpenFileAsync("Import Memory Link", ["ml5"]);
        if (result is null) return;
        var data = await TryReadAllBytesAsync(result);
        if (data is null) return;

        if (data.Length != size)
        {
            await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["Common_Error"], LocalizedStrings.Instance.Format("DLC5Editor_InvalidSizeExpected", size));
            return;
        }
        setter(data);
        await _dialogService.ShowInformationAsync(LocalizedStrings.Instance["DLC5Editor_SuccessTitle"], LocalizedStrings.Instance["DLC5Editor_MemoryLinkImported"]);
    }

    [RelayCommand]
    private async Task ExportMemoryLink1Async()
    {
        var path = await _dialogService.SaveFileAsync("Export Memory Link 1", "MemoryLink1.ml5", ["ml5"]);
        if (path is null) return;
        await TryWriteAllBytesAsync(path, _sav.Link1Data.ToArray());
    }

    [RelayCommand]
    private async Task ExportMemoryLink2Async()
    {
        var path = await _dialogService.SaveFileAsync("Export Memory Link 2", "MemoryLink2.ml5", ["ml5"]);
        if (path is null) return;
        await TryWriteAllBytesAsync(path, _sav.Link2Data.ToArray());
    }
}

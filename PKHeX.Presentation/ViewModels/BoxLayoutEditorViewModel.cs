using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class BoxLayoutEditorViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly IBoxDetailNameRead? _nameReader;
    private readonly IBoxDetailName? _nameWriter;
    private readonly IBoxDetailWallpaper? _wallpaper;

    public BoxLayoutEditorViewModel(SaveFile sav)
    {
        _sav = sav;
        _nameReader = sav as IBoxDetailNameRead;
        _nameWriter = sav as IBoxDetailName;
        _wallpaper = sav as IBoxDetailWallpaper;

        IsSupported = _nameReader is not null || _wallpaper is not null;
        CanEditNames = _nameWriter is not null;
        CanEditWallpaper = _wallpaper is not null;
        CanEditUnlocked = sav.BoxesUnlocked > 0;

        // Build list of possible unlocked box counts
        for (int i = 0; i <= sav.BoxCount; i++)
            UnlockedOptions.Add(i);
        
        _unlockedBoxes = Math.Min(sav.BoxCount, sav.BoxesUnlocked);

        LoadWallpaperNames();
        LoadBoxes();
    }

    public bool IsSupported { get; }
    public bool CanEditNames { get; }
    public bool CanEditWallpaper { get; }
    public bool CanEditUnlocked { get; }

    [ObservableProperty]
    private ObservableCollection<BoxLayoutItemViewModel> _boxes = [];

    [ObservableProperty]
    private ObservableCollection<int> _unlockedOptions = [];

    [ObservableProperty]
    private ObservableCollection<string> _wallpaperNames = [];

    [ObservableProperty]
    private int _unlockedBoxes;

    partial void OnUnlockedBoxesChanged(int value)
    {
        _sav.BoxesUnlocked = value;
    }

    private void LoadWallpaperNames()
    {
        WallpaperNames.Clear();
        var names = GameInfo.Strings.wallpapernames;
        
        int count = _sav.Generation switch
        {
            3 when _sav is SAV3 or SAV3RSBox => 16,
            4 or 5 or 6 => 24,
            7 => 16,
            8 when _sav is SAV8BS => 32,
            8 => 19,
            9 => 20,
            _ => 0
        };

        for (int i = 0; i < count; i++)
        {
            if (i < names.Length)
                WallpaperNames.Add(names[i]);
            else
                WallpaperNames.Add($"Wallpaper {i + 1}");
        }
    }

    private void LoadBoxes()
    {
        Boxes.Clear();
        for (int i = 0; i < _sav.BoxCount; i++)
        {
            var name = _nameReader?.GetBoxName(i) ?? BoxDetailNameExtensions.GetDefaultBoxName(i);
            var wallpaper = _wallpaper?.GetBoxWallpaper(i) ?? 0;
            Boxes.Add(new BoxLayoutItemViewModel(i, name, wallpaper, OnBoxNameChanged, OnBoxWallpaperChanged));
        }
    }

    private void OnBoxNameChanged(int box, string name)
    {
        _nameWriter?.SetBoxName(box, name);
    }

    private void OnBoxWallpaperChanged(int box, int wallpaper)
    {
        _wallpaper?.SetBoxWallpaper(box, wallpaper);
    }

    [RelayCommand]
    private void MoveUp()
    {
        // Box reordering is complex (needs to move PKM data too) - skip for now
    }

    [RelayCommand]
    private void MoveDown()
    {
        // Box reordering is complex - skip for now
    }
}

public partial class BoxLayoutItemViewModel : ViewModelBase
{
    private readonly Action<int, string> _onNameChanged;
    private readonly Action<int, int> _onWallpaperChanged;

    public BoxLayoutItemViewModel(int index, string name, int wallpaper, Action<int, string> onNameChanged, Action<int, int> onWallpaperChanged)
    {
        Index = index;
        _name = name;
        _wallpaper = wallpaper;
        _onNameChanged = onNameChanged;
        _onWallpaperChanged = onWallpaperChanged;
    }

    public int Index { get; }
    public string DisplayIndex => $"Box {Index + 1}";

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private int _wallpaper;

    partial void OnNameChanged(string value) => _onNameChanged(Index, value);
    partial void OnWallpaperChanged(int value) => _onWallpaperChanged(Index, value);
}

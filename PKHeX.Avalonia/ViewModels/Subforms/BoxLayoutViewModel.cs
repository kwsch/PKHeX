using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single box with its name and wallpaper.
/// </summary>
public partial class BoxDetailModel : ObservableObject
{
    public int Index { get; }

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private int _wallpaper;

    public BoxDetailModel(int index, string name, int wallpaper)
    {
        Index = index;
        _name = name;
        _wallpaper = wallpaper;
    }
}

/// <summary>
/// ViewModel for the Box Layout editor subform.
/// Allows renaming boxes and setting wallpaper indices.
/// </summary>
public partial class BoxLayoutViewModel : SaveEditorViewModelBase
{
    private readonly SaveFile _clone;
    private readonly bool _hasNames;
    private readonly bool _hasWallpapers;

    /// <summary>Box detail models.</summary>
    public ObservableCollection<BoxDetailModel> Boxes { get; } = [];

    /// <summary>Whether the save supports box names.</summary>
    public bool SupportsNames => _hasNames;

    /// <summary>Whether the save supports wallpapers.</summary>
    public bool SupportsWallpapers => _hasWallpapers;

    /// <summary>Maximum wallpaper index for the current generation.</summary>
    public int MaxWallpaper { get; }

    public BoxLayoutViewModel(SaveFile sav) : base(sav)
    {
        _clone = sav;
        _hasNames = sav is IBoxDetailNameRead;
        _hasWallpapers = sav is IBoxDetailWallpaper;

        MaxWallpaper = GetMaxWallpaper(sav);
        LoadBoxes();
    }

    private void LoadBoxes()
    {
        var nameReader = _clone as IBoxDetailNameRead;
        var wpReader = _clone as IBoxDetailWallpaper;

        for (int i = 0; i < _clone.BoxCount; i++)
        {
            var name = nameReader?.GetBoxName(i) ?? $"Box {i + 1}";
            var wallpaper = wpReader?.GetBoxWallpaper(i) ?? 0;
            Boxes.Add(new BoxDetailModel(i, name, wallpaper));
        }
    }

    /// <summary>
    /// Saves box names and wallpapers back to the save file.
    /// </summary>
    [RelayCommand]
    private void Save()
    {
        if (_clone is IBoxDetailName nameWriter)
        {
            foreach (var box in Boxes)
                nameWriter.SetBoxName(box.Index, box.Name);
        }

        if (_clone is IBoxDetailWallpaper wpWriter)
        {
            foreach (var box in Boxes)
                wpWriter.SetBoxWallpaper(box.Index, box.Wallpaper);
        }

        Modified = true;
    }

    private static int GetMaxWallpaper(SaveFile sav) => sav.Generation switch
    {
        3 => 15,
        4 or 5 or 6 => 23,
        7 => 15,
        8 => 31,
        9 => 19,
        _ => 15,
    };
}

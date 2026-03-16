using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using PKHeX.Avalonia.Converters;
using PKHeX.Core;
using SkiaSharp;

namespace PKHeX.Avalonia.Controls;

/// <summary>
/// Represents a single slot in a box/party editor. Replaces PictureBox from WinForms.
/// Used as the generic type T in <see cref="ISlotViewer{T}"/>.
/// </summary>
public partial class SlotModel : ObservableObject
{
    [ObservableProperty]
    private Bitmap? _image;

    [ObservableProperty]
    private int _slot;

    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private bool _isEmpty = true;

    /// <summary>The PKM entity backing this slot, if any.</summary>
    [ObservableProperty]
    private PKM? _entity;

    /// <summary>Showdown format text for tooltip display.</summary>
    public string ShowdownText => Entity is { Species: > 0 }
        ? ShowdownParsing.GetShowdownText(Entity)
        : string.Empty;

    partial void OnEntityChanged(PKM? value)
    {
        OnPropertyChanged(nameof(ShowdownText));
    }

    public void SetImage(SKBitmap? skBitmap)
    {
        Image = SKBitmapToAvaloniaBitmapConverter.ToAvaloniaBitmap(skBitmap);
        IsEmpty = skBitmap is null;
    }
}

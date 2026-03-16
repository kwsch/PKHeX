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

    /// <summary>Whether the slot matches the current search filter.</summary>
    [ObservableProperty]
    private bool _isHighlighted;

    /// <summary>Whether a search filter is currently active.</summary>
    [ObservableProperty]
    private bool _isSearchActive;

    /// <summary>Opacity to use when a search is active (1.0 if highlighted or no search, 0.3 if not matching).</summary>
    public double SearchOpacity => IsHighlighted ? 1.0 : (_isSearchActive ? 0.3 : 1.0);

    partial void OnIsHighlightedChanged(bool value) => OnPropertyChanged(nameof(SearchOpacity));
    partial void OnIsSearchActiveChanged(bool value) => OnPropertyChanged(nameof(SearchOpacity));

    /// <summary>The PKM entity backing this slot, if any.</summary>
    [ObservableProperty]
    private PKM? _entity;

    /// <summary>Showdown format text for tooltip display.</summary>
    public string ShowdownText => Entity is { Species: > 0 }
        ? ShowdownParsing.GetShowdownText(Entity)
        : string.Empty;

    /// <summary>Short summary for accessibility / screen readers (e.g. "Pikachu Lv.25").</summary>
    public string Summary => Entity is { Species: > 0 }
        ? $"{SpeciesName.GetSpeciesNameGeneration(Entity.Species, 2, Entity.Format)} Lv.{Entity.CurrentLevel}"
        : "Empty";

    /// <summary>Label describing the slot type (e.g. "Daycare", "Fused Kyurem").</summary>
    [ObservableProperty]
    private string _slotLabel = string.Empty;

    partial void OnEntityChanged(PKM? value)
    {
        OnPropertyChanged(nameof(ShowdownText));
        OnPropertyChanged(nameof(Summary));
    }

    public void SetImage(SKBitmap? skBitmap)
    {
        Image = SKBitmapToAvaloniaBitmapConverter.ToAvaloniaBitmap(skBitmap);
        IsEmpty = skBitmap is null;
    }
}

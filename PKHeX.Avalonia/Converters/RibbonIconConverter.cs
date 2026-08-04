using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace PKHeX.Avalonia.Converters;

/// <summary>
/// Resolves a bare ribbon icon resource name (from <c>RibbonItemViewModel.IconResource</c>) to an
/// Avalonia <see cref="Bitmap"/> from the embedded <c>avares://</c> ribbon assets. Returns null when
/// the asset is absent, keeping the asset-loading concern in the View layer.
/// </summary>
public sealed class RibbonIconConverter : IValueConverter
{
    public static readonly RibbonIconConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string { Length: > 0 } name)
            return null;

        var uri = new Uri($"avares://PKHeX.Avalonia/Resources/Ribbons/{name}.png");
        return AssetLoader.Exists(uri) ? new Bitmap(AssetLoader.Open(uri)) : null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

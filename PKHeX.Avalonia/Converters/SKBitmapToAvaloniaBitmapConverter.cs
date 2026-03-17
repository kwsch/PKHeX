using System;
using System.Globalization;
using System.IO;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using SkiaSharp;

namespace PKHeX.Avalonia.Converters;

/// <summary>
/// Converts an <see cref="SKBitmap"/> to an Avalonia <see cref="Bitmap"/> for display in UI controls.
/// </summary>
public class SKBitmapToAvaloniaBitmapConverter : IValueConverter
{
    public static readonly SKBitmapToAvaloniaBitmapConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not SKBitmap skBitmap)
            return null;

        return ToAvaloniaBitmap(skBitmap);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Converts an SKBitmap to an Avalonia Bitmap.
    /// </summary>
    public static Bitmap? ToAvaloniaBitmap(SKBitmap? skBitmap)
    {
        if (skBitmap is null)
            return null;

        using var image = SKImage.FromBitmap(skBitmap);
        if (image is null)
            return null;
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        if (data is null)
            return null;
        using var stream = new MemoryStream();
        data.SaveTo(stream);
        stream.Position = 0;
        return new Bitmap(stream);
    }
}

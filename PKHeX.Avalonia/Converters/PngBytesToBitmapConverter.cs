using System;
using System.Globalization;
using System.IO;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;

namespace PKHeX.Avalonia.Converters;

/// <summary>
/// Materializes a PNG <see cref="byte"/>[] (produced by the Application <c>ISpriteRenderer</c>) into an
/// Avalonia <see cref="Bitmap"/> for an <c>&lt;Image Source&gt;</c> binding. This is the single seam where
/// the UI framework re-enters the sprite pipeline; ViewModels/Models hold only bytes.
/// </summary>
public sealed class PngBytesToBitmapConverter : IValueConverter
{
    public static readonly PngBytesToBitmapConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is byte[] { Length: > 0 } bytes)
            return new Bitmap(new MemoryStream(bytes));
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

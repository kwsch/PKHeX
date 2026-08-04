using System;
using System.Globalization;
using Avalonia.Data.Converters;
using PKHeX.Application.Abstractions;

namespace PKHeX.Avalonia.Converters;

/// <summary>Maps <see cref="AppTheme"/> values to user-facing labels.</summary>
public sealed class AppThemeLabelConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value switch
    {
        AppTheme.Dark => "Dark",
        AppTheme.Light => "Light",
        AppTheme.HighContrast => "High Contrast",
        AppTheme.System => "Follow System",
        _ => value?.ToString(),
    };

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

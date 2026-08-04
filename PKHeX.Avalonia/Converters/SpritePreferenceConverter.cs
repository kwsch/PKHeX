using System;
using System.Globalization;
using Avalonia.Data.Converters;
using PKHeX.Avalonia.Services;

namespace PKHeX.Avalonia.Converters;

/// <summary>Maps <see cref="SpritePreference"/> values to user-facing labels (upstream wording).</summary>
public sealed class SpritePreferenceLabelConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value switch
    {
        SpritePreference.UseSuggested => "Use Suggested",
        SpritePreference.ForceSprites => "Force Sprites",
        SpritePreference.ForceMugshots => "Force Mugshots",
        SpritePreference.ForceArtwork => "Force Artwork",
        _ => value?.ToString(),
    };

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

using System;
using System.Collections.Concurrent;
using System.Reflection;
using SkiaSharp;

namespace PKHeX.Drawing.PokeSprite.Avalonia;

/// <summary>
/// Loads embedded PNG resources as <see cref="SKBitmap"/> objects, replacing the .resx/ResourceManager pattern.
/// </summary>
public static class ResourceLoader
{
    private static readonly Assembly _assembly = typeof(ResourceLoader).Assembly;
    private static readonly ConcurrentDictionary<string, SKBitmap?> _cache = new();
    private static readonly ConcurrentDictionary<string, string> _nameToManifest = new(StringComparer.OrdinalIgnoreCase);
    private static bool _initialized;

    private static void EnsureInitialized()
    {
        if (_initialized)
            return;

        var names = _assembly.GetManifestResourceNames();
        foreach (var name in names)
        {
            // Extract the logical name from the manifest resource name
            var key = ExtractResourceKey(name);
            if (key is not null)
                _nameToManifest.TryAdd(key, name);
        }
        _initialized = true;
    }

    private static string? ExtractResourceKey(string manifestName)
    {
        // Remove the .png extension
        if (!manifestName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            return null;

        // Get just the filename without extension
        var lastDot = manifestName.LastIndexOf('.', manifestName.Length - 5); // skip ".png"
        if (lastDot < 0)
            return null;

        var key = manifestName[(lastDot + 1)..^4]; // between last dot and .png
        key = key.Replace('-', '_'); // Normalize hyphens to underscores to match WinForms convention
        return key;
    }

    /// <summary>
    /// Gets an <see cref="SKBitmap"/> for the named resource.
    /// </summary>
    /// <param name="name">Resource name (e.g., "b_unknown", "slotHover68").</param>
    /// <returns>The decoded bitmap, or null if not found.</returns>
    public static SKBitmap? GetObject(string name)
    {
        if (_cache.TryGetValue(name, out var cached))
            return cached?.Copy(); // Return a copy so callers can mutate it

        EnsureInitialized();

        if (!_nameToManifest.TryGetValue(name, out var manifestName))
        {
            _cache.TryAdd(name, null);
            return null;
        }

        using var stream = _assembly.GetManifestResourceStream(manifestName);
        if (stream is null)
        {
            _cache.TryAdd(name, null);
            return null;
        }

        var bitmap = SKBitmap.Decode(stream);
        if (bitmap is not null)
        {
            // Ensure BGRA8888 format for consistent pixel manipulation
            if (bitmap.ColorType != SKColorType.Bgra8888)
            {
                var converted = new SKBitmap(bitmap.Width, bitmap.Height, SKColorType.Bgra8888, SKAlphaType.Unpremul);
                using var canvas = new SKCanvas(converted);
                canvas.DrawBitmap(bitmap, 0, 0);
                bitmap.Dispose();
                bitmap = converted;
            }
        }

        _cache.TryAdd(name, bitmap);
        return bitmap?.Copy(); // Return a copy so callers can mutate it
    }

    /// <summary>
    /// Gets a non-null <see cref="SKBitmap"/> for the named resource, throwing if not found.
    /// </summary>
    public static SKBitmap Get(string name) => GetObject(name) ?? throw new InvalidOperationException($"Resource '{name}' not found.");
}

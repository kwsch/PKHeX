using System;
using System.Collections.Concurrent;
using System.Reflection;
using SkiaSharp;

namespace PKHeX.Drawing.Misc.Avalonia;

/// <summary>
/// Loads embedded PNG resources as <see cref="SKBitmap"/> objects for the Misc drawing project.
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
            var key = ExtractResourceKey(name);
            if (key is not null)
                _nameToManifest.TryAdd(key, name);
        }
        _initialized = true;
    }

    private static string? ExtractResourceKey(string manifestName)
    {
        if (!manifestName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            return null;

        var lastDot = manifestName.LastIndexOf('.', manifestName.Length - 5);
        if (lastDot < 0)
            return null;

        return manifestName[(lastDot + 1)..^4];
    }

    /// <summary>
    /// Gets an <see cref="SKBitmap"/> for the named resource.
    /// </summary>
    public static SKBitmap? GetObject(string name)
    {
        if (_cache.TryGetValue(name, out var cached))
            return cached?.Copy();

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
        if (bitmap is not null && bitmap.ColorType != SKColorType.Bgra8888)
        {
            var converted = new SKBitmap(bitmap.Width, bitmap.Height, SKColorType.Bgra8888, SKAlphaType.Unpremul);
            using var canvas = new SKCanvas(converted);
            canvas.DrawBitmap(bitmap, 0, 0);
            bitmap.Dispose();
            bitmap = converted;
        }

        _cache.TryAdd(name, bitmap);
        return bitmap?.Copy();
    }

    /// <summary>
    /// Gets a non-null <see cref="SKBitmap"/> for the named resource, throwing if not found.
    /// </summary>
    public static SKBitmap Get(string name) => GetObject(name) ?? throw new InvalidOperationException($"Resource '{name}' not found.");
}

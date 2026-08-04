using PKHeX.Core;
using SkiaSharp;

namespace PKHeX.Avalonia.Services;

/// <summary>
/// Renders PKM sprites using SkiaSharp with real Pokémon sprites, returning PNG bytes.
/// </summary>
public sealed class AvaloniaSpriteRenderer : ISpriteRenderer
{
    private const int SpriteWidth = 68;
    private const int SpriteHeight = 56;

    private readonly SpriteLoader _loader = new();
    private readonly AppSettings _settings;
    private EntityContext _context = EntityContext.None;

    public AvaloniaSpriteRenderer(AppSettings settings)
    {
        _settings = settings;
    }

    public void Initialize(SaveFile sav)
    {
        _context = sav.Context;
        _loader.Style = SpriteStyleSelector.Resolve(_settings.Sprite.SpritePreference, sav);
        _loader.ClearCache();
    }

    public byte[]? GetSprite(PKM pk, bool isEgg = false)
    {
        if (pk.Species == 0)
            return GetEmptySlot();

        var baseSprite = _loader.GetSprite(
            pk.Species,
            pk.Form,
            (byte)pk.Gender,
            pk.IsEgg ? 0 : GetFormArg(pk),
            pk.IsShiny,
            _context);

        if (baseSprite is null)
            return CreatePlaceholderSprite(pk);

        using var composed = ComposeSprite(baseSprite, pk);
        return EncodePng(composed);
    }

    public byte[]? GetSprite(ushort species, byte form, byte gender, uint formarg, bool shiny, EntityContext context)
    {
        var skBitmap = _loader.GetSprite(species, form, gender, formarg, shiny, context);
        if (skBitmap == null) return null;
        return EncodePng(skBitmap);
    }

    public byte[]? GetItemSprite(int itemId)
    {
        var skBitmap = _loader.GetItemSprite(itemId);
        if (skBitmap is null) return null;
        return EncodePng(skBitmap);
    }

    public byte[]? GetEmptySlot()
    {
        using var surface = SKSurface.Create(new SKImageInfo(SpriteWidth, SpriteHeight));
        surface.Canvas.Clear(SKColors.Transparent);
        return EncodePng(surface);
    }

    private SKBitmap ComposeSprite(SKBitmap baseSprite, PKM pk)
    {
        var result = new SKBitmap(SpriteWidth, SpriteHeight);
        using var canvas = new SKCanvas(result);
        canvas.Clear(SKColors.Transparent);

        if (pk.IsEgg)
        {
            using var eggPaint = new SKPaint { Color = SKColors.White.WithAlpha(85) }; // ~33% opacity
            canvas.DrawBitmap(baseSprite, 0, 0, eggPaint);

            var eggSprite = _loader.GetEggSprite(pk.Species);
            if (eggSprite is not null)
                canvas.DrawBitmap(eggSprite, 0, 0);
        }
        else
        {
            canvas.DrawBitmap(baseSprite, 0, 0);
        }

        if (pk.HeldItem > 0)
        {
            var itemSprite = _loader.GetItemSprite(pk.HeldItem);
            if (itemSprite is not null)
            {
                // Bottom-right corner
                int x = SpriteWidth - itemSprite.Width - 2;
                int y = SpriteHeight - itemSprite.Height - 2;
                canvas.DrawBitmap(itemSprite, x, y);
            }
        }

        if (pk.IsShiny)
        {
            var shinyOverlay = _loader.GetShinyOverlay();
            if (shinyOverlay is not null)
            {
                using var shinyPaint = new SKPaint { Color = SKColors.White.WithAlpha(178) };
                canvas.DrawBitmap(shinyOverlay, 0, 0, shinyPaint);
            }
            else
            {
                DrawShinyIndicator(canvas);
            }
        }

        return result;
    }

    private static void DrawShinyIndicator(SKCanvas canvas)
    {
        using var paint = new SKPaint
        {
            Color = new SKColor(255, 215, 0), // Gold
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        // Simple star shape at top-left
        var path = new SKPath();
        float cx = 8, cy = 8, r = 6;
        for (int i = 0; i < 5; i++)
        {
            float angle = (float)(i * 144 - 90) * (float)Math.PI / 180f;
            float x = cx + r * (float)Math.Cos(angle);
            float y = cy + r * (float)Math.Sin(angle);
            if (i == 0)
                path.MoveTo(x, y);
            else
                path.LineTo(x, y);
        }
        path.Close();
        canvas.DrawPath(path, paint);
    }

    private static uint GetFormArg(PKM pk)
    {
        if (pk is IFormArgument fa)
            return fa.FormArgument;
        return 0;
    }

    private byte[] CreatePlaceholderSprite(PKM pk)
    {
        using var surface = SKSurface.Create(new SKImageInfo(SpriteWidth, SpriteHeight));
        var canvas = surface.Canvas;

        // An out-of-range/corrupt species can make the PersonalInfo lookup throw; fall back to
        // the "unknown type" neutral color rather than letting a bad placeholder crash rendering.
        int type1;
        try
        {
            type1 = pk.PersonalInfo.Type1;
        }
        catch (Exception)
        {
            type1 = -1;
        }

        var typeColor = GetTypeColor(type1);
        if (pk.IsShiny)
            typeColor = BlendColors(typeColor, new SKColor(255, 215, 0), 0.3f);

        using var bgPaint = new SKPaint
        {
            Color = typeColor,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };
        canvas.DrawRoundRect(new SKRoundRect(new SKRect(2, 2, SpriteWidth - 2, SpriteHeight - 2), 6), bgPaint);

        using var borderPaint = new SKPaint
        {
            Color = typeColor.WithAlpha(200),
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2
        };
        canvas.DrawRoundRect(new SKRoundRect(new SKRect(2, 2, SpriteWidth - 2, SpriteHeight - 2), 6), borderPaint);

        using var typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold);
        using var font = new SKFont(typeface, 12);
        using var textPaint = new SKPaint { Color = SKColors.White, IsAntialias = true };
        using var shadowPaint = new SKPaint { Color = SKColors.Black.WithAlpha(100), IsAntialias = true };

        var text = $"#{pk.Species}";
        canvas.DrawText(text, SpriteWidth / 2 + 1, SpriteHeight / 2 + 5, SKTextAlign.Center, font, shadowPaint);
        canvas.DrawText(text, SpriteWidth / 2, SpriteHeight / 2 + 4, SKTextAlign.Center, font, textPaint);

        return EncodePng(surface);
    }

    private static byte[] EncodePng(SKSurface surface)
    {
        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    private static byte[] EncodePng(SKBitmap bitmap)
    {
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    private static SKColor GetTypeColor(int type)
    {
        return type switch
        {
            0 => new SKColor(168, 168, 120),   // Normal
            1 => new SKColor(192, 48, 40),     // Fighting
            2 => new SKColor(168, 144, 240),   // Flying
            3 => new SKColor(160, 64, 160),    // Poison
            4 => new SKColor(224, 192, 104),   // Ground
            5 => new SKColor(184, 160, 56),    // Rock
            6 => new SKColor(168, 184, 32),    // Bug
            7 => new SKColor(112, 88, 152),    // Ghost
            8 => new SKColor(184, 184, 208),   // Steel
            9 => new SKColor(240, 128, 48),    // Fire
            10 => new SKColor(104, 144, 240),  // Water
            11 => new SKColor(120, 200, 80),   // Grass
            12 => new SKColor(248, 208, 48),   // Electric
            13 => new SKColor(248, 88, 136),   // Psychic
            14 => new SKColor(152, 216, 216),  // Ice
            15 => new SKColor(112, 56, 248),   // Dragon
            16 => new SKColor(112, 88, 72),    // Dark
            17 => new SKColor(238, 153, 172),  // Fairy
            _ => new SKColor(104, 160, 144)    // Unknown
        };
    }

    private static SKColor BlendColors(SKColor color1, SKColor color2, float ratio)
    {
        var r = (byte)(color1.Red * (1 - ratio) + color2.Red * ratio);
        var g = (byte)(color1.Green * (1 - ratio) + color2.Green * ratio);
        var b = (byte)(color1.Blue * (1 - ratio) + color2.Blue * ratio);
        return new SKColor(r, g, b);
    }
}

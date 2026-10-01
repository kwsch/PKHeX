extern alias atlas;

using System.Text.Json;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using PKHeX.Web.Services.Sprites;
using atlas::PKHeX.Web.SpriteAtlas;

namespace PKHeX.Web.Tests;

/// <summary>
/// The desktop's sprite images and the atlas generated from them, built once per test run, and a sprite catalog for component tests.
/// </summary>
internal static class SpriteFixtures
{
    private static readonly Lazy<ResxSpriteIndex> LazyIndex = new(() => ResxSpriteIndex.Load(ResxPath));
    private static readonly Lazy<AtlasFiles> LazyAtlas = new(() => AtlasWriter.Build(Index, SpriteSelection.Select(Index), PokeSpriteRoot));

    /// <summary>The PKHeX.Drawing.PokeSprite project directory.</summary>
    public static string PokeSpriteRoot => Path.Combine(SaveFixtures.RepositoryRoot, "PKHeX.Drawing.PokeSprite");

    /// <summary>The resx that names the desktop's images.</summary>
    public static string ResxPath => Path.Combine(PokeSpriteRoot, "Properties", "Resources.resx");

    /// <summary>Every image of the desktop, by resource name.</summary>
    public static ResxSpriteIndex Index => LazyIndex.Value;

    /// <summary>The generated atlas files.</summary>
    public static AtlasFiles Atlas => LazyAtlas.Value;

    /// <summary>The generated manifest, as the app reads it.</summary>
    public static SpriteManifest Manifest() => ParseManifest(Atlas.Manifest);

    /// <summary>Reads <paramref name="json"/> with the app's own manifest reader.</summary>
    public static SpriteManifest ParseManifest(string json) => JsonSerializer.Deserialize(json, SpriteManifestJson.Default.SpriteManifest)!;

    /// <summary>The sheet the app builds from the generated manifest.</summary>
    public static SpriteSheet Sheet() => SpriteSheet.From(Manifest());

    /// <summary>
    /// Registers a sprite catalog with <paramref name="context"/>: loaded with <paramref name="sheet"/>, or, without one, as in a build without sprites.
    /// </summary>
    public static SpriteCatalog AddCatalog(BunitContext context, SpriteSheet? sheet = null)
    {
        var catalog = new SpriteCatalog(new HttpClient(), context.JSInterop.JSRuntime);
        if (sheet is not null)
        {
            catalog.Use(sheet);
        }
        context.Services.AddSingleton(catalog);
        return catalog;
    }
}

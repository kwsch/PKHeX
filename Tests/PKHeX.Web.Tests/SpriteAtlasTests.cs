extern alias atlas;

using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using FluentAssertions;
using atlas::PKHeX.Web.SpriteAtlas;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The generated atlas is reproducible, holds each chosen image unchanged, and stays within host and browser limits.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed partial class SpriteAtlasTests
{
    [Fact]
    public void GeneratingTwiceGivesIdenticalFiles()
    {
        var again = AtlasWriter.Build(SpriteFixtures.Index, SpriteSelection.Select(SpriteFixtures.Index), SpriteFixtures.PokeSpriteRoot);
        var first = SpriteFixtures.Atlas;

        again.Atlas.Should().Equal(first.Atlas);
        (again.AtlasFileName, again.StylesheetFileName).Should().Be((first.AtlasFileName, first.StylesheetFileName));
        again.Stylesheet.Should().Be(first.Stylesheet);
        again.Manifest.Should().Be(first.Manifest);
        again.Sources.Should().Be(first.Sources);
    }

    [Fact]
    public void ManifestListsExactlyTheChosenImages()
    {
        var manifest = SpriteFixtures.Manifest();

        manifest.Sprites.Keys.Order(StringComparer.Ordinal).Should().Equal(SpriteSelection.Select(SpriteFixtures.Index));
        manifest.Sprites.Values.Select(e => e.Cell).Should().OnlyHaveUniqueItems();
        (manifest.Version, manifest.Image, manifest.Stylesheet, manifest.CellWidth, manifest.CellHeight, manifest.Columns)
            .Should().Be((AtlasWriter.ManifestVersion, SpriteFixtures.Atlas.AtlasFileName, SpriteFixtures.Atlas.StylesheetFileName, AtlasWriter.CellWidth, AtlasWriter.CellHeight, AtlasWriter.Columns));
    }

    /// <summary>The atlas and stylesheet are named by their content, so files from different builds are never combined.</summary>
    [Fact]
    public void AtlasAndStylesheetAreNamedByTheirContent()
    {
        var atlas = SpriteFixtures.Atlas;

        atlas.AtlasFileName.Should().Be($"pokemon.{Convert.ToHexStringLower(SHA256.HashData(atlas.Atlas))[..AtlasWriter.HashLength]}.png");
        atlas.StylesheetFileName.Should().Be($"sprites.{Convert.ToHexStringLower(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(atlas.Stylesheet)))[..AtlasWriter.HashLength]}.css");
    }

    [Fact]
    public void EveryCellHoldsItsSourceImageUnchanged()
    {
        var atlas = Png.Decode(SpriteFixtures.Atlas.Atlas);
        var used = new HashSet<(int, int)>();
        foreach (var (name, entry) in SpriteFixtures.Atlas.Entries)
        {
            var source = Png.Decode(File.ReadAllBytes(SpriteFixtures.Index.PathOf(name)));
            var x = entry.Cell % AtlasWriter.Columns * AtlasWriter.CellWidth;
            var y = entry.Cell / AtlasWriter.Columns * AtlasWriter.CellHeight;

            (entry.Width, entry.Height).Should().Be((source.Width, source.Height), name);
            atlas.Crop(x, y, source.Width, source.Height).Pixels.Should().Equal(source.Pixels, name);
            used.Add((x, y)).Should().BeTrue();
        }
    }

    [Fact]
    public void StylesheetHasOneRulePerCell()
    {
        var rules = Rule().Matches(SpriteFixtures.Atlas.Stylesheet);
        var entries = SpriteFixtures.Atlas.Entries.Values.ToDictionary(e => e.Cell);

        rules.Should().HaveCount(entries.Count);
        foreach (Match rule in rules)
        {
            var entry = entries[int.Parse(rule.Groups["cell"].Value)];
            Offset(rule.Groups["x"].Value).Should().Be(entry.Cell % AtlasWriter.Columns * AtlasWriter.CellWidth);
            Offset(rule.Groups["y"].Value).Should().Be(entry.Cell / AtlasWriter.Columns * AtlasWriter.CellHeight);
            (int.Parse(rule.Groups["w"].Value), int.Parse(rule.Groups["h"].Value)).Should().Be((entry.Width, entry.Height));
        }
        // Nothing but a comment and the rules: no imports, URLs or other selectors.
        Rule().Replace(SpriteFixtures.Atlas.Stylesheet, "").Trim().Should().MatchRegex(@"^/\*[^*]*\*/$");
    }

    [Fact]
    public void SourcesRecordTheHashOfEveryImage()
    {
        using var sources = JsonDocument.Parse(SpriteFixtures.Atlas.Sources);
        var sprites = sources.RootElement.GetProperty("sprites").EnumerateObject().ToArray();

        sprites.Select(s => s.Name).Should().Equal(SpriteSelection.Select(SpriteFixtures.Index));
        foreach (var sprite in sprites)
        {
            var file = sprite.Value.GetProperty("file").GetString()!;
            file.Should().NotContain("\\").And.StartWith("Resources/img/");
            var bytes = File.ReadAllBytes(Path.Combine(SpriteFixtures.PokeSpriteRoot, file));
            sprite.Value.GetProperty("sha256").GetString().Should().Be(Convert.ToHexStringLower(SHA256.HashData(bytes)));
        }
        SpriteFixtures.Atlas.Sources.Should().NotContain("\r", "the file must not depend on the system that generated it");
    }

    [Fact]
    public void AtlasStaysWithinHostAndBrowserLimits()
    {
        var atlas = Png.Decode(SpriteFixtures.Atlas.Atlas);

        atlas.Width.Should().BeLessThanOrEqualTo(AtlasWriter.MaxSide);
        atlas.Height.Should().BeLessThanOrEqualTo(AtlasWriter.MaxSide);
        SpriteFixtures.Atlas.Atlas.Length.Should().BeLessThanOrEqualTo(AtlasWriter.MaxBytes);
    }

    [Fact]
    public void WritesThePublishedFiles()
    {
        var directory = Directory.CreateTempSubdirectory("pkhex-sprites-");
        try
        {
            // An earlier build's outputs are removed, since everything in the directory is published; other files are left alone.
            string[] stale = ["pokemon.0000000000000000.png", "pokemon.png", "sprites.0000000000000000.css", "sprites.0000000000000000.css.br", "sprites.css", "manifest.json.gz"];
            foreach (var name in stale.Append("unrelated.txt"))
            {
                File.WriteAllText(Path.Combine(directory.FullName, name), "old");
            }

            AtlasWriter.Write(SpriteFixtures.Atlas, directory.FullName);

            directory.GetFiles().Select(f => f.Name).Order(StringComparer.Ordinal)
                .Should().Equal(SpriteFixtures.Atlas.FileNames.Append("unrelated.txt").Order(StringComparer.Ordinal));
            File.ReadAllBytes(Path.Combine(directory.FullName, SpriteFixtures.Atlas.AtlasFileName)).Should().Equal(SpriteFixtures.Atlas.Atlas);
            File.ReadAllBytes(Path.Combine(directory.FullName, AtlasWriter.ManifestFileName)).Take(3).Should().NotEqual(new byte[] { 0xEF, 0xBB, 0xBF }, "no byte order mark");

            // The precompressed copies hold exactly the text, and are written the same way every time.
            foreach (var (name, text) in new[] { (SpriteFixtures.Atlas.StylesheetFileName, SpriteFixtures.Atlas.Stylesheet), (AtlasWriter.ManifestFileName, SpriteFixtures.Atlas.Manifest), (AtlasWriter.SourcesFileName, SpriteFixtures.Atlas.Sources) })
            {
                var path = Path.Combine(directory.FullName, name);
                Decompress(new BrotliStream(File.OpenRead(path + ".br"), CompressionMode.Decompress)).Should().Be(text);
                Decompress(new GZipStream(File.OpenRead(path + ".gz"), CompressionMode.Decompress)).Should().Be(text);
            }
            var first = SpriteFixtures.Atlas.FileNames.ToDictionary(n => n, n => File.ReadAllBytes(Path.Combine(directory.FullName, n)));
            AtlasWriter.Write(SpriteFixtures.Atlas, directory.FullName);
            SpriteFixtures.Atlas.FileNames.Should().OnlyContain(n => File.ReadAllBytes(Path.Combine(directory.FullName, n)).SequenceEqual(first[n]));
        }
        finally
        {
            directory.Delete(true);
        }
    }

    [Fact]
    public void ResxNamesMapToTheirFiles()
    {
        SpriteFixtures.Index.PathOf("b_100_1").Replace('\\', '/').Should().EndWith("Resources/img/Big Pokemon Sprites/b_100-1.png");
        SpriteFixtures.Index.PathOf("b_490_e").Replace('\\', '/').Should().EndWith("Resources/img/Big Pokemon Sprites/b_490-e.png");
        SpriteFixtures.Index.Names.Should().OnlyContain(n => File.Exists(SpriteFixtures.Index.PathOf(n)));
    }

    private static string Decompress(Stream stream)
    {
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private static int Offset(string value) => value == "0" ? 0 : -int.Parse(value.TrimEnd('p', 'x'));

    [GeneratedRegex(@"\.sprite-c(?<cell>\d+)\{object-position:(?<x>0|-\d+px) (?<y>0|-\d+px);width:(?<w>\d+)px;height:(?<h>\d+)px\}\n")]
    private static partial Regex Rule();
}

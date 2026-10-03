using Bunit;
using FluentAssertions;
using PKHeX.Web.Services;
using PKHeX.Web.Services.Diagnostics;
using PKHeX.Web.Services.Sprites;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The app accepts only a manifest that matches the preloaded atlas, and resolves slots to its cells; a build without sprites loads nothing.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class SpriteSheetTests : IDisposable
{
    private readonly BunitContext context = new();

    public void Dispose() => context.Dispose();

    private static SlotSummary Slot(ushort species, byte form = 0, byte gender = 0, bool shiny = false, bool egg = false, bool item = false)
        => new(SlotRef.InBox(0, 0), true, true, species, null, egg, shiny, form, gender, item);

    private static int Cell(string name) => SpriteFixtures.Manifest().Sprites[name].Cell;

    [Fact]
    public void ReadsTheGeneratedManifest()
    {
        var sheet = SpriteFixtures.Sheet();

        sheet.Count.Should().Be(SpriteFixtures.Atlas.Entries.Count);
        sheet.ImagePath.Should().Be("sprites/" + SpriteFixtures.Atlas.AtlasFileName);
        sheet.StylesheetPath.Should().Be("sprites/" + SpriteFixtures.Atlas.StylesheetFileName);
        sheet.Contains("b_25_1c").Should().BeTrue();
        sheet.Contains("b_19_1").Should().BeFalse("Alolan forms cannot occur in Generation 6");
    }

    [Fact]
    public void ResolvesEachLayerToItsCell()
    {
        var sheet = SpriteFixtures.Sheet();

        sheet.Resolve(Slot(25, form: 1, shiny: true))!.Should().BeEquivalentTo(new { Base = Cell("b_25_1cs"), Unknown = (int?)null, Egg = (int?)null, Star = (int?)Cell("rare_icon_alt") });
        sheet.Resolve(Slot(1, form: 5))!.Should().BeEquivalentTo(new { Base = Cell("b_1"), Unknown = (int?)Cell("b_unknown"), Egg = (int?)null, Star = (int?)null });
        sheet.Resolve(Slot(490, egg: true))!.Should().BeEquivalentTo(new { Base = Cell("b_490"), Egg = (int?)Cell("b_490_e") });
    }

    [Fact]
    public void AFormTheAtlasLacksFallsBackLikeTheDesktop()
    {
        // Rattata form 1 is Alolan: the desktop has a sprite for it, but the Generation 6 atlas does not, so the default is shown marked unknown.
        var cells = SpriteFixtures.Sheet().Resolve(Slot(19, form: 1))!;

        (cells.Layers.Base, cells.Layers.UnknownForm).Should().Be(("b_19", true));
    }

    [Fact]
    public void EmptyPositionsAndBadEggsHaveNoSprite()
    {
        var sheet = SpriteFixtures.Sheet();

        sheet.Resolve(new SlotSummary(SlotRef.InBox(0, 0), false, true, 0, null, false, false)).Should().BeNull();
        sheet.Resolve(new SlotSummary(SlotRef.InBox(0, 0), true, false, 0, null, false, false)).Should().BeNull();
    }

    public static TheoryData<string, Func<SpriteManifest, SpriteManifest>> BrokenManifests => new()
    {
        { "another version", m => m with { Version = 2 } },
        { "another image", m => m with { Image = "../elsewhere.png" } },
        { "an unhashed image", m => m with { Image = "pokemon.png" } },
        { "another stylesheet", m => m with { Stylesheet = "https://example.invalid/x.css" } },
        { "no stylesheet", m => m with { Stylesheet = null! } },
        { "no layout", m => m with { Columns = 0 } },
        { "a cell outside the atlas", m => With(m, "b_1", new SpriteManifestEntry(m.Columns * m.Rows, 68, 56)) },
        { "a negative cell", m => With(m, "b_1", new SpriteManifestEntry(-1, 68, 56)) },
        { "two images in one cell", m => With(m, "b_1", new SpriteManifestEntry(m.Sprites["b_2"].Cell, 68, 56)) },
        { "an image larger than a cell", m => With(m, "b_1", new SpriteManifestEntry(m.Sprites["b_1"].Cell, 69, 56)) },
        { "no unknown mark", m => Without(m, "b_unknown") },
        { "no egg icon", m => Without(m, "b_egg") },
        { "no shiny star", m => Without(m, "rare_icon_alt") },
    };

    [Theory]
    [MemberData(nameof(BrokenManifests))]
    public void RejectsAManifestThatDoesNotMatchTheAtlas(string problem, Func<SpriteManifest, SpriteManifest> breakIt)
    {
        var from = () => SpriteSheet.From(breakIt(SpriteFixtures.Manifest()));

        from.Should().Throw<InvalidDataException>(problem);
    }

    [Fact]
    public async Task ABuildWithoutSpritesLoadsNothing()
    {
        context.JSInterop.Mode = JSRuntimeMode.Strict;
        using var handler = new RecordingHandler();
        var catalog = new SpriteCatalog(new HttpClient(handler) { BaseAddress = new Uri("https://localhost/") }, context.JSInterop.JSRuntime);

        await catalog.LoadAsync();

        catalog.State.Should().Be(SpriteCatalogState.NotIncluded);
        catalog.Resolve(Slot(25)).Should().BeNull();
        handler.Requests.Should().Be(0);
        context.JSInterop.Invocations.Should().BeEmpty();
    }

    [Fact]
    public async Task AFailedLoadIsRecordedForTheDiagnosticReport()
    {
        // The manifest request fails (the handler answers 404), so the catalog falls back to text and records why, by type only.
        using var handler = new RecordingHandler();
        var log = DiagnosticFixtures.NewLog(out var console);
        var catalog = new SpriteCatalog(new HttpClient(handler) { BaseAddress = new Uri("https://localhost/") }, context.JSInterop.JSRuntime, log);

        await catalog.LoadAsync(included: true);

        catalog.State.Should().Be(SpriteCatalogState.Failed);
        var entry = log.Entries.Should().ContainSingle().Subject;
        entry.Operation.Should().Be(DiagnosticOperation.Sprites);
        entry.Code.Name.Should().Be("sprites.not-loaded");
        entry.Code.ExceptionTypes.Should().Contain(typeof(HttpRequestException).FullName);
        console.Exceptions.Should().BeEmpty();
    }

    [Fact]
    public void ALoadedCatalogResolvesWithItsSheet()
    {
        var catalog = SpriteFixtures.AddCatalog(context, SpriteFixtures.Sheet());

        catalog.State.Should().Be(SpriteCatalogState.Loaded);
        catalog.Resolve(Slot(25))!.Base.Should().Be(Cell("b_25"));
    }

    private static SpriteManifest With(SpriteManifest m, string name, SpriteManifestEntry entry)
        => m with { Sprites = new Dictionary<string, SpriteManifestEntry>(m.Sprites) { [name] = entry } };

    private static SpriteManifest Without(SpriteManifest m, string name)
    {
        var sprites = new Dictionary<string, SpriteManifestEntry>(m.Sprites);
        sprites.Remove(name);
        return m with { Sprites = sprites };
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        public int Requests { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests++;
            return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.NotFound));
        }
    }
}

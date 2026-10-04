using Microsoft.Playwright;
using PKHeX.Core;
using PKHeX.Web.Components;
using PKHeX.Web.Services;
using PKHeX.Web.Services.Sprites;
using PKHeX.Web.State;
using Xunit;
using static Microsoft.Playwright.Assertions;
using static PKHeX.Web.Tests.ProofPage;

namespace PKHeX.Web.Tests;

/// <summary>
/// The sprite atlas in the published app: fetched once before a save can be chosen, the same for every save, and every sprite drawn from it
/// with no further request. A default publish shows text and requests no sprite file.
/// </summary>
[Collection(PublishedAppCollection.Name)]
[Trait(TestCategory.Name, TestCategory.E2E)]
public sealed class SpriteCatalogBrowserTests(PublishedAppFixture app)
{
    /// <summary>URL patterns of the generated files every sprite boot fetches: the manifest, and the hashed stylesheet and atlas it names.</summary>
    private static readonly string[] BootFilePatterns = ["**/sprites/manifest.json", "**/sprites/sprites.*.css", "**/sprites/pokemon.*.png"];

    /// <summary>
    /// Records when the file input first exists and is enabled: from then on a save can be chosen.
    /// Installed before navigation, so it sees the input appear.
    /// </summary>
    private const string ShellReadyRecorder = """
        (() => {
            const check = () => {
                const input = document.getElementById('save-file');
                if (input && !input.disabled && window.__shellReady === undefined) {
                    window.__shellReady = performance.now();
                }
            };
            new MutationObserver(check).observe(document, { subtree: true, childList: true, attributes: true });
        })();
        """;

    /// <summary>An XY save with a party member, a shiny cosplay Pikachu, a female Vivillon form, a Manaphy egg, and a female Pyroar in box 2.</summary>
    private static byte[] FirstSave() => SaveFixtures.Synthetic(false, customize: save =>
    {
        SaveFixtures.WithPartyMember("Leader")(save);
        SaveFixtures.WithBoxEntity(0, 1, pk => { pk.Species = (ushort)Species.Pikachu; pk.Form = 1; pk.SetShiny(); })(save);
        SaveFixtures.WithBoxEntity(0, 2, pk => { pk.Species = (ushort)Species.Vivillon; pk.Form = 3; pk.Gender = 1; })(save);
        SaveFixtures.WithBoxEntity(0, 3, pk => { pk.Species = (ushort)Species.Manaphy; pk.IsEgg = true; })(save);
        SaveFixtures.WithBoxEntity(1, 0, pk => { pk.Species = (ushort)Species.Pyroar; pk.Gender = 1; })(save);
        save.CurrentBox = 0;
    });

    /// <summary>
    /// An ORAS save with other species: Unown F, an egg holding an item, a form outside the listed range, both at once, and a shiny Hoopa in box 3.
    /// </summary>
    private static byte[] SecondSave() => SaveFixtures.Synthetic(true, customize: save =>
    {
        SaveFixtures.WithBoxEntity(0, 1, pk => { pk.Species = (ushort)Species.Unown; pk.Form = 5; })(save);
        SaveFixtures.WithBoxEntity(0, 2, pk => { pk.Species = (ushort)Species.Pikachu; pk.IsEgg = true; pk.HeldItem = 1; })(save);
        SaveFixtures.WithBoxEntity(0, 3, pk => { pk.Species = (ushort)Species.Bulbasaur; pk.Form = 5; })(save);
        SaveFixtures.WithBoxEntity(0, 4, pk => { pk.Species = (ushort)Species.Bulbasaur; pk.Form = 5; pk.IsEgg = true; pk.HeldItem = 1; })(save);
        SaveFixtures.WithBoxEntity(2, 5, pk => { pk.Species = (ushort)Species.Hoopa; pk.Form = 1; pk.SetShiny(); })(save);
        save.CurrentBox = 0;
    });

    [Trait(TestCategory.Needs, TestCategory.SpritePublish)]
    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task SpritesArePreloadedAndDrawnWithoutRequests(string engine, string prefix)
    {
        var sheet = PublishedSheet();
        var first = FirstSave();
        var second = SecondSave();

        await using var session = await BootWithSpritesAsync(engine, prefix);
        var page = session.Page;
        await Expect(page.Locator("#about-sprites")).ToContainTextAsync("Included");

        // Each sprite file was fetched once during the boot, and the atlas had arrived before a save could be chosen.
        foreach (var file in new[] { SpriteSheet.ManifestPath, sheet.StylesheetPath, sheet.ImagePath })
        {
            Assert.Equal(1, session.BootRequests.Count(p => p == "/" + prefix + file));
        }
        Assert.DoesNotContain("/" + prefix + "sprites/sources.json", session.BootRequests);
        var atlasBeforeShell = await page.EvaluateAsync<bool>("""
            () => {
                const atlas = performance.getEntriesByType('resource').find(e => /\/sprites\/pokemon\.[0-9a-f]+\.png$/.test(new URL(e.name).pathname));
                return atlas !== undefined && window.__shellReady !== undefined && atlas.responseEnd <= window.__shellReady;
            }
            """);
        Assert.True(atlasBeforeShell, "The atlas was not loaded before the file input was enabled.");

        // Two different saves, party and several boxes: every slot shows exactly the layers the app's own resolver chooses.
        await Load(page, first);
        await AssertSpritesAsync(page, sheet, first, "#party-grid", SlotRef.PartyPositions, i => SlotRef.InParty(i));
        await AssertBoxSpritesAsync(page, sheet, first, 0);
        await page.Locator("#box-next").ClickAsync();
        await AssertBoxSpritesAsync(page, sheet, first, 1);

        await Load(page, second);
        await Expect(page.Locator("#overview-game")).ToHaveTextAsync("Alpha Sapphire");
        await AssertBoxSpritesAsync(page, sheet, second, 0);
        await page.Locator("#box-select").SelectOptionAsync("2");
        await AssertBoxSpritesAsync(page, sheet, second, 2);

        // The list alternative draws the same sprites; neither view scrolls sideways on a phone.
        await page.SetViewportSizeAsync(375, 700);
        Assert.True(await NoHorizontalScrollAsync(page), "The grid with sprites scrolls horizontally at 375 px.");
        await page.Locator("#storage-as-list").CheckAsync();
        await Expect(page.Locator("#box-list .sprite")).ToHaveCountAsync(StorageView.Box(SaveFixtures.Open(second), 2).Slots.Count(s => s.CanOpen));
        Assert.True(await NoHorizontalScrollAsync(page), "The list with sprites scrolls horizontally at 375 px.");

        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");

        // A fresh visit that opens only the second save makes the same boot requests: nothing depends on the save.
        await using var other = await BootWithSpritesAsync(engine, prefix);
        await Load(other.Page, second);
        await AssertBoxSpritesAsync(other.Page, sheet, second, 0);
        await other.AssertNoNetworkOrPersistenceAsync();
        Assert.Equal(session.BootRequests.Order(StringComparer.Ordinal), other.BootRequests.Order(StringComparer.Ordinal));
    }

    /// <summary>
    /// Every atlas cell, as the browser draws it, has the same pixels as the browser's own decoding of its source file. This checks the
    /// generator's PNG codec and packing independently of the codec itself.
    /// </summary>
    [Trait(TestCategory.Needs, TestCategory.SpritePublish)]
    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(Engines))]
    public async Task EveryCellMatchesTheBrowsersDecodingOfItsSource(string engine)
    {
        var manifest = SpriteFixtures.ParseManifest(File.ReadAllText(Path.Combine(app.SpriteRoot, "sprites", "manifest.json")));
        using var sourcesJson = System.Text.Json.JsonDocument.Parse(File.ReadAllText(Path.Combine(app.SpriteRoot, "sprites", "sources.json")));
        var items = sourcesJson.RootElement.GetProperty("sprites").EnumerateObject().Select(s =>
        {
            var entry = manifest.Sprites[s.Name];
            var bytes = File.ReadAllBytes(Path.Combine(SpriteFixtures.PokeSpriteRoot, s.Value.GetProperty("file").GetString()!));
            return new
            {
                name = s.Name,
                x = entry.Cell % manifest.Columns * manifest.CellWidth,
                y = entry.Cell / manifest.Columns * manifest.CellHeight,
                width = entry.Width,
                height = entry.Height,
                png = Convert.ToBase64String(bytes),
            };
        }).ToArray();

        await using var session = await BootWithSpritesAsync(engine, "");
        var page = session.Page;
        await Load(page, FirstSave());
        await Expect(page.Locator("#box-grid .sprite img").First).ToBeVisibleAsync();

        // Draws from an img already on the page, so the check itself makes no request.
        var mismatches = await page.EvaluateAsync<string[]>("""
            async items => {
                const atlas = document.querySelector('#box-grid .sprite img');
                await atlas.decode();
                const read = (source, x, y, width, height) => {
                    const canvas = document.createElement('canvas');
                    canvas.width = width;
                    canvas.height = height;
                    const context = canvas.getContext('2d', { willReadFrequently: true });
                    context.drawImage(source, x, y, width, height, 0, 0, width, height);
                    return context.getImageData(0, 0, width, height).data;
                };
                const failed = [];
                for (const item of items) {
                    const bytes = Uint8Array.from(atob(item.png), c => c.charCodeAt(0));
                    const source = await createImageBitmap(new Blob([bytes], { type: 'image/png' }));
                    const expected = read(source, 0, 0, item.width, item.height);
                    const actual = read(atlas, item.x, item.y, item.width, item.height);
                    if (source.width !== item.width || source.height !== item.height || expected.some((v, i) => v !== actual[i])) {
                        failed.push(item.name);
                    }
                }
                return failed;
            }
            """, items);

        Assert.True(mismatches.Length == 0, $"Atlas cells differ from their sources: {string.Join(", ", mismatches.Take(20))}");
        Assert.Equal(manifest.Sprites.Count, items.Length);
        await session.AssertNoNetworkOrPersistenceAsync();
    }

    /// <summary>
    /// If any sprite file fails to load, the app shows text, says so in About, and never asks for a sprite again.
    /// </summary>
    [Trait(TestCategory.Needs, TestCategory.SpritePublish)]
    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(BlockedFiles))]
    public async Task AFailedLoadFallsBackToTextWithoutFurtherRequests(string engine, string blocked)
    {
        await using var session = await app.CreateSessionAsync(engine, "", sprites: true);
        await session.Page.RouteAsync(blocked, route => route.AbortAsync());
        await AssertTextFallbackAsync(session);
    }

    /// <summary>
    /// A sprite request that never answers must not leave the app on its loading message: after the catalog's time limit it starts with text.
    /// </summary>
    /// <remarks>
    /// One engine only, the first this run covers (Chromium in a full run): the time limit is the catalog's own C# code, the same in every engine,
    /// and each run waits out the full limit. The other engines' fallback is covered by <see cref="AFailedLoadFallsBackToTextWithoutFurtherRequests"/>.
    /// When CI runs one engine per job, each job runs it in its own engine, in parallel.
    /// </remarks>
    [TierFact(TestCategory.E2E)]
    [Trait(TestCategory.Needs, TestCategory.SpritePublish)]
    public async Task AStalledAtlasFallsBackToTextAfterTheTimeLimit()
    {
        await using var session = await app.CreateSessionAsync(PublishedAppFixture.Engines[0], "", sprites: true);
        // Never fulfilled, continued or aborted: the request stays pending.
        await session.Page.RouteAsync(BootFilePatterns[2], _ => { });
        var started = session.ElapsedMs;
        await AssertTextFallbackAsync(session, waitForNetworkIdle: false);
        Assert.True(session.ElapsedMs - started >= SpriteCatalog.LoadTimeout.TotalMilliseconds, "The app started before the sprite time limit.");
    }

    /// <summary>
    /// Opens the app, expecting it to start without sprites, then checks it shows text and makes no request after starting.
    /// </summary>
    private static async Task AssertTextFallbackAsync(AppSession session, bool waitForNetworkIdle = true)
    {
        var page = session.Page;
        await page.GotoAsync(session.AppUrl);
        await Expect(page.Locator("#save-file")).ToBeEnabledAsync(new() { Timeout = 60000 });
        if (waitForNetworkIdle)
        {
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }
        _ = session.TakeRecorded();

        await Expect(page.Locator("#about-sprites")).ToHaveTextAsync("Part of this build, but they could not be loaded; slots are shown as text.");
        await Load(page, FirstSave());
        await Expect(page.Locator("#box-grid button")).ToHaveCountAsync(30);
        await Expect(page.Locator(".sprite")).ToHaveCountAsync(0);
        await Expect(page.Locator("#box-grid-1")).ToHaveTextAsync(SlotText.Short(StorageView.Box(SaveFixtures.Open(FirstSave()), 0).Slots[1]));

        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task ADefaultPublishShowsTextAndRequestsNoSprites(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;

        Assert.DoesNotContain(session.BootRequests, p => p.Contains("/sprites/", StringComparison.Ordinal));
        await Expect(page.Locator("#about-sprites")).ToHaveTextAsync("Not included in this build; slots are shown as text.");
        await Load(page, FirstSave());
        await Expect(page.Locator("#box-grid button")).ToHaveCountAsync(30);
        await Expect(page.Locator(".sprite, #box-grid img, #party-grid img")).ToHaveCountAsync(0);

        await session.AssertNoNetworkOrPersistenceAsync();
    }

    /// <summary>One case per engine.</summary>
    public static IEnumerable<object[]> Engines() => PublishedAppFixture.Engines.Select(e => new object[] { e });

    /// <summary>Each engine with each sprite file blocked in turn.</summary>
    public static IEnumerable<object[]> BlockedFiles() =>
        from engine in PublishedAppFixture.Engines
        from file in BootFilePatterns
        select new object[] { engine, file };

    /// <summary>The sheet the published manifest describes, read as the app reads it.</summary>
    private SpriteSheet PublishedSheet() => SpriteSheet.From(SpriteFixtures.ParseManifest(File.ReadAllText(Path.Combine(app.SpriteRoot, "sprites", "manifest.json"))));

    /// <summary>Boots the sprite publish with <see cref="ShellReadyRecorder"/> installed, checking the boot as <see cref="PublishedAppFixture.BootAsync"/> does.</summary>
    private async Task<AppSession> BootWithSpritesAsync(string engine, string prefix)
    {
        var session = await app.CreateSessionAsync(engine, prefix, sprites: true);
        try
        {
            await session.Page.AddInitScriptAsync(ShellReadyRecorder);
            await session.Page.GotoAsync(session.AppUrl);
            await Expect(session.Page.Locator("#save-file")).ToBeEnabledAsync(new() { Timeout = 60000 });
            await app.AssertStaticBootAsync(session);
            return session;
        }
        catch
        {
            await session.DisposeAsync();
            throw;
        }
    }

    private static Task AssertBoxSpritesAsync(IPage page, SpriteSheet sheet, byte[] save, int box) =>
        AssertSpritesAsync(page, sheet, save, "#box-grid", StorageView.Box(SaveFixtures.Open(save), box).Slots.Count, i => SlotRef.InBox(box, i));

    /// <summary>
    /// Every slot of the grid shows the layers <see cref="SpriteSheet.Resolve"/> chooses for the same save read natively (none for empty slots),
    /// and every layer's image is loaded.
    /// </summary>
    private static async Task AssertSpritesAsync(IPage page, SpriteSheet sheet, byte[] save, string grid, int count, Func<int, SlotRef> slot)
    {
        var session = SaveFixtures.Open(save);
        var summaries = slot(0).IsParty ? StorageView.Party(session) : StorageView.Box(session, slot(0).Box).Slots;
        await Expect(page.Locator($"{grid} button")).ToHaveCountAsync(count);
        for (var i = 0; i < count; i++)
        {
            var cells = sheet.Resolve(summaries[i]);
            var expected = cells is null ? [] : cells.SpeciesClasses().Concat(cells.OverlayClasses()).ToArray();
            var actual = await page.Locator($"{grid}-{i} .sprite img").EvaluateAllAsync<string[]>("imgs => imgs.map(img => img.className)");
            Assert.True(expected.SequenceEqual(actual), $"{grid}-{i}: expected [{string.Join(", ", expected)}], got [{string.Join(", ", actual)}].");
            var faded = await page.Locator($"{grid}-{i} .sprite-species.sprite-faded").CountAsync();
            Assert.Equal(cells?.SpeciesFaded == true ? 1 : 0, faded);
        }
        var unloaded = await page.Locator($"{grid} .sprite img").EvaluateAllAsync<int>("imgs => imgs.filter(img => !img.complete || img.naturalWidth === 0).length");
        Assert.Equal(0, unloaded);
    }

    private static Task<bool> NoHorizontalScrollAsync(IPage page) =>
        page.EvaluateAsync<bool>("() => document.documentElement.scrollWidth <= document.documentElement.clientWidth");
}

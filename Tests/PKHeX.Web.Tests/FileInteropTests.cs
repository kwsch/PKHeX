using Microsoft.Playwright;
using PKHeX.Web.Services;
using Xunit;
using static Microsoft.Playwright.Assertions;
using static PKHeX.Web.Tests.ProofPage;

namespace PKHeX.Web.Tests;

/// <summary>
/// File drop, drop refusals, navigation guards and download naming in the published app (WEB-SAVE-001/002, WEB-SESSION-007).
/// </summary>
/// <remarks>
/// Drops are dispatched in the page with a script-built <c>DataTransfer</c>, since Playwright cannot drag files from the OS.
/// Synthetic events never trigger browser navigation, so the guards are checked through <c>defaultPrevented</c> instead.
/// Synthetic drops also skip the drag operation, whose <c>dropEffect</c> decides whether a drop happens at all, so link drops are
/// additionally made with a real Playwright drag. Folders cannot be built in script, so directory refusal is checked by calling
/// <c>classifyDrop</c> with mock items.
/// </remarks>
[Collection(PublishedAppCollection.Name)]
[Trait(TestCategory.Name, TestCategory.E2E)]
public sealed class FileInteropTests(PublishedAppFixture app)
{
    /// <summary>
    /// Dispatches a drop on the first element matching <c>selector</c> and returns whether the page prevented the browser's default action.
    /// Each file is <c>{ name, data }</c> with base64 data; <c>text</c>, when set, is added as a <c>text/uri-list</c> item.
    /// </summary>
    private const string DropScript = """
        ({ selector, files, text }) => {
            const dataTransfer = new DataTransfer();
            for (const file of files) {
                const binary = atob(file.data);
                const bytes = new Uint8Array(binary.length);
                for (let i = 0; i < binary.length; i++) bytes[i] = binary.charCodeAt(i);
                dataTransfer.items.add(new File([bytes], file.name, { type: 'application/octet-stream' }));
            }
            if (text) dataTransfer.setData('text/uri-list', text);
            const event = new DragEvent('drop', { dataTransfer, bubbles: true, cancelable: true });
            document.querySelector(selector).dispatchEvent(event);
            return event.defaultPrevented;
        }
        """;

    /// <summary>Adds a draggable element carrying a link, as if dragged from another page.</summary>
    private const string LinkSourceScript = """
        () => {
            const source = document.createElement('div');
            source.id = 'drag-link';
            source.draggable = true;
            source.textContent = 'link';
            source.addEventListener('dragstart', e => e.dataTransfer.setData('text/uri-list', 'https://example.com/'));
            document.body.prepend(source);
        }
        """;

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task DroppedFileUsesPickerPipelineAndRefusalsKeepSession(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        var url = page.Url;

        // Before any save is open, a refusal does not claim that a session was kept.
        Assert.True(await Drop(page, "#save-drop", [("main", SaveFixtures.Synthetic(true)), ("main2", SaveFixtures.Synthetic(true))]));
        await Expect(page.Locator("#message")).ToHaveTextAsync("Drop a single save file.");

        // One dropped file loads like a picked one, and its name is offered for the download.
        var xy = SaveFixtures.Synthetic(false);
        var expected = SaveFixtures.Parse(xy).Write().ToArray();
        Assert.True(await Drop(page, "#save-drop", [("Y save.sav", xy)]), "The drop zone did not take over the drop.");
        await Expect(page.Locator("#overview-game")).ToHaveTextAsync("X");
        Assert.True((await Download(page, "Y save.sav")).AsSpan().SequenceEqual(expected));

        // Refused drops explain themselves, and neither replace the session nor navigate.
        var oras = SaveFixtures.Synthetic(true);
        Assert.True(await Drop(page, "#save-drop", [("main", oras), ("main2", oras)]));
        await Expect(page.Locator("#message")).ToHaveTextAsync("Drop a single save file. The previous session was retained.");
        Assert.True(await Drop(page, "#save-drop", [], "https://example.com/main"));
        await Expect(page.Locator("#message")).ToHaveTextAsync("Only files can be dropped here, not text or links. The previous session was retained.");

        // Outside the zone, files and links are blocked and ignored.
        Assert.True(await Drop(page, "h1", [("main", oras)]), "A file dropped outside the zone was not blocked.");
        Assert.True(await Drop(page, "h1", [], "https://example.com/"), "A link dropped outside the zone was not blocked.");
        await Expect(page.Locator("#overview-game")).ToHaveTextAsync("X");
        Assert.True(page.Url == url, "A drop navigated the page.");

        // Text can still be dropped into a text field; a file cannot, and nothing can be dropped on other controls.
        await Select(page);
        Assert.False(await Drop(page, "#nickname", [], "Dropped"), "Text drops into a field were blocked.");
        Assert.True(await Drop(page, "#nickname", [("main", oras)]), "A file dropped on a field was not blocked.");
        Assert.True(await Drop(page, "#nicknamed", [], "https://example.com/"), "A link dropped on a checkbox was not blocked.");

        // A real drag of a link reaches the zone's drop handler and is explained, and dragging it elsewhere does not navigate.
        await page.EvaluateAsync(LinkSourceScript);
        await Expect(page.Locator("#message")).Not.ToHaveTextAsync(new System.Text.RegularExpressions.Regex("^Only files"));
        await page.DragAndDropAsync("#drag-link", "#save-drop");
        await Expect(page.Locator("#message")).ToHaveTextAsync("Only files can be dropped here, not text or links. The previous session was retained.");
        await page.DragAndDropAsync("#drag-link", "#nicknamed");
        await page.DragAndDropAsync("#drag-link", "h1");
        Assert.True(page.Url == url, "A dragged link navigated the page.");
        await page.Locator("#cancel-draft").ClickAsync();

        await Expect(page.Locator("#overview-game")).ToHaveTextAsync("X");
        Assert.True((await Download(page, "Y save.sav")).AsSpan().SequenceEqual(expected), "A refused drop changed the session.");
        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task PickerNamesAreSanitisedAndFoldersAreRefused(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;

        const string hostile = "backup\u0007 \u202Ecopy.sav";
        var bytes = SaveFixtures.Synthetic(true);
        await Load(page, bytes, hostile);
        await Expect(page.Locator("#overview-game")).ToHaveTextAsync("Omega Ruby");
        var sanitised = FileNaming.Sanitize(hostile);
        Assert.True(sanitised == "backup copy.sav");
        Assert.True((await Download(page, sanitised)).AsSpan().SequenceEqual(SaveFixtures.Parse(bytes).Write().Span));

        // Browsers strip leading dots when saving, so they are removed before the name is offered.
        await Load(page, bytes, "..sav");
        Assert.True((await Download(page, "sav")).AsSpan().SequenceEqual(SaveFixtures.Parse(bytes).Write().Span));

        // A dropped folder only shows up as an entry during a real drop, so classify mock items directly.
        var results = await page.EvaluateAsync<string[]>("""
            async () => {
                const { classifyDrop } = await import(new URL('browser.js', document.baseURI).href);
                const file = entry => ({ kind: 'file', webkitGetAsEntry: () => entry });
                const drop = (...items) => ({ items, files: { length: items.filter(i => i.kind === 'file').length } });
                return [
                    classifyDrop(drop(file({ isDirectory: true }))),
                    classifyDrop(drop(file({ isDirectory: false }))),
                    classifyDrop(drop(file(null))),
                    classifyDrop(drop({ kind: 'file' })),
                    classifyDrop(drop(file(null), { kind: 'string' })),
                    classifyDrop(drop(file(null), file(null))),
                    classifyDrop(drop({ kind: 'string' })),
                    classifyDrop(null),
                ];
            }
            """);
        Assert.Equal(["directory", "ok", "ok", "ok", "ok", "multiple", "not-a-file", "not-a-file"], results);

        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }

    private static Task<bool> Drop(IPage page, string selector, (string Name, byte[] Data)[] files, string? text = null) =>
        page.EvaluateAsync<bool>(DropScript, new
        {
            selector,
            files = files.Select(f => new { name = f.Name, data = Convert.ToBase64String(f.Data) }).ToArray(),
            text,
        });
}

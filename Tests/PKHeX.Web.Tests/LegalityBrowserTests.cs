using System.Text.Json;
using PKHeX.Core;
using PKHeX.Web.Services;
using PKHeX.Web.State;
using Xunit;
using static Microsoft.Playwright.Assertions;
using static PKHeX.Web.Tests.ProofPage;

namespace PKHeX.Web.Tests;

/// <summary>
/// Legality in the published app (WEB-LEGAL-001 to 003): opening a slot analyses it once idle, with native Core's verdict and findings; an edit
/// shows Stale at once and is analysed again only after the idle delay; findings link to the inspector; nothing leaves the page.
/// </summary>
[Collection(PublishedAppCollection.Name)]
[Trait(TestCategory.Name, TestCategory.E2E)]
public sealed class LegalityBrowserTests(PublishedAppFixture app)
{
    /// <summary>
    /// Records, from now on, every text <c>#legality-status</c> shows with <c>performance.now()</c> and the number of findings listed with it.
    /// It watches the whole panel, so a findings list rendered while the status is stale would be caught.
    /// </summary>
    private const string RecordStatus = """
        () => {
            window.legalityLog = [];
            const panel = document.getElementById('legality');
            const record = () => window.legalityLog.push({
                text: document.getElementById('legality-status').textContent,
                findings: panel.querySelectorAll('#legality-findings li').length,
                at: performance.now(),
            });
            record();
            new MutationObserver(record).observe(panel, { childList: true, characterData: true, subtree: true });
        }
        """;

    private sealed record StatusChange(string Text, int Findings, double At);

    private static LegalityStatus StatusOf(LegalityResult result) => result.Verdict switch
    {
        LegalityVerdict.Valid => LegalityStatus.Valid,
        LegalityVerdict.Invalid => LegalityStatus.Invalid,
        _ => LegalityStatus.Unavailable,
    };

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task SlotsAreAnalysedOnceIdleAndEditsGoStaleAtOnce(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        // Box 1, slot 1 holds a known illegal Ditto; slot 2 the known legal Zigzagoon.
        var bytes = SaveFixtures.Synthetic(true, legal: false, customize: SaveFixtures.WithBoxEntities([SaveFixtures.ReadEntity(true)]));
        var native = SaveFixtures.Open(bytes);
        await Load(page, bytes);

        // Opening a slot analyses it without a click, with native Core's verdict, findings and reports.
        await Select(page);
        var illegal = LegalityService.Default.Analyze(native, native.Select(SaveFixtures.FirstBoxSlot), out _);
        await Expect(page.Locator("#legality-status")).ToHaveTextAsync("Invalid");
        await Expect(page.Locator("#legality-findings li")).ToHaveCountAsync(illegal.Findings.Count);
        await Expect(page.Locator("#legality-summary")).ToHaveTextAsync(Components.LegalityText.Counts(illegal));
        Assert.True(await page.Locator("#legality-report").TextContentAsync() == illegal.Report, "Browser and native short reports differ.");
        Assert.True(await page.Locator("#legality-report-verbose").TextContentAsync() == illegal.VerboseReport, "Browser and native verbose reports differ.");
        await Expect(page.Locator("#legality-engine")).ToContainTextAsync($"PKHeX.Core {BuildInfo.CoreVersion}");
        await Expect(page.Locator("#legality-guarantee")).ToBeVisibleAsync();

        // A finding's link moves focus to the inspector section it is about.
        // The link must not navigate: the whole URL, fragment included, stays as it was.
        var link = page.Locator("#legality-findings a").First;
        var target = (await link.GetAttributeAsync("href"))!.TrimStart('#');
        var before = page.Url;
        await link.ClickAsync();
        await Expect(page.Locator($"#{target}")).ToBeFocusedAsync();
        Assert.Equal(before, page.Url);
        Assert.Equal("", await page.EvaluateAsync<string>("() => location.hash"));

        // The legal entity in the next slot.
        await Select(page, SlotRef.InBox(0, 1));
        await Expect(page.Locator("#legality-status")).ToHaveTextAsync("Valid");
        await Expect(page.Locator("#legality-summary")).ToHaveTextAsync("No problems found.");

        // Back on the illegal entity, an edit goes stale at once, with its findings withdrawn, is analysed only after the idle delay, and then
        // shows the result for the edited draft.
        await Select(page);
        await Expect(page.Locator("#legality-status")).ToHaveTextAsync("Invalid");
        var edited = native.Select(SaveFixtures.FirstBoxSlot);
        edited.EditNickname("Quill", edited.IsNicknamed);
        var expected = LegalityService.Default.Analyze(native, edited, out _);
        var expectedStatus = Components.LegalityText.Status(StatusOf(expected));
        await page.EvaluateAsync(RecordStatus);
        await page.Locator("#nickname").FillAsync("Quill");
        await Expect(page.Locator("#legality-status")).ToHaveTextAsync(expectedStatus);
        await Expect(page.Locator("#legality-findings li")).ToHaveCountAsync(expected.Findings.Count);
        Assert.True(await page.Locator("#legality-report").TextContentAsync() == expected.Report, "The edited draft's report differs from native Core.");
        var log = await page.EvaluateAsync<JsonElement>("() => window.legalityLog");
        // Consecutive records of the same text (one change can raise several mutations) are collapsed; each keeps its first record's time.
        var records = log.EnumerateArray()
            .Select(e => new StatusChange(e.GetProperty("text").GetString()!, e.GetProperty("findings").GetInt32(), e.GetProperty("at").GetDouble()))
            .ToList();
        var changes = records.Aggregate(new List<StatusChange>(), (list, change) =>
        {
            if (list.Count == 0 || list[^1].Text != change.Text)
            {
                list.Add(change);
            }
            return list;
        });
        var texts = changes.Select(c => c.Text).ToList();
        string[] sequence = ["Invalid", "Stale", "Pending", expectedStatus];
        Assert.True(texts.SequenceEqual(sequence), $"Unexpected status sequence: {string.Join(" → ", texts)}");
        Assert.True(records.Where(r => r.Text is "Stale" or "Pending").All(r => r.Findings == 0), "Findings were listed while the result was stale or pending.");
        var idle = changes[2].At - changes[1].At;
        Assert.True(idle >= DraftLegality.IdleDelay.TotalMilliseconds - 50, $"Analysis started {idle:F0} ms after the edit, before the idle delay.");

        // A refused edit is stale and cannot be analysed until it is corrected.
        await page.Locator("#nickname").EvaluateAsync("(e, v) => { e.value = v; e.dispatchEvent(new Event('input', { bubbles: true })); }", "Bad\u0007");
        await Expect(page.Locator("#legality-status")).ToHaveTextAsync("Stale");
        await Expect(page.Locator("#legality-findings li")).ToHaveCountAsync(0);
        await Expect(page.Locator("#analyze")).ToBeDisabledAsync();
        await page.Locator("#cancel-draft").ClickAsync();
        await Expect(page.Locator("#legality-status")).ToHaveTextAsync("Invalid");

        // The panel fits a phone without horizontal scrolling, with the findings and both reports expanded.
        await page.Locator("#legality-details summary").ClickAsync();
        await page.SetViewportSizeAsync(375, 800);
        Assert.True(await page.EvaluateAsync<bool>("() => document.documentElement.scrollWidth <= document.documentElement.clientWidth"), "The legality panel scrolls horizontally at 375 px.");

        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task AnApplyIsAnalysedAgainAndAPartyMemberAsAPartyMember(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        // ORAS, where the known legal entity is its trainer's own Pokémon (see SaveFixtures.Synthetic).
        var bytes = SaveFixtures.Synthetic(true, customize: SaveFixtures.WithPartyMember("Leader"));
        var native = SaveFixtures.Parse(bytes);
        await Load(page, bytes);

        await Select(page);
        await Expect(page.Locator("#legality-status")).ToHaveTextAsync("Valid");
        await page.Locator("#nickname").FillAsync("Applied");
        await page.Locator("#nicknamed").CheckAsync();
        await page.Locator("#apply").ClickAsync();
        await Expect(page.Locator("#message")).ToContainTextAsync("applied in memory");
        var applied = SaveFixtures.Open(bytes).Select(SaveFixtures.FirstBoxSlot);
        applied.EditNickname("Applied", true);
        var expected = NativeLegality.Of(native, applied.Preview(), StorageSlotType.Box);
        await Expect(page.Locator("#legality-status")).ToHaveTextAsync(expected.Verdict);
        Assert.True(await page.Locator("#legality-report").TextContentAsync() == expected.Report, "The applied entity's report differs from native Core.");

        await Select(page, SlotRef.InParty(0));
        var party = NativeLegality.Of(native, native.GetPartySlotAtIndex(0), StorageSlotType.Party);
        await Expect(page.Locator("#legality-status")).ToHaveTextAsync(party.Verdict);
        Assert.True(await page.Locator("#legality-report-verbose").TextContentAsync() == party.VerboseReport, "The party report differs from native Core.");

        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }
}

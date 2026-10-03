using System.Text;
using Microsoft.Playwright;
using PKHeX.Web.Components;
using PKHeX.Web.Services.Diagnostics;
using Xunit;
using static Microsoft.Playwright.Assertions;
using static PKHeX.Web.Tests.ProofPage;

namespace PKHeX.Web.Tests;

/// <summary>
/// Hostile input and safe diagnostics in the published app (WEB-SEC-003, WEB-SEC-005): stored names and file names render as inert text and
/// cannot reorder the page, the shell runs no inline script and the CSP blocks one, and a diagnostic report names a refused file by code only.
/// </summary>
[Collection(PublishedAppCollection.Name)]
[Trait(TestCategory.Name, TestCategory.E2E)]
public sealed class HostileInputBrowserTests(PublishedAppFixture app)
{
    /// <summary>Elements a stored name would create if it were parsed as markup.</summary>
    private const string InjectedElements = "main b, main i, main script, img[src=x]";

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task HostileNamesAndFileNamesRenderAsInertText(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        await Load(page, HostileFixtures.Save(), HostileFixtures.FileName);

        await Expect(page.Locator("#overview-file")).ToHaveTextAsync(HostileFixtures.ShownFileName);
        await Expect(page.Locator("#overview-trainer")).ToHaveTextAsync(HostileFixtures.TrainerName);
        await Expect(page.Locator("#box-title")).ToHaveTextAsync($"1. {TestText.Isolated(HostileFixtures.BoxName)}");
        var label = await page.Locator("#box-grid-0").GetAttributeAsync("aria-label");
        Assert.EndsWith($"\"{TestText.Isolated(HostileFixtures.ShownNickname)}\"", label);

        await Select(page);
        await Expect(page.Locator("#message")).ToContainTextAsync($"\"{TestText.Isolated(HostileFixtures.ShownNickname)}\"");
        await Expect(page.Locator("#inspect-nickname")).ToHaveTextAsync(HostileFixtures.ShownNickname);
        await Expect(page.Locator("#inspect-ot")).ToContainTextAsync(TestText.Isolated(HostileFixtures.TrainerName));
        await Expect(page.Locator("label[for=ot-friendship]")).ToContainTextAsync(TestText.Isolated(HostileFixtures.TrainerName));
        // The nickname box edits the stored name, so it shows it as stored, override and all.
        await Expect(page.Locator("#nickname")).ToHaveValueAsync(HostileFixtures.Nickname);

        await Expect(page.Locator(InjectedElements)).ToHaveCountAsync(0);
        // No text node the app rendered holds a bidirectional control outside an isolate it added.
        var stray = await page.EvaluateAsync<int>("""
            () => {
                const controls = /[\u061C\u200E\u200F\u202A-\u202E\u2066-\u2069]/;
                const walker = document.createTreeWalker(document.querySelector('main'), NodeFilter.SHOW_TEXT);
                let count = 0;
                for (let node = walker.nextNode(); node; node = walker.nextNode()) {
                    const inner = node.data.replace(/\u2068[^\u2068\u2069]*\u2069/g, '');
                    if (controls.test(inner) || /\u2068[^\u2069]*[\u061C\u200E\u200F\u202A-\u202E\u2066-\u2068][^\u2069]*\u2069/.test(node.data)) {
                        count++;
                    }
                }
                return count;
            }
            """);
        Assert.True(stray == 0, $"{stray} text nodes hold a bidirectional control outside an isolate.");
        Assert.Empty(session.Dialogs);
        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task TheShellRunsNoInlineScriptAndThePolicyBlocksOne(string engine, string prefix)
    {
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        await Load(page, SaveFixtures.Synthetic(false));
        await Select(page);

        var inline = await page.EvaluateAsync<int>("() => document.querySelectorAll('script:not([src])').length");
        Assert.True(inline == 0, "The page has inline script elements.");
        var handlers = await page.EvaluateAsync<int>("() => [...document.querySelectorAll('*')].filter(e => [...e.attributes].some(a => a.name.startsWith('on'))).length");
        Assert.True(handlers == 0, "The page has inline event handler attributes.");
        await session.AssertNoCspViolationsAsync();

        // An injected inline script and an inline handler are both refused by the policy, and reported.
        var ran = await page.EvaluateAsync<bool>("""
            () => {
                const script = document.createElement('script');
                script.textContent = 'window.__pkhexInlineRan = true;';
                document.head.appendChild(script);
                const target = document.createElement('div');
                target.setAttribute('onclick', 'window.__pkhexInlineRan = true;');
                document.body.appendChild(target);
                target.click();
                target.remove();
                script.remove();
                return window.__pkhexInlineRan === true;
            }
            """);
        Assert.False(ran, "Inline script ran despite the Content Security Policy.");
        var violations = await session.TakeCspViolationsAsync();
        Assert.Contains(violations, d => d.StartsWith("script-src", StringComparison.Ordinal));

        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(PublishedAppFixture.BrowserCases), MemberType = typeof(PublishedAppFixture))]
    public async Task ADiagnosticReportNamesARefusedFileByCodeOnly(string engine, string prefix)
    {
        const string secretName = "ZZSECRET-main";
        await using var session = await app.BootAsync(engine, prefix);
        var page = session.Page;
        await Load(page, new byte[512], secretName);
        await Expect(page.Locator("#message")).ToContainTextAsync("not recognised");

        await page.Locator("#about-toggle").ClickAsync();
        await Expect(page.Locator("#about-diag-preview")).ToHaveCountAsync(0);
        await page.Locator("#about-diag-prepare").ClickAsync();
        var preview = page.Locator("#about-diag-preview");
        await Expect(preview).ToBeFocusedAsync();
        var commit = await page.Locator("#about-commit").TextContentAsync();
        var report = await preview.TextContentAsync() ?? "";
        Assert.StartsWith(DiagnosticReport.Title + "\n", report);
        Assert.Contains($"Source commit: {commit}\n", report);
        Assert.Contains("Browser: Mozilla/5.0", report);
        Assert.Contains("Open save: none\n", report);
        Assert.Contains("Open: open.unrecognized\n", report);
        Assert.DoesNotContain("ZZSECRET", report);

        // The download is the preview, byte for byte, as UTF-8 text.
        var download = await page.RunAndWaitForDownloadAsync(() => page.Locator("#about-diag-download").ClickAsync());
        Assert.Equal(DiagnosticText.FileName, download.SuggestedFilename);
        var path = await download.PathAsync();
        Assert.Equal(report, Encoding.UTF8.GetString(await File.ReadAllBytesAsync(path!)));
        await Expect(page.Locator("#about-diag-status")).ToHaveTextAsync(DiagnosticText.Downloaded);

        // Copying either succeeds or leaves the preview selected to copy by hand; engines differ in clipboard access.
        await page.Locator("#about-diag-copy").ClickAsync();
        await Expect(page.Locator("#about-diag-status")).ToHaveTextAsync(new System.Text.RegularExpressions.Regex(
            $"^({System.Text.RegularExpressions.Regex.Escape(DiagnosticText.Copied)}|{System.Text.RegularExpressions.Regex.Escape(DiagnosticText.CopyRefused)})$"));
        if (await page.Locator("#about-diag-status").TextContentAsync() == DiagnosticText.CopyRefused)
        {
            // Chromium leaves the final line end out of the selection's text.
            var selected = await page.EvaluateAsync<string>("() => window.getSelection().toString()");
            Assert.Equal(report.TrimEnd('\n'), selected.TrimEnd('\n'));
        }

        await page.Locator("#about-diag-close").ClickAsync();
        await Expect(page.Locator("#about-diag-prepare")).ToBeFocusedAsync();
        await session.AssertNoNetworkOrPersistenceAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }
}

using Microsoft.Playwright;
using Xunit;
using static Microsoft.Playwright.Assertions;
using static PKHeX.Web.Tests.ProofPage;

namespace PKHeX.Web.Tests;

/// <summary>
/// Startup failures in the published app (WEB-APP-001/004): a browser without a required feature is told so before the runtime loads,
/// and a failed asset load ends in a retry screen, not a stuck loading message.
/// </summary>
/// <remarks>
/// Startup happens before any file can be chosen, so the root path is enough; the <c>/PKHeX/</c> path is covered by every other boot.
/// </remarks>
[Collection(PublishedAppCollection.Name)]
[Trait(TestCategory.Name, TestCategory.E2E)]
public sealed class ShellTests(PublishedAppFixture app)
{
    /// <summary>A browser without WebAssembly at all.</summary>
    private const string RemoveWebAssembly = "delete globalThis.WebAssembly;";

    /// <summary>A browser whose WebAssembly lacks every optional feature the runtime needs.</summary>
    private const string RejectEveryModule = "WebAssembly.validate = () => false;";

    /// <summary>
    /// Rejects only the probe modules containing <paramref name="opcode"/>: 253 (0xFD) is the SIMD prefix and 25 (0x19) is <c>catch_all</c>,
    /// and neither appears in the other probe. A swapped or wrong probe then reports the wrong feature.
    /// </summary>
    private static string RejectModulesContaining(int opcode) => $$"""
        const validate = WebAssembly.validate;
        WebAssembly.validate = bytes => Array.prototype.indexOf.call(bytes, {{opcode}}) >= 0 ? false : validate.call(WebAssembly, bytes);
        """;

    /// <remarks>The missing <c>BigInt64Array</c> case cannot be simulated: Playwright's own page scripts need it.</remarks>
    public static TheoryData<string, string, string> UnsupportedCases()
    {
        var data = new TheoryData<string, string, string>();
        foreach (var engine in PublishedAppFixture.Engines)
        {
            data.Add(engine, RemoveWebAssembly, "WebAssembly");
            data.Add(engine, RejectEveryModule, "WebAssembly SIMD, WebAssembly exception handling");
            data.Add(engine, RejectModulesContaining(253), "WebAssembly SIMD");
            data.Add(engine, RejectModulesContaining(25), "WebAssembly exception handling");
            data.Add(engine, "delete URL.createObjectURL;", "file downloads");
        }
        return data;
    }

    /// <summary>Records in the page whether the failure screen was ever unhidden, since a later successful start hides it again.</summary>
    private const string RecordFailureScreen = """
        window.__pkhexFailureShown = false;
        new MutationObserver(() => {
            const failed = document.getElementById('boot-failed');
            if (failed && !failed.hidden) {
                window.__pkhexFailureShown = true;
            }
        }).observe(document, { subtree: true, childList: true, attributes: true, attributeFilter: ['hidden'] });
        """;

    public static TheoryData<string> EngineCases() => [.. PublishedAppFixture.Engines];

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(UnsupportedCases))]
    public async Task UnsupportedBrowserIsToldWithoutStartingTheRuntime(string engine, string disableFeatures, string missing)
    {
        await using var session = await app.CreateSessionAsync(engine, "");
        var page = session.Page;
        await page.AddInitScriptAsync(disableFeatures);
        await page.GotoAsync(session.AppUrl);

        await Expect(page.Locator("#boot-unsupported")).ToBeVisibleAsync();
        await Expect(page.Locator("#boot-missing")).ToHaveTextAsync(missing);
        await Expect(page.Locator("#app")).ToBeHiddenAsync();
        await Expect(page.Locator("#boot-failed")).ToBeHiddenAsync();
        await Expect(page.Locator("#blazor-error-ui")).ToBeHiddenAsync();

        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        var (requests, _, _) = session.TakeRecorded();
        Assert.DoesNotContain(requests, r => new Uri(r.Url).AbsolutePath.StartsWith("/_framework/dotnet", StringComparison.Ordinal));
        await session.AssertNoCspViolationsAsync();
        Assert.True(session.PageErrors == 0, "Browser runtime errors occurred.");
    }

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(EngineCases))]
    public async Task FailedAssetLoadOffersRetryThatBoots(string engine)
    {
        await using var session = await app.CreateSessionAsync(engine, "");
        var page = session.Page;
        const string runtime = "**/_framework/*.wasm";
        await page.RouteAsync(runtime, route => route.AbortAsync());
        await page.GotoAsync(session.AppUrl);

        await Expect(page.Locator("#boot-failed")).ToBeVisibleAsync(new() { Timeout = 60000 });
        await Expect(page.Locator("#app")).ToBeHiddenAsync();
        await Expect(page.Locator("#blazor-error-ui")).ToBeHiddenAsync();
        await Expect(page.Locator("#boot-retry")).ToBeFocusedAsync();

        // Once the files are reachable again, Try again boots the app; the retry is checked like any other boot.
        await page.UnrouteAsync(runtime);
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        session.TakeRecorded();
        await page.Locator("#boot-retry").ClickAsync();
        await Expect(page.Locator("#save-file")).ToBeVisibleAsync(new() { Timeout = 60000 });
        await app.AssertStaticBootAsync(session);
        await session.AssertNoNetworkOrPersistenceAsync();
    }

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(EngineCases))]
    public async Task UnrelatedErrorDuringSlowBootIsNotAFailure(string engine)
    {
        // An error that is not about the app's files (say, from a browser extension) while the runtime is still downloading.
        await using var session = await app.CreateSessionAsync(engine, "");
        var page = session.Page;
        await page.AddInitScriptAsync(RecordFailureScreen);
        await page.AddInitScriptAsync("setTimeout(() => Promise.reject(new Error('unrelated')), 300);");
        await page.RouteAsync("**/_framework/*.wasm", async route =>
        {
            await Task.Delay(5000);
            await route.ContinueAsync();
        });
        await page.GotoAsync(session.AppUrl);

        await Expect(page.Locator("#save-file")).ToBeVisibleAsync(new() { Timeout = 60000 });
        Assert.False(await page.EvaluateAsync<bool>("() => window.__pkhexFailureShown"), "A slow but healthy start showed the failure screen.");
    }

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(EngineCases))]
    public async Task StalledDownloadOffersRetryWhileStillLoading(string engine)
    {
        // A download that never answers raises no error, so only the slow-loading hint offers a way out.
        await using var session = await app.CreateSessionAsync(engine, "");
        var page = session.Page;
        await page.RouteAsync("**/_framework/PKHeX.Core.*.wasm", _ => Task.CompletedTask);
        await page.GotoAsync(session.AppUrl);

        await Expect(page.Locator("#boot-slow")).ToBeVisibleAsync(new() { Timeout = 45000 });
        await Expect(page.Locator("#app")).ToBeVisibleAsync();
        await Expect(page.Locator("#boot-failed")).ToBeHiddenAsync();
        await Expect(page.Locator("#boot-slow-retry")).ToBeEnabledAsync();
    }

    [TierTheory(TestCategory.E2E)]
    [MemberData(nameof(EngineCases))]
    public async Task LeaveWarningFollowsUnsavedWork(string engine)
    {
        // The shell, not the workspace, arms the warning, so it must track edits made inside the workspace.
        await using var session = await app.BootAsync(engine, "");
        var page = session.Page;
        await Load(page, SaveFixtures.Synthetic(false));
        await Expect(page.Locator("#overview-game")).ToHaveTextAsync("X");
        await page.ReloadAsync();
        await Expect(page.Locator("#save-file")).ToBeVisibleAsync(new() { Timeout = 60000 });
        Assert.Empty(session.Dialogs);

        await Load(page, SaveFixtures.Synthetic(false));
        await Select(page);
        await page.Locator("#nickname").FillAsync("Unsaved");
        await Expect(page.Locator("#draft-state")).ToHaveTextAsync("Unapplied draft");
        await page.ReloadAsync();
        await Expect(page.Locator("#save-file")).ToBeVisibleAsync(new() { Timeout = 60000 });
        Assert.Equal(["beforeunload"], session.Dialogs);

        // An applied change with a clean draft is still only in memory.
        await Load(page, SaveFixtures.Synthetic(false));
        await Select(page);
        await page.Locator("#nickname").FillAsync("Applied");
        await page.Locator("#apply").ClickAsync();
        await Expect(page.Locator("#draft-state")).ToHaveTextAsync("No draft changes");
        await Expect(page.Locator("#session-state")).ToHaveTextAsync("Edited in memory");
        await page.ReloadAsync();
        await Expect(page.Locator("#save-file")).ToBeVisibleAsync(new() { Timeout = 60000 });
        Assert.Equal(["beforeunload", "beforeunload"], session.Dialogs);
    }
}

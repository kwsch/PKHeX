using System.Text;
using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using PKHeX.Web.Components;
using PKHeX.Web.Services;
using PKHeX.Web.Services.Diagnostics;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The opt-in diagnostic report panel (WEB-SEC-005), rendered with bUnit: nothing is gathered until asked, the preview is the whole report, and
/// copy and download hand over exactly what is shown.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class DiagnosticPanelTests : IAsyncLifetime
{
    private const string UserAgent = "Mozilla/5.0 (Test) Browser/1.0";

    private readonly BunitContext context = new();
    private readonly WorkspaceState state = SaveFixtures.NewState();
    private readonly DiagnosticLog log = DiagnosticFixtures.NewLog();
    private readonly BunitJSModuleInterop browser;

    public DiagnosticPanelTests()
    {
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        browser = context.JSInterop.SetupModule("./browser.js");
        browser.Mode = JSRuntimeMode.Loose;
        browser.Setup<string>("userAgent").SetResult(UserAgent);
        context.Services.AddSingleton(state);
        DiagnosticFixtures.AddDiagnostics(context.Services, log);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    /// <summary>The interop services are only asynchronously disposable.</summary>
    public async Task DisposeAsync() => await context.DisposeAsync();

    private IRenderedComponent<DiagnosticPanel> RenderPanel() => context.Render<DiagnosticPanel>(p => p.Add(c => c.Id, "diag"));

    /// <summary>Calls Blazor made to move focus (<c>ElementReference.FocusAsync</c>).</summary>
    private int FocusCalls() => context.JSInterop.Invocations.Count(i => i.Identifier == "Blazor._internal.domWrapper.focus");

    [Fact]
    public void NothingIsGatheredUntilTheUserAsks()
    {
        var panel = RenderPanel();

        panel.Find("#diag-title").TextContent.Should().Be(DiagnosticText.Title);
        panel.Markup.Should().Contain(DiagnosticText.Intro);
        panel.FindAll("#diag-preview").Should().BeEmpty();
        panel.FindAll("#diag-copy, #diag-download").Should().BeEmpty();
        browser.Invocations.Should().NotContain(i => i.Identifier == "userAgent", "the browser string is read only for a report");
    }

    [Fact]
    public void ThePreviewIsExactlyTheReport()
    {
        state.Open(SaveFixtures.Open(SaveFixtures.Synthetic(true), "ZZFILE-main"));
        log.Record(DiagnosticOperation.Open, DiagnosticCode.For(SaveLoadOutcome.Failed(LoadFailure.Unrecognized)));
        var panel = RenderPanel();

        panel.Find("#diag-prepare").Click();

        var expected = DiagnosticReport.Build(DiagnosticContext.ForBuild(UserAgent, "Pokémon Omega Ruby and Alpha Sapphire", DiagnosticFixtures.Start), log.Entries);
        panel.Find("#diag-preview").TextContent.Should().Be(expected);
        panel.Find("#diag-preview").GetAttribute("tabindex").Should().Be("0", "a long report can be scrolled from the keyboard");
        panel.Find("#diag-note").TextContent.Should().Be(DiagnosticText.PreviewNote);
        panel.Markup.Should().NotContain("ZZFILE");
        FocusCalls().Should().Be(1, "focus moves to the preview");
    }

    [Fact]
    public void CopyHandsOverThePreview()
    {
        var copy = browser.Setup<bool>("copyText", _ => true);
        copy.SetResult(true);
        var panel = RenderPanel();
        panel.Find("#diag-prepare").Click();

        panel.Find("#diag-copy").Click();

        copy.Invocations.Should().ContainSingle().Which.Arguments[0].Should().Be(panel.Find("#diag-preview").TextContent);
        panel.Find("#diag-status").TextContent.Should().Be(DiagnosticText.Copied);
        browser.Invocations.Should().NotContain(i => i.Identifier == "selectText");
    }

    [Fact]
    public void ARefusedCopySelectsThePreviewInstead()
    {
        browser.Setup<bool>("copyText", _ => true).SetResult(false);
        var select = browser.Setup<bool>("selectText", _ => true);
        select.SetResult(true);
        var panel = RenderPanel();
        panel.Find("#diag-prepare").Click();

        panel.Find("#diag-copy").Click();

        select.Invocations.Should().ContainSingle().Which.Arguments[0].Should().Be("diag-preview");
        panel.Find("#diag-status").TextContent.Should().Be(DiagnosticText.CopyRefused);
    }

    [Fact]
    public async Task DownloadHandsOverThePreviewAsUtf8Text()
    {
        var download = browser.SetupVoid("download", _ => true);
        var panel = RenderPanel();
        panel.Find("#diag-prepare").Click();
        var shown = panel.Find("#diag-preview").TextContent;

        // Left pending while the bytes are read, since the stream is released once the download call returns.
        var click = panel.Find("#diag-download").ClickAsync(new());
        var invocation = download.Invocations.Should().ContainSingle().Subject;
        invocation.Arguments[1].Should().Be(DiagnosticText.FileName);
        using var bytes = new MemoryStream();
        await ((DotNetStreamReference)invocation.Arguments[0]!).Stream.CopyToAsync(bytes);
        Encoding.UTF8.GetString(bytes.ToArray()).Should().Be(shown);
        download.SetVoidResult();
        await click;

        panel.Find("#diag-status").TextContent.Should().Be(DiagnosticText.Downloaded);
        state.Session.Should().BeNull("a report download is not a save download");
    }

    [Fact]
    public void ClearForgetsTheRecordedProblemsAndCloseReturnsFocus()
    {
        log.Record(DiagnosticOperation.Apply, new InvalidOperationException("x"));
        var panel = RenderPanel();
        panel.Find("#diag-prepare").Click();
        panel.Find("#diag-preview").TextContent.Should().Contain("Apply: unexpected");

        panel.Find("#diag-clear").Click();
        log.Entries.Should().BeEmpty();
        panel.Find("#diag-preview").TextContent.Should().Contain(DiagnosticReport.NoEntries).And.NotContain("Apply: unexpected");
        panel.Find("#diag-status").TextContent.Should().Be(DiagnosticText.Cleared);

        panel.Find("#diag-close").Click();
        panel.FindAll("#diag-preview").Should().BeEmpty();
        panel.FindAll("#diag-prepare").Should().ContainSingle();
        FocusCalls().Should().Be(2, "focus moves to the preview, then back to the prepare button");
    }
}

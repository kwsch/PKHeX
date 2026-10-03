using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using PKHeX.Web.Components;
using PKHeX.Web.Interop;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// How the workspace wires legality to the browser: a run waits for a real paint (<c>browser.js</c> <c>nextPaint</c>) before Core's
/// synchronous analysis, so the pending state is on screen first, and the wiring is undone when the workspace goes.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class WorkspaceLegalityTests : IAsyncLifetime
{
    private readonly BunitContext context = new();
    private readonly WorkspaceState state = SaveFixtures.NewState();
    private readonly BunitJSModuleInterop browser;

    public WorkspaceLegalityTests()
    {
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        browser = context.JSInterop.SetupModule("./browser.js");
        browser.Mode = JSRuntimeMode.Loose;
        context.Services.AddSingleton(state);
        context.Services.AddSingleton<TimeProvider>(new FakeTimeProvider());
        DiagnosticFixtures.AddDiagnostics(context.Services, DiagnosticFixtures.NewLog());
        SpriteFixtures.AddCatalog(context);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    /// <summary>The interop services are only asynchronously disposable.</summary>
    public async Task DisposeAsync() => await context.DisposeAsync();

    [Fact]
    public async Task ARunWaitsForTheBrowsersNextPaint()
    {
        context.Render<Workspace>();
        state.Open(SaveFixtures.Open(SaveFixtures.Synthetic(true)));
        state.OpenSlot(SaveFixtures.FirstBoxSlot);

        await state.Legality.RunNowAsync();

        browser.Invocations.Should().Contain(i => i.Identifier == "nextPaint", "the run waits for a paint before calling Core");
        state.Legality.Status.Should().Be(LegalityStatus.Valid);
    }

    [Fact]
    public async Task TheWorkspaceRendersTheResultWhenARunCompletes()
    {
        var workspace = context.Render<Workspace>();
        state.Open(SaveFixtures.Open(SaveFixtures.Synthetic(true)));
        state.OpenSlot(SaveFixtures.FirstBoxSlot);
        workspace.Render();
        workspace.Find("#legality-status").TextContent.Should().Be("Pending");

        await state.Legality.RunNowAsync();

        workspace.WaitForAssertion(() => workspace.Find("#legality-status").TextContent.Should().Be("Valid"));
    }

    [Fact]
    public void DisposingTheWorkspaceUnhooksIt()
    {
        var workspace = context.Render<Workspace>();
        workspace.Instance.Dispose();
        var changed = () => state.Legality.Schedule();
        changed.Should().NotThrow("a disposed workspace must not be asked to re-render");
    }
}

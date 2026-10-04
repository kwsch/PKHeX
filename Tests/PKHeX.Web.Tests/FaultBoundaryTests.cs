using Bunit;
using Bunit.TestDoubles;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using PKHeX.Web.Components;
using PKHeX.Web.Services;
using PKHeX.Web.Services.Diagnostics;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The workspace fault boundary, rendered with bUnit around a child that fails on demand.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class FaultBoundaryTests : IAsyncLifetime
{
    /// <summary>Carried by the injected exception; it must never reach the page.</summary>
    private const string SecretDetail = "secret-exception-detail";

    private readonly BunitContext context = new();
    private readonly WorkspaceState state = SaveFixtures.NewState();
    private readonly DiagnosticLog diagnostics;
    private readonly RecordingLogger<DiagnosticLog> console;

    public FaultBoundaryTests()
    {
        diagnostics = DiagnosticFixtures.NewLog(out console);
        context.Services.AddSingleton(state);
        DiagnosticFixtures.AddDiagnostics(context.Services, diagnostics);
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        context.JSInterop.SetupModule("./browser.js").Mode = JSRuntimeMode.Loose;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    /// <summary>The diagnostic panel's interop services are only asynchronously disposable.</summary>
    public async Task DisposeAsync() => await context.DisposeAsync();

    private IRenderedComponent<FaultBoundary> RenderBoundary() =>
        context.Render<FaultBoundary>(p => p.Add(b => b.ChildContent, (RenderFragment)(b => { b.OpenComponent<Failing>(0); b.CloseComponent(); })));

    private static void Fail(IRenderedComponent<FaultBoundary> boundary) => boundary.Find("#fail").Click();

    /// <summary>Calls Blazor made to move focus (<c>ElementReference.FocusAsync</c>).</summary>
    private int FocusCalls() => context.JSInterop.Invocations.Count(i => i.Identifier == "Blazor._internal.domWrapper.focus");

    [Fact]
    public void FaultShowsRecoveryWithoutExceptionDetails()
    {
        var boundary = RenderBoundary();
        boundary.FindAll("#child").Should().ContainSingle();

        Fail(boundary);
        boundary.FindAll("#child").Should().BeEmpty();
        boundary.Markup.Should().NotContain(SecretDetail).And.NotContain(nameof(InvalidOperationException));
        FocusCalls().Should().Be(1, "focus moves to the recovery screen's heading");
    }

    [Fact]
    public void WithoutASessionOnlyStartOverAndReloadAreOffered()
    {
        var boundary = RenderBoundary();
        Fail(boundary);
        boundary.FindAll("#fault-return").Should().BeEmpty();
        boundary.Find("#fault-discard").TextContent.Should().Be("Start over");
        boundary.FindAll("#fault-reload").Should().ContainSingle();

        boundary.Find("#fault-discard").Click();
        boundary.FindAll("#child").Should().ContainSingle();
    }

    [Fact]
    public void ReturnKeepsTheSessionAndDropsTheDraft()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false));
        state.Open(session);
        var draft = session.Select(SaveFixtures.FirstBoxSlot);
        draft.EditNickname("Unapplied", true);
        state.SetDraft(draft);

        var boundary = RenderBoundary();
        Fail(boundary);
        boundary.Find("#fault-return").Click();

        boundary.FindAll("#fault").Should().BeEmpty();
        boundary.FindAll("#child").Should().ContainSingle("the workspace is rendered again");
        FocusCalls().Should().Be(2, "focus moves back into the workspace instead of dropping to the page body");
        state.Session.Should().BeSameAs(session);
        state.Draft.Should().BeNull();

        // The boundary can catch the next fault too.
        Fail(boundary);
        boundary.FindAll("#fault").Should().ContainSingle();
    }

    [Fact]
    public void DiscardClosesTheSession()
    {
        state.Open(SaveFixtures.Open(SaveFixtures.Synthetic(false)));
        var boundary = RenderBoundary();
        Fail(boundary);
        boundary.Find("#fault-discard").Click();

        state.Session.Should().BeNull();
        boundary.FindAll("#child").Should().ContainSingle();
    }

    [Fact]
    public void ReloadForcesAFullPageLoad()
    {
        var boundary = RenderBoundary();
        Fail(boundary);
        boundary.Find("#fault-reload").Click();

        var navigation = context.Services.GetRequiredService<BunitNavigationManager>();
        navigation.History.Should().ContainSingle().Which.Options.ForceLoad.Should().BeTrue();
    }

    [Fact]
    public void TheFaultIsRecordedRedactedAndTheReportIsOffered()
    {
        var boundary = RenderBoundary();
        Fail(boundary);

        var entry = diagnostics.Entries.Should().ContainSingle().Subject;
        entry.Operation.Should().Be(DiagnosticOperation.Render);
        entry.Code.Name.Should().Be("unexpected");
        entry.Code.ExceptionTypes.Should().Equal(typeof(InvalidOperationException).FullName);
        console.Messages.Should().ContainSingle().Which.Should().Contain("unexpected").And.NotContain(SecretDetail);
        console.Exceptions.Should().BeEmpty("the exception, with its message, is never handed to the console logger");

        boundary.FindAll("#fault-diag-preview").Should().BeEmpty("nothing is gathered until the user asks");
        boundary.Find("#fault-diag-prepare").Click();
        var report = boundary.Find("#fault-diag-preview").TextContent;
        report.Should().Contain("Render: unexpected").And.Contain(typeof(InvalidOperationException).FullName!).And.NotContain(SecretDetail);
        boundary.Markup.Should().NotContain(SecretDetail);
    }

    /// <summary>A child that throws from its click handler, standing in for any failing workspace component.</summary>
    private sealed class Failing : ComponentBase
    {
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "p");
            builder.AddAttribute(1, "id", "child");
            builder.CloseElement();
            builder.OpenElement(2, "button");
            builder.AddAttribute(3, "id", "fail");
            builder.AddAttribute(4, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => throw new InvalidOperationException(SecretDetail)));
            builder.CloseElement();
        }
    }
}

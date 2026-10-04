using Bunit;
using FluentAssertions;
using PKHeX.Web.Components;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The legality acknowledgements beside Apply and Download: what each shows for each gate, and the flagged changes in order.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class AcknowledgementComponentTests : IDisposable
{
    private readonly BunitContext context = new();

    public void Dispose() => context.Dispose();

    private IRenderedComponent<ApplyAcknowledgement> RenderApply(LegalityGate gate, LegalityVerdict verdict = LegalityVerdict.Invalid, bool disabled = false)
        => context.Render<ApplyAcknowledgement>(p => p
            .Add(c => c.Id, "apply-ack")
            .Add(c => c.Gate, gate)
            .Add(c => c.Verdict, verdict)
            .Add(c => c.Disabled, disabled));

    [Fact]
    public void ApplyShowsAWaitingNoteOrACheckboxAndNothingWhenClear()
    {
        RenderApply(LegalityGate.Clear).Find(".apply-gate").ChildElementCount.Should().Be(0);

        var waiting = RenderApply(LegalityGate.Waiting);
        waiting.Find("#apply-ack-waiting").TextContent.Should().Be("Apply is available once legality has analysed the draft as it is now.");
        waiting.FindAll("input").Should().BeEmpty();

        var needed = RenderApply(LegalityGate.NeedsAcknowledgement);
        needed.Find("#apply-ack").HasAttribute("checked").Should().BeFalse();
        needed.Find("label[for=apply-ack]").TextContent.Trim().Should().Be("Legality reports this draft as Invalid. Apply it anyway; nothing is repaired.");

        var given = RenderApply(LegalityGate.Acknowledged, LegalityVerdict.Unavailable, disabled: true);
        given.Find("#apply-ack").HasAttribute("checked").Should().BeTrue();
        given.Find("#apply-ack").HasAttribute("disabled").Should().BeTrue();
        given.Find("label[for=apply-ack]").TextContent.Trim().Should().Be("Legality could not analyse this draft. Apply it anyway, without a result.");
    }

    [Fact]
    public void AValidResultHasNoAcknowledgementText()
    {
        var text = () => LegalityText.ApplyAcknowledgement(LegalityVerdict.Valid);
        text.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ExportListsFlaggedChangesPartyFirstThenByBoxAndSlot()
    {
        var flagged = new Dictionary<SlotRef, LegalityVerdict>
        {
            [SlotRef.InBox(2, 0)] = LegalityVerdict.Invalid,
            [SlotRef.InBox(0, 5)] = LegalityVerdict.Unavailable,
            [SlotRef.InParty(3)] = LegalityVerdict.Invalid,
            [SlotRef.InBox(0, 1)] = LegalityVerdict.Invalid,
        };
        bool? acknowledged = null;
        var rendered = context.Render<ExportAcknowledgement>(p => p
            .Add(c => c.Id, "export-ack")
            .Add(c => c.Flagged, flagged)
            .Add(c => c.OnAcknowledge, (bool value) => acknowledged = value));

        rendered.Find("p").TextContent.Should().Be(SessionStatusText.FlaggedIntro(4));
        rendered.FindAll("#export-ack-list li").Select(li => li.TextContent).Should().Equal(
            SessionStatusText.Flagged(SlotRef.InParty(3), LegalityVerdict.Invalid),
            SessionStatusText.Flagged(SlotRef.InBox(0, 1), LegalityVerdict.Invalid),
            SessionStatusText.Flagged(SlotRef.InBox(0, 5), LegalityVerdict.Unavailable),
            SessionStatusText.Flagged(SlotRef.InBox(2, 0), LegalityVerdict.Invalid));
        rendered.Find("#export-ack").HasAttribute("checked").Should().BeFalse();
        rendered.Find("#export-ack").Change(true);
        acknowledged.Should().BeTrue();
    }

    [Fact]
    public void ExportRendersNothingWithoutFlaggedChanges()
    {
        var rendered = context.Render<ExportAcknowledgement>(p => p
            .Add(c => c.Id, "export-ack")
            .Add(c => c.Flagged, new Dictionary<SlotRef, LegalityVerdict>()));
        rendered.Markup.Trim().Should().BeEmpty();
    }
}

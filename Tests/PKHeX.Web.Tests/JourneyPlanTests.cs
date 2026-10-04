using Bunit;
using Microsoft.AspNetCore.Components;
using PKHeX.Core;
using PKHeX.Web.Components;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The published journey's plan (<see cref="JourneyPlan"/>): it reaches every fieldset of the editor, names only controls the editor renders,
/// and its native output holds both edits and changes nothing else.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class JourneyPlanTests : IDisposable
{
    private readonly BunitContext context = new();

    public void Dispose() => context.Dispose();

    /// <summary>The journey save of <see cref="JourneyBrowserTests"/>'s shape: the known entity in box 1, slot 1 and a party member.</summary>
    private static byte[] Save(bool oras) => SaveFixtures.Synthetic(oras, customize: SaveFixtures.WithPartyMember());

    private IRenderedComponent<DraftEditor> Render(EditorDraft draft) => context.Render<DraftEditor>(p => p
        .Add(c => c.Draft, draft)
        .Add(c => c.OnEdit, EventCallback.Factory.Create<DraftEdit>(this, _ => { })));

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void TheJourneyEditsEveryFieldsetOfTheEditor(bool oras)
    {
        var bytes = Save(oras);
        var plan = JourneyPlan.For(SaveFixtures.Parse(bytes));
        var fieldsets = Render(SaveFixtures.Open(bytes).Select(plan.Box)).FindAll("fieldset[id]").Select(f => f.Id!).Order().ToList();

        Assert.Equal(9, fieldsets.Count);
        Assert.Equal(fieldsets, plan.Steps.Select(s => EditorFields.FieldsetOf(s.ControlId)!).Distinct().Order().ToList());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void EveryStepNamesAControlTheEditorRenders(bool oras)
    {
        var bytes = Save(oras);
        var plan = JourneyPlan.For(SaveFixtures.Parse(bytes));
        foreach (var (slot, steps) in new[] { (plan.Box, plan.BoxSteps), (plan.Party, plan.PartySteps) })
        {
            var editor = Render(SaveFixtures.Open(bytes).Select(slot));
            foreach (var step in steps)
            {
                Assert.Single(editor.FindAll($"#{step.ControlId}"));
                if (step.Action == JourneyAction.Search)
                {
                    Assert.Single(editor.FindAll($"#{step.ControlId}-search"));
                }
            }
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void EveryStepChangesTheValueItsControlShows(bool oras)
    {
        // A step that sets the value already shown edits nothing, and native Core's recipe would be a no-op too, so the journey would pass.
        var bytes = Save(oras);
        var plan = JourneyPlan.For(SaveFixtures.Parse(bytes));
        foreach (var (slot, steps) in new[] { (plan.Box, plan.BoxSteps), (plan.Party, plan.PartySteps) })
        {
            var editor = Render(SaveFixtures.Open(bytes).Select(slot));
            foreach (var step in steps)
            {
                var control = editor.Find($"#{step.ControlId}");
                if (step.Action == JourneyAction.Ticked)
                {
                    // Ticked by typing the name, so it must start unticked here.
                    Assert.False(control.HasAttribute("checked"), $"{step.ControlId} is already ticked.");
                }
                else
                {
                    Assert.NotEqual(step.Value, control.GetAttribute("value"));
                }
            }
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void NativeOutputHoldsBothEditsAndChangesNothingElse(bool oras)
    {
        var bytes = Save(oras);
        var native = SaveFixtures.Parse(bytes);
        var plan = JourneyPlan.For(native);

        plan.AssertReopened(SaveFixtures.Parse(plan.ExpectedEdited));
        plan.AssertOnlyTheEditedSlotsDiffer(plan.ExpectedEdited);
        Assert.Equal(native.Write().ToArray(), plan.NoOp);

        // Both slots really change, and the party member keeps its status and is never healed.
        var box = SaveFixtures.Slot(native, plan.Box).Read(native);
        var member = native.GetPartySlotAtIndex(plan.Party.Slot);
        Assert.NotEqual(box.Species, plan.ExpectedBox.Species);
        Assert.Equal(plan.Nickname, plan.ExpectedBox.Nickname);
        Assert.NotEqual(member.CurrentLevel, plan.ExpectedParty.CurrentLevel);
        Assert.Equal(member.Status_Condition, plan.ExpectedParty.Status_Condition);
        Assert.Equal(Math.Min(member.Stat_HPCurrent, plan.ExpectedParty.Stat_HPMax), plan.ExpectedParty.Stat_HPCurrent);
    }

    [Fact]
    public void ASaveWithNoEditablePartyMemberIsRefusedWithoutItsValues()
    {
        var native = SaveFixtures.Parse(SaveFixtures.Synthetic(false));
        var error = Assert.Throws<InvalidOperationException>(() => JourneyPlan.For(native));
        Assert.StartsWith("The save has no party member", error.Message);
    }
}

using Bunit;
using FluentAssertions;
using PKHeX.Web.Components;
using PKHeX.Web.Services;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The overview panel renders every value, as text only.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class SaveOverviewPanelTests : IDisposable
{
    private readonly BunitContext context = new();

    public void Dispose() => context.Dispose();

    [Fact]
    public void EveryValueIsRenderedUnderItsId()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false, customize: SaveOverviewTests.SetKnownTrainer), "main");
        var overview = SaveOverview.From(session);
        var panel = context.Render<SaveOverviewPanel>(p => p.Add(c => c.Session, session));

        var expected = new Dictionary<string, string>
        {
            ["overview-game"] = OverviewText.Game(overview),
            ["overview-family"] = OverviewText.Family(overview),
            ["overview-trainer"] = OverviewText.Trainer(overview),
            ["overview-language"] = OverviewText.Language(overview),
            ["overview-tid"] = OverviewText.TrainerId(overview),
            ["overview-sid"] = OverviewText.SecretId(overview),
            ["overview-playtime"] = OverviewText.PlayTime(overview),
            ["overview-money"] = OverviewText.Money(overview),
            ["overview-last-saved"] = OverviewText.LastSaved(overview),
            ["overview-file"] = "main",
            ["overview-size"] = OverviewText.Size(overview),
            ["overview-format"] = OverviewText.Format(overview),
            ["overview-integrity"] = OverviewText.Integrity,
        };
        panel.FindAll("dd").Select(d => d.Id).Should().Equal(expected.Keys);
        foreach (var (id, text) in expected)
        {
            panel.Find($"#{id}").TextContent.Should().Be(text);
        }
        panel.Find("section").GetAttribute("aria-labelledby").Should().Be("overview-title");
    }

    [Fact]
    public void HostileTrainerNameIsRenderedAsText()
    {
        const string hostile = "<b>x</b>&lt;"; // 12 characters, the Gen 6 limit
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false, customize: s => s.OT = hostile));
        var panel = context.Render<SaveOverviewPanel>(p => p.Add(c => c.Session, session));

        panel.Find("#overview-trainer").TextContent.Should().Be(hostile);
        panel.FindAll("#overview-trainer b").Should().BeEmpty();
    }
}

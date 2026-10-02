using System.Globalization;
using Bunit;
using FluentAssertions;
using PKHeX.Core;
using PKHeX.Web.Components;
using PKHeX.Web.Services;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The inspector's text: fixed sections, invariant numbers, and unknown or out-of-game values labelled rather than hidden (WEB-PKM-001).
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class InspectorTextTests : IDisposable
{
    private static readonly SaveCapabilities Capabilities = SaveFixtures.Open(SaveFixtures.Synthetic(false)).Capabilities;
    private readonly BunitContext context = new();

    public void Dispose() => context.Dispose();

    private static PK6 Known() => new(SaveFixtures.ReadEntity(true));

    private static IReadOnlyList<InspectorSection> Sections(PK6 pk, SlotRef? slot = null) =>
        InspectorText.Sections(EntityInspection.From(pk, slot ?? SaveFixtures.FirstBoxSlot, Capabilities));

    private static string Value(IReadOnlyList<InspectorSection> sections, string id) =>
        sections.SelectMany(s => s.Rows).Single(r => r.Id == id).Value;

    [Fact]
    public void SectionsAreFixedAndIdsAreUnique()
    {
        var sections = Sections(Known());

        sections.Select(s => s.Title).Should().Equal("Identity", "Stats", "Moves", "Origin and trainer", "Advanced");
        var ids = sections.Select(s => s.Id)
            .Concat(sections.SelectMany(s => s.Rows).Select(r => r.Id))
            .Concat(sections.Select(s => s.Table?.Id).OfType<string>())
            .ToList();
        ids.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void KnownValuesUseCoreNames()
    {
        var pk = Known();
        var sections = Sections(pk);

        Value(sections, "inspect-species").Should().Be("Zigzagoon (No. 263)");
        Value(sections, "inspect-nature").Should().Be(GameInfo.Strings.natures[(int)pk.Nature]);
        Value(sections, "inspect-ability").Should().StartWith(GameInfo.Strings.abilitylist[pk.Ability]);
        Value(sections, "inspect-form").Should().Be("No alternate forms");
        Value(sections, "inspect-format").Should().Be("PK6");
        Value(sections, "inspect-pid").Should().Be("0x" + pk.PID.ToString("X8", CultureInfo.InvariantCulture));
        Value(sections, "inspect-checksum").Should().Be("Valid");
    }

    [Fact]
    public void NumbersIgnoreTheBrowserCulture()
    {
        var pk = Known();
        pk.EXP = 1_059_860;
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            Value(Sections(pk), "inspect-exp").Should().Be("1,059,860");
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public void UnknownAndOutOfGameValuesAreLabelled()
    {
        var pk = Known();
        pk.HeldItem = 9999;
        pk.AbilityNumber = 3;
        pk.Move2 = (ushort)(Capabilities.Lists.Moves.Max(m => m.Value) + 1);
        pk.Move4 = 0;

        var sections = Sections(pk);

        Value(sections, "inspect-item").Should().Be("Unknown (stored value 9999)");
        Value(sections, "inspect-ability").Should().EndWith("(unknown slot, stored value 3)");
        var moves = sections.Single(s => s.Id == "inspect-moves").Table!.Rows;
        moves[1][1].Should().EndWith("(not available in this game)");
        moves[3][1].Should().Be("Empty");
        moves[3][2].Should().Be("–");
    }

    [Theory]
    [InlineData(0, 0, "Never infected")]
    [InlineData(3, 0, "Cured (strain 3)")]
    [InlineData(3, 2, "Infected (strain 3, 2 days left)")]
    public void PokerusStatesAreToldApart(int strain, int days, string expected) => InspectorText.Pokerus(strain, days).Should().Be(expected);

    [Fact]
    public void PartyMembersShowStoredHpAndStatus()
    {
        var pk = Known();
        pk.ResetPartyStats();
        pk.Stat_HPCurrent = 0;

        var sections = Sections(pk, SlotRef.InParty(0));

        Value(sections, "inspect-hp").Should().Be($"0 of {pk.Stat_HPMax}");
        Value(sections, "inspect-status").Should().Be("None");
        pk.Status_Condition = 1;
        Value(Sections(pk, SlotRef.InParty(0)), "inspect-status").Should().Be("Paralysis");
        pk.Status_Condition = 9;
        Value(Sections(pk, SlotRef.InParty(0)), "inspect-status").Should().Be("Unknown (stored value 9)");
        sections.Single(s => s.Id == "inspect-stats").Table!.Caption.Should().Contain("stored");
        Sections(Known()).SelectMany(s => s.Rows).Should().NotContain(r => r.Id == "inspect-hp", "boxed Pokémon store no HP");
    }

    [Fact]
    public void AnEggsFriendshipFieldIsShownAsItsHatchCounter()
    {
        var pk = Known();
        pk.IsEgg = true;
        pk.OriginalTrainerFriendship = 20;

        var sections = Sections(pk);
        var rows = sections.SelectMany(s => s.Rows).ToDictionary(r => r.Id);

        rows["inspect-friendship"].Label.Should().Be("Hatch counter (egg cycles left)");
        rows["inspect-friendship"].Value.Should().Be("20", "a hatch counter is not out of 255");
        rows["inspect-ot-friendship"].Label.Should().Contain("hatch counter");
        Sections(Known()).SelectMany(s => s.Rows).Single(r => r.Id == "inspect-friendship").Label.Should().Be("Friendship (current trainer)");
    }

    [Fact]
    public void ComponentRendersEverySectionWithHeadingsAndStoredTextAsText()
    {
        var pk = Known();
        pk.Nickname = "<b>x</b>";
        pk.OriginalTrainerName = "<i>OT</i>";
        var inspection = EntityInspection.From(pk, SaveFixtures.FirstBoxSlot, Capabilities);

        var rendered = context.Render<EntityInspector>(p => p.Add(c => c.Inspection, inspection));

        rendered.FindAll("section h3").Select(h => h.TextContent).Should().Equal("Identity", "Stats", "Moves", "Origin and trainer", "Advanced");
        rendered.FindAll("section").Select(s => s.GetAttribute("aria-labelledby"))
            .Should().Equal("inspect-identity", "inspect-stats", "inspect-moves", "inspect-origin", "inspect-advanced");
        rendered.Find("#inspect-nickname").TextContent.Should().Be("<b>x</b>");
        rendered.FindAll("#inspect-nickname b").Should().BeEmpty("a stored name is never parsed as markup");
        rendered.Find("#inspect-ot").TextContent.Should().StartWith("<i>OT</i>");
        rendered.FindAll("#inspect-stats-table tbody tr").Should().HaveCount(6);
        rendered.FindAll("#inspect-stats-table tbody th[scope=row]").Select(t => t.TextContent).Should().Equal(InspectorText.StatNames);
        rendered.FindAll("#inspect-moves-table tbody tr").Should().HaveCount(4);
    }

    [Fact]
    public void ComponentFollowsANewInspection()
    {
        var pk = Known();
        var rendered = context.Render<EntityInspector>(p => p.Add(c => c.Inspection, EntityInspection.From(pk, SaveFixtures.FirstBoxSlot, Capabilities)));
        pk.Nickname = "Renamed";

        rendered.Render(p => p.Add(c => c.Inspection, EntityInspection.From(pk, SaveFixtures.FirstBoxSlot, Capabilities)));

        rendered.Find("#inspect-nickname").TextContent.Should().Be("Renamed");
    }
}

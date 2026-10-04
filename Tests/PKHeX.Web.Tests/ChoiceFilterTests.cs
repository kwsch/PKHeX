using FluentAssertions;
using PKHeX.Core;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The searchable move and item boxes narrow Core's lists by name, ignoring case, accents and punctuation, and never drop the drafted value
/// or "(None)", so filtering can never change a choice.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class ChoiceFilterTests
{
    private static readonly IReadOnlyList<ComboItem> Small =
    [
        new("(None)", 0), new("Poké Ball", 4), new("King's Rock", 221), new("Pokédex", 999), new("Leftovers", 234), new("Pretty Feather", 571),
        new("Pretty Feather", 571),
    ];

    private static IReadOnlyList<ComboItem> Lists(bool items) =>
        items ? SaveFixtures.Open(SaveFixtures.Synthetic(false)).Capabilities.Lists.Items : SaveFixtures.Open(SaveFixtures.Synthetic(false)).Capabilities.Lists.Moves;

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("-'")]
    public void ASearchWithNoLetterOrDigitKeepsTheListItself(string? query)
    {
        var result = ChoiceFilter.Filter(Small, query, 4);
        result.Items.Should().BeSameAs(Small, "the box must not rebuild its options");
        result.Matches.Should().Be(5).And.Be(result.Total, "(None) and the repeated Pretty Feather are not counted");
    }

    [Theory]
    [InlineData("poke", new[] { 0, 4, 999 })]
    [InlineData("POKÉ", new[] { 0, 4, 999 })]
    [InlineData("kings rock", new[] { 0, 221 })]
    [InlineData("k-i-n-g", new[] { 0, 221 })]
    [InlineData("over", new[] { 0, 234 })]
    public void NamesContainingTheSearchMatchIgnoringCaseAccentsAndPunctuation(string query, int[] values)
    {
        ChoiceFilter.Filter(Small, query, 0).Items.Select(i => i.Value).Should().Equal(values);
    }

    [Fact]
    public void TheDraftedValueAndNoneAreAlwaysKeptInOrder()
    {
        var result = ChoiceFilter.Filter(Small, "pretty", 234);
        result.Items.Select(i => i.Value).Should().Equal(0, 234, 571, 571);
        result.Matches.Should().Be(1, "the drafted Leftovers is kept, not matched, and Pretty Feather counts once");
        result.Total.Should().Be(5);
    }

    [Fact]
    public void NoMatchLeavesOnlyTheDraftedValueAndNone()
    {
        var result = ChoiceFilter.Filter(Small, "zzz", 221);
        result.Items.Select(i => i.Value).Should().Equal(0, 221);
        result.Matches.Should().Be(0);
    }

    [Fact]
    public void ADraftedValueOutsideTheListAddsNothing()
    {
        ChoiceFilter.Filter(Small, "leftovers", 32767).Items.Select(i => i.Value).Should().Equal(0, 234);
    }

    [Fact]
    public void FoldingDropsAccentsCasePunctuationAndSpaces()
    {
        ChoiceFilter.Fold("Poké Ball").Should().Be("POKEBALL");
        ChoiceFilter.Fold("U-turn").Should().Be("UTURN");
        ChoiceFilter.Fold("King's Rock").Should().Be("KINGSROCK");
        ChoiceFilter.Fold("Flabébé").Should().Be("FLABEBE");
        ChoiceFilter.Fold("Ｐｏｋé").Should().Be("ＰＯＫE", "full-width letters are letters; only the accent goes");
        ChoiceFilter.Fold(null).Should().BeEmpty();
    }

    [Fact]
    public void CoresMoveListIsSearchedByName()
    {
        var moves = Lists(false);
        var thunder = ChoiceFilter.Filter(moves, "thun", 0);
        thunder.Items.Select(i => i.Text).Should().Equal("(None)", "Thunder", "Thunder Fang", "Thunder Punch", "Thunder Shock", "Thunder Wave", "Thunderbolt");
        thunder.Matches.Should().Be(6);
        thunder.Total.Should().Be(moves.Count - 1);
        ChoiceFilter.Filter(moves, "uturn", 0).Items.Select(i => i.Text).Should().Contain("U-turn");
    }

    [Fact]
    public void CoresItemListIsSearchedByNameWithAccents()
    {
        var items = Lists(true);
        var poke = ChoiceFilter.Filter(items, "poke", 0).Items.Select(i => i.Text).ToList();
        poke.Should().Contain("Poké Ball").And.Contain("Poké Doll");
        poke.Should().OnlyContain(t => t == "(None)" || ChoiceFilter.Fold(t).Contains("POKE"));
    }

    [Fact]
    public void TheSameListIsFoldedOnce()
    {
        // The cache is keyed on the list instance: a second search reuses the folded names, so a filtered result is consistent across calls.
        var first = ChoiceFilter.Filter(Small, "pok", 0);
        var second = ChoiceFilter.Filter(Small, "pok", 0);
        second.Items.Should().Equal(first.Items);
        second.Total.Should().Be(first.Total);
    }
}

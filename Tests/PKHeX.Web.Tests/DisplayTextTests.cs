using FluentAssertions;
using PKHeX.Web.Components;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// Display of stored names: bidirectional controls are removed, and an embedded name is isolated so it cannot reorder its
/// neighbours.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class DisplayTextTests
{
    /// <summary>Every Unicode Bidi_Control character.</summary>
    public static TheoryData<char> BidiControls { get; } =
        ['\u061C', '\u200E', '\u200F', '\u202A', '\u202B', '\u202C', '\u202D', '\u202E', '\u2066', '\u2067', '\u2068', '\u2069'];

    [Theory]
    [MemberData(nameof(BidiControls))]
    public void EveryBidiControlIsRemoved(char control)
    {
        DisplayText.IsBidiControl(control).Should().BeTrue();
        DisplayText.Plain($"a{control}b{control}").Should().Be("ab");
        DisplayText.Embed($"{control}ab").Should().Be(TestText.Isolated("ab"));
    }

    [Theory]
    [InlineData("Ziggy")]
    [InlineData("<b>x</b>")]
    [InlineData("שלום")]
    [InlineData("A‍B")]
    [InlineData("😀")]
    public void OtherTextIsKept(string name)
    {
        DisplayText.Plain(name).Should().BeSameAs(name, "a name without controls is returned as it is");
        DisplayText.Embed(name).Should().Be(TestText.Isolated(name));
    }

    [Fact]
    public void AnOverrideCannotEscapeTheIsolate()
    {
        // A stray pop-isolate in the name would close the app's isolate early; it is removed with the override.
        var embedded = DisplayText.Embed("\u202Eevil\u2069tail");
        embedded.Should().Be(TestText.Isolated("eviltail"));
        embedded[1..^1].Any(DisplayText.IsBidiControl).Should().BeFalse();
    }

    [Fact]
    public void EmbeddingIsStableAndNullStaysNull()
    {
        var once = DisplayText.Embed("\u202Ename");
        DisplayText.Embed(once).Should().Be(once);
        DisplayText.Plain(null).Should().BeNull();
        DisplayText.Plain("").Should().BeEmpty();
        DisplayText.Embed("").Should().Be(TestText.Isolated(""));
    }

    [Fact]
    public void OtherCharactersAreNotBidiControls()
    {
        Enumerable.Range(0, 0x10000).Select(i => (char)i).Count(DisplayText.IsBidiControl).Should().Be(BidiControls.Count);
    }
}

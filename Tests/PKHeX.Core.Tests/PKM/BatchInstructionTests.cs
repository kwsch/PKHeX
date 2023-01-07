using FluentAssertions;
using Xunit;
namespace PKHeX.Core.Tests;

public class BatchInstructionTests
{
    private const string TestInstructions = """
        =Species=4
        .Form=0
        .Nickname=Hello
        """;

    [Theory]
    [InlineData(TestInstructions)]
    public void ParseCount(string lines)
    {
        var len = StringInstructionSet.GetInstructionSetLength(lines);
        len.Should().Be(lines.Length);
        lines += "\n;\n.Species=0";
        var extra = StringInstructionSet.GetInstructionSetLength(lines);
        (len + 1).Should().Be(extra);
    }

    [Theory]
    [InlineData('=')]
    [InlineData('!')]
    [InlineData('>')]
    [InlineData('<')]
    [InlineData('≥')]
    [InlineData('≤')]
    [InlineData('f', false)]
    public void ParseComparer(char c, bool expect = true)
    {
        var comparer = StringInstruction.GetComparer(c);
        comparer.IsSupportedComparer().Should().Be(expect);
    }
}

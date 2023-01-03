using System;
using FluentAssertions;
using PKHeX.Core;
using Xunit;
namespace PKHeX.Tests;

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
        var text = lines.AsSpan();
        var len = StringInstructionSet.GetInstructionSetLength(text);
        len.Should().Be(lines.Length);
        lines += "\n;\n.Species=0";
        var extra = StringInstructionSet.GetInstructionSetLength(lines);
        (len + 1).Should().Be(extra);
    }
}

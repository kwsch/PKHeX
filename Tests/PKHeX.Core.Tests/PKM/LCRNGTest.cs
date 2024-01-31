using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests.PKM;

public class LCRNGTest
{
    [Theory]
    [InlineData(0x12345u, 0xAEA0DF8C, 12345u)]
    [InlineData(0xBADC0DED, 0xBADC0DED, 0u)]
    [InlineData(0, 0x0A3561A1, uint.MaxValue)]
    [InlineData(0x0A3561A1, 0, 1u)]
    public void FindFrame(uint start, uint end, uint expect)
    {
        var distance = LCRNG.GetDistance(start, end);
        distance.Should().Be(expect);
    }

    [Theory]
    [InlineData(8675309, 0x75C29428, 8675309)]
    [InlineData(0xBADC0DED, 0xBADC0DED, 0u)]
    [InlineData(0, 0xA170F641, uint.MaxValue)]
    [InlineData(0xA170F641, 0, 1u)]
    public void FindFrameXDRNG(uint start, uint end, uint expect)
    {
        var distance = XDRNG.GetDistance(start, end);
        distance.Should().Be(expect);
    }
}

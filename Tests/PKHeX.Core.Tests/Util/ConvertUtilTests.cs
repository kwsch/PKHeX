using FluentAssertions;
using Xunit;

namespace PKHeX.Tests.Util
{
    public class ConvertUtilTests
    {
        [Theory]
        [InlineData("-050 ", -50)]
        [InlineData("123 45678", 12345678)]
        [InlineData("--8765-43 21", -87654321)]
        public void CheckConvertValidI32(string v, int result)
        {
            var convert = Core.Util.ToInt32(v);
            convert.Should().Be(result);
        }

        [Theory]
        [InlineData("50", 50)]
        [InlineData("12 345678", 12345678)]
        [InlineData("87654 321", 87654321)]
        public void CheckConvertValidU32(string v, uint result)
        {
            var convert = Core.Util.ToUInt32(v);
            convert.Should().Be(result);
        }

        [Theory]
        [InlineData("0x50", 0x50)]
        [InlineData("0x12 345678", 0x12345678)]
        [InlineData("8aF5z4 32-1", 0x8aF54321)]
        public void CheckConvertValidHexU32(string v, uint result)
        {
            var convert = Core.Util.GetHexValue(v);
            convert.Should().Be(result);
        }
    }
}

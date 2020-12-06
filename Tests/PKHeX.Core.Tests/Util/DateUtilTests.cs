using FluentAssertions;
using Xunit;

namespace PKHeX.Tests.Util
{
    public class DateUtilTests
    {
        [Theory]
        [InlineData(2000, 1, 1)]
        [InlineData(2001, 1, 31)]
        public void RecognizesCorrectDates(int year, int month, int day)
        {
            Assert.True(Core.Util.IsDateValid(year, month, day), $"Failed to recognize {year}/{month}/{day}");
        }

        [Theory]
        [InlineData(2016, 1, 31)]
        [InlineData(2016, 2, 28)]
        [InlineData(2016, 3, 31)]
        [InlineData(2016, 4, 30)]
        [InlineData(2016, 5, 31)]
        [InlineData(2016, 6, 30)]
        [InlineData(2016, 7, 31)]
        [InlineData(2016, 8, 31)]
        [InlineData(2016, 9, 30)]
        [InlineData(2016, 10, 31)]
        [InlineData(2016, 11, 30)]
        [InlineData(2016, 12, 31)]
        public void RecognizesValidMonthBoundaries(int year, int month, int day)
        {
            Assert.True(Core.Util.IsDateValid(year, month, day), $"Incorrect month boundary for {year}/{month}/{day}");
        }

        [Fact]
        public void RecognizeCorrectLeapYear()
        {
            Assert.True(Core.Util.IsDateValid(2004, 2, 29));
        }

        [Theory]
        [InlineData(0, 0, 0, false, "Zero date")]
        [InlineData(2005, 2, 29, false, "Bad leap year")]
        [InlineData(0, 1, 1, false, "Zero year")]
        [InlineData(2000, 0, 1, false, "Zero month")]
        [InlineData(2000, 1, 0, false, "Zero day")]
        [InlineData(10000, 1, 0, false, "Big year")]
        [InlineData(2000, 13, 0, false, "Big month")]
        [InlineData(2000, 1, 32, false, "Big day")]
        [InlineData(2019, 11, 31, false, "Bad date, November doesn't have a 31st")]
        [InlineData(uint.MaxValue, uint.MaxValue, uint.MaxValue, false, "Failed with uint.MaxValue, negative")]
        public void CheckDate(uint year, uint month, uint day, bool cmp, string because)
        {
            var result = Core.Util.IsDateValid(year, month, day);
            result.Should().Be(cmp, because);
        }
    }
}

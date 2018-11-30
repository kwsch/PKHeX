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

        [Fact]
        public void FailsWithIncorrectLeapYear()
        {
            Assert.False(Core.Util.IsDateValid(2005, 2, 29));
        }

        [Fact]
        public void FailsWithZeroDate()
        {
            Assert.False(Core.Util.IsDateValid(0, 0, 0));
        }

        [Fact]
        public void FailsWithNegativeDate()
        {
            Assert.False(Core.Util.IsDateValid(-1, -1, -1));
        }

        [Fact]
        public void FailsWithBigDay()
        {
            Assert.False(Core.Util.IsDateValid(2000, 1, 32));
        }

        [Fact]
        public void FailsWithBigMonth()
        {
            Assert.False(Core.Util.IsDateValid(2000, 13, 1));
        }

        [Fact]
        public void FailsWithBigYear()
        {
            Assert.False(Core.Util.IsDateValid(10000, 1, 1));
        }

        [Fact]
        public void FailsWithZeroDay()
        {
            Assert.False(Core.Util.IsDateValid(2000, 1, 0));
        }

        [Fact]
        public void FailsWithZeroMonth()
        {
            Assert.False(Core.Util.IsDateValid(2000, 0, 1));
        }

        [Fact]
        public void FailsWithZeroYear()
        {
            Assert.False(Core.Util.IsDateValid(0, 1, 1));
        }

        [Fact]
        public void FailsWithMaxUInt()
        {
            Assert.False(Core.Util.IsDateValid(uint.MaxValue, uint.MaxValue, uint.MaxValue), "Failed with uint.MaxValue");
        }
    }
}

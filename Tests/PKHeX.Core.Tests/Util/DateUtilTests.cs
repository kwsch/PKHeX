using Xunit;

namespace PKHeX.Tests.Util
{
    public class DateUtilTests
    {
        [Fact]
        public void RecognizesCorrectDates()
        {
            Assert.True(Core.Util.IsDateValid(2000, 1, 1), "Failed to recognize 1/1/2000");
            Assert.True(Core.Util.IsDateValid(2001, 1, 31), "Failed to recognize 1/31/2001");
        }

        [Fact]
        public void MonthBoundaries()
        {
            Assert.True(Core.Util.IsDateValid(2016, 1, 31), "Incorrect month boundary for January");
            Assert.True(Core.Util.IsDateValid(2016, 2, 28), "Incorrect month boundary for February");
            Assert.True(Core.Util.IsDateValid(2016, 3, 31), "Incorrect month boundary for March");
            Assert.True(Core.Util.IsDateValid(2016, 4, 30), "Incorrect month boundary for April");
            Assert.True(Core.Util.IsDateValid(2016, 5, 31), "Incorrect month boundary for May");
            Assert.True(Core.Util.IsDateValid(2016, 6, 30), "Incorrect month boundary for June");
            Assert.True(Core.Util.IsDateValid(2016, 7, 31), "Incorrect month boundary for July");
            Assert.True(Core.Util.IsDateValid(2016, 8, 31), "Incorrect month boundary for August");
            Assert.True(Core.Util.IsDateValid(2016, 9, 30), "Incorrect month boundary for September");
            Assert.True(Core.Util.IsDateValid(2016, 10, 31), "Incorrect month boundary for October");
            Assert.True(Core.Util.IsDateValid(2016, 11, 30), "Incorrect month boundary for November");
            Assert.True(Core.Util.IsDateValid(2016, 12, 31), "Incorrect month boundary for December");
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
        public void TestUIntOverload()
        {
            Assert.True(Core.Util.IsDateValid((uint)2000, (uint)1, (uint)1), "Failed 1/1/2000");
            Assert.False(Core.Util.IsDateValid(uint.MaxValue, uint.MaxValue, uint.MaxValue), "Failed with uint.MaxValue");
        }
    }
}

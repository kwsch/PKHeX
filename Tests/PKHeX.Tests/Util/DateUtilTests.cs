using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PKHeX.Tests.Util
{
    [TestClass]
    public class DateUtilTests
    {
        const string DateUtilCategory = "Date Util Tests";

        [TestMethod]
        [TestCategory(DateUtilCategory)]
        public void RecognizesCorrectDates()
        {
            Assert.IsTrue(Core.Util.IsDateValid(2000, 1, 1), "Failed to recognize 1/1/2000");
            Assert.IsTrue(Core.Util.IsDateValid(2001, 1, 31), "Failed to recognize 1/31/2001");            
        }

        [TestMethod]
        [TestCategory(DateUtilCategory)]
        public void MonthBoundaries()
        {
            Assert.IsTrue(Core.Util.IsDateValid(2016, 1, 31), "Incorrect month boundary for January");
            Assert.IsTrue(Core.Util.IsDateValid(2016, 2, 28), "Incorrect month boundary for February");
            Assert.IsTrue(Core.Util.IsDateValid(2016, 3, 31), "Incorrect month boundary for March");
            Assert.IsTrue(Core.Util.IsDateValid(2016, 4, 30), "Incorrect month boundary for April");
            Assert.IsTrue(Core.Util.IsDateValid(2016, 5, 31), "Incorrect month boundary for May");
            Assert.IsTrue(Core.Util.IsDateValid(2016, 6, 30), "Incorrect month boundary for June");
            Assert.IsTrue(Core.Util.IsDateValid(2016, 7, 31), "Incorrect month boundary for July");
            Assert.IsTrue(Core.Util.IsDateValid(2016, 8, 31), "Incorrect month boundary for August");
            Assert.IsTrue(Core.Util.IsDateValid(2016, 9, 30), "Incorrect month boundary for September");
            Assert.IsTrue(Core.Util.IsDateValid(2016, 10, 31), "Incorrect month boundary for October");
            Assert.IsTrue(Core.Util.IsDateValid(2016, 11, 30), "Incorrect month boundary for November");
            Assert.IsTrue(Core.Util.IsDateValid(2016, 12, 31), "Incorrect month boundary for December");
        }

        [TestMethod]
        [TestCategory(DateUtilCategory)]
        public void RecognizeCorrectLeapYear()
        {
            Assert.IsTrue(Core.Util.IsDateValid(2004, 2, 29));
        }

        [TestMethod]
        [TestCategory(DateUtilCategory)]
        public void FailsWithIncorrectLeapYear()
        {
            Assert.IsFalse(Core.Util.IsDateValid(2005, 2, 29));
        }

        [TestMethod]
        [TestCategory(DateUtilCategory)]
        public void FailsWithZeroDate()
        {
            Assert.IsFalse(Core.Util.IsDateValid(0, 0, 0));
        }

        [TestMethod]
        [TestCategory(DateUtilCategory)]
        public void FailsWithNegativeDate()
        {
            Assert.IsFalse(Core.Util.IsDateValid(-1, -1, -1));
        }

        [TestMethod]
        [TestCategory(DateUtilCategory)]
        public void FailsWithBigDay()
        {
            Assert.IsFalse(Core.Util.IsDateValid(2000, 1, 32));
        }

        [TestMethod]
        [TestCategory(DateUtilCategory)]
        public void FailsWithBigMonth()
        {
            Assert.IsFalse(Core.Util.IsDateValid(2000, 13, 1));
        }

        [TestMethod]
        [TestCategory(DateUtilCategory)]
        public void FailsWithBigYear()
        {
            Assert.IsFalse(Core.Util.IsDateValid(10000, 1, 1));
        }

        [TestMethod]
        [TestCategory(DateUtilCategory)]
        public void FailsWithZeroDay()
        {
            Assert.IsFalse(Core.Util.IsDateValid(2000, 1, 0));
        }

        [TestMethod]
        [TestCategory(DateUtilCategory)]
        public void FailsWithZeroMonth()
        {
            Assert.IsFalse(Core.Util.IsDateValid(2000, 0, 1));
        }

        [TestMethod]
        [TestCategory(DateUtilCategory)]
        public void FailsWithZeroYear()
        {
            Assert.IsFalse(Core.Util.IsDateValid(0, 1, 1));
        }

        [TestMethod]
        [TestCategory(DateUtilCategory)]
        public void TestUIntOverload()
        {
            Assert.IsTrue(Core.Util.IsDateValid((uint)2000, (uint)1, (uint)1), "Failed 1/1/2000");
            Assert.IsFalse(Core.Util.IsDateValid(uint.MaxValue, uint.MaxValue, uint.MaxValue), "Failed with uint.MaxValue");
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Assert.IsTrue(PKHeX.Util.IsDateValid(2000, 1, 1), "Failed to recognize 1/1/2000");
            Assert.IsTrue(PKHeX.Util.IsDateValid(2001, 1, 31), "Failed to recognize 1/31/2001");            
        }

        [TestMethod]
        [TestCategory(DateUtilCategory)]
        public void MonthBoundaries()
        {
            Assert.IsTrue(PKHeX.Util.IsDateValid(2016, 1, 31), "Incorrect month boundary for January");
            Assert.IsTrue(PKHeX.Util.IsDateValid(2016, 2, 28), "Incorrect month boundary for February");
            Assert.IsTrue(PKHeX.Util.IsDateValid(2016, 3, 31), "Incorrect month boundary for March");
            Assert.IsTrue(PKHeX.Util.IsDateValid(2016, 4, 30), "Incorrect month boundary for April");
            Assert.IsTrue(PKHeX.Util.IsDateValid(2016, 5, 31), "Incorrect month boundary for May");
            Assert.IsTrue(PKHeX.Util.IsDateValid(2016, 6, 30), "Incorrect month boundary for June");
            Assert.IsTrue(PKHeX.Util.IsDateValid(2016, 7, 31), "Incorrect month boundary for July");
            Assert.IsTrue(PKHeX.Util.IsDateValid(2016, 8, 31), "Incorrect month boundary for August");
            Assert.IsTrue(PKHeX.Util.IsDateValid(2016, 9, 30), "Incorrect month boundary for September");
            Assert.IsTrue(PKHeX.Util.IsDateValid(2016, 10, 31), "Incorrect month boundary for October");
            Assert.IsTrue(PKHeX.Util.IsDateValid(2016, 11, 30), "Incorrect month boundary for November");
            Assert.IsTrue(PKHeX.Util.IsDateValid(2016, 12, 31), "Incorrect month boundary for December");
        }

        [TestMethod]
        [TestCategory(DateUtilCategory)]
        public void RecognizeCorrectLeapYear()
        {
            Assert.IsTrue(PKHeX.Util.IsDateValid(2004, 2, 29));
        }

        [TestMethod]
        [TestCategory(DateUtilCategory)]
        public void FailsWithIncorrectLeapYear()
        {
            Assert.IsFalse(PKHeX.Util.IsDateValid(2005, 2, 29));
        }

        [TestMethod]
        [TestCategory(DateUtilCategory)]
        public void FailsWithZeroDate()
        {
            Assert.IsFalse(PKHeX.Util.IsDateValid(0, 0, 0));
        }

        [TestMethod]
        [TestCategory(DateUtilCategory)]
        public void FailsWithNegativeDate()
        {
            Assert.IsFalse(PKHeX.Util.IsDateValid(-1, -1, -1));
        }

        [TestMethod]
        [TestCategory(DateUtilCategory)]
        public void FailsWithBigDay()
        {
            Assert.IsFalse(PKHeX.Util.IsDateValid(2000, 1, 32));
        }

        [TestMethod]
        [TestCategory(DateUtilCategory)]
        public void FailsWithBigMonth()
        {
            Assert.IsFalse(PKHeX.Util.IsDateValid(2000, 13, 1));
        }

        [TestMethod]
        [TestCategory(DateUtilCategory)]
        public void FailsWithBigYear()
        {
            Assert.IsFalse(PKHeX.Util.IsDateValid(10000, 1, 1));
        }

        [TestMethod]
        [TestCategory(DateUtilCategory)]
        public void FailsWithZeroDay()
        {
            Assert.IsFalse(PKHeX.Util.IsDateValid(2000, 1, 0));
        }

        [TestMethod]
        [TestCategory(DateUtilCategory)]
        public void FailsWithZeroMonth()
        {
            Assert.IsFalse(PKHeX.Util.IsDateValid(2000, 0, 1));
        }

        [TestMethod]
        [TestCategory(DateUtilCategory)]
        public void TestUIntOverload()
        {
            Assert.IsTrue(PKHeX.Util.IsDateValid((uint)2000, (uint)1, (uint)1), "Failed 1/1/2000");
            Assert.IsFalse(PKHeX.Util.IsDateValid(uint.MaxValue, uint.MaxValue, uint.MaxValue), "Failed with uint.MaxValue");
        }
    }
}

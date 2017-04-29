using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using PKHeX.Core;

namespace PKHeX.Tests.PKM
{
    [TestClass]
    public class PKMTests
    {
        private const string DateTestCategory = "PKM Date Tests";
        private const string PIDIVTestCategory = "PKM PIDIV Matching Tests";

        [TestMethod]
        [TestCategory(DateTestCategory)]
        public void MetDateGetterTest()
        {
            var pk = new DateTestPKM();

            // Ensure MetDate is null when components are all 0
            pk.MetDay = 0;
            pk.MetMonth = 0;
            pk.MetYear = 0;
            Assert.IsFalse(pk.MetDate.HasValue, "MetDate should be null when date components are all 0.");           

            // Ensure MetDate gives correct date
            pk.MetDay = 10;
            pk.MetMonth = 8;
            pk.MetYear = 16;
            Assert.AreEqual(new DateTime(2016, 8, 10).Date, pk.MetDate.Value.Date, "Met date does not return correct date.");

            // Ensure 0 year is calculated correctly
            pk.MetDay = 1;
            pk.MetMonth = 1;
            pk.MetYear = 0;
            Assert.AreEqual(2000, pk.MetDate.Value.Date.Year, "Year is not calculated correctly.");
        }

        [TestMethod]
        [TestCategory(DateTestCategory)]
        public void MetDateSetterTest()
        {
            var pk = new DateTestPKM();

            // Ensure setting to null zeros the components
            // -- Set to something else first
            pk.MetDay = 12;
            pk.MetMonth = 12;
            pk.MetYear = 12;
            // -- Act
            pk.MetDate = null;
            // -- Assert
            Assert.AreEqual(0, pk.MetDay, "Met_Day was not zeroed when MetDate is set to null");
            Assert.AreEqual(0, pk.MetMonth, "Met_Month was not zeroed when MetDate is set to null");
            Assert.AreEqual(0, pk.MetYear, "Met_Year was not zeroed when MetDate is set to null");

            // Ensure setting to a date sets the components
            var now = DateTime.UtcNow;
            // -- Set to something else first
            pk.MetDay = 12;
            pk.MetMonth = 12;
            pk.MetYear = 12;
            if (now.Month == 12)
            {
                // We don't want the test to work just because it's 12/12 right now.
                pk.MetMonth = 11;
            }
            // -- Act
            pk.MetDate = now;
            // -- Assert
            Assert.AreEqual(now.Day, pk.MetDay, "Met_Day was not correctly set");
            Assert.AreEqual(now.Month, pk.MetMonth, "Met_Month was not correctly set");
            Assert.AreEqual(now.Year - 2000, pk.MetYear, "Met_Year was not correctly set");
        }

        [TestMethod]
        [TestCategory(DateTestCategory)]
        public void EggMetDateGetterTest()
        {
            var pk = new DateTestPKM();

            // Ensure MetDate is null when components are all 0
            pk.EggMetDay = 0;
            pk.EggMetMonth = 0;
            pk.EggMetYear = 0;
            Assert.IsFalse(pk.MetDate.HasValue, "EggMetDate should be null when date components are all 0.");

            // Ensure MetDate gives correct date
            pk.EggMetDay = 10;
            pk.EggMetMonth = 8;
            pk.EggMetYear = 16;
            Assert.AreEqual(new DateTime(2016, 8, 10).Date, pk.EggMetDate.Value.Date, "Egg met date does not return correct date.");
        }

        [TestMethod]
        [TestCategory(DateTestCategory)]
        public void EggMetDateSetterTest()
        {
            var pk = new DateTestPKM();

            // Ensure setting to null zeros the components
            // -- Set to something else first
            pk.EggMetDay = 12;
            pk.EggMetMonth = 12;
            pk.EggMetYear = 12;
            // -- Act
            pk.EggMetDate = null;
            // -- Assert
            Assert.AreEqual(0, pk.EggMetDay, "Egg_Day was not zeroed when EggMetDate is set to null");
            Assert.AreEqual(0, pk.EggMetMonth, "Egg_Month was not zeroed when EggMetDate is set to null");
            Assert.AreEqual(0, pk.EggMetYear, "Egg_Year was not zeroed when EggMetDate is set to null");

            // Ensure setting to a date sets the components
            var now = DateTime.UtcNow;
            // -- Set to something else first
            pk.EggMetDay = 12;
            pk.EggMetMonth = 12;
            pk.EggMetYear = 12;
            if (now.Month == 12)
            {
                // We don't want the test to work just because it's 12/12 right now.
                pk.EggMetMonth = 11;
            }
            // -- Act
            pk.EggMetDate = now;
            // -- Assert
            Assert.AreEqual(now.Day, pk.EggMetDay, "Egg_Day was not correctly set");
            Assert.AreEqual(now.Month, pk.EggMetMonth, "Egg_Month was not correctly set");
            Assert.AreEqual(now.Year - 2000, pk.EggMetYear, "Egg_Year was not correctly set");
        }

        [TestMethod]
        [TestCategory(PIDIVTestCategory)]
        public void PIDIVMatchingTest()
        {
            // IVs are stored HP/ATK/DEF/SPE/SPA/SPD
            var pk1 = new PK3
            {
                PID = 0xE97E0000,
                IVs = new[] {17, 19, 20, 16, 13, 12}
            };
            Assert.AreEqual(PIDType.Method_1, MethodFinder.Analyze(pk1)?.Type, "Unable to match PID to Method 1 spread");
            var pk2 = new PK3
            {
                PID = 0x5271E97E,
                IVs = new[] {02, 18, 03, 12, 22, 24}
            };
            Assert.AreEqual(PIDType.Method_2, MethodFinder.Analyze(pk2)?.Type, "Unable to match PID to Method 2 spread");
            var pk4 = new PK3
            {
                PID = 0x31B05271,
                IVs = new[] {02, 18, 03, 05, 30, 11}
            };
            Assert.AreEqual(PIDType.Method_4, MethodFinder.Analyze(pk4)?.Type, "Unable to match PID to Method 4 spread");

            var pk3 = new PK3
            {
                PID = 0x0985A297,
                IVs = new[] {06, 01, 00, 07, 17, 07}
            };
            Assert.AreEqual(PIDType.XDC, MethodFinder.Analyze(pk3)?.Type, "Unable to match PID to XDC spread");

            var pkC = new PK3
            {
                PID = 0x9E27D2F6,
                IVs = new[] {04, 15, 21, 14, 18, 29}
            };
            Assert.AreEqual(PIDType.Channel, MethodFinder.Analyze(pkC)?.Type, "Unable to match PID to Channel spread");

            var pkCC = new PK4
            {
                PID = 0x00000037,
                IVs = new[] {16, 13, 12, 02, 18, 03},
                Species = 1,
                Gender = 0,
            };
            Assert.AreEqual(PIDType.CuteCharm, MethodFinder.Analyze(pkCC)?.Type, "Unable to match PID to Cute Charm spread");

            var pkASR = new PK4
            {
                PID = 0x07578CB7, // 0x5271E97E rerolled
                IVs = new[] {16, 13, 12, 02, 18, 03},
            };
            Assert.AreEqual(PIDType.G4AntiShiny, MethodFinder.Analyze(pkASR)?.Type, "Unable to match PID to Antishiny4 spread");
        }
    }
}

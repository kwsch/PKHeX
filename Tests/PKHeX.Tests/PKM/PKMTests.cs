using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
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
            Assert.AreEqual(PIDType.CXD, MethodFinder.Analyze(pk3)?.Type, "Unable to match PID to CXD spread");

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
            Assert.AreEqual(PIDType.G4MGAntiShiny, MethodFinder.Analyze(pkASR)?.Type, "Unable to match PID to Antishiny4 spread");

            var pkCS = new PK4
            {
                PID = 0xA9C1A9C6,
                // TID = 0,
                // SID = 0, // already default values, necessary for the forcing of a shiny PID
                IVs = new[] {22, 14, 23, 24, 11, 04}
            };
            Assert.AreEqual(PIDType.ChainShiny, MethodFinder.Analyze(pkCS)?.Type, "Unable to match PID to Chain Shiny spread");

            var pkS5 = new PK5
            {
                PID = 0xBEEF0037,
                TID = 01337,
                SID = 48097,
                // IVs = new[] {22, 14, 23, 24, 11, 04} // unnecessary
            };
            Assert.AreEqual(PIDType.G5MGShiny, MethodFinder.Analyze(pkS5)?.Type, "Unable to match PID to PGF Shiny spread");

            var pkR = new PK3
            {
                PID = 0x0000E97E,
                // TID = 0,
                // SID = 0, // already default values, necessary for the forcing of a shiny PID
                IVs = new[] {17, 19, 20, 16, 13, 12}
            };
            Assert.AreEqual(PIDType.BACD_R, MethodFinder.Analyze(pkR)?.Type, "Unable to match PID to BACD-R spread");

            var pkRA = new PK3
            {
                PID = 0x0000E980, // +2 of 8 to flip first shiny bit
                TID = 01337,
                SID = 60486,
                IVs = new[] {17, 19, 20, 16, 13, 12}
            };
            Assert.AreEqual(PIDType.BACD_R_A, MethodFinder.Analyze(pkRA)?.Type, "Unable to match PID to BACD-R antishiny spread");

            var pkU = new PK3
            {
                PID = 0x67DBFC33,
                // TID = 0,
                // SID = 0, // already default values, necessary for the forcing of a shiny PID
                IVs = new[] {12, 25, 27, 30, 02, 31}
            };
            Assert.AreEqual(PIDType.BACD_U, MethodFinder.Analyze(pkU)?.Type, "Unable to match PID to BACD-U spread");

            var pkUA = new PK3
            {
                PID = 0x67DBFC38, // +5 of 8 to flip first shiny bit
                TID = 01337,
                SID = 40657,
                IVs = new[] {12, 25, 27, 30, 02, 31}
            };
            Assert.AreEqual(PIDType.BACD_U_A, MethodFinder.Analyze(pkUA)?.Type, "Unable to match PID to BACD-U antishiny spread");

            var pkRS = new PK3 // berry fix zigzagoon: seed 0x0020
            {
                PID = 0x38CA4EA0,
                TID = 30317,
                SID = 00000,
                IVs = new[] { 00, 20, 28, 11, 19, 00 }
            };
            var a_pkRS = MethodFinder.Analyze(pkRS);
            Assert.AreEqual(PIDType.BACD_R_S, a_pkRS?.Type, "Unable to match PID to BACD-R shiny spread");
            Assert.AreEqual(true, 0x0020 == a_pkRS?.OriginSeed, "Unable to match PID to BACD-R shiny spread origin seed");

            var pkPS0 = new PK3 {PID = 0x7B2D9DA7}; // Zubat (Cave)
            Assert.AreEqual(true, MethodFinder.getPokeSpotSeeds(pkPS0, 0).Any(), "PokeSpot encounter info mismatch (Common)");
            var pkPS1 = new PK3 {PID = 0x3EE9AF66}; // Gligar (Rock)
            Assert.AreEqual(true, MethodFinder.getPokeSpotSeeds(pkPS1, 1).Any(), "PokeSpot encounter info mismatch (Uncommon)");
            var pkPS2 = new PK3 {PID = 0x9B667F3C}; // Surskit (Oasis)
            Assert.AreEqual(true, MethodFinder.getPokeSpotSeeds(pkPS2, 2).Any(), "PokeSpot encounter info mismatch (Rare)");
            
            var pk1U = new PK3
            {
                Species = 201, // Unown-C
                PID = 0x815549A2,
                IVs = new[] {02, 26, 30, 30, 11, 26}
            };
            Assert.AreEqual(PIDType.Method_1_Unown, MethodFinder.Analyze(pk1U)?.Type, "Unable to match PID to Method 1 Unown spread");
            var pk2U = new PK3
            {
                Species = 201, // Unown-M
                PID = 0x8A7B5190,
                IVs = new[] {14, 02, 21, 30, 29, 15}
            };
            Assert.AreEqual(PIDType.Method_2_Unown, MethodFinder.Analyze(pk2U)?.Type, "Unable to match PID to Method 2 Unown spread");
            var pk4U = new PK3
            {
                Species = 201, // Unown-C
                PID = 0x5FA80D70,
                IVs = new[] {02, 06, 03, 26, 04, 19}
            };
            Assert.AreEqual(PIDType.Method_4_Unown, MethodFinder.Analyze(pk4U)?.Type, "Unable to match PID to Method 4 Unown spread");
        }
    }
}

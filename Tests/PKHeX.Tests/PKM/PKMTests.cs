using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using PKHeX.Core;

namespace PKHeX.Tests
{
    [TestClass]
    public class PKMTests
    {
        private const string StringTestCategory = "PKM String Tests";

        [TestMethod]
        [TestCategory(StringTestCategory)]
        public void StringEncodingTest()
        {
            const string name_fabian = "Fabian♂";
            var pkm = new PK7 { OT_Name = name_fabian };
            var byte_fabian = new byte[]
            {
                0x46, 0x00, // F
                0x61, 0x00, // a
                0x62, 0x00, // b
                0x69, 0x00, // i
                0x61, 0x00, // a
                0x6E, 0x00, // n
                0x8E, 0xE0, // ♂
                0x00, 0x00, // \0 terminator
            };
            CheckStringGetSet(nameof(pkm.OT_Name), name_fabian, pkm.OT_Name, byte_fabian, pkm.OT_Trash);

            const string name_nidoran = "ニドラン♀";
            pkm.Nickname = name_nidoran;
            var byte_nidoran = new byte[]
            {
                0xCB, 0x30, // ニ
                0xC9, 0x30, // ド
                0xE9, 0x30, // ラ
                0xF3, 0x30, // ン
                0x40, 0x26, // ♀
                0x00, 0x00, // \0 terminator
            };
            CheckStringGetSet(nameof(pkm.Nickname), name_nidoran, pkm.Nickname, byte_nidoran, pkm.Nickname_Trash);
        }

        private static void CheckStringGetSet(string check, string instr, string outstr, byte[] indata, byte[] outdata)
        {
            outdata = outdata.Take(indata.Length).ToArray();

            Assert.IsTrue(indata.SequenceEqual(outdata),
                $"{check} did not set properly."
                + Environment.NewLine + string.Join(", ", outdata.Select(z => $"{z:X2}")));

            Assert.AreEqual(instr, outstr, $"{check} did not get properly.");
        }

        private const string DateTestCategory = "PKM Date Tests";

        [TestMethod]
        [TestCategory(DateTestCategory)]
        public void MetDateGetterTest()
        {
            var pk = new PK7();

            // Ensure MetDate is null when components are all 0
            pk.Met_Day = 0;
            pk.Met_Month = 0;
            pk.Met_Year = 0;
            Assert.IsFalse(pk.MetDate.HasValue, "MetDate should be null when date components are all 0.");

            // Ensure MetDate gives correct date
            pk.Met_Day = 10;
            pk.Met_Month = 8;
            pk.Met_Year = 16;
            Assert.AreEqual(new DateTime(2016, 8, 10).Date, pk.MetDate.Value.Date, "Met date does not return correct date.");

            // Ensure 0 year is calculated correctly
            pk.Met_Day = 1;
            pk.Met_Month = 1;
            pk.Met_Year = 0;
            Assert.AreEqual(2000, pk.MetDate.Value.Date.Year, "Year is not calculated correctly.");
        }

        [TestMethod]
        [TestCategory(DateTestCategory)]
        public void MetDateSetterTest()
        {
            var pk = new PK7();

            // Ensure setting to null zeros the components
            // -- Set to something else first
            pk.Met_Day = 12;
            pk.Met_Month = 12;
            pk.Met_Year = 12;
            // -- Act
            pk.MetDate = null;
            // -- Assert
            Assert.AreEqual(0, pk.Met_Day, "Met_Day was not zeroed when MetDate is set to null");
            Assert.AreEqual(0, pk.Met_Month, "Met_Month was not zeroed when MetDate is set to null");
            Assert.AreEqual(0, pk.Met_Year, "Met_Year was not zeroed when MetDate is set to null");

            // Ensure setting to a date sets the components
            var now = DateTime.UtcNow;
            // -- Set to something else first
            pk.Met_Day = 12;
            pk.Met_Month = 12;
            pk.Met_Year = 12;
            if (now.Month == 12)
            {
                // We don't want the test to work just because it's 12/12 right now.
                pk.Met_Month = 11;
            }
            // -- Act
            pk.MetDate = now;
            // -- Assert
            Assert.AreEqual(now.Day, pk.Met_Day, "Met_Day was not correctly set");
            Assert.AreEqual(now.Month, pk.Met_Month, "Met_Month was not correctly set");
            Assert.AreEqual(now.Year - 2000, pk.Met_Year, "Met_Year was not correctly set");
        }

        [TestMethod]
        [TestCategory(DateTestCategory)]
        public void EggMetDateGetterTest()
        {
            var pk = new PK7();

            // Ensure MetDate is null when components are all 0
            pk.Egg_Day = 0;
            pk.Egg_Month = 0;
            pk.Egg_Year = 0;
            Assert.IsFalse(pk.MetDate.HasValue, "EggMetDate should be null when date components are all 0.");

            // Ensure MetDate gives correct date
            pk.Egg_Day = 10;
            pk.Egg_Month = 8;
            pk.Egg_Year = 16;
            Assert.AreEqual(new DateTime(2016, 8, 10).Date, pk.EggMetDate.Value.Date, "Egg met date does not return correct date.");
        }

        [TestMethod]
        [TestCategory(DateTestCategory)]
        public void EggMetDateSetterTest()
        {
            var pk = new PK7();

            // Ensure setting to null zeros the components
            // -- Set to something else first
            pk.Egg_Day = 12;
            pk.Egg_Month = 12;
            pk.Egg_Year = 12;
            // -- Act
            pk.EggMetDate = null;
            // -- Assert
            Assert.AreEqual(0, pk.Egg_Day, "Egg_Day was not zeroed when EggMetDate is set to null");
            Assert.AreEqual(0, pk.Egg_Month, "Egg_Month was not zeroed when EggMetDate is set to null");
            Assert.AreEqual(0, pk.Egg_Year, "Egg_Year was not zeroed when EggMetDate is set to null");

            // Ensure setting to a date sets the components
            var now = DateTime.UtcNow;
            // -- Set to something else first
            pk.Egg_Day = 12;
            pk.Egg_Month = 12;
            pk.Egg_Year = 12;
            if (now.Month == 12)
            {
                // We don't want the test to work just because it's 12/12 right now.
                pk.Egg_Month = 11;
            }
            // -- Act
            pk.EggMetDate = now;
            // -- Assert
            Assert.AreEqual(now.Day, pk.Egg_Day, "Egg_Day was not correctly set");
            Assert.AreEqual(now.Month, pk.Egg_Month, "Egg_Month was not correctly set");
            Assert.AreEqual(now.Year - 2000, pk.Egg_Year, "Egg_Year was not correctly set");
        }
    }
}

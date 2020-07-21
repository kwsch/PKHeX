using System;
using System.Linq;
using FluentAssertions;
using PKHeX.Core;
using Xunit;

namespace PKHeX.Tests.PKM
{
    public class StringTests
    {
        [Fact]
        public void EncodesOTNameCorrectly()
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
        }

        [Fact]
        public void EncodesNicknameCorrectly()
        {
            const string name_nidoran = "ニドラン♀";
            var pkm = new PK7 { Nickname = name_nidoran };
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

        private static void CheckStringGetSet(string check, string instr, string outstr, byte[] indata,
            byte[] outdata)
        {
            instr.Should().BeEquivalentTo(outstr);

            outdata = outdata.Take(indata.Length).ToArray();

            indata.SequenceEqual(outdata).Should()
                .BeTrue($"expected {check} to set properly, instead got {string.Join(", ", outdata.Select(z => $"{z:X2}"))}");
        }
    }

    public class MetDateTests
    {
        [Fact]
        public void MetDateNullWhenDateComponentsAreAllZero()
        {
            var pk = new PK7
            {
                Met_Day = 0,
                Met_Month = 0,
                Met_Year = 0
            };

            pk.MetDate.HasValue.Should().BeFalse();
        }

        [Fact]
        public void MetDateReturnsCorrectDate()
        {
            var pk = new PK7
            {
                Met_Day = 10,
                Met_Month = 8,
                Met_Year = 16
            };

            pk.MetDate.GetValueOrDefault().Should().Be(new DateTime(2016, 8, 10).Date);
        }

        [Fact]
        public void MetDateCalculatesYear0Correctly()
        {
            var pk = new PK7
            {
                Met_Day = 1,
                Met_Month = 1,
                Met_Year = 0
            };

            pk.MetDate.GetValueOrDefault().Date.Year.Should().Be(2000);
        }

        [Fact]
        public void SettingToNullZerosComponents()
        {
            var pk = new PK7
            {
                Met_Day = 12,
                Met_Month = 12,
                Met_Year = 12
            };

            pk.MetDate = null;

            pk.Met_Day.Should().Be(0);
            pk.Met_Month.Should().Be(0);
            pk.Met_Year.Should().Be(0);
        }

        [Fact]
        public void SettingMetDateSetsComponents()
        {
            var pk = new PK7
            {
                Met_Day = 12,
                Met_Month = 12,
                Met_Year = 12
            };

            pk.MetDate = new DateTime(2005, 5, 5);

            pk.Met_Day.Should().Be(5);
            pk.Met_Month.Should().Be(5);
            pk.Met_Year.Should().Be(5);
        }
    }

    public class EggMetDateTests
    {
        [Fact]
        public void EggMetDateNullWhenDateComponentsAreAllZero()
        {
            var pk = new PK7
            {
                Egg_Day = 0,
                Egg_Month = 0,
                Egg_Year = 0
            };

            pk.EggMetDate.HasValue.Should().BeFalse();
        }

        [Fact]
        public void EggMetDateReturnsCorrectDate()
        {
            var pk = new PK7
            {
                Egg_Day = 10,
                Egg_Month = 8,
                Egg_Year = 16
            };

            pk.EggMetDate.GetValueOrDefault().Should().Be(new DateTime(2016, 8, 10).Date);
        }

        [Fact]
        public void EggMetDateCalculatesYear0Correctly()
        {
            var pk = new PK7
            {
                Egg_Day = 1,
                Egg_Month = 1,
                Egg_Year = 0
            };

            pk.EggMetDate.GetValueOrDefault().Date.Year.Should().Be(2000);
        }

        [Fact]
        public void SettingEggMetDateToNullZerosComponents()
        {
            var pk = new PK7
            {
                Egg_Day = 12,
                Egg_Month = 12,
                Egg_Year = 12
            };

            pk.EggMetDate = null;

            pk.Egg_Day.Should().Be(0);
            pk.Egg_Month.Should().Be(0);
            pk.Egg_Year.Should().Be(0);
        }

        [Fact]
        public void SettingEggMetDateSetsComponents()
        {
            var pk = new PK7
            {
                Egg_Day = 12,
                Egg_Month = 12,
                Egg_Year = 12
            };

            pk.EggMetDate = new DateTime(2005, 5, 5);

            pk.Egg_Day.Should().Be(5);
            pk.Egg_Month.Should().Be(5);
            pk.Egg_Year.Should().Be(5);
        }
    }
}

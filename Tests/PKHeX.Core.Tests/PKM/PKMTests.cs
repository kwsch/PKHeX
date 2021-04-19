using System;
using FluentAssertions;
using PKHeX.Core;
using Xunit;

namespace PKHeX.Tests.PKM
{
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

            pk.Met_Day.Should().Be(12);
            pk.Met_Month.Should().Be(12);
            pk.Met_Year.Should().Be(12);

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

            pk.Met_Day.Should().Be(12);
            pk.Met_Month.Should().Be(12);
            pk.Met_Year.Should().Be(12);

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

            pk.Egg_Day.Should().Be(12);
            pk.Egg_Month.Should().Be(12);
            pk.Egg_Year.Should().Be(12);

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

            pk.Egg_Day.Should().Be(12);
            pk.Egg_Month.Should().Be(12);
            pk.Egg_Year.Should().Be(12);

            pk.EggMetDate = new DateTime(2005, 5, 5);

            pk.Egg_Day.Should().Be(5);
            pk.Egg_Month.Should().Be(5);
            pk.Egg_Year.Should().Be(5);
        }
    }
}

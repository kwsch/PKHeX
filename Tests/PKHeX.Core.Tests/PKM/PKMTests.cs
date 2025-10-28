using System;
using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests.PKM;

public class MetDateTests
{
    [Fact]
    public void MetDateNullWhenDateComponentsAreAllZero()
    {
        var pk = new PK7
        {
            MetDay = 0,
            MetMonth = 0,
            MetYear = 0,
        };

        pk.MetDate.HasValue.Should().BeFalse();
    }

    [Fact]
    public void MetDateReturnsCorrectDate()
    {
        var pk = new PK7
        {
            MetDay = 10,
            MetMonth = 8,
            MetYear = 16,
        };

        pk.MetDate.GetValueOrDefault().Should().Be(new DateOnly(2016, 8, 10));
    }

    [Fact]
    public void MetDateCalculatesYear0Correctly()
    {
        var pk = new PK7
        {
            MetDay = 1,
            MetMonth = 1,
            MetYear = 0,
        };

        pk.MetDate.GetValueOrDefault().Year.Should().Be(2000);
    }

    [Fact]
    public void SettingToNullZerosComponents()
    {
        var pk = new PK7
        {
            MetDay = 12,
            MetMonth = 12,
            MetYear = 12,
        };

        pk.MetDay.Should().Be(12);
        pk.MetMonth.Should().Be(12);
        pk.MetYear.Should().Be(12);

        pk.MetDate = null;

        pk.MetDay.Should().Be(0);
        pk.MetMonth.Should().Be(0);
        pk.MetYear.Should().Be(0);
    }

    [Fact]
    public void SettingMetDateSetsComponents()
    {
        var pk = new PK7
        {
            MetDay = 12,
            MetMonth = 12,
            MetYear = 12,
        };

        pk.MetDay.Should().Be(12);
        pk.MetMonth.Should().Be(12);
        pk.MetYear.Should().Be(12);

        pk.MetDate = new DateOnly(2005, 5, 5);

        pk.MetDay.Should().Be(5);
        pk.MetMonth.Should().Be(5);
        pk.MetYear.Should().Be(5);
    }
}

public class EggMetDateTests
{
    [Fact]
    public void EggMetDateNullWhenDateComponentsAreAllZero()
    {
        var pk = new PK7
        {
            EggDay = 0,
            EggMonth = 0,
            EggYear = 0,
        };

        pk.EggMetDate.HasValue.Should().BeFalse();
    }

    [Fact]
    public void EggMetDateReturnsCorrectDate()
    {
        var pk = new PK7
        {
            EggDay = 10,
            EggMonth = 8,
            EggYear = 16,
        };

        pk.EggMetDate.GetValueOrDefault().Should().Be(new DateOnly(2016, 8, 10));
    }

    [Fact]
    public void EggMetDateCalculatesYear0Correctly()
    {
        var pk = new PK7
        {
            EggDay = 1,
            EggMonth = 1,
            EggYear = 0,
        };

        pk.EggMetDate.GetValueOrDefault().Year.Should().Be(2000);
    }

    [Fact]
    public void SettingEggMetDateToNullZerosComponents()
    {
        var pk = new PK7
        {
            EggDay = 12,
            EggMonth = 12,
            EggYear = 12,
        };

        pk.EggDay.Should().Be(12);
        pk.EggMonth.Should().Be(12);
        pk.EggYear.Should().Be(12);

        pk.EggMetDate = null;

        pk.EggDay.Should().Be(0);
        pk.EggMonth.Should().Be(0);
        pk.EggYear.Should().Be(0);
    }

    [Fact]
    public void SettingEggMetDateSetsComponents()
    {
        var pk = new PK7
        {
            EggDay = 12,
            EggMonth = 12,
            EggYear = 12,
        };

        pk.EggDay.Should().Be(12);
        pk.EggMonth.Should().Be(12);
        pk.EggYear.Should().Be(12);

        pk.EggMetDate = new DateOnly(2005, 5, 5);

        pk.EggDay.Should().Be(5);
        pk.EggMonth.Should().Be(5);
        pk.EggYear.Should().Be(5);
    }
}

using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests.Util;

public class DataUtilTests
{
    [Fact]
    public void GetsCorrectNumberOfSpeciesNames()
    {
        var names = GameLanguage.GetStrings("species", GameLanguage.DefaultLanguage);
        names.Length.Should().Be((int)Species.MAX_COUNT);
    }

    [Fact]
    public void GetsCorrectNumberOfAbilityNames()
    {
        var names = GameLanguage.GetStrings("abilities", GameLanguage.DefaultLanguage);
        names.Length.Should().Be((int)Ability.MAX_COUNT);
    }

    [Fact]
    public void GetsCorrectNumberOfMoveNames()
    {
        var names = GameLanguage.GetStrings("moves", GameLanguage.DefaultLanguage);
        names.Length.Should().Be((int)Move.MAX_COUNT);
    }
}

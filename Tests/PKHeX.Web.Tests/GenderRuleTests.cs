using FluentAssertions;
using PKHeX.Core;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>The genders offered for a species follow its personal data's gender ratio, as the desktop's gender toggle allows them.</summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class GenderRuleTests
{
    [Theory]
    [InlineData(Species.Zigzagoon, GenderRule.Either)]
    [InlineData(Species.Tauros, GenderRule.OnlyMale)]
    [InlineData(Species.Chansey, GenderRule.OnlyFemale)]
    [InlineData(Species.Magnemite, GenderRule.Genderless)]
    [InlineData(Species.Meowstic, GenderRule.Either)]
    public void TheRuleComesFromTheGenderRatio(Species species, GenderRule rule)
    {
        GenderRules.Of(PersonalTable.AO[(ushort)species]).Should().Be(rule);
        GenderRules.Of(PersonalTable.XY[(ushort)species]).Should().Be(rule);
    }

    [Fact]
    public void EachRuleAllowsItsGendersOnly()
    {
        GenderRules.Allowed(GenderRule.Either).Should().Equal(EntityGender.Male, EntityGender.Female);
        GenderRules.Allowed(GenderRule.OnlyMale).Should().Equal(EntityGender.Male);
        GenderRules.Allowed(GenderRule.OnlyFemale).Should().Equal(EntityGender.Female);
        GenderRules.Allowed(GenderRule.Genderless).Should().Equal(EntityGender.Genderless);
    }

    [Fact]
    public void EverySingleGenderRuleAllowsCoresFixedGender()
    {
        // Every species and form in the ORAS table: the one gender offered is the one Core fixes, and both are offered otherwise.
        var table = PersonalTable.AO;
        for (ushort species = 1; species <= table.MaxSpeciesID; species++)
        {
            var personal = table[species];
            var allowed = GenderRules.Allowed(GenderRules.Of(personal));
            if (personal.IsDualGender)
            {
                allowed.Should().HaveCount(2);
            }
            else
            {
                allowed.Should().Equal(personal.FixedGender());
            }
        }
    }

    [Fact]
    public void AnUnknownRuleIsAProgrammingError()
    {
        var allowed = () => GenderRules.Allowed((GenderRule)99);

        allowed.Should().Throw<ArgumentOutOfRangeException>();
    }
}

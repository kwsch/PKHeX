using FluentAssertions;
using PKHeX.Core;
using Xunit;

namespace PKHeX.Tests.Util
{
    public class DataUtilTests
    {
        [Fact]
        public void GetsCorrectNumberOfSpeciesNames()
        {
            var names = Core.Util.GetSpeciesList("en");
            names.Length.Should().Be((int)Species.MAX_COUNT);
        }

        [Fact]
        public void GetsCorrectNumberOfAbilityNames()
        {
            var names = Core.Util.GetAbilitiesList("en");
            names.Length.Should().Be((int)Ability.MAX_COUNT);
        }
    }
}

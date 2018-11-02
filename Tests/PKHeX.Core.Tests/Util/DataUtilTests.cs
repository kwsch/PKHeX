
using FluentAssertions;
using Xunit;

namespace PKHeX.Tests.Util
{
    public class DataUtilTests
    {
        [Fact]
        public void GetsCorrectNumberOfPokemonNames()
        {
            var names = PKHeX.Core.Util.GetSpeciesList("en");
            names.Length.Should().Be(810);
        }
    }
}

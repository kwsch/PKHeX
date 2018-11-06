using FluentAssertions;
using PKHeX.Core;
using Xunit;

namespace PKHeX.Tests.Util
{
    public class GeoLocationTests
    {
        [Theory]
        [InlineData("en", 1, "Japan")]
        public void ReturnsCorrectCountryNameByString(string language, int country, string expectedName)
        {
            GeoLocation.GetCountryName(language, country).Should().Be(expectedName);
        }

        [Theory]
        [InlineData(LanguageID.English, 10, "Argentina")]
        public void ReturnsCorrectCountryNameByLanguageId(LanguageID languageId, int country, string expectedName)
        {
            GeoLocation.GetCountryName(languageId, country).Should().Be(expectedName);
        }

        [Theory]
        [InlineData("en", 1, 2, "Tokyo")]
        public void ReturnsCorrectRegionNameByString(string language, int country, int region, string expectedName)
        {
            GeoLocation.GetRegionName(language, country, region).Should().Be(expectedName);
        }

        [Theory]
        [InlineData(LanguageID.Korean, 186, 1, "버뮤다")]
        public void ReturnsCorrectRegionNameByLanguageId(LanguageID languageId, int country, int region, string expectedName)
        {
            GeoLocation.GetRegionName(languageId, country, region).Should().Be(expectedName);
        }
    }
}

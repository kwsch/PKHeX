using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests.Util;

public class GeoLocationTests
{
    [Theory]
    [InlineData("en", 1, "Japan")]
    public void ReturnsCorrectCountryNameByString(string language, byte country, string expectedName)
    {
        GeoLocation.GetCountryName(language, country).Should().Be(expectedName);
    }

    [Theory]
    [InlineData(LanguageID.English, 10, "Argentina")]
    public void ReturnsCorrectCountryNameByLanguageId(LanguageID languageId, byte country, string expectedName)
    {
        GeoLocation.GetCountryName(languageId, country).Should().Be(expectedName);
    }

    [Theory]
    [InlineData("en", 1, 2, "Tokyo")]
    public void ReturnsCorrectRegionNameByString(string language, byte country, byte region, string expectedName)
    {
        GeoLocation.GetRegionName(language, country, region).Should().Be(expectedName);
    }

    [Theory]
    [InlineData(LanguageID.Korean, 186, 1, "버뮤다")]
    public void ReturnsCorrectRegionNameByLanguageId(LanguageID languageId, byte country, byte region, string expectedName)
    {
        GeoLocation.GetRegionName(languageId, country, region).Should().Be(expectedName);
    }
}

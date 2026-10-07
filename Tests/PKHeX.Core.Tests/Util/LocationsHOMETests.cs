using System.Linq;
using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests.Util;

public class LocationsHOMETests
{
    [Theory]
    [InlineData(0, false)]
    [InlineData(59993, false)]
    [InlineData(59994, true)]
    [InlineData(59995, true)]
    [InlineData(59996, true)]
    [InlineData(59997, true)]
    [InlineData(59998, true)]
    [InlineData(59999, true)]
    [InlineData(60000, true)]
    [InlineData(60001, false)]
    [InlineData(65534, false)]
    [InlineData(65535, false)]
    public void RecognizesSWSHSideTransferLocations(ushort location, bool expected)
    {
        LocationsHOME.IsLocationSWSH(location).Should().Be(expected);

        var pk = new PK8 { Version = GameVersion.SW, MetLocation = location };
        pk.IsSideTransfer.Should().Be(expected);
        pk.HasOriginalMetLocation.Should().Be(!expected);
    }

    [Theory]
    [InlineData(59994, GameVersion.LG)]
    [InlineData(59995, GameVersion.FR)]
    [InlineData(59996, GameVersion.VL)]
    [InlineData(59997, GameVersion.SL)]
    [InlineData(59998, GameVersion.SP)]
    [InlineData(59999, GameVersion.BD)]
    [InlineData(60000, GameVersion.PLA)]
    public void SWSHLocationListsIncludeLocalizedHOMEOrigins(ushort location, GameVersion origin)
    {
        foreach (var language in GameLanguage.AllSupportedLanguages)
        {
            var strings = GameInfo.GetStrings(language);
            var source = new MetDataSource(strings);
            foreach (var version in new[] { GameVersion.SW, GameVersion.SH })
            {
                var locations = source.GetLocationList(version, EntityContext.Gen8);
                var matches = locations.Where(z => z.Value == location).ToArray();
                matches.Should().ContainSingle($"{version} must preserve HOME location {location} in {language}");
                matches[0].Text.Should().Be(strings.gamelist[(int)origin]);
            }
        }
    }
}

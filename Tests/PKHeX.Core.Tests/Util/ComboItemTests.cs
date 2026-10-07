using System.Linq;
using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests.Util;

public class ComboItemTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(44)]
    [InlineData(45)]
    public void KnownValuesKeepTheirOriginalLabelsAndList(int value)
    {
        ComboItem[] list = [new("None", 0), new("Sword", 44), new("Shield", 45)];
        Core.Util.GetCBListWithValue(list, value).Should().BeSameAs(list);
    }

    [Theory]
    [InlineData(57)]
    [InlineData(255)]
    [InlineData(59994)]
    [InlineData(59995)]
    [InlineData(65535)]
    public void UnsupportedValuesAreSelectableWithoutMutatingSharedLists(int value)
    {
        ComboItem[] source = [new("None", 0), new("Known", 44)];
        var before = source.ToArray();
        var result = Core.Util.GetCBListWithValue(source, value);
        result.Should().HaveCount(source.Length + 1);
        result.Should().ContainSingle(z => z.Value == value);
        source.Should().Equal(before);
        Core.Util.GetCBListWithValue(result, value).Should().BeSameAs(result);
        Core.Util.GetCBListWithValue(result, 44).Should().BeSameAs(result);
    }

    [Fact]
    public void UnsupportedHOMEOriginIsRetainedWithoutChangingGameSources()
    {
        var source = GameInfo.Sources.VersionDataSource;
        var before = source.ToArray();
        const int unknown = (int)GameVersion.Invalid;
        var result = Core.Util.GetCBListWithValue(source, unknown);
        result.Should().ContainSingle(z => z.Value == unknown);
        source.Should().Equal(before);
    }

    [Fact]
    public void EmptyListsCanRetainAnUnrecognizedStoredLocation()
    {
        var result = Core.Util.GetCBListWithValue([], 30007);
        result.Should().ContainSingle(z => z.Value == 30007);
        result[0].Text.Should().NotBeNullOrEmpty();
    }
}

using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests.Legality;

public class LegalityRules
{
    [Theory]
    [InlineData(GameVersion.B)]
    public void HasOriginalMetLocation5(GameVersion g)
    {
        var pk5 = new PK5 { Version = g };
        pk5.HasOriginalMetLocation.Should().BeTrue();
    }

    [Theory]
    [InlineData(GameVersion.B)]
    [InlineData(GameVersion.X)]
    public void HasOriginalMetLocation6(GameVersion g)
    {
        var pk5 = new PK6 { Version = g };
        pk5.HasOriginalMetLocation.Should().BeTrue();
    }

    [Theory]
    [InlineData(GameVersion.B)]
    [InlineData(GameVersion.X)]
    [InlineData(GameVersion.SN)]
    public void HasOriginalMetLocation7(GameVersion g)
    {
        var pk5 = new PK7 { Version = g };
        pk5.HasOriginalMetLocation.Should().BeTrue();
    }
}

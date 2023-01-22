using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests.General;

public class GeneralTests
{
    [Fact]
    public void SWSH_Hypothesis()
    {
        GameVersion.SW.Should().Be((GameVersion)44);
        GameVersion.SH.Should().Be((GameVersion)45);
    }

    [Fact]
    public void BDSP_Hypothesis()
    {
        GameVersion.BD.Should().Be((GameVersion)48);
        GameVersion.SP.Should().Be((GameVersion)49);
    }

    [Fact]
    public void SV_Hypothesis()
    {
        GameVersion.SL.Should().Be((GameVersion)50);
        GameVersion.VL.Should().Be((GameVersion)51);
    }

    [Fact]
    public void StringsLoad() => GameInfo.GetStrings(GameLanguage.DefaultLanguage);

    [Fact]
    public void SourcesLoad() => GameInfo.Strings = GameInfo.GetStrings(GameLanguage.DefaultLanguage);
}

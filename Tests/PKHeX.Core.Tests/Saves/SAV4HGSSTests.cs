using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests.Saves;

public class SAV4HGSSTests
{
    [Fact]
    public void Constructor_DoesNotModifyData()
    {
        var data = new byte[SaveUtil.SIZE_G4RAW];
        data[0x1C] = 7; // SysInfo RTC date (day)
        data[0x80] = (byte)GameVersion.HG; // ROM code
        var original = (byte[])data.Clone();

        var sav = new SAV4HGSS(data);

        data.Should().Equal(original);
        sav.Version.Should().Be(GameVersion.HG);
    }

    [Fact]
    public void Blank_DoesNotWriteIntoSystemInfo()
    {
        var sav = new SAV4HGSS();
        sav.General[0x1C].Should().Be(0);
    }

    [Theory]
    [InlineData(GameVersion.HG)]
    [InlineData(GameVersion.SS)]
    public void BlankSaveFile_SetsVersion(GameVersion version)
    {
        var sav = (SAV4HGSS)BlankSaveFile.Get(version);
        sav.Version.Should().Be(version);
        sav.General[0x1C].Should().Be(0);
    }
}

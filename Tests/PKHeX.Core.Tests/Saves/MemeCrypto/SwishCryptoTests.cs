using FluentAssertions;
using PKHeX.Core;
using Xunit;

namespace PKHeX.Tests.Saves
{
    public class SwishCryptoTests
    {
        [Fact]
        public void SizeCheck()
        {
            SCTypeCode.Bool3.GetTypeSize().Should().Be(1);
        }

        [Fact]
        public void CanMakeBlankSAV8()
        {
            var sav = SaveUtil.GetBlankSAV(GameVersion.SW, "PKHeX");
            sav.Should().NotBeNull();
        }
    }
}
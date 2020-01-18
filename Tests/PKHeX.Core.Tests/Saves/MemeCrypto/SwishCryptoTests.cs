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
            SCTypeCode.Common3.GetTypeSize().Should().Be(1);
        }
    }
}
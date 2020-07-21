using System;
using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests.Legality
{
    public class RaidTests
    {
        public const string Charizard = "65E79FC0000085F1060060045CF0A3AA142C10005E00140015010000EE5501E2080A00000000000004FCFC0000000000000000008800000000001C0000000000000000000000000000000000000000008AAD000000000000430068006100720069007A00610072006400000000000000000035001E024C009B01181010080303030300000000000000002901FF7F1E3F0A000000000000000000000000000000000000000000000056006900630074006F0072006900610000000000000000000000010201000000320104080000000000000000000000000000000000002C000000020000000000FF0000000000000000000000000000004100720063006800690074000000000000000000000000000000FF000000000000000000140117000000A20009372804000000000000000100000200000000000000000000000000000000000000000064002901B700C10048013D01CE000000";

        [Theory]
        [InlineData(Charizard, 0xbefd08cf9e027d0a)]
        public void CheckMatch(string raw, ulong seed)
        {
            byte[] data = raw.ToByteArray();
            var pk8 = new PK8(data);

            var la = new LegalityAnalysis(pk8);
            var enc = la.EncounterMatch;

            var compare = enc switch
            {
                EncounterStatic8N r => r.Verify(pk8, seed),
                EncounterStatic8ND r => r.Verify(pk8, seed),
                EncounterStatic8NC r => r.Verify(pk8, seed),
                _ => throw new ArgumentException(nameof(enc)),
            };
            compare.Should().BeTrue();
        }
    }
}

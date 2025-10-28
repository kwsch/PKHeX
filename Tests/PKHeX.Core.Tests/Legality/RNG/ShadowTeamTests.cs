using FluentAssertions;
using System;
using Xunit;
using static PKHeX.Core.Encounters3XDTeams;

namespace PKHeX.Core.Tests.Legality.Shadow;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable xUnit1044

public static class ShadowTeamTests
{
    public static TheoryData<ReadOnlyMemory<TeamLock>, uint[]> DelcattyTestData => new()
    {
        { Delcatty, [0xD118BA52, 0xA3127782, 0x16D95FA5, 0x31538B48] },
        { Delcatty, [0x7D5FFE3E, 0x1D5720ED, 0xE0D89C99, 0x3494CDA1] },
        { Delcatty, [0xAEB0C3A6, 0x956DC2FD, 0x3C11DCE8, 0xC93DF897] },
        { Delcatty, [0xACCE2655, 0xFF2BA0A2, 0x22A8A7E6, 0x5F5380F4] },
        { Delcatty, [0xDC1D1894, 0xFC0F75E2, 0x97BFAEBC, 0x38DDE117] },
        { Delcatty, [0xDE278967, 0xFD86C9F7, 0x3E16FCFD, 0x1956D8B5] },
        { Delcatty, [0xF8CB4CAE, 0x42DE628B, 0x48796CDA, 0xF6EAD3E2] },
        { Delcatty, [0x56548F49, 0xA308E7DA, 0x28CB8ADF, 0xBEADBDC3] },
        { Delcatty, [0xF2AC8419, 0xADA208E3, 0xDB3A0BA6, 0x5EEF1076] },
        { Delcatty, [0x9D28899D, 0xA3ECC9F0, 0x606EC6F0, 0x451FAE3C] },
    };

    public static TheoryData<ReadOnlyMemory<TeamLock>, uint[]> ButterfreeTestData => new()
    {
        { Butterfree, [0x4D6BE487, 0xBB3EFBFB, 0x6FD7EE06, 0x289D435F, 0x0EC25CE5] },
        { Butterfree, [0xB04DF5B3, 0x661E499C, 0x94EB752D, 0xC5FA9DE5, 0x0A8C9738] },
        { Butterfree, [0xCBB9A3B0, 0x9AC1A0B8, 0xCA3CAD46, 0x54FFCA27, 0x1D5AEC4F] },
        { Butterfree, [0xB2AF145E, 0x455155C9, 0xB5CE4932, 0x4B8C6554, 0x55CE5E4B] },
        { Butterfree, [0x193A0F3B, 0xE1474ECF, 0x4C30D215, 0x72262B89, 0x9B2F5B53] },
        { Butterfree, [0xB73010B9, 0x361F1DB1, 0x2C65320A, 0x329A4A1E, 0x9334337E] },
        { Butterfree, [0xFB6A6770, 0xE0068ECC, 0xB99B326E, 0x08A18311, 0x92D31CC2] },
        { Butterfree, [0x5B1214BC, 0xB82FDDA9, 0x606D3D18, 0xA142F730, 0xCBA7A0C3] },
        { Butterfree, [0xC7315E32, 0x76566AA1, 0xC0CE436E, 0x98C45DA8, 0x9D1BDC4A] },
        { Butterfree, [0xB687F0AF, 0xC01DB6C6, 0xAD6DEC75, 0xDB041314, 0x0D949325] },
    };

    public static TheoryData<ReadOnlyMemory<TeamLock>, ushort, ushort, int[], uint[]> MawileAntiShinyTestData => new()
    {
        { Mawile, 12345, 51882, [31, 30, 29, 31, 23, 27], [0x4C3005E8, 0xD28DE40E, 0x049F2F05] },
    };

    [Theory]
    [MemberData(nameof(DelcattyTestData))]
    [MemberData(nameof(ButterfreeTestData))]
    public static void VerifyResults(ReadOnlyMemory<TeamLock> locks, uint[] team)
    {
        var pk = new PK3();
        Span<uint> seeds = stackalloc uint[XDRNG.MaxCountSeedsPID];
        var pid = team[^1];
        int count = XDRNG.GetSeeds(seeds, pid);
        var reg = seeds[..count];
        var match = IsAnySeedMatch(locks.Span, reg, pk);
        match.Should().BeTrue($"because the lock conditions for team's {(Species)locks.Span[0].Species} should have been verified");
    }

    [Theory]
    [MemberData(nameof(MawileAntiShinyTestData))]
    public static void VerifyMawileAntishiny(ReadOnlyMemory<TeamLock> team, ushort tid, ushort sid, int[] ivs, uint[] resultPIDs)
    {
        VerifyResultsAntiShiny(resultPIDs, team.Span, tid, sid, ivs);
    }

    private static bool IsAnySeedMatch(ReadOnlySpan<TeamLock> team, Span<uint> seeds, PK3 template)
    {
        foreach (var s in seeds)
        {
            var origin = XDRNG.Prev3(s);
            var seed = origin;

            var iv1 = XDRNG.Next15(ref seed); // IV1
            var iv2 = XDRNG.Next15(ref seed); // IV2
            _ = XDRNG.Next16(ref seed); // Ability
            var d16 = XDRNG.Next16(ref seed); // PID
            var e16 = XDRNG.Next16(ref seed); // PID

            template.IV32 = (iv2 << 15) | iv1;
            template.PID = (d16 << 16) | e16;

            var info = MethodFinder.Analyze(template);
            info.OriginSeed.Should().Be(origin);
            info.Type.Should().Be(PIDType.CXD, "because the PID should have matched the CXD spread");

            if (LockFinder.IsAllShadowLockValid(team, origin))
                return true;
        }
        return false;
    }

    private static void VerifyResultsAntiShiny(ReadOnlySpan<uint> resultPIDs, ReadOnlySpan<TeamLock> team, ushort tid, ushort sid, ReadOnlySpan<int> ivs)
    {
        var pk3 = new PK3
        {
            PID = resultPIDs[^1],
            TID16 = tid,
            SID16 = sid,
        };
        pk3.SetIVs(ivs);

        var info = MethodFinder.Analyze(pk3);
        info.Type.Should().Be(PIDType.CXD, "because the PID should have matched the CXD spread");
        bool result = LockFinder.IsAllShadowLockValid(team, info.OriginSeed, pk3.TSV);
        result.Should().BeTrue();

        // if you're here inspecting what's so special about this method,
        // double check that the Team's PIDs exactly match what's in the expected result array.
        // as of this test's date, the methods/fields aren't exposed for viewing.
    }
}

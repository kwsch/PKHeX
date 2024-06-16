using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using static PKHeX.Core.Encounters3XDTeams;
using static PKHeX.Core.Encounters3ColoTeams;

namespace PKHeX.Core.Tests.Legality.Shadow;

public static class ValidityTests
{
    public static IEnumerable<object[]> Lock1()
    {
        // Zubat (F) (Serious)
        yield return [Poochyena, 0xAF4E3161, new[] { 11, 29, 25, 6, 23, 10 }];

        // Murkrow (M) (Docile)
        yield return [Pineco, 0xC3A0F1E5, new[] { 30, 3, 9, 10, 27, 30 }];
    }

    public static IEnumerable<object[]> Lock2()
    {
        // Goldeen (F) (Serious)
        // Horsea (M) (Quirky)
        yield return [Spheal, 0xA459BF44, new[] { 0, 11, 4, 28, 6, 13 }];

        // Kirlia (M) (Hardy)
        // Linoone (F) (Hardy)
        yield return [Natu, 0x8E14DAB6, new[] { 29, 24, 30, 16, 3, 18 }];

        // Remoraid (M) (Docile) -- 73DB58CC
        // Golbat (M) (Bashful) -- F6B04390
        yield return [Roselia, 0x30E87CC7, new[] { 22, 11, 8, 26, 4, 29 }];

        // 519AEF0E
        // Duskull (M) (Quirky) -- 45BE3B97
        // Spinarak (F) (Hardy) -- E18F5A3E
        yield return [ColoMakuhita, 0xC252FEBA, new[] { 15, 9, 17, 16, 24, 22 }];

        // 559C5F72 -- Quirky F => skip
        // Duskull (M) (Quirky) -- A5AC2CCB
        // Spinarak (F) (Hardy) -- D08FF135
        yield return [ColoMakuhita, 0x61C676FC, new[] { 20, 28, 21, 18, 9, 1 }];

        // 3CCB97BA -- Quirky F => skip * 2, Hardy Skip
        // Duskull (M) (Quirky) -- 7F0D6783 @ 161
        // Spinarak (F) (Hardy) -- 6C03F545 @ 182
        yield return [ColoMakuhita, 0x3B27608D, new[] { 7, 12, 5, 19, 3, 7 }];
    }

    public static IEnumerable<object[]> Lock3()
    {
        // Luvdisc (F) (Docile)
        // Beautifly (M) (Hardy)
        // Roselia (M) (Quirky)
        yield return [Delcatty, 0x9BECA2A6, new[] { 31, 31, 25, 13, 22, 1 }];

        // Kadabra (M) (Docile)
        // Sneasel (F) (Hardy)
        // Misdreavus (F) (Bashful)
        yield return [Meowth, 0x77D87601, new[] { 10, 27, 26, 13, 30, 19 }];

        // Ralts (M) (Docile)
        // Voltorb (-) (Hardy)
        // Bagon (F) (Quirky)
        yield return [Numel, 0x37F95B26, new[] { 11, 8, 5, 10, 28, 14 }];
    }

    public static IEnumerable<object[]> Lock4()
    {
        // Ninetales (F) (Serious)
        // Jumpluff (M) (Docile)
        // Azumarill (F) (Hardy)
        // Shadow Tangela
        yield return [Butterfree, 0x2E49AC34, new[] { 15, 24, 7, 2, 11, 2 }];

        // Huntail (M) (Docile)
        // Cacturne (F) (Hardy)
        // Weezing (F) (Serious)
        // Ursaring (F) (Bashful)
        yield return [Arbok, 0x1973FD07, new[] { 13, 30, 3, 16, 20, 9 }];

        // Lairon (F) (Bashful)
        // Sealeo (F) (Serious)
        // Slowking (F) (Docile)
        // Ursaring (M) (Quirky)
        yield return [Primeape, 0x33893D4C, new[] { 26, 25, 24, 28, 29, 30 }];
    }

    public static IEnumerable<object[]> Lock5()
    {
        // many prior, all non-shadow
        yield return [Seedot, 0x8CBD29DB, new[] { 19, 29, 30, 0, 7, 2 }];
    }

    [Theory]
    [MemberData(nameof(Lock1))]
    [MemberData(nameof(Lock2))]
    [MemberData(nameof(Lock3))]
    [MemberData(nameof(Lock4))]
    [MemberData(nameof(Lock5))]
    public static void Verify(TeamLock[] teams, uint pid, int[] ivs)
    {
        var pk3 = new PK3 { PID = pid, IVs = ivs };
        var info = MethodFinder.Analyze(pk3);
        info.Type.Should().Be(PIDType.CXD, "because the PID should match the CXD spread");
        bool match = LockFinder.IsAllShadowLockValid(info, teams);
        match.Should().BeTrue($"because the lock conditions for {teams[0].Species} should have been verified");
    }
}

public static class PIDTests
{
    public static IEnumerable<object[]> TestData()
    {
        yield return
        [
            new uint[][]
            {
                [0xD118BA52, 0xA3127782, 0x16D95FA5, 0x31538B48],
                [0x7D5FFE3E, 0x1D5720ED, 0xE0D89C99, 0x3494CDA1],
                [0xAEB0C3A6, 0x956DC2FD, 0x3C11DCE8, 0xC93DF897],
                [0xACCE2655, 0xFF2BA0A2, 0x22A8A7E6, 0x5F5380F4],
                [0xDC1D1894, 0xFC0F75E2, 0x97BFAEBC, 0x38DDE117],
                [0xDE278967, 0xFD86C9F7, 0x3E16FCFD, 0x1956D8B5],
                [0xF8CB4CAE, 0x42DE628B, 0x48796CDA, 0xF6EAD3E2],
                [0x56548F49, 0xA308E7DA, 0x28CB8ADF, 0xBEADBDC3],
                [0xF2AC8419, 0xADA208E3, 0xDB3A0BA6, 0x5EEF1076],
                [0x9D28899D, 0xA3ECC9F0, 0x606EC6F0, 0x451FAE3C],
            },
            Delcatty,
        ];
        yield return
        [
            new uint[][]
            {
                [0x4D6BE487, 0xBB3EFBFB, 0x6FD7EE06, 0x289D435F, 0x0EC25CE5],
                [0xB04DF5B3, 0x661E499C, 0x94EB752D, 0xC5FA9DE5, 0x0A8C9738],
                [0xCBB9A3B0, 0x9AC1A0B8, 0xCA3CAD46, 0x54FFCA27, 0x1D5AEC4F],
                [0xB2AF145E, 0x455155C9, 0xB5CE4932, 0x4B8C6554, 0x55CE5E4B],
                [0x193A0F3B, 0xE1474ECF, 0x4C30D215, 0x72262B89, 0x9B2F5B53],
                [0xB73010B9, 0x361F1DB1, 0x2C65320A, 0x329A4A1E, 0x9334337E],
                [0xFB6A6770, 0xE0068ECC, 0xB99B326E, 0x08A18311, 0x92D31CC2],
                [0x5B1214BC, 0xB82FDDA9, 0x606D3D18, 0xA142F730, 0xCBA7A0C3],
                [0xC7315E32, 0x76566AA1, 0xC0CE436E, 0x98C45DA8, 0x9D1BDC4A],
                [0xB687F0AF, 0xC01DB6C6, 0xAD6DEC75, 0xDB041314, 0x0D949325],
            },
            Butterfree,
        ];
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public static void VerifyResults(IReadOnlyList<uint[]> results, TeamLock[] team)
    {
        var pk = new PK3();
        Span<uint> seeds = stackalloc uint[XDRNG.MaxCountSeedsPID];
        for (int i = 0; i < results.Count; i++)
        {
            var result = results[i];
            var pid = result[^1];
            int count = XDRNG.GetSeeds(seeds, pid);
            var reg = seeds[..count];
            bool match = false;
            foreach (var s in reg)
            {
                var seed = XDRNG.Prev3(s);
                PIDGenerator.SetValuesFromSeed(pk, PIDType.CXD, seed);
                var info = MethodFinder.Analyze(pk);
                info.OriginSeed.Should().Be(seed);
                info.Type.Should().Be(PIDType.CXD, "because the PID should have matched the CXD spread");
                if (!LockFinder.IsAllShadowLockValid(info, team))
                    continue;
                match = true;
                break;
            }
            match.Should().BeTrue($"because the lock conditions for result {i} and species {team[0].Species} should have been verified");
        }
    }

    private static ReadOnlySpan<uint> MawileTeamPIDs =>
    [
        0x4C3005E8, // Loudred
        0xD28DE40E, // Girafarig (re - rolled 64 times to next viable match)
        0x049F2F05, // Mawile
    ];

    private static ReadOnlySpan<int> MawileIVs => [31, 30, 29, 31, 23, 27];

    [Fact]
    public static void VerifyMawileAntishiny()
    {
        VerifyResultsAntiShiny(MawileTeamPIDs, Mawile, 12345, 51882, MawileIVs);
    }

    private static void VerifyResultsAntiShiny(ReadOnlySpan<uint> resultPIDs, TeamLock[] team, ushort tid, ushort sid, ReadOnlySpan<int> ivs)
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
        bool result = LockFinder.IsAllShadowLockValid(info, team, pk3.TSV);
        result.Should().BeTrue();

        // if you're here inspecting what's so special about this method,
        // double check that the Team's PIDs exactly match what's in the expected result array.
        // as of this test's date, the methods/fields aren't exposed for viewing.
    }
}

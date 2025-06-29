using System;
using FluentAssertions;
using Xunit;
using static PKHeX.Core.Encounters3XDTeams;
using static PKHeX.Core.Encounters3ColoTeams;

namespace PKHeX.Core.Tests.Legality.Shadow;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable xUnit1044

public static class ShadowTests
{
    public static TheoryData<uint, int[], ReadOnlyMemory<TeamLock>> LockData => new()
    {
        // 1 Lock
        { 0xAF4E3161, [11, 29, 25, 06, 23, 10], Poochyena },
        { 0xC3A0F1E5, [30, 03, 09, 10, 27, 30], Pineco },
        // 2 Locks
        { 0xA459BF44, [00, 11, 04, 28, 06, 13], Spheal },
        { 0x8E14DAB6, [29, 24, 30, 16, 03, 18], Natu },
        { 0x30E87CC7, [22, 11, 08, 26, 04, 29], Roselia },
        { 0xC252FEBA, [15, 09, 17, 16, 24, 22], ColoMakuhita },
        { 0x61C676FC, [20, 28, 21, 18, 09, 01], ColoMakuhita },
        { 0x3B27608D, [07, 12, 05, 19, 03, 07], ColoMakuhita },
        // 3 Locks
        { 0x9BECA2A6, [31, 31, 25, 13, 22, 01], Delcatty },
        { 0x77D87601, [10, 27, 26, 13, 30, 19], Meowth },
        { 0x37F95B26, [11, 08, 05, 10, 28, 14], Numel },
        // 4 Locks
        { 0x2E49AC34, [15, 24, 07, 02, 11, 02], Butterfree },
        { 0x1973FD07, [13, 30, 03, 16, 20, 09], Arbok },
        { 0x33893D4C, [26, 25, 24, 28, 29, 30], Primeape },
        // 5 Locks
        { 0x8CBD29DB, [19, 29, 30, 00, 07, 02], Seedot },
    };

    [Theory]
    [MemberData(nameof(LockData))]
    public static void Verify(uint pid, int[] ivs, ReadOnlyMemory<TeamLock> teams)
    {
        var pk3 = new PK3 { PID = pid, IVs = ivs };
        var info = MethodFinder.Analyze(pk3);
        info.Type.Should().Be(PIDType.CXD, "because the PID should match the CXD spread");
        bool match = LockFinder.IsAllShadowLockValid(teams.Span, info.OriginSeed);
        match.Should().BeTrue($"because the lock conditions for {teams.Span[0].Species} should have been verified");
    }
}

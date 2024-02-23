using System;
using System.Runtime.InteropServices;
using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests.General;

public class MarshalTests
{
    [Theory]
    [InlineData(10, typeof(PIDIV))]
    [InlineData(8, typeof(MoveResult))]
    [InlineData(8, typeof(EvolutionMethod))]
    [InlineData(8, typeof(Moveset))]
    [InlineData(8, typeof(SCXorShift32))]
    [InlineData(16, typeof(Xoroshiro128Plus))]
    [InlineData(16, typeof(Xoroshiro128Plus8b))]
    [InlineData(16, typeof(XorShift128))]
    public void MarshalSizeExact(int expect, Type t) => Marshal.SizeOf(t).Should().Be(expect);

    [Theory]
    [InlineData( 8, typeof(LeadSeed))]
    [InlineData( 8, typeof(NPCLock))]
    [InlineData( 8, typeof(IndividualValueSet))]
    [InlineData( 8, typeof(EvolutionOrigin))]
    [InlineData(16, typeof(DreamWorldEntry))]
    [InlineData(16, typeof(CheckResult))]
    [InlineData(16, typeof(EvolutionLink))]
    [InlineData(24, typeof(GenerateParam9))]
    public void MarshalSizeLessThanEqual(int expect, Type t) => Marshal.SizeOf(t).Should().BeLessOrEqualTo(expect);
}

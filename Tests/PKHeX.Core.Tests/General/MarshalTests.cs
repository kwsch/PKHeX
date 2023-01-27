using System;
using System.Runtime.InteropServices;
using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests.General;

public class MarshalTests
{
    [Theory]
    [InlineData(8, typeof(PIDIV))]
    [InlineData(8, typeof(MoveResult))]
    [InlineData(8, typeof(EvolutionMethod))]
    [InlineData(8, typeof(Moveset))]
    public void MarshalSizeExact(int expect, Type t) => Marshal.SizeOf(t).Should().Be(expect);

    [Theory]
    [InlineData( 8, typeof(NPCLock))]
    [InlineData( 8, typeof(IndividualValueSet))]
    [InlineData(16, typeof(DreamWorldEntry))]
    [InlineData(24, typeof(GenerateParam9))]
    public void MarshalSizeLessThanEqual(int expect, Type t) => Marshal.SizeOf(t).Should().BeLessOrEqualTo(expect);
}

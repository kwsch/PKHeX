using System.Runtime.InteropServices;
using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests.General;

public class MarshalTests
{
    [Fact]
    public void MarshalSize()
    {
        Marshal.SizeOf(typeof(NPCLock)).Should().BeLessOrEqualTo(8);
        Marshal.SizeOf(typeof(PIDIV)).Should().Be(8);
        Marshal.SizeOf(typeof(MoveResult)).Should().Be(8);
        Marshal.SizeOf(typeof(EvolutionMethod)).Should().Be(8);
        Marshal.SizeOf(typeof(Moveset)).Should().Be(8);
        Marshal.SizeOf(typeof(IndividualValueSet)).Should().BeLessOrEqualTo(8);
        Marshal.SizeOf(typeof(GenerateParam9)).Should().BeLessOrEqualTo(16);
    }
}

using System.Runtime.InteropServices;
using FluentAssertions;
using PKHeX.Core;
using Xunit;

namespace PKHeX.Tests.General;

public class MarshalTests
{
    [Fact]
    public void MarshalSize()
    {
        Marshal.SizeOf(typeof(NPCLock)).Should().Be(8);
        Marshal.SizeOf(typeof(PIDIV)).Should().Be(8);
        Marshal.SizeOf(typeof(MoveResult)).Should().Be(8);
        Marshal.SizeOf(typeof(EvolutionMethod)).Should().Be(8);
    }
}

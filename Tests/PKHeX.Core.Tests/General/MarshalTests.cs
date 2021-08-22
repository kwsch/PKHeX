using System.Runtime.InteropServices;
using FluentAssertions;
using PKHeX.Core;
using Xunit;

namespace PKHeX.Tests.General
{
    public class MarshalTests
    {
        [Fact]
        public void MarshalStructure()
        {
            new DecorationInventory3().ToBytes().Length.Should().Be(DecorationInventory3.SIZE);
        }

        [Fact]
        public void MarshalClass()
        {
            new Swarm3().ToBytesClass().Length.Should().Be(Swarm3.SIZE);
        }

        [Fact]
        public void MarshalSize()
        {
            Marshal.SizeOf(typeof(NPCLock)).Should().Be(8);
            Marshal.SizeOf(typeof(PIDIV)).Should().Be(8);
        }
    }
}

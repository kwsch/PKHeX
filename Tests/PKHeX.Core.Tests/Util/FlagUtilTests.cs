using System;
using System.Linq;
using FluentAssertions;
using PKHeX.Core;
using Xunit;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Tests.Util;

public class FlagUtilTests
{
    [Theory]
    [InlineData(1, 0, 0)]
    [InlineData(2, 0, 1)]
    [InlineData(0x8000_0000, 3, 7)]
    public void GetSetFlag(uint raw, int byteIndex, int bitIndex)
    {
        Span<byte> data = stackalloc byte[4];
        WriteUInt32LittleEndian(data, raw);
        var value = FlagUtil.GetFlag(data, byteIndex, bitIndex);
        value.Should().Be(true);

        var copy = new byte[data.Length];
        FlagUtil.SetFlag(copy, byteIndex, bitIndex, true);
        data.SequenceEqual(copy).Should().BeTrue();
    }

    [Theory]
    [InlineData(0x7FFF_FFFE, 0, 0)]
    public void ClearFlag(uint raw, int byteIndex, int bitIndex)
    {
        Span<byte> data = stackalloc byte[4];
        WriteUInt32LittleEndian(data, raw);
        var value = FlagUtil.GetFlag(data, byteIndex, bitIndex);
        value.Should().Be(false);

        // does nothing on empty
        var copy = new byte[data.Length];
        FlagUtil.SetFlag(copy, byteIndex, bitIndex, false);
        copy.All(z => z == 0).Should().BeTrue();

        // doesn't clear any other flag
        copy = data.ToArray();
        FlagUtil.SetFlag(copy, byteIndex, bitIndex, false);
        data.SequenceEqual(copy).Should().BeTrue();
    }
}

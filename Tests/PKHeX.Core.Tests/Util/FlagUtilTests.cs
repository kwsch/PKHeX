using System;
using FluentAssertions;
using Xunit;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core.Tests.Util;

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

        Span<byte> copy = stackalloc byte[data.Length];
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
        Span<byte> copy = stackalloc byte[data.Length];
        FlagUtil.SetFlag(copy, byteIndex, bitIndex, false);
        copy.ContainsAnyExcept<byte>(0).Should().BeFalse();

        // doesn't clear any other flag
        data.CopyTo(copy);
        FlagUtil.SetFlag(copy, byteIndex, bitIndex, false);
        data.SequenceEqual(copy).Should().BeTrue();
    }
}

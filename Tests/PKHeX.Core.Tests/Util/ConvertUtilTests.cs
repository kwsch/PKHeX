using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace PKHeX.Tests.Util
{
    public class ConvertUtilTests
    {
        [Theory]
        [InlineData("-050 ", -50)]
        [InlineData("123 45678", 12345678)]
        [InlineData("--8765-43 21", -87654321)]
        public void CheckConvertValidI32(string v, int result)
        {
            var convert = Core.Util.ToInt32(v);
            convert.Should().Be(result);
        }

        [Theory]
        [InlineData("50", 50)]
        [InlineData("12 345678", 12345678)]
        [InlineData("87654 321", 87654321)]
        public void CheckConvertValidU32(string v, uint result)
        {
            var convert = Core.Util.ToUInt32(v);
            convert.Should().Be(result);
        }

        [Theory]
        [InlineData("0x50", 0x50)]
        [InlineData("0x12 345678", 0x12345678)]
        [InlineData("8aF5z4 32-1", 0x8aF54321)]
        public void CheckConvertValidHexU32(string v, uint result)
        {
            var convert = Core.Util.GetHexValue(v);
            convert.Should().Be(result);
        }

        [Theory]
        [InlineData("01020304", 0x1020304)]
        public void CheckConvertHexString(string v, uint result)
        {
            var convert = Core.Util.GetBytesFromHexString(v);
            var u32 = BitConverter.ToUInt32(convert);
            u32.Should().Be(result);

            var remake = Core.Util.GetHexStringFromBytes(convert, 0, convert.Length);
            remake.Should().Be(v);
        }

        [Theory]
        [InlineData(0x12345678, 12345678)]
        public void CheckConvertBCD_Little(uint raw, int expect)
        {
            var data = BitConverter.GetBytes(raw);
            var result = Core.BinaryCodedDecimal.ToInt32LE(data);
            result.Should().Be(expect);

            var newData = Core.BinaryCodedDecimal.GetBytesLE(result, 4);
            data.SequenceEqual(newData).Should().BeTrue();
        }

        [Theory]
        [InlineData(0x78563412, 12345678)]
        public void CheckConvertBCD_Big(uint raw, int expect)
        {
            var data = BitConverter.GetBytes(raw);
            var result = Core.BinaryCodedDecimal.ToInt32BE(data);
            result.Should().Be(expect);

            var newData = Core.BinaryCodedDecimal.GetBytesBE(result, 4);
            data.SequenceEqual(newData).Should().BeTrue();
        }
    }

    public class FlagUtilTests
    {
        [Theory]
        [InlineData(1, 0, 0)]
        [InlineData(2, 0, 1)]
        [InlineData(0x8000_0000, 3, 7)]
        public void GetSetFlag(uint raw, int byteIndex, int bitIndex)
        {
            var data = BitConverter.GetBytes(raw);
            var value = Core.FlagUtil.GetFlag(data, byteIndex, bitIndex);
            value.Should().Be(true);

            var copy = new byte[data.Length];
            Core.FlagUtil.SetFlag(copy, byteIndex, bitIndex, true);
            copy.SequenceEqual(data).Should().BeTrue();
        }

        [Theory]
        [InlineData(0x7FFF_FFFE, 0, 0)]
        public void ClearFlag(uint raw, int byteIndex, int bitIndex)
        {
            var data = BitConverter.GetBytes(raw);
            var value = Core.FlagUtil.GetFlag(data, byteIndex, bitIndex);
            value.Should().Be(false);

            // does nothing on empty
            var copy = new byte[data.Length];
            Core.FlagUtil.SetFlag(copy, byteIndex, bitIndex, false);
            copy.All(z => z == 0).Should().BeTrue();

            // doesn't clear any other flag
            copy = (byte[])data.Clone();
            Core.FlagUtil.SetFlag(copy, byteIndex, bitIndex, false);
            copy.SequenceEqual(data).Should().BeTrue();
        }
    }
}

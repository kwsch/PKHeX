using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Xunit;
using static PKHeX.Core.PokeCrypto;

namespace PKHeX.Core.Tests.PKM;

public class PokeCryptoShuffleTests
{
    // All pokecrypto block sizes
    [Theory]
    [InlineData(12)]
    [InlineData(32)]
    [InlineData(56)]
    [InlineData(80)]
    [InlineData(88)]
    public void InPlaceShuffleMatchesShuffleArray([ConstantExpected] int blockSize)
    {
        ReadOnlySpan<byte> source = CreateData(8 + (4 * blockSize) + 16); // header, blocks, party stats (meh)

        for (uint sv = 0; sv < 32; sv++)
        {
            var expected = ShuffleArray(source, sv, blockSize);
            var actual = source.ToArray();

            var blocks = actual.AsSpan(8, 4 * blockSize);
            switch (blockSize)
            {
                case 12: Shuffle3(blocks, sv); break;
                case 32: Shuffle45(blocks, sv); break;
                case 56: Shuffle67(blocks, sv); break;
                case 80: Shuffle8(blocks, sv); break;
                case 88: Shuffle8A(blocks, sv); break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(blockSize), blockSize, null);
            }

            actual.Should().Equal(expected, $"shuffle value {sv} should match old ShuffleArray behavior");
        }
    }

    private static byte[] CreateData(int length)
    {
        var data = new byte[length];
        for (int i = 0; i < data.Length; i++)
            data[i] = unchecked((byte)(i * 31 + 7));
        return data;
    }
}

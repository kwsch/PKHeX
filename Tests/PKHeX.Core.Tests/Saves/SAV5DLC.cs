using FluentAssertions;
using Xunit;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core.Tests.Saves;

public class SAV5DLC
{
    [Fact]
    public void SetPokeDexSkin_RecomputesChecksumAfterSettingAvailabilityFlag()
    {
        var sav = new SAV5B2W2();

        // Create a random garbage PokeDex skin data with the last 4 bytes reserved for the availability flag and checksum
        byte[] data = new byte[PokeDexSkin5.SIZE];
        for (int i = 0; i < data.Length - sizeof(uint); i++)
            data[i] = (byte)i;

        sav.SetPokeDexSkin(data);

        sav.IsAvailablePokedexSkin.Should().BeTrue();
        ReadUInt32LittleEndian(sav.PokedexSkinData.Span[^sizeof(uint)..]).Should().Be(1u);

        const int offset = 0x6D800;
        var tail = sav.Data[(offset + PokeDexSkin5.SIZE)..];
        ReadUInt16LittleEndian(tail).Should().Be(1);
        ReadUInt16LittleEndian(tail[2..]).Should().Be(Checksums.CRC16_CCITT(sav.PokedexSkinData.Span));
    }
}

using Xunit;
using FluentAssertions;

namespace PKHeX.Core.Tests.Saves;

public static class PokeDex
{
    [Theory]
    [InlineData(Species.Bulbasaur)]
    [InlineData(Species.Voltorb)]
    [InlineData(Species.Genesect)]
    public static void Gen5(Species species)
    {
        var bw = new SAV5B2W2();
        SetDexSpecies(bw, (ushort)species, 0x54);
    }

    [Theory]
    [InlineData(Species.Landorus)]
    public static void Gen5Form(Species species)
    {
        var bw = new SAV5B2W2();
        SetDexSpecies(bw, (ushort)species, 0x54);
        CheckDexFlags5(bw, (ushort)species, 0, 0x54, 0xB);
    }

    private static void SetDexSpecies(SAV5B2W2 sav, ushort species, int regionSize)
    {
        var pk5 = new PK5 {Species = species, TID16 = 1337}; // non-shiny
        pk5.Gender = pk5.GetSaneGender();

        sav.SetBoxSlotAtIndex(pk5, 0);

        CheckFlags(sav, species, regionSize);
    }

    private static void CheckFlags(SAV5B2W2 sav, ushort species, int regionSize)
    {
        var dex = sav.Blocks.Zukan;
        var data = dex.Data;

        var bit = species - 1;
        var value = (byte) (1 << (bit & 7));
        var offset = bit >> 3;

        // Check the regular flag regions.
        var span = data[0x08..];
        span[offset].Should().Be(value, "caught flag");
        span[offset + regionSize].Should().Be(value, "seen flag");
        span[offset + regionSize + (regionSize * 4)].Should().Be(value, "displayed flag");
    }

    private static void CheckDexFlags5(SAV5B2W2 sav, ushort species, byte form, int regionSize, int formRegionSize)
    {
        var dex = sav.Blocks.Zukan;
        var data = dex.Data;

        var fc = sav.Personal[species].FormCount;
        var bit = sav.Zukan.DexFormIndexFetcher(species, fc);
        if (bit < 0)
            return;
        bit += form;
        var value = (byte)(1 << (bit & 7));
        var offset = bit >> 3;

        // Check the form flag regions.
        var span = data[(0x08 + (regionSize * 9))..];
        span[offset].Should().Be(value, "seen flag");
        span[offset + (formRegionSize * 2)].Should().Be(value, "displayed flag");
    }
}

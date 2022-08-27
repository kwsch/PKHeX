using System;
using Xunit;
using FluentAssertions;
using PKHeX.Core;

namespace PKHeX.Tests.Saves;

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

    private static void SetDexSpecies(SaveFile sav, ushort species, int regionSize)
    {
        var pk5 = new PK5 {Species = species, TID = 1337}; // non-shiny
        pk5.Gender = pk5.GetSaneGender();

        sav.SetBoxSlotAtIndex(pk5, 0);

        CheckFlags(sav, species, regionSize);
    }

    private static void CheckFlags(SaveFile sav, ushort species, int regionSize)
    {
        var dex = sav.PokeDex;
        var data = sav.Data;

        var bit = species - 1;
        var value = (byte) (1 << (bit & 7));
        var offset = bit >> 3;

        // Check the regular flag regions.
        var span = data.AsSpan(dex + 0x08);
        span[offset].Should().Be(value, "caught flag");
        span[offset + regionSize].Should().Be(value, "seen flag");
        span[offset + regionSize + (regionSize * 4)].Should().Be(value, "displayed flag");
    }

    private static void CheckDexFlags5(SaveFile sav, ushort species, byte form, int regionSize, int formRegionSize)
    {
        var dex = sav.PokeDex;
        var data = sav.Data;

        int fc = sav.Personal[species].FormCount;
        var bit = ((SAV5)sav).Zukan.DexFormIndexFetcher(species, fc);
        if (bit < 0)
            return;
        bit += form;
        var value = (byte)(1 << (bit & 7));
        var offset = bit >> 3;

        // Check the form flag regions.
        var span = data.AsSpan(dex + 0x08 + (regionSize * 9));
        span[offset].Should().Be(value, "seen flag");
        span[offset + (formRegionSize * 2)].Should().Be(value, "displayed flag");
    }
}

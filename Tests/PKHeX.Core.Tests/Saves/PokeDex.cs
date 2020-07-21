using Xunit;
using FluentAssertions;
using PKHeX.Core;

namespace PKHeX.Tests.Saves
{
    public static class PokeDex
    {
        [Theory]
        [InlineData(Species.Bulbasaur)]
        [InlineData(Species.Voltorb)]
        [InlineData(Species.Genesect)]
        public static void Gen5(Species species)
        {
            var bw = new SAV5B2W2();
            SetDexSpecies(bw, (int)species, 0x54);
        }

        [Theory]
        [InlineData(Species.Landorus)]
        public static void Gen5Form(Species species)
        {
            var bw = new SAV5B2W2();
            SetDexSpecies(bw, (int)species, 0x54);
            CheckDexFlags5(bw, (int)species, 0, 0x54, 0xB);
        }

        private static void SetDexSpecies(SaveFile sav, int species, int regionSize)
        {
            var pk5 = new PK5 {Species = species, TID = 1337}; // non-shiny
            pk5.Gender = pk5.GetSaneGender();

            sav.SetBoxSlotAtIndex(pk5, 0);

            CheckFlags(sav, species, regionSize);
        }

        private static void CheckFlags(SaveFile sav, int species, int regionSize)
        {
            var dex = sav.PokeDex;
            var data = sav.Data;

            var bit = species - 1;
            var val = (byte) (1 << (bit & 7));
            var ofs = bit >> 3;
            data[dex + 0x08 + ofs].Should().Be(val, "caught flag");
            data[dex + 0x08 + regionSize + ofs].Should().Be(val, "seen flag");
            data[dex + 0x08 + regionSize + (regionSize * 4) + ofs].Should().Be(val, "displayed flag");
        }

        private static void CheckDexFlags5(SaveFile sav, int species, int form, int regionSize, int formRegionSize)
        {
            var dex = sav.PokeDex;
            var data = sav.Data;

            var formDex = dex + 8 + (regionSize * 9);

            int fc = sav.Personal[species].FormeCount;
            var bit = ((SAV5)sav).Zukan.DexFormIndexFetcher(species, fc);
            if (bit < 0)
                return;
            bit += form;
            var val = (byte)(1 << (bit & 7));
            var ofs = bit >> 3;
            data[formDex + ofs].Should().Be(val, "seen flag");
            data[formDex + ofs + (formRegionSize * 2)].Should().Be(val, "displayed flag");
        }
    }
}

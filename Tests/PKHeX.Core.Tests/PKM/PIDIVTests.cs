using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests.PKM;

public class PIDIVTest
{
    // Note: IVs are stored HP/ATK/DEF/SPE/SPA/SPD

    [Fact]
    public void PIDIVMatchingTest3()
    {
        // Method 1/2/4
        var pk1 = new PK3 {PID = 0xE97E0000, IVs = [17, 19, 20, 16, 13, 12]};
        var ga1 = MethodFinder.Analyze(pk1);
        ga1.Type.Should().Be(PIDType.Method_1);
        var pk2 = new PK3 {PID = 0x5271E97E, IVs = [02, 18, 03, 12, 22, 24]};
        MethodFinder.Analyze(pk2).Type.Should().Be(PIDType.Method_2);
        var pk4 = new PK3 {PID = 0x31B05271, IVs = [02, 18, 03, 05, 30, 11]};
        MethodFinder.Analyze(pk4).Type.Should().Be(PIDType.Method_4);

        var gk1 = new PK3();
        PIDGenerator.SetValuesFromSeed(gk1, ga1.Type, ga1.OriginSeed);
        gk1.PID.Should().Be(pk1.PID);
        gk1.IVs.SequenceEqual(pk1.IVs).Should().BeTrue();
    }

    [Fact]
    public void PIDIVMatchingTest3Unown()
    {
        // Method 1/2/4, reversed for Unown.
        var pk1U = new PK3 {PID = 0x815549A2, IVs = [02, 26, 30, 30, 11, 26], Species = 201}; // Unown-C
        MethodFinder.Analyze(pk1U).Type.Should().Be(PIDType.Method_1_Unown);
        var pk2U = new PK3 {PID = 0x8A7B5190, IVs = [14, 02, 21, 30, 29, 15], Species = 201}; // Unown-M
        MethodFinder.Analyze(pk2U).Type.Should().Be(PIDType.Method_2_Unown);
        var pk4U = new PK3 {PID = 0x5FA80D70, IVs = [02, 06, 03, 26, 04, 19], Species = 201}; // Unown-A
        MethodFinder.Analyze(pk4U).Type.Should().Be(PIDType.Method_4_Unown);
    }

    [Fact]
    public void PIDIVMatchingTest3Method3()
    {
        // Method 3
        var m3R = new PK3 { PID = 0x3DD1BB49, IVs = [23, 12, 31, 09, 03, 03], Species = 001 }; // Regular
        MethodFinder.Analyze(m3R).Type.Should().Be(PIDType.Method_3);
        // Reversed for Unown
        var m3u = new PK3 { PID = 0xBB493DD1, IVs = [23, 12, 31, 09, 03, 03], Species = 201 }; // Unown
        MethodFinder.Analyze(m3u).Type.Should().Be(PIDType.Method_3_Unown);
    }

    [Fact]
    public void PIDIVMatchingTest3MiscCXD()
    {
        // Colosseum / XD
        var pk3 = new PK3 {PID = 0x0985A297, IVs = [06, 01, 00, 07, 17, 07]};
        var pv = MethodFinder.Analyze(pk3);
        pv.Type.Should().Be(PIDType.CXD);

        var gk3 = new PK3();
        PIDGenerator.SetValuesFromSeed(gk3, PIDType.CXD, pv.OriginSeed);
        gk3.PID.Should().Be(pk3.PID);
        gk3.IVs.SequenceEqual(pk3.IVs).Should().BeTrue();
    }

    [Fact]
    public void PIDIVMatchingTest3MiscChannel()
    {
        // Channel Jirachi
        var pkC = new PK3 {PID = 0x264750D9, IVs = [06, 31, 14, 27, 05, 27], SID16 = 45819, OriginalTrainerGender = 1, Version = GameVersion.R};
        var pv = MethodFinder.Analyze(pkC);
        pv.Type.Should().Be(PIDType.Channel);

        var gkC = new PK3();
        PIDGenerator.SetValuesFromSeed(gkC, PIDType.Channel, pv.OriginSeed);
        gkC.PID.Should().Be(pkC.PID);
        gkC.IVs.SequenceEqual(pkC.IVs).Should().BeTrue();
    }

    [Fact]
    public void PIDIVMatchingTest3Event()
    {
        // Restricted: TID16/SID16 are zero.
        var pkR = new PK3 {PID = 0x0000E97E, IVs = [17, 19, 20, 16, 13, 12]};
        MethodFinder.Analyze(pkR).Type.Should().Be(PIDType.BACD_R);

        // Restricted Antishiny: PID is incremented 2 times to lose shininess.
        var pkRA = new PK3 {PID = 0x0000E980, IVs = [17, 19, 20, 16, 13, 12], TID16 = 01337, SID16 = 60486};
        MethodFinder.Analyze(pkRA).Type.Should().Be(PIDType.BACD_R_A);

        // Unrestricted: TID16/SID16 are zero.
        var pkU = new PK3 {PID = 0x67DBFC33, IVs = [12, 25, 27, 30, 02, 31]};
        MethodFinder.Analyze(pkU).Type.Should().Be(PIDType.BACD_U);

        // Unrestricted Antishiny: PID is incremented 5 times to lose shininess.
        var pkUA = new PK3 {PID = 0x67DBFC38, IVs = [12, 25, 27, 30, 02, 31], TID16 = 01337, SID16 = 40657};
        MethodFinder.Analyze(pkUA).Type.Should().Be(PIDType.BACD_U_A);

        // berry fix zigzagoon: seed 0x0020
        var pkRS = new PK3 {PID = 0x38CA4EA0, IVs = [00, 20, 28, 11, 19, 00], TID16 = 30317, SID16 = 00000};
        var a_pkRS = MethodFinder.Analyze(pkRS);
        a_pkRS.Type.Should().Be(PIDType.BACD_R_S);
        a_pkRS.OriginSeed.Should().Be(0x0020);

        var gkRS = new PK3 { TID16 = 30317, SID16 = 00000 };
        PIDGenerator.SetValuesFromSeed(gkRS, PIDType.BACD_R_S, a_pkRS.OriginSeed);
        gkRS.PID.Should().Be(pkRS.PID);
        gkRS.IVs.SequenceEqual(pkRS.IVs).Should().BeTrue();

        // Unrestricted Antishiny nyx
        var nyxUA = new PK3 {PID = 0xBD3DF676, IVs = [00, 15, 05, 04, 21, 05], TID16 = 80, SID16 = 0};
        var nyx_pkUA = MethodFinder.Analyze(nyxUA);
        nyx_pkUA.Type.Should().Be(PIDType.BACD_U_AX);
    }

    [Fact]
    public void PIDIVMatchingTest4()
    {
        // Cute Charm: Male Bulbasaur
        var pkCC = new PK4 {PID = 0x00000037, IVs = [16, 13, 12, 02, 18, 03], Species = 1, Gender = 0};
        MethodFinder.Analyze(pkCC).Type.Should().Be(PIDType.CuteCharm);

        // Antishiny Mystery Gift: TID16/SID16 are zero. Original PID of 0x5271E97E is rerolled.
        var pkASR = new PK4 {PID = 0x07578CB7, IVs = [16, 13, 12, 02, 18, 03]};
        MethodFinder.Analyze(pkASR).Type.Should().Be(PIDType.G4MGAntiShiny);

        // Chain Shiny: TID16/SID16 are zero.
        var pkCS = new PK4 {PID = 0xA9C1A9C6, IVs = [22, 14, 23, 24, 11, 04]};
        MethodFinder.Analyze(pkCS).Type.Should().Be(PIDType.ChainShiny);
    }

    [Fact]
    public void PIDIVMatchingTest5()
    {
        // Shiny Mystery Gift PGF; IVs are unrelated.
        var pkS5 = new PK5 {PID = 0xBEEF0037, TID16 = 01337, SID16 = 48097};
        MethodFinder.Analyze(pkS5).Type.Should().Be(PIDType.G5MGShiny);
    }

    [Fact]
    public void PIDIVPokeSpotTest()
    {
        // XD PokeSpots: Check all 3 Encounter Slots (examples are one for each location).
        var pkPS0 = new PK3 { PID = 0x7B2D9DA7 }; // Zubat (Cave)
        MethodFinder.GetPokeSpotSeedFirst(pkPS0, 0).Type.Should().Be(PIDType.PokeSpot); // PokeSpot encounter info mismatch (Common)

        var pkPS1 = new PK3 { PID = 0x3EE9AF66 }; // Gligar (Rock)
        MethodFinder.GetPokeSpotSeedFirst(pkPS1, 1).Type.Should().Be(PIDType.PokeSpot); // PokeSpot encounter info mismatch (Uncommon)

        var pkPS2 = new PK3 { PID = 0x9B667F3C }; // Surskit (Oasis)
        MethodFinder.GetPokeSpotSeedFirst(pkPS2, 2).Type.Should().Be(PIDType.PokeSpot); // PokeSpot encounter info mismatch (Rare)
    }

    [Theory]
    [InlineData(30, 31, 31, 14, 31, 31, 0x28070031, 24, (int)Species.Pikachu, PokewalkerCourse4.YellowForest, PokewalkerSeedType.NoStroll)]
    public void PokewalkerIVTest(uint hp, uint atk, uint def, uint spA, uint spD, uint spE, uint seed, ushort expect, ushort species, PokewalkerCourse4 course, PokewalkerSeedType type)
    {
        Span<uint> tmp = stackalloc uint[LCRNG.MaxCountSeedsIV];
        var result = PokewalkerRNG.GetFirstSeed(species, course, tmp, hp, atk, def, spA, spD, spE);
        result.Type.Should().Be(type);
        result.PriorPoke.Should().Be(expect);
        result.Seed.Should().Be(seed);
    }

    [Fact]
    public void PIDIVPokewalkerTest()
    {
        PK4[] fakes =
        [
            new PK4 { Species = 025, PID = 0x34000089, TID16 = 20790, SID16 = 39664, Gender = 0}, // Pikachu
            new PK4 { Species = 025, PID = 0x7DFFFF60, TID16 = 30859, SID16 = 63760, Gender = 1}, // Pikachu
            new PK4 { Species = 025, PID = 0x7DFFFF65, TID16 = 30859, SID16 = 63760, Gender = 1}, // Pikachu
            new PK4 { Species = 025, PID = 0x7E000003, TID16 = 30859, SID16 = 63760, Gender = 1}, // Pikachu

            new PK4 { Species = 025, PID = 0x2100008F, TID16 = 31526, SID16 = 42406, Gender = 0}, // Pikachu
            new PK4 { Species = 025, PID = 0x71FFFF5A, TID16 = 49017, SID16 = 12807, Gender = 1}, // Pikachu
            new PK4 { Species = 025, PID = 0xC0000001, TID16 = 17398, SID16 = 31936, Gender = 1}, // Pikachu
            new PK4 { Species = 025, PID = 0x2FFFFF5E, TID16 = 27008, SID16 = 42726, Gender = 1}, // Pikachu
            new PK4 { Species = 025, PID = 0x59FFFFFE, TID16 = 51223, SID16 = 28044, Gender = 0}, // Pikachu
        ];
        foreach (var pk in fakes)
            MethodFinder.Analyze(pk).Type.Should().Be(PIDType.Pokewalker);
    }

    [Theory]
    [InlineData(0x00001234, 0x4DCB, 0xE161)]
    [InlineData(0x00005678, 0x734D, 0xC596)]
    public void Method1(uint seed, ushort rand0, ushort rand1)
    {
        uint first = (uint)(rand0 << 16);
        uint second = (uint)(rand1 << 16);
        Span<uint> seeds = stackalloc uint[LCRNG.MaxCountSeedsPID];
        int count = LCRNGReversal.GetSeeds(seeds, first, second);
        count.Should().NotBe(0);

        seeds[..count].IndexOf(seed).Should().NotBe(-1);
    }

    [Fact]
    public void PIDIVMethod4IVs()
    {
        var pk4 = new PK3 { PID = 0xFEE73213, IVs = [03, 29, 23, 30, 28, 24] };
        var analysis = MethodFinder.Analyze(pk4);
        analysis.Type.Should().Be(PIDType.Method_4);

        // See if any origin seed for the IVs matches what we expect
        // Load the IVs
        var iv32 = pk4.IV32;
        uint rand1 = iv32 & 0x7FFF; // HP/ATK/DEF
        uint rand3 = (iv32 >> 15) & 0x7FFF; // SPE/SPA/SPD

        Span<uint> seeds = stackalloc uint[LCRNG.MaxCountSeedsIV];
        int count = LCRNGReversalSkip.GetSeedsIVs(seeds, rand1 << 16, rand3 << 16);
        // Seeds need to be unrolled twice to account for the 2 PID rolls before IVs.

        var reg = seeds[..count];
        var index = reg.IndexOf(LCRNG.Next2(0x48FBAA42u));
        index.Should().NotBe(-1);
    }

    [Fact]
    public void PIDIVSearchEuclid()
    {
        const uint seed = 0x2E15555E;
        const uint rand0 = 0x20AD96A9;
        const uint rand1 = 0x7E1DBEC8;

        XDRNG.MaxCountSeedsIV.Should().BeGreaterThan(XDRNG.MaxCountSeedsPID);

        Span<uint> seeds = stackalloc uint[XDRNG.MaxCountSeedsIV];
        var cp = XDRNG.GetSeeds(seeds, rand0 & 0xFFFF0000, rand1 & 0xFFFF0000);
        var p = seeds[..cp];
        p.IndexOf(seed).Should().NotBe(-1);

        var ci = XDRNG.GetSeedsIVs(seeds, rand0 & 0x7FFF0000, rand1 & 0x7FFF0000);
        var i = seeds[..ci];
        i.IndexOf(seed).Should().NotBe(-1);
    }
}

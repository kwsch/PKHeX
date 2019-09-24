using System.Linq;
using FluentAssertions;
using PKHeX.Core;
using Xunit;

namespace PKHeX.Tests.PKM
{
    public class PIDIVTest
    {
        // Note: IVs are stored HP/ATK/DEF/SPE/SPA/SPD

        [Fact]
        public void PIDIVMatchingTest3()
        {
            // Method 1/2/4
            var pk1 = new PK3 {PID = 0xE97E0000, IVs = new[] {17, 19, 20, 16, 13, 12}};
            var ga1 = MethodFinder.Analyze(pk1);
            Assert.Equal(PIDType.Method_1, ga1.Type);
            var pk2 = new PK3 {PID = 0x5271E97E, IVs = new[] {02, 18, 03, 12, 22, 24}};
            Assert.Equal(PIDType.Method_2, MethodFinder.Analyze(pk2).Type);
            var pk4 = new PK3 {PID = 0x31B05271, IVs = new[] {02, 18, 03, 05, 30, 11}};
            Assert.Equal(PIDType.Method_4, MethodFinder.Analyze(pk4).Type);

            var gk1 = new PK3();
            PIDGenerator.SetValuesFromSeed(gk1, ga1.Type, ga1.OriginSeed);
            Assert.Equal(pk1.PID, gk1.PID);
            Assert.True(gk1.IVs.SequenceEqual(pk1.IVs), "Unable to match generated PID to Method 1 IVs");
        }

        [Fact]
        public void PIDIVMatchingTest3Unown()
        {
            // Method 1/2/4, reversed for Unown.
            var pk1U = new PK3 {PID = 0x815549A2, IVs = new[] {02, 26, 30, 30, 11, 26}, Species = 201}; // Unown-C
            Assert.Equal(PIDType.Method_1_Unown, MethodFinder.Analyze(pk1U).Type);
            var pk2U = new PK3 {PID = 0x8A7B5190, IVs = new[] {14, 02, 21, 30, 29, 15}, Species = 201}; // Unown-M
            Assert.Equal(PIDType.Method_2_Unown, MethodFinder.Analyze(pk2U).Type);
            var pk4U = new PK3 {PID = 0x5FA80D70, IVs = new[] {02, 06, 03, 26, 04, 19}, Species = 201}; // Unown-A
            Assert.Equal(PIDType.Method_4_Unown, MethodFinder.Analyze(pk4U).Type);
        }

        [Fact]
        public void PIDIVMatchingTest3Method3()
        {
            // Method 3, reversed for Unown.
            var m3R = new PK3 { PID = 0x3DD1BB49, IVs = new[] { 23, 12, 31, 09, 03, 03 }, Species = 001 }; // Regular
            var m3t = MethodFinder.Analyze(m3R).Type;
            Assert.Equal(PIDType.Method_3, m3t);
            var m3u = new PK3 { PID = 0xBB493DD1, IVs = new[] { 23, 12, 31, 09, 03, 03 }, Species = 201 }; // Unown
            var u3t = MethodFinder.Analyze(m3u).Type;
            Assert.Equal(PIDType.Method_3_Unown, u3t);
        }

        [Fact]
        public void PIDIVMatchingTest3Misc()
        {
            // Colosseum / XD
            var pk3 = new PK3 {PID = 0x0985A297, IVs = new[] {06, 01, 00, 07, 17, 07}};
            var ak3 = MethodFinder.Analyze(pk3);
            Assert.Equal(PIDType.CXD, MethodFinder.Analyze(pk3).Type);

            var gk3 = new PK3();
            PIDGenerator.SetValuesFromSeed(gk3, PIDType.CXD, ak3.OriginSeed);
            Assert.Equal(pk3.PID, gk3.PID);
            Assert.True(pk3.IVs.SequenceEqual(gk3.IVs), "Unable to match generated IVs to CXD spread");

            // Channel Jirachi
            var pkC = new PK3 {PID = 0x264750D9, IVs = new[] {06, 31, 14, 27, 05, 27}, SID = 45819, OT_Gender = 1, Version = (int)GameVersion.R};
            var akC = MethodFinder.Analyze(pkC);
            Assert.Equal(PIDType.Channel,akC.Type);

            var gkC = new PK3();
            PIDGenerator.SetValuesFromSeed(gkC, PIDType.Channel, akC.OriginSeed);
            Assert.Equal(pkC.PID, gkC.PID);
            Assert.True(pkC.IVs.SequenceEqual(gkC.IVs), "Unable to match generated IVs to Channel spread");
        }

        [Fact]
        public void PIDIVMatchingTest3Event()
        {
            // Restricted: TID/SID are zero.
            var pkR = new PK3 {PID = 0x0000E97E, IVs = new[] {17, 19, 20, 16, 13, 12}};
            Assert.Equal(PIDType.BACD_R, MethodFinder.Analyze(pkR).Type);

            // Restricted Antishiny: PID is incremented 2 times to lose shininess.
            var pkRA = new PK3 {PID = 0x0000E980, IVs = new[] {17, 19, 20, 16, 13, 12}, TID = 01337, SID = 60486};
            Assert.Equal(PIDType.BACD_R_A, MethodFinder.Analyze(pkRA).Type);

            // Unrestricted: TID/SID are zero.
            var pkU = new PK3 {PID = 0x67DBFC33, IVs = new[] {12, 25, 27, 30, 02, 31}};
            Assert.Equal(PIDType.BACD_U, MethodFinder.Analyze(pkU).Type);

            // Unrestricted Antishiny: PID is incremented 5 times to lose shininess.
            var pkUA = new PK3 {PID = 0x67DBFC38, IVs = new[] {12, 25, 27, 30, 02, 31}, TID = 01337, SID = 40657};
            Assert.Equal(PIDType.BACD_U_A, MethodFinder.Analyze(pkUA).Type);

            // berry fix zigzagoon: seed 0x0020
            var pkRS = new PK3 {PID = 0x38CA4EA0, IVs = new[] {00, 20, 28, 11, 19, 00}, TID = 30317, SID = 00000};
            var a_pkRS = MethodFinder.Analyze(pkRS);
            Assert.Equal(PIDType.BACD_R_S, a_pkRS.Type);
            Assert.True(0x0020 == a_pkRS.OriginSeed, "Unable to match PID to BACD-R shiny spread origin seed");

            var gkRS = new PK3 { TID = 30317, SID = 00000 };
            PIDGenerator.SetValuesFromSeed(gkRS, PIDType.BACD_R_S, a_pkRS.OriginSeed);
            Assert.Equal(pkRS.PID, gkRS.PID);
            Assert.True(pkRS.IVs.SequenceEqual(gkRS.IVs), "Unable to match generated IVs to BACD-R shiny spread");

            // Unrestricted Antishiny nyx
            var nyxUA = new PK3 {PID = 0xBD3DF676, IVs = new[] {00, 15, 05, 04, 21, 05}, TID = 80, SID = 0};
            var nyx_pkUA = MethodFinder.Analyze(nyxUA);
            Assert.Equal(PIDType.BACD_U_AX, nyx_pkUA.Type);
        }

        [Fact]
        public void PIDIVMatchingTest4()
        {
            // Cute Charm: Male Bulbasaur
            var pkCC = new PK4 {PID = 0x00000037, IVs = new[] {16, 13, 12, 02, 18, 03}, Species = 1, Gender = 0};
            Assert.Equal(PIDType.CuteCharm, MethodFinder.Analyze(pkCC).Type);

            // Antishiny Mystery Gift: TID/SID are zero. Original PID of 0x5271E97E is rerolled.
            var pkASR = new PK4 {PID = 0x07578CB7, IVs = new[] {16, 13, 12, 02, 18, 03}};
            Assert.Equal(PIDType.G4MGAntiShiny, MethodFinder.Analyze(pkASR).Type);

            // Chain Shiny: TID/SID are zero.
            var pkCS = new PK4 {PID = 0xA9C1A9C6, IVs = new[] {22, 14, 23, 24, 11, 04}};
            Assert.Equal(PIDType.ChainShiny, MethodFinder.Analyze(pkCS).Type);
        }

        [Fact]
        public void PIDIVMatchingTest5()
        {
            // Shiny Mystery Gift PGF; IVs are unrelated.
            var pkS5 = new PK5 {PID = 0xBEEF0037, TID = 01337, SID = 48097};
            Assert.Equal(PIDType.G5MGShiny, MethodFinder.Analyze(pkS5).Type);
        }

        [Fact]
        public void PIDIVPokeSpotTest()
        {
            // XD PokeSpots: Check all 3 Encounter Slots (examples are one for each location).
            var pkPS0 = new PK3 { PID = 0x7B2D9DA7 }; // Zubat (Cave)
            Assert.True(MethodFinder.GetPokeSpotSeeds(pkPS0, 0).Any(), "PokeSpot encounter info mismatch (Common)");
            var pkPS1 = new PK3 { PID = 0x3EE9AF66 }; // Gligar (Rock)
            Assert.True(MethodFinder.GetPokeSpotSeeds(pkPS1, 1).Any(), "PokeSpot encounter info mismatch (Uncommon)");
            var pkPS2 = new PK3 { PID = 0x9B667F3C }; // Surskit (Oasis)
            Assert.True(MethodFinder.GetPokeSpotSeeds(pkPS2, 2).Any(), "PokeSpot encounter info mismatch (Rare)");
        }

        [Fact]
        public void PIDIVPokewalkerTest()
        {
            var pkPW = new[]
            {
                new PK4 { Species = 025, PID = 0x34000089, TID = 20790, SID = 39664, Gender = 0}, // Pikachu
                new PK4 { Species = 025, PID = 0x7DFFFF60, TID = 30859, SID = 63760, Gender = 1}, // Pikachu
                new PK4 { Species = 025, PID = 0x7DFFFF65, TID = 30859, SID = 63760, Gender = 1}, // Pikachu
                new PK4 { Species = 025, PID = 0x7E000003, TID = 30859, SID = 63760, Gender = 1}, // Pikachu

                new PK4 { Species = 025, PID = 0x2100008F, TID = 31526, SID = 42406, Gender = 0}, // Pikachu
                new PK4 { Species = 025, PID = 0x71FFFF5A, TID = 49017, SID = 12807, Gender = 1}, // Pikachu
                new PK4 { Species = 025, PID = 0xC0000001, TID = 17398, SID = 31936, Gender = 1}, // Pikachu
                new PK4 { Species = 025, PID = 0x2FFFFF5E, TID = 27008, SID = 42726, Gender = 1}, // Pikachu
                new PK4 { Species = 025, PID = 0x59FFFFFE, TID = 51223, SID = 28044, Gender = 0}, // Pikachu
            };
            foreach (var pk in pkPW)
                Assert.Equal(PIDType.Pokewalker, MethodFinder.Analyze(pk).Type);
        }

        [Fact]
        public void PIDIVEncounterSlotTest()
        {
            // Modest Method 1
            var pk = new PK3 {PID = 0x6937DA48, IVs = new[] {31, 31, 31, 31, 31, 31}};
            var pidiv = MethodFinder.Analyze(pk);
            Assert.Equal(PIDType.Method_1, pidiv.Type);

            // Test for Method J
            {
                // Pearl
                pk.Version = (int) GameVersion.P;
                var results = FrameFinder.GetFrames(pidiv, pk).ToArray();

                var failSync = results.Where(z => z.Lead == LeadRequired.SynchronizeFail);
                var noSync = results.Where(z => z.Lead == LeadRequired.None);
                var sync = results.Where(z => z.Lead == LeadRequired.Synchronize);

                failSync.Should().HaveCount(1);
                sync.Should().HaveCount(37);
                noSync.Should().HaveCount(2);

                const SlotType type = SlotType.Grass;
                var slots = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 9 };
                Assert.True(slots.All(s => results.Any(z => z.GetSlot(type) == s)), "Required slots not present.");
                var slotsForType = results.Where(z => !z.LevelSlotModified).Select(z => z.GetSlot(type)).Distinct().OrderBy(z => z);
                Assert.True(slotsForType.SequenceEqual(slots), "Unexpected slots present.");
            }
            // Test for Method H and K
            {
                // Sapphire
                // pk.Version = (int)GameVersion.S;
                // var results = FrameFinder.GetFrames(pidiv, pk);
            }
        }

        [Fact]
        public void PIDIVMethod4IVs()
        {
            var pk4 = new PK3 { PID = 0xFEE73213, IVs = new[] { 03, 29, 23, 30, 28, 24 } };
            var analysis = MethodFinder.Analyze(pk4);
            analysis.Type.Should().Be(PIDType.Method_4);

            // See if any origin seed for the IVs matches what we expect
            // Load the IVs
            uint rand1 = 0; // HP/ATK/DEF
            uint rand3 = 0; // SPE/SPA/SPD
            var IVs = pk4.IVs;
            for (int i = 0; i < 3; i++)
            {
                rand1 |= (uint)IVs[i] << (5 * i);
                rand3 |= (uint)IVs[i+3] << (5 * i);
            }
            Assert.Contains(MethodFinder.GetSeedsFromIVsSkip(RNG.LCRNG, rand1, rand3), z => z == 0xFEE7047C);
        }

        [Fact]
        public void PIDIVSearchEuclid()
        {
            const uint seed = 0x2E15555E;
            const uint rand0 = 0x20AD96A9;
            const uint rand1 = 0x7E1DBEC8;
            var pidseeds = MethodFinder.GetSeedsFromPIDEuclid(RNG.XDRNG, rand0 >> 16,            rand1 >> 16);
            var ivseeds = MethodFinder.GetSeedsFromIVsEuclid(RNG.XDRNG, (rand0 >> 16) & 0x7FFF, (rand1 >> 16) & 0x7FFF);
            Assert.Contains(pidseeds, z => z == seed);
            Assert.Contains(ivseeds, z => z == seed);
        }
    }
}

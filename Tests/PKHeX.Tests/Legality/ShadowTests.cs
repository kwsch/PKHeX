using Microsoft.VisualStudio.TestTools.UnitTesting;
using PKHeX.Core;

namespace PKHeX.Tests.Legality
{
    [TestClass]
    public class ShadowTests
    {
        private const string LegalityValidCategory = "Shadow Lock Validity Tests";

        [TestMethod]
        [TestCategory(LegalityValidCategory)]
        public void VerifyLock1()
        {
            // Zubat (F) (Serious)
            Verify(Encounters3Teams.Poochyena, 0xAF4E3161, new[] { 11, 29, 25, 6, 23, 10 });

            // Murkrow (M) (Docile)
            Verify(Encounters3Teams.Pineco, 0xC3A0F1E5, new[] { 30, 3, 9, 10, 27, 30 });
        }

        [TestMethod]
        [TestCategory(LegalityValidCategory)]
        public void VerifyLock2()
        {
            // Goldeen (F) (Serious)
            // Horsea (M) (Quirky)
            Verify(Encounters3Teams.Spheal, 0xA459BF44, new[] { 0, 11, 4, 28, 6, 13 });

            // Kirlia (M) (Hardy)
            // Linoone (F) (Hardy)
            Verify(Encounters3Teams.Natu, 0x8E14DAB6, new[] { 29, 24, 30, 16, 3, 18 });

            // Remoraid (M) (Docile)
            // Golbat (M) (Bashful)
            VerifySingle(Encounters3Teams.Roselia, 0x30E87CC7, new[] { 22, 11, 8, 26, 4, 29 });
        }

        [TestMethod]
        [TestCategory(LegalityValidCategory)]
        public void VerifyLock3()
        {
            // Luvdisc (F) (Docile)
            // Beautifly (M) (Hardy)
            // Roselia (M) (Quirky)
            Verify(Encounters3Teams.Delcatty, 0x9BECA2A6, new[] { 31, 31, 25, 13, 22, 1 });

            // Kadabra (M) (Docile)
            // Sneasel (F) (Hardy)
            // Misdreavus (F) (Bashful)
            Verify(Encounters3Teams.Meowth, 0x77D87601, new[] { 10, 27, 26, 13, 30, 19 });

            // Ralts (M) (Docile)
            // Voltorb (-) (Hardy)
            // Bagon (F) (Quirky)
            VerifySingle(Encounters3Teams.Numel, 0x37F95B26, new[] { 11, 8, 5, 10, 28, 14 });
        }

        [TestMethod]
        [TestCategory(LegalityValidCategory)]
        public void VerifyLock4()
        {
            // Ninetales (M) (Serious)
            // Jumpluff (M) (Docile)
            // Azumarill (F) (Hardy)
            // Shadow Tangela
            VerifySingle(Encounters3Teams.Butterfree, 0x2E49AC34, new[] { 15, 24, 7, 2, 11, 2 });

            // Huntail (M) (Docile)
            // Cacturne (F) (Hardy)
            // Weezing (F) (Serious)
            // Ursaring (F) (Bashful)
            VerifySingle(Encounters3Teams.Arbok, 0x1973FD07, new[] { 13, 30, 3, 16, 20, 9 });

            // Lairon (F) (Bashful)
            // Sealeo (F) (Serious)
            // Slowking (F) (Docile)
            // Ursaring (M) (Quirky)
            VerifySingle(Encounters3Teams.Primeape, 0x33893D4C, new[] { 26, 25, 24, 28, 29, 30 });
        }

        [TestMethod]
        [TestCategory(LegalityValidCategory)]
        public void VerifyLock5()
        {
            // many prior, all non shadow
            VerifySingle(Encounters3Teams.Seedot, 0x8CBD29DB, new[] { 19, 29, 30, 0, 7, 2 });
        }

        private static void Verify(TeamLock[] teams, uint pid, int[] ivs, bool xd = true)
        {
            var pk3 = new PK3 { PID = pid, IVs = ivs };
            var info = MethodFinder.Analyze(pk3);
            Assert.AreEqual(PIDType.CXD, info.Type, "Unable to match PID to CXD spread!");
            bool match = GetCanOriginateFrom(teams, info, xd, out var _);
            Assert.IsTrue(match, "Unable to verify lock conditions: " + teams[0].Species);
        }

        private static void VerifySingle(TeamLock[] teams, uint pid, int[] ivs, bool xd = true)
        {
            var pk3 = new PK3 { PID = pid, IVs = ivs };
            var info = MethodFinder.Analyze(pk3);
            Assert.AreEqual(PIDType.CXD, info.Type, "Unable to match PID to CXD spread!");
            bool match = LockFinder.IsFirstShadowLockValid(info, teams, xd);
            Assert.IsTrue(match, "Unable to verify lock conditions: " + teams[0].Species);
        }

        /// <summary>
        /// Checks if the PIDIV can originate from
        /// </summary>
        /// <param name="possibleTeams"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        private static bool GetCanOriginateFrom(TeamLock[] possibleTeams, PIDIV info, bool XD, out uint origin)
        {
            foreach (var team in possibleTeams)
            {
                var result = LockFinder.FindLockSeed(info.OriginSeed, team.Locks, XD, out origin);
                if (result)
                    return true;
            }
            origin = 0;
            return false;
        }
    }
}

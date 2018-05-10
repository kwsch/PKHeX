using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PKHeX.Core;

namespace PKHeX.Tests.Simulator
{
    [TestClass]
    public class ShowdownSetTests
    {
        private const string SimulatorParse = "Set Parsing Tests";

        [TestMethod]
        [TestCategory(SimulatorParse)]
        public void SimulatorGetParse()
        {
            var set = new ShowdownSet(SetGlaceonUSUMTutor);
            string Sanitize(string str) => str.Replace("\r\n", "").Replace("\n", "");
            Assert.IsTrue(Sanitize(SetGlaceonUSUMTutor) == Sanitize(set.Text));
        }

        [TestMethod]
        [TestCategory(SimulatorParse)]
        public void SimulatorGetEncounters()
        {
            var set = new ShowdownSet(SetGlaceonUSUMTutor);
            var pk7 = new PK7 {Species = set.Species, AltForm = set.FormIndex, Moves = set.Moves};
            var encs = EncounterMovesetGenerator.GenerateEncounters(pk7, set.Moves, GameVersion.MN);
            Assert.IsTrue(!encs.Any());
            pk7.HT_Name = "PKHeX";
            encs = EncounterMovesetGenerator.GenerateEncounters(pk7, set.Moves, GameVersion.MN);
            var first = encs.FirstOrDefault();
            Assert.IsTrue(first != null);

            var egg = (EncounterEgg)first;
            var info = new SimpleTrainerInfo();
            var pk = egg.ConvertToPKM(info);
            Assert.IsTrue(pk.Species != set.Species);

            var la = new LegalityAnalysis(pk);
            Assert.IsTrue(la.Valid);

            var test = EncounterMovesetGenerator.GeneratePKMs(pk7, info).ToList();
            foreach (var t in test)
            {
                var la2 = new LegalityAnalysis(t);
                Assert.IsTrue(la2.Valid);
            }
        }

        //[TestMethod]
        //[TestCategory(SimulatorParse)]
        public void TestGenerate()
        {
            int count = 0;
            var tr = new SimpleTrainerInfo();
            for (int i = 1; i <= 807; i++)
            {
                var pk = new PK7 { Species = i };
                pk.Gender = pk.GetSaneGender();
                var ez = EncounterMovesetGenerator.GeneratePKMs(pk, tr);
                Debug.WriteLine($"Starting {i:000}");
                foreach (var e in ez)
                {
                    var la = new LegalityAnalysis(e);
                    Assert.IsTrue(la.Valid);
                    count++;
                }
                Debug.WriteLine($"Finished {i:000}");
            }
            Debug.WriteLine($"Generated {count} PKMs!");
        }

        private const string SetGlaceonUSUMTutor =
@"Glaceon (F) @ Assault Vest
IVs: 0 Atk
EVs: 252 HP / 252 SpA / 4 SpD
Ability: Ice Body
Level: 100
Shiny: Yes
Modest Nature
- Blizzard
- Water Pulse
- Shadow Ball
- Hyper Voice";

    }
}

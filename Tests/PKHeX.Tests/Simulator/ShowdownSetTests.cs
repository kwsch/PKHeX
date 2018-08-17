using System;
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

        static ShowdownSetTests()
        {
            if (!EncounterEvent.Initialized)
                EncounterEvent.RefreshMGDB();
        }

        [TestMethod]
        [TestCategory(SimulatorParse)]
        public void SimulatorGetParse()
        {
            foreach (var setstr in Sets)
            {
                var set = new ShowdownSet(setstr).Text;
                var lines = set.Split('\n').Select(z => z.Trim());
                Assert.IsTrue(lines.All(z => setstr.Contains(z)), setstr);
            }
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

        [TestMethod]
        [TestCategory(SimulatorParse)]
        public void SimulatorGetWC3()
        {
            var set = new ShowdownSet(SetROCKSMetang);
            var pk3 = new PK3 { Species = set.Species, AltForm = set.FormIndex, Moves = set.Moves };
            var encs = EncounterMovesetGenerator.GenerateEncounters(pk3, set.Moves, GameVersion.R);
            Assert.IsTrue(encs.Any());
            encs = EncounterMovesetGenerator.GenerateEncounters(pk3, set.Moves, GameVersion.R);
            var first = encs.FirstOrDefault();
            Assert.IsTrue(first != null);

            var wc3 = (WC3)first;
            var info = new SimpleTrainerInfo();
            var pk = wc3.ConvertToPKM(info);

            var la = new LegalityAnalysis(pk);
            Assert.IsTrue(la.Valid);
        }

        [TestMethod]
        [TestCategory(SimulatorParse)]
        public void SimulatorGetCelebi()
        {
            var set = new ShowdownSet(SetCelebi);
            var pk7 = new PK7 { Species = set.Species, AltForm = set.FormIndex, Moves = set.Moves };
            var encs = EncounterMovesetGenerator.GenerateEncounters(pk7, set.Moves, GameVersion.X);
            Assert.IsTrue(encs.Any());
            encs = EncounterMovesetGenerator.GenerateEncounters(pk7, set.Moves, GameVersion.X);
            var first = encs.FirstOrDefault();
            Assert.IsTrue(first != null);

            var enc = first;
            var info = new SimpleTrainerInfo();
            var pk = enc.ConvertToPKM(info);

            var la = new LegalityAnalysis(pk);
            Assert.IsTrue(la.Valid);
        }

        [TestMethod]
        [TestCategory(SimulatorParse)]
        public void SimulatorGetSmeargle()
        {
            var set = new ShowdownSet(SetSmeargle);
            var pk7 = new PK7 { Species = set.Species, AltForm = set.FormIndex, Moves = set.Moves };
            var encs = EncounterMovesetGenerator.GenerateEncounters(pk7, set.Moves, GameVersion.MN);
            Assert.IsTrue(encs.Any());
            encs = EncounterMovesetGenerator.GenerateEncounters(pk7, set.Moves, GameVersion.MN);
            var first = encs.FirstOrDefault();
            Assert.IsTrue(first != null);

            var enc = first;
            var info = new SimpleTrainerInfo();
            var pk = enc.ConvertToPKM(info);

            var la = new LegalityAnalysis(pk);
            Assert.IsTrue(la.Valid);
        }

        [TestMethod]
        [TestCategory(SimulatorParse)]
        public void SimulatorParseMultiple()
        {
            var text = string.Join("\r\n\r\n", Sets);
            var lines = text.Split(new[] {"\r\n", "\n"}, StringSplitOptions.None);
            var sets = ShowdownSet.GetShowdownSets(lines);
            Assert.IsTrue(sets.Count() == Sets.Length);

            sets = ShowdownSet.GetShowdownSets(Enumerable.Empty<string>());
            Assert.IsTrue(!sets.Any());

            sets = ShowdownSet.GetShowdownSets(new [] {"", "   ", " "});
            Assert.IsTrue(!sets.Any());
        }

        //[TestMethod]
        //[TestCategory(SimulatorParse)]
        public void TestGenerate()
        {
            int count = 0;
            var tr = new SimpleTrainerInfo();
            for (int i = 1; i < 807; i++)
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

        private const string SetROCKSMetang =
@"Metang
IVs: 20 HP / 3 Atk / 26 Def / 1 SpA / 6 SpD / 8 Spe
Ability: Clear Body
Level: 30
Adamant Nature
- Take Down
- Confusion
- Metal Claw
- Refresh";

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

        private const string SetSmeargle =
@"Smeargle @ Focus Sash
Ability: Own Tempo
EVs: 248 HP / 8 Def / 252 Spe
Jolly Nature
- Sticky Web
- Nuzzle
- Taunt
- Whirlwind";

        private const string SetCelebi =
@"Celebi @ Toxic Orb
Ability: Natural Cure
Jolly Nature
- Recover
- Heal Bell
- Safeguard
- Hold Back";

        private const string SetNicknamedTypeNull =
@"Reliance (Type: Null) @ Eviolite
EVs: 252 HP / 4 Def / 252 SpD
Ability: Battle Armor
Careful Nature
- Facade
- Swords Dance
- Sleep Talk
- Rest";

        private static readonly string[] Sets =
        {
            SetGlaceonUSUMTutor,
            SetNicknamedTypeNull,

@"Greninja @ Choice Specs
Ability: Battle Bond
EVs: 252 SpA / 4 SpD / 252 Spe
Timid Nature
- Hydro Pump
- Spikes
- Water Shuriken
- Dark Pulse",
        };
    }
}

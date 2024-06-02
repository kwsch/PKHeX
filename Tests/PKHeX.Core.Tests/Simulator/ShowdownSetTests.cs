using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests.Simulator;

public class ShowdownSetTests
{
    [Fact]
    public void SimulatorGetParse()
    {
        foreach (ReadOnlySpan<char> setstr in Sets)
        {
            var set = new ShowdownSet(setstr).GetSetLines();
            foreach (var line in set)
                setstr.Contains(line, StringComparison.Ordinal).Should().BeTrue($"Line {line} should be in the set {setstr}");
        }
    }

    [Fact]
    public void SimulatorGetEncounters()
    {
        var set = new ShowdownSet(SetGlaceonUSUMTutor);
        var pk7 = new PK7 {Species = set.Species, Form = set.Form, Moves = set.Moves};
        var encounters = EncounterMovesetGenerator.GenerateEncounters(pk7, set.Moves, GameVersion.MN);
        Assert.False(encounters.Any());
        pk7.HandlingTrainerName = "PKHeX";
        encounters = EncounterMovesetGenerator.GenerateEncounters(pk7, set.Moves, GameVersion.MN);
        var first = encounters.FirstOrDefault();
        Assert.NotNull(first);

        var egg = (EncounterEgg)first;
        var info = new SimpleTrainerInfo(GameVersion.SN);
        var pk = egg.ConvertToPKM(info);
        Assert.True(pk.Species != set.Species);

        var la = new LegalityAnalysis(pk);
        la.Valid.Should().BeTrue($"Encounter should have generated legally: {egg} {la.Report()}");

        var test = EncounterMovesetGenerator.GenerateEncounters(pk7, info, set.Moves).ToList();
        for (var i = 0; i < test.Count; i++)
        {
            var t = test[i];
            var convert = t.ConvertToPKM(info);
            var la2 = new LegalityAnalysis(convert);
            la2.Valid.Should().BeTrue($"Encounter {i} should have generated legally: {t} {la2.Report()}");
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData(Vivillon3DS.FancyFormID)]
    public void SimGetVivillonPostcardSV(byte form)
    {
        var pk9 = new PK9 { Species = (int)Species.Vivillon, Form = form };
        var moves = ReadOnlyMemory<ushort>.Empty;
        var encounters = EncounterMovesetGenerator.GenerateEncounters(pk9, moves, GameVersion.SL);
        encounters.OfType<EncounterSlot9>().Should().NotBeEmpty();
    }

    [Fact]
    public void SimulatorGetWC3()
    {
        var set = new ShowdownSet(SetROCKSMetang);
        var pk3 = new PK3 { Species = set.Species, Form = set.Form, Moves = set.Moves };
        var encs = EncounterMovesetGenerator.GenerateEncounters(pk3, set.Moves, GameVersion.R);
        Assert.True(encs.Any());
        encs = EncounterMovesetGenerator.GenerateEncounters(pk3, set.Moves, GameVersion.R);
        var first = encs.FirstOrDefault();
        Assert.NotNull(first);

        var wc3 = (WC3)first;
        var info = new SimpleTrainerInfo(GameVersion.R);
        var pk = wc3.ConvertToPKM(info);

        var la = new LegalityAnalysis(pk);
        Assert.True(la.Valid);
    }

    [Fact]
    public void SimulatorGetCelebi()
    {
        var set = new ShowdownSet(SetCelebi);
        var pk7 = new PK7 { Species = set.Species, Form = set.Form, Moves = set.Moves };
        var encs = EncounterMovesetGenerator.GenerateEncounters(pk7, set.Moves, GameVersion.X);
        Assert.True(encs.Any());
        encs = EncounterMovesetGenerator.GenerateEncounters(pk7, set.Moves, GameVersion.X);
        var first = encs.FirstOrDefault();
        Assert.NotNull(first);

        var info = new SimpleTrainerInfo(GameVersion.SN);
        var pk = first.ConvertToPKM(info);

        var la = new LegalityAnalysis(pk);
        Assert.True(la.Valid);
    }

    [Fact]
    public void SimulatorGetSplitBreed()
    {
        var set = new ShowdownSet(SetMunchSnorLax);
        var pk7 = new PK7 { Species = set.Species, Form = set.Form, Moves = set.Moves, HandlingTrainerName = "PKHeX" }; // !! specify the HT name, we need tutors for this one
        var encs = EncounterMovesetGenerator.GenerateEncounters(pk7, set.Moves, GameVersion.SN).ToList();
        Assert.True(encs.Count > 0);
        Assert.True(encs.All(z => z.Species > 150));

        var info = new SimpleTrainerInfo(GameVersion.SN);
        var enc = encs[0];
        var pk = enc.ConvertToPKM(info);

        var la = new LegalityAnalysis(pk);
        Assert.True(la.Valid);
    }

    [Fact]
    public void SimulatorGetVCEgg1()
    {
        var set = new ShowdownSet(SetSlowpoke12);
        var pk7 = new PK7 { Species = set.Species, Form = set.Form, Moves = set.Moves, HandlingTrainerName = "PKHeX" };
        var encs = EncounterMovesetGenerator.GenerateEncounters(pk7, set.Moves, GameVersion.GD).ToList();
        Assert.True(encs.Count > 0);

        var info = new SimpleTrainerInfo(GameVersion.SN);
        var enc = encs[0];
        var pk = enc.ConvertToPKM(info);

        var la = new LegalityAnalysis(pk);
        Assert.True(la.Valid);
    }

    [Fact]
    public void SimulatorGetSmeargle()
    {
        var set = new ShowdownSet(SetSmeargle);
        var pk7 = new PK7 { Species = set.Species, Form = set.Form, Moves = set.Moves };
        var encs = EncounterMovesetGenerator.GenerateEncounters(pk7, set.Moves, GameVersion.MN);
        Assert.True(encs.Any());
        encs = EncounterMovesetGenerator.GenerateEncounters(pk7, set.Moves, GameVersion.MN);
        var first = encs.FirstOrDefault();
        Assert.NotNull(first);

        var info = new SimpleTrainerInfo(GameVersion.SN);
        var pk = first.ConvertToPKM(info);

        var la = new LegalityAnalysis(pk);
        Assert.True(la.Valid);
    }

    [Fact]
    public void SimulatorParseMultiple()
    {
        var text = string.Join("\r\n\r\n", Sets);
        var sets = ShowdownParsing.GetShowdownSets(text);
        Assert.True(sets.Count() == Sets.Length);
    }

    [Fact]
    public void SimulatorParseEmpty()
    {
        string[] lines = ["", "   ", " "];
        var sets = ShowdownParsing.GetShowdownSets(lines);
        Assert.False(sets.Any());
    }

    [Theory]
    [InlineData(SetDuplicateMoves, 3)]
    public void SimulatorParseDuplicate(string text, int moveCount)
    {
        var set = new ShowdownSet(text);
        var result = set.Moves.AsSpan();
        var actual = result.Length - result.Count<ushort>(0);
        actual.Should().Be(moveCount);
    }

    [Theory]
    [InlineData(LowLevelElectrode)]
    public void SimulatorParseEncounter(string text)
    {
        var set = new ShowdownSet(text);
        var pk7 = new PK3 { Species = set.Species, Form = set.Form, Moves = set.Moves, CurrentLevel = set.Level };
        var encs = EncounterMovesetGenerator.GenerateEncounters(pk7, set.Moves);
        var tr3 = encs.First(z => z is EncounterTrade3);
        var pk3 = tr3.ConvertToPKM(new SimpleTrainerInfo(GameVersion.FR));

        var la = new LegalityAnalysis(pk3);
        la.Valid.Should().BeTrue(la.Report());
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(256)]
    public void BadFriendshipNotParsed(int value)
    {
        string input = $@"Eevee\nFriendship: {value}";
        var set = new ShowdownSet(input);
        value.Should().NotBe(set.Friendship);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(101)]
    public void BadLevelNotParsed(int value)
    {
        string input = $@"Eevee\nLevel: {value}";
        var set = new ShowdownSet(input);
        value.Should().NotBe(set.Level);
    }

    private const string LowLevelElectrode =
        @"BOLICHI (Electrode)
IVs: 19 HP / 16 Atk / 18 Def / 25 SpA / 19 SpD / 25 Spe
Ability: Static
Level: 3
Hasty Nature
- Charge
- Tackle
- Screech
- Sonic Boom";

    private const string SetDuplicateMoves =
        @"Kingler-Gmax @ Master Ball
Ability: Sheer Force
Shiny: Yes
EVs: 252 Atk / 4 SpD / 252 Spe
Jolly Nature
- Crabhammer
- Rock Slide
- Rock Slide
- X-Scissor";

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

    private const string SetMunchSnorLax =
        @"Snorlax @ Choice Band
Ability: Thick Fat
Level: 50
EVs: 84 HP / 228 Atk / 180 Def / 12 SpD / 4 Spe
Adamant Nature
- Double-Edge
- High Horsepower
- Self-Destruct
- Fire Punch";

    private const string SetSlowpoke12 =
        @"Threat (Slowpoke) @ Eviolite
Ability: Regenerator
Shiny: Yes
EVs: 248 HP / 252 Atk / 8 SpD
Adamant Nature
- Body Slam
- Earthquake
- Belly Drum
- Iron Tail";

    private static readonly string[] Sets =
    [
        SetGlaceonUSUMTutor,
        SetNicknamedTypeNull,
        SetMunchSnorLax,

        @"Greninja @ Choice Specs
Ability: Battle Bond
EVs: 252 SpA / 4 SpD / 252 Spe
Timid Nature
- Hydro Pump
- Spikes
- Water Shuriken
- Dark Pulse",
    ];
}

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
        var settings = new BattleTemplateExportSettings(BattleTemplateConfig.CommunityStandard);

        foreach (var setstr in Sets)
        {
            var set = new ShowdownSet(setstr).GetSetLines(settings);
            foreach (var line in set)
                setstr.Contains(line, StringComparison.Ordinal).Should().BeTrue($"`{line}` should be in the set: {setstr}");
        }
    }

    [Fact]
    public void SimulatorGetEncounters()
    {
        // Set must have visited US/UM. Sun/Moon origin can't obtain the moves unless traded.
        var set = new ShowdownSet(SetGlaceonUSUMTutor);
        var pk7 = new PK7 {Species = set.Species, Form = set.Form};
        var encounters = EncounterMovesetGenerator.GenerateEncounters(pk7, set.Moves, GameVersion.MN);
        Assert.False(encounters.Any());

        // Mark as traded, to allow tutor moves to be recognized as valid.
        // Find the first egg, generate, won't be evolved yet.
        pk7.HandlingTrainerName = TrainerName.ProgramINT;
        encounters = EncounterMovesetGenerator.GenerateEncounters(pk7, set.Moves, GameVersion.MN);
        var egg = encounters.OfType<EncounterEgg7>().FirstOrDefault();
        Assert.NotNull(egg);

        var trainer = new SimpleTrainerInfo(GameVersion.SN);
        var pk = egg.ConvertToPKM(trainer);
        Assert.True(pk.Species != set.Species);

        // Ensure was legally generated.
        var la = new LegalityAnalysis(pk);
        la.Valid.Should().BeTrue($"Encounter should have generated legally: {egg} {la.Report()}");

        // Check all possible encounters.
        var test = EncounterMovesetGenerator.GenerateEncounters(pk7, trainer, set.Moves).ToList();
        for (var i = 0; i < test.Count; i++)
        {
            var t = test[i];
            var convert = t.ConvertToPKM(trainer);
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
    public void SimulatorGetGift3()
    {
        var set = new ShowdownSet(SetROCKSMetang);
        var pk3 = new PK3 { Species = set.Species, Form = set.Form, Moves = set.Moves };
        var encs = EncounterMovesetGenerator.GenerateEncounters(pk3, set.Moves, GameVersion.R);
        Assert.True(encs.Any());
        encs = EncounterMovesetGenerator.GenerateEncounters(pk3, set.Moves, GameVersion.R);
        var first = encs.FirstOrDefault();
        Assert.NotNull(first);

        var gift = (EncounterGift3)first;
        var info = new SimpleTrainerInfo(GameVersion.R);
        var pk = gift.ConvertToPKM(info);

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
        var pk7 = new PK7 { Species = set.Species, Form = set.Form, Moves = set.Moves, HandlingTrainerName = TrainerName.ProgramINT }; // !! specify the HT name, we need tutors for this one
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
        var pk7 = new PK7 { Species = set.Species, Form = set.Form, Moves = set.Moves, HandlingTrainerName = TrainerName.ProgramINT };
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
    [InlineData(SetAllTokenExample)]
    public void SimulatorTranslate(string message, string languageOriginal = "en")
    {
        var settingsOriginal = new BattleTemplateExportSettings(BattleTemplateConfig.CommunityStandard, languageOriginal);
        if (!ShowdownParsing.TryParseAnyLanguage(message, out var set))
            throw new Exception("Input failed");

        var all = BattleTemplateLocalization.GetAll();
        foreach (var l in all)
        {
            var languageTarget = l.Key;
            if (languageTarget == languageOriginal)
                continue;

            var exportSettings = new BattleTemplateExportSettings(languageTarget);
            var translated = set.GetText(exportSettings);
            translated.Should().NotBeNullOrEmpty();
            translated.Should().NotBe(message);

            // Convert back, should be 1:1
            if (!ShowdownParsing.TryParseAnyLanguage(translated, out var set2))
                throw new Exception($"{languageTarget} parse failed");
            set2.InvalidLines.Should().BeEmpty();
            set2.Species.Should().Be(set.Species);
            set2.Form.Should().Be(set.Form);

            var result = set2.GetText(settingsOriginal);
            result.Should().Be(message);
        }
    }

    [Fact]
    public void StatNamesNoSubstring()
    {
        var all = BattleTemplateLocalization.GetAll();
        foreach (var l in all)
        {
            var languageTarget = l.Key;
            var x = l.Value.Config;

            CheckSubstring(x.StatNames.Names, languageTarget);
            CheckSubstring(x.StatNamesFull.Names, languageTarget);
        }

        static void CheckSubstring(ReadOnlySpan<string> statNames, string languageTarget)
        {
            // ensure no stat name is a substring of another
            for (int i = 0; i < statNames.Length; i++)
            {
                var name = statNames[i];
                for (int j = 0; j < statNames.Length; j++)
                {
                    if (i == j)
                        continue;
                    var other = statNames[j];
                    if (other.Contains(name) || name.Contains(other))
                        throw new Exception($"Stat name {name} is a substring of {other} in {languageTarget}");
                }
            }
        }
    }

    [Theory]
    [InlineData(SetAllTokenExample)]
    public void SimulatorTranslateHABCDS(string message, string languageOriginal = "en")
    {
        var settingsOriginal = new BattleTemplateExportSettings(BattleTemplateConfig.CommunityStandard, languageOriginal);
        if (!ShowdownParsing.TryParseAnyLanguage(message, out var set))
            throw new Exception("Input failed");

        var target = new BattleTemplateExportSettings("ja")
        {
            StatsIVs = StatDisplayStyle.HABCDS,
            StatsEVs = StatDisplayStyle.HABCDS,
        };

        var translated = set.GetText(target);
        translated.Should().NotBeNullOrEmpty();
        translated.Should().NotBe(message);

        // Convert back, should be 1:1
        if (!ShowdownParsing.TryParseAnyLanguage(translated, out var set2))
            throw new Exception("ja parse failed");
        set2.InvalidLines.Should().BeEmpty();
        set2.Species.Should().Be(set.Species);
        set2.Form.Should().Be(set.Form);

        var result = set2.GetText(settingsOriginal);
        result.Should().Be(message);
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
        var encs = EncounterMovesetGenerator.GenerateEncounters(pk7, set.Moves, GameVersion.FR);
        var tr3 = encs.OfType<EncounterTrade3>().First();
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
        """
        BOLICHI (Electrode)
        IVs: 19 HP / 16 Atk / 18 Def / 25 SpA / 19 SpD / 25 Spe
        Ability: Static
        Level: 3
        Hasty Nature
        - Charge
        - Tackle
        - Screech
        - Sonic Boom
        """;

    private const string SetDuplicateMoves =
        """
        Kingler-Gmax @ Master Ball
        Ability: Sheer Force
        Shiny: Yes
        EVs: 252 Atk / 4 SpD / 252 Spe
        Jolly Nature
        - Crabhammer
        - Rock Slide
        - Rock Slide
        - X-Scissor
        """;

    private const string SetROCKSMetang =
        """
        Metang
        IVs: 20 HP / 3 Atk / 26 Def / 1 SpA / 6 SpD / 8 Spe
        Ability: Clear Body
        Level: 30
        Adamant Nature
        - Take Down
        - Confusion
        - Metal Claw
        - Refresh
        """;

    private const string SetGlaceonUSUMTutor =
        """
        Glaceon (F) @ Assault Vest
        IVs: 0 Atk
        EVs: 252 HP / 252 SpA / 4 SpD
        Ability: Ice Body
        Shiny: Yes
        Modest Nature
        - Blizzard
        - Water Pulse
        - Shadow Ball
        - Hyper Voice
        """;

    private const string SetAllTokenExample =
        """
        Pikachu (F) @ Oran Berry
        Ability: Static
        Level: 69
        Shiny: Yes
        Friendship: 42
        Dynamax Level: 3
        Gigantamax: Yes
        EVs: 12 HP / 5 Atk / 6 Def / 17 SpA / 4 SpD / 101 Spe
        Quirky Nature
        IVs: 30 HP / 22 Atk / 29 Def / 7 SpA / 1 SpD / 0 Spe
        - Pound
        - Sky Attack
        - Hyperspace Fury
        - Metronome
        """;

    private const string SetSmeargle =
        """
        Smeargle @ Focus Sash
        Ability: Own Tempo
        EVs: 248 HP / 8 Def / 252 Spe
        Jolly Nature
        - Sticky Web
        - Nuzzle
        - Taunt
        - Whirlwind
        """;

    private const string SetCelebi =
        """
        Celebi @ Toxic Orb
        Ability: Natural Cure
        Jolly Nature
        - Recover
        - Heal Bell
        - Safeguard
        - Hold Back
        """;

    private const string SetNicknamedTypeNull =
        """
        Reliance (Type: Null) @ Eviolite
        EVs: 252 HP / 4 Def / 252 SpD
        Ability: Battle Armor
        Careful Nature
        - Facade
        - Swords Dance
        - Sleep Talk
        - Rest
        """;

    private const string SetMunchSnorLax =
        """
        Snorlax @ Choice Band
        Ability: Thick Fat
        Level: 50
        EVs: 84 HP / 228 Atk / 180 Def / 12 SpD / 4 Spe
        Adamant Nature
        - Double-Edge
        - High Horsepower
        - Self-Destruct
        - Fire Punch
        """;

    private const string SetSlowpoke12 =
        """
        Threat (Slowpoke) @ Eviolite
        Ability: Regenerator
        Shiny: Yes
        EVs: 248 HP / 252 Atk / 8 SpD
        Adamant Nature
        - Body Slam
        - Earthquake
        - Belly Drum
        - Iron Tail
        """;

    private static readonly string[] Sets =
    [
        SetGlaceonUSUMTutor,
        SetNicknamedTypeNull,
        SetMunchSnorLax,

        """
        Greninja-Ash @ Choice Specs
        Ability: Battle Bond
        EVs: 252 SpA / 4 SpD / 252 Spe
        Timid Nature
        - Hydro Pump
        - Spikes
        - Water Shuriken
        - Dark Pulse
        """,
    ];
}

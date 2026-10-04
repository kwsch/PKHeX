using FluentAssertions;
using PKHeX.Core;
using PKHeX.Web.Services;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// Drafting the species and form: choices come from Core's species list for the save and the species' form list, the change and
/// its dependent fields are Core's <see cref="SpeciesFormChange.ChangeSpeciesForm(PKM,ushort,byte,IPersonalTable)"/> byte for byte, a preview
/// reports exactly what the change would do without making it, refusals are never clamped, changing back leaves the draft clean, and a party
/// member's stats are recalculated without healing it.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class SpeciesFormDraftTests
{
    private const int Burn = 0x10;

    /// <summary>Offsets of the fields a species or form change may write in the PK6 structure.</summary>
    private static readonly int[] DependentOffsets =
    [
        0x08, 0x09, // species
        0x10, 0x11, 0x12, 0x13, // experience points
        0x14, 0x15, // ability and slot number
        0x1D, // fateful encounter flag, gender and form
        .. Enumerable.Range(0x40, 0x1A), // nickname
    ];

    internal const ushort Zigzagoon = (ushort)Species.Zigzagoon;
    private const ushort Linoone = (ushort)Species.Linoone, Magikarp = (ushort)Species.Magikarp, Chansey = (ushort)Species.Chansey, Magnemite = (ushort)Species.Magnemite;
    private const ushort Charizard = (ushort)Species.Charizard, Pikachu = (ushort)Species.Pikachu, Kyogre = (ushort)Species.Kyogre, Meowstic = (ushort)Species.Meowstic;

    private static EditorDraft Open(byte[] bytes) => SaveFixtures.Open(bytes).Select(SlotRef.InBox(0, 1));

    private static EditorDraft Draft(bool oras = true, Action<PK6>? change = null) => Open(AbilityGenderDraftTests.Boxed(oras, change));

    /// <summary>The stored entity changed by Core's species and form change, as the draft should hold it.</summary>
    private static PK6 Native(PK6 stored, ushort species, byte form, IPersonalTable personal)
    {
        var expected = (PK6)stored.Clone();
        expected.ChangeSpeciesForm(species, form, personal);
        return expected;
    }

    /// <summary>The offsets at which the draft differs from <paramref name="stored"/>.</summary>
    private static IEnumerable<int> ChangedOffsets(EditorDraft draft, PK6 stored)
    {
        var data = draft.Preview().Data.ToArray();
        return Enumerable.Range(0, data.Length).Where(i => data[i] != stored.Data[i]);
    }

    /// <summary>A party member at 7 HP with a burn and a stored Attack Core would not calculate, so a heal, a cleared status or a missed recalculation would show.</summary>
    private static byte[] InjuredAndBurned(bool oras) => SaveFixtures.Synthetic(oras, customize: SaveFixtures.WithPartyMember("Leader", p =>
    {
        p.Stat_HPCurrent = 7;
        p.Status_Condition = Burn;
        p.Stat_ATK = 1;
    }));

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void TheReadersShowTheStoredSpeciesAndForm(bool oras)
    {
        var draft = Draft(oras);
        var stored = draft.Preview();

        draft.Species.Should().Be(stored.Species).And.Be(Zigzagoon);
        draft.Form.Should().Be(0);
        draft.FormChoices.Should().BeEmpty("Zigzagoon has no alternate forms in Generation 6");
        draft.Capabilities.SpeciesChoices.Should().Contain(s => s.Value == Zigzagoon && s.Text == GameInfo.Strings.specieslist[Zigzagoon]);
        draft.Inspect().Identity.Species.Value.Should().Be(draft.Species);
    }

    [Fact]
    public void TheFormChoicesAreCoresFormListMarkedBattleOnlyAndNotInGame()
    {
        var oras = Draft(change: p => p.Species = Charizard);
        var strings = GameInfo.Strings;
        var names = FormConverter.GetFormList(Charizard, strings.types, strings.forms, GameInfo.GenderSymbolUnicode, EntityContext.Gen6);

        oras.FormChoices.Select(f => f.Name).Should().Equal(names, "the names are Core's");
        oras.FormChoices.Select(f => (f.Form, f.BattleOnly, f.InGame)).Should().Equal(((byte)0, false, true), ((byte)1, true, true), ((byte)2, true, true));
        oras.FormChoices.Should().BeSameAs(oras.FormChoices, "the list is kept until the species changes");

        Draft(change: p => p.Species = Pikachu).FormChoices.Should().HaveCount(7, "Omega Ruby and Alpha Sapphire have cosplay Pikachu");
        Draft(false, p => p.Species = Pikachu).FormChoices.Should().BeEmpty("X and Y give Pikachu no form choice, as the desktop's form box shows none");
        Draft(change: p => p.Species = Kyogre).FormChoices.Should().ContainSingle(f => f.BattleOnly).Which.Form.Should().Be(1);
        Draft(false, p => p.Species = Kyogre).FormChoices.Should().BeEmpty();
        Draft(false, p => p.Species = (ushort)Species.Scatterbug).FormChoices.Where(f => !f.InGame).Select(f => f.Form)
            .Should().Equal([(byte)18, (byte)19], "Core's list names the Fancy and Poké Ball patterns, which X and Y's personal data lack for Scatterbug");
    }

    [Theory]
    [InlineData(false, Linoone)]
    [InlineData(true, Linoone)]
    [InlineData(false, Magikarp)]
    [InlineData(true, Chansey)]
    [InlineData(true, Magnemite)]
    [InlineData(true, Pikachu)]
    public void ASpeciesChangeMatchesNativeCoreAndWritesOnlyItsDependentFields(bool oras, ushort species)
    {
        var draft = Draft(oras, p => p.RefreshAbility(2));
        var stored = draft.Preview();
        var expected = Native(stored, species, 0, draft.Capabilities.Personal);

        draft.EditSpeciesForm(species, 0);

        draft.Preview().Data.ToArray().Should().Equal(expected.Data.ToArray());
        ChangedOffsets(draft, stored).Should().BeSubsetOf(DependentOffsets);
        var drafted = draft.Preview();
        (drafted.PID, drafted.EncryptionConstant, drafted.IsShiny, drafted.Nature).Should().Be((stored.PID, stored.EncryptionConstant, stored.IsShiny, stored.Nature), "the PID is never changed");
        draft.Species.Should().Be(species);
        draft.AbilitySlot.Should().Be(2, "the slot is kept");
        draft.Ability.Should().Be(drafted.PersonalInfo.AbilityH);
        draft.GenderChoices.Should().Contain(draft.Gender, "the gender is one the species can have");
        draft.Inspect().Identity.Species.Value.Should().Be(species);
    }

    [Theory]
    [InlineData(Linoone)]
    [InlineData(Magikarp)]
    [InlineData(Chansey)]
    [InlineData(Magnemite)]
    [InlineData(Charizard)]
    public void APreviewReportsCoresFlagsAndValuesWithoutChangingTheDraft(ushort species)
    {
        var draft = Draft();
        var stored = draft.Preview();
        var expected = (PK6)stored.Clone();
        var flags = expected.ChangeSpeciesForm(species, 0, draft.Capabilities.Personal);

        var preview = draft.PreviewSpeciesForm(species, 0);

        preview.Changes.Should().Be(flags, "the preview lists the fields Core reports changed");
        preview.Before.Should().BeEquivalentTo(SpeciesFormValues.Of(stored, party: false));
        preview.After.Should().BeEquivalentTo(SpeciesFormValues.Of(expected, party: false));
        preview.SpeciesChanged.Should().BeTrue();
        preview.IsNoChange.Should().BeFalse();
        preview.Restores.Should().BeFalse();
        preview.HpChange.Should().BeNull("a boxed Pokémon has no HP");
        (preview.BattleOnly, preview.InGame).Should().Be((false, true));
        draft.IsDirty.Should().BeFalse("a preview changes nothing");
        draft.EditRevision.Should().Be(0, "a preview is not an edit, so legality stays current");

        draft.EditSpeciesForm(species, 0);
        draft.Preview().Data.ToArray().Should().Equal(expected.Data.ToArray(), "the change is the one previewed");
    }

    [Fact]
    public void AGrowthRateChangeKeepsTheLevelTheExperienceGivesAndSetsItsStart()
    {
        // Zigzagoon grows Medium Fast and Magikarp Slow, so the same experience points are a lower level for Magikarp.
        var draft = Draft(change: p => p.EXP = Experience.GetEXP(50, p.PersonalInfo.EXPGrowth) + 10);
        var experience = draft.Experience;
        var growth = PersonalTable.AO[Magikarp].EXPGrowth;
        var level = Experience.GetLevel(experience, growth);

        var preview = draft.PreviewSpeciesForm(Magikarp, 0);

        preview.LevelChanged.Should().BeTrue();
        (preview.Before.Level, preview.After.Level).Should().Be(((byte)50, level));
        preview.After.Experience.Should().Be(Experience.GetEXP(level, growth));
        preview.Changes.Should().HaveFlag(SpeciesFormChangeResult.EXP);
    }

    [Fact]
    public void AFormChangeMarksABattleOnlyForm()
    {
        var draft = Draft(change: p => p.Species = Charizard);
        var stored = draft.Preview();

        var preview = draft.PreviewSpeciesForm(Charizard, 1);

        preview.BattleOnly.Should().BeTrue();
        preview.SpeciesChanged.Should().BeFalse();
        preview.Changes.Should().HaveFlag(SpeciesFormChangeResult.Form);
        preview.Forms.Should().BeSameAs(draft.FormChoices, "the species is unchanged");
        draft.EditSpeciesForm(Charizard, 1);
        draft.Form.Should().Be(1);
        draft.Preview().Data.ToArray().Should().Equal(Native(stored, Charizard, 1, draft.Capabilities.Personal).Data.ToArray());
    }

    [Fact]
    public void APreviewOfANewSpeciesCarriesItsForms()
    {
        var draft = Draft();

        var preview = draft.PreviewSpeciesForm(Charizard, 2);

        preview.Forms.Select(f => f.Form).Should().Equal((byte)0, (byte)1, (byte)2);
        preview.After.Form.Should().Be(2);
        preview.BattleOnly.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(722)]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void ASpeciesOutsideTheGamesListIsRefused(int species)
    {
        var draft = Draft();

        Action[] refused = [() => draft.EditSpeciesForm(species, 0), () => draft.PreviewSpeciesForm(species, 0)];

        foreach (var edit in refused)
        {
            edit.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.SpeciesNotAvailable);
        }
        draft.IsDirty.Should().BeFalse();
        draft.EditRevision.Should().Be(0);
    }

    [Theory]
    [InlineData(Charizard, 3)]
    [InlineData(Charizard, -1)]
    [InlineData(Charizard, 255)]
    [InlineData(Charizard, int.MaxValue)]
    [InlineData(Linoone, 1)]
    [InlineData(Linoone, -1)]
    [InlineData(Pikachu, 1)]
    public void AFormOutsideTheSpeciesListIsRefused(ushort species, int form)
    {
        // An X save: Pikachu has no form choice there, so its cosplay forms are refused, not reset to 0 as Core would.
        var draft = Draft(false);

        Action[] refused = [() => draft.EditSpeciesForm(species, form), () => draft.PreviewSpeciesForm(species, form)];

        foreach (var edit in refused)
        {
            edit.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.FormNotAvailable);
        }
        draft.IsDirty.Should().BeFalse();
    }

    [Fact]
    public void ANicknamedPokemonKeepsItsNameAndOneThatIsNotTakesTheNewSpeciesName()
    {
        var plain = Draft(change: p => p.Language = (int)LanguageID.French);
        plain.IsNicknamed.Should().BeFalse();

        var preview = plain.PreviewSpeciesForm(Linoone, 0);
        plain.EditSpeciesForm(Linoone, 0);

        plain.Nickname.Should().Be(SpeciesName.GetSpeciesNameGeneration(Linoone, (int)LanguageID.French, 6), "it takes the species' name in its language");
        preview.Changes.Should().HaveFlag(SpeciesFormChangeResult.Nickname);
        preview.After.Nickname.Should().Be(plain.Nickname);

        var named = Draft(change: p =>
        {
            p.Nickname = "Ziggy";
            p.IsNicknamed = true;
        });
        named.EditSpeciesForm(Linoone, 0);
        (named.Nickname, named.IsNicknamed).Should().Be(("Ziggy", true));
    }

    [Fact]
    public void ChangingBackThroughSeveralChangesLeavesTheDraftClean()
    {
        var draft = Draft(change: p =>
        {
            p.RefreshAbility(1);
            p.EXP = Experience.GetEXP(40, p.PersonalInfo.EXPGrowth) + 123;
        });

        draft.EditSpeciesForm(Magikarp, 0);
        draft.EditSpeciesForm(Charizard, 1);
        draft.EditSpeciesForm(Magnemite, 0);
        draft.IsDirty.Should().BeTrue();
        var preview = draft.PreviewSpeciesForm(Zigzagoon, 0);
        draft.EditSpeciesForm(Zigzagoon, 0);

        preview.Restores.Should().BeTrue();
        draft.IsDirty.Should().BeFalse("the experience points, ability, gender and name the run changed are given back");
        draft.EditRevision.Should().Be(4);
    }

    [Fact]
    public void ChangingBackGivesBackTheStoredNameBytes()
    {
        // A byte after the name's terminator, which Charmander's longer name overwrites: only giving back the stored bytes leaves the draft clean.
        var draft = Draft(change: p => p.NicknameTrash[(p.Nickname.Length + 1) * 2] = 0x7F);
        GameInfo.Strings.specieslist[(int)Species.Charmander].Length.Should().BeGreaterThan(draft.Nickname.Length);

        draft.EditSpeciesForm((ushort)Species.Charmander, 0);
        draft.EditSpeciesForm(Zigzagoon, 0);

        draft.IsDirty.Should().BeFalse();
    }

    [Fact]
    public void AnEditOfAChangedValueEndsTheRunAndIsKept()
    {
        var draft = Draft(change: p => p.EXP = Experience.GetEXP(40, p.PersonalInfo.EXPGrowth) + 123);
        var stored = draft.Preview();

        draft.EditSpeciesForm(Magikarp, 0);
        draft.EditExperience(draft.Experience + 7);
        var edited = draft.Preview();
        draft.EditSpeciesForm(Zigzagoon, 0);

        draft.Preview().Data.ToArray().Should().Equal(Native(edited, Zigzagoon, 0, draft.Capabilities.Personal).Data.ToArray(), "Core sets the values, as no undo applies");
        draft.Experience.Should().NotBe(stored.EXP);
    }

    [Fact]
    public void ALanguageEditEndsTheRun()
    {
        // Core names a Pokémon that is not nicknamed in its language, so after a language edit changing back must not give back the old
        // language's name: "Linoone" is kept in French as another language's name, and Zigzagoon then takes its French name.
        var draft = Draft();
        draft.IsNicknamed.Should().BeFalse();

        draft.EditSpeciesForm(Linoone, 0);
        draft.EditLanguage((int)LanguageID.French);
        draft.Nickname.Should().Be(GameInfo.Strings.specieslist[Linoone], "the name is the species' name in another language, so it is kept");
        var edited = draft.Preview();
        var preview = draft.PreviewSpeciesForm(Zigzagoon, 0);
        draft.EditSpeciesForm(Zigzagoon, 0);

        preview.Restores.Should().BeFalse();
        draft.Preview().Data.ToArray().Should().Equal(Native(edited, Zigzagoon, 0, draft.Capabilities.Personal).Data.ToArray());
        draft.Nickname.Should().Be(SpeciesName.GetSpeciesNameGeneration(Zigzagoon, (int)LanguageID.French, 6));
    }

    [Fact]
    public void AnEditOfAnotherFieldDoesNotEndTheRun()
    {
        var draft = Draft();
        var stored = draft.Preview();

        draft.EditSpeciesForm(Magikarp, 0);
        draft.EditIv(0, (stored.IV_HP + 1) % 32);
        draft.EditSpeciesForm(Zigzagoon, 0);

        ChangedOffsets(draft, stored).Should().NotIntersectWith(DependentOffsets, "the run's values are given back");
    }

    [Fact]
    public void AGenderEditAndAFormChangeShareTheRun()
    {
        var draft = Draft(change: AbilityGenderDraftTests.MaleMeowstic);
        var stored = draft.Preview();

        draft.EditGender(EntityGender.Female);
        draft.Form.Should().Be(1);
        var preview = draft.PreviewSpeciesForm(Meowstic, 0);
        draft.EditSpeciesForm(Meowstic, 0);

        preview.Restores.Should().BeTrue();
        draft.IsDirty.Should().BeFalse();

        draft.EditSpeciesForm(Meowstic, 1);
        draft.Preview().Data.ToArray().Should().Equal(Native(stored, Meowstic, 1, draft.Capabilities.Personal).Data.ToArray(), "Core sets the gender from the form");
        draft.Gender.Should().Be(EntityGender.Female);
        draft.EditGender(EntityGender.Male);
        draft.IsDirty.Should().BeFalse();
    }

    [Fact]
    public void ChoosingTheDraftedSpeciesAndFormIsNoChange()
    {
        var draft = Draft();

        var preview = draft.PreviewSpeciesForm(Zigzagoon, 0);
        draft.EditSpeciesForm(Zigzagoon, 0);

        preview.IsNoChange.Should().BeTrue();
        preview.Changes.Should().Be(SpeciesFormChangeResult.None);
        draft.IsDirty.Should().BeFalse();
        draft.EditRevision.Should().Be(1, "an accepted edit marks legality stale, even one that changes nothing");
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void APartyMemberIsRecalculatedWithoutHealingAndMatchesNativeCore(bool oras)
    {
        var source = InjuredAndBurned(oras);
        var session = SaveFixtures.Open(source);
        var draft = session.Select(SlotRef.InParty(0));
        var stored = draft.Preview();

        var preview = draft.PreviewSpeciesForm(Chansey, 0);
        draft.EditSpeciesForm(Chansey, 0);

        var drafted = draft.Preview();
        (drafted.Stat_HPCurrent, drafted.Status_Condition).Should().Be((7, Burn), "HP is never raised and the status is kept");
        drafted.Stat_HPMax.Should().BeGreaterThan(stored.Stat_HPMax, "Chansey's HP is far higher");
        drafted.Stat_ATK.Should().NotBe(1, "the stats were recalculated");
        preview.After.Stats.Should().Equal(drafted.Stat_HPMax, drafted.Stat_ATK, drafted.Stat_DEF, drafted.Stat_SPA, drafted.Stat_SPD, drafted.Stat_SPE);
        preview.Before.Stats[1].Should().Be(1, "the stats before are the stored ones");
        preview.HpChange.Should().Be(new PartyHpChange(7, 7, stored.Stat_HPMax, drafted.Stat_HPMax));
        draft.Inspect().Stats.Source.Should().Be(StatsSource.Recalculated);
        session.Apply(draft);

        var output = SaveExporter.Export(session, session.Select(SlotRef.InParty(0)));
        var native = SaveFixtures.Parse(source);
        var pk = native.GetPartySlotAtIndex(0);
        pk.ChangeSpeciesForm(Chansey, 0, native.Personal);
        pk.ResetPartyStats();
        pk.Stat_HPCurrent = 7;
        pk.Status_Condition = Burn;
        native.SetPartySlotAtIndex(pk, 0, EntityImportSettings.None);
        output.Should().Equal(native.Write().ToArray(), "the session must match the native Core edit");
    }

    [Fact]
    public void APartyMemberChangedAndChangedBackKeepsItsStoredBattleState()
    {
        var draft = SaveFixtures.Open(InjuredAndBurned(true)).Select(SlotRef.InParty(0));

        draft.EditSpeciesForm(Chansey, 0);
        draft.EditSpeciesForm(Zigzagoon, 0);

        draft.IsDirty.Should().BeFalse("the stored stats, HP and status are kept once the calculation is back where it was");
    }

    [Fact]
    public void AChangeOfAMemberStoredWithoutStatsIsRefused()
    {
        var draft = SaveFixtures.Open(PartyApplyTests.WithoutStoredStats()).Select(SlotRef.InParty(0));

        Action[] refused = [() => draft.EditSpeciesForm(Linoone, 0), () => draft.PreviewSpeciesForm(Linoone, 0)];

        foreach (var edit in refused)
        {
            edit.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.PartyStatsMissing, "a species change is a stat edit");
        }
        draft.IsDirty.Should().BeFalse();
    }

    [Fact]
    public void TheChangeIsGatedOnTheFamilyAndRefusedForEggs()
    {
        var bytes = AbilityGenderDraftTests.Boxed(change: p => p.IsEgg = true);
        var save = SaveFixtures.Parse(bytes);
        SaveSession Session(EditableFields fields) => new(bytes.ToArray(), save, "fixture.sav", SaveCapabilities.For(save, SupportMatrix.Find(save)! with { Editable = fields }));
        var gated = Session(SupportMatrix.Find(save)!.Editable & ~EditableFields.Species).Select(SaveFixtures.FirstBoxSlot);
        var egg = SaveFixtures.Open(bytes).Select(SlotRef.InBox(0, 1));

        foreach (var (draft, error) in new[] { (gated, SessionError.FieldNotEditable), (egg, SessionError.EggNotEditable) })
        {
            Action[] edits = [() => draft.EditSpeciesForm(Linoone, 0), () => draft.PreviewSpeciesForm(Linoone, 0)];
            foreach (var edit in edits)
            {
                edit.Should().Throw<SessionException>().Which.Error.Should().Be(error);
            }
            draft.IsDirty.Should().BeFalse();
        }

        var onlySpecies = Session(EditableFields.Species).Select(SaveFixtures.FirstBoxSlot);
        onlySpecies.EditSpeciesForm(Linoone, 0);
        onlySpecies.Species.Should().Be(Linoone, "the fields that follow the species change with it, as in the desktop editor");
    }
}

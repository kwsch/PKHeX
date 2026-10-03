using FluentAssertions;
using PKHeX.Core;
using PKHeX.Web.Services;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// Drafting the ability slot and the gender (WEB-PKM-009, WEB-PKM-004): the slots come from Core's personal data for the species and form,
/// the ability and its slot number are written together, genders are limited to those the species can have, Meowstic's form follows its
/// gender as the desktop changes it, refusals are never clamped, and none of these edits touches a party member's battle state unless the
/// form changes.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class AbilityGenderDraftTests
{
    private const int Burn = 0x10;

    /// <summary>Offsets of the ability and the ability slot number in the PK6 structure.</summary>
    private const int AbilityOffset = 0x14, AbilityNumberOffset = 0x15;

    /// <summary>The byte that holds the fateful encounter flag, the gender and the form.</summary>
    private const int GenderOffset = 0x1D;

    internal const ushort Meowstic = (ushort)Species.Meowstic;
    private const ushort Gastly = (ushort)Species.Gastly, Tauros = (ushort)Species.Tauros, Chansey = (ushort)Species.Chansey, Magnemite = (ushort)Species.Magnemite;

    /// <summary>A box 1, slot 2 copy of the known legal Zigzagoon, changed by <paramref name="change"/>.</summary>
    internal static byte[] Boxed(bool oras = true, Action<PK6>? change = null) =>
        SaveFixtures.Synthetic(oras, customize: SaveFixtures.WithBoxEntity(0, 1, p => change?.Invoke(p)));

    /// <summary>
    /// Changes a Pokémon into a male Meowstic with its hidden ability (Prankster), 5 experience points into level 30, so a form change shows
    /// in the form, the ability and the experience points.
    /// </summary>
    internal static void MaleMeowstic(PK6 p)
    {
        p.Species = Meowstic;
        p.Form = 0;
        p.Gender = 0;
        p.RefreshAbility(2);
        p.EXP = Experience.GetEXP(30, p.PersonalInfo.EXPGrowth) + 5;
    }

    /// <summary>Changes a Pokémon into <paramref name="species"/> with its first ability and the gender given.</summary>
    private static Action<PK6> AsSpecies(ushort species, byte gender) => p =>
    {
        p.Species = species;
        p.Form = 0;
        p.Gender = gender;
        p.RefreshAbility(0);
        p.EXP = Experience.GetEXP(p.CurrentLevel, p.PersonalInfo.EXPGrowth);
    };

    private static EditorDraft Open(byte[] bytes) => SaveFixtures.Open(bytes).Select(SlotRef.InBox(0, 1));

    private static EditorDraft Draft(bool oras = true, Action<PK6>? change = null) => Open(Boxed(oras, change));

    /// <summary>A party member at 7 HP with a burn and a stored Attack Core would not calculate, so a heal, a cleared status or a recalculation would show.</summary>
    private static byte[] InjuredAndBurned(bool oras, Action<PK6>? change = null) => SaveFixtures.Synthetic(oras, customize: SaveFixtures.WithPartyMember("Leader", p =>
    {
        if (change is not null)
        {
            change(p);
            p.ResetPartyStats();
        }
        p.Stat_HPCurrent = 7;
        p.Status_Condition = Burn;
        p.Stat_ATK = 1;
    }));

    /// <summary>Asserts the draft holds exactly the stored entity changed by <paramref name="nativeEdit"/>, so the edit wrote nothing else.</summary>
    private static void AssertMatchesNative(EditorDraft draft, PK6 stored, Action<PK6> nativeEdit)
    {
        var expected = (PK6)stored.Clone();
        nativeEdit(expected);
        draft.Preview().Data.ToArray().Should().Equal(expected.Data.ToArray());
    }

    /// <summary>The offsets at which the draft differs from <paramref name="stored"/>.</summary>
    private static IEnumerable<int> ChangedOffsets(EditorDraft draft, PK6 stored)
    {
        var data = draft.Preview().Data.ToArray();
        return Enumerable.Range(0, data.Length).Where(i => data[i] != stored.Data[i]);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void TheReadersShowTheStoredAbilityAndGender(bool oras)
    {
        var draft = Draft(oras);
        var stored = draft.Preview();
        var personal = stored.PersonalInfo;

        draft.Ability.Should().Be(stored.Ability);
        draft.AbilityNumber.Should().Be(stored.AbilityNumber);
        draft.AbilitySlot.Should().Be(stored.AbilityNumber >> 1);
        draft.AbilityChoices.Select(c => c.Value).Should().Equal(0, 1, 2);
        draft.AbilityChoices.Select(c => c.Text).Should().Equal(draft.Capabilities.Lists.GetAbilityList(personal).Select(c => c.Text), "the names are Core's");
        draft.AbilityChoices.Select(c => c.Text).Should().Equal(
            $"{GameInfo.Strings.abilitylist[personal.Ability1]} (1)", $"{GameInfo.Strings.abilitylist[personal.Ability2]} (2)", $"{GameInfo.Strings.abilitylist[personal.AbilityH]} (H)");
        draft.RegularAbilitiesSame.Should().BeFalse();
        draft.Gender.Should().Be(stored.Gender);
        draft.GenderRule.Should().Be(GenderRule.Either);
        draft.GenderChoices.Should().Equal(EntityGender.Male, EntityGender.Female);
        draft.FormFollowsGender.Should().BeFalse();

        var inspected = draft.Inspect().Identity;
        inspected.Ability.Value.Should().Be(draft.Ability);
        inspected.AbilitySlot.Should().Be(draft.AbilityNumber);
        inspected.Gender.Should().Be(draft.Gender);
    }

    [Theory]
    [InlineData(false, 0)]
    [InlineData(false, 1)]
    [InlineData(false, 2)]
    [InlineData(true, 0)]
    [InlineData(true, 1)]
    [InlineData(true, 2)]
    public void EachSlotMatchesNativeCoreAndWritesOnlyTheAbility(bool oras, int slot)
    {
        var draft = Draft(oras, p => p.RefreshAbility(slot == 0 ? 1 : 0));
        var stored = draft.Preview();

        draft.EditAbilitySlot(slot);

        AssertMatchesNative(draft, stored, p => p.SetAbilityIndex(slot));
        ChangedOffsets(draft, stored).Should().BeSubsetOf([AbilityOffset, AbilityNumberOffset]);
        draft.AbilitySlot.Should().Be(slot);
        draft.AbilityNumber.Should().Be(1 << slot);
        draft.Ability.Should().Be(stored.PersonalInfo.GetAbilityAtIndex(slot));
        draft.Inspect().Identity.AbilitySlot.Should().Be(1 << slot);
        (draft.Preview().PID, draft.Preview().EncryptionConstant, draft.Gender, draft.Preview().IsShiny).Should().Be((stored.PID, stored.EncryptionConstant, stored.Gender, stored.IsShiny));
    }

    [Fact]
    public void TwoSlotsWithTheSameAbilityStayApart()
    {
        // Gastly has Levitate in every slot: only the slot number tells them apart, and legality analysis checks it.
        var draft = Draft(change: AsSpecies(Gastly, 0));
        draft.RegularAbilitiesSame.Should().BeTrue();
        draft.AbilityChoices.Select(c => c.Value).Should().Equal(0, 1, 2);

        draft.EditAbilitySlot(1);

        draft.AbilityNumber.Should().Be(2);
        draft.AbilitySlot.Should().Be(1);
        draft.Ability.Should().Be((int)Ability.Levitate);
        draft.EditAbilitySlot(0);
        draft.AbilityNumber.Should().Be(1);
        draft.IsDirty.Should().BeFalse();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void ASlotTheSpeciesDoesNotHaveIsRefused(int slot)
    {
        var draft = Draft();

        var edit = () => draft.EditAbilitySlot(slot);

        edit.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.AbilitySlotNotAvailable);
        draft.IsDirty.Should().BeFalse();
        draft.EditRevision.Should().Be(0);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(7)]
    [InlineData(255)]
    public void AStoredSlotNumberThatIsNoSlotIsKeptUntilASlotIsChosen(int number)
    {
        var draft = Draft(change: p => p.AbilityNumber = number);
        var stored = draft.Preview();

        draft.AbilitySlot.Should().BeNull();
        draft.EditGender(1 - stored.Gender);
        draft.EditGender(stored.Gender);
        draft.AbilityNumber.Should().Be(number, "no other edit touches the ability");
        draft.IsDirty.Should().BeFalse();

        draft.EditAbilitySlot(0);

        draft.AbilityNumber.Should().Be(1);
        draft.AbilitySlot.Should().Be(0);
    }

    [Fact]
    public void AStoredAbilityThatIsNotInItsSlotIsKeptUntilASlotIsChosen()
    {
        // The first slot's number with the hidden ability, as a Pokémon edited without the slot number would store.
        var draft = Draft(change: p =>
        {
            p.RefreshAbility(0);
            p.Ability = p.PersonalInfo.AbilityH;
        });
        var stored = draft.Preview();

        draft.AbilitySlot.Should().BeNull();
        draft.Ability.Should().Be(stored.PersonalInfo.AbilityH);

        draft.EditAbilitySlot(0);

        draft.Ability.Should().Be(stored.PersonalInfo.Ability1);
        draft.AbilityNumber.Should().Be(1);
        draft.AbilitySlot.Should().Be(0);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void AGenderChangeMatchesNativeCoreAndWritesOnlyTheGender(bool oras)
    {
        var draft = Draft(oras);
        var stored = draft.Preview();
        var other = (byte)(1 - stored.Gender);

        draft.EditGender(other);

        AssertMatchesNative(draft, stored, p => p.Gender = other);
        ChangedOffsets(draft, stored).Should().Equal(GenderOffset);
        (draft.Preview().Data[GenderOffset] & ~0x06).Should().Be(stored.Data[GenderOffset] & ~0x06, "the fateful flag and the form share the byte");
        draft.Gender.Should().Be(other);
        draft.Inspect().Identity.Gender.Should().Be(other);
        (draft.Preview().PID, draft.Ability, draft.AbilityNumber, draft.Preview().Nature).Should().Be((stored.PID, stored.Ability, stored.AbilityNumber, stored.Nature));
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(-1)]
    [InlineData(255)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void AGenderTheSpeciesCannotHaveIsRefused(int gender)
    {
        var draft = Draft();

        var edit = () => draft.EditGender(gender);

        edit.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.GenderNotAvailable);
        draft.IsDirty.Should().BeFalse();
        draft.EditRevision.Should().Be(0);
    }

    [Theory]
    [InlineData(Tauros, GenderRule.OnlyMale, EntityGender.Male, EntityGender.Female)]
    [InlineData(Chansey, GenderRule.OnlyFemale, EntityGender.Female, EntityGender.Male)]
    [InlineData(Magnemite, GenderRule.Genderless, EntityGender.Genderless, EntityGender.Male)]
    public void ASingleGenderSpeciesOffersOnlyItsGenderWhichCorrectsAWrongStoredValue(ushort species, GenderRule rule, byte only, byte wrong)
    {
        var right = Draft(change: AsSpecies(species, only));
        right.GenderRule.Should().Be(rule);
        right.GenderChoices.Should().Equal(only);
        var other = () => right.EditGender(wrong);
        other.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.GenderNotAvailable);
        right.EditGender(only);
        right.IsDirty.Should().BeFalse();

        // As the desktop's gender toggle sets a single-gender species' only gender.
        var wrongDraft = Draft(change: AsSpecies(species, wrong));
        var stored = wrongDraft.Preview();
        wrongDraft.EditGender(only);
        AssertMatchesNative(wrongDraft, stored, p => p.Gender = only);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void AMeowsticGenderChangeChangesItsFormAsTheDesktopDoes(bool oras)
    {
        var source = Boxed(oras, MaleMeowstic);
        var draft = Open(source);
        var stored = draft.Preview();
        var maleChoices = draft.AbilityChoices;
        draft.FormFollowsGender.Should().BeTrue();
        draft.GenderRule.Should().Be(GenderRule.Either);

        draft.EditGender(EntityGender.Female);

        var personal = SaveFixtures.Parse(source).Personal;
        AssertMatchesNative(draft, stored, p =>
        {
            p.ChangeSpeciesForm(Meowstic, 1, personal, 2);
            p.Gender = EntityGender.Female;
        });
        var female = draft.Preview();
        female.Form.Should().Be(1);
        draft.Gender.Should().Be(EntityGender.Female);
        draft.AbilitySlot.Should().Be(2, "the slot is kept");
        draft.Ability.Should().Be(female.PersonalInfo.AbilityH).And.NotBe(stored.Ability, "the female form's hidden ability is Competitive, not Prankster");
        draft.Ability.Should().Be((int)Ability.Competitive);
        draft.Experience.Should().Be(Experience.GetEXP(30, female.PersonalInfo.EXPGrowth), "Core sets the experience points to the start of the level");
        draft.Level.Should().Be(30);
        draft.AbilityChoices.Should().NotBeSameAs(maleChoices);
        draft.AbilityChoices[2].Text.Should().StartWith(GameInfo.Strings.abilitylist[(int)Ability.Competitive]);
        draft.FormFollowsGender.Should().BeTrue();
        (female.PID, female.EncryptionConstant, female.Nickname).Should().Be((stored.PID, stored.EncryptionConstant, stored.Nickname));

        draft.EditGender(EntityGender.Male);

        draft.IsDirty.Should().BeFalse("returning to the stored form gives back the stored experience points and ability");
    }

    [Fact]
    public void AMeowsticWithAChangedSlotKeepsThatSlotThroughAFormChangeAndBack()
    {
        var draft = Draft(change: MaleMeowstic);
        var stored = draft.Preview();

        draft.EditAbilitySlot(1);
        draft.EditGender(EntityGender.Female);
        draft.AbilitySlot.Should().Be(1);
        draft.EditGender(EntityGender.Male);

        draft.AbilitySlot.Should().Be(1, "the chosen slot is not undone by returning to the stored form");
        draft.Ability.Should().Be(stored.PersonalInfo.Ability2);
        draft.Experience.Should().Be(stored.EXP);
        draft.EditAbilitySlot(2);
        draft.IsDirty.Should().BeFalse();
    }

    [Fact]
    public void ExperienceEditedBeforeAFormChangeIsGivenBackByChangingBack()
    {
        // The form change and back undoes the form change, not the user's own edit before it.
        var draft = Draft(change: MaleMeowstic);
        var edited = draft.Experience + 3;
        draft.EditExperience(edited);

        draft.EditGender(EntityGender.Female);
        draft.EditGender(EntityGender.Male);

        draft.Experience.Should().Be(edited);
    }

    [Fact]
    public void ExperienceEditedAfterAFormChangeIsNotReplacedByTheOldValue()
    {
        var draft = Draft(change: MaleMeowstic);
        var stored = draft.Experience;
        draft.EditGender(EntityGender.Female);
        var start = draft.Experience;
        draft.EditExperience(start + 7);

        draft.EditGender(EntityGender.Male);

        // Changing back is itself a form change, which Core sets to the start of the level, as the desktop does; the experience points from
        // before the first change are not put back over the user's edit.
        draft.Experience.Should().Be(start).And.NotBe(stored);
        draft.Preview().Form.Should().Be(0);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(7)]
    public void AStoredSlotNumberThatIsNoSlotIsGivenBackByAFormChangeAndBack(int number)
    {
        var draft = Draft(change: p =>
        {
            MaleMeowstic(p);
            p.AbilityNumber = number;
        });

        draft.EditGender(EntityGender.Female);
        draft.AbilitySlot.Should().Be(0, "Core normalises a slot number that names no slot to the first slot on a form change");
        draft.EditGender(EntityGender.Male);

        draft.AbilityNumber.Should().Be(number);
        draft.IsDirty.Should().BeFalse();
    }

    [Theory]
    [InlineData(5, EntityGender.Male)] // Unown F, whose form name reads as a gender symbol
    [InlineData(12, EntityGender.Female)] // Unown M
    public void AGenderlessSpeciesWhoseFormNameIsALetterKeepsItsForm(byte form, byte wrong)
    {
        var draft = Draft(change: p =>
        {
            p.Species = (ushort)Species.Unown;
            p.Form = form;
            p.Gender = wrong;
            p.RefreshAbility(0);
        });
        var stored = draft.Preview();
        draft.FormFollowsGender.Should().BeFalse("only a species with two genders has gendered forms, as the desktop's ClickGender checks first");

        draft.EditGender(EntityGender.Genderless);

        AssertMatchesNative(draft, stored, p => p.Gender = EntityGender.Genderless);
        draft.Preview().Form.Should().Be(form);
    }

    [Fact]
    public void AMeowsticWhoseGenderDisagreesWithItsFormGetsTheChosenGender()
    {
        // Stored female form with a male gender: choosing female keeps the form and sets the gender; choosing male changes the form.
        var draft = Draft(change: p =>
        {
            MaleMeowstic(p);
            p.Form = 1;
            p.RefreshAbility(2);
        });
        var stored = draft.Preview();

        draft.EditGender(EntityGender.Female);

        AssertMatchesNative(draft, stored, p => p.Gender = EntityGender.Female);
    }

    [Fact]
    public void AMeowsticPartyMemberChangesFormThroughApplyAndExportAsNativeCore()
    {
        var source = InjuredAndBurned(false, MaleMeowstic);
        var session = SaveFixtures.Open(source);
        var draft = session.Select(SlotRef.InParty(0));
        var stored = draft.Preview();

        draft.EditGender(EntityGender.Female);

        draft.HpChange.Should().BeNull("Meowstic's forms have the same base stats, so nothing is recalculated");
        draft.Preview().Data[stored.SIZE_STORED..].ToArray().Should().Equal(stored.Data[stored.SIZE_STORED..].ToArray());
        session.Apply(draft);
        var output = SaveExporter.Export(session, session.Select(SlotRef.InParty(0)));
        var native = SaveFixtures.Parse(source);
        var pk = native.GetPartySlotAtIndex(0);
        pk.ChangeSpeciesForm(Meowstic, 1, native.Personal, 2);
        pk.Gender = EntityGender.Female;
        native.SetPartySlotAtIndex(pk, 0, EntityImportSettings.None);
        output.Should().Equal(native.Write().ToArray());
    }

    [Fact]
    public void AMeowsticGenderChangeOfAMemberStoredWithoutStatsIsRefused()
    {
        var source = SaveFixtures.Synthetic(false, customize: SaveFixtures.All(SaveFixtures.WithPartyMember(battle: p =>
        {
            MaleMeowstic(p);
            p.ResetPartyStats();
        }), save =>
        {
            var pk = save.GetPartySlotAtIndex(0);
            pk.Stat_HPMax = 0;
            pk.Stat_HPCurrent = 0;
            pk.WriteEncryptedDataParty(save.Data[save.GetPartyOffset(0)..]);
        }));
        var draft = SaveFixtures.Open(source).Select(SlotRef.InParty(0));

        var edit = () => draft.EditGender(EntityGender.Female);

        edit.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.PartyStatsMissing, "a form change is a stat edit");
        draft.IsDirty.Should().BeFalse();
    }

    [Fact]
    public void ValuesChangedAndChangedBackLeaveTheDraftClean()
    {
        var draft = Draft();
        var stored = draft.Preview();

        draft.EditAbilitySlot(2);
        draft.EditGender(1 - stored.Gender);
        draft.IsDirty.Should().BeTrue();
        draft.EditAbilitySlot(stored.AbilityNumber >> 1);
        draft.EditGender(stored.Gender);

        draft.IsDirty.Should().BeFalse();
    }

    [Fact]
    public void EachEditCountsAsAnEdit()
    {
        var draft = Draft();

        draft.EditAbilitySlot(2);
        draft.EditGender(draft.Gender);

        draft.EditRevision.Should().Be(2, "each accepted edit marks legality stale, even one that changes nothing");
    }

    [Fact]
    public void TheEditsAreGatedOnTheFamilyAndRefusedForEggs()
    {
        var bytes = Boxed(change: p => p.IsEgg = true);
        var save = SaveFixtures.Parse(bytes);
        SaveSession Session(EditableFields fields) => new(bytes.ToArray(), save, "fixture.sav", SaveCapabilities.For(save, SupportMatrix.Find(save)! with { Editable = fields }));
        var gated = Session(EditableFields.Nickname).Select(SaveFixtures.FirstBoxSlot);
        var egg = SaveFixtures.Open(bytes).Select(SlotRef.InBox(0, 1));

        foreach (var (draft, error) in new[] { (gated, SessionError.FieldNotEditable), (egg, SessionError.EggNotEditable) })
        {
            Action[] edits = [() => draft.EditAbilitySlot(0), () => draft.EditGender(0)];
            foreach (var edit in edits)
            {
                edit.Should().Throw<SessionException>().Which.Error.Should().Be(error);
            }
            draft.IsDirty.Should().BeFalse();
        }

        var onlyAbility = Session(EditableFields.Ability).Select(SaveFixtures.FirstBoxSlot);
        onlyAbility.EditAbilitySlot(2);
        var gender = () => onlyAbility.EditGender(0);
        gender.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.FieldNotEditable, "the ability and gender are gated separately");
        var onlyGender = Session(EditableFields.Gender).Select(SaveFixtures.FirstBoxSlot);
        onlyGender.EditGender(1);
        var ability = () => onlyGender.EditAbilitySlot(2);
        ability.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.FieldNotEditable);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void PartyEditsKeepTheBattleStateAndMatchNativeCore(bool oras)
    {
        var source = InjuredAndBurned(oras);
        var session = SaveFixtures.Open(source);
        var draft = session.Select(SlotRef.InParty(0));
        var stored = draft.Preview();
        var other = (byte)(1 - stored.Gender);

        draft.EditAbilitySlot(2);
        draft.EditGender(other);

        draft.Preview().Data[stored.SIZE_STORED..].ToArray().Should().Equal(stored.Data[stored.SIZE_STORED..].ToArray(), "no stat is affected, so the battle state is kept byte for byte");
        draft.HpChange.Should().BeNull();
        draft.Inspect().Stats.Source.Should().Be(StatsSource.Stored);
        session.Apply(draft);

        var output = SaveExporter.Export(session, session.Select(SlotRef.InParty(0)));
        var native = SaveFixtures.Parse(source);
        var pk = native.GetPartySlotAtIndex(0);
        pk.SetAbilityIndex(2);
        pk.Gender = other;
        native.SetPartySlotAtIndex(pk, 0, EntityImportSettings.None);
        output.Should().Equal(native.Write().ToArray(), "the session must match the native Core edit");
    }

    [Fact]
    public void APartyMemberStoredWithoutStatsCanHaveItsAbilityAndGenderEdited()
    {
        // These edits affect no stat, so they need no current HP to keep; Apply still refuses to write such a member.
        var draft = SaveFixtures.Open(PartyApplyTests.WithoutStoredStats()).Select(SlotRef.InParty(0));

        draft.EditAbilitySlot(2);
        draft.EditGender(1 - draft.Gender);

        draft.AbilitySlot.Should().Be(2);
    }
}

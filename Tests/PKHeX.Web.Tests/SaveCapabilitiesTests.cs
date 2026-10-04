using FluentAssertions;
using PKHeX.Core;
using PKHeX.Web.Services;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// Capabilities come from the concrete Core save type and the release allowlist, never from a generation number.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class SaveCapabilitiesTests
{
    [Theory]
    [InlineData(false, typeof(SAV6XY))]
    [InlineData(true, typeof(SAV6AO))]
    public void XYAndORASStorePK6AndApplyToPartyAndBoxes(bool oras, Type saveType)
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(oras));
        var capabilities = session.Capabilities;

        capabilities.Family.SaveType.Should().Be(saveType);
        capabilities.EntityType.Should().Be<PK6>();
        capabilities.HasParty.Should().BeTrue();
        capabilities.HasBoxes.Should().BeTrue();
        capabilities.Editable.Should().Be(EditableFields.Nickname | EditableFields.Language | EditableFields.Friendship | EditableFields.Level | EditableFields.Nature
            | EditableFields.Ivs | EditableFields.Evs | EditableFields.HeldItem | EditableFields.Moves | EditableFields.Pp | EditableFields.Ability | EditableFields.Gender
            | EditableFields.Species);
        capabilities.Personal.Should().BeSameAs(session.Working.Personal);
        capabilities.SpeciesChoices.Should().NotContain(s => s.Value == 0, "species 0 would empty the slot");
        capabilities.SpeciesChoices.Select(s => s.Value).Should().BeEquivalentTo(capabilities.Lists.Species.Where(s => s.Value != 0).Select(s => s.Value));
        capabilities.SpeciesChoices.Should().HaveCount(session.Working.MaxSpeciesID, "XY and ORAS hold every species up to Volcanion");
        capabilities.SaveLanguage.Should().Be(session.Working.Language);
        capabilities.MaxNicknameLength.Should().Be(session.Working.MaxStringLengthNickname);
        capabilities.CanApply(SaveFixtures.FirstBoxSlot).Should().BeTrue();
        capabilities.CanApply(SlotRef.InParty(0)).Should().BeTrue("PK6 has a party-stat policy");
        capabilities.CanApplyToParty.Should().BeTrue();
    }

    [Fact]
    public void AFamilyWithoutPartyWritesAppliesOnlyToBoxes()
    {
        var save = SaveFixtures.Parse(SaveFixtures.Synthetic(false));
        var family = SupportMatrix.Find(save)! with { WritesParty = false };

        var capabilities = SaveCapabilities.For(save, family);

        capabilities.CanApplyToParty.Should().BeFalse();
        capabilities.CanApply(SlotRef.InParty(0)).Should().BeFalse();
        capabilities.CanApply(SaveFixtures.FirstBoxSlot).Should().BeTrue();
    }

    [Fact]
    public void AFamilyIsNotAppliedToAnotherSaveType()
    {
        var xy = SaveFixtures.Parse(SaveFixtures.Synthetic(false));
        var oras = SupportMatrix.Families.Single(f => f.SaveType == typeof(SAV6AO));

        var act = () => SaveCapabilities.For(xy, oras);

        act.Should().Throw<NotSupportedException>();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ListsAreFilteredToTheSave(bool oras)
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(oras));
        var save = session.Working;
        var lists = session.Capabilities.Lists;

        lists.Species.Should().NotBeEmpty().And.OnlyContain(s => s.Value <= save.MaxSpeciesID);
        lists.Species.Max(s => s.Value).Should().Be(721, "Generation 6 ends at Volcanion");
        save.MaxSpeciesID.Should().Be(721);
        lists.Moves.Should().OnlyContain(m => m.Value <= save.MaxMoveID);
        lists.Items.Should().NotBeEmpty().And.OnlyContain(i => i.Value <= save.MaxItemID);
        lists.Balls.Should().NotBeEmpty().And.OnlyContain(b => b.Value <= save.MaxBallID);
        lists.Abilities.Should().OnlyContain(a => a.Value <= save.MaxAbilityID);
    }

    [Fact]
    public void OpeningASaveLeavesCoreGlobalListsUntouched()
    {
        var global = GameInfo.FilteredSources;

        var session = SaveFixtures.Open(SaveFixtures.Synthetic(true));

        GameInfo.FilteredSources.Should().BeSameAs(global, "session lists must never become Core's global lists");
        session.Capabilities.Lists.Should().NotBeSameAs(global);
    }

    [Fact]
    public void EachSessionHasItsOwnLists()
    {
        var first = SaveFixtures.Open(SaveFixtures.Synthetic(false)).Capabilities;
        var second = SaveFixtures.Open(SaveFixtures.Synthetic(false)).Capabilities;

        second.Lists.Should().NotBeSameAs(first.Lists);
    }

    [Fact]
    public void SaveTypesTheReleaseDoesNotOpenHaveNoCapabilities()
    {
        foreach (SaveFile save in new SaveFile[] { new SAV6AODemo(), new SAV7SM(), new SAV5BW() })
        {
            var build = () => SaveCapabilities.For(save);
            build.Should().Throw<NotSupportedException>(save.GetType().Name);
        }
    }

    [Fact]
    public void EveryFamilyStoresTheEntityTypeItIsListedWith()
    {
        // The editor and inspector are written for the listed type; a family whose save stores another type must not be listed with it.
        foreach (var family in SupportMatrix.Families)
        {
            var save = (SaveFile)Activator.CreateInstance(family.SaveType)!;
            save.BlankPKM.GetType().Should().Be(family.EntityType, family.Games);
            SaveCapabilities.For(save).Family.Should().BeSameAs(family);
        }
    }
}

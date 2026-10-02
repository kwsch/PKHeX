using FluentAssertions;
using PKHeX.Core;
using PKHeX.Web.Services;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The desktop's name rules (WEB-PKM-003), as <see cref="NameRules"/> mirrors them from <c>PKMEditor.UpdateIsNicknamed</c>,
/// <c>UpdateNickname</c> and <c>IsPossibleNotNicknamed</c>. Expected names come from Core's species names.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class NameRulesTests
{
    private const int English = (int)LanguageID.English, French = (int)LanguageID.French, German = (int)LanguageID.German;

    /// <summary>The known legal Zigzagoon: English, not nicknamed.</summary>
    private static PK6 Zigzagoon() => new(SaveFixtures.ReadEntity(true));

    private static string NameIn(int language) => SpeciesName.GetSpeciesNameGeneration((ushort)Species.Zigzagoon, language, 6);

    [Fact]
    public void TheFixtureIsAnEnglishZigzagoonThatIsNotNicknamed()
    {
        var pk = Zigzagoon();
        pk.Species.Should().Be((ushort)Species.Zigzagoon);
        pk.Language.Should().Be(English);
        pk.IsNicknamed.Should().BeFalse();
        pk.Nickname.Should().Be(NameIn(English));
        NameIn(French).Should().NotBe(NameIn(English), "the French name must differ for the cases below to mean anything");
    }

    [Theory]
    [InlineData("Quill", false, true)] // not a species name in any language: the desktop ticks the flag
    [InlineData("ZIGZAGOON", false, true)] // Generation 6 names are not in capitals
    [InlineData("Quill", true, true)]
    [InlineData("Zigzagoon", true, true)] // typing never clears the flag
    [InlineData("Zigzagoon", false, false)]
    public void TypingSetsTheFlagOnlyForANameThatIsNotTheSpecies(string name, bool before, bool expected) =>
        NameRules.FlagAfterTyping(Zigzagoon(), name, before).Should().Be(expected);

    [Theory]
    [InlineData(French)]
    [InlineData(German)]
    [InlineData((int)LanguageID.Japanese)]
    [InlineData((int)LanguageID.Korean)]
    public void TypingTheSpeciesNameInAnotherLanguageKeepsTheFlagClear(int language) =>
        NameRules.FlagAfterTyping(Zigzagoon(), NameIn(language), false).Should().BeFalse();

    [Theory]
    [InlineData(English)]
    [InlineData(French)]
    [InlineData(German)]
    public void ResettingACustomNameGivesTheDefaultForTheLanguage(int language) =>
        NameRules.NameAfterReset(Zigzagoon(), "Quill", language).Should().Be(NameIn(language));

    [Fact]
    public void ResettingASpeciesNameInAnyLanguageKeepsIt()
    {
        // The desktop's W8 case: an English species name stays when the language changes to French.
        NameRules.NameAfterReset(Zigzagoon(), NameIn(English), French).Should().Be(NameIn(English));
        NameRules.NameAfterReset(Zigzagoon(), NameIn(French), English).Should().Be(NameIn(French));
    }

    [Fact]
    public void ASpeciesOutsideTheFormatKeepsItsNameAndFlag()
    {
        var pk = Zigzagoon();
        pk.Species = 0;

        NameRules.FlagAfterTyping(pk, "Quill", false).Should().BeFalse();
        NameRules.NameAfterReset(pk, "Quill", French).Should().Be("Quill");
        NameRules.Describe(pk, English).DefaultName.Should().BeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)] // unused in Generation 6
    [InlineData((int)LanguageID.ChineseS)] // not a Generation 6 language
    [InlineData((int)LanguageID.ChineseT)]
    [InlineData(255)]
    public void ALanguageWithoutGameNamesHasNoDefaultAndKeepsTheName(int language)
    {
        var pk = Zigzagoon();
        pk.Language = language;

        NameRules.DefaultName(pk, language).Should().BeNull();
        NameRules.NameAfterReset(pk, "Quill", language).Should().Be("Quill", "an empty or foreign name is never written");
        var status = NameRules.Describe(pk, English);
        status.DefaultName.Should().BeNull();
        status.KeptOtherLanguageName.Should().BeFalse();
    }

    [Fact]
    public void EverySpeciesHasADefaultNameInEveryGameLanguage()
    {
        // So a default name is never empty, and clearing the flag never writes an empty name.
        var pk = Zigzagoon();
        foreach (var language in Language.GetAvailableGameLanguages(EntityContext.Gen6))
        {
            for (ushort species = 1; species <= pk.MaxSpeciesID; species++)
            {
                pk.Species = species;
                NameRules.DefaultName(pk, language).Should().NotBeNullOrEmpty($"species {species} in language {language}");
            }
        }
    }

    [Fact]
    public void DescribeReportsTheDefaultName()
    {
        var status = NameRules.Describe(Zigzagoon(), English);

        status.Should().Be(new NameStatus(NameIn(English), true, false, false, NameIn(English)));
    }

    [Fact]
    public void DescribeReportsANameKeptFromAnotherLanguage()
    {
        var pk = Zigzagoon();
        pk.Language = French;

        var status = NameRules.Describe(pk, English);

        status.DefaultName.Should().Be(NameIn(French));
        status.IsDefault.Should().BeFalse();
        status.KeptOtherLanguageName.Should().BeTrue();

        pk.IsNicknamed = true;
        NameRules.Describe(pk, English).KeptOtherLanguageName.Should().BeFalse("a nicknamed Pokémon keeps any name");
    }

    [Fact]
    public void DescribeReportsANotNicknamedCustomNameAsNotKept()
    {
        var pk = Zigzagoon();
        pk.Nickname = "Quill";

        var status = NameRules.Describe(pk, English);

        status.IsDefault.Should().BeFalse();
        status.KeptOtherLanguageName.Should().BeFalse("it is not a species name in any language");
    }

    [Theory]
    [InlineData("Ab😀")]
    [InlineData("Abé")]
    [InlineData("Ab中")]
    public void TheFontCheckIsCores(string name)
    {
        var pk = Zigzagoon();
        pk.Nickname = name;

        var status = NameRules.Describe(pk, English);

        var undefined = StringFontUtil.HasUndefinedCharacters(name, EntityContext.Gen6, LanguageID.English, LanguageID.English);
        status.HasUndefinedCharacters.Should().Be(undefined);
        status.Displayed.Should().Be(StringFontUtil.ReplaceUndefinedCharacters(name, EntityContext.Gen6, LanguageID.English, LanguageID.English));
    }

    [Fact]
    public void TheFontCheckFindsACharacterTheGameCannotShow()
    {
        var pk = Zigzagoon();
        pk.Nickname = "Ab😀";

        var status = NameRules.Describe(pk, English);

        status.HasUndefinedCharacters.Should().BeTrue();
        status.Displayed.Should().NotBe("Ab😀");
    }
}

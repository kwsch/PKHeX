using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests.Util;

public class DataUtilTests
{
    public static TheoryData<string> AllLanguages
    {
        get
        {
            var data = new TheoryData<string>();
            foreach (var lang in GameLanguage.AllSupportedLanguages)
                data.Add(lang);
            return data;
        }
    }

    [Theory]
    [MemberData(nameof(AllLanguages))]
    public void GetsCorrectNumberOfSpeciesNames(string language)
        => VerifyArrayLength(language, static s => s.specieslist, (int)Species.MAX_COUNT);

    [Theory]
    [MemberData(nameof(AllLanguages))]
    public void GetsCorrectNumberOfAbilityNames(string language)
        => VerifyArrayLength(language, static s => s.abilitylist, (int)Ability.MAX_COUNT);

    [Theory]
    [MemberData(nameof(AllLanguages))]
    public void GetsCorrectNumberOfMoveNames(string language)
        => VerifyArrayLength(language, static s => s.movelist, (int)Move.MAX_COUNT);

    private static void VerifyArrayLength(string language, Func<GameStrings, string[]> accessor, [ConstantExpected] int expected)
    {
        var strings = GameInfo.GetStrings(language);
        var names = accessor(strings);
        names.Length.Should().Be(expected);
    }
}

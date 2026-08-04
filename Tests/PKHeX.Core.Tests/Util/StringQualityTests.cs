using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests.Util;

public class StringQualityTests
{
    [Theory]
    [InlineData("ja")]
    [InlineData("en")]
    [InlineData("it")]
    [InlineData("de")]
    [InlineData("fr")]
    [InlineData("es")]
    [InlineData("es-419")]
    [InlineData("ko")]
    [InlineData("zh-Hans")]
    [InlineData("zh-Hant")]
    public void HasNoDuplicates(string language)
    {
        CheckMetLocations(language);
        CheckItemNames(language);
        CheckMoveNames(language);
        CheckSpeciesNames(language);
    }

    /// <summary>
    /// Checks for duplicate hashes in the species list.
    /// </summary>
    /// <remarks>
    /// Uses hashes instead of strings as other logic uses dictionaries of hashes.
    /// </remarks>
    private static void CheckSpeciesNames(string language)
    {
        var strings = GameInfo.GetStrings(language);
        var arr = strings.specieslist;
        var hashset = new HashSet<int>(arr.Length);
        var duplicates = new List<string>(0);
        foreach (var line in arr)
        {
            var hash = line.GetHashCode();
            if (!hashset.Add(hash))
                duplicates.Add(line);
        }
        duplicates.Count.Should().Be(0, "expected no duplicate species strings.");
    }

    private static void CheckMoveNames(string language)
    {
        var strings = GameInfo.GetStrings(language);
        var arr = strings.movelist;
        var duplicates = GetDuplicates(arr);
        duplicates.Count.Should().Be(0, "expected no duplicate move strings.");
    }

    private static void CheckItemNames(string language)
    {
        var strings = GameInfo.GetStrings(language);
        var arr = strings.itemlist;
        var duplicates = GetDuplicates(arr);
        var questionmarks = arr[129];
        duplicates.RemoveAll(z => z == questionmarks);
        duplicates.Count.Should().Be(0, "expected no duplicate item strings.");
    }

    private static List<string> GetDuplicates(string[] arr)
    {
        var hs = new HashSet<string>();
        var duplicates = new List<string>();
        foreach (var line in arr)
        {
            if (line.Length == 0)
                continue;
            if (hs.Contains(line))
                duplicates.Add(line);
            hs.Add(line);
        }
        return duplicates;
    }

    private static void CheckMetLocations(string language)
    {
        var strings = GameInfo.GetStrings(language);

        var sets = typeof(GameStrings).GetFields()
            .Where(z => typeof(ILocationSet).IsAssignableFrom(z.FieldType));

        bool iterated = false;
        var duplicates = new List<string>(0);
        foreach (var setField in sets)
        {
            iterated = true;
            var name = setField.Name;
            var group = setField.GetValue(strings) as ILocationSet;
            Assert.NotNull(group);

            var dict = new Dictionary<string, (int Bank, int Index)>();
            foreach (var (bank, mem) in group.GetAll())
            {
                var arr = mem.Span;
                bool sm0 = bank == 0 && name == nameof(GameStrings.Gen7);
                for (int index = 0; index < arr.Length; index++)
                {
                    var line = arr[index].ToLowerInvariant();
                    if (line.Length == 0)
                        continue;
                    if (sm0 && index % 2 != 0)
                        continue;

                    if (line is "----------" or "－－－－－－－－－－" or "——————" or "")
                        continue; // don't care
                    if (dict.TryGetValue(line, out var other))
                        duplicates.Add($"{name}\t{other.Bank}-{other.Index}\t{bank}-{index}\t{line}");
                    else
                        dict.Add(line, (bank, index));
                }
            }

            if (duplicates.Count == 0)
                continue;

            // None of the location names displayed to the user should be exactly the same.
            // This prevents a location list selection from being ambiguous/not what the user intended.
            var result = string.Join(Environment.NewLine, duplicates);
            Assert.Fail($"Disallowed - duplicate locations for {name}:{Environment.NewLine}{result}");
        }

        iterated.Should().BeTrue();
    }
}

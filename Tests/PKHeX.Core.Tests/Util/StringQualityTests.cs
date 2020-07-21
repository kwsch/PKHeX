using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using PKHeX.Core;
using Xunit;

namespace PKHeX.Tests.Util
{
    public class StringQualityTests
    {
        [Theory]
        [InlineData("ja")]
        [InlineData("en")]
        [InlineData("it")]
        [InlineData("de")]
        [InlineData("fr")]
        [InlineData("es")]
        [InlineData("ko")]
        [InlineData("zh")]
        public void HasNoDuplicates(string language)
        {
            CheckMetLocations(language);
            CheckItemNames(language);
            CheckMoveNames(language);
        }

        private static void CheckMoveNames(string language)
        {
            var strings = GameInfo.GetStrings(language);
            var arr = strings.movelist;
            var duplicates = GetDuplicates(arr);
            duplicates.Count.Should().Be(0, "expected no duplicate strings.");
        }

        private static void CheckItemNames(string language)
        {
            var strings = GameInfo.GetStrings(language);
            var arr = strings.itemlist;
            var duplicates = GetDuplicates(arr);
            var questionmarks = arr[129];
            duplicates.RemoveAll(z => z == questionmarks);
            duplicates.Count.Should().Be(0, "expected no duplicate strings.");
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

            const string prefix = "met";
            nameof(strings.metBW2_00000).StartsWith(prefix).Should()
                .BeTrue("expected field name to exist prior to using reflection to fetch all");
            var metstrings = typeof(GameStrings).GetFields().Where(z => z.Name.StartsWith(prefix));

            bool iterated = false;
            var duplicates = new List<string>();
            foreach (var p in metstrings)
            {
                iterated = true;
                var name = p.Name;
                var val = p.GetValue(strings);
                Assert.NotNull(val);
                var arr = (string[])val!;
                var hs = new HashSet<string>();

                bool sm0 = name == nameof(GameStrings.metSM_00000);
                for (int i = 0; i < arr.Length; i++)
                {
                    var line = arr[i];
                    if (line.Length == 0)
                        continue;
                    if (sm0 && i % 2 != 0)
                        continue;

                    if (hs.Contains(line))
                        duplicates.Add($"{name}\t{line}");
                    hs.Add(line);
                }
            }

            duplicates.Count.Should().Be(0, "expected no duplicate strings.");
            iterated.Should().BeTrue();
        }
    }
}
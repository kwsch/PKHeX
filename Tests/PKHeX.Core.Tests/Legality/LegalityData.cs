using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using PKHeX.Core;
using Xunit;

namespace PKHeX.Tests.Legality
{
    public class LegalityData
    {
        [Fact]
        public void EvolutionsOrdered() // feebas, see issue #2394
        {
            var trees = typeof(EvolutionTree).GetFields(BindingFlags.Static | BindingFlags.NonPublic);
            foreach (var t in trees)
            {
                var gen = Convert.ToInt32(t.Name[7].ToString());
                if (gen <= 4)
                    continue;

                var fTree = (EvolutionTree)t.GetValue(typeof(EvolutionTree));
                var fEntries = typeof(EvolutionTree).GetFields(BindingFlags.NonPublic | BindingFlags.Instance).First(z => z.Name == "Entries");

                var entries = (IReadOnlyList<EvolutionMethod[]>)fEntries.GetValue(fTree);
                var feebas = entries[(int)Species.Feebas];

                var t1 = (EvolutionType)feebas[0].Method;
                var t2 = (EvolutionType)feebas[1].Method;

                t1.IsLevelUpRequired().Should().BeFalse();
                t2.IsLevelUpRequired().Should().BeTrue();
            }
        }
    }
}
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests.Legality;

public class LegalityData
{
    [Fact]
    public void EvolutionsOrdered() // feebas, see issue #2394
    {
        int count = 0;
        var trees = typeof(EvolutionTree)
            .GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Select(z => z.GetValue(typeof(EvolutionTree)))
            .OfType<EvolutionTree>();

        foreach (var tree in trees)
        {
            var feebas = tree.Forward.GetForward((int)Species.Feebas, 0).Span;
            if (feebas.Length <= 1)
                continue;

            var t1 = feebas[0].Method;
            var t2 = feebas[1].Method;

            t1.IsLevelUpRequired().Should().BeFalse();
            t2.IsLevelUpRequired().Should().BeTrue();

            count++;
        }

        count.Should().NotBe(0);
    }

    [Fact]
    public void EvolutionsOrderedSV()
    {
        // SV Crabrawler added a second, UseItem evolution method. Need to be sure it's before the more restrictive level-up method.
        var tree = EvolutionTree.Evolves9;
        var crab = tree.Forward.GetForward((int)Species.Crabrawler, 0).Span;

        var t1 = crab[0].Method;
        var t2 = crab[1].Method;

        t1.IsLevelUpRequired().Should().BeFalse();
        t2.IsLevelUpRequired().Should().BeTrue();
    }
}

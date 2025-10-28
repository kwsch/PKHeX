using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests.Legality;

public class BallTests
{
    /// <summary>
    /// Some species can't use ability patch yet.
    /// </summary>
    [Theory]
    [InlineData(Species.Lunatone)]
    [InlineData(Species.Solrock)]
    [InlineData(Species.Rotom)]
    [InlineData(Species.Archen)]
    [InlineData(Species.Pikachu, true)] // can use ability patch
    public void IsAbilityPatchPossible(Species species, bool expect = false)
    {
        var p7 = BallContextHOME.IsAbilityPatchPossible(7, (ushort)species);
        p7.Should().Be(false);
        var p8 = BallContextHOME.IsAbilityPatchPossible(8, (ushort)species);
        p8.Should().Be(expect);
    }
}

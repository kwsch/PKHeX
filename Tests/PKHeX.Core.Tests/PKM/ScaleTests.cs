using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests;

/// <summary>
/// Legends Arceus scale tests for height and weight calculations, comparing the original game formulas with the fused floating point HOME formulas.
/// </summary>
public static class ScaleTests
{
    // 80, 540 => 499.7647f
    // 127, 160 => 160f
    // 141, 150 => 153.17647f
    // 95, 60 => 56.941177f
    [Theory]
    [InlineData(080, 540, 499.76474f, 499.7647f)]
    [InlineData(141, 150, 153.17648f, 153.17647f)]
    [InlineData(095, 060, 56.94118f, 56.941177f)]
    public static void CheckHeight(byte height, ushort averageHeight, float expectGame, float expectHOME)
    {
        var fake = new FakePersonalInfo { Height = averageHeight };
        var game = PA8.GetHeightAbsolute(fake, height);
        game.Should().Be(expectGame, "Game should match.");

        var result = PA8.GetHeightAbsoluteFused(averageHeight, height);
        result.Should().Be(expectHOME, "HOME should match.");
    }

    // 80, 540 => 499.7647f
    // 127, 160 => 160f
    // 141, 150 => 153.17647f
    // 95, 60 => 56.941177f
    [Theory]
    [InlineData(080, 176, 540, 6830, 6801.9976f, 6801.998f)]
    [InlineData(141, 123, 150, 0610, 618.5207f, 618.5206f)]
    [InlineData(095, 087, 060, 0099, 87.98418f, 87.98417f)]
    public static void CheckWeight(byte height, byte weight, ushort averageHeight, ushort averageWeight, float expectGame, float expectHOME)
    {
        var fake = new FakePersonalInfo { Height = averageHeight, Weight = averageWeight };
        var game = PA8.GetWeightAbsolute(fake, height, weight);
        game.Should().Be(expectGame, "Game should match.");

        var result = PA8.GetWeightAbsoluteFused(averageWeight, height, weight);
        result.Should().Be(expectHOME, "HOME should match.");
    }

    private sealed record FakePersonalInfo : IPersonalMisc
    {
        public int EvoStage { get; set; }
        public int Color { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
    }
}

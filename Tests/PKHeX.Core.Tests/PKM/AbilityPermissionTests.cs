using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests.PKM;

public static class AbilityPermissionTests
{
    [Theory]
    [InlineData(AbilityPermission.OnlyFirst, true, 0)]
    [InlineData(AbilityPermission.OnlySecond, true, 1)]
    [InlineData(AbilityPermission.OnlyHidden, true, 2)]
    [InlineData(AbilityPermission.Any12, false, 0)]
    [InlineData(AbilityPermission.Any12H, false, 0)]
    public static void IsFixedAbility(AbilityPermission value, bool expect, int zeroIndex)
    {
        var result = value.IsSingleValue(out var index);
        result.Should().Be(expect);
        if (result)
            index.Should().Be(zeroIndex);
    }

    [Fact]
    public static void IsZeroAny12() => AbilityPermission.Any12.Should().Be(0, "Initial unspecified value for most encounter templates.");

    [Theory]
    [InlineData(AbilityPermission.OnlyFirst, false)]
    [InlineData(AbilityPermission.OnlySecond, false)]
    [InlineData(AbilityPermission.OnlyHidden, true)]
    [InlineData(AbilityPermission.Any12, false)]
    [InlineData(AbilityPermission.Any12H, true)]
    public static void IsHiddenPossible(AbilityPermission value, bool expect) => value.CanBeHidden().Should().Be(expect);
}

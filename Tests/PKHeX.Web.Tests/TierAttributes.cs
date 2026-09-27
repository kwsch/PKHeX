using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// A <see cref="FactAttribute"/> in an opt-in tier (<see cref="TestCategory.E2E"/>, <see cref="TestCategory.RealSave"/> or <see cref="TestCategory.Perf"/>).
/// It is skipped unless the tier is named in <see cref="TestEnvironment.Tiers"/>.
/// </summary>
/// <remarks>
/// The class still carries the matching <c>[Trait(TestCategory.Name, ...)]</c>; <see cref="TestCategoryTests"/> checks that the two agree.
/// </remarks>
[AttributeUsage(AttributeTargets.Method)]
public sealed class TierFactAttribute : FactAttribute
{
    /// <summary>Marks a fact in <paramref name="tier"/>, skipped unless that tier is opted in.</summary>
    /// <param name="tier"><see cref="TestCategory.E2E"/>, <see cref="TestCategory.RealSave"/> or <see cref="TestCategory.Perf"/>; it must match the class's category.</param>
    public TierFactAttribute(string tier)
    {
        Tier = tier;
        Skip = TestEnvironment.SkipUnlessOptedIn(tier);
    }

    /// <summary>The opt-in tier this test belongs to.</summary>
    public string Tier { get; }
}

/// <summary>
/// A <see cref="TheoryAttribute"/> in an opt-in tier (<see cref="TestCategory.E2E"/>, <see cref="TestCategory.RealSave"/> or <see cref="TestCategory.Perf"/>).
/// It is skipped unless the tier is named in <see cref="TestEnvironment.Tiers"/>.
/// </summary>
/// <remarks>
/// The class still carries the matching <c>[Trait(TestCategory.Name, ...)]</c>; <see cref="TestCategoryTests"/> checks that the two agree.
/// </remarks>
[AttributeUsage(AttributeTargets.Method)]
public sealed class TierTheoryAttribute : TheoryAttribute
{
    /// <summary>Marks a theory in <paramref name="tier"/>, skipped unless that tier is opted in.</summary>
    /// <param name="tier"><see cref="TestCategory.E2E"/>, <see cref="TestCategory.RealSave"/> or <see cref="TestCategory.Perf"/>; it must match the class's category.</param>
    public TierTheoryAttribute(string tier)
    {
        Tier = tier;
        Skip = TestEnvironment.SkipUnlessOptedIn(tier);
    }

    /// <summary>The opt-in tier this test belongs to.</summary>
    public string Tier { get; }
}

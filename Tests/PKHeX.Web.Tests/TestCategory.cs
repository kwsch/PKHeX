namespace PKHeX.Web.Tests;

/// <summary>
/// Test tiers, applied with <c>[Trait(TestCategory.Name, ...)]</c> and selected with <c>--filter Category=...</c>.
/// </summary>
/// <remarks>
/// A run without <c>--filter</c> or <c>--settings</c> (including <c>dotnet test PKHeX.slnx</c>) executes only <see cref="Unit"/>; see <c>PKHeX.Web.Tests.runsettings</c>.
/// <see cref="TestCategoryTests"/> fails if a test has no tier or more than one.
/// The other tiers are opt-in: they run only when named in <see cref="TestEnvironment.Tiers"/> (their tests use <see cref="TierFactAttribute"/> or <see cref="TierTheoryAttribute"/>),
/// and select them with the filter as well. An opted-in tier fails rather than skips when its inputs are missing (<see cref="TestEnvironment.Required"/>).
/// Browser tests share <see cref="PublishedAppFixture"/>; real-save tests read their inputs through <see cref="RealSaves.Read"/>.
/// </remarks>
internal static class TestCategory
{
    /// <summary>Trait name used by <c>--filter Category=...</c>.</summary>
    public const string Name = "Category";

    /// <summary>Self-contained tests on synthetic Core saves.</summary>
    public const string Unit = "Unit";

    /// <summary>Playwright against the published app (<c>PKHEX_WEB_PUBLISHED</c>) with synthetic fixtures.</summary>
    public const string E2E = "E2E";

    /// <summary>Private real XY/ORAS saves (<c>PKHEX_XY_SAVE</c>, <c>PKHEX_ORAS_SAVE</c>); local runs only, never CI.</summary>
    public const string RealSave = "RealSave";
}

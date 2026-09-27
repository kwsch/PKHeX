namespace PKHeX.Web.Tests;

/// <summary>
/// Test tiers, applied with <c>[Trait(TestCategory.Name, ...)]</c> and selected with <c>--filter Category=...</c>.
/// </summary>
/// <remarks>
/// A run without <c>--filter</c> or <c>--settings</c> (including <c>dotnet test PKHeX.slnx</c>) executes only <see cref="Unit"/>; see <c>PKHeX.Web.Tests.runsettings</c>.
/// <see cref="TestCategoryTests"/> fails if a test has no tier or more than one.
/// The other tiers must be selected explicitly, and they fail rather than skip when their inputs are missing.
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

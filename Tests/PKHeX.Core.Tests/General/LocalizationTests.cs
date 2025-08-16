using Xunit;

namespace PKHeX.Core.Tests;

public static class LocalizationTests
{
    [Fact]
    public static void EncounterDisplay() => _ = EncounterDisplayLocalization.Cache.GetAll();

    [Fact]
    public static void MoveSource() => _ = MoveSourceLocalization.Cache.GetAll();

    [Fact]
    public static void LegalityCheck() => _ = LegalityCheckLocalization.Cache.GetAll();

    [Fact]
    public static void General() => _ = GeneralLocalization.Cache.GetAll();
}

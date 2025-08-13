using Xunit;

namespace PKHeX.Core.Tests;

public static class LocalizationTests
{
    [Fact]
    public static void EncounterDisplay() => EncounterDisplayLocalization.Cache.GetAll();

    [Fact]
    public static void MoveSource() => MoveSourceLocalization.Cache.GetAll();

    [Fact]
    public static void LegalityCheck() => LegalityCheckLocalization.Cache.GetAll();

    [Fact]
    public static void General() => GeneralLocalization.Cache.GetAll();
}

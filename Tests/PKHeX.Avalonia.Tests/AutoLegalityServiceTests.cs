using System;
using Microsoft.Extensions.DependencyInjection;
using PKHeX.Application;
using PKHeX.Application.Abstractions;
using PKHeX.Core;
using PKHeX.Infrastructure;
using PKHeX.Infrastructure.AutoLegality;
using Xunit;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Tests for <see cref="IAutoLegalityService"/> (issue #89 Phase 1): a pasted Showdown set becomes a
/// legal Pokémon for the loaded save's generation/format, and impossible/garbage input fails cleanly.
///
/// All tests live in one class so they run serially: the vendored engine exposes process-wide static
/// settings (e.g. APILegality.Timeout), and serial execution keeps that state stable.
/// </summary>
[Collection("AutoLegality")]
public class AutoLegalityServiceTests
{
    private readonly IAutoLegalityService _service = new AutoLegalityService();

    public AutoLegalityServiceTests()
    {
        GameInfo.CurrentLanguage = "en";
        GameInfo.Strings = GameInfo.GetStrings("en");
    }

    // Representative early-route wild Pokémon per generation: each is obtainable in the named game, so
    // the engine can find a real encounter and produce a legal result quickly.
    [Theory]
    [InlineData(GameVersion.C, "Sentret")]     // Gen 2
    [InlineData(GameVersion.E, "Zigzagoon")]   // Gen 3
    [InlineData(GameVersion.D, "Bidoof")]      // Gen 4
    [InlineData(GameVersion.B, "Patrat")]      // Gen 5
    [InlineData(GameVersion.X, "Bunnelby")]    // Gen 6
    [InlineData(GameVersion.SN, "Pikipek")]    // Gen 7
    [InlineData(GameVersion.SW, "Wooloo")]     // Gen 8
    [InlineData(GameVersion.SL, "Lechonk")]    // Gen 9
    public void TryLegalizeShowdownSet_GeneratesLegalPokemon(GameVersion version, string showdown)
    {
        var sav = BlankSaveFile.Get(version);

        var result = _service.TryLegalizeShowdownSet(sav, showdown);

        Assert.True(result.Success,
            $"Expected a legal result for '{showdown}' in {version}. Status={result.Status}. {result.MessageText}");
        Assert.NotNull(result.Pokemon);
        Assert.Equal(LegalizationStatus.Success, result.Status);

        // The service contract: a successful result MUST independently pass legality analysis.
        var la = new LegalityAnalysis(result.Pokemon!);
        Assert.True(la.Valid, "Generated Pokémon failed LegalityAnalysis:\n" + la.Report());
    }

    [Fact]
    public void TryLegalizeShowdownSet_RespectsRequestedDetails()
    {
        var sav = BlankSaveFile.Get(GameVersion.SW);
        const string set = "Wooloo\nLevel: 15\nAdamant Nature\n- Tackle";

        var result = _service.TryLegalizeShowdownSet(sav, set);

        Assert.True(result.Success, result.MessageText);
        var pk = result.Pokemon!;
        Assert.Equal((ushort)831, pk.Species); // Wooloo
        Assert.Equal(15, pk.CurrentLevel);
        Assert.Equal((int)Nature.Adamant, (int)pk.Nature);
        Assert.True(new LegalityAnalysis(pk).Valid);
    }

    [Fact]
    public void TryLegalizeShowdownSet_ReportsIsPopulatedOnSuccess()
    {
        var sav = BlankSaveFile.Get(GameVersion.SL);

        var result = _service.TryLegalizeShowdownSet(sav, "Lechonk");

        Assert.True(result.Success, result.MessageText);
        Assert.False(string.IsNullOrWhiteSpace(result.LegalityReport));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void TryLegalizeShowdownSet_EmptyInput_ReturnsInvalidSet(string? text)
    {
        var sav = BlankSaveFile.Get(GameVersion.SW);

        var result = _service.TryLegalizeShowdownSet(sav, text);

        Assert.False(result.Success);
        Assert.Equal(LegalizationStatus.InvalidSet, result.Status);
        Assert.Null(result.Pokemon);
        Assert.NotEmpty(result.Messages);
    }

    [Fact]
    public void TryLegalizeShowdownSet_GarbageText_ReturnsInvalidSet()
    {
        var sav = BlankSaveFile.Get(GameVersion.SW);

        var result = _service.TryLegalizeShowdownSet(sav, "this is not a pokemon set at all !!!");

        Assert.False(result.Success);
        Assert.Equal(LegalizationStatus.InvalidSet, result.Status);
        Assert.Null(result.Pokemon);
    }

    [Fact]
    public void TryLegalizeShowdownSet_ImpossibleSpeciesForGeneration_FailsWithoutCrash()
    {
        // A Gen 9 legendary requested inside a Gen 3 (Emerald) save: there is no encounter that can
        // produce it in that format, so the engine must report a non-success status rather than throw.
        var sav = BlankSaveFile.Get(GameVersion.E);

        var result = _service.TryLegalizeShowdownSet(sav, "Koraidon");

        Assert.False(result.Success);
        Assert.Null(result.Pokemon);
        Assert.Contains(result.Status,
            new[] { LegalizationStatus.Failed, LegalizationStatus.Timeout, LegalizationStatus.InvalidSet });
        Assert.NotEmpty(result.Messages);
    }

    [Fact]
    public void TryLegalizeShowdownSet_NullSave_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => _service.TryLegalizeShowdownSet(null!, "Pikachu"));
    }

    [Fact]
    public void AutoLegalityService_IsRegisteredInInfrastructure()
    {
        var services = new ServiceCollection();
        services.AddApplication();
        services.AddInfrastructure();

        using var provider = services.BuildServiceProvider();
        var resolved = provider.GetService<IAutoLegalityService>();

        Assert.NotNull(resolved);
        Assert.IsType<AutoLegalityService>(resolved);
    }
}

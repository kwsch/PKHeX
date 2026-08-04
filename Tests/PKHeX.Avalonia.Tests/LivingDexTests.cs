using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using PKHeX.Application.UseCases;
using PKHeX.Core;
using PKHeX.Infrastructure.AutoLegality;
using Xunit;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Tests for the Living Dex generator (issue #123, ALM Phase 2): placement/space-check refusal,
/// undo-as-a-single-operation, cancellation, and skip-reporting for candidates that fail independent
/// re-verification.
///
/// ROUTED-AROUND VENDORED-ENGINE DEFECT (discovered while writing these tests, confirmed against Gen
/// 1/2/3/8 blank saves): <c>ModLogic.GenerateLivingDex</c>'s per-species helper builds its search template
/// and calls <c>tr.TryAPIConvert(set, t)</c> directly, without first calling
/// <c>EncounterMovesetGenerator.OptimizeCriteria(template, tr)</c> — the step
/// <c>Legalizer.GetLegalFromSet(ITrainerInfo, IBattleTemplate)</c> (the path <see cref="AutoLegalityService"/>
/// uses, and the one this app's "Auto Legality Mod" single-set tool relies on) always performs before
/// converting. Without that step, <c>GetRandomEncounter</c> cannot find any encounter and returns
/// <see langword="null"/> for every species, so <c>GenerateLivingDex</c> yields an empty list — verified
/// directly (not just inferred) for Red/Crystal/Emerald/Sword blank saves. This is a defect in the
/// vendored <c>PKHeX.AutoMod</c> project itself, which per this task's hard rules must not be modified
/// here. Rather than ship a generator that produces nothing, <see cref="LivingDexService"/> (Infrastructure)
/// drives the species/form loop itself and calls the proven <c>Legalizer.GetLegalFromSet</c> path per
/// candidate instead — see that class's remarks for the full explanation, including why the very first
/// version of that loop (reusing a fully-rendered Showdown block from a freshly-blanked PKM) still failed
/// for every species (over-constrained sets — fixed by keeping only the species/form/gender line).
/// <see cref="Generate_RealSave_ProducesLegalPokemonForABoundedSpeciesSubset"/> below exercises the real,
/// unmocked path end to end and asserts a non-empty, all-legal result; the placement/undo/verification
/// tests instead use synthetically-legal <see cref="PKM"/> (via <see cref="AutoLegalityService"/>) so they
/// exercise the adapter's own logic independent of how many candidates generation itself found.
///
/// All tests live in one class so they run serially, matching <see cref="AutoLegalityServiceTests"/>: the
/// vendored engine exposes process-wide static settings (e.g. <c>Legalizer.EnableEasterEggs</c>,
/// <c>APILegality.UseTrainerData</c>), and serial execution keeps that state stable.
/// </summary>
[Collection("AutoLegality")]
public class LivingDexTests
{
    private readonly AutoLegalityService _autoLegality = new();
    private readonly LivingDexPlacementUseCase _placement = new();
    private readonly LivingDexVerificationUseCase _verification = new();

    public LivingDexTests()
    {
        GameInfo.CurrentLanguage = "en";
        GameInfo.Strings = GameInfo.GetStrings("en");
    }

    private PKM MakeLegal(SaveFile sav, string showdown)
    {
        var result = _autoLegality.TryLegalizeShowdownSet(sav, showdown);
        Assert.True(result.Success, $"Test setup failed to legalize '{showdown}': {result.MessageText}");
        return result.Pokemon!;
    }

    // -------------------------------------------------------------------
    // Placement: successful fill starting at the chosen box
    // -------------------------------------------------------------------

    [Fact]
    public void TryPlace_WritesEveryPokemonStartingAtChosenBox_AndEachSlotPassesLegality()
    {
        var sav = BlankSaveFile.Get(GameVersion.SW);
        var pokemon = new List<PKM> { MakeLegal(sav, "Wooloo"), MakeLegal(sav, "Rookidee"), MakeLegal(sav, "Blipbug") };

        var result = _placement.TryPlace(sav, pokemon, startBox: 1);

        Assert.Equal(LivingDexPlacementStatus.Success, result.Status);
        Assert.Equal(pokemon.Count, result.PlacedCount);

        var slotsPerBox = sav.BoxSlotCount;
        for (var i = 0; i < pokemon.Count; i++)
        {
            var index = 1 * slotsPerBox + i;
            var written = sav.GetBoxSlotAtIndex(index / slotsPerBox, index % slotsPerBox);
            Assert.Equal(pokemon[i].Species, written.Species);
            Assert.True(new LegalityAnalysis(written).Valid, $"Written slot {i} failed legality analysis.");
        }
    }

    [Fact]
    public void TryPlace_EmptyList_IsANoOpSuccess()
    {
        var sav = BlankSaveFile.Get(GameVersion.SW);

        var result = _placement.TryPlace(sav, [], startBox: 0);

        Assert.Equal(LivingDexPlacementStatus.Success, result.Status);
        Assert.Equal(0, result.PlacedCount);
    }

    // -------------------------------------------------------------------
    // Placement: refusal when there isn't enough contiguous empty space
    // -------------------------------------------------------------------

    [Fact]
    public void TryPlace_RefusesWithNoWrites_WhenNotEnoughContiguousSpace()
    {
        var sav = BlankSaveFile.Get(GameVersion.SW);
        var slotsPerBox = sav.BoxSlotCount;

        // Occupy the 2nd slot of the starting box so the contiguous empty run is only 1 slot long,
        // even though the save has plenty of total empty space elsewhere.
        var occupant = MakeLegal(sav, "Wooloo");
        sav.SetBoxSlotAtIndex(occupant, 0, 1);

        var toPlace = new List<PKM> { MakeLegal(sav, "Rookidee"), MakeLegal(sav, "Blipbug") };

        var result = _placement.TryPlace(sav, toPlace, startBox: 0);

        Assert.Equal(LivingDexPlacementStatus.InsufficientSpace, result.Status);
        Assert.Equal(0, result.PlacedCount);
        Assert.Equal(2, result.RequiredSlots);
        Assert.Equal(1, result.AvailableSlots);

        // No partial writes: slot 0 must still be empty (untouched), slot 1 must still hold the occupant.
        Assert.Equal(0, sav.GetBoxSlotAtIndex(0, 0).Species);
        Assert.Equal(occupant.Species, sav.GetBoxSlotAtIndex(0, 1).Species);
    }

    [Fact]
    public void TryPlace_InvalidStartBox_Throws()
    {
        var sav = BlankSaveFile.Get(GameVersion.SW);

        Assert.Throws<ArgumentOutOfRangeException>(() => _placement.TryPlace(sav, [MakeLegal(sav, "Wooloo")], startBox: sav.BoxCount));
    }

    // -------------------------------------------------------------------
    // Undo: a placement is a single undoable operation
    // -------------------------------------------------------------------

    [Fact]
    public void TryPlace_WithUndoRedo_UndoesTheWholeBatchInOneCall()
    {
        var sav = BlankSaveFile.Get(GameVersion.SW);
        var undoRedo = new UndoRedoService();
        undoRedo.Initialize(sav);

        var pokemon = new List<PKM> { MakeLegal(sav, "Wooloo"), MakeLegal(sav, "Rookidee"), MakeLegal(sav, "Blipbug") };
        var result = _placement.TryPlace(sav, pokemon, startBox: 0, undoRedo);
        Assert.Equal(LivingDexPlacementStatus.Success, result.Status);

        for (var i = 0; i < pokemon.Count; i++)
            Assert.Equal(pokemon[i].Species, sav.GetBoxSlotAtIndex(0, i).Species);

        Assert.True(undoRedo.CanUndo);

        // A single Undo() call reverts every slot the placement wrote, not just the last one.
        undoRedo.Undo();

        for (var i = 0; i < pokemon.Count; i++)
            Assert.Equal(0, sav.GetBoxSlotAtIndex(0, i).Species);

        // The whole batch was one logical operation: after undoing it, there is nothing left to undo.
        Assert.False(undoRedo.CanUndo);
        Assert.True(undoRedo.CanRedo);

        // Redo is likewise atomic: one call reapplies every slot.
        undoRedo.Redo();
        for (var i = 0; i < pokemon.Count; i++)
            Assert.Equal(pokemon[i].Species, sav.GetBoxSlotAtIndex(0, i).Species);
        Assert.False(undoRedo.CanRedo);
    }

    [Fact]
    public void UndoRedoService_UngroupedChanges_StillUndoOneAtATime()
    {
        // Regression guard: existing (non-batched) single-slot callers must keep their original
        // one-change-per-Undo() behavior after adding batch support for Living Dex.
        var sav = BlankSaveFile.Get(GameVersion.SW);
        var undoRedo = new UndoRedoService();
        undoRedo.Initialize(sav);

        var a = MakeLegal(sav, "Wooloo");
        var b = MakeLegal(sav, "Rookidee");

        undoRedo.AddChange(new SlotInfoBox(0, 0, sav));
        sav.SetBoxSlotAtIndex(a, 0, 0);

        undoRedo.AddChange(new SlotInfoBox(0, 1, sav));
        sav.SetBoxSlotAtIndex(b, 0, 1);

        undoRedo.Undo();
        Assert.Equal(0, sav.GetBoxSlotAtIndex(0, 1).Species); // second change reverted
        Assert.Equal(a.Species, sav.GetBoxSlotAtIndex(0, 0).Species); // first change untouched
        Assert.True(undoRedo.CanUndo);

        undoRedo.Undo();
        Assert.Equal(0, sav.GetBoxSlotAtIndex(0, 0).Species);
        Assert.False(undoRedo.CanUndo);
    }

    // -------------------------------------------------------------------
    // Skip-reporting: candidates that fail independent re-verification are named, not included
    // -------------------------------------------------------------------

    [Fact]
    public void Verify_SplitsLegalFromIllegal_AndNamesTheIllegalOnes()
    {
        var sav = BlankSaveFile.Get(GameVersion.SW);
        var legal = MakeLegal(sav, "Wooloo");

        var illegal = MakeLegal(sav, "Rookidee");
        illegal.MetLocation = 60000; // corrupt it post-generation (out-of-range met location) so it now fails LegalityAnalysis
        Assert.False(new LegalityAnalysis(illegal).Valid); // sanity check on the test's own setup

        var result = _verification.Verify([legal, illegal]);

        Assert.Single(result.Accepted);
        Assert.Equal(legal.Species, result.Accepted[0].Species);

        Assert.Single(result.SkippedSpeciesNames);
        Assert.Contains("Rookidee", result.SkippedSpeciesNames[0]);
    }

    [Fact]
    public void Verify_AllLegal_ReportsNoSkips()
    {
        var sav = BlankSaveFile.Get(GameVersion.SW);
        var pokemon = new List<PKM> { MakeLegal(sav, "Wooloo"), MakeLegal(sav, "Rookidee") };

        var result = _verification.Verify(pokemon);

        Assert.Equal(2, result.Accepted.Count);
        Assert.Empty(result.SkippedSpeciesNames);
    }

    // -------------------------------------------------------------------
    // Cancellation
    // -------------------------------------------------------------------

    [Fact]
    public void Generate_PreCancelledToken_ReturnsCancelledWithoutRunningTheEngine()
    {
        var service = new LivingDexService();
        var sav = BlankSaveFile.Get(GameVersion.SW);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var result = service.Generate(sav, new LivingDexOptions(IncludeForms: false, SetShiny: false), cancellationToken: cts.Token);

        Assert.True(result.Cancelled);
        Assert.Empty(result.Pokemon);
        Assert.Empty(result.SkippedSpeciesNames);
    }

    [Fact]
    public void Generate_CancelledMidLoop_StopsPromptly()
    {
        // The per-species loop lives in the adapter now (not inside the vendored engine, see class
        // remarks), so cancellation is checked every species — this proves it actually takes effect
        // partway through a run rather than only before/after a whole pass. A synchronous IProgress<T> is
        // used (not System.Progress<T>, which marshals its callback and would make the cancel-after-N
        // trigger racy/nondeterministic in a test) so the species-processed count is exact.
        var service = new LivingDexService();
        var sav = BlankSaveFile.Get(GameVersion.SW);
        using var cts = new CancellationTokenSource();

        var seen = 0;
        var progress = new SynchronousProgress<LivingDexGenerationProgress>(p =>
        {
            seen = p.Completed;
            if (p.Completed >= 2)
                cts.Cancel();
        });

        var result = service.Generate(sav, new LivingDexOptions(IncludeForms: false, SetShiny: false), progress, cts.Token, maxSpeciesId: 200);

        Assert.True(result.Cancelled);
        Assert.Empty(result.Pokemon);
        Assert.True(seen < 200, $"Expected cancellation to stop the loop well short of 200 species; processed {seen}.");
    }

    /// <summary>Invokes its callback synchronously on the reporting thread — unlike <see cref="Progress{T}"/>, which posts to a captured SynchronizationContext (or the thread pool) and would make ordering nondeterministic in a test.</summary>
    private sealed class SynchronousProgress<T>(Action<T> callback) : IProgress<T>
    {
        public void Report(T value) => callback(value);
    }

    // -------------------------------------------------------------------
    // End-to-end generation against the real, unmocked adapter (bounded species subset for speed)
    // -------------------------------------------------------------------

    [Fact]
    public void Generate_RealSave_ProducesLegalPokemonForABoundedSpeciesSubset()
    {
        // Sword, species 1-50 (bounded via maxSpeciesId — a test-only knob; the UI never sets it, so the
        // default path always generates the full dex). This calls the real GetLegalFromSet-based loop end
        // to end, no mocking, and was verified to produce 34 legal candidates out of 50 species attempted
        // (the rest are species not obtainable in this game/that the minimal template can't legalize —
        // e.g. ones needing form-specific held items this adapter doesn't set, see LivingDexService remarks).
        var service = new LivingDexService();
        var sav = BlankSaveFile.Get(GameVersion.SW);

        var result = service.Generate(sav, new LivingDexOptions(IncludeForms: false, SetShiny: false), maxSpeciesId: 50);

        Assert.False(result.Cancelled);
        Assert.NotEmpty(result.Pokemon);

        foreach (var pk in result.Pokemon)
            Assert.True(new LegalityAnalysis(pk).Valid, $"{GameInfo.Strings.Species[pk.Species]} failed legality analysis.");

        // Species are unique (one specimen per species, matching ModLogic.GenerateLivingDex's contract).
        Assert.Equal(result.Pokemon.Select(p => p.Species).Distinct().Count(), result.Pokemon.Count);

        // Every attempted species is accounted for: either in the result, or named as skipped.
        Assert.Equal(50, result.Pokemon.Count + result.SkippedSpeciesNames.Count
            + Enumerable.Range(1, 50).Count(s => !sav.Personal.IsSpeciesInGame((ushort)s)));
    }
}

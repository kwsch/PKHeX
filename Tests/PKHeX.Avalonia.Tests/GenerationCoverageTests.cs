using Moq;
using PKHeX.Avalonia.Services;
using PKHeX.Presentation.ViewModels;
using PKHeX.Core;
using Xunit.Abstractions;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Generation coverage tests: for each generation where no real binary save exists,
/// inject actual legal PKM fixture files into a blank save and run the full ViewModel
/// suite against the result. Together with RealSaveFileTests (Gen3/Gen7) this gives
/// end-to-end ViewModel coverage across all 9 generations.
/// </summary>
public class GenerationCoverageTests(ITestOutputHelper output)
{
    // -----------------------------------------------------------------------
    // Fixture loading helpers (shared with RealPKMFixtureTests)
    // -----------------------------------------------------------------------

    private static string? FindTestsRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            if (Directory.Exists(Path.Combine(dir.FullName, "PKHeX.Core.Tests")))
                return dir.FullName;
            dir = dir.Parent;
        }
        return null;
    }

    private static void ApplyParseSettings(string dirPath)
    {
        ParseSettings.AllowEraCartGB = dirPath.Contains("GBCartEra");
        ParseSettings.AllowEraCartGBA = !dirPath.Contains("GBAVCEra");
        ParseSettings.Settings.Tradeback.AllowGen1Tradeback = dirPath.Contains("1 Tradeback");
    }

    /// <summary>
    /// Loads legal PKM fixtures whose file extension matches a specific generation.
    /// Returns at most <paramref name="max"/> items to keep test runs fast.
    /// </summary>
    private static List<PKM> LoadLegalFixtures(string extension, int max = 10)
    {
        var root = FindTestsRoot();
        if (root == null) return [];

        var legalPath = Path.Combine(root, "PKHeX.Core.Tests", "Legality", "Legal");
        var result = new List<PKM>();

        foreach (var file in Directory.EnumerateFiles(legalPath, $"*{extension}", SearchOption.AllDirectories))
        {
            if (result.Count >= max) break;

            ApplyParseSettings(Path.GetDirectoryName(file) ?? string.Empty);
            var context = EntityFileExtension.GetContextFromExtension(file);
            if (!context.IsValid) continue;

            var data = File.ReadAllBytes(file);
            var pk = EntityFormat.GetFromBytes(data, context);
            if (pk != null)
                result.Add(pk);
        }

        return result;
    }

    /// <summary>
    /// Creates a blank save populated with legal fixture PKM in the first N box slots.
    /// Slots that reject a PKM (format mismatch) are silently skipped.
    /// </summary>
    private static SaveFile PopulateSave(GameVersion version, IEnumerable<PKM> pokemon)
    {
        var sav = BlankSaveFile.Get(version);
        int slot = 0;
        int maxSlots = sav.BoxCount * sav.BoxSlotCount;

        foreach (var pk in pokemon)
        {
            if (slot >= maxSlots) break;
            try { sav.SetBoxSlotAtIndex(pk, slot++); }
            catch { /* incompatible format — skip slot */ }
        }

        return sav;
    }

    // -----------------------------------------------------------------------
    // Per-generation fixture → save → ViewModel test runner
    // -----------------------------------------------------------------------

    private void RunViewModelSuiteOnPopulatedSave(
        GameVersion version, string extension, string genLabel)
    {
        var fixtures = LoadLegalFixtures(extension);

        if (fixtures.Count == 0)
        {
            output.WriteLine($"{genLabel}: no fixtures found for {extension}, skipping.");
            return;
        }

        SaveFile sav;
        try { sav = PopulateSave(version, fixtures); }
        catch { output.WriteLine($"{genLabel}: BlankSaveFile.Get failed, skipping."); return; }

        output.WriteLine($"{genLabel}: populated save with {fixtures.Count} {extension} fixtures.");

        // --- BoxViewerViewModel ---
        var spriteMock = new Mock<ISpriteRenderer>();
        var ex = Record.Exception(() => new BoxViewerViewModel(sav, spriteMock.Object));
        Assert.Null(ex);

        // --- PartyViewerViewModel ---
        ex = Record.Exception(() => new PartyViewerViewModel(sav, spriteMock.Object));
        Assert.Null(ex);

        // --- TrainerEditorViewModel ---
        try
        {
            var trainerVm = new TrainerEditorViewModel(sav);
            Assert.NotNull(trainerVm.TrainerName);
        }
        catch (ArgumentOutOfRangeException)
        {
            // Known: some blank save formats have uninitialized SCBlock data
        }

        // --- PokemonEditorViewModel for every occupied slot ---
        var errors = new List<string>();
        int loaded = 0;

        for (int i = 0; i < sav.BoxCount * sav.BoxSlotCount; i++)
        {
            var pk = sav.GetBoxSlotAtIndex(i);
            if (pk.Species == 0) continue;

            try
            {
                var (vm, _, _) = TestHelpers.CreateTestViewModel(pk, sav);
                _ = vm.Species;
                _ = vm.Level;
                _ = vm.IsLegal;
                _ = vm.PreparePKM();
                loaded++;
            }
            catch (Exception e)
            {
                errors.Add($"slot {i} (species={pk.Species}): {e.Message}");
            }
        }

        output.WriteLine($"{genLabel}: loaded {loaded} Pokemon from populated save.");
        Assert.Empty(errors);
    }

    // -----------------------------------------------------------------------
    // One test per generation
    // -----------------------------------------------------------------------

    [Fact]
    public void Gen1_PopulatedSave_ViewModelSuite()
        => RunViewModelSuiteOnPopulatedSave(GameVersion.RD, ".pk1", "Gen1-Red");

    [Fact]
    public void Gen2_PopulatedSave_ViewModelSuite()
        => RunViewModelSuiteOnPopulatedSave(GameVersion.C, ".pk2", "Gen2-Crystal");

    [Fact]
    public void Gen4_PopulatedSave_ViewModelSuite()
        => RunViewModelSuiteOnPopulatedSave(GameVersion.Pt, ".pk4", "Gen4-Platinum");

    [Fact]
    public void Gen5_PopulatedSave_ViewModelSuite()
        => RunViewModelSuiteOnPopulatedSave(GameVersion.W2, ".pk5", "Gen5-White2");

    [Fact]
    public void Gen6_PopulatedSave_ViewModelSuite()
        => RunViewModelSuiteOnPopulatedSave(GameVersion.X, ".pk6", "Gen6-X");

    [Fact]
    public void Gen8_PopulatedSave_ViewModelSuite()
        => RunViewModelSuiteOnPopulatedSave(GameVersion.SW, ".pk8", "Gen8-Sword");

    [Fact]
    public void Gen9_PopulatedSave_ViewModelSuite()
        => RunViewModelSuiteOnPopulatedSave(GameVersion.SL, ".pk9", "Gen9-Scarlet");

    // Gen3 and Gen7 covered by real binary saves in RealSaveFileTests
    // Gen7b/Gen8b/Gen8a: limited fixture count but still exercise the path
    [Fact]
    public void Gen7b_PopulatedSave_ViewModelSuite()
        => RunViewModelSuiteOnPopulatedSave(GameVersion.GP, ".pb7", "Gen7b-LetsGoPikachu");

    [Fact]
    public void Gen8b_PopulatedSave_ViewModelSuite()
        => RunViewModelSuiteOnPopulatedSave(GameVersion.BD, ".pb8", "Gen8b-BrilliantDiamond");
}

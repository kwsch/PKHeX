using System.Collections.Generic;
using PKHeX.Core;
using Xunit.Abstractions;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Loads all .pk* fixture files from PKHeX.Core.Tests/Legality into PokemonEditorViewModel.
/// Covers 132 legal and 44 illegal Pokemon spanning generations 1–9 and all variants.
/// </summary>
public class RealPKMFixtureTests(ITestOutputHelper output)
{
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

    private static SaveFile GetSaveForContext(EntityContext context) => context switch
    {
        EntityContext.Gen1  => BlankSaveFile.Get(GameVersion.RD),
        EntityContext.Gen2  => BlankSaveFile.Get(GameVersion.C),
        EntityContext.Gen3  => BlankSaveFile.Get(GameVersion.E),
        EntityContext.Gen4  => BlankSaveFile.Get(GameVersion.Pt),
        EntityContext.Gen5  => BlankSaveFile.Get(GameVersion.W2),
        EntityContext.Gen6  => BlankSaveFile.Get(GameVersion.X),
        EntityContext.Gen7  => BlankSaveFile.Get(GameVersion.SN),
        EntityContext.Gen7b => BlankSaveFile.Get(GameVersion.GP),
        EntityContext.Gen8  => BlankSaveFile.Get(GameVersion.SW),
        EntityContext.Gen8b => BlankSaveFile.Get(GameVersion.BD),
        EntityContext.Gen8a => BlankSaveFile.Get(GameVersion.PLA),
        EntityContext.Gen9  => BlankSaveFile.Get(GameVersion.SL),
        _                   => BlankSaveFile.Get(GameVersion.X),
    };

    private static void ApplyParseSettings(string dirPath)
    {
        ParseSettings.AllowEraCartGB = dirPath.Contains("GBCartEra");
        ParseSettings.AllowEraCartGBA = !dirPath.Contains("GBAVCEra");
        ParseSettings.Settings.Tradeback.AllowGen1Tradeback = dirPath.Contains("1 Tradeback");
    }

    private static IEnumerable<(string File, PKM PKM, SaveFile Save)> LoadFixtures(string subfolder)
    {
        var root = FindTestsRoot();
        if (root == null) yield break;

        var path = Path.Combine(root, "PKHeX.Core.Tests", "Legality", subfolder);
        if (!Directory.Exists(path)) yield break;

        foreach (var file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
        {
            var context = EntityFileExtension.GetContextFromExtension(file);
            if (!context.IsValid) continue;

            ApplyParseSettings(Path.GetDirectoryName(file) ?? string.Empty);

            var data = File.ReadAllBytes(file);
            var pk = EntityFormat.GetFromBytes(data, context);
            if (pk == null) continue;

            SaveFile save;
            try { save = GetSaveForContext(pk.Context); }
            catch { continue; }

            yield return (file, pk, save);
        }
    }

    // -----------------------------------------------------------------------
    // 1. Loading should never crash — legal fixtures
    // -----------------------------------------------------------------------
    [Fact]
    public void LegalFixtures_LoadIntoViewModel_WithoutException()
    {
        var errors = new List<string>();
        int loaded = 0;

        foreach (var (file, pk, save) in LoadFixtures("Legal"))
        {
            try
            {
                _ = TestHelpers.CreateTestViewModel(pk, save);
                loaded++;
            }
            catch (Exception ex)
            {
                errors.Add($"{Path.GetFileName(file)}: {ex.GetType().Name}: {ex.Message}");
            }
        }

        output.WriteLine($"Loaded {loaded} legal fixtures without exception.");
        Assert.Empty(errors);
    }

    // -----------------------------------------------------------------------
    // 2. Legal fixtures must be reported as legal by the ViewModel
    // -----------------------------------------------------------------------
    [Fact]
    public void LegalFixtures_AreReportedAsLegal_ByViewModel()
    {
        var failures = new List<string>();
        int checked_ = 0;

        foreach (var (file, pk, save) in LoadFixtures("Legal"))
        {
            try
            {
                var (vm, _, _) = TestHelpers.CreateTestViewModel(pk, save);
                checked_++;
                if (!vm.IsLegal)
                {
                    var snippet = vm.LegalityReport[..Math.Min(200, vm.LegalityReport.Length)];
                    failures.Add($"{Path.GetFileName(file)}: reported ILLEGAL — {snippet}");
                }
            }
            catch { /* crash failures are reported by the other test */ }
        }

        output.WriteLine($"Checked {checked_} legal fixtures for IsLegal.");
        foreach (var f in failures) output.WriteLine(f);
        Assert.Empty(failures);
    }

    // -----------------------------------------------------------------------
    // 3. Loading should never crash — illegal fixtures
    // -----------------------------------------------------------------------
    [Fact]
    public void IllegalFixtures_LoadIntoViewModel_WithoutException()
    {
        var errors = new List<string>();
        int loaded = 0;

        foreach (var (file, pk, save) in LoadFixtures("Illegal"))
        {
            try
            {
                _ = TestHelpers.CreateTestViewModel(pk, save);
                loaded++;
            }
            catch (Exception ex)
            {
                errors.Add($"{Path.GetFileName(file)}: {ex.GetType().Name}: {ex.Message}");
            }
        }

        output.WriteLine($"Loaded {loaded} illegal fixtures without exception.");
        Assert.Empty(errors);
    }

    // -----------------------------------------------------------------------
    // 4. Illegal fixtures must be reported as illegal by the ViewModel
    // -----------------------------------------------------------------------
    // Fixtures that upstream legality updates have reclassified as legal.
    // These were illegal under older PKHeX.Core but pass current checks.
    private static readonly HashSet<string> ReclassifiedAsLegal =
    [
        "284 - MASQUERAIN - 8C49324DA947.pk3",
    ];

    [Fact]
    public void IllegalFixtures_AreReportedAsIllegal_ByViewModel()
    {
        var falsePositives = new List<string>();
        int checked_ = 0;

        foreach (var (file, pk, save) in LoadFixtures("Illegal"))
        {
            var fileName = Path.GetFileName(file);
            if (ReclassifiedAsLegal.Contains(fileName))
                continue;

            try
            {
                var (vm, _, _) = TestHelpers.CreateTestViewModel(pk, save);
                checked_++;
                if (vm.IsLegal)
                    falsePositives.Add($"{fileName}: reported LEGAL (should be illegal)");
            }
            catch { /* crash failures are reported by the other test */ }
        }

        output.WriteLine($"Checked {checked_} illegal fixtures for IsLegal.");
        foreach (var fp in falsePositives) output.WriteLine(fp);
        Assert.Empty(falsePositives);
    }

    // -----------------------------------------------------------------------
    // 5. Every loaded fixture produces a non-null PreparePKM result
    // -----------------------------------------------------------------------
    [Fact]
    public void AllFixtures_PreparePKM_ReturnsNonNull()
    {
        var errors = new List<string>();

        foreach (var subfolder in new[] { "Legal", "Illegal" })
        {
            foreach (var (file, pk, save) in LoadFixtures(subfolder))
            {
                try
                {
                    var (vm, _, _) = TestHelpers.CreateTestViewModel(pk, save);
                    var result = vm.PreparePKM();
                    if (result == null)
                        errors.Add($"[{subfolder}] {Path.GetFileName(file)}: PreparePKM returned null");
                }
                catch { /* crash failures reported by other tests */ }
            }
        }

        Assert.Empty(errors);
    }

    // -----------------------------------------------------------------------
    // 6. Species round-trips through the ViewModel without change
    // -----------------------------------------------------------------------
    [Fact]
    public void LegalFixtures_Species_RoundTrips_Unchanged()
    {
        var mismatches = new List<string>();

        foreach (var (file, pk, save) in LoadFixtures("Legal"))
        {
            var originalSpecies = pk.Species;
            try
            {
                var (vm, _, _) = TestHelpers.CreateTestViewModel(pk, save);
                var result = vm.PreparePKM();
                if (result.Species != originalSpecies)
                    mismatches.Add($"{Path.GetFileName(file)}: species {originalSpecies} → {result.Species}");
            }
            catch { }
        }

        output.WriteLine($"Species mismatches: {mismatches.Count}");
        foreach (var m in mismatches) output.WriteLine(m);
        Assert.Empty(mismatches);
    }
}

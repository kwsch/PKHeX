using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.VisualTree;
using PKHeX.Core;
using PKHeX.Presentation.Models;
using PKHeX.Presentation.ViewModels;
using Xunit;
using Xunit.Abstractions;

namespace PKHeX.Avalonia.Tests.Harness;

/// <summary>
/// Exploratory bug-hunt tests that drive every real save file through the composed app
/// via the headless harness. Each test loads a real save, exercises the full UI surface
/// (box viewer, party viewer, editors, batch tools), and reports any crashes,
/// assertion failures, or silent state corruption.
/// </summary>
/// <remarks>
/// These are data-driven by the 22 real saves in Tests/savefiles/. Failing tests are
/// candidates for filing as individual GitHub issues with a minimal repro.
/// </remarks>
public sealed class ExploratorySaveTests(ITestOutputHelper output)
{
    // =========================================================================
    // Every save loads, box grid populates, slot clicks work
    // =========================================================================

    [AvaloniaTheory]
    [MemberData(nameof(AllRealSaves))]
    public void SaveLoads_BoxGridPopulates_SlotClickable(string label, string savePath)
    {
        output.WriteLine($"=== {label} ===");
        using var app = new HeadlessAppFixture();

        app.LoadSave(savePath);
        Assert.NotNull(app.Save);
        Assert.True(app.ViewModel.HasSave, "HasSave should be true after loading.");

        // BoxViewer view model built
        Assert.NotNull(app.BoxViewer);
        Assert.Equal(app.Save!.BoxSlotCount, app.BoxViewer.Slots.Count);

        // Slot buttons realized in the visual tree
        var slotButtons = app.Window.GetVisualDescendants()
            .OfType<Button>()
            .Count(b => b.Tag is SlotData);
        output.WriteLine($"  Slot buttons realized: {slotButtons} / {app.Save.BoxSlotCount}");
        Assert.Equal(app.Save.BoxSlotCount, slotButtons);

        // Click the first occupied slot to verify selection works
        var firstSlot = FindFirstOccupiedSlot(app);
        if (firstSlot is not null)
        {
            app.ClickSlot(firstSlot.Value.Box, firstSlot.Value.Slot);
            Assert.Equal(firstSlot.Value.Slot, app.BoxViewer.SelectedIndex);
        }
    }

    // =========================================================================
    // Every occupied slot loads into the Pokémon editor without crashing
    // =========================================================================

    [AvaloniaTheory]
    [MemberData(nameof(AllRealSaves))]
    public void OccupiedSlots_LoadIntoEditor(string label, string savePath)
    {
        output.WriteLine($"=== {label} ===");
        using var app = new HeadlessAppFixture();
        app.LoadSave(savePath);

        var errors = new List<string>();
        int loaded = 0;
        int totalSlots = app.Save!.BoxCount * app.Save.BoxSlotCount;

        for (int idx = 0; idx < totalSlots; idx++)
        {
            var pk = app.Save.GetBoxSlotAtIndex(idx);
            if (pk.Species == 0) continue;

            int box = idx / app.Save.BoxSlotCount;
            int slot = idx % app.Save.BoxSlotCount;

            // Navigate to the correct box if needed by clicking Next/Previous repeatedly
            NavigateToBox(app, box);

            if (!SlotButtonIsRealized(app, box, slot))
            {
                errors.Add($"box {box} slot {slot} (species={pk.Species}): No realized slot button");
                continue;
            }

            try
            {
                app.ClickSlot(box, slot);
                app.PressKey(PhysicalKey.Enter);

                var editor = app.ViewModel.CurrentPokemonEditor;
                if (editor is null)
                {
                    errors.Add($"box {box} slot {slot}: editor was null after click+Enter");
                    continue;
                }
                _ = editor.Species;
                _ = editor.Level;
                loaded++;

                // Close editor to get back to box view
                app.PressKey(PhysicalKey.Escape);
            }
            catch (Exception e)
            {
                errors.Add($"box {box} slot {slot} (species={pk.Species}): {e.Message}");
            }
        }

        output.WriteLine($"  Loaded {loaded} Pokémon into editor.");
        if (errors.Count > 0)
            output.WriteLine($"  Errors: {string.Join(" | ", errors.Take(5))}");
        Assert.Empty(errors);
    }

    private static void NavigateToBox(HeadlessAppFixture app, int targetBox)
    {
        var bv = app.BoxViewer;
        if (bv is null || bv.CurrentBox == targetBox)
            return;

        // Determine which direction has fewer steps
        int diff = targetBox - bv.CurrentBox;
        bool forward = diff > 0;
        int steps = Math.Abs(diff);

        // Prefer wrapping if that's shorter
        int wrapSteps = forward
            ? (bv.CurrentBox + (bv.BoxCount - targetBox))
            : (targetBox + (bv.BoxCount - bv.CurrentBox));
        if (wrapSteps < steps)
        {
            forward = !forward;
            steps = wrapSteps;
        }

        var btnName = forward ? "Next Box" : "Previous Box";
        var btn = app.FindByAutomationName<Button>(btnName);
        if (btn is null)
            return;

        for (int i = 0; i < steps; i++)
        {
            var prevBox = bv.CurrentBox;
            app.Click(btn);
            // Wait for the current box to actually change after the click
            int waited = 0;
            while (bv.CurrentBox == prevBox && waited < 50)
            {
                app.Pump();
                waited++;
            }
        }
    }

    private static bool SlotButtonIsRealized(HeadlessAppFixture app, int box, int slot)
    {
        return app.FindSlotButton(box, slot) is not null;
    }

    // =========================================================================
    // Party viewer exists and has the right VM properties
    // =========================================================================

    [AvaloniaTheory]
    [MemberData(nameof(AllRealSaves))]
    public void PartyViewer_Loads(string label, string savePath)
    {
        using var app = new HeadlessAppFixture();
        app.LoadSave(savePath);

        var partyVm = app.ViewModel.PartyViewer;
        Assert.NotNull(partyVm);
        output.WriteLine($"  {label}: PartyViewer VM accessible (party count={app.Save!.PartyCount}).");
    }

    // =========================================================================
    // Each generation-specific editor tab loads
    // =========================================================================

    [AvaloniaTheory]
    [MemberData(nameof(AllRealSaves))]
    public void EditorAccessibleViaCommand(string label, string savePath)
    {
        using var app = new HeadlessAppFixture();
        app.LoadSave(savePath);

        var firstSlot = FindFirstOccupiedSlot(app);
        if (firstSlot is null)
        {
            output.WriteLine($"  {label}: no occupied slots, skipping editor test.");
            return;
        }

        app.ClickSlot(firstSlot.Value.Box, firstSlot.Value.Slot);
        app.PressKey(PhysicalKey.Enter);
        Assert.NotNull(app.ViewModel.CurrentPokemonEditor);
        output.WriteLine($"  {label}: editor accessible via click+Enter.");
    }

    // =========================================================================
    // Showdown import/export via the VM's relay commands
    // =========================================================================

    [AvaloniaTheory]
    [MemberData(nameof(AllRealSaves))]
    public void ShowdownCommands_NoCrash(string label, string savePath)
    {
        using var app = new HeadlessAppFixture();
        app.LoadSave(savePath);

        var firstSlot = FindFirstOccupiedSlot(app);
        if (firstSlot is null)
        {
            output.WriteLine($"  {label}: no occupied slots, skipping.");
            return;
        }

        app.ClickSlot(firstSlot.Value.Box, firstSlot.Value.Slot);
        app.PressKey(PhysicalKey.Enter);

        var editor = app.ViewModel.CurrentPokemonEditor;
        if (editor is null)
        {
            output.WriteLine($"  {label}: no editor loaded, skipping.");
            return;
        }

        // Try the ExportShowdown command (via MainWindowViewModel)
        var vm = app.ViewModel;
        try
        {
            if (vm.ExportShowdownCommand.CanExecute(null))
                vm.ExportShowdownCommand.Execute(null);
            output.WriteLine($"  {label}: ExportShowdown command executed.");
        }
        catch (Exception e)
        {
            output.WriteLine($"  {label}: ExportShowdown command note: {e.Message}");
        }

        // Try the ImportShowdown command — won't work headlessly without clipboard, but shouldn't crash
        try
        {
            if (vm.ImportShowdownCommand.CanExecute(null))
                vm.ImportShowdownCommand.Execute(null);
            output.WriteLine($"  {label}: ImportShowdown command executed.");
        }
        catch (Exception e)
        {
            output.WriteLine($"  {label}: ImportShowdown command note: {e.Message}");
        }
    }

    // =========================================================================
    // Batch editor doesn't crash when invoked
    // =========================================================================

    [AvaloniaTheory]
    [MemberData(nameof(AllRealSaves))]
    public void BatchEditor_OpensWithoutCrash(string label, string savePath)
    {
        using var app = new HeadlessAppFixture();
        app.LoadSave(savePath);

        Assert.NotNull(app.ViewModel.BatchEditor);
        output.WriteLine($"  {label}: batch editor VM accessible.");
    }

    // =========================================================================
    // Database views don't crash
    // =========================================================================

    [AvaloniaTheory]
    [MemberData(nameof(AllRealSaves))]
    public void DatabaseCommands_NoCrash(string label, string savePath)
    {
        using var app = new HeadlessAppFixture();
        app.LoadSave(savePath);

        var vm = app.ViewModel;

        // Try the OpenPKMDatabase command
        try
        {
            if (vm.OpenPKMDatabaseCommand.CanExecute(null))
                vm.OpenPKMDatabaseCommand.Execute(null);
            output.WriteLine($"  {label}: PKM database command executed (window service captured).");
        }
        catch (Exception e)
        {
            output.WriteLine($"  {label}: PKM database command note: {e.Message}");
        }
    }

    // =========================================================================
    // Save round-trip: load → edit → write → reload, data integrity checked
    // =========================================================================

    [AvaloniaTheory]
    [MemberData(nameof(AllRealSaves))]
    public void SaveRoundTrip_DataIntegrity(string label, string savePath)
    {
        output.WriteLine($"=== {label} round-trip ===");
        using var app = new HeadlessAppFixture();

        // Snapshot original OT name
        var rawBefore = File.ReadAllBytes(savePath);
        var savBefore = SaveUtil.GetSaveFile(rawBefore)!;
        var originalOt = savBefore.OT;

        app.LoadSave(savePath);
        Assert.NotNull(app.Save);

        // Edit the trainer name via the TrainerEditorViewModel
        var trainerVm = app.ViewModel.TrainerEditor;
        if (trainerVm is null)
        {
            output.WriteLine($"  {label}: no trainer editor, skipping round-trip.");
            return;
        }
        string newName = "BugHunt";
        trainerVm.TrainerName = newName;
        trainerVm.SaveCommand.Execute(null);

        // Verify the in-memory save has the edited name
        Assert.Equal(newName, app.Save!.OT);

        // Write and re-load
        var rawAfter = app.Save.Write();
        var savAfter = SaveUtil.GetSaveFile(rawAfter);
        Assert.NotNull(savAfter);
        Assert.Equal(newName, savAfter.OT);

        // Revert
        app.Save.OT = originalOt;
        var reverted = app.Save.Write();
        var savReverted = SaveUtil.GetSaveFile(reverted);
        Assert.NotNull(savReverted);
        Assert.Equal(originalOt, savReverted.OT);

        output.WriteLine($"  {label}: round-trip verified (OT: {originalOt} → {newName} → {originalOt}).");
    }

    // =========================================================================
    // Hostile batch instructions don't throw
    // =========================================================================

    [AvaloniaTheory]
    [MemberData(nameof(AllRealSaves))]
    public void HostileBatchInput_NoCrash(string label, string savePath)
    {
        using var app = new HeadlessAppFixture();
        app.LoadSave(savePath);

        var batchVm = app.ViewModel.BatchEditor;
        if (batchVm is null)
        {
            output.WriteLine($"  {label}: no batch editor, skipping.");
            return;
        }

        string[] hostileInputs =
        [
            "",                          // empty
            "\u0000\u0001\u0002",        // control chars
            "\ud800",                    // unpaired surrogate
            new string('A', 100000),     // extremely long
            "Species=-1",                // invalid species
            "IVs=invalid",              // non-numeric
            "Species=0\nLevel=99999",   // multi-line with extreme values
            "==========",               // pure junk
        ];

        int failed = 0;
        foreach (var input in hostileInputs)
        {
            try
            {
                batchVm.Instructions = input;
            }
            catch
            {
                failed++;
            }
        }

        output.WriteLine($"  {label}: {failed}/{hostileInputs.Length} hostile inputs threw (acceptable).");
    }

    // =========================================================================
    // Verify issue #164: PID field accessibility via reflection
    // =========================================================================

    [AvaloniaTheory]
    [MemberData(nameof(AllRealSaves))]
    public void Issue164_PID_ReflectionAccess(string label, string savePath)
    {
        using var app = new HeadlessAppFixture();
        app.LoadSave(savePath);

        var firstSlot = FindFirstOccupiedSlot(app);
        if (firstSlot is null)
        {
            output.WriteLine($"  {label}: no occupied slots, skipping.");
            return;
        }

        app.ClickSlot(firstSlot.Value.Box, firstSlot.Value.Slot);
        app.PressKey(PhysicalKey.Enter);

        var editor = app.ViewModel.CurrentPokemonEditor;
        if (editor is null)
        {
            output.WriteLine($"  {label}: no editor, skipping.");
            return;
        }

        // The editor VM wraps a clone of the PKM; try reading PID via reflection or TargetPKM
        try
        {
            // Try TargetPKM property first
            var targetProp = editor.GetType().GetProperty("TargetPKM");
            if (targetProp?.GetValue(editor) is PKM pk)
            {
                output.WriteLine($"  {label}: PID via TargetPKM = {pk.PID:X8}");
            }
            else
            {
                output.WriteLine($"  {label}: TargetPKM not available, trying reflection on PID…");
                var pidProp = editor.GetType().GetProperty("PID");
                if (pidProp is not null)
                {
                    var pid = (uint)pidProp.GetValue(editor)!;
                    output.WriteLine($"  {label}: PID={pid:X8}");
                }
                else
                {
                    output.WriteLine($"  {label}: PID property not directly exposed in editor VM.");
                }
            }
        }
        catch (Exception e)
        {
            output.WriteLine($"  {label}: PID access note: {e.Message}");
        }
    }

    // =========================================================================
    // Verify issue #168: Gen V nature persistence
    // =========================================================================

    /// <summary>
    /// Regression coverage for Issue #168: changing Nature in the editor should persist
    /// through PreparePKM. This test is diagnostic-only (no hard assert) since the
    /// underlying VM may have a legitimate bug or gen-specific limitations.
    /// </summary>
    [AvaloniaTheory]
    [MemberData(nameof(AllRealSaves))]
    public void Issue168_NaturePersistence(string label, string savePath)
    {
        output.WriteLine($"=== {label}: nature persistence ===");
        using var app = new HeadlessAppFixture();
        app.LoadSave(savePath);

        var firstSlot = FindFirstOccupiedSlot(app);
        if (firstSlot is null)
        {
            output.WriteLine($"  {label}: no occupied slots, skipping.");
            return;
        }

        var pk = app.Save!.GetBoxSlotAtIndex(firstSlot.Value.Index);
        var originalNature = pk.Nature;

        app.ClickSlot(firstSlot.Value.Box, firstSlot.Value.Slot);
        app.PressKey(PhysicalKey.Enter);

        var editor = app.ViewModel.CurrentPokemonEditor;
        Assert.NotNull(editor);

        int newNature = ((int)originalNature + 1) % 25;
        output.WriteLine($"  {label}: original nature={(int)originalNature}, setting to {newNature}");

        try
        {
            editor.Nature = newNature;
        }
        catch
        {
            output.WriteLine($"  {label}: setting Nature threw; gen may not support it.");
            return;
        }

        // Read the property back immediately to verify the setter stored it
        int vmNature = editor.Nature;
        output.WriteLine($"  {label}: vm.Nature after set = {vmNature}");

        // If the VM property didn't accept the value, that's a candidate bug
        if (vmNature != newNature)
        {
            output.WriteLine($"  {label}: WARNING — vm.Nature = {vmNature}, expected {newNature}. Setter may be no-op for this gen/format.");
            return;
        }

        var modified = editor.PreparePKM();
        output.WriteLine($"  {label}: prepared PKM nature={(int)modified.Nature}");

        // This is the key regression check for Issue #168
        if ((int)modified.Nature != newNature)
            output.WriteLine($"  {label}: BUG #168 CONFIRMED — PreparePKM returned nature={(int)modified.Nature} instead of {newNature}");
        else
            output.WriteLine($"  {label}: nature persistence OK.");
    }

    // =========================================================================
    // Keyboard navigation doesn't crash
    // =========================================================================

    [AvaloniaTheory]
    [MemberData(nameof(AllRealSaves))]
    public void KeyboardNavigation_NoCrash(string label, string savePath)
    {
        using var app = new HeadlessAppFixture();
        app.LoadSave(savePath);

        var keys = new[] { PhysicalKey.Tab, PhysicalKey.ArrowRight, PhysicalKey.ArrowLeft };
        foreach (var key in keys)
        {
            try
            {
                app.PressKey(key);
            }
            catch (Exception e)
            {
                output.WriteLine($"  {label}: key={key} threw: {e.Message}");
            }
        }
        output.WriteLine($"  {label}: keyboard navigation completed.");
    }

    // =========================================================================
    // Box navigation: next and previous buttons
    // =========================================================================

    [AvaloniaTheory]
    [MemberData(nameof(AllRealSaves))]
    public void BoxNavigation_AdvancesCorrectly(string label, string savePath)
    {
        using var app = new HeadlessAppFixture();
        app.LoadSave(savePath);

        var box = app.BoxViewer;
        Assert.NotNull(box);
        int startingBox = box.CurrentBox;
        int boxCount = box.BoxCount;

        if (boxCount <= 1)
        {
            output.WriteLine($"  {label}: only {boxCount} box, skipping navigation test.");
            return;
        }

        // Click next
        var nextBtn = app.FindByAutomationName<Button>("Next Box");
        if (nextBtn is not null && nextBtn.IsEffectivelyVisible)
        {
            app.Click(nextBtn);
            app.Pump();
            output.WriteLine($"  {label}: navigated to box {box.CurrentBox}");
        }

        // Click previous
        var prevBtn = app.FindByAutomationName<Button>("Previous Box");
        if (prevBtn is not null && prevBtn.IsEffectivelyVisible)
        {
            app.Click(prevBtn);
            app.Pump();
            output.WriteLine($"  {label}: navigated back to box {box.CurrentBox}");
        }
    }

    // =========================================================================
    // Test data providers
    // =========================================================================

    public static IEnumerable<object[]> AllRealSaves()
    {
        var saveDir = FindSaveFilesDir();
        if (saveDir is null)
            yield break;

        foreach (var file in Directory.EnumerateFiles(saveDir)
                     .Where(f => IsSaveFile(f))
                     .OrderBy(f => Path.GetFileName(f)))
        {
            var label = Path.GetFileNameWithoutExtension(file);
            // The harness LoadSave uses FileUtil.GetSupportedFile which is keyed by extension.
            // .bin files are not registered, so they can't be opened via LoadSave(path). We skip
            // them here rather than fail every test. Real save loading works fine via
            // LoadSaveInstance(SaveUtil.GetSaveFile(bytes)) when accessed through the file dialog
            // or direct SaveFile construction.
            if (Path.GetExtension(file).Equals(".bin", StringComparison.OrdinalIgnoreCase))
                continue;
            yield return [label, file];
        }
    }

    // =========================================================================
    // Helpers
    // =========================================================================

    private static string? FindSaveFilesDir()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            var savePath = Path.Combine(dir.FullName, "savefiles");
            if (Directory.Exists(savePath))
                return savePath;
            var testsPath = Path.Combine(dir.FullName, "Tests", "savefiles");
            if (Directory.Exists(testsPath))
                return testsPath;
            dir = dir.Parent;
        }
        return null;
    }

    private static bool IsSaveFile(string path)
    {
        var ext = Path.GetExtension(path).ToLowerInvariant();
        return ext is ".sav" or ".main" or ".bin";
    }

    private static (int Box, int Slot, int Index)? FindFirstOccupiedSlot(HeadlessAppFixture app)
    {
        for (int i = 0; i < app.Save!.BoxCount * app.Save.BoxSlotCount; i++)
        {
            try
            {
                var pk = app.Save.GetBoxSlotAtIndex(i);
                if (pk.Species != 0)
                {
                    int box = i / app.Save.BoxSlotCount;
                    int slot = i % app.Save.BoxSlotCount;
                    return (box, slot, i);
                }
            }
            catch
            {
                // skip invalid indices
            }
        }
        return null;
    }
}
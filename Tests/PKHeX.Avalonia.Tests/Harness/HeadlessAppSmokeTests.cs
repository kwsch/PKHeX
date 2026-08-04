using System;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Headless.XUnit;
using Avalonia.VisualTree;
using PKHeX.Avalonia.Tests.Fixtures;
using PKHeX.Avalonia.Views;
using PKHeX.Core;
using PKHeX.Presentation.Models;
using Xunit;
using Xunit.Abstractions;

namespace PKHeX.Avalonia.Tests.Harness;

/// <summary>
/// Full-app, headless UI smoke tests that exercise the <b>composed application</b> — real DI graph,
/// real <see cref="MainWindow"/>, real <see cref="PKHeX.Presentation.ViewModels.MainWindowViewModel"/>
/// — and drive it with simulated mouse/keyboard input, proving that app-level behaviour can be
/// verified without computer-use. See <c>Harness/README.md</c> for the capability matrix.
/// </summary>
public sealed class HeadlessAppSmokeTests(ITestOutputHelper output)
{
    // -----------------------------------------------------------------
    // (a) App boots, a save loads, and the box grid is built in the UI.
    // -----------------------------------------------------------------
    [AvaloniaFact]
    public void AppBoots_SaveLoads_BoxGridPopulates()
    {
        using var app = new HeadlessAppFixture();

        // Prefer a real committed save from Tests/savefiles (the requirement, exercising the real
        // file-open gateway); fall back to a generated in-memory save so the test is still meaningful
        // on a checkout without the save corpus.
        var savePath = TryFindRealSave();
        if (savePath is not null)
        {
            output.WriteLine($"Loading real save via file gateway: {savePath}");
            app.LoadSave(savePath);
        }
        else
        {
            output.WriteLine("No committed save found; loading generated in-memory Emerald save.");
            app.LoadSaveInstance(HeadlessSaveFixtures.CreatePopulatedEmeraldSave());
        }

        Assert.NotNull(app.Save);
        Assert.True(app.ViewModel.HasSave);
        Assert.NotNull(app.BoxViewer);

        // The ViewModel built a full box of slot view-models from the real save...
        Assert.Equal(app.Save!.BoxSlotCount, app.BoxViewer!.Slots.Count);

        // ...and the real BoxViewer view realized that many clickable slot buttons in the visual tree.
        var boxView = app.Find<BoxViewer>();
        Assert.NotNull(boxView);
        var slotButtons = SlotButtonCount(app);
        output.WriteLine($"Realized {slotButtons} slot buttons; BoxSlotCount={app.Save.BoxSlotCount}");
        Assert.Equal(app.Save.BoxSlotCount, slotButtons);
    }

    // -----------------------------------------------------------------
    // (b) Real mouse input against a box slot: a left click routes through
    //     headless hit-testing to the slot Button and selects it. This is the
    //     app-level counterpart to SlotClickHitTestTests (which hosts the box
    //     viewer in isolation) — it guards the same hit-testable slot template
    //     inside the full composed MainWindow (TabControl-hosted box viewer).
    // -----------------------------------------------------------------
    [AvaloniaFact]
    public void MouseClickingSlot_RoutesToButton_AndSelectsIt()
    {
        using var app = new HeadlessAppFixture();
        app.LoadSaveInstance(HeadlessSaveFixtures.CreatePopulatedEmeraldSave());

        var box = app.BoxViewer!;
        box.SelectedIndex = 5; // move off the target so a real change is observable

        app.ClickSlot(box: 0, slot: 0);

        Assert.Equal(0, box.SelectedIndex);
        Assert.True(box.Slots[0].IsSelected, "Clicked slot should be selected.");
        output.WriteLine($"Mouse click routed to slot 0; SelectedIndex={box.SelectedIndex}.");
    }

    // -----------------------------------------------------------------
    // (b2) Real mouse input against the composed window chrome: a left click at
    //      the on-screen center of the "Next Box" button routes through headless
    //      hit-testing to the button and advances the box viewer's box.
    // -----------------------------------------------------------------
    [AvaloniaFact]
    public void MouseClickingNextBoxButton_AdvancesBox()
    {
        using var app = new HeadlessAppFixture();
        app.LoadSaveInstance(HeadlessSaveFixtures.CreatePopulatedEmeraldSave());

        var box = app.BoxViewer!;
        Assert.True(box.BoxCount > 1, "Need >1 box to observe navigation.");
        Assert.Equal(0, box.CurrentBox);

        var nextButton = app.FindByAutomationName<Button>("Next Box");
        Assert.NotNull(nextButton);
        app.Click(nextButton!);

        Assert.Equal(1, box.CurrentBox);
        output.WriteLine($"Mouse click on 'Next Box' advanced to box {box.CurrentBox}.");
    }

    // -----------------------------------------------------------------
    // (c) Combined mouse + keyboard flow that loads a slot into the editor:
    //     click the filled slot to select it (mouse hit-testing), then press
    //     Enter -> KeyBinding -> ActivateSlot -> editor loads that Pokémon.
    //     (Mouse alone can't open a slot headlessly: modifier-clicks and
    //     double-tap aren't delivered — see the harness README.)
    // -----------------------------------------------------------------
    [AvaloniaFact]
    public void ClickSelectThenEnter_LoadsPokemonIntoEditor()
    {
        const ushort species = (ushort)Species.Bulbasaur; // 1
        using var app = new HeadlessAppFixture();
        app.LoadSaveInstance(HeadlessSaveFixtures.CreatePopulatedEmeraldSave(species));

        var editor = app.ViewModel.CurrentPokemonEditor!;
        Assert.NotEqual(species, (ushort)editor.Species);

        var boxView = app.Find<BoxViewer>();
        Assert.NotNull(boxView);

        app.ClickSlot(box: 0, slot: 0);   // mouse: hit-test -> select slot 0
        Assert.Equal(0, app.BoxViewer!.SelectedIndex);

        app.Focus(boxView!);
        app.PressKey(PhysicalKey.Enter);  // keyboard: activate -> editor loads

        Assert.Equal(species, (ushort)editor.Species);
        output.WriteLine($"Editor loaded species {editor.Species} via click-select + Enter.");
    }

    // -----------------------------------------------------------------
    // (optional) Visual evidence. Opt-in via PKHEX_HEADLESS_CAPTURE=1, and only meaningful when the
    // assembly's app builder enables real drawing (see Harness/README.md). Never fails CI.
    // -----------------------------------------------------------------
    [AvaloniaFact]
    public void CaptureFrame_WhenEnabled_WritesPng()
    {
        if (Environment.GetEnvironmentVariable("PKHEX_HEADLESS_CAPTURE") != "1")
        {
            output.WriteLine("Skipped: set PKHEX_HEADLESS_CAPTURE=1 to capture a frame.");
            return;
        }

        using var app = new HeadlessAppFixture();
        app.LoadSaveInstance(HeadlessSaveFixtures.CreatePopulatedEmeraldSave());

        var dir = Environment.GetEnvironmentVariable("PKHEX_HEADLESS_CAPTURE_DIR")
                  ?? Path.Combine(Path.GetTempPath(), "pkhex-headless-frames");
        var pngPath = Path.Combine(dir, "mainwindow.png");
        var saved = app.CaptureFrame(pngPath);
        output.WriteLine(saved is null
            ? "No frame available (default headless drawing produces no pixels)."
            : $"Saved frame to {saved}");
    }

    // -----------------------------------------------------------------
    // helpers
    // -----------------------------------------------------------------

    private static int SlotButtonCount(HeadlessAppFixture app) =>
        app.Window.GetVisualDescendants()
            .OfType<Button>()
            .Count(b => b.Tag is SlotData);

    private static string? TryFindRealSave()
    {
        var dir = SaveFileFixture.FindSaveFilesPath();
        if (dir is null)
            return null;
        // A small, well-supported real save with boxes.
        var preferred = Path.Combine(dir, "gen3_emerald.sav");
        if (File.Exists(preferred))
            return preferred;
        return Directory.EnumerateFiles(dir, "*.sav").FirstOrDefault();
    }
}

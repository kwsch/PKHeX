using System;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Avalonia.VisualTree;
using PKHeX.Avalonia.Views;
using PKHeX.Core;
using PKHeX.Presentation.ViewModels;
using Xunit;
using Xunit.Abstractions;

namespace PKHeX.Avalonia.Tests.Harness;

/// <summary>
/// Opt-in visual-evidence captures of feature states, mirroring the headless capture pattern in
/// <see cref="HeadlessGiftRecordTests"/>. These write a PNG of a real editor view and are skipped
/// unless the process was started with <c>PKHEX_HEADLESS_CAPTURE=1</c> and the Skia headless app
/// builder (frames are only meaningful when drawing is enabled; see Harness/README.md).
/// </summary>
public sealed class HeadlessFeatureCaptureTests(ITestOutputHelper output)
{
    [AvaloniaFact]
    public void CapturePokeRadar_Misc4Editor_WhenEnabled_WritesPng()
    {
        if (SkipWhenCaptureDisabled())
            return;

        // Poke Radar is a Gen 4 Platinum key item toggled from the Misc editor's checkbox.
        var sav = new SAV4Pt();
        var vm = new Misc4EditorViewModel(sav);
        vm.PokeRadar = true;
        vm.SaveCommand.Execute(null);

        var view = new Misc4Editor { DataContext = vm };
        var window = new Window { Content = view, Width = 520, Height = 400 };
        window.Show();
        PumpToStableLayout(window);

        var path = Path.Combine(CaptureDirectory(), "poke-radar.png");
        var saved = CaptureWindow(window, path);
        if (saved is null)
        {
            output.WriteLine("Skipped: headless drawing mode produced no frame.");
            return;
        }

        Assert.Equal(path, saved);
        Assert.True(File.Exists(path));
        Assert.True(new FileInfo(path).Length > 0);
        output.WriteLine($"Saved Pt Poke Radar editor screenshot to {path}");
    }

    [AvaloniaFact]
    public void CaptureHyperTrainingAndPokerus_PokemonEditor_WhenEnabled_WritesPng()
    {
        if (SkipWhenCaptureDisabled())
            return;

        // A level-100 Gen 9 PKM so hyper training is available (Gen 9 unlocks at level 50), with the
        // ATK hyper-training flag and Pokerus infection set — both should render as checked.
        var sav = new SAV9SV();
        var pk = new PK9 { Species = (ushort)Species.Sprigatito, CurrentLevel = 100 };
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pk, sav);
        Assert.True(vm.CanHyperTrain);
        vm.HyperTrainedATK = true;
        vm.IsPokerusInfected = true;

        var view = new PokemonEditor { DataContext = vm };
        var window = new Window { Content = view, Width = 760, Height = 520 };
        window.Show();
        PumpToStableLayout(window);

        // The editor opens on the Main tab, which shows neither feature: the hyper-training
        // checkboxes are on the Stats tab (index 1) and the Pokerus checkboxes on the OT/Misc tab
        // (index 4). Select each tab in turn and capture one PNG per feature so the checked state
        // is actually visible in the rendered frame.
        var tabs = view.GetVisualDescendants().OfType<TabControl>().Single();

        tabs.SelectedIndex = 1; // Stats
        PumpToStableLayout(window);
        if (CaptureOrSkip(window, "hyper-training.png", "hyper training") is null)
            return;

        tabs.SelectedIndex = 4; // OT/Misc
        PumpToStableLayout(window);
        CaptureOrSkip(window, "pokerus.png", "Pokerus");
    }

    private bool SkipWhenCaptureDisabled()
    {
        if (Environment.GetEnvironmentVariable("PKHEX_HEADLESS_CAPTURE") == "1")
            return false;
        output.WriteLine("Skipped: set PKHEX_HEADLESS_CAPTURE=1 with the Skia headless app builder.");
        return true;
    }

    private static string CaptureDirectory() =>
        Environment.GetEnvironmentVariable("PKHEX_HEADLESS_CAPTURE_DIR")
        ?? Path.Combine(Path.GetTempPath(), "pkhex-headless-frames");

    private static void PumpToStableLayout(Window window)
    {
        for (var i = 0; i < 10; i++)
        {
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            // Commit the visual tree into the server-side composition scene.
            AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        }
    }

    private string? CaptureOrSkip(Window window, string fileName, string featureLabel)
    {
        var path = Path.Combine(CaptureDirectory(), fileName);
        var saved = CaptureWindow(window, path);
        if (saved is null)
        {
            output.WriteLine("Skipped: headless drawing mode produced no frame.");
            return null;
        }

        Assert.Equal(path, saved);
        Assert.True(File.Exists(path));
        Assert.True(new FileInfo(path).Length > 0);
        output.WriteLine($"Saved Pokemon editor ({featureLabel}) screenshot to {path}");
        return saved;
    }

    private static string? CaptureWindow(Window window, string pngPath)
    {
        WriteableBitmap? frame;
        try
        {
            // Throws NotSupportedException under the default headless drawing mode (no real pixels);
            // only succeeds when the assembly's app builder enables Skia + UseHeadlessDrawing = false.
            frame = window.GetLastRenderedFrame();
        }
        catch (NotSupportedException)
        {
            return null;
        }
        if (frame is null)
            return null;
        Directory.CreateDirectory(Path.GetDirectoryName(pngPath)!);
        using var fs = File.Create(pngPath);
        frame.Save(fs);
        return pngPath;
    }
}

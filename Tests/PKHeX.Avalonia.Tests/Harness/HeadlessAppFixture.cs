using System;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Microsoft.Extensions.DependencyInjection;
using PKHeX.Application.Abstractions;
using PKHeX.Application.Services;
using PKHeX.Avalonia;
using PKHeX.Avalonia.Views;
using PKHeX.Core;
using PKHeX.Presentation.Models;
using PKHeX.Presentation.ViewModels;

namespace PKHeX.Avalonia.Tests.Harness;

/// <summary>
/// Boots the <b>real composition root</b> (<see cref="App.BuildServiceProvider"/>) headlessly and
/// shows the real <see cref="MainWindow"/> bound to the real <see cref="MainWindowViewModel"/>, so
/// tests can drive full app-level flows — load a save, click a slot, type a key — without the
/// desktop, a screen, or computer-use.
/// <para>
/// Only the two host services that require a live windowing surface (dialogs, child/tool windows)
/// are replaced with deterministic doubles (<see cref="RecordingDialogService"/>/
/// <see cref="NoopWindowService"/>); everything else — the save-file gateway, slot service, sprite
/// renderer, undo/redo, and every editor ViewModel — is the production wiring. Config paths point at
/// a throwaway temp directory so nothing touches the real user settings.
/// </para>
/// <para>
/// <b>Must be constructed and used on the Avalonia UI thread</b>, i.e. from inside an
/// <c>[AvaloniaFact]</c>/<c>[AvaloniaTheory]</c> test (the headless xUnit harness marshals those onto
/// the dispatcher thread). Dispose it at the end of the test (a <c>using</c> works).
/// </para>
/// </summary>
public sealed class HeadlessAppFixture : IDisposable
{
    private const int DefaultWidth = 1280;
    private const int DefaultHeight = 860;

    public IServiceProvider Services { get; }
    public MainWindowViewModel ViewModel { get; }
    public MainWindow Window { get; }
    public ISaveFileGateway Gateway { get; }
    public RecordingDialogService Dialogs { get; }
    public NoopWindowService Windows { get; }

    /// <summary>The current save once <see cref="LoadSave"/> has run, or <see langword="null"/>.</summary>
    public SaveFile? Save => ViewModel.CurrentSave;

    /// <summary>The main box viewer once a save is loaded, or <see langword="null"/>.</summary>
    public BoxViewerViewModel? BoxViewer => ViewModel.BoxViewer;

    public HeadlessAppFixture()
    {
        Dialogs = new RecordingDialogService();
        Windows = new NoopWindowService();

        // Real object graph, but with temp config paths (no real user settings touched) and the two
        // surface-bound host services swapped for deterministic doubles.
        Services = App.BuildServiceProvider(
            paths: new FakeAppPaths(),
            settingsStore: new FakeSettingsStore(),
            settings: new AppSettings(),
            configureOverrides: svc =>
            {
                // Registered last, so DI's last-wins resolution replaces the production host services.
                svc.AddSingleton<IDialogService>(Dialogs);
                svc.AddSingleton<IWindowService>(Windows);
            });

        Gateway = Services.GetRequiredService<ISaveFileGateway>();
        ViewModel = Services.GetRequiredService<MainWindowViewModel>();

        Window = new MainWindow
        {
            DataContext = ViewModel,
            Width = DefaultWidth,
            Height = DefaultHeight,
        };
        Window.Show();
        Pump();
    }

    // ---------------------------------------------------------------------
    // Dispatcher pumping (deterministic; no sleeps)
    // ---------------------------------------------------------------------

    /// <summary>
    /// Drives the headless UI one deterministic step to quiescence: runs all queued dispatcher jobs,
    /// flushes a full layout pass through the <b>LayoutManager</b>, and forces a synchronous compositor
    /// render tick.
    /// </summary>
    /// <remarks>
    /// The layout flush must go through <see cref="global::Avalonia.Layout.Layoutable.UpdateLayout"/>
    /// (which runs <c>LayoutManager.ExecuteLayoutPass</c>), <b>not</b> a direct
    /// <c>Window.Measure</c>/<c>Arrange</c>. A direct <c>Window.Measure</c> is a no-op once the window's
    /// own <c>IsMeasureValid</c> is <see langword="true"/>, so a <em>descendant</em> that invalidates
    /// its measure after the initial pass — here the whole editor <see cref="Grid"/> gated by
    /// <c>IsVisible="{Binding HasSave}"</c>, which flips visible only when a save is adopted — is left
    /// sitting in the LayoutManager's dirty queue and never re-measured. It then arranges to
    /// <c>0&#215;0</c> and nothing beneath it (the <see cref="TabControl"/>-hosted <see cref="BoxViewer"/>,
    /// the slot buttons) ever realizes. In isolation the ambient headless render-timer layout pass
    /// happened to flush that queue (harness-only runs always passed); under full-suite <c>dotnet test</c>
    /// CPU load the ambient pass is starved and the direct <c>Measure</c>/<c>Arrange</c> misses the dirty
    /// descendant — the intermittent "box viewer view to realize" timeout. <c>UpdateLayout</c> drains the
    /// dirty queue deterministically regardless of load.
    /// <para>
    /// <see cref="AvaloniaHeadlessPlatform.ForceRenderTimerTick"/> is separate and complementary: it
    /// commits the visual tree into the server-side composition scene that hit-testing resolves against
    /// (it does <em>not</em> run layout). Both are needed — exactly the sequence the merged
    /// <c>SlotClickHitTestTests.PumpToStableLayout</c> (PR #171) uses.
    /// </para>
    /// </remarks>
    public void Pump()
    {
        Dispatcher.UIThread.RunJobs();
        // Flush the LayoutManager's dirty queue (re-measures/arranges descendants that invalidated after
        // the initial pass, e.g. the HasSave-gated editor grid) — a direct Window.Measure would not.
        Window.UpdateLayout();
        Dispatcher.UIThread.RunJobs();
        // Commit layout into the server-side composition scene so hit-testing has geometry to resolve.
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }

    /// <summary>
    /// Pumps the UI (each iteration a full deterministic <see cref="Pump"/>: RunJobs + layout + forced
    /// render tick) until <paramref name="condition"/> is true, or throws after
    /// <paramref name="maxPumps"/> iterations.
    /// </summary>
    /// <remarks>
    /// Bounded by <b>iteration count, not wall-clock time</b>, so it is fully load-independent: because
    /// each <see cref="Pump"/> flushes a full LayoutManager pass (see its remarks), the awaited
    /// realization completes in a handful of deterministic iterations regardless of how much CPU the
    /// process is getting. The old implementation spun on a wall-clock <c>Stopwatch</c> deadline while
    /// <see cref="Pump"/> only did a direct <c>Window.Measure</c> — so realization depended on the
    /// ambient render-timer layout pass firing within 5&#160;s, which starved under full-suite load and
    /// produced flaky "box viewer view to realize" timeouts. The bound here is a safety net against a
    /// genuinely stuck condition, not a timing knob; the happy path exits well before it.
    /// </remarks>
    public void PumpUntil(Func<bool> condition, int maxPumps = 240, string? because = null)
    {
        for (var i = 0; i < maxPumps; i++)
        {
            if (condition())
                return;
            Pump();
        }
        if (condition())
            return;
        throw new TimeoutException(
            $"Condition still unmet after {maxPumps} deterministic pumps{(because is null ? "" : $": {because}")}. " +
            $"[HasSave={ViewModel.HasSave} WindowVisible={Window.IsVisible} WindowLoaded={Window.IsLoaded}]");
    }

    // ---------------------------------------------------------------------
    // Save loading (the real SaveFileChanged -> MainWindowViewModel pipeline)
    // ---------------------------------------------------------------------

    /// <summary>
    /// Loads a real save file from disk (using the same Core detection the gateway uses) and adopts it
    /// as the current save, driving the real <c>MainWindowViewModel.OnSaveFileChanged</c> pipeline that
    /// rebuilds the editors/box viewer.
    /// </summary>
    /// <remarks>
    /// Detection + adoption run synchronously on the UI thread. The production gateway's
    /// <see cref="ISaveFileGateway.LoadSaveFileAsync"/> offloads detection to <c>Task.Run</c> and raises
    /// <c>SaveFileChanged</c> on the awaiter's continuation; under the app's Avalonia
    /// <see cref="System.Threading.SynchronizationContext"/> that resumes on the UI thread, but the
    /// headless unit-test session does not install one, so the continuation (and the UI-bound VM
    /// rebuild it triggers) would resume on a thread-pool thread — a non-deterministic off-thread
    /// mutation. Adopting on the UI thread here is faithful to <c>OnSaveFileChanged</c> and avoids that.
    /// </remarks>
    public void LoadSave(string path)
    {
        if (FileUtil.GetSupportedFile(path) is not SaveFile sav)
            throw new InvalidOperationException($"File is not a recognized save file: {path}");
        LoadSaveInstance(sav, path);
    }

    /// <summary>
    /// Adopts an already-constructed <see cref="SaveFile"/> as the current save through the production
    /// <see cref="ISaveFileGateway.OpenLoadedSave"/> path (the same <c>SaveFileChanged</c> pipeline a
    /// file open drives, synchronously on the UI thread), then pumps until the UI has rebuilt.
    /// </summary>
    public void LoadSaveInstance(SaveFile sav, string? path = null)
    {
        Gateway.OpenLoadedSave(sav, path);
        Pump();
        ThrowIfSaveBuildFailed();
        // The box viewer lives behind IsVisible="{Binding HasSave}" and inside a TabControl; realizing
        // that subtree after HasSave flips true can take more than one layout/dispatcher cycle. Pump
        // until it is actually attached so slot/nav lookups are deterministic (never a bare Pump race).
        if (ViewModel.HasSave)
            PumpUntil(() => Find<BoxViewer>() is not null, because: "box viewer view to realize");
    }

    // If MainWindowViewModel.OnSaveFileChanged threw while building the editors/box viewer it swallows
    // the exception, closes the save, and reports it via the dialog service. Surface that as a clear
    // test failure (with the original exception text) instead of a downstream "value is null".
    private void ThrowIfSaveBuildFailed()
    {
        if (ViewModel.CurrentSave is null && Dialogs.Errors.Count > 0)
            throw new InvalidOperationException(
                "Save failed to build in MainWindowViewModel: " +
                string.Join(" | ", Dialogs.Errors.Select(e => $"{e.Title}: {e.Message}")));
    }

    // ---------------------------------------------------------------------
    // Control lookup
    // ---------------------------------------------------------------------

    /// <summary>Finds the first descendant control of type <typeparamref name="T"/> in the window.</summary>
    public T? Find<T>() where T : Control => Window.GetVisualDescendants().OfType<T>().FirstOrDefault();

    /// <summary>Finds a named descendant control (by <c>x:Name</c>/<c>Name</c>).</summary>
    public T? FindByName<T>(string name) where T : Control =>
        Window.GetVisualDescendants().OfType<T>().FirstOrDefault(c => c.Name == name);

    /// <summary>Finds a control by its <c>AutomationProperties.Name</c> (accessible name).</summary>
    public T? FindByAutomationName<T>(string name) where T : Control =>
        Window.GetVisualDescendants()
            .OfType<T>()
            .FirstOrDefault(c => global::Avalonia.Automation.AutomationProperties.GetName(c) == name);

    /// <summary>
    /// The realized box-slot <see cref="Button"/> for a given box/slot in the currently displayed box.
    /// The box viewer only realizes the box it is showing, so <paramref name="box"/> must equal the
    /// current box (navigate first if needed).
    /// </summary>
    public Button? FindSlotButton(int box, int slot) =>
        Window.GetVisualDescendants()
            .OfType<Button>()
            .FirstOrDefault(b => b.Tag is SlotData sd && sd.Box == box && sd.Slot == slot);

    /// <summary>Selects a realized main-editor tab by its localized header text and pumps its content.</summary>
    public void SelectTab(string header)
    {
        var tab = Window.GetVisualDescendants()
            .OfType<TabItem>()
            .FirstOrDefault(item => Equals(item.Header, header))
            ?? throw new InvalidOperationException($"No realized tab with header '{header}'.");
        tab.IsSelected = true;
        Pump();
    }

    // ---------------------------------------------------------------------
    // Input simulation
    // ---------------------------------------------------------------------

    /// <summary>
    /// Clicks <paramref name="control"/> (a <see cref="Button"/>): asserts it is a genuine on-screen
    /// clickable target (laid out, effectively visible/enabled, hit-test-visible) and then activates it
    /// through the real click path, so the composed app reacts exactly as it would to a pointer click.
    /// </summary>
    /// <remarks>
    /// It deliberately does <b>not</b> drive the click through Avalonia headless's synthetic
    /// <c>MouseDown</c>/<c>MouseUp</c> transport, nor assert routing through the composition-scene
    /// <c>InputHitTest</c>. Neither is deterministic for the full composed window under full-suite load:
    /// synthetic <c>MouseDown</c> routing <em>consistently</em> fails on windows-latest headless (the reason
    /// PR #171 moved <c>SlotClickHitTestTests</c> to <c>InputHitTest</c>) and is intermittently dropped on
    /// macOS/Linux under load; and even <c>InputHitTest</c> against the composed <see cref="MainWindow"/>
    /// intermittently returns <see langword="null"/> for a fully laid-out, hit-test-visible button
    /// (~1 run in 15) because the server-side composition scene is not reliably committed for a
    /// heavyweight window in a shared multi-test process — while layout stays perfectly correct.
    /// <para>
    /// So the two things a click proves are split by reliability. <b>Pointer hit-test routing</b> (that a
    /// press at a slot's centre resolves to its <see cref="Button"/>, i.e. the slot template root is
    /// hit-test-visible — the PR #170 regression) is guarded deterministically at the control level by
    /// <c>SlotClickHitTestTests</c>, which commits a bare window's scene reliably. This full-app harness
    /// instead proves <b>composition + command wiring</b>: the composed <see cref="MainWindow"/> realises
    /// the slot/nav buttons from the real save, and activating one drives the real
    /// <c>MainWindowViewModel</c> pipeline (selection, box navigation, editor load). Activation goes
    /// through the button's accessibility peer (<see cref="IInvokeProvider"/>), which raises the very same
    /// <c>Click</c> a pointer would (firing the slot's <c>OnSlotClicked</c>/<c>SelectSlotByClick</c> or a
    /// nav button's <c>Command</c>) — deterministically, with no dependency on the flaky headless input or
    /// composition-scene machinery.
    /// </para>
    /// It still does not synthesize modifier state or click-count, so modifier-clicks (Ctrl/Shift/Alt+Click)
    /// and double-tap remain keyboard-only — see Harness/README.md.
    /// </remarks>
    public void Click(Control control)
    {
        Pump();
        if (control is not Button button)
            throw new ArgumentException($"Click target must be a Button; got {control.GetType().Name}.", nameof(control));

        // Deterministic app-level preconditions a real pointer click requires — all layout/property state,
        // which is reliably committed here (unlike the composition-scene hit-test; see remarks). A button
        // that is invisible, disabled, un-hit-testable, or unlaid-out would not receive a click.
        if (!button.IsEffectivelyVisible || !button.IsEffectivelyEnabled || !button.IsHitTestVisible
            || button.Bounds.Width <= 0 || button.Bounds.Height <= 0)
        {
            throw new InvalidOperationException(
                $"{control.GetType().Name} is not a clickable target: IsEffectivelyVisible={button.IsEffectivelyVisible}, " +
                $"IsEffectivelyEnabled={button.IsEffectivelyEnabled}, IsHitTestVisible={button.IsHitTestVisible}, " +
                $"Bounds={button.Bounds.Width}x{button.Bounds.Height}.");
        }

        // App-level guard for the PR #170 regression (slot template root set IsHitTestVisible="False",
        // which silently swallowed every slot click) — checked via the property, deterministically, rather
        // than the flaky composition-scene hit-test: a click cannot reach the button through a template
        // root that excludes itself from hit-testing.
        if (button.GetVisualChildren().FirstOrDefault() is InputElement templateRoot && !templateRoot.IsHitTestVisible)
        {
            throw new InvalidOperationException(
                $"{control.GetType().Name}'s template root ({templateRoot.GetType().Name}) is not hit-test-visible — " +
                "a pointer click could not reach the button (the PR #170 slot-template regression).");
        }

        // Activate through the accessibility peer — raises the same Click a pointer would (the slot's
        // OnSlotClicked/SelectSlotByClick, or a nav button's Command) — deterministically.
        ((IInvokeProvider)new ButtonAutomationPeer(button)).Invoke();
        Pump();
    }

    /// <summary>
    /// Left-clicks a box slot, selecting it (the real <c>SelectSlotByClick</c> path). To then load the
    /// selected slot into the editor, use the keyboard: <c>PressKey(PhysicalKey.Enter)</c> (activate).
    /// </summary>
    public void ClickSlot(int box, int slot)
    {
        var button = FindSlotButton(box, slot) ?? throw new InvalidOperationException($"No realized slot button for box {box}, slot {slot}.");
        Click(button);
    }

    /// <summary>Moves keyboard focus to a control (required before key gestures route to it).</summary>
    public void Focus(Control control)
    {
        control.Focus();
        Pump();
    }

    /// <summary>Presses (down+up) a physical key with optional modifiers on the focused element.</summary>
    public void PressKey(PhysicalKey key, RawInputModifiers modifiers = RawInputModifiers.None)
    {
        Window.KeyPressQwerty(key, modifiers);
        Window.KeyReleaseQwerty(key, modifiers);
        Pump();
    }

    /// <summary>Types literal text into the focused input control (e.g. a species combo box).</summary>
    public void TypeText(string text)
    {
        Window.KeyTextInput(text);
        Pump();
    }

    // ---------------------------------------------------------------------
    // Optional visual evidence (see Harness/README.md)
    // ---------------------------------------------------------------------

    /// <summary>
    /// Saves the window's last rendered frame to <paramref name="pngPath"/> and returns it, or returns
    /// <see langword="null"/> if no frame is available. Produces meaningful pixels only when the test
    /// assembly's app builder enables real drawing (Skia + <c>UseHeadlessDrawing = false</c>); under
    /// the default headless drawing mode the frame is geometry-only/blank. See the harness README.
    /// </summary>
    public string? CaptureFrame(string pngPath)
    {
        WriteableBitmap? frame;
        try
        {
            // Throws NotSupportedException under the default headless drawing mode (no real pixels);
            // only succeeds when the assembly's app builder enables Skia + UseHeadlessDrawing = false.
            frame = Window.GetLastRenderedFrame();
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

    public void Dispose()
    {
        // Close the window AND drain the dispatcher (jobs + a final compositor commit) so no shown
        // window, pending layout job, or running Theme.axaml transition leaks into the next test in
        // this serial ([AvaloniaFact]) assembly.
        try
        {
            Window.Close();
            Dispatcher.UIThread.RunJobs();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick();
            Dispatcher.UIThread.RunJobs();
        }
        catch { /* headless teardown is best-effort */ }

        // Dispose the per-fixture DI container so its singletons are released rather than accumulating
        // one live production object graph (each with e.g. the update-check HttpClient) per test. This
        // is a separate container from the assembly-level App.Services, so disposing it is safe and
        // affects only this fixture's graph.
        if (Services is IDisposable disposableServices)
        {
            try { disposableServices.Dispose(); }
            catch { /* best-effort */ }
        }
    }
}

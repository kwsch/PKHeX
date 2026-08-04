# Headless full-app UI harness

Boots the **real composed application** — the production DI graph, the real `MainWindow`, and the
real `MainWindowViewModel` — entirely in-process with **no desktop, no screen, and no mouse
takeover**, then drives it with simulated input. It exists so agents (and CI) can verify app-level
UI behaviour **without computer-use**, which blocks the user's physical screen/mouse.

This complements — it does not replace — the existing control-level `[AvaloniaFact]` tests
(`FilterableComboBoxTests`, `ThemeTests`) and the ViewModel/Core integration tests
(`RealSaveIntegrationTests`). The new capability here is composing **everything at once** and
exercising the wiring between the view, the code-behind, the services, and the ViewModels.

## Files

| File | Role |
|------|------|
| `HeadlessAppFixture.cs` | Boots the real container (`App.BuildServiceProvider`) with temp config paths + no-op dialog/window doubles, shows `MainWindow` headlessly, and exposes helpers: `LoadSave`/`LoadSaveInstance`, `ClickSlot`, `Click`, `PressKey`, `TypeText`, `Find<T>`/`FindByName`/`FindByAutomationName`/`FindSlotButton`, `Pump`/`PumpUntil`, `CaptureFrame`. |
| `HeadlessTestDoubles.cs` | `RecordingDialogService` / `NoopWindowService` — the only two host services swapped out, because native pickers and child windows need a live windowing surface headless can't provide. |
| `HeadlessSaveFixtures.cs` | Generates a deterministic Emerald save with a known Pokémon in box 0/slot 0, written to a temp file, so a smoke test always has a real save with a guaranteed-occupied slot. |
| `HeadlessAppSmokeTests.cs` | Three meaningful smoke tests + one opt-in frame-capture test. |

## How the seam works

`App.BuildServiceProvider(paths?, settingsStore?, settings?, configureOverrides?)` is the **single
source of truth** for the object graph, shared by production startup and this harness. Production
passes nothing; the harness passes throwaway temp paths (so no real user config is read/written) and
an overrides callback that registers the two test doubles last (DI last-registration-wins). The rest
— save-file gateway, slot service, sprite renderer, undo/redo, every editor ViewModel — is the exact
production wiring.

Tests must be written with `[AvaloniaFact]`/`[AvaloniaTheory]` so the body runs on the Avalonia UI
thread (the headless xUnit harness marshals it there). Determinism comes from **pumping the
dispatcher** (`Pump`/`PumpUntil`) — never `Thread.Sleep`, and never a wall-clock deadline. Each
`Pump`:

1. runs queued dispatcher jobs (`Dispatcher.UIThread.RunJobs`),
2. **flushes a full LayoutManager pass** (`Window.UpdateLayout()`), and
3. forces a synchronous compositor render tick (`AvaloniaHeadlessPlatform.ForceRenderTimerTick`).

Step 2 is load-bearing and must be `UpdateLayout()` — **not** a direct `Window.Measure()/Arrange()`.
A direct `Window.Measure` is a no-op once the window's own measure is valid, so a *descendant* that
invalidates its measure after the first pass — the whole editor grid gated by
`IsVisible="{Binding HasSave}"`, which flips visible only once a save is adopted — is left in the
LayoutManager's dirty queue and never re-measured. It then arranges to `0×0` and nothing beneath it
(the `TabControl`-hosted `BoxViewer`, the slot buttons) realizes. `UpdateLayout()` drains that queue.
Step 3 is separate: it commits the visual tree into the server-side composition scene that
hit-testing resolves against (it does not run layout). `PumpUntil` is bounded by **iteration count,
not elapsed time**, so it is independent of how much CPU the process is getting; the earlier
`Measure`-only pump relied on the ambient render-timer layout pass firing in time, a load-losing race
under full-suite `dotnet test` (the same class of flake PR #171 fixed in `SlotClickHitTestTests`).

## Capability matrix

### Can verify headlessly (covered / coverable here)

- App composition: the real DI container resolves `MainWindowViewModel` and every dependency.
- Loading a real save through the production `ISaveFileGateway.LoadSaveFileAsync` pipeline (the File
  > Open path), and the ViewModel rebuilding box viewer / party viewer / editors in response.
- Box grid population: the `BoxViewer` view realizes one clickable button per slot from the save.
- Click composition + command wiring: `Click(control)` asserts the target `Button` is a genuine
  clickable target (laid out, effectively visible/enabled, and hit-test-visible — including its
  template root, catching the PR #170 regression) and then activates it through the same `Click` a
  pointer would (a slot's selection, a nav button's `Command`). Proven by
  `MouseClickingSlot_RoutesToButton_AndSelectsIt` and `MouseClickingNextBoxButton_AdvancesBox`. It does
  **not** use synthetic `MouseDown`/`MouseUp` or the composition-scene `InputHitTest` — see the note
  below for why, and where pointer routing is guarded instead.
- Keyboard input: focus a control and send physical keys / `KeyBindings` (e.g. Enter to activate a
  slot → editor loads the Pokémon; Ctrl+C/V/Delete slot ops), and text input into focused controls.
- Combined mouse+keyboard flows: `ClickSelectThenEnter_LoadsPokemonIntoEditor`.
- Layout, bindings, styles/theme resolution (`Theme.axaml` is applied to the real window).
- Command enable/disable state, property-changed propagation, status/title text, tab content.

### Cannot verify headlessly (must use computer-use / a real desktop run)

- **Modifier-clicks and double-tap.** Headless mouse input does not merge keyboard-modifier state
  (Ctrl/Shift/Alt) into the pointer event's `KeyModifiers`, nor synthesize click-count, so the box
  viewer's Ctrl/Shift/Alt+Click (view/set/delete) and double-tap (activate) gestures cannot be driven
  by the mouse. Use their keyboard equivalents instead (Ctrl+C/V/Delete key bindings, Enter to
  activate), which route identically to the same commands.
- **Native drag-and-drop initiation.** `DragDrop.DoDragDropAsync` (used by `BoxViewer`/`PartyViewer`
  `OnSlotPointerMoved`) drives the **OS** drag loop, which headless has no implementation for — the
  call is a no-op / fails to start a session. The *receiving* side of a drop can be simulated via
  `HeadlessWindowExtensions.DragDrop(...)`, but that does not exercise the app's real drag-source
  code. End-to-end slot drag-drop between boxes, and OS file drops from Finder/Explorer, remain
  computer-use territory.
- **Native file/folder pickers** (`StorageProvider` open/save dialogs). No headless picker UI; the
  harness stubs `IDialogService` and injects paths directly.
- **Real modal/child windows and native menus** (`IWindowService.ShowDialogAsync`/`ShowTool`, the OS
  menu bar). Headless has no second window surface; these are stubbed/recorded.
- **Clipboard** backed by the OS, `RevealInFileManager`, update-check networking, and any real GPU
  rendering.
- **Actual rendered pixels** unless real drawing is enabled (see below).

## Note: how clicks work, and where pointer routing is guarded

`HeadlessAppFixture.Click(control)` splits the two things a real click proves, by how reliably each can
be verified headlessly:

1. **Activation (what this harness proves)** — the target `Button` is asserted to be a real clickable
   target (laid out with non-zero bounds, effectively visible and enabled, and hit-test-visible —
   including its template root, so the PR #170 regression that set a slot template root to
   `IsHitTestVisible="False"` is still caught here at the app level), then invoked through its
   accessibility peer (`IInvokeProvider`). That raises the *same* `Click` a pointer would — firing the
   slot's `OnSlotClicked`/`SelectSlotByClick` or a nav button's `Command` — so the composed
   `MainWindow` + real `MainWindowViewModel` pipeline (selection, box navigation, editor load) is
   exercised deterministically. These are all layout/property facts, which headless commits reliably.
2. **Pointer hit-test routing (guarded elsewhere)** — that a synthetic pointer press at a slot's centre
   actually resolves to its `Button` is guarded deterministically at the *control* level by
   `SlotClickHitTestTests` (PR #171), which commits a small bare window's composition scene reliably
   and asserts `InputHitTest`.

Why the split — this harness does **not** use synthetic `MouseDown`/`MouseUp` or the composition-scene
`InputHitTest` for its clicks:

- Synthetic `MouseDown`/`MouseUp` routing is not deterministic: PR #171 documents it *consistently*
  failing on windows-latest headless (render-scaling/coordinate handling), and it is intermittently
  dropped on macOS/Linux under full-suite CPU load.
- The composition-scene `InputHitTest` is unreliable for the *full composed window*: under full-suite
  load it intermittently returns `null` for a fully laid-out, hit-test-visible button (~1 run in 15),
  because the heavyweight `MainWindow`'s server-side scene is not always committed in a shared
  multi-test process — even though layout stays perfectly correct. (It works for the small isolated
  window in `SlotClickHitTestTests`, which is why routing is guarded there.)

Activating through the accessibility peer sidesteps both flaky machineries while still exercising the
real click handler and the whole app-level pipeline behind it.

## Optional: real screenshots for visual evidence

`HeadlessAppFixture.CaptureFrame(pngPath)` calls `TopLevel.GetLastRenderedFrame()`. Under the default
headless drawing mode (`UseHeadlessDrawing = true`, set in `TestAppBuilder`) the compositor produces
**no real pixels**, so the frame is blank/geometry-only and `CaptureFrame` may return `null`.
`HeadlessGiftRecordTests.CaptureSvGiftRecords_WhenEnabled_WritesPng` provides the reproducible
Scarlet/Violet Gifts-tab frame used by the main README.

Avalonia is a per-process singleton and drawing mode is fixed at app-build time. Setting
`PKHEX_HEADLESS_CAPTURE=1` before the test process starts makes `TestAppBuilder` select Skia, Inter,
and `UseHeadlessDrawing = false`; without it, the normal fast headless mode remains the default.
Run the capture in its own filtered test process so the slower native rasterizer never affects the
regular CI suite:

```
PKHEX_HEADLESS_CAPTURE=1 PKHEX_HEADLESS_CAPTURE_DIR=/path/to/out \
  dotnet test Tests/PKHeX.Avalonia.Tests -c Release \
  --filter FullyQualifiedName~HeadlessGiftRecordTests.CaptureSvGiftRecords_WhenEnabled_WritesPng
```

## Known caveat

Booting any `[AvaloniaFact]` assembly constructs the `App` once, which runs
`App.OnFrameworkInitializationCompleted` and therefore builds the *production* container with real
`AppPaths` (reading the developer's real config once). That predates this harness and is unrelated to
it — the fixture always builds its **own** container with temp paths, so the harness itself never
reads or writes real user settings.

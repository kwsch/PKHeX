---
name: new-generation-editor
description: Use when adding a new generation-specific save-data editor (a "Misc Editor"-style dialog scoped to one SAV<N> type, e.g. coins/BP/flags/records) to PKHeX-Avalonia — scaffolds the View, ViewModel, ViewLocator entry, open-command, and menu wiring across the Clean Architecture layers.
---

# New Generation Editor

## Overview

PKHeX-Avalonia's generation-specific editors (`Misc3Editor`, `Misc4Editor`, `Misc7Editor`, `PokeathlonEditor`, etc.) all follow one wiring pattern across the Clean Architecture split. Missing any one of the 5 steps below leaves a dead command or an unreachable dialog — this happened for several existing `OpenMisc*Command`s, which have a ViewModel/View/ViewLocator entry but **no menu item**, so they're unreachable from the UI. Don't repeat that gap.

## When to Use

- Adding a dialog that edits fields specific to one generation/game family (e.g. "Gen 10 Ribbons", "Misc (Gen 10)").
- NOT for fields that already have a cross-gen editor (Pokédex, Trainer Card, Box tools) — extend those instead.

## The 5 Steps

1. **ViewModel** — `PKHeX.Presentation/ViewModels/Misc<N>EditorViewModel.cs`
   - `partial class`, inherits `ViewModelBase`, constructor takes the concrete save type (`SAV<N>`, not the base `SaveFile`).
   - Read/write fields directly through `PKHeX.Core` APIs on that save object — no repository/service indirection needed for simple field editors.
   - Use `[ObservableProperty]` for bound fields and `[RelayCommand]` for actions (CommunityToolkit.Mvvm source generators).
   - Reference: [Misc4EditorViewModel.cs](../../../PKHeX.Presentation/ViewModels/Misc4EditorViewModel.cs)

2. **View** — `PKHeX.Avalonia/Views/Misc<N>Editor.axaml` + `.axaml.cs`
   - Code-behind is just `InitializeComponent()` in the constructor — no logic in code-behind.
   - Reference: [Misc4Editor.axaml.cs](../../../PKHeX.Avalonia/Views/Misc4Editor.axaml.cs)

3. **ViewLocator entry** — `PKHeX.Avalonia/ViewLocator.cs`
   - Add `[typeof(Misc<N>EditorViewModel)] = () => new Misc<N>Editor(),` to the `Map` dictionary (alphabetical by key).
   - This is the *only* place Views and ViewModels are coupled — Presentation never references Avalonia.

4. **Open command** — `PKHeX.Presentation/ViewModels/MainWindowViewModel.EditorDialogs.cs`
   ```csharp
   [RelayCommand(CanExecute = nameof(HasSave))]
   private async Task OpenMisc<N>Async()
   {
       if (CurrentSave is not SAV<N> sav) return;
       await _windowService.ShowDialogAsync(
           new Misc<N>EditorViewModel(sav),
           "Misc Editor (Gen <N>)");
   }
   ```
   The generation type-check (`CurrentSave is not SAV<N> sav`) is the guard — it's what makes the command safely bindable everywhere while only doing something for the right save type.

5. **Menu entry** — `PKHeX.Avalonia/Views/MainWindow.axaml`, inside the relevant `<MenuItem Header="Gen <N>">` block:
   ```xml
   <MenuItem Header="Misc (Gen <N>)" Command="{Binding OpenMisc<N>Command}" />
   ```
   If a `Gen <N>` top-level menu doesn't exist yet (brand-new generation), add it as a sibling of the existing `<MenuItem Header="Gen <N-1>">` block, in generation order — match the indentation of its immediate neighbors, not the block above it (existing gen blocks are inconsistently indented; don't propagate that or "fix" it as a drive-by).

   **Do not skip this step even though some existing `OpenMisc*Command`s lack it** — those are a known gap, not the pattern to follow.

## Verifying

- Build: `dotnet build PKHeX.sln -c Release`
- Launch the app, load a save of the target generation, open the new menu item, confirm the dialog appears and fields round-trip (edit → save → reload).
- Add a test alongside the existing `Misc<N>EditorTests.cs` files in `Tests/PKHeX.Avalonia.Tests/` if the editor has non-trivial field logic (bit-packed flags, checksums, etc.).

## Common Mistakes

| Mistake | Fix |
|---|---|
| ViewModel constructor takes `SaveFile` instead of `SAV<N>` | Use the concrete type — the whole point is generation-specific fields only that type exposes |
| Forgot the ViewLocator entry | Dialog throws/no-ops at runtime with no compile error — Presentation and Avalonia aren't statically linked here |
| Forgot the menu entry (step 5) | Command exists and compiles but is unreachable from the UI — grep `Command="{Binding Open<Name>Command}"` across `.axaml` to confirm it's wired |
| Logic in the View's code-behind | Keep code-behind to `InitializeComponent()`; put logic in the ViewModel |
| Editing `PKHeX.Core` to add a helper for the new editor | Don't — `PKHeX.Core` must stay 1:1 with upstream. Put helpers in `PKHeX.Application`/`PKHeX.Presentation` instead |

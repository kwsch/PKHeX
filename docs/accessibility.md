# Accessibility

PKHeX-Avalonia aims to be usable entirely with the keyboard and to expose a
meaningful structure to screen readers (VoiceOver on macOS, NVDA/Narrator on
Windows). This document covers keyboard shortcuts and what to expect from a
screen reader.

## Global shortcuts (main window)

| Shortcut | Action |
|---|---|
| `Ctrl+O` | Open a save file |
| `Ctrl+S` | Save |
| `Ctrl+Shift+S` | Save As... |
| `Ctrl+Z` / `Ctrl+Y` | Undo / Redo |
| `Alt` | Open the menu bar (standard Avalonia/OS behavior) |
| `Tab` / `Shift+Tab` | Move focus forward/backward through the window |

The main window's tab order follows its visual layout: menu bar → status bar
(update notification, if shown) → the persistent Pokémon editor panel (left)
→ the tabbed save editor (Box/Party/Trainer/Inventory/Events/Gifts/Batch,
right). `Ctrl+Tab` / `Ctrl+Shift+Tab` cycle through the save editor tabs.

## Box grid (Box tab)

The box grid is a 6×5 grid of slot buttons. It behaves like a single
keyboard-navigable region rather than 30 separate tab stops — press `Tab`
once to enter it, then use:

| Shortcut | Action |
|---|---|
| Arrow keys | Move the selection cursor one slot in that direction |
| `Home` / `End` | Jump to the first / last slot in the box |
| `PageUp` / `PageDown` | Switch to the previous / next box |
| `Enter` / `Space` | Activate the selected slot (loads it into the Pokémon editor) |
| `Ctrl+C` | View: load the selected slot's Pokémon into the editor (same as Ctrl+Click) |
| `Ctrl+V` | Set: write the Pokémon currently in the editor into the selected slot (same as Shift+Click) |
| `Delete` | Clear the selected slot (same as Alt+Click) |
| `Ctrl+F` | Open the Search & Seek tool |
| `F3` / `Shift+F3` | Seek to the next / previous match |

Each slot announces a concise, non-generic name to screen readers — for
example "Slot 12: Pikachu, Lv. 25, shiny" or "Slot 3: Empty" — instead of
just "button". The currently loaded species, level, shiny/egg status, and
legality are all reflected in the announcement.

## Party grid (Party tab)

Same slot model as the box grid, condensed to a 2×3 grid of six slots:

| Shortcut | Action |
|---|---|
| `↑` / `↓` | Move the selection cursor |
| `Enter` / `Space` | Edit the selected slot |
| `Ctrl+C` / `Ctrl+V` / `Delete` | View / Set / Clear, same semantics as the box grid |

Party slots announce e.g. "Party slot 2: Pikachu, Lv. 25, shiny, fainted" or
"Party slot 4: Empty".

## Dialogs

Modal dialogs (Settings, About, editor windows opened from the Tools menu,
etc.) follow standard conventions:

- `Esc` cancels/closes the dialog without applying changes.
- `Enter` confirms the dialog's default action (where one exists).
- Focus lands on the first meaningful control when the dialog opens, and
  returns to the control that opened it once the dialog closes.

## Screen reader structure

- The menu bar, all menu items, and dialog buttons expose their visible
  text as the accessible name automatically.
- Icon-only buttons (glyphs like ◀ ▶ 🔍 ✕, or sprite/icon-only pickers for
  species/item/ball selection) have an explicit `AutomationProperties.Name`
  so they announce something like "Previous Box" or "Species" instead of
  "button".
- Focus is always visible: the active control shows a focus ring in every
  theme (Dark/Light/High Contrast/System — see the theme system introduced
  in #133).

## What has and hasn't been verified

This pass added `AutomationProperties.Name`/`HelpText` across the view
sweep, keyboard navigation for the box/party grids, and a static guardrail
test (see `Tests/PKHeX.Avalonia.Tests/AccessibilityAuditTests.cs`) that
scans every `.axaml` view for icon-only buttons missing an accessible name.

**Not yet done: a live screen-reader smoke test.** VoiceOver (macOS) and
NVDA/Narrator (Windows) have not been run against the built app as part of
this change. Before relying on this for real screen-reader users, someone
should:

1. Launch the published app with VoiceOver (macOS) or NVDA/Narrator
   (Windows) running.
2. Open a save file, tab through the main window, and confirm the window
   structure, menu, and tab labels are announced sensibly.
3. Navigate into the box grid with the keyboard and confirm slot
   announcements include species/level/shiny/egg status.
4. Load a Pokémon and confirm the legality verdict in the Pokémon editor
   is announced or reachable via the screen reader's cursor.
5. Open a couple of dialogs (Settings, an editor from the Tools menu) and
   confirm Esc/Enter and focus-on-open/focus-return-on-close behave as
   documented above.

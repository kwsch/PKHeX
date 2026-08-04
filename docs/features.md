# Feature guide

Short, practical guide to the tools that go beyond a straight WinForms-to-Avalonia port. For
keyboard/screen-reader behavior see [accessibility.md](accessibility.md); for build artifacts see
[packaging.md](packaging.md).

## Auto-Legality Mod (Showdown → legal Pokémon)

Paste a [Pokémon Showdown](https://pokemonshowdown.com/) export-format set and get back a legal,
ready-to-inject `PKM` — same engine as the original Windows Auto Legality Mod plugin.

- **Open it:** Tools → Showdown → Import Set (`Ctrl+T`) imports a single set into the currently
  selected slot; Import Team imports a full team across the box. Export Set/Export Box/Export All
  Boxes (`Ctrl+Shift+T`) go the other direction, dumping the box back out as Showdown text.
- **Under the hood:** the legalization engine lives in `PKHeX.AutoMod` (vendored from
  [santacrab2/PKHeX-Plugins](https://github.com/santacrab2/PKHeX-Plugins), see
  [`PKHeX.AutoMod/VENDORED.md`](../PKHeX.AutoMod/VENDORED.md)) and picks an encounter, ball,
  moves, EVs/IVs, and any other detail needed to satisfy `PKHeX.Core`'s legality checker.
- **Caveats:** if no legal encounter exists for the requested species/form/game combination, the
  import fails with an error rather than producing an illegal mon. Nicknames, OT, and language are
  filled with sensible defaults when the Showdown text doesn't specify them.

## Living Dex generator

Fills an entire Pokédex's worth of boxes with legal Pokémon, one per species (and, where relevant,
per form), using the same Auto-Legality engine as the Showdown importer.

- **Open it:** Tools → Generate Living Dex.
- **What it does:** walks the currently loaded save's Pokédex, generates a legal encounter for
  each missing entry, and places it into open box slots. It reports which species it couldn't
  place (no free slot, no legal encounter for that game) rather than silently skipping them.
- **Caveats:** it needs enough free box space for the full dex; run it on a save with room to
  spare, or clear boxes first.

## Whole-save legality audit

A tool window that runs the legality checker over every Pokémon in the save at once, instead of
one slot at a time in the main editor.

- **Open it:** Tools → Data → Legality Audit.
- **What it shows:** one row per occupied slot (box + party) with its legality verdict, so you can
  find every illegal or flagged entry in a save without clicking through each box individually.

## LiveHeX (live console box editing over Wi-Fi)

Reads and writes Pokémon directly on a running Nintendo Switch game over the local network,
instead of editing an exported save file.

- **Open it:** Tools → LiveHeX.
- **Requires:** the console must be running
  [**sys-botbase**](https://github.com/olliz0r/sys-botbase) (a homebrew sysmodule — requires a
  hackable/CFW Switch) and be reachable over Wi-Fi; enter its IP address in the LiveHeX window to
  connect. USB connections are out of scope for v1.
- **Supported games:** Sword/Shield, Brilliant Diamond/Shining Pearl, Legends: Arceus, and
  Scarlet/Violet (see the firmware-version matrix in
  [`PKHeX.Infrastructure/LiveHex/NOTICE.LiveHeX.md`](../PKHeX.Infrastructure/LiveHex/NOTICE.LiveHeX.md)).
- **Implementation note:** unlike the vendored Auto-Legality engine, LiveHeX's connectivity is a
  **clean-room** re-implementation of the small, public sys-botbase TCP protocol
  (`PKHeX.Infrastructure/LiveHex/`) — it does not vendor upstream's USB/3DS-capable injection
  code. See the NOTICE file linked above for why.
- **Caveats:** this edits the console's live memory. A bad write can corrupt your in-game save;
  keep a backup (see the backup manager below) before making changes you're not sure about.

## Save backup manager & save diff

Automatic backups of every save you open or write, plus a tool to compare two saves slot-by-slot.

- **Open it:** Tools → Backup Manager.
- **What it does:** `SaveBackupService` (`PKHeX.Infrastructure/SaveBackupService.cs`) snapshots a
  save before it's overwritten, timestamped and kept in the app's per-platform data directory (see
  Platform config/data directories below). The Backup Manager window lists all backups for the
  current save, lets you restore one, and opens the diff view to compare two saves and see which
  boxes/slots changed.

## Theme system

Dark, Light, High Contrast, and Follow System themes, switchable at runtime with no restart.

- **Change it:** Options → Settings → Appearance → Theme.
- **Implementation:** `PKHeX.Avalonia/Services/ThemeService.cs` applies the selection via
  Avalonia's `ThemeVariant` (High Contrast is a custom variant defined in
  `PKHeX.Avalonia/Services/AppThemeVariants.cs`, layered on the Dark palette); resource
  dictionaries live in `PKHeX.Avalonia/Styles/Theme.axaml`. Follow System tracks the OS light/dark
  setting live. Your choice persists in app settings across restarts.

## Localization (9 languages)

The app shell — menus, dialogs, settings, status messages — is localized into English, Japanese,
Korean, French, Italian, German, Spanish, Simplified Chinese, and Traditional Chinese (the same
nine languages `PKHeX.Core` uses for game data).

- **Change it:** Options → Language, or Settings → Appearance → Language. Switching applies
  immediately across the whole UI, no restart required.
- **Where strings live:** `PKHeX.Presentation/Localization/Strings/*.json`, one flat JSON file per
  language; `en.json` is the source of truth and any missing key in another language falls back to
  English rather than showing blank text or a raw key.
- **Contributing a translation or fixing one:** see [CONTRIBUTING.md](../CONTRIBUTING.md), which
  covers the file format, placeholders, menu-mnemonic conventions, and the guardrail tests.
- **Status:** the core application shell is fully localized; a backlog of less-common editor
  dialogs is tracked in `Tests/PKHeX.Avalonia.Tests/localization-allowlist.txt` and localized
  incrementally.

## In-app update checker

Checks GitHub Releases for a newer version and shows what changed.

- **Where it lives:** `PKHeX.Infrastructure/GitHubUpdateCheckService.cs` behind the
  `IUpdateCheckService` port.
- **Behavior:** runs automatically at startup, failing silently (no dialog, no retry) if the check
  can't complete — for example, no network access. If you've just upgraded across a version
  boundary, a "What's New" changelog dialog shows automatically once on the first run of the new
  version.
- **Downloads:** point you at the [Releases](https://github.com/doctorllll/PKHeX/releases)
  page for the appropriate per-platform artifact — see the Download section in the
  [README](../README.md) and [packaging.md](packaging.md) for what's available per OS.

## OS drag-and-drop

- **Drag out (box/party slot → desktop):** writes a decrypted entity file (e.g. `.pk9`) to a temp
  location and hands the OS a real file reference via Avalonia's `IStorageProvider`. This is
  desktop-backed: it works on Windows, macOS, and Linux (X11 and Wayland) when running as a normal
  desktop app. If a future Avalonia backend can't resolve a real file path for the temp file (e.g.
  a sandboxed or browser-hosted build), drag-out degrades gracefully to in-app-only dragging (box
  ↔ party still works) instead of failing.
- **Drag in** accepts `.pk1`–`.pk9`, `.pb7`/`.pb8`, `.pa8`, encrypted `.ek*`, and Mystery Gift
  files, reusing the same detection/conversion/legality pipeline as the folder import feature.
  Incompatible or unreadable files are rejected with a message dialog rather than crashing or
  silently corrupting a slot.
- **Drag a save file** onto any part of the main window (a slot, the editor panel, or elsewhere)
  to open it, the same as File → Open.

## Platform config/data directories

Settings, backups, and caches live in the OS-standard location instead of next to the executable:

| OS | Config | Data (backups, caches) |
|---|---|---|
| Windows | `%APPDATA%\PKHeX-Avalonia` | `%LOCALAPPDATA%\PKHeX-Avalonia` |
| macOS | `~/Library/Application Support/PKHeX-Avalonia` | same as config |
| Linux | `$XDG_CONFIG_HOME/PKHeX-Avalonia` (falls back to `~/.config/PKHeX-Avalonia`) | `$XDG_DATA_HOME/PKHeX-Avalonia` (falls back to `~/.local/share/PKHeX-Avalonia`) |

Resolution logic is pure and unit-tested for all three platforms regardless of host OS — see
`PKHeX.Infrastructure/Configuration/AppPathResolver.cs`. Settings from the legacy
next-to-executable location are migrated automatically the first time the app starts with the new
version; nothing is lost, and the old location is left in place rather than deleted.

## Accessibility

Keyboard-first navigation for the box/party grids, accessible names on icon-only buttons, and
visible focus in every theme. Full shortcut reference and screen-reader notes in
[accessibility.md](accessibility.md).

# PKHeX-Avalonia

> Canonical instructions for all coding agents (Claude Code, Codex, GitHub Copilot). Claude loads this via the CLAUDE.md stub.

A native Avalonia (11.x) port of [PKHeX](https://github.com/kwsch/PKHeX), the Pokémon save editor —
cross-platform (Windows/macOS/Linux) instead of WinForms-only. Built on .NET 10 + Avalonia 11.x with
CommunityToolkit.MVVM.

Development is AI-assisted (Claude Code, Codex), and this is now publicly disclosed. The `.claude/`
and `.codex/` directories in this repo are real, in-use automation (hooks, skills, subagents) — not
decoration. Treat them as part of the project's tooling.

## Hard Rules

1. **`PKHeX.Core/` is a byte-for-byte upstream mirror** of kwsch/PKHeX. Never edit it manually to make
   something compile — port the fix into the consumer layers (`PKHeX.Application`, `PKHeX.Infrastructure`,
   `PKHeX.Presentation`, `PKHeX.Avalonia`) instead. The only exception is the `chore/sync-pkhex-core-*`
   branch produced by the `sync-upstream-core` skill, which replaces `PKHeX.Core/` wholesale from upstream.
2. **`PKHeX.AutoMod/` is vendored** (the Auto Legality Mod legalization engine from santacrab2/PKHeX-Plugins).
   See `PKHeX.AutoMod/VENDORED.md` for the re-sync procedure — no `.cs` source edits under `AutoMod/` or
   `Enhancements/`; if a Core sync breaks compilation, fix it there and log the change in that file.
3. **No direct pushes to `master`.** Every change is a branch + PR. Enforced by `.claude/hooks/block-main-writes.sh`
   (and the equivalent `.codex/hooks/block-main-writes.sh` for Codex).
4. **Bump `UIVersion`** in `Directory.Build.props` on every PR: `feat` → minor, `fix`/`chore`/`deps`/`refactor`
   → patch, breaking → major. (Top-level `<Version>` tracks upstream PKHeX.Core — not the one to bump.)

## Architecture

Five-project Clean Architecture split (verified against the `.csproj` files):

```
PKHeX.Core            no project references (vendored, upstream mirror)
PKHeX.Application      -> Core                                  (ports/abstractions, no UI deps)
PKHeX.Infrastructure   -> Application, Core, AutoMod             (implementations)
PKHeX.Presentation     -> Application, Core                      (Avalonia-free ViewModels)
PKHeX.Avalonia         -> Core, Application, Infrastructure, Presentation   (host/composition root)
PKHeX.AutoMod          -> Core                                   (vendored ALM engine)
```

Presentation depends on Application + Core only — it does **not** reference Infrastructure, so
ViewModels stay free of both Avalonia and implementation details. Avalonia is the only project that
references all four and is where DI wiring and Views live. This is enforced by
`Tests/PKHeX.Architecture.Tests/LayerDependencyTests.cs` (NetArchTest-based).

Patterns to know:
- **`ViewLocator`** (`PKHeX.Avalonia/ViewLocator.cs`) — the single place that maps a dialog ViewModel
  type to its View, via a compile-checked dictionary. It lives in the host so Presentation never
  references Views.
- **`IWindowService.ShowTool`** (`PKHeX.Application/Abstractions/IWindowService.cs`) — opens a modeless
  auxiliary tool window for panels that shouldn't crowd the main editor (e.g. batch search, box report).
  Singleton-per-VM (re-invoke focuses existing).
- **Thin per-gen editors** keep direct Core-block access (not wrapped in interactors).
- **Sprites cross as PNG `byte[]`** — `ISpriteRenderer` → `PngBytesToBitmapConverter` in Views.

### Architecture constraints

- **Application/Infrastructure** stay free of Avalonia, Skia, AND CommunityToolkit.Mvvm (plain
  events/POCOs). CommunityToolkit.Mvvm is allowed only in Presentation.
- **Sprite boundary:** `ISpriteRenderer` returns PNG `byte[]`; host `PngBytesToBitmapConverter`
  materializes the Bitmap in Views.
- **Navigation:** `IDialogService` (framework-free) + `IWindowService.ShowDialogAsync(vm, title)`.
- **`GameInfo.*` statics** (read in 51 files) are Core/Entities reads — left as-is; only language
  mutation is owned by Application LanguageService.
- **Workflow use cases** are stateless and `new`'d at call sites (not DI-injected) to avoid ctor bloat.

## Workflow

### Branch + PR flow
- Work in feature branches, commit there, push, and open a PR. Never `git push origin master`.
- A clean build is expected to produce **0 warnings**.

### Auto-merge policy
- Claude-created PRs are automatically merged once CI/checks pass — no manual check-in needed.
- Flow: `gh pr checks <n> --watch`, then `gh pr merge <n> --merge --delete-branch`, then delete
  local branch and switch back to master.

### Worktree shipping
- Git commit/push/PR must run from inside the agent's worktree (not the repo root).
- The `block-main-writes.sh` hook checks the shell's cwd branch — so shipping commands must come
  from a Bash session whose cwd is inside the feature-branch worktree.
- Bulk temp data goes in a temp dir inside the worktree on real disk, never on tmpfs (ENOSPC risk).

## Build / Test

```
dotnet build PKHeX.sln -c Release
dotnet test PKHeX.sln -c Release
```

A clean build is expected to produce **0 warnings**. Test projects live under `Tests/`:
`PKHeX.Core.Tests`, `PKHeX.Avalonia.Tests`, `PKHeX.Architecture.Tests`.

- `Tests/PKHeX.Avalonia.Tests/` — xUnit + Avalonia.Headless + Moq
- `Tests/PKHeX.Core.Tests/Legality/Legal/` — 133 legal PKM fixtures
- `Tests/PKHeX.Core.Tests/Legality/Illegal/` — 43 illegal PKM fixtures

## Guardrail Tests (fail a PR if ignored)

- **`Tests/PKHeX.Avalonia.Tests/AccessibilityAuditTests.cs`** — regex-scans every `.axaml` view for
  icon-only interactive controls (`Button`/`ToggleButton`/`RepeatButton` with no visible text) and
  requires `AutomationProperties.Name`. Justified exceptions go in `accessibility-allowlist.txt` next
  to the test.
- **`Tests/PKHeX.Avalonia.Tests/LocalizationAuditTests.cs`** — regex-scans `.axaml` views and
  ViewModels for hardcoded user-facing English string literals instead of `{loc:Loc Key}` /
  `LocalizedStrings`. New/migrated files are enforced by default; the not-yet-migrated backlog is
  listed in `localization-allowlist.txt`.
- **`Tests/PKHeX.Architecture.Tests/LayerDependencyTests.cs`** — enforces the project reference
  direction above (e.g. Application must not depend on Avalonia/Infrastructure/Presentation).

## Localization

Resource files live in `PKHeX.Presentation/Localization/Strings/` (`LocalizedStrings.cs` / `LocExtension.cs`
drive lookup) with one JSON file per language: **9 languages** — `de`, `en`, `es`, `fr`, `it`, `ja`, `ko`,
`zh-Hans`, `zh-Hant`. Any new user-facing string needs a key added to **all 9** files, not just `en.json`.

## Theming

Theming is driven by `IThemeService` (`PKHeX.Application/Abstractions/IThemeService.cs`, `AppTheme` enum)
and implemented in `PKHeX.Avalonia/Services/ThemeService.cs` using Avalonia's `ThemeVariant`/
`ThemeDictionaries` APIs (`PKHeX.Avalonia/Styles/Theme.axaml`), including tracking the OS light/dark
preference for the `System` option. Covered by `Tests/PKHeX.Avalonia.Tests/ThemeTests.cs`.

## Dependency policy

- Stay on latest **11.x Avalonia** and **SkiaSharp 3.x**.
- Avalonia 12 / SkiaSharp 4 deferred — they are major versions with breaking API changes and need
  dedicated, UI-tested PRs. Don't bundle them into routine sweeps.

## Upstream Sync Automation

A daily workflow checks kwsch/PKHeX against `.github/upstream-sync/last-synced-sha.txt` and opens a
`PKHeX.Core Sync Required` issue (labeled `sync`) when upstream has moved. The full sync process —
mirroring Core 1:1, fixing consumer call sites, an Avalonia frontend-parity review of upstream's
WinForms UI changes, version bump, PR, and auto-merge once CI is green — is encoded in
`.claude/skills/sync-upstream-core/SKILL.md`. In detail:
1. Fetch the latest PKHeX.Core SHA from kwsch/PKHeX
2. Branch `chore/sync-pkhex-core-<short7>`; mirror `PKHeX.Core/` 1:1
3. Fix broken call sites in consumers only (never in Core)
4. Bump UIVersion; write synced SHA to `last-synced-sha.txt`
5. Check for frontend parity gaps — classify upstream non-Core commits; open `frontend-parity`
   issues for genuine gaps without blocking the Core auto-merge
6. Verify build (0 warn/0 err) + tests + diff=0; open PR; auto-merge once CI is green

## Style preferences

- **Clean architecture over shims** — for new features/integrations, prefer the cleanest
  architecture-correct solution even if it needs a rewrite, over expedient hacks.
- **No planning docs in repo** — don't commit AI-planning artifacts (specs/plans) to git history
  or GitHub. If committed, rewrite them out of branch history before pushing.
- **Prefer clean solution** — lead with the architecture-correct design, not expedient options.

## Known bugs fixed (reference)

- MemoryEditorViewModel.Save(): HT memory feeling/intensity written from OT values (copy-paste bug)
- PokemonEditorViewModel.LoadFromPKM(): Premature Validate() before memory fields loaded
- MainWindowViewModel: BatchEditor.BatchEditCompleted event leak on save close
- PartyViewerViewModel: redundant always-true pattern check
- Various dead fields and redundant OnPropertyChanged calls

## UI testing (computer-use)

- Publish the .app bundle: `dotnet publish PKHeX.Avalonia/PKHeX.Avalonia.csproj -c Debug -r osx-arm64 --self-contained -o <dir>`
- Open with: `open <dir>/PKHeX.Avalonia.app`
- Grant accessibility by bundle ID: `io.pkhex.avalonia`
- Test saves load from `Tests/savefiles/` via File > Open (app does NOT accept CLI file-path arg)
- Re-screenshot before each click — window may shift between actions

## Modeless tool-window pattern

- `IWindowService.ShowTool(vm, title)` + `CloseAllTools()` for auxiliary panels
- Singleton-per-VM (re-invoke focuses existing)
- Remembers size/position per VM type for the session via static `ToolBounds` dict
- `MainWindowViewModel.OnSaveFileChanged` calls `CloseAllTools()`
- First consumer: box seek (`EntitySeekViewModel` + `IBoxNavigator`)

## Automation tooling

- `.claude/` hooks, skills, and agents are committed to the repo (AI assistance publicly disclosed).
- `.claude/worktrees/`, `.claude/settings.local.json`, `.claude/scheduled_tasks.lock` stay gitignored.
- Changes to `.claude/` content go through branch + PR like everything else.

## Project memory (distilled)

<!-- Curated snapshot of prior agent session knowledge (2026-07-17). Claude's private memory remains canonical; update via Working notes. -->

- **PKHeX.Core sync workflow** (`sync-upstream-core` skill): fetch latest SHA from kwsch/PKHeX (often ahead of the issue's named SHA — sync to tip), branch `chore/sync-pkhex-core-<short7>`, mirror `PKHeX.Core/` via `rsync -a --delete --exclude bin --exclude obj` and verify `diff -rq` = 0 (no fork edits, not even the `.csproj`), fix only consumer call sites, bump `UIVersion` (+1 patch) and `<Version>` to match upstream's, write the 40-char SHA to `.github/upstream-sync/last-synced-sha.txt`, then verify build/tests and auto-merge once CI is green. Watch CI in the background (`gh pr checks <n> --watch --fail-fast`) so it doesn't block the turn.
- **Frontend parity**: a green build only proves nothing broke, not that the Avalonia UI gained upstream's WinForms-side features. Syncs must classify upstream non-Core commits and open `frontend-parity`-labelled issues for genuine UI gaps without blocking the Core auto-merge.
- **UIVersion policy**: SemVer by change type — feat→minor, fix/chore/refactor/deps→patch, breaking→major (not a flat +1). One-time catch-up 1.1.44→1.20.0 happened 2026-06-25; documented in the README "Versioning" section.
- **Auto-merge**: Claude-created PRs merge automatically once checks pass (`gh pr merge --squash --delete-branch` or `--merge --delete-branch` depending on repo history), no manual check-in required, including edge cases like suspected duplicate work in another session — still always via branch+PR, never direct pushes to master.
- **Worktree shipping gotcha**: the `block-main-writes.sh` hook checks the shell's *cwd branch*; sandboxed Bash/subagents spawned from a main checkout get cwd pinned to repo root and `cd` doesn't persist across their tool calls, so they cannot ship from a worktree. Ship (commit/push/PR) from the agent that owns the worktree, or from the main session's Bash issuing `cd <worktree>` as its own separate call before subsequent git commands. Never stage bulk generated data on a tmpfs scratchpad — use a temp dir inside the worktree on real disk.
- **Dependency policy**: stay on latest 11.x Avalonia (11.3.18) + SkiaSharp 3.x; Avalonia 12 / SkiaSharp 4 are deferred, breaking-API majors needing a dedicated, UI-tested PR — don't bundle into routine dependency sweeps (`Avalonia.Diagnostics` itself caps at 11.3.18, so bumping core Avalonia to 12 would drop the Debug inspector).
- **Clean Architecture migration** (PR #80, `refactor/clean-architecture`): full 5-project split done as one big-bang PR; sprites cross layer boundaries as PNG `byte[]` (`ISpriteRenderer` → `PngBytesToBitmapConverter` in Views); navigation via `IDialogService` (framework-free) + `IWindowService`; `ViewLocator` is a compile-checked VM→View map living in the host; Application/Infrastructure stay free of Avalonia/Skia/CommunityToolkit.Mvvm; workflow use cases are stateless and `new`'d at call sites, not DI-injected.
- **Modeless tool-window pattern** (PR #111): `IWindowService.ShowTool(vm, title)` + `CloseAllTools()` for auxiliary panels that shouldn't crowd a view (not Flyout/Popup, not context menus); singleton-per-VM, remembers size/position per VM type for the session; `MainWindowViewModel.OnSaveFileChanged` closes all tools on save change. Batch-instruction search is NOT Core-blocked — `SearchSettings.BatchInstructions` already exists in Core.
- **Style preference**: for new features/integrations, lead with the cleanest architecture-correct solution even if it needs a rewrite, rather than an expedient shim (stated explicitly re: Auto Legality Mod support, issue #89).
- **No planning docs in repo**: never commit superpowers brainstorming specs/plans into repo history or GitHub; keep them outside the repo or unstaged, and rewrite branch history if they slip in.
- **UI testing via computer-use**: publish the real `.app` bundle (`dotnet publish ... -o <dir>` then `open <dir>/PKHeX.Avalonia.app`) rather than `dotnet run` — only a real bundle can be granted accessibility access (by bundle ID `io.pkhex.avalonia`) and screenshotted; clicks then actuate normally. The app has no CLI file-path launch arg — test saves must be loaded via File > Open, which defaults to `Tests/savefiles/`.
- **`.claude/` automation is committed** to the repo (reversed from an earlier local-only policy on 2026-07-11, since AI-assisted development is now publicly disclosed): hooks, skills, and agents go through the normal branch+PR flow. `.claude/worktrees/`, `.claude/settings.local.json`, and `.claude/scheduled_tasks.lock` stay gitignored.

## Cross-agent conventions

- This file (`AGENTS.md`) is the single source of truth for agent instructions in this repo. `CLAUDE.md` and `.github/copilot-instructions.md` are pointers to it — never edit them, never duplicate content into them.
- Reusable skills live in `.claude/skills/` (one folder per skill with a `SKILL.md`). GitHub Copilot reads that directory natively; Codex sees it via the `.agents/skills` symlink. New skills always go in `.claude/skills/`.
- Claude-specific subagent definitions live in `.claude/agents/`. If you are not Claude Code, you may read them as role/process guidance.
- Session continuity across tools: before ending substantial work in ANY tool (Claude Code, Codex, Copilot), record durable context — decisions made, gotchas discovered, in-progress state worth resuming — in the "Working notes" section below, or fold it into the relevant section above. This is the shared memory between agents.

## Working notes

- 2026-08-05 — `PKHeX.Android` is a real Avalonia host, not a PoC: it references `PKHeX.Avalonia`
  with `OutputType=Library` (the SDK rejects a `WinExe` as a self-contained Android dependency,
  NETSDK1150) and replaces only host adapters — SAF dialogs, a SAF↔local-path storage bridge,
  overlay-based `IWindowService` (Android's Avalonia backend has no `Window`), clipboard. Two
  Android-specific constraints to remember: the single view owns **one** native text-input
  connection, so a tool overlay must *become* the root content (`MainView.ShowOverlay/
  ShowMainContent`) rather than stack above the editor, or IME text goes to the wrong control; and
  the desktop's fixed 520px editor side panel does not fit a phone, so `AndroidMainView.axaml`
  makes the entity editor the first tab of one full-width `TabControl`. Android is not in `ci.yml`
  (macOS-only) — build it locally before tagging a release.
- 2026-08-05 — Releases: one `v<UIVersion>` tag ships macOS **and** Android from
  `release.yml`; the `release` job needs both legs, so a broken Android build blocks the release
  instead of publishing a macOS-only one. Procedure, signing tiers, Android keystore secrets, and
  how releases hang off the upstream-sync loop are in `docs/releasing.md`.

<!-- Any agent: append short dated notes here (YYYY-MM-DD — note). Prune notes when stale or once folded into the sections above. -->

- 2026-07-19 — Issue #167 adds a save-side Switch Mystery Gift record manager in `feat/switch-gift-records`: SWSH (50 WR8 records), BDSP (50 records + 10 one-day entries, 2048 received flags, serial lock), PLA (50 trimmed WA8 records), and SV (32 retained trimmed WC9 records). Imports are deliberately limited to documented conversions; no BCAT redemption forging. The BDSP flag adapter accesses the full bitfield directly because upstream Core's helper shifts by 8 instead of 3.
- 2026-07-19 — Switch gift records now have full-composition Avalonia headless coverage for SWSH/BDSP/PLA/SV, including rendered slot counts, SV deletion, and BDSP flag/serial-lock controls. The README screenshot is reproducibly generated by the opt-in Skia-backed `HeadlessGiftRecordTests.CaptureSvGiftRecords_WhenEnabled_WritesPng`; normal CI keeps lightweight headless drawing.
- 2026-07-19 — The inherited Avalonia port identifies `PKHeX-Avalonia` and Patrik Lleshaj as its product/publisher/author/copyright holder. This fork publishes under `doctorllll/PKHeX`; keep the attribution and update fork-owned URLs/package identifiers when shipping here.
- 2026-07-27 — The deferred `b483ad4` sync (issue #192) is resolved: upstream `5cceba5` (Gen4 misc encounter generating fixes) added `pk.Ball = (byte)Ball.Poke` on hatch in the new `EggHatchLegality.ForceHatch`, clearing the HG/SS ball flag, so `ShowdownSetTests.SimulatorGetEncounters` passes again. Synced straight to upstream tip `b916b06`, skipping the stuck point. Upstream's `IEncounterSlot4` gained `IsRerollMinimum31`/`IsBugContest`/`IsSafariHGSS`/`Location`; our `Tests/PKHeX.Core.Tests/Legality/RNG/MockSlot4.cs` was updated to match (consumer-layer fix, Core untouched).
- 2026-07-30 — Issue #196 sync targets upstream tip `7890222` (newer than the issue's `ce08c00`). Core's `IGenerateSeed64.GenerateSeed64` now requires `ITrainerInfo` so Legends: Arceus generation can use save-specific shiny rolls; the vendored AutoMod call forwards its existing source trainer and the divergence is logged in `PKHeX.AutoMod/VENDORED.md`. Frontend parity review found only Core/test changes and existing translation-resource updates, with no Avalonia gap.

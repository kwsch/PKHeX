# PKHeX.Web — MVP support report (XY/ORAS)

Recorded **2026-10-04**. This qualifies the raw XY/ORAS MVP described in `PKHeX.Web.md` (§Definition of MVP) and states where it is supported. It follows the earlier [WebAssembly proof](PKHeX.Web.WasmProof.md). **The support claim is desktop only:** the Chromium engine, Firefox and the WebKit engine. The full test tiers ran on Playwright's builds of each, and the installed Google Chrome 154 was measured in the Perf tier. Physical Safari (macOS), iPadOS Safari and Android Chrome have not been run (gate G-C), so tablets and phones are **not qualified**. Every number below was measured on the reference desktop with a static loopback host. None is a guarantee for other machines or networks.

## Result

| Requirement (`PKHeX.Web.md`) | Target | Result | Verdict |
|---|---|---|---|
| WEB-PERF-001 cold usable shell, 20 Mbps / 50 ms | ≤ 5 s | Chrome 154: 2.95 s median (2.93–3.18 s, 5 runs) | **Met** |
| WEB-PERF-001 cached usable shell, 20 Mbps / 50 ms | ≤ 2 s | Chrome 154: 0.62 s median (0.61–0.63 s); 9 requests, all 304 | **Met** |
| WEB-PERF-001 transfer and resource cost | measured, not guessed | 62 files, 5.49 MiB Brotli per cold boot; Core's embedded data is 65% of the published Core file | **Met** (recorded) |
| WEB-BOX-008 box change with warm assets | ≤ 100 ms | p95 4 ms (Chrome), 7 ms (Firefox), 4 ms (WebKit); max 11 ms; no requests | **Met** (default publish; the unshipped sprite publish was not timed) |
| WEB-PERF-004 selected-entity analysis, synthetic corpus | p95 ≤ 500 ms | warm p95 41 ms (Chrome), 45 ms (Firefox), 37 ms (WebKit); first analysis per page 279–355 ms | **Met** |
| WEB-PERF-004 selected-entity analysis, real saves | p95 ≤ 500 ms | 968 slots of the private XY and ORAS saves: warm p95 20 ms (Chrome), 17 ms (Firefox), 18 ms (WebKit); max 73 ms. The RealSave tier repeats this in Chromium only. | **Met** |
| WEB-PERF-002 peak memory, largest admitted save | measured; completes on target device | 106–111 MiB WebAssembly memory after the first full ORAS session; 153–161 MiB once settled over repeated sessions | **Met on desktop**; device run open (G-C) |
| WEB-PERF-002 repeated-session trend | no unbounded growth | 100 sessions in one page: one ~26 MiB step by session 15 in each engine, then flat for the remaining 85 | **Met** |
| WEB-PERF-002 oversized input | measured | Refusing a 16 MiB file raises the peak to 222–272 MiB for the rest of the page | **Limitation (scoped)**, see below |
| WEB-BROWSER-001 desktop qualification | E2E in three engines, physical Safari, claimed Edge | E2E 367/367 and RealSave 113/113 in Playwright's Chromium 153, Firefox 155 and WebKit 26.6; Chrome 154 measured in the Perf tier only; no physical Safari; Edge not claimed | **Met for the Chromium engine, Firefox and the WebKit engine**; Safari not qualified |
| WEB-BROWSER-002 tablet/mobile files | physical iPad and Android | not run | **Not qualified** (G-C) |
| WEB-BROWSER-003 lifecycle/memory | background, eviction, back navigation on devices | lifecycle tests pass in desktop engines; no device eviction run | **Not qualified on devices** (G-C) |

## Reference desktop and revisions

| Item | Value |
|---|---|
| Machine | Apple M4 (10 logical CPUs), 16 GiB, macOS 26.5 (25F71), arm64 |
| Reference browser | Google Chrome 154.0.8037.93 (stable, installed), driven headless by Playwright through `PKHEX_WEB_PERF_CHANNEL=chrome` |
| Engine builds | Playwright 1.63.0: Chromium 153.0.8010.12, Firefox 155.0, WebKit 26.6, all headless |
| SDK / runtime | .NET SDK 10.0.401, Microsoft.NETCore.App and Microsoft.AspNetCore.App 10.0.12; Microsoft.AspNetCore.Components.WebAssembly 10.0.12 |
| Publish | Default Release publish (trimmed, interpreter, no AOT), and the publish with sprites (`-p:PKHeXWebSprites=true`) for the atlas and sprite tests |
| Source | `web/main` at `d51780f7d` plus the M21 changes (`web/m21-qualification`) |
| Host | the test project's `StaticHost` on loopback, in deployment-caching mode for the Perf tier (precompressed files, ETags, the shipped `_headers`) |

Headless Chrome is the release browser, but runs without a visible window. The throttled rows use Chromium's DevTools network emulation, which adds 50 ms to each request rather than emulating TCP round trips. Firefox and WebKit cannot be throttled by Playwright, so they have loopback rows only.

## Startup (WEB-PERF-001)

Median of 5 cold and 5 warm boots per configuration, after one unrecorded boot (`boot-baseline.md`).

| Browser | Network | Cold shell | Warm shell | Cold transfer |
|---|---|---:|---:|---:|
| Chrome 154 | 20 Mbps / 50 ms | 2947 ms | 622 ms | 5.49 MiB (Brotli), 62 requests |
| Chrome 154 | loopback | 278 ms | 265 ms | 5.49 MiB |
| Chromium 153 | 20 Mbps / 50 ms | 2920 ms | 613 ms | 5.49 MiB |
| Firefox 155 | loopback | 283 ms | 279 ms | 5.49 MiB |
| WebKit 26.6 | loopback | 241 ms | 244 ms\* | 7.95 MiB (gzip) |

\*Playwright's WebKit does not accept Brotli from the plain-HTTP loopback host and, in every run, reused nothing from its cache on the warm boot (recorded since F8; cause not established). That says nothing about Safari.

## Memory (WEB-PERF-002)

The largest admitted input is a raw ORAS save (472 KiB). The fixture fills every box slot and party position (936 entities). The figure is the size of the .NET runtime's WebAssembly memory, read through its public `getDotnetRuntime` API. It only grows, so each reading is the peak so far (`memory-baseline.md`).

| Step | Chrome 154 | Firefox 155 | WebKit 26.6 |
|---|---:|---:|---:|
| Shell ready | 53.4 MiB | 62.8 MiB | 63.3 MiB |
| Opened (936 entities) | 77.0 MiB | 75.3 MiB | 76.0 MiB |
| Edited, analysed (legality tables loaded) and applied | 110.9 MiB | 108.5 MiB | 109.5 MiB |
| Downloaded and closed | 110.9 MiB | 108.5 MiB | 109.5 MiB |
| Settled over 40 repeated sessions | 159.8 MiB | 156.3 MiB | 157.8 MiB |
| Then refused a 16 MiB file | 230.1 MiB | 225.2 MiB | 227.2 MiB |

- **Repeated sessions:** each session opens the save, edits, analyses and applies a box slot, downloads and closes. With `PKHEX_WEB_PERF_SESSIONS=100`, WebAssembly memory grew once in each engine, by about 26 MiB, after session 15 (Chrome) or 13 (Firefox, WebKit), and then stayed flat for the remaining 85 sessions. In every run (four runs of 40 or 100 sessions, all engines) it settled at 153–161 MiB. The runtime enlarges its heap in steps of that size, so retention below one step over the flat stretch would not show. Chrome's JavaScript heap rose from 4.9 to 6.3 MiB over 100 sessions (about 15 KiB per session). That is small, but it is not shown to level off.
- **Where the memory goes** (the same Web code on desktop .NET): opening allocates 4.1 copies of the save, applying 4.6 and exporting 7.1. That is the staged clone, the write, and the reopen that checks the output.
- **Sprite atlas** (publish with sprites only): 1.2 MiB on disk, 2176 × 3248 px, about 27 MiB once decoded. Computed from the PNG header, not measured on a device.
- **Oversized input (scoped limitation):** a file at the 16 MiB read limit is read in full before Core refuses it. M21 changed the bounded read to hold the file once (it allocated 3.5 times the file). The remaining cost comes from Blazor's file stream transfer and the loader's two copies, which are required because Core normalises its input in place. That cost raises the page's peak by about 60–120 MiB (to 222–272 MiB) for the rest of the session; the rise varies with how much of the heap earlier sessions left free. A user who picks a large wrong file on a constrained device should reload the page. Lowering the read limit is a possible follow-up once device measurements exist.

## Legality analysis (WEB-PERF-004)

Time from the panel being marked busy to the rendered verdict, through the app's idle path, on loopback. Every verdict was checked against native Core.

| Corpus | Browser | First analysis | Warm p50 | Warm p95 | Warm max |
|---|---|---:|---:|---:|---:|
| Core's legality fixtures (18 PK6, each in an XY and an ORAS save) | Chrome 154 | 301 ms | 16 ms | 41 ms | 45 ms |
| | Firefox 155 | 355 ms | 13 ms | 45 ms | 55 ms |
| | WebKit 26.6 | 279 ms | 14 ms | 37 ms | 42 ms |
| Private saves (553 XY and 415 ORAS slots) | Chrome 154 | 277 ms | 13 ms | 20 ms | 53 ms |
| | Firefox 155 | 315 ms | 10 ms | 17 ms | 73 ms |
| | WebKit 26.6 | 267 ms | 11 ms | 18 ms | 57 ms |

The first analysis of a page loads Core's legality tables, a single stall of about 0.3 s. No warm analysis took over 200 ms, so the 300 ms idle analysis stays on.

## Browser matrix

| Workflow | Chromium 153 | Firefox 155 | WebKit 26.6 | Chrome 154 |
|---|---|---|---|---|
| Published journey (`JourneyBrowserTests`): picker and drop, every editor group, legality, apply, download, reopen, privacy trace, at `/` and `/PKHeX/` | Pass | Pass | Pass | Perf tier only |
| Keyboard-only journey | Pass | Pass | Pass | – |
| Real-save journeys and round trips (XY and ORAS; RealSave tier) | Pass | Pass | Pass | – |
| Hosting headers, 404, notices viewer (M20) | Pass | Pass | Pass | – |
| Boot, memory, box navigation, legality timing (Perf tier) | Measured | Measured | Measured | Measured |

On this commit the E2E tier passed 367 of 367 tests (default and sprite publishes), and the RealSave tier 113 of 113, with every test executed. Chrome 154 ran the Perf tier and the real-save timing only, and Chromium 153 shares its engine. The E2E tier was not repeated on it.

## MVP exit audit

Every "Must / MVP" story in `PKHeX.Web.md` §Prioritised implementation matrix, with the chunk (from `plan.md`) and tests that cover it.

| Story | Chunk | Tests | Note |
|---|---|---|---|
| APP-002 Version and licensing | F5, M1 | `BuildInfoTests`, `ThirdPartyNoticesTests`, `NoticesRestoreGraphTests` | |
| APP-003 Responsive shell | M18a | `ResponsiveBrowserTests` | |
| APP-004 Capability/error shell | M1, M17 | `BootScriptTests`, `ShellTests`, `FaultBoundaryTests` | unsupported-browser detection simulated only |
| SAVE-001 Choose local save | F4, M1 | `BrowserFileServiceTests`, `PickerBrowserTests` | |
| SAVE-002 File drop | F4, M1 | `FileInteropTests` | |
| SAVE-003 Detect and admit format | M2 | `SaveLoaderTests`, `SupportMatrixTests` | |
| SAVE-004 Malformed/unsupported input | M2, M17 | `SaveLoaderTests`, `HostileInputTests`, `PublishedAppTests` | |
| SAVE-007 Metadata/privacy/recents | F4, M19 | `FileNamingTests`, journey privacy trace | |
| SESSION-002 Draft/apply/discard | M7, M9 | `DraftEditorTests`, `ApplyTransactionTests` | |
| SESSION-003 Honest dirty status | M6 | `ExportFlowTests`, `SessionStatusTextTests` | |
| SESSION-004 Reset/discard session | M6, M16, M18b | `SessionExitPanelTests`, `ModalExitBrowserTests` | |
| SESSION-005 Navigation lifecycle | M16 | `LifecycleBrowserTests` | back-forward cache not restorable under Playwright |
| SESSION-007 Naming policy | M6 | `FileNamingTests`, `ExportFlowTests` | |
| OVERVIEW-001/002 Identity, time, currency | M3 | `SaveOverviewTests`, `OverviewTextTests`, `SaveOverviewPanelTests` | |
| PARTY-001 Display/select | M4, M9 | `StorageBrowserTests`, `PartyApplyTests` | |
| BOX-001 Grid/navigation | M4, M18c | `SlotGridTests`, `NavigationBrowserTests` | |
| BOX-004 Sprites and summaries | M5 | `SlotSpriteTests`, `SpriteCatalogBrowserTests` | default publish uses text placeholders (G-B) |
| BOX-008 Box rendering budget | M4, M21 | `BoxNavigationTimingTests` | |
| PKM-001 Capability-driven editor | M7 | `SaveCapabilitiesTests`, `InspectorBrowserTests` | |
| PKM-002 Species/form | R1–R3, M15 | `SpeciesFormDraftTests`, `SpeciesFormBrowserTests` | R3's Windows checklist (G-E) open |
| PKM-003 Nickname/language, PKM-006 Friendship | M10 | `NameAndFriendshipDraftTests`, `NameRulesTests`, `NameFriendshipBrowserTests` | |
| PKM-004 Gender, PKM-009 Ability/slot | M14 | `GenderRuleTests`, `AbilityGenderDraftTests`, `AbilityGenderBrowserTests` | |
| PKM-005 Level/EXP, PKM-007 Nature, PKM-014 Stats/characteristic | M11 | `LevelNatureDraftTests`, `PartyStatPolicyTests`, `LevelNatureBrowserTests` | |
| PKM-010 Held item, PKM-011 Moves, PKM-012 PP/PP Ups | M13, M18c | `ItemMoveDraftTests`, `ItemMoveBrowserTests` | |
| PKM-013 IVs/EVs | M12 | `IvEvDraftTests`, `IvEvBrowserTests` | |
| LEGAL-001/002/003 Analyse, issues, stale results | M8, M16 | `LegalityServiceTests`, `DraftLegalityTests`, `LegalityPanelTests`, `LegalityBrowserTests` | |
| EXP-001/002/003 Serialise, download, reopen | M6 | `ExportFlowTests`, `PublishedAppTests`, journey | |
| A11Y-001/002/003 Keyboard, focus, visual/touch | M18a–c, M19 | `AccessibilityBrowserTests`, `ContrastTokensTests`, keyboard-only journey | no screen reader run |
| SEC-002 Ephemeral sessions | M1, M16 | `LifecycleBrowserTests`, every browser test's storage check | |
| SEC-003 Hostile input/rendering | M17 | `HostileInputTests`, `HostileInputBrowserTests` | |
| SEC-005 Safe diagnostics | M17 | `DiagnosticReportTests`, `DiagnosticPanelTests` | |
| ERR-001–004 File, parser, fatal, interrupted | M2, M16, M17 | `SaveLoaderTests`, `UserMessagesTests`, `FaultBoundaryTests` | |
| PERF-002 Large save/memory | M21 | `MemoryBaselineTests` | device run open (G-C) |
| PERF-003 Sprite loading | M5 | `SpriteAtlasTests`, `SpriteCatalogBrowserTests` | |
| PERF-004 Legality scheduling | M8, M21 | `LegalityTimingTests`, `RealSaveLegalityTimingTests` | |
| BROWSER-001 Desktop qualification | M19, M21 | `JourneyBrowserTests`, this report | physical Safari open (G-C) |
| BROWSER-002 Tablet/mobile files | – | – | **not qualified** (G-C) |
| BROWSER-003 Lifecycle/memory | M16, M21 | `LifecycleBrowserTests`, `MemoryBaselineTests` | device eviction open (G-C) |
| TEST-003 Transaction tests | M9 | `ApplyTransactionTests` | |
| TEST-004 Legality/editor regressions | M8–M15 | the draft and browser tests above | |
| TEST-005 Published browser journey | M19 | `JourneyBrowserTests`, `PublishedJourney` | |
| HOST-002 Paths/headers/compression | M20 | `DeploymentHeadersTests`, `HostHeadersTests`, `HostingSnippetsTests` | no Cloudflare deployment seen (G-A) |
| HOST-003 Releases/rollback | M20 | README hosting guide | no deploy job until G-A |

## Gates

- **G-A maintainer interest:** not yet sought. It decides the host, the deploy job and where CI lives.
- **G-B sprite policy:** open. The default publish shows text placeholders. The sprite publish is built and tested but not distributed.
- **G-C physical devices:** **reduced scope.** Desktop only, as stated above. The checklist below is what qualifies a device.
- **G-D real-save fixtures:** used locally for the RealSave tier and the real-save timing. Never committed or used in CI.
- **G-E Windows desktop testing:** R3's WinForms checklist is still open. It gates R3's upstream PR, not this Web release.

### Device checklist (G-C)

Run on each device with the published app over HTTPS, with one XY and one ORAS save, and record the OS and browser versions:

1. Cold load, then reload; the shell becomes usable.
2. Open a save through the picker (Files app or document provider); the overview and the party and box grids appear.
3. Rotate between portrait and landscape with a draft open; the draft survives, and the editor and its return action work.
4. Edit a field of every group, check legality, acknowledge if asked, then apply.
5. Download. The file appears where the browser saves files, opens again in the app, and shows the edits.
6. Switch apps for a minute and return. Then background the browser until it reloads the tab: the session is gone and nothing claims it was saved.
7. Pick a file over 16 MiB, then open a save again and complete a session (memory after an oversized file).
8. With the sprite publish, browse every box (decoded atlas memory).

## Known limitations

- Not qualified on physical Safari, iPadOS, Android or any phone (G-C). The WebKit rows are engine coverage only.
- Picking a file near the 16 MiB read limit leaves the page's memory about 60–120 MiB higher until reload.
- No screen reader has been run (A11Y-001/002 are covered by keyboard and markup tests only).
- No Cloudflare Pages deployment has been seen; its header behaviour is modelled by the test host (G-A).
- An intermittent focus step in `ResponsiveBrowserTests` (Chromium, root path) is recorded since M18b; it passes when rerun.
- Only raw decrypted XY and ORAS saves open; the ORAS demo and every other family are refused, by design.

## Reproduction

```sh
dotnet publish PKHeX.Web/PKHeX.Web.csproj -c Release -o "$OUT"
dotnet publish PKHeX.Web/PKHeX.Web.csproj -c Release -p:PKHeXWebSprites=true -o "$OUT_SPRITES"
export PKHEX_WEB_PUBLISHED="$OUT/wwwroot" PKHEX_WEB_PUBLISHED_SPRITES="$OUT_SPRITES/wwwroot" PKHEX_WEB_PERF_REPORT="$(mktemp -d)"
PKHEX_WEB_TEST_TIERS=Perf PKHEX_WEB_PERF_CHANNEL=chrome dotnet test Tests/PKHeX.Web.Tests -c Release --filter Category=Perf
PKHEX_WEB_TEST_TIERS=Perf dotnet test Tests/PKHeX.Web.Tests -c Release --filter Category=Perf
PKHEX_WEB_TEST_TIERS=E2E dotnet test Tests/PKHeX.Web.Tests -c Release --filter Category=E2E
PKHEX_WEB_TEST_TIERS=Perf PKHEX_WEB_PERF_CHANNEL=chrome PKHEX_WEB_PERF_SESSIONS=100 dotnet test Tests/PKHeX.Web.Tests -c Release --filter "FullyQualifiedName~MemoryBaselineTests"
PKHEX_XY_SAVE=… PKHEX_ORAS_SAVE=… PKHEX_WEB_TEST_TIERS=RealSave PKHEX_WEB_PERF_CHANNEL=chrome dotnet test Tests/PKHeX.Web.Tests -c Release --filter Category=RealSave
```

The Perf tier writes `boot-baseline`, `memory-baseline`, `box-navigation` and `legality-timing` (`.md` and `.json`). The RealSave tier's real-save timing (Chromium or the channel browser only) writes `legality-timing-realsave` when `PKHEX_WEB_PERF_REPORT` is set. The Firefox and WebKit real-save rows above come from a one-off run of the same harness over all three engines.

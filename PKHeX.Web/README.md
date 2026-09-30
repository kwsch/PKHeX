# PKHeX Web

A static .NET 10 Blazor WebAssembly editor on PKHeX.Core, in development and not yet the PKHeX.Web MVP. It opens raw XY/ORAS saves locally, shows the party and one box at a time, edits an existing boxed Pokémon’s nickname fields (party members can be opened and analysed, but not yet changed), runs Core legality analysis, and downloads a validated save. No save-processing server, upload API, persistent browser storage, analytics, sprites, or external runtime assets are used.

Install Microsoft's .NET 10 SDK for your platform. The verified development environment uses the machine-wide ARM64 SDK 10.0.401 and runtime 10.0.12. The Web package is pinned to 10.0.12; tests use Microsoft.Playwright 1.63.0. No repository-wide SDK pin is required. Both projects are part of `PKHeX.slnx`.

From the repository root:

```sh
dotnet publish PKHeX.Web/PKHeX.Web.csproj -c Release -o PKHeX.Web/bin/Release/publish
python3 -m http.server 8080 --bind 127.0.0.1 --directory PKHeX.Web/bin/Release/publish/wwwroot
```

Open `http://127.0.0.1:8080/`. Serve only the published `wwwroot`, never the repository or directory containing your saves. Runtime hosting uses an ordinary static server; it does not need .NET. The app does not install a service worker or require `wasm-tools`; the default Release publish trims managed assemblies but uses the stock interpreter runtime without optional native relinking/AOT.

## App shell

- **Start screen.** Before a save is open, the page states that the save is processed on this device and never uploaded, that the host still sees ordinary requests for the app's own files and may log them, which files can be opened (including the emulator prerequisites: save in game and close the emulator first; save states are not save files; loading an older save state after editing can undo the edits), and that the save and edits are not kept. The games listed come from `Services/SupportMatrix`, which is also the allowlist `SaveLoader` enforces. Once a save is open, a one-line note keeps the privacy and temporary-state message in view.
- **About.** A disclosure panel with the version, the source commit and the source repository, links to the license and notices (opened in a new tab, so the session in this one survives), and the families this release opens, none of them qualified as supported yet.
- **Startup failures.** `wwwroot/boot.js` starts Blazor itself (`autostart="false"`) and is kept to ES2015 syntax so that the old browsers it turns away can still parse it (a Unit test checks this). A browser without WebAssembly, WebAssembly SIMD or exception handling, 64-bit integer arrays or Blob downloads gets an explanation, and the runtime is never downloaded. In .NET 10 `Blazor.start()` does not settle when a download fails, so the script watches for the loader's unhandled errors that name a file under `_framework/`. Unrelated errors, such as one from a browser extension, are ignored. Once such errors have been quiet for 3 seconds without a completed start, a retry screen replaces the loading message. A download that never answers raises no error, so after 30 seconds a "still loading" hint offers a retry while loading continues.
- **Faults.** The workspace sits inside `Components/FaultBoundary`. A fault there shows a recovery screen instead of the page-wide error bar, and focus moves to its heading. It offers: return to the workspace (the open save and every applied change are kept; the unapplied draft is dropped), discard the session, or reload. Focus returns to the workspace after recovering. The session lives in `State/WorkspaceState` outside the boundary, and an apply is staged on a clone, so a fault cannot leave it half-written. No exception details are shown; the exception, including its message and stack, is logged to the browser console only. The unsaved-changes warning is armed by the shell, so it stays in force while the recovery screen is up.

## Party and boxes

- **Views.** `Services/StorageView` reads the party and the shown box from the session's current revision, and `Components/StorageBrowser` reads them again whenever the session, its revision or the shown box changes. Nothing is cached at open, so an empty position can never show or open an earlier entity. Opening a slot (`WorkspaceState.OpenSlot`) is decided on the live save: it never replaces an unapplied draft, and an empty position or a bad egg opens nothing. Only the party and one box are read, and no legality analysis runs while browsing. Box counts, slot counts and stored box names come from Core (`BoxCount`, `BoxSlotCount`, `IBoxDetailNameRead`); a box with no stored name is shown with Core's numbered default. Party positions at or after the party count are empty, even if bytes remain there. An entity that fails Core's `PKM.Valid` (checksum or sanity flag) is shown as a bad egg, as the game and WinForms show it, with nothing read from it.
- **Positions.** `State/SlotRef` names a party position or box slot. `SaveSession.Select` opens a draft from it, reading the live save; `Components/SlotText` builds every label from the position and contents (e.g. "Box 3, slot 7 (row 2, column 1): Zigzagoon").
- **Grid and list.** `Components/SlotGrid` is a single tab stop: arrow keys move between slots, Home/End to the ends of the row, Ctrl+Home/End to the first and last slot, and Enter or Space opens one; keys held with Alt or Meta are left to the browser. `wwwroot/grid-keys.js`, loaded with the page so nothing is fetched once a save is open, stops those keys from also scrolling the page. "Show as a list" gives the same slots as a table with an Open button for each slot that can be opened. The box selector and Previous/Next wrap past either end, and the first box shown is the save's in-game current box.
- **Party.** Party members open for inspection and legality analysis (analysed as party members), but the nickname fields are read-only and Apply is refused: writing the party needs the stat, HP and status policy that a later change adds.

## Build provenance and notices

The About panel shows the Web version (the repository `Version`) and the git commit the build was made from, which also identifies the Core revision. Both are embedded at build time as assembly metadata by the `AddBuildProvenance` target in `PKHeX.Web.csproj`, using the SDK's built-in git query, so no `git` executable is needed. The commit is the checked-out one; uncommitted changes are not reflected. It is recorded only when the git repository is this repository, so a build from a source archive, or from a copy inside another repository, records `unknown`; pass `-p:PKHeXSourceCommit=<sha>` to record it explicitly.

The publish output includes `LICENSE.txt`, `THIRD-PARTY-NOTICES.md` (every package the Web build restores, with its version and license, split into published, removed by trimming and build-only) and the upstream .NET notices under `licenses/`. Serve them together with the rest of `wwwroot`. The E2E tier fails if the restore graph changes without a matching update to `THIRD-PARTY-NOTICES.md`, or if the publish contains a package file the notices do not list as published. The runtime pack version comes from the installed SDK, so a different SDK patch needs a notices update; those checks are therefore in the opt-in E2E tier, which CI runs on the pinned SDK, rather than in Unit.

## Reproduce tests

Private files are supplied through environment variables; never copy them into source or `wwwroot`. Use absolute paths. The two fixture variables must refer to valid, decrypted real saves, not synthetic substitutes. Test downloads live in temporary Playwright storage and are removed when the browser context closes. Screenshots and traces are not recorded.

```sh
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export PKHEX_XY_SAVE='/absolute/private/path/to/xy-save'
export PKHEX_ORAS_SAVE='/absolute/private/path/to/oras-save'
export PKHEX_WEB_TEST_TIERS='E2E,RealSave'
export PKHEX_WEB_PUBLISHED="$PWD/PKHeX.Web/bin/Release/publish/wwwroot"
export PKHEX_PROOF_EVIDENCE="$(mktemp -d -t pkhex-proof-evidence)"

dotnet build Tests/PKHeX.Web.Tests/PKHeX.Web.Tests.csproj -c Release
```

Install the browsers matching the pinned Playwright package. If PowerShell is installed, use the generated `playwright.ps1 install chromium firefox webkit` script in the test output directory. On macOS ARM64, the bundled equivalent is:

```sh
Tests/PKHeX.Web.Tests/bin/Release/net10.0/.playwright/node/darwin-arm64/node \
  Tests/PKHeX.Web.Tests/bin/Release/net10.0/.playwright/package/cli.js \
  install chromium firefox webkit
```

Tests are split into tiers by `Category`. A run without `--filter`, including `dotnet test PKHeX.slnx`, executes only `Unit`, so desktop contributors without browsers or private saves still get a clean run. Passing any `--filter` or `--settings` replaces that default (and a filter on the solution applies to every project), so select tiers explicitly:

```sh
dotnet test Tests/PKHeX.Web.Tests/PKHeX.Web.Tests.csproj -c Release --no-build                                                          # Unit (synthetic saves, no setup)
PKHEX_WEB_TEST_TIERS=E2E dotnet test Tests/PKHeX.Web.Tests/PKHeX.Web.Tests.csproj -c Release --no-build --filter Category=E2E             # published app + Playwright
PKHEX_WEB_TEST_TIERS=RealSave dotnet test Tests/PKHeX.Web.Tests/PKHeX.Web.Tests.csproj -c Release --no-build --filter Category=RealSave   # also needs the private saves and the publish
PKHEX_WEB_TEST_TIERS=E2E,RealSave dotnet test Tests/PKHeX.Web.Tests/PKHeX.Web.Tests.csproj -c Release --no-build --filter "Category=Unit|Category=E2E|Category=RealSave"
PKHEX_WEB_TEST_TIERS=Perf PKHEX_WEB_PERF_REPORT="$(mktemp -d)" dotnet test Tests/PKHeX.Web.Tests/PKHeX.Web.Tests.csproj -c Release --no-build --filter Category=Perf   # boot baseline, also needs the publish
```

Set `PLAYWRIGHT_BROWSERS_PATH` consistently for installation and testing if you want a custom browser cache location. It is separate from the machine-wide SDK.

`E2E`, `RealSave` and `Perf` are also opt-in: their tests run only when the tier is named in `PKHEX_WEB_TEST_TIERS` (separated by commas, semicolons or spaces; case-insensitive), and are skipped otherwise. This keeps runners that ignore the project's default filter to `Unit`: `vstest.console` run directly on the built assembly (as Azure Pipelines' VsTest task does) runs the Unit tests and skips the others instead of failing them. An opted-in tier still fails, rather than skips, when its other environment variables are missing; `RealSave` needs `PKHEX_WEB_PUBLISHED` as well as the saves, because its browser test uses the published app. A filter for a tier that is not opted in skips every test and reports success, so name the tier in both places; `PKHeX.Web/tools/trx-all-executed.sh <results.trx>` fails a run in which any test was not executed. CI runs `Unit`, `E2E` and `Perf`; `RealSave` is for local runs with private saves.

Browser tests share one fixture that starts a loopback-only static host for the supplied published files and checks every boot, at the root and under `/PKHeX/`: only published files are requested and none fails; each response carries the expected MIME type, `nosniff`, the Content-Security-Policy header and `Referrer-Policy`, and the page reports no CSP violations. Each test ends by checking that nothing reached the network after boot, that nothing was persisted in the browser, and that no CSP violation occurred, including across reloads. The `/PKHeX/` cases change only the served HTML base href to reproduce a subpath deployment; all application binaries are identical. The host has no upload or save-processing endpoint. An absent fixture variable or missing usable entity fails rather than silently skipping the real-save proof.

Real-save tests select the first occupied, checksum-valid, writable boxed PK6 in box/slot order and fail if none exists. Party members cannot be changed yet, so the round trip edits a boxed one. Outside the edited slot and the Gen 6 block-checksum footer, the edited export must be byte-identical to the no-op export. No entity is injected into the real fixture.

Evidence JSON contains only family, slot kind (box), browser/version, hosting path, timings, and pass flags. It contains no file paths, names, trainer identifiers, entity contents, or save hashes. Native reference outputs and browser downloads are compared in memory; private bytes are never included in assertion messages.

## Boot baseline

The `Perf` tier measures how long the published app takes to become usable (WEB-PERF-001) and writes `boot-baseline.md` and `boot-baseline.json` to `PKHEX_WEB_PERF_REPORT`. It records numbers only and has no pass/fail threshold; it fails only when a boot cannot be measured (the file input never became usable within 60 seconds, a page error, or a request for anything other than a published file). `PKHEX_WEB_PERF_RUNS` sets the number of measured boots per configuration (default 5), after one unrecorded boot.

- Each sample uses a new browser profile. The cold boot starts a new browser process on the empty profile; the warm boot closes that browser and relaunches it on the same profile, like a returning visit, so only what the browser stored in the profile (its HTTP cache) carries over.
- "Shell ready" is the time from navigation start until the file input exists and is enabled, recorded in the page by a mutation observer.
- Chromium is measured on the 20 Mbps / 50 ms profile that `PKHeX.Web.md` states its targets for (≤5 s cold, ≤2 s cached; engineering targets, not claims), throttled through the DevTools protocol, which adds the latency to each request rather than emulating TCP round trips. Playwright cannot throttle Firefox or WebKit, so all three engines are also measured on unthrottled loopback, which shows the startup cost without the network.
- The loopback host runs in a deployment-caching mode: it serves the precompressed `.br`/`.gz` file the browser accepts, sends `ETag`s and answers revalidations with 304, and marks fingerprinted `_framework` files immutable and everything else `no-cache`. Playwright's WebKit does not accept Brotli from the plain-HTTP loopback host, so its rows are served as gzip, which overstates what an HTTPS deployment transfers; the report's boot-set table gives both sizes.
- The report lists the files a cold boot downloads with their raw, Brotli and gzip sizes, the raw size of each group of data embedded in PKHeX.Core (every boot downloads all of it inside the assembly) and its share of the published file, and flags a warm boot that reused nothing from the cache, since that row then measures a second download. Playwright's WebKit did so in every local run; the cause has not been established, and it is not evidence about Safari.

Numbers from a shared CI runner vary between runs and are not the reference desktop the targets name; compare runs on the same machine.

## Continuous integration

`.github/workflows/web.yml` runs for pull requests, and for pushes to `master` and `web/main`, that touch Core, Web, WinForms, the Drawing projects or the tests. It has two jobs. The `web` job runs on Linux. The workflow has read-only repository permissions, no secrets, and actions pinned to commit SHAs. The `web` job installs SDK 10.0.401 and pins it with a `global.json` written outside the checkout, because the notices list the runtime pack version that SDK brings. It runs the Core tests and the Web `Unit` tier, publishes Release, checks the trim-analysis warnings against the baseline, installs the Playwright browsers and runs the `E2E` tier against that publish, with `PKHEX_WEB_TEST_TIERS=E2E` and a check that every E2E test was executed, then runs the `Perf` tier on its own with the same check. It uploads the published `wwwroot`, and at the end the test results and reports: normalised trim warnings, the `dotnet list package --include-transitive` inventory, a size report and the boot baseline, the last two also shown in the run summary. `RealSave` never runs in CI.

The `azure-parity` job repeats upstream's Azure Pipelines build on Windows, so that the Web projects cannot break it unnoticed. It runs on `windows-2022`, whose Visual Studio 2022 17.14 and SDK match Azure's `windows-2025` agent (GitHub's `windows-2025` now has Visual Studio 2026), and uses GitVersion 5.x for `/p:Version`, a `nuget.exe` restore of `PKHeX.slnx` and a Release build with x86 Visual Studio MSBuild. There is deliberately no SDK pin: Visual Studio MSBuild uses the newest SDK its version supports on the image, as upstream does. It then runs `vstest.console` over `**\bin\Release\**\*Tests.dll` without `PKHEX_WEB_TEST_TIERS`, and `PKHeX.Web/tools/trx-check-opt-in-skips.ps1` requires the Core and Web Unit tests to pass and only the opt-in tiers to be skipped. The run summary lists the toolchain it used.

The E2E test `PublishesOnlyStaticDeployableFiles` keeps the publish deployable as static files: at most 20,000 files, none over 25 MiB (the Cloudflare Pages limits), only the expected asset types in their expected folders (runtime files in `_framework/`, upstream notices in `licenses/`, the app shell, license and notices at the root; so no source maps, symbols, sources or stray data files), and every precompressed `.br`/`.gz` file next to the asset it compresses. `PKHeX.Web/tools/size-report.sh <wwwroot>` prints the size report locally.

Blazor hides trim-analysis warnings in a normal publish. `PKHeX.Web/tools/trim-warnings.sh` publishes again with them enabled and compares them with `PKHeX.Web/trim-warnings.baseline.txt`, ignoring source locations and the ordinals in compiler-generated names. Any difference fails: justify a new warning, or drop a fixed one, then run the script with `--update` and commit the baseline.

## Boundaries

- Core chooses the format, parses entities, analyzes legality, writes slots and serializes saves. No Core source or public API changes are required.
- Apply edits a temporary working copy; Download does not confirm filesystem persistence. A changed session remains visibly edited after downloading.
- Replacing a changed session requires explicit discard or cancel; users can cancel replacement and download first.
- An empty box is not an invitation to generate a Pokémon. The app does not import entities, legalize them, edit species/stats, or manipulate dex/records.
- Input and working-save integrity are checked, but console acceptance and physical Safari/iOS behavior are not established by these tests.
- Playwright WebKit is engine coverage, not a claim of physical-device qualification.

See the root `PKHeX.Web.WasmProof.md` for the actual recorded evidence and limitations.

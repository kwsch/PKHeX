# Local WebAssembly round-trip proof

This is a standalone .NET 10 Blazor WebAssembly experiment, not the full PKHeX.Web MVP. It opens raw XY/ORAS saves locally, edits an existing boxed Pokémon’s nickname fields, runs Core legality analysis, and downloads a validated save. No save-processing server, upload API, persistent browser storage, analytics, sprites, or external runtime assets are used.

Install Microsoft's .NET 10 SDK for your platform. The verified development environment uses the machine-wide ARM64 SDK 10.0.401 and runtime 10.0.12. The Web package is pinned to 10.0.12; tests use Microsoft.Playwright 1.63.0. No repository-wide SDK pin is required. Both projects are part of `PKHeX.slnx`.

From the repository root:

```sh
dotnet publish PKHeX.Web/PKHeX.Web.csproj -c Release -o PKHeX.Web/bin/Release/publish
python3 -m http.server 8080 --bind 127.0.0.1 --directory PKHeX.Web/bin/Release/publish/wwwroot
```

Open `http://127.0.0.1:8080/`. Serve only the published `wwwroot`, never the repository or directory containing your saves. Runtime hosting uses an ordinary static server; it does not need .NET. The proof does not install a service worker or require `wasm-tools`; the default Release publish trims managed assemblies but uses the stock interpreter runtime without optional native relinking/AOT.

## Build provenance and notices

The page footer shows the Web version (the repository `Version`) and the git commit the build was made from, which also identifies the Core revision. Both are embedded at build time as assembly metadata by the `AddBuildProvenance` target in `PKHeX.Web.csproj`, using the SDK's built-in git query, so no `git` executable is needed. The commit is the checked-out one; uncommitted changes are not reflected. It is recorded only when the git repository is this repository, so a build from a source archive, or from a copy inside another repository, records `unknown`; pass `-p:PKHeXSourceCommit=<sha>` to record it explicitly.

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
```

Set `PLAYWRIGHT_BROWSERS_PATH` consistently for installation and testing if you want a custom browser cache location. It is separate from the machine-wide SDK.

`E2E` and `RealSave` are also opt-in: their tests run only when the tier is named in `PKHEX_WEB_TEST_TIERS` (separated by commas, semicolons or spaces; case-insensitive), and are skipped otherwise. This keeps runners that ignore the project's default filter to `Unit`: `vstest.console` run directly on the built assembly (as Azure Pipelines' VsTest task does) runs the Unit tests and skips the others instead of failing them. An opted-in tier still fails, rather than skips, when its other environment variables are missing; `RealSave` needs `PKHEX_WEB_PUBLISHED` as well as the saves, because its browser test uses the published app. A filter for a tier that is not opted in skips every test and reports success, so name the tier in both places; `PKHeX.Web/tools/trx-all-executed.sh <results.trx>` fails a run in which any test was not executed. CI runs only `Unit` and `E2E`; `RealSave` is for local runs with private saves.

Browser tests share one fixture that starts a loopback-only static host for the supplied published files and checks every boot, at the root and under `/PKHeX/`: only published files are requested and none fails; each response carries the expected MIME type, `nosniff`, the Content-Security-Policy header and `Referrer-Policy`, and the page reports no CSP violations. Each test ends by checking that nothing reached the network after boot, that nothing was persisted in the browser, and that no CSP violation occurred, including across reloads. The `/PKHeX/` cases change only the served HTML base href to reproduce a subpath deployment; all application binaries are identical. The host has no upload or save-processing endpoint. An absent fixture variable or missing usable entity fails rather than silently skipping the real-save proof.

Real-save tests select the first occupied, checksum-valid, writable boxed PK6 in box/slot order and fail if none exists. Party slots are not editable in this proof. Outside the edited slot and the Gen 6 block-checksum footer, the edited export must be byte-identical to the no-op export. No entity is injected into the real fixture.

Evidence JSON contains only family, slot kind (box), browser/version, hosting path, timings, and pass flags. It contains no file paths, names, trainer identifiers, entity contents, or save hashes. Native reference outputs and browser downloads are compared in memory; private bytes are never included in assertion messages.

## Continuous integration

`.github/workflows/web.yml` runs for pull requests, and for pushes to `master` and `web/main`, that touch Core, Web, WinForms, the Drawing projects or the tests. It has two jobs. The `web` job runs on Linux. The workflow has read-only repository permissions, no secrets, and actions pinned to commit SHAs. The `web` job installs SDK 10.0.401 and pins it with a `global.json` written outside the checkout, because the notices list the runtime pack version that SDK brings. It runs the Core tests and the Web `Unit` tier, publishes Release, checks the trim-analysis warnings against the baseline, installs the Playwright browsers and runs the `E2E` tier against that publish, with `PKHEX_WEB_TEST_TIERS=E2E` and a check that every E2E test was executed. It uploads the published `wwwroot`, the test results and reports: normalised trim warnings, the `dotnet list package --include-transitive` inventory, and a size report that is also shown in the run summary. `RealSave` never runs in CI.

The `azure-parity` job repeats upstream's Azure Pipelines build on Windows, so that the Web projects cannot break it unnoticed. It runs on `windows-2022`, whose Visual Studio 2022 17.14 and SDK match Azure's `windows-2025` agent (GitHub's `windows-2025` now has Visual Studio 2026), and uses GitVersion 5.x for `/p:Version`, a `nuget.exe` restore of `PKHeX.slnx` and a Release build with x86 Visual Studio MSBuild. There is deliberately no SDK pin: Visual Studio MSBuild uses the newest SDK its version supports on the image, as upstream does. It then runs `vstest.console` over `**\bin\Release\**\*Tests.dll` without `PKHEX_WEB_TEST_TIERS`, and `PKHeX.Web/tools/trx-check-opt-in-skips.ps1` requires the Core and Web Unit tests to pass and only the opt-in tiers to be skipped. The run summary lists the toolchain it used.

The E2E test `PublishesOnlyStaticDeployableFiles` keeps the publish deployable as static files: at most 20,000 files, none over 25 MiB (the Cloudflare Pages limits), only the expected asset types in their expected folders (runtime files in `_framework/`, upstream notices in `licenses/`, the app shell, license and notices at the root; so no source maps, symbols, sources or stray data files), and every precompressed `.br`/`.gz` file next to the asset it compresses. `PKHeX.Web/tools/size-report.sh <wwwroot>` prints the size report locally.

Blazor hides trim-analysis warnings in a normal publish. `PKHeX.Web/tools/trim-warnings.sh` publishes again with them enabled and compares them with `PKHeX.Web/trim-warnings.baseline.txt`, ignoring source locations and the ordinals in compiler-generated names. Any difference fails: justify a new warning, or drop a fixed one, then run the script with `--update` and commit the baseline.

## Boundaries

- Core chooses the format, parses entities, analyzes legality, writes slots and serializes saves. No Core source or public API changes are required.
- Apply edits a temporary working copy; Download does not confirm filesystem persistence. A changed session remains visibly edited after downloading.
- Replacing a changed session requires explicit discard or cancel; users can cancel replacement and download first.
- An empty box is not an invitation to generate a Pokémon. The proof does not import entities, legalize them, edit species/stats, or manipulate dex/records.
- Input and working-save integrity are checked, but console acceptance and physical Safari/iOS behavior are not established by these tests.
- Playwright WebKit is engine coverage, not a claim of physical-device qualification.

See the root `PKHeX.Web.WasmProof.md` for the actual recorded evidence and limitations.

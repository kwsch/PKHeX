# Local WebAssembly round-trip proof

This is a standalone .NET 10 Blazor WebAssembly experiment, not the full PKHeX.Web MVP. It opens raw XY/ORAS saves locally, edits an existing boxed Pokémon’s nickname fields, runs Core legality analysis, and downloads a validated save. No save-processing server, upload API, persistent browser storage, analytics, sprites, or external runtime assets are used.

Install Microsoft's .NET 10 SDK for your platform. The verified development environment uses the machine-wide ARM64 SDK 10.0.401 and runtime 10.0.12. The Web package is pinned to 10.0.12; tests use Microsoft.Playwright 1.63.0. No repository-wide SDK pin is required. Both projects are part of `PKHeX.slnx`.

From the repository root:

```sh
dotnet publish PKHeX.Web/PKHeX.Web.csproj -c Release -o PKHeX.Web/bin/Release/publish
python3 -m http.server 8080 --bind 127.0.0.1 --directory PKHeX.Web/bin/Release/publish/wwwroot
```

Open `http://127.0.0.1:8080/`. Serve only the published `wwwroot`, never the repository or directory containing your saves. Runtime hosting uses an ordinary static server; it does not need .NET. The proof does not install a service worker or require `wasm-tools`; the default Release publish trims managed assemblies but uses the stock interpreter runtime without optional native relinking/AOT.

## Reproduce tests

Private files are supplied through environment variables; never copy them into source or `wwwroot`. Use absolute paths. The two fixture variables must refer to valid, decrypted real saves, not synthetic substitutes. Test downloads live in temporary Playwright storage and are removed when the browser context closes. Screenshots and traces are not recorded.

```sh
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export PKHEX_XY_SAVE='/absolute/private/path/to/xy-save'
export PKHEX_ORAS_SAVE='/absolute/private/path/to/oras-save'
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
dotnet test Tests/PKHeX.Web.Tests/PKHeX.Web.Tests.csproj -c Release --no-build                              # Unit (synthetic saves, no setup)
dotnet test Tests/PKHeX.Web.Tests/PKHeX.Web.Tests.csproj -c Release --no-build --filter Category=E2E         # published app + Playwright
dotnet test Tests/PKHeX.Web.Tests/PKHeX.Web.Tests.csproj -c Release --no-build --filter Category=RealSave    # also needs the private saves
dotnet test Tests/PKHeX.Web.Tests/PKHeX.Web.Tests.csproj -c Release --no-build --filter "Category=Unit|Category=E2E|Category=RealSave"
```

Set `PLAYWRIGHT_BROWSERS_PATH` consistently for installation and testing if you want a custom browser cache location. It is separate from the machine-wide SDK.

Tests start their own loopback-only static host, using the supplied published files. The `/PKHeX/` cases change only the served HTML base href to reproduce a subpath deployment; all application binaries are identical. The host has no upload or save-processing endpoint. An absent fixture variable or missing usable entity fails rather than silently skipping the real-save proof.

Real-save tests select the first occupied, checksum-valid, writable boxed PK6 in box/slot order and fail if none exists. Party slots are not editable in this proof. Outside the edited slot and the Gen 6 block-checksum footer, the edited export must be byte-identical to the no-op export. No entity is injected into the real fixture.

Evidence JSON contains only family, slot kind (box), browser/version, hosting path, timings, and pass flags. It contains no file paths, names, trainer identifiers, entity contents, or save hashes. Native reference outputs and browser downloads are compared in memory; private bytes are never included in assertion messages.

## Boundaries

- Core chooses the format, parses entities, analyzes legality, writes slots and serializes saves. No Core source or public API changes are required.
- Apply edits a temporary working copy; Download does not confirm filesystem persistence. A changed session remains visibly edited after downloading.
- Replacing a changed session requires explicit discard or cancel; users can cancel replacement and download first.
- An empty box is not an invitation to generate a Pokémon. The proof does not import entities, legalize them, edit species/stats, or manipulate dex/records.
- Input and working-save integrity are checked, but console acceptance and physical Safari/iOS behavior are not established by these tests.
- Playwright WebKit is engine coverage, not a claim of physical-device qualification.

See the root `PKHeX.Web.WasmProof.md` for the actual recorded evidence and limitations.

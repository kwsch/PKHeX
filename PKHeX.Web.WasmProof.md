# PKHeX.Web — local XY/ORAS WebAssembly round-trip proof

Recorded **2026-09-26**. This proves one narrow flow in desktop browser engines. A user opens a raw XY or ORAS save from local disk, edits the nickname of an existing boxed Pokémon, runs Core legality analysis, and downloads a validated save. Everything runs in a Release-published Blazor WebAssembly artifact served from a plain static host. It does **not** establish MVP completion, all-generation compatibility, console acceptance, physical Safari/iOS behavior, or mobile support.

## Result

| Requirement | Result |
|---|---|
| No-op and edited round trips, XY real save | **Pass**: 3 engines × 2 hosting paths |
| No-op and edited round trips, ORAS real save | **Pass**: 3 engines × 2 hosting paths |
| Browser downloads are byte-identical to native Core references (same Core revision) | **Pass**: all 12 real-save runs |
| Downloaded file reopens in the browser; edited fields, checksums and browser/native legality agree | **Pass** |
| Only the edited box slot and the Gen 6 block-checksum footer differ between the no-op and edited exports | **Pass** |
| Original fixture SHA-256 unchanged after each run | **Pass** |
| No network requests after application boot; no localStorage/sessionStorage/IndexedDB/Cache Storage/service worker/cookies | **Pass**: all 18 browser runs |
| Malformed, truncated, corrupted-checksum, unsupported-family (Gen 5 and non-BEEF ORAS-sized), oversized, and empty inputs are rejected while the existing session is retained | **Pass**: 6 browser runs |
| Canceled draft, canceled replacement, reload clearing the session | **Pass** |
| Existing legal/illegal PK6 legality fixtures classify identically in the browser | **Pass** |

Total: 25 tests, 25 passed, 0 failed, 0 skipped (`dotnet test` wall time ≈ 68 s, including build).

### Per-family / engine / path results

Timings come from one run on an Apple Silicon Mac, loopback host, fresh browser context, headless. "Boot" is navigation until the file input renders. "Total" covers boot, load, analyze, no-op download, reload of that download, edit, analyze, apply, edited download, browser reopen, analyze, and privacy checks. Every row: slot kind = box, no-op = native, edited = native, reopened OK, original unchanged, privacy passed.

| Family | Engine (Playwright build) | Path | Boot ms | Total ms |
|---|---|---|---:|---:|
| XY | Chromium 153.0.8010.12 | `/` | 228 | 1837 |
| XY | Chromium 153.0.8010.12 | `/PKHeX/` | 227 | 1856 |
| XY | Firefox 155.0 | `/` | 400 | 2422 |
| XY | Firefox 155.0 | `/PKHeX/` | 573 | 2486 |
| XY | WebKit 26.6 | `/` | 356 | 2079 |
| XY | WebKit 26.6 | `/PKHeX/` | 286 | 1869 |
| ORAS | Chromium 153.0.8010.12 | `/` | 312 | 1960 |
| ORAS | Chromium 153.0.8010.12 | `/PKHeX/` | 287 | 1974 |
| ORAS | Firefox 155.0 | `/` | 475 | 2415 |
| ORAS | Firefox 155.0 | `/PKHeX/` | 452 | 2332 |
| ORAS | WebKit 26.6 | `/` | 304 | 1849 |
| ORAS | WebKit 26.6 | `/PKHeX/` | 311 | 1851 |

WebKit rows are **engine coverage** via Playwright's WebKit build. They are not a physical Safari or iOS qualification. Loopback timings do not predict real network or device performance.

## Environment and revisions

| Item | Value |
|---|---|
| Core revision | `17157eb18013dc29a44f7bb7810117390431087b` plus the uncommitted proof files only (no Core source changes) |
| .NET SDK | 10.0.401 (commit `e34a38d2ae`), osx-arm64, the latest SDK in Microsoft's 10.0 release metadata on the recorded date |
| Runtime / host | Microsoft.NETCore.App and Microsoft.AspNetCore.App 10.0.12 |
| Installation | Machine-wide, `/usr/local/share/dotnet`, from Microsoft's official `dotnet-sdk-10.0.401-osx-arm64.pkg`. The pkg SHA-512 matches `releases.json` (`d431f774…c508ff82`). `pkgutil` reports Developer ID Installer: Microsoft Corporation, notarized. The installer receipt `com.microsoft.dotnet.dev.10.0.401.component.osx.arm64` is present. `dotnet --info` works from a fresh login shell via `/etc/paths.d`. |
| Workloads | None. The default Release publish uses the stock interpreter runtime; `wasm-tools` (relinking/AOT) is not required and was not installed. |
| Web packages | Microsoft.AspNetCore.Components.WebAssembly 10.0.12 |
| Test packages | Microsoft.Playwright 1.63.0, xunit 2.9.3, Microsoft.NET.Test.Sdk 18.0.1 |
| Browsers | Chromium 153.0.8010.12 (v1243), Firefox 155.0 (v1543), WebKit 26.6 (v2359). All are Playwright builds, run headless. |
| OS | macOS 26.5, arm64 |

## Publish

Default Release publish (trimming **enabled**, the Blazor default; no AOT; no relinking):

```sh
dotnet publish PKHeX.Web/PKHeX.Web.csproj -c Release -o <out>
```

- Warnings: **0**. Blazor suppresses trim-analysis warnings by default. Disclosure: a separate diagnostic publish with `-p:SuppressTrimAnalysisWarnings=false` reports **38** trim warnings (IL2026 ×20, IL2070 ×9, IL2046 ×3, IL2104 ×2, IL2065, IL2075, IL2098, IL2111). They come from Core's reflection-based batch editor (`BatchEditingBase`, `EntityBatchEditor`, `ReflectUtil`), which this proof does not use, and from `LengthAttribute` annotations on encounter/BinLinker span parameters. IL2104 is aggregated from `Microsoft.AspNetCore.Components` and `Microsoft.JSInterop`. None of these warnings caused a failure in the exercised XY/ORAS parse/legality/write paths. They remain a gate for broader Core feature use.
- Artifact (`wwwroot`): 177 files. 59 uncompressed assets totaling **27.4 MiB**, plus precompressed Brotli **5.8 MiB** and gzip **8.4 MiB**. The largest asset is `PKHeX.Core.*.wasm`: 17.8 MiB raw, 2.9 MiB Brotli, 4.7 MiB gzip. The largest runtime asset is `dotnet.native.*.wasm` (2.9 MiB raw).
- Hosting: any static server. `python3 -m http.server --bind 127.0.0.1 --directory <out>/wwwroot` serves `index.html` (`text/html`), `.wasm` (`application/wasm`) and `.js` (`text/javascript`) correctly. The `/PKHeX/` tests change only the `<base href>` in the served `index.html`; all binaries are identical.

## Reproduction

Use absolute paths. Keep private saves, the publish output, and evidence outside the repository and outside the served directory.

```sh
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export PKHEX_XY_SAVE='/absolute/private/path/to/xy-save'
export PKHEX_ORAS_SAVE='/absolute/private/path/to/oras-save'
OUT="$(mktemp -d)"
export PKHEX_WEB_PUBLISHED="$OUT/wwwroot"
export PKHEX_PROOF_EVIDENCE="$(mktemp -d)"
export PKHEX_WEB_TEST_TIERS='E2E,RealSave'

dotnet publish PKHeX.Web/PKHeX.Web.csproj -c Release -o "$OUT"
dotnet build Tests/PKHeX.Web.Tests/PKHeX.Web.Tests.csproj -c Release
P=Tests/PKHeX.Web.Tests/bin/Release/net10.0/.playwright
"$P/node/darwin-arm64/node" "$P/package/cli.js" install chromium firefox webkit
dotnet test Tests/PKHeX.Web.Tests/PKHeX.Web.Tests.csproj -c Release --no-build --filter "Category=E2E|Category=RealSave"
```

The browser and real-save tiers run only when named in `PKHEX_WEB_TEST_TIERS` and selected by the filter; without the opt-in they are skipped. Once opted in, a missing required variable fails the tests rather than skipping them. (This procedure was updated after the proof: at the time the tiers were not yet split, and a plain `dotnet test` ran everything.)

## What the tests check

**Native preflight** (`RealSavePreflightTests`). For each fixture: hash the file, parse it with `SaveUtil.GetSaveFile`, and require `SAV6XY`/`SAV6AO`, valid checksums and an exportable state. Then select the first occupied, checksum-valid, writable boxed `PK6` in box/slot order, failing with a clear message if there is none. Finally, confirm the Web session's no-op export equals native `SaveFile.Write()`.

**Real-save browser round trip** (`RealSaveBrowserTests.RealSavePublishedRoundTrip`, 12 cases):
1. Build native no-op and edited references from the same Core build. The deterministic nickname is `WASM Proof`, or `WASM Test` if the original already equals `WASM Proof`. The flag is set, and the write uses `EntityImportSettings.None`.
2. Assert that the native edit changed only the target slot's bytes before serialization.
3. Load the Release artifact from a loopback-only static host, and load the real file through the file input.
4. Analyze legality and compare the verdict and full report with native `LegalityAnalysis`. The report text is never printed.
5. Download the no-op export and compare it with the native bytes.
6. Reopen it through the file input, edit, analyze, apply, and download.
7. Compare the edited download byte-for-byte with the native reference. Reparse it: checksums valid, nickname applied, party count and party bytes unchanged. Restoring the nickname storage and flag must reproduce the original entity exactly. The only differing bytes between the no-op and edited exports are in the edited slot and the checksum footer.
8. Reopen the edited download in the browser (explicit replace), confirm the nickname, and compare legality with native again.
9. Run the privacy checks. Reload and confirm the session is gone. Recheck the original file's SHA-256.

**Failure paths** (`PublishedFailuresDraftsAndKnownLegality`, 6 cases, synthetic fixtures). These cover:
- a canceled draft blocking download until reverted;
- rejection of empty, 512-byte, truncated, first-byte-corrupted, 16 MiB + 1, Gen 5 and non-BEEF ORAS-sized inputs, each followed by a download proving the previous session is unchanged;
- canceled replacement keeping a pending draft;
- existing legal (Zigzagoon) and illegal (Ditto) PK6 fixtures giving the same Valid/Invalid verdict and report as native.

Synthetic saves are Core blank saves with a test-only BEEF footer. They supplement the real-save runs and do not replace them.

**Session unit tests** (`SessionTests`) cover input-buffer non-mutation, original-byte retention, draft/cancel/apply, export equality with native, rejection of invalid inputs and drafts, and legality classification.

## Implementation boundaries actually exercised

- Input: one local file, a 16 MiB limit enforced before and during streaming, and the original bytes copied before parsing. Only `SAV6XY`/`SAV6AO` with valid checksums are accepted. A rejected replacement leaves the current session untouched.
- Edit: the selected `PK6` is cloned, and only `Nickname` and `IsNicknamed` change. Apply runs `SlotInfoBox.CanWriteTo` (slot and entity) and writes to a cloned save with `EntityImportSettings.None`. It verifies the stored slot before committing.
- Export: `SaveFile.Write()` on a clone. A separate output copy is reparsed and validated before the download starts. The download is named `main`.
- There are no backend, telemetry, remote assets, sprites, persistent storage, service worker, crypto dependencies or auto-legalization. CSP is `default-src 'self'` with `'wasm-unsafe-eval'`.
- No public Core API changes were needed.

## Fixture note

The first XY save supplied had all 31 boxes empty (one party Pokémon only). The native preflight failed with "Real fixture contains no occupied, checksum-valid, writable boxed PK6", as the proof requires. The owner then supplied a different real XY save with occupied boxes, and all recorded results use that save. Save contents, hashes, trainer data, local paths, screenshots and traces are excluded from this report. No traces or screenshots were recorded.

## Not demonstrated

Other save families or wrappers, party editing, any field other than the nickname, console or game acceptance of the output, physical Safari/iOS or Android devices, mobile layouts, assistive-technology testing, memory or low-end device budgets, public hosting, and upstream/maintainer acceptance.

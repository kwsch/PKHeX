# PKHeX.Web — project vision and implementation design

Status: proposed. A narrow local WebAssembly proof exists (see [PKHeX.Web.WasmProof.md](PKHeX.Web.WasmProof.md)); the MVP is not implemented. Evidence inspected on **2026-09-26**.

**Demonstrated (2026-09-26, local only):** a Release-published, trimmed Blazor WebAssembly build of unmodified Core can open real raw XY and ORAS saves in Chromium, Firefox and Playwright WebKit (engine coverage only), at `/` and `/PKHeX/` on a plain static host. It validates checksums, runs legality analysis matching native Core, edits one boxed PK6 nickname through `EntityImportSettings.None`, and downloads output byte-identical to native Core for both no-op and edited round trips. It sends no post-boot network requests and uses no browser storage. Nothing else in this document is browser-proven.

**Maintainer interest:** not established. No maintainer outreach, issue, PR or public deployment has been made; all work is local exploration.

## Contents

- [Executive summary](#executive-summary)
- [Goals and non-goals](#goals-and-non-goals)
- [Repository analysis](#repository-analysis)
- [PKForge analysis](#pkforge-analysis)
- [Technology choice](#technology-choice)
- [Browser/WASM blocker analysis](#browserwasm-blocker-analysis)
- [Architecture](#architecture)
- [State model](#state-model)
- [Upstream refactoring opportunities](#upstream-refactoring-opportunities)
- [UX structure and accessibility](#ux-structure-and-accessibility)
- [Browser and device support](#browser-and-device-support)
- [Privacy and security model](#privacy-and-security-model)
- [Static hosting strategy](#static-hosting-strategy)
- [Performance](#performance)
- [Compatibility matrix](#compatibility-matrix)
- [Definition of MVP](#definition-of-mvp)
- [Testing strategy](#testing-strategy)
- [CICD](#cicd)
- [Git and upstream contribution strategy](#git-and-upstream-contribution-strategy)
- [Post-MVP roadmap](#post-mvp-roadmap)
- [User-story backlog](#user-story-backlog)
- [Prioritised implementation matrix](#prioritised-implementation-matrix)
- [Licensing](#licensing)
- [Risks and decision gates](#risks-and-decision-gates)
- [External technical references](#external-technical-references)
- [Adversarial Review Summary](#adversarial-review-summary)

## Executive summary

PKHeX.Web is a proposed first-class browser UI within PKHeX, alongside PKHeX.WinForms, consuming the same PKHeX.Core project. Its purpose is to let users inspect and edit locally exported Pokémon saves from desktop and tablet browsers without installing a Windows application. A curated, accessible editing workflow is more valuable initially than reproducing every desktop dialog.

The target is a standalone **Blazor WebAssembly application on .NET 10**, published as static assets. Save parsing, editing, legality analysis, and serialization happen on the user's device. No processing server, database, accounts, cloud storage, or telemetry is required. Downloading the application requires ordinary HTTP requests; opening a save must not transmit its contents, filename, or derived values.

Core already supplies much more than binary formats: slot operations/history, editing helpers, game data, localization, conversion, and legality formatting exist. Web must reuse these capabilities, extracting additional reusable behavior from WinForms only when evidence justifies it. PKForge demonstrates useful workflows but depends on a different pinned Core revision, Android integration, filesystem storage, and AutoMod. It is neither the codebase nor a dependency for this proposal.

Browser feasibility is promising but **not proven**. Confirmed obstacles include default AES/MD5 providers unsupported in the browser and Windows-targeted drawing assemblies. Existing cryptography provider interfaces make a local-only solution plausible. Compilation, published trimming, family-specific round trips, and real-browser measurements are mandatory gates before promising support.

## Goals and non-goals

### Goals

- Integrate upstream with a direct project reference to Core, familiar C# conventions, focused tests, and small contributions.
- Provide static hosting, easy self-hosting, and near-zero hosting cost within provider quotas.
- Preserve original input bytes; make edits and export explicit; discard application-owned save state on reload/close.
- Deliver a coherent open → summary → party/boxes → editor → legality → apply → export journey.
- Support keyboard and assistive technology use, responsive desktop/tablet layouts, and practical mobile use.
- Keep game rules, legality, checksums, and format conversion in Core. Explain the limitations of legality results.
- Publish an evidence-based support matrix with supported save families and browser versions for each release.

### Non-goals

No WinForms clone, desktop replacement, native plugin loading, cloud synchronization, accounts, backend save processing, or full editor parity in MVP. No automatic legalization, ROM-hack support imported from PKForge, console connection, save extraction from hardware, ROM distribution, or circumventing console-specific encryption. No bank, persistent recent saves, PWA installation, or service worker in MVP. Do not implement custom save parsers in Web to make unsupported formats appear supported.

## Repository analysis

### Evidence baseline

| Source | Revision / observation |
|---|---|
| Current PKHeX checkout | `17157eb18013dc29a44f7bb7810117390431087b` |
| Local PKForge checkout | `02e19b25c90bcbc0274669beeb2d217fabd7038b` |
| PKForge's embedded PKHeX checkout | `396600ed8fb2ce7a40919ab528e135afb75d04e4`; not the current source of truth |
| Tooling | `dotnet` absent from PATH and checked common user/system install locations; no builds, browser probes, or tests executed for this document |
| Working tree at start | Clean |
| Documentation convention | Root README/license; no established root `docs/` directory; this document belongs at root |

Repository findings are source inspection, not runtime proof. A negative text search is not a transitive compatibility guarantee. Relative links below refer to this checkout; PKForge links require the sibling clone. External sources are listed with access date at the end.

### Projects and reusable capabilities

[Directory.Build.props](Directory.Build.props) specifies version `26.08.26`, C# 14, and nullable references. Both [PKHeX.sln](PKHeX.sln) and [PKHeX.slnx](PKHeX.slnx) exist. The latter includes Core, Drawing, Drawing.PokeSprite, Drawing.Misc, WinForms, and Core tests. It references `LICENSE.md`, while this checkout's file is `LICENSE`; this pre-existing discrepancy is not a reason to rename files in a Web contribution.

| Area | Actual implementation | Web consequence |
|---|---|---|
| Core | [PKHeX.Core.csproj](PKHeX.Core/PKHeX.Core.csproj), `net10.0`, embedded `Resources/**`, no package references in this project | Reference the project, not a second NuGet copy or PKForge fork |
| Desktop graphics | [Drawing](PKHeX.Drawing/PKHeX.Drawing.csproj), [PokeSprite](PKHeX.Drawing.PokeSprite/PKHeX.Drawing.PokeSprite.csproj), [Misc](PKHeX.Drawing.Misc/PKHeX.Drawing.Misc.csproj), `net10.0-windows`; System.Drawing.Common in Drawing | Do not reference these assemblies from Web; distinguish static image files from Windows rendering code |
| Desktop UI | [WinForms project](PKHeX.WinForms/PKHeX.WinForms.csproj), `net10.0-windows` | Workflow evidence only; no control types in Web/shared Core APIs |
| Tests | [Core tests](Tests/PKHeX.Core.Tests/PKHeX.Core.Tests.csproj), `net10.0`, xUnit, FluentAssertions | Reuse fixtures and conventions where rights permit; desktop execution alone does not prove browser support |
| Save recognition | [SaveUtil](PKHeX.Core/Saves/Util/SaveUtil.cs), `TryGetSaveFile(Memory<byte>, out SaveFile?, string?)` | Feed a private mutable copy of browser bytes; the memory overload is not the path overload and does not wrap all exceptions |
| Save lifecycle | [SaveFile](PKHeX.Core/Saves/SaveFile.cs), `Clone()`, `Write(BinaryExportSetting)`, party/box getters and setters | Preserve concrete save implementation, exportability, checksums, metadata, and container handling |
| Container metadata | [SaveFileMetadata](PKHeX.Core/Saves/SaveFileMetadata.cs), `Finalize`, header/footer/handler | A raw `Data` download is not a correct generic export |
| PKM recognition/conversion | [EntityFormat](PKHeX.Core/PKM/Util/EntityFormat.cs), `GetFromBytes`; [EntityConverter](PKHeX.Core/PKM/Util/Conversion/EntityConverter.cs), `ConvertToType` | Conversion must use Core and inspect its result; not all paths are reversible or permitted |
| Editing | [CommonEdits](PKHeX.Core/Editing/CommonEdits.cs), applicators, `PKM`, format interfaces | Use typed fields and Core-derived limits, never a second rules database |
| Slot orchestration | [SaveDataEditor](PKHeX.Core/Editing/Saves/Editors/SaveDataEditor.cs), [SlotEditor](PKHeX.Core/Editing/Saves/Slots/SlotEditor.cs), [SlotChangelog](PKHeX.Core/Editing/Saves/Slots/SlotChangelog.cs), `ISlotInfo`, `IPKMView` | Existing reusable behavior; do not propose moving all slot logic out of WinForms |
| Legality | [LegalityAnalysis](PKHeX.Core/Legality/LegalityAnalysis.cs), [LegalityFormatting](PKHeX.Core/Legality/Formatting/LegalityFormatting.cs) | Analyze a cloned draft with appropriate personal table and slot context; display Core reports |
| Lists/data | [GameInfo](PKHeX.Core/Game/GameStrings/GameInfo.cs), [FilteredGameDataSource](PKHeX.Core/Game/GameStrings/FilteredGameDataSource.cs), personal tables, encounters | Instantiate session-appropriate filtered lists; avoid hardcoded species/move/item lists |
| Showdown | [ShowdownSet](PKHeX.Core/Editing/BattleTemplate/Showdown/ShowdownSet.cs), [ShowdownParsing](PKHeX.Core/Editing/BattleTemplate/Showdown/ShowdownParsing.cs) | Text parsing is separate from generating a legal encounter |

Relevant source details change the design:

1. `SaveFile.Buffer` owns mutable memory supplied by the parser. [SAV7](PKHeX.Core/Saves/SAV7.cs) clears signature bytes and signs output. Preserve original bytes before parsing; serialization need not be byte-identical to input.
2. `SaveFile.Write()` calls `GetFinalData()` and `Metadata.Finalize()`. Checksum calculation can mutate the save. Export from a snapshot and reopen a separate output copy.
3. [EntityImportSettings](PKHeX.Core/Saves/Storage/EntityImportSettings.cs) controls handler adaptation, dex updates, and records. Default setters resolve global options enabled by default. “Apply an edit” must not accidentally behave like receiving a traded Pokémon.
4. `SlotEditor.Set` uses default import settings for ordinary Set operations. Reusing it unchanged is incompatible with a surgical-edit contract; use explicit Core setters/settings within a controlled transaction or add a narrowly scoped settings overload upstream.
5. Early-generation interpretation is not always self-describing. [SaveLanguage](PKHeX.Core/Saves/Util/SaveLanguage.cs) uses inference/fallbacks, and WinForms `Main.SanityCheckSAV` includes user choices and FRLG personal-table handling. Memory parsing alone is insufficient for confident edition/language UX.
6. Some global mutable state exists (`GameInfo`, provider registration, language defaults, save import defaults). MVP uses one active session per tab and initializes immutable startup settings before parsing; it does not change global settings per component.

### Existing CI and contribution rules

No checked-in GitHub Actions workflow files were found under `.github` at the baseline. Do not claim to extend a workflow that is absent. Propose new, narrowly scoped jobs without replacing any external maintainer release pipeline.

[CONTRIBUTING.md](.github/CONTRIBUTING.md) requires maintainable, tested work and separating non-GUI behavior from UI. It says draft PRs may be rejected and directs questions to project community channels. The contribution strategy below follows that guidance rather than opening a long-lived draft PR.

## PKForge analysis

PKForge targets Android through MAUI on `net10.0-android`; supporting libraries target `net10.0`. Its “Chrome” project means Skia drawing/design primitives, not a browser front end. Its [README](../PKForge/README.md) describes architectural docs that are not present as text in the inspected `docs/` tree; source is the stronger evidence.

| Reference | Useful idea | Boundary / concern |
|---|---|---|
| [SaveSessionService](../PKForge/src/PKForge.Infrastructure/SaveSessionService.cs) | Replace a session only after successful opening; isolated baseline and revert | Document IDs, identity persistence, backups and disk-write confirmation do not map directly to browser downloads |
| [SaveEngineSession](../PKForge/src/PKForge.Engine/SaveEngineSession.cs) | Copies bytes before parsing; explicit `EntityImportSettings.None`; capability checks; stat previews on clones | Large class combines slots, bag, trainer, dex, metadata and generation-specific logic; do not copy this service wholesale |
| [MonFieldService](../PKForge/src/PKForge.Engine/MonFieldService.cs) | Conditional fields, form dependencies, explicit side-effect explanations | Repeats WinForms form-change orchestration, includes large PID search loops and hardcoded special cases; candidates for verified shared helpers, not Web copies |
| [TransferPreviewService](../PKForge/src/PKForge.Engine/TransferPreviewService.cs) | Dry-run destination conversion and a before/after explanation | Its permissive backward-transfer policy and custom conversion layers are not an upstream requirement |
| [SaveParser](../PKForge/src/PKForge.Engine/SaveParser.cs) | Early ambiguity, preservation of original envelope, useful unsupported-container messages | Adds padding handling, ROM-hack routes and custom save types; do not transplant these into Web |
| [FileBankService](../PKForge/src/PKForge.Infrastructure/FileBankService.cs) | Stable bank IDs, index/data consistency, backups and recovery | Files, locks and filesystem atomicity must become IndexedDB transactions in a later opt-in library, not a virtual filesystem bank in MVP |
| [ShowdownTeamService](../PKForge/src/PKForge.Engine/ShowdownTeamService.cs) | Preview parsed teams before placing any entities | Legalizes through AutoMod; that is not equivalent to Core Showdown import |
| `SaveEngineSession.GetMysteryGiftInbox`, inventory/dex/trainer methods | Feature exposure via `IMysteryGiftStorageProvider`, inventory pouches, concrete dex structures | Gift inspection does not establish complete injection support; generic seen/caught projections lose forms/languages/research detail |
| [Engine project](../PKForge/src/PKForge.Engine/PKForge.Engine.csproj), [AutoMod shim](../PKForge/src/PKForge.AutoMod/PKForge.AutoMod.csproj) | One engine revision per application | Engine directly depends on AutoMod; shim compiles external plugin sources. Keep this dependency out of baseline Web |
| Encounter, RTC, event, daycare, roamer and living-dex services | High-value post-MVP workflows | Evaluate independently against current Core; no automatic ROM-hack, generated collection, or legalization adoption |
| App sprite/update/community/Pokepark services | Mobile presentation and discovery ideas | Network services, runtime item-art fetches and social features conflict with a self-contained MVP |

[PKForge .gitmodules](../PKForge/.gitmodules) points Core at `sofianeelhor/PKForge-PKHeX`, not `kwsch/PKHeX`. Comments describing pristine or pinned Core do not establish equivalence with this checkout. Any apparent missing upstream capability must be checked here first. PKForge's Android trimming experience is not WASM validation: Android has different crypto, native graphics, threads, and storage.

The architectural lesson is small UI/application adapters over Core with explicit mutations and previews. Reproducing PKForge's independent domain DTO layer for every Core property would create a second maintenance burden inside the upstream repository. Use narrow view models where UI validation requires them; keep `SaveFile` and `PKM` in the C# application layer.

## Technology choice

Use `PKHeX.Web/PKHeX.Web.csproj`, standalone `Microsoft.NET.Sdk.BlazorWebAssembly`, `net10.0`, direct reference to Core, and inherited repository C# settings. Add it to both maintained solution formats. No ASP.NET server project, SSR, SignalR circuit, hosted Blazor template, or “Auto” rendering mode is required. All rendering and domain work run in the client.

Start with the normal interpreter/Jiterpreter configuration and Release trimming. AOT is an optional measured optimization, not a compatibility fix; larger downloads and toolchain cost must be justified by legality benchmarks. Do not describe browser execution as ordinary desktop JIT. Microsoft documents this tradeoff in [WebAssembly build tools and AOT](https://learn.microsoft.com/en-us/aspnet/core/blazor/webassembly-build-tools-and-aot?view=aspnetcore-10.0).

Use semantic Razor components and repository-owned CSS with grid/flex layouts and design tokens. Avoid a large UI suite initially. Use small JS modules only for browser file/drop/download APIs, focus/navigation hooks, and later worker integration. Do not duplicate game logic in JavaScript. Native file input remains the universal fallback; File System Access API is not required.

`InputFile`/browser file reading does not inherently upload when used in standalone WASM. Set an explicit read limit and enforce it while streaming; do not rely on the default `OpenReadStream` limit or the filename extension. Initial raw-save limit: **16 MiB**, one file at a time, no archives or whole memory cards in MVP. Reassess this release policy against actual accepted Core families and fixtures whenever Core changes. The inspected mainline save sizes fit below this cap; the cap is not a claim that every file below it is safe. See [file input guidance](https://learn.microsoft.com/en-us/aspnet/core/blazor/file-uploads?view=aspnetcore-10.0).

Export uses a local byte stream/Blob, a user-initiated download action, safe name, and object-URL cleanup. A completion callback means download initiation, not confirmed disk persistence. Keep generated output in memory only until the user leaves the export panel or generates another snapshot. See [download guidance](https://learn.microsoft.com/en-us/aspnet/core/blazor/file-downloads?view=aspnetcore-10.0).

No localStorage, IndexedDB, persistent preferences, recent-file list, or service worker in MVP. Ordinary browser HTTP caching of application assets is acceptable and distinct from save persistence. Post-MVP PWA caching contains app assets only; bank persistence requires an explicit opt-in with backup/eviction disclosure. PWA support must never introduce automatic restoration of an editing session.

## Browser/WASM blocker analysis

**Confirmed** means source plus platform documentation establishes an incompatible default path; it does not mean this document executed that path. **Risk** means investigation/measurement is required. **Avoidable** means the unsupported behavior is not needed in the proposed workflow.

| Blocker / status | Location | Impact | Proposed resolution | Upstream suitability |
|---|---|---|---|---|
| Default AES — confirmed | [IAesCryptographyProvider](PKHeX.Core/Saves/Encryption/Providers/IAesCryptographyProvider.cs), [MemeKey](PKHeX.Core/Saves/Encryption/MemeCrypto/MemeKey.cs), [HomeCrypto](PKHeX.Core/PKM/HOME/HomeCrypto.cs) | `Aes.Create()` unsupported in browser; Gen 7 save signing and HOME entity crypto affected | Register a reviewed managed synchronous provider before use; test ECB/CBC, no padding, in-place spans and IV behavior. Keep affected features disabled until validated | Existing extension point; browser implementation can live in Web initially; no Core crypto rewrite |
| Default MD5 — confirmed | [IMd5Provider](PKHeX.Core/Saves/Encryption/Providers/IMd5Provider.cs), [SAV8BS](PKHeX.Core/Saves/SAV8BS.cs) | BDSP checksum validation/export uses unsupported `MD5.HashData` | Managed provider through existing interface; browser/native known-answer comparison and BDSP round trip | Same approach; MD5 is required format compatibility, not a new security primitive |
| Windows graphics — confirmed | Drawing projects listed above | System.Drawing rendering cannot be a runtime Web dependency | Build-time static sprite manifest/atlas using existing source image files, fully fetched before file access; browser composition and placeholder if mapping unavailable | Keep Windows code unchanged; share only proven platform-neutral mapping if needed |
| Host filesystem assumptions — avoidable | `SaveFinder`, path overloads of `SaveUtil`, `BoxUtil`, `MysteryUtil`, `SCBlockUtil`, file exporters | Browser cannot scan user's disk or write arbitrary paths | Use byte-oriented Core methods; browser selection/download adapters; no directory scans at startup | No wholesale removal of desktop APIs from Core |
| Startup/network coupling — avoidable | [StartupUtil](PKHeX.Core/Editing/Program/StartupUtil.cs), [NetUtil](PKHeX.Core/Util/NetUtil.cs), `UpdateUtil`, URL-based Showdown/Pokepaste helpers | Disk initialization or synchronous network calls can fail/freeze and violate self-contained behavior | Initialize required resources explicitly; embedded event data; pasted Showdown text only; no auto-update API calls | Web composition choice, not generic async rewrite of Core |
| Reflection/trimming — risk | [ReflectUtil](PKHeX.Core/Util/ReflectUtil.cs), localization helpers, reflective batch/property workflows | `RequiresUnreferencedCode` and runtime property access need preservation analysis | Typed editor access; inspect publish warnings; precise annotations only with tests; no blanket warning suppression or preserve-all by default | Small shared annotations only when reproducible |
| Serialization — risk, not blanket blocker | [LocalizationStorage](PKHeX.Core/Util/Localization/LocalizationStorage.cs), generated localization contexts | Some serialization is already source-generated; selected paths still need published tests | Exercise localized legality and all UI-used data in trimmed publish; explicit source generation for new serializable metadata | Follow existing approach |
| Parallel seed search — optional performance/platform risk | [LumioseSolver](PKHeX.Core/Legality/Encounters/Templates/Gen9a/LumioseSolver.cs) | Optional searches use `Parallel.ForEach`, potentially billions of candidates | Keep `SearchShiny1`/`SearchShinyN` defaults false. Do not promise cancellation of synchronous analysis. Advanced search excluded | No requirement for shared-memory threads or cross-origin isolation in MVP |
| UI-thread CPU work — risk | Legality, parsing, synchronous cryptography; `Task.Run` also appears in localization | Async wrapping does not move CPU work off the browser UI thread | Measure complete workflows; bound/debounce inputs; offload isolated byte-based jobs to a worker only if needed and browser-tested | Web scheduling boundary; Core remains synchronous unless independently justified |
| Embedded data/startup cost — risk | Core resources, PokeSprite resources, `EmbeddedResourceCache` | Large downloads and retained tables; trimming does not automatically remove individual embedded resources | Measure raw/compressed artifact; input-independent sprite atlas fetch before file access; postpone resource restructuring until measured | No speculative split of Core datasets |
| Static mutable configuration — risk | Providers, GameInfo, SaveLanguage, import defaults | Cross-session leakage and racing background results | Single active save, startup-only provider registration, session revision tokens, explicitly scoped lists/settings | No service-locator framework or per-field global mutation |

[.NET 10 AES API](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.aes.create?view=net-10.0) and [MD5 API](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.md5.hashdata?view=net-10.0) mark the relevant methods unsupported on browser. The older [crypto compatibility note](https://learn.microsoft.com/en-us/dotnet/core/compatibility/cryptography/5.0/cryptography-apis-not-supported-on-blazor-webassembly) explicitly exempts SHA families and random-number generation; do not label every cryptographic call unsupported. `MemeKey` also uses `BigInteger`, not a native RSA dependency.

Web Crypto is not a drop-in provider: its API is asynchronous, does not expose AES-ECB, and its AES-CBC semantics do not directly implement Core's synchronous no-padding contract. See [SubtleCrypto encryption algorithms](https://developer.mozilla.org/en-US/docs/Web/API/SubtleCrypto/encrypt). Do not block on JS promises or substitute algorithms. Provider selection is a Foundation decision gate: prefer a maintained, license-compatible managed implementation with no native/browser-unsupported dependencies, then verify exact primitives and publish size. No specific dependency is approved by this document.

Searches of Core C# found no `DllImport`/`LibraryImport`, registry use, `Process.Start`, `Reflection.Emit`, or dynamic assembly loading matches. `IPlugin` exists but does not mean Web can load desktop plugins. `CollectionsMarshal` and other managed span helpers are not evidence of native-library dependencies. Absence of matches must be supplemented with publish analysis and runtime tests.

Fallback order: disable an unproven family/feature; supply a tested managed provider; make a narrowly justified shared refactor; measure a worker/AOT option; delay the feature. Do not silently replace this architecture with server processing.

## Architecture

```text
Browser file picker / file drop             Static host
            |                              app assets only
            v                                    |
      Browser file adapter <---------------------+
            | owned bytes
            v
      Save session coordinator ---- capability / validation view models
        | original bytes                        |
        | working SaveFile                      v
        | cloned PKM draft              Razor UI / local sprite assets
        v
      PKHeX.Core
        recognition, data, editing, legality, conversion, serialization
        |
        v
      export snapshot -> reopen/validate -> browser download

PKHeX.WinForms ----------------------> same PKHeX.Core
```

Proposed Web folders: `Components`, `Pages`, `State`, `Services`, `Interop`, and `wwwroot`. These are proposal names, not existing APIs. No separate Domain/Infrastructure project, mediator, repository pattern, database, or HTTP API is warranted for MVP.

### Minimal application interfaces

All names below are **proposed Web-owned contracts**:

- `BrowserFileService`: select/read a single bounded file, receive file-only drop, and download owned output bytes. Return sanitized display name and bytes; never expose a host path assumption.
- `SaveSession`: original byte owner, original name, working `SaveFile`, session ID, revision, selection, draft, pending/last export metadata, and error state. Components request commands rather than mutating the working save directly.
- `EditorDraft`: cloned `PKM`, source slot identity/revision, changed field values, structural validation messages, and last legality result/revision. No reflected universal property editor.
- `SaveCapabilities`: a Web view model derived from actual Core type/interfaces and the release's proven feature allowlist. A generation number alone cannot establish editable fields or supported save operations.
- `LegalityService`: call Core on a clone, format results, and associate result with session/draft revision. A small wrapper, not a new legality engine.
- `SpriteCatalog`: resolve species/form/gender/shiny/context to existing static assets or a text-labeled placeholder. Browser composition owns overlays; shared domain mapping can be extracted separately if justified. Load a fixed complete MVP atlas and manifest before enabling file access; missing variants use an in-memory/text placeholder and never trigger a per-entity request.

File selection is asynchronous; Core work may be synchronous. No `Task.Run` promise of background execution. Start single-threaded; if a gate fails, a separate worker runtime can own a copy of a PKM and receive/return serialized request/results with revision IDs. Do not share mutable `SaveFile` across UI and worker. Worker initialization must register the same providers and Core settings.

Keep browser APIs out of Core. Existing Core APIs stay intact. The only anticipated public Core API change is an optional explicit import-settings overload for slot operations if required to reuse history without default side effects; prove the need and preserve current desktop defaults. Other refactors below are candidates, not permission for broad changes.

## State model

| State / transition | Required behavior |
|---|---|
| Empty → loading | Read one bounded local file; retain previous session until new candidate fully parses and passes checks |
| Candidate parse | Copy original bytes before any parser call. Use memory API inside error boundary. Check exportability, family allowlist, integrity and interpretation |
| Ambiguous interpretation | Ask only among valid Core-supported edition/language choices. Keep decision in session; do not silently guess from filename or mutate original bytes |
| Loaded | One working save and immutable original bytes. No full UI list of cloned PKM for every box; materialize visible slots |
| Select | Clone chosen entity. Empty slots show an empty state; MVP does not create Pokémon from scratch |
| Edit | Change draft only; validate representable ranges/encoding and dependencies. Invalidate legality immediately; show stale status until recomputed |
| Apply | Verify source revision and slot; stage on a working-save clone, use explicit `EntityImportSettings.None`, validate the result, then replace the working save atomically and advance revision |
| Apply side effects | Display form/species-dependent changes before confirmation. No implicit dex/record/handler updates, encounter generation or legalization |
| Cancel draft | Restore from current slot; no working save mutation |
| Selection with draft | Offer Apply, Discard draft, Cancel; if structural validation fails, Apply stays unavailable. Selection stays within the current session |
| Open/close/reset with changes | First resolve draft with Apply/Discard draft/Cancel. Then separately resolve changed working session with Export/Discard session/Cancel. Apply alone never authorizes losing that session |
| Reset original | Confirm; parse a fresh copy of original bytes with the original interpretation choices; replace session only on success; clear draft/history/export indicators |
| Export | Resolve draft first; clone working save, call `Write`, validate/reopen a separate output copy; produce download only on success |
| Export initiated | Record exported revision and intended filename; display “Download started — verify your file.” Do not clear edits-vs-original or claim persistence |
| Refresh/close | No application persistence; register best-effort unload warning only while relevant. Mobile termination may provide no event |

Dirty state is application-owned: `draftDirty`, `hasChangesSinceOpen`, and `changesSinceLastExportAttempt` are separate concepts. Increment revisions for actual committed changes; no-op apply does not create a change. `SaveFile.State.Edited` alone is not reliable for arbitrary property setters. A fresh open/reset is clean; a changed session retains a visible “edited” state even after export. MVP uses a conservative warning while edited. For replacement, close or reset, Export generates a download and then requires explicit “Continue; I have checked my export” acknowledgement before discarding the old session. A failed/canceled export stays in the old session. Discard session is a separate explicitly destructive choice. When opening a replacement, parse/validate the candidate before presenting this final old-session decision; cancellation/failure leaves the old session intact. Browser unload warnings cannot guarantee recovery.

**PK6 party-stat policy:** Core `SaveFile.SetPartyValues` leaves existing party stats intact when `PartyStatsPresent` is true; `PKM.ResetPartyStats()` recalculates them but also restores full HP and clears status. For MVP, non-stat edits preserve stored battle stats, current HP and status. Species/form, level/EXP, nature or IV/EV edits recalculate through Core on the draft clone, set `Stat_Level` consistently, then restore status and current HP as `min(previous HP, new maximum HP)` (a fainted member stays at zero). Preview any HP reduction before Apply; never call `Heal()` or refill PP implicitly. For boxed PK6, calculate previews on a clone and let Core write stored-format data. Test injured, fainted, status-afflicted, level-changing and one-HP species cases. Later families require their own explicit policy rather than inheriting this one blindly.

**Slot write preflight:** Before direct Core setters, check the relevant `ISlotInfo.CanWriteTo(save)` and entity-specific `CanWriteTo(save, draft)` results as well as source coordinates/revision. Box locks and party configuration checks are not guaranteed by direct setters. For MVP edits of occupied party slots, do not change party count; stage all changes on a clone and compare count before commit. Later delete/move flows use Core party compaction rather than writing an empty entity into an arbitrary party index.

Do not promise secure memory erasure: managed/runtime/browser copies may remain until reclaimed, and browser history restoration can retain a document. On a restored back-forward-cache page, clear session state before re-enabling the UI; test this explicitly. The application makes no disk persistence requests for save content. Normal tab suspension is not treated as reload; warn users that the tab is temporary and can be evicted.

MVP supports draft cancellation and whole-session reset, not a generic undo promise. Later slot undo can reuse `SlotChangelog`, but tests must establish exactly which auxiliary state it captures. Non-slot operations need explicit transaction snapshots or their own history. No unlimited full-save history; impose a measured memory bound. Do not expose undo for a mutation it cannot fully reverse.

For future external imports, preview conversion and explicit handler/dex/record settings separately from editing an existing slot. Use destination Core capabilities and document irreversible loss. Cross-save operations must be transactional across both sessions or leave sources untouched until destination success.

## Upstream refactoring opportunities

These are conservative candidates. First reuse existing Core helpers; extract only the non-UI residue with behavior-preserving tests. No wholesale WinForms migration is necessary.

| Current location / responsibility | Why Web needs it | Proposed destination | Risk / separate PR |
|---|---|---|---|
| [PKMEditor.UpdateForm/UpdateSpecies](PKHeX.WinForms/Controls/PKM%20Editor/PKMEditor.cs): linked EXP, ability, gender and special form updates | A naïve property setter leaves inconsistent UI/data or hidden PID changes | Small typed Core editing operation accepting explicit user intent and returning changed-field information; keep control refresh in UI | Medium/high; tests for Gen 2/3 Unown, gender forms, growth/ability behavior; separate PR before corresponding editable field |
| [Main.SanityCheckSAV](PKHeX.WinForms/MainWindow/Main.cs): interpretation prompts and early-generation load checks | Browser memory-load path lacks desktop orchestration | Reuse `SaveLanguage`/`TryOverride` first; move remaining pure detection/choice result only to Core, preserve dialog in WinForms | Medium; do not turn fallback guesses into verified identity; separate PR only for actual missing reusable logic |
| [SlotEditor.Set](PKHeX.Core/Editing/Saves/Slots/SlotEditor.cs): default import settings | Share slot history with surgical editing | Core overload taking explicit `EntityImportSettings`; old overload retains defaults | Low/medium; standalone behavior-preserving PR if needed; not a new slot framework |
| [SAV_PokedexBDSP](PKHeX.WinForms/Subforms/Save%20Editors/Gen8/SAV_PokedexBDSP.cs) and other dex dialogs: bulk choices mapped to concrete structures | Later dex UI needs consistent seen/caught/form/language operations | Existing Core dex structures first; extract tested bulk intent helpers only when current public methods are insufficient | High risk of erasing form/research flags; family-by-family later PRs |
| [Drawing.PokeSprite](PKHeX.Drawing.PokeSprite/PKHeX.Drawing.PokeSprite.csproj): asset choice plus bitmap composition | Browser needs matching sprite variants | Web build asset manifest; move only proven pure key-selection logic to Core or existing neutral abstraction | Medium; licensing and generated artifact provenance required; no cross-platform System.Drawing shim |

Inventory, gifts, trainer fields and game-specific save blocks already have Core structures. Their existence is not justification for adding duplicate service abstractions. WinForms `PreparePKM` mixes control reads with typed mutations: extract individual domain operations only, never a control-shaped “prepare web PKM” API. Retain `IPKMView`, `ISaveFileProvider`, and existing editing abstractions where useful without forcing Razor components to emulate Windows controls.

## UX structure and accessibility

The start screen explains privacy, supported formats, temporary state, and console-save prerequisites. One prominent Open action plus a keyboard-accessible drop area. Unsupported formats get actionable guidance; do not offer a fake repair or upload fallback.

The loaded workspace uses a compact trainer/game overview, party strip, box selector/grid, and contextual editor. Desktop uses split panes; tablet can collapse the summary; mobile uses a list/grid screen then full-width editor with a clear return action. Preserve selection and focus when moving between these views. A persistent export action and textual draft/session state prevent confusing “Apply” with “download save.”

Editor sections: Identity, Stats, Moves, Origin/Trainer, and Advanced. Only supported editable fields appear; important preserved-but-read-only values remain inspectable. Legality is a concise status plus expandable report, with stale/unknown/error states separate from illegal. Avoid a red wall of diagnostics while typing. Warn that legality analysis is not an online acceptance guarantee.

Roadmap screens include Trainer, Pokédex, Gifts, Bag, and Library; do not show dead navigation in MVP. About exposes version/Core revision, licenses, support matrix and local diagnostic export. A diagnostic report is previewed before copying/downloading and excludes save data by default.

Target WCAG 2.2 AA. Use actual buttons/inputs, associated labels, logical headings, visible focus, error descriptions and live status messages. A box grid has one tab entry with arrow-key movement, Enter to edit, named coordinates/species, and an accessible alternative list. All drag/drop operations also have menu/button equivalents. No hover-only data; focus exposes the same summary. Provide text/icons alongside color, adequate contrast, reduced-motion behavior and zoom/reflow. Prefer 44 CSS pixel touch controls; satisfy WCAG's applicable 24 pixel minimum/spacing criterion. Test dialogs for focus containment and return, screen-reader announcements, and 200–400% zoom rather than claiming compliance from an automated score. See [WCAG 2.2](https://www.w3.org/TR/WCAG22/).

## Browser and device support

This is a proposed release policy, not a statement that the unbuilt application works today.

| Target | Release requirement | Special checks |
|---|---|---|
| Chrome / Edge desktop | Current and previous stable major at release | File input/drop/download, CSP, keyboard, published WASM |
| Firefox desktop | Current stable and current ESR | Download naming, focus, no reliance on Chromium filesystem APIs |
| Safari macOS | Current and previous major supported by selected .NET runtime | File reads, Blob download, memory, WebKit behavior |
| iPadOS Safari | Current and previous supported OS major | Files picker, touch layout, download discovery, suspension/eviction |
| Android Chrome | Current stable on physical midrange device | Document providers, low-memory behavior and orientation |
| iPhone Safari | Best-effort MVP usability; promotion requires complete physical-device workflow | Narrow screen, Files/share/download behavior, browser termination |
| Embedded webviews / legacy browsers | Not promised | Explain missing required features before file selection where detectable |

Record exact tested OS/browser versions in each release's support report; intersect policy with current .NET requirements. Playwright WebKit is useful automation but does not replace actual Safari/iOS tests. File System Access and shared-memory threads are not prerequisites. Desktop-first does not permit inaccessible mobile controls. A failed required iPad workflow blocks the tablet claim; publish a reduced support matrix honestly if necessary.

## Privacy and security model

Display: **“Your save is processed entirely on this device and is never uploaded.”** Also explain: the host receives normal requests for app assets and may retain access logs; the app does not send save content or identifiers. Do not say the website makes no network requests.

- Bundle scripts, fonts, sprites and game data locally. Same-origin requests can still disclose derived save data: fetch the complete fixed MVP sprite atlas/manifest before enabling file selection, never species/form/shiny-dependent URLs after opening. Missing artwork uses resident placeholders. Later optional modules must likewise use input-independent preload sets or remain disabled. No analytics, remote image lookups, automatic update checks, URL-based import, telemetry or automatic error reporting. Future analytics requires separate upstream agreement and privacy design.
- Treat files, filenames, PKM names, Showdown text and parser output as untrusted. Razor text rendering only; no `MarkupString`/HTML injection or executable imported markup. Sanitize names independently of OS path behavior; strip separators/control characters and cap length.
- Enforce read limits, one active import, finite previews and exception containment. Reject unsupported/corrupt saves for editing in MVP; do not silently “fix” a checksum. A later read-only recovery mode is separate work.
- Parse and mutate candidate copies; do not replace a valid session after failure. OOM/tab termination cannot be reliably caught; protect with memory budgets and clear temporary-state messaging.
- Drag/drop accepts local files only, prevents default navigation, and rejects directories/URLs/multiple files with an explanation. Global drop interception must not break normal text editing.
- Download bytes as binary, not executable HTML. Never place save content, trainer names or filenames in URLs, route state, network logs or analytics labels. Revoke Blob URLs when no longer needed, allowing enough time for browsers to start the download.
- Dependency review covers transitive packages, exact release versions, licenses, build scripts and asset provenance. No runtime third-party CDN. Keep deployment credentials out of client assets; a static client cannot keep secrets.
- CSP is defense in depth, not proof of no exfiltration: `connect-src 'self'` still permits same-origin requests. Test actual network behavior and keep the static host free of application upload endpoints.

Candidate production CSP: `default-src 'self'; script-src 'self' 'wasm-unsafe-eval'; style-src 'self'; img-src 'self'; font-src 'self'; connect-src 'self'; object-src 'none'; base-uri 'self'; form-action 'none'; frame-ancestors 'none'`. Validate against the exact published SDK output; add narrow hashes or specific sources only for demonstrated needs. Do not broadly add `unsafe-eval`/`unsafe-inline` to quiet errors. Set `X-Content-Type-Options: nosniff` and a restrictive referrer policy where hosting allows. Meta CSP is a fallback on hosts without custom headers, but cannot enforce `frame-ancestors`. See [Blazor CSP guidance](https://learn.microsoft.com/en-us/aspnet/core/blazor/security/content-security-policy?view=aspnetcore-10.0).

The browser sandbox does not make hostile files harmless: CPU exhaustion, memory exhaustion and parser defects still matter. A compromised distribution could read files the user selects, so reproducible release provenance and self-hosting instructions are part of the trust model. Do not promise protection against malicious extensions, compromised devices, or secure erasure after close.

## Static hosting strategy

Publish the standalone Web project in Release and deploy its `publish/wwwroot` content, not source or a development server. Runtime hosting needs no .NET installation. Use one root page with in-app tab state in MVP; avoid deep routes for individual saves or Pokémon. Configure `<base href="/">` or the deployment subpath with trailing slash and use base-relative asset URLs. No save-derived route/query/fragment state.

| Host | Deployment approach | Required verification |
|---|---|---|
| Cloudflare Pages | Upload prebuilt static output using CI/Direct Upload; no Pages Functions or Workers required | Free-plan 20,000-file limit and 25 MiB per asset; inspect every published asset, especially AOT output and sprite count. Native builds have 20-minute timeout and 500/month free allowance; prebuild avoids SDK-image assumptions |
| GitHub Pages | Artifact deployment after successful build, subpath base, `.nojekyll` when applicable to chosen publishing route | Published site ≤1 GB; official soft bandwidth 100 GB/month. MVP root navigation avoids reliance on rewrite support; deep links later need a tested routing solution |
| nginx / Apache / ordinary static hosting | Copy immutable release directory and switch served release atomically | Correct MIME, base path, CSP/headers, compression and cache rules; no custom executable service |
| Netlify or equivalent | Deploy same static artifact; host-specific headers/redirects only | Check current plan limits before selecting it; no assumption of identical quotas or routing behavior |

Cloudflare's [limits](https://developers.cloudflare.com/pages/platform/limits/) apply to site assets, not user save sizes. Browser-only saves never traverse a hosting upload API. Check the actual files submitted to the provider; compression is not a promise of bypassing a per-file limit. If a build exceeds limits, reduce/split static assets where supported or select another static host; do not introduce an object-storage backend by default. Dashboard drag/drop has a lower file-count limit than Wrangler according to [Direct Upload](https://developers.cloudflare.com/pages/get-started/direct-upload/).

Cloudflare [serving behavior](https://developers.cloudflare.com/pages/configuration/serving-pages/) treats a site without a top-level `404.html` as an SPA. Test missing asset responses so HTML fallbacks are not mistaken for WASM. GitHub Pages does not provide an equivalent arbitrary rewrite configuration; keeping MVP on one root route avoids brittle 404 hacks. For future routes, configure fallback only for navigation and preserve actual missing-asset errors. [GitHub Pages limits](https://docs.github.com/en/pages/getting-started-with-github-pages/github-pages-limits) must be rechecked before release.

Serve `.wasm` as `application/wasm`, JavaScript/CSS/JSON with their correct types, and binary data with an appropriate binary type. .NET may package assemblies as Webcil/WASM; inspect actual output rather than assuming every assembly is a `.dll`. If serving precompressed `.br`/`.gz`, set matching `Content-Encoding`, original content type and `Vary: Accept-Encoding`; otherwise let the host negotiate compression. Never merely rename compressed bytes. Test response headers and boot integrity from the deployed artifact. See [standalone hosting guidance](https://learn.microsoft.com/en-us/aspnet/core/blazor/host-and-deploy/webassembly/?view=aspnetcore-10.0).

Use content-hashed immutable assets with long cache lifetimes; revalidate entry HTML, boot metadata and non-fingerprinted resources. Release label includes Web version and Core commit. Retain complete prior release artifacts for rollback; do not mix assets between releases. Existing sessions keep their loaded runtime; show update information on the next open/reload rather than forcing reload during editing. Self-hosting documentation must include both `/` and `/PKHeX/` examples and local HTTP serving, not `file://` launching.

No service worker in MVP. Later PWA work must version its asset cache, avoid caching save bytes/Blob outputs, and activate an update only after an explicit safe reload. Test multiple tabs and old cached clients. Do not call `skipWaiting` and force-refresh a dirty session. Offline cold launch is a later guarantee, not implied by ordinary HTTP caching. See [PWA lifecycle guidance](https://learn.microsoft.com/aspnet/core/blazor/progressive-web-app).

## Performance

Inspected resource directories occupy approximately **16 MiB Core** and **29 MiB PokeSprite** on this filesystem (`du -sh`); these are disk observations, not published or transferred bundle sizes. Measure the actual Release artifact, compression, runtime, tables and sprites separately. Do not promise a tiny bundle based on the absence of NuGet references.

Render one visible box and party; avoid prebuilding full-save component trees or legality results. Fetch a fixed sprite atlas/manifest before enabling file access, cache small summaries by save revision, and invalidate changed slots. Lazily render visible sprite regions without any selection-dependent network fetch. Legality runs on demand and after a 300 ms idle debounce only if measurements show acceptable responsiveness. Mark a result stale immediately on edit; discard results for superseded revisions. Never label an unexamined slot legal.

Initial engineering targets, **not measured claims**: cold usable shell ≤5 s on a named reference desktop with a 20 Mbps/50 ms test profile; cached shell ≤2 s; ordinary box changes ≤100 ms after assets are warm; selected-entity analysis p95 ≤500 ms for the admitted fixture corpus. Record device/OS/browser and corpus. If synchronous operations cause repeated >200 ms UI stalls, either gate the affected feature or implement/test worker isolation before claiming responsive support. No automatic whole-save analysis in MVP. The privacy-preserving startup atlas is a deliberate upfront cost: measure it within the startup budget, reduce/optimize the fixed asset set or use placeholders if needed, and never regain speed by leaking selected species through lazy network requests.

Track original bytes, working save, temporary mutation/export snapshots, entity copies, JS/managed transfer buffers, and output Blob concurrently. Release superseded buffers and listeners. Choose an input cap and family allowlist based on peak measurements on the reference iPad and Android device, not only desktop. The 16 MiB read cap is a safety ceiling, not an allocation budget. If a device cannot complete the maximum admitted workflow, narrow the documented device/family combination.

Do not split Core into lazy-loaded assemblies speculatively. Lazy loading is most useful for later optional UI/assets; Core and its embedded legality data are baseline dependencies. AOT, additional worker runtime memory, further atlas optimizations and resource packaging are measured follow-ups, each with download/CPU/memory tradeoffs.

## Compatibility matrix

Legend: **C** = capability exists in inspected Core; **I** = Web implementation/fixtures required; **B** = identified default browser blocker on the named path; **P** = proven on published Web. **No row has P today.** A Core class is not a promise of complete Web feature support. All admitted families need load, view, permitted edits, legality and export/reload tests; gift/dex/inventory interfaces vary independently.

| Generation / family | Core evidence | Relevant capabilities / caveats | Web status and intended stage |
|---|---|---|---|
| 1: Red/Blue/Yellow | `SAV1` | Party/boxes, early DV/stat-experience and language/edition differences | C/I; MVP+ after Gen 6 vertical slice; no known mandatory AES/MD5 path identified |
| 2: Gold/Silver/Crystal | `SAV2` | Party/boxes, DVs, Unown/shiny relationships, RTC | C/I; MVP+; special fields require different editor semantics |
| 3: Ruby/Sapphire/Emerald/FRLG | `SAV3RS`, `SAV3E`, `SAV3FRLG` | Party/boxes, PID-correlated attributes, ambiguous edition/personal tables | C/I; MVP+ after ambiguity and correlation gates |
| 4: Diamond/Pearl/Platinum/HGSS | `SAV4DP`, `SAV4Pt`, `SAV4HGSS` | Party/boxes, native save structures and wrapped `.dsv` handling | C/I; MVP+; container preservation is separately gated |
| 5: BW/B2W2 | `SAV5BW`, `SAV5B2W2` | Party/boxes, format-specific origin and gift structures | C/I; MVP+ |
| 6: XY/ORAS | `SAV6XY`, `SAV6AO` | Party/boxes, PK6, legality, direct raw-save export | C/I; **MVP release target**, raw decrypted saves only |
| 6: ORAS demo | `SAV6AODemo` | Different scope and capacities | C/I; Post-MVP, not implicitly included with ORAS |
| 7: SM/USUM | `SAV7SM`, `SAV7USUM` | Signature normalization/export invokes MemeCrypto AES | C/I/B for signed export; MVP+ only after AES provider proof |
| 7: Let's Go | `SAV7b` | PB7 and nontraditional storage/party model, awakening values | C/I; MVP+ separate adapter/UX gate; do not inherit the SM/USUM AES claim |
| 8: Sword/Shield | `SAV8SWSH` | PK8, block saves, Dynamax/Gigantamax | C/I; MVP+; SwishCrypto/SHA path must be exercised, not confused with AES |
| 8: BDSP | `SAV8BS` | PB8; MD5 checksum checks/writes | C/I/B; MVP+ only after MD5 provider proof |
| 8: Legends Arceus | `SAV8LA` | PA8, research dex, Alpha and move-shop data | C/I; MVP+ for basic editing, research editors Post-MVP |
| 9: Scarlet/Violet | `SAV9SV` | PK9, revision-dependent blocks, Tera metadata | C/I; MVP+; revision corpus required |
| 9a: Legends Z-A | `SAV9ZA` | Separate context; optional expensive encounter seed recovery | C/I; Post-MVP pending performance and field/correlation coverage |
| Stadium / Stadium 2 | `SAV1Stadium`, `SAV1StadiumJ`, `SAV2Stadium` | Non-mainline layouts and storage rules | C/I; Post-MVP; no generic six-slot party assumption |
| Colosseum / XD / Pokémon Box | `SAV3Colosseum`, `SAV3XD`, `SAV3RSBox` | Different entity/storage formats and containers | C/I; Post-MVP; test actual crypto paths rather than assume all use AES |
| Battle Revolution / Ranch | `SAV4BR`, `SAV4Ranch` | Special storage and export behavior | C/I; Post-MVP |
| Bulk storage / whole-card containers | `Bank3`, `Bank4`, `Bank7` in Core `Saves/Storage`; `SAV3GCMemoryCard` separately | Existing bulk save formats are not the proposed IndexedDB library; whole-card editing requires preserving unrelated content. `SAV_BEEF` is a Gen 6/7 checksum base, not a bank | C/I; Long-term; excluded from MVP |
| Standalone HOME entities | `PKH`, `HomeCrypto` | AES-CBC outside ordinary `.pk6` editing | C/I/B; Long-term; does not authorize fabricating HOME trackers |
| ROM hacks / unknown future games | No generic support claim | PKForge custom parsers are not authority here | Excluded; no guessed support |

Detailed family acceptance lives in fixtures and a release capability manifest. Within an admitted family, hide/disable unsupported operations based on actual interfaces and data validity. Never force a save into the nearest generation. Broader Core recognition can produce “recognized by Core, not enabled in this Web release” instead of silently opening an untested editor.

## Definition of MVP

MVP is a **complete raw XY/ORAS save editing/export loop**, not a viewer marketed as supporting every Core format. XY/ORAS provides modern, common PKM fields without the known Gen 7 signing or BDSP MD5 paths, allowing an initial browser proof before expanding families. This is a scope recommendation, not a compatibility result. If its browser gates fail, resolve them before release; do not quietly substitute a backend.

Must include static load, local picker/drop, explicit privacy/temporary-state copy, recognized-family checks, original-byte preservation, trainer/save summary, party and boxes, selection, draft editing, contextual legality, atomic apply, and validated save download. Display trainer name, game, language, correctly formatted IDs, playtime and money when provided; trainer edits and progress/badges are later.

**Editable PK6 fields in MVP:** species/form (through verified shared dependent-field behavior), nickname and nickname flag, language, gender where allowed, level/EXP, friendship, nature, ability/slot, held item, four moves with PP/PP Ups, IVs and EVs. Show calculated stats/characteristic. Preserve all other bytes/metadata except documented Core-required normalization. Origin, trainer identity, PID/EC, shiny state/type, met/egg data, ribbons/marks/memories and generation-specific fields are inspectable where practical but not editable in MVP. Do not generate a replacement encounter or PID to make a legality result green.

Core representational limits and encoding are hard validation; legality findings are warnings with explicit acknowledgement before applying/exporting affected edits, not a universal prohibition on saving an illegal Pokémon. A legality exception or uncompleted analysis is “unavailable,” never “legal.” Existing illegal entities can still be inspected and exported; indicate limitations without unsolicited modifications.

No Pokémon creation in empty slots, external PKM import, party reordering, destructive slot operations, cross-save transfer, dex/trainer/bag/gift editing, undo history, bank, or PWA in MVP. A useful edit/apply/export workflow takes priority over these additions. Must stories outside MVP are prerequisites for their named later milestone, not a demand to ship them on day one.

## Testing strategy

Use existing xUnit Core tests for domain regressions. Add small Web application unit tests for session transactions, explicit import settings, field validation, capability mapping and naming. Do not test mere forwarding methods. Share synthetic/non-sensitive fixtures only with documented provenance; do not commit users' saves or trainer data without permission. Legality fixtures need known expected outcomes at the pinned Core revision.

| Test layer | Cases and pass criteria |
|---|---|
| Provider gate | AES ECB/CBC encryption/decryption, no-padding, overlap/in-place, IV/session reuse, MD5 vectors; compare desktop reference bytes and browser output; SHA-dependent saves tested separately |
| Load/session | Empty, oversized, truncated, checksum-corrupt, wrong-family, misleading extension/name, parser exception and canceled picker. Old session and original bytes unchanged on failure |
| Interpretation | Gen 1–3 language/edition ambiguity; cancellation; FRLG personal table; no silent filename override; matrix promotion only after coverage |
| Draft/apply | Each editable field, min/max/invalid encoding, species/form dependencies, no-op, cancel, stale selection, apply failure, preserved unknown fields; explicit no handler/dex/record side effects |
| Slot operations later | Party compaction/count, eggs/last-member restrictions, locked slots, move/copy/swap/clear semantics, rollback after second-write failure, undo/redo scope |
| Round trip | Load → no-op export → reopen and verify checksum/container/semantic invariants; edit one field → export → reopen and verify intended field and unaffected ranges |
| Byte regression | Compare browser output with the same Core revision/native reference for deterministic fixtures. Record expected normalized/checksum/signature regions; do not require input equality for every format |
| Metadata preservation | Raw files, then separately admitted `.dsv`/`.gci`/other wrappers; headers, footers, handler finalization, filename conventions; no exporting only `SaveFile.Data` |
| Legality | Known valid/invalid entities; stale/unknown/error handling; format and slot context; refresh after apply; no automatic legalization; localized reports survive trimming |
| Browser E2E | Playwright Chromium/Firefox/WebKit against **published Release assets** on a static server: open fixture, select, edit, apply, download, reopen downloaded bytes |
| Privacy/security | Intercept requests after opening; allow static asset fetches only, no user-derived URL/body/header data. Check storage remains empty of saves; hostile names render as text; no URL drops/navigation |
| Accessibility | Keyboard-only full journey, screen-reader grid/editor/dialog, contrast, zoom, reduced motion, touch targets; automated checks plus manual NVDA/VoiceOver where available |
| Deployment | Root and `/PKHeX/`, MIME/encoding/CSP, offline after boot vs unsupported offline cold start, stale cache/mixed-release failure, missing assets; verify no backend dependency |
| Device/performance | Physical Safari/iPad/Android lifecycle and downloads; repeated open/edit/export/close memory trend; reference corpus latency and large accepted file peak usage |

For corrupt save tests, a deliberately invalid signature/checksum is distinct from an unrecognized format and from an unsupported Web family. Release claims name all tested families/revisions/browsers and known limitations. No new family is enabled because the same UI compiled for it.

## CI/CD

Propose new workflows because none are checked in at the baseline. A Linux job restores/builds/tests Core and Web by explicit project paths, avoiding Windows-only solution builds. A Windows regression job builds the existing desktop solution/configuration as appropriate. Pin an approved .NET 10 SDK patch in the Web workflow initially; a repository-wide `global.json` is a separate maintainer decision, not required churn.

Sequence: restore → Core tests → Web application tests → Release publish → review platform/trimming warnings → static artifact checks → browser E2E → upload build/test/size artifacts. Once clean, fail on new unexplained warnings rather than blanket suppression. Install `wasm-tools` for workloads that need it (AOT/runtime relinking); record workload/SDK versions. Interpreter publish is the primary gate; an optional AOT comparison job is not a release prerequisite.

Browser tests run on a static file server with deployment-like headers, never only the dev host. Artifacts include version/Core revision, dependency/license inventory, compressed/uncompressed sizes, family matrix, test reports and static output. Generated publish output, caches, downloaded runtime/workloads and temporary user fixtures stay out of git. Generated sprite manifests are reproducible build products; source asset provenance is tracked.

PR builds have read-only permissions and no deployment credentials; do not run untrusted PR code with secrets via `pull_request_target`. Pin third-party actions to reviewed revisions. Optional deployment is a separate trusted branch/tag or manual job with maintainer-owned environment approval and artifact promotion. Do not enable a public upstream domain or deploy automatically just because the Web skeleton lands. Release rollback redeploys an entire known-good static artifact; no database migration exists in MVP.

## Git and upstream contribution strategy

The eventual target is `PKHeX.Web/` alongside the existing projects, not a permanently diverging engine fork. Use a working fork with `origin` for contributor branches and `upstream` for maintainer code. Branches use focused `web/*` names unless maintainers request another convention. Inspect the actual upstream default branch rather than assuming `main` or `master`.

Before substantial upstream UI work, seek maintainer interest through the channels in CONTRIBUTING and present the static/client-side goal, minimal vertical slice, asset/crypto implications, and expected maintenance ownership. This document authorizes no outreach. Acceptance is uncertain; keep useful standalone Core fixes reviewable even if Web itself is declined.

Rebase small topic branches onto upstream before submission and after relevant Core changes; rerun affected tests. Do not rewrite others' shared work. Separate behavior-preserving refactors from new UI, dependency additions and formatting. Keep commits focused with rationale, before/after behavior, tests and migration impact. No mass formatting, solution churn, copied Core tree, generated publish output, or bundled developer artifacts. Respect the upstream preference for ready, passing PRs rather than a long-lived draft PR series.

### Proposed contribution sequence

| Step | Deliverable | Dependencies / review boundary |
|---|---|---|
| 1 | Maintainer design alignment; local feasibility report with crypto, trimmed loading, legality and export proof | No public support claims; no implementation in this documentation task |
| 2 | Minimal shared fixes/refactors demonstrated by the proof, each with desktop regression tests | Existing provider interfaces reused; import-settings overload only if needed; form-dependent behavior extraction separate from UI |
| 3 | Standalone Web skeleton, project/solution integration, local-only file adapter, and publish/test CI | One reviewable foundation PR or paired small CI PR; no backend or desktop graphics reference |
| 4 | XY/ORAS complete vertical slice: load, summary, party/boxes, inspect, no-op export/reopen | Establish safe round trip before a large editor; privacy and error gates included |
| 5 | Draft editor, selected legality, explicit surgical apply and changed-save export | Field groups can be sequential small PRs; finish acceptance tests before claiming MVP |
| 6 | Accessible responsive polish, physical-browser qualification, static hosting/release documentation | May develop alongside earlier work; all accessibility/privacy requirements gate MVP |
| 7 | Additional save families and PKM import/export, one capability/fixture gate at a time | AES/MD5 adapters before affected families; no all-generation checkbox |
| 8 | Trainer/dex/gifts/bag and other independent roadmap editors | Separate upstream interest and per-family tests; library/PWA/AutoMod remain separate decisions |

Every PR explains why any Core change belongs there, shows desktop behavior preserved, and names untested cases. Retain local experiments outside tracked source unless promoted into reviewed tests. This document task makes **no commits or PRs**.

## Post-MVP roadmap

MVP+ expands tested retail families, portable PKM import/export, transactional party/box organization, and basic trainer editing. Older generation fields must use their own semantics, including DVs/stat experience, PID correlations and absent abilities. Modern contexts require their own capability maps; “Gen 8” is not one format.

Post-MVP brings family-specific Pokédex, Wonder Card/gift album, bag, richer trainer/progress, origin/ribbons/memories and game metadata, Showdown text tools, encounter lookup, batch operations, and selected save-specific editors. Derive high-value categories from current WinForms: daycare, RTC/berries, roaming/event progress, trainer records, unlockables, Hall of Fame, and research/dex structures. Raw block/flag editors are expert features and need separate risk/UX review.

A later browser-local library uses IndexedDB only after opt-in. Store original entity bytes, format and provenance alongside searchable derived metadata; version schema and preserve backup portability. Browser storage is evictable and origin-bound, never a backup guarantee. Living-dex views describe the user's collection rather than automatically generating Pokémon. Cross-save transfers show conversion changes and avoid deleting a source before destination success.

PWA/offline install, advanced mobile UX and heavier worker-based analysis are separate milestones. Auto Legality requires independent dependency/licensing/performance/maintenance review and upstream agreement; not every desirable PKForge feature belongs upstream. No automatic cloud/social services or unsupported backward conversions.

## User-story backlog

Each story below states a user outcome, concrete acceptance criteria (AC), priority, dependencies and milestone. **Must** means required for the named milestone; **Should/Could** are deferrable; **Won't-for-now** explicitly excludes baseline delivery. Dependencies are prerequisites, not instructions to ship all related epics together. All writes inherit the session/transaction rules above; all external inputs inherit privacy/error constraints. “Supported” always means the release allowlist, not merely a Core class.

### Application shell

| ID / title | User story | Acceptance criteria | Priority / milestone | Dependencies / notes |
|---|---|---|---|---|
| WEB-APP-001 Static boot | As a user, I want the editor to start from a static URL so I need no application server. | Published Release output boots at root and subpath; shell has Open and About; no server circuit or API call; startup failure offers retry without file selection. | Must / Foundation | WEB-HOST-001 |
| WEB-APP-002 Version and licensing | As a reporter, I want exact version and license information so I can identify my build. | About shows Web version, Core revision, license/asset notices, tested-family matrix and build provenance; these work without a remote lookup. | Must / MVP | WEB-APP-001; WEB-SEC-004 |
| WEB-APP-003 Responsive shell | As a tablet user, I want usable navigation without shrinking desktop dialogs. | Split view on desktop, collapsible tablet panes and stacked mobile editor; no loss of selection or draft during resize; export remains reachable at zoom. | Must / MVP | WEB-APP-001; WEB-A11Y-003 |
| WEB-APP-004 Capability/error shell | As a user, I want clear startup failures rather than a blank page. | Detect missing required browser capabilities where possible; distinguish failed asset load from unsupported browser; fatal component errors retain an unaffected session when possible and offer safe reset. | Must / MVP | WEB-APP-001; WEB-ERR-003 |
| WEB-APP-005 Offline installation | As a returning user, I want optional offline startup. | Explicit install/offline status; cache app assets only; safe update prompt; no dirty-session forced reload; cold offline and multi-tab update tests pass. | Could / Post-MVP | WEB-HOST-004; no save persistence |

### Save file loading

| ID / title | User story | Acceptance criteria | Priority / milestone | Dependencies / notes |
|---|---|---|---|---|
| WEB-SAVE-001 Choose local save | As a user, I want to open one local save without uploading it. | File picker accepts extensionless saves; bounded read with 16 MiB ceiling; cancel preserves current session; read failure leaves state intact; network trace contains no input-derived data. | Must / MVP | WEB-APP-001; WEB-SEC-001 |
| WEB-SAVE-002 File drop | As a pointer user, I want to drop a save into the app. | Same validation pipeline as picker; prevent browser navigation; reject multiple files, directories and URL/text drops; picker remains keyboard alternative. | Must / MVP | WEB-SAVE-001 |
| WEB-SAVE-003 Detect and admit format | As a user, I want accurate format/support feedback. | Call Core memory recognition on a copy; distinguish recognized-but-not-enabled family from unknown data; admit only tested raw XY/ORAS in MVP; never trust extension alone. | Must / MVP | WEB-SAVE-001; WEB-TEST-002 |
| WEB-SAVE-004 Malformed/unsupported input | As a user, I want an explanation when a file cannot be edited. | Distinguish unknown format, invalid integrity, oversized input, encrypted-console prerequisites and parser failure; no silent repair or overwrite; previous session remains usable. | Must / MVP | WEB-SAVE-003; WEB-ERR-001 |
| WEB-SAVE-005 Preserve source | As a user, I want my original file protected from normalization. | Own an untouched byte copy before parsing; Gen 7 regression later proves source signature bytes unchanged; “download original” returns those exact bytes; reset uses a new copy. | Must / Foundation | None; source-copy contract is tested using fixture bytes before picker UI |
| WEB-SAVE-006 Resolve ambiguous identity | As an early-generation user, I want to choose edition/language when the save cannot prove them. | Offer valid Core choices and show inferred versus selected identity; cancellation leaves previous session; apply correct personal table; reset repeats stored choice; filename is a hint only. | Must / MVP+ | WEB-SAVE-003; WEB-CROSS-004 |
| WEB-SAVE-007 Metadata/privacy/recents | As a privacy-conscious user, I want to know what opening a file retains. | Display sanitized filename, byte size, game/context and local-processing notice; no recent-file database or persistent handles; reload shows empty state. | Must / MVP | WEB-SAVE-003; WEB-SEC-002 |
| WEB-SAVE-008 Wrapped save admission | As an emulator user, I want supported wrapper bytes preserved. | Each enabled `.dsv`/`.gci` or other wrapper has dedicated detection/export/reload fixture; metadata headers/footers survive; unsupported whole cards/archive files receive guidance. | Should / MVP+ | WEB-EXP-004; family-specific gate |

### Save session

| ID / title | User story | Acceptance criteria | Priority / milestone | Dependencies / notes |
|---|---|---|---|---|
| WEB-SESSION-001 Isolated working session | As a user, I want edits isolated from my source file. | One active working save per tab; owned source bytes and candidate copies; failed open does not replace active save; session IDs prevent old callbacks changing a replacement session. | Must / Foundation | WEB-SAVE-005 |
| WEB-SESSION-002 Draft/apply/discard | As an editor, I want to review changes before applying them. | Selection creates cloned draft; Apply stages on save clone with explicit import settings; invalid/stale draft blocked; Cancel discards draft; no dex/records/handler side effects; success refreshes slot and revision. | Must / MVP | WEB-SESSION-001; WEB-PKM-001 |
| WEB-SESSION-003 Honest dirty status | As a user, I want to distinguish applied edits from downloads. | Separate draft, changed-since-open and exported-revision indicators; no-op apply stays clean; download initiation never says saved-to-disk; later edits invalidate export indicator. | Must / MVP | WEB-SESSION-002; WEB-EXP-002 |
| WEB-SESSION-004 Reset/discard session | As a user, I want to restore the opened save. | Confirm destructive reset; reopen original copy with chosen interpretation; clear draft/history and export indicators; reset failure does not silently discard valid working state. | Must / MVP | WEB-SESSION-001 |
| WEB-SESSION-005 Navigation lifecycle | As a user, I want warnings before losing temporary work. | Selection resolves draft; replace/close/reset then separately offers Export/Discard session/Cancel and requires explicit continue after a download; best-effort unload warning while edited; no auto-save; restored back-forward-cache pages clear session before interaction; document mobile termination limits. | Must / MVP | WEB-SESSION-003; WEB-BROWSER-003 |
| WEB-SESSION-006 Bounded undo/redo | As an organizer, I want reversible slot changes. | Verify Core history covers every affected region; undo/redo restores source and destination/party state; new change clears redo; bounded history; unsupported non-slot undo is not advertised. | Should / MVP+ | WEB-BOX-005; WEB-TEST-003 |
| WEB-SESSION-007 Naming policy | As a user, I want exports I can recognize and restore correctly. | Retain original display name separately; strip path/control characters; preserve required extension or extensionless `main`; offer original and edited-name suggestions; explain when console restoration requires renaming. | Must / MVP | WEB-SAVE-007 |

### Trainer/save overview

| ID / title | User story | Acceptance criteria | Priority / milestone | Dependencies / notes |
|---|---|---|---|---|
| WEB-OVERVIEW-001 Identity summary | As a user, I want to confirm which save I opened. | Show trainer, game/context, language, appropriate displayed IDs, raw IDs where meaningful, and revision/format; unknown fields labeled unknown, not invented. | Must / MVP | WEB-SAVE-003 |
| WEB-OVERVIEW-002 Time/currency/metadata | As a user, I want a useful read-only save summary. | Show Core-provided playtime, money, file size and integrity/exportability; unsupported values omitted; formatting preserves full values and units. | Must / MVP | WEB-OVERVIEW-001 |
| WEB-OVERVIEW-003 Progress summary | As a returning player, I want badges/progress context. | Game-specific capability mapping, fixtures for flags/counts, read-only summary; no universal eight-badge assumption or interpretation of unknown flags. | Should / Post-MVP | WEB-GAME-001 |

### Party

| ID / title | User story | Acceptance criteria | Priority / milestone | Dependencies / notes |
|---|---|---|---|---|
| WEB-PARTY-001 Display/select | As a user, I want to inspect my party and empty positions. | Use actual party capability/count; labeled occupied and empty slots; select clones the correct entity; empty slot never opens stale entity; keyboard and touch work. | Must / MVP | WEB-SESSION-001; WEB-BOX-004 |
| WEB-PARTY-002 Reorder | As a player, I want to reorder members safely. | Button/menu operation plus optional drag; Core restrictions and party compaction honored; atomic commit; blocked move changes nothing; order survives export/reload. | Should / MVP+ | WEB-SESSION-002; WEB-TEST-003 |
| WEB-PARTY-003 Move/replace | As an organizer, I want to move members between party and boxes. | Destination preview, overwrite confirmation, slot restrictions and valid party-count rules; move is distinct from copy; failed destination write preserves both sides. | Should / MVP+ | WEB-BOX-005; WEB-PARTY-002 |
| WEB-PARTY-004 Delete | As an organizer, I want to remove a party member deliberately. | Confirmation names target; enforce Core/game last-member/egg constraints; compact party correctly; undo restores complete operation; no unrelated data change. | Should / MVP+ | WEB-PARTY-002; WEB-SESSION-006 |

### Boxes

| ID / title | User story | Acceptance criteria | Priority / milestone | Dependencies / notes |
|---|---|---|---|---|
| WEB-BOX-001 Grid/navigation | As a user, I want to browse boxes without loading the entire collection UI. | Actual Core box/slot counts; previous/next and selector; empty slots distinct; selection coordinates stable; only visible box rendered; no hardcoded 30-slot rule. | Must / MVP | WEB-SESSION-001 |
| WEB-BOX-002 Names | As an organizer, I want readable box names. | Display existing name when supported, otherwise numbered label; later rename validates encoding/length and commits explicitly; name persists through export/reload. | Should / MVP+ | WEB-BOX-001; read-only label included in MVP |
| WEB-BOX-003 Wallpapers | As a user, I want game-appropriate box appearance. | Use capability-gated local assets and text fallback; color contrast preserved; wallpaper edit only where supported; unavailable art never blocks editing. | Could / Post-MVP | WEB-BOX-002; WEB-SEC-004 |
| WEB-BOX-004 Sprites and summaries | As a user, I want recognizable entities without relying on images alone. | Correct species/form/gender/shiny mapping where asset exists; placeholder with name otherwise; hover and focus expose same summary; decorative icons have proper accessibility treatment. | Must / MVP | WEB-PERF-003; WEB-A11Y-001 |
| WEB-BOX-005 Move/swap/copy | As an organizer, I want explicit slot operations. | Actions distinctly labeled; destination choice/overwrite preview; atomic source/destination changes with Core slot rules; explicit surgical settings; error rollback; no silent source deletion on copy. | Should / MVP+ | WEB-SESSION-002; WEB-TEST-003 |
| WEB-BOX-006 Drag operations | As a pointer user, I want convenient slot movement. | Drag uses same command as menus; distinguish internal slot drag from external file; visible destination/cancel; every action possible without dragging. | Could / MVP+ | WEB-BOX-005; WEB-A11Y-002 |
| WEB-BOX-007 Clear and multiselect | As an organizer, I want efficient deliberate clearing. | Single-slot clear confirms identity; bulk selection explicitly shows count/targets; one atomic action with undo; locked slots rejected before mutation; no hidden selection across boxes. | Should / Post-MVP | WEB-SESSION-006; WEB-BOX-005 |
| WEB-BOX-008 Box rendering budget | As a large-save user, I want responsive browsing. | Warm visible-box changes meet measured target; sprite requests bounded; no whole-save legality pass on navigation; selection/focus survives summary refresh. | Must / MVP | WEB-BOX-001; WEB-PERF-002 |

### Pokémon editor

| ID / title | User story | Acceptance criteria | Priority / milestone | Dependencies / notes |
|---|---|---|---|---|
| WEB-PKM-001 Capability-driven editor | As an editor, I want only applicable controls and preservation of hidden data. | Draft cloned from slot; fields depend on concrete Core format/interfaces plus release allowlist; unsupported fields preserved; all edits validate before Apply; no reflected generic property grid. | Must / MVP | WEB-SESSION-001 |
| WEB-PKM-002 Species/form | As an editor, I want species/form edits with understandable consequences. | Use Core lists/personal table and shared dependency behavior; preview changes to gender/ability/EXP/stats; no blind PID reroll; invalid/battle-only selections explained; export/reload matches draft. | Must / MVP | WEB-PKM-001; shared-refactor gate |
| WEB-PKM-003 Nickname/language | As an editor, I want correctly encoded names. | Nickname flag and name explicit; Core format length/encoding respected; language changes preview default-name implications; no silent truncation; untouched encoded data remains preserved. | Must / MVP | WEB-PKM-001 |
| WEB-PKM-004 Gender | As an editor, I want gender options that reflect species and format. | Genderless/fixed-gender rules shown; contextual choices; PID-derived formats require later correlated-edit support; changes invalidate legality and update preview. | Must / MVP | WEB-PKM-002; MVP PK6 only |
| WEB-PKM-005 Level/EXP | As an editor, I want consistent level and experience. | Core growth curve controls conversion; synchronized level/EXP; valid stored ranges; form/species growth changes visible; stat preview updates; no incidental save mutation. | Must / MVP | WEB-PKM-002 |
| WEB-PKM-006 Friendship | As an editor, I want to edit the intended friendship value. | Label which current/OT/handler value is affected; range validation by capability; no automatic ownership change; apply affects only intended fields. | Must / MVP | WEB-PKM-001 |
| WEB-PKM-007 Nature | As an editor, I want nature editing with accurate stats. | Core nature choices and stat preview; PK6 independent nature edit; older PID-correlated formats gated; later minted Pokémon preserve separate stat nature unless explicitly edited. | Must / MVP | WEB-PKM-001 |
| WEB-PKM-008 Stat nature/mints | As a modern-game user, I want distinct original and stat nature. | Only supported formats expose stat nature; indicate mint effect; editing original nature does not silently remove intentional mint; export/reload preserves both. | Should / MVP+ | WEB-PKM-007; WEB-CROSS-004 |
| WEB-PKM-009 Ability/slot | As an editor, I want the ability and ability slot to remain consistent. | Choices from form personal data; slot representation mapped correctly; hidden ability availability/legality explained; species/form edits revalidate; no hand-maintained ability table. | Must / MVP | WEB-PKM-002 |
| WEB-PKM-010 Held item | As an editor, I want game-appropriate item choices. | Core-filtered item list, explicit none, stored ID validation; unavailable-item legality warning distinct from parser failure; preserve item if unchanged. | Must / MVP | WEB-PKM-001 |
| WEB-PKM-011 Moves | As an editor, I want to change four moves with clear diagnostics. | Core move IDs/names; empty move supported; no claim that a selectable move is learnable; stale legality immediately marked; order and unaffected slots preserved. | Must / MVP | WEB-PKM-001; WEB-LEGAL-002 |
| WEB-PKM-012 PP/PP Ups | As an editor, I want move PP and PP Ups to agree. | Core move PP rules and representable ranges; move change displays required PP effects; unsupported PP Ups rejected; no silent unrelated move reset. | Must / MVP | WEB-PKM-011 |
| WEB-PKM-013 IVs/EVs | As an editor, I want stats investment editing with correct limits. | Explicit HP/Atk/Def/SpA/SpD/Spe labels; Core per-stat/total constraints; no silent clamping; stale legality and recalculated stats; Gen 1/2 DVs/stat experience not mislabeled IVs/EVs. | Must / MVP | WEB-PKM-001 |
| WEB-PKM-014 Stats/characteristic | As a user, I want trustworthy calculated values. | Compute on draft clone through Core; show calculated values read-only; characteristic when supported; party apply follows the explicit PK6 stat/HP/status policy in State model, including fainted and HP-clamp fixtures. | Must / MVP | WEB-PKM-005; WEB-PKM-007; WEB-PKM-013 |
| WEB-PKM-015 PID/encryption constant | As an advanced editor, I want raw identity fields with clear risks. | Strict hexadecimal/uint validation; relevant fields only; preview derived gender/nature/shiny changes; no random regeneration; report correlation violations; export/reload retains exact selected value. | Should / Post-MVP | WEB-PKM-001; WEB-LEGAL-003 |
| WEB-PKM-016 Shiny state/type | As an advanced editor, I want explicit shiny changes. | Show current state in MVP; later distinguish star/square only where meaningful; Core helper results preview PID/ID changes; no guarantee of legal encounter or hidden auto-legalization. | Should / Post-MVP | WEB-PKM-015; WEB-PKM-017 |
| WEB-PKM-017 OT and IDs | As an advanced editor, I want original-trainer identity controls. | Names/gender and TID/SID validated; displayed versus stored ID formats explained; shiny relationship preview; no “make mine” rewrite by default; event fixed-trainer issues reported. | Should / Post-MVP | WEB-PKM-001; WEB-LEGAL-003 |
| WEB-PKM-018 Handling trainer | As an advanced editor, I want to inspect and intentionally edit handler data. | Capability-specific current handler, name/gender/language/friendship as stored; apply does not re-adapt through default import settings; legality refreshed; absent fields untouched. | Should / Post-MVP | WEB-PKM-017 |
| WEB-PKM-019 Met/encounter/origin | As an editor, I want accurate origin data. | Origin game, met location/level, ball and met date from contextual Core lists; calendar/range validation; encounter report refreshed; no generated encounter or automatic date correction. | Should / Post-MVP | WEB-PKM-001; WEB-LEGAL-004 |
| WEB-PKM-020 Egg data | As a breeder, I want egg-specific fields only when meaningful. | Egg flag, egg location/date and hatch state use format capabilities; contradictory states produce validation/legality feedback; changing egg state previews dependent data; no silent hatch operation. | Should / Post-MVP | WEB-PKM-019 |
| WEB-PKM-021 Fateful/Pokérus | As an advanced editor, I want supported encounter flags and infection data. | Separate fateful flag from legality; Pokérus strain/days/cured state use Core limits; absent modern-format support hidden; no automatic event validity claim. | Should / Post-MVP | WEB-PKM-019 |
| WEB-PKM-022 Markings | As an organizer, I want format-correct markings. | Show only stored marking styles/counts; keyboard toggles have names/state; unrelated flags preserved; serialized entity matches selection. | Could / MVP+ | WEB-PKM-001 |
| WEB-PKM-023 Ribbons/marks | As a collector, I want supported awards represented accurately. | Capability-filtered ribbons, counts, marks and selected title where applicable; clear unsupported/illegal distinctions; no bulk grant by default; round-trip tests across supported contexts. | Should / Post-MVP | WEB-PKM-001; WEB-CROSS-004 |
| WEB-PKM-024 Contest data | As a contest player, I want contest stats and sheen. | Format-specific categories/ranges, explicit sheen, preserved unrelated stats; no assumption every game stores contests identically. | Could / Post-MVP | WEB-PKM-001 |
| WEB-PKM-025 Memories | As an advanced editor, I want supported OT/handler memories. | Use Core memory contexts/text choices; intensity/feeling/argument dependencies validated; no generic free-form memory string written into binary fields. | Should / Post-MVP | WEB-PKM-018 |
| WEB-PKM-026 Relearn/records | As an editor, I want relearn moves and move acquisition records. | Core capabilities gate relearn slots, TR flags and move-shop data; edits explicit; current moves not silently replaced; legality and export/reload tested. | Should / Post-MVP | WEB-PKM-011; WEB-CROSS-004 |
| WEB-PKM-027 Dynamax/Gigantamax | As a Sword/Shield user, I want relevant power metadata. | Capability-gated Dynamax level/Gigantamax flag; Core limits; incompatible species/encounter flagged; no generation-wide assumption that every Gen 8 entity supports both. | Should / MVP+ | WEB-CROSS-004; WEB-PKM-001 |
| WEB-PKM-028 Tera types | As a Scarlet/Violet user, I want original and overridden Tera information. | Distinguish original/override semantics and unset value; Core-supported choices including special cases; legality report reflects edit; no HOME-based assumptions. | Should / MVP+ | WEB-CROSS-004; WEB-PKM-001 |
| WEB-PKM-029 Legends metadata | As a Legends player, I want format-specific attributes. | Alpha/Noble, size and effort/move metadata exposed only by actual context; no cross-Legends field equivalence assumed; per-field fixture and legality regression before enabling. | Should / Post-MVP | WEB-CROSS-004; WEB-PKM-001 |
| WEB-PKM-030 HOME tracker | As an advanced user, I want transparent modern metadata handling. | Preserve tracker by default; display with privacy caution; no fabrication or reset shortcut; any future editing requires separate upstream-approved semantics and tests. | Won't-for-now / Long-term/experimental | WEB-CROSS-003; AES needed for standalone HOME crypto, not every tracker display |
| WEB-PKM-031 Other generation fields | As a user of another supported game, I want meaningful fields without false universal controls. | Family-specific DV/stat experience, awakening/hyper-training/size/form-argument and other supported interfaces catalogued; read-only until tested; each added editable field has preservation/round-trip cases. | Should / Post-MVP | WEB-CROSS-004 |

### Legality

| ID / title | User story | Acceptance criteria | Priority / milestone | Dependencies / notes |
|---|---|---|---|---|
| WEB-LEGAL-001 Analyze selected entity | As a user, I want an explicit Core legality result. | Run on cloned draft with personal table and slot context; show pending, valid, invalid, unavailable separately; Core version visible; no mutation or network call. | Must / MVP | WEB-PKM-001; WEB-TEST-004 |
| WEB-LEGAL-002 Human-readable issues | As an editor, I want understandable findings. | Use Core reports/severity rather than guessing from translated text; link to applicable editor section where determinable; text plus icon; verbose detail expandable. | Must / MVP | WEB-LEGAL-001 |
| WEB-LEGAL-003 Refresh/stale results | As an editor, I want findings that match current values. | Edits immediately invalidate prior result; revision-tagged debounce/manual refresh; obsolete completion ignored; error never appears legal; apply/export acknowledge unresolved/invalid result without automatic repair. | Must / MVP | WEB-LEGAL-001; WEB-SESSION-002 |
| WEB-LEGAL-004 Encounter details | As an advanced user, I want evidence behind a result. | Display Core matched encounter and available context, distinguish no/ambiguous match; no inferred proof of authenticity; verbose output survives trimmed publish/localization. | Should / MVP+ | WEB-LEGAL-002 |
| WEB-LEGAL-005 Grid indicators | As an organizer, I want optional legality markers. | Only analyzed revisions show verdicts; unknown is distinct; on-demand bounded work; invalidation after writes; no blocking whole-save scan while opening. | Should / MVP+ | WEB-LEGAL-003; WEB-PERF-002 |
| WEB-LEGAL-006 Optional legalization | As an advanced user, I may want deliberate repair suggestions. | Separate upstream/dependency decision; preview every changed field, explicit confirmation, bounded execution, known unsupported cases; never part of ordinary Analyze or Apply. | Won't-for-now / Long-term/experimental | WEB-SEC-004; WEB-LEGAL-004; no AutoMod dependency in MVP |

### PKM import/export

| ID / title | User story | Acceptance criteria | Priority / milestone | Dependencies / notes |
|---|---|---|---|---|
| WEB-ENTITY-001 Export selected PKM | As a user, I want a portable entity file. | Resolve unapplied draft choice; correct Core format bytes/extension; sensible sanitized filename; export/reparse equality of supported fields; never mutate save merely by exporting. | Should / MVP+ | WEB-EXP-002; WEB-PKM-001 |
| WEB-ENTITY-002 Import entity file | As a user, I want to place a local `.pk*` or other admitted entity. | Bound read, Core recognition, destination type checks and conversion preview; malformed entities rejected safely; overwrite confirmation; no change until acceptance. | Should / MVP+ | WEB-ENTITY-001; WEB-CROSS-001 |
| WEB-ENTITY-003 Drop entity | As a user, I want to drop an entity onto a chosen slot. | File classification explicit; same preview/validation as picker; do not confuse save-open with entity import; keyboard import equivalent; no automatic overwrite. | Could / MVP+ | WEB-ENTITY-002; WEB-BOX-005 |
| WEB-ENTITY-004 Import side effects | As a user, I want to know whether import updates ownership/dex/records. | Show conversion and explicit import options; default preserve metadata and no dex/record changes; an enabled option previews effect and uses Core settings; existing-slot edit policy stays separate. | Must / MVP+ | WEB-ENTITY-002; WEB-SESSION-002 |

### Save export

| ID / title | User story | Acceptance criteria | Priority / milestone | Dependencies / notes |
|---|---|---|---|---|
| WEB-EXP-001 Serialize safely | As a user, I want a valid modified save. | Export from working-save clone through `Write`; respect exportability and metadata; known checksums/signatures generated by Core; failure produces no replacement output and leaves session intact. | Must / MVP | WEB-SESSION-001; WEB-TEST-002 |
| WEB-EXP-002 Download snapshot | As a user, I want a clearly named local download. | Resolve draft first; user-initiated local Blob download; name policy and console-name warning; label initiation accurately; retry from same snapshot possible; Blob resources released. | Must / MVP | WEB-EXP-001; WEB-SESSION-007 |
| WEB-EXP-003 Reopen validation | As a user, I want the exported bytes checked before delivery. | Reparse separate output copy, check intended family/identity/checksum and changed-field values; serialization normalization accounted for; failure blocks download with recoverable message. | Must / MVP | WEB-EXP-001; WEB-TEST-002 |
| WEB-EXP-004 Container/integrity regression | As an emulator user, I want surrounding save structure intact. | Raw no-op browser/native comparison first; separately admitted wrappers retain header/footer/handler effects; expected normalization documented; no universal byte-identical-input assertion. | Must / MVP+ | WEB-EXP-003; WEB-TEST-002 |

### Pokédex

| ID / title | User story | Acceptance criteria | Priority / milestone | Dependencies / notes |
|---|---|---|---|---|
| WEB-DEX-001 View dex | As a collector, I want the game's actual dex state. | Use concrete game structures; show dex regions/revision and supported seen/caught/form/language/shiny data; unknown capability omitted, not zeroed. | Should / Post-MVP | WEB-CROSS-004 |
| WEB-DEX-002 Seen/caught edits | As an editor, I want safe individual dex changes. | Explicit seen/caught actions with game-required dependencies; preview effects; no conversion of all entries to a generic boolean model; preserve unrelated records. | Should / Post-MVP | WEB-DEX-001; WEB-SESSION-002 |
| WEB-DEX-003 Forms/languages/shiny | As a collector, I want detailed registration controls. | Only stored combinations shown; enforce Core structure sizes/flags; retain gender/display-form distinctions; per-family round-trip fixtures. | Should / Post-MVP | WEB-DEX-002 |
| WEB-DEX-004 Bulk completion | As an editor, I want deliberate bulk dex changes. | Preview target region/species/flags and count; confirm; atomic operation/reset path; no automatic all-languages/research completion; safe defaults retain unrelated progress. | Could / Post-MVP | WEB-DEX-003 |
| WEB-DEX-005 Research/alternate dex | As a Legends user, I want research-aware editing. | Separate research tasks/points from seen/caught; use game/revision-specific structures; no “complete” shortcut until dependencies tested; rollback and export/reload proven. | Could / Post-MVP | WEB-DEX-001; WEB-GAME-001 |

### Trainer editing

| ID / title | User story | Acceptance criteria | Priority / milestone | Dependencies / notes |
|---|---|---|---|---|
| WEB-TRAINER-001 Trainer identity | As an editor, I want to change supported trainer fields intentionally. | Clone/stage name/gender/IDs; format-aware lengths and displayed/raw IDs; warn existing Pokémon ownership is unchanged; explicit apply; no rewriting all OT data. | Should / MVP+ | WEB-OVERVIEW-001; WEB-SESSION-002 |
| WEB-TRAINER-002 Money/playtime | As a user, I want safe currency and time edits. | Core maxima/units; no overflow or silent clamp; unsupported time fields read-only; transaction and reload test confirm exact intended values. | Should / MVP+ | WEB-OVERVIEW-002; WEB-TRAINER-001 |
| WEB-TRAINER-003 Language/progress | As an advanced user, I want appropriate language and game-specific trainer settings. | Distinguish encoding/interpretation from stored language; warn dependencies; badge/progress/avatar/records individually capability-gated; unknown flags untouched. | Should / Post-MVP | WEB-TRAINER-001; WEB-GAME-001 |

### Mystery Gifts

| ID / title | User story | Acceptance criteria | Priority / milestone | Dependencies / notes |
|---|---|---|---|---|
| WEB-GIFT-001 Gift album | As a user, I want to inspect stored Wonder Cards and album state. | Detect `IMysteryGiftStorageProvider`/actual game capability; list occupied/empty entries and redemption metadata; unsupported families explained; no generic gift-count assumptions. | Should / Post-MVP | WEB-CROSS-004 |
| WEB-GIFT-002 Import/validate card | As a collector, I want to preview a local gift file. | Bound read, Core format parse, game/generation compatibility, duplicate/full-album checks and redemption implications; no live event download; preview before mutation. | Should / Post-MVP | WEB-GIFT-001; WEB-SEC-001 |
| WEB-GIFT-003 Inject/remove | As an editor, I want deliberate album changes. | Explicit slot choice/confirmation; Core storage setters and relevant flags; transactional rollback; distinguish album injection from generating a Pokémon; removal honors format semantics. | Should / Post-MVP | WEB-GIFT-002; WEB-SESSION-002 |
| WEB-GIFT-004 Generation coverage | As a user of an older game, I want accurate gift support claims. | Matrix names admitted formats such as PGT/PCD/PGF/WC variants after tests; unsupported card rejected without conversion guess; fixture verifies exported save album/flags. | Must / Post-MVP | WEB-GIFT-003; per-family admission |

### Inventory/bag

| ID / title | User story | Acceptance criteria | Priority / milestone | Dependencies / notes |
|---|---|---|---|---|
| WEB-BAG-001 View pockets | As a user, I want to inspect inventory by actual pocket. | Enumerate Core pouch capabilities/counts; display stored items and quantities; distinguish unused slots from unknown IDs; do not reorder just by viewing. | Should / Post-MVP | WEB-CROSS-004 |
| WEB-BAG-002 Edit quantities/items | As an editor, I want validated bag changes. | Core pocket item rules and count bounds; key-item restrictions and full pocket handling; explicit staged apply; preserve unrelated slots and special pouch flags. | Should / Post-MVP | WEB-BAG-001; WEB-SESSION-002 |
| WEB-BAG-003 Bulk inventory | As an advanced editor, I want deliberate batch bag changes. | Preview pocket/items/counts; exclude unsafe key-item/progress changes by default; atomic commit/reset; no generic “all items” across generations. | Could / Post-MVP | WEB-BAG-002 |

### Showdown

| ID / title | User story | Acceptance criteria | Priority / milestone | Dependencies / notes |
|---|---|---|---|---|
| WEB-SHOWDOWN-001 Export text | As a player, I want a Showdown representation of a Pokémon. | Use Core export; local copy/download; identify omitted fields such as origin/IDs/ribbons; format respects chosen context; no upload or URL shortening. | Should / Post-MVP | WEB-ENTITY-001 |
| WEB-SHOWDOWN-002 Parse/import text | As a player, I want a pasted set applied explicitly. | Core parsing with line/field errors; preview represented fields only against selected draft; no generated legal encounter; illegal combinations reported; no silent dropping of failed sets. | Should / Post-MVP | WEB-SHOWDOWN-001; WEB-LEGAL-003 |
| WEB-SHOWDOWN-003 Teams/generation | As a player, I want batch text and destination awareness. | Validate each set against destination context; preview capacity/placement; reject URL fetches; atomic selected import; explain missing information and unsupported fields. | Could / Post-MVP | WEB-SHOWDOWN-002; WEB-BOX-005 |

### Save-specific/game-specific editors

| ID / title | User story | Acceptance criteria | Priority / milestone | Dependencies / notes |
|---|---|---|---|---|
| WEB-GAME-001 Capability catalog | As a user, I want only supported game tools listed. | Catalog from actual Core structures/WinForms evidence for daycare, RTC/berries, events/roamers, records, Hall of Fame and research; every enabled editor names families/revisions and tests. | Should / Post-MVP | WEB-CROSS-004 |
| WEB-GAME-002 Daycare/RTC | As a player, I want selected practical save tools. | Separate daycare storage/withdrawal from RTC repair; preview entity/time effects and destination capacity; explicit confirmation; per-game fixtures; no automatic startup repair. | Could / Post-MVP | WEB-GAME-001; WEB-BOX-005 |
| WEB-GAME-003 Progress/events/records | As an advanced editor, I want curated progress tools. | Named operations backed by Core structures; show dependencies and scope; avoid magic flag lists copied from PKForge; atomic commit and family-specific regression. | Could / Post-MVP | WEB-GAME-001 |
| WEB-GAME-004 Expert blocks/flags | As an expert, I may want raw save inspection/editing. | Separate explicit expert-mode proposal; bounded inputs; typed block metadata where available; no claims of safe arbitrary writes; export/reset and corruption tests before enabling. | Won't-for-now / Long-term/experimental | WEB-GAME-001; WEB-ERR-002 |
| WEB-GAME-005 Encounter lookup | As a collector, I want local encounter information. | Use embedded Core data, contextual filters and clear match limitations; no remote fetch or automatic entity generation; expensive searches bounded/gated. | Should / Post-MVP | WEB-LEGAL-004; WEB-PERF-004 |
| WEB-GAME-006 Batch edits | As an organizer, I want repeatable multi-entity changes. | Explicit supported operations, preview count/diffs, no arbitrary reflected script engine; all-or-nothing apply and bounded history; per-entity legality remains distinct. | Could / Post-MVP | WEB-BOX-007; WEB-SESSION-006 |

### Cross-generation handling

| ID / title | User story | Acceptance criteria | Priority / milestone | Dependencies / notes |
|---|---|---|---|---|
| WEB-CROSS-001 Core conversion | As a user, I want older entities imported only through supported conversion. | Core recognition/type conversion; inspect result codes; target game availability checked separately; unsupported paths refused; source bytes unchanged. | Must / MVP+ | WEB-PKM-001; no PKForge converter dependency |
| WEB-CROSS-002 Conversion preview | As a user, I want to understand irreversible changes. | Before/after field diff and legality; disclose lost fields/moves/metadata and one-way paths; explicit accept/cancel; no claim that format conversion guarantees legal provenance. | Must / MVP+ | WEB-CROSS-001 |
| WEB-CROSS-003 Cross-save transfer | As a collector, I want safe local transfers between saves. | Separate source/destination session identities; conversion preview; copy default; source deletion only after destination success and explicit move intent; both exports required and clearly labeled. | Should / Post-MVP | WEB-CROSS-002; WEB-SESSION-001; revisit single-session globals |
| WEB-CROSS-004 Family admission | As a user, I want honest game coverage. | Add each compatibility row only after load/edit/legality/export tests, format-specific field mapping, device budgets and crypto gates; hidden/read-only unsupported fields preserved. | Must / MVP+ | WEB-TEST-002; WEB-TEST-004; WEB-PERF-001 |

### Pokémon bank / local library

| ID / title | User story | Acceptance criteria | Priority / milestone | Dependencies / notes |
|---|---|---|---|---|
| WEB-BANK-001 Opt-in local storage | As a collector, I want a private persistent library without cloud accounts. | Separate explicit IndexedDB consent; explain origin/eviction/private-mode limits; denied/quota-failed storage leaves source save intact; no editing-session auto-save. | Should / Post-MVP | WEB-SEC-002; WEB-ENTITY-001 |
| WEB-BANK-002 Add/browse | As a collector, I want to keep Pokémon across sessions. | Store immutable original entity bytes, format, stable ID and optional provenance; atomic data/index transaction; list/preview by metadata; duplicate policy asks or labels, never silently deletes. | Should / Post-MVP | WEB-BANK-001 |
| WEB-BANK-003 Search/filter | As a collector, I want to find species/forms/origins/shinies. | Species, form, generation, origin and shiny filters; derived indexes versioned/rebuildable from bytes; pagination/virtualization; unknown origin distinct from absent. | Should / Post-MVP | WEB-BANK-002 |
| WEB-BANK-004 Return to save | As a collector, I want to copy a library entity into a save. | Destination conversion/legality preview and overwrite checks; failed import preserves library; source library entry retained by default; no unsupported backward transfer. | Should / Post-MVP | WEB-BANK-002; WEB-CROSS-002 |
| WEB-BANK-005 Backup/restore | As a collector, I want a portable backup before storage can be lost. | Versioned archive with entity bytes/manifest; validate size/count/path traversal and hashes; transactional restore preview/duplicate handling; corrupt backup does not destroy existing library; no server. | Must / Post-MVP | WEB-BANK-002; required before advertising library as durable |
| WEB-BANK-006 Migration/clear | As a user, I want safe upgrades and complete deletion. | Versioned schema migration with backup/export path; no destructive auto-upgrade on failure; clear library requires explicit confirmation and removes indexes/data; session save untouched. | Must / Post-MVP | WEB-BANK-005 |

### Living Dex / collection tools

| ID / title | User story | Acceptance criteria | Priority / milestone | Dependencies / notes |
|---|---|---|---|---|
| WEB-COLLECT-001 National progress | As a collector, I want to see missing species in my library. | Counts derived from actual entries and selected dex scope; missing species list; no generated entities; context excludes unavailable species explicitly. | Should / Post-MVP | WEB-BANK-003 |
| WEB-COLLECT-002 Shiny/forms/generations | As a collector, I want separate collection goals. | Select normal/shiny, form policy and generation/origin filters; denominators visible; gender/cosmetic/battle-only forms distinguished; no conflation with in-game dex flags. | Could / Post-MVP | WEB-COLLECT-001 |
| WEB-COLLECT-003 Provenance/duplicates | As a collector, I want source tracking and duplicate review. | Optional source label stored locally; exact-byte duplicates distinct from heuristic same-identity matches; review before removal; unknown provenance stays unknown. | Should / Post-MVP | WEB-BANK-002; WEB-BANK-005 |

### Accessibility

| ID / title | User story | Acceptance criteria | Priority / milestone | Dependencies / notes |
|---|---|---|---|---|
| WEB-A11Y-001 Keyboard/screen reader | As an assistive-technology user, I want the complete editing journey. | Named controls, logical headings, labeled box coordinates/occupancy, arrow-key grid and list alternative; keyboard reaches edit/apply/export; screen reader announces selection and errors without excessive chatter. | Must / MVP | WEB-APP-001 |
| WEB-A11Y-002 Focus and alternatives | As a keyboard user, I want predictable focus and non-drag actions. | Dialog focus contained/restored; validation focuses summary then field; no focus loss after box render; every drag operation has equivalent commands; escape cancels safely. | Must / MVP | WEB-A11Y-001 |
| WEB-A11Y-003 Visual/touch access | As a low-vision or touch user, I want readable controls. | WCAG 2.2 AA contrast/reflow checks, textual status, zoom to 400%, usable target spacing, reduced motion; touch operation not dependent on hover; no obscured apply/export controls. | Must / MVP | WEB-APP-001 |
| WEB-A11Y-004 Language readiness | As a non-English user, I want game data and future UI localization to remain coherent. | Core names/reports use explicit language; UI strings separated from code; no parsing localized report strings into logic; future locale change refreshes views without changing save language. | Should / MVP+ | WEB-LEGAL-002; full UI translation not MVP gate |

### Privacy/security

| ID / title | User story | Acceptance criteria | Priority / milestone | Dependencies / notes |
|---|---|---|---|---|
| WEB-SEC-001 No save transmission | As a user, I want proof that file actions remain local. | Published E2E inspects requests/URLs/headers/bodies; only fixed input-independent app assets fetched; compare traces for different saves/selections; no telemetry/per-entity sprite requests/event lookup; save-derived strings absent from traffic; privacy copy distinguishes asset hosting logs. | Must / Foundation | WEB-HOST-001 |
| WEB-SEC-002 Ephemeral sessions | As a user, I want reload to discard work unless I exported it. | No save bytes/derived identifiers in localStorage, IndexedDB, Cache Storage, URLs or history state; reload/close recovery tests show empty session; later library opt-in is isolated. | Must / MVP | WEB-SESSION-001 |
| WEB-SEC-003 Hostile input/rendering | As a user, I want malformed files/names contained. | Size/operation limits; filename/path/control sanitization; names displayed as text; file-only drop; parser errors redacted; CSP and no third-party runtime code verified. | Must / MVP | WEB-SAVE-001; WEB-ERR-002 |
| WEB-SEC-004 Dependency/license provenance | As a self-hoster, I want auditable code and assets. | Exact dependencies/transitives and licenses inventoried; sprites/fonts have attribution; no copied PKForge network/art dependencies; corresponding source and build instructions accompany releases. | Must / Foundation | licensing decision gate |
| WEB-SEC-005 Safe diagnostics | As a reporter, I want useful bug details without exposing my save. | Opt-in preview/copy/download with build, browser, operation and sanitized error code; no raw bytes, paths, names, IDs or exception payload by default; explicit warning before adding personal data. | Must / MVP | WEB-ERR-003; WEB-APP-002 |

### Error handling

| ID / title | User story | Acceptance criteria | Priority / milestone | Dependencies / notes |
|---|---|---|---|---|
| WEB-ERR-001 File errors | As a user, I want actionable load failures. | Distinguish no selection/read/size/format/family/integrity errors; point to supported input requirements; no fabricated certainty about corruption when only detection failed; retry possible. | Must / MVP | WEB-SAVE-001 |
| WEB-ERR-002 Mutation/parser failures | As a user, I want failed operations to preserve data. | Exception boundary around Core memory parsing and staged writes; restore previous valid session/selection; malformed PKM later follows same policy; no partially applied transaction or downloadable failed output. | Must / MVP | WEB-SESSION-001 |
| WEB-ERR-003 Fatal UI/recovery | As a user, I want a safe response to a broken screen. | Error boundary offers reset/reload with loss warning and redacted diagnostics; if working state remains valid allow guarded export; never export known inconsistent state; no auto-reporting. | Must / MVP | WEB-APP-001; WEB-ERR-002 |
| WEB-ERR-004 Interrupted operations | As a user, I want cancellation/failure handled honestly. | Cancel asynchronous reads; disable duplicate apply/export; revision tokens ignore obsolete completions; do not promise canceling synchronous Core code; memory/device termination limits disclosed. | Must / MVP | WEB-SESSION-001 |

### Performance

| ID / title | User story | Acceptance criteria | Priority / milestone | Dependencies / notes |
|---|---|---|---|---|
| WEB-PERF-001 Startup/bundle baseline | As a user, I want a reasonably fast first load. | Measure Release transfer/raw sizes and cold/warm startup against stated profile; list largest assets and resource cost; meet target or document a scoped release limitation; no guessed download numbers. | Must / Foundation | WEB-HOST-001 |
| WEB-PERF-002 Large save/memory | As a tablet user, I want browsing/export to remain usable. | Measure peak original/working/snapshot/Blob allocations and repeated-session trend; admitted fixture maximum completes on target device; no whole-save UI/legality materialization. | Must / MVP | WEB-PERF-001; WEB-SESSION-001 |
| WEB-PERF-003 Sprite loading | As a user, I want efficient sprites that do not disclose my collection through requests. | Fixed complete MVP atlas/manifest fetched before opening files, independent of save content; visible sprites rendered from resident assets; missing variants use placeholders; no per-entity requests; catalog reproducible and within host limits. | Must / MVP | WEB-SEC-004; WEB-HOST-002 |
| WEB-PERF-004 Legality scheduling | As an editor, I want analysis without misleading or frozen controls. | Measure selected analysis corpus; debounce and stale-state handling; no automatic brute-force solver flags; when budget fails, gate feature or test worker isolation; `Task.Run` alone is not a solution. | Must / MVP | WEB-LEGAL-001 |
| WEB-PERF-005 AOT/lazy options | As a maintainer, I want optimizations backed by data. | Compare interpreter and AOT build/download/analysis/memory; optional editor/asset lazy loading measured; reject change if host limits or startup regress without agreed benefit. | Could / Post-MVP | WEB-PERF-001; WEB-PERF-004 |

### Browser support

| ID / title | User story | Acceptance criteria | Priority / milestone | Dependencies / notes |
|---|---|---|---|---|
| WEB-BROWSER-001 Desktop qualification | As a desktop user, I want an accurate support statement. | Published E2E across Chromium/Firefox/WebKit plus physical Safari and claimed Edge versions; exact versions recorded; root/subpath, file, editor and export workflows pass. | Must / MVP | WEB-TEST-005 |
| WEB-BROWSER-002 Tablet/mobile files | As a mobile user, I want usable local file operations. | Physical iPad/Android picker, Files/document provider, orientation and download/reopen checks; no File System Access dependency; phone limitations explicitly listed. | Must / MVP | WEB-TEST-005; WEB-A11Y-003 |
| WEB-BROWSER-003 Lifecycle/memory | As a mobile user, I want honest data-loss expectations. | Test background/foreground, tab eviction/reload and back navigation; no false autosave promise; restored page clears retained session; large-file/device limitations documented. | Must / MVP | WEB-PERF-002; WEB-SEC-002 |

### Testing

| ID / title | User story | Acceptance criteria | Priority / milestone | Dependencies / notes |
|---|---|---|---|---|
| WEB-TEST-001 Core/provider proof | As a maintainer, I want browser feasibility established before broad UI work. | Native Core baseline plus published browser resource/legality/serialization probe; AES/MD5 provider vectors before affected features; report unavailable tests honestly; no compile-only support claim. | Must / Foundation | WEB-HOST-001 |
| WEB-TEST-002 Round-trip corpus | As a maintainer, I want save integrity regression protection. | Provenance-cleared valid/corrupt fixtures; no-op/edit export/reopen, checksum and allowed-diff assertions; browser/native comparison at same revision; family/revision coverage manifest. | Must / Foundation | WEB-TEST-001 |
| WEB-TEST-003 Transaction tests | As a maintainer, I want mutations either complete or absent. | Failure injection after staged writes, explicit import-setting assertions, original-byte immutability, stale draft rejection; later slot history and two-slot rollback checks. | Must / MVP | WEB-SESSION-002 |
| WEB-TEST-004 Legality/editor regressions | As a maintainer, I want field changes verified against Core. | Known legality fixtures; supported field limits/dependencies, hidden-field preservation and formatting; trimmed publish includes required data/localization; no automatic repair. | Must / MVP | WEB-TEST-001; WEB-PKM-001 |
| WEB-TEST-005 Published browser journey | As a maintainer, I want proof of the shipped artifact. | Playwright runs static Release output; picker/drop/select/edit/apply/download/reopen; privacy/storage assertions; keyboard/automated accessibility checks; root/subpath deployment. | Must / MVP | WEB-EXP-003; WEB-HOST-002 |
| WEB-TEST-006 CI evidence | As a reviewer, I want reproducible checks and artifacts. | Linux Core/Web tests and publish; desktop regression job; warnings triaged; reports/sizes/versioned output retained; no deployment secrets in PR execution. | Must / Foundation | WEB-TEST-001; WEB-HOST-001 |

### Hosting/deployment

| ID / title | User story | Acceptance criteria | Priority / milestone | Dependencies / notes |
|---|---|---|---|---|
| WEB-HOST-001 Static artifact | As a self-hoster, I want an ordinary directory to serve. | Standalone Release output works from HTTP static server with no .NET server/backend/database; Web references Core directly; output excludes secrets and user fixtures; documented build prerequisites. | Must / Foundation | None |
| WEB-HOST-002 Paths/headers/compression | As a deployer, I want reliable boot on common hosts. | Root and `/PKHeX/` work; correct WASM MIME/encoding/CSP; immutable/revalidated cache split; no HTML returned as required binary resource; Cloudflare asset/file limits checked. | Must / MVP | WEB-HOST-001 |
| WEB-HOST-003 Releases/rollback | As a maintainer, I want controlled public deployments. | Version/Core revision in artifact; optional trusted deployment job only; complete artifact promotion/rollback; no automatic forced reload or mixed-version publish; self-host instructions and support matrix. | Must / MVP | WEB-TEST-006; WEB-HOST-002 |
| WEB-HOST-004 PWA updates | As an offline user, I want updates without losing edits. | Versioned app-only cache; no save caching; old/new multi-tab tests; explicit safe activation; blocked update leaves active session usable; offline cold launch proven before advertised. | Could / Post-MVP | WEB-HOST-003; WEB-SEC-002 |

## Prioritised implementation matrix

Story metadata is the authoritative per-story mapping; every story belongs to exactly one milestone below. Priority describes importance within that milestone, not an arbitrary numeric score. Dependencies are acyclic and never require a later milestone. Components may be co-developed in one vertical slice; Foundation fixtures define contracts before picker/editor UI exists.

| Stage | Story coverage / outcomes | Sequencing and exit gate |
|---|---|---|
| Foundation | WEB-APP-001; WEB-SAVE-005; WEB-SESSION-001; WEB-SEC-001, 004; WEB-PERF-001; WEB-TEST-001, 002, 006; WEB-HOST-001 | Privacy/asset/dependency boundaries, test fixtures and static publish proof first. Session scaffolding and original-byte contract precede UI. No public support claim from this stage |
| MVP | All rows marked MVP: shell 002–004; save 001–004, 007; session 002–005, 007; overview 001–002; party 001; boxes 001, 004, 008; PKM 001–007, 009–014; legality 001–003; export 001–003; accessibility 001–003; security 002–003, 005; errors 001–004; performance 002–004; browser 001–003; tests 003–005; hosting 002–003 | Raw XY/ORAS full journey. Prove no-op export before editor; shared form semantics before species/form controls. Privacy, accessibility, physical device, warning and round-trip gates required |
| MVP+ | Save 006, 008; session 006; overview remains read-only except trainer 001–002; party 002–004; boxes 002, 005–006; PKM 008, 022, 027–028; legality 004–005; entity 001–004; export 004; cross 001–002, 004; accessibility 004 | Expand one family at a time. AES/MD5 proof before affected saves. Conversion preview before external import; atomic slot operations before drag/undo. Should/Could work can be omitted without changing MVP |
| Post-MVP | App 005; overview 003; boxes 003, 007; PKM 015–021, 023–026, 029, 031; all dex/gift/bag/Showdown stories; trainer 003; game 001–003, 005–006; cross 003; all bank/collection stories; performance 005; hosting 004 | Family-specific save editors independently reviewable. Bank consent and backup/restore/migration gates before durable-library claim. PWA does not imply save persistence |
| Long-term/experimental | WEB-PKM-030; WEB-LEGAL-006; WEB-GAME-004; whole-card/container support and unsupported-family investigations require new scoped stories | Separate upstream suitability decisions for tracker modification, legalization and raw expert editing. No implicit implementation authorization |

The Foundation feasibility probe can use a tiny ephemeral harness or a reviewed browser test project in future implementation. This documentation task did not create one. UI completeness, full game coverage, and universal browser compatibility must never be inferred from a green compile.

## Licensing

[PKHeX LICENSE](LICENSE) contains GPL version 3, and [Core package metadata](PKHeX.Core/PKHeX.Core.csproj) explicitly declares `GPL-3.0-or-later`. [PKForge LICENSE](../PKForge/LICENSE) contains GPL version 3 and its README declares GPLv3 or later. This supports a GPL-compatible upstream project; it does not establish rights to redistribute every third-party asset under the code license.

Distribute Web under the upstream license policy and preserve notices. Browser-delivered managed/WASM binaries are distributed artifacts: provide matching corresponding source, build inputs/instructions and dependency notices with each release. Do not treat static hosting as avoiding source obligations. This is a project release requirement; resolve uncertain third-party terms with maintainers before incorporation rather than offering a blanket legal conclusion.

PKHeX's README identifies QRCoder as MIT; Web does not need its Windows drawing dependency for MVP. PKForge separately credits PKSM UI, community game art, game-icons.net, runtime item art and Pokémon rights holders. Reusing PKForge code would require attribution and source review; reusing its visual design/assets additionally requires asset-specific provenance. Avoid copying its art/network dependencies. Existing PKHeX sprite use is not proof of broader trademark/art permissions for a new public host; retain known attribution and have maintainers decide release asset policy.

The AES/MD5 implementation, any future UI toolkit, test tooling redistributed with releases, AutoMod and other transitive libraries need license/version review. AutoMod's presence in a GPL Android app does not establish suitability, maintenance commitment, or browser support in upstream Web. No binary plugin loading or native injection libraries.

## Risks and decision gates

| Gate | Required evidence / decision | Default until resolved |
|---|---|---|
| Maintainer interest and ownership | Agreement on project scope, maintenance/release owner and support expectations. Status 2026-09-26: not yet sought; no outreach made | Local/fork exploration only; no assumed upstream acceptance |
| Browser feasibility | Release publish, resources/localization, XY/ORAS parsing/legality/export in real engines. Status 2026-09-26: XY/ORAS open → legality → boxed nickname edit → export demonstrated locally in three desktop engines ([proof](PKHeX.Web.WasmProof.md)); localization, other families and devices unproven | No supported Web families claimed; XY/ORAS are proof-level only |
| Crypto provider | Exact synchronous AES/MD5 behavior, license/transitives, browser/native vectors and affected save fixtures | SM/USUM signed export, BDSP checksums and standalone HOME crypto disabled |
| Shared edit semantics | Desktop-equivalent dependent-field behavior with explicit side effects and no duplicate game rules | Do not expose a write control whose dependencies are unknown |
| Asset policy | Provenance/attribution and static sprite mapping reviewed; measured host file/size limits | Text placeholders and no third-party network fetch |
| Performance/device budget | Corpus, named reference devices, peak memory, analysis latency and artifact sizes | Restrict slow features/families; no async-cancellation or mobile-parity promises |
| Integrity coverage | Provenance-cleared fixtures and expected serialization normalizations | Disable untested wrappers/families; no raw-data export shortcuts |
| Public deployment | Trusted release artifact, support report, source/notices, tested host headers/cache | No automatic upstream deployment |

Remaining implementation decisions intentionally require empirical gates: exact managed crypto package, measured browser/device release versions, asset distribution approval, whether measured legality costs justify worker isolation, and whether maintainers accept any shared API additions. The architecture, MVP behavior and acceptance tests above constrain those decisions; an agent must not fill them with invented compatibility claims or broaden the scope to bypass a gate.

## External technical references

Primary documentation checked **2026-09-26**. Microsoft pages may render content for multiple versions; use the selected .NET 10 view and exact SDK output, not an older template example. Revalidate hosting quotas and browser policy at implementation/release time.

- [Microsoft: standalone WASM hosting](https://learn.microsoft.com/en-us/aspnet/core/blazor/host-and-deploy/webassembly/?view=aspnetcore-10.0) — static output, base path, MIME/compression and deployment behavior.
- [Microsoft: AOT/build tools](https://learn.microsoft.com/en-us/aspnet/core/blazor/webassembly-build-tools-and-aot?view=aspnetcore-10.0) — interpreter/Jiterpreter versus AOT and publish-time tooling.
- [Microsoft: AES API](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.aes.create?view=net-10.0), [MD5 API](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.md5.hashdata?view=net-10.0), [crypto compatibility note](https://learn.microsoft.com/en-us/dotnet/core/compatibility/cryptography/5.0/cryptography-apis-not-supported-on-blazor-webassembly) — platform limitations and exceptions.
- [Microsoft: file input](https://learn.microsoft.com/en-us/aspnet/core/blazor/file-uploads?view=aspnetcore-10.0), [downloads](https://learn.microsoft.com/en-us/aspnet/core/blazor/file-downloads?view=aspnetcore-10.0) — browser reading limits and local download mechanisms.
- [Microsoft: CSP](https://learn.microsoft.com/en-us/aspnet/core/blazor/security/content-security-policy?view=aspnetcore-10.0), [PWA lifecycle](https://learn.microsoft.com/aspnet/core/blazor/progressive-web-app) — runtime policy and offline update considerations.
- [Cloudflare Pages limits](https://developers.cloudflare.com/pages/platform/limits/), [Direct Upload](https://developers.cloudflare.com/pages/get-started/direct-upload/), [serving behavior](https://developers.cloudflare.com/pages/configuration/serving-pages/), [custom headers](https://developers.cloudflare.com/pages/configuration/headers/) — actual Pages behavior, not Workers limits.
- [GitHub Pages limits](https://docs.github.com/en/pages/getting-started-with-github-pages/github-pages-limits) — published-site and hosting quotas.
- [MDN: SubtleCrypto encrypt](https://developer.mozilla.org/en-US/docs/Web/API/SubtleCrypto/encrypt) — asynchronous supported algorithms; not a synchronous ECB provider.
- [MDN: beforeunload](https://developer.mozilla.org/en-US/docs/Web/API/Window/beforeunload_event) — unreliable mobile unload notification; warnings are best effort.
- [W3C WCAG 2.2](https://www.w3.org/TR/WCAG22/) — accessibility target and manual/automated verification basis.

## Adversarial Review Summary

A separate reviewer subagent examined the complete first draft and challenged it against the two local repositories and primary platform/hosting documentation. The author independently re-read the cited Core paths and revised the document. This was an architectural/source review, not a build or browser test.

| Finding | Verification and resulting decision |
|---|---|
| **P1: lazy sprite requests disclose save-derived species/forms/shinies to host logs** | Confirmed from the proposed request flow: same-origin static requests are still visible to the host. Replaced per-entity lazy requests with a fixed complete MVP atlas/manifest fetched before file access; rendering may remain lazy, requests may not. Missing variants use resident placeholders. Added cross-save traffic comparison acceptance criteria and acknowledged upfront size cost |
| **P1: party edits can retain stale calculated stats or inadvertently heal** | Verified `SaveFile.SetPartyValues` skips recomputation when stats exist, while `PKM.ResetPartyStats` updates level/stats, fills HP and clears status. Added explicit PK6 recalculate/preserve-status/clamp-HP policy, preservation of fainted state, dependent-change preview and regression cases. Added slot permission/party-count preflight for direct setters |
| **P2: dependency cycle and later-milestone prerequisite** | Naming depended on download, which depended on naming; Foundation session depended on MVP original-byte ownership. Removed the reverse naming dependency, promoted original-byte ownership to Foundation without picker-UI dependency, and checked all 144 story IDs/dependencies for acyclicity and stage ordering |
| **P2: wrong class cited for bank support** | Verified `SAV_BEEF` is the Gen 6/7 checksum base, while `SaveUtil` selects `Bank3`, `Bank4` and `Bank7` for bulk storage. Corrected compatibility evidence and separated whole-memory-card handling from browser-local library design |
| **P2: draft Apply is not sufficient before replacing/closing a session** | Confirmed the flow could imply that applying to memory made replacement safe. Added two distinct decisions: resolve the draft, then Export/Discard session/Cancel for replace/close/reset, with explicit continuation after download initiation and preservation of the old session on failure |

A bounded follow-up review confirmed all five fixes and found one minor wording inconsistency: atlases were still listed as optional follow-up work. That wording now says further atlas optimizations, consistent with mandatory input-independent MVP atlas loading.

**Rejected criticisms:** none of the five actionable findings were rejected. The reviewer also challenged and independently confirmed the existing import-setting side effects, export/finalization behavior, crypto extension points and browser restrictions, Cloudflare limits, PKForge fork/AutoMod coupling, contribution guidance, and code-license metadata. Those conclusions remain; no broad Core rewrite, backend fallback or PKForge dependency was introduced.

**Unresolved risks:** at the time of this review, no .NET SDK or browser harness was available. A later local proof ([PKHeX.Web.WasmProof.md](PKHeX.Web.WasmProof.md)) demonstrated a Release-trimmed XY/ORAS nickname round trip in desktop engines. It did not show localization, other families, crypto, sprites, memory budgets, devices or maintainer acceptance, which remain open. Default publishes suppress Core trim warnings; 38 surface when enabled (mostly reflection-based batch editing, unused by the proof). Published WASM execution, trimming/localization/resource retention, managed crypto dependency selection, static sprite redistribution rights, real startup/analysis/memory costs, physical-device behavior and upstream acceptance remain gates. Correcting the proposal does not mark any compatibility row as browser-proven. No application code, commits, PRs or deployment were produced.

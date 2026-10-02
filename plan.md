# PKHeX.Web — delivery plan

## Context

`PKHeX.Web.md` sets out a static Blazor WebAssembly editor that runs alongside WinForms on top of an unmodified `PKHeX.Core`. `PKHeX.Web.WasmProof.md` records **Goal 1 as done**. A trimmed Release publish opens real XY/ORAS saves in Chromium, Firefox and WebKit, at `/` and `/PKHeX/`. It runs legality analysis, edits one boxed PK6 nickname through `EntityImportSettings.None`, and downloads output byte-identical to native Core. It made no Core changes. That proof is still untracked: `PKHeX.Web/` (`ProofSession.cs`, `App.razor`, `browser.js`) and `Tests/PKHeX.Web.Tests/` (Playwright + `StaticHost`).

Remaining goals:
2. **Narrowly justified shared refactors.** These are upstream PRs to `kwsch/PKHeX`. Each needs its own evidence and desktop regression tests.
3. **Web foundation + CI.** Promote the proof into a maintainable project skeleton, integrate it with the solution, and add a Linux publish/test/E2E workflow plus a Windows regression job.
4. **XY/ORAS MVP.** The full journey from the "Definition of MVP" section of `PKHeX.Web.md`: open → summary → party/boxes → draft editor → legality → atomic apply → validated export. It has to be accessible, private and static-hosted.

Each numbered chunk below is **one commit**. Every commit must build, and its tests must pass.

## Reference project

`PKForge` (`../PKForge`, Android save editor on `PKHeX.Core`) is the reference project. **Every chunk, in every phase, must be compared against it before it is considered done.**
- Find PKForge's code for the same job (e.g. `src/PKForge.Engine/SaveParser.cs`, `SaveEngineSession.cs`, `WriteSafety.cs`, `src/PKForge.Infrastructure/SafeSaveWriter.cs`) and its tests (`tests/PKForge.*.Tests`).
- Record, in the chunk's review: what we match, where we are deliberately stricter or different (and why), and what PKForge does that we don't. Each gap is either adopted in the chunk or assigned to a named later chunk in this plan.
- Verify PKForge's claims about Core against Core's source before relying on them. PKForge is prior art, not an authority; Core's behaviour and this plan's requirements win.
- Skip what is out of Web scope (emulator containers, ROM hacks, backups/restore points, bank storage), but say so in the review.

## Branching

- Remotes: `origin` = `jcreek/PKHeX` (fork), `upstream` = `kwsch/PKHeX`. The upstream default branch is `master`.
- `web/main` (fork only) holds the design docs, `plan.md` and all Web work. Topic branches for Web PRs are cut from it.
- Each upstream refactor goes on its own branch cut from `upstream/master`, e.g. `web/refactor-species-form`. It contains **no** Web files or docs. After merge (or while pending), rebase `web/main` onto it.
- Rebase onto `upstream/master` before each PR and rerun the affected tests.

## Human-only gates (not commits)

- **G-A Maintainer interest:** before the Web foundation PR goes upstream, ask through the CONTRIBUTING channels. Present the static/client-only goal, the proof results, asset/crypto implications and maintenance ownership. Refactor PRs are useful on their own and do not wait on this. Also raise: `dotnet.native.js`/`.wasm` are Emscripten output (MIT/NCSA), and the runtime pack's upstream notices do not mention Emscripten; ask whether the Web notices should add it or whether the .NET notices are considered sufficient. Also ask where Web CI should live. Upstream's CI is an Azure Pipelines classic pipeline (`project-pokemon/PKHeX`, definition 1), defined in the Azure DevOps UI with no file in the repository, so only maintainers can change it. The options are to accept `.github/workflows/web.yml` (GitHub Actions is already enabled upstream: `submit-nuget` runs there), or to add equivalent steps to the Azure pipeline, for which we supply the step list. Mention that its VsTest step (`**\$(BuildConfiguration)\*test*.dll`) matches no assembly (`No test sources found`), because the DLLs sit under `bin\Release\net10.0\`, so no test has run upstream in CI; `**\bin\Release\**\*Tests.dll` would fix it.
- **G-B Sprite/asset policy:** maintainers confirm that redistributing PokeSprite images on a public static host is acceptable. Until then, M5 ships text placeholders only.
- **G-C Physical devices:** real Safari (macOS), iPadOS Safari and Android Chrome runs for M21.
- **G-D Real-save fixtures:** the owner supplies XY/ORAS saves via env vars for local runs. They are never committed and never used in CI.
- **G-E Windows desktop testing:** a dedicated manual pass in WinForms on Windows. The macOS setup can build WinForms but cannot run it, so no WinForms UI change has been clicked through yet. Each change that touches WinForms adds its cases to a checklist (R3's is below). The upstream PR for that change waits until its checklist passes.

---

## Phase 0 — Record the starting point

**0.1 Add plan and design docs.** On `web/main`: commit `plan.md`, `PKHeX.Web.md` and `PKHeX.Web.WasmProof.md` only. The proof code stays untracked until F1.

## Phase 1 — Shared refactors (Goal 2)

MVP needs only one new shared API. Candidates in `PKHeX.Web.md` were checked against MVP needs:

| Candidate | Decision | Why |
|---|---|---|
| Species/form dependent-field update (`PKMEditor.UpdateSpecies/UpdateForm/SetAbilityList`, `PKHeX.WinForms/Controls/PKM Editor/PKMEditor.cs:1056,1231,498`) | **Do** | WEB-PKM-002 needs desktop-equivalent side effects: recomputing EXP for the level, the ability slot, a sane gender, gender-forms, Gen 3 Unown PID and the default nickname. Today this logic sits inside control handlers. |
| `SlotEditor.Set` overload with explicit `EntityImportSettings` (`PKHeX.Core/Editing/Saves/Slots/SlotEditor.cs:46`) | **Defer** | MVP has no undo. The proof already writes through `SlotInfoBox.WriteTo(..., EntityImportSettings.None)`. Revisit with WEB-SESSION-006 (MVP+). |
| `Main.SanityCheckSAV` interpretation extraction | **Defer** | XY/ORAS are self-describing. Needed for Gen 1–3 (MVP+). |
| Trim annotations for the 38 IL warnings | **Defer** | None of the warnings are on the XY/ORAS paths used (see proof). CI tracks them as a baseline instead. |

Branch `web/refactor-species-form` from `upstream/master`:

**R1 Characterise current behaviour (tests only).** Add `Tests/PKHeX.Core.Tests/Editing/SpeciesFormChangeTests.cs`. Its cases pin down what the WinForms handlers produce today, using existing Core calls:
- EXP after a growth-rate change at a fixed level
- ability index kept or clamped across forms (`SetAbilityIndex`, `PKHeX.Core/Editing/CommonEdits.cs:71`)
- `GetSaneGender` (`PKHeX.Core/Editing/Applicators/GenderApplicator.cs:59`) for genderless, fixed-gender and gender-form species (Meowstic, Pyroar, Indeedee)
- Unown in Gen 3 (`SetPIDUnown3`)
- the default nickname when not nicknamed
- form reset to 0 when `FormInfo.HasFormSelection` is false

Use PK3, PK4, PK6 and PK8 samples. Mark Gen 2 Unown (UI loop over random IVs) as out of scope.

**R2 Core helper.** Add a small typed operation, e.g. `PKHeX.Core/Editing/SpeciesFormChange.cs`. Its signature is along the lines of `SpeciesFormChangeResult ChangeSpeciesForm(this PKM pk, ushort species, byte form, IPersonalTable table)`. It returns `[Flags]` changed fields: EXP, Ability, Gender, PID, Nickname, Form.
- It keeps the level and never rerolls the PID, except the Gen 3 Unown form, which needs it.
- It does not touch UI, strings or lists. Before writing new logic, reuse `FormInfo`, `FormConverter`, `Experience`, `SetAbilityIndex`, `GetSaneGender` and `SetDefaultNickname`.
- Make R1 pass against the helper as well as the old path.

**R3 Route WinForms through the helper.** `UpdateSpecies`/`UpdateForm` call the helper and then refresh their controls from the returned flags. Control-only behaviour stays in WinForms: form combobox population, HaX, Gen 2 Unown IV loop, sprite. Rerun R1 and the full Core test suite. Also check by hand in WinForms (Windows): change species/form on Pumpkaboo, Meowstic, Unown (Gen 3) and a species with a different growth rate, then compare the before/after fields.

**R3 status:** code complete on `web/refactor-species-form-winforms`. The WinForms Release build succeeds on macOS, and the R1 tests pass. Two adversarial reviews were done; their fixes are in. **Not yet run in the WinForms UI** (gate G-E).

**R3 Windows checklist (G-E).** Use a build from before R3 (`web/main` at `a64f4f8ac`) and the R3 build side by side, on the same saves. For each case:
1. Make the change.
2. Record the fields named in the case.
3. Save, then reopen the file.
4. Compare the fields with the pre-R3 build.

"Same" means the same as the pre-R3 build. Anything marked *changed* is a deliberate difference; confirm that the new result happens.

| # | Save / entity | Steps | Expect |
|---|---|---|---|
| W1 | Any Gen 6+ | Change Pikachu → Magikarp → Chansey (different growth rates) | Level re-derived from the EXP on the new curve, EXP snapped to that level's minimum; level box and EXP bar match. Same. |
| W2 | Gen 6+ | Species change on a not-nicknamed mon, then on a nicknamed one | Name follows the species / nickname kept. Same. |
| W3 | Gen 6+ | Species change on a mon with its second ability, then its hidden ability | Slot kept. *Changed:* the hidden ability-number box (developer ability view) now matches the kept slot; before, it went stale. |
| W4 | Gen 4 (HGSS/Pt) | Onix → Bulbasaur (same abilities) → Magnemite | Ability picker index carried through; PID unchanged. Same. |
| W5 | Gen 6 (XY/ORAS) | Meowstic form ♂ ↔ ♀ | Gender follows the form. Same. |
| W6 | Gen 6, English and German UI | Pumpkaboo/Gourgeist size forms | Form changes; gender untouched. Same. In German, clicking gender changes the size form: that was already the case before R3 and is not a regression. |
| W7 | Gen 3 (RS/E/FRLG) | Unown form change | PID rerolled until it yields the form. *Changed:* the nature box, stat colours/tooltip and shiny display now update too. Reselecting the current form no longer rerolls the PID. |
| W8 | Gen 3 | "PIKACHU" with Nicknamed ticked → Raichu. Then an English "BULBASAUR" with the language set to French → Ivysaur | *Changed (accepted):* Gen 3 has no stored nickname flag, so the checkbox is not honoured. The first is renamed to "RAICHU"; the second keeps "BULBASAUR". |
| W9 | Gen 4 | Unown form change | PID unchanged. Same. |
| W10 | SV | Oinkologne or Meowstic, then Charizard, then click the gender toggle | No error dialog; form stays 0. Before R3 the form was briefly set to 1 (Mega stats shown). |
| W11 | SV, custom template = Charizard (template folder) | View Meowstic, load an SV save, load it again (the species stays the same), then click the gender toggle | No error dialog; form stays 0. This is the review case the fix covers after a save load. |
| W12 | ZA then SV | Meowstic on a Mega form in a ZA save, then load an SV save showing Meowstic and click gender | No error dialog; valid ♂/♀ form. |
| W13 | Gen 6+ party slot | Species change on a party member | Stats recalculated; HP/status as before. Same. |
| W14 | Gen 1/2, and HaX mode (any gen) | Species/form changes, including Gen 2 Unown | Old handler code path; everything the same. |
| W15 | Egg (Gen 4+) | Species change on an egg | *Changed:* the egg name is kept, not re-derived. |
| W16 | Any | Change UI language while a mon is loaded | Mon unchanged after the reload. Same. |

Record the results (pass, or the fields that differ) in the upstream PR text.

→ Open the upstream PR (R1–R3) once the R3 checklist passes. The PR text covers the rationale, confirms desktop behaviour is unchanged, and names the untested cases.

## Phase 2 — Web foundation + CI (Goal 3)

Branch `web/foundation` from `web/main`.

**F1 Promote the proof into a skeleton.** Commit `PKHeX.Web/` using the proposed folders: `Components/`, `Pages/`, `State/`, `Services/`, `Interop/`, `wwwroot/`.
- Split `ProofSession` (`PKHeX.Web/Services/ProofSession.cs`) into:
  - `State/SaveSession`: original bytes, working `SaveFile`, session ID, revision, `HasChangesSinceOpen`, `ExportedRevision`
  - `State/EditorDraft`: clone, source slot + revision, dirty flag
  - `Services/SaveLoader`: bounded open, copy before parse, family allowlist, integrity check
  - `Services/SaveExporter`: clone → `Write()` → reopen-validate
- Keep behaviour identical and keep the nickname-only UI for now.
- Commit `Tests/PKHeX.Web.Tests/`. Try replacing the linked `<Compile Include=…ProofSession.cs>` with a `ProjectReference` to `PKHeX.Web`. If the BlazorWebAssembly SDK blocks that, keep linked compile items for `State/` and `Services/`.
- `.gitignore` publish output.

**F1 status:** code complete on `web/foundation` (staged, not committed). Unit, synthetic E2E and RealSave tiers pass. `.gitignore` needed no change (`bin/` and `publish/` already cover the publish output). Compared with PKForge: copy-before-parse and `EntityImportSettings.None` match; the unchanged-round-trip check goes to M2 and the structural slot diff to M9.

**F2 Solution integration.** Add `PKHeX.Web` and `PKHeX.Web.Tests` to `PKHeX.slnx`, and remove `PKHeX.sln` so that `.slnx` is the only solution. Confirm they inherit from `Directory.Build.props` (C# 14, nullable) and fix any new nullable warnings. Pin `Microsoft.AspNetCore.Components.WebAssembly` to 10.0.12. Do not add a repo-wide `global.json`.

**F2 status:** code complete on `web/f2-solution-integration`. Both projects inherit C# 14 and nullable, and the Release build of `PKHeX.slnx` has 0 warnings. The Web projects now treat nullable warnings as errors: PKForge sets `TreatWarningsAsErrors` repo-wide, but here it covers nullable only and stays off Core/WinForms, and trim warnings are left to the F6 baseline. Adding `PKHeX.Web.Tests` to the solution made `dotnet test PKHeX.slnx` run the browser and real-save tests, which fail without their environment, so the tier tagging from F3 was pulled forward: every Web test carries `[Trait("Category", Unit|E2E|RealSave)]` (`TestCategory.cs`), and when no `--filter` is given `PKHeX.Web.Tests.runsettings` limits the run to `Unit`. It is applied only without a filter, because VSTest ANDs the two. A `TestCategoryTests` guard (itself Unit) fails if any test has no tier or more than one, so an untagged test cannot silently drop out of every run. The Unit-only default applies only when neither `--filter` nor `--settings` is given; a filter on the solution reaches every project, so exclusion filters such as `FullyQualifiedName!~X` pull in E2E/RealSave, and CI (F6) must always pass explicit `Category` filters. Removing `PKHeX.sln` goes beyond "no solution churn" (`PKHeX.Web.md` §upstream): the foundation PR text must call it out as a separate, revertible decision for maintainers (current Visual Studio and Rider open `.slnx`), and upstream edits to `PKHeX.sln` will conflict as modify/delete when rebasing. Upstream's Azure pipeline builds `PKHeX.slnx` on a Windows agent (`nuget.exe` restore, then Visual Studio MSBuild with `/p:Version=$(GitVersion.SemVer)`) on every push and PR, so after the foundation merges it also builds `PKHeX.Web` and `PKHeX.Web.Tests` with a toolchain we have not tested (`dotnet pack` skips them: `IsPackable` is false). A failure there breaks upstream's build, so F7 must prove it first.

**F3 Split test tiers.** Tagging and the Unit-only default landed in F2; F3 adds the fixture and tier-specific checks below.
- `Category=Unit`: session and naming tests with synthetic Core blank saves plus a test-only BEEF footer, as the proof does.
- `Category=E2E`: Playwright against the published output with synthetic fixtures.
- `Category=RealSave`: env-var driven. When this category is selected and a variable is missing, the test fails, not skips. (Since F6 the tier must also be opted in with `PKHEX_WEB_TEST_TIERS`; unless it is, it is skipped. See the F6 status.)
- CI runs only Unit and E2E.
- Move `StaticHost` + privacy assertions into a reusable `PublishedAppFixture`. It covers request interception, storage emptiness, root and `/PKHeX/`, and deployment-like headers (MIME, CSP header, `nosniff`).

**F3 status:** code complete on `web/f3-test-tiers`. `PublishedAppFixture` (an xUnit collection fixture) owns the static host, Playwright, and browsers cached per engine. `BootAsync(engine, prefix)` checks static-only boot requests and deployment headers on every boot response. Those are the MIME type per extension (an unmapped extension fails), `nosniff`, the CSP header and `Referrer-Policy`. Boot also fails on a request that got no response or on any CSP violation. Violations are reported by an init script to a context-level binding, so they survive reloads, and every engine is checked. Recording is swapped out atomically before the boot checks, so no request falls between the boot check and the after-boot check. `AssertStaticBootAsync` rechecks a reload as a second boot. `RealSaves` withholds the private path from IO errors and from `ToString`. `AppSession.AssertNoNetworkOrPersistenceAsync` checks that nothing reached the network after boot, that storage is empty and that no violations occurred. Cached browsers are replaced if they disconnect. Dialogs are still auto-accepted, but their types are recorded in `AppSession.Dialogs` for M16's `beforeunload` tests. A Unit test keeps the host CSP header equal to the `index.html` meta CSP plus the header-only `frame-ancestors`. The header check still exercises the test host, not a shipped deployment config; M20 should make `StaticHost` serve the checked-in `_headers` so the same assertions cover it. Boot timings come from a browser process reused across tests, so F8 must launch its own browser for cold-boot numbers. The proof-UI driver and byte oracles moved to `ProofPage`, which M1–M6 will replace. `RealSaves.Read` is the single RealSave entry point, with native preflight. `TestEnvironment.Required` makes a selected E2E/RealSave tier fail, not skip, without its inputs; this was verified with the variables unset. A `NoTestIsSkipped` guard forbids static skips, on facts and on data rows. The new E2E test `BootsWithDeploymentHeadersAndNoPersistence` (3 engines × 2 paths) also checks the base href. Tier counts: Unit 17, E2E 12, RealSave 14, all passing. Compared with PKForge: real saves likewise come only from env vars and are never committed. PKForge returns early and passes when a fixture variable is missing, and CI runs all its tests untiered; we are deliberately stricter. PKForge has no browser or hosting tests, so there is nothing to adopt.

**F4 Browser file interop.** Add `Interop/BrowserFileService` + `wwwroot/browser.js`:
- single-file picker with the 16 MiB limit enforced while streaming
- file-only drop that rejects multiple files, directories and URL/text drops, and prevents navigation
- Blob download with delayed object-URL revoke

Add `Services/FileNaming`, which sanitises separators and control characters, caps length, and preserves the extensionless `main` (WEB-SESSION-007). Unit tests cover the naming rules. E2E covers picker and drop.

**F4 status:** code complete on `web/f4-browser-file-interop`.
- **Picker and drop.** `Components/SaveFilePicker` wraps the `InputFile` in a drop zone. A valid drop is handed to the same `<input type="file">` through a `change` event, so picked and dropped files take one read path: `BrowserFileService.ReadAsync`. That checks the declared size and then counts bytes while streaming (`ReadBoundedAsync`). Every read failure except cancellation is reported as `ReadFailed`, never as a page error. Choosing the same file again (a retry) raises a new `change`, because Blazor's `InputFile` clears the input on every click, including keyboard activation.
- **Busy state.** `App` stays busy from the start of a read until its result arrives.
- **Typed outcomes.** Reads return `FileReadStatus` (`Ok`/`Empty`/`TooLarge`/`ReadFailed`), and refused drops report `DropRejection` (`MultipleFiles`/`Directory`/`NotAFile`/`Busy`). Neither carries display text, ready for M2's taxonomy.
- **Drop zone.** It always accepts the drag (`dropEffect = 'copy'`), because `'none'` would cancel the drop before a refusal could be explained. If registering the zone fails, the page falls back to the picker alone, and a failed `browser.js` import is retried on next use.
- **Navigation guard.** `drop-guard.js` is a classic script placed before the Blazor boot script in `index.html`, so it runs before the WASM app starts. It blocks file drops everywhere outside the zone. It blocks link and text drops everywhere except enabled, writable text inputs, textareas and contenteditable elements, so checkboxes and buttons are covered and text editing still works. A single file with extra string items (the `file://` uri-list file managers add) is accepted, because nothing is fetched from it.
- **Lifecycle code moved.** `setDirty`, `beforeunload` and `pageshow` moved to `lifecycle.js` for M16, with braces added and no change in behaviour.
- **File name.** `SaveSession.FileName` keeps the name the file was opened as (default `main`), and downloads use it.
  - `FileNaming.Sanitize` is idempotent, because names are sanitised on read, on load and again on download. It also strips leading dots, which every engine removes when saving.
  - The UI shows the name as a suggestion, because browsers can still adjust it: Firefox collapses spaces and renames `.lnk`, and Chromium replaces a leading `~`.
- **Tests.**
  - Refusals of multiple files and non-files are covered with synthetic drops; link drops also with a real Playwright drag. A mutation check confirmed the real-drag test fails with the old `'none'` drop effect.
  - A real folder drop and drags from the OS cannot be scripted. Directory refusal is tested by calling `classifyDrop` with mock entries, and the guard through `defaultPrevented`. A real drag of a folder, two files, a link and nickname text is a manual check.
  - Fuzzing `Sanitize` with 300k generated names found no case that broke idempotence, the length cap, the character rules or the device-name rule.
  - Tier counts: Unit 72, E2E 24, RealSave 14, all passing. The diagnostic trim publish still reports the same 38 warnings.
- **Adversarial review.**
  - Fixed: unexplained link/busy refusals, the checkbox navigation hole, non-idempotent naming and the length cap, uncontained read exceptions, the boot-time guard gap, drag-over flicker and a vacuous test assertion.
  - Second review fixed: leading-dot names offered under a different name than the one saved, a cached failed module import, the guard's load order, drag-over flicker in WebKit, stale xmldoc and brace-less JS.
  - Second review, rejected: that choosing the same file again does nothing. `InputFile` already clears the input on click, so the extra reset that was added for it has been removed.
  - Third review fixed: refusal messages no longer say a session was kept when none is open, and a failed rejection callback no longer surfaces as an unhandled promise rejection.
  - Not changed: re-entrant drops in the moment before re-render. Disabling the input from JS would get out of step with Blazor's rendered `disabled` state.
  - Not changed: the 2× peak copy of a 16 MiB read. It is the same as before and goes to M21's memory measurements.
- **Compared with PKForge:**
  - Matches the streamed size cap with a zero-byte reject (`AndroidEmulatorScanner`, 32 MiB), and keeping the display name apart from the bytes.
  - Deliberately stricter on naming. `BankArchive.SanitizeFileName` uses `Path.GetInvalidFileNameChars`, which strips only `/` and `\0` on Unix/WASM. `FileNaming` also removes control, format and line-separator characters (bidi overrides) and unpaired surrogates, replaces Windows-reserved characters, trims trailing dots and prefixes device names.
  - It caps at 120 characters and keeps the extension, where PKForge cuts at 20. It never rewrites the extension; PKForge exports `{base}-modified{ext}` with `.sav` as the default, which would break the extensionless `main`.
  - The edited-name suggestion and the console-rename warning go to M6.
  - Android document picking, folder scans and emulator roots are out of Web scope. PKForge has no drag-and-drop to compare.

**F5 Build provenance.** MSBuild embeds the Web version + Core git commit as assembly metadata. `THIRD-PARTY-NOTICES.md` for Web lists runtime packages and licenses (WEB-SEC-004). CI later prints a `dotnet list package --include-transitive` inventory.

**F5 status:** code complete on `web/f5-build-provenance`.
- **Provenance.** The `AddBuildProvenance` target in `PKHeX.Web.csproj` embeds `PKHeXWebVersion` (the repository `Version`, since Web ships with the same release as Core) and `PKHeXSourceCommit` as `AssemblyMetadata`.
  - The commit is the `RevisionId` of the git `SourceRoot` from the SDK's built-in `Microsoft.Build.Tasks.Git`, so no `git` executable is needed. The repository holds both Core and Web, so one commit identifies both.
  - It is used only when that root is this repository's root. Otherwise a copy inside another repository records the outer repository's commit; this was reproduced, and it now records `unknown`.
  - `SourceRevisionId` is not used: the shared `Directory.Build.props` sets it to a build timestamp and stays untouched.
  - Without `.git` the commit is `unknown`; `-p:PKHeXSourceCommit=<sha>` overrides it. The commit is the checked-out one; uncommitted changes are not reflected, and the notices and README say so.
  - Worktrees, shallow clones and detached HEAD record the right commit, and a new commit regenerates the assembly info.
- **Reading it.** `Services/BuildInfo` reads the metadata (missing or blank → `unknown`). The page footer shows `PKHeX.Web {version} · {12-char commit}` with the full commit as its title; M1's About panel replaces it. The footer is how E2E proves the trimmed publish keeps the metadata. CoreLib's link attributes do not remove `AssemblyMetadataAttribute`.
- **Notices.** `PKHeX.Web/THIRD-PARTY-NOTICES.md` lists every package in the restore graph with its exact version and license, in three tables:
  - **Published:** at least one file in the publish. `blazor.webassembly.js` comes from `Components.WebAssembly`; `Internal.Assets` carries an identical copy and is listed with it.
  - **Removed by trimming:** restored for the browser, no file published.
  - **Build-only:** Analyzers, ILLink.Tasks, Sdk.WebAssembly.Pack.

  It includes the .NET MIT license verbatim, the upstream repository URL for the corresponding source, and a pointer to the build instructions. It makes no licensing statement beyond the repository `LICENSE` and Core's declared `GPL-3.0-or-later`.

  Publish copies the notices, the repository `LICENSE` (as `LICENSE.txt`) and the four distinct upstream `THIRD-PARTY-NOTICES` files of the published packages (runtime pack, ASP.NET Core, Components, Extensions) into `wwwroot`, the latter under `licenses/`. Their package directories come from the resolved assets, not from a version property. The build fails if any directory or file is missing, which also keeps the wildcard from matching the project's own notices file.
- **Tests.**
  - Unit: `BuildInfoTests` checks the commit equals `git rev-parse HEAD` (a stale `--no-build` run fails on purpose, with a message), the version, the `unknown` fallback and abbreviation. `ThirdPartyNoticesTests` checks every restored package is listed once with its version and no stale rows, that only published rows name a notices file, that every package mapped to a notices file carries exactly that file upstream, and the published-name mapping.
  - E2E: the boot test checks the footer on 3 engines × 2 paths. `PublishesLicenseAndNotices` checks, byte for byte, the published license, the notices and the exact set of upstream notice files. It also maps every `_framework` file (fingerprint stripped, `.wasm` → `.dll`) to the restored packages containing a file of that name. Each file must belong to a package listed as published, each published package must have a file, and no trimmed package may have one.
  - Mutation checks all failed as expected: a dropped row, a wrong version, a wrong mapping, a published package moved to build-only or to trimmed, and a trimmed package listed as published.
  - Tier counts: Unit 87, E2E 25, RealSave 14, all passing. The diagnostic trim publish still reports the same 38 warnings, none from PKHeX.Web. The Release build of `PKHeX.slnx` has 0 warnings.
- **Adversarial review.**
  - Fixed: the publish was not checked against the tables; "shipped" wording contradicted the tables; an outer repository's commit could be recorded; the notices overclaimed on corresponding source (dirty trees, no URL), on embedded data licensing and on "or later" for Web; the notices paths were coupled to one package version; the git test crashed without `git`; prerelease versions could not be parsed.
  - Documented, not changed: the runtime pack, ILLink and WebAssembly pack versions come from the installed SDK (no `global.json`, per F2), so another SDK patch needs a notices update, which the Unit tier reports.
  - Not changed: the notices are not linked from the UI until M1. `.md`/`.txt` are not in the test host's MIME map, which will need entries when M1 links them. `web.config` lands outside `wwwroot`, which is not deployed.
- **Not done here.** Reproducible builds: the shared timestamp `SourceRevisionId` stays in `InformationalVersion`, and deterministic output is F6/M20 territory. A dirty-tree marker: the SDK git task does not report one, and CI builds from clean checkouts. The CI `dotnet list package --include-transitive` print is F6.
- **Compared with PKForge:** PKForge embeds no build provenance and has no notices file; it credits its art and PKSM UI in its README. Nothing to adopt. Its credited art and network assets are out of Web scope (M5, gate G-B).

**F6 CI workflow.** Add `.github/workflows/web.yml`:
- runs on `ubuntu-latest`, triggered by `pull_request` + `push`, with `permissions: contents: read` and actions pinned to commit SHAs
- setup-dotnet 10.0.401 → restore → Core tests → Web Unit tests → `dotnet publish -c Release` → diagnostic publish with `SuppressTrimAnalysisWarnings=false`, diffed against a checked-in `PKHeX.Web/trim-warnings.baseline.txt` (fails on new warnings)
- static artifact checks: no `.map`/source/fixtures, file count and max asset size under the Cloudflare limits (20k files, 25 MiB)
- size report (raw/br/gz of the largest assets); Playwright browser install (cached) → E2E
- uploads the publish output, test results and size/warning/license reports
- no secrets and no `pull_request_target`

**F6 status:** code complete on `web/f6-ci-workflow`. Not yet run on GitHub; the first push to the fork is the first real run.
- **Workflow.** `.github/workflows/web.yml` has one `ubuntu-latest` job. It runs on `pull_request`, and on `push` to `master` and `web/main` only (feature branches are covered by their PRs, so nothing runs twice), both path-filtered to Core, Web, their tests, `Directory.Build.props`, `LICENSE`, `PKHeX.slnx`, `.editorconfig`, `.gitattributes` and the workflow itself. It has `contents: read`, `persist-credentials: false`, `shell: bash` (which adds `pipefail`), per-PR concurrency that cancels superseded PR runs but never cancels runs on merge targets, a 45-minute timeout, and actions pinned to commit SHAs (checkout v7.0.1, setup-dotnet v6.0.0, cache v6.1.0, upload-artifact v7.0.1). `setup-dotnet` alone does not pin the SDK: without a `global.json`, `dotnet` uses the newest one on the runner, and an image update would move the runtime pack and ILLink past the versions in the notices. So a `global.json` with `rollForward: disable` is written above the checkout, and the step fails unless `dotnet --version` is 10.0.401. There is still no repository `global.json` (F2). It restores the two test projects, not the `.slnx`, because WinForms targets `net10.0-windows`. Then it runs Core tests, Web Unit, publish, the trim baseline, the package inventory, the size report, the Playwright install and E2E. After Restore, each step runs whenever its inputs exist (`!cancelled()` plus the outcome of Restore, Publish or the Playwright install), so a Core failure or a trim diff does not hide the Web tiers, and the `wwwroot` is uploaded even when E2E fails. The NuGet cache is keyed on the SDK version and project hashes, the Playwright cache on the test project. `install --with-deps` always runs, because the cache does not hold system libraries. It uploads the `wwwroot` artifact, plus the trx results and reports even on failure.
- **Upstream Core test.** `EffortExpLegalityTests.ZeroEVs_ReturnsZero` fails on `upstream/master` itself. Upstream's `9c170d99c` returns `-gainedEXP` when vitamins explain every EV, as its xmldoc documents, but the test still expects 0. CI excludes that one test by name, with a comment. The fix is on `web/fix-effortexp-test` (from `upstream/master`, test only) for an upstream PR. Drop the exclusion once it is merged.
- **Trim baseline.** `PKHeX.Web/tools/trim-warnings.sh` publishes with `SuppressTrimAnalysisWarnings=false` and a warnings-only file logger. It normalises each warning to `ILxxxx: message`: the source location and project suffix are dropped, so Core line shifts do not matter; the ordinals in compiler-generated names (`>b__16_0`, `>d__69`, `<>c__DisplayClass5_0`) become `N`, because adding a member earlier in the type renumbers them; CR is stripped for Windows logs; duplicates are kept. It compares against `PKHeX.Web/trim-warnings.baseline.txt` (38 lines: IL2026 ×20, IL2070 ×9, IL2046 ×3, IL2104 ×2, IL2065, IL2075, IL2098, IL2111, as recorded by the proof). Any difference fails, removals included, so the baseline stays exact and, while it is not empty, a run that reported nothing cannot pass. `--update` rewrites it; `--report` checks its argument before the publish. A publish repeated straight after another still re-runs ILLink and reports all 38, so no forcing was needed; that relies on `Directory.Build.props` stamping a timestamp `SourceRevisionId`, and if that ever goes, the script fails closed (0 ≠ 38).
- **Artifact checks.** These are the E2E test `PublishesOnlyStaticDeployableFiles`, not YAML, so they run locally too. It enforces ≤ 20,000 files, each ≤ 25 MiB, and an allowlist by folder taken from the actual publish: `_framework/` holds `.wasm .js .dat`, `licenses/` holds `.txt`, and the root holds `.html .css .js` plus `LICENSE.txt` and `THIRD-PARTY-NOTICES.md`; no other folder is allowed. Every `.br`/`.gz` must sit next to the asset it compresses. Mutation checks failed as expected: a `.map` file, a 26 MiB file, an extensionless `main`, an orphan `.css.br`, a root `save.dat`, a root `notes.txt` and `_framework/sub/a.wasm`. M20's `_headers` (and any `_redirects`) will need adding to the root list.
- **Reports.** `PKHeX.Web/tools/size-report.sh` prints totals and the largest assets with their Brotli/gzip sizes. Currently that is 189 files and 27.68 MiB raw; `PKHeX.Core` is 17.85 MiB raw, 2.88 MiB Brotli. The report goes to the step summary. The table is cut with `awk` rather than `head`, so `sort` cannot die of SIGPIPE under `pipefail` on a large publish (reproduced with 3,000 files). Both scripts are portable bash (macOS 3.2 and GNU) and shellcheck-clean, and `.gitattributes` now keeps `*.sh` LF for Windows checkouts; the workflow is actionlint-clean and indented per `.editorconfig`.
- **Adversarial review.** Also run in a Linux `dotnet/sdk:10.0.401` container on a shallow clone: Unit, publish, trim (38) and the size report matched, and `BuildInfoTests` passes on a depth-1 clone.
  - Fixed: the unenforced SDK pin; missing `pipefail`; the size report's SIGPIPE; ordinal churn in the baseline; later results hidden by an early failure and no `wwwroot` upload on E2E failure; duplicate push+PR runs and cancelled merge-target runs; an allowlist that ignored folders; `--report` without a path; CRLF logs; stray `DOTNET_SKIP_FIRST_TIME_EXPERIENCE`; field placement in the test class; 2-space YAML against `.editorconfig`.
  - Confirmed fine: YAML anchors in workflows, the pinned SHAs against their tags, `pwsh` and passwordless sudo on the runner, `--no-build` E2E after the publish rebuilds, provenance on the PR merge ref, Linux case sensitivity of the notices, and no private data in the uploads.
  - Not changed: the exclusion filter names the old test, so after the upstream fix (which renames it) it silently matches nothing; the workflow comment says to drop it.
- **Opt-in browser tiers.** F2's Unit-only default comes from `RunSettingsFilePath`, which only `dotnet test` and Visual Studio read. `vstest.console` on the built assembly, which is what Azure's VsTest task runs, ignores it. `dotnet vstest PKHeX.Web.Tests.dll` reproduced the result: 40 E2E/RealSave failures. Upstream's pattern matches nothing today, but fixing it (see G-A) would turn its build red. So E2E and RealSave are now also opt-in:
  - They run only when named in `PKHEX_WEB_TEST_TIERS` (e.g. `E2E,RealSave`; case-insensitive), and are otherwise skipped with a reason, through `TierFactAttribute`/`TierTheoryAttribute`. `PublishedAppFixture` is unchanged. An unfiltered run still creates it and its setup throws, but xUnit 2.9.3 does not report a fixture failure against skipped tests (the review confirmed this with a marker file).
  - The same `dotnet vstest` run now gives 98 passed, 8 skipped (one per opt-in test method), 0 failed.
  - An opted-in tier with missing inputs still fails, not skips (F3). A filter without the opt-in does skip everything and reports success; that is the trade-off for being safe under runners that ignore the filter. CI closes the gap with `PKHeX.Web/tools/trx-all-executed.sh`, which fails the E2E step's results unless every counted test was executed.
  - Guards (Unit):
    - `OptInTiersUseTheMatchingTierAttribute` requires the tier attribute to match the class's category. Unit tests must use exactly `[Fact]`/`[Theory]`, and any other test attribute is rejected, because a custom subclass could skip unnoticed.
    - `NoTestIsSkipped` became `NoTestIsSkippedExceptByTheOptIn`. It still reads the run-time `Skip` of every test and data attribute, so skips set in a constructor are caught. The only skip it allows is a tier attribute's own opt-in reason.
    - `IsOptedIn` parsing is tested as a pure function, so no test mutates the process environment.
  - Known limit (unreproduced; the review put its confidence at about 30%): a runner that discovers without the opt-in and executes with it in a separate process, such as some IDE flows, would run a not-enumerated theory with no arguments. `dotnet test`, `vstest.console` and CI discover and run in one process.
- **Tests.** Running the workflow's steps locally in order: Core 798 passed plus 1 existing skip, with the exclusion; Unit 98 (87, less the renamed guard, plus 12 new cases: the tier-attribute guard, the renamed skip guard and 10 opt-in parsing rows); E2E 26 opted in, and the TRX check passes on them; RealSave 14 opted in. The `PKHeX.slnx` Release build has 0 warnings.
- **Compared with PKForge:** its `build.yml` also runs on `ubuntu-latest` with restore → `--no-restore` test. We are deliberately stricter:
  - it floats `10.0.x`; we pin 10.0.401, which the notices tests need
  - it pins actions by tag; we pin to SHAs
  - it grants `contents: write` to the whole job; we grant `contents: read` only
  - it runs its tests untiered; we select tiers explicitly
  - it has no publish, artifact or trim checks
  Its signing, release and APK steps are out of Web scope; a deploy job is M20. Nothing to adopt.

**F7 Azure-parity Windows job.** Upstream's Azure pipeline already builds `PKHeX.slnx` Release on Windows (see G-A and F2), so a plain Windows build would duplicate it. Instead, add a `windows-latest` job to `web.yml` that reproduces the Azure steps we cannot run ourselves:
- `nuget.exe` restore of `PKHeX.slnx`
- a Visual Studio MSBuild build of `PKHeX.slnx` in Release with `/p:Version=<semver>`
- `vstest.console.exe` on `**\bin\Release\**\*Tests.dll`, which is the corrected pattern, without `PKHEX_WEB_TEST_TIERS`

It must show:
- the Web projects build under that toolchain
- Core tests pass there
- Web Unit passes, and E2E/RealSave are skipped rather than failing (the F6 opt-in)

Match the agent as closely as possible. Build 9738's logs show:
- image `windows-2025` (pin that, not `windows-latest`)
- Visual Studio 2022 17.14 MSBuild, x86 (`msbuildArchitecture: x86`)
- `BuildPlatform` "Any CPU"
- GitVersion with `/updateassemblyinfo`
- the agent's floating SDK, 10.0.111 (runtime 10.0.11) at the time
- `NETSDK1233` ("Targeting .NET 10.0 or higher in Visual Studio 2022 17.14 is not supported") 18 times, so the Web projects would be built with an unsupported toolchain

Known risk: two Unit tests depend on the runtime pack that the SDK brings, and would fail on that agent (inferred from the code, not yet run on Windows):
- `ThirdPartyNoticesTests.ListsEveryRestoredPackageOnceWithItsVersion` (version mismatch)
- `ThirdPartyNoticesTests.PublishedPackagesNameTheNoticesThatCoverThem` (the 10.0.12 runtime pack directory is not restored)

Decide in F7 whether those checks move to an opt-in tier, read the versions from the SDK, or are left to the Linux job. This guards Phase 1, the solution edits and the foundation PR's effect on upstream's build.

**F7 status:** code complete on `web/f7-azure-parity`. The first GitHub run (PR #10, run 36339720009) found the runner image mismatch below; the fix awaits the second run.
- **What Azure does.** Build 9738's public logs (`dev.azure.com/project-pokemon/PKHeX/_apis/build/builds/9738/logs`) give the exact steps:
  - image `windows-2025` 20260907.255.1
  - GitTools `gitversion/setup` + `execute` v3.2.1 with GitVersion.Tool 5.12.0 (`versionSpec 5.x`) and `/updateassemblyinfo`, giving SemVer `26.8.27`
  - NuGet tool installer `>=7.0.0`, which downloads the newest release (7.9.0 then; the image's copy happened to be the same), and `nuget.exe restore PKHeX.slnx`
  - Visual Studio build: x86 `MSBuild\Current\Bin\msbuild.exe` 17.14 with `/nologo /nr:false /p:Version=… /p:platform="Any CPU" /p:configuration="Release" /p:VisualStudioVersion="17.0"`, not parallel
  - VsTest with `**\Release\*test*.dll,!**\obj\**`, which finds no sources
  - `dotnet pack` of PKHeX.Core only

  **Two SDKs are in play.** Visual Studio MSBuild resolves the newest SDK that VS 17.14 supports, 10.0.111 (runtime 10.0.11), while the `dotnet` CLI on the same agent picks 10.0.400. NETSDK1233 appears 18 times in the console log: 6 projects (Core, three Drawing, WinForms, Core.Tests), each shown three times.
- **First run: image mismatch.** On GitHub, `windows-2025` is now the Visual Studio 2026 image (`windows-2025-vs2026`; VS 18.10.1). Its MSBuild resolved SDK 10.0.401, and it has no VS 17 `vstest.console`, so the test step stopped at its guard (whose message was split by bash-style quote escaping, now fixed). The Linux `web` job passed, moved notices tests included. Restore and build of the whole solution, Web included, succeeded there, which is useful evidence for when Azure moves to VS 2026. Azure's `windows-2025` agent has not moved: build 9754 (2026-09-27, image 20260922.270.2) still used VS 2022 17.14 and SDK 10.0.112, with the same 18 NETSDK1233 lines. GitHub's `windows-2022` image has VS 2022 17.14 and SDK 10.0.112 as well, so the job now runs there; only the Windows Server version differs from Azure.
- **Job.** The `azure-parity` job in `web.yml` repeats those steps on `windows-2022` (see above), with pwsh steps in place of the classic tasks:
  - Checkout uses `fetch-depth: 0`. A fork has no release tags, so on anything other than `kwsch/PKHeX` it first fetches upstream's tags; otherwise GitVersion would count from 0.1.0. On pull requests GitVersion reads the merge ref, so versions are prereleases (probably `…-PullRequestN.x`; confirm from the toolchain report), where Azure's master builds are not.
  - There is deliberately no `setup-dotnet` or SDK pin. `DOTNET_SDK_VERSION` moved from workflow to `web`-job env so it does not appear to pin this job.
  - `NuGet/setup-nuget` v4.0 with `>=7.0.0`, as Azure's installer.
  - `microsoft/setup-msbuild` v3.0.0 with `msbuild-architecture: x86` (pinned by SHA, like the GitTools actions).
  - The toolchain report goes to the step summary. It lists VS, MSBuild, the SDK MSBuild resolved (read from the build log), the CLI SDK, NuGet, the version and the projects that report NETSDK1233. It warns, rather than fails, when VS is not 17.14 or the SDK is not 10.0.1xx, because the image drifts under Azure too.
  - `vstest.console.exe` (found with `vswhere`, VS `[17.0,18.0)`, as the VsTest task does; the step fails clearly if there is none or the pattern does not find exactly the two test assemblies) runs over `**\bin\Release\**\*Tests.dll` without `PKHEX_WEB_TEST_TIERS`, with the same `ZeroEVs_ReturnsZero` exclusion as the Linux job.
  - Then `PKHeX.Web/tools/trx-check-opt-in-skips.ps1` fails unless the run outcome is `Completed`, every result has a test definition, no test failed, Core and Web each passed at least one test, and every Web test that did not run was skipped with the opt-in reason (with at least one such skip).
  - The TRX, the text build log and the binlog are uploaded.
  - Azure's artifact and `dotnet pack` steps are left out; they do not build Web.
  - The workflow paths gain `PKHeX.WinForms/**` and `PKHeX.Drawing*/**`, because this job builds the whole solution.
- **Runtime-pack tests.** The two notices checks moved to the E2E tier, in the new `NoticesRestoreGraphTests` (a `[TierFact(E2E)]` class without the published-app fixture). The notices only matter for a publish, and CI runs E2E on the pinned SDK. `MapsPublishedNamesToPackageFileNames` stays Unit. The notices file, the README, the csproj comment and the `TestCategory.E2E` xmldoc now say so. A mutation (one version changed in the notices) fails both moved tests when opted in.
- **Tests.**
  - Unit 96 (98 less the 2 moved) and E2E 28 (26 plus the 2 moved); the TRX check passes on them.
  - `dotnet vstest` on the Web DLL without opt-in: 96 passed, 10 skipped (the opt-in methods, now including the 2 moved tests), 0 failed.
  - The `.ps1` check passes on that TRX. It fails on a failed Web test, on a Web skip with another reason, on a Web DLL with no skips, on a missing assembly and on a missing file.
  - The workflow is actionlint-clean.
- **Not verified yet** (needs a run on `windows-2022`):
  - whether `nuget.exe` 7.9 and VS 17.14 MSBuild restore and build the BlazorWebAssembly project, including the runtime pack download and ILLink/WebAssembly tasks under .NET Framework MSBuild
  - whether `AddUpstreamNotices` and build provenance work there
  - whether the Web Unit tier passes on SDK 10.0.112
  - the NETSDK1233 count, which should be 8 projects once Web and Web.Tests are in the solution
- **Adversarial review.**
  - Fixed: NuGet taken from the image instead of Azure's `>=7.0.0` installer; the README CI section missing the Windows job; an unclear failure when no VS 17 `vstest.console` exists; `DOTNET_SDK_VERSION` visible to the unpinned job; comments overstating native-command handling and parity; TRX results without a definition ignored and the run outcome unchecked; the script not executable; the SDK regex relying on NETSDK1233 lines.
  - Accepted: WinForms/Drawing-only changes now also run the Linux job; the restore-graph notices check now runs only after a successful publish and Playwright install, and plain local `dotnet test` no longer reports notices drift (F5's "reported by the Unit tier" is superseded).
  - Open until the first run: vstest.console 17.14 with `Microsoft.NET.Test.Sdk` 18.0.1; nullable diagnostics from 10.0.111's compiler (Web treats them as errors); GitTools v3.2.1 declaring Node 20.
- **Compared with PKForge:** it has no Windows job, and its CI has no second toolchain to match. Nothing to adopt.

**F8 Performance baseline (WEB-PERF-001).** A script (`PKHeX.Web/tools/measure.*` or a test) records cold and warm boot under Playwright network throttling (20 Mbps/50 ms) plus artifact sizes, and uploads them as a CI artifact. There is no pass/fail threshold yet.

**F8 status:** code complete on `web/f8-perf-baseline`. Not yet run on GitHub.
- **Tier.** A new opt-in tier, `Perf`, with a single test, `BootBaselineTests.RecordsBootBaseline`.
  - It sits in a collection with parallelization disabled, so it never runs alongside other browser tests.
  - It needs `PKHEX_WEB_PUBLISHED` and `PKHEX_WEB_PERF_REPORT` (the output directory), and fails without them, like the other tiers. `PKHEX_WEB_PERF_RUNS` is optional (default 5, validated).
  - It writes `boot-baseline.md` and `boot-baseline.json`.
  - It fails only if a boot cannot be measured: the shell is not usable within 60 s, a page error occurs, or a response is anything other than a GET for a published file (a favicon 404 is allowed). The error names the boot, e.g. `chromium (20 Mbps / 50 ms) run 0 cold`.
  - It has no timing threshold.
- **Measurement (`BootBaseline`).**
  - **Configurations.** Chromium runs throttled to 20 Mbps / 50 ms, set per page before navigation through CDP `Network.emulateNetworkConditions`, which adds the latency to each request. Chromium, Firefox and WebKit also run on unthrottled loopback, because Playwright can throttle only Chromium.
  - **Samples.** Each configuration boots once unrecorded, then N times, each on its own temporary browser profile. The cold boot launches a new browser process on the empty profile (which meets the F3 note). The warm boot closes that browser and relaunches it on the same profile, like a returning visit, so only what the browser stored in the profile carries over, not in-process caches.
  - **"Shell ready"** is `performance.now()` when `#save-file` first exists and is enabled, recorded by a `MutationObserver` init script. DOMContentLoaded comes from navigation timing.
  - **Traffic.** Requests, 304s and body bytes per encoding come from the host's own log, so they are the same for every engine.
  - **Build.** The version and commit are read from the published page's footer, not from the test assembly. M1 must update this together with the E2E footer check when it moves the build details.
  - **Machine.** It records the CPU model, logical CPUs, memory available to .NET, OS, architecture, browser versions (Playwright's headless builds) and the Playwright version.
- **Host.** `StaticHost` now serves requests concurrently, for every user of the host, and gained an opt-in `deploymentCaching` mode; E2E still uses the default mode. The mode:
  - serves the `.br`/`.gz` sibling by `Accept-Encoding` (Brotli, then gzip), with `Vary`
  - sends a strong `ETag` per representation and answers `If-None-Match` with 304
  - sends `immutable` for fingerprinted `_framework` files (a name heuristic) and `no-cache` otherwise, which is M20's planned `_headers` policy. M20 should replace the heuristic with the checked-in file.

  Warm boots make only 7 requests, and that is configured by the publish. The .NET 10 boot manifest embedded in `dotnet.js` marks the 52 fingerprinted resources `"cache": "force-cache"`, which the loader uses instead of its `no-cache` default. So only the 7 unfingerprinted files are revalidated: `index.html`, `app.css`, the three root scripts, `blazor.webassembly.js` and `dotnet.js`. The host's `immutable` header therefore does not change the measured warm boots; its ETag/304 answers matter for those 7 files. For M20, the cached start depends on revalidating those 7 files cheaply, and the cold start depends on compressed delivery.
- **Resource cost.** WEB-PERF-001's "resource cost" is read as the cost of Core's embedded data, from `PKHeX.Web.md`'s risk table ("Embedded data/startup cost … Core resources … trimming does not automatically remove individual embedded resources"). The report lists PKHeX.Core's embedded resources by folder, with their raw size and their share of the published `PKHeX.Core` file: 1731 resources, 11.68 MiB, 65% of its 17.85 MiB (text 4.91, legality 4.47, byte 1.97, localize 0.33). Every boot downloads all of it. CPU and main-thread cost appear only as the loopback shell time; memory belongs to M21 (PERF-002). Any reduction belongs to PERF-005 (Post-MVP), not F8.
- **Report.** It gives median and min–max per configuration and phase, with the `PKHeX.Web.md` targets alongside (≤5 s cold, ≤2 s cached, for the throttled profile only, marked not enforced). It also lists the boot set: the files a cold boot fetched, with raw, Brotli and gzip sizes on disk, which complements the whole-publish `size-report.sh`. It flags any warm boot that reused nothing from the cache.
- **CI.** A `Web boot baseline` step in the `web` job, after the E2E check, runs `--filter Category=Perf` with the opt-in, then `trx-all-executed.sh` on `web-perf.trx`, and appends the Markdown to the run summary. The report lands in `$REPORTS_DIR/perf`, inside the `pkhex-web-results` artifact, which is uploaded `always()`. The report is written only after every boot succeeds, so a failed run uploads the TRX and its error but no partial report. That is accepted: a baseline with missing configurations would be misleading, and the error names the boot that failed.
- **Local baseline** (Apple M4, 10 logical CPUs, 16 GiB, macOS 26.5; Chromium 153.0.8010.12, Firefox 155.0, WebKit 26.6, headless; Playwright 1.63.0; 5 runs, median):

  | Engine | Network | Cold shell | Warm shell | Cold transfer |
  |---|---|---:|---:|---:|
  | Chromium | 20 Mbps / 50 ms | 2731 ms | 483 ms | 5.35 MiB (br), 59 requests |
  | Chromium | loopback | 245 ms | 238 ms | 5.35 MiB (br) |
  | Firefox | loopback | 270 ms | 267 ms | 5.35 MiB (br) |
  | WebKit | loopback | 232 ms | 233 ms* | 7.77 MiB (gzip) |

  The boot set is 59 files, 25.44 MiB raw / 5.35 MiB Brotli / 7.77 MiB gzip, of which `PKHeX.Core` is 17.85 / 2.89 / 4.73 MiB. Both throttled targets are met on this machine with room to spare. It is not the named reference desktop, so this makes no PERF-001 verdict; M21 owns that.

  \*Playwright's WebKit does not accept Brotli from the plain-HTTP loopback host. In every local run its warm boot re-downloaded all 59 files without revalidating, even after relaunching on the same persistent profile, so the cause is not the ephemeral context. The report flags it. The cause has not been established, and it says nothing about Safari (G-C).

  In a single-run mutation check made before the concurrency fix, with caching off, the throttled cold boot served raw bytes (25.44 MiB) and took 11.2 s. The warm Chromium boot re-downloaded 18.29 MiB in 10 requests, and warm Firefox 0.44 MiB in 9.
- **Rejected: throttling on the host for Firefox and WebKit.** `StaticHost` could delay and rate-limit its own responses, but that would measure our shaper against each engine's connection handling rather than a browser's network emulation, and it would not be comparable with the Chromium row. The plan asks for Playwright network throttling. The profile is therefore stated for Chromium only; the other engines' loopback rows show their startup cost, and their network behaviour is left to the real-device runs in G-C.
- **Tests.**
  - **Unit 138**, up from 96:
    - 2 `IsOptedIn` rows for Perf
    - 25 `StaticHostTests` (fingerprint rule, encoding choice, `If-None-Match`)
    - 15 `BootBaselineReportTests` (median, Markdown, the no-reuse flag, JSON round trip, run-count parsing, Core resource grouping)
  - **E2E 30**, up from 28: `StaticHostServingTests` exercises the caching mode over HTTP (including the 304 and the rewritten subpath page) and the default mode unchanged. Those two tests open a loopback `HttpListener`, which on Windows needs administrator rights for a `127.0.0.1` prefix, so they are opt-in E2E rather than Unit. That keeps `dotnet test PKHeX.slnx` working for non-admin Windows contributors. Every E2E test executed.
  - **Perf 1** (about 1 min 16 s for 5 runs).
  - **`dotnet vstest` without the opt-in:** 138 passed, 13 skipped (the opt-in methods: 10 as before, plus Perf and the 2 host-serving tests), 0 failed. `pwsh` is not installed locally, so the `.ps1` check was not run here; its reason regex already accepts `Perf`. The azure-parity job will run the new Unit tests on Windows; none of them opens a listener.
  - **Mutations:**
    - With the shell selector changed, the test fails after the 60 s timeout, naming the boot, instead of hanging.
    - With caching off, the host sends no 304s and the warm rows re-download (see above).
  - **Other checks:** `actionlint` is clean. No app source changed, so the trim baseline is unaffected, and CI re-runs it anyway.
- **Adversarial reviews.**
  - **First review, fixed:**
    - The host served one request at a time; it now serves them concurrently. The numbers did not move, and E2E still passes.
    - The build was taken from the test assembly's `BuildInfo`; it now comes from the published page.
    - A failed boot gave a bare timeout; the error now names the boot.
    - Closing a crashed browser's page could replace the real failure.
    - Aborting a failed response could throw from an unawaited task; every exception there is now contained.
    - The report now says that the browsers are headless and that CDP latency is per request.
  - **Second review (against this plan), fixed:**
    - This status wrongly credited the 7 warm requests to the loader's `no-cache` default; the cause is `force-cache` in the boot manifest.
    - "Resource cost" was not addressed.
    - No chunk owned the reference-desktop verdict; M21 now does.
    - CPU model and memory were not recorded.
    - "Warm" reused the same browser process; it is now a relaunch on the same profile.
    - The host-throttle alternative was not weighed (see above).
    - The two listener tests were in Unit.
    - The `trx-all-executed.sh` header and the README's CI order were stale.
    - The caching-off mutation was misreported as "full" re-downloads.
  - **Accepted:**
    - "Shell ready" may be a few milliseconds early. It is taken when the file input is in the DOM and enabled, and `InputFile` attaches its listeners just after, in `OnAfterRenderAsync`.
    - The ETag cache assumes files do not change while the host runs.
    - E2E keeps a response log that nothing reads (a few thousand small records).
    - The fingerprint rule is a name heuristic until M20.
    - A failed Perf run writes no partial report (see CI).
- **Compared with PKForge:**
  - What it has:
    - `PerfTrace`: in-app timings, opt-in and compiled out of Release (`[Conditional("DEBUG"), Conditional("DIAGNOSTIC")]`).
    - `EncounterLookupTests` and `Gen89SweepTests`: they print stopwatch timings, and `EncounterLookupTests` also asserts a loose 20 s bound.
    - CI: it uploads only the APK, with no size or performance artifact.
  - Its `Platforms/Android/linker.xml` preserves all of PKHeX.Core, because full trimming under AOT stripped the resources Core loads by name. It bears on our largest asset: the Web publish keeps those resources (the proof's legality checks depend on them), and they are 65% of `PKHeX.Core`. Any future attempt to trim or split them (PERF-005) must keep what Core loads by name.
  - Nothing to adopt for F8. In-app operation timing is relevant to M8 (legality latency, PERF-004) and M21 (memory, PERF-002). Android startup is out of Web scope.

→ Gate G-A, then open the upstream foundation PR (F1–F8). It must not claim support for anything.

## Phase 3 — XY/ORAS MVP (Goal 4)

Topic branches from `web/foundation`, in the order `PKHeX.Web.md` §"Proposed contribution sequence" gives. The no-op round trip lands first (M1–M6), then the editor (M7–M16), then polish and qualification (M17–M21). Every chunk adds or extends Unit and/or E2E tests.

### Slice A — load, browse, no-op export
- **M1 Shell.** Start screen with privacy copy ("processed entirely on this device…"), supported formats, temporary-state warning, and Open + keyboard-accessible drop zone. About panel shows version/Core commit (F5), licenses and the support matrix. Top-level `ErrorBoundary` with safe reset (WEB-APP-001–004, ERR-003).

  **M1 status:** code complete on `web/m1-shell`.
  - **Structure.** `App.razor` is now the shell: heading, About toggle, About panel, and the workspace inside `Components/FaultBoundary`.
    - The proof UI moved to `Components/Workspace.razor` with the same element ids, so the proof E2E and RealSave flows still drive it. Changes there: the h1 and the privacy and temporary-state paragraphs moved out; an "Open a save file" / "Open another save file" heading is added above the picker; a note is added to the loaded view; the initial message differs after a recovery; `SyncDraft` became `ResetDraftView`; and the state changes go through `WorkspaceState`.
    - Its state (session, pending replacement, draft, draft validity, `HasUnsavedWork`) moved to the scoped `State/WorkspaceState`, outside the boundary.
    - The shell arms `lifecycle.js`'s `setDirty` from `WorkspaceState.Changed`, so the leave warning stays in force while the recovery screen is shown. Before, the workspace armed it on every render, and a fault would have cleared it on dispose. `Apply` raises `Changed` straight after `SaveSession.Apply`, so the warning is armed even if reselecting the slot fails.
  - **Start screen** (`Components/StartScreen`, h2 "Before you open a save"):
    - the exact privacy sentence;
    - that the host sees ordinary requests for the app's files and may log them, while the app never sends the save or anything identifying it;
    - what can be opened: raw, decrypted X/Y/OR/AS `main`, up to 16 MiB. The save must first be copied off the console or emulator and is never decrypted, repaired or converted, and other games are refused;
    - the emulator prerequisites, adopted from PKForge: save in game and close the emulator first; save states cannot be opened; loading an older save state after editing can undo the edits;
    - that the save and edits are not stored in the browser, and a reload discards them.

    The Open action is the native file input under its own h2, with a 44 px selector button. It stays in one place for opening and replacing, so its drop-zone registration survives opening a save. The drop zone is an extra target around it.
  - **About** (`Components/AboutPanel`): a disclosure (`aria-expanded`/`aria-controls`, `hidden` when collapsed; no modal until M18's focus trapping). It shows:
    - the version, the full commit and the source repository (`https://github.com/kwsch/PKHeX`, adopted from PKForge's repository link; it points to the corresponding source for GPLv3), as `#about-version`, `#about-commit` and `#about-source`. These replace the footer's `#build-label`, and the now unused `BuildInfo.ShortCommit`/`Abbreviate` were removed;
    - links to `LICENSE.txt`, `THIRD-PARTY-NOTICES.md` and the four `licenses/*.txt`. They are relative, so they work under `/PKHeX/`, and open in a new tab with `noopener noreferrer`, because leaving the tab ends the session;
    - the families this release opens, each "in testing; not yet qualified as supported", with a line that no browser is qualified yet.

    It makes no support claim. The loader's refusal now reads "This release opens only raw X/Y and Omega Ruby/Alpha Sapphire saves" rather than "supported".
  - **Support matrix** (`Services/SupportMatrix`): the families this release opens (`SAV6XY`, `SAV6AO`, matched by exact type). `SaveLoader` now reads its allowlist from it, so the panel and the loader cannot disagree. This is equivalent to the old `is SAV6XY or SAV6AO`, because both types are sealed; the ORAS demo was already excluded. `SaveSession.Family` still names XY/ORAS itself; M3's overview replaces it.
  - **Startup** (`wwwroot/boot.js`, a classic script because of the CSP). It uses ES2015 syntax only (no `?.`, `??`, optional catch binding, async, spread or `**`), so it parses in old browsers such as Safari 12, which it exists to turn away; a Unit test guards this. Blazor starts with `autostart="false"`.
    - **Capability check.** Before starting, it checks for WebAssembly, WebAssembly SIMD and exception handling (by validating minimal modules; the bytes were checked to decode as `i8x16.popcnt(i8x16.splat)` and legacy `try … catch_all`), `BigInt64Array`, and Blob object URLs. If anything is missing, it names it and never downloads the runtime.
    - **Failed load.** A failed asset load shows "The app's files could not be loaded" with a focused **Try again** (a reload; no file is involved). The fallback text lives in `index.html`; the script only unhides it. Blazor's error bar is kept hidden under a failure screen.
    - **Found while testing: `Blazor.start()` never settles on a failed download.** In .NET 10 its promise does not settle when a download fails. Chromium then throws "Failed to start platform" as an unhandled rejection; Firefox and WebKit just hang. The runtime's `onExit`/`onAbort` module hooks are not called either.
      - What every engine does raise is the loader's unhandled "download '…/_framework/…' failed" rejection. So only unhandled errors that name `/_framework/` arm a 3 s timer, and the retry screen appears once they have stopped and start has not completed. A healthy boot raises none (every E2E boot requires zero page errors).
      - The first version counted any unhandled error. The review showed that an unrelated error, such as one from a browser extension, during a slow boot showed the failure screen. An E2E test now covers that case, and a mutation back to "any error" fails it in all 3 engines.
      - A start that completes anyway restores the app. In testing, however, a single aborted assembly download stopped the start for good in every engine, retry or not.
    - **Stalled download.** A download that never answers raises nothing, so after 30 s a non-destructive "Still loading … Try again" hint appears, while the loading message stays and loading continues.
  - **Fault boundary** (`Components/FaultBoundary`, an `ErrorBoundaryBase`): a recovery screen whose heading takes focus, which announces it; it has no live region, which would announce it twice.
    - With a session open it offers **Return to the workspace** (`RecoverAfterFault`: keeps the session and every applied change, and drops the draft and any pending replacement, which the fault may have left half-done) and **Discard session**; without one, **Start over**. It always offers **Reload page**, with the loss warning.
    - After recovering, focus moves into the workspace (a script-focusable wrapper), instead of dropping to the page body.
    - The session is safe to keep because `SaveSession.Apply` stages on a clone and swaps only after verifying, and exports are still validated.
    - No exception text is rendered. The exception, including its message and stack, goes to the browser console only; M17's redaction should consider it. Guarded export after a fault is simply the normal Download on the kept session; redacted diagnostics are M17.
  - **Other changes:**
    - The page title is "PKHeX Web", with a single `.page` container.
    - `StaticHost` and the fixture map `.md` → `text/markdown` and `.txt` → `text/plain` (F5's note).
    - `BootBaseline` reads the build from the About panel (F8's note).
    - A cold boot is now 60 requests and a warm one 8, because of `boot.js`. The F5 and F8 entries above still describe the footer, `#build-label` and "7 unfingerprinted files" as they were at the time.
    - `App.DisposeAsync` now tolerates a disconnected page.
  - **Tests.**
    - **Unit 154** (up from 138):
      - `WorkspaceStateTests` (5): unsaved-work rules, replacement, recovery keeping the applied session and its export, discard.
      - `SupportMatrixTests` (3): exactly XY/ORAS, the demo and other families not enabled, and a Core-recognised BW save refused by the loader.
      - `FaultBoundaryTests` (5), with **bUnit 2.11.3**. It is test-only, and the test project still resolves the Components 10.0.12 packages. They cover: the recovery screen without exception text and with focus moved; Start over without a session; Return keeps the session, drops the draft, moves focus back and catches a second fault; Discard; Reload forces a full load.
      - `BootScriptTests` (6): the ES2015 syntax guard.
      - 3 `BuildInfo` abbreviation rows were removed with the code.
    - **E2E 57** (up from 30):
      - The boot test (3 engines × 2 paths) now checks: the privacy copy; the About disclosure; the version, commit and source link; the row count; and every license link. Each link must resolve under the hosting path and carry `target="_blank"` with `noopener`. Every published license file must be linked, and each must be served by the test host with its type and be byte-identical to the published file. This checks the link attributes and the test host, not a browser opening the link or a real deployment's headers (M20).
      - `ShellTests`, all × 3 engines:
        - An unsupported browser, 5 cases × 3 engines, with no `_framework/dotnet*` request: WebAssembly deleted; every module rejected; only the SIMD probe rejected; only the exception-handling probe rejected; `URL.createObjectURL` deleted. The single-probe cases catch a swapped or wrong probe. A missing `BigInt64Array` cannot be simulated, because Playwright's own page scripts need it.
        - A failed `.wasm` load shows the focused retry, then boots cleanly once reachable, checked like any boot.
        - An unrelated rejection during a boot slowed by 5 s never shows the failure screen, recorded by an observer.
        - A stalled `PKHeX.Core` download shows the slow-loading hint within 45 s, with the app's loading message still in place.
        - The leave warning: no dialog with an unmodified save, one with a dirty draft, and one with an applied change and a clean draft.
      - Every E2E test executed.
    - **RealSave 14** and **Perf 1** pass; Perf reads the build from About.
    - **Mutation checks:** disabling the shell's dirty sync fails the leave-warning test in all 3 engines, and so does counting any error as a boot failure for the slow-boot test.
    - **Other checks:** the trim baseline is unchanged (38), and the `PKHeX.slnx` Release build has 0 warnings. Screenshots at 1280 px and 375 px show no horizontal scroll, and keyboard Tab reaches About first.
  - **Adversarial review.**
    - **Fixed:**
      - a slow but healthy boot shown as failed after any unrelated error;
      - `boot.js` syntax that old browsers cannot parse, which would leave them stuck on "Loading";
      - no retry offered for a stalled download;
      - the leave warning not armed if reselecting after an apply fails;
      - the privacy and temporary-state note gone once a save was open;
      - the Open control under the wrong heading;
      - a double announcement from `role="alert"` plus focus;
      - focus dropping to the body after recovery;
      - `DisposeAsync` not tolerating a disconnected page;
      - "Nothing is stored in this browser" being untrue, since the HTTP cache keeps the app's files;
      - the loader saying "supported";
      - dead `ShortCommit`;
      - the missing unsupported-browser, applied-change and slow-boot tests;
      - status claims that the UI moved "unchanged" and that exact-type matching changed behaviour;
      - the missed PKForge emulator prerequisites and source link.
    - **Not changed:**
      - The license-link test uses the test host (M20 checks the real headers).
      - `WorkspaceStateTests` raises `NotifyChanged` itself, because it tests the state class; the leave-warning E2E covers the workspace wiring.
      - The old F5/F8 entries were left as history.
  - **Not verified yet:**
    - bUnit under the azure-parity job's `vstest.console` 17.14 (next CI run).
    - How browsers present `.md` served as `text/markdown`; some may download it rather than show it, which is acceptable, and M20 sets the real headers.
    - The unsupported-browser detection is exercised by simulation only; no real legacy browser was run (G-C).
  - **Compared with PKForge:**
    - `Views/AboutPopup.cs` shows the version (with a diagnostic marker), authorship, an engine credit ("Engine PKHeX · chrome PKSM (GPL-3)"), a sprite credit and the repository URL. We adopted the repository link. Our PKHeX.Core mention is a descriptive sentence rather than a credit line. We are stricter: the source commit, the full license and package notices, and the support matrix. Its art and sprite credits belong to M5/G-B.
    - `HomePage.cs` gives per-emulator prerequisites (save in game and close the emulator, no save states, restart normally afterwards). These are adopted on the start screen.
    - `App.CreateWindow` catches startup exceptions and shows the raw exception text, so startup is never a blank screen. We match that, but deliberately show no exception text (SEC-005), and we separate an unsupported browser, a failed load and a stalled load, which Android does not need.
    - PKForge has no in-app error boundary or session-keeping recovery, and nothing further to adopt.
- **M2 Load pipeline + error taxonomy.** Typed outcomes: `Empty`, `TooLarge`, `ReadFailed`, `Unrecognized`, `RecognizedNotEnabled(family)`, `IntegrityFailed`, `ParserFault`. The replacement candidate is parsed before the old session is touched, and failure keeps the session (SAVE-001–004, ERR-001/002/004). Extends the proof's failure-path tests.
  - Unchanged round trip at open (from PKForge `SaveEngineSession.ValidateUnchangedRoundTrip`): `Write()` of the freshly parsed, unmodified save must equal the original bytes, otherwise the outcome is `IntegrityFailed`. This catches saves that pass their checksums but that Core would not write back identically.
  - Move user-facing wording out of `State/` and `Services/`: `SaveLoader`, `SaveSession` and `EditorDraft` currently throw `InvalidDataException` with display messages (carried over from the proof). They return or throw typed outcomes instead, and the UI maps them to text.

  **M2 status:** code complete on `web/m2-load-pipeline`.
  - **Outcomes** (`Services/SaveLoadOutcome`). `SaveLoader.Load` no longer throws for bad input. It returns a `SaveLoadOutcome`: a session, or a `LoadFailure`:
    - `Empty`, `TooLarge`, `ReadFailed`
    - `Unrecognized`
    - `RecognizedNotEnabled`, with a `RecognizedSave` (Core type, version, generation)
    - `IntegrityFailed`, with an `IntegrityProblem` (`NotExportable`, `ChecksumsInvalid`, `RoundTripMismatch`) and the `RecognizedSave`
    - `ParserFault`

    It carries no text and no exception details, ready for M17's sanitised codes. `FileReadResult.Open()` (in `Interop`, so `Services` does not depend on the browser types) maps a failed read to the same taxonomy, so picker, drop and tests share one entry point.
  - **Pipeline order:** size → own copy → parse a second copy → recognised → enabled → exportable → checksums → unchanged round trip → session. Everything from the parse to building the session (including its occupied-slot scan) is inside one `try`, so a Core exception at any point is `ParserFault`. `OutOfMemoryException` is not caught.
  - **Unchanged round trip** (from PKForge): `SaveLoader.WriteForComparison` (`save.Clone().Write()`) must equal the original bytes. It writes a clone because `Write()` refreshes checksums in the save's own buffer. A test runs it on a save with broken checksums and requires the buffer to stay unchanged; a mutation to `save.Write()` fails it. `SaveExporter` reopens through the same loader, so every export is also round-trip checked, and a reopen failure is `ExportRevalidationFailed`.
    - For valid XY/ORAS, Core's write is the buffer with checksums refreshed, so a natural mismatch cannot be produced. `RoundTripMismatch`, `NotExportable` and `ParserFault` are tested through an internal `Load` overload that replaces the parse and write steps.
    - `NotExportable` is a defensive check: Core marks only blank saves built without data as not exportable, never one parsed from bytes.
    - `ParserFault` also covers exceptions from our own code in the `try` (the session's slot scan, naming). It is not logged; redacted diagnostics for it belong to M17.
    - All 14 real XY/ORAS saves available locally round-trip exactly: the two G-D fixtures and the 12 XY Citra saves. They are exportable and checksum-valid, checked with a throwaway script outside the repository.
  - **Typed operation errors** (`State/SessionError`). `SaveSession`, `EditorDraft` and `SaveExporter` throw `SessionException(SessionError)`, whose message is the code name. There are 14 codes. The nickname check is split into too long, control characters and not representable. No `InvalidDataException` remains in the app.
  - **Wording** (`Components/UserMessages`) maps every outcome, integrity problem, session error and drop rejection to text.
    - `Unrecognized` states what the release opens (raw, decrypted X/Y and OR/AS `main` copied off the console or emulator) and claims nothing about corruption.
    - `RecognizedNotEnabled` says "This looks like a Generation N save", with a separate line for the ORAS demo. Game names are left to M3.
    - The family names in every refusal come from `SupportMatrix`, so the loader, refusals and the About panel name them the same way.
    - WEB-SAVE-004's "encrypted-console prerequisites" is met by the `Unrecognized` text: it asks for the decrypted `main` exported with a save manager on the console, or taken from an emulator. Core cannot tell an encrypted save from other unknown data, so there is no separate outcome.
    - `IntegrityFailed` names the family and the failed check, and says nothing was repaired or changed.
    - Unexpected exceptions keep the generic text and are logged to the browser console only.
  - **Failure keeps state.** `WorkspaceState.Accept(SaveLoadOutcome)` returns `Refused`, `Opened` or `Held`. A failure changes nothing. A parsed candidate opens when nothing would be lost, and is otherwise held as the pending replacement, replacing any earlier one. `Workspace` no longer cancels a pending replacement on every file pick, so a failed read or load keeps the session, the draft and any pending replacement.
    - The replacement panel names the waiting file (`#replace-name`, and the confirm button). A refusal while a file is waiting adds "{name} is still waiting to replace it", so the refused file and the waiting one cannot be confused.
  - **Tests.**
    - **Unit 196** (up from 154):
      - `SaveLoaderTests`: every outcome, including the zero-filled ORAS-size and truncated files as `Unrecognized` with nothing recognised; BW and the ORAS demo as `RecognizedNotEnabled`; the injected faults; no exception text in the outcome; caller bytes unchanged; read-status mapping.
      - `UserMessagesTests`: distinct, non-empty text for every value, none says "supported", and the exact text of every load outcome. That last test pins the wording, because the E2E tier builds its expected text from the same mapping.
      - `WorkspaceStateTests`: `Accept` opening or holding, and every `LoadFailure` refused without changing the session, draft, draft validity or pending replacement, and without raising `Changed`.
      - `SessionTests`: every refusal now asserts its `SessionError`, and a new test covers `SlotNotOccupied` and `EntityChecksumInvalid`.
    - **E2E 57:** `PublishedFailuresDraftsAndKnownLegality` now asserts the exact message for each of the seven rejected inputs, after checking that each fixture really fails as intended. It also checks that a pending replacement survives a rejected file, is named in the panel, and is named in the refusal. Every E2E test executed.
    - **RealSave 14:** `SaveFixtures.Open` requires a successful outcome, so the preflight now also proves the round trip on the private saves.
    - **Mutation checks:**
      - Restoring the unconditional `CancelReplace` on file pick fails the pending-replacement E2E check in every engine.
      - Cancelling the replacement in `Accept` on a refusal fails all 7 refusal rows.
      - Writing the save itself instead of a clone fails the round-trip write test.
    - **Other checks:** the trim baseline is unchanged (38), and the `PKHeX.slnx` Release build has 0 warnings.
  - **Adversarial review.**
    - **Fixed:**
      - the waiting file was not named, so after a refused pick "Discard current session and open" could open a file other than the one just picked;
      - the round-trip test injected its own write step and could not fail;
      - the "failure changes nothing" rule lived only in the component, and was tested only by E2E;
      - the E2E expected text came from the mapping under test; exact strings are now pinned in Unit;
      - `NotExportable` was described as a real Core condition;
      - `ParserFault`'s xmldoc said only Core throws;
      - `Services` depended on `Interop`;
      - the family names differed between refusals;
      - "copied off the console" did not mention the save manager.
    - **Recorded, not changed:** `ParserFault` is not logged (M17). WEB-SAVE-004's encrypted-console case is met by the `Unrecognized` wording (see above).
  - **Not verified yet:** no manual pass in a browser; the E2E tier drives the same seven refusals in three engines. A save that passes its checksums but fails the round trip has not been seen in real XY/ORAS data.
  - **Compared with PKForge:**
    - Matches: copying before parsing (`SaveEngineSession` constructor), and `ValidateUnchangedRoundTrip` (`SaveEngineSession.cs`). PKForge runs the round trip only in `WriteSafety` for Gen 3 layout checks; we run it at every open.
    - Stricter: PKForge reports every failure as one `InvalidDataException("… not a recognized save file")` and does not check `ChecksumsValid` or `Exportable` at open. We separate recognised-not-enabled, integrity by cause, and parser faults.
    - Out of scope: `SaveParser`'s RetroArch container decoding, SRAM padding trim, Luminescent and ROM-hack detection, and the GameCube memory-card hint; none applies to raw XY/ORAS. Its edition hint for shared-layout saves maps to WEB-SAVE-006 (MVP+).
- **M3 Overview.** Trainer name, game, language, TID/SID in the save's display format (`TrainerIDFormat`), playtime, money, sanitised filename and size, integrity status. Unknown values are labelled, never invented (OVERVIEW-001/002, SAVE-007).

  **M3 status:** code complete on `web/m3-overview`.
  - **Model** (`Services/SaveOverview`): a record of typed values with no text, built by `SaveOverview.From(session)` from the working save on every render, so it follows the current revision.
    - **Game:** `Version`, plus `VersionValid` (`IsVersionValid()`), and the `SupportedFamily` matched by exact type.
    - **Trainer:** the name is `OT`, or null when blank or whitespace. The language is kept raw, with its name looked up in `GameInfo.LanguageDataSource(generation, context)`; a value that is not listed has a null name.
    - **IDs:** `TrainerIDDisplayFormat` and `DisplayTID`/`DisplaySID`, padded with Core's own format strings. There is no raw ID32, because in the 16-bit format the displayed IDs are the stored values.
    - **Other values:** playtime, `Money`, and `SAV6.Played.LastSavedDate` (null when the stored date is invalid).
    - **File:** the sanitised file name, and the size from the new `SaveSession.OriginalLength`, so no copy is made.
    - **Integrity** is not recomputed. `ChecksumsValid` is stale after an in-memory apply, because Core refreshes checksums only on `Write()`. Instead, the overview states what the loader checked before the session existed, and that every download is revalidated.
    - **Left out:** Gen 6 has no save revision, so none is shown.
  - **Wording** (`Components/OverviewText`): all text and formatting, with invariant culture throughout.
    - The game is Core's English name (`GameInfo.GetVersionName`, e.g. "X", "Omega Ruby"), and the language is Core's own label (e.g. "FRA (Français)", as WinForms shows it).
    - Unknown values: "Unknown (stored value N)", "Not set in this save" and "Not recorded".
    - Playtime is "123 h 04 min 05 s", with the hours never capped. Money is "1,234,567 Pokédollars". Size is "415,232 bytes (405.5 KiB)", with the exact bytes first.
  - **Panel** (`Components/SaveOverviewPanel`): a `<section aria-labelledby>` with an h2 "Save overview" and a `<dl>` of 13 `#overview-*` values, all rendered as text. The `dl` is a two-column grid that stacks below 30rem.
  - **Workspace:** the overview replaces the "Loaded save" heading and the `Family`/hard-coded `Integrity: Valid` line. The slot picker now sits under its own "Box slots" heading, and `#session-state`/`#session-note` follow the overview. `SaveSession.Family` is removed, and `SaveExporter`'s identity check compares the exact save type instead (the same meaning, since the types are sealed).
  - **Found while testing:**
    - The loader opens a save whose stored game is outside its family, e.g. an XY layout storing OR, or 0, because it matches families by type (layout). The overview labels such a game as unknown with its raw value rather than naming it. Refusing such saves is not in M3's scope; noted for M17's hostile-input pass.
    - `SaveFixtures.Synthetic` gained an optional `customize` hook for setting trainer values before the write.
  - **Tests.**
    - **Unit 215** (up from 196):
      - `SaveOverviewTests` (10): every value read from XY and ORAS saves with distinct known values; blank and whitespace OT; languages 0, 6 and 42; an invalid last-saved date; a stored game outside the family (three cases); and a nickname apply leaving the trainer summary identical.
      - `OverviewTextTests` (5): exact strings for known, ORAS, unknown and largest values, and identical text under de-DE, fr-FR, ar-SA and hi-IN.
      - `SaveOverviewPanelTests` (2, bUnit): every id in order with its text, and a markup trainer name rendered as text.
    - **E2E 63** (up from 57): `OverviewShowsTheOpenSave` (3 engines × 2 paths) asserts all 13 values against pinned strings, including the sanitised hostile file name "Serena's <save>" → "Serena's _save_", and checks for no horizontal scroll at 375 px. The earlier `#family` checks now use `#overview-game`.
    - **RealSave 14:** the browser round trip now compares the overview's family, trainer, TID, SID and money with native Core values, with the private values withheld from failure messages. The preflight compares the session's save type.
    - **Mutation checks:** swapping `DisplayTID`/`DisplaySID` fails the XY and ORAS value tests; showing a blank OT as-is fails both blank-name rows; formatting with the current culture fails the locale test.
    - **Other checks:** the trim baseline is unchanged (38), and the `PKHeX.slnx` Release build has 0 warnings.
  - **Not verified yet:**
    - No manual browser pass or screenshots. The E2E tier covers the values and the 375 px layout in three engines.
    - No separate adversarial review yet (M1 and M2 each had one).
  - **Compared with PKForge:**
    - **Matches:** PKForge has no overview screen; its save description (`SaveEngine.TryDescribe`) and trainer card (`SaveEngineSession.GetTrainer`) show OT, playtime, language, TID16/SID16 and money. We show the same values, with game names from Core's English strings.
    - **Stricter:**
      - IDs follow Core's `TrainerIDFormat`/`DisplayTID` rather than always raw TID16/SID16. The two are the same for Gen 6, but only ours holds for Gen 7+.
      - A blank OT and an unknown language are labelled, where PKForge falls back to the file name or drops the tag.
      - Money is shown in full with separators, not with a plain `ToString`.
      - We add file size, integrity and the last-saved date, which PKForge never shows.
      - We don't mask a money read failure the way `ReadMoneySafe` does; for SAV6 it is a plain block read, and the fault boundary covers a Core bug.
    - **Not adopted:**
      - `EditionPair` and the edition-from-own-Pokémon guess, because XY/ORAS store one version (the shared-layout hint is WEB-SAVE-006, MVP+).
      - Trainer editing, which is WEB-TRAINER-001/002 (MVP+).
      - BP/coin stats, which are WEB-OVERVIEW-003 (post-MVP).
- **M4 Party + box grid.** Party strip plus the current box only. Box selector and prev/next use Core `BoxCount`/`BoxSlotCount` and box names. The grid is a single tab stop with arrow keys and Enter, plus a list alternative. Slots are labelled with coordinates and species text. Empty slots never open a stale entity (PARTY-001, BOX-001/008, A11Y-001). Slot labels are built in the UI from box/slot coordinates, replacing `SaveSession.SlotLabel`.

  **M4 status:** code complete on `web/m4-party-box-grid`.
  - **Positions** (`State/SlotRef`): a party position or a box slot, zero-based, created through `InParty`/`InBox`. `ToSlotInfo` gives Core's `SlotInfoParty`/`SlotInfoBox`, replacing `SaveSession.GetSlot(save, int)` and the flat index. `SlotRef.PartyPositions` is 6, because Core's `MaxPartyCount` is private.
  - **Session.** `OccupiedSlots` (a scan of every box at open) and `SlotLabel` are removed. `Select(SlotRef)` reads the live working save, so a draft is never taken from an earlier revision. Empty, out-of-range and party positions at or after `PartyCount` are `SlotNotOccupied`, even when a released member's bytes remain there; an entity that fails `PKM.Valid` (checksum or sanity flag, a bad egg as the game and WinForms' `SlotUtil` treat it) is `EntityInvalid`, renamed from `EntityChecksumInvalid`, which checked the checksum only. `EditorDraft.SlotIndex` became `Slot`, and legality uses the slot's own type, so a party member is analysed as `StorageSlotType.Party`.
  - **Party is inspect-only** (decided for M4): `EditorDraft.CanApply` is false for party drafts, and `Apply` refuses them with the new `SessionError.PartyApplyNotAvailable` before staging anything. The nickname fields are read-only for a party draft, so it can never become dirty. M9 lifts this with the party-stat policy.
  - **Views** (`Services/StorageView`): typed `SlotSummary` (occupied, readable, species, nickname when nicknamed, egg, shiny) and `BoxView`, read from the current revision. `StorageBrowser` reads them again whenever the session, its revision or the shown box changes (the working save changes only through an apply, which advances the revision), so a keystroke in the nickname field does not decrypt the party and a box. Only the party and the shown box are read, and no legality runs while browsing (BOX-008). Box names come from `IBoxDetailNameRead`, null when blank; `BoxName` reads a name without reading slots, for the selector. A bad egg (failing `PKM.Valid`) is reported as not readable, with nothing read from it. The first box shown is the save's in-game `CurrentBox`, or box 1 when that is out of range.
  - **Text** (`Components/SlotText`, invariant culture): "Party position 2", "Box 3, slot 7 (row 2, column 1)", contents ("Zigzagoon \"Ziggy\", shiny", "Egg", "Empty", "Bad egg", or "Unknown species (stored value N)"), and box titles from the stored name or Core's `GetDefaultBoxName`. The selector shows "5. Box 5", so equal names stay distinct. Columns: 6 for a box and 2 for the party, as the games lay them out; rows follow `BoxSlotCount`.
  - **UI.**
    - `Components/SlotGrid`: `role="grid"` rows of gridcell buttons with a roving tabindex. Arrows move (no wrap at the edges), Home/End go to the row ends, Ctrl+Home/End to the first and last slot, and Enter/Space open a slot as any button does. The draft's slot is `aria-selected`, and a newly created grid (e.g. after leaving the list view) puts its tab stop there. Each button's accessible name is position plus contents, and its visible text is the short contents. The column count is a class, not an inline style, because the CSP (`style-src 'self'`) blocks style attributes.
    - `wwwroot/grid-keys.js`: a classic script in `index.html` that cancels the default (scrolling) of navigation keys inside `[role=grid]`. It deviates from the plan's lazily imported module: an import after a save is opened is a request after boot, which `AssertNoNetworkOrPersistenceAsync` rightly fails, and would reveal to the host that a save was opened. Boot is now 61 requests cold and 9 warm (confirmed by a 2-run Perf tier; timings did not move).
    - `Components/SlotList`: the list alternative, a table with Position, Contents and an Open button for each slot that can be opened; the draft's row has `aria-current`.
    - `Components/StorageBrowser`: "Party and boxes" with a "Show as a list" toggle, the party ("Party (n of 6)"), the box title, Previous/Next (wrapping, as WinForms does) and a box selector. The view choice and current box live in `WorkspaceState` (`CurrentBox`, `ShowBox`, `ShowAsList`); navigation raises no `Changed`, because it cannot affect unsaved work. The box is kept across fault recovery and reset on open and discard.
    - `WorkspaceState.OpenSlot` decides activation on the live save and returns a typed `SlotOpening`. It never replaces a dirty or refused draft (`DraftPending`; navigation is still allowed). It leaves the slot that is already open as it is (`AlreadyOpen`, which keeps its legality result). An empty slot or a bad egg closes a clean draft and opens nothing (`Empty`, `Unreadable`). `Workspace` maps each outcome to text. Opening announces "Opened {label}." in `#message`. The draft section is labelled by its heading and shows its position (`#draft-slot`); a party draft shows `#party-note`. Previous/Next announce the box they show in a visually hidden status (`#box-status`); the selector announces its own value.
    - At 375 px the selector takes its own row with Previous/Next beneath it, and slot text shrinks. Species names can break mid-word in the 6-column grid; these are text placeholders until M5's sprites.
  - **Tests.**
    - **Unit 273** (up from 215):
      - `StorageViewTests` (14): Core box and slot counts for XY and ORAS; slot contents; egg, shiny and nickname; bad eggs by checksum and by sanity flag (with a valid checksum); six party positions with empties; bytes after the party count never shown or opened; stored, blank, whitespace and hostile box names; in-game current box (4 rows); views following a new revision.
      - `SlotTextTests` (11): exact positions, contents, labels and box titles, identical under de-DE, ar-SA and hi-IN.
      - `SlotGridTests` (20, bUnit): rows, cells, names and the single tab stop; 15 key movements, including the edges, non-navigation keys and Alt/Meta-modified keys, with focus following; activation of any slot; `aria-selected`; the tab stop kept in range when the grid shrinks and keys still moving focus afterwards, and placed on the open slot in a new grid.
      - `SessionTests` (+4): the party member (not the boxed copy) is opened and analysed as party, matching native Core; a party apply is refused and changes nothing; out-of-range positions; select reads the current revision.
      - `WorkspaceStateTests` (+4): the start box and wrapping; the box kept on recovery and reset on open and discard; `OpenSlot` opening party and box entities, closing the clean draft on empty and unreadable positions, keeping the open slot's draft, and never replacing unapplied or refused work.
      - `StorageBrowserPanelTests` (5, bUnit): the views show a new revision after an apply, and a new session at the same revision and box; a re-render without a new revision does not read the save again; box navigation and the named selector; Previous/Next announced.
      - The existing tests moved to `SlotRef`; `SaveFixtures` gained `FirstBoxSlot` and `WithPartyMember`, and `WritableSlot` returns a `SlotRef`.
    - **E2E 69** (up from 63): `StorageBrowserTests` (3 engines × 2 paths) covers:
      - 31 boxes starting at the in-game box, and wrapping both ways;
      - a hostile box name rendered as text;
      - keyboard only: one tab stop, arrow/Home/Ctrl+Home movement with each key's default cancelled, Enter opening the slot, and Tab leaving the grid;
      - an empty slot opening nothing;
      - a party member read-only with Apply disabled and native party legality;
      - a dirty draft not replaced;
      - the list view opening the same slot;
      - no horizontal scroll at 375 px in both views;
      - privacy and no page errors.

      `ProofPage.Select` now drives the box selector and grid, and every E2E test executed.
    - **RealSave 14:** the preflight checks that the native writable slot shows as openable and opens the same entity. The round trip addresses the slot by `SlotRef`.
    - **Mutation checks:**
      - every slot a tab stop fails 17 of the 18 grid tests there were then;
      - allowing party apply fails the refusal test;
      - ignoring the party count fails the leftover-bytes test;
      - wrapping ArrowRight across rows fails its edge row;
      - `grid-keys.js` without `preventDefault` fails the E2E key check in all 3 engines. A first version of that check compared `scrollY`, which also moves when focus scrolls a slot into view; it now reads each key event's `defaultPrevented`.
    - **Other checks:** the trim baseline is unchanged (38), and the `PKHeX.slnx` Release build has 0 warnings. Screenshots of the XY G-D save at 1280 px and 375 px show the party, box grid and navigation with no horizontal scroll.
  - **Adversarial review.**
    - **Fixed:**
      - Activation rules lived only in `Workspace` and were tested only by E2E, and they were decided from the render-time summary. They are now `WorkspaceState.OpenSlot` on the live save, with Unit tests.
      - Every workspace re-render, such as each nickname keystroke, decrypted 36 entities and decoded 31 box names. Reads are now keyed on session, revision and box.
      - Reopening the already-open clean slot re-created its draft and dropped the legality result.
      - A re-created grid put its tab stop on the first slot rather than the open one.
    - **Mutation checks:** a cache key without the revision, no `AlreadyOpen` check, no dirty-draft guard and no tab-stop placement each fail their test.
    - **Recorded, not changed:**
      - Stored box names and nicknames are rendered as text, but bidi overrides and other format characters in them can still reorder the surrounding text, as with M3's trainer name; noted for M17's hostile-input pass.
      - If Core threw while reading the shown box, "Return to the workspace" would show the same box and fault again; Discard session still escapes. Valid XY/ORAS saves have not been seen to do this.
  - **Second adversarial review** (independent, read-only, with its own probes and mutations).
    - **Fixed:**
      - `SlotGrid` replaced its element-reference array when the slot count changed, but Blazor captures `@ref` only when an element is created, so the next arrow key threw ("ElementReference has not been configured correctly"; reproduced in bUnit). References are now a dictionary that is never replaced, and the shrink test presses keys afterwards. Latent for XY/ORAS, whose grids never change size.
      - The session part of the read cache and the caching itself were untested: dropping either passed every Unit and E2E test. Both are now pinned (a new session at the same revision and box; a change made without an apply is not read on re-render).
      - Readability used `ChecksumValid`, where Core (`G6PKM.Valid` also requires `Sanity == 0`), the game and WinForms treat a set sanity flag as a bad egg. It now uses `PKM.Valid`, the text is "Bad egg", and the error is `EntityInvalid`.
      - Alt- and Meta-modified arrows moved focus while `grid-keys.js` left the browser's own shortcut to run as well; they are now ignored. The navigation rules moved to the pure `Components/GridNavigation`.
      - Previous/Next changed the box silently for screen readers; they now announce it.
      - Empty-slot text used `GrayText` (#808080 in some engines, about 3.9:1); it is now mixed from `CanvasText`.
      - The locale test built its expected value from the code under test; it now compares literals.
      - "Apply or cancel the draft before opening another Pokémon" was also shown for empty slots; it now says "another slot".
      - This entry wrongly said PKForge's `Snapshot` is built once (it is built on demand; a stale comment in its `PartyTests` says otherwise), called `entity.Valid` legality, and gave 16 for a mutation that failed 17 tests; the README said "per occupied slot" for the list's Open buttons.
    - **Mutation checks:** a cache key without the session, no caching, `ChecksumValid` in the view or in `Select`, lost element references, and no modifier check each fail their test.
    - **Not changed:** the "Opened …" message is built from the activated slot's summary, which is the current revision's because every apply re-renders; `aria-selected="false"` stays on unselected cells, as the ARIA grid pattern uses it for selectable cells; the grid's name is the box title, next to the heading that numbers it; `WorkspaceState.ShowBox` repeats the wrapping of Core's `BoxEdit.MoveLeft/MoveRight`, which needs a `BoxEdit` instance.
  - **Not verified yet:**
    - No screen reader was run; the names, roles and announcements are checked in the DOM only (M18 adds axe). How `display: contents` rows reach the platform accessibility tree is unverified.
    - No physical touch device (G-C).
  - **Compared with PKForge:**
    - **Matches:** `SlotSummary` (species, nickname, shiny, egg), with readability from `PKM.Valid` (PKForge stores it as `IsLegal`, but it is the checksum and sanity check, not legality); snapshots read live rather than cached at open; the party as a first-class storage target (its box `-1`, our `SlotRef.InParty`); box names through `IBoxDetailName` with a numbered fallback; a `ValidateCoordinates`-style range check before reading.
    - **Stricter:**
      - PKForge lists only party members `0..PartyCount-1`; we show all six positions and treat bytes after the count as empty.
      - Its `Snapshot` is built on demand, like our views, but decodes every slot of every box on each access; we read only the party and the shown box, once per revision.
      - Its fallback name is "BOX 01"; ours is Core's `GetDefaultBoxName`.
      - It has no keyboard grid or list alternative (its UI is gamepad and touch).
    - **Not adopted:** sprites and held-item marks (M5); box rename and wallpapers (`BoxLayoutService`; WEB-BOX-002/003, MVP+ and post-MVP); move, swap, release and sort (WEB-BOX-005, WEB-PARTY-002–004, MVP+); party edits (M9). Second-screen and gamepad views are out of Web scope.
- **M5 Sprite catalog.**
  - (a) Build-time generator: it reads the existing `PKHeX.Drawing.PokeSprite` resource images and emits a fixed atlas + JSON manifest into `wwwroot`. It is reproducible, and a provenance note is committed.
  - (b) Runtime `SpriteCatalog` loads the whole atlas before file input is enabled. It resolves species/form/gender/shiny, falls back to a text placeholder, and makes no per-entity requests. E2E asserts identical request traces for two different saves (BOX-004, PERF-003, SEC-001).
  - Until G-B is cleared, the atlas stays behind a build flag that defaults to placeholders.

  **M5 status:** code complete on `web/m5-sprite-catalog`.
  - **Flag.** `PKHeXWebSprites` (default `false`) in `PKHeX.Web.csproj`. A default build publishes no sprite file and requests none; slots are text, as in M4, and About (`#about-sprites`) says "Not included in this build". `-p:PKHeXWebSprites=true` runs the generator at publish and adds `wwwroot/sprites/` through `ResolvedFileToPublish` (the `AddUpstreamNotices` pattern). Default builds, Azure's included, never build or run the generator. `BuildInfo.SpritesIncluded` reads the flag from new assembly metadata.
  - **Generator** (`PKHeX.Web.SpriteAtlas`, a net10.0 console project in `PKHeX.slnx`, never published):
    - `ResxSpriteIndex` reads PokeSprite's `Properties/Resources.resx`, which maps each resource name to its file (`b_100_1` → `Big Pokemon Sprites/b_100-1.png`), rather than guessing from file names.
    - `SpriteSelection` resolves every Generation 6 species, every form in `PersonalTable.AO`, every gender and both shininesses, and packs exactly the images that resolve, plus each species' default, `b_unknown`, `b_egg`, `b_490_e` and `rare_icon_alt`: 1,838 images.
    - `Png` is its own minimal codec (user decision: no image library). It decodes 8-bit greyscale, RGB, palette, grey+alpha and RGBA with `tRNS`, all five filters and CRC and length checks, and refuses anything else. It encodes RGBA with the usual filter heuristic and `ZLibStream`, with no time or text chunks.
    - `AtlasWriter` packs 68x56 cells in ordinal name order (2176x3248, 1.24 MiB). It writes `pokemon.{hash}.png`, `sprites.{hash}.css` (one `.sprite-c{n}` rule per cell: `object-position` and size, because the CSP blocks inline styles), `manifest.json` (the two names, the layout and each key's cell) and `sources.json` (each image's source file and SHA-256), with `.br`/`.gz` copies of the text files. It removes its earlier outputs first.
    - Two runs give identical bytes. The sources carry only `sRGB`/`pHYs` ancillary chunks, which change no samples.
  - **Choosing a sprite** (`Services/Sprites/SpriteKeys`, compiled into both the app and the generator). It uses the desktop's own naming code, `PKHeX.Drawing.PokeSprite/Util/SpriteName.cs`, link-compiled, with no refactor of Drawing. It repeats `SpriteBuilder5668s`, which the desktop suggests for XY/ORAS, in the Gen 6 context:
    - the `b` key, then `c`; for a shiny entity without a shiny image, the same without shininess; then `b_{species}` with `b_unknown` at 50%; then `b_unknown` alone
    - `AllowShinySprite` is true (the WinForms default)
    - eggs follow `ShowEggSpriteAsHeldItem` (default true): the egg icon at (18,1) over the species, or, when the egg holds an item, the species faded to 33% under the egg; `b_490_e` for Manaphy
    - the shiny star `rare_icon_alt` at 70%
    - held-item icons are not drawn
  - **Runtime.**
    - `Program.cs` awaits `SpriteCatalog.LoadAsync()` between `Build()` and `RunAsync()`, so `#save-file` cannot exist before the atlas is resident. It fetches the manifest (`HttpClient`, source-generated JSON) and validates it: version, hashed names only, cells in range and not shared, required keys present. `browser.js` `preloadSprites` then loads the stylesheet and `decode()`s the atlas into a module-held `Image`.
    - Every sprite is an `img` of that exact URL. Browsers serve it from the document's list of loaded images, so no request follows; E2E proves this in 3 engines. A failure, or 20 s without completion, leaves the catalog `Failed`: text only, and no `img` is ever rendered.
    - `SlotSummary` gained `Form`, `Gender` and `HoldsItem`, which a bad egg does not carry.
    - `Components/SlotSprite` renders an `aria-hidden` span of `alt=""` images: a species group (species plus unknown mark), then the egg and star. It appears in `SlotGrid` (the button also gets `title` = its label) and in `SlotList`. The visible text stays.
    - `app.css` clips the sprite to 68x56, pixelated, with `zoom: .7` below 40rem.
  - **Notices and docs.** `THIRD-PARTY-NOTICES.md` has a sprite section: provenance, pokesprite as PKHeX's README credits it (its MIT text reproduced; its README says the images are © Nintendo/Creatures/GAME FREAK and MIT covers the rest), and the rights holders. About credits them when loaded. `PKHeX.Web/README.md` has a Sprites section.
  - **CI.** `web.yml` publishes again with the flag after the trim, inventory, size and upload steps (both publishes share `PKHeX.Web/obj`). It appends a sprite size table to the summary and passes `PKHEX_WEB_PUBLISHED_SPRITES` to E2E. The sprite publish is never uploaded (G-B). `PKHeX.Web.SpriteAtlas/**` is added to the paths. A flagged restore adds no package to the Web restore graph, so the notices checks are unaffected. The azure-parity job now also builds the generator (NETSDK1233 count +1 expected).
    - **First GitHub run (PR #16, run 36909562455):** every step passed except E2E, where 96 of 97 passed. `StaticHostServingTests.DefaultModeServesRawFilesWithoutCachingHeaders` saw `[200]` instead of `[200, 404]`. That was a race from F8's concurrent host, not from sprites: `StaticHost` logged each response after `Close()`, so a client could finish reading the 404 before its entry existed. Entries are now recorded before the response is sent, and a response that then fails to send also logs status 0. The host tests passed 20 runs in a row, and Unit and Perf still pass.
    - **CI time** (M5's first run: `web` took 17 min, up from 10, and `azure-parity` 10 min, up from 4):
      - The browser cache was keyed on the test project's hash, so M5's edit to it downloaded all three browsers (about 410 MiB) again. It is now keyed on the Playwright version. The apt install of their system libraries still runs every time, at whatever speed the mirror gives (3½ min here, 14 min in M3's run).
      - E2E grew by 2:40. The stalled-atlas case now runs only in Chromium, which saves two 20 s waits, so E2E is 95 tests.
      - The Perf baseline now runs only on pushes to the merge targets, saving about 1:50 per PR.
      - The next run (36916668550) took 26 min, 15:52 of it installing the browsers; the azure-parity job took 4.5 min. Across 11 runs the install is bimodal: 0:34–3:28 in 7 runs and 14–25 min in 4. Most of the step is `--with-deps` apt-installing about 181 packages from the runner's Ubuntu mirror, and the browser cache cannot hold them. The `web` job now runs in `mcr.microsoft.com/playwright/dotnet:v1.63.0-noble`, pinned by digest (1.3 GiB compressed), with `--ipc=host --user 1001`. The image carries the browsers, their libraries and SDK 10.0.401, so `setup-dotnet`, the browser cache and the install step are gone. The SDK pin stays as a check. A new step fails unless the csproj's Playwright version equals the job's `PLAYWRIGHT_VERSION` and the image has all four browser builds. The artifact paths are set from `$GITHUB_WORKSPACE`, because `github.workspace` is the host's path in a container job.
      - The trim baseline no longer publishes a second time. The main publish logs the warnings (`SuppressTrimAnalysisWarnings=false`), and `trim-warnings.sh --log` reads that log. Publishes with the setting on and off gave identical files (198, compared by hash with a fixed `SourceRevisionId`). That saves about 55 s.
      - Not yet run on GitHub: the image pull time, and `--user 1001` with checkout, caches and uploads.
      - Deferred: running the E2E engines in parallel (5:41 one test at a time). It needs a fixture per engine and a flakiness check of the timing-sensitive tests under CPU contention first.
      - **Done later (`web/ci-scoped-e2e`, after M6's first CI run took 11.5 min with E2E at 6:52):**
        - The engines run as three `e2e` jobs, one per engine (`PKHEX_WEB_ENGINES`), on separate runners. No fixture per engine was needed, and with no shared CPU the contention concern does not arise.
        - Each job publishes for itself, and builds the sprite publish only when needed, so it never waits for `web` and the sprite publish is still never uploaded.
        - A `changes` job (`PKHeX.Web/tools/ci-changes.sh`) skips the publish checks and E2E on pull requests that cannot change the app (WinForms, Drawing, Core tests, docs), and skips the sprite publish and its tests (`[Trait("Needs", "SpritePublish")]`, filtered out with `Needs!=SpritePublish`) when no sprite input changed. Pushes to `master`/`web/main` always run everything.
        - The Chromium-only stalled-atlas test now uses the run's first engine, so each job runs it in its own engine.
        - Engine-free E2E tests (8: the publish and notices checks) run in every job.
        - Local simulation of the job's test step: 37 tests in about 1:45 per engine with sprites, 29 in 1:09 without. The union of the three jobs equals the 95 tests of a full run.
        - Unit +9: engine selection parsing, unknown engines refused, and a guard that `Needs` carries only `SpritePublish` and only on E2E tests.
        - Not yet run on GitHub.
      - The azure-parity `vstest.console` step grew from 1:43 to 5:30, but its 1,185 tests ran in 14 s (the slowest took 3.6 s). Its log shows 5 min 3 s passing before the first test assembly was found. The time went to the two searches that run first: `vswhere -find '**\TestPlatform\vstest.console.exe'`, which walks the whole Visual Studio install, and `Get-ChildItem -Recurse` over the checkout, which includes the full git history. The first is replaced by vswhere's `installationPath` plus the fixed `Common7\IDE\Extensions\TestPlatform` location. The second now skips `.git`, which holds no assemblies. Each search prints its time, so the next run shows which one it was. Not yet run: `pwsh` is not installed locally.
  - **Tests.**
    - **Unit 360** (up from 273):
      - `PngCodecTests` (26): each filter and colour type from hand-assembled PNGs with hand-worked filter bytes, the Paeth tie-break, split `IDAT`, CRC/16-bit/interlace/unknown critical chunk/wrong length/bad filter/bad palette index rejected, round trip and determinism, every packed source decodes.
      - `SpriteKeysTests` (21): literal desktop names (cosplay Pikachu, Pyroar ♀, Meowstic, Unown, Vivillon, Floette, Hoopa), fallbacks, eggs, the star, and completeness: every Gen 6 tuple (over 4,000) has its own sprite with no unknown mark, so PKForge-style gap list is empty.
      - `SpriteAtlasTests` (9): reproducible bytes and names, names that are content hashes, every cell equal to its decoded source, one CSS rule per cell and nothing else, `sources.json` hashes, the limits, stale-output removal, and `.br`/`.gz` round trips.
      - `SpriteSheetTests` (19): hashed-name and layout validation (13 broken manifests), resolution to cells, the Alolan fallback, and a build without sprites making no request or JS call.
      - `SlotSpriteTests` (5, bUnit): no image without sprites; decorative layers with the label and text kept; exact layer classes, including the grouped fade; bad eggs; the list view.
      - `BuildInfoTests` (+7).
    - **E2E 95** (up from 69):
      - `SpriteCatalogBrowserTests`, 3 engines:
        - Across both paths: the manifest, stylesheet and atlas are each fetched once, the atlas before the input was enabled. Two different saves across the party and three boxes show exactly the resolved layers with every image loaded. The list view works, with no horizontal scroll at 375 px. Nothing is requested after boot, and a second fresh visit with only the second save has the same boot requests.
        - Every one of the 1,838 cells, drawn by the browser from the atlas, equals the browser's own decoding of its source.
        - Each file aborted (×3) falls back to text with no later request; so does a stalled atlas, after the 20 s limit (Chromium only, since the limit is engine-independent C# and each run waits it out).
        - A default publish shows text and requests nothing under `sprites/`.
      - `SpritePublishAddsOnlyTheAtlasFiles`, and the default allowlist now also requires no `sprites/`. The fixture serves the sprite publish from a second, lazily started host; `.png` is mapped to `image/png`. Every E2E test executed.
    - **RealSave 14** pass (default publish). Trim baseline unchanged (38); `PKHeX.slnx` Release 0 warnings; actionlint clean. Screenshots of the XY G-D save at 1280 and 375 px show the sprites (Vivillon patterns told apart) with no horizontal scroll.
    - **Mutation checks:**
      - skipping the atlas preload fails the boot-count check, and, with that disabled, the after-boot trace check, in every case
      - a palette-alpha error in the decoder fails the browser pixel check in all 3 engines
      - a changed Paeth tie-break fails the new tie test
      - swapped shininess, a dropped unknown mark, a timestamp chunk and a removed manifest-name check each fail Unit
      - a symmetric Sub-predictor change made the generator fail on a source and fail the publish
      - an Average and a Paeth `pa` tie change were equivalent on this data: the atlas bytes were identical, since no source or chosen row exercises them
  - **Adversarial review** (after the dev work, independent, with its own probes).
    - **Fixed:**
      - A stalled sprite request left the app on "Loading" forever, and `boot.js`'s slow hint had already cleared. The catalog now gives up after 20 s, with an E2E case.
      - For an egg holding an item whose form has no sprite, the unknown mark stayed at 50% while the desktop fades the composed image to 33% (about 17%). The species and mark are now one faded group, which composites as the desktop does, with Unit and E2E cases.
      - Fixed atlas, stylesheet and manifest names let a browser combine files from two deployments, and cells shift when an image is added. The atlas and stylesheet are now content-hashed and named by the manifest.
      - The atlas was not regenerated when Core's data changed without its API: the generator's copy of Core is now an input.
      - Sprites overflowed their slots between 481 and about 585 px: the zoom now starts at 40rem.
      - The egg icon's box overhung the sprite: the sprite is clipped.
      - "Shiny sprites … (MIT)" implied the images were MIT: reworded from pokesprite's own README.
      - Found while fixing: a stale unhashed atlas from an earlier run survived in `obj`. The build's new "exactly one atlas" check caught it; the generator now removes hashed and unhashed outputs.
    - **Recorded, not changed:**
      - The E2E layer check takes its expectations from `SpriteSheet.Resolve`; mapping fidelity rests on the literal `SpriteKeysTests`. Offsets and opacities are checked by class, not by rendering.
      - Cross-OS determinism is checked within one run, not against a golden hash (zlib-ng and Brotli ship with .NET).
      - `AllowShinySprite` is set in `SpriteKeys`' static constructor.
      - Invalid entities (forms past Gen 6's range, species above 721) show the default plus the unknown mark where the desktop may find a later game's image; documented in the README.
      - The Perf tier measures only the default publish; the atlas (1.24 MiB, about 28 MB decoded) belongs in M21's measurements.
      - The generator reruns on every flagged publish (about a second), because `Directory.Build.props`' timestamp recompiles it.
  - **Not verified yet:** no screen reader (the sprite is `aria-hidden`); decoded-atlas memory on iPad/Android (M21); physical devices (G-C); the first GitHub run of the sprite publish and of the generator under azure-parity's VS 17.14 MSBuild.
  - **Compared with PKForge:**
    - **Matches:** bundled PokeSprite `Big` sprites (normal and shiny) as the base set; a mirror of `SpriteName`'s naming; an ordered fallback ending in an unknown placeholder; a gap list, which here is empty and enforced by `EveryGenerationSixFormHasItsOwnSprite`; text until sprites are ready.
    - **Stricter:**
      - PKForge copies `SpriteName`'s logic (`PkhexName()`); we compile the desktop file itself, so the two cannot drift.
      - It draws an egg as its species; we draw eggs as the desktop does.
      - It warms sprites in the background and fetches HOME, Showdown and item art from PokeAPI at runtime; we fetch one input-independent atlas before a save can be chosen, and nothing after (SEC-001).
      - Its sprites have no accessible treatment; ours are decorative, with text and names kept.
      - It trims margins and keeps an LRU cache of decoded images; one resident atlas makes both unnecessary here.
    - **Not adopted:** network art and the downloadable pack (SEC-001); artwork for species 9xx (not Gen 6); held-item icons (M13); the legality dot (M8); lock and mark badges (MVP+); shiny-over-gender priority (Gen 6 has shiny images for every gendered sprite).
- **M6 Export flow + dirty model.** Separate `draftDirty` / `hasChangesSinceOpen` / `changesSinceLastExport`. Export shows "Download started — verify your file". Replace/close of a changed session asks for Export / Discard session / Cancel, and requires "Continue; I have checked my export" before continuing (EXP-001–003, SESSION-003/005/007). E2E: open → no-op export → reopen = native bytes.

  **M6 status:** code complete on `web/m6-export-flow`.
  - **Dirty model** (`State/ExportStatus`, `SaveSession.ExportStatus`): `Unchanged`, `NotExported`, `ExportedCurrent` (`ExportedRevision == Revision`) and `ChangedSinceExport`, worked out from `HasChangesSinceOpen`, `Revision` and `ExportedRevision`.
    - `HasChangesSinceOpen` stays true after a download, so `#session-state` still says "Edited in memory".
    - The leave warning (`HasUnsavedWork`) stays armed after a download, as `PKHeX.Web.md` asks.
    - A download of an unchanged session leaves it `Unchanged`; a later apply makes it `ChangedSinceExport`, because that download holds the original.
  - **Leaving a session** (`State/SessionExit`, `WorkspaceState.Exit`/`ExitStage`): `Replace(candidate)` or `Close`; M16 adds Reset. The stage is computed from the live session and draft on every read, never stored:
    - `ResolveDraft` (draft dirty or refused) → `ResolveSession` (changes not covered by a download of the current revision) → `ConfirmExport` (the current revision was downloaded).
    - `Ready` covers an exit left with nothing to lose by an editor action (a draft cancelled in the editor). It waits for the user, so an editor action never closes or replaces the save by itself.
    - An apply after the download takes the exit back to `ResolveSession`, so the confirmation never covers changes the file does not hold. The planned per-exit export record was dropped for this: a download of the current revision made before the exit also leads straight to the confirmation, which still has to be given explicitly.
    - Each step (`ApplyDraftForExit`, `DiscardDraftForExit`, `ConfirmExportChecked`, `DiscardSessionForExit`) checks its stage and completes the exit as soon as nothing is left to lose. A request on an unchanged session completes at once.
    - `CancelExit` keeps the session, draft and editor. A file opened during an exit replaces the candidate, and turns a close into a replace, at the same stage. Refused files change nothing.
    - `Pending`, `OfferReplacement`, `ConfirmReplace` and `CancelReplace` are removed.
    - `WorkspaceState.ApplyDraft` now holds the apply-then-reselect that the editor's Apply did, so the exit's draft step shares it.
  - **Download** (`Workspace.ExportAsync`, shared by the Download button and the panel): `SaveExporter.Export`, unchanged (clone → `Write` → reopen through the loader), then the Blob download, then `MarkExported`. A failure records nothing, so the exit stays at `ResolveSession`. "Download again" retries from the same revision.
  - **Naming** (WEB-SESSION-007, deferred from F4). **User decision:** the edited name is the default, stamped with the user's local date and time.
    - `FileNaming.EditedName` gives `{stem}-modified-{yyyy-MM-dd-HHmmss}{extension}` (`main` → `main-modified-2026-10-01-143205`), with invariant digits. The stem is shortened first, so the stamp and extension survive the 120-character cap, and the result is stable under `Sanitize`. The `-modified` suffix is PKForge's.
    - `Services/ExportNaming` defaults to `Edited` once changes are applied and to `Original` while the session is unchanged, because those bytes are the original's.
    - The time comes from an injected `TimeProvider` (`GetLocalNow`); the runtime takes its zone from the browser. The stamp is taken when each download starts.
    - A radio group offers both names with their current values. `#rename-note` appears whenever the chosen name is not exactly `main`.
  - **UI.**
    - `Components/SessionStatusText` holds every string.
    - `#export-state` is a new indicator, and `#close-session` ("Close save") a new button in a "Download" section.
    - `Components/SessionExitPanel` replaces the `#replace-*` section. It shows one step's buttons (`#exit-apply-draft`, `#exit-discard-draft`, `#exit-export`, `#exit-discard-session`, `#exit-continue`, `#exit-cancel`) and names the waiting file in `#exit-name`. Its heading takes focus at each new step; focus trapping is M18.
    - The panel takes the exit, stage and apply availability as parameters. It first read `WorkspaceState` directly, and Blazor did not re-render it after a step, because none of its parameters had changed; the E2E close test caught this in all engines.
  - **Tests.**
    - **Unit 398** (up from 360):
      - `WorkspaceStateTests`: the replacement test was rewritten, and 8 exit tests added: immediate completion, the draft → session → download order, an apply after the download, an earlier download, discarding the draft, Cancel, a file opened during a close, and `Ready`.
      - `SessionTests`: every `ExportStatus` transition, including a no-op apply.
      - `FileNamingTests` (+20): stamp and extension, an earlier stamp replaced rather than repeated, invariant digits under de-DE/ar-SA/hi-IN/th-TH, the cap with no extension, a short one and a 16-character one, surrogate pairs, reserved characters and device names.
      - `SessionStatusTextTests` (5): pinned strings, and no download text says "saved".
      - `SessionExitPanelTests` (5, bUnit): the buttons per step, focus moves only on a new step, a refused draft can only be discarded, busy disables every button, and a markup name is rendered as text.
    - **E2E 107** (up from 95): `ExportFlowTests`, 3 engines × 2 paths.
      - `DownloadsAreTrackedNamedInLocalTimeAndReopenUnchanged`:
        - open → no-op download (`main`, native bytes) → reopen → the same bytes again;
        - every `#export-state` value;
        - the edited name stamped in the browser's zone, run with Playwright `TimezoneId` `Pacific/Kiritimati` (UTC+14);
        - the original name with no rename note;
        - the leave warning still firing after a download.
      - `LeavingAChangedSessionNeedsAnExportOrAnExplicitDiscard`: Close at the draft step with focus on the heading, then Cancel; a waiting file named; Discard draft; no Continue before a download; a panel download; Continue offered; an apply after Cancel removes it; Discard session closes; an unchanged session closes at once; Apply draft → download → Continue opens the waiting file. Focus returns to Close save after a cancelled close, and to `#open-title` once an exit completes. 120-character names cause no horizontal scroll at 375 px.
      - The existing pending-replacement and RealSave replace flows were moved to the new ids (`#exit-name`, `#exit-cancel`, `#exit-continue`). `ProofPage` gained `DownloadEdited`/`DownloadNamed`, and `BootAsync` an optional `timezoneId`.
    - **RealSave 14** pass. The edited download is checked as `main-modified-<stamp>`, and replacing the session with its own export now needs only the confirmation.
    - **Mutation checks:**
      - counting any earlier download as current fails 2 Unit tests;
      - not recording the download fails all 12 `ExportFlowTests` cases;
      - stamping in UTC fails the time-zone test in all 6 cases;
      - removing the wrapping CSS fails the 375 px check in all 6 cases;
      - removing the focus restore fails the focus check in all 6 cases.
    - **Other checks:** the trim baseline is unchanged (38), and the `PKHeX.slnx` Release build has 0 warnings. A manual keyboard pass on the published app at 375 px over the private XY save (Close → Download save → Continue): focus lands on the panel heading at each step, and there is no horizontal scroll.
  - **Adversarial review** (after the dev work, with its own probes in a scripted browser).
    - **Fixed:**
      - The panel did not re-render between steps (found by E2E during development).
      - A held Close left the previous status message in place.
      - Long file names: an unbroken 120-character name in the name choice and the exit panel widened the page to 989–1,662 px at 375 px (probed). The fix wraps `#message`, `#exit` and `#download-name` anywhere, and sets `min-width: 0` on the fieldset.
      - Focus dropped to the page body when the panel went away after Cancel, Continue or Discard (probed in Chromium). Focus now returns to Close save after a cancelled close, and otherwise to `#open-title` (now `tabindex="-1"`), above the status message. An exit that completed at once never moved focus.
      - Stamps stacked when an edited download was reopened and edited again (`main-modified-…-modified-…`), until the cap cut the stem. An existing ASCII-digit stamp at the end of the stem is now replaced.
      - An exit step that no longer matched its stage threw an uncaught `InvalidOperationException`, which would have shown the fault screen. Every step now goes through `RunExitStep`, which reports a refusal instead. This was not reproduced (Blazor re-renders before a second click reaches a removed button); it is hardening.
    - **Recorded, not changed:**
      - The name preview shows the time of the last render; each download takes its own stamp, and the label says so.
      - The name choice resets when a session is opened or closed.
      - Discard draft closes the editor rather than reopening the slot, so after a later Cancel the user reselects it.
      - If an apply succeeds but reopening the slot then fails (an entity that passed the staged read-back check would have to fail `Select`), the old draft stays and Apply reports `StaleDraft`; Discard draft still works.
      - Unverified, and not new in M6: Chromium may ask before allowing a second automatic download when the click's user activation has expired, which could make "Download again" wait on a permission prompt while the page says a download started. Headless Playwright allows it. Belongs to G-C's real-browser runs.
  - **Not verified yet:** no screen reader; physical devices (G-C); how Safari and mobile browsers treat the suggested name.
  - **Compared with PKForge:**
    - **Matches/adopted:** the `-modified` suffix with the extension kept (`BoxBrowserPage.ExportModifiedSaveAsync`).
    - **Stricter:**
      - PKForge falls back to `.sav` when there is no extension, which would break `main`; we never add an extension.
      - It reports "Exported X" as done; we say only that a download started.
      - It shows `error.Message`; we show typed text.
      - It shares `session.Serialize()` without reopening it; `SaveExporter` reopens and checks the output.
      - It has no download tracking and no replace/close confirmation, because it writes in place with a backup per write (`SafeSaveWriter`), and its quit prompt relies on those backups.
    - **Out of scope:** in-place writes, backups and restore points, and the Android share sheet.

### Slice B — editor, legality, apply
- **M7 Draft + capability model + inspector.** `SaveCapabilities` comes from the concrete type + the release allowlist (`SAV6XY`, `SAV6AO`). The read-only PK6 inspector has sections Identity, Stats, Moves, Origin/Trainer and Advanced, showing PID/EC, shiny, OT, met, ribbons count, etc. It uses session-scoped `FilteredGameDataSource` lists. There is no reflected property grid (PKM-001).

  **M7 status:** code complete on `web/m7-inspector`.
  - **Capabilities** (`State/SaveCapabilities`, built once per session as `SaveSession.Capabilities`): the intersection of the concrete Core save type and the release allowlist. `SupportedFamily` now also lists the entity type (`PK6`), the draftable fields (`State/EditableFields`: `Nickname` only) and `WritesParty` (false until M9), so the About panel, the loader and the capabilities share one source. `For` refuses a type the release does not open, or one whose `BlankPKM` is not the listed entity type.
    - `CanApply(slot)` replaces the hard-coded `!Slot.IsParty`; the refusal is still `PartyApplyNotAvailable`.
    - `Lists` is a `FilteredGameDataSource` per session, never assigned to `GameInfo.FilteredSources`, so opening a save changes no global Core state.
  - **Draft** (`State/EditorDraft`): owns a private working `PK6` clone that only typed edit methods change; readers get a copy through `Preview()`. `IsDirty` compares every stored byte. `Editable` is empty for a draft that cannot be applied, so the UI offers nothing that could never be kept; `EditNickname` is gated on the family's fields (new `SessionError.FieldNotEditable`).
    - Returning to the stored nickname copies the stored name bytes back, including those after the terminator, so a flag-only change or a typed-and-deleted name never rewrites the encoding (WEB-PKM-003's "untouched encoded data remains preserved"). Core's `SetString` leaves bytes past the new terminator alone, so this matters only after a longer intermediate name; the test covers that case.
    - `SaveSession.Apply` and `SaveExporter` now require the slot to read back byte for byte (`SaveSession.StoresExactly`, stored-format bytes with a valid checksum) rather than the nickname fields only. With `EntityImportSettings.None`, Core's `SAV6.SetPKM` (which would reset boxed Furfrou/Hoopa forms) is skipped, so no Core normalisation is expected on this path.
  - **Inspector** (`Services/EntityInspection`, `Components/InspectorText`, `Components/EntityInspector`): five sections, Identity, Stats, Moves, Origin and trainer, Advanced, read through typed members only (no reflection; ribbons via `IRibbonSetRibbons.RibbonCount`, not `RibbonInfo.GetRibbonInfo`, which is `RequiresUnreferencedCode`). Names come from Core's strings; a value Core cannot name shows as "Unknown (stored value N)", and one outside the session's lists keeps Core's name with "(not available in this game)". Party members show stored stats, HP and status; boxed Pokémon show stats calculated by `PKM.GetStats`, which writes nothing. An egg's friendship field is shown as its hatch counter. Characteristic and Hidden Power come from Core. Stats and moves are captioned tables with row headers; everything else is a `<dl>`. The view is rebuilt when the draft changes, not per render, and shows unapplied edits. The draft section's heading is now "Selected Pokémon", with the nickname controls under "Nickname draft".
  - **Tests.**
    - **Unit 451** (up from 407): `SaveCapabilitiesTests` (8: XY/ORAS mapping, lists filtered to the save, global lists untouched, a list set per session, unopened types refused, every family's entity type), `EntityInspectionTests` (21: each section against Core, box stats calculated without changing the entity, party stats/HP/status as stored, the Generation 5+ status encoding, no HP against calculated stats, unknown and out-of-game values, out-of-range values in every field render without throwing, box and party), `InspectorTextTests` (11, including bUnit: an egg's hatch counter, fixed headed sections, unique ids, invariant numbers under de-DE, stored names as text, re-render on a new inspection), and `SessionTests` (+4: detached preview, stored nickname bytes restored, a refused edit leaves the draft unchanged, byte-for-byte read-back).
    - **E2E 113** (up from 107): `InspectorBrowserTests`, 3 engines × 2 paths: 20 values from every section against native Core, a drafted nickname shown at once and a refused one not, Cancel restoring it, a party member's stored stats and HP, no horizontal scroll at 375 px, no network or storage. The sprite cases ran against a publish with sprites.
    - **RealSave 14** pass; the round trip now also compares every inspector row with native Core on the real slot (values withheld from messages). Across all 968 entities of both private saves, no inspector value falls back to "Unknown" or "not available".
    - **Mutation checks** (each failed as expected, restored from a scratchpad copy): calculating stats on the entity itself, `CanApply` always true, assigning the session lists to `GameInfo.FilteredSources`, dropping the unknown fallback, dropping the out-of-game mark, rewriting the nickname instead of restoring its bytes (survived at first with a short intermediate name; the test now uses a longer one), and not rebuilding the inspector after an edit (E2E, Chromium).
    - **Other checks:** trim baseline unchanged (38); `PKHeX.slnx` Release 0 warnings. Screenshots at 1280 and 375 px found table cells breaking mid-word (`overflow-wrap: anywhere` lowers the cells' minimum width); cells now wrap only between words, with tighter padding.
  - **Adversarial review** (after the dev work, with throwaway probes against Core and the private saves).
    - **Fixed:**
      - A party member's status was decoded with the Generation 1-4 flags (`StatusCondition`/`GetStatusType`), but PK6 stores a Generation 5+ `StatusType` value in the low byte, as WinForms' `StatusConditionView` reads it. Paralysis, Burn and Poison all showed as Sleep, and the unit test had pinned the wrong encoding. The private saves' party members all have no status, so RealSave could not catch it. The status is now the low byte, named from a fixed table (no `Enum.GetName`, which leans on reflection metadata).
      - After any accepted edit the Advanced section said "Checksum: Invalid", because edits do not refresh the in-memory checksum. The inspected copy is now refreshed, as a write would, so the row shows what an apply stores.
      - An egg's friendship field holds its hatch counter; it was labelled "Friendship … of 255". It is now labelled as the hatch counter, as WinForms does, in Identity and Origin.
      - A party member without stored stats showed its stored HP against the calculated maximum; HP is now shown only with stored stats.
      - The inspector note now says a refused edit is not shown, and the moves table caption no longer repeats the heading.
      - Each fix was mutation-checked: putting the old code back fails its new test.
    - **Checked, not changed:**
      - The one real-save value matching "Unknown" is XY's own location name "Unknown Dungeon", so the claim that nothing falls back holds. Out-of-range values in every field render without throwing; `PersonalTable6AO` clamps unknown species.
      - With `EntityImportSettings.None`, `SAV6.SetPKM` is skipped, so the byte-for-byte read-back cannot trip on Core's boxed Furfrou/Hoopa form reset.
      - The inspector is rebuilt on every path that changes the draft (open, edit, apply, cancel, exit steps, session replace, fault recovery).
    - **Recorded, not changed:**
      - `EditNickname` is gated on the family's fields, not on `Editable`, so a party draft can still be edited in memory (the UI makes it read-only and Apply refuses it). Gating it on `Editable` would leave Apply's party refusal unreachable from a dirty draft; M9 revisits both.
      - Boxed eggs' nicknames are editable, as before M7. M10 (nickname/language) should decide that.
      - Ribbon count is Core's `RibbonCount`, which excludes the contest and battle memory ribbon counters.
      - Every export now builds a second session's lists when it reopens the output; this is small and is measured with the rest in M21.
  - **Not verified yet:** no screen reader; physical devices (G-C).
  - **Compared with PKForge** (`MonSummary`, `MonSummaryService`):
    - **Matches:** the section split (Info/Stats/Moves/Origin), base stats with IVs and EVs, characteristic, Hidden Power, ribbon count, Pokérus and markings, and leaving a value unnamed rather than inventing one.
    - **Stricter:** PKForge hand-writes the characteristic table and computes the tie-break itself; we use Core's `Characteristic` and strings. It wraps each section in a `Try` that swallows exceptions and hides the row; we gate by capability and read typed members, and a test shows out-of-range values in every field render without throwing. We count ribbons with Core's typed `RibbonCount`, not Core's reflection-based `RibbonInfo`. It falls back to `#id` for unknown names; we keep the stored value and say it is unknown, and mark values outside the game's lists.
    - **Not adopted:** ribbon names (WEB-PKM-023, Post-MVP); move type, power, category and effect text (M13); Tera type and marks (not Gen 6); L/R navigation between summaries (M18, if wanted); the legality verdict on the summary (M8).

- **M8 Legality service.** Analysis runs on a draft clone with `working.Personal` + slot type, tagged with the revision. An edit marks the result stale immediately. Refresh is manual plus a 300 ms idle debounce. Pending, Valid, Invalid, Unavailable and Stale are separate states. The report shows Core severity with a text + icon summary and an expandable detail. The "not an online acceptance guarantee" note is included. Results from superseded revisions are dropped (LEGAL-001–003, PERF-004).
- **M9 Generalised apply transaction + party.**
  - Apply stages on a `working.Clone()` and runs `ISlotInfo.CanWriteTo` for slot + entity with `EntityImportSettings.None`. It verifies the stored slot, checks party count is unchanged, and swaps atomically with a revision bump. A no-op apply is not a change.
  - Covers party slots too. It implements the **PK6 party-stat policy** from `PKHeX.Web.md` §State model: non-stat edits keep stored stats/HP/status; stat-affecting edits recalculate, keep status, and clamp HP to min(prev, newMax); fainted stays at 0. An HP-reduction preview is shown.
  - Structural slot diff before the swap (from PKForge `WriteSafety.CheckWriteSafety`): compare every party and box slot of the working save and the staged candidate. Refuse the apply if any slot outside the targeted one changed, or if a readable entity became unreadable. This moves the slot-level part of the proof test's `AssertOnlyRangeDiffers` guarantee into the app. The slot diff does not cover non-slot blocks (dex, records), so keep the whole-file byte-range check as a test oracle.
  - Failure-injection tests (SESSION-002, TEST-003).
- **M10 Nickname/language + friendship.** Covers the nickname flag, Core encoding/length checks with no silent truncation, language change shown with its default-name implications, and the labelled OT friendship value (PKM-003, 006).
- **M11 Level/EXP, nature, stats/characteristic.** Level and EXP stay in sync through `Experience`. PK6 nature is independent of PID. Calculated stats and characteristic are shown on the clone (PKM-005, 007, 014).
- **M12 IVs/EVs.** Per-stat and total EV limits from Core, with no silent clamping. Changes mark legality stale and recalculate stats (PKM-013).
- **M13 Held item, moves, PP/PP Ups.** Uses Core-filtered item/move lists. Empty moves are allowed. PP rules come from Core. The UI never claims a move is learnable (PKM-010–012).
- **M14 Ability/slot + gender.** Ability choices use `GetAbilityList(PersonalInfo)` and slot mapping. Gender follows species rules (PKM-004, 009).
- **M15 Species/form.** Uses the Phase 1 helper (depends on R2; carry the commit if the PR is not yet merged). The confirmation preview lists the returned changed-field flags (PKM-002).
- **M16 Acknowledgements + lifecycle.**
  - An Invalid or Unavailable legality result needs explicit acknowledgement before apply/export.
  - Reset to original re-parses a fresh copy.
  - Discard session is a separate destructive action.
  - A `beforeunload` warning is shown only while edited.
  - A `pageshow` `persisted` event (page restored from the back-forward cache) clears the session.
  - E2E covers reload, back-forward cache and storage emptiness (SESSION-004/005, SEC-002, BROWSER-003).

### Slice C — hardening, polish, qualification
- **M17 Diagnostics + hostile input.** An opt-in diagnostic preview/copy/download holds build, browser, operation and a sanitised error code, with no save data. E2E renders hostile filenames and nicknames as text, and checks the CSP is honoured with no inline script (SEC-003, SEC-005).
- **M18 Responsive + accessibility.** Desktop split panes, collapsible tablet panes, and a stacked mobile editor with a return action. Dialog focus is trapped and restored. Validation focuses the summary and then the field. Includes reduced motion, 44px targets, contrast tokens and 400% reflow. Automated axe check in Playwright (bundle `axe-core` in test assets only) (APP-003, A11Y-002/003).
- **M19 Full published journey E2E.** 3 engines × 2 paths with synthetic fixtures. Picker and drop → select box + party → edit one field from each group → legality → apply → export → reopen → assert fields plus unchanged bytes outside the slot and checksum regions. Privacy trace and keyboard-only run. The RealSave tier gets the same journey for local G-D runs (TEST-004/005).
- **M20 Hosting + release.**
  - Checked-in `wwwroot/_headers` (Cloudflare: CSP, `nosniff`, referrer policy, immutable cache for fingerprinted assets, revalidate for `index.html` / boot json) and a meta-CSP fallback.
  - `PKHeX.Web/README.md` becomes a self-hosting guide (root + `/PKHeX/`, nginx/Apache snippets, no `file://`).
  - Optional `workflow_dispatch` deploy job behind a protected environment, promoting the CI artifact. It is not enabled for upstream (HOST-002/003).
- **M21 Qualification + support report.** Memory/peak measurements on the largest admitted fixture and a repeated-session trend (PERF-002). Name the reference desktop (CPU model, memory, OS, browser), run the F8 `Perf` tier on it, and record the WEB-PERF-001 verdict against the `PKHeX.Web.md` startup targets: met, or a documented, scoped limitation. Physical-device runs come from G-C (BROWSER-001/002). Update `PKHeX.Web.md` status/compatibility matrix (XY/ORAS → **P** only where proven) and publish a support report modelled on `PKHeX.Web.WasmProof.md`.

MVP exit = every "Must / MVP" story in `PKHeX.Web.md` §Prioritised implementation matrix is covered by a chunk above and its test, and gates G-B/G-C are resolved or documented as reduced scope.

## Verification (every chunk)

- `dotnet build PKHeX.Web/PKHeX.Web.csproj -c Release` and `dotnet test Tests/PKHeX.Core.Tests` (Core-touching chunks).
- `dotnet test Tests/PKHeX.Web.Tests --filter Category=Unit`.
- `dotnet publish PKHeX.Web/PKHeX.Web.csproj -c Release -o $OUT`, then `--filter Category=E2E` with `PKHEX_WEB_TEST_TIERS=E2E` and `PKHEX_WEB_PUBLISHED=$OUT/wwwroot`.
- Before Phase 3 PRs and after M9/M15/M19: `--filter Category=RealSave` with `PKHEX_WEB_TEST_TIERS=RealSave`, `PKHEX_WEB_PUBLISHED` and the private XY/ORAS env vars (proof README procedure).
- Trim-warning baseline unchanged, or the diff explained in the commit.
- PKForge comparison done and recorded (see "Reference project").
- Phase 1 also needs a Windows WinForms build (F7 job or local) and the R3 Windows checklist (gate G-E).
- Manually drive the app with `python3 -m http.server` on the published `wwwroot` for UI chunks.

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

**F8 Performance baseline (WEB-PERF-001).** A script (`PKHeX.Web/tools/measure.*` or a test) records cold and warm boot under Playwright network throttling (20 Mbps/50 ms) plus artifact sizes, and uploads them as a CI artifact. There is no pass/fail threshold yet.

→ Gate G-A, then open the upstream foundation PR (F1–F8). It must not claim support for anything.

## Phase 3 — XY/ORAS MVP (Goal 4)

Topic branches from `web/foundation`, in the order `PKHeX.Web.md` §"Proposed contribution sequence" gives. The no-op round trip lands first (M1–M6), then the editor (M7–M16), then polish and qualification (M17–M21). Every chunk adds or extends Unit and/or E2E tests.

### Slice A — load, browse, no-op export
- **M1 Shell.** Start screen with privacy copy ("processed entirely on this device…"), supported formats, temporary-state warning, and Open + keyboard-accessible drop zone. About panel shows version/Core commit (F5), licenses and the support matrix. Top-level `ErrorBoundary` with safe reset (WEB-APP-001–004, ERR-003).
- **M2 Load pipeline + error taxonomy.** Typed outcomes: `Empty`, `TooLarge`, `ReadFailed`, `Unrecognized`, `RecognizedNotEnabled(family)`, `IntegrityFailed`, `ParserFault`. The replacement candidate is parsed before the old session is touched, and failure keeps the session (SAVE-001–004, ERR-001/002/004). Extends the proof's failure-path tests.
  - Unchanged round trip at open (from PKForge `SaveEngineSession.ValidateUnchangedRoundTrip`): `Write()` of the freshly parsed, unmodified save must equal the original bytes, otherwise the outcome is `IntegrityFailed`. This catches saves that pass their checksums but that Core would not write back identically.
  - Move user-facing wording out of `State/` and `Services/`: `SaveLoader`, `SaveSession` and `EditorDraft` currently throw `InvalidDataException` with display messages (carried over from the proof). They return or throw typed outcomes instead, and the UI maps them to text.
- **M3 Overview.** Trainer name, game, language, TID/SID in the save's display format (`TrainerIDFormat`), playtime, money, sanitised filename and size, integrity status. Unknown values are labelled, never invented (OVERVIEW-001/002, SAVE-007).
- **M4 Party + box grid.** Party strip plus the current box only. Box selector and prev/next use Core `BoxCount`/`BoxSlotCount` and box names. The grid is a single tab stop with arrow keys and Enter, plus a list alternative. Slots are labelled with coordinates and species text. Empty slots never open a stale entity (PARTY-001, BOX-001/008, A11Y-001). Slot labels are built in the UI from box/slot coordinates, replacing `SaveSession.SlotLabel`.
- **M5 Sprite catalog.**
  - (a) Build-time generator: it reads the existing `PKHeX.Drawing.PokeSprite` resource images and emits a fixed atlas + JSON manifest into `wwwroot`. It is reproducible, and a provenance note is committed.
  - (b) Runtime `SpriteCatalog` loads the whole atlas before file input is enabled. It resolves species/form/gender/shiny, falls back to a text placeholder, and makes no per-entity requests. E2E asserts identical request traces for two different saves (BOX-004, PERF-003, SEC-001).
  - Until G-B is cleared, the atlas stays behind a build flag that defaults to placeholders.
- **M6 Export flow + dirty model.** Separate `draftDirty` / `hasChangesSinceOpen` / `changesSinceLastExport`. Export shows "Download started — verify your file". Replace/close of a changed session asks for Export / Discard session / Cancel, and requires "Continue; I have checked my export" before continuing (EXP-001–003, SESSION-003/005/007). E2E: open → no-op export → reopen = native bytes.

### Slice B — editor, legality, apply
- **M7 Draft + capability model + inspector.** `SaveCapabilities` comes from the concrete type + the release allowlist (`SAV6XY`, `SAV6AO`). The read-only PK6 inspector has sections Identity, Stats, Moves, Origin/Trainer and Advanced, showing PID/EC, shiny, OT, met, ribbons count, etc. It uses session-scoped `FilteredGameDataSource` lists. There is no reflected property grid (PKM-001).
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
- **M21 Qualification + support report.** Memory/peak measurements on the largest admitted fixture and a repeated-session trend (PERF-002). Physical-device runs come from G-C (BROWSER-001/002). Update `PKHeX.Web.md` status/compatibility matrix (XY/ORAS → **P** only where proven) and publish a support report modelled on `PKHeX.Web.WasmProof.md`.

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

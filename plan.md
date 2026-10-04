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

- **G-A Maintainer interest:** before the Web foundation PR goes upstream, ask through the CONTRIBUTING channels. Present the static/client-only goal, the proof results, asset/crypto implications and maintenance ownership. Refactor PRs are useful on their own and do not wait on this. Also raise: `dotnet.native.js`/`.wasm` are Emscripten output (MIT/NCSA), and the runtime pack's upstream notices do not mention Emscripten; ask whether the Web notices should add it or whether the .NET notices are considered sufficient. Also ask where Web CI should live. Upstream's CI is an Azure Pipelines classic pipeline (`project-pokemon/PKHeX`, definition 1), defined in the Azure DevOps UI with no file in the repository, so only maintainers can change it. The options are to accept `.github/workflows/web.yml` (GitHub Actions is already enabled upstream: `submit-nuget` runs there), or to add equivalent steps to the Azure pipeline, for which we supply the step list. Mention that its VsTest step (`**\$(BuildConfiguration)\*test*.dll`) matches no assembly (`No test sources found`), because the DLLs sit under `bin\Release\net10.0\`, so no test has run upstream in CI; `**\bin\Release\**\*Tests.dll` would fix it. Also ask about public deployment (HOST-003, deferred from M20): which host serves the official site (Cloudflare Pages, which reads the shipped `_headers`; GitHub Pages, which cannot send headers, so only the meta CSP would apply; or another), under which domain, whether a `workflow_dispatch` deploy job that promotes a tested CI artifact from a protected environment belongs upstream or only on a fork, and who holds the deployment credentials. The deploy job is written once this is answered.
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

**F3 status:** code complete on `web/f3-test-tiers`. `PublishedAppFixture` (an xUnit collection fixture) owns the static host, Playwright, and browsers cached per engine. `BootAsync(engine, prefix)` checks static-only boot requests and deployment headers on every boot response. Those are the MIME type per extension (an unmapped extension fails), `nosniff`, the CSP header and `Referrer-Policy`. Boot also fails on a request that got no response or on any CSP violation. Violations are reported by an init script to a context-level binding, so they survive reloads, and every engine is checked. Recording is swapped out atomically before the boot checks, so no request falls between the boot check and the after-boot check. `AssertStaticBootAsync` rechecks a reload as a second boot. `RealSaves` withholds the private path from IO errors and from `ToString`. `AppSession.AssertNoNetworkOrPersistenceAsync` checks that nothing reached the network after boot, that storage is empty and that no violations occurred. Cached browsers are replaced if they disconnect. Dialogs are still auto-accepted, but their types are recorded in `AppSession.Dialogs` for M16's `beforeunload` tests. A Unit test keeps the host CSP header equal to the `index.html` meta CSP plus the header-only `frame-ancestors`. The header check still exercises the test host, not a shipped deployment config; M20 should make `StaticHost` serve the checked-in `_headers` so the same assertions cover it (done in M20). Boot timings come from a browser process reused across tests, so F8 must launch its own browser for cold-boot numbers. The proof-UI driver and byte oracles moved to `ProofPage`, which M1–M6 will replace. `RealSaves.Read` is the single RealSave entry point, with native preflight. `TestEnvironment.Required` makes a selected E2E/RealSave tier fail, not skip, without its inputs; this was verified with the variables unset. A `NoTestIsSkipped` guard forbids static skips, on facts and on data rows. The new E2E test `BootsWithDeploymentHeadersAndNoPersistence` (3 engines × 2 paths) also checks the base href. Tier counts: Unit 17, E2E 12, RealSave 14, all passing. Compared with PKForge: real saves likewise come only from env vars and are never committed. PKForge returns early and passes when a fixture variable is missing, and CI runs all its tests untiered; we are deliberately stricter. PKForge has no browser or hosting tests, so there is nothing to adopt.

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
- **Artifact checks.** These are the E2E test `PublishesOnlyStaticDeployableFiles`, not YAML, so they run locally too. It enforces ≤ 20,000 files, each ≤ 25 MiB, and an allowlist by folder taken from the actual publish: `_framework/` holds `.wasm .js .dat`, `licenses/` holds `.txt`, and the root holds `.html .css .js` plus `LICENSE.txt` and `THIRD-PARTY-NOTICES.md`; no other folder is allowed. Every `.br`/`.gz` must sit next to the asset it compresses. Mutation checks failed as expected: a `.map` file, a 26 MiB file, an extensionless `main`, an orphan `.css.br`, a root `save.dat`, a root `notes.txt` and `_framework/sub/a.wasm`. M20's `_headers` (and any `_redirects`) will need adding to the root list (M20 added `_headers` and `404.html`).
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
  - sends `immutable` for fingerprinted `_framework` files (a name heuristic) and `no-cache` otherwise, which is M20's planned `_headers` policy. M20 should replace the heuristic with the checked-in file (done: the host now serves the published `_headers`).

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
    - The fingerprint rule is a name heuristic until M20 (replaced by the published `_headers` in M20).
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
      - `EditNickname` is gated on the family's fields, not on `Editable`, so a party draft can still be edited in memory (the UI makes it read-only and Apply refuses it). Gating it on `Editable` would leave Apply's party refusal unreachable from a dirty draft; M9 revisits both. *(M9: party drafts are now applicable in XY/ORAS; the gate stays on the family for families without party writes.)*
      - Boxed eggs' nicknames are editable, as before M7. M10 (nickname/language) should decide that.
      - Ribbon count is Core's `RibbonCount`, which excludes the contest and battle memory ribbon counters.
      - Every export now builds a second session's lists when it reopens the output; this is small and is measured with the rest in M21.
  - **Not verified yet:** no screen reader; physical devices (G-C).
  - **Compared with PKForge** (`MonSummary`, `MonSummaryService`):
    - **Matches:** the section split (Info/Stats/Moves/Origin), base stats with IVs and EVs, characteristic, Hidden Power, ribbon count, Pokérus and markings, and leaving a value unnamed rather than inventing one.
    - **Stricter:** PKForge hand-writes the characteristic table and computes the tie-break itself; we use Core's `Characteristic` and strings. It wraps each section in a `Try` that swallows exceptions and hides the row; we gate by capability and read typed members, and a test shows out-of-range values in every field render without throwing. We count ribbons with Core's typed `RibbonCount`, not Core's reflection-based `RibbonInfo`. It falls back to `#id` for unknown names; we keep the stored value and say it is unknown, and mark values outside the game's lists.
    - **Not adopted:** ribbon names (WEB-PKM-023, Post-MVP); move type, power, category and effect text (M13); Tera type and marks (not Gen 6); L/R navigation between summaries (M18, if wanted); the legality verdict on the summary (M8).

- **M8 Legality service.** Analysis runs on a draft clone with `working.Personal` + slot type, tagged with the revision. An edit marks the result stale immediately. Refresh is manual plus a 300 ms idle debounce. Pending, Valid, Invalid, Unavailable and Stale are separate states. The report shows Core severity with a text + icon summary and an expandable detail. The "not an online acceptance guarantee" note is included. Results from superseded revisions are dropped (LEGAL-001–003, PERF-004).

  **M8 status:** code complete on `web/m8-legality`.
  - **Service** (`Services/LegalityService`): runs Core's `LegalityAnalysis` on `EditorDraft.ToStoredEntity()` (a copy), with `working.Personal` and the slot's `StorageSlotType`, after `EnsureOwns`/`EnsureCurrent`. `EditorDraft.Analyze` is removed.
    - Everything comes from Core, through `LegalityLocalizationContext`:
      - the verdict
      - the short report (still the E2E/RealSave oracle)
      - the verbose report
      - the findings: invalid current moves (`FormatMove`), invalid relearn moves (`FormatRelearn`, Gen 6+), invalid checks (`Humanize`), then `Severity.Fishy` checks, each with Core's `Judgement` and `CheckIdentifier`
    - Problems are exactly the lines of Core's short report, in order.
    - **Trainer context.** Desktop calls `ParseSettings.InitFromSaveFileData(sav)` when it loads a save. Some Core checks, such as `HistoryVerifier.VerifyHandlerState`, run only with that active trainer set. The service now does the same for each analysis and clears it afterwards (`LegalityService.InTrainerContext`, serialised by a lock because it is Core global state). Without this, the web said Valid where desktop says Invalid: with `CurrentHandler` flipped, 23 of 547 XY and 118 of 413 ORAS entities in the private saves.
    - **Core's exception guard.** `LegalityAnalysis.cs` starts with `#define SUPPRESS`, so Core catches exceptions inside the analysis and returns `Parsed = false`, writing the cause only to its debug output. The service reports that as `Unavailable` with a stand-in console note (`NotParsedMessage`). An exception outside Core's guard (the personal-table lookup, formatting the reports) is caught by the service: any except `OutOfMemoryException` is `Unavailable` with no text, and the exception goes to the console through `DraftLegality.AnalysisFailed`.
    - An internal constructor takes the analyser, for tests.
  - **Result** (`State/LegalityResult`): `LegalityVerdict` (`Valid`/`Invalid`/`Unavailable`), `LegalityTag(EditorDraft, EditRevision)`, findings, both reports, problem and warning counts. No UI wording.
  - **Edit revision**: `EditorDraft.EditRevision` counts accepted edits; a refused edit leaves it unchanged.
  - **Scheduling** (`State/DraftLegality`, owned by `WorkspaceState.Legality`). **User decision:** analysis runs whenever the draft changes. That covers opening a slot, an accepted edit, an apply or cancel reopening the slot, and the exit's apply step.
    - `Schedule()` waits `IdleDelay` (300 ms) through `Task.Delay(…, TimeProvider, token)` and restarts on every edit.
    - `RunNowAsync()` ("Analyze now") cancels the wait and runs at once.
    - `Status` is worked out from the live draft every time (None, NotAnalyzed, Pending, Stale, Valid, Invalid, Unavailable), so an accepted edit is Stale at once.
    - A run marks itself running (`aria-busy` on the panel) and awaits `Yield`, which in the browser is `browser.js` `nextPaint` (an animation frame, then a timeout, with a 100 ms fallback for hidden tabs). It then calls Core only if its tag is still current, and keeps the result only if it still is. Superseded runs never call Core.
    - A refused edit (`!DraftValid`) is Stale (or Not analyzed) and does not run.
    - `IsRunning` (and so `aria-busy`) is true only for a run of the draft as it is now. A run that an edit superseded during its paint wait does not count.
    - The result is stored before `AnalysisFailed` is raised, and a throwing handler cannot undo it.
    - A draft from an earlier revision of the session (which happens only if reopening the slot after an apply fails) is `Unavailable` rather than Stale for good.
    - `Reset` on open, discard, recovery and every new draft. `WorkspaceState` now takes a `TimeProvider` (the DI singleton) and is `IDisposable`.
    - `AutoRun` exists as the PERF-004 gate. It stays on, because the timings below are well inside the budget.
  - **UI** (`Components/LegalityPanel`, `Components/LegalityText`):
    - `#legality-status` (role=status) shows one word per state beside an `aria-hidden` icon, coloured for light and dark.
    - `#legality-summary` reads "No problems found.", "N problems, M warnings.", Unavailable's "does not mean the Pokémon is legal", or Stale's "analysed again once you stop typing" (or "Choose Analyze now" when `AutoRun` is off, or "the last edit was refused").
    - `#legality-findings` is listed only for a result matching the draft as it is now. Each finding shows Core's text, which already begins with Core's severity word, so none is added. Where `Services/LegalitySections` maps the `CheckIdentifier` to an inspector section, there is a "Show in {section}" link. The link prevents navigation and moves focus to the heading (inspector headings are now `tabindex="-1"`) through `Interop/BrowserPage`, which reuses the already-loaded `browser.js` module, so it makes no request.
    - A collapsed `#legality-details` holds `#legality-report` and `#legality-report-verbose`.
    - The notes are the diagnostic note, "not an online acceptance guarantee", and "Analysed by PKHeX.Core {version}, source commit {12}" (new `BuildInfo.CoreVersion`, read from the running Core assembly).
    - `#analyze` ("Analyze now") is disabled only while a run is in progress or after a refused edit, so it also skips the idle delay after a slot is opened.
  - **Fixtures.** Legality now runs with the save as Core's active trainer, so `SaveFixtures.Synthetic` gives the save the known legal Zigzagoon's original trainer (ID, "Gouki", gender). The ORAS save is now Alpha Sapphire, the Zigzagoon's game, where it is its trainer's own Pokémon and legal. No X/Y save can be its original trainer's, because Core matches the exact game, so in an XY save it is Invalid, as on desktop. Tests that need a legal entity use the ORAS save. Four E2E overview checks now expect "Alpha Sapphire".
  - **Tests.**
    - All comparisons with native Core go through `NativeLegality.Of`, which analyses in the same trainer context, so no oracle can agree with the app by also leaving the trainer out.
    - **Unit 549** (up from 451):
      - `LegalityServiceTests` (47), including `TheSavesTrainerIsPartOfTheAnalysis` (XY and ORAS). With the current handler flipped on every fixture, at least one verdict must depend on the trainer, and every service verdict and report must match desktop's.
        - all 18 PK6 fixtures in Core's legality tests, in XY and ORAS saves: verdict, short report, problems = Core's report lines, warnings = Core's Fishy count
        - a guard that the corpus contains both problems and warnings
        - party slot type
        - nothing mutated
        - the edited draft is what gets analysed
        - exception → Unavailable with the cause handed back
        - OOM not swallowed
        - foreign and stale drafts refused
      - `LegalitySectionsTests` (14): every `CheckIdentifier` mapped, spot rows, every link target is a real inspector heading.
      - `DraftLegalityTests` (17), on `FakeTimeProvider` (new test-only package `Microsoft.Extensions.TimeProvider.Testing` 10.10.0):
        - 299 ms vs 300 ms
        - Stale at once
        - each edit restarts the delay
        - an edit during the paint wait supersedes the run without calling Core
        - a new draft or session drops the result
        - Analyze now cancels the wait and does not repeat a current result
        - refused edits
        - `AutoRun` off
        - dispose
        - apply, cancel and recovery
        - failure reported, and a throwing failure handler keeps the Unavailable result
        - a superseded run is not busy
        - a draft from an earlier revision is Unavailable
        - `Changed` raised
        - the "nothing runs yet" checks wait up to 250 ms for a run to *start*, which a fired timer does at once, rather than for it to complete
      - `LegalityPanelTests` (18, bUnit): every status word and icon, findings with links and click-through, nothing listed while Stale/Pending/Not analyzed, Unavailable wording, Stale wording per mode, Analyze now offered while waiting and disabled while running or refused, `aria-busy` only while running, Core text rendered as text, the engine and guarantee notes, counts wording.
      - `WorkspaceLegalityTests` (3, bUnit): a run calls `browser.js` `nextPaint` before Core; the workspace renders the result when a run completes; a disposed workspace is unhooked.
      - `SessionTests`: the legality cases moved out (−2), plus `EditRevisionCountsAcceptedEditsOnly`.
      - `SaveFixtures.NewState()` (a fake clock that never advances by itself) replaces `new WorkspaceState()` in the existing tests.
    - **E2E 125** (up from 113): `LegalityBrowserTests`, 3 engines × 2 paths.
      - Opening a slot shows the native verdict without a click, with the native finding count, summary, short and verbose reports, the Core version and the guarantee note.
      - A finding link focuses its inspector heading. The whole URL, fragment included, is unchanged and `location.hash` is empty. The first version compared `AbsolutePath`, which ignores the fragment, and passed with navigation allowed.
      - An edit on the illegal entity records, in the page, exactly Invalid → Stale → Pending → native verdict. Pending starts ≥ 250 ms after Stale, and no finding is listed during Stale or Pending.
      - A refused edit is Stale with Analyze now disabled, then Cancel restores it.
      - No horizontal scroll at 375 px with the details open.
      - An apply is analysed again with the applied entity's native report, and a party member gets the party-context verbose report.
      - No network or storage use, and no page errors.
      - The existing proof, inspector, export and RealSave flows pass unchanged with auto-analysis.
    - **RealSave 14** pass.
    - **Perf 2** (up from 1). New `LegalityTimingTests` (`legality-timing.md`/`.json`, in the same non-parallel collection as the boot baseline). It covers the same 18 fixtures × XY/ORAS, opened through the app's idle path in each engine on loopback. It times `aria-busy` to the verdict render, records the first analysis per page apart, and checks every verdict against native. CI's renamed "Web performance baseline" step appends it to the run summary. Local results (Apple M4, 10 logical CPUs, 16 GiB, macOS 26.5):

      | Engine | First analysis | Warm p50 | Warm p95 | Warm max |
      |---|---:|---:|---:|---:|
      | Chromium 153 | 310 ms | 17 ms | 44 ms | 56 ms |
      | Firefox 155 | 380 ms | 13 ms | 45 ms | 56 ms |
      | WebKit 26.6 | 288 ms | 13 ms | 42 ms | 44 ms |

      The p95 ≤ 500 ms target is met with room to spare, and no warm analysis exceeds 200 ms. So the 300 ms debounce stays on, as `PKHeX.Web.md` allows "only if measurements show acceptable responsiveness". The first analysis of a page (Core loading its legality tables) is one stall of about 300–400 ms, not repeated. This is not the named reference desktop; M21 owns the verdict.
    - **Mutation checks** (each restored from a scratchpad copy):
      - dropping the supersede checks → fails 1 Unit test
      - not bumping `EditRevision` → fails 6
      - an idle timer that is not restarted by later edits → fails 1, but only after the fix below
      - not catching analysis exceptions → fails 2
      - showing an older tag's verdict as current → fails 2
      - dropping warnings → fails 7
      - passing `Result` instead of `Current` to the panel → fails the E2E "no findings while stale" check (Chromium, both paths)
      - after the second review:
        - analysing without the trainer context → fails 39 Unit tests
        - raising the failure event before storing the result, with a handler that throws → fails 1
        - counting a superseded run as busy → fails 1
        - dropping a stale draft's run silently → fails 1
        - not wiring the paint wait → fails 1 (bUnit)
        - disabling Analyze now while merely waiting → fails 1
        - the non-restarting idle timer, against the new run-start wait → fails 1
        - removing the link's `preventDefault` → fails the E2E link check (Chromium, both paths)
    - **Other checks:** the trim baseline is unchanged (38), and the `PKHeX.slnx` Release build has 0 warnings. Screenshots at 1280 and 375 px over the private XY save show Valid with a Fishy warning, Stale after an edit, and no horizontal scroll. The workflow is actionlint-clean.
  - **First review** (after the dev work, with screenshots and mutations).
    - **Fixed:**
      - The idle-timer mutation survived at first: the fake clock fires timers at once but their continuations are posted, so asserting straight after `Advance` could not see a run. Tests now let fired runs settle (`Pass`).
      - The E2E Stale check used a legal entity with no findings, so it could not see findings leaking into the stale state. It now runs on the illegal entity and records the findings count at every status change.
      - The timing harness threw from Playwright's `PageError` event thread. It now counts errors and fails afterwards.
      - `display: flex` on the details summary hid its disclosure marker.
      - A "Warning:" label duplicated Core's own "Fishy:"/"Invalid" prefix.
      - "0 problems, 1 warning" now reads "No problems, 1 warning".
    - **Recorded, not changed:**
      - The tests show "Pending" is in the DOM before Core runs, but not that it was painted. A `MutationObserver` sees DOM changes, not frames. The paint wait is `requestAnimationFrame` then a timeout.
      - `#legality-status` is a live region, so a screen reader hears Stale → Pending → verdict after each pause in typing. M18 should decide whether that is too chatty.
      - `LegalitySections` has a throwing default arm, not "no default arm": C# requires handling unnamed enum values. The Unit guard catches a new Core identifier either way.
      - Real-save timing (968 entities) was not measured; only the committed synthetic corpus is in the Perf tier. M21 measures the admitted corpus on the reference desktop.
      - If reopening a slot after a successful apply ever failed (see M6), the old draft's result would stay shown. It describes the same bytes that were applied.
      - Acknowledging Invalid/Unavailable before apply/export is M16.
  - **Second review** (a separate adversarial review in a subagent, with probes over the private saves and mutations). Every item was fixed:
    - **Missing trainer context:** web Valid where desktop says Invalid (reproduced); now matches desktop.
    - **False claim about Core's exception handling:** the README, `plan.md` and the service remarks said Core does not catch its own analysis exceptions. It does (`#define SUPPRESS`), so Core-caught failures never reached the console; they now get a console note.
    - **Vacuous E2E check:** the URL check ignored the fragment (reproduced by mutation).
    - **Analyze now blocked:** it was disabled during the idle wait after opening a slot.
    - **Lost result:** a throwing failure handler lost the Unavailable result.
    - **Busy too long:** `aria-busy` stayed on for a superseded run.
    - **Stale for good:** a draft from an earlier revision stayed Stale and silently never ran.
    - **Paint wait untested:** nothing checked that the workspace waits for a real paint.
    - **Weak timer wait:** the unit timer wait could hide a bug on a slow machine.
    - **Smaller issues:**
      - doc placement in `SaveFixtures`
      - missing xmldoc on `BrowserPage.DisposeAsync`
      - the duplicated module import, now shared as `Interop/BrowserModule`
      - no guard that the corpus fits in box 1
      - the `InspectorArea.Identity` doc
    - **Not reproduced:** Core's own caught-exception path. 13 malformed entities (out-of-range species, form, version, location, ball, ability, item, move, language) all parsed and were reported Invalid, so the `!Parsed` branch is covered by reading only.
    - **Not changed:** the verbose report is built on every run, even while collapsed. Its cost is inside the timings above.
  - **Not verified yet:** no screen reader; physical devices (G-C); the paint before analysis on slow devices; the first GitHub run of the Perf step with the legality timing.
  - **Compared with PKForge** (`LegalityAssistService.GetReport`, `LegalityAssistUi`):
    - **Matches:** invalid current and relearn move lines plus non-valid and Fishy checks, each with Core's severity; a severity icon and text per line; problems ordered before warnings; Core's localized text rather than rewritten text.
    - **Stricter:**
      - PKForge analyses with `new LegalityAnalysis(pk)` alone; we pass the save's personal table and the slot type.
      - It runs analysis on `Task.Run` with no revision tagging; we tag each result with the draft state and drop superseded runs before they call Core.
      - It shows `error.Message` on failure; we show Unavailable with no exception text.
      - It labels an unsupported game "Not analyzed"; we never show an unexamined or failed state as legal and keep Stale apart.
    - **Not adopted:** fix suggestions and "Legalize" (WEB-LEGAL-006, no AutoMod dependency); per-move verdict rows (M13); encounter text outside Core's verbose report (WEB-LEGAL-004, MVP+); grouping by `CheckIdentifier` (we link to inspector sections instead); `Task.Run` (no threads in WASM).
- **M9 Generalised apply transaction + party.**
  - Apply stages on a `working.Clone()` and runs `ISlotInfo.CanWriteTo` for slot + entity with `EntityImportSettings.None`. It verifies the stored slot, checks party count is unchanged, and swaps atomically with a revision bump. A no-op apply is not a change.
  - Covers party slots too. It implements the **PK6 party-stat policy** from `PKHeX.Web.md` §State model: non-stat edits keep stored stats/HP/status; stat-affecting edits recalculate, keep status, and clamp HP to min(prev, newMax); fainted stays at 0. An HP-reduction preview is shown.
  - Structural slot diff before the swap (from PKForge `WriteSafety.CheckWriteSafety`): compare every party and box slot of the working save and the staged candidate. Refuse the apply if any slot outside the targeted one changed, or if a readable entity became unreadable. This moves the slot-level part of the proof test's `AssertOnlyRangeDiffers` guarantee into the app. The slot diff does not cover non-slot blocks (dex, records), so keep the whole-file byte-range check as a test oracle.
  - Failure-injection tests (SESSION-002, TEST-003).

  **M9 status:** code complete on `web/m9-apply-party`.
  - **Capabilities:** `SupportMatrix` now sets `WritesParty: true` for XY and ORAS, so party members open for editing (nickname) and can be applied. `PartyApplyNotAvailable` stays for a family without a party-stat policy; `SaveCapabilities.For(save, family)` (internal) lets tests cover one.
  - **Apply transaction** (`SaveSession.Apply`), all on `Working.Clone()`; a refusal at any stage leaves the session (working save, revision, flags, original bytes) as it was:
    1. owns, current, `CanApply`;
    2. a party member without stored stats is refused (new `PartyStatsMissing`): Core's `SetPartyValues` would recalculate them, healing it and clearing its status;
    3. Core's `CanWriteTo` checks (a lone party egg is refused as before, `SlotNotWritable`);
    4. the write, through an internal `StagedWriter` seam (default: `slot.WriteTo(…, EntityImportSettings.None)`), handed a copy of the entity;
    5. the read-back (`StoresExactly`) now compares party-format bytes for a party slot, so stored stats, HP and status must match the draft; box slots are compared in the stored format (box reads carry a party-sized buffer whose tail is not stored);
    6. party count unchanged (new `PartyCountChanged`);
    7. structural slot diff (`State/SlotImages`, from PKForge `WriteSafety.CheckWriteSafety`): all six party positions (also past the count) and every box slot of the working save and the candidate; any change outside the target is refused (new `UntargetedSlotChanged`).
    - The planned "write left the save unchanged" no-op branch was dropped: a dirty draft must read back exactly, so the slot always changes. A no-op apply is still not a change (`!IsDirty` returns early).
    - `SaveExporter` uses the same read-back and also refuses a changed party count (`ExportIdentityMismatch`).
  - **Party-stat policy** (`State/PartyStatPolicy`): `Recalculate` keeps status and HP, calls Core's `ResetPartyStats` (stats + `Stat_Level`), then restores status and sets HP to min(previous, new maximum). Never `Heal`/`HealPP`. `EditorDraft` commits every edit through `Commit(candidate, affectsStats)`; only a party draft with `affectsStats` is recalculated, so the nickname edit keeps the stored battle bytes. **User decision:** the policy and preview ship now; no typed edit affects stats yet, so `EditorDraft.EditForTest` (internal, test-only) reaches it until M11 adds the first stat edit and replaces it.
  - **HP preview:** `EditorDraft.HpChange` (`PartyHpChange`: previous/new HP and maximum, `IsReduction`, `IsFainted`) and `Components/PartyHpPreview` (`#party-hp-preview`, role=status, text in `Components/PartyText`): shown only when HP drops, says the status is kept and nothing is healed, and says when the member will be fainted. Invariant numbers.
  - **UI:** `#party-note` shows only for a family without party writes; opening a party member says its stored stats, HP and status are kept. New user messages for the three new errors. README updated.
  - **Tests.**
    - **Unit 616** (up from 549):
      - `PartyStatPolicyTests` (14, including a member without stats refused): lower level clamps HP, injured not healed by a higher level, fainted stays fainted, status kept (sleep, burn, paralysis), Shedinja's one HP, PP not refilled, only party bytes change, `PartyHpChange.Between` cases. Oracle: Core's `GetStats`.
      - `PartyApplyTests` (12, including a stat edit of a member stored without stats refused at the edit): nickname edit on an injured, burned member keeps every party-only byte and the export equals native Core's `SetPartySlotAtIndex(…, None)` (XY and ORAS); a stat edit follows the policy through apply and export; reduction previewed; fainted stays fainted; a non-stat edit keeps stats; boxed drafts have no HP change; a later party position is written in place; missing stats refused; lone egg refused; egg beside a member written.
      - `ApplyTransactionTests` (30): 9 injected faults × 3 targets (party 1, party 2, box): write reports failure, changes the entity, skips the slot, heals the member, also changes a box slot / another member / the sixth party position past the count, corrupts a neighbour, adds a member. Each asserts the exact refusal, the session untouched (working reference and bytes, revision, flags, original bytes), the draft still dirty, and that the same draft applies once the fault is removed. Plus a writer that throws after writing, a writer that cannot change the read-back reference, and the default writer equal to a native write with `EntityImportSettings.None`.
      - `SlotImagesTests` (3), `PartyHpPreviewTests` (6, bUnit, including the live region kept while its text arrives, checked on bUnit's render diff), `SaveCapabilitiesTests` (+2, party writes; a family without them; a family refused for another save type), `SessionTests` (party select now applicable; the party refusal moved to a family without party writes).
    - **E2E 131** (up from 125): `PartyApplyBrowserTests`, 3 engines × 2 paths: a party nickname edit on an injured, burned member with a second member present. The message says the battle state is kept, no HP preview, apply, legality analysed again as a party member (native verbose report), export byte-identical to native Core, HP and status kept, the other member unchanged, only the party slot and the block-checksum footer differ, no network or storage. `StorageBrowserTests` now expects an editable party member and no `#party-note`. Sprite cases ran against a publish with sprites.
    - **RealSave 26** (up from 14): `RealSavePartyRoundTrip` on the first party member of each private save, 3 engines × 2 paths × 2 families: byte-identical to the native edit, only the party slot and footer differ, the member's other bytes (stats, HP, status) unchanged, party legality matches native, nothing kept by the browser.
    - **Apply timing** (throwaway probe, not committed; 6 applies alternating party/box on loopback): Chromium 6–13 ms, Firefox 9–17 ms, WebKit 5–14 ms. The slot diff (two snapshots of 936 slots on ORAS) is affordable.
    - **Mutation checks** (each restored from a scratchpad copy; Unit tier): no structural diff → 12 fail; stored-size read-back for party → 2; no party-count check → 3; no missing-stats refusal → 1; writer handed the session's own copy → 7; diff skipping party positions past the count → 5; `Heal()` instead of the policy → 9; status not restored → 4; HP left at the new maximum → 5; policy never run → 2; policy run on non-stat edits → 1.
    - **Other checks:** trim baseline unchanged (38); `PKHeX.slnx` Release 0 warnings. Screenshots at 1280 and 375 px (Chromium, private XY save, first party member opened, renamed and applied): the open message says the battle state is kept, the inspector shows the stored HP unchanged after apply, legality re-runs, and there is no horizontal scroll.
  - **Review** (inline, after the dev work; nothing needed fixing):
    - Core's Gen 6 party write path has no override other than `PartyCount`; with `EntityImportSettings.None` only `SetPartyValues` runs, and it keeps present stats.
    - The exit's apply step (`ApplyDraftForExit`) uses the same `Apply`, so a party draft can be applied while leaving a session.
    - `SlotInfoParty.WriteTo` realigns its slot to the party count; `Select` only opens positions below the count, and the read-back uses a fresh slot at the draft's position, so a realigned write cannot pass.
    - The slot diff does not cover entity storage outside the party and boxes (XY/ORAS battle box, daycare). No apply writes there, and the whole-file byte-range oracles in E2E and RealSave cover them.
  - **Adversarial review** (after the first review, with throwaway probes against Core and the private saves, and mutations).
    - **Fixed:**
      - **A party member stored without stats would have been written fainted.** A stat edit ran the policy with no previous HP (min(0, new maximum) = 0), which also gave the draft stats, so Apply's missing-stats guard, which checked the draft, let it through (reproduced: 0/103 HP written). Now a stat edit of such a member is refused at the edit (`PartyStatsMissing`, draft unchanged), `PartyStatPolicy.Recalculate` refuses a member without stats, and Apply checks the stored member rather than the draft. Latent until M11, but the guard was wrong by design.
      - **The HP preview might not be announced.** `#party-hp-preview` was a `role=status` element inserted together with its text, which screen readers do not reliably announce. The live region (`#party-hp-region`) is now always rendered and only its content changes.
      - The `PartyStatsMissing` message said writing "would heal it", which is wrong for a member at full HP; it now says Core would recalculate the stats, restoring HP and clearing the status.
      - Mutations: dropping the edit-time refusal → fails 1; dropping the policy's precondition → fails 1; inserting the live region with its text → fails 2. Apply checking the draft instead of the stored member survives on its own: while every edit goes through `Commit`, a draft can only have stats if the stored member did. It is kept as the backstop for the edit paths M10–M15 add.
    - **Checked, not changed:**
      - All 8 party members of both private saves (6 XY, 2 ORAS) apply and export a nickname edit with no refusal and no slot-diff hit.
      - Their stored stats and `Stat_Level` equal Core's calculation, and the XY and AO personal tables give the same stats for each, so `PK6.PersonalInfo` (AO) is safe for XY saves.
      - Stored HP above the maximum (corrupt) is clamped to the new maximum by a stat edit, and the preview reports that reduction.
    - After the fixes: Unit 616, E2E 131 (republished, with sprites), RealSave 26, trim baseline 38, `PKHeX.slnx` Release 0 warnings.
  - **Recorded, not changed:**
    - A lone party egg can be renamed in the draft, but Apply is refused with the generic "cannot be edited" message (Core's `InvalidPartyConfiguration`). A party of only eggs cannot occur in game; M10 can give it its own message if eggs stay editable. *(M10: eggs are not editable, so this is reachable only through tests.)*
    - The open message for a party member says its stats, HP and status are kept. That is true for every edit in this release; M11 must reword it when the first stat edit arrives.
    - A party member without stored stats can still be opened and edited; only Apply refuses it. No real-save member lacks stats.
    - `PartyApplyNotAvailable` is unreachable in this release (both families write the party); it is kept for later families, which need their own policy.
    - Boxed eggs' and party eggs' nicknames are editable (M10 decides). *(M10: eggs are read-only.)*
    - The HP preview is a live region; M18 decides whether that is too chatty.
  - **Not verified yet:** no screen reader; physical devices (G-C); the HP preview in the live UI (nothing triggers it until M11).
  - **Compared with PKForge** (`WriteSafety.CheckWriteSafety`, `SaveEngineSession.SetEntityCore`, `EvolutionService.Apply`, `MonFieldService`):
    - **Matches:** surgical slot writes with `EntityImportSettings.None`; a per-slot byte diff of every party and box slot that refuses any untargeted change.
    - **Stricter:** the diff runs in the session before the swap, not on serialized bytes afterwards; it includes party positions past the count; the target must read back exactly in party format; party count is checked; failures are typed errors, not text. PKForge calls `ResetPartyStats` after a party form edit (`MonFieldService`), which heals and clears status; we keep status and never raise HP.
    - **Different, to decide in M11/M15:** PKForge's evolution adds the gained maximum HP to current HP (as the game does on level-up), which can revive a fainted member; we follow `PKHeX.Web.md` (min(previous, new maximum), fainted stays fainted).
    - **Not adopted:** route/layout-risk detection for Gen 3 ROM hacks (out of Web scope); multi-slot `WriteScope` (WEB-BOX-005, MVP+).
- **M10 Nickname/language + friendship.** Covers the nickname flag, Core encoding/length checks with no silent truncation, language change shown with its default-name implications, and the labelled OT friendship value (PKM-003, 006).

  **M10 status:** code complete on `web/m10-name-friendship`.
  - **User decisions:**
    - Friendship is two labelled fields, original trainer (OT) and handling trainer (HT). HT is editable only when a handling trainer is stored. `CurrentHandler` is never changed.
    - Eggs are read-only for every M10 field. This settles "eggs' nicknames editable (M10 decides)" from M7 and M9.
    - The name follows the WinForms rules.
  - **Name rules** (`Services/NameRules`, pure). Each mirrors a WinForms method:
    - `FlagAfterTyping` (`UpdateIsNicknamed`): typing a name that is not the species' name in any Gen 6 language sets the flag; typing never clears it.
    - `NameAfterReset` (`UpdateNickname` / `IsPossibleNotNicknamed`): clearing the flag, or changing the language while it is clear, gives Core's default name for the language. A name that is already the species' name in some language is kept, as in the R3 W8 case.
    - `Describe` gives the default name, a name kept from another language, and Core's font check (`StringFontUtil`, the WinForms font warning).
    - The desktop's egg and Gen 5 branches are left out: eggs are not edited, and no Gen 5 format is opened.
  - **Draft** (`State/EditorDraft`):
    - `EditNickname(text, flag)` stays as the exact primitive. `TypeNickname`, `SetNicknamed`, `EditLanguage`, `EditTrainerFriendship` and `EditHandlerFriendship` are added.
    - Every edit commits a candidate through `Commit(…, affectsStats: false)`. Friendship does not affect Gen 6 stats.
    - Name writes share `SetName`. The length, control-character and `NicknameNotRepresentable` checks are unchanged. The stored name keeps its stored bytes, and an unchanged name is not re-encoded.
    - The language must be in the session's `Lists.Languages`. A language change that renames also needs `EditableFields.Nickname`.
    - New errors: `LanguageNotAvailable`, `FriendshipOutOfRange` (refused, never clamped), `NoHandlingTrainer`, `EggNotEditable`.
    - `Editable` is `None` for an egg.
    - `EditableFields` gains `Language` and `Friendship`, and `SupportMatrix` grants all three to XY and ORAS. `SaveCapabilities.SaveLanguage` feeds the font check.
  - **UI:**
    - `Components/DraftEditor` is extracted from `Workspace`, with all wording in `Components/EditorText`.
    - Name fieldset: nickname, flag, a language `<select>` from Core's list (a stored value outside the list is shown as "Unknown (stored value N)"), and `#name-note`, an always-present live region.
    - Friendship fieldset: the OT and HT fields, each label naming its trainer; `#ht-note` when there is no handler; and `#friendship-note`, which says which value the game uses now and that edits never change who holds the Pokémon.
    - Text that does not parse ("", "1.5", "1e2", "-1") is refused like an out-of-range value.
    - An accepted edit reloads every field from the draft, so earlier refused input is replaced. Friendship text that already reads as the drafted value ("071") is kept, so the field is not rewritten under the cursor.
    - Eggs show `#egg-note`, with every field read-only.
    - The open message lists the editable fields (`EditorText.EditableSummary`). The heading is now "Edit" and the button "Apply changes".
  - **Tests.**
    - **Unit 694** (up from 616):
      - `NameRulesTests` (28): a WinForms parity table, other-language species names, species out of range, `Describe`, and the font check against Core.
      - `NameAndFriendshipDraftTests` (28):
        - each edit equals the native Core edit byte for byte
        - clearing the flag restores the stored bytes
        - the W8 language case, the reset of a custom name, and a language round trip leaving the draft clean
        - unlisted languages (0, 6, ChineseS, −1) refused
        - 0, 123 and 255 accepted; 256, −1 and `int.MaxValue` refused unclamped
        - HT accepted (current handler kept) and refused without a handler
        - every egg edit refused
        - per-field family gating
        - a party member (injured, burned, with a stored Attack Core would not calculate) keeps its battle bytes through apply and export, equal to native `SetPartySlotAtIndex(…, None)`, in XY and ORAS
        - a box export equal to native Core
      - `DraftEditorTests` (22, bUnit): labels, HT read-only with `aria-describedby`, egg read-only, the auto-tick, every name-note state, the font warning, refused friendship kept as typed, an accepted edit replacing refused input, the typed text kept, invariant digits under ar-SA, the unlisted language option, the live region, a new draft reloading every field, and `EditableSummary`.
      - Updated: `SaveCapabilitiesTests`, `SessionTests`, and `PartyApplyTests` (the two egg apply tests now reach Apply through `EditForTest`, since eggs are no longer editable).
    - **E2E 155** (up from 131): `NameFriendshipBrowserTests`, 3 engines × 2 paths:
      - a language change renaming a Pokémon that is not nicknamed, with the note; 256 refused and kept as typed with Apply disabled; apply, legality and export byte-identical to native Core; only the slot and footer differ
      - the flag rules (auto-tick, clearing back to the default and a clean draft, a kept name after a language change)
      - party OT and HT friendship labelled per trainer, keeping HP, status and current handler, byte-identical to native Core
      - an egg and a Pokémon without a handler offering only what can be changed
      - no horizontal scroll at 375 px; no network or storage use
      - `StorageBrowserTests` and `LegalityBrowserTests` updated for the new open message and the auto-tick.
      - The sprite cases ran against a publish with sprites.
    - **RealSave 38** (up from 26): `RealSaveFriendshipRoundTrip` on the first writable boxed Pokémon that is not an egg, 3 engines × 2 paths × 2 families. It edits OT friendship, and HT friendship when a handler is stored (otherwise it checks the field is read-only). Byte-identical to native Core; only the slot and footer differ.
    - **Mutation checks** (Unit tier; each file restored from a scratchpad copy; the number is how many tests failed):
      - clamping friendship → 4
      - no egg gate in `Require` → 1
      - no auto-tick → 7
      - a reset that ignores species names → 4
      - HT editable without a handler → 1
      - friendship declared stat-affecting → 2 (only after the party test gained a stored stat Core would not calculate)
      - languages not validated → 4
      - a language change that never renames → 3
      - friendship text always rewritten → 1
      - the rename note not tracked → 2
      - no reload after an accepted edit → 4
      - the font check dropped → 3
    - **Other checks:**
      - The trim baseline is unchanged (38), and the `PKHeX.slnx` Release build has 0 warnings.
      - Screenshots at 1280 and 375 px (Chromium, private XY save, first party member, language changed to German, OT friendship 256) show the kept-name note, the refusal, Apply disabled and no horizontal scroll.
  - **Review** (with throwaway probes against the private saves):
    - Across all 976 non-egg entities of both private saves, no stored Pokémon would be renamed by clearing its flag again, or flip its flag by retyping its own name. None is reported as keeping another language's name or as using characters the font cannot show. 292 have a handling trainer.
    - **Recorded, not changed:**
      - A language change keeps a species name from another language, as WinForms does (W8), so the Pokémon can become Invalid. The note says so.
      - Core names languages "ENG (English)", and the notes use those names as they are.
      - A refused field has no visual invalid state or `aria-invalid`; only `#message` reports it. M18 owns validation focus and styling.
      - A read-only HT field looks like an editable one, apart from the note. M18.
      - `#name-note` is a live region and is announced after each name change. M18 decides whether that is too chatty, together with the legality status.
      - Fixed-nickname encounters (Core's `SetDefaultNickname(LegalityAnalysis)`) are not used for the default name: WinForms' `UpdateNickname` does not use them either. M15 revisits with species changes.
  - **Adversarial review** (after the first review, with throwaway probes against Core, the browsers and the private saves, and mutations).
    - **Fixed:**
      - **Clearing the flag could write an empty or foreign name.**
        - For a stored language Core has no Gen 6 names for, Core's "default name" is empty (0, the unused 6, 255) or Chinese (9, 10).
        - So clearing the flag on a custom name, or changing language from such a value, wrote an empty or Chinese nickname, and the note read "…default is ;". Reproduced with a probe.
        - `NameRules.DefaultName` now gives a name only for the context's game languages (Core's `Language.GetAvailableGameLanguages`), and otherwise keeps the name. The note says the game has no default name for it.
        - A Unit test checks that every Gen 6 species has a non-empty default name in every game language, so no extra empty-name guard is needed. Such a guard was tried and dropped: its mutation survived.
      - **Friendship fields wiped partly typed text.**
        - With `type=number`, a browser reports "1e" or "-" as an empty value. The refusal then wrote "" back over the visible text (reproduced in Chromium), so refused input was not kept as typed.
        - The fields are now `type=text inputmode=numeric`, and the draft still refuses anything but 0–255.
      - **A stale refusal stayed in `#message`** after the field was corrected. This was already so before M10, but more fields made it confusing. An accepted edit now clears the message if it is still the last edit's refusal.
      - Mutations:
        - dropping the game-language check → fails 2 Unit tests
        - `type=number` back → fails 1 Unit test
        - not clearing the refusal → fails the E2E check (all 6 browser cases)
    - **Checked, not changed:**
      - Core truncates a 20-character PK6 nickname to 12 without error, so refusing over-long names (rather than relying on Core) is needed, and the PKForge comparison holds.
      - Hostile species values (0, 722, 1000, 65535) get no default name and no auto-tick, and nothing throws. Hostile save languages (0, 6, 255) do not throw in the font check.
      - WinForms' `UpdateNickname` re-read confirms the W8 behaviour: a species name from any language is kept on a language change.
    - **Recorded, not changed:**
      - A Pokémon whose current handler is the handling trainer but which has no handling-trainer name gets a note saying the game uses the handler's value, which cannot be edited. Neither private save has one (0 of 976), and Core's legality reports the state.
      - Typing the species name back after the flag was auto-set keeps the flag, so the draft stays dirty. WinForms does the same.
      - A refused name is replaced by the drafted name when another field's edit is accepted, so text the game could not store is not kept on screen.
    - After the fixes: Unit 694, E2E 155 (republished, with sprites), RealSave 38, trim baseline 38, `PKHeX.slnx` Release 0 warnings.
  - **Not verified yet:** no screen reader; physical devices (G-C).
  - **Compared with PKForge** (`SaveEngineSession.ApplyEdit` / `ApplyMetEdit`, `MonFieldService.ApplyTrainerEdit`):
    - **Matches:**
      - OT and HT friendship as separate values
      - clearing the flag gives the default name (`SetDefaultNickname`)
    - **Stricter:**
      - PKForge clamps friendship with `Math.Clamp`; we refuse.
      - PKForge forces `IsNicknamed = true` on any name change; we follow WinForms' species-name rule.
      - PKForge writes the nickname with no length check, which Core silently truncates; we refuse.
      - PKForge changes the language with no name handling; we apply WinForms' rule and say what happened.
      - PKForge has no egg gate on these edits, and no font check.
    - **Not adopted:** HT name, gender and language edits, and clearing the handler (Post-MVP); `SetDefaultNickname(LegalityAnalysis)` for fixed-nickname encounters (M15).
- **M11 Level/EXP, nature, stats/characteristic.** Level and EXP stay in sync through `Experience`. PK6 nature is independent of PID. Calculated stats and characteristic are shown on the clone (PKM-005, 007, 014).

  **M11 status:** code complete on `web/m11-level-nature`.
  - **User decisions:**
    - Re-entering the current level keeps the experience points; only a changed level resets them to the start of the level (PKForge's behaviour; WinForms resets on any level text change).
    - A party stat edit that leaves the level and Core's calculated stats as they were for the stored member keeps the stored battle state byte for byte, so undoing an edit leaves the draft clean.
  - **Draft** (`State/EditorDraft`):
    - `EditLevel(int)`: refused outside 1–100 (`LevelOutOfRange`), never clamped; a changed level sets `EXP = Experience.GetEXP(level, growth)`. Only `EXP` is written: Core's `CurrentLevel` setter also writes the party level byte (0xEC), which a boxed draft carries but does not store.
    - `EditExperience(long)`: refused below 0 or above the level 100 threshold (`ExperienceOutOfRange`); the level follows.
    - `EditNature(int)`: must be in the session's `Lists.Natures` (`NatureNotAvailable`); sets `Nature` only. PK6 does not override `StatAlignment`, so the stats use it; PID, EC, gender, ability and shininess are untouched.
    - All three are stat edits (`affectsStats: true`). Readers: `Level`, `Experience`, `Nature`, `Progress` (`State/LevelProgress`: level, EXP, level start, next level threshold, maximum).
    - `EditableFields` gains `Level` and `Nature`; `SupportMatrix` grants both to XY and ORAS.
    - `EditForTest` stays only for egg Apply tests; the party tests now use `EditLevel`.
  - **Party-stat policy:** `PartyStatPolicy.AfterStatEdit(candidate, stored)` copies the stored party section back when the level and `GetStats` match the stored member's, and otherwise runs `Recalculate`. The level check is an equivalent mutant in Gen 6 (HP grows with every level, and Shedinja's Attack does), and is kept as a guard.
  - **Inspector:** `StatsFacts.Nature` (`Services/NatureEffect`, Core's Atk/Def/Spe/SpA/SpD order mapped to the summary order) marks the raised and lowered stat in text ("Attack (raised by nature)"), not by colour. A new `StatsSource.Recalculated` captions a drafted party member's recalculated stats as what applying will store. Characteristic and Hidden Power were already read from the draft.
  - **UI** (`Components/DraftEditor`):
    - "Level and experience" fieldset: `#level` and `#exp` (text, numeric keyboard, refused input kept as typed, "050" not rewritten), and the live region `#level-note` with the level's range and the distance to the next level.
    - "Nature" fieldset: `#nature` from Core's list (an unlisted stored value shows as "Unknown (stored value N)"), and `#nature-note` with the stat effect and that the Gen 6 nature is stored apart from the PID.
    - `PartyText.KeptOnEdit` is reworded (the M9 note): name, language and friendship edits keep the battle state; level, experience and nature edits recalculate it, keep the status and never raise HP. `EditableSummary`, the egg note and `EggNotEditable` name the new fields. README updated.
  - **Tests.**
    - **Unit 756** (up from 694):
      - `LevelNatureDraftTests` (36, 2 from the adversarial review): native byte parity (XY and ORAS); the party tail of a boxed draft untouched; one species per growth rate (level 50 and level 100 thresholds as literals); same level keeps EXP; EXP across a level boundary; maximum accepted and one more refused; levels 0/101/−1/`int.MaxValue` and EXP −1/`long.MaxValue`/2³² refused unclamped; stored EXP above the curve; nature changes only itself; all 25 natures; 25/−1/255 refused; nature round trip clean; family and egg gating; party level edit equals native Core through apply and export with an odd stored Attack recalculated; party nature edit keeps HP and status; EXP within a level and a nature round trip keep the stored battle state; level drop previewed; fainted stays fainted; Shedinja.
      - `PartyStatPolicyTests` (+3), `InspectorTextTests` (+10: markers for five natures, neutral and unknown natures, markers against Core's calculation for all 25, the three captions), `DraftEditorTests` (+13, bUnit: level sets EXP, EXP sets level, 7 refusals kept as typed, "050", nature list and notes, unlisted nature, invariant digits under ar-SA, egg read-only, summary).
      - Updated: `SaveCapabilitiesTests`, `PartyApplyTests`.
    - **E2E 167** (up from 155): `LevelNatureBrowserTests`, 3 engines × 2 paths: a boxed level + nature edit (EXP and note follow, EXP past the curve refused and kept, Apply disabled, nature note, inspector markers, legality, export byte-identical to native Core, only the slot and footer differ, no scroll at 375 px, no network or storage); a burned level-50 party member: its level retyped a key at a time (through 5) leaves the draft clean, typing 55 keeps its HP, then a level drop (HP preview text, recalculated caption, apply, export equal to the native policy oracle, burn kept). Every E2E test executed.
    - **RealSave 50** (up from 38): `RealSaveLevelNatureRoundTrip`, 3 engines × 2 paths × 2 families: a level step and nature change on the first writable boxed non-egg, and a level step on the first party member, byte-identical to native Core with the policy; only those two slots and the footer differ.
    - **Mutation checks** (Unit tier; files restored from a scratchpad copy): clamp level → 6 fail; clamp EXP → 5; `CurrentLevel` setter → 3; same level rewrites EXP → 2; nature not a stat edit → 2; nature unvalidated → 3; no recalculated caption → 1; no unchanged-calculation restore → 2; always restore → 9; restore without the level check → 0 (equivalent, see above); no raised marker → 5; Core order unmapped → 6; EXP not reloaded → 2; number fields always rewritten → 2.
    - **Other checks:** trim baseline unchanged (38); `PKHeX.slnx` Release 0 warnings. Driven on the private XY save (Chromium, 1280 and 375 px): a party Greninja at level 100 lowered to 90 with Adamant shows the HP preview (289 → 261), the level and nature notes and the recalculated caption, with no horizontal scroll.
  - **Adversarial review** (with probe tests and mutations).
    - **Fixed:**
      - **Typing a party member's level could lower its HP for good.** Every keystroke is an edit, so changing 90 to 95 passes through 9 (Backspace, then 5). The level 9 edit clamped HP to the level 9 maximum, and the level 95 edit took min(that HP, new maximum), so the member would have been written at 26 HP instead of 177 (reproduced by a probe). The policy now takes the HP kept and the status restored from the stored member, not from the draft (`AfterStatEdit`), so no intermediate value outlives its keystroke.
      - **Retyping the stored level reset its experience points.** 90 → "9" → "90" set EXP to the start of level 90, so the draft stayed dirty and the progress within the level was lost, defeating the "re-entering keeps EXP" decision. A level edit that returns to the stored level now restores the stored EXP.
      - The first E2E used `FillAsync`, which sends the whole value as one input event, so neither bug could show in the browser. The party E2E now types with `PressSequentiallyAsync` on a level-50 member.
      - Mutations: HP taken from the draft → 1 Unit and all 6 party E2E cases fail; no stored-level EXP restore → 1 Unit and all 6 party E2E cases fail.
    - **Checked, not changed:** refused input and the draft-valid state follow M10 (an accepted edit elsewhere reloads every field); no other component lists the editable fields; a party member stored without stats is still refused at the level and nature edits.
    - After the fixes: Unit 756, E2E 167 (every test executed), RealSave 50.
  - **Recorded, not changed:**
    - Each keystroke in the level and EXP fields is a full edit (and a legality restart, debounced as before); the live notes update with it. M18 decides whether the notes are too chatty.
    - Stored EXP above the curve reads as level 100 and is kept by re-entering 100, but typing that value is refused like any out-of-range value.
    - A nature change can make a fixed-nature encounter Invalid; legality reports it (M16 acknowledgements).
    - Level-up HP gain is not modelled: HP is min(previous, new maximum), so a level gain never raises current HP (`PKHeX.Web.md`).
  - **Not verified yet:** no screen reader; physical devices (G-C).
  - **Compared with PKForge** (`SaveEngineSession.ApplyEdit`, `MonFieldService`, `MonSummaryService`):
    - **Matches:** a level edit sets the level's minimum EXP; an unchanged level does not rewrite EXP (`UnchangedLevelInAnEditDoesNotRewriteExperience`); Gen 5+ nature set without touching the PID; characteristic shown.
    - **Stricter:** PKForge clamps the level (`Math.Clamp`); we refuse. PKForge's `ApplyEdit` never recalculates stored party stats after a level or nature edit (the display recomputes on a clone, so the save keeps stale stats); we apply the PK6 policy with an HP preview. PKForge has no direct EXP edit; we add one, kept in step through `Experience`.
    - **Not adopted:** Gen 3/4 PID-derived nature (`TrySetPidDerived`; those formats are not opened) and Gen 8+ mints/stat nature (WEB-PKM-008, MVP+). The desktop's stat-label nature shortcuts (`StatEditor.ClickStatLabel`) are not adopted either; the nature box is the one control.
- **M12 IVs/EVs.** Per-stat and total EV limits from Core, with no silent clamping. Changes mark legality stale and recalculate stats (PKM-013).

  **M12 status:** code complete on `web/m12-ivs-evs`.
  - **User decision:** an EV edit is refused when it **raises** the total above 510; an edit that lowers a stat is always accepted, so a stored total over the limit can be brought down one field at a time (WinForms instead drafts any 0–252 value and blocks the save while the total is over 510).
  - **Why refusal is needed here** (checked in Core): the PK6 IV setters clamp a value above 31 and, for a negative value, OR the unmasked `uint` into `IV32`, overwriting the neighbouring IVs and the egg and nickname flags; the EV setters store `(byte)value`. WinForms' `StatEditor.UpdateIVs/UpdateEVs` rewrites over-maximum text to the maximum.
  - **Draft** (`State/EditorDraft`):
    - `EditIv(stat, value)`: refused outside 0–`MaxIV` (`IvOutOfRange`); written through Core's `SetIV` for that stat only.
    - `EditEv(stat, value)`: refused outside 0–`MaxEV` (`EvOutOfRange`), and when the new total is above `EffortValues.Max510` and above the drafted total (`EvTotalAboveLimit`).
    - The stat index is the summary order (HP, Atk, Def, SpA, SpD, Spe), mapped to Core's order (HP, Atk, Def, Spe, SpA, SpD); an index outside 0–5 is an `ArgumentOutOfRangeException`.
    - Both are stat edits (`affectsStats: true`). Readers: `Ivs`, `Evs`, `IvTotal`, `EvTotal`, `MaxIv`, `MaxEv`, `MaxEvTotal`, `HiddenPowerType`, `StatCount`.
    - `EditableFields` gains `Ivs` and `Evs`, gated separately; `SupportMatrix` grants both to XY and ORAS.
    - No party-policy change: an EV step that changes no stat (stats grow every 4) already keeps the stored battle state.
  - **UI** (`Components/DraftEditor`, text in `Components/EditorText`):
    - "IVs and EVs" fieldset: a table with a row per stat (row header marked raised/lowered by nature, as the inspector does) and columns "IV (0–31)" and "EV (0–252)" from Core's limits. Fields `#iv-0..5` and `#ev-0..5` are text with a numeric keyboard, named by `aria-labelledby` (row and column headers), refused input kept as typed, "031" not rewritten.
    - Live regions `#iv-note` (IV total of 186, the Hidden Power type, and that the IVs decide it and the characteristic) and `#ev-note` (total of 510 and remaining, as the desktop's tooltip, plus Core's `EffortValues.GetGrade`: 508 uses every effective EV, 510 is the most, above 510 says to lower an EV).
    - `EditableSummary`, the egg note, `EggNotEditable` and `PartyText.KeptOnEdit` name IVs and EVs; README updated; `app.css` gives the table fixed columns so it fits 375 px.
  - **Tests.**
    - **Unit 800** (up from 756; 798 before the adversarial review):
      - `IvEvDraftTests` (27, 2 from the adversarial review): each IV and EV edit equals native Core for every stat (XY and ORAS); the readers follow the summary order and match the inspector; IVs 0 and 31 keep the egg and nickname flags; IVs −1, 32, `int.MaxValue`, `int.MinValue` and EVs −1, 253, 255, 256, `int.MaxValue` refused unclamped; stat index outside 0–5; total exactly 510 accepted and 511 refused; a stored 756 lowered a keystroke at a time while raises stay refused; a stored EV of 255 kept until changed, and refused if retyped; changed and changed back leaves the draft clean; family gating (IVs and EVs separately) and egg gating; a party EV edit through apply and export equals native Core with the odd stored Attack recalculated, HP and burn kept; a party IV edit never heals; one EV more keeps the stored battle state; a party EV typed a key at a time (252 → 248 through 2 and 24) keeps no HP clamp from the passing keystrokes, and typed back leaves the draft clean; IV and EV edits of a member stored without stats refused (`PartyStatsMissing`).
      - `DraftEditorTests` (+17, bUnit): headers, nature markers and `aria-labelledby`; IV and EV gated separately; IV note and EV total after edits; 8 refusals kept as typed; "031"; each EV grade and a stored total over the limit; the unknown Hidden Power wording; invariant digits under ar-SA; an accepted edit replacing refused IV/EV input; a new draft reloading them; the egg table read-only; `EditableSummary`.
      - Updated: `SaveCapabilitiesTests`; `PartyApplyTests.WithoutStoredStats` made internal for reuse.
    - **E2E 179** (up from 167): `IvEvBrowserTests`, 3 engines × 2 paths: a boxed Speed IV (32 refused and kept, Apply disabled), Attack and Speed EVs typed a key at a time to 504, 7 HP EVs refused as over the total and kept, 6 accepted (510 note), inspector Hidden Power and EV total, legality, export byte-identical to native Core with only the slot and footer differing, no scroll at 375 px, no network or storage; a burned level-50 party member given 252 HP EVs a key at a time (maximum raised, current HP not), then back to 0 EVs and an HP IV of 0 (HP preview), apply and export equal to the native policy oracle, burn kept. `LevelNatureBrowserTests` updated for the longer open message.
    - **RealSave 62** (up from 50): `RealSaveIvEvRoundTrip`, 3 engines × 2 paths × 2 families: a Speed IV step and an EV step (lowering one where any is stored) on the first writable boxed non-egg, and an HP IV step on the first party member, byte-identical to native Core with the policy; only those two slots and the footer differ.
    - **Mutation checks** (Unit tier; files restored from a scratchpad copy; failing tests): clamp IV → 5; clamp EV → 8; no total check → 3; total check refusing lowers → 1; IV not a stat edit → 1; EV not a stat edit → 2; summary→Core order unmapped → 5; IV edit ungated → 1; EVs gated by the IV flag → 1; IV field always rewritten → 1; IV/EV fields not reloaded → 3; the 508 grade dropped → 1; no nature marker in the table → 1; EV fields read-only by the IV flag → 0 at first, 1 after adding `IvAndEvFieldsFollowTheirOwnFlag`.
    - **Other checks:** trim baseline unchanged (38); `PKHeX.slnx` Release 0 warnings; every E2E and RealSave test executed. Driven on the private XY save (Chromium, 1280 and 375 px): the first party member (494 EVs) given an HP IV of 0 shows the HP preview (289 → 254), the IV note (155 of 186, Hidden Power Dragon), an Attack EV of 253 refused and kept as typed with Apply disabled, the nature markers in the row headers, and no horizontal scroll.
  - **Adversarial review** (with throwaway probes against Core, the private saves and the published app, and mutations).
    - **Confirmed:** Core's PK6 setters behave as the draft assumes: `IV_SPD = -1` sets the egg and nickname flags, `IV_HP = -1` makes every IV 31 and sets the egg flag, `IV_ATK = 40` stores 31, `EV_ATK = 256` stores 0.
    - **Private saves** (968 non-egg entities, 553 XY and 415 ORAS, boxes and party): none has an EV above 252 or a total above 510; the readers match the inspector for every one; retyping every stored IV and EV leaves every draft clean. For the party members, one EV step that stays within a multiple of 4 keeps the stored battle state; the probe's 3 recalculations were members at 510 stepping down across a multiple of 4 (252 → 251), which changes a stat at level 100, as it should.
    - **Published app** (private XY save): retyping a party member's HP EV a key at a time leaves the draft clean with HP unchanged.
    - **Fixed (tests only):**
      - The first retype test ended on the stored value, which restores the stored battle state whatever happened before, so it survived the mutation "HP taken from the draft, not the stored member" (`PartyStatPolicy`). It now types 248 over a stored 252 at level 100 through 2 and 24, then types 252 back; the mutation fails it, as it fails M11's level test.
      - Added the `PartyStatsMissing` refusal for IV and EV edits.
      - `DraftEditorTests.WithEvs` took Core's stat order and `IvEvDraftTests.WithEvs` the summary order; both now take the summary order the fields use.
    - **Checked, not changed:** no id collisions with the new `stat-*`, `iv-*`, `ev-*` ids; refused prefixes cannot block a value within the limit, because every prefix of a number is smaller than it.
    - After the review: Unit 800. No app code changed, so the E2E 179 and RealSave 62 runs above stand.
  - **Recorded, not changed:**
    - At 320 px (WCAG 400% reflow) the page scrolls sideways by 2 px. The overflow is the inspector's stats table from M7/M11 (row headers such as "Sp. Atk (lowered by nature)"), not the new IV/EV table, which fits; M18 owns reflow.
    - A stored EV above 252 shows as stored in its field under the "EV (0–252)" header, with no note of its own; legality reports it as Invalid. Neither private save has one.
    - Typing an EV that would pass the total is refused at the first keystroke that passes it: with 494 EVs elsewhere, typing 252 stops at "2" (the "25" is refused, and is replaced by the drafted 2 once another edit is accepted, as in M10). The refusal message says to lower another EV first.
    - A stored EV above 252 (or a stored total above 510) is kept until that field is changed; retyping it is refused like any out-of-range value, as for stored EXP above the curve in M11. Lowering it to a value still above 252 is refused too: only the total has the "lowering is accepted" rule.
    - Each keystroke in an IV or EV field is a full edit, as in M11; the live notes update with it. M18 decides whether they are too chatty.
    - The legality note for 508 or all-equal EVs (Fishy) comes from the analysis, not the editor; IVs fixed by an encounter (flawless count, fixed sets) are not enforced by the editor and are reported by legality (M16 acknowledgements).
  - **Not verified yet:** no screen reader; physical devices (G-C).
  - **Compared with PKForge** (`SaveEngineSession.ApplyEdit`, `ClampAll`, `SetIVsFromAppOrder`/`SetEVsFromAppOrder`, `LegalityAssistService`):
    - **Matches:** the app-order to Core-order mapping of the six stats.
    - **Stricter:** PKForge clamps each IV and EV (`Math.Clamp`) and has no total check on edit (its legality assistant trims a total over 510 after the fact); we refuse both. PKForge's `ApplyEdit` does not recalculate stored party stats; we apply the PK6 policy with an HP preview.
    - **Not adopted:** presets and max/reset chips, setting IVs from a Hidden Power type (`HiddenPower.SetIVsForType`), and Gen 7+ hyper training, AVs and GVs (other formats).
- **M13 Held item, moves, PP/PP Ups.** Uses Core-filtered item/move lists. Empty moves are allowed. PP rules come from Core. The UI never claims a move is learnable (PKM-010–012).

  **M13 status:** code complete on `web/m13-item-moves`.
  - **User decisions:**
    - A move change follows WinForms' `MoveChoice.HealPP`: the new move gets full PP for the PP Ups it keeps; an emptied slot has no PP or PP Ups; a move PP Ups cannot be used on (`Legal.IsPPUpAvailable(move)`, e.g. Sketch) has them removed. Changing back to the stored move restores its stored PP and PP Ups.
    - A PP Ups change refills PP to the new maximum (WinForms `CB_PPUps` → `HealPP`); changing back to the stored count of the stored move restores the stored PP.
  - **Core facts relied on** (checked in source): `FilteredGameDataSource.Items` is `GetItemDataSource(sav.HeldItems)` (`(None)` first, nothing above `MaxItemID`); `.Moves` is `LegalMoveDataSource` (no Z, Max or Torque moves) up to `MaxMoveID` (617 XY, 621 ORAS), with `(None)` first and no dummied Gen 6 moves; `GetMovePP(move, ups) = GetBasePP(move) * (5 + ups) / 5`. PK6's `Move1` setter writes only the move ID; `SetMoves` and `FixMoves` rewrite or reorder every slot, so neither is used. `MovePPVerifier` allows any PK6 PP up to the healed maximum, in boxes and the party.
  - **Draft** (`State/EditorDraft`):
    - `EditHeldItem(int)`: must be in `Lists.Items` (`ItemNotAvailable`); only the item is written.
    - `EditMove(slot, move)`: slot 0–3 (`ArgumentOutOfRangeException`), move in `Lists.Moves` (`MoveNotAvailable`); writes only that slot's move, PP and PP Ups by the rules above. The PP Ups a new move keeps are the slot's last count on a move that can take them (at first the stored count), not the replaced move's: a move box picks a move per keystroke of type-ahead, so "sky" passes Sketch on its way to Sky Attack (second review). A stored count above 3 is never carried. Choosing the drafted move again keeps its PP. Duplicates are allowed (legality reports them, as in WinForms). No compaction (WEB-PKM-011 "order and unaffected slots preserved"; WinForms' `EditPK6` runs `FixMoves` on save).
    - `EditPpUps(slot, ups)`: refused for an empty slot (`MoveSlotEmpty`), outside 0–3 (`PpUpsOutOfRange`, the constant `MaxPpUps`, since Core names no limit) and above 0 on a move that cannot take them (`PpUpsNotAllowed`).
    - `EditPp(slot, pp)`: refused for an empty slot and outside 0 to `GetMovePP(move, ups)`, capped at 255, the byte PK6 stores PP in (`PpOutOfRange`); never clamped.
    - None affects stats (`affectsStats: false`), so a party member keeps its stored battle state byte for byte; a member stored without stats can still have them edited (Apply refuses it as before).
    - Readers: `HeldItem`, `Moves` and `StoredMoves` (`State/MoveSlot`: move, PP, PP Ups, maximum, `CanTakePpUps`), `MoveCount`, `MaxPpUps`. `State/MoveSlots` reads and writes one slot's PP by index (Core indexes only the move).
    - `State/MoveChange.Between` works out what a move or PP Ups edit did (`MoveSet`, `PpUpsCleared`, `Emptied`, `Restored`, `PpUpsChanged`), from the slot before, after and as stored; the text lives in the UI.
    - `EditableFields` gains `HeldItem`, `Moves` and `Pp` (PP and PP Ups), gated separately; `SupportMatrix` grants all three to XY and ORAS. The `PartyStatPolicy` remark now says a *stat* edit never refills PP.
  - **UI** (`Components/DraftEditor`, text in `Components/EditorText`):
    - "Held item" fieldset: `#held-item` from Core's list, each value once (Core lists Pretty Feather in two pouches), described by `#item-note` (legality reports an item never released in the generation: Core's `ItemVerifier` checks no more). A stored value outside the list is named as the inspector names it: Core's name marked "(not available in this game)", such as an Omega Ruby move or item on a Pokémon traded into X, or "Unknown (stored value N)".
    - The move and item boxes are `Components/ChoiceSelect`, which renders again only when its own value, list or state changes. Every box (language, nature, item, moves, PP Ups) sets its value on the `<select>`, not as an option's `selected` attribute (second review).
    - "Moves" fieldset: a table with a row per slot ("Move 1"–"Move 4" row headers) and columns Move (`#move-0..3`, Core's list with "(None)", unlisted stored moves shown as stored), PP (`#pp-0..3`, numeric text, kept as typed when refused, "05" not rewritten, read-only for an empty slot), PP Ups (`#ppups-0..3`, 0–3, disabled for an empty slot; a move without PP Ups offers only 0 and its stored count, so no choice from the box is refused; a stored count above 3 shows as "N (stored; above the maximum)") and Max PP (`#maxpp-0..3`). Every field is named by `aria-labelledby` (row and column headers).
    - `#move-note` (live region) states the last move or PP Ups edit's PP effect ("Move 1 is now Thunderbolt, with full PP: 21 of 21 (2 PP Ups).", the Sketch removal, the emptied slot, the restored stored PP); any other accepted edit clears it. `#move-list-note` always says the list does not say what the Pokémon can learn and legality analysis checks that (WEB-PKM-011).
    - `EditableSummary`, the egg note, `EggNotEditable` and `PartyText.KeptOnEdit` name the held item, moves, PP and PP Ups; `UserMessages` has text for the six new refusals. README updated (summary, party, capabilities, a "Held item and moves" bullet).
    - `app.css`: fixed number columns; below 34rem each row becomes two grid lines (slot and move, then PP, PP Ups and maximum under the wrapped PP headers), so the move box keeps room for a name at 375 px.
  - **Tests.**
    - **Unit 864** (up from 800; 857 before the second review):
      - `ItemMoveDraftTests` (44, 4 from the second review): readers in order and matching the inspector; item, move, PP and PP Ups edits equal native Core (XY and ORAS) with only that slot's bytes changed; each slot edited alone keeps the others in place; clearing a slot leaves the gap; filling an empty slot; a duplicate move allowed; Sketch removes PP Ups and refuses new ones; back to the stored move through other moves and an empty slot restores 20 of 49 PP and leaves the draft clean; the drafted move chosen again keeps its PP; PP Ups refill and restore; PP −1/50/255/256/`int.MaxValue`/`int.MinValue` and PP Ups −1/4/255/`int.MaxValue` refused unclamped; empty-slot PP and PP Ups refused; slot outside 0–3; moves past `MaxMoveID`, a Z-move, a Max Move, 0x7FFF and −1, and items past `MaxItemID`, an unholdable item, −1 and 0xFFFF refused; a stored unlisted item and stored PP above the maximum kept until changed; stored PP Ups on Sketch removable; changed and changed back leaves the draft clean; family gating (each flag) and egg gating; party item, move, PP Ups and PP edits keep the battle state, preview no HP change and equal native Core through apply and export; a member stored without stats can have its moves edited; each edit counts (legality stale); `MoveChange.Between` for every kind; PP Ups carried through Sketch and an empty slot; the last count chosen carried, and the stored one after a restore; a stored count of 200 not carried; PP 256 refused whatever the PP Ups.
      - `DraftEditorTests` (+20, bUnit, 3 from the second review): ids, headers and `aria-labelledby`; the boxes list exactly Core's values; empty slot and Sketch controls; stored Sketch PP Ups removable; the note for every kind of change; held item choice; 5 refusals kept as typed; an accepted edit replacing refused PP; unlisted item, move and PP Ups shown as stored; moves and PP follow their own flags; invariant digits under ar-SA; a new draft reloading the fields and clearing the note; egg read-only; `EditableSummary`; an Omega Ruby move and item in an X save named as the inspector names them; a keystroke elsewhere leaves the move and item boxes unrendered; the note for carried and dropped PP Ups.
      - Updated: `SaveCapabilitiesTests`.
    - **E2E 197** (up from 179; 191 before the second review): `ItemMoveBrowserTests`, 3 engines × 2 paths: a boxed item change (inspector follows), a move change with its note and PP, PP 22 over 21 refused as typed with Apply disabled, PP typed a key at a time, an empty slot filled, a PP Ups change refilling PP with its note, a slot emptied (PP Ups disabled), legality, export byte-identical to native Core with only the slot and footer differing, no horizontal scroll at 375 px and a move box at least 150 px wide, no network or storage; an injured, burned party member given a move and an item keeps 7 HP, its burn and its odd stored Attack, shows no HP preview or recalculated caption, and exports equal to native Core; the boxes show what the draft holds (second review): "sky" typed into Surf's closed move box reaches Sky Attack with its 3 PP Ups, a PP Ups box the user changed shows the stored count restored with the stored move, and the next Pokémon's nature box shows its stored nature, not the one chosen for the last. `IvEvBrowserTests` updated for the longer open message. Every E2E test executed.
    - **RealSave 74** (up from 62): `RealSaveItemMoveRoundTrip`, 3 engines × 2 paths × 2 families: a move and item change on the first writable boxed non-egg and a PP step on the first party member, byte-identical to native Core; only those two slots and the footer differ.
    - **Mutation checks** (Unit tier; files restored from a scratchpad copy; failing tests): clamp PP → 9; no refill on a move change → 8; no stored-move restore → 3; Sketch PP Ups kept → 1; `FixMoves` compaction → 10; `SetMoves` for the whole moveset → 3; item unvalidated → 3; moves gated by the PP flag → 1; move edit as a stat edit → 1; no PP Ups refill → 4; no PP Ups restore → 1; PP Ups range widened → 2; PP Ups allowed on Sketch → 1; empty slot PP editable → 1; `Restored` never reported → 2; PP fields not reloaded → 6; no PP effect in the note → 1; empty slot PP field writable → 2; Sketch offered every PP Ups count → 2; move note not tracked → 3. Removing the phone layout fails all 6 boxed E2E cases (horizontal scroll at 375 px). Second review: the replaced move's count carried → 4 Unit, and all 6 "boxes show what the draft holds" E2E cases; a stored count above 3 carried → 2; no byte cap → 1; carried count not updated by a PP Ups edit → 1; no carried note → 1; long lists always rendered → 1; duplicates kept → 1; traded moves named "Unknown" → 1; PP Ups or nature shown through options' `selected` attribute → 2 E2E each (Firefox, both paths).
    - **Other checks:** trim baseline unchanged (38); `PKHeX.slnx` Release 0 warnings. Driven on the private XY save (Chromium, 1280 and 375 px): the first party member's second move changed to Surf, its first slot's PP Ups lowered, PP 99 refused and an item chosen, with no horizontal scroll.
  - **Adversarial review** (throwaway probes against the private saves and the published app).
    - **Fixed:** at 375 px the five-column table gave the move box no width at all: the move names could not be seen, and the table overflowed its fieldset without scrolling the page, so the E2E scroll check passed. Below 34rem each row is now two grid lines, and the E2E checks the move box's width.
    - **Private saves** (968 non-egg entities, 553 XY and 415 ORAS, boxes and party): every stored move and item is in Core's lists; no PP Ups above 3 or on a move that cannot take them; no empty slot with PP; no gap before a filled slot. Retyping every stored value, and taking each move to another, to empty and back, each PP Ups count away and back, and the item away and back, leaves every draft clean; a party move change keeps the battle state. Two XY slots store PP above the maximum: shown as stored and kept until edited.
    - **Checked, not changed:** a refused PP prefix cannot block a value within the maximum (every prefix is smaller); the selects can only send listed values, so a refusal from a box cannot leave it showing an unapplied choice.
  - **Second adversarial review** (throwaway probes in the published app in all three engines, against Core and a `web/main` publish; files restored from scratchpad copies).
    - **Fixed:**
      - **Type-ahead lost PP Ups.** Typing into a closed move box selects a match per keystroke and fires a change for each, in Chromium, Firefox and WebKit: "sky" on a Surf with 3 PP Ups passed Sketch, which removed them, and Sky Attack ended with none. The same keystroke trap as M11's level field. A new move now takes the slot's last count on a move that can take them.
      - **Stored PP Ups above 3 corrupted PP.** A move change kept a stored count of 200 and set PP to Core's 615, which the PP byte stored as 103; PP 300 was accepted against that maximum and stored as 44. Such counts are no longer carried, and PP above 255 is refused.
      - **Firefox showed choices the draft no longer held.** Firefox follows the HTML standard: once the user has chosen an option, a later change of an option's `selected` attribute does not change the selection. A restored PP Ups count showed 0 while the draft held 2, and, from M11, choosing a nature, cancelling and opening another Pokémon still showed the chosen nature instead of the stored one (the language box from M10 had the same pattern). Every box now sets its value on the `<select>`, as `StorageBrowser`'s box select already did.
      - **Keystroke cost doubled.** Each render rebuilt about 2,900 options (four move boxes and the item box): a friendship keystroke took a median 4.7 ms (Chromium), 7.0 (Firefox) and 4.0 (WebKit) against 2.2, 3.0 and 2.0 before M13 (Apple M4). With `ChoiceSelect` it is 2.4, 3.0 and 2.0.
      - Moves and items Core knows but the game's list leaves out (Omega Ruby's Origin Pulse, Precipice Blades, Dragon Ascent and Hyperspace Fury, and 19 items such as Audinite, on a Pokémon traded into X or Y) showed as "Unknown (stored value N)"; they are now named as the inspector names them.
      - The item note claimed legality reports "an item this Pokémon could not have"; Core checks only that the item was released in the generation.
      - Core lists Pretty Feather (571) twice; the box offers it once.
    - **Checked, not changed:** arrow keys on a closed select open its list on macOS, so only type-ahead was seen to pass through intermediate moves here; the carry fix covers arrow keys on other platforms too.
  - **Recorded, not changed:**
    - The move and item boxes are native `<select>` lists (about 620 moves and 400 items), searchable by the browser's type-ahead only; a searchable picker is M18's.
    - Below 34rem the moves table uses grid rows; browsers may drop table semantics for rows displayed as grids, so each field's name comes from `aria-labelledby` alone. M18 owns the screen-reader pass.
    - Each keystroke in a PP field is a full edit, as in M11/M12, and each type-ahead step in a move box is a move change, announced by `#move-note`; M18 decides whether the notes are too chatty.
    - From M10, by reading the code (not reproduced): a refused language choice, such as a language whose default name the format cannot store unchanged, leaves the language box showing the refused choice, because the editor records the choice before the edit runs. The other boxes cannot refuse a listed value.
    - The maximum PP uses the stored PP Ups count, so a stored count above 3 (none in the private saves) allows PP up to Core's figure for it until the count is changed.
    - Duplicate moves, moves the species cannot learn and items it could not hold are reported by legality, not blocked (M16 acknowledgements).
    - Relearn moves stay read-only (WEB-PKM-026, Post-MVP).
  - **Not verified yet:** no screen reader; physical devices (G-C).
  - **Compared with PKForge** (`SaveEngineSession.ApplyEdit`, `ApplyMoveDetails`, `SetPPUps`, `ClampPP`, `GetMoveDetails`; `MonInfoService.GetHeldItems`; `InfoPickers.ShowHeldItemsAsync`; `MoveDetailsTests`):
    - **Matches:** the holdable item list comes from the save's `HeldItems`; an empty slot has no PP Ups; the maximum shown is `GetMovePP(move, ups)`.
    - **Stricter:** PKForge clamps PP and PP Ups (`Math.Clamp`, `MoveDetailsTests` pins 999 → maximum) and its `ApplyEdit` writes a move without touching PP, so a new move keeps the old move's PP; we refuse instead of clamping and set PP on a move change. It allows PP Ups on Sketch; we refuse them. It writes any item ID through `ApplyEdit`; we accept only the game's list.
    - **Not adopted:** the "Show all" item filter (HaX), item descriptions and icons, type icons in the move list, the relearn editor (WEB-PKM-026, Post-MVP), and batch `healpp`/`move1_ppups` commands.
- **M14 Ability/slot + gender.** Ability choices use `GetAbilityList(PersonalInfo)` and slot mapping. Gender follows species rules (PKM-004, 009).

  **M14 status:** code complete on `web/m14-ability-gender`.
  - **User decision:** Meowstic's form is its gender in Gen 6. A gender change changes the form too, as WinForms does (`ClickGender` sets `CB_Form`, and `UpdateForm` calls Core's `ChangeSpeciesForm`).
  - **Core facts relied on** (checked in source):
    - PK6 `Ability` (0x14) and `AbilityNumber` (0x15) are separate raw bytes with no PID link (`PIDAbility` is −1 for PK6). Valid slot numbers are 1, 2 and 4 (`AbilityVerifier.IsValidAbilityBits`).
    - `SetAbilityIndex` changes the PID only for Format ≤ 5; for PK6 it is `RefreshAbility(n)`, which writes `1 << n` and the slot's ability.
    - `GetAbilityList(pi)` returns 3 items ("Name (1)", "Name (2)", "Name (H)"). Each value is the ability ID, and duplicate abilities are kept.
    - The PK6 `Gender` setter does not mask the value, so 3 or more spills into `Form`.
    - `GenderVerifier` checks Gen 6-origin Pokémon only against a fixed or genderless ratio. Gen 3–5 origins are checked against the PID.
    - `ChangeSpeciesForm` sets EXP to the start of the level, keeps the slot through `RefreshAbility` and returns `None` when the form is unchanged.
  - **Draft** (`State/EditorDraft`):
    - `EditAbilitySlot(slot)`:
      - Refused outside 0 to `AbilityCount` − 1 (`AbilitySlotNotAvailable`), never clamped.
      - Writes through Core's `SetAbilityIndex`, so the ability and its slot number always change together.
      - `affectsStats: false`.
    - `EditGender(gender)`:
      - Only the values in `GenderChoices` are accepted (`GenderNotAvailable`). `State/GenderRule` maps the personal data's ratio to Either / OnlyMale / OnlyFemale / Genderless.
      - A single-gender species is offered only its gender, which also corrects a wrong stored value, as `ClickGender` does.
      - Ordinary species: writes the gender bits only, `affectsStats: false`.
      - Meowstic (a dual-gender species whose current form's name in Core's form list is ♂/♀, the WinForms test): calls `ChangeSpeciesForm(species, form, save.Personal, current slot)`, then sets the gender, which covers a stored form/gender mismatch. `affectsStats: true`.
      - Changing back undoes the last form change's side effects: the EXP (same level), ability and slot number it had before are given back, but only when nothing has changed them since (second review). Changing gender and back leaves the draft clean, and an edit made in between is kept.
    - Readers:
      - `Ability`, `AbilityNumber`, `AbilitySlot` (null when the pair names no slot), `AbilityChoices` (values are slots; cached per species/form), `RegularAbilitiesSame`.
      - `Gender`, `GenderRule`, `GenderChoices`, `FormFollowsGender`, `Dependents` (`State/FormDependents`: form, ability, slot number, EXP).
    - `SaveCapabilities.Personal` is the save's personal table. `EditableFields` gains `Ability` and `Gender`, gated separately, and `SupportMatrix` grants both to XY and ORAS.
  - **UI** (`Components/DraftEditor`, text in `Components/EditorText`):
    - "Ability and gender" fieldset:
      - `#ability` is a `ChoiceSelect` over the slots. A pair that names no slot is shown as "Name (stored; unknown slot, stored value N)".
      - `#ability-note` (live) explains (1)/(2)/(H), says legality decides whether the hidden ability is possible, and covers identical regular abilities and a mismatched stored pair.
      - `#gender` offers only the allowed genders. It is disabled when the only one is already stored; a value the species cannot have is shown as stored.
      - `#gender-note` states the species rule, or for Meowstic that the form follows the gender.
      - `#gender-change` (live) says what a Meowstic gender change also changed (form, ability, EXP). Any other edit clears it.
    - Text updated to name the ability and gender:
      - `EditableSummary`, `EggReadOnly` and `UserMessages.EggNotEditable`.
      - `PartyText.KeptOnEdit` (it also says a gender edit that changes the form recalculates).
      - Two new refusal texts.
      - README (intro, Party, Capabilities, a new "Ability and gender" bullet, the real-save round trips).
    - `InspectorText.Gender` is now public.
  - **Tests.**
    - **Unit 935** (up from 864; 929 before the adversarial review):
      - `AbilityGenderDraftTests` (49, 6 from the review):
        - Readers match Core's names and the inspector.
        - Each slot in XY and ORAS equals native `SetAbilityIndex`, with only 0x14/0x15 changed.
        - Gastly's identical slots stay apart.
        - Slots −1/3/4/`int.MaxValue`/`int.MinValue` are refused.
        - Stored slot numbers 0/3/7/255 and a mismatched pair are kept until a slot is chosen.
        - A gender change touches only bits 1–2 of 0x1D. Genders 2/3/4/−1/255/`int.MaxValue`/`int.MinValue` are refused on a dual-gender species.
        - Tauros, Chansey and Magnemite: only their gender is offered, and a wrong stored value is corrected.
        - Meowstic, male to female (XY and ORAS), equals native `ChangeSpeciesForm` plus gender: Competitive, EXP at the start of the level, new choices; back to male is clean.
        - Meowstic keeps a changed slot through a form change and back.
        - A stored form/gender mismatch is handled.
        - A Meowstic party member goes through apply and export equal to native Core with its battle state kept. One stored without stats is refused (`PartyStatsMissing`).
        - Party ability and gender edits keep the battle state and equal native Core.
        - Family and egg gating, changed-back cleanliness and the edit count.
        - From the review:
          - EXP edited before a form change is given back by changing back.
          - EXP edited after it is not replaced by the old value.
          - Stored slot numbers 0 and 7 are given back by a form change and back.
          - Unown F and M given the genderless value keep their form.
      - `GenderRuleTests` (8), including every ORAS species against Core's `FixedGender()`.
      - `DraftEditorTests` (+14, bUnit):
        - Ability box values, Core's texts and live note; an ability choice is drafted.
        - Identical slots are noted; an unmatched pair is shown as stored.
        - Gender options per rule; a wrong or unknown stored gender is shown and corrected.
        - The Meowstic change note, and that it clears.
        - Each flag gates separately; a new draft reloads the fields.
        - Updated: egg read-only, `EditableSummary`, and the `ChoiceSelect` count and order.
      - Updated: `SaveCapabilitiesTests`.
    - **E2E 209** (up from 197): `AbilityGenderBrowserTests`, 3 engines × 2 paths.
      - A boxed Zigzagoon: Core's three slot names, the hidden slot and a gender change followed by the inspector, legality, export byte-identical to native Core with only the slot and footer differing, no scroll at 375 px.
      - An injured, burned Meowstic party member changed to female: the note, Competitive (H), EXP, 7 HP and burn kept, no HP preview or recalculated caption, export equal to native Core.
      - `ItemMoveBrowserTests` updated for the longer open message (its 6 cases failed on it until then). Every E2E test executed.
    - **RealSave 86** (up from 74): `RealSaveAbilityGenderRoundTrip`, 3 engines × 2 paths × 2 families. A slot step on the first writable boxed non-egg and a gender change on the first dual-gender, non-Meowstic party member are byte-identical to native Core, and only those two slots and the footer differ.
    - **Mutation checks** (Unit tier; files restored from a scratchpad copy; failing tests):

      | Mutation | Failing tests |
      | --- | --- |
      | Ability ID written without the slot number | 18 |
      | Slot range unchecked | 5 |
      | Choices valued by ability ID | 8 |
      | Choices cached once (not per form) | 3 |
      | Gender unchecked | 10 |
      | Every species either gender | 11 |
      | Meowstic form not changed | 5 |
      | No restore on return | 3 |
      | Gender not set after the form change | 1 |
      | Form change not a stat edit | 1 |
      | Ability gated by the gender flag | 1 |
      | Change note not tracked | 1 |
      | Unmatched pair shown as slot 0 | 2 |
      | Gender box offers all genders | 5 |
      | Fixed gender not read-only | 4 |
      | Ability box not reloaded | 7 |

      Review fixes (failing tests):

      | Mutation | Failing tests |
      | --- | --- |
      | No dual-gender guard | 2 |
      | Undo even after an edit since | 1 |
      | Never undo | 6 |
      | Ability not undone | 2 |
      | EXP not undone | 6 |

      The survivor, "undo record kept after changing back" (0), is equivalent: after the form changes back, the next form change always leads away from the record's "before" form, so the record can never match.
    - **Other checks:**
      - Trim baseline unchanged (38). `PKHeX.slnx` Release builds with 0 warnings.
      - Driven on the private XY save (Chromium, 1280 and 375 px): a stored Meowstic changed gender with the form note shown, no horizontal scroll, and changing back left no draft changes.
  - **Adversarial review** (throwaway probe against the private saves):
    - 968 non-egg entities (553 XY, 415 ORAS, boxes and party):
      - Every stored ability pair names its slot, and every stored gender is allowed.
      - 527 have identical regular abilities and 25 the hidden slot.
      - 297 have a single gender; 2 XY Meowstic.
    - Choosing the stored slot, then every slot and back, and every allowed gender and back, leaves every draft clean, including both Meowstic.
  - **Second adversarial review** (throwaway probes against Core's form lists, the XY and ORAS personal tables and the private saves; mutations):
    - **Fixed:**
      - **Unown's form changed with its gender.** Unown's forms F and M read as gender symbols ('F', 'M') in Core's form list, so an Unown F or M with a wrong stored gender, given the genderless value, became Unown C. WinForms' `ClickGender` returns for any species without two genders before it looks at the form; the draft now does the same.
      - **Changing back put stored values over the user's edits.** Returning to the stored form restored the stored EXP and ability pair whatever had happened since, so an EXP edit made before or after the form change was lost, with the draft looking clean. The draft now records the last form change and undoes only the side effects nothing has changed since.
    - **Checked, not changed:**
      - Core's Gen 6 form lists name genders only for Meowstic (two-gender) and Unown (genderless).
      - XY and ORAS personal data have the same abilities and gender ratios for every species and form, so the ORAS table that `PK6.PersonalInfo` uses gives an XY save the right choices.
      - The 968 private entities still change to every slot and gender and back cleanly, including the 2 Meowstic.
      - After the fixes: Unit 935; E2E `AbilityGenderBrowserTests` 12 and RealSave `RealSaveAbilityGenderRoundTrip` 12 rerun on a new publish and passed. No other app behaviour changed.
  - **Recorded, not changed:**
    - A Meowstic gender change resets EXP within the level, as WinForms (Core's `ChangeSpeciesForm`) does, including when changing back after an EXP edit; the note says so.
    - A Meowstic stored with invalid slot bits is normalised to slot 0 by the form change, as Core documents, and given back by changing back.
    - Choosing the gender a Meowstic stored with a mismatched form already has changes its form to match, as WinForms does (no private save has one).
    - A stored Gen 3–5-origin PK6 whose gender disagrees with its PID is reported by legality. This release changes no PIDs (WEB-PKM-004 leaves PID-derived formats to later correlated-edit support).
    - Whether the hidden ability (or a slot of identical regular abilities) is possible for the encounter is reported by legality, not blocked (M16 acknowledgements).
    - The ability and gender boxes are native selects: each choice is a full edit, announced by the live notes. M18 decides whether that is too chatty.
  - **Not verified yet:** no screen reader; physical devices (G-C).
  - **Compared with PKForge** (`SaveEngineSession.ApplyEdit` 175–273, `GetAbilityChoices`, the `RefreshAbility` "Potential" edit, `TrySetPidDerived`, `MonInfoService.GetAbilityChoices`):
    - **Matches:** slot edits through Core's `RefreshAbility`; choices from the personal data.
    - **Stricter:**
      - PKForge's Gen 5+ ability edit writes the ability ID without `AbilityNumber`, so the pair can disagree; we write both.
      - Its choice list skips duplicate abilities, so slot labels can be wrong; we keep every slot.
      - It clamps gender to 0–2 with no species check; we offer and accept only the species' genders.
      - It does not change Meowstic's form; we do, as WinForms does.
    - **Not adopted:** the HaX "show all abilities" list, and PID-derived gender and ability for Gen 3–5 (other formats).
- **M15 Species/form.** Uses the Phase 1 helper (depends on R2; carry the commit if the PR is not yet merged). The confirmation preview lists the returned changed-field flags (PKM-002).

  **M15 status:** code complete on `web/m15-species-form`. R2's `ChangeSpeciesForm` is already on `web/main` (R1–R3 not yet merged upstream; gate G-E).
  - **Core facts relied on** (checked in source and with a throwaway probe):
    - `FilteredGameDataSource.Species` includes 0 ("---") and 721 species for XY and ORAS.
    - `FormInfo.HasFormSelection` uses the save's personal table, so an XY save has no form choice for 22 species whose extra forms are ORAS-only (cosplay Pikachu, Primal Kyogre/Groudon, the ORAS Megas, Hoopa Unbound).
    - Every Gen 6 form in Core's list has a name; 58 are battle-only (`FormInfo.IsBattleOnlyForm`). In XY, only Scatterbug/Spewpa's Fancy and Poké Ball patterns are listed but not present (`IsPresentInGame`).
    - `ChangeSpeciesForm` never changes a PK6's PID; it does not touch form arguments; the PK6 name setter keeps bytes after the terminator.
    - Gen 6 Furfrou in a trimmed form outside the party is reported by `FormVerifier`.
  - **Draft** (`State/EditorDraft`):
    - `PreviewSpeciesForm(species, form)` returns a `State/SpeciesFormPreview`: Core's returned `SpeciesFormChangeResult` flags, the values before and after (`SpeciesFormValues`: species, form, level, EXP, ability and slot number, gender, name, flag, the six stats as stored for a party member or calculated for a boxed one), the party HP change, battle-only and in-game flags, the new species' forms, and whether it gives back a run. It is refused exactly as the edit would be, changes nothing and is not counted as an edit.
    - `EditSpeciesForm(species, form)` writes the same candidate. Species must be in `SaveCapabilities.SpeciesChoices` (Core's list without 0; `SpeciesNotAvailable`); the form must be in `FormChoices`, or 0 for a species without forms (`FormNotAvailable`, never reset as Core would). It is a stat edit (`PartyStatPolicy`; `PartyStatsMissing` for a member stored without stats).
    - `FormChoices` (`State/FormChoice`: Core's name, battle-only, in game) follows `PKMEditor.SetForms`: personal-data form selection and more than one name. Cached per species.
    - **Changing back.** M14's single-step Meowstic undo is generalised to a run of species and form changes, gender-driven Meowstic changes included: the draft before the run's first change, and the dependent values (species, form, EXP, ability, slot number, gender, name, flag) after its last. Returning to the run's start with none of those edited since gives back the stored EXP, ability pair, gender and name bytes. Editing one of them ends the run; other edits do not.
    - `EditableFields.Species`; `SupportMatrix` grants it to XY and ORAS.
  - **UI** (`Components/DraftEditor`, text in `Components/EditorText`):
    - A "Species and form" fieldset first: `#species` (`ChoiceSelect`, which gains `FocusAsync`) and `#form` ("No alternate forms" when there are none; forms marked "(battle only)" and "(not in this game)"; a stored form outside the list shown as stored); `#species-note` says nothing changes until confirmed, what is kept, and that legality decides.
    - Choosing a species (its first form, or the drafted form for the drafted species) or a form shows `#species-preview` (live): a title and one line per Core flag with before/after values, changed stats, a party member's current HP, a give-back line, and battle-only / not-in-game warnings. `#species-confirm` makes the change; `#species-cancel` drops it; both return focus to `#species`. A refused preview is shown in the region and the boxes show the draft. Choosing the drafted values, any other accepted edit or a new draft ends the preview. `#species-change` (live) repeats what the confirmed change did; any other edit clears it.
    - Text updated: `EditableSummary` ("Its species, form, nickname, …"), `EggReadOnly`, `UserMessages.EggNotEditable`, `PartyText.KeptOnEdit`, two refusal texts, README (intro, Party, Capabilities, a "Species and form" bullet, the Meowstic wording, the real-save round trips).
  - **Tests.**
    - **Unit 989** (up from 935; 987 before the adversarial review):
      - `SpeciesFormDraftTests` (42, 1 from the review): readers; Core's form lists for Charizard, Pikachu and Kyogre in XY and ORAS, with battle-only and Scatterbug's not-in-XY patterns; species changes in XY and ORAS (same growth, different growth, fixed gender, genderless, forms) equal native `ChangeSpeciesForm` writing only species, EXP, ability, gender/form and name bytes, PID/EC/nature/shiny kept, slot kept; previews equal Core's flags and the edit, change nothing, count nothing; growth-rate level change; battle-only form change; species −1/0/722/`int.MaxValue`/`int.MinValue` and out-of-list forms refused (including cosplay Pikachu in X); nicknamed kept, French not-nicknamed renamed; a four-step run and back is clean; stored bytes after the name terminator are given back; an edit of a changed value ends the run and is kept, an IV edit does not; Meowstic gender and form edits share the run; a party member (XY and ORAS) recalculated at 7 HP with its burn through apply and export equals native Core plus the party-stat policy, and changed back keeps its stored battle state; a member stored without stats refused; family and egg gating.
      - `DraftEditorTests` (+12, bUnit, 1 from the review): species and form boxes; battle-only labels; a stored unlisted form; preview without an edit, its lines, confirm, the change note and its clearing; cancel; reselecting the drafted species; another edit ends the preview; a refused preview; a party preview's stat and HP lines; the flag gate; a new draft. Updated: egg read-only, `EditableSummary`, the `ChoiceSelect` count and order. Updated `SaveCapabilitiesTests` (species list without 0, 721 entries).
    - **E2E 233** (up from 209; 221 before the adversarial review): `SpeciesFormBrowserTests`, 3 engines × 2 paths.
      - A boxed Zigzagoon: Charizard's forms with battle-only labels, a Mega X preview warning, cancel with focus back and no draft change; then Linoone previewed (title, rename line) and confirmed; inspector, legality verdict, export byte-identical to native Core with only the slot and footer differing; no scroll at 375 px.
      - An injured, burned party member changed to Chansey: preview with the stored stats (Attack 1) and new HP; recalculated caption, 7 HP kept, export equal to native Core.
      - From the review: a refused preview (a party member stored without stats) puts the species box back to the drafted species; a pending preview disables Apply with `#species-pending` until it is cancelled.
      - Run with a `-p:PKHeXWebSprites=true` publish for the sprite tests. Every E2E test executed.
    - **RealSave 98** (up from 86): `RealSaveSpeciesFormRoundTrip`, 3 engines × 2 paths × 2 families. A species change of the first writable boxed non-egg and of the first party member are byte-identical to native Core plus the party-stat policy, and only those two slots and the footer differ.
    - **Mutation checks** (Unit tier; files restored from a scratchpad copy; failing tests):

      | Mutation | Failing tests |
      | --- | --- |
      | Forms not gated on personal data | 2 |
      | Species 0 offered | 3 |
      | Species unchecked | 1 |
      | Form unchecked | 4 |
      | Preview not settled for a party member | 5 |
      | Never undo | 10 |
      | Undo even after an edit since | 2 |
      | Run not continued | 1 |
      | Name bytes not given back | 1 (0 before `ChangingBackGivesBackTheStoredNameBytes`) |
      | Battle-only not marked | 3 |
      | Not-in-game not marked | 1 (0 before the Scatterbug case) |
      | Gated by the gender flag | 1 |
      | Edit skips the stat policy | 2 |
      | Another edit keeps the preview | 2 |
      | Choice made at once, without a preview | 6 |
      | Refused preview not shown | 1 |

    - **Other checks:** trim baseline unchanged (38). `PKHeX.slnx` Release builds with 0 warnings.
  - **Adversarial review** (throwaway probe against the private saves, deleted): 968 non-egg entities (553 XY, 415 ORAS, boxes and party; 256 with forms), each changed to every species (697,928 changes) and back, and each to every form of its species and back. Every preview's flags equal Core's, every boxed change equals native Core byte for byte, every preview equals the edit it describes, and every change and back left the draft clean. No refusals.
  - **Second adversarial review** (each finding shown by a failing test first, then fixed; mutations):
    - **Fixed:**
      - **A language edit did not end the run.** Core names a Pokémon that is not nicknamed in its language, so after Zigzagoon → Linoone, a French language edit (which keeps "Linoone", another language's name) and a change back, the draft restored the English "Zigzagoon" instead of Core's French "Zigzaton". The language is now part of the run's key.
      - **Reselecting the drafted species could show a false refusal.** For a stored form outside the species' list (Zigzagoon stored as form 3), choosing another species and then the drafted one previewed the stored form and showed "no such form". Choosing the drafted species now just ends the preview.
      - **A refused preview left the browser's box on the refused species.** The value Blazor renders did not change, so it did not reset the select the user had changed (every engine; bUnit cannot see it). Both boxes are now keyed on a refusal count, so a refusal renders them again with the draft's values.
      - **Apply and Download ignored a pending preview.** The boxes showed the previewed species while Apply, Download and the exit panel's apply would have written the draft without it. The editor now reports a preview to `Workspace` (`OnPreviewChanged`), which disables them, guards Apply and export, and shows `EditorText.SpeciesFormPending` until the change is made or dropped.
    - Mutations (failing tests): language not in the run's key 1; drafted species reselected previews its form 1; refusal does not reset the boxes 6 (E2E); pending preview does not hold back Apply 6 (E2E).
    - **Checked, not changed:** a stale `previewing` reference after a new draft cannot hold anything back (compared by reference with the current draft); a language edit also ends the run of a nicknamed Pokémon, which Core would not rename, so changing back then uses Core's values rather than giving the old ones back. That is safe, so it was left.
    - After the fixes: Unit 989, E2E 233 and RealSave 98 on new publishes; trim baseline unchanged (38); `PKHeX.slnx` Release builds with 0 warnings.
  - **Recorded, not changed:**
    - Form timers (Furfrou, Hoopa) are kept as stored, as the desktop editor's species/form change keeps them; legality reports a trimmed Furfrou in a box or a missing timer. PKForge sets the timer to its maximum on entering a timed form; not adopted (no form-argument field in this release; Post-MVP).
    - Rotom's appliance move is not swapped (PKForge does; the desktop does not).
    - Battle-only and not-in-game forms are offered, marked and warned, as the desktop offers them; legality reports them, and M16's acknowledgement gates apply/export of an Invalid result.
    - The preview is a live region inside the editor, not a modal dialog; M18 decides focus trapping.
  - **Not verified yet:** no screen reader; no separate manual drive beyond the E2E runs (Chromium, Firefox and WebKit, both paths, 375 px); physical devices (G-C).
  - **Compared with PKForge** (`SaveEngineSession.ApplyEdit` 144–148, `MonFieldService.GetForm`/`SetForm`, `MonFieldTests`):
    - **Matches:** forms from Core's form list for the entity context; battle-only and not-in-game forms flagged and warned; ability slot kept; EXP following the growth rate.
    - **Stricter:**
      - PKForge's species edit sets `Species` and `Form = 0` only, leaving EXP, ability, gender and name stale; we use Core's `ChangeSpeciesForm`.
      - Its form change syncs gender only for two-entry ♂/♀ lists (the Unown F/M case M14 fixed) and recalculates party stats with `ResetPartyStats`, healing and clearing status; we follow the PK6 party-stat policy.
      - It applies at once; we preview first and give back a run on return.
    - **Not adopted:** form-timer seeding and the Rotom move swap (above).
- **M16 Acknowledgements + lifecycle.**
  - An Invalid or Unavailable legality result needs explicit acknowledgement before apply/export.
  - Reset to original re-parses a fresh copy.
  - Discard session is a separate destructive action.
  - A `beforeunload` warning is shown only while edited.
  - A `pageshow` `persisted` event (page restored from the back-forward cache) clears the session.
  - E2E covers reload, back-forward cache and storage emptiness (SESSION-004/005, SEC-002, BROWSER-003).

  **M16 status:** code complete on `web/m16-acknowledgements-lifecycle`.
  - **User decisions:** the download acknowledgement covers flagged applied changes, not whichever Pokémon is open; Discard session is a top-level button with one confirmation; Apply waits for the current result rather than letting an unresolved one be acknowledged.
  - **Apply acknowledgement** (`State/LegalityGate`, `DraftLegality.Gate`/`Acknowledge`):
    - A changed draft is applied only with a result for it as it is now. A Valid result is Clear. An Invalid or Unavailable result needs an acknowledgement. Any other status is Waiting.
    - The acknowledgement is the result's `LegalityTag`, so any accepted edit withdraws it, even one back to the acknowledged values. `Reset` (every new draft) forgets it.
    - `WorkspaceState.ApplyDraft` refuses with `LegalityNotCurrent` or `LegalityNotAcknowledged` and changes nothing. A clean draft writes nothing, so it needs no result. The exit's Apply draft uses the same check.
  - **Flagged changes** (`SaveSession.FlaggedChanges`, `Apply(draft, verdict)`):
    - Each slot whose latest apply was Invalid or Unavailable is recorded with that verdict, and a later Valid apply of the slot removes it.
    - Pokémon that were already illegal when the file was opened, and were not changed, are not flagged and download without asking (PKHeX.Web.md §MVP).
    - `ExportNeedsAcknowledgement` holds until `AcknowledgeExport` is given for the current revision, so any later apply withdraws it. `SaveExporter.Export` checks it first (`ExportNotAcknowledged`), so no download path skips it. The transaction-only `Apply(draft)` is now internal and is kept for tests.
  - **Reset to original** (`ExitIntent.Reset`, `WorkspaceState.RequestReset`):
    - It parses a fresh copy of the original bytes under the same name (`Reopen`, which tests can replace) before asking anything. XY and ORAS have no interpretation choices to repeat.
    - A failure throws `ResetFailed` and keeps the session, the draft and any exit in progress. PKForge's `RevertToBaseline` closes the session instead; we are stricter.
    - The fresh session waits as the exit's candidate and is resolved like a replace: draft, then session, then download, each step cancellable. Completing it clears the draft, the changes, the flagged changes and the download status.
    - It is offered only once changes are applied.
  - **Discard session** (`ExitIntent.Discard`, `ExitStage.ConfirmDiscard`): one confirmation that names what is lost (`SessionStatusText.DiscardPrompt`: the draft, changes not downloaded, or an unchecked download), with no draft or download step. A file opened meanwhile turns it into a replace. With nothing to lose it closes at once.
  - **Lifecycle** (`wwwroot/lifecycle.js`):
    - `pagehide` with `persisted` hides the document (`data-session-ended`, `visibility: hidden`), so a restored copy never shows or takes input for the old session.
    - `pageshow` with `persisted` clears the leave flag and then reloads, so the reload does not warn again.
    - `beforeunload` stays armed only while `HasUnsavedWork` (unchanged since M1).
  - **UI:**
    - `Components/ApplyAcknowledgement` (`#apply-ack`, `#apply-ack-waiting`) sits in a fixed-height region, so the note and the checkbox take turns without shifting the page. `Components/ExportAcknowledgement` (`#export-ack`, `#export-ack-list`, party first, then box and slot) appears in the Download section and both appear in the exit panel (`#exit-apply-ack`, `#exit-export-ack`).
    - A new Session section holds `#close-session`, `#reset-session`, `#discard-session` and their notes.
    - After a cancelled close, reset or discard, focus returns to the button that started it. The exit panel names a waiting file only for a replace: a reset showed "Waiting to open: main", which a new panel test caught.
    - README updated: acknowledgements, reset, discard, page lifecycle, boundaries. The stale "does not edit species/stats" boundary was removed.
  - **Tests.**
    - **Unit 1020** (up from 989; 1019 before the adversarial review):
      - `LegalityAcknowledgementTests` (10): waiting, Valid, Invalid with acknowledgement and withdrawal, an edit back to the acknowledged values, Unavailable, nothing to acknowledge, a clean draft, a Valid re-apply clearing the flag, the download acknowledgement per revision including `SaveExporter`, reset clearing the flags.
      - `WorkspaceStateTests` (+9): reset by download confirmation and by discard; cancel at each step; at once when unchanged; a failed parse keeping everything; the original bytes copied; discard asking once; discard after a download; discard with nothing to lose; a file opened during a discard.
      - `SessionExitPanelTests` (+5, 1 from the review), `AcknowledgementComponentTests` (4) and `SessionStatusTextTests` (+3).
      - Existing state and panel tests now analyse and acknowledge through `SaveFixtures.ReadyToApply`/`ApplyAsync`.
    - **E2E 260** (up from 233): `LifecycleBrowserTests`.
      - Invalid acknowledgement before apply and before its download, withdrawn by an edit and by a later apply, including in the exit panel. Reset, with focus and a byte-identical download. Discard. Leaving for `about:blank` and coming back, unchanged and edited. These four run on 3 engines × 2 paths.
      - A simulated back-forward restore (hidden on `pagehide`, reloaded on `pageshow` with no dialog) runs on 3 engines.
      - No engine restores a page from its back-forward cache under Playwright, even a plain static page, and even Chromium with `--disable-back-forward-cache` removed (`notRestoredReasons`: `masked`; throwaway probe). So the back-navigation test checks only that a back navigation starts empty, and the restore is covered by the simulation. Real restores belong to G-C.
      - Every existing Apply goes through `ProofPage.Apply`/`ReadyToApply` (wait for a verdict, tick `#apply-ack` if shown). The download helpers tick `#export-ack`/`#exit-export-ack` if shown. The exit Apply-draft E2E acknowledges explicitly. Every E2E test executed.
    - **RealSave 98** pass with the gate.
    - **Mutation checks** (Unit tier unless noted; files restored from a scratchpad copy; failing tests):

      | Mutation | Failing tests |
      | --- | --- |
      | Acknowledgement survives an edit | 1 |
      | Invalid treated as Clear | 4 |
      | Apply without a current result | 1 |
      | Apply without the acknowledgement | 3 |
      | Flag not recorded | 5 |
      | Valid re-apply keeps the flag | 1 |
      | Download acknowledgement ignores the revision | 1 |
      | `SaveExporter` skips the check | 1 |
      | Discard without confirmation | 4 |
      | Failed reset closes the session | 1 |
      | Reset opens at once | 4 |
      | Reset candidate shown as a waiting file | 1 |
      | `lifecycle.js` keeps the leave flag on restore (E2E) | 3 |
      | `lifecycle.js` does not hide on `pagehide` (E2E) | 3 |
      | No acknowledgement at the Download again step (review) | 1 |
      | No focus move after an immediate discard or close (review, E2E) | 12 |

    - **Other checks:** trim baseline unchanged (38); `PKHeX.slnx` Release has 0 warnings. A manual drive of the published app with the private XY save at 1280 and 375 px (party member applied, Session section, discard prompt, reset panel) showed no horizontal scroll and focus on the panel heading.
  - **Recorded, not changed:**
    - WebKit's own text viewer sets an inline style that the app's CSP header blocks (`style-src-attr`). That page is the browser's, not ours, so the back-navigation test leaves for `about:blank`. M20 should decide whether `.txt`/`.md` license files need a laxer header (M20: they get their own text policy).
    - Our `beforeunload` listener is always registered, even when it does nothing; in some engines a listener can make the page ineligible for the back-forward cache. That favours privacy, so it was kept. It is unverified, because Playwright restores no page from the cache at all.
    - The legality acknowledgement is a checkbox, not a dialog; M18 decides focus handling. A refused Apply or Download caused by a missing acknowledgement reports a message but does not move focus to the checkbox.
  - **Adversarial review** (each finding shown by a failing test or probe first, then fixed):
    - **Fixed:**
      - **Download again could be disabled with no way to enable it.** After a download, unticking the acknowledgement in the Download section and then closing reached the confirmation step. There "Download again" was disabled and the panel had no checkbox. The panel now shows the acknowledgement at that step too (new panel test; it failed before).
      - **Focus dropped to the page body after an immediate discard or close.** A Discard session or Close save with nothing to lose completes without a panel and removes the button that had focus (probed in Chromium: `BODY`). The Close case dates from M6. Focus now moves to the open heading (`focusOpenTitle`). The discard and export-flow E2E tests now check this.
      - **The discard prompt** told a user with an unapplied draft only to "cancel and download". It now says to apply the draft too.
      - **A vacuous test was relabelled.** The back-navigation test cannot see the back-forward cache under Playwright (above); its comment and this entry claimed more than it shows. The unverified Firefox `beforeunload` claim was reworded.
    - **Checked, not changed:**
      - PKForge's Pokémon editor save (`BoxBrowserViewModel.SaveEditAsync`) writes with no legality check; the comparison below stands.
      - A checkbox that the state refuses would leave the browser's box out of step with Blazor's value. That is unreachable here, because each checkbox is rendered only while its acknowledgement can be given. `RunAcknowledgement` stays as a guard.
      - The E2E download helpers tick a shown acknowledgement, so they would not notice an unexpected flag; `LifecycleBrowserTests` checks the flags explicitly.
      - Applying a draft during a reset's draft step is allowed, as in a replace, though the reset then drops it.
  - **Not verified yet:** no screen reader; real back-forward-cache restores and mobile tab eviction on physical devices (G-C).
  - **Compared with PKForge** (`SaveSessionService.RevertToBaseline`, `BoxBrowserViewModel.DiscardPartialEdits`, `TransferPreviewPrompt`, `BoxBrowserPage` batch confirm, `App.OnSleep/OnResume`):
    - **Matches:** the reset reopens the stored baseline bytes through the normal open path. Illegal results are confirmed rather than blocked ("Send anyway", the batch "would become ILLEGAL").
    - **Stricter:**
      - PKForge's single-Pokémon save has no legality gate; we acknowledge every Invalid or Unavailable apply and again the download.
      - A failed revert closes its session; ours keeps it.
      - Its revert runs without confirmation after a failed write; our reset is user-initiated and offers a download first.
    - **Out of scope:** Android sleep/resume, restore points and backups; Android has no back-forward cache or unload warning to compare.

### Slice C — hardening, polish, qualification
- **M17 Diagnostics + hostile input.** An opt-in diagnostic preview/copy/download holds build, browser, operation and a sanitised error code, with no save data. E2E renders hostile filenames and nicknames as text, and checks the CSP is honoured with no inline script (SEC-003, SEC-005).

  **M17 status:** code complete on `web/m17-diagnostics-hostile-input`.
  - **User decisions:** an unexpected exception is reported as its type names and method frames, never its message; the browser console gets the same redacted form; names are shown with bidirectional controls removed and are isolated inside sentences; an XY/ORAS layout storing a game outside its family keeps opening, labelled as unknown (as Core and the desktop do; pinned by `SaveOverviewTests`).
  - **Redaction** (`Services/Diagnostics/DiagnosticCode`):
    - It is built only from the app's own names: the outcome (`open.parser-fault`, `open.integrity.round-trip-mismatch`, `session.staged-edit-mismatch`, `unexpected`), the Core save type a refused file was recognised as, up to 4 exception type names (outermost first) and up to 12 method names. The frames come from the innermost exception with a stack, parsed from `StackTrace` as text, with no arguments, files or lines.
    - It never reads `Message`, `Data` or `ToString()`. No `TargetSite`/`StackFrame.GetMethod`, so the trim baseline is unchanged.
    - `SaveLoader` now keeps a parser fault's redacted code on the outcome (`SaveLoadOutcome.Faulted`, `Fault`), so M2's unlogged `ParserFault` is now recorded with where it was thrown.
    - A not-parsed legality analysis is `LegalityNotParsedException`, so the type names it; `NotParsedMessage` was removed.
  - **Log** (`DiagnosticLog`, scoped, in memory only):
    - It keeps the last 20 entries (UTC time, `DiagnosticOperation`, code), across sessions in the tab; a reload empties it.
    - It records every refused file, every unexpected exception, legality Unavailable, workspace faults and the failure-class `SessionError`s (`IsFailure`: staged write, readback, party count, untargeted slot, not writable, foreign/stale draft, reset, export). Refusals of input (level, nickname, acknowledgement, …) are not recorded.
    - It is the one place failures reach the console, as the code text, never with the exception. Every `Logger.Log*(ex, …)` in `Workspace` and `FaultBoundary` now goes through it.
  - **Report** (`DiagnosticReport`, `Components/DiagnosticPanel`, `DiagnosticText`):
    - Nothing is gathered until "Prepare a diagnostic report". The preview `<pre>` (focusable, scrolls) is the whole report: version, commit, Core version, sprites, the user agent (control characters removed, 512 max), the open family or none, and the entries.
    - It never holds the file name, sizes, names, IDs or bytes, and no option adds personal data. SEC-005's "warning before adding personal data" is met by offering none; the preview warns against attaching saves or screenshots with names.
    - **Copy** uses `navigator.clipboard`; a refusal selects the preview to copy by hand. **Download** gives `pkhex-web-diagnostics.txt` (UTF-8, through `BrowserFileService`, not touching the export state). **Clear** and **Close** are offered; focus moves to the preview and back to the button.
    - It is in About (`#about-diag-*`) and on the fault screen (`#fault-diag-*`).
  - **Hostile names** (`Components/DisplayText`):
    - `Plain` removes the 12 Unicode Bidi_Control characters; `Embed` also wraps the name in FSI…PDI. With the controls removed, nothing can close the isolate early.
    - Embedded: slot labels and messages (nickname), box selector and heading (stored box name), the OT/HT inspector rows and friendship labels, name notes and species-preview name lines, and waiting file names in the exit texts. Plain: trainer overview and inspector nickname values, and box captions.
    - The nickname box still shows and edits the stored name.
  - **Found while testing:** the Write tool turned `\u202E`-style escapes into raw bidi characters in three source files (a Trojan Source hazard). They were converted back to escapes, and every source file was scanned for raw Bidi_Control characters at the end.
  - **Tests.**
    - **Unit 1073** (up from 1020; 1076 after the adversarial review):
      - `DiagnosticCodeTests` (15): distinct codes for every outcome and error; recognised type; types and frames without a message carrying a sentinel; frame parsing, cap and length; type-chain cap; not-parsed type; the log's capacity, clock, `Changed`, and a console that never gets the exception; failures versus input refusals.
      - `DiagnosticReportTests` (6): the exact empty report; entries; browser cleaning; no trainer, nickname, box, file name, session id or size from a sentinel save.
      - `DiagnosticPanelTests` (6): nothing before opt-in (not even the user agent); the preview equals the report and excludes the file name; copy; refused copy selects; the download's bytes equal the preview; clear and close with focus.
      - `DisplayTextTests` (20), `HostileInputTests` (2), and one more each in `FaultBoundaryTests`, `SlotTextTests`, `OverviewTextTests` and `ContentSecurityPolicyTests` (no inline script, handler or `javascript:`; no `unsafe-inline`/`unsafe-eval`). `SaveLoaderTests` checks the redacted fault.
      - Text tests updated for isolates (`TestText.Isolated`). `FaultBoundaryTests` is now async-disposed (the panel's interop services).
    - **E2E 278** (up from 260): `HostileInputBrowserTests`, 3 engines × 2 paths.
      - Markup trainer, box and OT names, an RLO+markup nickname and a markup/RLO/reserved file name render as text. No `b`/`i`/`script`/`img[src=x]` is created, no text node in `main` holds a bidi control outside an isolate, the nickname box keeps the stored value, and there are no dialogs, network, storage or CSP violations.
      - No inline script or `on*` attribute after boot; an injected inline script and an inline handler do not run and raise `script-src*` violations (`AppSession.TakeCspViolationsAsync`).
      - A refused file appears in the report as `open.unrecognized` with the build commit and not its name. The download equals the preview. Copy succeeds or leaves the preview selected (Chromium drops the last line end from the selection). Focus moves to the preview and back.
      - Four existing literals updated for isolated names. Every E2E test executed (`trx-all-executed.sh`), with the default and sprite publishes.
    - **RealSave 98** pass.
    - **Mutation checks** (Unit tier unless noted; files restored from a scratchpad copy; failing tests):

      | Mutation | Failing tests |
      | --- | --- |
      | Message included in the code | 8 |
      | Frames keep arguments | 2 |
      | Frames from the outermost exception | 1 |
      | Console gets the exception | 2 |
      | Input refusals recorded | 3 |
      | Fault not recorded | 1 |
      | Loader keeps no fault | 2 |
      | Report includes the file name | 1 |
      | Preview shown before opt-in | 8 |
      | Bidi controls kept | 17 |
      | No isolate | 33 |
      | Nickname not isolated in slot labels | 5 |
      | CSP allows inline script | 2 |
      | Refused open not recorded (E2E) | 6 |

    - **Other checks:** trim baseline unchanged (38); `PKHeX.slnx` Release has 0 warnings.
  - **Adversarial review** (each finding shown by a failing test or probe first, then fixed; throwaway probes deleted):
    - **Probed, holds:**
      - Frames survive the trimmed WASM publish. A JS failure forced behind `Workspace.ShowSection` (a throwing `scrollIntoView`) reported `JSException` with `BrowserPage.FocusAsync` and `Workspace.ShowSection` in all 3 engines. The JS error's text (a fake secret and a file path) did not appear.
      - Core's short and verbose reports and its findings for the hostile save quote no stored name, so rendering them without isolates is safe.
    - **Fixed:**
      - **The download name options embedded the file name without an isolate** ("Original name: …", "Edited name: … (stamped …)"), unlike every other embedded file name. A right-to-left name could carry the stamp's digits and the words after it into its own direction. `FileNaming` strips the controls but not right-to-left letters, so the E2E bidi scan could not see it.
      - **A box or trainer name made only of bidi controls showed as empty.** Those characters are not whitespace, so `StorageView`/`SaveOverview` kept the name, and stripping left "3. " followed by an empty isolate or a blank trainer. `DisplayText.PlainOrNull` treats it as blank, so Core's "Box 3" and "Not set in this save" are shown.
      - **A failed sprite load bypassed the log.** `SpriteCatalog` still wrote to `Console.Error` and never reached the report, so the README's "every unexpected exception" was wrong. `DiagnosticLog` is now a singleton, so the catalog (a singleton loaded before render) records `sprites.not-loaded` with the exception's type. An internal `LoadAsync(bool)` makes the failure path unit-testable.
      - **Frames carried assembly-qualified generic arguments** (`d__23`1[[System.Boolean, System.Private.CoreLib, Version=…]].MoveNext`, seen in the probe). They used up the 200-character cap and could cut off the method name; they are now removed, keeping the arity.
    - Mutations (failing tests): name option not isolated 2; control-only names not blank 2; generic arguments kept 1; sprite failure not recorded 1.
    - **Checked, not changed:**
      - An acknowledgement made against a result that has just changed is recorded as `unexpected` under Acknowledge. It is a benign race, but rare, and it was a console warning before.
      - A drop-zone registration failure is still swallowed: the picker keeps working and drops stay blocked.
      - The report preview `<pre>` has no accessible name (ARIA does not allow naming a generic element); left to M18's axe pass.
    - After the fixes: Unit 1076, E2E 278, RealSave 98 on new publishes; trim baseline unchanged (38); `PKHeX.slnx` Release has 0 warnings.
  - **Recorded, not changed:**
    - Blazor's own framework-level errors outside the fault boundary (and the boot script's) still reach the console in the framework's form.
    - Core's legality finding text is shown as Core writes it; no finding was seen to quote a stored name.
    - A report's preview is a snapshot: failures recorded while it is open appear after Clear or Close and Prepare.
    - WebKit's text viewer and the CSP header for `.txt`/`.md` remain M20's (M16 note; resolved in M20).
  - **Not verified yet:** no manual drive of the published app beyond the E2E runs (the 375 px layout of the panel is unchecked); no screen reader; physical devices (G-C).
  - **Compared with PKForge** (`AboutPopup`, `BoxBrowserPage`/`PokeparkPage` alerts, `BankArchive.SanitizeFileName`):
    - **Matches:** build identity in About.
    - **Stricter:** PKForge has no diagnostic report (only a "diagnostic" label on debug builds), and its alerts show `ex.Message`; we never show or log a message. It does not strip bidi controls from names.
    - **Out of scope:** `BankArchive.SanitizeFileName` covers bank export names (bank storage is out of Web scope); our file names were already cleaned by `FileNaming` (M1).
- **M18 Responsive + accessibility.** Desktop split panes, collapsible tablet panes, and a stacked mobile editor with a return action. Dialog focus is trapped and restored. Validation focuses the summary and then the field. Includes reduced motion, 44px targets, contrast tokens and 400% reflow. Automated axe check in Playwright (bundle `axe-core` in test assets only) (APP-003, A11Y-002/003). With the extras M7–M17 left to it, it is split into three chunks, each its own branch and PR:
  - **User decisions:** the exit panel becomes a native modal `<dialog>` (Escape cancels); the species preview and diagnostic report stay inline. Validation uses an error summary when Apply or Download is activated (`aria-disabled`, still focusable), with `aria-invalid` and an inline error on a refused field; typing never moves focus. Live regions stay only for verdicts and discrete choices; notes under typed fields are read through `aria-describedby`, and legality announces only the final verdict. In scope: read-only field styling, previous/next Pokémon in the editor, and a searchable move/item picker.
- **M18a Responsive layout, visual access, axe.** Panes and their focus moves, colour tokens, targets, reflow, reduced motion, the diagnostic preview's name, and the axe check.

  **M18a status:** code complete on `web/m18a-responsive-layout`.
  - **Panes** (`Components/Workspace`, `State/WorkspaceView`, `wwwroot/app.css`):
    - The open session is a storage pane (overview, changes, party and boxes), an editor pane (the selected Pokémon), then Download and Session. `#export-state` moved into Download, next to the button it describes. The DOM order is the reading and tab order at every width; only CSS places the panes.
    - Wide (75rem and up): side by side, storage 40rem. Medium: stacked, with `#storage-toggle` ("Party and boxes", `aria-expanded`, `aria-controls="pane-storage"`) folding storage away while a Pokémon is open. Narrow (under 40rem): one pane at a time; `#editor-return` ("Back to party and boxes") returns.
    - `WorkspaceView` holds the pane and the fold. Opening a slot, or activating the selected one again, shows the editor; a closed draft, a new session, a fault recovery and a discarded exit draft reset both. A refused open (draft pending) keeps the pane. The width only decides, in CSS, which choice applies, so a resize re-renders nothing and keeps the draft, the selection and the box. Folded storage stays rendered.
    - Focus: on a narrow screen opening a slot moves focus to `#draft-title` (`focusIfNarrow`, the same media query text as the CSS, pinned by a test), since the slot that had it is now hidden; wider screens keep focus in the grid as before. The return action focuses the selected slot (`WorkspaceLayout.SlotElementId`, grid or list), or `#storage-title` when another box is shown.
  - **Visual access:**
    - Colour tokens (`--bg`, `--fg`, `--muted`, `--border`, `--focus`, `--selected`, `--valid`, `--invalid`, `--warning`), light and dark. The page sets its own background instead of `Canvas`. `--warning` changed from `#b26a00` (3.9:1 on white, below AA for the Stale and Unavailable text) to `#8f5600`.
    - Every control is at least 44px tall: text fields, selects and `summary` now too, checkbox and radio labels (`.check`), finding links and About's standalone links. Inline links in sentences are exempt (WCAG 2.5.8).
    - A `prefers-reduced-motion` block; the page has no motion.
    - The diagnostic preview is a named region (`role="region"`, "Report preview"), the M17 axe note.
    - At 320px nothing scrolls sideways. M12's 2px overflow of the inspector stats table no longer reproduces in any engine, even with the "Sp. Atk (lowered by nature)" header (an Adamant party member), so no rule was added for it: a row-header wrap rule written for it failed no test when removed and was dropped.
  - **axe:** `Deque.AxeCore.Playwright` 4.13.0 (MPL-2.0, bundles axe-core) in `PKHeX.Web.Tests` only; the publish and `THIRD-PARTY-NOTICES.md` are unchanged. `Accessibility.AssertNoViolationsAsync` runs every WCAG 2.0–2.2 A and AA rule, none disabled. Injecting it raised no CSP violation in any engine.
  - **Tests.**
    - **Unit 1094** (up from 1076; 1095 after the adversarial review):
      - `WorkspaceViewTests` (13): open and return keep the draft, the selected slot again, a refused open keeps the pane, the fold needs a draft and ends with it, apply and cancel keep the editor, a new session, a fault and a discarded draft reset it, the slot ids, a box not shown, the shared media query, the panes' markup with focus calls, return to the heading.
      - `ContrastTokensTests` (5): both schemes' ratios, the same tokens in both, no colour outside the tokens (comments ignored), the ratio formula.
      - `DiagnosticPanelTests`: the preview's role and name.
    - **E2E 296** (up from 278): every E2E test executed (`trx-all-executed.sh`), with the default and sprite publishes.
      - `ResponsiveBrowserTests`, 3 engines × 2 paths: desktop side by side with focus kept in the grid; phone pane switch, focus to the editor heading and back to the slot, the heading when another box is shown, a refused open; tablet fold, ignored on a wide screen and back; the draft, selection and box kept through 1280 → 375 → 800 → 1280; at 320px no sideways scroll on the start screen, About with a report, the loaded save and a party member's editor, and Download and Apply reachable (nothing on top of them); at 375px every control at least 44px; no motion in any stylesheet rule or shown element, with and without the reduced-motion preference.
      - `AccessibilityBrowserTests`, 3 engines × 2 paths: no axe violation on the start screen, About with a report, the grid, the list, the editor with an Invalid result and its findings, and the exit panel, at 1280 and 375 in light, and at 1280 in dark.
      - `StorageBrowserTests` now uses the return action at 375px, and checks the editor heading's focus. `ProofPage.Select` returns to storage first when the return action is shown.
    - **RealSave 98** pass.
    - **Mutation checks** (Unit tier unless noted; files restored from a scratchpad copy; failing tests):

      | Mutation | Failing tests |
      | --- | --- |
      | Opening a slot does not show the editor | 4 |
      | A hard-coded colour | 1 |
      | The old warning colour | 1 |
      | A slot id for a box not shown | 2 |
      | No focus move to the editor | 1 |
      | A reset keeps the fold | 2 |
      | The script's media query drifts | 1 |
      | The preview unnamed | 1 |
      | Fold without a draft | 1 |
      | Selects under 44px (E2E, Chromium) | 2 |
      | A low-contrast empty slot (E2E, Chromium; axe) | 2 |
      | The narrow layout keeps storage (E2E, Chromium) | 2 |
      | A transition on slots (E2E) | 6 |

    - The transition mutation first survived: the motion check ran while the list was shown, so no slot was on screen. It now also scans every stylesheet rule, and runs on the grid.
    - **Other checks:** trim baseline unchanged (38); `PKHeX.slnx` Release has 0 warnings; no raw Bidi_Control characters in the changed files.
  - **Adversarial review** (each finding shown by a probe or failing test first, then fixed; the throwaway probe was deleted):
    - **Fixed:**
      - **A resize could drop focus to the page body.** With a slot open on a desktop and focus in the box navigation, narrowing the window hid the storage pane and left focus on a hidden element (probed in Chromium: `BODY`); widening to a medium layout with storage folded did the same. `browser.js` now watches the narrow and wide media queries, and when the focused element is no longer shown, focuses the first heading still shown (editor, storage, open). Focus that stays visible is left alone.
      - **A phone could not get back to a pending draft from another box.** After the return action and a box change, every slot refused to open ("Apply or cancel the draft…"), Apply was in the hidden editor and Download was disabled; only browsing back to the draft's box helped. `#editor-resume` ("Back to the selected Pokémon", narrow only, top of the storage pane) shows the editor and focuses its heading.
      - **The returned slot was not the grid's tab stop.** The return action focused the selected slot from script while the roving tab stop stayed where arrow keys had left it (probed: focus `box-grid-0`, tab stop `box-grid-1`), so Tab and Shift+Tab led back to another slot. A focused slot now takes the tab stop (`@onfocus`).
      - **The boot failure screens had widened to 90rem** with `.page`; they are 56rem again.
      - **A test step claimed more than it checked.** The responsive test's "return lands on the storage heading" step never reached that case. It now does: a resize and box change, the return action focusing `#storage-title`, then the resume action.
    - Mutations (failing tests): no focus keeper 2 (E2E, Chromium); resume hidden 2 (E2E, Chromium); focus does not take the tab stop 1.
    - **Checked, not changed:**
      - `WorkspaceView.HasDraft` is set only by opening a slot; every path that creates a draft opens a slot or replaces an existing draft, and every path that closes one resets the view.
      - At a fractional width between 39.99rem and 40rem (or 74.99rem and 75rem) neither the narrow nor the medium rules apply: the panes are stacked with neither action, which loses nothing.
      - Clicking or tabbing into a slot already set the tab stop; the new focus handler only changes script focus.
    - After the fixes: Unit 1095, E2E 296, all executed; trim baseline unchanged (38); `PKHeX.slnx` Release has 0 warnings.
  - **CI run on PR #30 (WebKit, Linux):** axe found `color-contrast` on `#about-toggle` in dark mode: WebKit on Linux draws a native button as white on `#c0c0c0` (1.81:1). macOS WebKit, Chromium and Firefox did not show it, so the local runs passed.
    - **Cause:** buttons, selects and fields kept the browser's own colours, which the tokens and `ContrastTokensTests` never covered, so their contrast depended on the engine and platform.
    - **Shown first:** a new check in `AccessibilityBrowserTests` (`AssertControlsUseTokens`) requires every visible button, select and text field to draw its text and background in token colours. It failed locally in all three engines before the fix (for example WebKit's light button background is the same `#c0c0c0`), so it does not need Linux to catch this.
    - **Fixed:** a `--control` token (`#f2f2f2` / `#2b2b2b`) for buttons and the file button, and `--bg` for selects and fields, all with `--fg` text and a `--border` outline; disabled controls use `--muted` text and a dashed border. `ContrastTokensTests` now also checks `--fg` and `--muted` on `--control` (4.5:1) and `--border` on it (3:1). Checkboxes and radios keep their native look.
    - **Found while fixing:** macOS WebKit draws a select with any colour set at its native 23px and ignores `min-height`, which the 44px target check caught. Selects now have `height: 2.75rem` (44px at the default text size, growing with it); WebKit keeps its arrow (checked in screenshots, light and dark).
    - Not reproduced on Linux WebKit locally (the Playwright image would not fit in the free disk space); the next CI run is the confirmation.
  - **Recorded, not changed:**
    - A narrow screen remembers its pane through a detour to a wider layout (the choice made on the phone is kept).
    - The exit panel, live regions and validation focus are M18b's; prev/next and the picker M18c's.
  - **Visual check:** screenshots of the published app with the private XY save (a party member open) at 1280px light, 800px dark and 375px: side-by-side panes, stacked panes, and the editor alone with its heading at the top.
  - **Not verified yet:** no screen reader; the layout on physical tablets and phones, and their rotation (G-C); no keyboard-only manual drive beyond the browser tests.
  - **Compared with PKForge** (`src/PKForge.App/Views/Kit.cs`, `BoxBrowserPage.cs`):
    - **Stricter:** PKForge sets no semantic or accessibility properties, uses 28–40 dp minimum heights (`Kit.cs:413,759`, `BoxBrowserPage.cs:5148,5268`) and has no accessibility tests; we check names, contrast, targets and reflow, and run axe.
    - **Out of scope:** its Android-only adaptive layout.
- **M18b Modal exit dialog, validation summary, live notes, read-only styling.**

  **M18b status:** code complete on `web/m18b-modal-exit-validation`.
  - **User decisions** (recorded under M18): the exit panel is a native modal `<dialog>` and Escape cancels; validation uses an error summary on activating Apply or Download (`aria-disabled`, still focusable), with `aria-invalid` and an inline error on the refused field, and typing never moves focus; live regions only for verdicts and discrete choices, typed notes through `aria-describedby`, legality announcing only the final verdict; read-only field styling.
  - **Refusals** (`State/FieldRefusal`, `Components/DraftEdit`, `Components/EditorFields`, `Components/FieldError`):
    - Every editor edit carries the id of its control. `WorkspaceState.DraftRefusal` records the last refused one (`RefuseDraftEdit`); `DraftValid` is now derived from it, and `AcceptDraftEdit`, a new draft, a new session, a discarded exit draft and a fault recovery clear it. `SetDraftValid` is gone.
    - The refused control gets `aria-invalid="true"`, and its fieldset's error line (`{fieldset}-error`, after the fieldset's controls, so table cells share one) is added to its `aria-describedby`. `ChoiceSelect` gains `Invalid`. An unexpected failure says `ValidationText.FieldFailed`.
    - A refusal is no longer written to `#message`: that is a live region, so every refused keystroke interrupted. Failures are still recorded, redacted, for the diagnostic report.
  - **Readiness and the error summary** (`State/ActionReadiness`, `Components/ErrorSummary`, `Components/ValidationText`):
    - `ForApply` (busy, not writable, no changes, refused field, pending species preview, legality waiting, not acknowledged) and `ForDownload` (busy, refused field, pending preview, unapplied draft, flagged changes not acknowledged) give every reason in order. They set the buttons' `aria-disabled` and guard their handlers, so the two cannot disagree.
    - `#apply` and `#download` drop `disabled`. Activating one while it cannot act shows `#apply-summary`/`#download-summary` beside it and focuses it (`role="region"`, `tabindex="-1"`, a heading). Each reason is a sentence with a link where there is somewhere to go: the refused field, `species-confirm`, `apply-ack`, `apply`, `export-ack`. Links prevent the default (under `/PKHeX/` a fragment would navigate) and move focus after render. A link into the editor shows the editor pane first (review fix below).
    - The summary follows the reasons as they are resolved, goes once none is left, and is forgotten then, so a later refusal does not bring it back; a new draft or session drops it. Typing never shows or focuses one.
    - The exit dialog's Apply draft and Download keep native `disabled`: their acknowledgement is right beside them.
  - **Announcements:**
    - No longer live: `level-note`, `iv-note`, `ev-note`, `name-note` and the party HP preview. Level/experience, IVs, EVs, both friendship fields and the name fields are described by their notes; `#apply` by `party-hp-preview` while shown.
    - The rename from a flag or language choice moves to a new live `#name-change` (`EditorText.RenameNote`; `NameNote` loses its `renamed` argument).
    - Still live (discrete choices): the species preview and change, nature, ability, gender and move notes, `#message`, the box status.
    - `#legality-status` loses `role="status"`; a visually hidden `#legality-announce` holds "Legality: Valid/Invalid/Unavailable" and is empty while not analysed, pending or stale, so a repeated verdict is announced again.
  - **Exit dialog** (`Components/SessionExitPanel`, `BrowserPage.ShowModalAsync`, `browser.js` `showModal`):
    - `<dialog id="exit" aria-labelledby="exit-title">`, shown with `showModal()` when an exit first renders, before the heading focus. It is removed, not closed, when the exit ends; the workspace's focus return is unchanged.
    - Escape: `browser.js` prevents the `cancel` event's default (Razor has no `:preventDefault` for `oncancel`; it rendered a literal attribute, caught by a unit test), and `@oncancel` cancels the exit unless a download is being prepared. A close the browser forces anyway (Chromium's close-watcher rules) raises `@onclose`: it cancels, or shows the dialog again while busy.
    - `#exit-status` (`role="status"`) repeats the page's message inside the dialog, since `#message` is inert behind it.
    - Token colours, a `--fg` backdrop at 40%, `width: min(56rem, 100vw - 2rem)`, `max-height: 100dvh - 2rem`, scrolling inside.
    - The picker and drop zone are inert while it is open. The `Held` state path is kept; the PublishedAppTests step that sets a file during an exit now says it is not a user path.
  - **Read-only styling** (`app.css`): read-only fields keep `--fg` text, with the `--control` background and a dashed border; `button[aria-disabled="true"]` looks like a disabled button; a refused field gets a 2px `--invalid` border and its error line `--invalid` bold text. The rules are written as specifically as the field rule (found while testing: written plainly, both lost to it, and the E2E check failed).
  - **Tests.**
    - **Unit 1158** (up from 1095; 1156 before the first adversarial review, 1157 before the second):
      - `ActionReadinessTests` (15 with cases): every reason, the order, none when ready.
      - `ErrorSummaryTests` (29 with cases): nothing without a reason, the region, sentences and links, a link asking for focus without navigating, field error text, every reason's text and target, every control's fieldset, unknown ids.
      - `WorkspaceValidationTests` (8, 1 from the review): Apply and Download focusable (`aria-disabled`, `type="button"`), activating Apply, a refusal on its control and not in `#message`, the summary's link and its correction, a resolved activation not coming back, Download linking to Apply, a link into the hidden editor, a new draft dropping it.
      - `DraftEditorTests` (+6): a refused text field, its clearing, a table cell, a choice, a failure without a reason, every edit naming its control; note and describedby tests updated.
      - `SessionExitPanelTests` (+4, 1 from the second review): a modal dialog shown once and named by its heading with the status, Escape and busy Escape, a forced close and busy reopening, a dialog shown open when the modal fails. Now async-disposed (`BrowserPage`).
      - `WorkspaceStateTests` (+1): the refusal's lifetime. `LegalityPanelTests`, `PartyHpPreviewTests` updated.
    - **E2E 332** (up from 296; 326 before the second adversarial review), every test executed (`trx-all-executed.sh`), with the default and sprite publishes. The last full run after the review fix had one failure: M18a's resize step in `ResponsiveBrowserTests` (Chromium, `/PKHeX/`) did not see focus on `#draft-title`. It passed in the run before and in 10 of 10 reruns, so it is recorded below as intermittent:
      - `ValidationBrowserTests`, 3 engines × 2 paths: a refused level marked, described, with the `--invalid` border, focus kept and `#message` unchanged; Apply reached by keyboard, Enter focuses the summary, its link (Option+Tab in WebKit, as in Safari) focuses the field with no navigation; corrected, the mark goes and `#legality-announce` holds the verdict. Download at 375px from the storage pane: summary, its link shows the editor and focuses Apply; applied and acknowledged, Download acts. Read-only fields: `--fg` on `--control`, dashed, against an editable field's `--fg` on `--bg`, solid.
      - `ValidationBrowserTests` (second review): Apply pressed at once after typing focuses a waiting summary; when the Valid result resolves it, focus returns to Apply.
      - `ModalExitBrowserTests`, 3 engines × 2 paths: `:modal`; 8 Tabs and 8 Shift+Tabs never focus behind the dialog; a forced click on Download behind it does nothing; Escape cancels with focus on Close save and the draft kept; a browser-forced close (`dialog.close()`) cancels the same way (second review); Escape also at the download step (with `#exit-status`); at 320px no sideways scroll and every choice fully on screen when scrolled to.
      - Updated: the refusal assertions in ItemMove, IvEv, LevelNature and NameFriendship read the fieldset's error line; the rename reads `#name-change`. `ToBeDisabled` on `#apply`/`#download` holds unchanged (Playwright treats `aria-disabled` as disabled).
    - **RealSave 98** pass.
    - **Mutation checks** (Unit tier unless noted; files restored from a scratchpad copy; failing tests):

      | Mutation | Failing tests |
      | --- | --- |
      | Apply natively disabled | 1 |
      | Summary not focused | 4 |
      | Summary link navigates | 1 |
      | Refusal in the status message | 1 |
      | No `aria-invalid` | 4 |
      | Level note live | 1 |
      | Legality announces Pending | 3 |
      | Escape ignored | 1 |
      | Busy Escape cancels | 1 |
      | Forced close not shown again while busy | 1 |
      | Dialog not shown as modal | 1 |
      | No status in the dialog | 1 |
      | Refusal kept by a new draft | 1 |
      | Activation not forgotten | 1 |
      | Busy not a reason | 2 |
      | Unapplied draft not a download reason | 3 |
      | Error line in the wrong fieldset | 1 |
      | Wrong control id for EV edits | 2 |
      | Name change not live | 1 |
      | Summary not reset by a new draft | 1 |
      | `show()` instead of `showModal()` (E2E, Chromium) | 2 |
      | Read-only fields unstyled (E2E, Chromium) | 2 |
      | Invalid border loses to the field rule (E2E, Chromium) | 2 |
      | Summary link does not show the editor (review) | 1 |
      | Failed modal not shown open (second review) | 1 |
      | Summary title not a heading (second review) | 1 |
      | Resolved summary does not refocus (second review, E2E, Chromium) | 2 |
      | Forced close ignored (`@onclose` removed; E2E, Chromium) | 2 |

    - **Other checks:** trim baseline unchanged (38); `PKHeX.slnx` Release has 0 warnings; no raw Bidi_Control characters in the changed files.
  - **Adversarial review** (each finding shown by a failing test first, then fixed):
    - **Fixed:** **a Download summary link could lead into a hidden pane.** On a phone with the party and boxes shown, the editor pane is hidden, so "Go to Apply changes" (and a refused field or pending preview) focused nothing. A link into the editor now shows the editor pane and focuses its target once rendered (`Workspace.ShowReason`). The E2E Download test now starts from the storage pane.
    - **Checked, not changed:**
      - A refused species or form preview is not an edit, so it stays in its own live region rather than becoming a field error.
      - The exit dialog is removed while open when an exit ends; every engine took it out of the top layer (the export-flow and lifecycle E2E tests continue on the page afterwards).
      - Legality Waiting has no link: Analyze now is disabled while a run is in progress, and the reason resolves itself.
  - **Second adversarial review** (each finding shown by a failing probe first, then fixed; the probes were folded into the tests above and deleted):
    - **Fixed:**
      - **Focus dropped to the page body when a focused summary resolved.** Apply pressed right after typing shows "Apply is available once legality has analysed the draft as it is now" and focuses it; when the Valid result arrived the summary was removed with focus inside it (probed: `BODY` in all three engines). A summary that goes now returns focus to its button (`browser.js` `focusIfLost`, only when focus is on the body). The same holds for a Download summary that showed only "being prepared".
      - **A dialog that could not be shown as a modal was invisible.** A `dialog` that is not open is not displayed, so a failed `showModal` left the exit unresolvable while the code comment claimed it was "shown in place". It is now rendered `open` on failure, and the next exit tries the modal again.
      - **The summary title was a paragraph**, not the heading the plan and README describe; it is an `h3` now, under the editor's and Download's `h2`.
    - **Probed, holds:** a browser-forced close (`dialog.close()`, as Android back or repeated Escape under Chromium's close-watcher rules) reaches `@onclose` in all three engines and cancels the exit; the probe is now part of `ModalExitBrowserTests`. An edit that comes back Invalid keeps the summary with its acknowledgement reason rather than removing it.
    - **Checked, not changed:**
      - Legality Waiting stays without a link: `DraftLegality.AutoRun` has no switch in the UI, so the result always arrives by itself.
      - Apply on an egg says "no changes to apply"; the egg note above the fields already says why nothing can change.
      - In dark mode the backdrop (`--fg` at 40%) lightens the page behind the dialog rather than darkening it; the dialog keeps its border and full-contrast tokens, and axe passes in dark mode.
  - **Recorded, not changed:**
    - WebKit and Safari reach links only with Option+Tab by default, which also applies to legality finding links; a keyboard user there needs that setting or key.
    - The exit dialog's Apply draft and Download stay natively disabled (their acknowledgement is beside them in the dialog).
    - `ResponsiveBrowserTests` failed once in a full local run at the resize that hides the focused box navigation (focus not yet on the editor heading), then passed 10 of 10 alone. `browser.js` moves focus one animation frame after the media query changes, well within the 5s expectation, so the cause is not known; M18b does not touch that path. Watch it in CI.
    - M18c's previous/next Pokémon and the searchable move/item picker are still to come.
  - **Not verified yet:** no screen reader (announcements, the dialog's name, the summary's reading); physical devices and their Escape equivalents, such as Android back (G-C); no manual keyboard-only drive beyond the browser tests.
  - **Compared with PKForge** (`Views/PadMenu`, `Services/TransferPreviewPrompt`, `BoxBrowserPage` confirmations and steppers):
    - **Matches:** confirmations are modal overlays with a scrim that take over input while open, and B (the gamepad's back) cancels, as Escape does here.
    - **Stricter:** PKForge sets no semantic or accessibility properties, so its overlays have no focus containment or names for assistive technology; ours is a native modal dialog with a named heading and focus return. Its numeric steppers clamp (`Math.Clamp`, `BoxBrowserPage.cs:594,2846`), where we refuse and say why on the field; it has no error summary or disabled-action explanation.
    - **Not adopted:** a scrim tap that cancels (PadMenu's overlay closes on a tap outside); a native modal does not light-dismiss, so a stray tap cannot discard an exit step.
- **M18c Previous/next Pokémon and the searchable move/item picker.**

  **M18c status:** code complete on `web/m18c-prev-next-picker`.
  - **User decisions:** the picker is a search field above each native select (not an ARIA combobox or a `datalist`), for the four move boxes and the held item only (species keeps its select and confirmed preview). Previous/next walks one sequence, the party then every box, wrapping, skipping empty slots and bad eggs. A draft with unapplied or refused changes is never replaced: the buttons stay focusable and say why, as Apply does.
  - **Previous and Next Pokémon** (`Services/StorageView.Neighbour`, `State/WorkspaceState.Step`, `State/ActionReadiness.ForStep`, `Components/Workspace`):
    - `Neighbour(session, from, ±1)`: party positions 0–5, then every box slot in Core's order (`BoxCount` × `BoxSlotCount`), wrapping. It skips what cannot be opened, with the same test as `SlotSummary.CanOpen` (`ReadOccupied` plus `PKM.Valid`); eggs open, read-only. It reads one position at a time from the current revision, only on activation; it returns null when nothing else can be opened, and refuses any other direction or a position outside the save. `StorageView.Slot` summarises one position for the opened message.
    - `Step(direction)`: `DraftPending` for a dirty or refused draft, decided before anything is read; `NoOther`; otherwise `ShowBox` for a box target, so the storage browser follows, then the existing `OpenSlot`. Its own `StepOutcome`/`SlotStep`, so `SlotOpening`'s `_ =>` arm cannot absorb a new value.
    - `ForStep` reasons, in order: busy, refused field, pending species preview, unapplied draft. The preview is stricter than opening a slot from the grid (which ignores it): the buttons sit beside it, and it would otherwise vanish unannounced.
    - UI: `<nav id="draft-steps" aria-label="Other Pokémon in this save">` under `#draft-slot`, with `#draft-prev` and `#draft-next` (`aria-disabled`, handlers guarded). Activated while blocked: `#step-summary` (`ErrorSummary`, `h3`), focused, with links to Apply changes, the refused field or the species confirmation; it follows, resets and returns focus to its button as Apply's does. Opened: the grid's message (`OpenedMessage`, shared with `OpenSlot`) and reset path; focus stays on the button and the editor pane stays shown at every width. No other: "No other Pokémon in this save can be opened."
    - `SlotGrid`: the roving tab stop now also moves to a changed `Selected` slot shown in the grid (a party member, no draft or the same selection leave it where the user put it), so Tab and the phone's return action land on the stepped-to slot.
  - **Searchable moves and held item** (`State/ChoiceFilter`, `Components/ChoiceSearch`, `Components/ChoiceSelect`):
    - `ChoiceFilter.Filter(items, query, keep)`: names containing the search after folding both (FormD, non-spacing marks dropped, invariant upper case, letters and digits only), so "poke ball" finds "Poké Ball", "uturn" "U-turn", "kings rock" "King's Rock". Folding is done in managed code, not by culture collation, so it does not depend on the runtime's ICU data. It always keeps the drafted value and "(None)", so filtering never changes a choice; it keeps Core's order; a blank search returns the list itself. Folded names are cached per list (`ConditionalWeakTable`). Counts exclude "(None)" and duplicates.
    - `ChoiceSearch`: `<input type="search" id="{Id}-search">` with a visible "Search" label whose visually hidden rest completes the name ("Search moves for Move 2", "Search held items"; first review fix below), described by a note that is not live (`#{Id}-search-note`: "Type to search the 617 moves.", "6 of 617 moves match.", "No move matches. The list below keeps only the current choice and (None)."), then the `ChoiceSelect`. The search is the component's own state, so a keystroke renders only it and its box; a search that finds the same entries keeps the shown list, so the box is not rebuilt. It is disabled with its box and cleared when the draft is replaced (after apply or cancel too).
    - `app.css`: `.choice-search` stacks the search, note and box in the move cell; `.draft-steps` puts the buttons side by side, wrapping on a phone.
  - **Found by the browser tests, fixed:** narrowing a box showed "(None)" while the draft still held its move, in all three engines. Blazor rewrote the unkeyed options in place, so the selected option element became another entry, and the select's unchanged value was not set again. `ChoiceSelect` keys every option by value, so the drafted option stays the same element and stays selected.
  - **Tests.**
    - **Unit 1206** (up from 1158; 1205 before the adversarial review):
      - `SlotStepTests` (14 with cases): party → box → wrap order in XY and ORAS, backwards, empty slots, a bad egg and party positions past the count skipped and eggs not, stepping from an unopenable position, one Pokémon (none other), the direction, a position outside the save; `Step` opening and showing the box, a party member leaving the box shown, refusing dirty and refused drafts without changing the box, no other keeping the draft, reading the current revision.
      - `ChoiceFilterTests` (16 with cases): the same list for a search with no letter or digit, case, accents and punctuation, the drafted value and "(None)" kept in order, no match, an unlisted drafted value, folding, Core's move and item lists.
      - `DraftEditorTests` (+7, 1 from the review): ids, names and the note's description, visible search labels with the held item's label on its box; narrowing without an edit, a choice from the narrowed box; accented items and no match; an unlisted stored value while searching; a search keystroke renders only its own box (and none for the same entries); a new draft clears the search. The egg test covers the disabled search fields.
      - `WorkspaceValidationTests` (+4): focusable steps and their summary with the Apply link, a refused field linking to it, a step opening the next Pokémon, following into its box with the grid's tab stop and wrapping without moving focus to the heading, no other.
      - `SlotGridTests` (+1), `ActionReadinessTests` (+3 with cases), `ErrorSummaryTests` (step reasons' text and links).
    - **E2E 344** (up from 332), every test executed (`trx-all-executed.sh`), with the default and sprite publishes:
      - `PickerBrowserTests`, 3 engines × 2 paths: "THUN" narrows Move 1 to the six matches, Tackle and "(None)" with the draft unchanged; Thunderbolt chosen from it with its PP note; "poke ball" finds Poké Ball, chosen, and a search matching nothing keeps it; apply clears the search; the export byte-identical to native Core with only the slot and footer differing; at 320px no sideways scroll and a 44px search above its box.
      - `NavigationBrowserTests`, 3 engines × 2 paths: Next from party 1 to box 1 and box 3 (the empty slots skipped, the box selector following, focus kept, the slot the grid's tab stop), wrapping, Previous; with an unapplied change the summary is focused and its link focuses Apply; at 375px the editor stays shown and the return action focuses the stepped-to slot.
      - `AccessibilityBrowserTests`: axe on the editor with a narrowed move box and the step summary, and the controls drawn in token colours.
    - **RealSave 100** (up from 98): `StepsVisitEveryOpenablePokemonInOrder` on the private XY and ORAS saves: Next visits every position the party and boxes show as openable, in order, once, and wraps; Previous the reverse (counts only in messages).
    - **Mutation checks** (Unit tier unless noted; files restored from a scratchpad copy; failing tests):

      | Mutation | Failing tests |
      | --- | --- |
      | No wrap | 6 |
      | Empty slots not skipped | 11 |
      | Bad eggs not skipped | 6 |
      | Step ignores a dirty draft | 1 |
      | Box not shown on a step | 2 |
      | Filter drops the drafted value | 3 |
      | Filter drops "(None)" | 6 |
      | Accent-sensitive folding | 4 |
      | Punctuation kept | 5 |
      | Search note live | 2 |
      | Search not cleared by a new draft | 2 |
      | Search not disabled with its box | 2 |
      | Same entries rebuild the box | 1 |
      | Grid tab stop does not follow | 2 |
      | Pending preview not a step reason | 2 |
      | Steps natively disabled | 1 |
      | Step summary not focused | 2 |
      | Step summary not reset by a new draft | 1 |
      | Step moves focus to the heading | 1 |
      | Options not keyed (E2E, all engines) | 6 |

    - **Other checks:** trim baseline unchanged (38); `PKHeX.slnx` Release has 0 warnings; no raw Bidi_Control characters in the changed files.
  - **Visual check:** the published app with the private XY save in Chromium at 1280px light, 800px dark and 375px: Next walked from the party into box 1 with focus kept on the button, the move and item searches narrowed their boxes with the draft unchanged, and nothing scrolled sideways. The blank search note was shortened after seeing it repeated in four move rows on a phone.
  - **Adversarial review** (throwaway probes in the published app with the private XY save, and a throwaway E2E probe in all three engines, deleted afterwards; the fix shown by a failing test first):
    - **Fixed:** **the held item's label sat on the search field.** "Held item" was directly above the new, empty search box, so a sighted user read the search as the held item field, while clicking the label focused the select further down; and no search field had a visible label, so speech input users could not know their names. Each search now has a visible "Search" label (the rest of the name visually hidden, so the visible text starts the name), and the held item's label is rendered directly above its box (`ChoiceSearch.SelectLabel`). `DraftEditorTests.SearchFieldsHaveVisibleLabelsAndTheItemLabelSitsOnItsBox` failed before the fix.
    - **Probed, holds:**
      - Escape in a search field: Chromium and Firefox clear it and fire `input`, so the full list returns; WebKit keeps both the text and the narrowed list. The field and the list never disagree.
      - The box shows the drafted value through narrowing, clearing, and a search that excludes a move chosen from an earlier search, in all three engines.
      - 40 auto-repeated Enter presses on Next Pokémon (the XY save, Chromium): 0.7 s, focus kept on the button, no page or console errors.
      - Keystroke cost with all five searches active: about 3.3 ms per friendship keystroke against 3.2 without (Chromium, synchronous event handling), so refiltering on every editor render is left as it is.
      - A legality result for the Pokémon stepped away from is never shown for the next one: results are tied to the draft instance (`DraftLegality.Result`, `IsCurrent`), as for opening from the grid.
      - The step summary returns focus to the button that showed it (a mutation sending it to Apply fails a Unit test).
    - **Checked, not changed:** pressing Next again in a save with no other Pokémon repeats the same status text, which a screen reader may not announce twice; the first press says why.
    - After the fix: Unit 1206; E2E 344, every test executed; trim baseline unchanged (38). One earlier run of `ResponsiveBrowserTests.PanesFollowTheWidthWithoutLosingTheDraft` (Chromium, root path) failed, then passed 3 of 3 alone: the resize step M18b records as intermittent; its message was not captured.
  - **Checked, not changed:**
    - Playwright will not click an `aria-disabled` button, and WebKit (as Safari) does not focus a clicked button; the tests use the keyboard where that matters, as a user reaching the button would.
    - Type-ahead on a closed move box still edits per keystroke, so `EditMove`'s PP Ups carry rule stays; the search is the way to find a move without passing through others.
    - The searches are cleared after an apply, since apply opens a new draft.
  - **Recorded, not changed:**
    - Opening a slot from the grid still ignores a pending species preview (M15 behaviour); only the steps wait for it.
    - Matches are listed in Core's alphabetical order, with no prefix-first ranking.
  - **Not verified yet:** no screen reader (the search fields' names and notes, the step navigation's name); physical devices (G-C); no manual keyboard-only drive beyond the browser tests.
  - **Compared with PKForge** (`PKForge.Domain/MonSummary.cs` `SummaryNavigation.Step`, `Views/MonSummaryScreen.cs`, `Views/PickerMenu.cs` `Filter`, `Views/InfoPickers.cs`; WinForms `MoveChoice`/`PKMEditor` autocomplete):
    - **Matches:** stepping skips empty slots and wraps; the picker is a substring match ignoring case, with "(None)" always offered.
    - **Stricter or different:** PKForge steps only within the current box or the party, in a read-only summary, does not skip bad eggs, and its editor's `SelectSlot` silently discards unapplied edits; ours crosses from the party through every box inside the editor and never replaces unapplied work. Its filter is accent-sensitive and uses hard-coded English `GameInfo` names; ours folds accents and punctuation and searches the session's `FilteredGameDataSource` lists. WinForms has no next/previous Pokémon and matches by prefix only (`AutoCompleteMode.SuggestAppend`).
    - **Not adopted:** L/R shoulder bindings (no keyboard shortcut was asked for), PKForge's "Show all" (HaX) filter, legal-first move ordering (`MoveChoiceOrder`, WinForms' `LegalMoveComboSource`: the UI never claims a move is learnable, WEB-PKM-011), and keyword search by type, category or learn method.
- **M19 Full published journey E2E.** 3 engines × 2 paths with synthetic fixtures. Picker and drop → select box + party → edit one field from each group → legality → apply → export → reopen → assert fields plus unchanged bytes outside the slot and checksum regions. Privacy trace and keyboard-only run. The RealSave tier gets the same journey for local G-D runs (TEST-004/005).

  **M19 status:** code complete on `web/m19-published-journey`. Tests only: no app code changed.
  - **Choices made** (no user decision was needed):
    - The family follows the hosting path: XY at the root and ORAS under `/PKHeX/`. Each engine therefore covers both families, and both an Invalid result (acknowledged before apply and download) and a Valid one, without running every journey twice.
    - The keyboard run edits text fields and ticks checkboxes; it does not change selects. Type-ahead on a closed select differs between engines, and the full journey already covers every select.
  - **Plan** (`JourneyPlan`, Core only):
    - **Targets:** the first writable boxed PK6 that is not an egg, and the first party member that has party stats, is not an egg, can be either gender, is not Meowstic, has a first move, and has no more than 3 PP Ups on it. A save with no such member is refused with a message that carries no stored value.
    - **The boxed Pokémon gets:** species (Zigzagoon↔Linoone, previewed and confirmed), nickname plus its flag, OT friendship (and HT friendship when there is a handler), nature, held item, and move 1 chosen through its search field.
    - **The party member gets:** level, a Speed IV, an EV, the next ability slot, the other gender, and move 1's PP. It then follows the party-stat policy: stats recalculated, the stored status kept, HP never raised.
    - Together the two cover all nine editor fieldsets. Each native recipe is the one the RealSave round trips already proved.
    - `FirstWritableNonEgg`, `LevelStep`, `IvStep` and the EV choice moved here from `RealSaveBrowserTests`, so both tiers share one recipe.
  - **Driver** (`PublishedJourney`), all steps in one session:
    1. The picker opens a decoy, and a drop replaces it at once, since the decoy has no changes. `FileDrops` now holds the drop script that `FileInteropTests` had.
    2. A no-op download equals native Core's output.
    3. Each slot is edited, its legality is compared with native Core (verdict and report), and it is applied.
    4. The edited download equals native Core's output byte for byte, with the stamped name. It reopens natively with both Pokémon byte-equal to the plan, and nothing differs outside the two slots and the checksum footer.
    5. Reopened in the app through the picker (`#exit-continue`): every edited control holds its value, legality agrees with native Core again, and a no-op download gives back the same file.
    6. Privacy trace:
       - The page address never changes, and no dialog appears.
       - No console message contains the typed nickname, the file name, the trainer name or the box names. `AppSession.ConsoleMessages` records message text only, and that text never reaches an assertion message.
       - Nothing reaches the network or storage after boot, there are no page errors, and the given byte arrays are unchanged.
  - **Tests.**
    - **Unit 1215** (up from 1206; 1213 before the adversarial review): `JourneyPlanTests` (9 with cases).
      - The steps reach exactly the nine fieldsets the editor renders (`fieldset[id]` through bUnit).
      - Every step names a control the editor renders, including the search fields.
      - Every step changes the value its control shows, and the name flag starts unticked (adversarial review).
      - The native output reopens with both edits, changes nothing else, and keeps the party member's status and HP.
      - A save with no editable party member is refused.
    - **E2E 356** (up from 344). Every test executed (`trx-all-executed.sh`), with the default and sprite publishes; the full local run took 17 min.
      - **`JourneyBrowserTests.FullJourneyMatchesNativeCore`** (3 engines × 2 paths): the journey above on a synthetic save with a sentinel box name and file name. A second fresh visit then opens only the decoy and selects a party member. It must make the same boot requests and none after (WEB-SEC-001).
      - **`JourneyBrowserTests.KeyboardOnlyJourney`** (3 engines × 2 paths), using only key presses:
        - Open the file input with Space and answer the file chooser.
        - Enter on the party grid's tab stop; type the level and friendship; tick a shown acknowledgement with Space; Enter on Apply.
        - Enter on the box grid's tab stop; type a nickname, which ticks the flag by itself.
        - Tick the download acknowledgement, then Enter on Download. The download equals native Core's output.
        - A counter of trusted pointer, mouse and touch presses must stay at 0.
        - `TabToAsync` goes forward with Tab, or back with Shift+Tab when the control comes earlier in the page. WebKit adds Option, as in Safari.
    - **RealSave 112** (up from 100): `RealSaveFullJourney` runs the same journey on the private XY and ORAS saves (dropped as `journey-private.sav`), with their trainer and box names as sentinels, and writes `journey-*.json` evidence (pass flags and the number of fieldsets only).
    - **Mutation checks** (files restored from a scratchpad copy; app mutations published to a scratchpad folder; failing tests):

      | Mutation | Failing tests |
      | --- | --- |
      | The journey skips the nature fieldset | 2 |
      | The plan heals the party member (Unit, plus E2E Chromium: "The edited download differs from native Core") | 4 |
      | The app writes the typed nickname to the console (E2E, Chromium: the privacy trace) | 2 |
      | `#apply` taken out of the tab order (E2E, Chromium: "#apply could not be reached with the keyboard") | 2 |
      | The keyboard run clicks Apply once (E2E, Chromium: the pointer counter) | 2 |
      | The plan sets the nature it already has (adversarial review; passed every test before) | 4 (2 Unit, 2 E2E Chromium: "The nature step would not change the value shown") |

    - **Other checks:** trim baseline unchanged (38); `PKHeX.slnx` Release has 0 warnings; no raw Bidi_Control characters in the changed files.
  - **Found while writing:**
    - Typing a nickname ticks the flag (`NameRules.FlagAfterTyping`), so pressing Space on it afterwards cleared it. The keyboard run now checks that the flag was ticked by typing.
    - In Firefox, Tab past the last control moves focus into the browser's toolbar, and the page never gets it back. A forward-only search for an earlier control therefore failed; the helper now goes back with Shift+Tab, as a keyboard user would.
  - **Adversarial review** (each finding shown by a probe or a surviving mutation first, then fixed; the throwaway probe was deleted):
    - **Fixed:**
      - **A step that changed nothing passed every test.** With the plan setting the nature the Pokémon already had, the Unit tests and the full E2E journey all passed: native Core's recipe was a no-op too, so the byte compare agreed, and the claim that every field group is edited was not enforced. On the private saves, nothing showed the recipes always change their field. The driver now checks that each control does not already show the step's value before acting, which covers the private saves too. `JourneyPlanTests.EveryStepChangesTheValueItsControlShows` checks the same on the synthetic saves. A probe of the synthetic saves showed every step changing its control (for example nature 5 → 3, PP 35 → 34).
      - **The name flag step did not edit anything.** Typing the nickname already ticks "Is nicknamed", so `CheckAsync` was always a no-op. The step is now `JourneyAction.Ticked`: it checks the flag was ticked by typing, and the Unit guard checks it started unticked.
      - **The private journey's file name was a weak sentinel.** It was dropped as "main", which the case-insensitive console check would match in any unrelated message containing the word. It is now `journey-private.sav`.
      - **The keyboard run did not check the console,** although it types a nickname. It now checks the nickname, file name and box name.
    - **Probed, holds:** the console-leak mutant (the app writing the typed nickname) fails the full journey in Firefox and WebKit as well as Chromium, and `#apply` out of the tab order fails the keyboard run in all three.
    - **Checked, not changed:**
      - The two visits' boot requests are made before any save is opened, so their equality only shows a deterministic boot. That nothing fetched depends on the save rests on the check that nothing is requested after boot, which both visits make. With sprites, `SpriteCatalogBrowserTests` already compares two saves' requests.
      - `ProofPage.AssertOnlyRangeDiffers` exempts the last 0x200 bytes as the Gen 6 checksum footer, as every round trip before it does; the byte compare with native Core covers that range.
    - After the fixes: Unit 1215; journey E2E 12 and RealSave journey 12 pass.
  - **Review** (of the tests themselves):
    - The healing mutation passed the legality comparisons, since Core's legality does not judge stored battle stats. The byte compare of the download caught it.
    - The console check matches only values of at least 4 characters, so a short trainer name cannot match unrelated text. The synthetic and private saves have longer sentinels: the nickname and file name are always checked.
    - The trace comparison covers boot requests only; that is enough, because nothing is requested after boot in either visit.
  - **Not verified yet:**
    - No hand drive of the published app beyond these browser runs: the keyboard-only journey is automated in Playwright, not done by hand.
    - No screen reader; physical devices (G-C).
    - The first CI run of the new tests.
  - **Compared with PKForge** (`tests/PKForge.Engine.Tests/SaveRoundTripTests.cs`, `SaveWriteSafetyTests.cs`, `StatOrderAndNoOpEditTests.cs`):
    - **Matches:** an unchanged round trip is byte-identical, and an edit is serialised, revalidated and reopened with the field kept (`EditThenSerializeRevalidatesAndPersistsField`, one nickname).
    - **Stricter:**
      - Ours runs through the shipped static app in three engines rather than the engine API, edits every field group across a box slot and a party member, and compares the whole file with native Core.
      - PKForge's `ScopedWriteIsRefusedWhenAnUntargetedSlotChanges` checks that bytes outside the target slot are unchanged. We assert the same on every export, plus the party position.
      - PKForge has no UI journey, keyboard, privacy-trace or accessibility tests.
    - **Out of scope:** its emulator-container and ROM-hack write-safety cases.
- **M20 Hosting + release.**
  - Checked-in `wwwroot/_headers` (Cloudflare: CSP, `nosniff`, referrer policy, immutable cache for fingerprinted assets, revalidate for `index.html` / boot json) and a meta-CSP fallback.
  - `PKHeX.Web/README.md` becomes a self-hosting guide (root + `/PKHeX/`, nginx/Apache snippets, no `file://`).
  - Optional `workflow_dispatch` deploy job behind a protected environment, promoting the CI artifact. It is not enabled for upstream (HOST-002/003).

  **M20 status:** code complete on `web/m20-hosting-release`.
  - **User decisions:**
    - No deploy job in M20. Which host serves the official site, and whether the job lives upstream, is a maintainer question, now under G-A.
    - The license and notices get their own CSP.
    - A static `404.html` is added.
    - The official site is hosted at the root, and the subpath is a README note rather than a script.
  - **`wwwroot/_headers`** (Cloudflare Pages format, which Cloudflare's docs and its asset-server source (`workers-sdk` `pages-shared/asset-server/handler.ts`) define):
    - Every response gets the app CSP (the meta policy plus `frame-ancestors 'none'`), `nosniff`, `no-referrer` and `Cache-Control: no-cache`.
    - Fingerprinted files are `public, max-age=31536000, immutable`: `_framework/*.wasm`, `*.dat`, `dotnet.*.js`, and the sprite `pokemon.*.png` and `sprites.*.css`. The rules name only fingerprinted shapes, so a future unfingerprinted loader cannot be cached for a year by accident. The loaders `blazor.webassembly.js` and `dotnet.js` (the boot manifest) revalidate.
    - `LICENSE.txt`, `THIRD-PARTY-NOTICES.md` and `licenses/*` get `default-src 'none'; img-src 'self'; style-src 'unsafe-inline'; frame-ancestors 'none'`.
    - Each override detaches the earlier value (`! Name`) before setting its own. Cloudflare applies matching rules in file order, deleting detached headers before setting, and joins a header set twice with a comma.
    - The meta CSP fallback in `index.html` is unchanged.
  - **`wwwroot/404.html`:** no script, style, link or base, so it is the same at any depth and under a subpath. Without it, Cloudflare Pages treats the site as a single-page app and answers every missing path with `index.html` and status 200.
  - **Test host** (`Tests/PKHeX.Web.Tests`):
    - `HostHeaders` parses the subset of `_headers` the file uses and applies it as Cloudflare does. It refuses placeholders, absolute URLs, two splats, a repeated pattern, an over-long line and over 100 rules.
    - `StaticHost` loads the published `_headers` at start (and fails without it), sends what it gives each path, never serves `_headers`, and answers a missing file with `404.html` and status 404. `Cache-Control` is still sent only in deployment-caching mode, plus Cloudflare's `no-store` on a 404, so the browser tests behave as before.
    - F8's name heuristic and the hard-coded constants are gone. The expected values live in `ExpectedHeaders`, written independently of the file, and the boot checks compare against them.
  - **Hosting snippets** (`PKHeX.Web/hosting/`, outside `wwwroot`, never published):
    - `nginx.conf`: two `map $uri` blocks for the cache rule and the policy, `include mime.types` plus markdown, `gzip_static`, `_headers` refused, and `error_page 404`.
    - `apache.conf` (`.htaccess` or `<Directory>`): `AddType`/`AddEncoding` so a `.br`/`.gz` copy keeps its asset's type, rewrites to the copy the browser accepts, the same headers, and `<If>` overrides.
    - Their patterns match the end of the path, so they also work under a subpath.
  - **README:** a Hosting section now opens it: build, trying it locally (`file://` cannot work), what to deploy (one publish as a whole, atomic switch, keep the previous release to roll back), a headers table with the reasons, Cloudflare Pages / nginx / Apache / GitHub Pages notes (no headers there; `.nojekyll` on branch publishing; a project site needs the subpath), the subpath note (edit the base href and delete `index.html.br`/`.gz`), the 404 page, and `curl` checks for a deployment. The test-host, boot-baseline and artifact-check paragraphs now describe the shipped `_headers`.
  - **Found while testing:**
    - **Firefox requests `/favicon.ico` for a text document**, which `default-src 'none'` blocked (`img-src` violations on `LICENSE.txt`, found in the nginx probe). The text policy adds `img-src 'self'`; it still allows no script.
    - **An nginx `types` block in `server` replaced the whole MIME table:** HTML, JavaScript and WebAssembly were all served as `application/octet-stream`. The snippet includes `mime.types` first; two `types` blocks merge.
    - **An Apache `SetEnvIf … no-gzip` line was unnecessary.** With `mod_deflate` loaded, a precompressed response was not compressed again with or without it, because deflate leaves a response that already has a content coding alone. The line was removed.
  - **Tests.**
    - **Unit 1296** (up from 1215; 1293 before the adversarial review):
      - `HostHeadersTests`: order, detach before set, joining, splats, refused syntax and the line limit, a missing file, and the shipped file's cache rule and headers for every shape of path.
      - `HostingSnippetsTests`: nginx and Apache give every shape of path, at `/` and `/PKHeX/` (Apache also as the `.br`/`.gz` it rewrites to), the same cache rule, policy and headers as `ExpectedHeaders`, and both read as 4 overrides.
      - `NotFoundPageTests`: nothing runs or loads.
      - `ContentSecurityPolicyTests`: the shipped app and text policies, and the text policy's exact directives.
      - The F8 fingerprint test moved into `HostHeadersTests`.
    - **E2E 367** (up from 356), every test executed (`trx-all-executed.sh`), with the default and sprite publishes. The full run had one failure: `ResponsiveBrowserTests.PanesFollowTheWidthWithoutLosingTheDraft` (Chromium, root path), whose resize step did not see focus on `#draft-title`. That is the intermittent step M18b and M18c record. It passed 5 of 5 alone, and M20 does not touch that path.
      - `DeploymentHeadersTests`: every file of both publishes gets the expected headers from `_headers` and from both snippets, and more than 50 are fingerprinted. A missing `.wasm`, a missing page and `_headers` are each `404.html` with status 404 and the app policy, at both paths. `LICENSE.txt` and an upstream notice open in each engine's own viewer with the text policy and no CSP violation (3 engines × 2 paths).
      - `StaticHostServingTests`: the rules' headers in both modes, the 404 page with `no-store`, and a host refusing to start without `_headers`.
      - `PublishesOnlyStaticDeployableFiles`: allows exactly `_headers` and `404.html` at the root (and only `index.html` and `404.html` as HTML), and requires both to ship byte for byte.
    - **RealSave 112** pass (the host change reaches every browser tier).
    - **Mutation checks** (files restored from a scratchpad copy; failing tests):

      | Mutation | Failing tests |
      | --- | --- |
      | No immutable rule for `_framework/*.wasm` | 2 |
      | `dotnet.*.js` widened to `*.js` (the loader cached for good) | 2 |
      | `nosniff` dropped | 16 |
      | Text policy rules removed | 3 |
      | A script in `404.html` | 1 |
      | nginx CSP drifts (`form-action 'self'`) | 16 |
      | nginx `dotnet` pattern also matches `dotnet.js` | 1 |
      | Apache text-policy block removed | 4 |
      | License under the app policy (E2E; header assertion skipped to reach the recorder) | 2 (WebKit, `style-src-attr`) |
      | Text policy without `img-src` (E2E, same) | 2 (Firefox, `img-src`) |
      | The host serves `_headers` (E2E, Chromium) | 3 |
      | A publish without `_headers` (E2E, Chromium: the host refuses to start) | 3 |
    - **Other checks:** trim baseline unchanged (38); `PKHeX.slnx` Release has 0 warnings; no raw Bidi_Control characters in the changed files.
  - **Adversarial review** (each finding shown by a failing test, probe or surviving mutation first, then fixed):
    - **Fixed:**
      - **The snippets left the runtime's ICU data without a binary type.** Apache 2.4 sent no `Content-Type` for `_framework/*.dat`. nginx used the `http` block's `default_type`, which is `text/plain` when the block sets none (probed by removing it from the image's `nginx.conf`). The README requires a binary type. `apache.conf` now has `AddType application/octet-stream .dat` and `nginx.conf` `default_type application/octet-stream`, and both servers then sent it. `HostingSnippetsTests.BothSnippetsSendABinaryTypeForTheRuntimeData` failed before the fix.
      - **The model's anchoring was untested.** With the leading `^` dropped from `HostHeaders`' pattern, every test passed. Two cases now check that `/LICENSE.txt` and `/licenses/*` do not match under another folder; that mutation fails 2.
      - **The README required `text/javascript`,** but nginx's standard table sends `application/javascript`, which browsers accept. The README now names either.
      - **The README's subpath note did not cover Cloudflare.** Pages reads only the `_headers` at the top of a deployment, so under `/PKHeX/` the shipped file would be ignored. The note now says to move it up and prefix its patterns.
    - **Probed, holds:**
      - Cloudflare's parser (`workers-sdk` `workers-shared/utils/configuration/parseHeaders.ts`) trims lines, takes `! ` as a detach, allows one splat and limits files to 100 rules and 2,000-character lines. That is the model for every construct `_headers` uses. Where Cloudflare silently drops an invalid line, the model refuses it.
      - Windows checkouts may turn the files to CRLF: with `_headers`, `404.html`, `index.html` and both snippets in CRLF, the Unit tier still passes (the new type check was first written with a `$` that missed CR, and was corrected).
      - The `Perf` tier, which uses the host's caching mode, passes on the new host: warm boots still only revalidate (9 requests, all 304). A favicon 404 now carries the 404 page's body (about 470 bytes, shown as `identity` in Firefox's cold row), as it would on a real host.
    - **Checked, not changed:**
      - Cloudflare puts `no-transform` in `Cache-Control` on responses it serves precompressed. Our `/*` rule replaces that header, as any `_headers` value does. It only asks intermediaries not to re-encode, so it was left out.
      - A request for a directory such as `/licenses/` falls under the text policy and gets the 404 page, which has nothing that policy blocks.
    - Mutations (failing tests): unanchored patterns 2; either snippet's binary type removed 1.
    - After the fixes: Unit 1296; the deployment E2E tests (13) pass; the fixed snippets were run again on nginx and Apache with the three-engine probe; `PKHeX.slnx` Release has 0 warnings.
  - **Manual hosting checks** (scratchpad copies of the publish; nothing in the repository):
    - nginx 1.30.5 (Docker `nginx:stable`) with `hosting/nginx.conf`, and Apache 2.4.66 (macOS `/usr/sbin/httpd`, the snippet as `.htaccess`), each at `/` and, on a copy with the base href edited and the compressed index removed, at `/PKHeX/`.
    - `curl` with and without `Accept-Encoding` checked `index.html`, a fingerprinted `.wasm`, `dotnet.js`, `app.css`, `LICENSE.txt`, the notices, an upstream notice, `_headers` and a missing file. Each got its type, encoding (`.br`/`.gz` with the asset's own type), cache rule and policy, and the 404 page.
    - A throwaway Playwright probe booted each of the four in Chromium, Firefox and WebKit until the file input was enabled, then opened `LICENSE.txt`, with no CSP violation, console error or failed response.
    - Under `/PKHeX/` on nginx, leaving the stale `index.html.gz` in place served `<base href="/">`, which confirms the README note.
    - The README's `curl` commands were run against the nginx container.
  - **Not verified yet:**
    - No Cloudflare Pages deployment: the `_headers` semantics come from Cloudflare's docs and asset-server source and are reproduced by the test host, but no response from Cloudflare itself was seen. This waits for the G-A hosting answer.
    - GitHub Pages and other hosts are documentation only.
    - `brotli_static` was not run (the stock nginx image has no `ngx_brotli`).
    - Physical devices (G-C).
  - **Compared with PKForge** (`.github/workflows/build.yml`, `README`):
    - **Different:** PKForge builds, signs and publishes an APK release from one job with `contents: write` and its secrets. We ship no deploy job yet, and the one G-A will decide on is specified to promote an already-tested CI artifact from a protected environment rather than rebuild.
    - **Out of scope:** PKForge has no web hosting, headers or self-hosting guide to compare. Its Android signing and store release are out of Web scope.
- **M21 Qualification + support report.** Memory/peak measurements on the largest admitted fixture and a repeated-session trend (PERF-002). Name the reference desktop (CPU model, memory, OS, browser), run the F8 `Perf` tier on it, and record the WEB-PERF-001 verdict against the `PKHeX.Web.md` startup targets: met, or a documented, scoped limitation. Physical-device runs come from G-C (BROWSER-001/002). Update `PKHeX.Web.md` status/compatibility matrix (XY/ORAS → **P** only where proven) and publish a support report modelled on `PKHeX.Web.WasmProof.md`.

  **M21 status:** code complete on `web/m21-qualification`. The support report is `PKHeX.Web.SupportReport.md`.
  - **User decisions:**
    - The reference desktop is this Mac (Apple M4, 10 logical CPUs, 16 GiB, macOS 26.5) with the installed Google Chrome 154.0.8037.93, driven headless by Playwright through a channel. The Playwright builds are measured alongside.
    - G-C is reduced scope. The claim is desktop only: Safari (macOS), iPadOS and Android are listed as not qualified, and the report carries the device checklist.
    - Refusing a file at the 16 MiB limit cost a lot of memory (below). The read path is fixed in this chunk rather than documented or capped.
  - **Perf tier, new and changed:**
    - `PerfBrowser`: one launch helper for every Perf harness. `PKHEX_WEB_PERF_CHANNEL` (validated against Playwright's Chromium channels) runs the Chromium rows on an installed release browser; CI leaves it unset. Reports name the browser by its channel (`chrome`).
    - `MemoryBaseline` (WEB-PERF-002), on `SaveFixtures.Full(true)`, a full ORAS save (every box slot and party position, 936 entities). It reads the .NET runtime's WebAssembly memory through the public `getDotnetRuntime(0).localHeapViewU8()`, in all three engines. That memory never shrinks, so each reading is the peak so far. Chromium also gives its JS heap after a forced collection (CDP).
      - Steps: shell, open, edit and apply, download and close.
      - Repeated sessions in one page: 40 by default, `PKHEX_WEB_PERF_SESSIONS` to change it.
      - Then two refusals at the read limit.
      - Native accounting of the same Web code (bytes allocated per step, as copies of the input).
      - The atlas's decoded size from its PNG header.
    - `BoxNavigationTiming` (WEB-BOX-008): every box of the full save, Next and back after an unrecorded lap, timed in the page from the click to the render of the new box heading. It checks that the heading names the expected box and that no request reaches the host.
    - `LegalityTiming` takes its saves, slots and engines as parameters. `RealSaveLegalityTimingTests` (RealSave, in the non-parallel collection) times every slot the private saves let the user open, in Chromium only (or the channel browser): about six minutes, against twenty for all three engines. Its report gives positions and verdicts only, goes to the test output, and goes to `PKHEX_WEB_PERF_REPORT` when set.
    - CI appends the memory and box reports to the run summary. The tier now takes about six minutes, on pushes only.
  - **App change:** `BrowserFileService.ReadBoundedAsync` reads straight into one array sized from the declared length (clamped to the limit) and hands it over. It grows on a stream that sends more, trims on one that sends less, and probes one byte at the limit. The old `MemoryStream` + `ToArray` allocated 56 MiB for a 16 MiB file (3.5×); now 16 MiB. In the browser, the peak after refusing a 16 MiB file fell from about 228 to about 190 MiB (measured with the refusals before the repeated sessions).
  - **Results on the reference desktop** (details in the support report):
    - **PERF-001:** Chrome cold 2947 ms and warm 622 ms on 20 Mbps / 50 ms, so both targets are met. 62 files and 5.49 MiB Brotli per cold boot.
    - **BOX-008:** p95 4–7 ms, max 11 ms, against 100 ms.
    - **PERF-004:**
      - Synthetic corpus: warm p95 37–45 ms.
      - Private saves (968 slots): warm p95 17–20 ms, max 73 ms.
      - First analysis per page: 267–355 ms.
      - Every verdict equals native Core's.
    - **PERF-002:**
      - 106–111 MiB after the first full session; settled at 153–161 MiB over repeated sessions.
      - 100 sessions: one ~26 MiB step by session 15, then flat.
      - The Chrome JS heap creeps about 15 KiB per session.
      - A refused 16 MiB file raises the peak to 222–272 MiB (scoped limitation; the rest is Blazor's stream transfer and the loader's two required copies).
      - The atlas is about 27 MiB decoded.
  - **Docs:**
    - `PKHeX.Web.md`: the status line, the risk row, and the XY/ORAS row is now **P** on desktop only, replacing "No row has P today".
    - `PKHeX.Web/README.md`: the performance section, with the new variables and the gated real-save timing.
    - The support report has the MVP exit audit mapping every Must/MVP story to its chunk and tests.
  - **Found while measuring:**
    - **The refusals hid the trend.** Run first, the 16 MiB refusal's high-water mark covered the repeated sessions, so they looked flat whatever happened. With the refusals moved last, the sessions grew. The refusals now always come last.
    - **The first trend rules were wrong.** A share of the second half, and then "more than one step", each flagged runs that were in fact one or two heap steps followed by a plateau. A 100-session probe showed every engine settling at the same level. The rule now flags growth in the last quarter of the run, and the report says what it cannot rule out (retention below one ~26 MiB step).
    - The first native accounting threw `ExportNotAcknowledged`: the edited corpus entity is Invalid, so the export needs the acknowledgement, as in the page.
  - **Tests:**
    - **Unit 1338** (up from 1296): `PerfReportTests` (channel parsing, growth sessions and the flag, memory, box and legality Markdown, JSON round trip, native accounting of the full save, the atlas header, the full fixture, session parsing) and `BrowserFileServiceTests` (declared length right, low, over the limit or absent; trimmed; one byte over; one allocation).
    - **E2E 367** and **RealSave 113** (112 before plus the real-save timing, about six minutes on Chrome: warm p95 20 ms) pass on this commit, every test executed.
    - **Perf 4** on Chrome and on the Playwright builds; the real-save timing on Chrome.
  - **Mutation checks** (files restored from a scratchpad copy; failing tests):

    | Mutation | Failing tests |
    | --- | --- |
    | The read copies the buffer again (`ToArray`) | 1 |
    | No one-byte probe at the limit | 2 |
    | Growth in the last quarter never flagged | 3 |
    | Equal readings counted as growth | 8 |
    | Channel not validated | 3 |
    | Channel applied to every engine | 1 |
    | A native step not recorded | 1 |
    | Atlas height read from the width | 1 |
    | The full fixture without its party | 1 |
    | Box timing stops at the click, not the render (Perf, Chromium) | 1 |

  - **Other checks:** trim baseline unchanged (38); `PKHeX.slnx` Release has 0 warnings; the workflow is actionlint-clean; no raw Bidi_Control characters in the changed files.
  - **First review** (inline, while measuring): the refusals masking the trend, and the two trend rules, above. The memory report's units: small steps showed as "0.0 MiB" (now KiB), and copies were counted against the save even for the 16 MiB file (now against each step's input).
  - **Adversarial review** (every "Met" in the support report traced to a number in a report file of this commit's runs, and the tooling the change touches re-run):
    - **Fixed:**
      - **The Windows parity job would have failed.** The real-save timing had been gated on `RealSave` and `Perf` together, with a two-tier `TierFactAttribute`. Its skip reason ("Opt-in tiers; set …=RealSave,Perf …") does not match `trx-check-opt-in-skips.ps1`'s pattern, which azure-parity applies to every unopted run, so the job would fail on its first push. A RealSave-only run also always held one skipped test, which `trx-all-executed.sh` rejects. Shown by applying the script's pattern to a TRX of an unopted run (no `pwsh` here). The two-tier attribute is gone. The timing is a plain RealSave test in one engine (see above), and its skip now matches.
      - **The report's memory ranges came from one run.** Across the four runs the first session is 106–111 MiB, the settled level 153–161 MiB and the refused 16 MiB file 222–272 MiB. The report had 109–111, 156–160 and 225–272, and an earlier note here claimed every step varied by under 10 MiB, which the refusal does not.
      - **The support claim named Chrome,** but only the Perf tier ran on Chrome 154. The E2E and RealSave tiers ran on Playwright's Chromium 153. The claim now names the Chromium engine, Firefox and the WebKit engine, and says what ran where (`PKHeX.Web.md` too).
      - **Box timing was on the default publish only.** The report now says the unshipped sprite publish was not timed.
    - **Checked, not changed:**
      - The read now allocates the declared length up front, up to the limit, before the bytes arrive. A local `File`'s size is the browser's own and is checked against the limit first, so this costs no more than a real file of that size.
      - Each memory configuration is one run, not a median. Across four runs (Chrome and the Playwright builds, 40 and 100 sessions), each session step varied by up to 10 MiB and the settled level stayed at 153–161 MiB. The refusal's peak varied far more (222–272 MiB), with how much of the heap earlier sessions left free, so the report gives it as a range.
      - The box timing ends at the render that changes the heading. Blazor applies one render's changes together, so the grid is in place by then, but the paint after it is not included.
  - **Not verified yet:**
    - Physical devices (G-C): no memory, file-provider or eviction run on an iPad, Android or Safari.
    - No screen reader.
    - Edge is not claimed.
    - No Cloudflare deployment.
    - The first GitHub run of the extended Perf step.
  - **Compared with PKForge:**
    - **Different:** PKForge's README claims every mainline generation, the side games and romhacks, with no per-family qualification record, and it has no memory or startup measurement. We claim raw XY/ORAS on named desktop browsers only, each verdict tied to a measured report, and list what is not qualified.
    - **Out of scope:** Android device and emulator support.

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

# PKHeX.Web — delivery plan

## Context

`PKHeX.Web.md` sets out a static Blazor WebAssembly editor that runs alongside WinForms on top of an unmodified `PKHeX.Core`. `PKHeX.Web.WasmProof.md` records **Goal 1 as done**. A trimmed Release publish opens real XY/ORAS saves in Chromium, Firefox and WebKit, at `/` and `/PKHeX/`. It runs legality analysis, edits one boxed PK6 nickname through `EntityImportSettings.None`, and downloads output byte-identical to native Core. It made no Core changes. That proof is still untracked: `PKHeX.Web/` (`ProofSession.cs`, `App.razor`, `browser.js`) and `Tests/PKHeX.Web.Tests/` (Playwright + `StaticHost`).

Remaining goals:
2. **Narrowly justified shared refactors.** These are upstream PRs to `kwsch/PKHeX`. Each needs its own evidence and desktop regression tests.
3. **Web foundation + CI.** Promote the proof into a maintainable project skeleton, integrate it with the solutions, and add a Linux publish/test/E2E workflow plus a Windows regression job.
4. **XY/ORAS MVP.** The full journey from the "Definition of MVP" section of `PKHeX.Web.md`: open → summary → party/boxes → draft editor → legality → atomic apply → validated export. It has to be accessible, private and static-hosted.

Each numbered chunk below is **one commit**. Every commit must build, and its tests must pass.

## Branching

- Remotes: `origin` = `jcreek/PKHeX` (fork), `upstream` = `kwsch/PKHeX`. The upstream default branch is `master`.
- `web/main` (fork only) holds the design docs, `plan.md` and all Web work. Topic branches for Web PRs are cut from it.
- Each upstream refactor goes on its own branch cut from `upstream/master`, e.g. `web/refactor-species-form`. It contains **no** Web files or docs. After merge (or while pending), rebase `web/main` onto it.
- Rebase onto `upstream/master` before each PR and rerun the affected tests.

## Human-only gates (not commits)

- **G-A Maintainer interest:** before the Web foundation PR goes upstream, ask through the CONTRIBUTING channels. Present the static/client-only goal, the proof results, asset/crypto implications and maintenance ownership. Refactor PRs are useful on their own and do not wait on this.
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

**F2 Solution integration.** Add `PKHeX.Web` and `PKHeX.Web.Tests` to `PKHeX.slnx` and `PKHeX.sln`. Confirm they inherit from `Directory.Build.props` (C# 14, nullable) and fix any new nullable warnings. Pin `Microsoft.AspNetCore.Components.WebAssembly` to 10.0.12. Do not add a repo-wide `global.json`.

**F3 Split test tiers.**
- `Category=Unit`: session and naming tests with synthetic Core blank saves plus a test-only BEEF footer, as the proof does.
- `Category=E2E`: Playwright against the published output with synthetic fixtures.
- `Category=RealSave`: env-var driven. When this category is selected and a variable is missing, the test fails, not skips.
- CI runs only Unit and E2E.
- Move `StaticHost` + privacy assertions into a reusable `PublishedAppFixture`. It covers request interception, storage emptiness, root and `/PKHeX/`, and deployment-like headers (MIME, CSP header, `nosniff`).

**F4 Browser file interop.** Add `Interop/BrowserFileService` + `wwwroot/browser.js`:
- single-file picker with the 16 MiB limit enforced while streaming
- file-only drop that rejects multiple files, directories and URL/text drops, and prevents navigation
- Blob download with delayed object-URL revoke

Add `Services/FileNaming`, which sanitises separators and control characters, caps length, and preserves the extensionless `main` (WEB-SESSION-007). Unit tests cover the naming rules. E2E covers picker and drop.

**F5 Build provenance.** MSBuild embeds the Web version + Core git commit as assembly metadata. `THIRD-PARTY-NOTICES.md` for Web lists runtime packages and licenses (WEB-SEC-004). CI later prints a `dotnet list package --include-transitive` inventory.

**F6 CI workflow.** Add `.github/workflows/web.yml`:
- runs on `ubuntu-latest`, triggered by `pull_request` + `push`, with `permissions: contents: read` and actions pinned to commit SHAs
- setup-dotnet 10.0.401 → restore → Core tests → Web Unit tests → `dotnet publish -c Release` → diagnostic publish with `SuppressTrimAnalysisWarnings=false`, diffed against a checked-in `PKHeX.Web/trim-warnings.baseline.txt` (fails on new warnings)
- static artifact checks: no `.map`/source/fixtures, file count and max asset size under the Cloudflare limits (20k files, 25 MiB)
- size report (raw/br/gz of the largest assets); Playwright browser install (cached) → E2E
- uploads the publish output, test results and size/warning/license reports
- no secrets and no `pull_request_target`

**F7 Windows regression job.** Add a `windows-latest` job (same workflow or `desktop.yml`) that builds `PKHeX.sln` Release and runs `PKHeX.Core.Tests`. This guards Phase 1 and the solution edits.

**F8 Performance baseline (WEB-PERF-001).** A script (`PKHeX.Web/tools/measure.*` or a test) records cold and warm boot under Playwright network throttling (20 Mbps/50 ms) plus artifact sizes, and uploads them as a CI artifact. There is no pass/fail threshold yet.

→ Gate G-A, then open the upstream foundation PR (F1–F8). It must not claim support for anything.

## Phase 3 — XY/ORAS MVP (Goal 4)

Topic branches from `web/foundation`, in the order `PKHeX.Web.md` §"Proposed contribution sequence" gives. The no-op round trip lands first (M1–M6), then the editor (M7–M16), then polish and qualification (M17–M21). Every chunk adds or extends Unit and/or E2E tests.

### Slice A — load, browse, no-op export
- **M1 Shell.** Start screen with privacy copy ("processed entirely on this device…"), supported formats, temporary-state warning, and Open + keyboard-accessible drop zone. About panel shows version/Core commit (F5), licenses and the support matrix. Top-level `ErrorBoundary` with safe reset (WEB-APP-001–004, ERR-003).
- **M2 Load pipeline + error taxonomy.** Typed outcomes: `Empty`, `TooLarge`, `ReadFailed`, `Unrecognized`, `RecognizedNotEnabled(family)`, `IntegrityFailed`, `ParserFault`. The replacement candidate is parsed before the old session is touched, and failure keeps the session (SAVE-001–004, ERR-001/002/004). Extends the proof's failure-path tests.
- **M3 Overview.** Trainer name, game, language, TID/SID in the save's display format (`TrainerIDFormat`), playtime, money, sanitised filename and size, integrity status. Unknown values are labelled, never invented (OVERVIEW-001/002, SAVE-007).
- **M4 Party + box grid.** Party strip plus the current box only. Box selector and prev/next use Core `BoxCount`/`BoxSlotCount` and box names. The grid is a single tab stop with arrow keys and Enter, plus a list alternative. Slots are labelled with coordinates and species text. Empty slots never open a stale entity (PARTY-001, BOX-001/008, A11Y-001).
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
- `dotnet publish PKHeX.Web/PKHeX.Web.csproj -c Release -o $OUT`, then `--filter Category=E2E` with `PKHEX_WEB_PUBLISHED=$OUT/wwwroot`.
- Before Phase 3 PRs and after M9/M15/M19: `--filter Category=RealSave` with the private XY/ORAS env vars (proof README procedure).
- Trim-warning baseline unchanged, or the diff explained in the commit.
- Phase 1 also needs a Windows WinForms build (F7 job or local) and the R3 Windows checklist (gate G-E).
- Manually drive the app with `python3 -m http.server` on the published `wwwroot` for UI chunks.

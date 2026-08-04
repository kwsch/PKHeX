---
name: sync-upstream-core
description: Use when a "PKHeX.Core Sync Required" issue is open (the daily auto-generated sync-labeled issue) and PKHeX.Core must be brought 1:1 with upstream kwsch/PKHeX, propagated to the Avalonia layer, version-bumped, and shipped as a merged PR. Keywords - PKHeX.Core sync, upstream sync, kwsch/PKHeX, last-synced-sha, UIVersion bump.
---

# Sync Upstream PKHeX.Core

## Overview

Brings the vendored `PKHeX.Core/` to **byte-for-byte parity** with upstream
[kwsch/PKHeX](https://github.com/kwsch/PKHeX) at the SHA named in a `sync` issue,
fixes any Avalonia-layer call sites the upstream change touched, **reviews upstream's
non-Core (WinForms UI / sprite) changes for Avalonia frontend-parity gaps**, bumps the
version, opens a PR, and **auto-merges once CI is green**.

**Core principle:** `PKHeX.Core` is a 1:1 mirror of upstream. We never edit it to make
things compile — upstream API changes are absorbed by editing **consumers only**.

**Second principle:** a clean build proves nothing *broke* — it does **not** prove the
Avalonia frontend gained the user-facing changes upstream made to its (WinForms) UI.
Upstream is a WinForms app; this fork reimplements that UI in Avalonia, so a new upstream
editor/field/sprite compiles fine here while being entirely absent from the app. Every sync
therefore includes a **Frontend Parity Review** (step 5) that classifies upstream's non-Core
changes and opens `frontend-parity` tracking issues for genuine gaps — without ever blocking
the Core auto-merge.

## When to Use

- An open issue labeled `sync` titled `PKHeX.Core Sync Required ...` (auto-generated daily at 08:00 UTC).
- User says "sync issue 87", "run the upstream sync", "adopt the latest PKHeX.Core".

**Not for:** feature issues, bug reports, or anything without the `sync` label. This skill assumes the sync-issue shape.

## Invocation

Pass the issue number, or find it:

```bash
N="${1:-$(gh issue list --label sync --state open --json number --jq '.[0].number')}"
```

## Hard Rules (read before every run)

1. **Never edit `PKHeX.Core/`** except by mirroring upstream. A build error is fixed in `PKHeX.Application` / `PKHeX.Presentation` / `PKHeX.Infrastructure` / `PKHeX.Avalonia` / `Tests` — never in Core. If a "fix" seems to need a Core edit, you've misdiagnosed.
2. **No direct pushes to `master`.** Everything goes through the sync branch + PR.
3. **Auto-merge is gated.** Merge only when ALL pass: `diff`=0, `dotnet build` clean, `dotnet test` green, **and CI green**. Any failure → stop, report, leave PR open.
4. **Preserve fork-only fields** in `Directory.Build.props`: `Company/Authors/Copyright = Patrik Lleshaj` and `<UIVersion>`. Only the `<Version>` *value* mirrors upstream.
5. **Use the full 40-char SHA** in `last-synced-sha.txt` (the daily checker compares it exactly).
6. **Build-green ≠ feature-parity.** A passing build only means no consumer *broke*. Upstream UI/feature changes the Avalonia frontend should adopt are surfaced by the **Frontend Parity Review** (step 5) and tracked as separate `frontend-parity` issues — they never block the Core sync's auto-merge.

## Workflow

Track these as a TodoWrite checklist and stop at the first failed gate. (The Frontend Parity
Review in step 5 is **not** a gate — it records findings and opens follow-up issues, never blocks.)

### 1 — Preflight & parse

```bash
git switch master && git pull --ff-only
git status --porcelain            # MUST be empty, else stop
gh issue view "$N" --json title,labels,body   # MUST have the `sync` label, else stop
# Authoritative latest SHA (don't trust body parsing):
LATEST_SHA=$(gh api 'repos/kwsch/PKHeX/commits?path=PKHeX.Core&per_page=1' --jq '.[0].sha')
SHORT="${LATEST_SHA:0:7}"
```

Read the issue body for the human-readable **commit list** and the **changed-files** summary — you'll reuse them in the PR.

### 2 — Branch

```bash
git switch -c "chore/sync-pkhex-core-$SHORT"
```

### 3 — Mirror Core 1:1 and verify

```bash
rm -rf /tmp/pkhex-upstream && mkdir /tmp/pkhex-upstream && cd /tmp/pkhex-upstream
git init -q && git remote add origin https://github.com/kwsch/PKHeX.git
git sparse-checkout init --cone && git sparse-checkout set PKHeX.Core
git fetch -q --depth 1 origin "$LATEST_SHA" && git checkout -q FETCH_HEAD
cd - >/dev/null

rsync -a --delete --exclude bin --exclude obj /tmp/pkhex-upstream/PKHeX.Core/ PKHeX.Core/
diff -rq -x bin -x obj PKHeX.Core /tmp/pkhex-upstream/PKHeX.Core   # MUST print nothing
```

`diff` non-empty → the mirror failed; fix before continuing. (`rsync --delete` also removes
files upstream deleted and adds new ones — note any in the PR.)

### 4 — Build & propagate to the Avalonia layer

```bash
dotnet build PKHeX.sln -c Release
```

If it fails, the upstream commits renamed/changed a public API (e.g. `StatNature` → `StatAlignment`).
Find and fix the call sites **outside Core**:

```bash
grep -rn "OldSymbol" PKHeX.Application PKHeX.Presentation PKHeX.Infrastructure PKHeX.Avalonia Tests
```

Re-build until **0 warnings, 0 errors**. Record what changed (or "None") for the PR's
"Avalonia-layer changes (build)" section. This step only covers *build breakage* — whether the
frontend should **gain** new user-facing behavior is the separate Frontend Parity Review (step 5).

### 5 — Frontend Parity Review (not a gate)

The clean build in step 4 only proves nothing *broke*. It does **not** prove the Avalonia frontend
gained the user-facing changes upstream made to its **WinForms UI**. Review the non-Core changes in
this sync's range and classify them — this is the step that catches "upstream added an editor / field /
sprite set we now need in Avalonia too."

```bash
OLD_SHA=$(cat .github/upstream-sync/last-synced-sha.txt)   # still the PREVIOUS sha at this point (step 6 overwrites it)
# Per-commit enumeration of non-Core touches. NOTE: the compare API's .files list is capped at 300
# (and commits at 250) — unreliable for big ranges; iterate per commit, whose file list is complete:
for sha in $(gh api "repos/kwsch/PKHeX/compare/$OLD_SHA...$LATEST_SHA" --jq '.commits[].sha'); do
  n=$(gh api "repos/kwsch/PKHeX/commits/$sha" --jq '[.files[].filename|select(startswith("PKHeX.Core/")|not)]|length')
  [ "$n" -gt 0 ] && echo "$sha  ${n} non-Core  $(gh api repos/kwsch/PKHeX/commits/$sha --jq '.commit.message|split("\n")[0]')"
done
```

Look at `PKHeX.WinForms/**` (UI) and `PKHeX.Drawing.*` (sprite/asset additions). New **public** Core
APIs that surface user-facing data also count (they usually imply a new field/control). The strongest
gap signal is a **new UI file added upstream** — fetch it with a history-bearing clone:

```bash
# A blobless clone gives full history + path filtering without downloading every blob:
git clone --filter=blob:none --no-checkout https://github.com/kwsch/PKHeX.git /tmp/pkhex-hist 2>/dev/null
git -C /tmp/pkhex-hist log --diff-filter=A --name-only --format='' "$OLD_SHA..$LATEST_SHA" \
    -- 'PKHeX.WinForms/**/*.cs' | grep -v '\.Designer\.cs$' | sort -u   # new forms/controls/editors
```

Classify each non-Core-touching commit, then cross-check `GAP` candidates against the existing
Avalonia surface (`PKHeX.Avalonia/Views/*.axaml`, `PKHeX.Presentation/ViewModels/*`, `ViewLocator.cs`,
`MainWindowViewModel.*.cs`):

| Verdict | Meaning | Action |
|---|---|---|
| Core-only | no non-Core files | none |
| WinForms-internal | Designer regen, refactor, layout reflow, localization-only, bulk asset re-encode | none |
| different-impl | a WinForms widget Avalonia builds differently (e.g. GDI `ExperienceBar` ↔ `NumericUpDown`) | note, none |
| already-covered | Avalonia already has the equivalent | note where |
| **GAP** | upstream added/changed user-facing behavior Avalonia lacks | track (below) |

For each **GAP**: do **not** block the sync. Keep the sync PR pure (Core + version). Open a tracking
issue labeled `frontend-parity` (create the label once with `gh label create frontend-parity`):

```bash
gh issue create --label frontend-parity \
  --title "frontend-parity: <feature> (<upstream short sha>)" \
  --body "Upstream <sha> added <what>. Core API: <type/SaveBlock>. Avalonia: suggest <XView>+<XViewModel>, wire in MainWindowViewModel.EditorDialogs.cs (gated <SAV type>), register in ViewLocator.cs."
```

Record the full classification in the PR body's **Frontend parity review** section (it replaces the
old thin "Avalonia-layer changes: None" line). If the range has zero non-Core commits, say so explicitly.

> Daily syncs are ~1–6 commits, so the per-commit loop is plenty. The blobless clone matters only for
> large backfill windows where the compare-API caps bite.

### 6 — Version housekeeping (`Directory.Build.props` + SHA file)

```bash
printf '%s\n' "$LATEST_SHA" > .github/upstream-sync/last-synced-sha.txt

# UIVersion: +1 patch
CUR=$(grep -oE '<UIVersion>[^<]+' Directory.Build.props | sed 's/<UIVersion>//')
NEW=$(echo "$CUR" | awk -F. '{printf "%d.%d.%d",$1,$2,$3+1}')

# <Version>: mirror upstream's value at this SHA (usually unchanged)
UPVER=$(curl -s "https://raw.githubusercontent.com/kwsch/PKHeX/$LATEST_SHA/Directory.Build.props" \
        | grep -oE '<Version>[^<]+' | sed 's/<Version>//')
```

Then **edit `Directory.Build.props`**: set `<UIVersion>` to `$NEW`; set `<Version>` to `$UPVER`
**only if it differs** from the current value. Leave `Company/Authors/Copyright` untouched.

### 7 — Verify (all three are gates)

```bash
dotnet build PKHeX.sln -c Release    # 0 warnings, 0 errors
dotnet test  PKHeX.sln -c Release    # all pass
diff -rq -x bin -x obj PKHeX.Core /tmp/pkhex-upstream/PKHeX.Core   # nothing => 1:1
```

### 8 — Commit, push, PR

Commit in the established style (one sync commit; add a second `chore: align <Version>` commit
only if `<Version>` changed):

```bash
git add -A
git commit -m "sync: upstream PKHeX.Core to $SHORT and bump to $NEW" -m "<body: ported commits, consumer changes, housekeeping, Closes #$N>"
git push -u origin "chore/sync-pkhex-core-$SHORT"
```

Fill `pr-template.md` (same directory) and open the PR — the body **must** contain `Closes #$N`:

```bash
gh pr create --title "sync: upstream PKHeX.Core to $SHORT + bump $NEW" --body-file /tmp/pr-body.md
PR=$(gh pr view --json number --jq .number)
```

### 9 — Watch CI (non-blocking) → auto-merge

**Do NOT foreground-wait** — the 3-OS matrix takes minutes and will blow the Bash timeout.
Launch the watch as a **background** Bash command (`run_in_background: true`); the harness
re-invokes you when it exits:

```bash
gh pr checks "$PR" --watch --fail-fast    # run_in_background: true
```

On completion:
- **exit 0 (all green)** → merge and clean up:
  ```bash
  gh pr merge "$PR" --merge --delete-branch    # merge commit, matches history; Closes #N auto-closes the issue
  git switch master && git pull --ff-only
  ```
- **non-zero (red/pending-fail)** → fetch the failing job log, **stop, report, leave PR open**:
  ```bash
  gh run view --log-failed -R doctorllll/PKHeX "$(gh run list --branch "chore/sync-pkhex-core-$SHORT" --limit 1 --json databaseId --jq '.[0].databaseId')"
  ```

If `gh pr checks` errors with "no checks reported yet" right after PR creation, relaunch the
same background watch after a short wait — checks take a moment to register.

## Quick Reference

| Thing | Value |
|---|---|
| Upstream | `kwsch/PKHeX`, folder `PKHeX.Core` |
| Branch | `chore/sync-pkhex-core-<short7>` |
| Version file | `Directory.Build.props` → `<UIVersion>` (+1 patch), `<Version>` (mirror upstream) |
| SHA file | `.github/upstream-sync/last-synced-sha.txt` (full 40-char SHA) |
| Build/test | `dotnet build/test PKHeX.sln -c Release` |
| Merge | `gh pr merge <PR> --merge --delete-branch` |

## Common Mistakes

- **Editing `PKHeX.Core` to fix a build error** — fix the consumer call site instead; Core stays 1:1.
- **Foreground-waiting on CI** — blocks for minutes and times out; use a background watch.
- **Merging on red** — auto-merge is gated on green CI only.
- **Squashing** — use `--merge`; the repo uses merge commits.
- **Bumping `<Version>` blindly** — only when upstream's `<Version>` actually changed.
- **Short SHA in `last-synced-sha.txt`** — must be the full 40-char SHA or the daily checker re-fires.
- **Overwriting `Company/Authors/Copyright`** — those are fork-only; mirror just the `<Version>` value.
- **Assuming a clean build means the frontend is current** — it only means nothing *broke*. New upstream WinForms editors/fields/sprites compile fine while being absent from the Avalonia app; run the Frontend Parity Review (step 5) every sync.
- **Blocking the sync on a frontend gap** — the Core sync auto-merges on green CI; parity gaps become separate `frontend-parity` issues, never a merge blocker.

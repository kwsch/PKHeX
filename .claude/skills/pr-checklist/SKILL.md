---
name: pr-checklist
description: Use before opening a pull request in PKHeX-Avalonia, or when asked to check a branch/diff is PR-ready — verifies UIVersion was bumped correctly, no PKHeX.Core edits snuck in, and upstream UI changes have frontend-parity coverage.
---

# PR Checklist

## Overview

Three checks this repo needs on every PR that a clean `dotnet build` won't catch. Run all three before `gh pr create`.

## The 3 Checks

### 1. UIVersion bumped, by change type

```bash
git diff main...HEAD -- Directory.Build.props
```

- Any user-facing change (fix, feature, chore, dep bump) → `<UIVersion>` in `Directory.Build.props` must have moved.
- The bump size follows SemVer **by change type**, not a flat +1:
  - **MAJOR** — breaking changes
  - **MINOR** — new editors/tools/capabilities
  - **PATCH** — fixes, refactors, chores, dep bumps, routine `PKHeX.Core` syncs
- The top-level `<Version>` (date-stamped, e.g. `26.05.05`) tracks upstream `PKHeX.Core` — do NOT bump that one; only `<UIVersion>`.
- No diff to `Directory.Build.props` at all on a user-facing PR = missing bump, fix before opening.

### 2. No PKHeX.Core edits (unless this is a sync PR)

```bash
git diff main...HEAD --stat -- PKHeX.Core/
```

- Expected output: empty.
- `PKHeX.Core` must stay a byte-for-byte mirror of upstream `kwsch/PKHeX`. Any consumer-side fix belongs in `PKHeX.Application`/`PKHeX.Infrastructure`/`PKHeX.Presentation`/`PKHeX.Avalonia` instead.
- Exception: PRs from the `sync-upstream-core` skill on a `chore/sync-pkhex-core-*` branch — those are expected to touch `PKHeX.Core`.

### 3. Frontend-parity coverage (sync PRs only)

Only applies when this PR is an upstream `PKHeX.Core` sync (branch `chore/sync-pkhex-core-*`):

- Confirm the sync's Frontend Parity Review step ran — check the PR body/commit for a note on upstream's non-Core (WinForms UI) changes and whether they need Avalonia equivalents.
- Any genuine gap should already be a tracked `frontend-parity`-labeled issue, not silently dropped.
- See the `sync-upstream-core` skill for the full review process — this check only confirms it happened, it doesn't replace it.

## Quick Reference

| Check | Command | Pass condition |
|---|---|---|
| UIVersion bumped | `git diff main...HEAD -- Directory.Build.props` | `<UIVersion>` changed, by correct SemVer type |
| No Core edits | `git diff main...HEAD --stat -- PKHeX.Core/` | Empty (unless sync PR) |
| Frontend parity | Check PR body / `frontend-parity` issues | Reviewed if this is a sync PR |

## Common Mistakes

| Mistake | Fix |
|---|---|
| Bumping `<Version>` instead of `<UIVersion>` | `<Version>` mirrors upstream's date stamp — never hand-edit it outside a sync |
| Flat +1 patch bump for a new feature | New editors/tools/capabilities are MINOR, not PATCH |
| Editing `PKHeX.Core` to fix a compile error after an upstream change | Port the fix into the consuming layer instead — see the `sync-upstream-core` skill's "Core principle" |
| Opening a sync PR without a Frontend Parity Review note | Re-run step 5 of `sync-upstream-core` before opening |

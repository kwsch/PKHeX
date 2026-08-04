<!--
PR body template for the sync-upstream-core skill.
Fill every <PLACEHOLDER>, drop the "<Version>" line and the second commit if <Version> didn't change,
write the filled result to /tmp/pr-body.md, then: gh pr create --body-file /tmp/pr-body.md
-->
## Summary

Syncs `PKHeX.Core` 1:1 with upstream [kwsch/PKHeX](https://github.com/kwsch/PKHeX) from `<OLD_SHORT>` → [`<NEW_SHORT>`](https://github.com/kwsch/PKHeX/commit/<NEW_SHA>). Resolves #<ISSUE>.

**<N> `PKHeX.Core` files** copied byte-identical from upstream (verified: `diff -rq` against the upstream tree = 0 differences).

### Upstream commits ported
<!-- one bullet per commit, newest first -->
- [`<short>`](https://github.com/kwsch/PKHeX/commit/<sha>) — <message>

### Avalonia-layer changes (build — consumers only, Core stays 1:1)
<!-- "None — no consumer API changed." OR the list below -->
- `<path/to/ConsumerFile.cs>` — <what changed, e.g. `pk.StatNature` → `pk.StatAlignment`>

### Frontend parity review (step 5 — non-Core UI/sprite changes)
<!-- Classify the non-Core commits in this range. "No non-Core commits in range." if empty. -->
- Reviewed `<N>` non-Core commit(s): `<X>` WinForms-internal/cosmetic, `<Y>` different-impl, `<Z>` already-covered.
- **Gaps tracked:** <none, OR `frontend-parity` issue links #__ , #__>

### Housekeeping
- `last-synced-sha.txt` → `<NEW_SHORT>`
- `UIVersion` `<OLD_UI>` → `<NEW_UI>`
- `<Version>` `<OLD_VER>` → `<NEW_VER>`  <!-- drop this line if unchanged -->

## Verification
- ✅ `dotnet build PKHeX.sln -c Release` — 0 warnings, 0 errors
- ✅ `dotnet test PKHeX.sln -c Release` — **<X> passed**, <Y> skipped, 0 failed
- ✅ `PKHeX.Core` byte-identical to upstream `<NEW_SHORT>`

Closes #<ISSUE>

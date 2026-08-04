# Upstream Sync Workflow

The workflows here keep the fork's `PKHeX.Core` mirror aligned with upstream
[PKHeX.Core](https://github.com/kwsch/PKHeX/tree/master/PKHeX.Core) without touching the
Avalonia UI layers.

## How it works

1. `Check PKHeX.Core Upstream Sync` runs daily at 08:00 UTC (or manually) and creates a tracking issue.
2. Compares the last synced commit SHA (stored in `last-synced-sha.txt`) with the latest upstream commits
3. If new commits are found, it creates an issue with:
   - List of new commits with links
   - Link to the full diff
   - Instructions for running the sync action
4. Run `Sync PKHeX.Core` from the Actions tab. It mirrors `PKHeX.Core`, bumps `UIVersion`, runs the
   full build/test suite, pushes a `chore/sync-pkhex-core-*` branch, and opens a pull request.

## After syncing

Once the sync pull request is green and reviewed:

1. Merge the pull request; it updates `last-synced-sha.txt` with the full 40-character upstream SHA.
2. Close the corresponding sync issue if it is still open.

## Files

- `last-synced-sha.txt` - Stores the last synced commit SHA
- `../workflows/check-upstream-sync.yml` - Daily detector and issue creator
- `../workflows/sync-upstream.yml` - Validated mirror + pull request creator

## Manual trigger

Go to Actions > "Sync PKHeX.Core" > Run workflow. The read-only detector is
Actions > "Check PKHeX.Core Upstream Sync".
